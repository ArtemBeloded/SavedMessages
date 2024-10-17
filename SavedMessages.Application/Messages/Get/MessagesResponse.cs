namespace SavedMessages.Application.Messages.Get
{
    public record MessagesResponse(
        Guid Id,
        DateTime CreatedInUtc,
        string Text,
        bool IsEdited,
        MessageFileResponse File);

    public record MessageFileResponse(
        string FileName,
        byte[] FileData,
        string ContentType);
}
