using NUnit.Framework;
using System;
using DBTools_Utilities;

namespace TestDBTools
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void SelectQuery_ValidParameters_ReturnsCorrectQuery()
        {
            // Arrange
            string fields = "id,name,email";
            string table = "users";
            string conditions = "id = @whereParam0";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT id,name,email FROM users WHERE id = @whereParam0"));
        }

        [Test]
        public void SelectQuery_NoConditions_ReturnsQueryWithoutWhere()
        {
            // Arrange
            string fields = "*";
            string table = "users";
            string conditions = "";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT * FROM users"));
        }

        [Test]
        public void SelectQuery_InvalidTableName_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = "users; DROP TABLE users--";
            string conditions = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_SqlInjectionInCondition_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = "users";
            string conditions = "id = 1 OR 1=1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_CommentInjectionAttempt_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = "users";
            string conditions = "id = 1 -- comment";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void InsertQuery_ValidParameters_ReturnsParameterizedQuery()
        {
            // Arrange
            string[] fields = { "name", "email", "age" };
            string table = "users";
            string[] values = { "John", "john@example.com", "30" };

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            Assert.That(result, Is.EqualTo("INSERT INTO users(name,email,age) VALUES(@param0,@param1,@param2)"));
        }

        [Test]
        public void InsertQuery_InvalidTableName_ThrowsException()
        {
            // Arrange
            string[] fields = { "name" };
            string table = "users; DROP TABLE users--";
            string[] values = { "John" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void InsertQuery_InvalidFieldName_ThrowsException()
        {
            // Arrange
            string[] fields = { "name; DROP TABLE users--" };
            string table = "users";
            string[] values = { "John" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void UpdateQuery_ValidParameters_ReturnsParameterizedQuery()
        {
            // Arrange
            string[] fields = { "name", "email" };
            string table = "users";
            string[] values = { "John", "john@example.com" };
            string condition = "id = @whereParam0";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.That(result, Is.EqualTo("UPDATE users SET name=@setParam0,email=@setParam1 WHERE id = @whereParam0"));
        }

        [Test]
        public void UpdateQuery_NoCondition_ReturnsQueryWithoutWhere()
        {
            // Arrange
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John" };
            string condition = "";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.That(result, Is.EqualTo("UPDATE users SET name=@setParam0"));
        }

        [Test]
        public void UpdateQuery_InvalidTableName_ThrowsException()
        {
            // Arrange
            string[] fields = { "name" };
            string table = "users; DROP TABLE users--";
            string[] values = { "John" };
            string condition = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }

        [Test]
        public void DeleteQuery_ValidParameters_ReturnsQuery()
        {
            // Arrange
            string table = "users";
            string condition = "id = @whereParam0";

            // Act
            string result = Utils.DeleteQuery(table, condition);

            // Assert
            Assert.That(result, Is.EqualTo("DELETE FROM users WHERE id = @whereParam0"));
        }

        [Test]
        public void DeleteQuery_InvalidTableName_ThrowsException()
        {
            // Arrange
            string table = "users; DROP TABLE users--";
            string condition = "id = @whereParam0";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.DeleteQuery(table, condition));
        }

        [Test]
        public void InsertQueryLegacy_ValidParameters_ReturnsQueryWithEmbeddedValues()
        {
            // Arrange
            string[] fields = { "name", "age" };
            string table = "users";
            string[] values = { "John", "30" };

            // Act
#pragma warning disable CS0618 // Type or member is obsolete
            string result = Utils.InsertQueryLegacy(fields, table, values);
#pragma warning restore CS0618 // Type or member is obsolete

            // Assert
            Assert.That(result, Does.Contain("INSERT INTO users"));
            Assert.That(result, Does.Contain("'John'"));
            Assert.That(result, Does.Contain("30"));
        }

        [Test]
        public void UpdateQueryLegacy_ValidParameters_ReturnsQueryWithEmbeddedValues()
        {
            // Arrange
            string[] fields = { "name", "age" };
            string table = "users";
            string[] values = { "John", "30" };
            string condition = "id = 1";

            // Act
#pragma warning disable CS0618 // Type or member is obsolete
            string result = Utils.UpdateQueryLegacy(fields, table, values, condition);
#pragma warning restore CS0618 // Type or member is obsolete

            // Assert
            Assert.That(result, Does.Contain("UPDATE  users SET"));
            Assert.That(result, Does.Contain("'John'"));
            Assert.That(result, Does.Contain("WHERE id = 1"));
        }

        [Test]
        public void SelectQuery_QualifiedFieldName_Succeeds()
        {
            // Arrange
            string fields = "users.id,users.name";
            string table = "users";
            string conditions = "";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT users.id,users.name FROM users"));
        }

        [Test]
        public void SelectQuery_BackticksInIdentifier_Succeeds()
        {
            // Arrange
            string fields = "`user_id`,`user_name`";
            string table = "`users`";
            string conditions = "";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT `user_id`,`user_name` FROM `users`"));
        }

        [Test]
        public void SelectQuery_WildcardField_Succeeds()
        {
            // Arrange
            string fields = "*";
            string table = "users";
            string conditions = "";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT * FROM users"));
        }

        [Test]
        public void SelectQuery_QuoteFollowedByOr_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = "users";
            string conditions = "name = 'test' OR active = 1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_ConditionWithParameterPlaceholder_Succeeds()
        {
            // Arrange
            string fields = "id,name";
            string table = "users";
            string conditions = "age > @whereParam0 AND status = @whereParam1";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT id,name FROM users WHERE age > @whereParam0 AND status = @whereParam1"));
        }

        [Test]
        public void SelectQuery_EmptyTableName_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = "";
            string conditions = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_NullTableName_ThrowsException()
        {
            // Arrange
            string fields = "id";
            string table = null;
            string conditions = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }
    }
}
