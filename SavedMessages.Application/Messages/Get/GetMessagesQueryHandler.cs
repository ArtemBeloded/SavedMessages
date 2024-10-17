using MediatR;
using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Get
{
    public sealed class GetMessagesQueryHandler
        : IRequestHandler<GetMessagesQuery, Result<List<MessagesResponse>>>
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessagesQueryHandler(IMessageRepository messageRepository)
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
                m.Text.Contains(request.SearchTerm) ||
                m.File.FileName.Contains(request.SearchTerm));
            }

            messages = messages.OrderByDescending(f => f.CreatedInUtc);

            var response = await messages
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
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
