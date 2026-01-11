using NUnit.Framework;
using System;
using DBTools.Model;

namespace TestDBTools
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void GenericObject_CanBeInstantiated()
        {
            // Act
            var obj = new GenericObject();

            // Assert
            Assert.That(obj, Is.Not.Null);
        }

        [Test]
        public void GenericObject_ColumnsProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject();
            string[] expectedColumns = { "Id", "Name", "Email" };

            // Act
            obj.columns = expectedColumns;

            // Assert
            Assert.That(obj.columns, Is.EqualTo(expectedColumns));
            Assert.That(obj.columns.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenericObject_ValuesProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject();
            object[] expectedValues = { 1, "John Doe", "john@example.com" };

            // Act
            obj.values = expectedValues;

            // Assert
            Assert.That(obj.values, Is.EqualTo(expectedValues));
            Assert.That(obj.values.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenericObject_ValuesStringProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject();
            string[] expectedValues = { "1", "John Doe", "john@example.com" };

            // Act
            obj.valuesString = expectedValues;

            // Assert
            Assert.That(obj.valuesString, Is.EqualTo(expectedValues));
            Assert.That(obj.valuesString.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenericObject_TypesProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject();
            string[] expectedTypes = { "Int32", "String", "String" };

            // Act
            obj.types = expectedTypes;

            // Assert
            Assert.That(obj.types, Is.EqualTo(expectedTypes));
            Assert.That(obj.types.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenericObject_AllProperties_CanBeSetTogether()
        {
            // Arrange
            var obj = new GenericObject();

            // Act
            obj.columns = new[] { "Id", "Name" };
            obj.values = new object[] { 1, "John" };
            obj.valuesString = new[] { "1", "John" };
            obj.types = new[] { "Int32", "String" };

            // Assert
            Assert.That(obj.columns, Is.Not.Null);
            Assert.That(obj.values, Is.Not.Null);
            Assert.That(obj.valuesString, Is.Not.Null);
            Assert.That(obj.types, Is.Not.Null);
            Assert.That(obj.columns.Length, Is.EqualTo(2));
            Assert.That(obj.values.Length, Is.EqualTo(2));
        }

        [Test]
        public void GenericObject_Values_CanContainDifferentTypes()
        {
            // Arrange
            var obj = new GenericObject();
            object[] mixedValues = { 1, "text", 3.14, true, DateTime.Now };

            // Act
            obj.values = mixedValues;

            // Assert
            Assert.That(obj.values[0], Is.TypeOf<int>());
            Assert.That(obj.values[1], Is.TypeOf<string>());
            Assert.That(obj.values[2], Is.TypeOf<double>());
            Assert.That(obj.values[3], Is.TypeOf<bool>());
            Assert.That(obj.values[4], Is.TypeOf<DateTime>());
        }

        [Test]
        public void GenericObject_Values_CanContainNull()
        {
            // Arrange
            var obj = new GenericObject();
            object[] valuesWithNull = { 1, null, "text" };

            // Act
            obj.values = valuesWithNull;

            // Assert
            Assert.That(obj.values[1], Is.Null);
        }

        [Test]
        public void GenericObjectSimple_CanBeInstantiated()
        {
            // Act
            var obj = new GenericObject_Simple();

            // Assert
            Assert.That(obj, Is.Not.Null);
        }

        [Test]
        public void GenericObjectSimple_ColumnProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject_Simple();
            string expectedColumn = "UserId";

            // Act
            obj.column = expectedColumn;

            // Assert
            Assert.That(obj.column, Is.EqualTo(expectedColumn));
        }

        [Test]
        public void GenericObjectSimple_ValueProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject_Simple();
            object expectedValue = 123;

            // Act
            obj.value = expectedValue;

            // Assert
            Assert.That(obj.value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void GenericObjectSimple_TypeProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var obj = new GenericObject_Simple();
            string expectedType = "Int32";

            // Act
            obj.type = expectedType;

            // Assert
            Assert.That(obj.type, Is.EqualTo(expectedType));
        }

        [Test]
        public void GenericObjectSimple_AllProperties_CanBeSetTogether()
        {
            // Arrange
            var obj = new GenericObject_Simple();

            // Act
            obj.column = "Age";
            obj.value = 30;
            obj.type = "Int32";

            // Assert
            Assert.That(obj.column, Is.EqualTo("Age"));
            Assert.That(obj.value, Is.EqualTo(30));
            Assert.That(obj.type, Is.EqualTo("Int32"));
        }

        [Test]
        public void GenericObjectSimple_ValueProperty_CanContainDifferentTypes()
        {
            // Arrange & Act
            var obj1 = new GenericObject_Simple { value = 123 };
            var obj2 = new GenericObject_Simple { value = "text" };
            var obj3 = new GenericObject_Simple { value = 3.14 };
            var obj4 = new GenericObject_Simple { value = true };

            // Assert
            Assert.That(obj1.value, Is.TypeOf<int>());
            Assert.That(obj2.value, Is.TypeOf<string>());
            Assert.That(obj3.value, Is.TypeOf<double>());
            Assert.That(obj4.value, Is.TypeOf<bool>());
        }

        [Test]
        public void GenericObjectSimple_ValueProperty_CanBeNull()
        {
            // Arrange
            var obj = new GenericObject_Simple();

            // Act
            obj.value = null;

            // Assert
            Assert.That(obj.value, Is.Null);
        }

        [Test]
        public void GenericObject_CanBeUsedInCollection()
        {
            // Arrange
            var list = new System.Collections.Generic.List<GenericObject>();

            // Act
            list.Add(new GenericObject
            {
                columns = new[] { "Id" },
                values = new object[] { 1 },
                types = new[] { "Int32" }
            });
            list.Add(new GenericObject
            {
                columns = new[] { "Name" },
                values = new object[] { "John" },
                types = new[] { "String" }
            });

            // Assert
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0].values[0], Is.EqualTo(1));
            Assert.That(list[1].values[0], Is.EqualTo("John"));
        }

        [Test]
        public void GenericObjectSimple_CanBeUsedInCollection()
        {
            // Arrange
            var list = new System.Collections.Generic.List<GenericObject_Simple>();

            // Act
            list.Add(new GenericObject_Simple { column = "Id", value = 1, type = "Int32" });
            list.Add(new GenericObject_Simple { column = "Name", value = "John", type = "String" });

            // Assert
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0].value, Is.EqualTo(1));
            Assert.That(list[1].value, Is.EqualTo("John"));
        }

        [Test]
        public void GenericObject_EmptyArrays_CanBeSet()
        {
            // Arrange
            var obj = new GenericObject();

            // Act
            obj.columns = new string[0];
            obj.values = new object[0];
            obj.types = new string[0];

            // Assert
            Assert.That(obj.columns, Is.Not.Null);
            Assert.That(obj.values, Is.Not.Null);
            Assert.That(obj.types, Is.Not.Null);
            Assert.That(obj.columns.Length, Is.EqualTo(0));
        }

        [Test]
        public void GenericObjectSimple_WithDateTime_WorksCorrectly()
        {
            // Arrange
            var obj = new GenericObject_Simple();
            var now = DateTime.Now;

            // Act
            obj.column = "CreatedDate";
            obj.value = now;
            obj.type = "DateTime";

            // Assert
            Assert.That(obj.value, Is.TypeOf<DateTime>());
            Assert.That(obj.value, Is.EqualTo(now));
        }

        [Test]
        public void GenericObject_WithDBNull_WorksCorrectly()
        {
            // Arrange
            var obj = new GenericObject();

            // Act
            obj.columns = new[] { "OptionalField" };
            obj.values = new object[] { DBNull.Value };
            obj.types = new[] { "String" };

            // Assert
            Assert.That(obj.values[0], Is.EqualTo(DBNull.Value));
        }
    }
}
