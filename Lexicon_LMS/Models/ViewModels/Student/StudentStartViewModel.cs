
using Lexicon_LMS.Models;
using System;
using Lexicon_LMS.Models.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class StudentStartViewModel
    {
        public Module CurrentModule { get; set; }
        public List<Module> PastModules { get; set; }
        public List<Module> FutureModules { get; set; }
        public List<CourseActivity> PastActivities { get; set; }
        public IEnumerable<CourseActivity> OnGoingActivites { get; set; }
        public List<CourseActivity> FutureActivites { get; set; }

        public IEnumerable<CourseActivity> LateAssignments { get; set; }
        public IEnumerable<CourseActivity> OnGoingAssignments { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<Document> MyDocuments { get; set; }
    }
}
