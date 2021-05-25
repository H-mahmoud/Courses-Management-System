using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Courses_Management_System.ViewModels
{
    public class AddCourseViewModel
    {
        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Scheduled Time")]
        [DataType("datetime-local")]
        public DateTime ScheduledTime { get; set; }

        [Required]
        [MaxLength(256)]
        [DisplayName("Teacher Username")]
        public string TeacherUsername { get; set; }
    }
}