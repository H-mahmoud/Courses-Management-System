using Courses_Management_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courses_Management_System.App_Start
{
    public class DataSeeding
    {

        public static void Seed()
        {
            CMSContext _context = new CMSContext();

            if (!_context.Roles.Any(x => x.Name == "Admin")) {

                RoleStore<IdentityRole> _store = new RoleStore<IdentityRole>(_context);
                RoleManager<IdentityRole> _manager = new RoleManager<IdentityRole>(_store);

                var admin = new IdentityRole { Name = "Admin" };
                var teacher = new IdentityRole { Name = "Teacher" };
                var student = new IdentityRole { Name = "Student" };

                _manager.Create(admin);
                _manager.Create(teacher);
                _manager.Create(student);
            }


            if (!_context.Users.Any(x => x.UserName == "Admin"))
            {

                UserStore<Users> _store = new UserStore<Users>(_context);
                UserManager<Users> _manager = new UserManager<Users>(_store);

                var user = new Users { UserName = "admin", FirstName = "Hassan", LastName = "Hassan", Email = "hassan@cms.com" };

                var result =_manager.Create(user, "HassanAdmin");
                if (result.Succeeded)
                    _manager.AddToRoles(user.Id, "Admin");
                else {
                    Console.WriteLine(result.Errors);
                }
            }
        }


    }
}