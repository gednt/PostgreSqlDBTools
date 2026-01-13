# Utils.cs

## Description
This file contains utility functions for database operations. These functions are designed to simplify common tasks such as data formatting, validation, query building, and data manipulation.

---

## Key Features
- **Validation Functions**: Ensure safe and secure database operations by validating identifiers, field lists, and WHERE conditions.
- **Query Utilities**: Build dynamic SQL queries for SELECT, INSERT, and UPDATE operations.
- **Data Manipulation Modules**: Perform CRUD operations with parameterized queries to prevent SQL injection.
- **Security Enhancements**: Strict validation to prevent SQL injection and enforce safe query practices.

---

## Use Cases
### 1. Security Helpers
- **Validation of Identifiers**: Ensures that table names, column names, and other identifiers contain only safe characters to prevent SQL injection.
- **Validation of Field Lists**: Validates comma-separated field lists for safety.
- **Validation of WHERE Conditions**: Ensures that WHERE conditions use parameterized queries and do not contain literal values.

### 2. Query Utilities
- **Dynamic Query Building**: Use `QueryBuilder` to dynamically generate SQL queries for INSERT and SELECT operations.
- **Database Connection Management**: Establishes and manages connections to the PostgreSQL database.
- **Data Retrieval**: Fetches data from the database as `DataView` or string arrays.

### 3. Data Manipulation Modules
- **Select**: Retrieves data based on specified fields, table, and conditions.
- **Insert**: Inserts data into the database using parameterized queries.
- **Update**: Updates existing records in the database with parameterized queries.
- **Delete**: Deletes records from the database based on conditions.

---

## Usage
### Example
```csharp
// Example usage of Utils
var utils = new Utils("localhost", "mydb", "user", "password", "5432");

// Validate identifiers
utils.ValidateIdentifier("tableName", "table");

// Build and execute a query
var dataView = utils.SelectDv("*", "tableName", "id = @whereParam0");

// Insert data
var success = utils.Insert(new[] {"column1", "column2"}, "tableName", new[] {"value1", "value2"});

// Update data
var updated = utils.Update(new[] {"column1"}, "tableName", new[] {"newValue"}, "id = @whereParam0");

// Delete data
var deleted = utils.Delete("tableName", "id = @whereParam0");
```

---

## Related Files
- `DMF_DBTools.cs`: Core database management functionality that may use utilities from `Utils.cs`.
- `UtilsController.cs`: Provides a higher-level interface for utility functions.