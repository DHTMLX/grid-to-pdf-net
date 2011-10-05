using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using PdfSharp;
using System.IO;
using PdfSharp.Drawing;

using PdfSharp.Pdf;
using System.Web;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.Drawing;
using System.Reflection;

namespace grid_pdf_net
{
    public class PDFImages
    {
        private Dictionary<Types, Stream> images = null;
        public enum Types{
            CheckboxOn,
            CheckboxOff,
            RadiobuttonOn,
            RadiobuttonOff,
            Footer,
            Header
        }
        private Dictionary<Types, string> paths;
        public PDFImages(ColorProfile colorScheme){
            switch (colorScheme){
                case ColorProfile.Color:
                    {
                        paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "grid_pdf_net.Images.ChOffColor.png"},
                            {Types.CheckboxOn, "grid_pdf_net.Images.ChOnColor.png"},
                            {Types.Footer, "grid_pdf_net.Images.footer.jpg"},
                            {Types.Header, "grid_pdf_net.Images.header.jpg"},
                            {Types.RadiobuttonOff, "grid_pdf_net.Images.RaOffColor.png"},
                            {Types.RadiobuttonOn, "grid_pdf_net.Images.RaOnColor.png"}

                        };
                        break;
                    }
                case ColorProfile.FullColor:{
                    paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "grid_pdf_net.Images.ChOffColor.png"},
                            {Types.CheckboxOn, "grid_pdf_net.Images.ChOnColor.png"},
                            {Types.Footer, "grid_pdf_net.Images.footer.jpg"},
                            {Types.Header, "grid_pdf_net.Images.header.jpg"},
                            {Types.RadiobuttonOff, "grid_pdf_net.Images.RaOffColor.png"},
                            {Types.RadiobuttonOn, "grid_pdf_net.Images.RaOnColor.png"}

                        };
                    break;
                    }
                case ColorProfile.Gray:
                    {
                        paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "grid_pdf_net.Images.ChOffGray.png"},
                            {Types.CheckboxOn, "grid_pdf_net.Images.ChOnGray.png"},
                            {Types.Footer, "grid_pdf_net.Images.footer.jpg"},
                            {Types.Header, "grid_pdf_net.Images.header.jpg"},
                            {Types.RadiobuttonOff, "grid_pdf_net.Images.RaOffGray.png"},
                            {Types.RadiobuttonOn, "grid_pdf_net.Images.RaOnGray.png"}

                        };
                        break;
                    }
                default:
                    paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "grid_pdf_net.Images.ChOffBw.png"},
                            {Types.CheckboxOn, "grid_pdf_net.Images.ChOnBw.png"},
                            {Types.Footer, "grid_pdf_net.Images.footer.jpg"},
                            {Types.Header, "grid_pdf_net.Images.header.jpg"},
                            {Types.RadiobuttonOff, "grid_pdf_net.Images.RaOffBw.png"},
                            {Types.RadiobuttonOn, "grid_pdf_net.Images.RaOnBw.png"}

                        };
                    break;

            }
        }
        protected void loadImages()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            images = new Dictionary<Types, Stream>(6);
            foreach (Types type in Enum.GetValues(typeof(Types)))
            {
                images[type] = assembly.GetManifestResourceStream(paths[type]);
            }
            
        }
        public XImage Get(Types type)
        {
            if (images == null)
            {
                this.loadImages();
            }

            var img = new Bitmap(this.images[type], false);
            return (XImage)img;
        }
        
    }
}
