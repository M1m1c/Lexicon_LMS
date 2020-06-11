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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lexicon_LMS.Controllers
{
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public ActivityController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        // GET: Activity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var activity = await context.Activities.FindAsync(id);

           
            if (activity == null)
            {
                return NotFound();
            }
            
            var model = mapper.Map<CourseActivityViewModel>(activity);         
            model.ActivityTypeName = context.ActivityTypes.Find(int.Parse(model.ActivityTypeId)).Name;

            return View(model);
        }

        private List<SelectListItem> GetActivityTypesForDropDown()
        {          
            return context.ActivityTypes.ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList(); ;
        }

        // GET: Activity/Create
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(int? moduleId)
        {
            if (context.Modules.Find(moduleId) == null) 
            {
                return NotFound();
            }
            var courseActivity = new CourseActivityViewModel
            {
                ModuleId = (int)moduleId,
                ActivityTypes = GetActivityTypesForDropDown()
                
            };
            return View(courseActivity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("ActivityName,ActivityDescription,StartDate,EndDate,ActivityTypeId,ModuleId")]CourseActivityViewModel courseActivity)
        {
            if (ModelState.IsValid)
            {
                var module = context.Modules.Find(courseActivity.ModuleId);
                var activity = mapper.Map<CourseActivity>(courseActivity);
                activity.ActivityTypeId = int.Parse(courseActivity.ActivityTypeId);

                if (module != null)
                {                 
                    try
                    {
                        context.Activities.Add(activity);
                        module.Activities.Add(activity);
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