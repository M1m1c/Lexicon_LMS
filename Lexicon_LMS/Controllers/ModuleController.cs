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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public ModuleController(ApplicationDbContext context, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        // GET: Module
        public ActionResult Index()
        {
            return View();
        }

        // GET: Module/ModuleDetails/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await context.Modules.FirstOrDefaultAsync(a => a.Id == id);
            if (module is null)
            {
                return NotFound();
            }
            var course = await context.Courses.FirstOrDefaultAsync(a => a.Id == module.CourseId);
            if (course is null)
            {
                return NotFound();
            }
            var model = new ModuleDetailsViewModel()
            {
                CourseId = course.Id,
                CourseName = course.CourseName,
                Id = module.Id,
                ModuleName = module.ModuleName,
                Description = module.Description,
                StartDate = module.StartDate,
                EndDate = module.EndDate,
                ParentStartDate = course.EndDate,
                ParentEndDate = course.EndDate
            };
            model.CourseId = course.Id;
            model.CourseName = course.CourseName;
            return View(model);
        }

        // GET: Module/Create
        public ActionResult ModuleCreate(int? Courseid)
        {
            if (Courseid == null)
            {
                return NotFound();
            }
            var course = context.Courses.Include(c => c.Users).FirstOrDefault(c => c.Id == Courseid);
           
            var model = new ModuleViewModel { CourseId = (int)Courseid };

            model.StartDate = course.StartDate;
            model.EndDate = course.EndDate.AddDays(-1);
            return View(model);
        }

        // POST: Module/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModuleCreate([Bind("ModuleName,Description,StartDate,EndDate,CourseId")]ModuleViewModel moduleViewModel)
        {         
            if (ModelState.IsValid)
            {
                var module = mapper.Map<Module>(moduleViewModel);

                try
                {
                    // TODO: Add insert logic here
                    context.Add(module);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Details", "Courses", new { Id = moduleViewModel.CourseId });
                }
                catch
                {
                    return View(moduleViewModel);
                }

            }
            return View(moduleViewModel);
        }

     

        // GET: Activity/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }
            var model = mapper.Map<ModuleViewModel>(module);
            return View(model);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ModuleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var module = mapper.Map<Module>(model);
                try
                {
                    context.Update(module);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!context.Modules.Any(m => m.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details),"Courses", new { Id=model.CourseId });
            }
            return View(model);
        }


        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var module = await context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var module = await context.Modules.FindAsync(id);
            var courseId = module.CourseId;
            context.Modules.Remove(module);
            await context.SaveChangesAsync();
            return RedirectToAction("Details","Courses", new { Id = courseId });
        }

        public IActionResult AddParticipant(int courseId)
        {
            return RedirectToAction("CreateUser", "Teacher", new { courseId = courseId });
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyStartDate(DateTime startDate, int courseId)
        {
            var course = unitOfWork.CourseRepository.GetCourseById(courseId);
       
            if ((startDate - course.StartDate).TotalSeconds < 0)
            {
                return Json($"Module date can't be before course start date");
            }         
            return Json(true);
        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEndDate(DateTime endDate, int courseId)
        {
            var course = unitOfWork.CourseRepository.GetCourseById(courseId);

            if ((endDate - course.EndDate).TotalSeconds > 0)
            {
                return Json($"Module End date can't be after course End date");
            }
            return Json(true);
        }
    }
}