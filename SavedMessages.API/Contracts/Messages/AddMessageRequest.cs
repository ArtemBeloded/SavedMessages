namespace SavedMessages.API.Contracts.Messages
{
    public record AddMessageRequest(
        Guid UserId,
        string Text,
        IFormFile? File);
}
