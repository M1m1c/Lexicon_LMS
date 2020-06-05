using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Data
{
    public class SeedData
    {
        public static async Task IntializeAsync(IServiceProvider service)
        {
            var options = service.GetRequiredService<DbContextOptions<ApplicationDbContext>>();

            using (var context = new ApplicationDbContext(options))
            {
                var userManager = service.GetRequiredService<UserManager<User>>();
                var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

                var rolenames = new[] { "Teacher", "Student" };

                foreach (var name in rolenames)
                {
                    if (await roleManager.RoleExistsAsync(name)) continue;

                    var role = new IdentityRole { Name = name };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }
            }
        }
    }
}
