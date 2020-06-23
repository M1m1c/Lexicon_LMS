using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Lexicon_LMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Lexicon_LMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration _configuration;

        public TeacherController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Users()
        {
            var users = await context.Users.Include(u => u.Course).ToListAsync();
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
        public IActionResult CreateUser(int? courseId)
        {
            if (courseId != null)
            {
                var course = unitOfWork.CourseRepository.GetCourseById((int)courseId);
                var model = new UserViewModel { Role = "Student", CourseName = course.CourseName, CourseId = course.Id, Roles = GetRolesForDropDown() };
                return View(model);
            }
            return View(new UserViewModel { Roles = GetRolesForDropDown() });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateUser(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
              //  var user = mapper.Map<User>(userViewModel);         
                var user =    new User
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
                    if (userViewModel.CourseId != 0 && role.Name == "Student")
                    {
                        user.CourseId = userViewModel.CourseId;
                    }

                    var result = await userManager.CreateAsync(user, _configuration["DefaultPassword"]);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

                    var addedUser = await userManager.FindByNameAsync(user.Email);

                    var addToRoleResult = await userManager.AddToRoleAsync(addedUser, role.Name);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));

                  
                    return RedirectToAction(nameof(Users));
                }

            }
            return View(userViewModel);
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

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherStartPartial()
        {
            var today = DateTime.Now;

            var user = await userManager.GetUserAsync(User);
            var role = await roleManager.FindByNameAsync("student");
            var students = context.UserRoles.Where(user => user.RoleId == role.Id);

            IQueryable<Course> onGoing = context.Courses.Where(c => c.StartDate <= today && c.EndDate > today);
            IQueryable<Module> module = context.Modules.Where(m => m.StartDate <= today && m.EndDate > today);
            IQueryable<Course> past = context.Courses.Where(c => c.EndDate < today);
            IQueryable<Course> future = context.Courses.Where(c => c.StartDate >= today);
            IQueryable<Document> assignments = context.Documents.Include(d => d.Course).Include(d => d.Module).Include(d => d.Activity).Include(d => d.User).Where(d => students.Any(s => s.UserId == d.UserId));
                
            var viewModel = new TeacherPageViewModel
            {
                OnGoingCourses = onGoing?.ToList(),
                onGoingModules = module?.ToList(),
                pastCourses = past?.ToList(),
                futureCourses = future?.ToList(),
                Assignments = assignments?.ToList()
            };
            return View(viewModel);
        }
    }
}