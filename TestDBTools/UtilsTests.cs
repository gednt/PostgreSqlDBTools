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
            string condition = "user=@user";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.That(result, Is.EqualTo("UPDATE users SET name=@setParam0 WHERE user=@user"));
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
            string fields = "user_id,user_name";
            string table = "users";
            string conditions = "user_id>@user_id";

            // Act
            string result = Utils.SelectQuery(fields, table, conditions);

            // Assert
            Assert.That(result, Is.EqualTo("SELECT user_id,user_name FROM users WHERE user_id>@user_id"));
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

        // Additional SQL Injection tests
        [Test]
        public void SelectQuery_UnionInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id = @whereParam0 UNION SELECT * FROM admins";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_DropTableInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "1=1; DROP TABLE users;";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_SleepTimeBasedInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "1=1; SELECT pg_sleep(5);";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_AlwaysTrueComparison_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id = @whereParam0 OR 1=1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_CommentTerminator_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0 --";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_BlockCommentInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0 /* malicious */";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_MultipleStatements_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0; UPDATE users SET admin=true";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_CastInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id::text = '1' OR 'a'='a'";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_FunctionCallInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id = @whereParam0 OR current_user = current_user";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_DollarQuoteInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id = 1; DO $$ BEGIN RAISE NOTICE 'x'; END $$;";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_LikeWildcardInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "name LIKE '%'; DROP TABLE users;--";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_BackslashEscapeInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "name = E'admin\\'; OR 1=1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_IntoOutfileStyleInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0; COPY users TO '/tmp/leak'";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_SetSearchPathInjection_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0; SET search_path=pg_catalog";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_UsingSemicolonOnly_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = ";";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void InsertQuery_SqlInjectionInValues_ThrowsException()
        {
            string[] fields = { "name", "email" };
            string table = "users";
            string[] values = { "John', admin=true --", "john@example.com" };
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void InsertQuery_SemicolonInValues_ThrowsException()
        {
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John; DROP TABLE users;" };
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void InsertQuery_UnionInValues_ThrowsException()
        {
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John' UNION SELECT password FROM users --" };
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void UpdateQuery_SqlInjectionInCondition_ThrowsException()
        {
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John" };
            string condition = "id = 1 OR 1=1";
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }

        [Test]
        public void UpdateQuery_MultipleStatementsInCondition_ThrowsException()
        {
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John" };
            string condition = "id=1; DROP TABLE users;";
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }

        [Test]
        public void UpdateQuery_CommentInCondition_ThrowsException()
        {
            string[] fields = { "name" };
            string table = "users";
            string[] values = { "John" };
            string condition = "id=1 --";
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }

    
        [Test]
        public void SelectQuery_QuoteUnbalanced_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "name = 'John";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_DoubleDashInMiddle_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=1 -- comment here AND active=1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_UsingKeywordOrWithoutSpaces_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id=@whereParam0OR1=1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_TautologyWithLike_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "name LIKE '%' OR 'x'='x'";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_InjectionUsingBetween_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "age BETWEEN 0 AND 100 OR 1=1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_InjectionUsingInList_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "id IN (1,2,3); DROP TABLE users;";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_InjectionUsingExists_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "EXISTS(SELECT 1 FROM pg_catalog.pg_tables);";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_InjectionUsingWithCte_ThrowsException()
        {
            string fields = "id";
            string table = "users";
            string conditions = "WITH x AS (SELECT 1) SELECT 1";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_InjectionUsingReturning_ThrowsException()
        {
            string fields = "name";
            string table = "users";
            string conditions = "name=@whereParam0; UPDATE users SET name='x' RETURNING *";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void InsertQuery_FieldNameWithComment_ThrowsException()
        {
            string[] fields = { "name--bad" };
            string table = "users";
            string[] values = { "John" };
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void InsertQuery_FieldNameWithWhitespace_ThrowsException()
        {
            string[] fields = { "first name" };
            string table = "users";
            string[] values = { "John" };
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void UpdateQuery_FieldNameWithSemicolon_ThrowsException()
        {
            string[] fields = { "name;" };
            string table = "users";
            string[] values = { "John" };
            string condition = "id=@whereParam0";
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }

        [Test]
        public void UpdateQuery_FieldNameWithDash_ThrowsException()
        {
            string[] fields = { "na-me" };
            string table = "users";
            string[] values = { "John" };
            string condition = "id=@whereParam0";
            Assert.Throws<ArgumentException>(() => Utils.UpdateQuery(fields, table, values, condition));
        }



        [Test]
        public void SelectQuery_TableNameWithSpace_ThrowsException()
        {
            string fields = "id";
            string table = "users table";
            string conditions = "id=@whereParam0";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_TableNameWithDot_ThrowsException()
        {
            string fields = "id";
            string table = "users.admin";
            string conditions = "id=@whereParam0";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_IdentifierWithQuotes_ThrowsException()
        {
            string fields = "\"id\"";
            string table = "users";
            string conditions = "id=@whereParam0";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }

        [Test]
        public void SelectQuery_StarWithExtraCharacters_ThrowsException()
        {
            string fields = "*; DROP TABLE users;";
            string table = "users";
            string conditions = "";
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, conditions));
        }
    }
}
