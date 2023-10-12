using Com.Efrata.Service.Purchasing.Lib.Enums;
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

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.ExcelGenerator
{
    public class BudgetCashflowDivisionExcelGenerator : IBudgetCashflowDivisionExcelGenerator
    {
        private readonly IBudgetCashflowService _budgetCashflowService;
        private readonly IdentityService _identityService;
        private readonly List<DivisionDto> _divisions;
        private readonly List<CurrencyDto> _currencies;
        private readonly List<UnitDto> _units;
        private List<UnitDto> _selectedUnits;
        private List<DivisionDto> _selectedDivisions;

        public BudgetCashflowDivisionExcelGenerator(IServiceProvider serviceProvider)
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
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                var budgetCashflowDivisions = GetDivisionBudgetCashflow(divisionId, dueDate);
                var rowData = budgetCashflowDivisions.SelectMany(element => element.Items).ToList();

                var unitIds = budgetCashflowDivisions.SelectMany(element => element.UnitIds).Distinct().Where(element => element > 0).ToList();

                var divisionIds = _units.Where(element => unitIds.Contains(element.Id)).Select(element => element.DivisionId).Distinct().ToList();

                var lastColumn = 3 + 1 + (divisionIds.Count * 2) + (unitIds.Count * 2) + 1;

                SetTitle(worksheet, divisionId, dueDate, lastColumn);
                SetTableHeader(worksheet, unitIds, divisionIds);
                //SetLeftRemark(worksheet, rowData);
                SetData(worksheet, rowData, lastColumn);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private void SetData(ExcelWorksheet worksheet, List<BudgetCashflowDivisionItemDto> rowData, int lastColumn)
        {
            var writeableRow = 6;
            var writeableColumn = 3;

            var startingLeftRemarkRow = 6;

            var operatingActivitiesRow = startingLeftRemarkRow;

            var cashInCashOutStartingRow = 6;
            var operatingCashInRow = cashInCashOutStartingRow;

            var isRevenueWritten = false;
            var isRevenueFromOtherWritten = false;
            // oaci
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ExportSales; layoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isRevenueWritten)
                {
                    isRevenueWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pendapatan Operasional:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashInRow += 1;
                }

                if (!isRevenueFromOtherWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales)
                {
                    isRevenueFromOtherWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pendapatan Operasional Lain-lain:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashInRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashInRow += 1;
                }
            }
            writeableColumn = 3;

            // oaci total
            var oaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oaciTotalCurrencyIds.Count > 1)
                oaciTotalCurrencyIds = oaciTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total Penerimaan Operasional";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in oaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                operatingActivitiesRow += 1;
                operatingCashInRow += 1;
            }
            
            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, operatingCashInRow, "CASH IN", worksheet);
            cashInCashOutStartingRow = operatingCashInRow;
            var operatingCashOutRow = cashInCashOutStartingRow;

            var isCOGSWritten = false;
            var isMarketingExpenseWritten = false;
            var isSalesCost = false;
            var isGeneralAdministrativeExpense = false;
            var isGeneralAdministrationCost = false;
            var isOtherOperatingExpense = false;

            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial; layoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isCOGSWritten)
                {
                    isCOGSWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "HPP/Biaya Produksi:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                if (!isMarketingExpenseWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                {
                    isMarketingExpenseWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = " ";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                if (!isSalesCost && layoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost)
                {
                    isSalesCost = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Biaya Penjualan:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                if (!isGeneralAdministrativeExpense && layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation)
                {
                    isGeneralAdministrativeExpense = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Biaya Administrasi & Umum:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                if (!isGeneralAdministrationCost && layoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost)
                {
                    isGeneralAdministrationCost = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Biaya umum dan administrasi";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                if (!isOtherOperatingExpense && layoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                {
                    isOtherOperatingExpense = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Biaya Operasional Lain-lain:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    operatingActivitiesRow += 1;
                    operatingCashOutRow += 1;
                }
            }
            writeableColumn = 3;

            var oacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (oacoTotalCurrencyIds.Count > 1)
                oacoTotalCurrencyIds = oacoTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total Pengeluaran Biaya Operasional";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in oacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                operatingActivitiesRow += 1;
                operatingCashOutRow += 1;
            }


            var oadiffCurrencyIds = oaciTotalCurrencyIds.Concat(oacoTotalCurrencyIds).Distinct().ToList();
            if (oadiffCurrencyIds.Count > 1)
                oadiffCurrencyIds = oadiffCurrencyIds.Where(element => element > 0).ToList();

            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, operatingCashOutRow, "CASH OUT", worksheet);
            SetActivitiesSummary(startingRow: operatingCashOutRow, endingRow: operatingCashOutRow + oadiffCurrencyIds.Count, remark: "Surplus/Deficit- Kas dari kegiatan Operasional", worksheet: worksheet);
            cashInCashOutStartingRow = operatingCashOutRow + oadiffCurrencyIds.Count;
            var investingCashInRow = cashInCashOutStartingRow;

            foreach (var currencyId in oadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInCurrencyNominal - cashOutCurrencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInNominal - cashOutNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = diffActualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                operatingActivitiesRow += 1;
            }

            SetLeftRemarkActivities(startingLeftRemarkRow, operatingActivitiesRow, "OPERASIONAL ACTIVITIES", worksheet);
            var investingActivitiesRow = operatingActivitiesRow;
            startingLeftRemarkRow = operatingActivitiesRow;

            var isEmptyWritten = false;
            // iaci
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInDeposit; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isEmptyWritten)
                {
                    isEmptyWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Penerimaan dari Investasi:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    investingActivitiesRow += 1;
                    investingCashInRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    investingActivitiesRow += 1;
                    investingCashInRow += 1;
                }
            }
            writeableColumn = 3;

            // iaci total
            var iaciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInDeposit && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iaciTotalCurrencyIds.Count > 1)
                iaciTotalCurrencyIds = iaciTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total Penerimaan Investasi";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in iaciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                investingActivitiesRow += 1;
                investingCashInRow += 1;
            }

            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, investingCashInRow, "CASH IN", worksheet);
            cashInCashOutStartingRow = investingCashInRow;
            var investingCashOutRow = cashInCashOutStartingRow;

            var isAssetPurchaseWritten = false;

            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.MachineryPurchase; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isAssetPurchaseWritten)
                {
                    isAssetPurchaseWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pembayaran pembelian asset tetap:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    investingActivitiesRow += 1;
                    investingCashOutRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                if (layoutOrder.ToDescriptionString() == "Deposito")
                {
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                }
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    investingActivitiesRow += 1;
                    investingCashOutRow += 1;
                }
            }
            writeableColumn = 3;

            var iacoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (iacoTotalCurrencyIds.Count > 1)
                iacoTotalCurrencyIds = iacoTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total Pengeluaran Investasi";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in iacoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                investingActivitiesRow += 1;
                investingCashOutRow += 1;
            }

            var iadiffCurrencyIds = iaciTotalCurrencyIds.Concat(iacoTotalCurrencyIds).Distinct().ToList();
            if (iadiffCurrencyIds.Count > 1)
                iadiffCurrencyIds = iadiffCurrencyIds.Where(element => element > 0).ToList();

            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, investingCashOutRow, "CASH OUT", worksheet);
            SetActivitiesSummary(investingCashOutRow, investingCashOutRow + iadiffCurrencyIds.Count, "Surplus/Deficit-Kas dalam kegiatan Investasi", worksheet);
            cashInCashOutStartingRow = investingCashOutRow + iadiffCurrencyIds.Count;
            var financingCashInRow = cashInCashOutStartingRow;

            foreach (var currencyId in iadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInCurrencyNominal - cashOutCurrencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInNominal - cashOutNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = diffActualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                investingActivitiesRow += 1;
            }

            SetLeftRemarkActivities(startingLeftRemarkRow, investingActivitiesRow, "INVESTING ACTIVITIES", worksheet);
            var financingActivitiesRow = investingActivitiesRow;
            startingLeftRemarkRow = investingActivitiesRow;

            isEmptyWritten = false;
            var isOthersWritten = false;
            // faci
            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers; layoutOrder++)
            {

                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isEmptyWritten)
                {
                    isEmptyWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Penerimaan dari Pendanaan:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashInRow += 1;
                }

                if (!isOthersWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates)
                {
                    isOthersWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Penerimaan lain-lain dari pendanaan:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashInRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                if (layoutOrder.ToDescriptionString() == "Pencairan pinjaman (Loan Withdrawal)")
                {
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                }
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashInRow += 1;
                }
            }
            writeableColumn = 3;

            // iaci total
            var faciTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (faciTotalCurrencyIds.Count > 1)
                faciTotalCurrencyIds = faciTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total Penerimaan Pendanaan";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in faciTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                financingActivitiesRow += 1;
                financingCashInRow += 1;
            }

            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, financingCashInRow, "CASH IN", worksheet);
            cashInCashOutStartingRow = financingCashInRow;
            var financingCashOutRow = cashInCashOutStartingRow;

            var isLoanInstallmentWritten = false;
            var isBankExpensesWritten = false;
            var isCashOutOthersWritten = false;

            for (var layoutOrder = BudgetCashflowCategoryLayoutOrder.CashOutInstallments; layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers; layoutOrder++)
            {
                var selectedData = rowData.Where(element => element.LayoutOrder == layoutOrder);

                writeableColumn = 3;

                if (!isLoanInstallmentWritten)
                {
                    isLoanInstallmentWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pembayaran angsuran dan bunga Pinjaman:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashOutRow += 1;
                }

                if (!isBankExpensesWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutBankAdministrationFee)
                {
                    isBankExpensesWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pembayaran Biaya Administrasi Bank:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashOutRow += 1;
                }

                if (!isCashOutOthersWritten && layoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutAffiliates)
                {
                    isCashOutOthersWritten = true;
                    worksheet.Cells[writeableRow, writeableColumn].Value = "Pengeluaran lain-lain dari Pendanaan:";
                    worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Merge = true;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[writeableRow, writeableColumn, writeableRow, lastColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashOutRow += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = layoutOrder.ToDescriptionString();
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

                var currencyIds = selectedData.Select(item => item.CurrencyId).Distinct().ToList();
                if (currencyIds.Count > 1)
                    currencyIds = currencyIds.Where(element => element > 0).ToList();

                foreach (var currencyId in currencyIds)
                {
                    var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                    if (currency == null)
                        currency = new CurrencyDto();

                    worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

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
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.CurrencyNominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            nominalDivision += data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Value = data.Nominal;
                            worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            writeableColumn += 1;

                            actual += data.ActualNominal;
                        }

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominalDivision;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = actual;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // reset
                    writeableColumn = 4;
                    writeableRow += 1;

                    financingActivitiesRow += 1;
                    financingCashOutRow += 1;
                }
            }
            writeableColumn = 3;

            var facoTotalCurrencyIds = rowData
                .Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers)
                .Select(element => element.CurrencyId)
                .Distinct()
                .ToList();

            if (facoTotalCurrencyIds.Count > 1)
                facoTotalCurrencyIds = facoTotalCurrencyIds.Where(element => element > 0).ToList();

            worksheet.Cells[writeableRow, writeableColumn].Value = "Total pengeluaran pendanaan";
            worksheet.Cells[writeableRow, writeableColumn].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            writeableColumn += 1;

            foreach (var currencyId in facoTotalCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = currencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = nominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = actualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                financingActivitiesRow += 1;
                financingCashOutRow += 1;
            }

            var fadiffCurrencyIds = faciTotalCurrencyIds.Concat(facoTotalCurrencyIds).Distinct().ToList();
            if (fadiffCurrencyIds.Count > 1)
                fadiffCurrencyIds = fadiffCurrencyIds.Where(element => element > 0).ToList();

            SetLeftCashInCashOutRemark(cashInCashOutStartingRow, financingCashOutRow, "CASH OUT", worksheet);
            SetActivitiesSummary(financingCashOutRow, financingCashOutRow + fadiffCurrencyIds.Count, "Surplus/Deficit-Kas dalam kegiatan Pendanaan", worksheet);

            foreach (var currencyId in fadiffCurrencyIds)
            {
                var currency = _currencies.FirstOrDefault(element => element.Id == currencyId);
                if (currency == null)
                    currency = new CurrencyDto();

                worksheet.Cells[writeableRow, writeableColumn].Value = currency.Code;
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableColumn += 1;

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

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInCurrencyNominal - cashOutCurrencyNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;

                        worksheet.Cells[writeableRow, writeableColumn].Value = cashInNominal - cashOutNominal;
                        worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        writeableColumn += 1;
                    }

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffCurrencyNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;

                    worksheet.Cells[writeableRow, writeableColumn].Value = divisionDiffNominal;
                    worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableColumn += 1;
                }

                worksheet.Cells[writeableRow, writeableColumn].Value = diffActualNominal;
                worksheet.Cells[writeableRow, writeableColumn].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[writeableRow, writeableColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[writeableRow, writeableColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // reset
                writeableColumn = 4;
                writeableRow += 1;

                financingActivitiesRow += 1;
            }

            SetLeftRemarkActivities(startingLeftRemarkRow, financingActivitiesRow, "FINANCING ACTIVITIES", worksheet);
            SetFooter(startingRow: financingActivitiesRow, worksheet: worksheet);
        }

        private void SetFooter(int startingRow, ExcelWorksheet worksheet)
        {
            worksheet.Cells[$"A{startingRow}"].Value = "Saldo Awal Kas";
            worksheet.Cells[$"A{startingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{startingRow}:C{startingRow}"].Merge = true;
            worksheet.Cells[$"A{startingRow}:C{startingRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{startingRow}:C{startingRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"A{startingRow + 1}"].Value = "TOTAL SURPLUS/DEFISIT KAS";
            worksheet.Cells[$"A{startingRow + 1}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{startingRow + 1}:C{startingRow + 1}"].Merge = true;
            worksheet.Cells[$"A{startingRow + 1}:C{startingRow + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{startingRow + 1}:C{startingRow + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"A{startingRow + 2}"].Value = "Saldo Akhir Kas";
            worksheet.Cells[$"A{startingRow + 2}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{startingRow + 2}:C{startingRow + 2}"].Merge = true;
            worksheet.Cells[$"A{startingRow + 2}:C{startingRow + 2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{startingRow + 2}:C{startingRow + 2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{startingRow + 3}"].Value = "Saldo Real Kas";
            worksheet.Cells[$"C{startingRow + 3}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{startingRow + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{startingRow + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{startingRow + 4}"].Value = "Selisih";
            worksheet.Cells[$"C{startingRow + 4}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{startingRow + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{startingRow + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{startingRow + 5}"].Value = "Rate";
            worksheet.Cells[$"C{startingRow + 5}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{startingRow + 5}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{startingRow + 5}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"B{startingRow + 6}"].Value = "TOTAL SURPLUS (DEFISIT) EQUIVALENT";
            worksheet.Cells[$"B{startingRow + 6}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{startingRow + 6}:C{startingRow + 6}"].Merge = true;
            worksheet.Cells[$"B{startingRow + 6}:C{startingRow + 6}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"B{startingRow + 6}:C{startingRow + 6}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        private void SetActivitiesSummary(int startingRow, int endingRow, string remark, ExcelWorksheet worksheet)
        {
            worksheet.Cells[startingRow, 2].Value = remark;
            worksheet.Cells[startingRow, 2].Style.Font.Bold = true;
            worksheet.Cells[startingRow, 2, endingRow - 1, 3].Merge = true;
            worksheet.Cells[startingRow, 2, endingRow - 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[startingRow, 2, endingRow - 1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        private void SetLeftCashInCashOutRemark(int cashInCashOutStartingRow, int cashInCashOutRow, string remark, ExcelWorksheet worksheet)
        {
            worksheet.Cells[cashInCashOutStartingRow, 2].Value = remark;
            worksheet.Cells[cashInCashOutStartingRow, 2].Style.Font.Bold = true;
            worksheet.Cells[cashInCashOutStartingRow, 2, cashInCashOutRow - 1, 2].Merge = true;
            worksheet.Cells[cashInCashOutStartingRow, 2, cashInCashOutRow - 1, 2].Style.TextRotation = 90;
            worksheet.Cells[cashInCashOutStartingRow, 2, cashInCashOutRow - 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInCashOutStartingRow, 2, cashInCashOutRow - 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        private void SetLeftRemarkActivities(int startingLeftRemarkRow, int activitiesRow, string remark, ExcelWorksheet worksheet)
        {
            worksheet.Cells[startingLeftRemarkRow, 1].Value = remark;
            worksheet.Cells[startingLeftRemarkRow, 1].Style.Font.Bold = true;
            worksheet.Cells[startingLeftRemarkRow, 1, activitiesRow - 1, 1].Merge = true;
            worksheet.Cells[startingLeftRemarkRow, 1, activitiesRow - 1, 1].Style.TextRotation = 90;
            worksheet.Cells[startingLeftRemarkRow, 1, activitiesRow - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[startingLeftRemarkRow, 1, activitiesRow - 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        private void SetLeftRemark(ExcelWorksheet worksheet, List<BudgetCashflowDivisionItemDto> rowData)
        {
            var oaciRowData = rowData.Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.ExportSales && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation).ToList();
            var oaciCount = oaciRowData.Count;
            var oaciTotalCount = oaciRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? oaciRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : oaciRowData.Select(element => element.CurrencyId).Distinct().Count();
            var oacoRowData = rowData.Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.LocalRawMaterial && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost).ToList();
            var oacoCount = oacoRowData.Count;
            var oacoTotalCount = oacoRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? oacoRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : oacoRowData.Select(element => element.CurrencyId).Distinct().Count();
            var oadiffTotalCount = oaciRowData.Concat(oacoRowData).Select(element => element.CurrencyId).Distinct().Count() > 1 ? oaciRowData.Concat(oacoRowData).Select(element => element.CurrencyId).Distinct().Where(element => element > 0).Count() : oaciRowData.Concat(oacoRowData).Select(element => element.CurrencyId).Distinct().Count();

            var iaciRowData = rowData.Where(element => element.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashInDeposit || element.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashInOthers).ToList();
            var iaciCount = iaciRowData.Count;
            var iaciTotalCount = iaciRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? iaciRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : iaciRowData.Select(element => element.CurrencyId).Distinct().Count();
            var iacoRowData = rowData.Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit).ToList();
            var iacoCount = iacoRowData.Count;
            var iacoTotalCount = iacoRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? iacoRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : iacoRowData.Select(element => element.CurrencyId).Distinct().Count();
            var iadiffTotalCount = iaciRowData.Concat(iacoRowData).Select(element => element.CurrencyId).Distinct().Count() > 1 ? iaciRowData.Concat(iacoRowData).Select(element => element.CurrencyId).Distinct().Where(element => element > 0).Count() : iaciRowData.Concat(iacoRowData).Select(element => element.CurrencyId).Distinct().Count();

            var faciRowData = rowData.Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers).ToList();
            var faciCount = faciRowData.Count;
            var faciTotalCount = faciRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? faciRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : faciRowData.Select(element => element.CurrencyId).Distinct().Count();
            var facoRowData = rowData.Where(element => element.LayoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && element.LayoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers).ToList();
            var facoCount = facoRowData.Count;
            var facoTotalCount = facoRowData.Select(element => element.CurrencyId).Distinct().Count() > 1 ? facoRowData.Select(element => element.CurrencyId).Where(element => element > 0).Distinct().Count() : facoRowData.Select(element => element.CurrencyId).Distinct().Count();
            var fadiffTotalCount = faciRowData.Concat(facoRowData).Select(element => element.CurrencyId).Distinct().Count() > 1 ? faciRowData.Concat(facoRowData).Select(element => element.CurrencyId).Distinct().Where(element => element > 0).Count() : faciRowData.Concat(facoRowData).Select(element => element.CurrencyId).Distinct().Count();

            var writeableRow = 6;
            var startingRow = 6;
            var writeableCol = 1;
            var cashInCashOutWriteableCol = 2;

            var operatingActivitiesCount = oaciCount + 2 + oaciTotalCount + 6 + oacoCount + oacoTotalCount + oadiffTotalCount;
            var operatingActivitiesCashInCount = oaciCount + 2 + oaciTotalCount;
            var operatingActivitiesCashOutCount = oacoCount + 6 + oacoTotalCount;
            var investingActivitiesCount = iaciCount + 1 + iaciTotalCount + 1 + iacoCount + iacoTotalCount + iadiffTotalCount;
            var investingActivitiesCashInCount = iaciCount + 1 + iaciTotalCount;
            var investingActivitiesCashOutCount = iacoCount + 1 + iacoTotalCount;
            var financingActivitiesCount = faciCount + 2 + faciTotalCount + 3 + facoCount + facoTotalCount + fadiffTotalCount;
            var financingActivitiesCashInCount = faciCount + 2 + faciTotalCount;
            var financingActivitiesCashOutCount = 3 + facoCount + facoTotalCount;

            worksheet.Cells[writeableRow, writeableCol].Value = "OPERATING ACTIVITIES";
            worksheet.Cells[writeableRow, writeableCol].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + operatingActivitiesCount - 1, writeableCol].Merge = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + operatingActivitiesCount - 1, writeableCol].Style.TextRotation = 90;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + operatingActivitiesCount - 1, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + operatingActivitiesCount - 1, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[writeableRow, cashInCashOutWriteableCol].Value = "CASH IN";
            worksheet.Cells[writeableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, cashInCashOutWriteableCol, writeableRow + operatingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[writeableRow, cashInCashOutWriteableCol, writeableRow + operatingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[writeableRow, cashInCashOutWriteableCol, writeableRow + operatingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[writeableRow, cashInCashOutWriteableCol, writeableRow + operatingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var cashInOutWriteableRow = startingRow + operatingActivitiesCashInCount;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Value = "CASH OUT";
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + operatingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + operatingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + operatingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + operatingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var diffWriteableRow = startingRow + operatingActivitiesCashInCount + operatingActivitiesCashOutCount;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Value = "Surplus/Deficit- Kas dari kegiatan Operasional";
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + oadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Merge = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + oadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + oadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += operatingActivitiesCount;
            worksheet.Cells[writeableRow, writeableCol].Value = "INVESTING ACTIVITIES";
            worksheet.Cells[writeableRow, writeableCol].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + investingActivitiesCount - 1, writeableCol].Merge = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + investingActivitiesCount - 1, writeableCol].Style.TextRotation = 90;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + investingActivitiesCount - 1, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + investingActivitiesCount - 1, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            cashInOutWriteableRow = startingRow + operatingActivitiesCount;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Value = "CASH IN";
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            cashInOutWriteableRow = startingRow + operatingActivitiesCount + investingActivitiesCashInCount;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Value = "CASH OUT";
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + investingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            diffWriteableRow = startingRow + operatingActivitiesCount + investingActivitiesCashInCount + investingActivitiesCashOutCount;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Value = "Surplus/Deficit-Kas dalam kegiatan Investasi";
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + iadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Merge = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + iadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + iadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += investingActivitiesCount;
            worksheet.Cells[writeableRow, writeableCol].Value = "FINANCING ACTIVITIES";
            worksheet.Cells[writeableRow, writeableCol].Style.Font.Bold = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + financingActivitiesCount - 1, writeableCol].Merge = true;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + financingActivitiesCount - 1, writeableCol].Style.TextRotation = 90;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + financingActivitiesCount - 1, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[writeableRow, writeableCol, writeableRow + financingActivitiesCount - 1, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            cashInOutWriteableRow = startingRow + operatingActivitiesCount + investingActivitiesCount;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Value = "CASH IN";
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashInCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            cashInOutWriteableRow = startingRow + operatingActivitiesCount + investingActivitiesCount + financingActivitiesCashInCount;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Value = "CASH OUT";
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Merge = true;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.TextRotation = 90;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cashInOutWriteableRow, cashInCashOutWriteableCol, cashInOutWriteableRow + financingActivitiesCashOutCount - 1, cashInCashOutWriteableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            diffWriteableRow = startingRow + operatingActivitiesCount + investingActivitiesCount + financingActivitiesCashInCount + financingActivitiesCashOutCount;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Value = "Surplus/Deficit-Kas dalam kegiatan Pendanaan";
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol].Style.Font.Bold = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + fadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Merge = true;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + fadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[diffWriteableRow, cashInCashOutWriteableCol, diffWriteableRow + fadiffTotalCount - 1, cashInCashOutWriteableCol + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        private void SetTableHeader(ExcelWorksheet worksheet, List<int> unitIds, List<int> divisionIds)
        {
            var writeableCol = 1;
            var headerRow = 5;

            worksheet.Cells[headerRow, writeableCol].Value = "KETERANGAN";
            worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
            worksheet.Cells[headerRow, writeableCol, headerRow, writeableCol + 2].Style.Font.Bold = true;
            worksheet.Cells[headerRow, writeableCol, headerRow, writeableCol + 2].Merge = true;
            worksheet.Cells[headerRow, writeableCol, headerRow, writeableCol + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[headerRow, writeableCol, headerRow, writeableCol + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableCol += 3;
            worksheet.Cells[headerRow, writeableCol].Value = "MATA UANG";
            worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
            worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
            worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableCol += 1;
            _selectedUnits = _units.Where(unit => unitIds.Contains(unit.Id)).ToList();
            _selectedDivisions = _divisions.Where(division => divisionIds.Contains(division.Id)).ToList();

            foreach (var division in _selectedDivisions)
            {
                var units = _selectedUnits.Where(unit => unit.DivisionId == division.Id);
                foreach (var unit in units)
                {
                    worksheet.Cells[headerRow, writeableCol].Value = $"{unit.Name} VALAS";
                    worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
                    worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
                    worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableCol += 1;

                    worksheet.Cells[headerRow, writeableCol].Value = $"{unit.Name} IDR";
                    worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
                    worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
                    worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableCol += 1;
                }

                worksheet.Cells[headerRow, writeableCol].Value = $"DIVISI {division.Name} VALAS";
                worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
                worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
                worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableCol += 1;

                worksheet.Cells[headerRow, writeableCol].Value = $"DIVISI {division.Name} IDR";
                worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
                worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
                worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                writeableCol += 1;
            }

            worksheet.Cells[headerRow, writeableCol].Value = "ACTUAL";
            worksheet.Cells[headerRow, writeableCol].Style.Font.Size = 14;
            worksheet.Cells[headerRow, writeableCol].Style.Font.Bold = true;
            worksheet.Cells[headerRow, writeableCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[headerRow, writeableCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        private void SetTitle(ExcelWorksheet worksheet, int divisionId, DateTimeOffset dueDate, int lastColumn)
        {
            var company = "PT EFRATA GARMINDO UTAMA";
            var title = "LAPORAN BUDGET CASH FLOW";
            var divisionName = "SEMUA DIVISI";

            var division = _divisions.FirstOrDefault(element => element.Id == divisionId);
            if (division != null)
                divisionName = $"DIVISI: {division.Name}";

            var date = $"JATUH TEMPO s.d. {dueDate.AddMonths(1).AddHours(_identityService.TimezoneOffset).DateTime.ToString("MMMM yyyy", new CultureInfo("id-ID"))}";

            worksheet.Cells[1, 1].Value = company;
            worksheet.Cells[1, 1].Style.Font.Size = 20;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, lastColumn].Merge = true;

            worksheet.Cells[2, 1].Value = title;
            worksheet.Cells[2, 1].Style.Font.Size = 20;
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1, 2, lastColumn].Merge = true;

            worksheet.Cells[3, 1].Value = divisionName;
            worksheet.Cells[3, 1].Style.Font.Size = 20;
            worksheet.Cells[3, 1].Style.Font.Bold = true;
            worksheet.Cells[3, 1, 3, lastColumn].Merge = true;

            worksheet.Cells[4, 1].Value = date;
            worksheet.Cells[4, 1].Style.Font.Size = 20;
            worksheet.Cells[4, 1].Style.Font.Bold = true;
            worksheet.Cells[4, 1, 4, lastColumn].Merge = true;
        }
    }
}
