namespace SavedMessages.API.Contracts.Messages
{
    public sealed record UpdateMessageRequest(
        string Text,
        IFormFile? File);
}
