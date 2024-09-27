﻿using MediatR;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Messages.Delete
{
    internal sealed class DeleteMessageCommandHandler
        : IRequestHandler<DeleteMessageCommand, Result>
    {
        private readonly IMessageRepository _messageRepositoty;

        public DeleteMessageCommandHandler(IMessageRepository messageRepositoty)
        {
            _messageRepositoty = messageRepositoty;
        }

        public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken) 
        {
            var message = await _messageRepositoty.GetByIdAsync(request.MessageId);

            if(message is null) 
            {
                return Result.Failure<MessagesResponse>(MessageErrors.NotFoundMessageById(request.MessageId));
            }

            _messageRepositoty.Remove(message);

            return Result.Success();
        }
    }
}
