using Moq;
using NUnit.Framework;
using SavedMessages.Application.Users;
using SavedMessages.Application.Users.RegisterUser;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.UserHandlersTests
{
    [TestFixture]
    public class RegisterUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private RegisterUserCommandHandler _handler;
        private Mock<User> _userMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _userMock = new Mock<User>();
            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object);
        }

        [Test]
        public async Task Handle_UserRegisteredSuccessfully_ReturnsSuccess()
        {
            //Arrange
            var email = "test@gmail.com";
            var fistName = "firstNameTest";
            var lastName = "lastNameTest";
            var password = "passwordTest";
            var passwordHash = "passwordHashTest";

            var command = new RegisterUserCommand(
                email,
                fistName,
                lastName,
                password);

            _userRepositoryMock.Setup(repo => repo
                .IsExistsByEmail(command.Email))
                    .ReturnsAsync(false);
            _passwordHasherMock.Setup(hasher => hasher
                .Hash(command.Password))
                    .Returns(passwordHash);
            _userRepositoryMock.Setup(repo => repo.Add(It.IsAny<User>()))
                .Verifiable();
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task Handle_EmailAlreadyInUse_ReturnsFailure()
        {
            //Arrange
            var email = "test@gmail.com";
            var fistName = "firstNameTest";
            var lastName = "lastNameTest";
            var password = "passwordTest";

            var command = new RegisterUserCommand(
                email,
                fistName,
                lastName,
                password);

            _userRepositoryMock.Setup(repo => repo
                .IsExistsByEmail(command.Email))
                    .ReturnsAsync(true);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(UserErrors.EmailAlreadyInUse.Type, result.Error.Type);
            Assert.AreEqual(UserErrors.EmailAlreadyInUse.Description, result.Error.Description);
            Assert.AreEqual(UserErrors.EmailAlreadyInUse.Code, result.Error.Code);
        }

        //[Test]
        //public async Task Handle_UserCreationFails_ReturnFailure()
        //{
        //    //Arrange
        //    var email = "test@gmail.com";
        //    var fistName = "firstNameTest";
        //    var lastName = "lastNameTest";
        //    var password = "passwordTest";
        //    var passwordHash = "passwordHashTest";

        //    var command = new RegisterUserCommand(
        //        email,
        //        fistName,
        //        lastName,
        //        password);

        //    _userRepositoryMock.Setup(repo => repo
        //        .IsExistsByEmail(command.Email))
        //            .ReturnsAsync(false);
        //    _passwordHasherMock.Setup(hasher => hasher
        //        .Hash(command.Password))
        //            .Returns(passwordHash);


        //    User.Create(command.Email, command.FirstName, command.LastName, passwordHash)
        //        .Returns(Result.Failure<User>(Error.Validation(
        //        "User.InvalidCredentials",
        //        "The provided credentials are invalid")));
        //}
    }
}
