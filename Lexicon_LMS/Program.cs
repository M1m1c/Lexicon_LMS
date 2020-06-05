using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexicon_LMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lexicon_LMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
          var host=  CreateHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var config = services.GetRequiredService<IConfiguration>();
                try
                {
                    SeedData.IntializeAsync(services).Wait();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
