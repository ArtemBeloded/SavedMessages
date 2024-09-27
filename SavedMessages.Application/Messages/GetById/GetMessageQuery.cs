using MediatR;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Application.Users.GetUser;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.GetById
{
    public record GetMessageQuery(Guid Id) : IRequest<Result<MessagesResponse>>;
}
