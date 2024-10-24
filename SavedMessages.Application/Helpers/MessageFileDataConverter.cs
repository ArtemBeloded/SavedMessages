using Microsoft.AspNetCore.Http;

namespace SavedMessages.Application.Helpers
{
    public static class MessageFileDataConverter
    {
        public static byte[] ConvertMessageFileToByteArray(IFormFile fileData)
        {
            using var stream = fileData.OpenReadStream();
            using var reader = new BinaryReader(stream);
            using var outputStream = new MemoryStream();

            byte[] buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }

            return outputStream.ToArray();
        }
    }
}
