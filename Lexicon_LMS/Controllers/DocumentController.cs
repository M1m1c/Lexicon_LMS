using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment hostingEnvironment;
        public DocumentController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            this.context = context;
            this.mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }


        // GET: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? holderId, HolderTypeEnum holderType,string userId, IFormFile file, string url)
        {

            if (await DoesTypeWithIdExist(holderType, holderId) == false)
            {
                return NotFound();
            }

            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            Document document = await InstantiateDocument(holderId, holderType, file, user);

            if (string.IsNullOrEmpty(document.FilePath))
            {
                return NotFound();
            }
           
            using (var stream = new FileStream(document.FilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var alreadyExisitngDoc = context.Documents.FirstOrDefault(d => d.FilePath == document.FilePath);
            if (alreadyExisitngDoc==null)
            {
                await context.Documents.AddAsync(document);
                user.Documents.Add(document);
                
            }
            else
            {
                try
                {
                    alreadyExisitngDoc.UploadDate = DateTime.Now;
                    context.Documents.Update(alreadyExisitngDoc);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            await context.SaveChangesAsync();
            return Redirect(url);
        }

        private async Task<Document> InstantiateDocument(int? holderId, HolderTypeEnum holderType, IFormFile file, User user)
        {
            var document = new Document()
            {
                UserId = user.Id,
                User = user,
                Name = file.FileName,
                UploadDate = DateTime.Now
            };

            string path = "";
            
            switch (holderType)
            {
                case HolderTypeEnum.Course:
                    document.CourseId = holderId;
                    document.Course = await context.Courses.FindAsync(holderId);
                    path = GetPath(document.Course, null, null, user.Id);
                    break;
                case HolderTypeEnum.Module:
                    document.ModuleId = holderId;
                    document.Module = await context.Modules.FindAsync(holderId);
                    var mCourse = await context.Courses.FindAsync(document.Module.CourseId);
                    path = GetPath(mCourse, document.Module, null, user.Id);
                    break;
                case HolderTypeEnum.Activity:
                    document.ActivityId = holderId;
                    document.Activity = await context.Activities.FindAsync(holderId);
                    var aModule = await context.Modules.FindAsync(document.Activity.ModuleId);
                    var aCourse = await context.Courses.FindAsync(aModule.CourseId);
                    path = GetPath(aCourse, aModule, document.Activity, user.Id);
                    break;
            }
            document.FilePath = $"{path}{file.FileName}";

            return document;
        }

        private async Task<bool> DoesTypeWithIdExist(HolderTypeEnum holderType, int? holderId)
        {
            bool retflag = false;
            switch (holderType)
            {
                case HolderTypeEnum.Course:
                    if (await context.Courses.FindAsync(holderId) != null)
                    {
                        retflag = true;
                    }
                    break;
                case HolderTypeEnum.Module:
                    if (await context.Modules.FindAsync(holderId) != null)
                    {
                        retflag = true;
                    }
                    break;
                case HolderTypeEnum.Activity:
                    if (await context.Activities.FindAsync(holderId) != null)
                    {
                        retflag = true;
                    }
                    break;
            }
            return retflag;
        }

        private string GetPath(Course course, Module module, CourseActivity activity, string userId)
        {
            var basePath = Directory.GetCurrentDirectory() + "\\wwwroot" + "\\Files";
            string path = "";
            if (course != null && module != null && activity != null)
            {
                //create the directory using /course/module/activity/userId/
                path = $"{basePath}\\{course.CourseName}\\{module.ModuleName}\\{activity.ActivityName}\\{userId}\\";

            }
            else if (course != null && module != null)
            {
                //create the directory using /course/module/userId/
                path = $"{basePath}\\{course.CourseName}\\{module.ModuleName}\\{userId}\\";
            }
            else if (course != null)
            {
                //create the directory using /course/userId/
                path = $"{basePath}\\{course.CourseName}\\{userId}\\";
            }

            return path;
        }

      

        // GET: Activity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var doc = await context.Documents.FindAsync(id);


            if (doc == null)
            {
                return NotFound();
            }
            var model = mapper.Map<DocumentViewModel>(doc);
            var user = await context.Users.FindAsync(doc.UserId);
            model.UpploaderName = user.Email;
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await context.Documents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return NotFound();
            }
            var model = mapper.Map<DocumentViewModel>(doc);
            var user = await context.Users.FindAsync(doc.UserId);
            model.UpploaderName = user.Email;
            return View(model);
        }

        // POST: Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await context.Documents.FindAsync(id);
            context.Documents.Remove(doc);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Courses");
        }
    }
}