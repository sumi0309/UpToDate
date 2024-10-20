using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Up_To_Date__UTD_.IntegrationTests.Controllers
{
    public class HomeControllerTests : IClassFixture<CustomWebApplicationFactory<Up_To_Date__UTD_.Program>>
    {
        private readonly HttpClient _client;

        public HomeControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            // Create an HTTP client to make requests to your app
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Index_Returns_View_With_Valid_Data()
        {
            // Act
            var response = await _client.GetAsync("/Home/Index");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        [Fact]
        public async Task Privacy_Returns_View_With_Valid_Data()
        {
            // Act
            var response = await _client.GetAsync("/Home/Privacy");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        [Fact]
        public async Task Error_Returns_View_With_ErrorViewModel()
        {
            // Act
            var response = await _client.GetAsync("/Home/Error");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }
    }
}
