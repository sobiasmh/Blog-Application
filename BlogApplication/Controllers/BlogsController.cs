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

        public async Task<IActionResult> CreateBlog(int? id)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;

            if (id.HasValue)
            {
                var blogPost = await _context.BlogPosts
                                              .FirstOrDefaultAsync(b => b.Id == id.Value);

                if (blogPost == null)
                {
                    return NotFound();
                }

                return View(blogPost);
            }

            return View(new BlogPost());
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateBlogPost(BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View("CreateBlog", blogPost);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                blogPost.Content = Regex.Replace(blogPost.Content, "<.*?>", string.Empty); 
                blogPost.UserId = user.Id;

                if (blogPost.Id == 0) 
                {
                    _context.BlogPosts.Add(blogPost);
                }
                else
                {
                    var existingBlog = await _context.BlogPosts.FindAsync(blogPost.Id);
                    if (existingBlog == null) return NotFound();

                    existingBlog.Title = blogPost.Title;
                    existingBlog.Content = blogPost.Content;
                    existingBlog.CategoryId = blogPost.CategoryId;
                    existingBlog.CreatedDate = blogPost.CreatedDate;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("MyBlogs");
            }

            return RedirectToAction("/Identity/Account/Login"); 
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null || blogPost.UserId != user.Id)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBlogs");
        }
    }
}
