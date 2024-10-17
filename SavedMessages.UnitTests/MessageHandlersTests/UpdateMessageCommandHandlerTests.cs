using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SavedMessages.Application.Messages.Update;
using SavedMessages.Domain.Messages;
using System.Text;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.MessageHandlersTests
{
    [TestFixture]
    public class UpdateMessageCommandHandlerTests
    {
        private Mock<IMessageRepository> _messageRepositoryMock;
        private Mock<IMessageFileRepository> _messageFileRepositoryMock;
        private UpdateMessageCommandHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _messageRepositoryMock= new Mock<IMessageRepository>();
            _messageFileRepositoryMock = new Mock<IMessageFileRepository>();
            _handler = new UpdateMessageCommandHandler(
                _messageRepositoryMock.Object,
                _messageFileRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_MessageNotFound_ReturnsFailure()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextMessage";
            var request = new UpdateMessageCommand(messageId, messageTextTest, FileData: null);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync((Message)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(request.Id).Type, result.Error.Type);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(request.Id).Description, result.Error.Description);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(request.Id).Code, result.Error.Code);
        }

        [Test]
        public async Task Handle_UpdateMessageText_Success()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var newMessageTextTest = "updateMessageTextTest";
            var request = new UpdateMessageCommand(messageId, newMessageTextTest, FileData: null);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(message);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(request.Text, message.Text);
            _messageRepositoryMock.Verify(repo => repo.Update(message), Times.Once);
        }

        [Test]
        public async Task Handle_RemoveFileFromMessage_Success()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var messageFileContentTest = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "text/plain";
            var messageFileName = "TestFile.txt";
            var messageFile = MessageFile.Create(
                message.Id,
                messageFileContentTest,
                contentType,
                messageFileName).Value;

            message.AttachFile(messageFile);

            var newMessageTextTest = "updateMessageTextTest";
            var request = new UpdateMessageCommand(messageId, newMessageTextTest, FileData: null);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(message);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(message.File);
            _messageFileRepositoryMock.Verify(repo => repo.Remove(messageFile), Times.Once);
            _messageRepositoryMock.Verify(repo => repo.Update(message), Times.Once);
        }

        [Test]
        public async Task Handle_UpdateFileInMessage_Success()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var messageFileContent = new byte[1];
            var messageFileType = "text/plain";
            var messageFileName = "file.txt";
            var messageFileFile = MessageFile.Create(message.Id, messageFileContent, messageFileType, messageFileName).Value;
            message.AttachFile(messageFileFile);

            var fileDataMock = new Mock<IFormFile>();
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
            fileDataMock.Setup(f => f.OpenReadStream()).Returns(fileContent);
            fileDataMock.Setup(f => f.FileName).Returns("newFile.txt");
            fileDataMock.Setup(f => f.ContentType).Returns("text/plain");

            var newMessageTextTest = "updateMessageTextTest";
            var request = new UpdateMessageCommand(messageId, newMessageTextTest, fileDataMock.Object);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id))
                .ReturnsAsync(message);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(message.File);
            Assert.AreEqual(fileDataMock.Object.FileName, message.File.FileName);
            _messageRepositoryMock.Verify(repo => repo.Update(message), Times.Once);
        }

        [Test]
        public async Task Hanlde_CreateNewFileInMessage_Success() 
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var fileDataMock = new Mock<IFormFile>();
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
            fileDataMock.Setup(f => f.OpenReadStream()).Returns(fileContent);
            fileDataMock.Setup(f => f.FileName).Returns("newFile.txt");
            fileDataMock.Setup(f => f.ContentType).Returns("text/plain");

            var newMessageTextTest = "updateMessageTextTest";
            var request = new UpdateMessageCommand(messageId, newMessageTextTest, fileDataMock.Object);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id))
               .ReturnsAsync(message);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(message.File);
            Assert.AreEqual(fileDataMock.Object.FileName, message.File.FileName);
            _messageRepositoryMock.Verify(repo => repo.Update(message), Times.Once);
        }
    }
}
