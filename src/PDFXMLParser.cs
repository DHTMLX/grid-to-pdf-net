using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using PdfSharp;
namespace DHTMLX.Export.PDF
{

    public enum ColorProfile
    {
        Color,
        FullColor,
        Gray,
        Default
    }

    public class PDFXMLParser
    {
        private XmlElement root;
        private PDFColumn[][] columns;
        private PDFRow[] rows;
        private double[] widths;
	    private Boolean header = false;
	    private Boolean footer = false;
	    private Boolean without_header = false;
        private ColorProfile profile = ColorProfile.Gray;
        private PageOrientation orientation = PageOrientation.Portrait;
        private PageSize size = PageSize.A4;


        public ColorProfile StringToColorProfile(string profile)
        {
            if(profile == null)
                return ColorProfile.Default;
            switch (profile.ToLower())
            {
                case "gray" : {
                    return ColorProfile.Gray;
                    
                }
                case "full_color":
                    {
                        return ColorProfile.FullColor;
                    }
                case "color":
                    {
                        return ColorProfile.Color;
                    }
                default:
                    return ColorProfile.Default;

            }
        }

	    public void SetXML(String xml)
	   {

		   

		    XmlDocument dom = new XmlDocument();;
		 //   try {
			    dom.LoadXml(xml);
		 //   }catch(SAXException se) {
		//	    se.printStackTrace();
		  //  }
		    root = dom.DocumentElement;

		    String header_string = root.GetAttribute("header");
		    if ((header_string != null)&&(header_string.ToLower().Equals("true") == true)) {
			    header = true;
		    }
		    String footer_string = root.GetAttribute("footer");
		    if ((footer_string != null)&&(footer_string.ToLower().Equals("true") == true)) {
			    footer = true;
		    }
		    String profile_string = root.GetAttribute("profile");
		    if (profile_string != null) {
                profile = StringToColorProfile(profile_string);
		    }

		    String orientation_string = root.GetAttribute("orientation");
		    GetHeaderInfo();
		    if (orientation_string != "") {
			    if (orientation_string.ToLower().Equals("landscape")) {
                    this.orientation = PageOrientation.Landscape;
                    this.size = PageSize.A4;
			    } else {
                    this.orientation = PageOrientation.Portrait;
                    this.size = PageSize.A4;
			    }
		    } else {

			    double sum_width = 0;
			    for (int i = 0; i < widths.Length; i++)
				    sum_width += widths[i];
                if (sum_width / widths.Length < 80)
                {
                    this.orientation = PageOrientation.Landscape;
                    this.size = PageSize.Letter;
                }
                else
                {
                    this.orientation = PageOrientation.Portrait;
                    this.size = PageSize.Letter;
                }
		    }
		    String w_header = root.GetAttribute("without_header");
		    if ((w_header != null)&&(w_header.ToLower().Equals("true") == true))
			    without_header = true;
	    }
	    public PDFColumn[][] GetHeaderInfo() {
		    PDFColumn[] colLine = null;
		    XmlNodeList n1 = root.GetElementsByTagName("columns");
		    if ((n1 != null)&&(n1.Count > 0)) {
			    columns = new PDFColumn[n1.Count][];
			    for (int i = 0; i < n1.Count; i++) {
				    XmlElement cols = (XmlElement) n1[i];
				    XmlNodeList n2 = cols.GetElementsByTagName("column");
				    if ((n2 != null)&&(n2.Count > 0)) {
					    colLine = new PDFColumn[n2.Count];
					    for (int j = 0; j < n2.Count; j++) {
						    XmlElement col_xml = (XmlElement) n2[j];
						    PDFColumn col = new PDFColumn();
						    col.Parse(col_xml);
						    colLine[j] = col;
					    }
				    }
				    columns[i] = colLine;
			    }
		    }
		    createWidthsArray();
		    optimizeColumns();
		    return columns;
	    }

	    private void createWidthsArray() {
		    widths = new double[columns[0].Length];
		    for (int i = 0; i < columns[0].Length; i++) {
			    widths[i] = columns[0][i].GetWidth();
		    }
	    }

	    private void optimizeColumns() {
		    for (int i = 1; i < columns.Length; i++) {
			    for (int j = 0; j < columns[i].Length; j++) {
				    columns[i][j].SetWidth(columns[0][j].GetWidth());
			    }
		    }
		    for (int i = 0; i < columns.Length; i++) {
			    for (int j = 0; j < columns[i].Length; j++) {
				    if (columns[i][j].GetColspan() > 0) {
					    for (int k = j + 1; k < j + columns[i][j].GetColspan(); k++) {
						    columns[i][j].SetWidth(columns[i][j].GetWidth() + columns[i][k].GetWidth());
						    columns[i][k].SetWidth(0);
					    }
				    }
				    if (columns[i][j].GetRowspan() > 0) {
					    for (int k = i + 1; k < i + columns[i][j].GetRowspan(); k++) {
						    columns[i][j].SetHeight(columns[i][j].GetHeight() + 1);
						    columns[k][j].SetHeight(0);
					    }
				    }
			    }
		    }
	    }

	    public PDFRow[] GetGridContent() {
		    XmlNodeList nodes = root.GetElementsByTagName("row");
		    if ((nodes != null)&&(nodes.Count > 0)) {
                rows = new PDFRow[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
				    rows[i] = new PDFRow();
				    rows[i].Parse(nodes[i]);
			    }
		    }
		    return rows;
		
	    }

	    public double[] GetWidths() {
		    return widths;
	    }

	    public bool GetHeader() {
		    return header;
	    }

        public bool GetFooter()
        {
		    return footer;
	    }

	    public PageOrientation GetOrientation() {
		    return orientation;
	    }

        public PageSize GetSize()
        {
            return this.size;
        }

	    public ColorProfile GetProfile() {
		    return profile;
	    }

        public double[] GetSizes()
        {
            var sizes = new double[2];
            var siz= PageSizeConverter.ToSize(this.size);

            if (this.orientation == PageOrientation.Landscape)
            {
                    
                sizes[0] = siz.Height;
                sizes[1] = siz.Width;
            }
            else
            {
                sizes[1] = siz.Height;
                sizes[0] = siz.Width;
            }

            return sizes;
            
        }

        public bool GetWithoutHeader()
        {
		    return without_header;
	    }
    }
}
