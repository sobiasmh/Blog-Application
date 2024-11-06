using BlogApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApplication.Controllers
{
    public class BlogsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public BlogsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult CreateBlog()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddBlogPost(string Title, string Content, string Category)
        {
          
            return RedirectToAction("Index"); 
        }

    }
}
