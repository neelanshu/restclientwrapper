using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestaurantsNearMe.ApiInfrastructure.Api.Request;
using RestaurantsNearMe.ApiInfrastructure.Api.Response;
using RestaurantsNearMe.ApiInfrastructure.Serialization;
using RestaurantsNearMe.ApiInfrastructure.Tests.Fakes;

namespace RestaurantsNearMe.ApiInfrastructure.Tests.Http
{
    [TestFixture]
    public class ApiResponseFactoryTests
    {
        private static ApiResponseFactory CreateFactory(
           ICustomJsonSerializer serializer = null,
           IApiConfiguration config = null)
        {
            return new ApiResponseFactory(
                serializer ?? new Mock<ICustomJsonSerializer>(MockBehavior.Loose).Object,
                config ?? new FakeApiConfiguration());
        }

        private class CustomType { }

        
        [Test]
        public async void ShouldCallPartialDeSerializer_FlagIsTrue_TokenIsProvided_AndContentTypeIsJson()
        {
            var mockSerializer = new Mock<ICustomJsonSerializer>(MockBehavior.Loose);
            var responseContent = new StringContent("{}", Encoding.UTF8, "application/json");
            var factory = CreateFactory(serializer: mockSerializer.Object);

            await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage() { Content = responseContent }, true, "a");

            mockSerializer.Verify(j => j.PartialDeserialize<CustomType>(It.IsAny<string>(),It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShouldReturnApiResponseWithStatusCodeSetToTheResponseStatusCode()
        {
            var expectedStatusCode = HttpStatusCode.PartialContent;
            var factory = CreateFactory();

            var actualResponse = await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage { StatusCode = expectedStatusCode },false,string.Empty);

            Assert.AreEqual(expectedStatusCode, actualResponse.StatusCode);
        }

        [Test]
        public async Task ShouldReturnApiResponseWithNullBody_WhenResponseHasNoContent()
        {
            var factory = CreateFactory();

            var actualResponse = await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage(), false,string.Empty);

            Assert.Null(actualResponse.Body);
            Assert.Null(actualResponse.BodyAsObject);
        }

        [Test]
        public async Task ShouldReturnApiResponseWithNullBody_WhenResponseIsByteArray()
        {
            var responseContent = new ByteArrayContent(Encoding.UTF8.GetBytes("abcdefg"));
            var factory = CreateFactory();

            var actualResponse = await factory.CreateApiResponseAsync<byte[]>(new HttpResponseMessage() { Content = responseContent },false,string.Empty);

            Assert.Null(actualResponse.Body);
        }

        [Test]
        public async Task ShouldReturnApiResponseWithBodyAsObjectSetToTheResponseContentBytes_WhenResponseIsByteArray()
        {
            var expectedBodyAsObject = Encoding.UTF8.GetBytes("expectedBytes");
            var responseContent = new ByteArrayContent(expectedBodyAsObject);
            var factory = CreateFactory();

            var actualResponse = await factory.CreateApiResponseAsync<byte[]>(new HttpResponseMessage() { Content = responseContent }, false,string.Empty);

            Assert.AreEqual(expectedBodyAsObject, actualResponse.BodyAsObject);
        }

       
        [Test]
        public async Task ShouldReturnApiResponseWithBodyAsObjectSetToNull_WhenResponseIsNotByteArray_AndContentTypeIsNotJson()
        {
            var responseContent = new StringContent("{}", Encoding.UTF8, "application/xml");
            var factory = CreateFactory();

            var actualResponse = await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage() { Content = responseContent }, false,string.Empty);

            Assert.Null(actualResponse.BodyAsObject);
        }

        [Test]
        public async Task ShouldNotCallSerializer_WhenResponseIsNotByteArray_AndContentTypeIsNotJson()
        {
            var mockSerializer = new Mock<ICustomJsonSerializer>(MockBehavior.Loose);
            var responseContent = new StringContent("{}", Encoding.UTF8, "application/xml");
            var factory = CreateFactory(serializer: mockSerializer.Object);

            await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage() { Content = responseContent }, false,string.Empty);

            mockSerializer.Verify(j => j.Deserialize<CustomType>(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ShouldPassResponseContentStringToJsonSerializer_WhenResponseIsNotByteArray_AndContentTypeIsJson()
        {
            var expectedJsonText = "{\"id\": 7}";
            var responseContent = new StringContent(expectedJsonText, Encoding.UTF8, "application/json");
            var mockSerializer = new Mock<ICustomJsonSerializer>(MockBehavior.Loose);
            mockSerializer.Setup(j => j.Deserialize<CustomType>(expectedJsonText))
                          .Returns(new CustomType())
                          .Verifiable();
            var factory = CreateFactory(serializer: mockSerializer.Object);

            await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage() { Content = responseContent },false,string.Empty);

            mockSerializer.Verify();
        }

        [Test]
        public async Task ShouldReturnApiResponseWithBodyAsObjectSetToResultDeserializedByTheJsonSerializer_WhenResponseIsNotByteArray_AndContentTypeIsJson()
        {
            var expectedBodyAsObject = new CustomType();
            var responseContent = new StringContent("{}", Encoding.UTF8, "application/json");
            var mockSerializer = new Mock<ICustomJsonSerializer>(MockBehavior.Loose);
            mockSerializer.Setup(j => j.Deserialize<CustomType>(It.IsAny<string>())).Returns(expectedBodyAsObject);
            var factory = CreateFactory(serializer: mockSerializer.Object);

            var actualResponse = await factory.CreateApiResponseAsync<CustomType>(new HttpResponseMessage() { Content = responseContent },false,string.Empty);

            Assert.AreEqual(expectedBodyAsObject, actualResponse.BodyAsObject);
        }
    }
}
