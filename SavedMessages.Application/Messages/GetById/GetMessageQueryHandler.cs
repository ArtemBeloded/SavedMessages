using MediatR;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Messages.GetById
{
    internal sealed class GetMessageQueryHandler
        : IRequestHandler<GetMessageQuery, Result<MessagesResponse>>
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessageQueryHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result<MessagesResponse>> Handle(GetMessageQuery request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.Id);

            if (message is null) 
            {
                return Result.Failure<MessagesResponse>(MessageErrors.NotFoundMessageById(request.Id));
            }

            var messageResponse = new MessagesResponse(
                message.Id,
                message.CreatedInUtc,
                message.Text,
                message.IsEdited,
                message.File != null 
                ? new MessageFileResponse(
                    message.File.FileName,
                    message.File.Data,
                    message.File.ContentType)
                : null);

            return messageResponse;
        }
    }
}
