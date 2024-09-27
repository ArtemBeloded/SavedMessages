﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavedMessages.API.Contracts.Messages;
using SavedMessages.API.Extensions;
using SavedMessages.Application.Messages.Create;
using SavedMessages.Application.Messages.Delete;
using SavedMessages.Application.Messages.Get;
using SavedMessages.Application.Messages.GetById;
using SavedMessages.Application.Messages.Update;
using SavedMessages.Domain.Shared;

namespace SavedMessages.API.Endpoints
{
    [Route("api/messages")]
    public sealed class MessagesController : Controller
    {
        private ISender _sender;

        public MessagesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("addmessage")]
        public async Task<IResult> AddMessage([FromBody]AddMessageRequest request) 
        {
            var command = new CreateMessageCommand(request.UserId, request.Text);

            Result result = await _sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
        }

        [HttpGet]
        //[Authorize]
        public async Task<IResult> GetMessages(string? searchTerm) 
        {
            var query = new GetMessagesQuery(searchTerm);

            Result<List<MessagesResponse>> result = await _sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }

        [HttpGet("{id:guid}")]
        public async Task<IResult> GetMessage(Guid id) 
        {
            var query = new GetMessageQuery(id);

            Result<MessagesResponse> result = await _sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        }

        [HttpPut("{id:guid}")]
        public async Task<IResult> UpdateMessage(
            Guid id,
            [FromBody] UpdateMessageRequest request)
        {
            var command = new UpdateMessageCommand(id, request.Text);

            Result result = await _sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
        }

        [HttpDelete("id:guid")]
        public async Task<IResult> DeleteMessage(Guid id) 
        {
            var command = new DeleteMessageCommand(id);

            Result result = await _sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
        }
    }
}