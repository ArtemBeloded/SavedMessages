using Moq;
using NUnit.Framework;
using SavedMessages.Application.Users.GetUser;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.UserHandlersTests
{
    [TestFixture]
    public class GetUserQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private GetUserQueryHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _userRepositoryMock= new Mock<IUserRepository>();
            _handler = new GetUserQueryHandler(_userRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_UserFound_ReturnsUserResponse() 
        {
            //Arrange
            var email = "test@gmail.com";
            var firstName = "firstNameTest";
            var lastName = "lastNameTest";
            var passwordHash = "passwordHashTest";
            var user = User.Create(email, firstName, lastName, passwordHash);

            _userRepositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync(user.Value);

            var query = new GetUserQuery(email);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            var userResponse = result.Value;
            Assert.AreEqual(user.Value.Id, userResponse.Id);
            Assert.AreEqual(user.Value.FirstName, userResponse.FirstName);
            Assert.AreEqual(user.Value.LastName, userResponse.LastName);
        }

        [Test]
        public async Task Handle_UserNotFound_ReturnsFailure() 
        {
            //Arrange
            var email = "notfound@gmail.com";
            _userRepositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync((User)null);

            var query = new GetUserQuery(email);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Type, result.Error.Type);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Description, result.Error.Description);
            Assert.AreEqual(UserErrors.NotFoundByEmail(email).Code, result.Error.Code);
            Assert.IsTrue(result.IsFailure);
        }
    }
}
