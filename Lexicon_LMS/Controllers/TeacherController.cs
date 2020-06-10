﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public TeacherController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Users()
        {
            var users = await context.Users.ToListAsync();
            var currentUserId = userManager.GetUserId(User);

            var models = mapper.Map<IEnumerable<UserViewModel>>(users.Where(q => q.Id != currentUserId));

            foreach (var m in models)
            {
                m.Role = context.Roles.Find(context.UserRoles.FirstOrDefault(ur => ur.UserId == m.Id).RoleId).Name;
            }

            return View(models);
        }

        private List<SelectListItem> GetRolesForDropDown()
        {
            return context.Roles.ToList().Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList(); ;
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult CreateUser()
        {
            return View(new UserViewModel { Roles = GetRolesForDropDown() });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
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

                    if (role.Name == "Student")
                    {
                        //TODO make sure that this redirects to an existing action and controller
                        return RedirectToAction("AssignToCourse", addedUser.Id);
                    }

                    return RedirectToAction(nameof(Users));
                }

            }
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = mapper.Map<UserViewModel>(user);
            model.Role = context.Roles.Find(context.UserRoles.FirstOrDefault(ur => ur.UserId == model.Id).RoleId).Name;

            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = mapper.Map<EditUserViewModel>(user);
            model.Role = context.Roles.Find(context.UserRoles.FirstOrDefault(ur => ur.UserId == model.Id).RoleId).Name;
            model.Roles = GetRolesForDropDown();
            return View(model);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditUser(string id, [Bind("Age,FirstName,LastName,Role")] EditUserViewModel editUserVM)
        {
            if (!context.Users.Any(u => u.Id == id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await context.Users.FindAsync(id);
                user.FirstName = editUserVM.FirstName;
                user.LastName = editUserVM.LastName;
                user.Age = editUserVM.Age;

                var role = await roleManager.FindByNameAsync(editUserVM.Role);
                if (role != null) 
                {
                    try
                    {
                        context.Update(user);
                       
                        if (!await userManager.IsInRoleAsync(user, role.Name))
                        {
                            var addToRoleResult = await userManager.AddToRoleAsync(user, role.Name);

                            if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
                        }

                        var test = await userManager.GetRolesAsync(user);
                        if ( test.Count > 1) 
                        {
                            await userManager.RemoveFromRolesAsync(user, test.Where(t => t != role.Name));
                        }

                        await context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Users));
                }
            }
            return View(editUserVM);
        }
        private bool UserExists(string id)
        {
            return context.Users.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = mapper.Map<UserViewModel>(user);
            model.Role = context.Roles.Find(context.UserRoles.FirstOrDefault(ur => ur.UserId == model.Id).RoleId).Name;
            return View(model);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await context.Users.FindAsync(id);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
    }
}