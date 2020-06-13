using Lexicon_LMS.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.ViewModels.Courses
{
    public class CourseCreateViewModel
    {
        [Required]
        [Display(Name = "Course name")]
        public string CourseName { get; set; }
        
        [StringLength(1000)]
        [Display(Name = "Level")]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [CurrentDate(ErrorMessage = "Date can't be before the current date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [AfterStartDate]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

    }
}
