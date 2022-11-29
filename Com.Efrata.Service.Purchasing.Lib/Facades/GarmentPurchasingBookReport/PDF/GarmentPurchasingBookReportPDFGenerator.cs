using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport.PDF
{
    public static class GarmentPurchasingBookReportPDFGenerator
    {

        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 5);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, BaseColor.White);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 5);
        private static readonly Font _smallerBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 5, 0, BaseColor.White);
        private static readonly List<string> _accountingCategories = new List<string>() { "BB", "BP", "BE", "PRC" };

        public static MemoryStream Generate(ReportDto report, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier, int timezoneOffset)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            var stream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            SetTitle(document, startDate, endDate, isForeignCurrency, isImportSupplier, timezoneOffset);
            SetTable(document, report, isForeignCurrency, isImportSupplier, timezoneOffset);
            SetCategory(document, report.Categories, isForeignCurrency, isImportSupplier);
            SetCurrency(document, report.Currencies, isForeignCurrency, isImportSupplier);
            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetCurrency(Document document, List<ReportCurrencyDto> currencies, bool isForeignCurrency, bool isImportSupplier)
        {
            if (isForeignCurrency || isImportSupplier)
            {
                var table = new PdfPTable(3)
                {
                    WidthPercentage = 50,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                var cellCenter = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var cellLeft = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var cellRight = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                cellCenter.Phrase = new Phrase("Mata Uang", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
                table.AddCell(cellCenter);

                foreach (var currency in currencies)
                {
                    cellLeft.Phrase = new Phrase(currency.CurrencyCode, _normalFont);
                    table.AddCell(cellLeft);
                    cellRight.Phrase = new Phrase(currency.CurrencyAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(currency.Amount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                document.Add(table);

                document.Add(new Paragraph("\n"));
            }
            else
            {
                var table = new PdfPTable(2)
                {
                    WidthPercentage = 50,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                var cellCenter = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var cellLeft = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                var cellRight = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_CENTER
                };

                cellCenter.Phrase = new Phrase("Mata Uang", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
                table.AddCell(cellCenter);

                foreach (var currency in currencies)
                {
                    cellLeft.Phrase = new Phrase(currency.CurrencyCode, _normalFont);
                    table.AddCell(cellLeft);
                    cellRight.Phrase = new Phrase(currency.Amount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                document.Add(table);

                document.Add(new Paragraph("\n"));
            }
        }

        private static void SetCategory(Document document, List<ReportCategoryDto> categories, bool isForeignCurrency, bool isImportSupplier)
        {
            var cellCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            if (isForeignCurrency || isImportSupplier)
            {
                var table = new PdfPTable(4)
                {
                    WidthPercentage = 30,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                };

                cellCenter.Phrase = new Phrase("Kategori", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Mata Uang", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total Valas", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
                table.AddCell(cellCenter);

                foreach (var category in categories)
                {
                    cellLeft.Phrase = new Phrase(category.CategoryName, _normalFont);
                    table.AddCell(cellLeft);
                    cellLeft.Phrase = new Phrase(category.CurrencyCode, _normalFont);
                    table.AddCell(cellLeft);
                    cellRight.Phrase = new Phrase(category.CurrencyAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(category.Amount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                document.Add(table);
            }
            else
            {
                var table = new PdfPTable(2)
                {
                    WidthPercentage = 30,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                cellCenter.Phrase = new Phrase("Kategori", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
                table.AddCell(cellCenter);

                foreach (var category in categories)
                {
                    cellLeft.Phrase = new Phrase(category.CategoryName, _normalFont);
                    table.AddCell(cellLeft);
                    cellRight.Phrase = new Phrase(category.Amount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                document.Add(table);
            }

            document.Add(new Paragraph("\n"));
        }

        private static void SetTable(Document document, ReportDto report, bool isForeignCurrency, bool isImportSupplier, int timezoneOffset)
        {
            if (!isForeignCurrency && !isImportSupplier)
            {
                SetTableLocal(document, report, timezoneOffset);
            }

            if (isForeignCurrency)
            {
                SetTableLocalForeignCurrency(document, report, timezoneOffset);
            }

            if (isImportSupplier)
            {
                SetTableImport(document, report, timezoneOffset);
            }

        }

        private static void SetTableImport(Document document, ReportDto report, int timezoneOffset)
        {
            var table = new PdfPTable(18)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            var cellCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellCenterWithBackground = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BaseColor.LightGray
            };

            var cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Phrase = new Phrase("Tanggal Bon", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Supplier", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Nama Barang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Surat Jalan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BP Besar", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BP Kecil", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Invoice", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Faktur Pajak", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. NI", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kategori Pembukuan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 4;
            cellCenterWithBackground.Phrase = new Phrase("Bea Cukai", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Colspan = 3;
            cellCenterWithBackground.Phrase = new Phrase("Pembelian", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("Tanggal BC", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BC", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Jenis BC", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Ket Nilai Impor", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Mata Uang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("DPP Valas", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Rate", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);

            foreach (var accountingCategory in _accountingCategories)
            {
                var items = report.Data.Where(element => element.AccountingCategoryName == accountingCategory).ToList();
                var currencyCodes = items.Select(element => element.CurrencyCode).Distinct().ToList();

                if (items.Count > 0)
                {
                    cellLeft.Colspan = 18;
                    cellLeft.Phrase = new Phrase(GetAccountingCategoryFullString(accountingCategory), _normalBoldFont);
                    table.AddCell(cellLeft);
                    cellLeft.Colspan = 1;
                }

                foreach (var item in items)
                {
                    cellCenter.Rowspan = 1;
                    cellCenter.Colspan = 1;
                    cellCenter.Phrase = new Phrase(item.CustomsArrivalDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"), _normalFont);
                    table.AddCell(cellCenter);
                    cellLeft.Phrase = new Phrase($"{item.SupplierCode} - {item.SupplierName}", _normalFont);
                    table.AddCell(cellLeft);
                    cellLeft.Phrase = new Phrase(item.ProductName, _normalFont);
                    table.AddCell(cellLeft);
                    cellCenter.Phrase = new Phrase(item.GarmentDeliveryOrderNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.BillNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.PaymentBill, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InvoiceNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.VATNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InternalNoteNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.AccountingCategoryName, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.CustomsDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"), _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.CustomsNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.CustomsType, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.ImportValueRemark, _normalFont);
                    table.AddCell(cellCenter);
                    cellRight.Phrase = new Phrase(item.CurrencyCode, _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyDPPAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyRate.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.Total.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                if (items.Count > 0)
                {
                    var currencyTotalRemark = $"RINCIAN TOTAL {GetAccountingCategoryFullString(accountingCategory)}";
                    cellRight.Colspan = 14;
                    cellRight.Rowspan = currencyCodes.Count;
                    cellRight.Phrase = new Phrase(currencyTotalRemark, _normalBoldFont);
                    table.AddCell(cellRight);
                    cellRight.Colspan = 1;
                    cellRight.Rowspan = 1;

                    foreach (var currencyCode in currencyCodes)
                    {
                        var currencyDPPTotal = items.Sum(item => item.CurrencyDPPAmount);
                        var grandTotal = items.Sum(item => item.Total);

                        cellCenter.Colspan = 1;
                        cellCenter.Phrase = new Phrase(currencyCode, _normalBoldFont);
                        table.AddCell(cellCenter);
                        cellCenter.Colspan = 1;

                        cellRight.Phrase = new Phrase(currencyDPPTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase("", _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase(grandTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                    }

                    var totalRemark = $"TOTAL {GetAccountingCategoryFullString(accountingCategory)}";
                    cellRight.Colspan = 17;
                    cellRight.Phrase = new Phrase(totalRemark, _normalBoldFont);
                    table.AddCell(cellRight);

                    var total = items.Sum(item => item.Total);
                    cellRight.Colspan = 1;
                    cellRight.Phrase = new Phrase(total.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);
                }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
        }

        private static void SetTableLocalForeignCurrency(Document document, ReportDto report, int timezoneOffset)
        {
            var table = new PdfPTable(18)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            var cellCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellCenterWithBackground = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BaseColor.LightGray
            };

            var cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Phrase = new Phrase("Tanggal Bon", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Supplier", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Nama Barang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Surat Jalan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BP Besar", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BP Kecil", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Invoice", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Faktur Pajak", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. NI", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kategori Pembukuan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kuantitas", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Mata Uang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kurs", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 4;
            cellCenterWithBackground.Phrase = new Phrase("Pembelian", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("DPP Valas", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("DPP (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("PPN (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("PPh (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);

            foreach (var accountingCategory in _accountingCategories)
            {
                var items = report.Data.Where(element => element.AccountingCategoryName == accountingCategory).ToList();
                var currencyCodes = items.Select(element => element.CurrencyCode).Distinct().ToList();

                if (items.Count > 0)
                {
                    cellLeft.Colspan = 18;
                    cellLeft.Phrase = new Phrase(GetAccountingCategoryFullString(accountingCategory), _normalBoldFont);
                    table.AddCell(cellLeft);
                    cellLeft.Colspan = 1;
                }

                foreach (var item in items)
                {
                    cellCenter.Rowspan = 1;
                    cellCenter.Colspan = 1;
                    cellCenter.Phrase = new Phrase(item.CustomsArrivalDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"), _normalFont);
                    table.AddCell(cellCenter);
                    cellLeft.Phrase = new Phrase($"{item.SupplierCode} - {item.SupplierName}", _normalFont);
                    table.AddCell(cellLeft);
                    cellLeft.Phrase = new Phrase(item.ProductName, _normalFont);
                    table.AddCell(cellLeft);
                    cellCenter.Phrase = new Phrase(item.GarmentDeliveryOrderNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.BillNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.PaymentBill, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InvoiceNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.VATNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InternalNoteNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.AccountingCategoryName, _normalFont);
                    table.AddCell(cellCenter);
                    cellRight.Phrase = new Phrase(item.InternalNoteQuantity.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyCode, _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyRate.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyDPPAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.DPPAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.VATAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.IncomeTaxAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.Total.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                if (items.Count > 0)
                {
                    var currencyTotalRemark = $"RINCIAN TOTAL {GetAccountingCategoryFullString(accountingCategory)}";
                    cellRight.Colspan = 11;
                    cellRight.Rowspan = currencyCodes.Count;
                    cellRight.Phrase = new Phrase(currencyTotalRemark, _normalBoldFont);
                    table.AddCell(cellRight);
                    cellRight.Colspan = 1;
                    cellRight.Rowspan = 1;

                    foreach (var currencyCode in currencyCodes)
                    {
                        var dppTotal = items.Sum(item => item.DPPAmount);
                        var currencyDPPTotal = items.Sum(item => item.CurrencyDPPAmount);
                        var vatTotal = items.Sum(item => item.VATAmount);
                        var incomeTaxTotal = items.Sum(item => item.IncomeTaxAmount);
                        var grandTotal = items.Sum(item => item.Total);

                        cellCenter.Colspan = 2;
                        cellCenter.Phrase = new Phrase(currencyCode, _normalBoldFont);
                        table.AddCell(cellCenter);
                        cellCenter.Colspan = 1;

                        cellRight.Phrase = new Phrase(currencyDPPTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase(dppTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase(vatTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase(incomeTaxTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                        cellRight.Phrase = new Phrase(grandTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                        table.AddCell(cellRight);
                    }

                    var totalRemark = $"TOTAL {GetAccountingCategoryFullString(accountingCategory)}";
                    cellRight.Colspan = 17;
                    cellRight.Phrase = new Phrase(totalRemark, _normalBoldFont);
                    table.AddCell(cellRight);

                    var total = items.Sum(item => item.Total);
                    cellRight.Colspan = 1;
                    cellRight.Phrase = new Phrase(total.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);
                }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
        }

        private static void SetTableLocal(Document document, ReportDto report, int timezoneOffset)
        {
            var table = new PdfPTable(16)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            var cellCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellCenterWithBackground = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BaseColor.LightGray
            };

            var cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Phrase = new Phrase("Tanggal Bon", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Supplier", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Nama Barang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Surat Jalan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            //cellCenterWithBackground.Phrase = new Phrase("No. BP Besar", _subHeaderFont);
            //table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. BP Kecil", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Invoice", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. Faktur Pajak", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("No. NI", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kategori Pembukuan", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Kuantitas", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Mata Uang", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 4;
            cellCenterWithBackground.Phrase = new Phrase("Pembelian", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 2;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("Total (IDR)", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Rowspan = 1;
            cellCenterWithBackground.Colspan = 1;
            cellCenterWithBackground.Phrase = new Phrase("DPP", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("PPN", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("PPh", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);
            cellCenterWithBackground.Phrase = new Phrase("Koreksi", _subHeaderFont);
            table.AddCell(cellCenterWithBackground);

            foreach (var accountingCategory in _accountingCategories)
            {
                var items = report.Data.Where(element => element.AccountingCategoryName == accountingCategory).ToList();

                if (items.Count > 0)
                {
                    cellLeft.Colspan = 16;
                    cellLeft.Phrase = new Phrase(GetAccountingCategoryFullString(accountingCategory), _normalBoldFont);
                    table.AddCell(cellLeft);
                    cellLeft.Colspan = 1;
                }
                //var currencyIds = items.Select(item => item.CurrencyId).Distinct().ToList();
                foreach (var item in items)
                {
                    cellCenter.Rowspan = 1;
                    cellCenter.Colspan = 1;
                    cellCenter.Phrase = new Phrase(item.CustomsArrivalDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"), _normalFont);
                    table.AddCell(cellCenter);
                    cellLeft.Phrase = new Phrase($"{item.SupplierCode} - {item.SupplierName}", _normalFont);
                    table.AddCell(cellLeft);
                    cellLeft.Phrase = new Phrase(item.ProductName, _normalFont);
                    table.AddCell(cellLeft);
                    cellCenter.Phrase = new Phrase(item.GarmentDeliveryOrderNo, _normalFont);
                    table.AddCell(cellCenter);
                    //cellCenter.Phrase = new Phrase(item.BillNo, _normalFont);
                    //table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.PaymentBill, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InvoiceNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.VATNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.InternalNoteNo, _normalFont);
                    table.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase(item.AccountingCategoryName, _normalFont);
                    table.AddCell(cellCenter);
                    cellRight.Phrase = new Phrase(item.InternalNoteQuantity.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.CurrencyCode, _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.DPPAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.VATAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.IncomeTaxAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.PriceCorrection.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                    cellRight.Phrase = new Phrase(item.Total.ToString("0,0.00", CultureInfo.InvariantCulture), _normalFont);
                    table.AddCell(cellRight);
                }

                if (items.Count > 0)
                {
                    var totalRemark = $"TOTAL {GetAccountingCategoryFullString(accountingCategory)}";
                    var dppTotal = items.Sum(item => item.DPPAmount);
                    var vatTotal = items.Sum(item => item.VATAmount);
                    var incomeTaxAmount = items.Sum(item => item.IncomeTaxAmount);
                    var grandTotal = items.Sum(item => item.Total);

                    var corectionTotal = items.Sum(item => item.PriceCorrection);

                    cellRight.Colspan = 11;
                    cellRight.Phrase = new Phrase(totalRemark, _normalBoldFont);
                    table.AddCell(cellRight);
                    cellRight.Colspan = 1;

                    cellRight.Phrase = new Phrase(dppTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);

                    cellRight.Phrase = new Phrase(vatTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);

                    cellRight.Phrase = new Phrase(incomeTaxAmount.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);

                    cellRight.Phrase = new Phrase(corectionTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);

                    cellRight.Phrase = new Phrase(grandTotal.ToString("0,0.00", CultureInfo.InvariantCulture), _normalBoldFont);
                    table.AddCell(cellRight);
                }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
        }

        private static string GetAccountingCategoryFullString(string accountingCategory)
        {
            switch (accountingCategory)
            {
                case "BB":
                    return "Bahan Baku";
                case "BP":
                    return "Bahan Pembantu";
                case "BE":
                    return "Bahan Embalage";
                case "PRC":
                    return "Proses";
                default:
                    return "";
            }    
        }

        private static void SetTitle(Document document, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier, int timezoneOffset)
        {
            var title = "LAPORAN BUKU PEMBELIAN LOKAL";
            if (isForeignCurrency)
                title = "LAPORAN BUKU PEMBELIAN LOKAL VALAS";

            if (isImportSupplier)
                title = "LAPORAN BUKU PEMBELIAN IMPOR";

            var start = startDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy");
            var end = endDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy");

            var table = new PdfPTable(1)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.SetWidths(new float[] { 1f });

            var cellCenter = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellLeft = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenter.Phrase = new Phrase(title, _headerFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase($"Periode {start} sampai {end}", _headerFont);
            table.AddCell(cellCenter);

            document.Add(table);
        }
    }
}
