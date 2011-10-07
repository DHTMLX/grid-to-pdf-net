using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace DHTMLX.Export.PDF
{
    public class PDFCell
    {
        private String value = "";
	    private String bgColor = "";
	    private String textColor = "";
	    private bool bold = false;
	    private bool italic = false;
	    private String align = "";

	    public void Parse(XmlNode parent){
		    value = parent.FirstChild.Value;
		    XmlElement el = (XmlElement) parent;
		    bgColor = (el.HasAttribute("bgColor")) ? el.GetAttribute("bgColor") : "";
		    textColor = (el.HasAttribute("textColor")) ? el.GetAttribute("textColor") : "";
		    bold = (el.HasAttribute("bold")) ? el.GetAttribute("bold").Equals("bold") : false;
            italic = (el.HasAttribute("italic")) ? el.GetAttribute("italic").Equals("italic") : false;
		    align = (el.HasAttribute("align")) ? el.GetAttribute("align") : "";
	    }

	    public String GetValue() {
		    return value;
	    }

	    public String GetBgColor() {
		    return bgColor;
	    }

	    public String GetTextColor() {
		    return textColor;
	    }

	    public Boolean GetBold() {
            return bold;
	    }
	
	    public Boolean GetItalic() {
            return italic;
	    }
	
	    public String GetAlign() {
		    return align;
	    }
    }
}
