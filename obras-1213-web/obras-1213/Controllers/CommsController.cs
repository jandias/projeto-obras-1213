using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using obras_1213.Models;
using obras_1213.Models.View;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;

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
                    string errors = "";
                    XmlSchemaSet xsds = new XmlSchemaSet();
                    xsds.Add("si2.isel.pt/2013/TrabFinal", Server.MapPath("~/Content/xml/comunicado.xsd"));
                    XDocument xdoc = XDocument.Load(new StreamReader( Request.Files["CommFile"].InputStream ));
                    xdoc.Validate(xsds, (o, e) =>
                    {
                        errors += "Erro de validação do documento XML: " + e.Message;
                    });
                    if (errors.Length == 0)
                    {
                        XNamespace myns = "si2.isel.pt/2013/TrabFinal";
                        XElement docDate = xdoc.Root.Element(myns + "data");
                        if (docDate != null)
                        {
                            XAttribute attr = docDate.Attribute("entrada-em-vigor");
                            if (attr != null)
                            {
                                try
                                {
                                    DateTime dd = DateTime.Parse(docDate.Value);
                                    DateTime vd = DateTime.Parse(attr.Value);
                                    if (vd < dd)
                                    {
                                        errors += " A data de entrada em vigor é inferior à data de emissão.";
                                    }
                                }
                                catch (Exception e)
                                {
                                    errors += "Erro na validação do formato das datas: " + e.Message;
                                }
                            }
                            else
                            {
                                docDate.Add(new XAttribute("entrada-em-vigor", docDate.Value));
                            }
                        }
                    }
                    if (errors.Length == 0)
                    {
                        var comm = new Communication(departmentId, shopId, xdoc.ToString(SaveOptions.OmitDuplicateNamespaces));
                        if (comm.Insert())
                            msg = "Dados inseridos com sucesso.";
                        else
                            msg = "Os dados não foram inseridos.";
                    }
                    else
                    {
                        msg = "Os dados não foram inseridos: " + errors;
                    }
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

        [HttpGet]
        public ActionResult ListAll()
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader sr = new StringReader(Communication.FindAllAsXml()))
            {
                using (XmlReader xread = XmlReader.Create(sr))
                {
                    using (StringWriter sw = new StringWriter(sb))
                    {
                        XslCompiledTransform transf = new XslCompiledTransform();
                        transf.Load(Server.MapPath("~/Content/xslt/comms2table.xslt"));
                        transf.Transform(xread, null, sw);
                    }
                }
            }
            ViewBag.CommsHtml = sb.ToString();
            return View();
        }
    }
}
