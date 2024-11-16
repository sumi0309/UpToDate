using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog; 
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Up_To_Date__UTD_.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
            Log.Information("NewsController initialized.");
        }

        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            Log.Information("Fetching news list. Page number: {PageNumber}, Page size: {PageSize}", pageNumber, pageSize);
            var totalNewsCount = await _context.News.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNewsCount / (double)pageSize);

            var newsItems = await _context.News
                .OrderBy(n => n.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            Log.Information("Fetched {NewsCount} news items for page {PageNumber}.", newsItems.Count, pageNumber);
            return View(newsItems);
        }

        [Authorize]
        public async Task<IActionResult> ShowSearchForm()
        {
            Log.Information("Accessed news search form.");
            return View();
        }

        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            Log.Information("Search initiated with phrase: {SearchPhrase}", SearchPhrase);
            var searchResults = await _context.News.Where(j => j.NewsHeading.Contains(SearchPhrase)).ToListAsync();

            if (searchResults == null || searchResults.Count == 0)
            {
                Log.Warning("No search results found for phrase: {SearchPhrase}", SearchPhrase);
                ViewBag.Message = "No search results found!";
            }
            else
            {
                Log.Information("{SearchResultsCount} search results found for phrase: {SearchPhrase}", searchResults.Count, SearchPhrase);
            }

            return View("Index", searchResults);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                Log.Warning("Attempted to view news details with invalid ID.");
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                Log.Warning("News item with ID: {NewsId} not found.", id);
                return NotFound();
            }

            Log.Information("Displaying details for news item with ID: {NewsId}.", id);
            return View(news);
        }

        [Authorize]
        public IActionResult Create()
        {
            Log.Information("Accessed Create view for a new news item.");
            if (!User.IsInRole("Admin"))
            {
                Log.Warning("Unauthorized create attempt: User {UserName} tried to create a news", User.Identity.Name);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NewsHeading,NewsDescription")] News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                Log.Information("New news item created with ID: {NewsId}, Heading: {NewsHeading}.", news.Id, news.NewsHeading);
                return RedirectToAction(nameof(Index));
            }

            Log.Warning("Model state invalid for creating news item: {NewsHeading}.", news.NewsHeading);
            return View(news);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Log.Warning("Attempted to edit news with invalid ID.");
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                Log.Warning("News item with ID: {NewsId} not found for editing.", id);
                return NotFound();
            }
            Log.Information("Accessed edit view for news item with ID: {NewsId}.", id);
            if (!User.IsInRole("Editor"))
            {
                Log.Warning("Unauthorized edit attempt: User {UserName} tried to edit news with ID: {NewsId}.", User.Identity.Name, id);
            }
            return View(news);
        }

        [Authorize(Roles = "Editor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NewsHeading,NewsDescription")] News news)
        {
            if (id != news.Id)
            {
                Log.Warning("Attempted to update news item with mismatched ID: {NewsId}.", id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    Log.Information("News item with ID: {NewsId} updated successfully.", news.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        Log.Warning("Concurrency error: News item with ID: {NewsId} not found during update.", news.Id);
                        return NotFound();
                    }
                    else
                    {
                        Log.Error("Unexpected error occurred during news item update with ID: {NewsId}.", news.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            Log.Warning("Model state invalid for updating news item with ID: {NewsId}.", news.Id);
            return View(news);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                Log.Warning("Attempted to delete news with invalid ID.");
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                Log.Warning("News item with ID: {NewsId} not found for deletion.", id);
                return NotFound();
            }

            Log.Information("Accessed delete view for news item with ID: {NewsId}.", id);
            if (!User.IsInRole("Admin"))
            {
                Log.Warning("Unauthorized delete attempt: User {UserName} tried to delete news with ID: {NewsId}.", User.Identity.Name, id);
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                Log.Information("News item with ID: {NewsId} deleted successfully.", id);
            }
            else
            {
                Log.Warning("Attempted to delete non-existing news item with ID: {NewsId}.", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
