using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuggestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var suggestions = _context.Suggestions.ToList();
            return View(suggestions);
        }

        [HttpPost]
        public IActionResult Create(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Index");
            }

            var suggestion = new Suggestion
            {
                Content = content,
                DatePosted = DateTime.Now
            };

            _context.Suggestions.Add(suggestion);
            _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }

}
