using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;

namespace obras_1213.Controllers
{
    [Authorize]
    public class ShopsController : BaseController
    {
        //
        // GET: /Shops/

        public ActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader sr = new StringReader(Shop.FindAllXml()))
            {
                using (XmlReader xread = XmlReader.Create(sr))
                {
                    using (StringWriter sw = new StringWriter(sb))
                    {
                        XslCompiledTransform transf = new XslCompiledTransform();
                        transf.Load(Server.MapPath("~/Content/xslt/shops2table.xslt"));
                        transf.Transform(xread, null, sw);
                    }
                }
            }
            ViewBag.ShopsTableHtml = sb.ToString();
            return View();
        }

    }
}
