using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lexicon_LMS.ViewModels.Courses
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }

        public ICollection<Module> Modules { get; set; }

    }
}
