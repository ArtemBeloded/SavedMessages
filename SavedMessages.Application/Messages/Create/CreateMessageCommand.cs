using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Create
{
    public record CreateMessageCommand(Guid UserId, string Text) : IRequest<Result>;
}
