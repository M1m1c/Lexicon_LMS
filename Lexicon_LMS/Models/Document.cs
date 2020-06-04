using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }

        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public int ActivityId { get; set; }

        public User User { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
        public CourseActivity Activity { get; set; }
    }
}
