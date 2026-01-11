using NUnit.Framework;
using System;
using DBTools.Security;

namespace TestDBTools
{
    [TestFixture]
    public class SecurityAuthTests
    {
        private Auth _auth;

        [SetUp]
        public void SetUp()
        {
            _auth = new Auth();
        }

        [Test]
        public void ConvertToSHA256_WithSimpleString_ReturnsValidHash()
        {
            // Arrange
            string input = "password123";

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(64)); // SHA256 produces 64 hex characters
            Assert.That(hash, Does.Match("^[0-9a-f]{64}$")); // All lowercase hex
        }

        [Test]
        public void ConvertToSHA256_SameInputTwice_ReturnsSameHash()
        {
            // Arrange
            string input = "testpassword";

            // Act
            string hash1 = _auth.ConvertToSHA256(input);
            string hash2 = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA256_DifferentInputs_ReturnsDifferentHashes()
        {
            // Arrange
            string input1 = "password1";
            string input2 = "password2";

            // Act
            string hash1 = _auth.ConvertToSHA256(input1);
            string hash2 = _auth.ConvertToSHA256(input2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA256_EmptyString_ReturnsValidHash()
        {
            // Arrange
            string input = "";

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(64));
            // SHA256 of empty string is a known value
            Assert.That(hash, Is.EqualTo("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"));
        }

        [Test]
        public void ConvertToSHA256_WithSpecialCharacters_ReturnsValidHash()
        {
            // Arrange
            string input = "p@ssw0rd!#$%";

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(64));
            Assert.That(hash, Does.Match("^[0-9a-f]{64}$"));
        }

        [Test]
        public void ConvertToSHA256_WithUnicodeCharacters_ReturnsValidHash()
        {
            // Arrange
            string input = "пароль123";

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(64));
            Assert.That(hash, Does.Match("^[0-9a-f]{64}$"));
        }

        [Test]
        public void ConvertToSHA512_WithSimpleString_ReturnsValidHash()
        {
            // Arrange
            string input = "password123";

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(128)); // SHA512 produces 128 hex characters
            Assert.That(hash, Does.Match("^[0-9a-f]{128}$")); // All lowercase hex
        }

        [Test]
        public void ConvertToSHA512_SameInputTwice_ReturnsSameHash()
        {
            // Arrange
            string input = "testpassword";

            // Act
            string hash1 = _auth.ConvertToSHA512(input);
            string hash2 = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA512_DifferentInputs_ReturnsDifferentHashes()
        {
            // Arrange
            string input1 = "password1";
            string input2 = "password2";

            // Act
            string hash1 = _auth.ConvertToSHA512(input1);
            string hash2 = _auth.ConvertToSHA512(input2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA512_EmptyString_ReturnsValidHash()
        {
            // Arrange
            string input = "";

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(128));
            // SHA512 of empty string is a known value
            Assert.That(hash, Is.EqualTo("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e"));
        }

        [Test]
        public void ConvertToSHA512_WithSpecialCharacters_ReturnsValidHash()
        {
            // Arrange
            string input = "p@ssw0rd!#$%";

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(128));
            Assert.That(hash, Does.Match("^[0-9a-f]{128}$"));
        }

        [Test]
        public void ConvertToSHA512_WithUnicodeCharacters_ReturnsValidHash()
        {
            // Arrange
            string input = "пароль123";

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(128));
            Assert.That(hash, Does.Match("^[0-9a-f]{128}$"));
        }

        [Test]
        public void ConvertToSHA256_WithLongString_ReturnsValidHash()
        {
            // Arrange
            string input = new string('a', 1000);

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(64));
        }

        [Test]
        public void ConvertToSHA512_WithLongString_ReturnsValidHash()
        {
            // Arrange
            string input = new string('a', 1000);

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(128));
        }

        [Test]
        public void SHA256_And_SHA512_ProduceDifferentHashes()
        {
            // Arrange
            string input = "password123";

            // Act
            string sha256Hash = _auth.ConvertToSHA256(input);
            string sha512Hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(sha256Hash, Is.Not.EqualTo(sha512Hash));
            Assert.That(sha256Hash.Length, Is.LessThan(sha512Hash.Length));
        }

        [Test]
        public void ConvertToSHA256_CaseSensitive_ProducesDifferentHashes()
        {
            // Arrange
            string input1 = "Password";
            string input2 = "password";

            // Act
            string hash1 = _auth.ConvertToSHA256(input1);
            string hash2 = _auth.ConvertToSHA256(input2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA512_CaseSensitive_ProducesDifferentHashes()
        {
            // Arrange
            string input1 = "Password";
            string input2 = "password";

            // Act
            string hash1 = _auth.ConvertToSHA512(input1);
            string hash2 = _auth.ConvertToSHA512(input2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void ConvertToSHA256_KnownValue_ProducesExpectedHash()
        {
            // Arrange
            string input = "hello";
            // Known SHA256 hash for "hello"
            string expectedHash = "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824";

            // Act
            string hash = _auth.ConvertToSHA256(input);

            // Assert
            Assert.That(hash, Is.EqualTo(expectedHash));
        }

        [Test]
        public void ConvertToSHA512_KnownValue_ProducesExpectedHash()
        {
            // Arrange
            string input = "hello";
            // Known SHA512 hash for "hello"
            string expectedHash = "9b71d224bd62f3785d96d46ad3ea3d73319bfbc2890caadae2dff72519673ca72323c3d99ba5c11d7c7acc6e14b8c5da0c4663475c2e5c3adef46f73bcdec043";

            // Act
            string hash = _auth.ConvertToSHA512(input);

            // Assert
            Assert.That(hash, Is.EqualTo(expectedHash));
        }
    }
}
