using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.PdfGenerator
{
    public class BudgetCashflowDivisionPdf : IBudgetCashflowDivisionPdf
    {
        private readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
        private readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
        private readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private readonly Font _normalBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9, 0, BaseColor.White);
        private readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private readonly Font _smallerBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, BaseColor.White);

        private readonly IBudgetCashflowService _budgetCashflowService;
        private readonly IdentityService _identityService;
        private readonly List<DivisionDto> _divisions;
        private readonly List<CurrencyDto> _currencies;
        private readonly List<UnitDto> _units;
        private List<UnitDto> _selectedUnits;
        //private List<UnitDto> _writtenUnits;
        private List<DivisionDto> _selectedDivisions;
        //private List<DivisionDto> _writtenDivisions;

        public BudgetCashflowDivisionPdf(IServiceProvider serviceProvider)
        {
            _budgetCashflowService = serviceProvider.GetService<IBudgetCashflowService>();
            _identityService = serviceProvider.GetService<IdentityService>();

            var cache = serviceProvider.GetService<IDistributedCache>();

            var jsonDivisions = cache.GetString(MemoryCacheConstant.Divisions);
            _divisions = JsonConvert.DeserializeObject<List<DivisionDto>>(jsonDivisions, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            var jsonUnits = cache.GetString(MemoryCacheConstant.Units);
            _units = JsonConvert.DeserializeObject<List<UnitDto>>(jsonUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            var jsonCurrencies = cache.GetString(MemoryCacheConstant.Currencies);
            _currencies = JsonConvert.DeserializeObject<List<CurrencyDto>>(jsonCurrencies, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        private List<BudgetCashflowDivisionDto> GetDivisionBudgetCashflow(int divisionId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowDivisionDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                result.Add(_budgetCashflowService.GetBudgetCashflowDivision(layoutOrder, divisionId, dueDate));
            }

            return result;
        }

        public MemoryStream Generate(int divisionId, DateTimeOffset dueDate)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            //writer.CloseStream = false;
            // calling PDFFooter class to Include in document
            //writer.PageEvent = new PDFFooter();

            document.Open();

            var budgetCashflowDivisions = GetDivisionBudgetCashflow(divisionId, dueDate);
            var rowData = budgetCashflowDivisions.SelectMany(element => element.Items).ToList();

            var unitIds = budgetCashflowDivisions.SelectMany(element => element.UnitIds).Distinct().Where(element => element > 0).ToList();

            var divisionIds = _units.Where(element => unitIds.Contains(element.Id)).Select(element => element.DivisionId).Distinct().ToList();

            SetTitle(document, divisionId, dueDate);
            //SetDivisionTable(document, unitIds, divisionIds, rowData);
            var dataRows = GetDataRows(rowData);
            SetData(document, dataRows);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private void SetTitle(Document document, int divisionId, DateTimeOffset dueDate)
        {
            var company = "PT EFRATA GARMINDO UTAMA";
            var title = "LAPORAN BUDGET CASHFLOW";
            var divisionName = "SEMUA DIVISI";

            var division = _divisions.FirstOrDefault(element => element.Id == divisionId);
            if (division != null)
                divisionName = $"DIVISI: {division.Name}";

            var date = $"PERIODE S.D. {dueDate.AddMonths(1).AddHours(_identityService.TimezoneOffset).DateTime.ToString("MMMM yyyy", new CultureInfo("id-ID")).ToUpper()}";

            var table = new PdfPTable(1)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Phrase = new Phrase(company, _headerFont),
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase(title, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(divisionName, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(date, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("\n", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }

        private void SetDivisionTable(Document document, List<int> unitIds, List<int> divisionIds, List<BudgetCashflowDivisionItemDto> rowData)
        {
            //int div = divisionIds.Count > 2 ? 2 : divisionIds.Count;
            //int uni = unitIds.Count > 2 ? 2 : unitIds.Count;
            //int div = divisionIds.Count;
            //int uni = unitIds.Count;

            _selectedUnits = _units.Where(unit => unitIds.Contains(unit.Id)).ToList();
            _selectedDivisions = _divisions.Where(division => divisionIds.Contains(division.Id)).ToList();

            int lastColumn = 5 + 1 + (_selectedDivisions.Count * 2) + (_selectedUnits.Count * 2) + 1;

            //var headerTable = new PdfPTable(lastColumn)
            //{
            //    WidthPercentage = 100
            //};

            var table = new PdfPTable(lastColumn)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>();
            for (var i = 1; i <= lastColumn; i++)
            {
                if (i == 1 || i == 2)
                {
                    widths.Add(3f);
                    continue;
                }

                if (i == 3 || i == 4)
                {
                    widths.Add(1f);
                    continue;
                }

                if (i == 5)
                {
                    widths.Add(15f);
                    continue;
                }

                if (i == 6)
                {
                    widths.Add(5f);
                    continue;
                }

                widths.Add(10f);
            }
            //headerTable.SetWidths(widths.ToArray());
            table.SetWidths(widths.ToArray());

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            cell.BackgroundColor = new BaseColor(197, 90, 17);
            cell.Colspan = 5;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("KETERANGAN", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("MATA UANG", _normalBoldWhiteFont);
            table.AddCell(cell);



            //_selectedUnits = _units.Where(unit => unitIds.Contains(unit.Id)).ToList();
            //_selectedDivisions = _divisions.Where(division => divisionIds.Contains(division.Id)).ToList().GetRange(0, 1);

            foreach (var division in _selectedDivisions)
            {
                var units = _selectedUnits.Where(unit => unit.DivisionId == division.Id);
                foreach (var unit in units)
                {
                    cell.BackgroundColor = new BaseColor(23, 50, 80);
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase($"{unit.Name} VALAS", _normalBoldWhiteFont);
                    table.AddCell(cell);

                    cell.BackgroundColor = new BaseColor(23, 50, 80);
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase($"{unit.Name} IDR", _normalBoldWhiteFont);
                    table.AddCell(cell);
                }

                cell.BackgroundColor = new BaseColor(23, 50, 80);
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase($"DIVISI {division.Name} VALAS", _normalBoldWhiteFont);
                table.AddCell(cell);

                cell.BackgroundColor = new BaseColor(23, 50, 80);
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase($"DIVISI {division.Name} IDR", _normalBoldWhiteFont);
                table.AddCell(cell);
            }

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("ACTUAL", _normalBoldWhiteFont);
            table.AddCell(cell);

            //document.Add(table);
            //document.NewPage();
            //document.Add(table);

            // COUNT ROWS
            var oaciRow = 0;
            var oaciTotalRow = 0;
            var oacoRow = 0;
            var oacoTotalRow = 0;
            var oaDiffRow = 0;
            var iaciRow = 0;
            var iaciTotalRow = 0;
            var iacoRow = 0;
            var iacoTotalRow = 0;
            var iaDiffRow = 0;
            var faciRow = 0;
            var faciTotalRow = 0;
            var facoRow = 0;
            var facoTotalRow = 0;
            var faDiffRow = 0;

            // OACI ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ExportSales; layoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    oaciRow += 1;
                }
            }

            var oaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oaciTotalCurrencyIds.Count > 1)
                oaciTotalCurrencyIds = oaciTotalCurrencyIds.Where(element => element > 0).ToList();

            // OACI TOTAL ROWS
            foreach (var currencyId in oaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                oaciTotalRow += 1;
            }

            // OACO ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial; layoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    oacoRow += 1;
                }
            }

            var oacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oacoTotalCurrencyIds.Count > 1)
                oacoTotalCurrencyIds = oacoTotalCurrencyIds.Where(element => element > 0).ToList();

            // OACO TOTAL ROWS
            foreach (var currencyId in oacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                oacoTotalRow += 1;
            }

            var oadiffCurrencyIds = oaciTotalCurrencyIds.Concat(oacoTotalCurrencyIds).Distinct().ToList();
            if (oadiffCurrencyIds.Count > 1)
                oadiffCurrencyIds = oadiffCurrencyIds.Where(element => element > 0).ToList();

            // OA DIFF ROWS
            foreach (var currencyId in oadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                oaDiffRow += 1;
            }

            // IACI ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInDeposit; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    iaciRow += 1;
                }
            }

            var iaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iaciTotalCurrencyIds.Count > 1)
                iaciTotalCurrencyIds = iaciTotalCurrencyIds.Where(element => element > 0).ToList();

            // IACI TOTAL ROWS
            foreach (var currencyId in iaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                iaciTotalRow += 1;
            }

            // IACO ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.MachineryPurchase; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    iacoRow += 1;
                }
            }

            var iacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iacoTotalCurrencyIds.Count > 1)
                iacoTotalCurrencyIds = iacoTotalCurrencyIds.Where(element => element > 0).ToList();

            // IACO TOTAL ROWS
            foreach (var currencyId in iacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                iacoTotalRow += 1;
            }

            var iadiffCurrencyIds = iaciTotalCurrencyIds.Concat(iacoTotalCurrencyIds).Distinct().ToList();
            if (iadiffCurrencyIds.Count > 1)
                iadiffCurrencyIds = iadiffCurrencyIds.Where(element => element > 0).ToList();

            // IA DIFF ROWS
            foreach (var currencyId in iadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                iaDiffRow += 1;
            }

            // FACI ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    faciRow += 1;
                }
            }

            var faciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (faciTotalCurrencyIds.Count > 1)
                faciTotalCurrencyIds = faciTotalCurrencyIds.Where(element => element > 0).ToList();

            // FACI TOTAL ROWS
            foreach (var currencyId in faciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                faciTotalRow += 1;
            }

            // FACO ROWS
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashOutInstallments; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    facoRow += 1;
                }
            }

            var facoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (facoTotalCurrencyIds.Count > 1)
                facoTotalCurrencyIds = facoTotalCurrencyIds.Where(element => element > 0).ToList();

            // FACO TOTAL ROWS
            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                facoTotalRow += 1;
            }

            var fadiffCurrencyIds = faciTotalCurrencyIds.Concat(facoTotalCurrencyIds).Distinct().ToList();
            if (fadiffCurrencyIds.Count > 1)
                fadiffCurrencyIds = fadiffCurrencyIds.Where(element => element > 0).ToList();

            // FA DIFF ROWS
            foreach (var currencyId in fadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                faDiffRow += 1;
            }

            cell.BackgroundColor = new BaseColor(255, 255, 255);
            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 2 + oaciRow + oaciTotalRow + 6 + oacoRow + oacoTotalRow + oaDiffRow;
            cell.Phrase = new Phrase("OPERATING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 2 + oaciRow + oaciTotalRow;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            // OACI
            var isRevenueWritten = false;
            var isRevenueFromOtherWritten = false;
            int firstOaci = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ExportSales; layoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isRevenueWritten)
                {
                    isRevenueWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pendapatan Operasional:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isRevenueFromOtherWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                {
                    isRevenueFromOtherWritten = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pendapatan Operasional Lain-lain:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("\n", _smallerFont);
                    table.AddCell(cell);

                    var oaciLabel = "";
                    if ((int)layoutOrder != firstOaci)
                    {
                        oaciLabel = layoutOrder.ToDescriptionString();
                        firstOaci = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(oaciLabel, _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }

            }

            // OACI TOTAL
            bool firstOaciTotal = true;
            foreach (var currencyId in oaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var oaciTotalLabel = firstOaciTotal ? "Total Penerimaan Operasional" : "";
                firstOaciTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(oaciTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 6 + oacoRow + oacoTotalRow;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);
            cell.Rotation = 0;

            // OACO
            var isCOGSWritten = false;
            var isMarketingExpenseWritten = false;
            var isSalesCost = false;
            var isGeneralAdministrativeExpense = false;
            var isGeneralAdministrationCost = false;
            var isOtherOperatingExpense = false;
            int firstOaco = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial; layoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isCOGSWritten)
                {
                    isCOGSWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("HPP/Biaya Produksi:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isMarketingExpenseWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                {
                    isMarketingExpenseWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isSalesCost && layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                {
                    isSalesCost = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Penjualan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isGeneralAdministrativeExpense && layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation)
                {
                    isGeneralAdministrativeExpense = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Administrasi & Umum:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isGeneralAdministrationCost && layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost)
                {
                    isGeneralAdministrationCost = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerBoldFont);
                    table.AddCell(cell);

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 3;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya umum dan administrasi", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isOtherOperatingExpense && layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                {
                    isOtherOperatingExpense = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Operasional Lain-lain:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerFont);
                    table.AddCell(cell);

                    var oacoLabel = "";
                    if ((int)layoutOrder != firstOaco)
                    {
                        oacoLabel = layoutOrder.ToDescriptionString();
                        firstOaco = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(oacoLabel, _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }
            }

            // OACO TOTAL
            bool firstOacoTotal = true;
            foreach (var currencyId in oacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var oacoTotalLabel = firstOacoTotal ? "Total Pengeluaran Biaya Operasional" : "";
                firstOacoTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(oacoTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            // OA DIFF
            bool firstOaDiff = true;
            foreach (var currencyId in oadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var oadiffLabel = firstOaDiff ? "Surplus/Deficit- Kas dari kegiatan Operasional" : "";
                firstOaDiff = false;

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 4;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(oadiffLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInNominal - cashOutNominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", diffActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.BackgroundColor = new BaseColor(255, 255, 255);
            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1 + iaciRow + iaciTotalRow + 1 + iacoRow + iacoTotalRow + iaDiffRow;
            cell.Phrase = new Phrase("INVESTING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1 + iaciRow + iaciTotalRow;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            // IACI
            var isEmptyWritten = false;
            int firstIaci = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInDeposit; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isEmptyWritten)
                {
                    isEmptyWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Penerimaan dari Investasi:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("\n", _smallerFont);
                    table.AddCell(cell);

                    var iaciLabel = "";
                    if ((int)layoutOrder != firstIaci)
                    {
                        iaciLabel = layoutOrder.ToDescriptionString();
                        firstIaci = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(iaciLabel, _smallerBoldFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }
            }

            // IACI TOTAL
            bool firstIaciTotal = true;
            foreach (var currencyId in iaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var iaciTotalLabel = firstIaciTotal ? "Total Penerimaan Investasi" : "";
                firstIaciTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(iaciTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1 + iacoRow + iacoTotalRow;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);
            cell.Rotation = 0;

            // IACO
            var isAssetPurchaseWritten = false;
            int firstIaco = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.MachineryPurchase; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isAssetPurchaseWritten)
                {
                    isAssetPurchaseWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pembayaran pembelian asset tetap:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerFont);
                    table.AddCell(cell);

                    var iacoLabel = "";
                    if ((int)layoutOrder != firstIaco)
                    {
                        iacoLabel = layoutOrder.ToDescriptionString();
                        firstIaco = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    if (layoutOrder.ToDescriptionString() == "Deposito")
                    {
                        cell.Phrase = new Phrase(iacoLabel, _smallerBoldFont);
                    }
                    else
                    {
                        cell.Phrase = new Phrase(iacoLabel, _smallerFont);
                    }
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }
            }

            // IACO TOTAL
            bool firstIacoTotal = true;
            foreach (var currencyId in iacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var iacoTotalLabel = firstIacoTotal ? "Total Pengeluaran Investasi" : "";
                firstIacoTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(iacoTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            // IA DIFF
            bool firstIaDiff = true;
            foreach (var currencyId in iadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var iadiffLabel = firstIaDiff ? "Surplus/Deficit-Kas dalam kegiatan Investasi" : "";
                firstIaDiff = false;

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 4;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(iadiffLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInNominal - cashOutNominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", diffActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.BackgroundColor = new BaseColor(255, 255, 255);
            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 2 + faciRow + faciTotalRow + 3 + facoRow + facoTotalRow + faDiffRow;
            cell.Phrase = new Phrase("FINANCING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 2 + faciRow + faciTotalRow;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            // FACI
            isEmptyWritten = false;
            var isOthersWritten = false;
            int firstFaci = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isEmptyWritten)
                {
                    isEmptyWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Penerimaan dari Pendanaan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isOthersWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates)
                {
                    isOthersWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Penerimaan lain-lain dari pendanaan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("\n", _smallerFont);
                    table.AddCell(cell);

                    var faciLabel = "";
                    if ((int)layoutOrder != firstFaci)
                    {
                        faciLabel = layoutOrder.ToDescriptionString();
                        firstFaci = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    if (layoutOrder.ToDescriptionString() == "Pencairan pinjaman (Loan Withdrawal)")
                    {
                        cell.Phrase = new Phrase(faciLabel, _smallerBoldFont);
                    }
                    else
                    {
                        cell.Phrase = new Phrase(faciLabel, _smallerFont);
                    }
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }
            }

            // FACI TOTAL
            bool firstFaciTotal = true;
            foreach (var currencyId in faciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var faciTotalLabel = firstFaciTotal ? "Total Penerimaan Pendanaan" : "";
                firstFaciTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(faciTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 3 + facoRow + facoTotalRow;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);
            cell.Rotation = 0;

            // FACO
            var isLoanInstallmentWritten = false;
            var isBankExpensesWritten = false;
            var isCashOutOthersWritten = false;
            int firstFaco = 0;
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashOutInstallments; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                if (!isLoanInstallmentWritten)
                {
                    isLoanInstallmentWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pembayaran angsuran dan bunga Pinjaman:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isBankExpensesWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutBankAdministrationFee)
                {
                    isBankExpensesWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pembayaran Biaya Administrasi Bank:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (!isCashOutOthersWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutAffiliates)
                {
                    isCashOutOthersWritten = true;

                    cell.Rotation = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = lastColumn - 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pengeluaran lain-lain dari Pendanaan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerFont);
                    table.AddCell(cell);

                    var facoLabel = "";
                    if ((int)layoutOrder != firstFaco)
                    {
                        facoLabel = layoutOrder.ToDescriptionString();
                        firstFaco = (int)layoutOrder;
                    }

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 2;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(facoLabel, _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(currency.Code, _smallerFont);
                    table.AddCell(cell);

                    var actual = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var currencyNominalDivision = 0.0;
                        var nominalDivision = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var data = selectedData.FirstOrDefault(element => element.CurrencyId == currencyId && element.UnitId == unit.Id);
                            if (data == null)
                                data = new BudgetCashflowDivisionItemDto();

                            currencyNominalDivision += data.CurrencyNominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.CurrencyNominal), _smallerFont);
                            table.AddCell(cell);

                            nominalDivision += data.Nominal;

                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 1;
                            cell.Rowspan = 1;
                            cell.Phrase = new Phrase(string.Format("{0:n}", data.Nominal), _smallerFont);
                            table.AddCell(cell);

                            actual += data.ActualNominal;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominalDivision), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominalDivision), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", actual), _smallerFont);
                    table.AddCell(cell);
                }
            }

            // FACO TOTAL
            bool firstFacoTotal = true;
            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var facoTotalLabel = firstFacoTotal ? "Total pengeluaran pendanaan" : "";
                firstFacoTotal = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(facoTotalLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", currencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", nominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", actualNominal), _smallerFont);
                table.AddCell(cell);
            }

            // FA DIFF
            bool firstFaDiff = true;
            foreach (var currencyId in fadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var fadiffLabel = firstFaDiff ? "Surplus/Deficit-Kas dalam kegiatan Pendanaan" : "";
                firstFaDiff = false;

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 4;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(fadiffLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currency.Code, _smallerFont);
                table.AddCell(cell);

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", cashInNominal - cashOutNominal), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffCurrencyNominal), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", divisionDiffNominal), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", diffActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            // DUMMY FOOTER
            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 5;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("Saldo Awal Kas", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 5;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("TOTAL SURPLUS/DEFISIT KAS", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 5;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("Saldo Akhir Kas", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("Saldo Real Kas", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("Selisih", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 2;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerBoldFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Rate", _smallerBoldFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = lastColumn - 5;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerFont);
            table.AddCell(cell);

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 5;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("TOTAL SURPLUS (DEFISIT) EQUIVALENT", _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(" ", _smallerFont);
                table.AddCell(cell);

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);

                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Colspan = 1;
                        cell.Rowspan = 1;
                        cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                        table.AddCell(cell);
                    }

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                    table.AddCell(cell);
                }

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
                table.AddCell(cell);
            }

            //document.Add(headerTable);
            document.Add(table);
            //document.NewPage();
            //document.Add(headerTable);
        }

        private int _oaciRow;
        private int _oaciTotalRow;
        private int _oacoRow;
        private int _oacoTotalRow;
        private int _oaDiffRow;
        private int _iaciRow;
        private int _iaciTotalRow;
        private int _iacoRow;
        private int _iacoTotalRow;
        private int _iaDiffRow;
        private int _faciRow;
        private int _faciTotalRow;
        private int _facoRow;
        private int _facoTotalRow;
        private int _faDiffRow;

        private List<string> _dynamicColumns;

        private void SetData(Document document, List<PdfDto> data)
        {
            var operatingActivitiesCashInRowspan = _oaciRow + _oaciTotalRow + 2;
            var operatingActivitiesCashOutRowspan = _oacoRow + _oacoTotalRow + 6;
            var operatingActivitiesRowSpan = operatingActivitiesCashInRowspan + operatingActivitiesCashOutRowspan + _oaDiffRow;

            var investingActivitiesCashInRowspan = _iaciRow + _iaciTotalRow + 1;
            var investingActivitiesCashOutRowspan = _iacoRow + _iacoTotalRow + 1;
            var investingActivitiesRowSpan = investingActivitiesCashInRowspan + investingActivitiesCashOutRowspan + _iaDiffRow;

            var financingActivitiesCashInRowspan = _faciRow + _faciTotalRow + 2;
            var financingActivitiesCashOutRowspan = _facoRow + _facoTotalRow + 3;
            var financingActivitiesRowSpan = financingActivitiesCashInRowspan + financingActivitiesCashOutRowspan + _faDiffRow;

            var isFirstTable = true;

            SetDynamicColumns();
            var splittedColumns = SplitList(_dynamicColumns, 7);

            foreach (var splittedColumn in splittedColumns)
            {
                var columnLength = splittedColumn.Count;
                if (isFirstTable)
                {
                    columnLength += 5;
                }

                var table = new PdfPTable(columnLength)
                {
                    WidthPercentage = 100
                };

                var headerBlueCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = new BaseColor(23, 50, 80),
                    MinimumHeight = 35
                };

                var leftRotatedCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    Rotation = -90
                };

                var rightAlignCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 30
                };

                var leftAlignCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 30
                };

                var centerAlignCell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 30
                };

                if (isFirstTable)
                {
                    var widths = new List<float>();
                    for (var i = 1; i <= columnLength; i++)
                    {
                        if (i == 1 || i == 2)
                        {
                            widths.Add(3f);
                            continue;
                        }

                        if (i == 3)
                        {
                            widths.Add(1f);
                            continue;
                        }

                        if (i == 4)
                        {
                            widths.Add(15f);
                            continue;
                        }

                        widths.Add(10f);
                    }
                    table.SetWidths(widths.ToArray());

                    var headerOrangeCell = new PdfPCell()
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        BackgroundColor = new BaseColor(197, 90, 17),
                        Colspan = 4,
                        MinimumHeight = 35
                    };

                    headerOrangeCell.Phrase = new Phrase("KETERANGAN", _normalBoldWhiteFont);
                    table.AddCell(headerOrangeCell);

                    headerBlueCell.Phrase = new Phrase("MATA UANG", _normalBoldWhiteFont);
                    table.AddCell(headerBlueCell);

                    foreach (var column in splittedColumn)
                    {
                        headerBlueCell.Phrase = new Phrase(column.ToUpper(), _normalBoldWhiteFont);
                        table.AddCell(headerBlueCell);
                    }

                    #region OPERATING ACTIVITIES
                    leftRotatedCell.Rowspan = operatingActivitiesRowSpan;
                    leftRotatedCell.Phrase = new Phrase("OPERATING ACTIVITIES", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftRotatedCell.Rowspan = operatingActivitiesCashInRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength - 2;
                    leftAlignCell.Phrase = new Phrase("Pendapatan Operasional:", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var operatingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.ExportSales && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation).ToList();

                    var previousLayoutOrder = (BudgetCashflowCategoryLayoutOrder)0;
                    foreach (var datum in operatingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("Pendapatan Operasional Lain-lain:", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "OACITOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = operatingCashInTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Penerimaan Operasional", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashInTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.OthersOperationalCost).ToList();

                    leftRotatedCell.Rowspan = operatingActivitiesCashOutRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    table.AddCell(leftRotatedCell);

                    previousLayoutOrder = 0;
                    foreach (var datum in operatingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("HPP/Biaya Produksi:", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);

                                leftAlignCell.Phrase = new Phrase("Biaya Penjualan:", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("Biaya Administrasi & Umum:", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("Biaya Umum dan Administrasi", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("Biaya Operasional Lainnya", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "OACOTOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = operatingCashOutTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Pengeluaran Biaya Operasional", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashOutTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "OADIFF").ToList();

                    var isFirst = true;
                    foreach (var datum in operatingDiffTotalData)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Operasional", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }
                        else
                        {
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion

                    #region INVESTING ACTIVITIES
                    leftRotatedCell.Rowspan = investingActivitiesRowSpan;
                    leftRotatedCell.Phrase = new Phrase("INVESTING ACTIVITIES", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftRotatedCell.Rowspan = investingActivitiesCashInRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength - 2;
                    leftAlignCell.Phrase = new Phrase("Penerimaan dari Investasi:", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var investingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashInDeposit && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashInOthers).ToList();

                    previousLayoutOrder = 0;
                    foreach (var datum in investingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            //if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                            //{
                            //    leftAlignCell.Colspan = columnLength - 2;
                            //    leftAlignCell.Phrase = new Phrase("Pendapatan Operasional Lain-lain:", _normalBoldFont);
                            //    table.AddCell(leftAlignCell);
                            //    leftAlignCell.Colspan = 1;
                            //}

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalBoldFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "IACITOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = operatingCashInTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Penerimaan Investasi", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashInTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.MachineryPurchase && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashOutDeposit).ToList();

                    leftRotatedCell.Rowspan = investingActivitiesCashOutRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength - 2;
                    leftAlignCell.Phrase = new Phrase("Pembayaran Pembelian Aset Tetap", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    previousLayoutOrder = 0;
                    foreach (var datum in investingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "IACOTOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = operatingCashOutTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Pengeluaran Investasi", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in investingCashOutTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "IADIFF").ToList();

                    isFirst = true;
                    foreach (var datum in investingDiffTotalData)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Investasi", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }
                        else
                        {
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion

                    #region FINANCING ACTIVITIES
                    leftRotatedCell.Rowspan = financingActivitiesRowSpan;
                    leftRotatedCell.Phrase = new Phrase("FINANCING ACTIVITIES", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftRotatedCell.Rowspan = financingActivitiesCashInRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength - 2;
                    leftAlignCell.Phrase = new Phrase("Penerimaan dari Pendanaan:", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var financingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers).ToList();

                    previousLayoutOrder = 0;
                    foreach (var datum in financingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates)
                            {
                                leftAlignCell.Colspan = columnLength - 2;
                                leftAlignCell.Phrase = new Phrase("Penerimaan Lain-Lain dari Pendanaan", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalBoldFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "FACITOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = financingCashInTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Penerimaan Pendanaan", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in financingCashInTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashOutInstallments && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashOutOthers).ToList();

                    leftRotatedCell.Rowspan = financingActivitiesCashOutRowspan;
                    leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength - 2;
                    leftAlignCell.Phrase = new Phrase("Pembayaran Pembelian Aset Tetap", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    previousLayoutOrder = 0;
                    foreach (var datum in financingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            leftAlignCell.Phrase = new Phrase("\n");
                            table.AddCell(leftAlignCell);
                            leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            table.AddCell(leftAlignCell);
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "FACOTOTAL").ToList();

                    rightAlignCell.Colspan = 2;
                    rightAlignCell.Rowspan = financingCashOutTotalData.Count;
                    rightAlignCell.Phrase = new Phrase("Total Pengeluaran Investasi", _normalBoldFont);
                    table.AddCell(rightAlignCell);
                    rightAlignCell.Colspan = 1;
                    rightAlignCell.Rowspan = 1;

                    foreach (var datum in financingCashOutTotalData)
                    {
                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "FADIFF").ToList();

                    isFirst = true;
                    foreach (var datum in financingDiffTotalData)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Pendanaan", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }
                        else
                        {
                            rightAlignCell.Colspan = 3;
                            rightAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                            table.AddCell(rightAlignCell);
                            rightAlignCell.Colspan = 1;
                        }

                        centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion
                }
                else
                {
                    var widths = new List<float>();
                    for (var i = 1; i <= columnLength; i++)
                    {
                        widths.Add(10f);
                    }
                    table.SetWidths(widths.ToArray());

                    foreach (var column in splittedColumn)
                    {
                        headerBlueCell.Phrase = new Phrase(column.ToUpper(), _normalBoldWhiteFont);
                        table.AddCell(headerBlueCell);
                    }

                    #region OPERATING ACTIVITIES
                    //leftRotatedCell.Rowspan = operatingActivitiesRowSpan;
                    //leftRotatedCell.Phrase = new Phrase("OPERATING ACTIVITIES", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    //leftRotatedCell.Rowspan = operatingActivitiesCashInRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength;
                    leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var operatingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.ExportSales && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation).ToList();

                    var previousLayoutOrder = (BudgetCashflowCategoryLayoutOrder)0;
                    foreach (var datum in operatingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "OACITOTAL").ToList();

                    //rightAlignCell.Colspan = 2;
                    //rightAlignCell.Rowspan = operatingCashInTotalData.Count;
                    //rightAlignCell.Phrase = new Phrase("Total Penerimaan Operasional", _normalBoldFont);
                    //table.AddCell(rightAlignCell);
                    //rightAlignCell.Colspan = 1;
                    //rightAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashInTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.OthersOperationalCost).ToList();

                    //leftRotatedCell.Rowspan = operatingActivitiesCashOutRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    previousLayoutOrder = 0;
                    foreach (var datum in operatingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);

                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "OACOTOTAL").ToList();

                    //leftAlignCell.Colspan = 2;
                    //leftAlignCell.Rowspan = operatingCashOutTotalData.Count;
                    //leftAlignCell.Phrase = new Phrase("Total Pengeluaran Biaya Operasional", _normalBoldFont);
                    //table.AddCell(leftAlignCell);
                    //leftAlignCell.Colspan = 1;
                    //leftAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashOutTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var operatingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "OADIFF").ToList();

                    //leftAlignCell.Colspan = 3;
                    //leftAlignCell.Rowspan = operatingDiffTotalData.Count;
                    //leftAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Operasional", _normalBoldFont);
                    //table.AddCell(leftAlignCell);
                    //leftAlignCell.Colspan = 1;
                    //leftAlignCell.Rowspan = 1;

                    foreach (var datum in operatingDiffTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion

                    #region INVESTING ACTIVITIES
                    //leftRotatedCell.Rowspan = investingActivitiesRowSpan;
                    //leftRotatedCell.Phrase = new Phrase("INVESTING ACTIVITIES", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    //leftRotatedCell.Rowspan = investingActivitiesCashInRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength;
                    leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var investingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashInDeposit && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashInOthers).ToList();

                    previousLayoutOrder = 0;
                    foreach (var datum in investingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("Pendapatan Operasional Lain-lain:", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalBoldFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "IACITOTAL").ToList();

                    //rightAlignCell.Colspan = 2;
                    //rightAlignCell.Rowspan = operatingCashInTotalData.Count;
                    //rightAlignCell.Phrase = new Phrase("Total Penerimaan Investasi", _normalBoldFont);
                    //table.AddCell(rightAlignCell);
                    //rightAlignCell.Colspan = 1;
                    //rightAlignCell.Rowspan = 1;

                    foreach (var datum in operatingCashInTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.MachineryPurchase && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashOutDeposit).ToList();

                    //leftRotatedCell.Rowspan = investingActivitiesCashOutRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength;
                    leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    previousLayoutOrder = 0;
                    foreach (var datum in investingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "IACOTOTAL").ToList();

                    //rightAlignCell.Colspan = 2;
                    //rightAlignCell.Rowspan = investingCashOutTotalData.Count;
                    //rightAlignCell.Phrase = new Phrase("Total Pengeluaran Investasi", _normalBoldFont);
                    //table.AddCell(rightAlignCell);
                    //rightAlignCell.Colspan = 1;
                    //rightAlignCell.Rowspan = 1;

                    foreach (var datum in investingCashOutTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var investingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "IADIFF").ToList();

                    //isFirst = true;
                    foreach (var datum in investingDiffTotalData)
                    {
                        //if (isFirst)
                        //{
                        //    isFirst = false;
                        //    rightAlignCell.Colspan = 3;
                        //    rightAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Investasi", _normalBoldFont);
                        //    table.AddCell(rightAlignCell);
                        //    rightAlignCell.Colspan = 1;
                        //}
                        //else
                        //{
                        //    rightAlignCell.Colspan = 3;
                        //    rightAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                        //    table.AddCell(rightAlignCell);
                        //    rightAlignCell.Colspan = 1;
                        //}

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion

                    #region FINANCING ACTIVITIES
                    //leftRotatedCell.Rowspan = financingActivitiesRowSpan;
                    //leftRotatedCell.Phrase = new Phrase("FINANCING ACTIVITIES", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    //leftRotatedCell.Rowspan = financingActivitiesCashInRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH IN", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength;
                    leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    var financingCashInData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers).ToList();

                    previousLayoutOrder = 0;
                    foreach (var datum in financingCashInData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            if (layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates)
                            {
                                leftAlignCell.Colspan = columnLength;
                                leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                                table.AddCell(leftAlignCell);
                                leftAlignCell.Colspan = 1;
                            }

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalBoldFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashInTotalData = data.Where(datum => datum.SpecialOrderRemark == "FACITOTAL").ToList();

                    //rightAlignCell.Colspan = 2;
                    //rightAlignCell.Rowspan = financingCashInTotalData.Count;
                    //rightAlignCell.Phrase = new Phrase("Total Penerimaan Pendanaan", _normalBoldFont);
                    //table.AddCell(rightAlignCell);
                    //rightAlignCell.Colspan = 1;
                    //rightAlignCell.Rowspan = 1;

                    foreach (var datum in financingCashInTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashOutData = data.Where(datum => datum.LayoutOrder >= (int)BudgetCashflowCategoryLayoutOrder.CashOutInstallments && datum.LayoutOrder <= (int)BudgetCashflowCategoryLayoutOrder.CashOutOthers).ToList();

                    //leftRotatedCell.Rowspan = financingActivitiesCashOutRowspan;
                    //leftRotatedCell.Phrase = new Phrase("CASH OUT", _normalFont);
                    //table.AddCell(leftRotatedCell);

                    leftAlignCell.Colspan = columnLength;
                    leftAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                    table.AddCell(leftAlignCell);
                    leftAlignCell.Colspan = 1;

                    previousLayoutOrder = 0;
                    foreach (var datum in financingCashOutData)
                    {
                        var layoutOrder = (BudgetCashflowCategoryLayoutOrder)datum.LayoutOrder;

                        if (layoutOrder != previousLayoutOrder)
                        {
                            previousLayoutOrder = layoutOrder;

                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase(layoutOrder.ToDescriptionString(), _normalFont);
                            //table.AddCell(leftAlignCell);
                        }
                        else
                        {
                            //leftAlignCell.Phrase = new Phrase("\n");
                            //table.AddCell(leftAlignCell);
                            //leftAlignCell.Phrase = new Phrase("\n", _normalFont);
                            //table.AddCell(leftAlignCell);
                        }

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingCashOutTotalData = data.Where(datum => datum.SpecialOrderRemark == "FACOTOTAL").ToList();

                    //rightAlignCell.Colspan = 2;
                    //rightAlignCell.Rowspan = financingCashOutTotalData.Count;
                    //rightAlignCell.Phrase = new Phrase("Total Pengeluaran Investasi", _normalBoldFont);
                    //table.AddCell(rightAlignCell);
                    //rightAlignCell.Colspan = 1;
                    //rightAlignCell.Rowspan = 1;

                    foreach (var datum in financingCashOutTotalData)
                    {
                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }

                    var financingDiffTotalData = data.Where(datum => datum.SpecialOrderRemark == "FADIFF").ToList();

                    //isFirst = true;
                    foreach (var datum in financingDiffTotalData)
                    {
                        //if (isFirst)
                        //{
                        //    isFirst = false;
                        //    rightAlignCell.Colspan = 3;
                        //    rightAlignCell.Phrase = new Phrase("Surplus/Deficit - Kas dari Kegiatan Pendanaan", _normalBoldFont);
                        //    table.AddCell(rightAlignCell);
                        //    rightAlignCell.Colspan = 1;
                        //}
                        //else
                        //{
                        //    rightAlignCell.Colspan = 3;
                        //    rightAlignCell.Phrase = new Phrase("\n", _normalBoldFont);
                        //    table.AddCell(rightAlignCell);
                        //    rightAlignCell.Colspan = 1;
                        //}

                        //centerAlignCell.Phrase = new Phrase(datum.CurrencyCode, _normalFont);
                        //table.AddCell(centerAlignCell);

                        foreach (var key in splittedColumn)
                        {
                            rightAlignCell.Phrase = new Phrase(datum.Values.GetValueOrDefault(key), _normalFont);
                            table.AddCell(rightAlignCell);
                        }
                    }
                    #endregion
                }

                document.Add(table);
                document.NewPage();
                isFirstTable = false;
            }
        }

        private void SetDynamicColumns()
        {
            _dynamicColumns = new List<string>();
            foreach (var division in _selectedDivisions)
            {
                foreach (var unit in _selectedUnits.Where(element => element.DivisionId == division.Id))
                {
                    _dynamicColumns.Add($"{unit.Name} Valas");
                    _dynamicColumns.Add($"{unit.Name} IDR");
                }
                _dynamicColumns.Add($"Divisi {division.Name} Valas");
                _dynamicColumns.Add($"Divisi {division.Name} IDR");
            }

            _dynamicColumns.Add($"Actual");
        }

        private static List<List<T>> SplitList<T>(List<T> locations, int nSize = 7)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        private List<PdfDto> GetDataRows(List<BudgetCashflowDivisionItemDto> rowData)
        {
            var result = new List<PdfDto>();

            var unitIds = rowData.Select(element => element.UnitId).Distinct().Where(element => element > 0).ToList();
            var divisionIds = _units.Where(element => unitIds.Contains(element.Id)).Select(element => element.DivisionId).Distinct().ToList();
            _selectedUnits = _units.Where(unit => unitIds.Contains(unit.Id)).OrderBy(unit => unit.Name).ToList();
            _selectedDivisions = _divisions.Where(division => divisionIds.Contains(division.Id)).OrderBy(division => division.BudgetCashflowColumnOrder).ToList();

            #region OPERATING ACTIVITIES
            _oaciRow = 0;
            _oaciTotalRow = 0;
            _oacoRow = 0;
            _oacoTotalRow = 0;
            _oaDiffRow = 0;

            #region OACI
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ExportSales; layoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _oaciRow += 1;
                }
            }
            #endregion

            #region OACI TOTAL
            var oaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oaciTotalCurrencyIds.Count > 1)
                oaciTotalCurrencyIds = oaciTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in oaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "OACITOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _oaciTotalRow += 1;
            }
            #endregion

            #region OACO
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial; layoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();



                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder == layoutOrder && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder == layoutOrder && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder == layoutOrder && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _oacoRow += 1;
                }
            }
            #endregion

            #region OACO TOTAL
            var oacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oacoTotalCurrencyIds.Count > 1)
                oacoTotalCurrencyIds = oacoTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in oacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "OACOTOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _oacoTotalRow += 1;
            }
            #endregion

            #region OADIFF
            var oadiffCurrencyIds = oaciTotalCurrencyIds.Concat(oacoTotalCurrencyIds).Distinct().ToList();
            if (oadiffCurrencyIds.Count > 1)
                oadiffCurrencyIds = oadiffCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in oadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "OADIFF";
                row.CurrencyCode = currency.Code;

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", cashInNominal - cashOutNominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionDiffCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionDiffNominal));
                }

                row.Values.Add($"Actual", string.Format("{0:n}", diffActualNominal));
                result.Add(row);

                _oaDiffRow += 1;
            }
            #endregion
            #endregion

            #region INVESTING ACTIVITIES
            _iaciRow = 0;
            _iaciTotalRow = 0;
            _iacoRow = 0;
            _iacoTotalRow = 0;
            _iaDiffRow = 0;

            #region IACI
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInDeposit; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();



                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _iaciRow += 1;
                }
            }
            #endregion

            #region IACI TOTAL
            var iaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iaciTotalCurrencyIds.Count > 1)
                iaciTotalCurrencyIds = iaciTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in iaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "IACITOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _iaciTotalRow += 1;
            }
            #endregion

            #region IACO
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.MachineryPurchase; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();



                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _iacoRow += 1;
                }
            }
            #endregion

            #region IACO TOTAL
            var iacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iacoTotalCurrencyIds.Count > 1)
                iacoTotalCurrencyIds = iacoTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in iacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "IACOTOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _iacoTotalRow += 1;
            }
            #endregion

            #region IADIFF
            var iadiffCurrencyIds = iaciTotalCurrencyIds.Concat(iacoTotalCurrencyIds).Distinct().ToList();
            if (iadiffCurrencyIds.Count > 1)
                iadiffCurrencyIds = iadiffCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in iadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "IADIFF";
                row.CurrencyCode = currency.Code;

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", cashInNominal - cashOutNominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionDiffCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionDiffNominal));
                }

                row.Values.Add($"Actual", string.Format("{0:n}", diffActualNominal));
                result.Add(row);

                _iaDiffRow += 1;
            }
            #endregion
            #endregion

            #region FINANCING ACTIVITIES
            _faciRow = 0;
            _faciTotalRow = 0;
            _facoRow = 0;
            _facoTotalRow = 0;
            _faDiffRow = 0;

            #region FACI
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();



                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _faciRow += 1;
                }
            }
            #endregion

            #region FACI TOTAL
            var faciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (faciTotalCurrencyIds.Count > 1)
                faciTotalCurrencyIds = faciTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in faciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "FACITOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _faciTotalRow += 1;
            }
            #endregion

            #region FACO
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashOutInstallments; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    var row = new PdfDto();
                    row.LayoutOrder = (int)layoutOrder;
                    row.CurrencyCode = currency.Code;

                    var actualNominal = 0.0;
                    foreach (var division in _selectedDivisions)
                    {
                        var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();



                        var divisionCurrencyNominal = 0.0;
                        var divisionNominal = 0.0;
                        foreach (var unit in selectedUnits)
                        {
                            var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                            divisionCurrencyNominal += currencyNominal;

                            var nominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.Nominal);
                            divisionNominal += nominal;

                            var actual = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.ActualNominal);
                            actualNominal += actual;

                            row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                            row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                        }

                        row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                        row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                    }
                    row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                    result.Add(row);

                    _facoRow += 1;
                }
            }
            #endregion

            #region FACO TOTAL
            var facoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iacoTotalCurrencyIds.Count > 1)
                iacoTotalCurrencyIds = iacoTotalCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in iacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "FACOTOTAL";
                row.CurrencyCode = currency.Code;

                var actualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionCurrencyNominal = 0.0;
                    var divisionNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var currencyNominal = rowData
                                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                                .Sum(element => element.CurrencyNominal);
                        divisionCurrencyNominal += currencyNominal;

                        var nominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);
                        divisionNominal += nominal;

                        var actual = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);
                        actualNominal += actual;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", currencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", nominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionNominal));
                }
                row.Values.Add($"Actual", string.Format("{0:n}", actualNominal));
                result.Add(row);

                _facoTotalRow += 1;
            }
            #endregion

            #region IADIFF
            var fadiffCurrencyIds = faciTotalCurrencyIds.Concat(facoTotalCurrencyIds).Distinct().ToList();
            if (fadiffCurrencyIds.Count > 1)
                fadiffCurrencyIds = fadiffCurrencyIds.Where(element => element > 0).ToList();

            foreach (var currencyId in fadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                var row = new PdfDto();
                row.SpecialOrderRemark = "FADIFF";
                row.CurrencyCode = currency.Code;

                var diffActualNominal = 0.0;
                foreach (var division in _selectedDivisions)
                {
                    var selectedUnits = _selectedUnits.Where(unit => unit.DivisionId == division.Id).ToList();

                    var divisionDiffCurrencyNominal = 0.0;
                    var divisionDiffNominal = 0.0;
                    foreach (var unit in selectedUnits)
                    {
                        var cashInCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashOutCurrencyNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.CurrencyNominal);

                        var cashInNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashOutNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.Nominal);

                        var cashInActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        var cashOutActualNominal = rowData
                            .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers && element.CurrencyId == currencyId && element.UnitId == unit.Id)
                            .Sum(element => element.ActualNominal);

                        divisionDiffCurrencyNominal += cashInCurrencyNominal - cashOutCurrencyNominal;
                        divisionDiffNominal += cashInNominal - cashOutNominal;
                        diffActualNominal += cashInActualNominal - cashOutActualNominal;

                        row.Values.Add($"{unit.Name} Valas", string.Format("{0:n}", cashInCurrencyNominal - cashOutCurrencyNominal));
                        row.Values.Add($"{unit.Name} IDR", string.Format("{0:n}", cashInNominal - cashOutNominal));
                    }

                    row.Values.Add($"Divisi {division.Name} Valas", string.Format("{0:n}", divisionDiffCurrencyNominal));
                    row.Values.Add($"Divisi {division.Name} IDR", string.Format("{0:n}", divisionDiffNominal));
                }

                row.Values.Add($"Actual", string.Format("{0:n}", diffActualNominal));

                _faDiffRow += 1;
            }
            #endregion
            #endregion

            return result;
        }

        public class PdfDto
        {
            public PdfDto()
            {
                Values = new Dictionary<string, string>();
            }

            public int LayoutOrder { get; set; }
            public string CurrencyCode { get; set; }
            public string SpecialOrderRemark { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        public class PDFFooter : PdfPageEventHelper
        {
            // write on top of document
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                //  document.Add(new Paragraph("\n"));
                base.OnOpenDocument(writer, document);
                //    PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                //    tabFot.SpacingAfter = 20F;
                //    PdfPCell cell;
                //    tabFot.TotalWidth = 300F;
                //    cell = new PdfPCell(new Phrase("Header ONe"));
                //    tabFot.AddCell(cell);
                //    tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);
            }

            // write on start of each page
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
                var normalFontUnderlined = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8, Font.UNDERLINE);
                var boldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

                // document.Add(new Paragraph("\n"));
                base.OnStartPage(writer, document);
                //PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                //tabFot.SpacingAfter = 20F;
                //PdfPCell cell;
                //tabFot.TotalWidth = 300F;
                //cell = new PdfPCell(new Phrase("Header Every Page"));
                //tabFot.AddCell(cell);
                //tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);

                #region Header
                PdfPTable headerTable = new PdfPTable(new float[] { 1F });



                headerTable.TotalWidth = 1500F;
                PdfPCell cellHeaderCS4 = new PdfPCell() { Border = Rectangle.NO_BORDER, PaddingTop = 1, PaddingBottom = 1 };
                PdfPCell cellHeader = new PdfPCell() { Border = Rectangle.NO_BORDER, PaddingTop = 1, PaddingBottom = 1 };


                cellHeaderCS4.Phrase = new Phrase("PT EFRATA GARMINDO UTAMA", header_font);
                cellHeaderCS4.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable.AddCell(cellHeaderCS4);

                cellHeaderCS4.Phrase = new Phrase("SISTEM INFORMASI ABSENSI", normalFontUnderlined);
                cellHeaderCS4.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable.AddCell(cellHeaderCS4);


                cellHeaderCS4.Phrase = new Phrase("LAPORAN ABSENSI KARYAWAN PERIODIK UPAH", boldFont);
                cellHeaderCS4.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerTable.AddCell(cellHeaderCS4);

                cellHeaderCS4.Phrase = new Phrase("   ", boldFont);
                cellHeaderCS4.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerTable.AddCell(cellHeaderCS4);


                headerTable.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);

                document.Add(headerTable);

                #endregion

            }

            // write on end of each page
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                PdfPCell cell;

                tabFot.TotalWidth = 300F;
                cell = new PdfPCell(new Phrase("  "));
                cell.Border = Rectangle.NO_BORDER;
                tabFot.AddCell(cell);
                tabFot.WriteSelectedRows(0, -1, 150, document.Bottom, writer.DirectContent);
            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
            }
        }
    }
}
