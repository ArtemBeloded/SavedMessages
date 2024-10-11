using MediatR;
using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;
using System.Linq;

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
            var messages = _messageRepository.GetMessages()
                .Include(f => f.File)
                .Where(x => x.UserId == request.UserId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm)) 
            {
                messages = messages.Where(m =>
                m.Text.Contains(request.SearchTerm));
            }

            messages = messages.OrderByDescending(f => f.CreatedInUtc);

            var response = await messages
                .Select(m => new MessagesResponse(
                    m.Id,
                    m.CreatedInUtc,
                    m.Text,
                    m.IsEdited,
                    new MessageFileResponse(
                        m.File.FileName,
                        m.File.Data,
                        m.File.ContentType))).ToListAsync();

            return response;
        }
    }
}
