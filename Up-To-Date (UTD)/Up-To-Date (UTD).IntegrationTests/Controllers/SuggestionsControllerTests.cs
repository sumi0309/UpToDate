using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Up_To_Date__UTD_.IntegrationTests.Controllers
{
    public class SuggestionsControllerTests : IClassFixture<CustomWebApplicationFactory<Up_To_Date__UTD_.Program>>
    {
        private readonly HttpClient _client;

        public SuggestionsControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            // Create an HTTP client to make requests to your app
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Index_Returns_Success()
        {
            // Act
            var response = await _client.GetAsync("/Suggestions");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        [Fact]
        public async Task Post_CreateSuggestion_Adds_New_Suggestion()
        {
            // Arrange
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("content", "New Test Suggestion")
            });

            // Act
            var response = await _client.PostAsync("/Suggestions/Create", postData);

            // Assert
            response.EnsureSuccessStatusCode();
            var suggestions = await _client.GetStringAsync("/Suggestions");
            suggestions.Should().Contain("New Test Suggestion");
        }
    }
}
