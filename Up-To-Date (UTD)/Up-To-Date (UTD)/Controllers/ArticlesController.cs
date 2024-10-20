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

        public ArticlesController(ApplicationDbContext context, string? path=null)
        {
            _context = context;
            if (path == null)
            {
                _path=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            }
            else
            {
                _path = path;
            }
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile file)
        {
                if (file != null && file.Length > 0)
                {
                var filePath = Path.Combine(_path, file.FileName);
                //var filePath = "E:/Masters/ENPM680 Project/Up-To-Date (UTD)/Up-To-Date (UTD)/wwwroot/uploads/Resume - Sumiran Jaiswal.pdf";
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