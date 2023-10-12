using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public static class ImportPurchasingBookReportPdfTemplate
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _smallerBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, BaseColor.White);

        public static MemoryStream Generate(LocalPurchasingBookReportViewModel viewModel, int timezoneOffset, DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
            var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

            var document = new Document(PageSize.A4.Rotate(), 5, 5, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            SetHeader(document, d1, d2, timezoneOffset);
            document.Add(new Paragraph("\n"));
            SetReportTable(document, viewModel, timezoneOffset);
            //document.Add(new Paragraph("\n"));
            //SetCategoryCurrencySummaryTable(document, viewModel.CategorySummaries, viewModel.CategorySummaryTotal, viewModel.CurrencySummaries);
            //SetFooter(document);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetHeader(Document document, DateTime dateFrom, DateTime dateTo, int timezoneOffset)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 95
            };
            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Phrase = new Phrase("PT EFRATA GARMINDO UTAMA", _headerFont)
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase("BUKU PEMBELIAN IMPOR", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"Dari {dateFrom.AddHours(timezoneOffset):yyyy-dd-MM} Sampai {dateTo.AddHours(timezoneOffset):yyyy-dd-MM}", _subHeaderFont);
            table.AddCell(cell);

            document.Add(table);
        }

        private static void SetReportTableHeader(PdfPTable table)
        {
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new BaseColor(23, 50, 80)
            };

            var cellColspan5 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 5,
                BackgroundColor = new BaseColor(23, 50, 80)
            };

            var cellRowspan2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 2,
                BackgroundColor = new BaseColor(23, 50, 80)
            };

            var cellColRowspan2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 2,
                Rowspan = 2,
                BackgroundColor = new BaseColor(23, 50, 80)
            };

            cellRowspan2.Phrase = new Phrase("Tanggal", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kode Suppl.", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Supplier", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Keterangan", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Bon Penerimaan", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Inv.", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. SPB/NI", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kategori", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Unit", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellColspan5.Phrase = new Phrase("PIB", _smallerBoldWhiteFont);
            table.AddCell(cellColspan5);

            cellRowspan2.Phrase = new Phrase("Ket. Nilai Impor", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellColRowspan2.Phrase = new Phrase("Nilai", _smallerBoldWhiteFont);
            table.AddCell(cellColRowspan2);

            cellRowspan2.Phrase = new Phrase("Total (IDR)", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cell.Phrase = new Phrase("Tanggal", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("No", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("BM", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPH Impor", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPN Impor", _smallerBoldWhiteFont);
            table.AddCell(cell);
        }

        private static void SetReportTable(Document document, LocalPurchasingBookReportViewModel viewModel, int timezoneOffset)
        {
            var table = new PdfPTable(18)
            {
                WidthPercentage = 97
            };
            table.SetWidths(new float[] { 5f, 5f, 7f, 6f, 6f, 5f, 5f, 7f, 6f, 5f, 5f, 5f, 5f, 5f, 5f, 6f, 7f, 9f, });

            SetReportTableHeader(table);

            var cellNoBorderRight = new PdfPCell()
            {
                BorderWidthRight = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellNoBorderLeft = new PdfPCell()
            {
                BorderWidthLeft = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var listCategoryReports = viewModel.Reports.Where(x => x.AccountingUnitName != null).OrderBy(order => order.AccountingLayoutIndex).GroupBy(x => x.AccountingCategoryName).ToList();
            var summaryUnit = new Dictionary<string, decimal>();

            foreach (var cat in listCategoryReports)
            {
                var categoryName = cat.Select(x => x.AccountingCategoryName).FirstOrDefault();
                cellAlignLeft.Phrase = new Phrase(categoryName, _smallerBoldFont);
                cellAlignLeft.Colspan = 18;
                table.AddCell(cellAlignLeft);

                var totalUnit = new Dictionary<string, decimal>();
                var totalCurrency = new Dictionary<string, Dictionary<string, decimal>>();
                decimal total = 0;

                foreach (var element in cat)
                {
                    var dateReceipt = element.ReceiptDate.HasValue ? element.ReceiptDate.GetValueOrDefault().ToString("yyyy-dd-MM") : string.Empty;
                    var dateCorrection = element.CorrectionDate.HasValue ? element.CorrectionDate.GetValueOrDefault().ToString("yyyy-dd-MM") : string.Empty;
                    var datePib = element.PIBDate.HasValue ? element.PIBDate.GetValueOrDefault().ToString("yyyy-dd-MM") : string.Empty;

                    cell.Phrase = new Phrase(dateReceipt, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.SupplierCode, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.SupplierName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.Remark, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.URNNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.InvoiceNo, _smallerFont);
                    table.AddCell(cell);

                    //cell.Phrase = new Phrase(element.UPONo, _smallerFont);
                    if(element.DataSourceSort ==1)
                    {
                        cell.Phrase = new Phrase(element.UPONo, _smallerFont);
                    }
                    else
                    {
                        cell.Phrase = new Phrase(element.CorrectionNo, _smallerFont);
                    }
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.CategoryCode + " - " + element.CategoryName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.AccountingUnitName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(datePib, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.PIBNo, _smallerFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", element.PIBBM), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", element.PIBIncomeTax), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", element.PIBVat), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cell.Phrase = new Phrase(element.PIBImportInfo, _smallerFont);
                    table.AddCell(cell);

                    cellNoBorderRight.Phrase = new Phrase(element.CurrencyCode, _smallerFont);
                    table.AddCell(cellNoBorderRight);

                    cellNoBorderLeft.Phrase = new Phrase(string.Format("{0:n}", element.DPP), _smallerFont);
                    table.AddCell(cellNoBorderLeft);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", element.Total), _smallerFont);
                    table.AddCell(cellAlignRight);

                    if (totalUnit.ContainsKey(element.AccountingUnitName))
                        totalUnit[element.AccountingUnitName] += element.Total;
                    else
                        totalUnit.Add(element.AccountingUnitName, element.Total);

                    if (totalCurrency.ContainsKey(element.CurrencyCode))
                    {

                        totalCurrency[element.CurrencyCode]["DPP"] += element.DPP;
                        totalCurrency[element.CurrencyCode]["Total"] += element.Total;
                    }
                    else
                    { 
                        totalCurrency.Add(element.CurrencyCode, new Dictionary<string, decimal>()
                        {
                            {"DPP", element.DPP },
                            {"Total", element.Total }
                        });
                    }

                    total += element.Total;
                }

                if (totalCurrency.Count() > 0)
                    cellAlignRight.Phrase = new Phrase("TOTAL " + categoryName, _smallerBoldFont);
                    cellAlignRight.Colspan = 15;
                    cellAlignRight.Rowspan = totalCurrency.Count();
                    table.AddCell(cellAlignRight);
                    cellAlignRight.Colspan = 1;
                    cellAlignRight.Rowspan = 1;
                    foreach (var v in totalCurrency)
                    {
                        cellNoBorderRight.Phrase = new Phrase(v.Key, _smallerBoldFont);
                        table.AddCell(cellNoBorderRight);

                        foreach (var x in v.Value)
                        {
                            cellNoBorderLeft.Phrase = new Phrase(string.Format("{0:n}", x.Value), _smallerBoldFont);
                            table.AddCell(cellNoBorderLeft);
                        }
                    }
                    cellAlignRight.Phrase = new Phrase("", _smallerBoldFont);
                    cellAlignRight.Colspan = 17;
                    table.AddCell(cellAlignRight);
                    cellAlignRight.Colspan = 1;

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", total), _smallerBoldFont);
                    table.AddCell(cellAlignRight);

                if (totalUnit.Count() > 0)
                    foreach (var v in totalUnit)
                    {
                        cellAlignRight.Phrase = new Phrase(categoryName, _smallerBoldFont);
                        cellAlignRight.Colspan = 14;
                        table.AddCell(cellAlignRight);
                        cellAlignRight.Colspan = 1;

                        cell.Colspan = 1;
                        cell.Phrase = new Phrase(v.Key, _smallerBoldFont);
                        table.AddCell(cell);

                        cellNoBorderRight.Phrase = new Phrase("", _smallerBoldFont);
                        table.AddCell(cellNoBorderRight);

                        cellNoBorderLeft.Phrase = new Phrase("", _smallerBoldFont);
                        table.AddCell(cellNoBorderLeft);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value), _smallerBoldFont);
                        table.AddCell(cellAlignRight);

                        if (summaryUnit.ContainsKey(v.Key))
                            summaryUnit[v.Key] += v.Value;
                        else
                            summaryUnit.Add(v.Key, v.Value);
                    }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            var summaryTable = new PdfPTable(5)
            {
                WidthPercentage = 95,

            };

            var widthSummaryTable = new List<float>() { 2f, 1f, 2f, 1f, 3f };
            summaryTable.SetWidths(widthSummaryTable.ToArray());

            summaryTable.AddCell(GetCategorySummaryTable(viewModel.CategorySummaries, viewModel.CategorySummaryTotal));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetUnitSummaryTable(summaryUnit));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetCurrencySummaryTable(viewModel.CurrencySummaries));

            document.Add(summaryTable);
        }

        private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummaries, decimal categorySummaryTotal)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 2f, 3f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            cell.Phrase = new Phrase("Kategori", _smallerBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerBoldFont);
            table.AddCell(cell);

            foreach (var categorySummary in categorySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Phrase = new Phrase(categorySummary.Category, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", categorySummary.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase("", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(string.Format("{0:n}", categorySummaryTotal), _smallerFont);
            table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetUnitSummaryTable(Dictionary<string, decimal> unitSummaries)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 2f, 3f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Unit", _smallerBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerBoldFont);
            table.AddCell(cell);

            decimal totalSummary = 0;
            foreach (var unitSummary in unitSummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Phrase = new Phrase(unitSummary.Key, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", unitSummary.Value), _smallerFont);
                table.AddCell(cell);

                totalSummary += unitSummary.Value;
            }

            cell.Phrase = new Phrase("", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(string.Format("{0:n}", totalSummary), _smallerFont);
            table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetCurrencySummaryTable(List<Summary> currencySummaries)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f, 3f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Mata Uang", _smallerBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total", _smallerBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerBoldFont);
            table.AddCell(cell);

            foreach (var currency in currencySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(currency.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotalCurrency), _smallerFont);
                table.AddCell(cell);
            }

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static void SetCategoryCurrencySummaryTable(Document document, List<Summary> categorySummaries, decimal categorySummaryTotal, List<Summary> currencySummaries)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 95,

            };

            var widths = new List<float>() { 6f, 1f, 3f };
            table.SetWidths(widths.ToArray());

            table.AddCell(GetCategorySummaryTable(categorySummaries, categorySummaryTotal));
            table.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            table.AddCell(GetCurrencySummaryTable(currencySummaries));

            document.Add(table);
        }

        private static void SetFooter(Document document)
        {

        }

    }
}
