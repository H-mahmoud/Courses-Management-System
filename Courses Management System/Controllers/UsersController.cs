
using Courses_Management_System.App_Start;
using Courses_Management_System.Models;
using Courses_Management_System.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Courses_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        #region Properties
        public CMSContext _context { get; set; }
        public UserStore<Users> _userstore { get; set; }
        public UserManager<Users> _usermanager { get; set; }

        public RoleStore<IdentityRole> _rolestore { get; set; }
        public RoleManager<IdentityRole> _rolemanager { get; set; }

        public IdentityRole TeacherRole { get; set; }
        public IdentityRole StudentRole { get; set; }

        #endregion

        // Constructor
        public UsersController()
        {
            _context = new CMSContext();
            _userstore = new UserStore<Users>(_context);
            _usermanager = new UserManager<Users>(_userstore);
            _rolestore = new RoleStore<IdentityRole>(_context);
            _rolemanager = new RoleManager<IdentityRole>(_rolestore);

            TeacherRole = _rolemanager.FindByName("Teacher");
            StudentRole = _rolemanager.FindByName("Student");
        }


        // GET: Users
        public ActionResult Index(string Id)
        {
            var role = _rolemanager.FindById(Id);
            if (role != null)
            {
                var users = _context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).ToList();

                ViewBag.Users = users;
                ViewBag.Role = role;

                ViewData["Teacher"] = TeacherRole.Id;
                ViewData["Student"] = StudentRole.Id;
                return View();

            }

            return RedirectToAction("Index", "Profile");
        }

        public ActionResult Add(string Id)
        {
            var role = _rolemanager.FindById(Id);
            if (role != null) {
                ViewBag.Role = role;

                ViewData["Teacher"] = TeacherRole.Id;
                ViewData["Student"] = StudentRole.Id;

                return View();
            }

            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string Id, AddUserViewModel UserModel)
        {
            var role = _rolemanager.FindById(Id);
            if (role == null)
                return RedirectToAction("Index", "Profile");

            if (ModelState.IsValid) {
                try
                {
                    var user = AutoMap.Mapper.Map<Users>(UserModel);
                    user.UserName = user.Email.ToLower();
                    var result = _usermanager.Create(user, UserModel.Password);
                    if (result.Succeeded)
                    {
                        _usermanager.AddToRole(user.Id, role.Name);
                        return RedirectToAction("Index", new { Id = Id });
                    }
                    else 
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("Error", error);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            ViewBag.Role = role;
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            return View();
        }

        public ActionResult Delete(string Id)
        {
            var user = _usermanager.FindById(Id);
            if (user != null)
            {
                ViewBag.User = user;

                ViewData["Teacher"] = TeacherRole.Id;
                ViewData["Student"] = StudentRole.Id;

                return View();
            }

            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(string Id)
        {

            var user = _usermanager.FindById(Id);
            if (user == null)
                return RedirectToAction("Index", "Profile");

            var role = user.Roles.Select(x => x.RoleId).FirstOrDefault();
            var result = _usermanager.Delete(user);
            if (result.Succeeded)
                return RedirectToAction("Index", new { Id = role });

            ViewBag.User = user;
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            return View("Delete");
        }

    }
}