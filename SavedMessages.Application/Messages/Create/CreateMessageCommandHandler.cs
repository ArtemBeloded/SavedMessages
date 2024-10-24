using MediatR;
using SavedMessages.Application.Helpers;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Messages.Create
{
    public sealed class CreateMessageCommandHandler
        : IRequestHandler<CreateMessageCommand, Result>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public CreateMessageCommandHandler(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(CreateMessageCommand command, CancellationToken cancellationToken) 
        {
            if (string.IsNullOrWhiteSpace(command.Text) && command.FileData is null) 
            {
                return Result.Failure(MessageErrors.IvalidMessageData);
            }

            if (!await _userRepository.IsExistsById(command.UserId)) 
            {
                return Result.Failure(UserErrors.NotFoundById(command.UserId));
            }

            Result<Message> messageResult = Message.Create(
                command.UserId,
                command.Text);

            if (messageResult.IsFailure) 
            {
                return Result.Failure(messageResult.Error);
            }

            var message = messageResult.Value;

            if (command.FileData is not null) 
            {
                Result<MessageFile> messageFileResult = MessageFile.Create(
                    message.Id,
                    MessageFileDataConverter.ConvertMessageFileToByteArray(command.FileData),
                    command.FileData.ContentType,
                    command.FileData.FileName);

                if (messageFileResult.IsFailure) 
                {
                    return Result.Failure(messageFileResult.Error);
                }

                var result = message.AttachFile(messageFileResult.Value);

                if (result.IsFailure) 
                {
                    return Result.Failure(messageResult.Error);
                }
            }

            _messageRepository.Add(message);

            return Result.Success();
        }
    }
}
