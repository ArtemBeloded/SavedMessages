using MediatR;
using SavedMessages.Application.Helpers;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Messages.Update
{
    public sealed class UpdateMessageCommandHandler
        : IRequestHandler<UpdateMessageCommand, Result>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageFileRepository _messageFileRepository;
        private const int TimeOfUpdateOpportunityInMinutas = 15;

        public UpdateMessageCommandHandler(
            IMessageRepository messageRepository,
            IMessageFileRepository messageFileRepository)
        {
            _messageRepository = messageRepository;
            _messageFileRepository = messageFileRepository;
        }

        public async Task<Result> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.Id);

            if(message is null) 
            {
                return Result.Failure<MessagesResponse>(MessageErrors.NotFoundMessageById(request.Id));
            }

            var dateTimeNow = DateTime.Now;
            TimeSpan difference = dateTimeNow - message.CreatedInUtc;

            if (difference.TotalMinutes > TimeOfUpdateOpportunityInMinutas) 
            {
                return Result.Failure(MessageErrors.ExpiredTimeToUpdateDataMessage(message.Id));
            }

            var result = message.Update(request.Text);

            if (result.IsFailure) 
            {
                return Result.Failure(result.Error);
            }

            if (message.File is not null)
            {
                if (request.FileData is not null)
                {
                    var resultFileUpdate = message.File.Update(
                        MessageFileDataConverter.ConvertMessageFileToByteArray(request.FileData),
                        request.FileData.ContentType,
                        request.FileData.FileName);

                    if (resultFileUpdate.IsFailure)
                    {
                        return Result.Failure(resultFileUpdate.Error);
                    }
                }
                else
                {
                    _messageFileRepository.Remove(message.File);

                    var resultFileRemove = message.RemoveMessageFile();

                    if (resultFileRemove.IsFailure)
                    {
                        return Result.Failure(resultFileRemove.Error);
                    }
                }
            }
            else if (request.FileData is not null)
            {
                var messageFileCreateResult = MessageFile.Create(
                    message.Id,
                    MessageFileDataConverter.ConvertMessageFileToByteArray(request.FileData),
                    request.FileData.ContentType,
                    request.FileData.FileName);

                if (messageFileCreateResult.IsFailure)
                {
                    return Result.Failure(messageFileCreateResult.Error);
                }

                _messageFileRepository.Add(messageFileCreateResult.Value);

                var resultAttachFileToMessage = message.AttachFile(messageFileCreateResult.Value);

                if (resultAttachFileToMessage.IsFailure)
                {
                    return Result.Failure(resultAttachFileToMessage.Error);
                }
            }

            _messageRepository.Update(message);

            return Result.Success();
        }
    }
}
