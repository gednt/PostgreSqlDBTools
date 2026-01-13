using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DbTools.Model;
using Npgsql;

namespace DbTools
{
    /// <summary>
    /// Helper class to hold WHERE clause parsing results
    /// </summary>
    internal class WhereClauseResult
    {
        public string WhereClause { get; set; }
        public List<NpgsqlParameter> Parameters { get; set; }
    }

    /// <summary>
    /// Generic controller for LINQ-style database manipulation.
    /// Compatible with any model type T, provides Entity Framework-like CRUD operations.
    /// </summary>
    /// <typeparam name="T">The model type to work with</typeparam>
    public class UtilsController<T> : Utils where T : class, new()
    {
        private readonly string _tableName;
        private readonly string _primaryKeyName;
        private readonly bool _autoIncrement;

        /// <summary>
        /// Creates a new UtilsController for the specified model type
        /// </summary>
        /// <param name="host">Database host</param>
        /// <param name="database">Database name</param>
        /// <param name="uid">Database user</param>
        /// <param name="password">Database password</param>
        /// <param name="port">Database port</param>
        /// <param name="tableName">Table name (defaults to type name)</param>
        /// <param name="primaryKeyName">Primary key column name (defaults to "Id")</param>
        /// <param name="autoIncrement">Whether primary key is auto-increment</param>
        public UtilsController(
            string host, 
            string database, 
            string uid, 
            string password, 
            string port = "5432",
            string tableName = null,
            string primaryKeyName = "Id",
            bool autoIncrement = true) 
            : base(host, database, uid, password, port)
        {
            _tableName = tableName ?? typeof(T).Name;
            _primaryKeyName = primaryKeyName;
            _autoIncrement = autoIncrement;
        }

        /// <summary>
        /// Creates a new UtilsController using existing connection parameters
        /// </summary>
        /// <param name="tableName">Table name (defaults to type name)</param>
        /// <param name="primaryKeyName">Primary key column name (defaults to "Id")</param>
        /// <param name="autoIncrement">Whether primary key is auto-increment</param>
        public UtilsController(
            string tableName = null,
            string primaryKeyName = "Id",
            bool autoIncrement = true) 
            : base()
        {
            _tableName = tableName ?? typeof(T).Name;
            _primaryKeyName = primaryKeyName;
            _autoIncrement = autoIncrement;
        }

        #region LINQ-Style Query Methods

        /// <summary>
        /// Returns all records as IQueryable for LINQ operations
        /// </summary>
        public IQueryable<T> AsQueryable()
        {
            return GetAll().AsQueryable();
        }

        /// <summary>
        /// Gets all records from the table
        /// </summary>
        public List<T> GetAll()
        {
            ConnectDB();
            DataView dv = SelectDv("*", _tableName, "");
            return MapDataViewToList(dv);
        }

        /// <summary>
        /// Filters records using a LINQ expression
        /// Example: Where(x => x.Name == "John")
        /// </summary>
        public List<T> Where(Expression<Func<T, bool>> predicate)
        {
            ConnectDB();
            
            // Parse expression to SQL WHERE clause
            var result = ParseWhereExpression(predicate);
            
            // Set parameters if any
            if (result.Parameters.Count > 0)
            {
                this.NpgsqlParameters = result.Parameters;
            }
            
            DataView dv = SelectDv("*", _tableName, result.WhereClause);
            return MapDataViewToList(dv);
        }

