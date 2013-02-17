using obras_1213.Models;
using obras_1213.Models.View;
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
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel login, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(login.EmployeeID.ToString(), ""))
                {
                    FormsAuthentication.SetAuthCookie(login.EmployeeID.ToString(), false);
                    this.Employee = Employee.Find(login.EmployeeID);
                    this.ShopID = login.ShopID;
                    return Redirect(String.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Número de funcionário incorrecto.");
                }
            }
            return View(login);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}
