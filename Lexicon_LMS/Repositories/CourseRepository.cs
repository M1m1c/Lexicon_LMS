using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.ViewModels.Courses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private ApplicationDbContext _db;
        private IMapper _mapper;

        public CourseRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void Add(Course course)
        {
            _db.Add(course);
        }

        public async Task<CourseDetailsViewModel> GetDetailsViewModelAsync(int? id)
        {
            return await _mapper.ProjectTo<CourseDetailsViewModel>(_db.Courses).FirstOrDefaultAsync(model => model.Id == id);
        }
    }
}
