﻿
using Lexicon_LMS.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class ModuleViewModel
    {

        public int Id { get; set; }
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }
        public string Description { get; set; }

        [Remote(action: "VerifyStartDate", controller: "Module", AdditionalFields = nameof(CourseId))]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        [AfterStartDate()]
        [Remote(action: "VerifyEndDate", controller: "Module", AdditionalFields = nameof(CourseId))]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
        public ICollection<DocumentViewModel> Documents { get; set; }
        public IEnumerable<CourseActivityViewModel> Activities { get; set; }
    }

  
}
