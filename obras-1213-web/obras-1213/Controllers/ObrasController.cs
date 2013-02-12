using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Controllers
{
    [Authorize]
    public class ObrasController : Controller
    {
        //
        // GET: /Obras/

        public ActionResult Index()
        {
            return View(Work.FindAll());
        }

        [HttpGet]
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
    }
}
