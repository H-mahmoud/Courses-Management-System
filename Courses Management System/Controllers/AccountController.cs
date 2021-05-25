using Courses_Management_System.Models;
using Courses_Management_System.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.Mvc;

namespace Courses_Management_System.Controllers
{
    public class AccountController : Controller
    {
        #region properties
        public CMSContext _context { get; set; }
        public UserStore<Users> _userstore { get; set; }
        public UserManager<Users> _usermanager { get; set; }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        #endregion

        public AccountController()
        {
            _context = new CMSContext();
            _userstore = new UserStore<Users>(_context);
            _usermanager = new UserManager<Users>(_userstore);

        }


        // GET: Account
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Profile");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel Post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _usermanager.Find(Post.Username, Post.Password);
                    if (user != null)
                    {
                        AuthenticationManager.SignOut();
                        var identity = _usermanager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignIn(identity);

                        return RedirectToAction("Index", "Profile");
                    }
                    else {
                        ModelState.AddModelError("Error", "Invalid Username or Password");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
            return View();
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index");
        }
    }
}