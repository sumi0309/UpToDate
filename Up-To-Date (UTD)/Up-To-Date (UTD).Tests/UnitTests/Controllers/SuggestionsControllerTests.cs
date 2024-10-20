using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Xunit;

namespace Up_To_Date__UTD_.Tests
{
    public class SuggestionsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly SuggestionsController _controller;

        public SuggestionsControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new SuggestionsController(_context);

            // Seed the database with some initial data
            _context.Suggestions.AddRange(new List<Suggestion>
            {
                new Suggestion { Id = 1, Content = "Test suggestion 1", DatePosted = DateTime.Now },
                new Suggestion { Id = 2, Content = "Test suggestion 2", DatePosted = DateTime.Now }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void Index_Returns_ViewResult_With_Suggestions()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Suggestion>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_Redirects_To_Index_When_Content_Is_Valid()
        {
            // Arrange
            string validContent = "Valid suggestion content";

            // Act
            var result = _controller.Create(validContent) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Verify the suggestion was added
            var suggestionsCount = _context.Suggestions.Count();
            Assert.Equal(3, suggestionsCount); // 2 existing + 1 new
        }

        [Fact]
        public void Create_Redirects_To_Index_When_Content_Is_Empty()
        {
            // Arrange
            string invalidContent = "";

            // Act
            var result = _controller.Create(invalidContent) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Verify no new suggestion was added
            var suggestionsCount = _context.Suggestions.Count();
            Assert.Equal(2, suggestionsCount); // Remains the same
        }

        public void Dispose()
        {
            // Clean up the in-memory database
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
