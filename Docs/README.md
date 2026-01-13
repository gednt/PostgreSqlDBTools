# PostgreSqlDBTools

## Overview
PostgreSqlDBTools is a .NET 8 project designed to provide tools for interacting with PostgreSQL databases. It includes utilities for database management, data export, and security features. The project is divided into two main components:

1. **DBTools**: Contains the core functionality for database operations.
2. **TestDBTools**: Contains unit tests for the core functionality.

---

## Project Structure

### DBTools

| File | Description |
|------|-------------|
| `DMF_DBTools.cs` | Core database management functionality. |
| `Model/GenericObject_Simple.cs` | Simplified model for generic objects. |
| `Utils.cs` | Utility functions for database operations, including validation, query building, and data manipulation. |
| `Properties/AssemblyInfo.cs` | Assembly metadata. |
| `Model/GenericObject.cs` | Model for generic objects. |
| `DataExport.cs` | Handles data export operations. |
| `UtilsController.cs` | Controller for utility functions, providing LINQ-style queries and CRUD operations. |

### TestDBTools

| File | Description |
|------|-------------|
| `DataExportTests.cs` | Unit tests for `DataExport.cs`. |
| `ModelTests.cs` | Unit tests for models. |
| `UtilsControllerTests.cs` | Unit tests for `UtilsController.cs`. |
| `UtilsTests.cs` | Unit tests for `Utils.cs`. |
| `Properties/AssemblyInfo.cs` | Assembly metadata for the test project. |

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL database

### Build and Run
1. Clone the repository:
   ```bash
   git clone https://github.com/gednt/PostgreSqlDBTools.git
   ```
2. Navigate to the project directory:
   ```bash
   cd PostgreSqlDBTools
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run the tests:
   ```bash
   dotnet test
   ```

---

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

---

## License
This project is licensed under the MIT License. See the LICENSE file for details.