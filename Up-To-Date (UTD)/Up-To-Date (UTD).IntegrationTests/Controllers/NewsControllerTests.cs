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

        public NewsControllerTests(CustomWebApplicationFactory<Up_To_Date__UTD_.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Index_Returns_View_With_News_Items()
        {
            // Act
            var response = await _client.GetAsync("/News");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        [Fact]
        public async Task ShowSearchForm_Returns_View()
        {
            // Act
            var response = await _client.GetAsync("/News/ShowSearchForm");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShowSearchResults_Returns_News_If_Exists()
        {
            // Arrange
            string searchPhrase = "Existing News Heading"; // Adjust this to match an existing news item in your database
            var response = await _client.PostAsync("/News/ShowSearchResults", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SearchPhrase", searchPhrase)
            }));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

       

        [Fact]
        public async Task Create_Returns_View()
        {
            // Act
            var response = await _client.GetAsync("/News/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

       

        [Fact]
        public async Task Edit_Returns_View_For_Existing_News()
        {
            // Arrange
            int newsId = 1; // Replace with an existing news Id
            var response = await _client.GetAsync($"/News/Edit/{newsId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        

        [Fact]
        public async Task Delete_Returns_View_For_Existing_News()
        {
            // Arrange
            int newsId = 1; // Replace with an existing news Id
            var response = await _client.GetAsync($"/News/Delete/{newsId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

      
    }
}
