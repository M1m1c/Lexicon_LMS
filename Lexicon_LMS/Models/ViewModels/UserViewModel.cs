using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Models.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public int Age { get; set; }

        [Display(Name = "First name")]
        [Required]
        public String FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public String LastName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }

        [Display(Name = "Course name")]
        public string CourseName { get; set; }

        public int CourseId { get; set; }
    }
}

