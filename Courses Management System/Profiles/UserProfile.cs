using AutoMapper;
using Courses_Management_System.Models;
using Courses_Management_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courses_Management_System.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AddUserViewModel, Users>();
            CreateMap<Users, UpdateProfileViewModel>();
        }
    }
}