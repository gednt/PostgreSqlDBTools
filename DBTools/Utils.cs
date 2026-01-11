using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTools.Model;
namespace DBTools_Utilities
{
    /// <summary>
    /// Utils Class of the PostgreSQL DBTools Library
    /// </summary>
    public class Utils : DBToolsDll.DBTools
    {
        public Utils(String Host, String Database, String Uid, String pwd, String port)
        {
            this.Host = Host;
            this.Database = Database;
            this.Uid = Uid;
            this.Password = pwd;
            this.Port = port;
        }
        public Utils()
        {

        }

        #region Security Helpers
        /// <summary>
        /// Validates that an identifier (table name, column name) contains only safe characters.
        /// This helps prevent SQL injection in cases where identifiers cannot be parameterized.
        /// </summary>
        /// <param name="identifier">The identifier to validate</param>
        /// <param name="identifierType">Type of identifier for error messaging</param>
        private void ValidateIdentifier(string identifier, string identifierType)
        {
            ValidateIdentifierStatic(identifier, identifierType);
        }

        /// <summary>
        /// Validates a comma-separated list of field identifiers.
        /// </summary>
        /// <param name="fieldList">Comma-separated field list</param>
        private void ValidateFieldList(string fieldList)
        {
            ValidateFieldListStatic(fieldList);
        }

        /// <summary>
        /// Validates that an identifier (table name, column name) contains only safe characters.
        /// This helps prevent SQL injection in cases where identifiers cannot be parameterized.
        /// Allows alphanumeric characters, underscores, double quotes, and periods for qualified names.
        /// For field lists (with multiple fields), use ValidateFieldList instead.
        /// </summary>
        /// <param name="identifier">The identifier to validate</param>
        /// <param name="identifierType">Type of identifier for error messaging</param>
        private static void ValidateIdentifierStatic(string identifier, string identifierType)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException($"The {identifierType} name cannot be null or empty.");
            }

            // Special handling for field lists (SELECT field1, field2, ... FROM ...)
            if (identifierType == "fields" && (identifier.Contains(",") || identifier.Contains("*")))
            {
                ValidateFieldListStatic(identifier);
                return;
            }

