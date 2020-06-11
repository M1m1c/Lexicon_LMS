﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class TeacherPageViewModel
    {
        public int CourseId { get; set; }
        public List<Course> OnGoingCourses { get; set; }
        public List<Course> pastCourses { get; set; }
        public List<Course> futureCourses { get; set; }
        public List<Module> onGoingModule { get; set; }
        

    }
}
