using AutoMapper;
using Courses_Management_System.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courses_Management_System.App_Start
{
    public class AutoMap
    {
        public static IMapper Mapper { get; set; }
        public static void Configure() {

            var config = new MapperConfiguration( option => {
                            option.AddProfile(new UserProfile());
                            option.AddProfile(new CourseProfile());
                        });

            Mapper = config.CreateMapper();


        }
    }
}