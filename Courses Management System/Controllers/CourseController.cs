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
    public class CourseController : Controller
    {

        #region Properties
        public CMSContext _context { get; set; }

        public RoleStore<IdentityRole> _rolestore { get; set; }
        public RoleManager<IdentityRole> _rolemanager { get; set; }
        public UserStore<Users> _userstore { get; set; }
        public UserManager<Users> _usermanager { get; set; }

        public IdentityRole TeacherRole { get; set; }
        public IdentityRole StudentRole { get; set; }
        #endregion

        public CourseController()
        {
            _context = new CMSContext();
            _rolestore = new RoleStore<IdentityRole>(_context);
            _rolemanager = new RoleManager<IdentityRole>(_rolestore);

            _userstore = new UserStore<Users>(_context);
            _usermanager = new UserManager<Users>(_userstore);

            TeacherRole = _rolemanager.FindByName("Teacher");
            StudentRole = _rolemanager.FindByName("Student");
        }

        [Authorize(Roles = "Admin,Student")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["Teacher"] = TeacherRole.Id;
                ViewData["Student"] = StudentRole.Id;
            }

            int Teacher_Per_Course = 1;
            ViewBag.Courses = _context.Courses
                                .Select(x => new AllCoursesViewModel { 
                                    Id = x.Id,
                                    Name = x.Name,
                                    Time = x.ScheduledTime,
                                    Teacher = x.Users.Where(r => r.Roles.Select(c => c.RoleId).FirstOrDefault() == TeacherRole.Id).Select(y => y.UserName).FirstOrDefault(),
                                    // -1 if no students or teachers in the course
                                    Students = x.Users.Count() - Teacher_Per_Course,
                                    Registered = x.Users.Where(r => r.Roles.Select(c => c.RoleId).FirstOrDefault() == StudentRole.Id && r.UserName == User.Identity.Name).Count(),

                                })
                                .ToList();

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Add(AddCourseViewModel model)
        {
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            if (ModelState.IsValid) {
                try
                {
                    var teacher = _usermanager.FindByName(model.TeacherUsername);

                    if (teacher == null || teacher.Roles.Select(x => x.RoleId).FirstOrDefault() != TeacherRole.Id)
                    {
                        ModelState.AddModelError("Error", "Teacher not found.");
                        throw new Exception("Teacher not found.");
                    }

                    Courses course = AutoMap.Mapper.Map<Courses>(model);
                    course.Id = Guid.NewGuid().ToString();

                    _context.Courses.Add(course);
                    teacher.Courses.Add(course);

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("Error", "Failed to add course.");
                }

            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Update(string Id)
        {
            AddCourseViewModel course = _context.Courses.Where(c => c.Id == Id).Select(x => new AddCourseViewModel
                                                    { 
                                                        Name = x.Name,
                                                        ScheduledTime = x.ScheduledTime,
                                                        TeacherUsername = x.Users.Where(r => r.Roles.Select(c => c.RoleId).FirstOrDefault() == TeacherRole.Id).Select(y => y.UserName).FirstOrDefault(),
                                                    }).FirstOrDefault();
            if (course == null)
                return RedirectToAction("Index");


            ViewBag.CourseId = Id;

            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(string Id, AddCourseViewModel model)
        {
            if (ModelState.IsValid) {
                try
                {
                    var course = _context.Courses.Find(Id);
                    if (course == null)
                        throw new Exception();

                    var CurrentTeacher = _context.Users.Find(course.Users.Where(r => r.Roles.Select(c => c.RoleId).FirstOrDefault() == TeacherRole.Id).Select(y => y.Id).FirstOrDefault());
                    var NewTeacher = _usermanager.FindByName(model.TeacherUsername);
                    
                    if (NewTeacher == null || NewTeacher.Roles.Select(x => x.RoleId).FirstOrDefault() != TeacherRole.Id)
                    {
                        ModelState.AddModelError("Error", "Teacher not found.");
                        throw new Exception();
                    }

                    if (CurrentTeacher != null)
                    {
                        if (CurrentTeacher.Id != NewTeacher.Id)
                        {
                            CurrentTeacher.Courses.Remove(course);
                            NewTeacher.Courses.Add(course);
                        }
                    }
                    else {
                        NewTeacher.Courses.Add(course);
                    }

                    course.Name = model.Name;
                    course.ScheduledTime = model.ScheduledTime;

                    _context.SaveChanges();
                    return RedirectToAction("index");
                }
                catch
                {
                    ModelState.AddModelError("Error", "Failed to update course.");
                }
            }

            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string Id)
        {
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            var course = _context.Courses.Find(Id);
            if (course == null)
                return RedirectToAction("Index");

            ViewBag.Course = course;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmDelete(string Id)
        {
            ViewData["Teacher"] = TeacherRole.Id;
            ViewData["Student"] = StudentRole.Id;

            var course = _context.Courses.Find(Id);
            if (course != null)
                _context.Courses.Remove(course);
                _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Student")]
        public ActionResult Register(string Id)
        {
            var course = _context.Courses.Find(Id);
            if (course == null)
                return RedirectToAction("Index");

            ViewBag.Course = course;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public ActionResult ConfirmRegister(string Id)
        {
            try
            {
                var course = _context.Courses.Find(Id);
                if (course == null)
                    return RedirectToAction("Index");

                var user = _usermanager.FindByName(User.Identity.Name);

                user.Courses.Add(course);
                _context.SaveChanges();

                return RedirectToAction("Registered");
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }


            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Student, Teacher")]
        public ActionResult Registered()
        {
            var user = _usermanager.FindByName(User.Identity.Name);

            int Teacher_Per_Course = 1;
            ViewBag.Courses = user.Courses.Select(x => new AllCoursesViewModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Time = x.ScheduledTime,
                                    Teacher = x.Users.Where(r => r.Roles.Select(c => c.RoleId).FirstOrDefault() == TeacherRole.Id).Select(y => y.UserName).FirstOrDefault(),
                                    Students = x.Users.Count() - Teacher_Per_Course,
                                    Registered = 1,

                                })
                                .ToList();

            return View();
        }

    }
}