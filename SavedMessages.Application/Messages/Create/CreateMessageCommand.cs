using MediatR;
using Microsoft.AspNetCore.Http;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Create
{
    public record CreateMessageCommand(
        Guid UserId,
        string Text,
        IFormFile FileData) : IRequest<Result>;
}
