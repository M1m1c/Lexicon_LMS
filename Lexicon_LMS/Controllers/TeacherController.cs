using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon_LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public TeacherController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

       
        public IActionResult Index()
        {
            return View();
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