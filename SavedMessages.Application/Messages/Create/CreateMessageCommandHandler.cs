using MediatR;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Create
{
    internal sealed class CreateMessageCommandHandler
        : IRequestHandler<CreateMessageCommand, Result>
    {
        private readonly IMessageRepository _messageRepository;

        public CreateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result> Handle(CreateMessageCommand command, CancellationToken cancellationToken) 
        {
            Result<Message> messageResult = Message.Create(
                command.UserId,
                command.Text);

            if (messageResult.IsFailure) 
            {
                return Result.Failure(messageResult.Error);
            }

            _messageRepository.Add(messageResult.Value);

            return Result.Success();
        }
    }
}
