using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Create(int? holderId, string userId, HolderTypeEnum holderType, IFormFile file, string url)
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

            var docViewModel = new DocumentViewModel()
            {
                HolderId = holderId,
                UserId = userId,
                User = user,
                HolderType = holderType,
                Name = file.FileName,
                UploadDate = DateTime.Now
            };

            string path="";
            var document = mapper.Map<Document>(docViewModel);

            switch (docViewModel.HolderType)
            {
                case HolderTypeEnum.Course:
                    document.CourseId = docViewModel.HolderId;
                    document.Course = await context.Courses.FindAsync(docViewModel.HolderId);
                    path = GetPath(document.Course,null,null,user.Id);
                    break;
                case HolderTypeEnum.Module:
                    document.ModuleId = docViewModel.HolderId;
                    document.Module = await context.Modules.FindAsync(docViewModel.HolderId);
                    var mCourse = await context.Courses.FindAsync(document.Module.CourseId);
                    path = GetPath(mCourse, document.Module, null, user.Id);
                    break;
                case HolderTypeEnum.Activity:
                    document.ActivityId = docViewModel.HolderId;
                    document.Activity = await context.Activities.FindAsync(docViewModel.HolderId);
                    var aModule= await context.Modules.FindAsync(document.Activity.ModuleId);
                    var aCourse = await context.Courses.FindAsync(aModule.CourseId);
                    path = GetPath(aCourse, aModule, document.Activity, user.Id);
                    break;
            }

            if (string.IsNullOrEmpty(path))
            {
                return NotFound();
            }

            var filePath = $"{path}\\{file.FileName}";
            //TODO check if the directory exists, otherwise create it

            await context.Documents.AddAsync(document);
            user.Documents.Add(document);
            await context.SaveChangesAsync();

            return RedirectToPage(url);
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

        private string GetPath(Course course,Module module,CourseActivity activity, string userId)
        {
            var basePath = hostingEnvironment.ContentRootPath + "\\Files";
            string path="";
            if (course != null && module != null && activity != null) 
            {
                //create the directory using /course/module/activity/userId/
                path = $"{basePath}\\{course.CourseName}\\{module.ModuleName}\\{activity.ActivityName}\\{userId}";
                
            }
            else if (course != null && module != null)
            {
                //create the directory using /course/module/userId/
                path = $"{basePath}\\{course.CourseName}\\{module.ModuleName}\\{userId}";
            }
            else if (course != null)
            {
                //create the directory using /course/userId/
                path = $"{basePath}\\{course.CourseName}\\{userId}";
            }
           
            return path;
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(DocumentViewModel viewModel)
        //{
        //    string path = hostingEnvironment.ContentRootPath;
        //    if (ModelState.IsValid)
        //    {
        //        var user = await context.Users.FindAsync(viewModel.UserId);
        //        var document = mapper.Map<Document>(viewModel);
        //        document.UploadDate = DateTime.Now;
        //        document.User = user;
        //        switch (viewModel.HolderType)
        //        {
        //            case HolderTypeEnum.Course:
        //                document.CourseId = viewModel.HolderId;
        //                document.Course = await context.Courses.FindAsync(viewModel.HolderId);
        //                break;
        //            case HolderTypeEnum.Module:
        //                document.ModuleId = viewModel.HolderId;
        //                document.Module = await context.Modules.FindAsync(viewModel.HolderId);
        //                break;
        //            case HolderTypeEnum.Activity:
        //                document.ActivityId = viewModel.HolderId;
        //                document.Activity = await context.Activities.FindAsync(viewModel.HolderId);
        //                break;
        //        }
        //        await context.Documents.AddAsync(document);
        //        user.Documents.Add(document);
        //        await context.SaveChangesAsync();
        //        return RedirectToAction("Start", "User");
        //    }
        //    return View(viewModel);
        //}


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