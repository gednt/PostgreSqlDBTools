# TestDBTools

Unit test project for the DBTools library.

## Test Coverage

This test project includes comprehensive unit tests for:

- **UtilsTests** (26 tests): Tests for SQL query building methods and security validation
  - Parameterized query generation (SELECT, INSERT, UPDATE, DELETE)
  - SQL injection prevention validation
  - Identifier validation (table names, field names)
  
- **UtilsControllerTests** (26 tests): Tests for LINQ-style database operations
  - Constructor and configuration tests
  - LINQ expression compilation tests
  - CRUD operation API tests
  
- **DataExportTests** (17 tests): Tests for CSV export/import functionality
  - CSV generation with various options
  - DataTable parsing from CSV
  - Special character handling
  
- **SecurityAuthTests** (18 tests): Tests for authentication/hashing utilities
  - SHA256 hash generation
  - SHA512 hash generation
  - Edge cases and known value validation
  
- **ModelTests** (9 tests): Tests for data model classes
  - GenericObject property tests
  - GenericObject_Simple property tests
  - Type handling and null value tests

**Total: 96 unit tests**

## Requirements

- .NET Framework 4.5 or higher
- NUnit 3.13.3
- Mono (for Linux/Mac) or .NET Framework (for Windows)

## Running Tests

### On Linux/Mac with Mono:

```bash
# Install dependencies
sudo apt-get install mono-complete  # Ubuntu/Debian
# or
brew install mono  # Mac with Homebrew

# Restore packages
mono nuget.exe restore DBTools.sln

# Build the solution
xbuild DBTools.sln /p:Configuration=Debug

# Install NUnit console runner
mono nuget.exe install NUnit.ConsoleRunner -Version 3.16.3 -OutputDirectory tools

# Run tests
mono tools/NUnit.ConsoleRunner.3.16.3/tools/nunit3-console.exe TestDBTools/bin/Debug/TestDBTools.dll
```

### On Windows with Visual Studio:

```bash
# Restore packages
nuget restore DBTools.sln

# Build the solution
msbuild DBTools.sln /p:Configuration=Debug

# Run tests using Test Explorer in Visual Studio
# Or use NUnit console runner:
packages\NUnit.ConsoleRunner.3.16.3\tools\nunit3-console.exe TestDBTools\bin\Debug\TestDBTools.dll
```

## Test Structure

All tests follow the Arrange-Act-Assert (AAA) pattern for clarity and consistency:

```csharp
[Test]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data
    var input = "test data";
    
    // Act - Execute the method being tested
    var result = MethodUnderTest(input);
    
    // Assert - Verify the result
    Assert.That(result, Is.EqualTo(expectedValue));
}
```

## Adding New Tests

1. Add your test method to the appropriate test class (or create a new one)
2. Use the `[Test]` attribute to mark test methods
3. Use the `[SetUp]` attribute for initialization code that runs before each test
4. Follow the existing naming convention: `MethodName_Scenario_ExpectedBehavior`
5. Rebuild the project and run the tests

## Notes

- Most UtilsController tests are structural tests that verify the API exists and compiles correctly. Full integration tests would require a test database connection.
- Tests that validate SQL injection prevention are critical for security and should not be removed or weakened.
- Legacy method tests include obsolete warnings as expected since those methods are marked as deprecated.
