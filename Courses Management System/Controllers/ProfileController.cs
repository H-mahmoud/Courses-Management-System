using Courses_Management_System.App_Start;
using Courses_Management_System.Models;
using Courses_Management_System.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Courses_Management_System.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        #region Properties
        public CMSContext _context { get; set; }
        public RoleStore<IdentityRole> _rolestore{ get; set; }
        public RoleManager<IdentityRole> _rolemanager { get; set; }

        public UserStore<Users> _userstore { get; set; }
        public UserManager<Users> _usermanager { get; set; }
        #endregion

        // Constructor
        public ProfileController()
        {
            _context = new CMSContext();
            _rolestore = new RoleStore<IdentityRole>(_context);
            _rolemanager = new RoleManager<IdentityRole>(_rolestore);

            _userstore = new UserStore<Users>(_context);
            _usermanager = new UserManager<Users>(_userstore);
        }

        // GET: Profile
        public ActionResult Index()
        {

            if (User.IsInRole("Admin")) {
                var Teacher = _rolemanager.FindByName("Teacher");
                var Student = _rolemanager.FindByName("Student");
                ViewData["Teacher"] = Teacher.Id;
                ViewData["Student"] = Student.Id;
            }

            return View();
        }

        public ActionResult Update()
        {
            if (User.IsInRole("Admin"))
            {
                var Teacher = _rolemanager.FindByName("Teacher");
                var Student = _rolemanager.FindByName("Student");
                ViewData["Teacher"] = Teacher.Id;
                ViewData["Student"] = Student.Id;
            }

            var user = _usermanager.FindByName(User.Identity.Name);
            var profile = AutoMap.Mapper.Map<UpdateProfileViewModel>(user);

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UpdateProfileViewModel model)
        {

            if (User.IsInRole("Admin"))
            {
                var Teacher = _rolemanager.FindByName("Teacher");
                var Student = _rolemanager.FindByName("Student");
                ViewData["Teacher"] = Teacher.Id;
                ViewData["Student"] = Student.Id;
            }

            if (ModelState.IsValid) {

                try
                {
                    var user = _usermanager.FindByName(User.Identity.Name);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    if (model.NewPassword != null) {
                        var result = _usermanager.ChangePassword(user.Id, model.CurrentPassword, model.NewPassword);
                        if (!result.Succeeded) {
                            foreach(var error in result.Errors)
                                ModelState.AddModelError("Error", error);
                            throw new Exception();
                        }
                    }

                    _usermanager.Update(user);

                    return RedirectToAction("Logout", "Account");
                }
                catch (Exception e) {
                    ModelState.AddModelError("Error", "Failed to update profile.");
                }
            }

            return View();
        }
    }
}