        /// <summary>
        /// Gets the first record matching the predicate, or null if not found
        /// </summary>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var results = Where(predicate);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Gets a single record matching the predicate, throws exception if multiple found
        /// </summary>
        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            var results = Where(predicate);
            if (results.Count > 1)
            {
                throw new InvalidOperationException("Sequence contains more than one element");
            }
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Checks if any records match the predicate
        /// </summary>
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate).Any();
        }

        /// <summary>
        /// Counts records matching the predicate
        /// </summary>
        public new int Count(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return GetAll().Count;
            }
            return Where(predicate).Count;
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Inserts a new entity into the database
        /// </summary>
        public bool Add(T entity)
        {
            ConnectDB();
            var genericObjects = QueryBuilder(entity, _primaryKeyName, _autoIncrement);
            if (genericObjects.Count == 0)
            {
                return false;
            }

            var obj = genericObjects[0];
            return Insert(obj.columns, _tableName, obj.valuesString);
        }

        /// <summary>
        /// Updates an existing entity in the database
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="predicate">Condition for which records to update</param>
        public bool Update(T entity, Expression<Func<T, bool>> predicate)
        {
            ConnectDB();
            var genericObjects = QueryBuilder(entity, _primaryKeyName, _autoIncrement);
            if (genericObjects.Count == 0)
            {
                return false;
            }

            var obj = genericObjects[0];
            
            // Parse the predicate to get WHERE clause
            var result = ParseWhereExpression(predicate);
            
            // Set parameters for WHERE clause
            if (result.Parameters.Count > 0)
            {
                this.NpgsqlParameters = result.Parameters;
            }
            
            return Update(obj.columns, _tableName, obj.valuesString, result.WhereClause);
        }

        /// <summary>
        /// Deletes records matching the predicate
        /// </summary>
        public bool Remove(Expression<Func<T, bool>> predicate)
        {
            ConnectDB();
            
            // Parse the predicate to get WHERE clause
            var result = ParseWhereExpression(predicate);
            
            // Set parameters for WHERE clause
            if (result.Parameters.Count > 0)
            {
                this.NpgsqlParameters = result.Parameters;
            }
            
            return Delete(_tableName, result.WhereClause);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps a DataView to a list of entities
        /// </summary>
        private List<T> MapDataViewToList(DataView dv)
        {
            var result = new List<T>();
            
            if (dv == null || dv.Count == 0)
            {
                return result;
            }

            var properties = typeof(T).GetProperties();
            
            foreach (DataRowView row in dv)
            {
                var entity = new T();
                
                foreach (var prop in properties)
                {
                    try
                    {
                        if (dv.Table.Columns.Contains(prop.Name))
                        {
                            var value = row[prop.Name];
                            
                            if (value != DBNull.Value)
                            {
                                // Handle type conversion
                                if (prop.PropertyType == typeof(DateTime) && value is string)
                                {
                                    DateTime dateValue;
                                    if (DateTime.TryParse((string)value, out dateValue))
                                    {
                                        prop.SetValue(entity, dateValue);
                                    }
                                }
                                else if (prop.PropertyType.IsGenericType && 
                                         prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    // Handle nullable types
                                    var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                                    prop.SetValue(entity, Convert.ChangeType(value, underlyingType));
                                }
                                else
                                {
                                    prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
                                }
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Skip properties that can't be mapped due to type mismatch
                    }
                    catch (FormatException)
                    {
                        // Skip properties with invalid format
                    }
                    catch (InvalidCastException)
                    {
                        // Skip properties that can't be cast
                    }
                }
                
                result.Add(entity);
            }
            
            return result;
        }

        /// <summary>
        /// Parses a LINQ expression into a SQL WHERE clause with parameters
        /// </summary>
        private WhereClauseResult ParseWhereExpression(Expression<Func<T, bool>> predicate)
        {
            var parameters = new List<NpgsqlParameter>();
            var whereClause = ParseExpression(predicate.Body, parameters);
            return new WhereClauseResult
            {
                WhereClause = whereClause,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Recursively parses an expression tree into SQL
        /// </summary>
        private string ParseExpression(Expression expression, List<NpgsqlParameter> parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    var andExp = (BinaryExpression)expression;
                    return string.Format("({0}) AND ({1})", ParseExpression(andExp.Left, parameters), ParseExpression(andExp.Right, parameters));
                
                case ExpressionType.OrElse:
                    var orExp = (BinaryExpression)expression;
                    return string.Format("({0}) OR ({1})", ParseExpression(orExp.Left, parameters), ParseExpression(orExp.Right, parameters));
                
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return ParseBinaryExpression((BinaryExpression)expression, parameters);
                
                case ExpressionType.Not:
                    var notExp = (UnaryExpression)expression;
                    return string.Format("NOT ({0})", ParseExpression(notExp.Operand, parameters));
                
                case ExpressionType.MemberAccess:
                    var memberExp = (MemberExpression)expression;
                    if (memberExp.Expression.NodeType == ExpressionType.Parameter)
                    {
                        return memberExp.Member.Name;
                    }
                    break;
                
                case ExpressionType.Constant:
                    var constExp = (ConstantExpression)expression;
                    var paramName = string.Format("@param{0}", parameters.Count);
                    parameters.Add(new NpgsqlParameter(paramName, constExp.Value ?? DBNull.Value));
                    return paramName;
            }
            
            // For complex expressions, try to evaluate them
            try
            {
                var value = Expression.Lambda(expression).Compile().DynamicInvoke();
                var paramName = string.Format("@param{0}", parameters.Count);
                parameters.Add(new NpgsqlParameter(paramName, value ?? DBNull.Value));
                return paramName;
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(string.Format("Expression type {0} is not supported", expression.NodeType), ex);
            }
        }

        /// <summary>
        /// Parses a binary comparison expression into SQL
        /// </summary>
        private string ParseBinaryExpression(BinaryExpression expression, List<NpgsqlParameter> parameters)
        {
            string left = ParseExpression(expression.Left, parameters);
            string right = ParseExpression(expression.Right, parameters);
            
            string op;
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    op = "=";
                    break;
                case ExpressionType.NotEqual:
                    op = "!=";
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                default:
                    throw new NotSupportedException(string.Format("Binary operator {0} is not supported", expression.NodeType));
            }
            
            return string.Format("{0} {1} {2}", left, op, right);
        }

        #endregion
    }
}
