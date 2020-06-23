using Lexicon_LMS.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class ModuleEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Module Name")]
        [Required]
        public string ModuleName { get; set; }
        [Required]
        public string Description { get; set; }

        [Remote(action: "VerifyStartDateWithId", controller: "Module", AdditionalFields = "CourseId,EndDate,Id")]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        [AfterStartDate()]
        [Remote(action: "VerifyEndDateWithId", controller: "Module", AdditionalFields = "CourseId,StartDate,Id")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
        public ICollection<DocumentViewModel> Documents { get; set; }
        public IEnumerable<CourseActivityViewModel> Activities { get; set; }

        public ICollection<ModuleViewModel> UnavilableDates { get; set; }
    }
}
