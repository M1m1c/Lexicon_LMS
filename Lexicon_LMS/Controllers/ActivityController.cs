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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly DocumentController _documentController;
        public ActivityController(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, DocumentController documentController)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            _documentController = documentController;
        }
        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        // GET: Activity/Details/5
        public async Task<IActionResult> Details(int? id, int? courseId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await context.Activities.FindAsync(id);
            var module = await context.Modules.FindAsync(activity.ModuleId);
            var course = await context.Courses.FindAsync(module.CourseId);

            if (activity == null)
            {
                return NotFound();
            }
            var model = ToCourseActivityViewModel(activity, courseId);
            model.ModuleName = module.ModuleName;
            model.CourseName = course.CourseName;

            var documents = context.Documents.Where(d => d.ActivityId == activity.Id);

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    foreach (var user in context.Users)
                    {
                        if (await userManager.IsInRoleAsync(user, "Teacher") && doc.UserId == user.Id)
                        {
                            model.TeacherDocuments.Add(mapper.Map<DocumentViewModel>(doc));
                        }
                        else if (User.IsInRole("Teacher") && await userManager.IsInRoleAsync(user, "Student") && doc.UserId == user.Id)
                        {
                            var temp = mapper.Map<DocumentViewModel>(doc);
                            temp.UpploaderName = user.Email;
                            model.StudentDocuments.Add(temp);
                        }
                    }
                }
            }

            var tempdoc = documents.FirstOrDefault(d => d.UserId == userManager.GetUserId(User));

            if (User.IsInRole("Student") && tempdoc != null)
            {
                model.MyStudentDocument = mapper.Map<DocumentViewModel>(tempdoc);
            }

            // model.Documents = mapper.Map<ICollection<DocumentViewModel>>(context.Documents.Where(d => d.ActivityId == id));

            return View(model);
        }

        private List<SelectListItem> GetActivityTypesForDropDown()
        {
            return context.ActivityTypes.ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList(); ;
        }

        // GET: Activity/Create
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(int? moduleId)
        {


            var module = await context.Modules.FindAsync(moduleId);
            var course = await context.Courses.FindAsync(module.CourseId);
            if (module == null)
            {
                return NotFound();
            }

            var courseActivity = new CourseActivityViewModel
            {
                ModuleId = (int)moduleId,
                CourseName = course.CourseName,
                ModuleName = module.ModuleName,
                StartDate = module.StartDate,
                EndDate = module.EndDate,
                ActivityTypes = GetActivityTypesForDropDown(),
                CourseId = module.CourseId
            };
            return View(courseActivity);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("ActivityName,ActivityDescription,StartDate,EndDate,ActivityTypeId,ModuleId")] CourseActivityViewModel courseActivity)
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
                        TempData["UserMessage"] = $"Activity: {activity.ActivityName} - was added.";
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
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id, int? courseId)
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
            return View(ToCourseActivityViewModel(activity, courseId));
        }

        // POST: Activity/Edit/5
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseActivityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var activity = mapper.Map<CourseActivity>(model);
                try
                {
                    context.Update(activity);
                    await context.SaveChangesAsync();
                    TempData["UserMessage"] = $"Activity: {activity.ActivityName} - Saved changes.";
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!context.Activities.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id, model.CourseId });
            }
            return View(model);
        }

        // GET: Activity/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id, int? courseId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await context.Activities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(ToCourseActivityViewModel(activity, courseId));
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? courseId)
        {
            var activity = await context.Activities.FindAsync(id);
            var docs = context.Documents.Where(d => d.ActivityId == id);
            var fileDeletionSuccess = await _documentController.CallDeletionOfFiles(docs.ToList());
            if (fileDeletionSuccess == true)
            {
                context.Activities.Remove(activity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Courses", new { id = (int)courseId });
            }
            return NotFound();

            //TempData["UserMessage"] = $"Activity: {activity.ActivityName} - was deleted.";
           
        }


        private CourseActivityViewModel ToCourseActivityViewModel(CourseActivity act, int? courseId)

        {
            CourseActivityViewModel ret = mapper.Map<CourseActivityViewModel>(act);
            ret.TeacherDocuments = new List<DocumentViewModel>();
            ret.StudentDocuments = new List<DocumentViewModel>();
            ret.ActivityTypeName = context.ActivityTypes.Find(act.ActivityTypeId).Name;
            ret.ActivityTypes = GetActivityTypesForDropDown();
            ret.CourseId = (int)courseId;
            ret.ModuleId = (int)act.ModuleId;
            ret.ActivityId = act.Id;
            return ret;
        }


        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyActivityStartDate(DateTime startDate, int moduleId)
        {

            var module = await context.Modules.FindAsync(moduleId);
            if ((startDate - module.StartDate).TotalSeconds < 0)
            {
                return Json($"Activity start date can't be before Module start date");
            }
            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyActivityEndDate(DateTime endDate, int moduleId)
        {

            var module = await context.Modules.FindAsync(moduleId);
            if (endDate > module.EndDate)
            {
                return Json($"Activity end date can't be after Module end date");
            }
            return Json(true);
        }
    }
}