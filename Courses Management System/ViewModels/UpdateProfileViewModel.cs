using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Courses_Management_System.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required]
        [MaxLength(30)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
    }
}