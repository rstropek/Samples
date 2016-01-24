using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using AspNetCore1Angular2Intro.Models;
using AspNetCore1Angular2Intro.Services;
using Microsoft.Extensions.OptionsModel;
using System;

namespace AspNetCore1Angular2Intro.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private INameGenerator nameGenerator;
        private BooksDemoDataOptions options;

        public BooksController(INameGenerator nameGenerator, IOptions<BooksDemoDataOptions> options)
        {
            this.nameGenerator = nameGenerator;
            this.options = options.Value;
        }

        [HttpGet]
        public IEnumerable<Book> Get()
        {
            var numberOfBooks = new Random().Next(this.options.MinimumNumberOfBooks, this.options.MaximumNumberOfBooks + 1);
            var result = new Book[numberOfBooks];
            for (var i = 0; i<numberOfBooks; i++)
            {
                result[i] = new Book
                {
                    ID = i,
                    Title = this.nameGenerator.GenerateRandomBookTitle(),
                    Description = "This is an awesome book...",
                    Price = 42.0M
                };
            }

            return result;
        }
    }
}
