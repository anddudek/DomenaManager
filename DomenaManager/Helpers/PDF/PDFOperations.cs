using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

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
            gfx.DrawString("Domena", standardFont, XBrushes.Black,
                new XRect(10, 10, page.Width / 3, page.Height / 5), XStringFormats.TopLeft);
            
            XImage img = XImage.FromGdiPlusImage(DomenaManager.Properties.Resources.Domena);
            gfx.DrawImage(img, 2 * page.Width / 3, 10, page.Width / 3, page.Height / 5);

            return doc;
        }
    }
}
