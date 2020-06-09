using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult Details(int id)
        {
            return View();
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

        // GET: Module/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Module/Delete/5
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