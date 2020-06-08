using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Lexicon_LMS.ViewModels.Courses;
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

        public Task<CourseDetailsViewModel> GetDetailsViewModelAsync(int id)
        {
          
        }
    }
}
