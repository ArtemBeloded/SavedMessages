using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SavedMessages.Application.Messages.Create;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.MessageHandlersTests
{
    [TestFixture]
    public class CreateMessageCommandHandlerTests
    {
        private Mock<IMessageRepository> _messageRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private CreateMessageCommandHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _messageRepositoryMock= new Mock<IMessageRepository>();
            _userRepositoryMock= new Mock<IUserRepository>();
            _handler = new CreateMessageCommandHandler(
                _messageRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ValidCommand_CreatesMessageSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var command = new CreateMessageCommand(userId, messageTextTest, FileData: null);

            _userRepositoryMock.Setup(repo => repo.IsExistsById(userId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _messageRepositoryMock.Verify(repo => repo.Add(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public async Task Handle_EmptyTextAndFileData_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateMessageCommand(userId, Text: "", FileData: null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(MessageErrors.IvalidMessageData.Type, result.Error.Type);
            Assert.AreEqual(MessageErrors.IvalidMessageData.Description, result.Error.Description);
            Assert.AreEqual(MessageErrors.IvalidMessageData.Code, result.Error.Code);
            _messageRepositoryMock.Verify(repo => repo.Add(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var command = new CreateMessageCommand(userId, messageTextTest, FileData: null);

            _userRepositoryMock.Setup(repo => repo.IsExistsById(userId)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(UserErrors.NotFoundById(command.UserId).Type, result.Error.Type);
            Assert.AreEqual(UserErrors.NotFoundById(command.UserId).Description, result.Error.Description);
            Assert.AreEqual(UserErrors.NotFoundById(command.UserId).Code, result.Error.Code);
            _messageRepositoryMock.Verify(repo => repo.Add(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task Handle_ValidCommandWithFile_CreatesMessageWithFileSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var fileDataMock = new Mock<IFormFile>();
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("This is a test file content."));
            fileDataMock.Setup(f => f.OpenReadStream()).Returns(fileContent);
            fileDataMock.Setup(f => f.FileName).Returns("testFile.txt");
            fileDataMock.Setup(f => f.ContentType).Returns("text/plain");

            var command = new CreateMessageCommand(userId, messageTextTest, fileDataMock.Object);

            _userRepositoryMock.Setup(repo => repo.IsExistsById(userId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _messageRepositoryMock.Verify(repo => repo.Add(It.IsAny<Message>()), Times.Once);
        }
    }
}
