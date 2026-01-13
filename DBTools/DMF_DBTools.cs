using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using DbTools.Model;
using System.Dynamic;

namespace DbTools
{
    /// <summary>
    /// DBTools is a PostgreSQL Library to manipulate data in PostgreSQL Databases
    /// </summary>
    public class DBTools
    {
        #region private variables
        private string host;

        private string uid;

        private string password;

        private string database;

        private string query;

        private string error;

        private string table;
        private string port;

        private string _connectionString;

        private List<NpgsqlParameter> npgsqlParameters;
        public int count;
        #endregion

        #region public getters and setters
        public DBTools()
        {
            //STANDARD PostgreSQL TCP/IP Port
            port = "5432";
        }
        /// <summary>
        /// It is used to place the server address of the database <br></br>
        /// Example(localhost or 127.0.0.1) <br></br>
        /// It is in this formatting.
        /// Host
        /// </summary>
        public string Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
            }
        }
        /// <summary>
        /// It is used to place the userName of your database <br></br>
        /// Example(root or admin). <br></br>
        /// It is in this formatting
        /// user
        /// </summary>
        public string Uid
        {
            get
            {
                return this.uid;
            }
            set
            {
                this.uid = value;
            }
        }
        /// <summary>
        /// It is used to place the password of the database <br></br>
        ///
        /// </summary>
        public string Password
        {
            get
            {

                return this.password;
            }
            set
            {
                this.password = value;
            }
        }
        /// <summary>
        /// It is used to set the Database name <br></br>
        ///
        /// </summary>
        public string Database
        {
            get
            {
                return this.database;
            }
            set
            {
                this.database = value;
            }
        }
        /// <summary>
        /// It is used to set the Query <br></br>
        ///
        /// </summary>
        public string Query
        {
            get
            {
                return this.query;
            }
            set
            {
                this.query = value;
            }
        }

        public string Error
        {
            get
            {
                return this.error;
            }
            set
            {
                this.error = value;
            }
        }

        public string Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.table = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }
        /// <summary>
        /// Specify the port of the database: Standard 5432
        ///
        /// </summary>
        public string Port { get => port; set => port = value; }
        public string ConnectionString
        {


            get
            {
                _connectionString = String.Format("Host={0};Port={1};Database={2};Username={3};Password={4};", host, port, database, uid, password);
                return _connectionString;

            }

        }

        public List<NpgsqlParameter> NpgsqlParameters { get => npgsqlParameters; set => npgsqlParameters = value; }
        #endregion

        #region legacy getters and setters
        public void setHost(string host)
        {
            this.Host = host;
        }

        public string getHost()
        {
            return this.Host;
        }

        public void setUid(string uid)
        {
            this.Uid = uid;
        }

        public string getUid()
        {
            return this.Uid;
        }

        public void setPassword(string password)
        {
            this.Password = password;
        }

        public string getPassword()
        {
            return this.Password;
        }

        public void setQuery(string query)
        {
            this.Query = query;
        }

        public string getQuery()
        {
            return this.Query;
        }

        public void setDataBase(string database)
        {
            this.Database = database;
        }

        public string getDatabase()
        {
            return this.Database;
        }
        #endregion
        #region Public Day One Methods
        /// <summary>
        /// Executes the Query in the database.<br/>
        /// This method is deprecated, use <see cref="PostgreSQLExecuteQuery(string)"/> instead, for better security and compliance with the standards.
        /// </summary>
        /// 
        [Obsolete("This method is deprecated and should use PostgreSQLExecuteQuery instead", false)]
        public void postgreSQLExecuteQuery()
        {

            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
            npgsqlConnection.Open();
            try
            {
                NpgsqlCommand npgsqlCommand = npgsqlConnection.CreateCommand();
                npgsqlCommand.CommandText = this.getQuery();
                npgsqlCommand.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                this.Error = ex.ToString();
            }
            finally
            {
                bool flag = npgsqlConnection.State == ConnectionState.Open;
                if (flag)
                {
                    npgsqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves the DataView Representation in the database.<br/>
        /// This method is deprecated
        /// </summary>
        /// <returns></returns>
        /// 
        [Obsolete("This Method is deprecated, use RetrieveObjectPostgreSQL or RetrieveDataPostgreSQL instead", false)]
        public DataView retrieveDataPostgreSQL()
        {
            DataView defaultView = new DataView();
            try
            {

                NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
                NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
                //Verifica se a conexão foi aberta com sucesso
                try
                {
                    npgsqlConnection.Open();
                    npgsqlCommand = npgsqlConnection.CreateCommand();
                    npgsqlCommand.CommandText = this.getQuery();
                    NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                    DataSet dataSet = new DataSet();
                    npgsqlDataAdapter.Fill(dataSet);
                    this.Count = dataSet.Tables.Count;
                    defaultView = dataSet.Tables[0].DefaultView;
                    npgsqlConnection.Close();
                }
                catch (NpgsqlException e)
                {
                    Error = e.ToString();

                }



            }
            catch (Exception)
            {
                DataSet dataSet2 = new DataSet();
                defaultView = dataSet2.Tables[0].DefaultView;
            }
            return defaultView;
        }
        #endregion

        #region Public improved methods
        /// <summary>
        /// Returns a list of <see cref="GenericObject"/> that can be used in a variety of scenarios
        /// </summary>
        /// <returns></returns>
        public List<GenericObject> RetrieveObjectPostgreSQL()
        {
            using (Npgsql.NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(this.Query, conn);
                if (NpgsqlParameters != null)
                    command.Parameters.AddRange(NpgsqlParameters.ToArray());
                NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                npgsqlDataAdapter.Fill(dataSet);
                List<GenericObject> lstObject = new List<GenericObject>();

                List<String> columns = new List<string>();
                List<String> types = new List<string>();
                DataView values = dataSet.Tables[0].DefaultView;
                int cont = 0;
                foreach (var column in values.Table.Columns)
                {
                    columns.Add(column.ToString());
                    types.Add(dataSet.Tables[0].Columns[columns[cont]].DataType.Name);
                    cont++;

                }

                for (cont = 0; cont < values.Count; cont++)
                {
                    lstObject.Add(new GenericObject
                    {
                        columns = columns.ToArray(),
                        types = types.ToArray(),
                        values = values[cont].Row.ItemArray
                        //  valuesString = values[cont].DataView
                    });
                }

                return lstObject;
            }

        }


        /// <summary>
        /// Retrieves a list of entities of the database, available to be parsed to any object when needed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<ExpandoObject> RetrieveObjectPostgreSQL_Entity(Type obj)
        {
            using (Npgsql.NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString))
            {
              
                List<ExpandoObject> lstObject = new List<ExpandoObject>();



                try
                {
                    NpgsqlCommand command = new NpgsqlCommand(this.Query, conn);
                    if (NpgsqlParameters != null)
                        command.Parameters.AddRange(NpgsqlParameters.ToArray());
                    NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    npgsqlDataAdapter.Fill(dataSet);

                    DataView values = dataSet.Tables[0].DefaultView;

                    foreach (DataRow dr in values.Table.Rows)
                    {
                        lstObject.Add(new ExpandoObject { });

                        IDictionary<string, object> dictionary = (IDictionary<string, object>)lstObject[lstObject.Count - 1];


                        foreach (DataColumn column in dr.Table.Columns)
                        {
                            Console.WriteLine(column.ColumnName);

                            dictionary.Add(column.ColumnName, dr[column.ColumnName]);


                        }

                    }
                }
                catch(NpgsqlException e)
                {
                    Error = e.Message;
                }
               


                return lstObject;
            }

        }

        /// <summary>
        /// Executes a sql query and, if sucessful, returns a void string, if error, returns the error message<br/>
        /// Concatenating the query is not recommendable, use <see cref="NpgsqlParameters"/> to pass your parameters before executing this command.<br/>
        /// </summary>
        /// 
        public void PostgreSQLExecuteQuery(String query = "")
        {

            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString))
            {
                npgsqlConnection.Open();
                try
                {
                    NpgsqlCommand npgsqlCommand = npgsqlConnection.CreateCommand();
                    npgsqlCommand.CommandText = this.getQuery();
                    if (NpgsqlParameters != null)
                        npgsqlCommand.Parameters.AddRange(NpgsqlParameters.ToArray());
                    npgsqlCommand.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    this.Error = ex.ToString();
                }
                npgsqlConnection.Close();
                //finally
                //{
                //    bool flag = npgsqlConnection.State == ConnectionState.Open;
                //    if (flag)
                //    {
                //        npgsqlConnection.Close();
                //    }
                //}
            }

        }



        /// <summary>
        /// Retrieves the DataView Representation in the database.<br/>
        /// Concatenating the query is not recommendable, use <see cref="NpgsqlParameters"/> to pass your parameters before executing this command.<br/>
        /// </summary>
        /// <returns></returns>
        public DataView RetrieveDataPostgreSQL(String query = "")
        {
            DataView defaultView = new DataView();
            try
            {

                using (Npgsql.NpgsqlConnection conn = new NpgsqlConnection(this.ConnectionString))
                {
                    try
                    {
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query != "" ? query : this.Query, conn);
                        if (npgsqlParameters != null)
                            npgsqlCommand.Parameters.AddRange(NpgsqlParameters.ToArray());
                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                        DataSet dataSet = new DataSet();
                        npgsqlDataAdapter.Fill(dataSet);
                        this.Count = dataSet.Tables.Count;
                        defaultView = dataSet.Tables[0].DefaultView;
                    }
                    catch (NpgsqlException e)
                    {
                        Error = e.ToString();

                    }

                }



            }
            catch (Exception e)
            {
                DataSet dataSet2 = new DataSet();
                defaultView = dataSet2.Tables[0].DefaultView;
            }
            return defaultView;
        }


        #endregion
    }
}
