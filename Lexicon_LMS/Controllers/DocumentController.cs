using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public DocumentController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        // GET: Courses/Create
        public async Task<IActionResult> Create(int? holderId, string userId, HolderTypeEnum holderType)
        {
           
            if (await DoesTypeWithIdExist(holderType, holderId) == false)
            {
                return NotFound();
            }
            var user=await context.Users.FindAsync(userId);

            if (user==null)
            {
                return NotFound();
            }

            var doc = new DocumentViewModel()
            {
                HolderId = holderId,
                UserId = userId,
                User = user,
                HolderType = holderType
            };

            return View(doc);
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

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel viewModel)
        { 
            if (ModelState.IsValid)
            {
                var user = await context.Users.FindAsync(viewModel.UserId);
                var document = mapper.Map<Document>(viewModel);
                document.UploadDate = DateTime.Now;
                document.User = user;
                switch (viewModel.HolderType)
                {
                    case HolderTypeEnum.Course:
                        document.CourseId = viewModel.HolderId;
                        document.Course= await context.Courses.FindAsync(viewModel.HolderId);
                        break;
                    case HolderTypeEnum.Module:
                        document.ModuleId = viewModel.HolderId;
                        document.Module = await context.Modules.FindAsync(viewModel.HolderId);
                        break;
                    case HolderTypeEnum.Activity:
                        document.ActivityId = viewModel.HolderId;
                        document.Activity = await context.Activities.FindAsync(viewModel.HolderId);
                        break;
                }
                await context.Documents.AddAsync(document);
                user.Documents.Add(document);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),"Courses");
            }
            return View(viewModel);
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