using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;
using System.Net;

namespace Up_To_Date__UTD_.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the database context and configure logging.
        public SuggestionsController(ApplicationDbContext context)
        {
            _context = context;
            Log.Information("SuggestionsController initialized.");
        }

        // GET: Displays the list of suggestions.
        [HttpGet]
        public IActionResult Index()
        {
            Log.Information("Fetching list of suggestions.");
            var suggestions = _context.Suggestions.ToList();
            Log.Information("Total suggestions fetched: {SuggestionCount}", suggestions.Count);
            return View(suggestions);
        }

        // POST: Creates a new suggestion if the content is valid.
        [HttpPost]
        public async Task<IActionResult> Create(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                Log.Warning("Attempted to create a suggestion with empty or whitespace content.");
                return RedirectToAction("Index");
            }

            try
            {

                var encodedContent = WebUtility.HtmlEncode(content);
                var suggestion = new Suggestion
                {
                    Content = encodedContent,
                    DatePosted = DateTime.Now
                };
                Log.Information("Contents encoded before storing in database to prevent XSS");
                _context.Suggestions.Add(suggestion);
                await _context.SaveChangesAsync();

                Log.Information("New suggestion created with ID: {SuggestionId}, Content: {Content}", suggestion.Id, suggestion.Content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating a new suggestion.");
                return StatusCode(500, "An error occurred while processing your request.");
            }

            return RedirectToAction("Index");
        }
    }
}
