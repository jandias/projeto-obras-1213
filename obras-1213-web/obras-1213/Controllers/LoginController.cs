using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace obras_1213.Controllers
{
    public class LoginController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string employeeNumber, string ReturnUrl)
        {
            if (Membership.ValidateUser(employeeNumber, ""))
            {
                FormsAuthentication.SetAuthCookie(employeeNumber, false);
                this.Employee = Employee.Find(Int32.Parse(employeeNumber));
                return Redirect(String.IsNullOrEmpty(ReturnUrl) ? Url.RouteUrl("Default") : ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Número de funcionário incorrecto.");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}
