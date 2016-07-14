using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.IO;
using System.Reflection;

namespace SignalrOwinHelloWorld
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Setup static file serving
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot")),
                EnableDefaultFiles = true
            });

            // Enable access from every URL with CORS
            app.UseCors(CorsOptions.AllowAll);

            // Map SignalR hubs
            app.MapSignalR();
        }
    }
}
