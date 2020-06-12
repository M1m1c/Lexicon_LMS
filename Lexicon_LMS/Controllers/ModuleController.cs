using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
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


        public ModuleController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            
        }
        // GET: Module
        public ActionResult Index()
        {
            return View();
        }

        // GET: Module/Details/5
        public async Task<ActionResult> ModuleDetails(int? id)
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

            return View(new ModuleViewModel { CourseId = (int)Courseid });
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

                    return RedirectToAction(nameof(Details), "Courses", new { Id = moduleViewModel.CourseId });
                }
                catch
                {
                    return View(moduleViewModel);
                }

            }
            return View(moduleViewModel);
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Module/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
            return RedirectToAction(nameof(Details),"Courses", new { Id = courseId });
        }

        public IActionResult AddParticipant(int courseId)
        {
            return RedirectToAction("CreateUser", "Teacher", new { courseId = courseId });
        }
    }
}