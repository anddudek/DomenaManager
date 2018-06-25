using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;

namespace DomenaManager.Helpers
{
    public static class PDFOperations
    {
        public static PdfDocument CreateTemplate()
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont standardFont = new XFont("Calibri", 12);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString("Domena \nManager", standardFont, XBrushes.Black,
                new XRect(40, 30, page.Width / 3 - 20, page.Height / 5), XStringFormats.TopLeft);
            
            XImage img = XImage.FromGdiPlusImage(DomenaManager.Properties.Resources.Domena);
            
            gfx.DrawImage(img, 2 * page.Width / 3, 30, (page.Width / 3) - 40, page.Width * img.PixelHeight / (3 * img.PixelWidth) - 28);

            return doc;
        }
    }
}
