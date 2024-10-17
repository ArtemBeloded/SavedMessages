using Moq;
using NUnit.Framework;
using SavedMessages.Application.Users;
using SavedMessages.Application.Users.LoginUser;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.UserHandlersTests
{
    [TestFixture]
    public class LoginUserQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<ITokenProvider> _tokenProviderMock;
        private LoginUserQueryHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock= new Mock<IPasswordHasher>();
            _tokenProviderMock= new Mock<ITokenProvider>();
            _handler = new LoginUserQueryHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenProviderMock.Object);
        }

        [Test]
        public async Task Handle_UserFound_PasswordVerified_ReturnToken() 
        {
            //Arrange
            var email = "test@gmail.com";
            var firstName = "firstNameTest";
            var lastName = "lastNameTest";
            var passwordHash = "passwordHashTest";
            var user = User.Create(email, firstName, lastName, passwordHash);
            var token = "tokenTest";
            var password = "passwordTest";

            _userRepositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync(user.Value);
            _passwordHasherMock.Setup(hasher => hasher.Verify(password, user.Value.PasswordHash))
                .Returns(true);
            _tokenProviderMock.Setup(tokenProvider =>
                tokenProvider.Create(user.Value))
                    .Returns(token);

            var query = new LoginUserQuery(email, password);
            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(token, result.Value.Token);
        }

        [Test]
        public async Task Handle_UserNotFound_ReturnsFailure() 
        {
            //Arrange
            var email = "test@gmail.com";
            var password = "passwordTest";
            _userRepositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync((User)null);

            var query = new LoginUserQuery(email, password);
            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Type, result.Error.Type);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Description, result.Error.Description);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Code, result.Error.Code);
            Assert.IsTrue(result.IsFailure);
        }

        [Test]
        public async Task Handle_PasswordIncorrect_ReturnsFailure() 
        {
            //Arrange
            var email = "test@gmail.com";
            var firstName = "firstNameTest";
            var lastName = "lastNameTest";
            var passwordHash = "passwordHashTest";
            var user = User.Create(email, firstName, lastName, passwordHash);
            var password = "wrongPasswordTest";

            _userRepositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync(user.Value);
            _passwordHasherMock.Setup(hasher => hasher.Verify(password, user.Value.PasswordHash))
                .Returns(false);

            var query = new LoginUserQuery(email, password);
            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(UserErrors.InvalidCredentials.Type, result.Error.Type);
            Assert.AreEqual(UserErrors.InvalidCredentials.Description, result.Error.Description);
            Assert.AreEqual(UserErrors.InvalidCredentials.Code, result.Error.Code);
        }
    }
}
