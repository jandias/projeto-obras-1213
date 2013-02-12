using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Controllers
{
    public class ObrasController : Controller
    {
        //
        // GET: /Obras/
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            return View(Work.FindAll());
        }

        [HttpGet, Authorize]
        public ActionResult Details(int id)
        {
            try
            {
                return View(Work.Find(id));
            }
            catch (ModelException ex)
            {
                ModelState.AddModelError("", ex);
                return View("Index");
            }
        }

        [HttpPost, Authorize(Roles = "receptionist")]
        public ActionResult Index(string carLicense)
        {
            return View();
        }
    }
}
