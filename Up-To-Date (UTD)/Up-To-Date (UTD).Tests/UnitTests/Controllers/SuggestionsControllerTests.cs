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
        private readonly ApplicationDbContext _context; // The in-memory database context
        private readonly SuggestionsController _controller; // The controller being tested

        public SuggestionsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new SuggestionsController(_context);
            _context.Suggestions.AddRange(new List<Suggestion>
            {
                new Suggestion { Id = 1, Content = "Test suggestion 1", DatePosted = DateTime.Now },
                new Suggestion { Id = 2, Content = "Test suggestion 2", DatePosted = DateTime.Now }
            });
            _context.SaveChanges();
        }

        // Test for the Index action
        [Fact]
        public void Index_Returns_ViewResult_With_Suggestions()
        {
            var result = _controller.Index() as ViewResult;
            Assert.NotNull(result); 
            Assert.IsType<ViewResult>(result); 
            var model = Assert.IsAssignableFrom<List<Suggestion>>(result.Model); 
            Assert.Equal(2, model.Count); 
        }

        // Test for Create action with valid content
        [Fact]
        public async Task Create_Redirects_To_Index_When_Content_Is_Valid()
        {
            string validContent = "Valid suggestion content";
            var result = await _controller.Create(validContent) as RedirectToActionResult;
            Assert.NotNull(result); 
            Assert.Equal("Index", result.ActionName); 
            var suggestionsCount = _context.Suggestions.Count(); 
            Assert.Equal(3, suggestionsCount); 
        }

        // Test for Create action with empty content
        [Fact]
        public async Task Create_Redirects_To_Index_When_Content_Is_Empty()
        {
            string invalidContent = "";
            var result = await _controller.Create(invalidContent) as RedirectToActionResult;
            Assert.NotNull(result); 
            Assert.Equal("Index", result.ActionName); 
            var suggestionsCount = _context.Suggestions.Count(); 
            Assert.Equal(2, suggestionsCount); 
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); 
            _context.Dispose(); 
        }
    }
}
