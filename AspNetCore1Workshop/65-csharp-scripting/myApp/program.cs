using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;

namespace myApp
{
    
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
    
    [Route("calc")]
    public class GreetingController : Controller
    {
        [HttpGet]
        public async Task<string> Get(string formula)
        {
            var result = await CSharpScript.EvaluateAsync(formula);
            return result.ToString();
        }
    }
}
