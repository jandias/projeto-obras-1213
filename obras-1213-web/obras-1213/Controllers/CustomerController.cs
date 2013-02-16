using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View(Customer.FindAll());
        }

        [HttpPost]
        public ActionResult Index(Customer newCustomer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!newCustomer.Insert())
                    {
                        ModelState.AddModelError("", "Não foi possível alterar os dados do cliente.");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (ModelException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(Customer.FindAll());
        }

        [HttpPost]
        public ActionResult Update(Customer cust)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!cust.Update())
                    {
                        ModelState.AddModelError("", "Não foi possível alterar os dados do cliente.");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (ModelException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return Index();
        }
    }
}
