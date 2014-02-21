using BeeInMyGarden.Data;
using System.Linq;
using System.Web.Mvc;

namespace BeeInMyGarden.Web.Controllers
{
    public class BlogController : Controller
    {
        public ActionResult Index(int? id)
        {
			// Open context to underlying SQL database
			using (var context = new BlogContext())
			{
				// Make sure that it contains database
				context.GenerateDemoData();

				// Build query to retrieve blog items
				IQueryable<BlogItem> blogs = context.BlogItems;
				if (id.HasValue)
				{
					// Uri contains id -> look for the blog article with this id
					blogs = blogs.Where(b => b.BlogId == id.Value);
				}
				else
				{
					// Uri does not contain an id -> take 5 latest articles
					blogs = blogs.OrderByDescending(b => b.BlogId)
						.Take(5);
				}

				// Execute query
				var blogResult = blogs.ToArray();
				if (blogResult.Length > 0)
				{
					// Found blogs -> render them
					return View(blogResult);
				}
				else
				{
					// No blogs found -> return http error
					return HttpNotFound("This blog entry has not been written yet");
				}
			}
        }
    }
}
