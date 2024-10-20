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

        // Constructor to initialize the HTTP client for testing.
        public HomeControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            // Create an HTTP client to make requests to your app.
            _client = factory.CreateClient();
        }

        // Test to verify that the Index action returns a valid view.
        [Fact]
        public async Task Index_Returns_View_With_Valid_Data()
        {
            var response = await _client.GetAsync("/Home/Index");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        // Test to verify that the Privacy action returns a valid view.
        [Fact]
        public async Task Privacy_Returns_View_With_Valid_Data()
        {
            var response = await _client.GetAsync("/Home/Privacy");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        // Test to verify that the Error action returns a valid view with an error model.
        [Fact]
        public async Task Error_Returns_View_With_ErrorViewModel()
        {
            var response = await _client.GetAsync("/Home/Error");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }
    }
}
