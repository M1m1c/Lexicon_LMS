using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ModuleId { get; set; }
        public int ActivityTypeId { get; set; }

        public Module Module { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ActivityType ActivityType { get; set; }


    }
}
