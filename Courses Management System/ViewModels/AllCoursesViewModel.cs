using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courses_Management_System.ViewModels
{
    public class AllCoursesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public int Students { get; set; }

        public DateTime Time { get; set; }

        public int Registered { get; set; }
    }
}