using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnpaidDispositionReport;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public static class UnpaidDispositionReportDetailPDFTemplate
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

        public static MemoryStream Generate(UnpaidDispositionReportDetailViewModel viewModel, int timezoneOffset, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency, int accountingUnitId, int divisionId)
        {
            var date = (dateTo.HasValue ? dateTo.Value : DateTimeOffset.MaxValue).ToUniversalTime();
            var unitName = "SEMUA UNIT";
            var divisionName = "SEMUA DIVISI";
            var separator = " - ";

            if (accountingUnitId > 0 && divisionId == 0)
            {
                var summary = viewModel.Reports.FirstOrDefault();
                if (summary != null)
                {
                    unitName = $"UNIT {summary.AccountingUnitName}";
                    separator = "";
                    divisionName = "";
                }
                else
                {
                    unitName = "";
                    separator = "";
                    divisionName = "";
                }
            }
            else if (divisionId > 0 && accountingUnitId == 0)
            {
                var summary = viewModel.Reports.FirstOrDefault();
                if (summary != null)
                {
                    divisionName = $"DIVISI {summary.DivisionName}";
                    separator = "";
                    unitName = "";
                }
                else
                {
                    divisionName = "";
                    separator = "";
                    unitName = "";
                }
            }
            else if (accountingUnitId > 0 && divisionId > 0)
            {
                var summary = viewModel.Reports.FirstOrDefault();
                if (summary != null)
                {
                    unitName = $"UNIT {summary.AccountingUnitName}";
                    separator = " - ";
                    divisionName = $"DIVISI {summary.DivisionName}";
                }
                else
                {
                    divisionName = "";
                    separator = "";
                    unitName = "";
                }
            }

            var document = new Document(PageSize.A4.Rotate(), 5, 5, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            SetHeader(document, date, timezoneOffset, isImport, isForeignCurrency, unitName, separator, divisionName);

            SetReportTable(document, viewModel, timezoneOffset, isImport, isForeignCurrency);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static PdfPCell GetCurrencySummaryTable(List<Summary> currencySummaries)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cellHeader = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };


            cellHeader.Phrase = new Phrase("Mata Uang", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Total", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            foreach (var currency in currencySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(currency.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotal), _smallerFont);
                table.AddCell(cell);

                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotalCurrency), _smallerFont);
                //table.AddCell(cell);
            }

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        //private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummaries, decimal categorySummaryTotal)
        //{
        //    var table = new PdfPTable(2)
        //    {
        //        WidthPercentage = 100
        //    };

        //    var widths = new List<float>() { 1f, 2f };
        //    table.SetWidths(widths.ToArray());

        //    // set header
        //    var cell = new PdfPCell()
        //    {
        //        HorizontalAlignment = Element.ALIGN_CENTER,
        //        VerticalAlignment = Element.ALIGN_CENTER
        //    };

        //    cell.Phrase = new Phrase("Kategori", _smallerFont);
        //    table.AddCell(cell);

        //    cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
        //    table.AddCell(cell);

        //    foreach (var categorySummary in categorySummaries)
        //    {
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.Phrase = new Phrase(categorySummary.Category, _smallerFont);
        //        table.AddCell(cell);

        //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        cell.Phrase = new Phrase(string.Format("{0:n}", categorySummary.SubTotal), _smallerFont);
        //        table.AddCell(cell);
        //    }

        //    cell.Phrase = new Phrase("", _smallerFont);
        //    table.AddCell(cell);

        //    cell.Phrase = new Phrase(string.Format("{0:n}", categorySummaryTotal), _smallerFont);
        //    table.AddCell(cell);

        //    return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        //}

        private static PdfPCell GetUnitSummaryTable(List<Summary> unitSummaries)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cellHeader = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var emptyCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
            };

            cellHeader.Phrase = new Phrase("Unit", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Total (IDR)", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            foreach (var unitSummary in unitSummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(unitSummary.Name, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", unitSummary.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            //cell.Phrase = new Phrase("", _smallerFont);
            //table.AddCell(cell);

            //cell.Phrase = new Phrase(string.Format("{0:n}", totalSummary), _smallerFont);
            //table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetUnitSummaryValasTable(List<Summary> unitSummaries)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 2f, 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cellHeader = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellNoBorderBot = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthBottom = 0
            };

            var cellNoBorderTop = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthTop = 0
            };

            var cellNoBorderTopAndBot = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthTop = 0,
                BorderWidthBottom = 0
            };

            var emptyCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
            };

            cellHeader.Phrase = new Phrase("Unit", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Currency", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Total", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            List<Summary> summaries = new List<Summary>();

            foreach (var unitSummary in unitSummaries)
            {
                if (summaries.Any(x => x.Name == unitSummary.Name))
                    summaries.Add(new Summary
                    {
                        Name = "",
                        CurrencyCode = unitSummary.CurrencyCode,
                        SubTotal = unitSummary.SubTotal,
                        SubTotalCurrency = unitSummary.SubTotalCurrency,
                        AccountingLayoutIndex = unitSummary.AccountingLayoutIndex
                    });
                else
                    summaries.Add(unitSummary);
            }

            var lastItem = summaries.Last();
            foreach (var summary in summaries)
            {
                if (summary.Equals(lastItem))
                {
                    cellNoBorderTop.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderTop.Phrase = new Phrase(summary.Name, _smallerFont);
                    table.AddCell(cellNoBorderTop);
                }
                else if (String.IsNullOrEmpty(summary.Name))
                {
                    cellNoBorderTopAndBot.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderTopAndBot.Phrase = new Phrase(summary.Name, _smallerFont);
                    table.AddCell(cellNoBorderTopAndBot);
                }
                else
                {
                    cellNoBorderBot.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderBot.Phrase = new Phrase(summary.Name, _smallerFont);
                    table.AddCell(cellNoBorderBot);
                }

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(summary.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", summary.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            //cell.Phrase = new Phrase("", _smallerFont);
            //table.AddCell(cell);

            //cell.Phrase = new Phrase(string.Format("{0:n}", totalSummary), _smallerFont);
            //table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummary)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cellHeader = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            cellHeader.Phrase = new Phrase("Category", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Total", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            foreach (var category in categorySummary)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(category.CategoryName, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", category.SubTotal), _smallerFont);
                table.AddCell(cell);

                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotalIDR), _smallerFont);
                //table.AddCell(cell);
            }

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetCategoryValasTable(List<Summary> categorySummaries)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 2f, 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cellHeader = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellNoBorderBot = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthBottom = 0
            };

            var cellNoBorderTop = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthTop = 0
            };

            var cellNoBorderTopAndBot = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                BorderWidthTop = 0,
                BorderWidthBottom = 0
            };

            var emptyCell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER
            };

            cellHeader.Phrase = new Phrase("Category", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Currency", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Total", _smallerBoldWhiteFont);
            table.AddCell(cellHeader);

            List<Summary> summaries = new List<Summary>();

            foreach (var categorySummary in categorySummaries)
            {
                if (summaries.Any(x => x.CategoryName == categorySummary.CategoryName))
                    summaries.Add(new Summary
                    {
                        CategoryName = "",
                        CurrencyCode = categorySummary.CurrencyCode,
                        SubTotal = categorySummary.SubTotal,
                        
                    });
                else
                    summaries.Add(categorySummary);
            }

            var lastItem = summaries.Last();
            foreach (var summary in summaries)
            {
                if (summary.Equals(lastItem))
                {
                    cellNoBorderTop.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderTop.Phrase = new Phrase(summary.CategoryName, _smallerFont);
                    table.AddCell(cellNoBorderTop);
                }
                else if (String.IsNullOrEmpty(summary.CategoryName))
                {
                    cellNoBorderTopAndBot.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderTopAndBot.Phrase = new Phrase(summary.CategoryName, _smallerFont);
                    table.AddCell(cellNoBorderTopAndBot);
                }
                else
                {
                    cellNoBorderBot.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellNoBorderBot.Phrase = new Phrase(summary.CategoryName, _smallerFont);
                    table.AddCell(cellNoBorderBot);
                }

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(summary.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", summary.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            //cell.Phrase = new Phrase("", _smallerFont);
            //table.AddCell(cell);

            //cell.Phrase = new Phrase(string.Format("{0:n}", totalSummary), _smallerFont);
            //table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static void SetReportTable(Document document, UnpaidDispositionReportDetailViewModel viewModel, int timezoneOffset, bool isImport, bool isForeignCurrency)
        {
            var table = new PdfPTable(12)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>();
            for (var i = 0; i < 12; i++)
            {
                if (i == 0 | i == 10)
                {
                    widths.Add(1f);
                    continue;
                }

                widths.Add(2f);
            }
            table.SetWidths(widths.ToArray());

            SetReportTableHeader(table);

            var grouppedByCategoryNames = viewModel.Reports.Where(item => item.CategoryName != null).OrderBy(order => order.CategoryLayoutIndex).GroupBy(x => x.CategoryName).ToList();

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

            var cellColspan9 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Colspan = 9
            };


            int no = 1;
            foreach (var grouppedCategory in grouppedByCategoryNames)
            {
                var totalCurrencies = new Dictionary<string, decimal>();
                foreach (var data in grouppedCategory)
                {
                    cell.Phrase = new Phrase(no.ToString(), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.DispositionDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd-MM-yyyy"), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.DispositionNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.URNNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.UPONo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.InvoiceNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.SupplierName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.CategoryName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.AccountingUnitName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.PaymentDueDate.GetValueOrDefault().ToString("dd-MM-yyyy"), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.CurrencyCode, _smallerFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.Total), _smallerFont);
                    table.AddCell(cellAlignRight);

                    // Currency summary
                    if (totalCurrencies.ContainsKey(data.CurrencyCode))
                    {
                        totalCurrencies[data.CurrencyCode] += data.Total;
                    }
                    else
                    {
                        totalCurrencies.Add(data.CurrencyCode, data.Total);
                    }

                    no++;
                }

                foreach (var totalCurrency in totalCurrencies)
                {
                    cellColspan9.Phrase = new Phrase();
                    table.AddCell(cellColspan9);

                    cell.Phrase = new Phrase("JUMLAH", _smallerBoldFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(totalCurrency.Key, _smallerBoldFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalCurrency.Value), _smallerBoldFont);
                    table.AddCell(cellAlignRight);
                }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            //var summaryTable = new PdfPTable(5)
            //{
            //    WidthPercentage = 95,

            //};            

            //var widthSummaryTable = new List<float>() { 2f, 1f, 2f,1f,2f};
            //summaryTable.SetWidths(widthSummaryTable.ToArray());

            ////UnitSummary
            //if (isForeignCurrency || isImport)
            //    summaryTable.AddCell(GetUnitSummaryValasTable(viewModel.UnitSummaries));
            //else
            //    summaryTable.AddCell(GetUnitSummaryTable(viewModel.UnitSummaries));

            //summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });

            ////CategorySummary
            //if (isForeignCurrency || isImport)
            //    summaryTable.AddCell(GetCategoryValasTable(viewModel.CategorySummaries));
            //else
            //    summaryTable.AddCell(GetCategorySummaryTable(viewModel.CategorySummaries));

            //summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            ////CurrencySummary
            //summaryTable.AddCell(GetCurrencySummaryTable(viewModel.CurrencySummaries));



            //document.Add(summaryTable);

            //New PDF Summary Table Setter
            
            var currencyTable = new PdfPTable(5)
            {
                WidthPercentage = 95,

            };

            var leftTable = new PdfPTable(1)
            {
                WidthPercentage = 95,

            };
            var centerTable = new PdfPTable(1)
            {
                WidthPercentage = 95,
            };
            var rightTable = new PdfPTable(1)
            {
                WidthPercentage = 95,

            };

            var leftTableCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                Border = Rectangle.NO_BORDER

            };
            var centerTableCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                Border = Rectangle.NO_BORDER
            };
            var RightTableCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                Border = Rectangle.NO_BORDER

            };

            var currencyWidthTable = new List<float>() { 3f, 1f, 3f, 1f, 3f };
            currencyTable.SetWidths(currencyWidthTable.ToArray());

            //UnitSummary
            if (isForeignCurrency || isImport)
                leftTable.AddCell(GetUnitSummaryValasTable(viewModel.UnitSummaries));
            else
                leftTable.AddCell(GetUnitSummaryTable(viewModel.UnitSummaries));

            leftTableCell.AddElement(leftTable);
            currencyTable.AddCell(leftTableCell);

            currencyTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });


            //CategorySummary
            if (isForeignCurrency || isImport)
                centerTable.AddCell(GetCategoryValasTable(viewModel.CategorySummaries));
            else
                centerTable.AddCell(GetCategorySummaryTable(viewModel.CategorySummaries));

            centerTableCell.AddElement(centerTable);
            currencyTable.AddCell(centerTableCell);

            currencyTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });

            //CurrencySummary
            rightTable.AddCell(GetCurrencySummaryTable(viewModel.CurrencySummaries));
            RightTableCell.AddElement(rightTable);
            currencyTable.AddCell(RightTableCell);

            //RightTableCell.AddElement(rightTable);
            //currencyTable.AddCell(RightTableCell);

            document.Add(currencyTable);
        }

        private static void SetReportTableHeader(PdfPTable table)
        {
            var cell = new PdfPCell()
            {
                BackgroundColor = new BaseColor(23, 50, 80),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 2
            };

            cell.Phrase = new Phrase("No", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Tgl Disposisi", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("No Disposisi", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("No SPB", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("No BP", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("No Invoice", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Supplier", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Kategori", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Unit", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Jatuh Tempo", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Currency", _smallerBoldWhiteFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Saldo", _smallerBoldWhiteFont);
            table.AddCell(cell);
        }

        private static void SetHeader(Document document, DateTimeOffset dateTo, int timezoneOffset, bool isImport, bool isForeignCurrency, string unitName, string separator, string divisionName)
        {
            var dueDateString = $"{dateTo:dd/MM/yyyy}";
            if (dateTo == DateTimeOffset.MaxValue)
                dueDateString = "-";

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

            var title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) LOKAL";

            if (isForeignCurrency)
                title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) LOKAL VALAS";
            else if (isImport)
                title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) IMPOR";

            cell.Phrase = new Phrase(title, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(unitName + separator + divisionName, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"PERIODE S.D. {dueDateString}", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
