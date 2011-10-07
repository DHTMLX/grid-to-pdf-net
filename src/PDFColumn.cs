using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace DHTMLX.Export.PDF
{
    public class PDFColumn
    {
        private String colName;
        private String type;
        private String align;
        private int colspan;
        private int rowspan;
        private double width = 0;
        private int height = 1;
        private bool is_footer = false;

        public void Parse(XmlElement parent)
        {
            is_footer = parent.ParentNode.ParentNode.Name.Equals("foot");

            XmlNode text_node = parent.FirstChild;
            if (text_node != null)
                colName = text_node.Value;
            else
                colName = "";

            String width_string = parent.GetAttribute("width");

            if (width_string.Length > 0)
            {
                width = int.Parse(width_string);
            }
            type = parent.GetAttribute("type");
            align = parent.GetAttribute("align");
            String colspan_string = parent.GetAttribute("colspan");
            if (colspan_string.Length > 0)
            {
                colspan = int.Parse(colspan_string);
            }
            String rowspan_string = parent.GetAttribute("rowspan");
            if (rowspan_string.Length > 0)
            {
                rowspan = int.Parse(rowspan_string);
            }
        }

        public bool IsFooter()
        {
            return is_footer;
        }

        public double GetWidth()
        {
            return width;
        }

        public void SetWidth(double width)
        {
            this.width = width;
        }

        public int GetColspan()
        {
            return colspan;
        }

        public int GetRowspan()
        {
            return rowspan;
        }

        public int GetHeight()
        {
            return height;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public string GetName()
        {
            return colName;
        }

        public string GetAlign()
        {
            return align;
        }

        public String getType()
        {
            return type;
        }
    }
}
