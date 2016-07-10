using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Text;

namespace ObserverInDiagnostics
{
    public class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder().UseKestrel().UseStartup<Startup>().Build().Run();
    }

    public class Startup : IObserver<KeyValuePair<string, object>>
    {
        private readonly byte[] message = Encoding.UTF8.GetBytes("Hello World");

        public void ConfigureServices(IServiceCollection services)
        {
            // Note how the Observable pattern is used in DiagnosticListener. It implements
            // IObservable<T>, therefore we can subscribe to events.
            var listener = new DiagnosticListener("Microsoft.AspNetCore");
            listener.Subscribe(this);

            services.AddSingleton<DiagnosticSource>(listener);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/error", errorApp =>
                // Throw exception for demo purposes
                errorApp.Run(h => { throw new DivideByZeroException(); }));

            app.Run(h => h.Response.Body.WriteAsync(message, 0, message.Length));
        }

        #region Implementation of IObserver<T>
        public void OnCompleted() => Console.WriteLine("OnCompleted");
        public void OnError(Exception error) { throw error; }
        public void OnNext(KeyValuePair<string, object> value) => Console.WriteLine($"OnNext: {value.Key}");
        #endregion
    }
}
