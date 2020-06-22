
using Lexicon_LMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.ViewModels.Student
{
    public class StudentStartViewModel
    {
        public List<Module> OnGoingModules { get; set; }
        public List<Module> PastModules { get; set; }
        public List<Module> FutureModules { get; set; }
        public List<CourseActivity> PastActivities { get; set; }
        public IEnumerable<CourseActivity> OnGoingActivites { get; set; }
        public List<CourseActivity> FutureActivites { get; set; }

        public IEnumerable<CourseActivity> LateAssignments { get; set; }
        public IEnumerable<CourseActivity> OnGoingAssignments { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
}
