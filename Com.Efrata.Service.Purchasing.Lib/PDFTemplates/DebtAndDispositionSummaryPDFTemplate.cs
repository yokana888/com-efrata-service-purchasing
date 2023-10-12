using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public static class DebtAndDispositionSummaryPDFTemplate
    {
        private static readonly BaseColor _headerBackgroundColor = new BaseColor(System.Drawing.Color.DarkBlue);
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 16);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _smallerHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, new BaseColor(System.Drawing.Color.White));
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

        public static MemoryStream Generate(List<DebtAndDispositionSummaryDto> data, int timezoneOffset, DateTimeOffset dueDate, int accountingUnitId, bool isImport, bool isForeignCurrency, int divisionId)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 5, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            var unitName = "SEMUA UNIT";
            if (accountingUnitId > 0)
            {
                var summary = data.FirstOrDefault();
                if (summary != null)
                {
                    unitName = $"UNIT {summary.AccountingUnitName}";
                }
            }

            if (divisionId > 0)
            {
                var datum = data.FirstOrDefault();
                if (datum != null)
                    unitName = $"DIVISI {datum.DivisionName} {unitName}";
            }

            var title = "";
            if (!isImport && !isForeignCurrency)
            {
                title = "LAPORAN REKAP DATA HUTANG & DISPOSISI LOKAL";
            }


            if (!isImport && isForeignCurrency)
                title = "LAPORAN REKAP DATA HUTANG & DISPOSISI LOKAL VALAS";

            if (isImport)
                title = "LAPORAN REKAP DATA HUTANG & DISPOSISI IMPORT";

            SetHeader(document, title, unitName, dueDate.AddHours(timezoneOffset));

            var categoryData = data
                .GroupBy(element => new { element.CategoryCode, element.CurrencyCode })
                .Select(element => new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.Key.CategoryCode,
                    CategoryName = element.FirstOrDefault().CategoryName,
                    CurrencyCode = element.Key.CurrencyCode,
                    DebtTotal = element.Sum(sum => sum.DebtTotal),
                    DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                    Total = element.Sum(sum => sum.DebtTotal) + element.Sum(sum => sum.DispositionTotal)
                })
                .ToList();

            SetCategoryTable(document, categoryData);

            document.Add(new Paragraph(" "));

            if (!isImport && !isForeignCurrency)
            {
                SetUnitTable(document, data);
            }
            else
            {
                SetUnitCurrencyTable(document, data);
                document.Add(new Paragraph(" "));
                SetSeparatedUnitCurrencyTable(document, data);
            }

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        public static MemoryStream GenerateDisposition(List<DebtAndDispositionSummaryDto> data, int timezoneOffset, DateTimeOffset dueDate, int unitId, bool isImport, bool isForeignCurrency)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 5, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            var unitName = "SEMUA UNIT";
            if (unitId > 0)
            {
                var summary = data.FirstOrDefault();
                if (summary != null)
                {
                    unitName = $"UNIT {summary.UnitName}";
                }
            }

            var title = "";
            if (!isImport && !isForeignCurrency)
            {
                title = "LAPORAN REKAP DATA DISPOSISI LOKAL";
            }


            if (!isImport && isForeignCurrency)
                title = "LAPORAN REKAP DATA DISPOSISI LOKAL VALAS";

            if (isImport)
                title = "LAPORAN REKAP DATA DISPOSISI IMPORT";

            SetHeader(document, title, unitName, dueDate.AddHours(timezoneOffset));

            var categoryData = data
                .GroupBy(element => new { element.CategoryCode, element.CurrencyCode })
                .Select(element => new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.Key.CategoryCode,
                    CategoryName = element.FirstOrDefault().CategoryName,
                    CurrencyCode = element.Key.CurrencyCode,
                    DispositionTotal = element.Sum(sum => sum.DispositionTotal)
                })
                .ToList();

            SetCategoryDispositionTable(document, categoryData);

            document.Add(new Paragraph(" "));

            if (!isImport && !isForeignCurrency)
            {
                SetUnitDispositionTable(document, data);
            }
            else
            {
                SetUnitCurrencyDispositionTable(document, data);
                document.Add(new Paragraph(" "));
                SetSeparatedUnitCurrencyDispositionTable(document, data);
            }

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetSeparatedUnitCurrencyTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var debtData = data.Where(element => element.DispositionTotal == 0);
            var dispositionData = data.Where(element => element.DebtTotal == 0);

            var table = new PdfPTable(4)
            {
                WidthPercentage = 50,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f, 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("TOTAL", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var accountingUnit in accountingUnits)
            {
                cellAlignLeft.Phrase = new Phrase(accountingUnit, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                var currencyDebtData = debtData
                    .Where(element => element.AccountingUnitName == accountingUnit)
                    .GroupBy(element => element.CurrencyCode)
                    .Select(element => new DebtAndDispositionSummaryDto()
                    {
                        CurrencyCode = element.Key,
                        DebtTotal = element.Sum(sum => sum.DebtTotal),
                    })
                    .ToList();

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("HUTANG", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                foreach (var currencyDatum in currencyDebtData)
                {
                    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.DebtTotal), _smallerFont);
                    table.AddCell(cellAlignRight);
                }

                var currencyDispositionData = dispositionData
                    .Where(element => element.AccountingUnitName == accountingUnit)
                    .GroupBy(element => element.CurrencyCode)
                    .Select(element => new DebtAndDispositionSummaryDto()
                    {
                        CurrencyCode = element.Key,
                        DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                    })
                    .ToList();

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("DISPOSISI", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                foreach (var currencyDatum in currencyDispositionData)
                {
                    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.DispositionTotal), _smallerFont);
                    table.AddCell(cellAlignRight);
                }
            }

            document.Add(table);
        }

        private static void SetSeparatedUnitCurrencyDispositionTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var dispositionData = data.Where(element => element.DebtTotal == 0);

            var table = new PdfPTable(3)
            {
                WidthPercentage = 50,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("DISPOSISI", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var accountingUnit in accountingUnits)
            {
                cellAlignLeft.Phrase = new Phrase(accountingUnit, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                var currencyDispositionData = data
                    .Where(element => element.AccountingUnitName == accountingUnit)
                    .GroupBy(element => element.CurrencyCode)
                    .Select(element => new DebtAndDispositionSummaryDto()
                    {
                        CurrencyCode = element.Key,
                        DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                    })
                    .ToList();

                foreach (var currencyDatum in currencyDispositionData)
                {
                    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
                    table.AddCell(cellAlignLeft);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.DispositionTotal), _smallerFont);
                    table.AddCell(cellAlignRight);
                }
            }

            document.Add(table);
        }

        private static void SetUnitCurrencyTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var table = new PdfPTable(2)
            {
                WidthPercentage = 30,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            //cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            //table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("TOTAL", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            //foreach (var unit in units)
            //{
            //    cellAlignLeft.Phrase = new Phrase(unit, _smallerFont);
            //    table.AddCell(cellAlignLeft);

            //    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
            //    table.AddCell(cellAlignLeft);

            //    cellAlignLeft.Phrase = new Phrase("", _smallerFont);
            //    table.AddCell(cellAlignLeft);

            //    var currencyData = data
            //        .Where(element => element.UnitName == unit)
            //        .GroupBy(element => element.CurrencyCode)
            //        .Select(element => new DebtAndDispositionSummaryDto()
            //        {
            //            CurrencyCode = element.Key,
            //            DebtTotal = element.Sum(sum => sum.DebtTotal),
            //            DispositionTotal = element.Sum(sum => sum.DispositionTotal),
            //            Total = element.Sum(sum => sum.DebtTotal) + element.Sum(sum => sum.DispositionTotal)
            //        })
            //        .ToList();

            //    foreach (var currencyDatum in currencyData)
            //    {
            //        cellAlignLeft.Phrase = new Phrase("", _smallerFont);
            //        table.AddCell(cellAlignLeft);

            //        cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
            //        table.AddCell(cellAlignLeft);

            //        cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.Total), _smallerFont);
            //        table.AddCell(cellAlignRight);
            //    }
            //}

            var currencyData = data
                    .GroupBy(element => element.CurrencyCode)
                    .Select(element => new DebtAndDispositionSummaryDto()
                    {
                        CurrencyCode = element.Key,
                        DebtTotal = element.Sum(sum => sum.DebtTotal),
                        DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                        Total = element.Sum(sum => sum.DebtTotal) + element.Sum(sum => sum.DispositionTotal)
                    })
                    .ToList();

            foreach (var currencyDatum in currencyData)
            {
                cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.Total), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetUnitCurrencyDispositionTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var table = new PdfPTable(2)
            {
                WidthPercentage = 30,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            //cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            //table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("TOTAL", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            var currencyData = data
                    .GroupBy(element => element.CurrencyCode)
                    .Select(element => new DebtAndDispositionSummaryDto()
                    {
                        CurrencyCode = element.Key,
                        DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                        Total = element.Sum(sum => sum.DebtTotal) + element.Sum(sum => sum.DispositionTotal)
                    })
                    .ToList();

            foreach (var currencyDatum in currencyData)
            {
                cellAlignLeft.Phrase = new Phrase(currencyDatum.CurrencyCode, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", currencyDatum.DispositionTotal), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetUnitTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var table = new PdfPTable(3)
            {
                WidthPercentage = 30,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("Total (IDR)", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var accountingUnit in accountingUnits)
            {
                cellAlignLeft.Phrase = new Phrase(accountingUnit, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignLeft.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignLeft);

                var debtTotal = data.Where(element => element.AccountingUnitName == accountingUnit).Sum(sum => sum.DebtTotal);

                cellAlignRight.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase("Hutang Lokal", _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", debtTotal), _smallerFont);
                table.AddCell(cellAlignRight);

                var dispositionTotal = data.Where(element => element.AccountingUnitName == accountingUnit).Sum(sum => sum.DispositionTotal);

                cellAlignRight.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase("Disposisi", _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", dispositionTotal), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetUnitDispositionTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var accountingUnits = data.Select(element => element.AccountingUnitName).Distinct().ToList();

            var table = new PdfPTable(2)
            {
                WidthPercentage = 30,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 1f, 1f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("UNIT", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("Total", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var accountingUnit in accountingUnits)
            {
                cellAlignLeft.Phrase = new Phrase(accountingUnit, _smallerFont);
                table.AddCell(cellAlignLeft);

                var dispositionTotal = data.Where(element => element.AccountingUnitName == accountingUnit).Sum(sum => sum.DispositionTotal);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", dispositionTotal), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetCategoryTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var table = new PdfPTable(5)
            {
                WidthPercentage = 95,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 2f, 1f, 2f, 2f, 2f };
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor,
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("KATEGORI", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("HUTANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("DISPOSISI", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("JUMLAH", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var datum in data)
            {
                cellAlignLeft.Phrase = new Phrase(datum.CategoryName, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignCenter.Phrase = new Phrase(datum.CurrencyCode, _smallerFont);
                table.AddCell(cellAlignCenter);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", datum.DebtTotal), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", datum.DispositionTotal), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", datum.Total), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetCategoryDispositionTable(Document document, List<DebtAndDispositionSummaryDto> data)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 95,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var widths = new List<float>() { 2f, 1f, 2f};
            table.SetWidths(widths.ToArray());

            var cellAlignCenterHeader = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = _headerBackgroundColor,
            };

            var cellAlignCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellAlignCenterHeader.Phrase = new Phrase("KATEGORI", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("MATA UANG", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            cellAlignCenterHeader.Phrase = new Phrase("DISPOSISI", _smallerHeaderFont);
            table.AddCell(cellAlignCenterHeader);

            foreach (var datum in data)
            {
                cellAlignLeft.Phrase = new Phrase(datum.CategoryName, _smallerFont);
                table.AddCell(cellAlignLeft);

                cellAlignCenter.Phrase = new Phrase(datum.CurrencyCode, _smallerFont);
                table.AddCell(cellAlignCenter);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", datum.DispositionTotal), _smallerFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        private static void SetHeader(Document document, string title, string unitName, DateTimeOffset dueDate)
        {
            var dueDateString = $"{dueDate:dd/MM/yy}";
            if (dueDate == DateTimeOffset.MaxValue)
                dueDateString = "-";
            var table = new PdfPTable(1)
            {
                WidthPercentage = 95,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Phrase = new Phrase("PT EFRATA GARMINDO UTAMA", _headerFont),
                PaddingLeft = 0
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase(title, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(unitName, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"JATUH TEMPO S.D. {dueDateString}", _subHeaderFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
