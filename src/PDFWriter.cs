using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp;
using System.IO;
using PdfSharp.Drawing;

using PdfSharp.Pdf;
using System.Web;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;


namespace DHTMLX.Export.PDF
{



    public class PDFWriter
    {

        protected PDFImages images;


        protected PDFXMLParser parser;
        protected PdfDocument pdf;
        protected PdfPage page;
        protected List<PdfPage> pages;
        protected List<XGraphics> graphics;
        protected XGraphics gfx;
        protected double[] widths;
        protected double widthSum = 0;
        protected double headerHeight = 0;
        protected double pageWidth = 0;
        protected double pageHeight = 0;
        public string ContentType { get { return "application/pdf"; } }
	    public double OffsetTop = 30;
	    public double OffsetBottom = 30;
	    public double OffsetLeft = 30;
	    public double OffsetRight = 30;
	    public double LineHeight = 20;
	    public double CellOffset = 3;
        public double BorderWidth = 0.5;
	    public double HeaderImgHeight = 100;
	    public double FooterImgHeight = 100;
        protected int fontSize = 9;
        protected string bgColor;
        protected string lineColor;
        protected string headerTextColor;
        protected string scaleOneColor;
        protected string scaleTwoColor;
        protected string gridTextColor;
        protected string pageTextColor;
        protected string watermarkTextColor;
	
	    public double HeaderLineHeight = 30;
	    public string PageNumTemplate = "Page {pageNum}/{allNum}";
	    public string Watermark = null;
	
	    protected PDFColumn[][] cols = null;

        protected XFont f1 = null;
        protected XFont f2 = null;
        protected XFont f3 = null;
        protected XFont f4 = null;

        protected bool firstPage = false;
        protected double footerHeight = 0;
        protected int cols_stat;
        protected int rows_stat;


        public MemoryStream Generate(string xml)
        {
            var data = new MemoryStream();

            Generate(xml, data);
            return data;
        }
        public void Generate(string xml, HttpResponse resp)
        {
            var data = new MemoryStream();

            resp.ContentType = ContentType;
            resp.HeaderEncoding = Encoding.UTF8;
            resp.AppendHeader("Content-Disposition", "attachment;filename=grid.pdf");
            resp.AppendHeader("Cache-Control", "max-age=0");
            Generate(xml, data);

            data.WriteTo(resp.OutputStream);


        }
	    public void Generate(String xml, Stream resp){
		    parser = new PDFXMLParser();
		    try {
			    parser.SetXML(xml);
			
			    createPDF();
			    setColorProfile();
			    headerPrint();
			    printRows();
			    printFooter();
			    printPageNumbers();
			    outputPDF(resp);

		    } catch (Exception e) {
			    
		    } 
	    }

        protected PdfPage newPage()
        {
            page = new PdfPage(pdf);
            pdf.AddPage(page);

            gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Replace);
            if (graphics == null)
                graphics = new List<XGraphics>();
            graphics.Add(gfx);
            pages.Add(page);

            page.Size = parser.GetSize();
            page.Orientation = parser.GetOrientation();
            return page;
        }
	    protected void createPDF() {
		    pdf =  new PdfDocument();

            var font_settings = new XPdfFontOptions(PdfFontEncoding.Unicode);

            f1 = createFont("Verdana", fontSize);
		
            pdf.Version = 14;
		    pages = new List<PdfPage>();
            newPage();

		    double[] sizes = parser.GetSizes();
		    pageWidth = sizes[0] - OffsetLeft - OffsetRight;
		    pageHeight = sizes[1] - OffsetTop - OffsetBottom;
		    printHeader();
		
	    }

	    private void headerPrint(){
		    
		    if (cols == null) {
			    cols = parser.GetHeaderInfo();
			    widths = parser.GetWidths();
			    for (int i = 0; i < cols[0].Length; i++) {
				    widthSum += cols[0][i].GetWidth();
			    }
		    }
		
		    cols_stat = cols.Length;
		
		    double[] bg = RGBColor.GetColor(bgColor);
		    double[] border = RGBColor.GetColor(lineColor);
		    double x = OffsetLeft;
		    double y = OffsetTop;
		    int lines = 0;

		    for (int row = 0; row < cols.Length; row++) {
			    x = OffsetLeft;
			    if (cols[row][0].IsFooter()) continue;
			    for (int j = 0; j < cols[row].Length; j++) {
				    PDFColumn column = cols[row][j];
				
				    x += printColumn(column, bg, border, x, y);
			    }
			    y += HeaderLineHeight;
			    lines++;
		    }
		
		    headerHeight = lines*HeaderLineHeight;
		    footerHeight = (cols.Length - lines)*HeaderLineHeight;
		    y = pageHeight-headerHeight-footerHeight;
		    y = Math.Floor(y/LineHeight)*LineHeight+OffsetTop+headerHeight;
		
		    for (int i = 0; i < cols.Length; i++) {
			    if (!cols[i][0].IsFooter()) continue;
			    x = OffsetLeft;
			    for (int j = 0; j < cols[i].Length; j++) {
				    PDFColumn column = cols[i][j];
				    x += printColumn(column, bg, border, x, y);
			    }
			    y += HeaderLineHeight;
		    }
	    }

