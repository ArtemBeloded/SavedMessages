using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Get
{
    public record GetMessagesQuery(
        Guid UserId,
        string? SearchTerm,
        int Page,
        int PageSize) : IRequest<Result<List<MessagesResponse>>>;
}
