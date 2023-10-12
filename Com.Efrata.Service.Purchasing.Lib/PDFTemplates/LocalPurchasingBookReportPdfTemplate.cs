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
    public static class LocalPurchasingBookReportPdfTemplate
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 16);
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

            SetReportTable(document, viewModel, timezoneOffset);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        //private static void SetCategoryCurrencySummaryTable(Document document, List<Summary> categorySummaries, decimal categorySummaryTotal, List<Summary> currencySummaries)
        //{
        //    var table = new PdfPTable(3)
        //    {
        //        WidthPercentage = 95,

        //    };

        //    var widths = new List<float>() { 6f, 1f, 3f };
        //    table.SetWidths(widths.ToArray());

        //    table.AddCell(GetCategorySummaryTable(categorySummaries, categorySummaryTotal));
        //    table.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
        //    table.AddCell(GetCurrencySummaryTable(currencySummaries));

        //    document.Add(table);
        //}

        private static PdfPCell GetCurrencySummaryTable(List<Summary> currencySummaries)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Mata Uang", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total", _smallerFont);
            table.AddCell(cell);

            foreach (var currency in currencySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(currency.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummaries, decimal categorySummaryTotal)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Kategori", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
            table.AddCell(cell);

            foreach (var categorySummary in categorySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
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
                WidthPercentage = 95
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Unit", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
            table.AddCell(cell);

            decimal totalSummary = 0;
            foreach (var unitSummary in unitSummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
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

        private static void SetReportTable(Document document, LocalPurchasingBookReportViewModel viewModel, int timezoneOffset)
        {
            var table = new PdfPTable(14)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>();
            for (var i = 0; i < 14; i++)
            {
                if (i == 1)
                {
                    widths.Add(1f);
                    continue;
                }

                if (i == 2 || i == 3)
                {
                    widths.Add(3f);
                    continue;
                }

                widths.Add(2f);
            }
            table.SetWidths(widths.ToArray());

            SetReportTableHeader(table);

            var listCategoryReports = viewModel.Reports.Where(x => x.AccountingUnitName != null).OrderBy(order => order.AccountingLayoutIndex).GroupBy(x => x.AccountingCategoryName).ToList();

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellColspan2NoBorderLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 2,
                BorderWidthLeft = 0
            };

            var cellColspan8NoBorderRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Colspan = 8,
                BorderWidthRight = 0
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRightColspan9 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER,
                Colspan = 9
            };

            var categoryCell = new PdfPCell()
            {
                BorderWidthTop = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var totalUnitCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var summaryUnit = new Dictionary<string, decimal>();

            foreach (var cat in listCategoryReports)
            {
                var categoryName = cat.Select(x => x.AccountingCategoryName).FirstOrDefault();
                categoryCell.Phrase = new Phrase(categoryName, _smallBoldFont);
                categoryCell.Colspan = 15;
                table.AddCell(categoryCell);

                decimal total = 0;
                decimal totalDPP = 0;
                decimal totalPPN = 0;
                decimal totalPPH = 0;

                var totalUnit = new Dictionary<string, Dictionary<string, decimal>>();

                foreach (var data in cat)
                {
                    cell.Phrase = new Phrase(data.ReceiptDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("yyyy-dd-MM"), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.SupplierCode, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.SupplierName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.Remark, _smallerFont);
                    table.AddCell(cell);

                    //cell.Phrase = new Phrase(data.IPONo, _smallerFont);
                    //table.AddCell(cell);

                    //cell.Phrase = new Phrase(data.DONo, _smallerFont);
                    //table.AddCell(cell);

                    cell.Phrase = new Phrase(data.URNNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.InvoiceNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.VATNo, _smallerFont);
                    table.AddCell(cell);

                    if (data.DataSourceSort == 1)
                    {
                        cell.Phrase = new Phrase(data.UPONo, _smallerFont);
                    }else
                    {
                        cell.Phrase = new Phrase(data.CorrectionNo, _smallerFont);

                    }
                    table.AddCell(cell);


                    cell.Phrase = new Phrase(data.AccountingCategoryName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.AccountingUnitName, _smallerFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.DPP), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.VAT), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.IncomeTax), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.Total), _smallerBoldFont);
                    table.AddCell(cellAlignRight);

                    if (totalUnit.ContainsKey(data.AccountingUnitName))
                    {
                        totalUnit[data.AccountingUnitName]["DPP"] += data.DPP;
                        totalUnit[data.AccountingUnitName]["VAT"] += data.VAT * data.CurrencyRate;
                        totalUnit[data.AccountingUnitName]["TAX"] += data.IncomeTax * data.CurrencyRate;
                        totalUnit[data.AccountingUnitName]["TOTAL"] += data.Total;
                    }
                    else
                    {
                        totalUnit.Add(data.AccountingUnitName, new Dictionary<string, decimal>() {
                            { "DPP", data.DPP },
                            { "VAT", data.VAT * data.CurrencyRate},
                            { "TAX", data.IncomeTax * data.CurrencyRate},
                            { "TOTAL", data.Total }
                        });
                    }

                    if (summaryUnit.ContainsKey(data.AccountingUnitName))
                        summaryUnit[data.AccountingUnitName] += data.Total;
                    else
                        summaryUnit.Add(data.AccountingUnitName, data.Total);

                    totalDPP += data.DPP;
                    totalPPN += data.VAT;
                    totalPPH += data.IncomeTax;
                    total += data.Total;
                }

                //var cellGrandTotal = new PdfPCell()
                //{
                //    HorizontalAlignment = Element.ALIGN_RIGHT,
                //    VerticalAlignment = Element.ALIGN_CENTER,
                //    Colspan = 16
                //};

                //cellGrandTotal.Phrase = new Phrase("Grand Total", _smallerBoldFont);
                //table.AddCell(cellGrandTotal);

                //cellGrandTotal.Phrase = new Phrase(string.Format("{0:n}", grandTotal), _smallerBoldFont);
                //table.AddCell(cellGrandTotal);

                cellColspan8NoBorderRight.Phrase = new Phrase();
                table.AddCell(cellColspan8NoBorderRight);

                cellColspan2NoBorderLeft.Phrase = new Phrase($"TOTAL {categoryName}", _smallBoldFont);
                table.AddCell(cellColspan2NoBorderLeft);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalDPP), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalPPN), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalPPH), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", total), _smallerBoldFont);
                table.AddCell(cellAlignRight);

                if (totalUnit.Count() > 0)
                    foreach (var v in totalUnit)
                    {
                        cellAlignRightColspan9.Phrase = new Phrase($"{categoryName}", _smallBoldFont);
                        table.AddCell(cellAlignRightColspan9);

                        cell.Phrase = new Phrase($"{v.Key}  ", _smallerFont);
                        table.AddCell(cell);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["DPP"]), _smallerFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["VAT"]), _smallerFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["TAX"]), _smallerFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["TOTAL"]), _smallerBoldFont);
                        table.AddCell(cellAlignRight);

                        //totalUnitCell.Phrase = new Phrase(string.Format("{0:n}", v.Value), _smallFont);
                        //totalUnitCell.Colspan = 4;
                        //table.AddCell(totalUnitCell);
                    }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            var summaryTable = new PdfPTable(5)
            {
                WidthPercentage = 95,

            };

            var widthSummaryTable = new List<float>() { 3f, 1f, 3f, 1f, 2f };
            summaryTable.SetWidths(widthSummaryTable.ToArray());

            summaryTable.AddCell(GetCategorySummaryTable(viewModel.CategorySummaries, viewModel.CategorySummaryTotal));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetUnitSummaryTable(summaryUnit));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetCurrencySummaryTable(viewModel.CurrencySummaries));

            document.Add(summaryTable);
        }

        private static void SetReportTableHeader(PdfPTable table)
        {
            var cell = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellColspan3 = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Colspan = 3
            };

            var cellRowspan2 = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Rowspan = 2
            };

            cellRowspan2.Phrase = new Phrase("Tanggal", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kode Supplier", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Supplier", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Keterangan", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            //cellRowspan2.Phrase = new Phrase("No. PO", _smallerFont);
            //table.AddCell(cellRowspan2);

            //cellRowspan2.Phrase = new Phrase("No. Surat Jalan", _smallerFont);
            //table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Bon Penerimaan", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Inv.", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Faktur Pajak", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. SPB/NI", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kategori", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Unit", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellColspan3.Phrase = new Phrase("Pembelian", _smallerBoldWhiteFont);
            table.AddCell(cellColspan3);

            cellRowspan2.Phrase = new Phrase("Total", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cell.Phrase = new Phrase("DPP", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPN", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPH", _smallerBoldWhiteFont);
            table.AddCell(cell);
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
                Phrase = new Phrase("PT EFRATA GARMINDO UTAMA", _headerFont),
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase("BUKU PEMBELIAN LOKAL", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"Dari {dateFrom.AddHours(timezoneOffset):yyyy-dd-MM} Sampai {dateTo.AddHours(timezoneOffset):yyyy-dd-MM}", _subHeaderFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
