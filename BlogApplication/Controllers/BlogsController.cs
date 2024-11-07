using BlogApplication.Data;
using BlogApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BlogApplication.Controllers
{
    public class BlogsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public BlogsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult CreateBlog()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddBlogPost(BlogPost blogPost)


        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {

                    blogPost.Content = Regex.Replace(blogPost.Content, "<.*?>", string.Empty); // Strip HTML tags
                    blogPost.UserId = user.Id;
                    _context.BlogPosts.Add(blogPost);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View("CreateBlog", blogPost);
        }

        public async Task<IActionResult> MyBlogs()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var myBlogs = await _context.BlogPosts
                                        .Where(b => b.UserId == user.Id)
                                        .Include(b => b.Category) 
                                        .ToListAsync();

            return View(myBlogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blogPost = await _context.BlogPosts
                                          .Include(b => b.Category)
                                          .FirstOrDefaultAsync(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }





    }
}
