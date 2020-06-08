using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
