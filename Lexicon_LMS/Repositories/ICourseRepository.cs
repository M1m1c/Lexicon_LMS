using Lexicon_LMS.Models;
using Lexicon_LMS.ViewModels.Courses;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public interface ICourseRepository
    {
        void Add(Course course);
        Task<CourseDetailsViewModel> GetDetailsViewModelAsync(int? id);
        Course GetCourseById(int courseId);
    }
}
