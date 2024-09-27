using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Delete
{
    public record DeleteMessageCommand(Guid MessageId): IRequest<Result>;
}
