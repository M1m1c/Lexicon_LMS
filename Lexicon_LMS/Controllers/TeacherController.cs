using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public TeacherController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //TODO map to view model instead of doing mannually here
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Users()
        {
            var users = await context.Users.ToListAsync();
            var currentUserId = userManager.GetUserId(User);


            var model = users.Where(q => q.Id != currentUserId).Select(u => new UserViewModel
            {
                Id = u.Id,
                Age = u.Age,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = context.Roles.Find(context.UserRoles.FirstOrDefault(ur => ur.UserId == u.Id).RoleId).Name
            });
 
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateUser(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    Email = userViewModel.Email,
                    UserName = userViewModel.Email,
                    Age = userViewModel.Age

                };

                var role = await roleManager.FindByNameAsync(userViewModel.Role);

                if (role != null)
                {
                    var result = await userManager.CreateAsync(user, userViewModel.Password);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

                    var addedUser = await userManager.FindByNameAsync(user.Email);

                    var addToRoleResult = await userManager.AddToRoleAsync(addedUser, role.Name);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));

                    return RedirectToAction("Index","Home");
                }
               
            }
            return View();
        }
    }
}