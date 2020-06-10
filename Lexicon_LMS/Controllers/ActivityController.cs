using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon_LMS.Controllers
{
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext context;
        public ActivityController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        // GET: Activity/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Activity/Create
        public ActionResult Create(int? moduleId)
        {
            if (context.Modules.Find(moduleId) == null) 
            {
                return NotFound();
            }
            var courseActivity = new CourseActivity { ModuleId = (int)moduleId };
            return View(courseActivity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("ActivityName,ActivityDescription,StartDate,EndDate,ActivityTypeId,ModuleId")]CourseActivity courseActivity)
        {
            if (ModelState.IsValid)
            {
                var module = context.Modules.Find(courseActivity.ModuleId);
                if (module != null)
                {                 
                    try
                    {
                        context.Activities.Add(courseActivity);
                        module.Activities.Add(courseActivity);
                        await context.SaveChangesAsync();

                        return RedirectToAction(nameof(Details), "Courses", new { Id = module.CourseId });
                    }
                    catch
                    {
                        return View(courseActivity);
                    }
                }
          
            }
            return View(courseActivity);
        }

        // GET: Activity/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Activity/Edit/5
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

        // GET: Activity/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Activity/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}