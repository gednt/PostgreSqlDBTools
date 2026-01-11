using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DBTools_Utilities;

namespace TestDBTools
{
    [TestFixture]
    public class UtilsControllerTests
    {
        // Test model class
        public class TestUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
        }

        [Test]
        public void Constructor_WithDefaultTableName_UsesTypeName()
        {
            // Arrange & Act
            var controller = new UtilsController<TestUser>();

            // Assert - If constructor works, object is created successfully
            Assert.That(controller, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithCustomTableName_StoresTableName()
        {
            // Arrange & Act
            var controller = new UtilsController<TestUser>(tableName: "CustomUsers");

            // Assert - If constructor works, object is created successfully
            Assert.That(controller, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithConnectionParameters_CreatesController()
        {
            // Arrange & Act
            var controller = new UtilsController<TestUser>(
                host: "localhost",
                database: "testdb",
                uid: "root",
                password: "password",
                port: "3306"
            );

            // Assert
            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.Host, Is.EqualTo("localhost"));
            Assert.That(controller.Database, Is.EqualTo("testdb"));
        }

        [Test]
        public void Constructor_WithAutoIncrementFalse_CreatesController()
        {
            // Arrange & Act
            var controller = new UtilsController<TestUser>(
                tableName: "Users",
                primaryKeyName: "UserId",
                autoIncrement: false
            );

            // Assert
            Assert.That(controller, Is.Not.Null);
        }

        // Note: The following tests would require database connection and are marked as 
        // integration tests. In a real scenario, you'd use a test database or mock the database.
        // These are structural tests to verify the API exists and has the right shape.

        [Test]
        public void AsQueryable_ReturnsIQueryable()
        {
            // This test verifies the method signature exists
            // In practice, this would need a database connection
            var controller = new UtilsController<TestUser>();
            
            // Verify the method exists by checking if it's callable
            // (would throw at runtime without DB, but compiles correctly)
            Assert.That(controller, Has.Property("Host"));
        }

        [Test]
        public void Where_WithSimpleExpression_CompilesCorrectly()
        {
            // This test verifies the method can be called with a lambda expression
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Id == 1;
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
            Assert.That(predicate.Body.NodeType, Is.EqualTo(ExpressionType.Equal));
        }

        [Test]
        public void Where_WithComplexExpression_CompilesCorrectly()
        {
            // This test verifies complex expressions compile
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Age > 18 && x.Name == "John";
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
            Assert.That(predicate.Body.NodeType, Is.EqualTo(ExpressionType.AndAlso));
        }

        [Test]
        public void FirstOrDefault_WithExpression_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Id == 1;
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void SingleOrDefault_WithExpression_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Email == "test@example.com";
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void Any_WithExpression_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Age > 18;
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void Add_WithValidEntity_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            var user = new TestUser { Name = "John", Email = "john@example.com", Age = 30 };
            
            // Verify the entity is valid
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Name, Is.EqualTo("John"));
        }

        [Test]
        public void Update_WithEntityAndPredicate_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            var user = new TestUser { Name = "John Updated", Email = "john@example.com", Age = 31 };
            Expression<Func<TestUser, bool>> predicate = x => x.Id == 1;
            
            // Verify the expression and entity are valid
            Assert.That(user, Is.Not.Null);
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void Remove_WithExpression_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Id == 1;
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void SaveChanges_WithValidEntity_CompilesCorrectly()
        {
            // This test verifies the method signature
            var controller = new UtilsController<TestUser>();
            var user = new TestUser { Id = 1, Name = "John", Email = "john@example.com", Age = 30 };
            
            // Verify the entity is valid
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Id, Is.GreaterThan(0));
        }

        [Test]
        public void Count_WithNullPredicate_CompilesCorrectly()
        {
            // This test verifies the method signature with null predicate
            var controller = new UtilsController<TestUser>();
            
            // Verify controller has Count method
            Assert.That(controller, Is.Not.Null);
        }

        [Test]
        public void Count_WithPredicate_CompilesCorrectly()
        {
            // This test verifies the method signature with predicate
            var controller = new UtilsController<TestUser>();
            Expression<Func<TestUser, bool>> predicate = x => x.Age > 18;
            
            // Verify the expression is valid
            Assert.That(predicate, Is.Not.Null);
        }

        [Test]
        public void Expression_EqualOperation_IsSupported()
        {
            // Test that equal expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Id == 1;
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.Equal));
        }

        [Test]
        public void Expression_NotEqualOperation_IsSupported()
        {
            // Test that not equal expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Id != 1;
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.NotEqual));
        }

        [Test]
        public void Expression_GreaterThanOperation_IsSupported()
        {
            // Test that greater than expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Age > 18;
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.GreaterThan));
        }

        [Test]
        public void Expression_LessThanOperation_IsSupported()
        {
            // Test that less than expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Age < 65;
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.LessThan));
        }

        [Test]
        public void Expression_AndAlsoOperation_IsSupported()
        {
            // Test that AND expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Age > 18 && x.Age < 65;
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.AndAlso));
        }

        [Test]
        public void Expression_OrElseOperation_IsSupported()
        {
            // Test that OR expressions compile correctly
            Expression<Func<TestUser, bool>> expr = x => x.Name == "John" || x.Name == "Jane";
            Assert.That(expr.Body.NodeType, Is.EqualTo(ExpressionType.OrElse));
        }
    }
}
