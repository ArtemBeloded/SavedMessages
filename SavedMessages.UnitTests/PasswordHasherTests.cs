using NUnit.Framework;
using SavedMessages.Persistence.Services;

namespace SavedMessages.UnitTests
{
    [TestFixture]
    public class PasswordHasherTests
    {
        private PasswordHasher _passwordHasher;

        [SetUp]
        public void SetUp()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Test]
        public void Hash_Password_ReturnsNnEmptyHash() 
        {
            //Arrange
            var password = "passwordTest";

            //Act
            string hash = _passwordHasher.Hash(password);

            //Assert
            Assert.IsNotEmpty(hash);
            Assert.IsTrue(hash.Contains("-"));
        }

        [Test]
        public void Verify_CorrectPassword_ReturnsTrue() 
        {
            //Arrange
            var password = "passwordTest";
            var hash = _passwordHasher.Hash(password);

            //Act
            var result = _passwordHasher.Verify(password, hash);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Verify_IncorrectPassword_ReturnsFasle()
        {
            //Arrange
            var password = "passwordTest";
            var wrongPassword = "wrogPasswordTest";
            var hash = _passwordHasher.Hash(password);

            //Act
            var result = _passwordHasher.Verify(wrongPassword, hash);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Hash_SamePasswordTwice_ReturnsDifferentHashes() 
        {
            //Arrange
            var password = "passwordTest";

            //Act
            var hash1 = _passwordHasher.Hash(password);
            var hash2 = _passwordHasher.Hash(password);

            //Assert
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
