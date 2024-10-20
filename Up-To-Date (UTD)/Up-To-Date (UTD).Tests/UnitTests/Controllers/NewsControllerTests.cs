using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Xunit;

public class NewsControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        // Setup the in-memory database options
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "NewsDatabase")
            .Options;

        // Initialize the context with the in-memory database
        _context = new ApplicationDbContext(options);
        _controller = new NewsController(_context);
    }

    [Fact]
    public async Task Create_ValidModel_ReturnsRedirectToIndex()
    {
        // Arrange
        var news = new News { Id = 1, NewsHeading = "Test Heading", NewsDescription = "Test Description" };

        // Ensure that the model state is valid
        _controller.ModelState.Clear();

        // Act
        var result = await _controller.Create(news);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify that the news item was added to the context
        var addedNews = await _context.News.FindAsync(news.Id);
        Assert.NotNull(addedNews);
        Assert.Equal(news.NewsHeading, addedNews.NewsHeading);
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var news = new News { Id = 1, NewsHeading = "Test Heading", NewsDescription = "Test Description" };

        // Set ModelState to be invalid
        _controller.ModelState.AddModelError("NewsHeading", "Required");

        // Act
        var result = await _controller.Create(news);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(news, viewResult.Model);

        // Verify that the news item was not added to the context
        var addedNews = await _context.News.FindAsync(news.Id);
        Assert.Null(addedNews);  // Should not be added due to invalid model state
    }

    private ApplicationDbContext GetInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ShowSearchResults_ValidSearchPhrase_ReturnsResults()
    {
        // Arrange
        var context = GetInMemoryContext("SearchResultsDb");
        context.News.Add(new News
        {
            Id = 1,
            NewsHeading = "Initial Heading",
            NewsDescription = "Initial Description"
        });
        context.News.Add(new News
        {
            Id = 2,
            NewsHeading = "Another Heading",
            NewsDescription = "Another Description"
        });
        context.SaveChanges();

        var controller = new NewsController(context);
        string searchPhrase = "Initial"; // Search phrase that matches the heading

        // Act
        var result = await controller.ShowSearchResults(searchPhrase);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<News>>(viewResult.Model);
        Assert.Single(model); // Should return one result
        Assert.Equal("Initial Heading", model[0].NewsHeading);
    }

    [Fact]
    public async Task ShowSearchResults_NoResults_ReturnsMessage()
    {
        // Arrange
        var context = GetInMemoryContext("NoResultsDb");
        context.News.Add(new News
        {
            Id = 1,
            NewsHeading = "Initial Heading",
            NewsDescription = "Initial Description"
        });
        context.SaveChanges();

        var controller = new NewsController(context);
        string searchPhrase = "Nonexistent"; // Search phrase that does not match any heading

        // Act
        var result = await controller.ShowSearchResults(searchPhrase);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<News>>(viewResult.Model);
        Assert.Empty(model); // Should return no results
        Assert.Equal("No search results found!", controller.ViewBag.Message);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidId_RemovesNewsItem()
    {
        // Arrange
        var context = GetInMemoryContext("DeleteConfirmedDb");
        var newsItem = new News
        {
            Id = 1,
            NewsHeading = "Heading to Delete",
            NewsDescription = "Description to Delete"
        };
        context.News.Add(newsItem);
        context.SaveChanges();

        var controller = new NewsController(context);

        // Act
        var result = await controller.DeleteConfirmed(newsItem.Id);

        // Assert
        Assert.IsType<RedirectToActionResult>(result); // Check if redirecting to Index
        var deletedNewsItem = await context.News.FindAsync(newsItem.Id);
        Assert.Null(deletedNewsItem); // Verify that the item has been deleted
    }
}
