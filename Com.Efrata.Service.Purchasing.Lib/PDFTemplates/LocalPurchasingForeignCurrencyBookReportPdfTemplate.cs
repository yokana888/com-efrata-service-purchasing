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
    public static class LocalPurchasingForeignCurrencyBookReportPdfTemplate
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

            //document.Add(new Paragraph("\n"));

            //SetCategoryCurrencySummaryTable(document, viewModel.CategorySummaries, viewModel.CategorySummaryTotal, viewModel.CurrencySummaries);

            //SetCategoryTable(document, viewModel.CategorySummaries, viewModel.CategorySummaryTotal);

            //SetCurrencyTable(document, viewModel.CurrencySummaries);

            //SetFooter(document);

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
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 1f, 1f };
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

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
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

        private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummaries, decimal categorySummaryTotal)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
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
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var emptyCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
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

        //private static void SetFooter(Document document)
        //{

        //}

        //private static void SetCurrencyTable(Document document, List<Summary> currencySummaries)
        //{

        //}

        //private static void SetCategoryTable(Document document, List<Summary> categorySummaries, decimal categorySummaryTotal)
        //{

        //}

        private static void SetReportTable(Document document, LocalPurchasingBookReportViewModel viewModel, int timezoneOffset)

        {
            var table = new PdfPTable(16)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>();
            for (var i = 0; i < 16; i++)
            {
                if (i == 1 || i == 10)
                {
                    widths.Add(1f);
                    continue;
                }

                if (i == 3)
                {
                    widths.Add(3f);
                    continue;
                }

                widths.Add(2f);
            }
            table.SetWidths(widths.ToArray());

            SetReportTableHeader(table);

            var grouppedByAccountingCategoriNames = viewModel.Reports.Where(x => x.AccountingUnitName != null).OrderBy(order => order.AccountingLayoutIndex).GroupBy(x => x.AccountingCategoryName).ToList();

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellColspan2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 2
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellAlignRightColspan3 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 3
            };

            var categoryCell = new PdfPCell()
            {
                BorderWidthTop = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var totalCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderWidthBottom = 0
            };

            var totalCellNoBorderTopAndBot = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderWidthBottom = 0,
                BorderWidthTop = 0
            };

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

            var totalDPPCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var totalCategoryUnitCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var totalUnitCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var summaryUnit = new Dictionary<string, decimal>();

            foreach (var grouppedAccountingCategory in grouppedByAccountingCategoriNames)
            {
                var accountingCategoryName = grouppedAccountingCategory.Select(x => x.AccountingCategoryName).FirstOrDefault();
                categoryCell.Phrase = new Phrase(accountingCategoryName, _smallBoldFont);
                categoryCell.Colspan = 18;
                table.AddCell(categoryCell);

                var totalUnit = new Dictionary<string, Dictionary<string, decimal>>();
                var totalCurrencies = new Dictionary<string, Dictionary<string, decimal>>();
                decimal totalIdrDpp = 0;
                decimal totalIdr = 0;
                decimal totalIdrVat = 0;
                decimal totalIdrTax = 0;

                foreach (var data in grouppedAccountingCategory)
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

                    cell.Phrase = new Phrase(data.URNNo, _smallerFont);
                    table.AddCell(cell);

                    //cell.Phrase = new Phrase(data.DONo, _smallerFont);
                    //table.AddCell(cell);

                    cell.Phrase = new Phrase(data.InvoiceNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.VATNo, _smallerFont);
                    table.AddCell(cell);

                    //cell.Phrase = new Phrase(data.UPONo, _smallerFont);
                    if(data.DataSourceSort == 1)
                    {
                        cell.Phrase = new Phrase(data.UPONo, _smallerFont);
                    }
                    else
                    {
                        cell.Phrase = new Phrase(data.CorrectionNo, _smallerFont);

                    }
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.CategoryCode + " - " + data.CategoryName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.AccountingUnitName, _smallerFont);
                    table.AddCell(cell);

                    cellNoBorderRight.Phrase = new Phrase(data.CurrencyCode, _smallerFont);
                    table.AddCell(cellNoBorderRight);

                    cellNoBorderLeft.Phrase = new Phrase(string.Format("{0:n}", data.DPP), _smallerFont);
                    table.AddCell(cellNoBorderLeft);

                    //dppCell.Phrase = new Phrase(data.CurrencyCode + " " + string.Format("{0:n}", data.DPP), _smallerFont);
                    //table.AddCell(dppCell);

                    //cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyRate), _smallerFont);
                    //table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.DPPCurrency), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.VAT * data.CurrencyRate), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.IncomeTax * data.CurrencyRate), _smallerFont);
                    table.AddCell(cellAlignRight);

                    //var subTotalIdr = data.IncomeTaxBy == "Supplier" ? data.Total + (data.IncomeTax * data.CurrencyRate) : data.Total;
                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.TotalCurrency), _smallerFont);
                    table.AddCell(cellAlignRight);

                    // Units summary
                    if (totalUnit.ContainsKey(data.AccountingUnitName))
                    {
                        totalUnit[data.AccountingUnitName]["DPP"] += data.DPP;
                        totalUnit[data.AccountingUnitName]["VAT"] += data.VAT * data.CurrencyRate;
                        totalUnit[data.AccountingUnitName]["TAX"] += data.IncomeTax * data.CurrencyRate;
                        totalUnit[data.AccountingUnitName]["DPPCurrency"] += data.DPPCurrency;
                        totalUnit[data.AccountingUnitName]["TOTAL"] += data.TotalCurrency;
                    }
                    else
                    {
                        totalUnit.Add(data.AccountingUnitName, new Dictionary<string, decimal>() {
                            { "DPP", data.DPP },
                            { "VAT", data.VAT * data.CurrencyRate},
                            { "TAX", data.IncomeTax * data.CurrencyRate},
                            { "DPPCurrency", data.DPPCurrency },
                            { "TOTAL", data.TotalCurrency }
                        });
                    }

                    if(summaryUnit.ContainsKey(data.AccountingUnitName))
                        summaryUnit[data.AccountingUnitName] += data.TotalCurrency;
                    else
                        summaryUnit.Add(data.AccountingUnitName, data.TotalCurrency);


                    //var dpp = data.IncomeTaxBy == "Supplier" ? data.DPP + data.IncomeTax : data.DPP;
                    //var dppCurrency = data.IncomeTaxBy == "Supplier" ? data.DPPCurrency + (data.IncomeTax * data.CurrencyRate) : data.DPPCurrency;
                    // Currencies summary
                    if (totalCurrencies.ContainsKey(data.CurrencyCode))
                    {
                        totalCurrencies[data.CurrencyCode]["DPP"] += data.DPP;
                        totalCurrencies[data.CurrencyCode]["VAT"] += data.VAT * data.CurrencyRate;
                        totalCurrencies[data.CurrencyCode]["TAX"] += data.IncomeTax * data.CurrencyRate;
                        totalCurrencies[data.CurrencyCode]["DPPCurrency"] += data.DPPCurrency;
                        totalCurrencies[data.CurrencyCode]["TOTAL"] += data.TotalCurrency;
                    }
                    else
                    {
                        totalCurrencies.Add(data.CurrencyCode, new Dictionary<string, decimal>() {
                            { "DPP", data.DPP },
                            { "VAT", data.VAT * data.CurrencyRate},
                            { "TAX", data.IncomeTax * data.CurrencyRate},
                            { "DPPCurrency", data.DPPCurrency },
                            { "TOTAL", data.TotalCurrency }
                    } );
                    }

                    totalIdr += data.TotalCurrency;
                    totalIdrDpp += data.DPPCurrency;
                    totalIdrTax += data.IncomeTax * data.CurrencyRate;
                    totalIdrVat += data.VAT * data.CurrencyRate;
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
                totalCell.Phrase = new Phrase($"TOTAL {accountingCategoryName}", _smallBoldFont);
                totalCell.Colspan = 10;
                table.AddCell(totalCell);

                foreach (var totalCurrency in totalCurrencies)
                {
                    cellNoBorderRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Key), _smallerFont);
                    table.AddCell(cellNoBorderRight);

                    cellNoBorderLeft.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value["DPP"]), _smallerFont);
                    table.AddCell(cellNoBorderLeft);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value["DPPCurrency"]), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value["VAT"]), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value["TAX"]), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value["TOTAL"]), _smallerBoldFont);
                    table.AddCell(cellAlignRight);

                    totalCellNoBorderTopAndBot.Phrase = new Phrase();
                    totalCellNoBorderTopAndBot.Colspan = 10;
                    table.AddCell(totalCellNoBorderTopAndBot);
                }

                cellAlignRightColspan3.Phrase = new Phrase(string.Format("{0:n}", totalIdrDpp), _smallerFont);
                table.AddCell(cellAlignRightColspan3);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalIdrVat), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalIdrTax), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalIdr), _smallerBoldFont);
                table.AddCell(cellAlignRight);

                if (totalUnit.Count() > 0)
                    foreach (var v in totalUnit)
                    {
                        totalCategoryUnitCell.Phrase = new Phrase($"{accountingCategoryName}", _smallBoldFont);
                        totalCategoryUnitCell.Colspan = 8;
                        table.AddCell(totalCategoryUnitCell);

                        cellColspan2.Phrase = new Phrase($"{v.Key}  ", _smallBoldFont);
                        table.AddCell(cellColspan2);

                        cellAlignRightColspan3.Phrase = new Phrase(string.Format("{0:n}", v.Value["DPPCurrency"]), _smallerFont);
                        table.AddCell(cellAlignRightColspan3);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["VAT"]), _smallerFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["TAX"]), _smallerFont);
                        table.AddCell(cellAlignRight);

                        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", v.Value["TOTAL"]), _smallerBoldFont);
                        table.AddCell(cellAlignRight);

                        //totalUnitCell.Phrase = new Phrase(string.Format("{0:n}", v.Value), _smallerFont);
                        //totalUnitCell.Colspan = 6;
                        //table.AddCell(totalUnitCell);
                    }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            var summaryTable = new PdfPTable(5)
            {
                WidthPercentage = 95,

            };

            var widthSummaryTable = new List<float>() { 2f, 1f, 2f, 1f, 4f };
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
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellColspan5 = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 5
            };

            var cellColspan2 = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 2
            };

            var cellRowspan2 = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
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

            cellRowspan2.Phrase = new Phrase("No. Bon Penerimaan", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            //cellRowspan2.Phrase = new Phrase("No. Surat Jalan", _smallerFont);
            //table.AddCell(cellRowspan2);

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

            cellColspan5.Phrase = new Phrase("Pembelian", _smallerBoldWhiteFont);
            table.AddCell(cellColspan5);

            cellRowspan2.Phrase = new Phrase("Total (IDR)", _smallerBoldWhiteFont);
            table.AddCell(cellRowspan2);

            cellColspan2.Phrase = new Phrase("DPP Valas", _smallerBoldWhiteFont);
            table.AddCell(cellColspan2);

            //cell.Phrase = new Phrase("Kurs BP", _smallerFont);
            //table.AddCell(cell);

            cell.Phrase = new Phrase("DPP (IDR)", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPN (IDR)", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPH (IDR)", _smallerBoldWhiteFont);
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
                Phrase = new Phrase("PT EFRATA GARMINDO UTAMA", _headerFont)
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase("BUKU PEMBELIAN LOKAL VALAS", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"Dari {dateFrom.AddHours(timezoneOffset):yyyy-dd-MM} Sampai {dateTo.AddHours(timezoneOffset):yyyy-dd-MM}", _subHeaderFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
