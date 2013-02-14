using obras_1213.Models;
using obras_1213.Models.View;
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
                return View("Details", new WorkViewModel(Work.Find(id)));
            }
            catch (ModelException ex)
            {
                ModelState.AddModelError("", ex);
                return View("Index");
            }
        }

        [HttpGet, Authorize(Roles = "receptionist")]
        public ActionResult New()
        {
            return View(new NewWorkViewModel());
        }

        [HttpPost, Authorize(Roles = "receptionist")]
        public ActionResult New(NewWorkViewModel newWork)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return RedirectToAction("Details", new { id = Work.Insert(1, newWork.CarLicensePlate, newWork.Actions) });
                }
                catch (ModelException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(newWork);
        }

        [HttpPost, Authorize(Roles = "receptionist")]
        public ActionResult Change(Work workData)
        {
            return View();
        }

        [HttpPost, Authorize(Roles = "receptionist")]
        public ActionResult RemovePart(int id, string partId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Work.Find(id).RemovePart(partId))
                    {
                        return RedirectToAction("Details", new { id = id });
                    }
                    ModelState.AddModelError("", "Não foi possível remover essa peça da obra.");
                }
                catch (ModelException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return Details(id);
        }

        [HttpPost, Authorize(Roles = "receptionist")]
        public ActionResult AddPart(int id, WorkPart newPart)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Work.Find(id).AddPart(newPart))
                    {
                        return RedirectToAction("Details", new { id = id });
                    }
                    ModelState.AddModelError("", "Não é possível adicionar essa peça à obra. Será que já existe?");
                }
                catch (ModelException)
                {
                    ModelState.AddModelError("", "Não é possível adicionar essa peça à obra. Será que já existe?");
                }
            }
            return Details(id);
        }
    }
}
