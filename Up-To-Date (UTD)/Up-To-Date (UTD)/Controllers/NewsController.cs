using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Up_To_Date__UTD_.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the database context.
        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Displays a paginated list of news items.
        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            var totalNewsCount = await _context.News.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNewsCount / (double)pageSize);

            var newsItems = await _context.News
                .OrderBy(n => n.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(newsItems);
        }

        // GET: Returns the view for the search form.
        [Authorize]
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // POST: Displays the search results based on the search phrase.
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            var searchResults = await _context.News.Where(j => j.NewsHeading.Contains(SearchPhrase)).ToListAsync();
            if (searchResults == null || searchResults.Count == 0)
            {
                ViewBag.Message = "No search results found!";
            }
            return View("Index", searchResults);
        }

        // GET: Displays the details of a specific news item.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Returns the view for creating a new news item (only for authorized users).
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Creates a new news item (only for Admin role).
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NewsHeading,NewsDescription")] News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: Returns the view for editing a news item (only for authorized users).
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Updates an existing news item (only for Editor role).
        [Authorize(Roles = "Editor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NewsHeading,NewsDescription")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: Returns the view for deleting a news item (only for authorized users).
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Confirms the deletion of a news item (only for Admin role).
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if a news item exists by ID.
        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
