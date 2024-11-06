using BlogApplication.Data;
using BlogApplication.Models;
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
        [HttpPost]
        public IActionResult AddBlogPost(BlogPost blogPost)

        {
            if (!ModelState.IsValid)
            {
                _context.BlogPosts.Add(blogPost);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View("CreateBlog", blogPost);
        }




    }
}
