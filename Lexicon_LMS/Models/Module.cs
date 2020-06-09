using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }


        public int CourseId { get; set; }

        public Course Course { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<CourseActivity> Activities { get; set; }
    }
}
