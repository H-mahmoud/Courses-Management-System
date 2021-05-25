using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Courses_Management_System.Models
{
    public class Courses
    {
        [Key]
        public string Id{ get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }

        public virtual ICollection<Users> Users { get; set; }

    }
}