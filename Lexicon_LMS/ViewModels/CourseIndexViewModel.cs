using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.ViewModels
{
    public class CourseIndexViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Course")]
        public string CourseName { get; set; }

        [StringLength(1000)]
        [Display(Name = "Level")]
        public string Description { get; set; }
        
        [Display(Name = "Start date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }
    }
}
