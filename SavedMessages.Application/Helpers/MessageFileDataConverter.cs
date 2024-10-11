using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Helpers
{
    public static class MessageFileDataConverter
    {
        public static byte[] ConvertMessageFileToByteArray(IFormFile fileData)
        {
            using var stream = fileData.OpenReadStream();
            using var reader = new BinaryReader(stream);
            using var outputStream = new MemoryStream();  // используется для хранения только во время чтения

            byte[] buffer = new byte[4096];  // буфер для порционного чтения
            int bytesRead;

            // Чтение из потока небольшими частями
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }

            // Возвращаем результат
            return outputStream.ToArray();
        }
    }
}
