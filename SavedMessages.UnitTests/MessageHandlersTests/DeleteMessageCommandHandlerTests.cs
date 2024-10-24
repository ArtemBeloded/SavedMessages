using Moq;
using NUnit.Framework;
using SavedMessages.Application.Messages.Delete;
using SavedMessages.Domain.Messages;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.UnitTests.MessageHandlersTests
{
    [TestFixture]
    public class DeleteMessageCommandHandlerTests
    {
        private Mock<IMessageRepository> _messageRepositoryMock;
        private DeleteMessageCommandHandler _handler;

        [SetUp]
        public void SetUp() 
        {
            _messageRepositoryMock = new Mock<IMessageRepository>();
            _handler = new DeleteMessageCommandHandler(_messageRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_MessageNotFound_ReturnsFailure() 
        {
            //Arrange
            var messageId = Guid.NewGuid();
            var command = new DeleteMessageCommand(messageId);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(command.MessageId))
                .ReturnsAsync((Message)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(command.MessageId).Type, result.Error.Type);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(command.MessageId).Description, result.Error.Description);
            Assert.AreEqual(MessageErrors.NotFoundMessageById(command.MessageId).Code, result.Error.Code);
        }

        [Test]
        public async Task Handle_ValidRequest_RemovesMessageAndReturnsSuccess() 
        {
            //Arrange
            var messageId = Guid.NewGuid();
            var messageTextTest = "messageTextTest";
            var message = Message.Create(messageId, messageTextTest).Value;

            var command = new DeleteMessageCommand(message.Id);

            _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(command.MessageId))
                .ReturnsAsync(message);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            _messageRepositoryMock.Verify(repo => repo.Remove(message), Times.Once);
        }
    }
}
