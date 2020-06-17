using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class CourseActivityViewModel
    {
        public int Id { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public string ActivityTypeName { get; set; }
        public string ActivityTypeId { get; set; }
        public IEnumerable<SelectListItem> ActivityTypes { get; set; }

        public ICollection<DocumentViewModel> TeacherDocuments { get; set; }
        public ICollection<DocumentViewModel> StudentDocuments { get; set; }
        public DocumentViewModel MyStudentDocument { get; set; }
    }
}
