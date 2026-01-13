# UtilsController.cs

## Description
The `UtilsController<T>` class is a generic controller for LINQ-style database manipulation. It is compatible with any model type `T` and provides Entity Framework-like CRUD operations. This class is designed to simplify database interactions by offering a high-level interface for common operations.

---

## Key Features
- **LINQ-style Query Methods**: Perform operations like `Where`, `FirstOrDefault`, `SingleOrDefault`, `Any`, and `Count` using LINQ expressions.
- **CRUD Operations**: Includes methods for adding, updating, and removing records, as well as saving changes.
- **Helper Methods**: Utility functions for mapping data and parsing expressions.

---

## Constructors
### UtilsController(string host, string database, string uid, string password, string port = "5432", string tableName = null, string primaryKeyName = "Id", bool autoIncrement = true)
Creates a new `UtilsController` for the specified model type.

#### Parameters:
- `host`: Database host.
- `database`: Database name.
- `uid`: Database user.
- `password`: Database password.
- `port`: Database port (default: `5432`).
- `tableName`: Table name (defaults to type name).
- `primaryKeyName`: Primary key column name (defaults to `"Id"`).
- `autoIncrement`: Whether the primary key is auto-increment (default: `true`).

### UtilsController(string tableName = null, string primaryKeyName = "Id", bool autoIncrement = true)
Creates a new `UtilsController` using existing connection parameters.

#### Parameters:
- `tableName`: Table name (defaults to type name).
- `primaryKeyName`: Primary key column name (defaults to `"Id"`).
- `autoIncrement`: Whether the primary key is auto-increment (default: `true`).

---

## Methods

### LINQ-Style Query Methods
- **`IQueryable<T> AsQueryable()`**: Returns all records as `IQueryable` for LINQ operations.
- **`List<T> GetAll()`**: Retrieves all records from the table.
- **`List<T> Where(Expression<Func<T, bool>> predicate)`**: Filters records using a LINQ expression.
- **`T FirstOrDefault(Expression<Func<T, bool>> predicate)`**: Gets the first record matching the predicate, or `null` if not found.
- **`T SingleOrDefault(Expression<Func<T, bool>> predicate)`**: Gets a single record matching the predicate, throws an exception if multiple records are found.
- **`bool Any(Expression<Func<T, bool>> predicate)`**: Checks if any records match the predicate.
- **`int Count(Expression<Func<T, bool>> predicate = null)`**: Counts records matching the predicate.

### CRUD Operations
- **`bool Add(T entity)`**: Inserts a new entity into the database.
- **`bool Update(T entity, Expression<Func<T, bool>> predicate)`**: Updates an existing entity in the database.
- **`bool Remove(Expression<Func<T, bool>> predicate)`**: Deletes records matching the predicate.
- **`bool SaveChanges(T entity)`**: Saves changes to an entity by its primary key value.

### Helper Methods
- **`List<T> MapDataViewToList(DataView dv)`**: Maps a `DataView` to a list of entities.
- **`WhereClauseResult ParseWhereExpression(Expression<Func<T, bool>> predicate)`**: Parses a LINQ expression into a SQL `WHERE` clause with parameters.
- **`string ParseExpression(Expression expression, List<NpgsqlParameter> parameters)`**: Recursively parses an expression tree into SQL.
- **`string ParseBinaryExpression(BinaryExpression expression, List<NpgsqlParameter> parameters)`**: Parses a binary comparison expression into SQL.

---

## Usage
### Example
```csharp
// Example usage of UtilsController
var controller = new UtilsController<MyModel>("localhost", "mydb", "user", "password");

// Adds a new record
controller.Add(new MyModel { Name = "John" });

// Query records
var results = controller.Where(x => x.Name == "John");

// Updates a record
controller.Update(new MyModel { Name = "John Doe" }, x => x.Name == "John");

// Deletes a record
controller.Remove(x => x.Name == "John Doe");
```

---

## Related Files
- `Utils.cs`: Contains utility functions used by `UtilsController`.
- `DMF_DBTools.cs`: Core database management functionality that complements `UtilsController`.