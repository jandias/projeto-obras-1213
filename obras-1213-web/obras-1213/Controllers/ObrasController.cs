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
            return View();
        }

    }
}
