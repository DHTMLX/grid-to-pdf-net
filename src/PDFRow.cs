using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace DHTMLX.Export.PDF
{
    public class PDFRow
    {
        private PDFCell[] cells;
	
	    public void Parse(XmlNode parent){
		   XmlNodeList nodes = ((XmlElement) parent).GetElementsByTagName("cell");
		    XmlNode text_node;
		    if ((nodes != null)&&(nodes.Count > 0)) {
                cells = new PDFCell[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
				    PDFCell cell = new PDFCell();
				    text_node = nodes[i];
				    if (text_node != null)
					    cell.Parse(text_node);
				    cells[i] = cell;
			    }
		    }
	    }
	
	    public PDFCell[] GetCells() {
		    return cells;
	    }
    }
}
