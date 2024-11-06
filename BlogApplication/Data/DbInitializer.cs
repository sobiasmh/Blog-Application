using BlogApplication.Models;

namespace BlogApplication.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any categories.
            if (context.Categories.Any())
            {
                return;   // DB has been seeded
            }

            var categories = new Category[]
            {
                new Category { Name = "Technology" },
                new Category { Name = "Health" },
                new Category { Name = "Finance" },
                new Category { Name = "Travel" },
                new Category { Name = "Lifestyle" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
