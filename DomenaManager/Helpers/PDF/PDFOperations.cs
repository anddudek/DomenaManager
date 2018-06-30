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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Windows;

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

            XImage img = XImage.FromGdiPlusImage(DomenaManager.Properties.Resources.Domena);

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
            var ecw = new Wizards.EditChargeWizard(selectedCharge);
            ecw.Measure(new Size(720, 750));
            ecw.Arrange(new Rect(new Size(720, 750)));
            ecw.UpdateLayout();

            /*
            


            System.Windows.Window w = new System.Windows.Window();
            w.Width = 550;
            w.Height = 600;
            w.Content = ecw;


           */
            var lv = ecw.cListView;
            lv.FontSize = 20;
            ecw.UpdateLayout();
                        
            GridView gridView = lv.View as GridView;
            if (gridView != null)
            {
                foreach (var column in gridView.Columns)
                {
                    if (double.IsNaN(column.Width))
                        column.Width = column.ActualWidth;
                    column.Width = double.NaN;
                    
                }
            }
            
            var myStream = SnapShotPNG(lv, 1);
            
                

            XImage img = XImage.FromGdiPlusImage(System.Drawing.Bitmap.FromStream(myStream));
            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.DrawImage(img, 40, page.Height / 5 + 60, page.Width - 80, (img.PointHeight / img.PointWidth) * (page.Width - 80));

            myStream.Dispose();
            gfx.Dispose();
        }

        private static MemoryStream SnapShotPNG(ListView source, int zoom)
        {
            double actualWidth = source.ActualWidth;
            source.Measure(new Size(source.ActualWidth, Double.PositiveInfinity));
            source.Arrange(new Rect(0, 0, actualWidth, source.DesiredSize.Height));
            double actualHeight = source.ActualHeight;

            double renderHeight = actualHeight * zoom;
            double renderWidth = actualWidth * zoom;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(zoom, zoom));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            MemoryStream ms = new MemoryStream();
            encoder.Save(ms);

            return ms;            
        }
    }
}
