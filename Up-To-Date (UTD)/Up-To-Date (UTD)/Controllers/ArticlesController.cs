using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Data;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _path;

        // Constructor initializes the database context and file upload path.
        public ArticlesController(ApplicationDbContext context, string? path = null)
        {
            _context = context;
            _path = path ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        }

        // Displays the list of articles.
        public IActionResult Index()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        // Returns the view for creating a new article.
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handles the creation of a new article and file upload.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(_path, file.FileName);               
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }               
                article.FilePath = $"/uploads/{file.FileName}";
            }            
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();            
            return View("Index", await _context.Articles.ToListAsync());
        }
    }
}