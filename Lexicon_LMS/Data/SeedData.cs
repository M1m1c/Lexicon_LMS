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

                var roleNames = new[] { "Teacher", "Student" };

                foreach (var name in roleNames)
                {
                    if (await roleManager.RoleExistsAsync(name)) continue;

                    var role = new IdentityRole { Name = name };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }
                var teacherEmail = "teacher@lexicon.se";

                var foundUser = await userManager.FindByEmailAsync(teacherEmail);

                if (foundUser != null) return;

                var user = new User
                {
                    UserName = teacherEmail,
                    Email = teacherEmail

                };

                var addUserResult = await userManager.CreateAsync(user);

                if (!addUserResult.Succeeded) throw new Exception(string.Join("\n", addUserResult.Errors));


                var adminUser = await userManager.FindByNameAsync(teacherEmail);

                foreach (var role in roleNames)
                {
                    if (await userManager.IsInRoleAsync(adminUser, role)) continue;

                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
                }

            }
        }
    }
}
