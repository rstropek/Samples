using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using AspNetCore1Angular2Intro.Models;
using AspNetCore1Angular2Intro.Services;
using Microsoft.Extensions.OptionsModel;
using System;
using Microsoft.ApplicationInsights;

namespace AspNetCore1Angular2Intro.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private INameGenerator nameGenerator;
        private BooksDemoDataOptions options;
        private TelemetryClient telemetryClient;

        public BooksController(INameGenerator nameGenerator, IOptions<BooksDemoDataOptions> options, TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            this.nameGenerator = nameGenerator;
            this.options = options.Value;
        }

        [HttpGet]
        public IEnumerable<Book> Get()
        {
            var numberOfBooks = new Random().Next(this.options.MinimumNumberOfBooks, this.options.MaximumNumberOfBooks + 1);

            this.telemetryClient.TrackEvent("Generating books");

            var result = new Book[numberOfBooks];
            for (var i = 0; i < numberOfBooks; i++)
            {
                result[i] = new Book
                {
                    ID = i,
                    Title = this.nameGenerator.GenerateRandomBookTitle(),
                    Description = @"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, 
sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea 
rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.",
                    Price = 42.0M
                };
            }

            return result;
        }


        [HttpPost]
        public IActionResult Post(Book newBook)
        {
            throw new NotImplementedException();
        }
    }
}
