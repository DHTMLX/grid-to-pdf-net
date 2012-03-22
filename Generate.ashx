<%@ WebHandler Language="C#" Class="Generator" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DHTMLX.Export.PDF;

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
