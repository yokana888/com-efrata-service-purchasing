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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.ExcelGenerator
{
    public class BudgetCashflowUnitExcelGenerator : IBudgetCashflowUnitExcelGenerator
    {
        private readonly IBudgetCashflowService _budgetCashflowService;
        private readonly IdentityService _identityService;
        private readonly List<UnitDto> _units;
        private readonly List<CurrencyDto> _currencies;

        public BudgetCashflowUnitExcelGenerator(IServiceProvider serviceProvider)
        {
            _budgetCashflowService = serviceProvider.GetService<IBudgetCashflowService>();
            _identityService = serviceProvider.GetService<IdentityService>();

            var cache = serviceProvider.GetService<IDistributedCache>();

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

        private List<BudgetCashflowItemDto> GetOperatingActivitiesCashIn(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder <= BudgetCashflowCategoryLayoutOrder.ExternalIncomeVATCalculation)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetOperatingActivitiesCashInTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashInOperatingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.CurrencyId).ToList();
        }

        private List<BudgetCashflowItemDto> GetOperatingActivitiesCashOut(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder < BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial)
                    continue;
                else if (layoutOrder >= BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial && layoutOrder <= BudgetCashflowCategoryLayoutOrder.OthersOperationalCost)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetOperatingActivitiesCashOutTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashOutOperatingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.CurrencyId).ToList();
        }

        private List<BudgetCashflowItemDto> GetOperatingActivitiesDifference(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetDiffOperatingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.CurrencyId).ToList();
        }

        private List<BudgetCashflowItemDto> GetInvestingActivitiesCashIn(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInDeposit)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else if (layoutOrder == BudgetCashflowCategoryLayoutOrder.CashInOthers)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else if (layoutOrder < BudgetCashflowCategoryLayoutOrder.CashInDeposit)
                    continue;
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetInvestingActivitiesCashInTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashInInvestingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetInvestingActivitiesCashOut(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder < BudgetCashflowCategoryLayoutOrder.MachineryPurchase)
                    continue;
                else if (layoutOrder >= BudgetCashflowCategoryLayoutOrder.MachineryPurchase && layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutDeposit)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetInvestingActivitiesCashOutTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashOutInvestingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetInvestingActivitiesDifference(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetDiffInvestingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetFinancingActivitiesCashIn(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder < BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal)
                    continue;
                else if (layoutOrder >= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawal && layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashInLoanWithdrawalOthers)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetFinancingActivitiesCashInTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashInFinancingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetFinancingActivitiesCashOut(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            foreach (BudgetCashflowCategoryLayoutOrder layoutOrder in Enum.GetValues(typeof(BudgetCashflowCategoryLayoutOrder)))
            {
                if (layoutOrder < BudgetCashflowCategoryLayoutOrder.CashOutInstallments)
                    continue;
                else if (layoutOrder >= BudgetCashflowCategoryLayoutOrder.CashOutInstallments && layoutOrder <= BudgetCashflowCategoryLayoutOrder.CashOutOthers)
                    result.AddRange(_budgetCashflowService.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate));
                else
                    break;
            }

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetFinancingActivitiesCashOutTotal(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetCashOutFinancingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetFinancingActivitiesDifference(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetDiffFinancingActivitiesByUnit(unitId, dueDate));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        private List<BudgetCashflowItemDto> GetRowDataWorstCase(int unitId, DateTimeOffset dueDate)
        {
            var result = new List<BudgetCashflowItemDto>();
            result.AddRange(_budgetCashflowService.GetBudgetCashflowWorstCase(dueDate, unitId));

            return result.OrderBy(element => element.LayoutOrder).ToList();
        }

        public MemoryStream Generate(int unitId, DateTimeOffset dueDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                var unit = _units.FirstOrDefault(element => element.Id == unitId);

                SetTitle(worksheet, unit, dueDate);
                SetBestCaseWorstCaseMark(worksheet);
                SetTableHeader(worksheet, unit);

                worksheet.Cells["A5:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // oaci = operating activities cash in
                var oaci = GetOperatingActivitiesCashIn(unitId, dueDate);
                // oaciTotal = operating activities cash in total
                var oaciTotal = GetOperatingActivitiesCashInTotal(unitId, dueDate);
                // oaco = operating activities cash out
                var oaco = GetOperatingActivitiesCashOut(unitId, dueDate);
                // oacoTotal = operating activities cash out total
                var oacoTotal = GetOperatingActivitiesCashOutTotal(unitId, dueDate);
                // oadiff = opearting activities difference
                var oadiff = GetOperatingActivitiesDifference(unitId, dueDate);
                // iaci = investing activities cash in
                var iaci = GetInvestingActivitiesCashIn(unitId, dueDate);
                // iaciTotal = investing activities cash in total
                var iaciTotal = GetInvestingActivitiesCashInTotal(unitId, dueDate);
                // iaco = investing activities cash out
                var iaco = GetInvestingActivitiesCashOut(unitId, dueDate);
                // iacoTotal = investing activities cash out total
                var iacoTotal = GetInvestingActivitiesCashOutTotal(unitId, dueDate);
                // iadiff = investing activities difference
                var iadiff = GetInvestingActivitiesDifference(unitId, dueDate);
                // faci = financing activities cash in
                var faci = GetFinancingActivitiesCashIn(unitId, dueDate);
                // faciTotal = financing activities cash in total
                var faciTotal = GetFinancingActivitiesCashInTotal(unitId, dueDate);
                // faco = financing activities cash out
                var faco = GetFinancingActivitiesCashOut(unitId, dueDate);
                // fatotal = financing activities cash out total
                var facoTotal = GetFinancingActivitiesCashOutTotal(unitId, dueDate);
                // fadiff = financing activities difference
                var fadiff = GetFinancingActivitiesDifference(unitId, dueDate);

                var worstCases = GetRowDataWorstCase(unitId, dueDate);

                SetLeftRemarkColumn(oaci.Count, oaciTotal.Count, oaco.Count, oacoTotal.Count, oadiff.Count, iaci.Count, iaciTotal.Count, iaco.Count, iacoTotal.Count, iadiff.Count, faci.Count, faciTotal.Count, faco.Count, facoTotal.Count, fadiff.Count, worksheet);
                SetData(oaci, oaciTotal, oaco, oacoTotal, oadiff, iaci, iaciTotal, iaco, iacoTotal, iadiff, faci, faciTotal, faco, facoTotal, fadiff, worstCases, worksheet);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private void SetData(List<BudgetCashflowItemDto> oaci, List<BudgetCashflowItemDto> oaciTotal, List<BudgetCashflowItemDto> oaco, List<BudgetCashflowItemDto> oacoTotal, List<BudgetCashflowItemDto> oadiff, List<BudgetCashflowItemDto> iaci, List<BudgetCashflowItemDto> iaciTotal, List<BudgetCashflowItemDto> iaco, List<BudgetCashflowItemDto> iacoTotal, List<BudgetCashflowItemDto> iadiff, List<BudgetCashflowItemDto> faci, List<BudgetCashflowItemDto> faciTotal, List<BudgetCashflowItemDto> faco, List<BudgetCashflowItemDto> facoTotal, List<BudgetCashflowItemDto> fadiff, List<BudgetCashflowItemDto> worstCases, ExcelWorksheet worksheet)
        {
            var startingRow = 8;

            worksheet.Cells[$"C{startingRow}"].Value = "Pendapatan Operasional:";
            worksheet.Cells[$"C{startingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{startingRow}:K{startingRow}"].Merge = true;
            worksheet.Cells[$"C{startingRow}:K{startingRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{startingRow}:K{startingRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var writeableRow = startingRow + 1;
            var isRevenueFromOtherWritten = false;
            foreach (var item in oaci)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales && !isRevenueFromOtherWritten)
                {
                    isRevenueFromOtherWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Pendapatan Operasional Lain-lain:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total Penerimaan Operasional";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in oaciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "HPP/Biaya Produksi:";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += 1;

            var isMarketingExpenseWritten = false;
            var isSalesCostWritten = false;
            var isGeneralAdministrativeExpenseWritten = false;
            var isGeneralCostAdministrativeWritten = false;
            var isOtherOperatingExpenseWritten = false;

            foreach (var item in oaco)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost && !isMarketingExpenseWritten)
                {
                    isMarketingExpenseWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = " ";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost && !isSalesCostWritten)
                {
                    isSalesCostWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Biaya Penjualan:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation && !isGeneralAdministrativeExpenseWritten)
                {
                    isGeneralAdministrativeExpenseWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Biaya Administrasi & Umum:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost && !isGeneralCostAdministrativeWritten)
                {
                    isGeneralCostAdministrativeWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Biaya umum dan administrasi";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && !isOtherOperatingExpenseWritten)
                {
                    isOtherOperatingExpenseWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Biaya Operasional Lain-lain:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total Pengeluaran Biaya Operasional";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in oacoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            foreach (var item in oadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Penerimaan dari Investasi:";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += 1;

            foreach (var item in iaci)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total Penerimaan Investasi";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in iaciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Pembayaran pembelian asset tetap:";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += 1;

            foreach (var item in iaco)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                if (item.LayoutOrder.ToDescriptionString() == "Deposito")
                {
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                }
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total Pengeluaran Investasi";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in iacoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            foreach (var item in iadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Penerimaan dari Pendanaan:";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += 1;

            var isCashInAffiliates = false;
            foreach (var item in faci)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates && !isCashInAffiliates)
                {
                    isCashInAffiliates = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Penerimaan lain-lain dari pendanaan:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                if (item.LayoutOrder.ToDescriptionString() == "Pencairan pinjaman (Loan Withdrawal)")
                {
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                }
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total Penerimaan Pendanaan";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in faciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Pembayaran angsuran dan bunga Pinjaman:";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            writeableRow += 1;

            var isBankExpenseWritten = false;
            var isOthersWritten = false;
            foreach (var item in faco)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutBankAdministrationFee && !isBankExpenseWritten)
                {
                    isBankExpenseWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Pembayaran Biaya Administrasi Bank:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutAffiliates && !isOthersWritten)
                {
                    isOthersWritten = true;
                    worksheet.Cells[$"C{writeableRow}"].Value = "Pengeluaran lain-lain dari Pendanaan:";
                    worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Merge = true;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{writeableRow}:K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    writeableRow += 1;
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"C{writeableRow}"].Value = item.LayoutOrder.ToDescriptionString();
                worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = worstCase.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = worstCase.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = worstCase.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            worksheet.Cells[$"C{writeableRow}"].Value = "Total pengeluaran pendanaan";
            worksheet.Cells[$"C{writeableRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{writeableRow}"].Merge = true;
            worksheet.Cells[$"C{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"C{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            foreach (var item in facoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }

            foreach (var item in fadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                worksheet.Cells[$"D{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"D{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"D{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"E{writeableRow}"].Value = item.BestCaseCurrencyNominal;
                worksheet.Cells[$"E{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"E{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"E{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"F{writeableRow}"].Value = item.BestCaseNominal;
                worksheet.Cells[$"F{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"F{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"F{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"G{writeableRow}"].Value = item.BestCaseActualNominal;
                worksheet.Cells[$"G{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"G{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"G{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"H{writeableRow}"].Value = currencyCode;
                worksheet.Cells[$"H{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"I{writeableRow}"].Value = item.CurrencyNominal;
                worksheet.Cells[$"I{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"I{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"I{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"J{writeableRow}"].Value = item.Nominal;
                worksheet.Cells[$"J{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"J{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"J{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[$"K{writeableRow}"].Value = item.ActualNominal;
                worksheet.Cells[$"K{writeableRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{writeableRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"K{writeableRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                writeableRow += 1;
            }
        }

        private void SetLeftRemarkColumn(int oaciCount, int oaciTotalCount, int oacoCount, int oacoTotalCount, int oadiffCount, int iaciCount, int iaciTotalCount, int iacoCount, int iacoTotalCount, int iadiffCount, int faciCount, int faciTotalCount, int facoCount, int facoTotalCount, int fadiffCount, ExcelWorksheet worksheet)
        {
            var operatingActivitiesCashInRowCount = 2 + oaciCount + oaciTotalCount;
            var operatingActivitiesCashOutRowCount = 6 + oacoCount + oacoTotalCount;
            var operatingActivitiesRowsCount = 2 + oaciCount + oaciTotalCount + 6 + oacoCount + oacoTotalCount + oadiffCount;
            var investingActivitiesCashInRowCount = 1 + iaciCount + iaciTotalCount;
            var investingActivitiesCashOutRowCount = 1 + iacoCount + iacoTotalCount;
            var investingActivitiesRowsCount = 1 + iaciCount + iaciTotalCount + 1 + iacoCount + iacoTotalCount + iadiffCount;
            var financingActivitiesCashInRowsCount = 2 + faciCount + faciTotalCount;
            var financingActivitiesCashOutRowsCount = 3 + facoCount + facoTotalCount;
            var financingActivitiesRowsCount = 1 + faciCount + faciTotalCount + 3 + facoCount + facoTotalCount + fadiffCount;

            var operatingActivitiesStartingRow = 8;
            worksheet.Cells[$"A{operatingActivitiesStartingRow}"].Value = "OPERATING ACTIVITIES";
            worksheet.Cells[$"A{operatingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{operatingActivitiesStartingRow}:A{operatingActivitiesStartingRow + operatingActivitiesRowsCount - 1}"].Merge = true;
            worksheet.Cells[$"A{operatingActivitiesStartingRow}:A{operatingActivitiesStartingRow + operatingActivitiesRowsCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"A{operatingActivitiesStartingRow}:A{operatingActivitiesStartingRow + operatingActivitiesRowsCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"A{operatingActivitiesStartingRow}:A{operatingActivitiesStartingRow + operatingActivitiesRowsCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{operatingActivitiesStartingRow}"].Value = "CASH IN";
            worksheet.Cells[$"B{operatingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{operatingActivitiesStartingRow}:B{operatingActivitiesStartingRow + operatingActivitiesCashInRowCount - 1}"].Merge = true;
            worksheet.Cells[$"B{operatingActivitiesStartingRow}:B{operatingActivitiesStartingRow + operatingActivitiesCashInRowCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{operatingActivitiesStartingRow}:B{operatingActivitiesStartingRow + operatingActivitiesCashInRowCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{operatingActivitiesStartingRow}:B{operatingActivitiesStartingRow + operatingActivitiesCashInRowCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var operatingActivitiesCashOutStartingRow = operatingActivitiesStartingRow + operatingActivitiesCashInRowCount;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}"].Value = "CASH OUT";
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}:B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount - 1}"].Merge = true;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}:B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}:B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow}:B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount}"].Value = "Surplus/Deficit- Kas dari kegiatan Operasional";
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount}:C{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount + oadiffCount - 1}"].Merge = true;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount}:C{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount + oadiffCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"B{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount}:C{operatingActivitiesCashOutStartingRow + operatingActivitiesCashOutRowCount + oadiffCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var investingActivitiesStartingRow = operatingActivitiesStartingRow + operatingActivitiesRowsCount;
            worksheet.Cells[$"A{investingActivitiesStartingRow}"].Value = "INVESTING ACTIVITIES";
            worksheet.Cells[$"A{investingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{investingActivitiesStartingRow}:A{investingActivitiesStartingRow + investingActivitiesRowsCount - 1}"].Merge = true;
            worksheet.Cells[$"A{investingActivitiesStartingRow}:A{investingActivitiesStartingRow + investingActivitiesRowsCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"A{investingActivitiesStartingRow}:A{investingActivitiesStartingRow + investingActivitiesRowsCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"A{investingActivitiesStartingRow}:A{investingActivitiesStartingRow + investingActivitiesRowsCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{investingActivitiesStartingRow}"].Value = "CASH IN";
            worksheet.Cells[$"B{investingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{investingActivitiesStartingRow}:B{investingActivitiesStartingRow + investingActivitiesCashInRowCount - 1}"].Merge = true;
            worksheet.Cells[$"B{investingActivitiesStartingRow}:B{investingActivitiesStartingRow + investingActivitiesCashInRowCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{investingActivitiesStartingRow}:B{investingActivitiesStartingRow + investingActivitiesCashInRowCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{investingActivitiesStartingRow}:B{investingActivitiesStartingRow + investingActivitiesCashInRowCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var investingActivitiesCashOutStartingRow = investingActivitiesStartingRow + investingActivitiesCashInRowCount;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}"].Value = "CASH OUT";
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}:B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount - 1}"].Merge = true;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}:B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}:B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow}:B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount}"].Value = "Surplus/Deficit-Kas dalam kegiatan Investasi";
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount}:C{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount + iadiffCount - 1}"].Merge = true;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount}:C{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount + iadiffCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"B{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount}:C{investingActivitiesCashOutStartingRow + investingActivitiesCashOutRowCount + iadiffCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var financingActivitiesStartingRow = investingActivitiesStartingRow + investingActivitiesRowsCount;
            worksheet.Cells[$"A{financingActivitiesStartingRow}"].Value = "FINANCING ACTIVITIES";
            worksheet.Cells[$"A{financingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{financingActivitiesStartingRow}:A{financingActivitiesStartingRow + financingActivitiesRowsCount - 1}"].Merge = true;
            worksheet.Cells[$"A{financingActivitiesStartingRow}:A{financingActivitiesStartingRow + financingActivitiesRowsCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"A{financingActivitiesStartingRow}:A{financingActivitiesStartingRow + financingActivitiesRowsCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"A{financingActivitiesStartingRow}:A{financingActivitiesStartingRow + financingActivitiesRowsCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{financingActivitiesStartingRow}"].Value = "CASH IN";
            worksheet.Cells[$"B{financingActivitiesStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{financingActivitiesStartingRow}:B{financingActivitiesStartingRow + financingActivitiesCashInRowsCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{financingActivitiesStartingRow}:B{financingActivitiesStartingRow + financingActivitiesCashInRowsCount - 1}"].Merge = true;
            worksheet.Cells[$"B{financingActivitiesStartingRow}:B{financingActivitiesStartingRow + financingActivitiesCashInRowsCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{financingActivitiesStartingRow}:B{financingActivitiesStartingRow + financingActivitiesCashInRowsCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var financingActivitiesCashOutStartingRow = financingActivitiesStartingRow + financingActivitiesCashInRowsCount;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}"].Value = "CASH OUT";
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}:B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount - 1}"].Merge = true;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}:B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount - 1}"].Style.TextRotation = 90;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}:B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow}:B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount}"].Value = "Surplus/Deficit-Kas dalam kegiatan Pendanaan";
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount}:C{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount + fadiffCount - 1}"].Merge = true;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount}:C{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount + fadiffCount - 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"B{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount}:C{financingActivitiesCashOutStartingRow + financingActivitiesCashOutRowsCount + fadiffCount - 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var footerStartingRow = operatingActivitiesStartingRow + operatingActivitiesRowsCount + investingActivitiesRowsCount + financingActivitiesRowsCount;
            worksheet.Cells[$"A{footerStartingRow}"].Value = "Saldo Awal Kas";
            worksheet.Cells[$"A{footerStartingRow}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{footerStartingRow}:C{footerStartingRow}"].Merge = true;
            worksheet.Cells[$"A{footerStartingRow}:C{footerStartingRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{footerStartingRow}:C{footerStartingRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"A{footerStartingRow + 1}"].Value = "TOTAL SURPLUS/DEFISIT KAS";
            worksheet.Cells[$"A{footerStartingRow + 1}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{footerStartingRow + 1}:C{footerStartingRow + 1}"].Merge = true;
            worksheet.Cells[$"A{footerStartingRow + 1}:C{footerStartingRow + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{footerStartingRow + 1}:C{footerStartingRow + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"A{footerStartingRow + 2}"].Value = "Saldo Akhir Kas";
            worksheet.Cells[$"A{footerStartingRow + 2}"].Style.Font.Bold = true;
            worksheet.Cells[$"A{footerStartingRow + 2}:C{footerStartingRow + 2}"].Merge = true;
            worksheet.Cells[$"A{footerStartingRow + 2}:C{footerStartingRow + 2}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{footerStartingRow + 2}:C{footerStartingRow + 2}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{footerStartingRow + 3}"].Value = "Saldo Real Kas";
            worksheet.Cells[$"C{footerStartingRow + 3}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{footerStartingRow + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{footerStartingRow + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{footerStartingRow + 4}"].Value = "Selisih";
            worksheet.Cells[$"C{footerStartingRow + 4}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{footerStartingRow + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{footerStartingRow + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"C{footerStartingRow + 5}"].Value = "Rate";
            worksheet.Cells[$"C{footerStartingRow + 5}"].Style.Font.Bold = true;
            worksheet.Cells[$"C{footerStartingRow + 5}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[$"A{footerStartingRow + 5}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"B{footerStartingRow + 6}"].Value = "TOTAL SURPLUS (DEFISIT) EQUIVALENT";
            worksheet.Cells[$"B{footerStartingRow + 6}"].Style.Font.Bold = true;
            worksheet.Cells[$"B{footerStartingRow + 6}:C{footerStartingRow + 6}"].Merge = true;
            worksheet.Cells[$"B{footerStartingRow + 6}:C{footerStartingRow + 6}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"B{footerStartingRow + 6}:C{footerStartingRow + 6}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        private void SetTableHeader(ExcelWorksheet worksheet, UnitDto unit)
        {
            var unitName = "";
            if (unit != null)
                unitName = unit.Name;

            worksheet.Cells["A6"].Value = "";
            worksheet.Cells["A6:A7"].Merge = true;
            worksheet.Cells["A6:A7"].Style.Font.Size = 14;
            worksheet.Cells["A6:A7"].Style.Font.Bold = true;
            worksheet.Cells["A6:A7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["A6:A7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A6:A7"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            worksheet.Cells["B6"].Value = "KETERANGAN";
            worksheet.Cells["B6:C7"].Merge = true;
            worksheet.Cells["B6:C7"].Style.Font.Size = 14;
            worksheet.Cells["B6:C7"].Style.Font.Bold = true;
            worksheet.Cells["B6:C7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["B6:C7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["B6:C7"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            worksheet.Cells["D6"].Value = unitName;
            worksheet.Cells["D6:F6"].Merge = true;
            worksheet.Cells["D6:F6"].Style.Font.Size = 14;
            worksheet.Cells["D6:F6"].Style.Font.Bold = true;
            worksheet.Cells["D6:F6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["D6:F6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["D6:F6"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["G6"].Value = "RP (000)";
            worksheet.Cells["G6:G7"].Merge = true;
            worksheet.Cells["G6:G7"].Style.Font.Size = 14;
            worksheet.Cells["G6:G7"].Style.Font.Bold = true;
            worksheet.Cells["G6:G7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["G6:G7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["G6:G7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["H6"].Value = unitName;
            worksheet.Cells["H6:J6"].Merge = true;
            worksheet.Cells["H6:J6"].Style.Font.Size = 14;
            worksheet.Cells["H6:J6"].Style.Font.Bold = true;
            worksheet.Cells["H6:J6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["H6:J6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["H6:J6"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["K6"].Value = "RP (000)";
            worksheet.Cells["K6:K7"].Merge = true;
            worksheet.Cells["K6:K7"].Style.Font.Size = 14;
            worksheet.Cells["K6:K7"].Style.Font.Bold = true;
            worksheet.Cells["K6:K7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["K6:K7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["K6:K7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

            worksheet.Cells["D7"].Value = "Mata Uang";
            worksheet.Cells["D7"].Style.Font.Size = 14;
            worksheet.Cells["D7"].Style.Font.Bold = true;
            worksheet.Cells["D7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["D7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["D7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["E7"].Value = "Nominal Valas";
            worksheet.Cells["E7"].Style.Font.Size = 14;
            worksheet.Cells["E7"].Style.Font.Bold = true;
            worksheet.Cells["E7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["E7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["E7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["F7"].Value = "Nominal IDR";
            worksheet.Cells["F7"].Style.Font.Size = 14;
            worksheet.Cells["F7"].Style.Font.Bold = true;
            worksheet.Cells["F7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["F7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["F7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

            worksheet.Cells["H7"].Value = "Mata Uang";
            worksheet.Cells["H7"].Style.Font.Size = 14;
            worksheet.Cells["H7"].Style.Font.Bold = true;
            worksheet.Cells["H7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["H7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["H7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["I7"].Value = "Nominal Valas";
            worksheet.Cells["I7"].Style.Font.Size = 14;
            worksheet.Cells["I7"].Style.Font.Bold = true;
            worksheet.Cells["I7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["I7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["I7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            worksheet.Cells["J7"].Value = "Nominal IDR";
            worksheet.Cells["J7"].Style.Font.Size = 14;
            worksheet.Cells["J7"].Style.Font.Bold = true;
            worksheet.Cells["J7"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["J7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["J7"].Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

        }

        private void SetBestCaseWorstCaseMark(ExcelWorksheet worksheet)
        {
            worksheet.Cells["D5"].Value = "BEST CASE";
            worksheet.Cells["D5:F5"].Merge = true;
            worksheet.Cells["D5:F5"].Style.Font.Size = 14;
            worksheet.Cells["D5:F5"].Style.Font.Bold = true;
            worksheet.Cells["D5:F5"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["D5:F5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["D5:F5"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            worksheet.Cells["G5"].Value = "ACTUAL";
            worksheet.Cells["G5"].Style.Font.Size = 14;
            worksheet.Cells["G5"].Style.Font.Bold = true;
            worksheet.Cells["G5"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["G5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["G5"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            worksheet.Cells["H5"].Value = "WORST CASE";
            worksheet.Cells["H5:J5"].Merge = true;
            worksheet.Cells["H5:J5"].Style.Font.Size = 14;
            worksheet.Cells["H5:J5"].Style.Font.Bold = true;
            worksheet.Cells["H5:J5"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["H5:J5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["H5:J5"].Style.Fill.BackgroundColor.SetColor(Color.Red);
            worksheet.Cells["K5"].Value = "ACTUAL";
            worksheet.Cells["K5"].Style.Font.Size = 14;
            worksheet.Cells["K5"].Style.Font.Bold = true;
            worksheet.Cells["K5"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["K5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["K5"].Style.Fill.BackgroundColor.SetColor(Color.Red);
        }

        private void SetTitle(ExcelWorksheet worksheet, UnitDto unit, DateTimeOffset dueDate)
        {
            var company = "PT EFRATA GARMINDO UTAMA";
            var title = "LAPORAN BUDGET CASH FLOW";
            var unitName = "UNIT: ";
            if (unit != null)
                unitName += unit.Name;

            var dueDateString = $"{dueDate.AddMonths(1).AddHours(_identityService.TimezoneOffset).DateTime.ToString("MMMM yyyy", new CultureInfo("id-ID"))}";
            var date = $"JATUH TEMPO s.d. {dueDateString}";

            worksheet.Cells["A1"].Value = company;
            worksheet.Cells["A1:K1"].Merge = true;
            worksheet.Cells["A1:K1"].Style.Font.Size = 20;
            worksheet.Cells["A1:K1"].Style.Font.Bold = true;
            worksheet.Cells["A2"].Value = title;
            worksheet.Cells["A2:K2"].Merge = true;
            worksheet.Cells["A2:K2"].Style.Font.Size = 20;
            worksheet.Cells["A2:K2"].Style.Font.Bold = true;
            worksheet.Cells["A3"].Value = unitName;
            worksheet.Cells["A3:K3"].Merge = true;
            worksheet.Cells["A3:K3"].Style.Font.Size = 20;
            worksheet.Cells["A3:K3"].Style.Font.Bold = true;
            worksheet.Cells["A4"].Value = date;
            worksheet.Cells["A4:K4"].Merge = true;
            worksheet.Cells["A4:K4"].Style.Font.Size = 20;
            worksheet.Cells["A4:K4"].Style.Font.Bold = true;
        }
    }
}