            // For single identifiers: allow alphanumeric, underscore, double quote (for escaping), period (for qualified names)
            // PostgreSQL uses double quotes for identifiers instead of backticks
            // This is a conservative approach to prevent injection
            foreach (char c in identifier)
            {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '"' && c != '.')
                {
                    throw new ArgumentException($"The {identifierType} name '{identifier}' contains invalid characters. Only alphanumeric characters, underscores, double quotes, and periods are allowed.");
                }
            }
        }

        /// <summary>
        /// Validates a comma-separated list of field identifiers.
        /// Each field can contain alphanumeric characters, underscores, double quotes, periods, and asterisks.
        /// Spaces are allowed only between tokens for readability (e.g., "field1, field2").
        /// </summary>
        /// <param name="fieldList">Comma-separated field list</param>
        private static void ValidateFieldListStatic(string fieldList)
        {
            if (string.IsNullOrWhiteSpace(fieldList))
            {
                throw new ArgumentException("The field list cannot be null or empty.");
            }

            // Split by comma and validate each field
            string[] fields = fieldList.Split(',');
            foreach (string field in fields)
            {
                string trimmedField = field.Trim();
                if (string.IsNullOrEmpty(trimmedField))
                {
                    throw new ArgumentException("The field list contains empty fields.");
                }

                // Check for spaces within the field (not at boundaries which are trimmed)
                // Spaces should only appear between words in expressions like "table.field AS alias"
                // For basic field names, no spaces should be present
                bool hasSpace = trimmedField.Contains(" ");
                
                // Allow alphanumeric, underscore, double quote, period, asterisk (for SELECT *)
                // PostgreSQL uses double quotes instead of backticks
                // Spaces are allowed but trigger additional validation
                foreach (char c in trimmedField)
                {
                    if (!char.IsLetterOrDigit(c) && c != '_' && c != '"' && c != '.' && c != '*' && c != ' ')
                    {
                        throw new ArgumentException($"The field '{trimmedField}' contains invalid characters. Only alphanumeric characters, underscores, double quotes, periods, asterisks, and spaces are allowed.");
                    }
                }

                // If field contains spaces, validate it's not trying to inject SQL
                // Check for common SQL keywords that shouldn't appear in field names
                if (hasSpace)
                {
                    string upperField = trimmedField.ToUpper();
                    string[] dangerousKeywords = { " OR ", " AND ", " UNION ", " SELECT ", " INSERT ", " UPDATE ", " DELETE ", " DROP ", " CREATE ", " ALTER ", " EXEC ", " EXECUTE ", "--", "/*", "*/" };
                    foreach (string keyword in dangerousKeywords)
                    {
                        if (upperField.Contains(keyword))
                        {
                            throw new ArgumentException($"The field '{trimmedField}' contains potentially dangerous SQL keywords.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates that a WHERE condition uses parameterized queries and does not contain literal values.
        /// This enforces security by rejecting conditions that appear to embed values directly.
        /// </summary>
        /// <param name="condition">The WHERE condition to validate</param>
        /// <param name="hasParameters">Whether NpgsqlParameters is set</param>
        private void ValidateParameterizedCondition(string condition, bool hasParameters)
        {
            ValidateParameterizedConditionStatic(condition, hasParameters);
        }

        /// <summary>
        /// Validates that a WHERE condition uses parameterized queries and does not contain literal values.
        /// This enforces security by rejecting conditions that appear to embed values directly.
        /// </summary>
        /// <param name="condition">The WHERE condition to validate</param>
        /// <param name="hasParameters">Whether NpgsqlParameters is set</param>
        private static void ValidateParameterizedConditionStatic(string condition, bool hasParameters)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                // Empty condition is acceptable (no WHERE clause)
                return;
            }

            // Check for common SQL injection patterns that are never legitimate in WHERE clauses
            string upperCondition = condition.ToUpper();
            string[] injectionPatterns = { " OR 1=1", " OR '1'='1'", " OR TRUE", " OR 1 = 1", "OR 1=1", "OR '1'='1'", "OR TRUE", "OR 1 = 1" };
            foreach (string pattern in injectionPatterns)
            {
                if (upperCondition.Contains(pattern))
                {
                    throw new ArgumentException($"WHERE condition contains potentially dangerous SQL pattern: {pattern.Trim()}");
                }
            }

            // Check for SQL comment markers that could be used for injection
            if (upperCondition.Contains("--") || upperCondition.Contains("/*") || upperCondition.Contains("*/"))
            {
                throw new ArgumentException("WHERE conditions cannot contain SQL comment markers (--, /*, or */) as these may indicate injection attempts.");
            }

            // Check for single quotes followed by OR/AND which typically indicates injection attempts
            // Pattern: 'xxx' OR/AND which is common in SQL injection
            if (condition.Contains("'") && (upperCondition.Contains("' OR ") || upperCondition.Contains("' AND ")))
            {
                throw new ArgumentException("WHERE conditions must use parameter placeholders (e.g., @whereParam0) instead of literal values. Detected pattern: quote followed by OR/AND.");
            }

            // If condition contains comparison operators and no @ symbol (parameter marker), likely not parameterized
            string[] comparisonOperators = { " = ", " != ", " <> ", " > ", " < ", " >= ", " <= ", " LIKE ", " IN " };
            bool hasComparison = false;
            foreach (string op in comparisonOperators)
            {
                if (upperCondition.Contains(op))
                {
                    hasComparison = true;
                    break;
                }
            }

            // If there's a comparison but no parameter markers and no parameters set, it's likely unsafe
            if (hasComparison && !condition.Contains("@") && hasParameters == false)
            {
                throw new ArgumentException("WHERE conditions with comparison operators should use parameter placeholders (e.g., @whereParam0). Pass values through NpgsqlParameters property.");
            }
        }
        #endregion

        #region query utilities
        /// <summary>
        /// Returns a list of a representation of any given object to be used into the Insert and select clauses of this library.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<GenericObject> QueryBuilder(Object obj, String primaryKeyName = "", bool autoIncrement = true)
        {
            connectDB();
            var arrayObject = obj.GetType().GetProperties();
            List<GenericObject_Simple> values = new List<GenericObject_Simple>();
            //List<String> fields = new List<string>();
            //List<String> type = new List<string>();
            List<GenericObject> lstReturn = new List<GenericObject>();
            List<String> columns = new List<string>();
            List<Object> valuesReturn = new List<Object>();
            List<String> typesReturn = new List<string>();
            int cont = 0;
            foreach (var i in arrayObject)
            {

                try
                {
                    if (i.PropertyType.Name.ToString() == "DateTime")
                    {
                        values.Add(new GenericObject_Simple
                        {
                            value = DateTime.Parse((string)obj.GetType().GetProperty(i.Name).GetValue(obj, null).ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                            ,
                            column = i.Name
                            ,
                            type = i.PropertyType.Name
                        });
                        // type.Add(i.PropertyType.Name);

                    }
                    else
                    {
                        switch (autoIncrement)
                        {
                            case false:
                                //  fields.Add(i.Name);
                                values.Add(new GenericObject_Simple
                                {
                                    value = (string)obj.GetType().GetProperty(i.Name).GetValue(obj, null).ToString()
                                  ,
                                    column = i.Name
                                  ,
                                    type = i.PropertyType.Name
                                });
                                // type.Add(i.PropertyType.Name);
                                cont++;
                                break;
                            case true:
                                if (i.Name == primaryKeyName)
                                {

                                }
                                else
                                {
                                    values.Add(new GenericObject_Simple
                                    {
                                        value = (string)obj.GetType().GetProperty(i.Name).GetValue(obj, null).ToString()
                                        ,
                                        column = i.Name
                                        ,
                                        type = i.PropertyType.Name
                                    });
                                    cont++;
                                }
                                break;
                        }






                    }

                }
                catch
                {

                }



            }
            List<String> valueString = new List<string>();
            foreach (var value in values)
            {
                valueString.Add(value.value.ToString());
            }

            for (cont = 0; cont < values.Count; cont++)
            {
                columns.Add(values[cont].column);
                valuesReturn.Add(values[cont].value);
                typesReturn.Add(values[cont].type);
            }
            lstReturn.Add(new GenericObject { columns = columns.ToArray(), values = valuesReturn.ToArray(), types = typesReturn.ToArray(), valuesString = valueString.ToArray() });





            return lstReturn;


        }
        public void connectDB()
        {
            setDataBase(Database);
            setHost(Host);
            setPassword(Password);
            setUid(Uid);
        }
        /// <summary>
        /// Returns a string array of the objects of the Database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// 

        public String[] getInBd(String query)
        {
            setQuery(query);

            DataView dv = new DataView();
            dv = RetrieveDataPostgreSQL();
            String[] arrayQuery = new String[dv.Count];
            for (int cont = 0; cont < dv.Count; cont++)
            {
                arrayQuery[cont] = dv[cont][0].ToString();
            }
            return arrayQuery;


        }
        /// <summary>
        /// Returns a Dataview of the objects of the Database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataView getInBdDv(String query)
        {
            setQuery(query);

            DataView dv = new DataView();
            try
            {
                dv = RetrieveDataPostgreSQL();
            }
            catch (Exception e)
            {
                Error = e.ToString();
                // System.Windows.MessageBox.Show(error);
                return null;

            }
            String[] arrayQuery = new String[dv.Count];
            for (int cont = 0; cont < dv.Count; cont++)
            {
                arrayQuery[cont] = dv[cont][0].ToString();
            }
            return dv;
        }
        /// <summary>
        /// Executes any sql query that returns no value
        /// </summary>
        /// <param name="query"></param>
        public void ExecuteQuery(String query)
        {
            setQuery(query);
            PostgreSQLExecuteQuery();


        }
        #endregion

        #region Data Manipulation modules
        //MODULOS DE MANIPULAÇAO DE DADOS
        /// <summary>
        /// Returns a DataView based on the parameters given<br/>
        /// This class can and should be used with the <see cref="QueryBuilder(object)">QueryBuilder Command</see><br/>
        /// NOTE: For security, _fields and _table should be validated/sanitized as they cannot be parameterized.<br/>
        /// The _conditions parameter MUST use parameter placeholders (e.g., "id = @whereParam0") and values MUST be passed through NpgsqlParameters property before calling this method.
        /// This method will reject conditions that contain literal values.
        /// </summary>
        /// <param name="_fields">Field names to select</param>
        /// <param name="_table">Table name</param>
        /// <param name="_conditions">WHERE conditions (MUST use parameter placeholders like @whereParam0 for values)</param>
        /// <returns></returns>
        public DataView Select(String _fields, String _table, String _conditions)
        {
            // Validate table and field names to prevent injection
            // Table and field names cannot be parameterized in PostgreSQL
            ValidateIdentifier(_table, "table");
            ValidateIdentifier(_fields, "fields");

            // Validate that conditions are parameterized
            ValidateParameterizedCondition(_conditions, this.NpgsqlParameters != null && this.NpgsqlParameters.Count > 0);

            String query = "";
            if (_conditions != "")
            {
                query = String.Format("SELECT {0} FROM {1} WHERE {2}", _fields, _table, _conditions);
            }
            else
            {
                query = String.Format("SELECT {0} FROM {1}", _fields, _table);
            }

            return getInBdDv(query);


        }
        /// <summary>
        /// Inserts the data into the database based on the parameters given<br/>
        /// This class can and should be used with the <see cref="QueryBuilder(object)">QueryBuilder Command</see><br/>
        /// Uses parameterized queries to prevent SQL injection.
        /// </summary>
        /// <param name="_fields"></param>
        /// <param name="_table"></param>
        /// <param name="_values"></param>
        /// <returns></returns>
        public bool Insert(String[] _fields, String _table, String[] _values)
        {
            // Validate table name
            ValidateIdentifier(_table, "table");

            String fields = "", paramPlaceholders = "";
            List<Npgsql.NpgsqlParameter> parameters = new List<Npgsql.NpgsqlParameter>();

            //BUILD FIELDS AND PARAMETERS
            String query = "INSERT INTO " + _table + "(";

            for (int cont = 0; cont < _fields.Length; cont++)
            {
                // Validate field names
                ValidateIdentifier(_fields[cont], "field");
                fields += _fields[cont] + ",";
                paramPlaceholders += "@param" + cont + ",";
                
                // Add parameterized value
                parameters.Add(new Npgsql.NpgsqlParameter("@param" + cont, _values[cont]));
            }
            fields = fields.Remove(fields.Length - 1, 1);
            paramPlaceholders = paramPlaceholders.Remove(paramPlaceholders.Length - 1, 1);

            //FINALIZE QUERY
            query += fields + ") VALUES(" + paramPlaceholders + ")";
            
            //SET PARAMETERS AND EXECUTE
            this.NpgsqlParameters = parameters;
            setQuery(query);
            PostgreSQLExecuteQuery();
            
            if (Error != null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Updates the database using parameterized queries to prevent SQL injection.<br/>
        /// NOTE: The condition parameter MUST use parameter placeholders (e.g., "id = @whereParam0") and values MUST be passed through NpgsqlParameters property before calling this method.
        /// This method will reject conditions that contain literal values.
        /// </summary>
        /// <param name="_fields"></param>
        /// <param name="_table"></param>
        /// <param name="_values"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Update(String[] _fields, String _table, String[] _values, String condition = "")
        {
            // Validate table name
            ValidateIdentifier(_table, "table");

            // Validate that condition is parameterized if provided
            // Note: We check for existing parameters before we add the SET parameters
            bool hasWhereParameters = this.NpgsqlParameters != null && this.NpgsqlParameters.Count > 0;
            ValidateParameterizedCondition(condition, hasWhereParameters);

            String setClause = "";
            List<Npgsql.NpgsqlParameter> parameters = new List<Npgsql.NpgsqlParameter>();

            // Merge with any existing parameters (for WHERE clause) first
            if (this.NpgsqlParameters != null)
            {
                parameters.AddRange(this.NpgsqlParameters);
            }

            //BUILD SET CLAUSE WITH PARAMETERS
            String query = "UPDATE " + _table + " SET ";

            for (int cont = 0; cont < _fields.Length; cont++)
            {
                // Validate field names
                ValidateIdentifier(_fields[cont], "field");
                
                if (cont > 0)
                    setClause += ",";
                    
                // Use setParam prefix to avoid conflicts with WHERE clause parameters
                setClause += _fields[cont] + "=@setParam" + cont;
                
                // Add parameterized value
                parameters.Add(new Npgsql.NpgsqlParameter("@setParam" + cont, _values[cont]));
            }

            query += setClause;
            
            if (!string.IsNullOrEmpty(condition))
            {
                query += " WHERE " + condition;
            }

            //SET PARAMETERS AND EXECUTE
            this.NpgsqlParameters = parameters;
            setQuery(query);
            PostgreSQLExecuteQuery();
            
            if (Error != null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Deletes the row from the database.<br/>
        /// For security reasons, the use of a condition is mandatory.<br/>
        /// NOTE: The condition parameter MUST use parameter placeholders (e.g., "id = @whereParam0") and values MUST be passed through NpgsqlParameters property before calling this method.
        /// This method will reject conditions that contain literal values.
        /// </summary>
        /// <param name="_table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Delete(String _table, String condition)
        {
            // Validate table name
            ValidateIdentifier(_table, "table");

            // Validate that condition is parameterized
            ValidateParameterizedCondition(condition, this.NpgsqlParameters != null && this.NpgsqlParameters.Count > 0);

            //BUILD DELETE QUERY
            String query = "DELETE FROM " + _table + " WHERE " + condition;
            
            //EXECUTE QUERY
            setQuery(query);
            PostgreSQLExecuteQuery();
            
            if (Error != null)
            {
                return false;
            }
            return true;
        }
        //MODULOS DE MANIPULAÇAO DE DADOS
        /// <summary>
        /// Returns a DataView based on the query without the select clause<br/>
        /// NOTE: For security, MUST use parameter placeholders (e.g., @whereParam0) for values in WHERE conditions and pass values through NpgsqlParameters property before calling this method.
        /// Table and field names should be validated separately and cannot be parameterized.
        /// This method will reject queries that contain obvious SQL injection patterns.
        /// </summary>
        /// <param name="query_without_select">Query without SELECT keyword (MUST use parameter placeholders for values in WHERE conditions)</param>
        /// <returns></returns>
        public DataView Select(String query_without_select)
        {
            // Validate that any WHERE clause in the query uses parameters
            if (!string.IsNullOrWhiteSpace(query_without_select))
            {
                string upperQuery = query_without_select.ToUpper();
                string whereKeyword = " WHERE ";
                int whereIndex = upperQuery.IndexOf(whereKeyword);
                
                if (whereIndex >= 0)
                {
                    // Extract the WHERE clause (from WHERE to end or to next major clause)
                    string whereClause = query_without_select.Substring(whereIndex + whereKeyword.Length);
                    
                    // Stop at GROUP BY, ORDER BY, LIMIT, HAVING, UNION if present
                    string upperWhereClause = whereClause.ToUpper();
                    int endIndex = whereClause.Length;
                    string[] clauseTerminators = { " GROUP BY ", " ORDER BY ", " LIMIT ", " HAVING ", " UNION " };
                    foreach (string terminator in clauseTerminators)
                    {
                        int terminatorIndex = upperWhereClause.IndexOf(terminator);
                        if (terminatorIndex >= 0 && terminatorIndex < endIndex)
                        {
                            endIndex = terminatorIndex;
                        }
                    }
                    
                    whereClause = whereClause.Substring(0, endIndex).Trim();
                    
                    // Validate the WHERE clause is parameterized
                    ValidateParameterizedCondition(whereClause, this.NpgsqlParameters != null && this.NpgsqlParameters.Count > 0);
                }
            }

            return getInBdDv("SELECT " + query_without_select);


        }

        //MODULOS DE MANIPULAÇAO DE DADOS
        /// <summary>
        /// Returns a string based on the parameters given<br/>
        /// NOTE: For security, this method validates identifiers and checks for SQL injection patterns. 
        /// The _conditions parameter should use parameter placeholders (e.g., @whereParam0) for values.
        /// Use NpgsqlParameters to provide the actual values when executing the query.
        /// </summary>
        /// <param name="_fields">Field names to select</param>
        /// <param name="_table">Table name</param>
        /// <param name="_conditions">WHERE conditions (should use parameter placeholders like @whereParam0 for values)</param>
        /// <returns></returns>
        public static string SelectQuery(String _fields, String _table, String _conditions)
        {
            // Validate identifiers
            ValidateIdentifierStatic(_table, "table");
            ValidateIdentifierStatic(_fields, "fields");

            // Validate for injection patterns
            // Note: Static method can't check NpgsqlParameters, so we only check for obvious injection patterns
            // Pass false for hasParameters but the validation will still check for @ symbols in comparisons
            if (!string.IsNullOrWhiteSpace(_conditions))
            {
                // For static method, we check for injection patterns but allow conditions without parameters
                // if they contain @ symbols (indicating intent to use parameters)
                string upperCondition = _conditions.ToUpper();
                
                // Check for obvious injection patterns
                string[] injectionPatterns = { " OR 1=1", " OR '1'='1'", " OR TRUE", " OR 1 = 1", "OR 1=1", "OR '1'='1'", "OR TRUE", "OR 1 = 1" };
                foreach (string pattern in injectionPatterns)
                {
                    if (upperCondition.Contains(pattern))
                    {
                        throw new ArgumentException($"WHERE condition contains potentially dangerous SQL pattern: {pattern.Trim()}");
                    }
                }
                
                // Check for comment markers
                if (upperCondition.Contains("--") || upperCondition.Contains("/*") || upperCondition.Contains("*/"))
                {
                    throw new ArgumentException("WHERE conditions cannot contain SQL comment markers (--, /*, or */).");
                }
                
                // Check for quote + OR/AND pattern
                if (_conditions.Contains("'") && (upperCondition.Contains("' OR ") || upperCondition.Contains("' AND ")))
                {
                    throw new ArgumentException("WHERE conditions must use parameter placeholders. Detected: quote followed by OR/AND.");
                }
            }

            String query = "";
            if (_conditions != "")
            {
                query = String.Format("SELECT {0} FROM {1} WHERE {2}", _fields, _table, _conditions);
            }
            else
            {
                query = String.Format("SELECT {0} FROM {1}", _fields, _table);
            }

            return query;


        }
        /// <summary>
        /// Returns an insert query with parameterized placeholders based on the parameters given<br/>
        /// This method returns a query with @param0, @param1, etc. placeholders.<br/>
        /// Use NpgsqlParameters to provide the actual values when executing the query.
        /// </summary>
        /// <param name="_fields">Field names</param>
        /// <param name="_table">Table name</param>
        /// <param name="_values">Values (used only to determine placeholder count)</param>
        /// <returns>Parameterized INSERT query string</returns>
        public static string InsertQuery(String[] _fields, String _table, String[] _values)
        {
            // Validate table name
            ValidateIdentifierStatic(_table, "table");
            
            String fields = "", paramPlaceholders = "";
            String query = "INSERT INTO " + _table + "(";

            for (int cont = 0; cont < _fields.Length; cont++)
            {
                // Validate field names
                ValidateIdentifierStatic(_fields[cont], "field");
                fields += _fields[cont] + ",";
                paramPlaceholders += "@param" + cont + ",";
            }
            fields = fields.Remove(fields.Length - 1, 1);
            paramPlaceholders = paramPlaceholders.Remove(paramPlaceholders.Length - 1, 1);

            query += fields + ") VALUES(" + paramPlaceholders + ")";
            
            return query;
        }
        
        /// <summary>
        /// Legacy method that creates INSERT query with embedded values (DEPRECATED - SQL INJECTION RISK)<br/>
        /// This method directly embeds values in the SQL query which is vulnerable to SQL injection.<br/>
        /// Use InsertQuery() with NpgsqlParameters instead for better security.
        /// </summary>
        /// <param name="_fields">Field names</param>
        /// <param name="_table">Table name</param>
        /// <param name="_values">Values to embed (UNSAFE)</param>
        /// <returns>INSERT query string with embedded values</returns>
        [Obsolete("This method is deprecated due to SQL injection risks. Use InsertQuery() with NpgsqlParameters instead.", false)]
        public static string InsertQueryLegacy(String[] _fields, String _table, String[] _values)
        {
            String fields = "", values = "";
            //MONTA OS CAMPOS
            String query = "INSERT INTO " + _table + "(";

            for (int cont = 0; cont < _fields.Length; cont++)
            {
                fields += _fields[cont] + ",";
            }
            fields = fields.Remove(fields.Length - 1, 1);
            fields += ") VALUES(";
            //VALORES
            for (int cont = 0; cont < _fields.Length; cont++)
            {
                double numero;
                if (double.TryParse(_values[cont], out numero) == false)
                {
                    if (values != "''" && _values[cont] != null)
                    {
                        if (_values[cont].Length > 0)
                        {
                            if (_values[cont].Substring(0, 1) != "'")
                            {
                                values += "'" + _values[cont] + "',";
                            }
                            else
                            {
                                values += _values[cont] + ",";
                            }

                        }
                        else
                        {
                            if (_values[cont].Length == 0)
                                values += "null,";
                        }

                    }
                    else
                    {
                        if (_values[cont].Length > 0)
                        {
                            if (_values[cont].Substring(0, 1) != "'")
                            {
                                values += _values[cont];
                            }

                        }
                        else
                        {
                            values += "null,";
                        }
                    }

                }
                else
                {
                    values += _values[cont].Replace(",", ".") + ",";
                }

            }
            values = values.Remove(values.Length - 1, 1);

            //FINALIZA A QUERY
            query += fields;
            query += values;

            query += ")";
            //RETORNA A QUERY
            return query;


        }
        /// <summary>
        /// Returns an UPDATE query with parameterized placeholders<br/>
        /// This method returns a query with @setParam0, @setParam1, etc. placeholders for SET values.<br/>
        /// Use NpgsqlParameters to provide the actual values when executing the query.<br/>
        /// NOTE: The condition parameter should also use parameter placeholders (e.g., @whereParam0) for values to avoid naming conflicts.
        /// </summary>
        /// <param name="_fields">Field names</param>
        /// <param name="_table">Table name</param>
        /// <param name="_values">Values (used only to determine placeholder count)</param>
        /// <param name="condition">WHERE condition (should use parameter placeholders like @whereParam0 for values)</param>
        /// <returns>Parameterized UPDATE query string</returns>
        public static string UpdateQuery(String[] _fields, String _table, String[] _values, String condition = "")
        {
            // Validate table name
            ValidateIdentifierStatic(_table, "table");
            
            String setClause = "";
            String query = "UPDATE " + _table + " SET ";

            for (int cont = 0; cont < _fields.Length; cont++)
            {
                // Validate field names
                ValidateIdentifierStatic(_fields[cont], "field");
                
                if (cont > 0)
                    setClause += ",";
                    
                // Use setParam prefix to avoid conflicts with WHERE clause parameters
                setClause += _fields[cont] + "=@setParam" + cont;
            }

            query += setClause;
            
            if (!string.IsNullOrEmpty(condition))
            {
                query += " WHERE " + condition;
            }

            return query;
        }

        /// <summary>
        /// Legacy method that creates UPDATE query with embedded values (DEPRECATED - SQL INJECTION RISK)<br/>
        /// This method directly embeds values in the SQL query which is vulnerable to SQL injection.<br/>
        /// Use UpdateQuery() with NpgsqlParameters instead for better security.
        /// </summary>
        /// <param name="_fields">Field names</param>
        /// <param name="_table">Table name</param>
        /// <param name="_values">Values to embed (UNSAFE)</param>
        /// <param name="condition">WHERE condition</param>
        /// <returns>UPDATE query string with embedded values</returns>
        [Obsolete("This method is deprecated due to SQL injection risks. Use UpdateQuery() with NpgsqlParameters instead.", false)]
        public static string UpdateQueryLegacy(String[] _fields, String _table, String[] _values, String condition = "")
        {
            String fields = "", values = "";
            //MONTA OS CAMPOS
            String query = "UPDATE  " + _table + " SET ";
            //VALORES
            for (int cont = 0; cont < _fields.Length; cont++)
            {
                double numero;
                if (double.TryParse(_values[cont], out numero) == false)
                {
                    if (values != "''" && _values[cont] != null)
                    {
                        if (_values[cont].Length > 0)
                        {
                            if (_values[cont].Substring(0, 1) != "'")
                            {
                                _values[cont] = "'" + _values[cont] + "'";
                            }
                            else
                            {
                                _values[cont] = _values[cont] + "";
                            }

                        }
                        else
                        {
                            if (_values[cont].Length == 0)
                                _values[cont] = "null";
                        }

                    }
                    else
                    {
                        if (_values[cont] == null)
                        {
                            _values[cont] = "null";
                        }
                        else
                        {
                            if (_values[cont].Length > 0)
                            {
                                if (_values[cont].Substring(0, 1) != "'")
                                {
                                    _values[cont] = _values[cont];
                                }

                            }
                            else
                            {
                                _values[cont] = "null";
                            }
                        }

                    }

                }
                else
                {
                    _values[cont] = _values[cont].Replace(",", ".");
                }

            }
            for (int cont = 0; cont < _fields.Length; cont++)
            {
                fields += _fields[cont] + "=" + _values[cont] + ",";
            }
            //fields = fields.Remove(fields.Length - 1, 1);
            fields = fields.Substring(0, fields.Length - 1);
            fields += " WHERE " + condition;
            query += fields;

            //Retorna a query
            return query;


        }
        /// <summary>
        /// Returns a DELETE query<br/>
        /// For security reasons, the use of a condition is mandatory.<br/>
        /// NOTE: The condition parameter should use parameter placeholders (e.g., "id = @id") for values.
        /// Use NpgsqlParameters to provide the actual values when executing the query.
        /// </summary>
        /// <param name="_table">Table name</param>
        /// <param name="condition">WHERE condition (should use parameter placeholders for values)</param>
        /// <returns>DELETE query string</returns>
        public static string DeleteQuery(String _table, String condition)
        {
            // Validate table name
            ValidateIdentifierStatic(_table, "table");

            String query = "DELETE FROM " + _table + " WHERE " + condition;
            
            return query;
        }
        #endregion



    }
}
