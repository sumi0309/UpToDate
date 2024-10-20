using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Up_To_Date__UTD_.Models;
using System.Net.Http.Json;

namespace Up_To_Date__UTD_.IntegrationTests.Controllers
{
    public class NewsControllerTests : IClassFixture<CustomWebApplicationFactory<Up_To_Date__UTD_.Program>>
    {
        private readonly HttpClient _client;

        // Constructor to initialize the HTTP client for testing.
        public NewsControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test to verify that the Index action returns a view with news items.
        [Fact]
        public async Task Index_Returns_View_With_News_Items()
        {
            var response = await _client.GetAsync("/News");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        // Test to verify that the ShowSearchForm action returns a view.
        [Fact]
        public async Task ShowSearchForm_Returns_View()
        {
            var response = await _client.GetAsync("/News/ShowSearchForm");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Test to verify that ShowSearchResults returns news if it exists.
        [Fact]
        public async Task ShowSearchResults_Returns_News_If_Exists()
        {
            string searchPhrase = "Existing News Heading"; 
            var response = await _client.PostAsync("/News/ShowSearchResults", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SearchPhrase", searchPhrase)
            }));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Test to verify that the Create action returns a view.
        [Fact]
        public async Task Create_Returns_View()
        {
            var response = await _client.GetAsync("/News/Create");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Test to verify that the Edit action returns a view for an existing news item.
        [Fact]
        public async Task Edit_Returns_View_For_Existing_News()
        {
            int newsId = 1; // Replace with an existing news Id
            var response = await _client.GetAsync($"/News/Edit/{newsId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Test to verify that the Delete action returns a view for an existing news item.
        [Fact]
        public async Task Delete_Returns_View_For_Existing_News()
        {
            int newsId = 1; // Replace with an existing news Id
            var response = await _client.GetAsync($"/News/Delete/{newsId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
