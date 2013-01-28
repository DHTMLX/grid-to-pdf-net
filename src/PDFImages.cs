using System;
using System.Collections.Generic;

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

namespace DHTMLX.Export.PDF
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
        protected string imagePath = "DHTMLX.Export.PDF.Images.";
        public PDFImages(ColorProfile colorScheme){
            imagePath = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Images.";
   
            switch (colorScheme){
                case ColorProfile.Color:
                    {
                        paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "ChOffColor.png"},
                            {Types.CheckboxOn, "ChOnColor.png"},
                            {Types.Footer, "footer.jpg"},
                            {Types.Header, "header.jpg"},
                            {Types.RadiobuttonOff, "RaOffColor.png"},
                            {Types.RadiobuttonOn, "RaOnColor.png"}

                        };
                        break;
                    }
                case ColorProfile.FullColor:{
                    paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "ChOffColor.png"},
                            {Types.CheckboxOn, "ChOnColor.png"},
                            {Types.Footer, "footer.jpg"},
                            {Types.Header, "header.jpg"},
                            {Types.RadiobuttonOff, "RaOffColor.png"},
                            {Types.RadiobuttonOn, "RaOnColor.png"}

                        };
                    break;
                    }
                case ColorProfile.Gray:
                    {
                        paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "ChOffGray.png"},
                            {Types.CheckboxOn, "ChOnGray.png"},
                            {Types.Footer, "footer.jpg"},
                            {Types.Header, "header.jpg"},
                            {Types.RadiobuttonOff, "gRaOffGray.png"},
                            {Types.RadiobuttonOn, "RaOnGray.png"}

                        };
                        break;
                    }
                default:
                    paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "ChOffBw.png"},
                            {Types.CheckboxOn, "ChOnBw.png"},
                            {Types.Footer, "footer.jpg"},
                            {Types.Header, "header.jpg"},
                            {Types.RadiobuttonOff, "RaOffBw.png"},
                            {Types.RadiobuttonOn, "RaOnBw.png"}

                        };
                    break;

            }
            var keys = paths.Keys;
            var updPath = new Dictionary<Types, string>();
            foreach (var key in keys)
            {
                updPath.Add(key, imagePath + paths[key]);
            }
            paths = updPath;
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
