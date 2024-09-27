using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Update
{
    public record UpdateMessageCommand(
        Guid Id,
        string Text) : IRequest<Result>;
}
