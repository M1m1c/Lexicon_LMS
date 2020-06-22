using Lexicon_LMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }

        public string UserId { get; set; }

        public int? HolderId { get; set; }
        public HolderTypeEnum HolderType { get; set; }

        public User User { get; set; }
        public string UpploaderName { get; set; }

        public string NewComments { get; set; }

        public ICollection<AssignmentComments> Comments { get; set; }
    }

  
}
