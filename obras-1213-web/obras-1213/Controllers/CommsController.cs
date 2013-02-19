using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;
using obras_1213.Models;
using obras_1213.Models.View;

namespace obras_1213.Controllers
{
    [Authorize]
    public class CommsController : Controller
    {
        //
        // GET: /Comms/

        [HttpGet]
        public ActionResult Index()
        {
            return View( new CommsViewModel());
        }

        [HttpPost]
        public ActionResult Add(int shopId, int departmentId)
        {
            String msg = "";
            if( shopId > 0 && departmentId > 0 && Request.Files["CommFile"] != null ) {
                try {
                    string xmlContent = new StreamReader( Request.Files["CommFile"].InputStream ).ReadToEnd();
                    var comm = new Communication(departmentId, shopId, xmlContent);
                    if( comm.Insert() )
                        msg="Dados inseridos com sucesso.";
                    else
                        msg="Os dados não foram inseridos.";
                }
                catch (ModelException ex)
                {
                    msg="Os dados não foram inseridos: "+ex.Message;
                }
            }
            else
                msg = "Campos submetidos inválidos.";

            TempData["report"] = msg;
            return View();
        }

        [HttpGet]
        public ActionResult AjaxDepartments(int shopId) {
            return PartialView("_Departments", CommsViewModel.GetDepartments(shopId) );
        }

        [HttpGet]
        public ActionResult Download()
        {
            try
            {
                Response.ContentType = "text/xml; charset=utf-8";
                Response.AddHeader("Content-Disposition", "attachment; filename=\"comunicados.xml\"");
                string xmlText = Communication.FindAllAsXml();
                ValidateXml(xmlText);
                return this.Content(xmlText, "text/xml", System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return RedirectToAction("Index");
            }
        }

        private void ValidateXml(string xmlText) {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlText);
            document.Schemas.Add( null, Server.MapPath(Url.Content("~/Content/xml/comunicados.xsd")) );
            // Validation errors thrown as exceptions. Callback used for error details
            document.Validate((sender, e) => {});
        }

        [HttpPost]
        public ActionResult List(DateTime date)
        {
            return View(Communication.List(date));
        }
    }
}
