using System;
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

        [Display(Name = "Level")]
        public string Description { get; set; }
        public int DifficultyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public ICollection<ModuleViewModel> Modules { get; set; }

        public int? ModuleId { get; set; }

        public ICollection<DocumentViewModel> Documents { get; set; }

    }
}
