using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TodoBackend.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "TodoBackend.Api";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}