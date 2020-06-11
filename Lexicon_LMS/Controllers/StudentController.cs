using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        
    }
}