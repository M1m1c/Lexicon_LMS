using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.ViewModels
{
    public class CourseCreateViewModel
    {
        [Required]
        [Display(Name = "Course name")]
        public string CourseName { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
    }
}
