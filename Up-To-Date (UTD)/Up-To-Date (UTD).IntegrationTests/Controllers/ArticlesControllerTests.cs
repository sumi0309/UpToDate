using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Xunit;

public class ArticlesControllerIntegrationTests
{
    private readonly ApplicationDbContext _context;
    private readonly ArticlesController _controller;
    private readonly string _uploadPath;

    public ArticlesControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ArticlesTestDb")
            .Options;

        _context = new ApplicationDbContext(options);

        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "test_uploads");

        _controller = new ArticlesController(_context, _uploadPath);
    }

    [Fact]
    public void Index_ReturnsViewWithArticles()
    {
        _context.Articles.Add(new Article { Id = 1, Title = "Test Article 1", Content = "Test Content 1" });
        _context.Articles.Add(new Article { Id = 2, Title = "Test Article 2", Content = "Test Content 2" });
        _context.SaveChanges();

        var result = _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IQueryable<Article>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Create_ValidArticleAndFile_ReturnsIndexWithArticles()
    {
        var article = new Article { Title = "New Article", Content = "New Content" };

        var fileMock = new Mock<IFormFile>();
        var fileName = "testfile.txt";
        var fileContent = "This is a test file.";
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write(fileContent);
        writer.Flush();
        memoryStream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(memoryStream.Length);

        var result = await _controller.Create(article, fileMock.Object);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IQueryable<Article>>(viewResult.Model);

        Assert.Equal(1, model.Count());
        Assert.Equal("/uploads/testfile.txt", model.First().FilePath);

        if (File.Exists(Path.Combine(_uploadPath, fileName)))
        {
            File.Delete(Path.Combine(_uploadPath, fileName));
        }
    }

    [Fact]
    public async Task Create_NoFile_ReturnsIndexWithArticles()
    {
        var article = new Article { Title = "No File Article", Content = "Article without file" };
        IFormFile file = null;

        var result = await _controller.Create(article, file);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IQueryable<Article>>(viewResult.Model);

        Assert.Equal(1, model.Count());
        Assert.Null(model.First().FilePath); 
    }
}
