using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;
using System.Windows.Media.Imaging;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace DomenaManager.Helpers
{
    public static class PDFOperations
    {
        public static XFont RegularFont()
        {
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            return new XFont("Calibri", 12, XFontStyle.Regular, options);
        }

        public static XFont TitleFont()
        {
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            return new XFont("Calibri", 16, XFontStyle.Bold, options);
        }

        public static PdfDocument CreateTemplate()
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.AddPage();
            PopulatePage(page);

            return doc;
        }

       public static void PopulatePage(PdfPage page)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString(
                "Dagmara Chruściel\nZarządzanie Nieruchomościami \"Domena\"\nul. Cinciały 5/1, 58-560 Jelenia Góra \nTel. 509 940 020 , zarzadca.domena@gmail.com\nNIP 6111618781 Regon 368669809",
                RegularFont(), XBrushes.Black,
                new XRect(40, 30, page.Width / 3 + 20, page.Height / 5), XStringFormats.TopLeft);

            var bitmap = Properties.Resources.Domena;
            var drawable = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            XImage img = XImage.FromBitmapSource(drawable);

            gfx.DrawImage(img, 2 * page.Width / 3, 30, (page.Width / 3) - 40, page.Width * img.PixelHeight / (3 * img.PixelWidth) - 28);
            gfx.Dispose();
        }

        public static void AddTitle(PdfPage page, string title)
        {            
            XGraphics gfx = XGraphics.FromPdfPage(page);
            var a = page.Elements.Where(x => x.Key == "XGraphics");
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(
                title,
                TitleFont(), XBrushes.Black,
                new XRect(page.Width / 3, page.Height / 5 + 30, page.Width / 3 + 20, page.Height / 5), XStringFormats.TopLeft);
            gfx.Dispose();
        }

        public static void AddChargeTable(PdfPage page, ChargeDataGrid selectedCharge)
        {
            Document doc = new Document();
            Style style = doc.Styles["Normal"];
            style.Font.Name = "Calibri";            
            style = doc.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Calibri";
            style.Font.Size = 12;

            var section = doc.AddSection();
            var ownerFrame = section.AddTextFrame();
            ownerFrame.Height = "3.0cm";
            ownerFrame.Width = "7.0cm";
            ownerFrame.Left = ShapePosition.Left;
            ownerFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            ownerFrame.Top = "5.0cm";
            ownerFrame.RelativeVertical = RelativeVertical.Page;

            var paragraph = ownerFrame.AddParagraph(selectedCharge.Owner.OwnerName + Environment.NewLine + selectedCharge.Owner.MailAddress);
            paragraph.Style = "Table";

            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(1.5), XUnit.FromCentimeter(8.5), "12cm", paragraph);
            gfx.Dispose();

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            // Save the document...
            string filename = "HelloMigraDoc.pdf";
            renderer.PdfDocument.Save(filename);
            }        
    }
}