	    private double printColumn(PDFColumn column, double[] bg, double[] border, double x, double y){
		    double width = column.GetWidth()*pageWidth/widthSum;
		    double height = column.GetHeight()*HeaderLineHeight;
		    if (height > 0 && width >0) {
			
			    XRect cellIn = new XRect();
			    cellIn.Location = new XPoint(x, y);
			    cellIn.Size = new XSize(width, height);

                gfx.DrawRectangle(new XSolidBrush(RGBColor.GetXColor(bg)), cellIn);

			    String label = textWrap(column.GetName(), width - 2*CellOffset, f1);
                double text_x = x +  (width - gfx.MeasureString(label, f1).Width) / 2;

                double text_y = y +  (height + f1.Size) / 2;
                gfx.DrawString(label, f1, new XSolidBrush(RGBColor.GetXColor(headerTextColor)), new XPoint(text_x, text_y));

                var points = new XPoint[4];
                points[0] = new XPoint(x + width, y + height);
                points[1] = new XPoint(x + width, y);
                points[2] = new XPoint(x,y);
                points[3] = new XPoint(x, y + height);
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), points[0], points[1]);
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), points[1], points[2]);
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), points[2], points[3]);
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), points[3], points[0]);

		    }
            return width;
	    }

        protected XFont createFont(string name, double size)
        {
            var font_settings = new XPdfFontOptions(PdfFontEncoding.Unicode);
            return new XFont(name, size, new XFontStyle(), font_settings);
      
        }
	    private void printRows(){
		    PDFRow[] rows = parser.GetGridContent();
		    rows_stat = rows.Length;
		    double[] rowColor;
		    double[] border = RGBColor.GetColor(lineColor);
		    double y = OffsetTop + headerHeight;
            bool cellsLined = false;
		    XFont cf;
            int rowsOnPage = 0;
		    for (int rowNum = 0; rowNum < rows.Length; rowNum++) {
			    double x = OffsetLeft;
			    PDFCell[] cells = rows[rowNum].GetCells();
			    if (rowNum%2 == 0) {
				    rowColor = RGBColor.GetColor(scaleOneColor);
			    } else {
				    rowColor = RGBColor.GetColor(scaleTwoColor);
			    }
                rowsOnPage++;
               
                for (int colNum = 0; colNum < cells.Length; colNum++)
                {
         #region columnDrawing
                    double height = cols[0][ colNum ].GetHeight() * LineHeight;
				    double width = widths[ colNum ]*pageWidth/widthSum;
				    String al = cells[ colNum ].GetAlign();
				    if (al.ToLower().Equals(""))
					    al = cols[0][ colNum ].GetAlign();
				    String tp = cols[0][ colNum ].getType();

                    XRect cellIn = new XRect();
                    cellIn.Location = new XPoint(x, y);
                    cellIn.Size = new XSize(width, LineHeight);
             
                  
                    gfx.DrawRectangle(new XSolidBrush(RGBColor.GetXColor(((!cells[ colNum ].GetBgColor().Equals("")) && (parser.GetProfile() == ColorProfile.FullColor)) ? RGBColor.GetColor(cells[colNum].GetBgColor()) : rowColor)), cellIn);

				    if (cells[ colNum ].GetBold()){
					    if (cells[ colNum ].GetItalic()){
						    if (f4 == null){
                                f4 = createFont("Helvetica-BoldOblique", fontSize);
                               
						    }
						    cf = f4;
					    } else {
						    if (f2 == null){
                                f2 = createFont("Helvetica-Bold", fontSize);
						
						    }
						    cf = f2;
					    }
				    } else if (cells[colNum].GetItalic()){
					    if (f3==null){
                            f3 = createFont("Helvetica-Oblique", fontSize);
				
					    }	
					    cf = f3;
				    } else {
					    cf = f1;
				    }

                    XTextFormatter text = new XTextFormatter(gfx);
                    text.Font = cf;
                    text.Text = textWrap(cells[colNum].GetValue(), width - 2*CellOffset, cf);


                    String label = textWrap(cells[colNum].GetValue(), width - 2 * CellOffset, cf);
                   


				    
				    if (al.ToLower().Equals("left") == true) {
					    
                        text.Alignment = XParagraphAlignment.Left;
				    } else {
                        if (al.ToLower().Equals("right") == true)
                        {
                            text.Alignment = XParagraphAlignment.Right;
						    			
					    } else {
                            text.Alignment = XParagraphAlignment.Center;
						    
					    }
				    }
                

				  
                    var checkbox_width = 15/2;//approxmatelly...
				    if (tp.ToLower().Equals("ch")) {
                        double ch_x = width / 2 - checkbox_width;
                        double ch_y = LineHeight / 2 - checkbox_width;
                        if (text.Text.ToLower().Equals("1") == true)
                        {
						    drawCheckbox(true, ch_x, ch_y, cellIn);
					    } else {
						    drawCheckbox(false, ch_x, ch_y, cellIn);
					    }
				    } else {
                        if (tp.ToLower().Equals("ra"))
                        {
                            double ch_x = width / 2 - checkbox_width;
                            double ch_y = LineHeight / 2 - checkbox_width;
                            if (text.Text.ToLower().Equals("1") == true)
                            {
							    drawRadio(true, ch_x, ch_y, cellIn);
						    } else {
							    drawRadio(false, ch_x, ch_y, cellIn);
						    }
					    } else {
						    double text_y = (LineHeight + f1.Size)/2;

                            XRect text_cell = new XRect();
                            text_cell.Size = new XSize(width - 2 * LineHeight / 5, gfx.MeasureString(text.Text, text.Font).Height);

                            text_cell.Location = new XPoint(x + LineHeight/5, y + (LineHeight - text_cell.Size.Height) / 1.7);


                            text.DrawString(text.Text, text.Font, new XSolidBrush((!cells[colNum].GetTextColor().Equals("") && parser.GetProfile().Equals("full_color")) ? RGBColor.GetXColor(cells[colNum].GetTextColor()) : RGBColor.GetXColor(gridTextColor)), text_cell);
                            
					    }
				    }
				    x += width;
#endregion
                }



                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(OffsetLeft, y), new XPoint(OffsetLeft + pageWidth, y));

                y += LineHeight;
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(OffsetLeft, y), new XPoint(OffsetLeft + pageWidth, y));


			    if (y + LineHeight - OffsetTop + footerHeight>= pageHeight) {
                    var left = OffsetLeft;
                    var top = OffsetTop + headerHeight;

                    top = OffsetTop + headerHeight;
                    gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(left, top), new XPoint(left, top + rowsOnPage * LineHeight));

                    var widths = parser.GetWidths();
                    for (int colNum = 0; colNum < widths.Length; colNum++)
                    {
                        left += widths[colNum] * pageWidth / widthSum;
                        gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(left, top), new XPoint(left, top + rowsOnPage * LineHeight));
                    }
                    
                    newPage();
                    rowsOnPage = 0;
				    if (firstPage == true) {
					    pageHeight += HeaderImgHeight;
					    OffsetTop -= HeaderImgHeight;
					    firstPage = false;
				    }
				    headerPrint();
				    y = OffsetTop + headerHeight;
			    }
			    x = OffsetLeft;
		    }
            f1 = createFont("Helvetica", fontSize);

            if (!cellsLined)
            {
                var left = OffsetLeft;
                var top = OffsetTop + headerHeight;
               

                top = OffsetTop + headerHeight;
                gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(left, top), new XPoint(left, top + rowsOnPage * LineHeight));

                var widths = parser.GetWidths();
                for (int colNum = 0; colNum < widths.Length; colNum++)
                {
                    left += widths[colNum] * pageWidth / widthSum;
                    gfx.DrawLine(new XPen(RGBColor.GetXColor(border), BorderWidth), new XPoint(left, top), new XPoint(left, top + rowsOnPage * LineHeight));
                }
            }



	    }

	    private void printPageNumbers(){
		    
            for (var i = 1; i <= pages.Count; i++)
            {
                var page = pages[i-1];
                var graph = graphics[i-1]; 
			    String str = PageNumTemplate;
                str = str.Replace("{pageNum}", i.ToString());
			    str = str.Replace("{allNum}", pages.Count.ToString());

                XTextFormatter text = new XTextFormatter(graph);
                double x = pageWidth + OffsetLeft - graph.MeasureString(str, f1).Width;
			    double y = pageHeight + OffsetTop + f1.Size;
                text.DrawString(str, f1, new XSolidBrush(RGBColor.GetXColor(pageTextColor)), new XRect(x, y, graph.MeasureString(str, f1).Width, f1.Size));


			    printWatermark(page, graph);
			   
		    }
	    }
        
        private void printWatermark(PdfPage currentPage, XGraphics graph)
        {
            if (Watermark == null)
                return;
            XTextFormatter text = new XTextFormatter(graph);
            text.Text = Watermark;
            text.Font = f1;

            XRect placeholder = new XRect();
            double x = OffsetLeft;
		    double y = pageHeight + OffsetTop + f1.Size;
            placeholder.Location = new XPoint(x, y);
            placeholder.Size = new XSize(graph.MeasureString(text.Text, text.Font).Width, graph.MeasureString(text.Text, text.Font).Height);
            text.DrawString(text.Text, text.Font, new XSolidBrush(RGBColor.GetXColor(watermarkTextColor)), placeholder);
		 
	    }

	    private void printHeader(){
		    Boolean header = parser.GetHeader();
		    if (header == true) {
                XImage im = images.Get(PDFImages.Types.Footer);            
                gfx.DrawImage(im, new XPoint(OffsetLeft, OffsetTop));
			    pageHeight -= HeaderImgHeight;
			    OffsetTop += HeaderImgHeight;
			    firstPage = true;
		    }
	    }

	    private void printFooter(){
		    Boolean footer = parser.GetFooter();
		    if (footer == true) {			   
                XImage im = images.Get(PDFImages.Types.Footer); ;		  
                gfx.DrawImage(im, new XPoint(OffsetLeft, pageHeight + OffsetTop - FooterImgHeight));
			    pageHeight -= FooterImgHeight;
			    OffsetTop += FooterImgHeight;
			    firstPage = true;
		    }
	    }

	    private void outputPDF(Stream resp) {
            pdf.Save(resp, false);
	    }

	    private String textWrap(String text, double width, XFont f) {
		    if ((gfx.MeasureString(text, f).Width <= width)||(text.Length == 0)) {
			    return text;
		    }
		    while ((gfx.MeasureString(text + "...", f).Width  > width)&&(text.Length > 0)) {
			    text = text.Substring(0, text.Length - 1);
		    }
		    return text + "...";
	    }


	    private void drawRadio(bool value, double x, double y, XRect cellIn){

            var img = images.Get(value ? PDFImages.Types.RadiobuttonOn : PDFImages.Types.RadiobuttonOff);
            gfx.DrawImage(img, new XPoint(cellIn.TopLeft.X + x, cellIn.TopLeft.Y + y));

	    }
             
	
	    private void drawCheckbox(bool value, double x, double y, XRect cellIn){
            var img = images.Get(value ? PDFImages.Types.CheckboxOn : PDFImages.Types.CheckboxOff);
            gfx.DrawImage(img, new XPoint(cellIn.TopLeft.X + x, cellIn.TopLeft.Y + y));
           
	    }

	    private void setColorProfile() {
		    ColorProfile profile = parser.GetProfile();
            if (this.images == null)
            {
                this.images = new PDFImages(profile);
            }
            if (profile == ColorProfile.Color || profile == ColorProfile.FullColor)
            {
			    bgColor = "D1E5FE";
			    lineColor = "A4BED4";
			    headerTextColor = "000000";
			    scaleOneColor = "FFFFFF";
			    scaleTwoColor = "E3EFFF";
			    gridTextColor = "000000";
			    pageTextColor = "000000";
			   
			    watermarkTextColor = "8b8b8b";
		    } else {
			    if (profile == ColorProfile.Gray) {
				    bgColor = "E3E3E3";
				    lineColor = "B8B8B8";
				    headerTextColor = "000000";
				    scaleOneColor = "FFFFFF";
				    scaleTwoColor = "EDEDED";
				    gridTextColor = "000000";
				    pageTextColor = "000000";
				  
				    watermarkTextColor = "8b8b8b";
			    } else {
				    bgColor = "FFFFFF";
				    lineColor = "000000";
				    headerTextColor = "000000";
				    scaleOneColor = "FFFFFF";
				    scaleTwoColor = "FFFFFF";
				    gridTextColor = "000000";
				    pageTextColor = "000000";
				   
				    watermarkTextColor = "000000";
			    }
		    }
	    }


	    public int ColsStat {
            get { return this.cols_stat; }
	    }
	
	    public int RowsStat {
		    get { return this.rows_stat;}
	    }
	    /// <summary>
	    /// Set text which will be printed at the bottom of each page
	    /// </summary>
	    /// <param name="mark">Text to be printed</param>
	    public void SetWatermark(string mark) {
		    Watermark = mark;	
	    }

    }
}
