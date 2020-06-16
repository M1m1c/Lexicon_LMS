using Lexicon_LMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Services
{
    public class LookupService : ILookupService
    {
        private readonly ApplicationDbContext _context;
        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SelectListItem> GetLevels() =>

            _context.Difficulties
                .Select(d =>
                          new SelectListItem()
                          {
                              Value = d.Id.ToString(),
                              Text = d.Level
                          })
                .ToList();
    }
}