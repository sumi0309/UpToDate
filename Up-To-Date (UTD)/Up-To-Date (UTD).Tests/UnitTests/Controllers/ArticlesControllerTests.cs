using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Xunit;


namespace Up_To_Date__UTD_.Tests.UnitTests.Controllers
{
    public class ArticlesControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_AddsArticleAndReturnsIndexViewSupreme()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ArticlesController(context, "E:\\Masters\\ENPM680 Project\\Up-To-Date (UTD)\\Up-To-Date (UTD).Tests\\wwwroot\\uploads\\");

            var article = new Article
            {
                Title = "New Article",
                Content = "This is some article content",  // Set the required Content field
                FilePath = "" // You can initialize this as an empty string; it will be set later
            };

            // Mock the file to be uploaded
            var fileMock = new Mock<IFormFile>();
            var content = "File content";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            // Replace file path generation with a mock or in-memory path
            var mockFilePath = $"/uploads/{fileName}";
            article.FilePath = mockFilePath; // Set this to mock path directly in the test

            // Act
            var result = await controller.Create(article, fileMock.Object);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var articles = await context.Articles.ToListAsync();
            Assert.Single(articles);
            Assert.Equal(mockFilePath, articles[0].FilePath);
        }

    }
}
