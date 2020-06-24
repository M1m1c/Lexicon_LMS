using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.ViewModels;
using AutoMapper;
using Lexicon_LMS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Lexicon_LMS.ViewModels.Courses;
using Lexicon_LMS.Models.ViewModels;

namespace Lexicon_LMS.Controllers
{

    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DocumentController _documentController;

        public CoursesController(ApplicationDbContext context, IMapper mapper, IUnitOfWork unitOfWork, DocumentController documentController)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _documentController = documentController;
        }

        // GET: Courses
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            var model = _context.Courses
                    .Include(c => c.Difficulties)
                    .Select(c => new CourseIndexViewModel
                    {
                        Id = c.Id,
                        CourseName = c.CourseName,
                        Description = c.Difficulties.Level,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    });

            return View(await model.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, int? moduleId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _unitOfWork.CourseRepository.GetDetailsViewModelAsync(id);
            model.Description = _context.Difficulties.Find(model.DifficultyId).Level;
            model.Documents = _mapper.Map<ICollection<DocumentViewModel>>(_context.Documents.Where(d => d.CourseId == model.Id));

            if (moduleId != null)
            {
                model.ModuleId = moduleId;
            }

            foreach (var mod in model.Modules)
            {
                var activeties = await _context.Activities.Where(a => a.ModuleId == mod.Id).ToListAsync();
                mod.Activities = _mapper.Map<IEnumerable<CourseActivityViewModel>>(activeties);
                mod.Documents = _mapper.Map<ICollection<DocumentViewModel>>(_context.Documents.Where(d => d.ModuleId == mod.Id));

                foreach (var act in mod.Activities)
                {
                    var result = await _context.ActivityTypes.FindAsync(int.Parse(act.ActivityTypeId));
                    act.ActivityTypeName = result.Name;
                    act.CourseId = (int)id;
                }
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = _mapper.Map<Course>(model);
                _unitOfWork.CourseRepository.Add(course);
                await _unitOfWork.CompleateAsync();
                TempData["UserMessage"] = $"Course: {model.CourseName} - was added.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseName,Description,DifficultyId,StartDate,EndDate")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    TempData["UserMessage"] = $"Course: {course.CourseName} - Saved changes.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id });
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Difficulties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var course = await _context.Courses.FindAsync(id);
            var modules = _context.Modules.Where(m => m.CourseId == id);
            var activities = _context.Activities.Where(a => modules.Any(m => m.Id == a.ModuleId));

            var users = _context.Users.Where(u => u.CourseId == id);
            foreach (var u in users)
            {
                _context.Remove(u);
            }
            //await _context.SaveChangesAsync();

            var courseDocs = _context.Documents.Where(d => d.CourseId == id);
            var moduleDocs = _context.Documents.Where(d => modules.Any(m => d.ModuleId == m.Id));
            var activityDocs = _context.Documents.Where(d => activities.Any(a => d.ActivityId == a.Id));

            List<Document> allDocs = new List<Document>();
            allDocs.AddRange(courseDocs);
            allDocs.AddRange(moduleDocs);
            allDocs.AddRange(activityDocs);

            var fileDeletionSuccess = await _documentController.CallDeletionOfFiles(allDocs);
            if (fileDeletionSuccess == true)
            {
                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();

            //TempData["UserMessage"] = $"Course: {course.CourseName} - was deleted.";
           
        }



        public IActionResult AddParticipant(int courseId)
        {
            return RedirectToAction("CreateUser", "Teacher", new { courseId = courseId });
        }


        public async Task<IActionResult> ShowPaticipants(int? courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.CourseId == courseId).ToListAsync();

            var model = _mapper.Map<IEnumerable<UserViewModel>>(users);
            model.Select(m => m.CourseName = course.CourseName);

            return View(model);

        }


        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
