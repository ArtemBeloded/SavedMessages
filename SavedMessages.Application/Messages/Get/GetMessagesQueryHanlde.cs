using MediatR;
using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Get
{
    internal sealed class GetMessagesQueryHanlde
        : IRequestHandler<GetMessagesQuery, Result<List<MessagesResponse>>>
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessagesQueryHanlde(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result<List<MessagesResponse>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = _messageRepository.GetMessages();

            var response = await messages
                .Select(m => new MessagesResponse(
                    m.Id,
                    m.CreatedInUtc,
                    m.Text)).ToListAsync();

            return response;
        }
    }
}
