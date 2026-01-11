using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using DBTools_Utilities;
using DBTools.Model;

namespace TestDBTools
{
    [TestFixture]
    public class DataExportTests
    {
        private DataExport _dataExport;

        [SetUp]
        public void SetUp()
        {
            _dataExport = new DataExport();
        }

        [Test]
        public void ToCsv_WithValidData_ReturnsCsvString()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Name", "Email" },
                    types = new[] { "Int32", "String", "String" },
                    values = new object[] { 1, "John Doe", "john@example.com" }
                },
                new GenericObject
                {
                    columns = new[] { "Id", "Name", "Email" },
                    types = new[] { "Int32", "String", "String" },
                    values = new object[] { 2, "Jane Smith", "jane@example.com" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: true);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain("Int32,String,String"));
            Assert.That(result, Does.Contain("Id,Name,Email"));
            Assert.That(result, Does.Contain("1,John Doe,john@example.com"));
            Assert.That(result, Does.Contain("2,Jane Smith,jane@example.com"));
        }

        [Test]
        public void ToCsv_WithoutColumnNames_ReturnsDataOnly()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Name" },
                    types = new[] { "Int32", "String" },
                    values = new object[] { 1, "John" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Not.Contain("Id,Name"));
            Assert.That(result, Does.Not.Contain("Int32,String"));
            Assert.That(result, Does.Contain("1,John"));
        }

        [Test]
        public void ToCsv_WithoutTypes_ReturnsColumnsAndData()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Name" },
                    types = new[] { "Int32", "String" },
                    values = new object[] { 1, "John" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.Contain("Id,Name"));
            Assert.That(result, Does.Not.Contain("Int32,String"));
            Assert.That(result, Does.Contain("1,John"));
        }

        [Test]
        public void ToCsv_WithSemicolonSeparator_UsesSemicolon()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Name" },
                    types = new[] { "Int32", "String" },
                    values = new object[] { 1, "John" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ';', showColums: true, showTypes: false);

            // Assert
            Assert.That(result, Does.Contain("Id;Name"));
            Assert.That(result, Does.Contain("1;John"));
        }

        [Test]
        public void ToCsv_WithDBNullValue_ReturnsEmptyField()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Name", "Email" },
                    types = new[] { "Int32", "String", "String" },
                    values = new object[] { 1, "John", DBNull.Value }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: false);

            // Assert
            Assert.That(result, Does.Contain("1,John,"));
        }

        [Test]
        public void ToCsv_WithBackslashInData_ReplacesWithForwardSlash()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Path" },
                    types = new[] { "String" },
                    values = new object[] { @"C:\Users\John" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            Assert.That(result, Does.Contain("C:/Users/John"));
            Assert.That(result, Does.Not.Contain(@"\"));
        }

        [Test]
        public void ToCsv_WithNewlineInData_RemovesNewline()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Description" },
                    types = new[] { "String" },
                    values = new object[] { "Line1\r\nLine2" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            Assert.That(result, Does.Not.Contain("\r\n"));
            Assert.That(result, Does.Contain("Line1Line2"));
        }

        [Test]
        public void ToDataTable_WithValidCsv_ReturnsDataTable()
        {
            // Arrange
            string csv = "Id,Name,Email\n1,John,john@example.com\n2,Jane,jane@example.com";

            // Act
            DataTable result = _dataExport.ToDataTable(csv, ',', specifyColumnTypes: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Rows.Count, Is.GreaterThan(0));
            Assert.That(result.Columns.Count, Is.EqualTo(3));
        }

        [Test]
        public void ToDataTable_WithTypesSpecified_ParsesTypes()
        {
            // Arrange
            string csv = "Int32,String,String\nId,Name,Email\n1,John,john@example.com";

            // Act
            DataTable result = _dataExport.ToDataTable(csv, ',', specifyColumnTypes: true);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Columns.Count, Is.EqualTo(3));
        }

        [Test]
        public void ToDataTable_WithSemicolonSeparator_ParsesCorrectly()
        {
            // Arrange
            string csv = "Id;Name;Email\n1;John;john@example.com";

            // Act
            DataTable result = _dataExport.ToDataTable(csv, ';', specifyColumnTypes: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Columns.Count, Is.EqualTo(3));
        }

        [Test]
        public void ToDataTable_WithEmptyCsv_ReturnsEmptyDataTable()
        {
            // Arrange
            string csv = null;

            // Act
            DataTable result = _dataExport.ToDataTable(csv, ',', specifyColumnTypes: false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Rows.Count, Is.EqualTo(0));
        }

        [Test]
        public void ToCsv_MultipleRows_FormatsCorrectly()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Value" },
                    types = new[] { "Int32", "Int32" },
                    values = new object[] { 1, 100 }
                },
                new GenericObject
                {
                    columns = new[] { "Id", "Value" },
                    types = new[] { "Int32", "Int32" },
                    values = new object[] { 2, 200 }
                },
                new GenericObject
                {
                    columns = new[] { "Id", "Value" },
                    types = new[] { "Int32", "Int32" },
                    values = new object[] { 3, 300 }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            Assert.That(result, Does.Contain("1,100"));
            Assert.That(result, Does.Contain("2,200"));
            Assert.That(result, Does.Contain("3,300"));
            
            // Check that we have the right number of lines (3 data rows)
            string[] lines = result.Split('\n');
            Assert.That(lines.Length, Is.EqualTo(3));
        }

        [Test]
        public void ToCsv_WithAllOptions_IncludesAllInformation()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new[] { "Id", "Active" },
                    types = new[] { "Int32", "Boolean" },
                    values = new object[] { 1, true }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: true);

            // Assert
            string[] lines = result.Split('\n');
            Assert.That(lines.Length, Is.EqualTo(3)); // types, columns, data
            Assert.That(lines[0], Is.EqualTo("Int32,Boolean"));
            Assert.That(lines[1], Is.EqualTo("Id,Active"));
            Assert.That(lines[2], Does.Contain("1"));
            Assert.That(lines[2], Does.Contain("True"));
        }
    }
}
