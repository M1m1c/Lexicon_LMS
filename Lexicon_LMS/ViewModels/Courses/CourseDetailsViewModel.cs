﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;

namespace Lexicon_LMS.ViewModels.Courses
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public ICollection<ModuleViewModel> Modules { get; set; }

    }
}
