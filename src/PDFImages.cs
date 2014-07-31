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
        protected double headerHeight = -1;
        protected double footerHeight = -1;
        private Dictionary<Types, string> paths;
        protected string imagePath = null;
        protected string assemblyName = null;
        public PDFImages(ColorProfile colorScheme, string header_path, string footer_path){
            assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            imagePath = assemblyName + ".Images.";
   
            switch (colorScheme){
                case ColorProfile.Color:
                    {
                        paths = new Dictionary<Types, string>()
                        {
                            {Types.CheckboxOff, "ChOffColor.png"},
                            {Types.CheckboxOn, "ChOnColor.png"},
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

            updPath.Add(Types.Header, !string.IsNullOrEmpty(header_path) ? header_path : imagePath + "header.jpg");
            updPath.Add(Types.Footer, !string.IsNullOrEmpty(footer_path) ? footer_path : imagePath + "footer.jpg");

            paths = updPath;
        }
        protected void loadImages()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            images = new Dictionary<Types, Stream>(6);
            foreach (Types type in Enum.GetValues(typeof(Types)))
            {
                if (paths[type].StartsWith(assemblyName))
                {
                    images[type] = assembly.GetManifestResourceStream(paths[type]);
                }
                else
                {
                    var img_stream = new System.IO.MemoryStream();

                    var file_str = (Stream)File.OpenRead(paths[type]);
                    byte[] bytes = new byte[file_str.Length];
                    file_str.Read(bytes, 0, (int)file_str.Length);
                    img_stream.Write(bytes, 0, (int)file_str.Length);
                    file_str.Close();

                    images[type] = img_stream;
                }
            }
            
        }
        public double HeaderHeight
        {
            get
            {
                if (headerHeight == -1)
                {
                    headerHeight = this.Get(Types.Header).PointHeight;
                }
                return headerHeight;
            }
        }
        public double FooterHeight
        {
            get
            {
                if (footerHeight == -1)
                {
                    footerHeight = this.Get(Types.Footer).PointHeight;
                }
                return footerHeight;
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
