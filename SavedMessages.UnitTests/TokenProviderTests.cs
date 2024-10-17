using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using SavedMessages.Domain.Users;
using SavedMessages.Persistence.Services;
using System.Text;

namespace SavedMessages.UnitTests
{
    [TestFixture]
    public class TokenProviderTests
    {
        private TokenProvider _tokenProvider;
        private Mock<IConfiguration> _configurationMock;
        private User _testUser;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(config => config["Jwt:Secret"]).Returns("SuperSecretKeyThatIsLongEnough12345");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");
            var expirationMock = new Mock<IConfigurationSection>();
            expirationMock.Setup(x => x.Value).Returns("60");
            _configurationMock.Setup(config => config.GetSection("Jwt:ExpirationInMinutes")).Returns(expirationMock.Object);

            _tokenProvider = new TokenProvider(_configurationMock.Object);

            var email = "test@gmail.com";
            var firstName = "firstNameTest";
            var lastName = "lastNameTest";
            var password = "passwordTest";

            _testUser = User.Create(email, firstName, lastName, password).Value;
        }

        [Test]
        public void Create_ReturnsValidToken() 
        {
            //Act
            var token = _tokenProvider.Create(_testUser);

            //Assert
            Assert.IsNotEmpty(token);

            var handler = new JsonWebTokenHandler();
            var result = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configurationMock.Object["Jwt:Issuer"],
                ValidAudience = _configurationMock.Object["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationMock.Object["Jwt:Secret"]!))
            });

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void Create_IncludesUserClaimsInToken() 
        {
            //Act
            var token = _tokenProvider.Create(_testUser);

            //Assert
            var handler = new JsonWebTokenHandler();
            var tokenClaims = handler.ReadJsonWebToken(token).Claims;

            Assert.IsNotNull(tokenClaims);
            Assert.IsTrue(tokenClaims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.ToString()));
            Assert.IsTrue(tokenClaims.Any(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == _testUser.Email));
        }
    }
}
