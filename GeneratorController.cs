using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


using DHTMLX.Export.PDF;

namespace Grid2Pdf.Controllers
{
    [HandleError]
    public class GeneratorController : Controller
    {
        
        public ActionResult Generate()
        {
            var generator = new PDFWriter();          
            var xml = this.Server.UrlDecode(this.Request.Form["grid_xml"]);
            MemoryStream pdf = generator.Generate(xml);

            return File(pdf.ToArray(), generator.ContentType, "grid.pdf");       
        }

        
    }
}
