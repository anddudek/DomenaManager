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
using Serilog;
using System.Data.Entity;

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
            Image image = paragraph.AddImage("Images/DomenaLogo.png");
            image.ScaleWidth = 0.4;

            // Create footer            
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Zarządzanie Nieruchomościami \"Domena\"   ·   +48 509 940 020   ·   zarzadca.domena@gmail.com");
            paragraph.Format.Font.Color = Colors.Gray;
            paragraph.Format.Font.Size = 9;            
            paragraph.Format.Alignment = ParagraphAlignment.Center;
        }

        public static void AddTitle(Document document, string title)
        {
            var section = document.LastSection;
            Paragraph paragraph = section.AddParagraph(title);
            paragraph.Format.SpaceBefore = "-1cm";
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceAfter = "0.5cm";            
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
            Table address = new Table();
            Column col = address.AddColumn(Unit.FromCentimeter(4));
            col.Format.Alignment = ParagraphAlignment.Left;
            col = address.AddColumn(Unit.FromCentimeter(4));
            col.Format.Alignment = ParagraphAlignment.Right;
            address.Borders.Width = 1;

            var ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Płatnik:"); ownerRow.Cells[1].AddParagraph(selectedCharge.Owner.OwnerName);
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Budynek:"); ownerRow.Cells[1].AddParagraph(selectedCharge.Building.GetAddress());
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Lokal:"); ownerRow.Cells[1].AddParagraph(selectedCharge.Apartment.ApartmentNumber.ToString());
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Ilość osób:"); ownerRow.Cells[1].AddParagraph(selectedCharge.Apartment.Locators.ToString());
            ownerRow = address.AddRow();
            double percentage;
            using (var db = new DB.DomenaDBContext())
            {
                var apartments = db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId == selectedCharge.Building.BuildingId);
                double totalArea = (apartments.Sum(x => x.AdditionalArea) + apartments.Sum(x => x.ApartmentArea));
                percentage = Math.Round(100 * (selectedCharge.Apartment.ApartmentArea + selectedCharge.Apartment.AdditionalArea) / totalArea, 2);
            }
            ownerRow.Cells[0].AddParagraph("Procent własności:"); ownerRow.Cells[1].AddParagraph(percentage.ToString() + "%");
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Powierzchnia ogółem:"); ownerRow.Cells[1].AddParagraph((selectedCharge.Apartment.AdditionalArea + selectedCharge.Apartment.ApartmentArea).ToString() + " m2");
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Powierzchnia grzewcza:"); ownerRow.Cells[1].AddParagraph((selectedCharge.Apartment.ApartmentArea).ToString() + " m2");

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.Format.RightIndent = "0";
            paragraph.AddText(selectedCharge.Owner.OwnerName + Environment.NewLine + selectedCharge.Owner.MailAddress);
            paragraph.Format.SpaceBefore = "-5.5cm";
            paragraph.Format.SpaceAfter = "3.5cm";

            address.Borders.Color = Colors.Transparent;
            document.LastSection.Add(address);

            Paragraph sep = new Paragraph();
            sep.Format.SpaceAfter = "0.5cm";
            document.LastSection.Add(sep);

            Table table = new Table();
            table.Borders.Width = 0.5;

            table.AddColumn(Unit.FromCentimeter(5.5));
            table.AddColumn(Unit.FromCentimeter(3.75));
            var column = table.AddColumn(Unit.FromCentimeter(2.25));
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn(Unit.FromCentimeter(1.25));
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn(Unit.FromCentimeter(2.25));
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Shading.Color = new Color(135, 176, 77);
            Cell cell = row.Cells[0];
            cell.AddParagraph("KATEGORIA");
            cell.Format.Font.Bold = true;
            cell = row.Cells[1];
            cell.AddParagraph("JEDNOSTKA");
            cell.Format.Font.Bold = true;
            cell = row.Cells[3];
            cell.AddParagraph("KOSZT JEDN.");
            cell.Format.Font.Bold = true;
            cell = row.Cells[4];
            cell.AddParagraph("JEDN.");
            cell.Format.Font.Bold = true;
            cell = row.Cells[5];
            cell.AddParagraph("SUMA");
            cell.Format.Font.Bold = true;
            cell = row.Cells[2];
            cell.AddParagraph("GRUPA");
            cell.Format.Font.Bold = true;

            var length = selectedCharge.Components.Count;
            int groupsCount;
            double sum = 0;
            List<BuildingChargeGroupBankAccount> bankAccounts;

            using (var db = new DB.DomenaDBContext())
            {
                bankAccounts = db.BuildingChargeGroupBankAccounts.Include(x => x.Building).Include(x => x.GroupName).Where(x => !x.IsDeleted && x.Building.BuildingId == selectedCharge.Building.BuildingId).ToList();
                foreach (var c in selectedCharge.Components)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(db.CostCategories.FirstOrDefault(x => x.BuildingChargeBasisCategoryId.Equals(c.CostCategoryId)).CategoryName);
                    cell = row.Cells[1];
                    cell.AddParagraph(EnumCostDistribution.CostDistributionToString((EnumCostDistribution.CostDistribution)c.CostDistribution));
                    cell = row.Cells[3];
                    cell.AddParagraph(c.CostPerUnit + " zł");
                    cell = row.Cells[4];

                    string units = "";
                    switch ((EnumCostDistribution.CostDistribution)c.CostDistribution)
                    {
                        default:
                            break;
                        case EnumCostDistribution.CostDistribution.PerApartmentTotalArea:
                            units = (selectedCharge.Apartment.AdditionalArea + selectedCharge.Apartment.ApartmentArea).ToString();
                            break;
                        case EnumCostDistribution.CostDistribution.PerAdditionalArea:
                            units = (selectedCharge.Apartment.AdditionalArea).ToString();
                            break;
                        case EnumCostDistribution.CostDistribution.PerApartment:
                            units = "1";
                            break;
                        case EnumCostDistribution.CostDistribution.PerApartmentArea:
                            units = (selectedCharge.Apartment.ApartmentArea).ToString();
                            break;
                        case EnumCostDistribution.CostDistribution.PerLocators:
                            units = (selectedCharge.Apartment.Locators).ToString();
                            break;
                    }
                    cell.AddParagraph(units);
                    cell = row.Cells[5];
                    cell.AddParagraph(c.Sum + " zł");
                    cell = row.Cells[2];
                    cell.AddParagraph(c.GroupName.GroupName);
                    sum += c.Sum;
                }
                var groups = selectedCharge.Components.GroupBy(x => x.GroupName.GroupName);
                row = table.AddRow();
                row.HeightRule = RowHeightRule.Exactly;
                row.Height = 1;
                row.Shading.Color = Colors.Black;
                groupsCount = groups.Count();
                foreach (var g in groups)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph("Razem - " + g.Key);
                    cell = row.Cells[5];
                    cell.AddParagraph(g.Select(c => c.Sum).Sum() + " zł");
                }
            }
            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Razem");
            cell.Format.Font.Bold = true;
            cell = row.Cells[5];
            cell.AddParagraph(sum + " zł");
            cell.Format.Font.Bold = true;

            table.SetEdge(0, 0, 6, length + 3 + groupsCount, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 1, Colors.Black);

            document.LastSection.Add(table);
            if (bankAccounts != null && bankAccounts.Count > 0 && selectedCharge.Components != null && 
                selectedCharge.Components.Count() > 0 && selectedCharge.Components.GroupBy(x => x.GroupName).Count() > 0 
                )
            {
                paragraph = document.LastSection.AddParagraph();

                paragraph.AddText("Wpłat należy dokonywać regularnie do dnia 10 każdego miesiąca na rachunek bankowy: ");

                foreach (var g in selectedCharge.Components.GroupBy(x => x.GroupName))
                {
                    var ba = bankAccounts.FirstOrDefault(x => x.GroupName.BuildingChargeGroupNameId == g.Key.BuildingChargeGroupNameId);
                    if (ba != null)
                    {
                        paragraph = document.LastSection.AddParagraph();
                        paragraph.AddText(g.Key.GroupName + ": " + ba.BankAccount);
                    }
                }
            }

            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();
            
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            // Save the document...

            try
            {
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
                    System.IO.FileInfo file = new System.IO.FileInfo("Reports\\");
                    file.Directory.Create();
                    string filename = selectedCharge.ChargeDate.ToString("MMMM_yyyy") + "_" + selectedCharge.Building.Name + "_" + selectedCharge.Apartment.ApartmentNumber + ".pdf";
                    renderer.PdfDocument.Save(Path.Combine(file.FullName, filename.Replace(' ', '_')));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd zapisu pliku - plik może być aktualnie używany. Spróbuj ponownie.");
                Log.Logger.Error(e, "Error in report file save");
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
                System.IO.FileInfo file = new System.IO.FileInfo("Reports\\");
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
