using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models
{
    public class AssignmentComments
    {
        public int Id { get; set; }

        public string Comments { get; set; }

        public DateTime Date { get; set; }


        //F keys
        public string UserId { get; set; }
        public int DocumentId { get; set; }

        //Nav prop

        public User User { get; set; }
        public Document Document { get; set; }

    }
}
