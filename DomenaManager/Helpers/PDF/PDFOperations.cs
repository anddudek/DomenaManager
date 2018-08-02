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
using System.Windows.Forms;
using LibDataModel;
using System.IO;

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

        public static Document CreateTemplate()
        {
            Document doc = new Document();
            PopulatePage(doc);

            return doc;
        }

       public static void PopulatePage(Document document)
        {
            Section section = document.AddSection();

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.RightIndent = "9cm";
            paragraph.Format.Borders.Width = 2.5;
            paragraph.Format.Borders.Color = Colors.Green;
            paragraph.Format.Borders.Distance = 3;
            paragraph.AddText("Dagmara Chruściel\nZarządzanie Nieruchomościami \"Domena\"\nul. Cinciały 5/1, 58-560 Jelenia Góra \nTel. 509 940 020 , zarzadca.domena@gmail.com\nNIP 6111618781 Regon 368669809");

            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.Format.RightIndent = 0;
            Image image = paragraph.AddImage("Images/DomenaLogo.png");
            paragraph.Format.SpaceBefore = "-4.5cm";
            image.ScaleWidth = 0.4;
            

            /*XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString(
                "Dagmara Chruściel\nZarządzanie Nieruchomościami \"Domena\"\nul. Cinciały 5/1, 58-560 Jelenia Góra \nTel. 509 940 020 , zarzadca.domena@gmail.com\nNIP 6111618781 Regon 368669809",
                RegularFont(), XBrushes.Black,
                new XRect(40, 30, page.Width / 3 + 20, page.Height / 5), XStringFormats.TopLeft);

            var bitmap = Properties.Resources.Domena;
            var drawable = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            XImage img = XImage.FromBitmapSource(drawable);

            gfx.DrawImage(img, 2 * page.Width / 3, 30, (page.Width / 3) - 40, page.Width * img.PixelHeight / (3 * img.PixelWidth) - 28);
            gfx.Dispose();*/
        }

        public static void AddTitle(Document document, string title)
        {
            var section = document.LastSection;
            Paragraph paragraph = section.AddParagraph(title);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            /*XGraphics gfx = XGraphics.FromPdfPage(page);
            var a = page.Elements.Where(x => x.Key == "XGraphics");
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(
                title,
                TitleFont(), XBrushes.Black,
                new XRect(page.Width / 3, page.Height / 5 + 30, page.Width / 3 + 20, page.Height / 5), XStringFormats.TopLeft);
            gfx.Dispose();*/
        }

        public static void AddChargeTable(Document document, ChargeDataGrid selectedCharge, bool useDefaultFolder = false)
        {
            Document doc = document;
            Style style = doc.Styles["Normal"];
            style.Font.Name = "Calibri";
            style = doc.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Calibri";
            style.Font.Size = 12;

            var section = doc.LastSection;

            /*var ownerFrame = section.AddTextFrame();
            ownerFrame.Height = "3.0cm";
            ownerFrame.Width = "7.0cm";
            ownerFrame.Left = ShapePosition.Right;
            ownerFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            ownerFrame.Top = "12.0cm";
            ownerFrame.RelativeVertical = RelativeVertical.Page;
            var paragraph = ownerFrame.AddParagraph(selectedCharge.Owner.OwnerName + Environment.NewLine + selectedCharge.Owner.MailAddress);
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Style = "Table";
            paragraph.Format.SpaceAfter = "1cm";

            paragraph = section.AddParagraph(selectedCharge.Owner.OwnerName + Environment.NewLine + selectedCharge.Owner.MailAddress);
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Style = "Table";
            paragraph.Format.SpaceAfter = "1cm";*/

            Table address = new Table();
            Column col = address.AddColumn(Unit.FromCentimeter(8.5));
            col.Format.Alignment = ParagraphAlignment.Left;
            col = address.AddColumn(Unit.FromCentimeter(8.5));
            col.Format.Alignment = ParagraphAlignment.Left;
            address.Borders.Width = 0;
            Row addressRow = address.AddRow();
            addressRow.Cells[0].AddParagraph(selectedCharge.Owner.OwnerName + Environment.NewLine + selectedCharge.Owner.MailAddress);
            string apartmentsFrame = "Dotyczy mieszkań nr: " + selectedCharge.Apartment.ApartmentNumber + Environment.NewLine + "Budynek: " + selectedCharge.Building.GetAddress();
            addressRow.Cells[1].AddParagraph(apartmentsFrame);

            address.SetEdge(0, 0, 2, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0, Colors.Transparent);
            address.Format.SpaceBefore = "1cm";
            address.Format.SpaceAfter = "1cm";

            document.LastSection.Add(address);

            Table table = new Table();
            table.Borders.Width = 0.5;

            table.AddColumn(Unit.FromCentimeter(7));
            table.AddColumn(Unit.FromCentimeter(3.75));
            var column = table.AddColumn(Unit.FromCentimeter(2.5));
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn(Unit.FromCentimeter(1.25));
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn(Unit.FromCentimeter(2.5));
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Shading.Color = Colors.Green;
            Cell cell = row.Cells[0];
            cell.AddParagraph("KATEGORIA");
            cell.Format.Font.Bold = true;
            cell = row.Cells[1];
            cell.AddParagraph("JEDNOSTKA");
            cell.Format.Font.Bold = true;
            cell = row.Cells[2];
            cell.AddParagraph("KOSZT JEDN.");
            cell.Format.Font.Bold = true;
            cell = row.Cells[3];
            cell.AddParagraph("JEDN.");
            cell.Format.Font.Bold = true;
            cell = row.Cells[4];
            cell.AddParagraph("SUMA");
            cell.Format.Font.Bold = true;

            /*
            Row apartmentRow = table.AddRow();
            apartmentRow.Cells[0].Format.Font.Bold = true;
            apartmentRow.Cells[0].AddParagraph(selectedCharge.Apartment.ApartmentNumber.ToString());
            */

            var length = selectedCharge.Components.Count;
            double sum = 0;

            using (var db = new DB.DomenaDBContext())
            {
                foreach (var c in selectedCharge.Components)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(db.CostCategories.FirstOrDefault(x => x.BuildingChargeBasisCategoryId.Equals(c.CostCategoryId)).CategoryName);
                    cell = row.Cells[1];
                    cell.AddParagraph(EnumCostDistribution.CostDistributionToString((EnumCostDistribution.CostDistribution)c.CostDistribution));
                    cell = row.Cells[2];
                    cell.AddParagraph(c.CostPerUnit + " zł");
                    cell = row.Cells[3];
                    if (c.CostDistribution == 0)
                    {
                        cell.AddParagraph("1");
                    }
                    else if (c.CostDistribution == 1)
                    {
                        var area = selectedCharge.Apartment.AdditionalArea + selectedCharge.Apartment.ApartmentArea;
                        cell.AddParagraph(area.ToString());
                    }
                    cell = row.Cells[4];
                    cell.AddParagraph(c.Sum + " zł");
                    sum += c.Sum;
                }
            }
            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Razem");
            cell = row.Cells[4];
            cell.AddParagraph(sum + " zł");

            table.SetEdge(0, 0, 5, length + 2, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);

            document.LastSection.Add(table);

            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            /*XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(1.5), XUnit.FromCentimeter(8.5), "12cm", paragraph);
            gfx.Dispose();*/

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            // Save the document...
            //string filename = "HelloMigraDoc.pdf";
            //renderer.PdfDocument.Save(filename);
            if (!useDefaultFolder)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF file|*.pdf";
                sfd.Title = "Zapisz raport jako...";
                sfd.ShowDialog();
                if (sfd.FileName != "")
                {
                    renderer.PdfDocument.Save(sfd.FileName);
                }
            }
            else
            {
                System.IO.FileInfo file = new System.IO.FileInfo("C:\\DomenaManager\\Reports\\");
                file.Directory.Create();
                string filename = selectedCharge.ChargeDate.ToString("MMMM_yyyy") + "_" + selectedCharge.Building.Name + "_" + selectedCharge.Apartment.ApartmentNumber + ".pdf";
                renderer.PdfDocument.Save(Path.Combine(file.FullName, filename.Replace(' ', '_')));                
            }
        }

        public static void AddSummaryTable(Document document, System.Windows.Controls.DataGrid selectedDG, int year, Apartment apartment, Owner owner, Building building, bool useDefaultFolder = false)
        {
            Document doc = document;
            Style style = doc.Styles["Normal"];
            style.Font.Name = "Calibri";
            style = doc.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Calibri";
            style.Font.Size = 12;

            var section = doc.LastSection;

            Table address = new Table();
            Column col = address.AddColumn(Unit.FromCentimeter(8.5));
            col.Format.Alignment = ParagraphAlignment.Left;
            col = address.AddColumn(Unit.FromCentimeter(8.5));
            col.Format.Alignment = ParagraphAlignment.Left;
            address.Borders.Width = 0;
            Row addressRow = address.AddRow();
            addressRow.Cells[0].AddParagraph(owner.OwnerName + Environment.NewLine + owner.MailAddress);
            string apartmentsFrame = "Dotyczy mieszkań nr: " + apartment.ApartmentNumber + Environment.NewLine + "Budynek: " + building.GetAddress();
            addressRow.Cells[1].AddParagraph(apartmentsFrame);

            address.SetEdge(0, 0, 2, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0, Colors.Transparent);
            address.Format.SpaceBefore = "1cm";
            address.Format.SpaceAfter = "1cm";

            document.LastSection.Add(address);

            var iSrc = ((SummaryDataGridRow[])selectedDG.ItemsSource);
            string[,] data = new string[iSrc[0].charges.Length + 2, 14];
            for (int j = 0; j < iSrc.Length; j++)
            {
                SummaryDataGridRow dr = iSrc[j];
                //string[] data = new string[dr.charges.Length + 2];
                data[0,j] = dr.month;
                for (int i = 0; i < dr.charges.Length; i++)
                {
                    data[i + 1,j] = dr.charges[i];
                }
                data[dr.charges.Length + 1, j] = data[dr.charges.Length - 1, j];
                data[dr.charges.Length - 1, j] = dr.chargesSum;

                var temp = data[dr.charges.Length, j];
                data[dr.charges.Length, j] = data[dr.charges.Length + 1, j];
                data[dr.charges.Length + 1, j] = temp;
            }            
                        
            Table table = new Table();
            table.Borders.Width = 0.5;

            table.AddColumn(Unit.FromCentimeter(2));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            table.AddColumn(Unit.FromCentimeter(1.1));
            
            Row monthRow = table.AddRow();
            monthRow.VerticalAlignment = VerticalAlignment.Center;
            monthRow.BottomPadding = "0.2cm";
            monthRow.Shading.Color = Colors.Green;

            Cell title = monthRow.Cells[0];
            title.AddParagraph("Kategoria");

            for (int i = 0; i < 14; i++)
            {
                Cell cell = monthRow.Cells[i+1];
                TextFrame tf = cell.AddTextFrame();               
                tf.Orientation = TextOrientation.Upward;
                var p = tf.AddParagraph(data[0, i]);
            }

            int add = 0;
            for (int i = 0; i<iSrc[0].charges.Length + 1; i++)
            {
                Row row = table.AddRow();
                row.Format.Font.Size = Unit.FromPoint(8);
                row.VerticalAlignment = VerticalAlignment.Center;
                Cell cell = row.Cells[0];
                if (i == iSrc[0].charges.Length - 2)
                {
                    add++;
                }
                cell.AddParagraph((string)selectedDG.Columns[1 + i + add].Header);
                for (int j = 0; j < 14; j++)
                {
                    cell = row.Cells[j + 1];
                    var dataToFill = data[i + 1, j];
                    cell.AddParagraph(dataToFill);
                }
            }

            table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);

            document.LastSection.Add(table);

            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();

            if (!useDefaultFolder)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF file|*.pdf";
                sfd.Title = "Zapisz raport jako...";
                sfd.ShowDialog();
                if (sfd.FileName != "")
                {
                    renderer.PdfDocument.Save(sfd.FileName);
                }
            }
            else
            {
                System.IO.FileInfo file = new System.IO.FileInfo("C:\\DomenaManager\\Reports\\");
                file.Directory.Create();
                string filename = year.ToString() + "_" + building.Name + "_" + apartment.ApartmentNumber + ".pdf";
                renderer.PdfDocument.Save(Path.Combine(file.FullName, filename.Replace(' ', '_')));
            }
        }

        public static void PrepareSingleChargeReport(ChargeDataGrid selectedCharge, bool useDefaultFolder)
        {
            Document doc = CreateTemplate();
            AddTitle(doc, "Naliczenie z dnia: " + selectedCharge.ChargeDate.ToString("dd-MM-yyyy"));
            AddChargeTable(doc, selectedCharge, useDefaultFolder);
        }

        public static void PrepareSingleYearSummary(System.Windows.Controls.DataGrid selectedDG, int year, Apartment apartment, Owner owner, Building building, bool useDefaultFolder)
        {
            Document doc = CreateTemplate();
            AddTitle(doc, "Zestawienie roczne - " + year);
            AddSummaryTable(doc, selectedDG, year, apartment, owner, building, false);
        }
    }
}
