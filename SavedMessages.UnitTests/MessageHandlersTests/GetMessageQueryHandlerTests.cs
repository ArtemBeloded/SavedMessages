using Moq;
using NUnit.Framework;
using SavedMessages.Application.Messages.GetById;
using SavedMessages.Domain.Messages;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.MessageHandlersTests
{
    [TestFixture]
    public class GetMessageQueryHandlerTests
    {
        private Mock<IMessageRepository> _messageRepositoryMock;
        private GetMessageQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock = new Mock<IMessageRepository>();
            _handler = new GetMessageQueryHandler(_messageRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_MessageNotFound_ReturnsFailureResult() 
        {
            //Arrange
            var messageId = Guid.NewGuid();
            var query = new GetMessageQuery(messageId);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync((Message)null);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(query.Id).Type, result.Error.Type);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(query.Id).Description, result.Error.Description);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(query.Id).Code, result.Error.Code);
        }

        [Test]
        public async Task Handle_MessageFoundWithOutFile_ReturnsMessageResponse() 
        {
            //Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var query = new GetMessageQuery(messageId);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync(message);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(message.Id, result.Value.Id);
            Assert.AreEqual(message.Text, result.Value.Text);
            Assert.AreEqual(message.IsEdited, result.Value.IsEdited);
            Assert.IsNull(result.Value.File);
            Assert.AreEqual(message.CreatedInUtc, result.Value.CreatedInUtc);
        }

        [Test]
        public async Task Handle_MessageFoundWithFile_ReturnsMessageWithFileResponse() 
        {
            //Arrange
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

            var query = new GetMessageQuery(messageId);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync(message);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(message.Id, result.Value.Id);
            Assert.AreEqual(message.Text, result.Value.Text);
            Assert.AreEqual(message.IsEdited, result.Value.IsEdited);
            Assert.AreEqual(message.CreatedInUtc, result.Value.CreatedInUtc);
            Assert.IsNotNull(result.Value.File);
            Assert.AreEqual(messageFile.FileName, result.Value.File.FileName);
            Assert.AreEqual(messageFile.Data, result.Value.File.FileData);
            Assert.AreEqual(messageFile.ContentType, result.Value.File.ContentType);
        }
    }
}
