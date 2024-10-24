using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Domain.Messages;
using Message = SavedMessages.Domain.Messages.Message;

namespace SavedMessages.UnitTests.MessageHandlersTests
{
    [TestFixture]
    public class GetMessagesQueryHandlerTests
    {
        private Mock<IMessageRepository> _messageRepositoryMock;
        private GetMessagesQueryHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _messageRepositoryMock= new Mock<IMessageRepository>();
            _handler = new GetMessagesQueryHandler(_messageRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_NoSearchTerm_ReturnsPaginatedMessages()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 1;
            var pageSize = 2;
            string? searchTerm = null;
            var request = new GetMessagesQuery(userId, searchTerm, page, pageSize);

            var messages = new List<Message>
            {
                Message.Create(request.UserId, "Message 1").Value,
                Message.Create(request.UserId, "Message 2").Value
            };

            var mockMessages = messages.AsQueryable().BuildMockDbSet();

            _messageRepositoryMock
                .Setup(repo => repo.GetMessages())
                .Returns(mockMessages.Object);

            var messageFileContentTest = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "text/plain";
            var messageFileName = "TestFile.txt";
            foreach (var message in messages)
            {
                var messageFile = MessageFile.Create(message.Id,
                    messageFileContentTest,
                    contentType,
                    messageFileName).Value;
                message.AttachFile(messageFile);
            }
            
            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual("Message 2", result.Value[0].Text);
            Assert.AreEqual("Message 1", result.Value[1].Text);
        }

        [Test]
        public async Task Handle_WithSearchTerm_ReturnsFilteredMessages()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 1;
            var pageSize = 2;
            string? searchTerm = "important";
            var request = new GetMessagesQuery(userId, searchTerm, page, pageSize);

            var messages = new List<Message>
            {
                Message.Create(request.UserId, "This is an important message").Value,
                Message.Create(request.UserId, "Just another message").Value
            };

            var mockMessages = messages.AsQueryable().BuildMockDbSet();

            _messageRepositoryMock
                .Setup(repo => repo.GetMessages())
                .Returns(mockMessages.Object);

            var messageFileContentTest = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "text/plain";
            var messageFileName = "TestFile.txt";
            foreach (var message in messages)
            {
                var messageFile = MessageFile.Create(message.Id,
                    messageFileContentTest,
                    contentType,
                    messageFileName).Value;
                message.AttachFile(messageFile);
            }

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual(messages[0].Text, result.Value[0].Text);
        }

        [Test]
        public async Task Handle_MessagesWithFiles_ReturnsMessagesWithFileDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var page = 1;
            var pageSize = 2;
            string? searchTerm = null;
            var request = new GetMessagesQuery(userId, searchTerm, page, pageSize);

            var messageTextTest = "Message with file";
            var messageWithFile = Message.Create(request.UserId, messageTextTest).Value;

            var messageFileContentTest = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "text/plain";
            var messageFileName = "TestFile.txt";
            var messageFile = MessageFile.Create(
                messageWithFile.Id,
                messageFileContentTest,
                contentType,
                messageFileName).Value;
            messageWithFile.AttachFile(messageFile);

            var messages = new List<Message> { messageWithFile };

            _messageRepositoryMock
                .Setup(repo => repo.GetMessages())
                .Returns(messages.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count);
            Assert.IsNotNull(result.Value[0].File);
            Assert.AreEqual(messageWithFile.File.FileName, result.Value[0].File.FileName);
            Assert.AreEqual(messageWithFile.File.ContentType, result.Value[0].File.ContentType);
            Assert.AreEqual(messageWithFile.File.Data, result.Value[0].File.FileData);
        }
    }
}
