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
using System.Globalization;

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

        public static Document CreateTemplate(Owner owner)
        {
            Document doc = new Document();
            PopulatePage(doc, owner);

            return doc;
        }

       public static void PopulatePage(Document document, Owner owner)
        {
            // Create header
            Section section = document.AddSection();
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.TopMargin = Unit.FromCentimeter(5.5);
            Paragraph paragraph = section.Headers.Primary.AddParagraph();
            paragraph.Format.RightIndent = "9cm";
            paragraph.Format.SpaceBefore = "-1cm";
            Image image = paragraph.AddImage("Images/DomenaLogo.png");
            image.ScaleWidth = 0.4;

            paragraph = section.Headers.Primary.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.Format.RightIndent = "0";
            paragraph.AddText(owner.OwnerName + Environment.NewLine + owner.MailAddress);
            paragraph.Format.SpaceBefore = "-4.5cm";
            paragraph.Format.SpaceAfter = "3.5cm";

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
            //paragraph.Format.SpaceBefore = "-1cm";
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceAfter = "0.5cm";            
        }

        public static Table OwnerTableInfo(Owner owner, Building building, Apartment apartment)
        {
            Table address = new Table();
            Column col = address.AddColumn(Unit.FromCentimeter(4));
            col.Format.Alignment = ParagraphAlignment.Left;
            col = address.AddColumn(Unit.FromCentimeter(4));
            col.Format.Alignment = ParagraphAlignment.Right;
            address.Borders.Width = 1;

            var ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Płatnik:"); ownerRow.Cells[1].AddParagraph(owner.OwnerName);
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Budynek:"); ownerRow.Cells[1].AddParagraph(building.GetAddress());
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Lokal:"); ownerRow.Cells[1].AddParagraph(apartment.ApartmentNumber.ToString());
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Ilość osób:"); ownerRow.Cells[1].AddParagraph(apartment.Locators.ToString());
            ownerRow = address.AddRow();
            double percentage;
            using (var db = new DB.DomenaDBContext())
            {
                var apartments = db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId == building.BuildingId);
                double totalArea = (apartments.Sum(x => x.AdditionalArea) + apartments.Sum(x => x.ApartmentArea));
                percentage = Math.Round(100 * (apartment.ApartmentArea + apartment.AdditionalArea) / totalArea, 2);
            }
            ownerRow.Cells[0].AddParagraph("Procent własności:"); ownerRow.Cells[1].AddParagraph(percentage.ToString() + "%");
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Powierzchnia ogółem:"); ownerRow.Cells[1].AddParagraph((apartment.AdditionalArea + apartment.ApartmentArea).ToString() + " m2");
            ownerRow = address.AddRow();
            ownerRow.Cells[0].AddParagraph("Powierzchnia grzewcza:"); ownerRow.Cells[1].AddParagraph((apartment.ApartmentArea).ToString() + " m2");

            address.Borders.Color = Colors.Transparent;
            return address;
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
            Table address = OwnerTableInfo(selectedCharge.Owner, selectedCharge.Building, selectedCharge.Apartment);
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
                Paragraph paragraph = document.LastSection.AddParagraph();

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
            // Initial data

            Document doc = document;
            Style style = doc.Styles["Normal"];
            style.Font.Name = "Calibri";
            style = doc.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Calibri";
            style.Font.Size = 12;

            var section = doc.LastSection;

            // Tables

            Table summaryTable = new Table();
            Row[] summaryRows = new Row[14];
            double[] paymentsSum = new double[12];
            double[] chargesSum = new double[12];
            int distinctGroupsCount;

            using (var db = new DB.DomenaDBContext())
            {
                var charges = db.Charges.Include(x => x.Components).Include(x => x.Components.Select(y => y.GroupName)).Where(x => !x.IsDeleted && x.ApartmentId.Equals(apartment.ApartmentId) && x.ChargeDate.Year.Equals(year));
                var componentsList = new List<ChargeComponent>();
                foreach (var charge in charges)
                {
                    componentsList.AddRange(charge.Components);
                }
                var groups = componentsList.Select(x => x.GroupName);

                var payments = db.Payments.Include(x => x.ChargeGroup).Where(x => !x.IsDeleted && x.ApartmentId == apartment.ApartmentId && x.PaymentRegistrationDate.Year == year);
                var paymentGroups = payments.Select(x => x.ChargeGroup);

                var allGroups = new List<BuildingChargeGroupName>();
                allGroups.AddRange(groups);
                allGroups.AddRange(paymentGroups);
                var distinctGroups = allGroups.Distinct();
                distinctGroupsCount = distinctGroups.Count();

                var uniqueCategories = componentsList.GroupBy(x => x.CostCategoryId).Select(x => x.FirstOrDefault()).Select(x => x.CostCategoryId);

                summaryTable.Borders.Width = 0.5;
                summaryTable.AddColumn(Unit.FromCentimeter(2));
                int columnsCount = distinctGroups.Count() * 2 + 3;
                for (int i = 0; i < columnsCount; i++)
                {
                    var column = summaryTable.AddColumn(Unit.FromCentimeter((double)17 / columnsCount));
                    column.Format.Alignment = ParagraphAlignment.Center;
                }

                // |--Podsumowanie--|
                var summaryRow = summaryTable.AddRow();
                summaryRow.Shading.Color = new Color(135, 176, 77);
                summaryRow.Cells[0].MergeRight = columnsCount;
                summaryRow.Cells[0].AddParagraph("Podsumowanie");
                summaryRow.Cells[0].Format.Font.Bold = true;
                summaryRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;

                // |--Miesiąc--|--G1--|--G2--|--Razem--|
                summaryRow = summaryTable.AddRow();
                summaryRow.Shading.Color = new Color(135, 176, 77);
                summaryRow.Cells[0].MergeDown = 1;
                summaryRow.Cells[0].AddParagraph("Miesiąc");
                summaryRow.Cells[0].Format.Font.Bold = true;
                summaryRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                summaryRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                int iter = 1;
                foreach (var dg in distinctGroups)
                {
                    summaryRow.Cells[iter].MergeRight = 1;
                    summaryRow.Cells[iter].AddParagraph(dg.GroupName);
                    summaryRow.Cells[iter].Format.Font.Bold = true;
                    summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                    iter += 2;
                }
                summaryRow.Cells[iter].MergeRight = 2;
                summaryRow.Cells[iter].AddParagraph("Razem");
                summaryRow.Cells[iter].Format.Font.Bold = true;
                summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;

                // |--Miesiąc (merged)--|--G1 wpłaty--|--G1 naliczenia--|--Suma wpłat--|--Suma naliczeń--|--Saldo--|
                summaryRow = summaryTable.AddRow();
                summaryRow.Shading.Color = new Color(135, 176, 77);
                iter = 1;
                foreach (var dg in distinctGroups)
                {
                    summaryRow.Cells[iter].AddParagraph(dg.GroupName + " wpłaty");
                    summaryRow.Cells[iter].Format.Font.Bold = true;
                    summaryRow.Cells[iter].VerticalAlignment = VerticalAlignment.Center;
                    summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                    iter++;
                    summaryRow.Cells[iter].AddParagraph(dg.GroupName + " naliczenia");
                    summaryRow.Cells[iter].Format.Font.Bold = true;
                    summaryRow.Cells[iter].VerticalAlignment = VerticalAlignment.Center;
                    summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                    iter++;
                }
                summaryRow.Cells[iter].AddParagraph("Suma wpłat");
                summaryRow.Cells[iter].Format.Font.Bold = true;
                summaryRow.Cells[iter].VerticalAlignment = VerticalAlignment.Center;
                summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                iter++;
                summaryRow.Cells[iter].AddParagraph("Suma naliczeń");
                summaryRow.Cells[iter].Format.Font.Bold = true;
                summaryRow.Cells[iter].VerticalAlignment = VerticalAlignment.Center;
                summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                iter++;
                summaryRow.Cells[iter].AddParagraph("Saldo");
                summaryRow.Cells[iter].Format.Font.Bold = true;
                summaryRow.Cells[iter].VerticalAlignment = VerticalAlignment.Center;
                summaryRow.Cells[iter].Format.Alignment = ParagraphAlignment.Center;
                iter++;

                summaryRows[0] = summaryTable.AddRow();
                summaryRows[0].Cells[0].AddParagraph("Zeszły rok");
                for (int i = 1; i< columnsCount; i++)
                {
                    summaryRows[0].Cells[i].AddParagraph("-");
                }
                double lastYearSaldo = Payments.CalculateSaldo(year - 1, apartment);
                double thisYearSaldo = lastYearSaldo;
                summaryRows[0].Cells[columnsCount].AddParagraph(lastYearSaldo + " zł");

                for (int i = 1; i < 13; i++)//months
                {
                    summaryRows[i] = summaryTable.AddRow();
                    summaryRows[i].Cells[0].AddParagraph(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i, 1).ToString("MMMM")));
                }
                
                summaryRows[13] = summaryTable.AddRow();
                summaryRows[13].Cells[0].AddParagraph("Razem");
                summaryRows[13].Cells[0].Format.Font.Bold = true;

                int summaryGroupIterator = 1;
                foreach (var g in distinctGroups)
                {
                    if (g != null)
                    {
                        Table address = OwnerTableInfo(owner, building, apartment);
                        document.LastSection.Add(address);

                        Paragraph sep = new Paragraph();
                        sep.Format.SpaceAfter = "0.5cm";
                        document.LastSection.Add(sep);

                        // iterujemy po kazdej grupie a w srodku po kazdej kategorii + Wplty w grupie + suma
                        var categoriesInGroup = componentsList.Where(x => x.GroupName != null && x.GroupName.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId).GroupBy(x => x.CostCategoryId).Select(x => x.FirstOrDefault()).Select(x => x.CostCategoryId);

                        Table groupTable = new Table();
                        groupTable.Borders.Width = 0.5;

                        groupTable.AddColumn(Unit.FromCentimeter(2));
                        Column col;
                        foreach (var cat in categoriesInGroup)
                        {
                            col = groupTable.AddColumn(Unit.FromCentimeter((double)14 / categoriesInGroup.Count()));
                            col.Format.Alignment = ParagraphAlignment.Center;
                        }
                        col = groupTable.AddColumn(Unit.FromCentimeter(1.5));
                        col.Format.Alignment = ParagraphAlignment.Center;
                        col = groupTable.AddColumn(Unit.FromCentimeter(1.5));
                        col.Format.Alignment = ParagraphAlignment.Center;

                        // Header row
                        Row currentRow = groupTable.AddRow();
                        currentRow.Shading.Color = new Color(135, 176, 77);
                        Cell cell = currentRow.Cells[0];
                        cell.AddParagraph(g.GroupName);
                        cell.Format.Font.Bold = true;
                        cell.Format.Alignment = ParagraphAlignment.Center;
                        cell.MergeRight = categoriesInGroup.Count() + 2;

                        currentRow = groupTable.AddRow();
                        currentRow.Shading.Color = new Color(135, 176, 77);
                        cell = currentRow.Cells[0];
                        cell.AddParagraph("Miesiąc");
                        cell.Format.Font.Bold = true;
                        var iterator = 1;
                        foreach (var cat in categoriesInGroup)
                        {
                            cell = currentRow.Cells[iterator];
                            cell.AddParagraph(db.CostCategories.FirstOrDefault(x => x.BuildingChargeBasisCategoryId == cat).CategoryName);
                            cell.Format.Font.Bold = true;
                            iterator++;
                        }

                        cell = currentRow.Cells[iterator];
                        cell.AddParagraph("Wpłaty");
                        cell.Format.Font.Bold = true;
                        
                        cell = currentRow.Cells[iterator+1];
                        cell.AddParagraph("Saldo");
                        cell.Format.Font.Bold = true;

                        double currentYear = 0;
                        for (int i = 1; i < 13; i++)//months
                        {
                            currentRow = groupTable.AddRow();
                            cell = currentRow.Cells[0];
                            cell.AddParagraph(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i, 1).ToString("MMMM")));
                            
                            var thisMonthComponents = new List<ChargeComponent>();
                            charges.Where(x => x.ChargeDate.Month == i).ToList().ForEach(x => thisMonthComponents.AddRange(x.Components));
                                                        
                            double groupSum = 0;
                            iterator = 1;
                            foreach (var cat in categoriesInGroup)
                            {
                                var currentComponets = thisMonthComponents.Where(x => x.CostCategoryId == cat && x.GroupName.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId);
                                groupSum += currentComponets.Sum(x => x.Sum);
                                currentRow.Cells[iterator].AddParagraph(currentComponets.Sum(x => x.Sum).ToString() + " zł");
                                iterator++;
                            }
                            //Wplaty
                            var groupPayments = payments.Where(x => x.PaymentRegistrationDate.Month == i && x.ChargeGroup.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                            currentRow.Cells[iterator].AddParagraph(groupPayments.ToString() + " zł");
                            iterator++;
                            summaryRows[i].Cells[summaryGroupIterator].AddParagraph(groupPayments.ToString() + " zł");
                            paymentsSum[i - 1] += groupPayments;

                            //Suma
                            currentRow.Cells[iterator].AddParagraph((groupPayments - groupSum).ToString() + " zł");//(groupPayments - groupSum).ToString() + " zł";       
                            currentYear += groupPayments - groupSum;
                            summaryRows[i].Cells[summaryGroupIterator + 1].AddParagraph(groupSum.ToString() + " zł");
                            chargesSum[i - 1] += groupSum;
                        }

                        currentRow = groupTable.AddRow();
                        cell = currentRow.Cells[0];
                        cell.AddParagraph("Razem");
                        cell.Format.Font.Bold = true;
                        iterator = 1;
                        foreach (var cat in categoriesInGroup)
                        {
                            cell = currentRow.Cells[iterator];
                            cell.AddParagraph("-");
                            cell.Format.Font.Bold = true;
                            iterator++;
                        }

                        cell = currentRow.Cells[iterator];
                        cell.Format.Font.Bold = true;
                        cell.AddParagraph("-");

                        cell = currentRow.Cells[iterator + 1];
                        cell.Format.Font.Bold = true;
                        cell.AddParagraph(currentYear.ToString() + " zł");

                        document.LastSection.Add(groupTable);
                        section.AddPageBreak();

                        summaryRows[13].Cells[summaryGroupIterator].AddParagraph("-");
                        summaryRows[13].Cells[summaryGroupIterator].Format.Font.Bold = true;
                        summaryRows[13].Cells[summaryGroupIterator + 1].AddParagraph("-");
                        summaryRows[13].Cells[summaryGroupIterator + 1].Format.Font.Bold = true;
                        summaryGroupIterator += 2;
                    }
                }
            }

            // Summary table

            for (int i = 0; i < 12; i++)
            {
                summaryRows[i + 1].Cells[distinctGroupsCount * 2 + 1].AddParagraph(paymentsSum[i].ToString() + " zł");
                summaryRows[i + 1].Cells[distinctGroupsCount * 2 + 2].AddParagraph(chargesSum[i].ToString() + " zł");
                summaryRows[i + 1].Cells[distinctGroupsCount * 2 + 3].AddParagraph((paymentsSum[i] - chargesSum[i]).ToString() + " zł");
            }
            summaryRows[13].Cells[distinctGroupsCount * 2 + 1].AddParagraph(paymentsSum.DefaultIfEmpty(0).Sum().ToString() + " zł");
            summaryRows[13].Cells[distinctGroupsCount * 2 + 1].Format.Font.Bold = true;
            summaryRows[13].Cells[distinctGroupsCount * 2 + 2].AddParagraph(chargesSum.DefaultIfEmpty(0).Sum().ToString() + " zł");
            summaryRows[13].Cells[distinctGroupsCount * 2 + 2].Format.Font.Bold = true;
            summaryRows[13].Cells[distinctGroupsCount * 2 + 3].AddParagraph((paymentsSum.DefaultIfEmpty(0).Sum() - chargesSum.DefaultIfEmpty(0).Sum()).ToString() + " zł");
            summaryRows[13].Cells[distinctGroupsCount * 2 + 3].Format.Font.Bold = true;

            document.LastSection.Add(summaryTable);

            // Finish document and save

            MigraDoc.Rendering.DocumentRenderer docRenderer = new MigraDoc.Rendering.DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();

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
                    string filename = year.ToString() + "_" + building.Name + "_" + apartment.ApartmentNumber + ".pdf";
                    renderer.PdfDocument.Save(Path.Combine(file.FullName, filename.Replace(' ', '_')));
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("Error during pdf save", e);
                MessageBox.Show("Błąd zapisu pliku pdf. Zamknij wszystkie otwarte dokumenty pdf i spróbuj ponownie.");
            }
        }       

        public static void PrepareSingleChargeReport(ChargeDataGrid selectedCharge, bool useDefaultFolder)
        {
            Document doc = CreateTemplate(selectedCharge.Owner);
            AddTitle(doc, "Naliczenie z dnia: " + selectedCharge.ChargeDate.ToString("dd-MM-yyyy"));
            AddChargeTable(doc, selectedCharge, useDefaultFolder);
        }

        public static void PrepareSingleYearSummary(System.Windows.Controls.DataGrid selectedDG, int year, Apartment apartment, Owner owner, Building building, bool useDefaultFolder)
        {
            Document doc = CreateTemplate(owner);
            AddTitle(doc, "Zestawienie roczne - " + year);
            AddSummaryTable(doc, selectedDG, year, apartment, owner, building, false);
        }
    }
}
