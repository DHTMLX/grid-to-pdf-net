using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using grid_pdf_net;
namespace Generator
{

    public class Generator : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var writer = new PDFWriter();
            var xml = context.Request.Form["grid_xml"];
            xml = context.Server.UrlDecode(xml);
            writer.Generate(xml, context.Response);
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}