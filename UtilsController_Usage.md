# UtilsController<T> Usage Guide

The `UtilsController<T>` class provides Entity Framework-like LINQ manipulation for PostgreSQL databases. It is a generic controller compatible with any model type and offers CRUD operations with LINQ-style syntax.

## Features

- **Generic Type Support**: Works with any POCO (Plain Old CLR Object) model
- **LINQ Expressions**: Use lambda expressions for filtering and querying
- **Parameterized Queries**: All operations use NpgsqlParameter for SQL injection protection
- **Entity Framework-like API**: Familiar methods like `Where()`, `FirstOrDefault()`, `Add()`, `Update()`, `Remove()`

## Installation

The `UtilsController<T>` class is part of the `DBTools_Utilities` namespace in the DBTools library.

```csharp
using DBTools_Utilities;
```

## Basic Usage

### 1. Define Your Model

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

### 2. Create an Instance of UtilsController

```csharp
// Option 1: Specify connection parameters
var userController = new UtilsController<User>(
    host: "localhost",
    database: "mydb",
    uid: "postgres",
    password: "password",
    port: "5432",
    tableName: "users",        // Optional: defaults to type name "User"
    primaryKeyName: "Id",      // Optional: defaults to "Id"
    autoIncrement: true        // Optional: defaults to true
);

// Option 2: Use default constructor if connection is already set
var userController = new UtilsController<User>(
    tableName: "users",
    primaryKeyName: "Id",
    autoIncrement: true
);
```

## CRUD Operations

### Create (Insert)

```csharp
// Add a new user
var newUser = new User
{
    Name = "John Doe",
    Email = "john@example.com",
    CreatedDate = DateTime.Now
};

bool success = userController.Add(newUser);
if (success)
{
    Console.WriteLine("User added successfully!");
}
```

### Read (Select/Query)

```csharp
// Get all users
List<User> allUsers = userController.GetAll();

// Filter with LINQ expression
List<User> johns = userController.Where(u => u.Name == "John Doe");

// Get first match or null
User firstJohn = userController.FirstOrDefault(u => u.Name == "John Doe");

// Get single match (throws exception if multiple found)
User singleUser = userController.SingleOrDefault(u => u.Id == 1);

// Check if any records match
bool hasJohns = userController.Any(u => u.Name == "John Doe");

// Count records
int totalUsers = userController.Count();
int johnsCount = userController.Count(u => u.Name == "John Doe");

// Use as IQueryable for advanced LINQ
IQueryable<User> queryableUsers = userController.AsQueryable();
var recentUsers = queryableUsers
    .Where(u => u.CreatedDate > DateTime.Now.AddDays(-7))
    .OrderBy(u => u.Name)
    .ToList();
```

### Update

```csharp
// Update specific users
var updatedUser = new User
{
    Name = "Jane Doe",
    Email = "jane@example.com"
};

// Update all users with Id = 1
bool updateSuccess = userController.Update(updatedUser, u => u.Id == 1);

// Or use SaveChanges for a single entity by primary key
var user = userController.FirstOrDefault(u => u.Id == 1);
if (user != null)
{
    user.Name = "Updated Name";
    bool saved = userController.SaveChanges(user);
}
```

### Delete (Remove)

```csharp
// Delete users matching a condition
bool deleted = userController.Remove(u => u.Id == 1);

// Delete multiple users
bool deletedMultiple = userController.Remove(u => u.CreatedDate < DateTime.Now.AddYears(-1));
```

## Advanced Filtering

### Complex Expressions

```csharp
// Combine multiple conditions
List<User> filtered = userController.Where(u => 
    u.Name == "John Doe" && u.CreatedDate > DateTime.Now.AddDays(-30)
);

// Use OR conditions
List<User> multipleNames = userController.Where(u => 
    u.Name == "John Doe" || u.Name == "Jane Doe"
);

// Comparison operators
List<User> recentUsers = userController.Where(u => 
    u.Id > 100 && u.Id <= 200
);
```

### Supported Expression Types

The UtilsController supports the following LINQ expression types:

- **Equality**: `==`, `!=`
- **Comparison**: `>`, `>=`, `<`, `<=`
- **Logical**: `&&` (AND), `||` (OR), `!` (NOT)
- **Member Access**: Property references (e.g., `u.Name`)
- **Constants**: Literal values in expressions

## Example: Complete Application

```csharp
using System;
using System.Collections.Generic;
using DBTools_Utilities;

namespace MyApplication
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Initialize controller
            var productController = new UtilsController<Product>(
                host: "localhost",
                database: "shop_db",
                uid: "postgres",
                password: "password",
                tableName: "products"
            );

            // Add a new product
            var newProduct = new Product
            {
                Name = "Widget",
                Price = 29.99m,
                Stock = 100
            };
            productController.Add(newProduct);

            // Get all products
            var allProducts = productController.GetAll();
            Console.WriteLine($"Total products: {allProducts.Count}");

            // Find expensive products
            var expensiveProducts = productController.Where(p => p.Price > 20.0m);
            foreach (var product in expensiveProducts)
            {
                Console.WriteLine($"{product.Name}: ${product.Price}");
            }

            // Update stock
            var widget = productController.FirstOrDefault(p => p.Name == "Widget");
            if (widget != null)
            {
                widget.Stock = 150;
                productController.SaveChanges(widget);
            }

            // Delete out-of-stock products
            productController.Remove(p => p.Stock == 0);
        }
    }
}
```

## Security Notes

All query operations use parameterized queries with NpgsqlParameter to prevent SQL injection attacks. Table and field names are validated to ensure they contain only safe characters.

## Comparison with Utils Class

The `UtilsController<T>` extends the base `Utils` class with:

1. **Type Safety**: Generic type parameter ensures compile-time type checking
2. **LINQ Expressions**: Lambda expressions instead of string-based queries
3. **Automatic Mapping**: Converts DataView results to strongly-typed objects
4. **Simplified API**: Methods like `Add()`, `Remove()`, `Where()` instead of raw SQL

## Migration from Utils to UtilsController

**Before (Utils):**
```csharp
var utils = new Utils("localhost", "mydb", "postgres", "password", "5432");
DataView dv = utils.Select("*", "users", "name = 'John'");
```

**After (UtilsController<T>):**
```csharp
var controller = new UtilsController<User>("localhost", "mydb", "postgres", "password", "5432");
List<User> users = controller.Where(u => u.Name == "John");
```

## Limitations

- Expression tree parsing supports common scenarios but may not handle all complex LINQ operations
- Custom SQL functions in expressions are not supported
- Navigation properties (relationships) are not automatically loaded
- For complex queries, consider using the base `Utils` class methods with raw SQL

## Tips

1. **Use Specific Filters**: Apply filters in `Where()` expressions instead of loading all data with `GetAll()`
2. **Batch Operations**: For bulk inserts/updates, consider using the base `Utils` class for better performance
3. **Connection Management**: Reuse controller instances when possible to minimize connection overhead
4. **Primary Key**: Ensure your model has a property matching the `primaryKeyName` parameter for `SaveChanges()` to work

## Support

For issues or questions, please refer to the main DBTools documentation or submit an issue on the repository.
