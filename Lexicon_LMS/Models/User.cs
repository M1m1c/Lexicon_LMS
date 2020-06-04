using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class User : IdentityUser
    {
        public int Age { get; set; }
              
        public int? CourseId { get; set; }

        public Course Course { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
