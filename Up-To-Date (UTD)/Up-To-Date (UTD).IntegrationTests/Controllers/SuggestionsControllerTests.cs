using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Up_To_Date__UTD_.IntegrationTests.Controllers
{
    public class SuggestionsControllerTests : IClassFixture<CustomWebApplicationFactory<Up_To_Date__UTD_.Program>>
    {
        private readonly HttpClient _client;

        // Constructor to initialize the HTTP client for testing.
        public SuggestionsControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test to verify that the Index action returns a successful response.
        [Fact]
        public async Task Get_Index_Returns_Success()
        {
            var response = await _client.GetAsync("/Suggestions");
            response.EnsureSuccessStatusCode(); 
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        // Test to verify that posting a new suggestion adds it to the list.
        [Fact]
        public async Task Post_CreateSuggestion_Adds_New_Suggestion()
        {
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("content", "New Test Suggestion")
            });
            var response = await _client.PostAsync("/Suggestions/Create", postData);
            response.EnsureSuccessStatusCode();
            var suggestions = await _client.GetStringAsync("/Suggestions");
            suggestions.Should().Contain("New Test Suggestion");
        }
    }
}
