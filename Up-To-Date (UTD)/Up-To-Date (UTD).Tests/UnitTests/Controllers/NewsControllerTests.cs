using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Xunit;

public class NewsControllerTests
{
    private readonly ApplicationDbContext _context; // The in-memory database context
    private readonly NewsController _controller; // The controller being tested

    public NewsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "NewsDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new NewsController(_context);
    }

    // Test for the Create action with a valid model
    [Fact]
    public async Task Create_ValidModel_ReturnsRedirectToIndex()
    {
        var news = new News { Id = 1, NewsHeading = "Test Heading", NewsDescription = "Test Description" };
        _controller.ModelState.Clear();
        var result = await _controller.Create(news);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName); 
        var addedNews = await _context.News.FindAsync(news.Id);
        Assert.NotNull(addedNews); 
        Assert.Equal(news.NewsHeading, addedNews.NewsHeading); 
    }

    // Test for the Create action with an invalid model
    [Fact]
    public async Task Create_InvalidModel_ReturnsViewWithModel()
    {
        var news = new News { Id = 1, NewsHeading = "Test Heading", NewsDescription = "Test Description" };
        _controller.ModelState.AddModelError("NewsHeading", "Required");
        var result = await _controller.Create(news);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(news, viewResult.Model); 
        var addedNews = await _context.News.FindAsync(news.Id);
        Assert.Null(addedNews);  
    }

    // Helper method to create a new in-memory context with a specified database name
    private ApplicationDbContext GetInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    // Test for the ShowSearchResults action with a valid search phrase
    [Fact]
    public async Task ShowSearchResults_ValidSearchPhrase_ReturnsResults()
    {
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
        string searchPhrase = "Initial"; 

        var result = await controller.ShowSearchResults(searchPhrase);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<News>>(viewResult.Model); 
        Assert.Single(model); 
        Assert.Equal("Initial Heading", model[0].NewsHeading); 
    }

    // Test for the ShowSearchResults action with no matching results
    [Fact]
    public async Task ShowSearchResults_NoResults_ReturnsMessage()
    {
        var context = GetInMemoryContext("NoResultsDb");
        context.News.Add(new News
        {
            Id = 1,
            NewsHeading = "Initial Heading",
            NewsDescription = "Initial Description"
        });
        context.SaveChanges();

        var controller = new NewsController(context);
        string searchPhrase = "Nonexistent"; 

        var result = await controller.ShowSearchResults(searchPhrase);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<News>>(viewResult.Model);
        Assert.Empty(model); 
        Assert.Equal("No search results found!", controller.ViewBag.Message); 
    }

    // Test for the DeleteConfirmed action with a valid ID
    [Fact]
    public async Task DeleteConfirmed_ValidId_RemovesNewsItem()
    {
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

        var result = await controller.DeleteConfirmed(newsItem.Id);

        Assert.IsType<RedirectToActionResult>(result); 
        var deletedNewsItem = await context.News.FindAsync(newsItem.Id);
        Assert.Null(deletedNewsItem); 
    }
}
