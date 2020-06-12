using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class ModuleDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Module Starts")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Module Ends")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        //TODO: Add Activities and Documents View Models
       


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Course Starts")]
        public DateTime ParentStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Course Ends")]
        public DateTime ParentEndDate { get; set; }
    }
}