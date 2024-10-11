using MediatR;
using Microsoft.AspNetCore.Http;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Messages.Update
{
    public record UpdateMessageCommand(
        Guid Id,
        string Text,
        IFormFile FileData) : IRequest<Result>;
}
