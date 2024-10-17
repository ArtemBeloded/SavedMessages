using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SavedMessages.Application.Helpers;

namespace SavedMessages.UnitTests
{
    [TestFixture]
    public class MessageFileDataConverterTests
    {
        [Test]
        public void ConvertMessageFileToByteArray_ReturnsCorrectByteArray_WhenFileHasData() 
        {
            //Arrange
            var mockFile = new Mock<IFormFile>();
            byte[] fileContent = { 1, 2, 3, 4, 5 };
            var memoryStream = new MemoryStream(fileContent);
            mockFile.Setup(f => f.OpenReadStream())
                .Returns(memoryStream);

            //Act
            var result = MessageFileDataConverter
                .ConvertMessageFileToByteArray(mockFile.Object);

            //Assert
            Assert.AreEqual(fileContent, result);
        }

        [Test]
        public void ConvertMessageFileTByteArray_ReturnsEmptyByteArray_WhenFileIsEmpty() 
        {
            //Arrange
            var mockFile = new Mock<IFormFile>();
            var emptyStream = new MemoryStream();
            mockFile.Setup(f => f.OpenReadStream())
                .Returns(emptyStream);

            //Act
            var result = MessageFileDataConverter
                .ConvertMessageFileToByteArray(mockFile.Object);

            //Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void ConvertMessageFileToByteArray_ReadsFileInChanks() 
        {
            //Arrange
            var mockFile = new Mock<IFormFile>();
            byte[] fileContent = new byte[5000];

            for (int i = 0; i < fileContent.Length; i++)
            {
                fileContent[i] = (byte)i;
            }
            
            var memoryStream = new MemoryStream(fileContent);
            mockFile.Setup(f => f.OpenReadStream())
                .Returns(memoryStream);

            //Act
            var result = MessageFileDataConverter
                .ConvertMessageFileToByteArray(mockFile.Object);

            //Assert
            Assert.AreEqual(fileContent, result);
        }

        [Test]
        public void ConvertMessageFileToByteArray_ClosesStreamsProperly() 
        {
            //Arrange
            var mockFile = new Mock<IFormFile>();
            var mockStream = new Mock<Stream>();

            mockStream.Setup(s => s.CanRead).Returns(true);
            mockStream.Setup(s => s.Read(It.IsAny<byte[]>(), 0, 4096))
                .Returns(0);
            mockFile.Setup(f => f.OpenReadStream())
                .Returns(mockStream.Object);

            //Act
            MessageFileDataConverter.ConvertMessageFileToByteArray(mockFile.Object);

            //Assert
            mockStream.Verify(s => s.Read(It.IsAny<byte[]>(), 0, 4096), Times.AtLeastOnce);
            mockStream.Verify(s => s.CanRead, Times.Once);
        }
    }
}
