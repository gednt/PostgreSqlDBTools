# UtilsTests.cs

## Description
This file contains unit tests for the `Utils.cs` file in the DBTools project. The tests ensure that the utility functions behave as expected under various conditions, including edge cases and error scenarios.

---

## Key Features
- **Validation Tests**: Ensure that identifier validation, field list validation, and WHERE condition validation work correctly.
- **Query Utility Tests**: Verify the correctness of dynamic query building methods.
- **Data Manipulation Tests**: Test CRUD operations for proper functionality and security.
- **Security Tests**: Validate that SQL injection attempts are correctly identified and rejected.

---

## Usage
### Running the Tests
```bash
dotnet test TestDBTools/TestDBTools.csproj
```

### Example Test Cases
- **Validation Tests**:
  - Test that invalid table names throw appropriate exceptions.
  - Test that field lists with dangerous characters are rejected.
- **Query Utility Tests**:
  - Test that `QueryBuilder` generates correct SQL queries.
  - Test that parameterized queries are built securely.
- **Data Manipulation Tests**:
  - Test that `Insert`, `Update`, and `Delete` methods work as expected.
  - Test that invalid conditions are rejected.

---

## Related Files
- `Utils.cs`: The file being tested.
- `UtilsControllerTests.cs`: Contains tests for the controller that uses utilities from `Utils.cs`.