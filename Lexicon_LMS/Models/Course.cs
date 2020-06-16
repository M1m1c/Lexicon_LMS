using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lexicon_LMS.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        [Display(Name = "Level")]
        public string Description { get; set; }
        public int DifficultyId { get; set; }
        public Difficulty Difficulties { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Document> Documents { get; set; }


    }
}
