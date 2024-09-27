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

namespace SavedMessages.Application.Messages.Update
{
    internal sealed class UpdateMessageCommandHandler
        : IRequestHandler<UpdateMessageCommand, Result>
    {
        private readonly IMessageRepository _messageRepository;

        public UpdateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.Id);

            if(message is null) 
            {
                return Result.Failure<MessagesResponse>(MessageErrors.NotFoundMessageById(request.Id));
            }

            var result = message.Update(request.Text);

            if (result.IsFailure) 
            {
                return Result.Failure(result.Error);
            }

            _messageRepository.Update(message);

            return Result.Success();
        }
    }
}
