using MediatR;
using SavedMessages.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Messages.Get
{
    public record GetMessagesQuery(
        Guid UserId,
        string? SearchTerm) : IRequest<Result<List<MessagesResponse>>>;
}
