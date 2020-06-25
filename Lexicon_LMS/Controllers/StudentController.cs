﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexicon_LMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;


        public StudentController(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public async Task<bool>  HasCourse()
        {
            var retflag = true;
            var user=await userManager.GetUserAsync(User);

            if (user.CourseId==null)
            {
                retflag = false;
               // return RedirectToPage("~/Views/Student/StudentStartPartial.cshtml");
            }

            return retflag;
        }

        public IActionResult HasNoCourse()
        {
            return View();
        }

        public async Task<IActionResult> Course()
        {
            if (await HasCourse() == false)
            {
                return RedirectToAction(nameof(HasNoCourse));
            }

            return View();
        }
        public async Task<IActionResult> StudentStartPartial()
        {
            var currentUserId = userManager.GetUserId(User);
            var today = DateTime.Now;
            var studentCourse = await context.Users.Include(u => u.Course).Where(u => currentUserId == u.Id).FirstOrDefaultAsync();
            var modules = await context.Modules.Include(m => m.Activities).ToListAsync();
            var activities = await context.Activities.Include(a => a.Documents).ToListAsync();
            var assignmentType = await context.ActivityTypes.FirstOrDefaultAsync(t => t.Name == "Assignment");
            var myDocument = context.Documents.Where(d => d.UserId == currentUserId);

            var currentModule = modules.FirstOrDefault(m => m.CourseId == studentCourse.CourseId && m.StartDate <= today && m.EndDate >= today);
            var pastModules = modules.Where(m => m.CourseId == studentCourse.CourseId && m.EndDate < today).ToList();
            var futureModules = modules.Where(m => m.CourseId == studentCourse.CourseId && m.StartDate > today).ToList();
            var ongoingActivities = activities.Where(a => a.StartDate <= today && a.EndDate >= today && a.Module.CourseId == studentCourse.CourseId && !myDocument.Any(d => d.ActivityId == a.ActivityTypeId));
            var ongoingAssignments = ongoingActivities?.Where(a => a.ActivityTypeId == assignmentType.Id && !myDocument.Any(d => d.ActivityId == a.ActivityTypeId));
            var lateAssignments = activities.Where(a => a.EndDate < today && a.ActivityTypeId == assignmentType.Id && (a.Documents.FirstOrDefault(d => d.UserId == currentUserId) == null));

            foreach (var item in ongoingActivities)
            {
                var type = await context.ActivityTypes.FindAsync(item.ActivityTypeId);
                item.ActivityType = type;
            }

            var viewModel = new StudentStartViewModel
            {
                CourseId = (int)studentCourse.CourseId,
                CourseName = studentCourse.Course.CourseName,
                CurrentModule = currentModule,
                PastModules = pastModules,
                FutureModules = futureModules,
                FutureActivites = null,
                PastActivities = null,
                OnGoingActivites = ongoingActivities,
                OnGoingAssignments = ongoingAssignments,
                LateAssignments = lateAssignments
            };


            IQueryable<Document> myDocs = context.Documents
                .Include(d => d.Course).Include(d => d.Module)
                .Include(d => d.Activity).Include(d => d.User)
                .Where(d => d.UserId == currentUserId);
            viewModel.MyDocuments = myDocs.ToList();

            return View(viewModel);


        }



    }
}