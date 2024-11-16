using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog; 
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

        public ArticlesController(ApplicationDbContext context, string? path = null)
        {
            _context = context;
            _path = path ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Log.Information("ArticlesController initialized with path: {Path}", _path);
        }

        public IActionResult Index()
        {
            Log.Information("Fetching the list of articles.");
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            Log.Information("Accessed Create view for new article.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    Log.Information("Processing file upload: {FileName}, Size: {Size} bytes", file.FileName, file.Length);

                    var contentType = file.ContentType;
                    if (contentType != "application/pdf")
                    {
                        Log.Warning("Invalid file type uploaded. Only PDF files are allowed. File: {FileName}", file.FileName);
                        ModelState.AddModelError("File", "Only PDF files are allowed.");
                        return View("FileTypeError");
                    }


                    const long maxSize = 5 * 1024 * 1024; 
                    if (file.Length > maxSize)
                    {
                        Log.Warning("File uploaded exceeds maximum size. File size: {Size} bytes, Max allowed size: {MaxSize} bytes", file.Length, maxSize);
                        ModelState.AddModelError("File", "File size exceeds 5 MB limit.");
                        return View("FileSizeError"); 
                    }

                    if (!Directory.Exists(_path))
                    {
                        Log.Warning("Upload directory does not exist. Creating directory: {Path}", _path);
                        Directory.CreateDirectory(_path);
                    }

                    var filePath = Path.Combine(_path, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        Log.Information("File {FileName} successfully uploaded to {FilePath}.", file.FileName, filePath);
                    }
                    article.FilePath = $"/uploads/{file.FileName}";
                }
                else
                {
                    Log.Warning("File upload attempt without file or with empty file.");
                }

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                Log.Information("New article created with ID: {ArticleId}, Title: {Title}", article.Id, article.Title);

                return View("Index", await _context.Articles.ToListAsync());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating a new article.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public IActionResult Download(int articleId)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == articleId);

            if (article == null || string.IsNullOrEmpty(article.FilePath))
            {
                Log.Warning("Attempted to download file for article with ID {ArticleId}, but the file was not found.", articleId);
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")+article.FilePath;

            if (!System.IO.File.Exists(filePath))
            {
                Log.Warning("File for article ID {ArticleId} not found at path {FilePath}.", articleId, filePath);
                return NotFound();
            }

            Log.Information("User is downloading file for article with ID {ArticleId} from path {FilePath}.", articleId, filePath);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }

    }
}
