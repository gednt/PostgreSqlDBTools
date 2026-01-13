# DMF_DBTools.cs

## Description
This file contains the core database management functionality for the PostgreSqlDBTools project. It provides methods for interacting with PostgreSQL databases, including connection management, query execution, and other database operations.

---

## Key Features
- Database connection management.
- Query execution.
- Error handling for database operations.
- Data retrieval and manipulation.
- Support for parameterized queries to prevent SQL injection.

---

## Usage
### Example
```csharp
// Example usage of DMF_DBTools
var dbTools = new DBTools();

// Set connection details
dbTools.Host = "localhost";
dbTools.Database = "mydb";
dbTools.Uid = "user";
dbTools.Password = "password";

// Execute a query
dbTools.Query = "SELECT * FROM table_name";
var data = dbTools.RetrieveDataPostgreSQL();

// Insert data using parameterized queries
dbTools.Query = "INSERT INTO table_name (column1, column2) VALUES (@param1, @param2)";
dbTools.NpgsqlParameters = new List<NpgsqlParameter>
{
    new NpgsqlParameter("@param1", "value1"),
    new NpgsqlParameter("@param2", "value2")
};
dbTools.PostgreSQLExecuteQuery();

// Retrieve objects
dbTools.Query = "SELECT * FROM table_name";
var objects = dbTools.RetrieveObjectPostgreSQL();
```

### Common Methods
- **`PostgreSQLExecuteQuery`**: Executes a SQL query with optional parameterized values.
- **`RetrieveDataPostgreSQL`**: Retrieves data as a `DataView`.
- **`RetrieveObjectPostgreSQL`**: Retrieves data as a list of `GenericObject`.
- **`RetrieveObjectPostgreSQL_Entity`**: Retrieves data as a list of `ExpandoObject` for dynamic mapping.

---

## Dependencies
- Npgsql: A .NET data provider for PostgreSQL.

---

## Related Files
- `Utils.cs`: Contains utility functions that complement the functionality in `DMF_DBTools.cs`.
- `UtilsController.cs`: Provides a higher-level interface for utility functions.