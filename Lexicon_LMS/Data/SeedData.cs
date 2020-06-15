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

                await SeedRoles(roleManager, roleNames);

                var teacherEmail = "teacher@lexicon.se";
                var teacherPassword= "Abc12#";

                await SeedTeacher(userManager, roleManager, roleNames, teacherEmail, teacherPassword);

                await SeedActiityTypes(context);

                await SeedDifficulties(context);
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager, string[] roleNames)
        {
            foreach (var name in roleNames)
            {
                if (await roleManager.RoleExistsAsync(name)) continue;

                var role = new IdentityRole { Name = name };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task SeedTeacher(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, string[] roleNames,string teacherEmail,string teacherPassword)
        {
            var foundUser = await userManager.FindByEmailAsync(teacherEmail);

            if (foundUser != null) return;

            var user = new User
            {
                UserName = teacherEmail,
                Email = teacherEmail

            };

            var addUserResult = await userManager.CreateAsync(user, teacherPassword);

            if (!addUserResult.Succeeded) throw new Exception(string.Join("\n", addUserResult.Errors));


            var teacherUser = await userManager.FindByNameAsync(teacherEmail);

            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(teacherUser, role)) continue;

                var addToRoleResult = await userManager.AddToRoleAsync(teacherUser, role);

                if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
            }
        }

        private static async Task SeedActiityTypes(ApplicationDbContext context)
        {
            if (context.ActivityTypes.Any())
            {
                return;
            }

            context.ActivityTypes.AddRange(
                new ActivityType { Name = "Assignment" },
                new ActivityType { Name = "E-Learning" },
                new ActivityType { Name = "Lecture" },
                new ActivityType { Name = "Exam" }
                );

            await context.SaveChangesAsync();
        }

        private static async Task SeedDifficulties(ApplicationDbContext context)
        {
            if (context.Difficulties.Any())
            {
                return;
            }

            context.Difficulties.AddRange(
                new Difficulty { Level = "Beginner" },
                new Difficulty { Level = "Intermediate" },
                new Difficulty { Level = "Advanced" },
                new Difficulty { Level = "Professional" }
                );

            await context.SaveChangesAsync();
        }
    }
}
