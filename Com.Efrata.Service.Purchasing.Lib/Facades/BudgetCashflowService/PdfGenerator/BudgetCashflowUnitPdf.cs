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
    public class BudgetCashflowUnitPdf : IBudgetCashflowUnitPdf
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _normalBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9, 0, BaseColor.White);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _smallerBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, BaseColor.White);

        private readonly IBudgetCashflowService _budgetCashflowService;
        private readonly IdentityService _identityService;
        private readonly List<UnitDto> _units;
        private readonly List<CurrencyDto> _currencies;

        public BudgetCashflowUnitPdf(IServiceProvider serviceProvider)
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

        private List<BudgetCashflowItemDto> GetCashDiff(List<BudgetCashflowItemDto> oadiff, List<BudgetCashflowItemDto> iadiff, List<BudgetCashflowItemDto> fadiff)
        {
            var result = oadiff.Concat(iadiff).Concat(fadiff).GroupBy(element => new { element.CurrencyId }).Select(element => new BudgetCashflowItemDto()
            {
                //CurrencyId = element.Key.CurrencyId,
                //ActualNominal = element.Sum(sum => sum.ActualNominal),
                //BestCaseActualNominal = element.Sum(sum => sum.BestCaseActualNominal),
                //BestCaseCurrencyNominal = element.Sum(sum => sum.BestCaseCurrencyNominal),
                //BestCaseNominal = element.Sum(sum => sum.BestCaseNominal),
                //CurrencyNominal = element.Sum(sum => sum.CurrencyNominal),
                //Nominal = element.Sum(sum => sum.Nominal),
                //CategoryName = element.FirstOrDefault().CategoryName,
                //CurrencyCode = element.Key.CurrencyCode,
                //DispositionTotal = element.Sum(sum => sum.DispositionTotal),
                //Total = element.Sum(sum => sum.DebtTotal) + element.Sum(sum => sum.DispositionTotal)
            });

            return result.ToList();
        }

        public MemoryStream Generate(int unitId, DateTimeOffset dueDate)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            var unit = _units.FirstOrDefault(element => element.Id == unitId);

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

            var cashdiff = GetCashDiff(oadiff, iadiff, fadiff);

            SetTitle(document, unit, dueDate);
            SetUnitTable(document, unit, oaci.Count, oaciTotal.Count, oaco.Count, oacoTotal.Count, oadiff.Count, iaci.Count, iaciTotal.Count, iaco.Count, iacoTotal.Count, iadiff.Count, faci.Count, faciTotal.Count, faco.Count, facoTotal.Count, fadiff.Count, oaci, oaciTotal, oaco, oacoTotal, oadiff, iaci, iaciTotal, iaco, iacoTotal, iadiff, faci, faciTotal, faco, facoTotal, fadiff, worstCases, cashdiff);
           
            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private void SetTitle(Document document, UnitDto unit, DateTimeOffset dueDate)
        {
            var company = "PT EFRATA GARMINDO UTAMA";
            var title = "LAPORAN BUDGET CASHFLOW";
            var unitName = "UNIT: ";
            if (unit != null)
                unitName += unit.Name;

            var dueDateString = $"{dueDate.AddMonths(1).AddHours(_identityService.TimezoneOffset).DateTime.ToString("MMMM yyyy", new CultureInfo("id-ID")).ToUpper()}";
            var date = $"PERIODE S.D. {dueDateString}";

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

            cell.Phrase = new Phrase(unitName, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(date, _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }

        private void SetUnitTable(Document document, UnitDto unit, int oaciCount, int oaciTotalCount, int oacoCount, int oacoTotalCount, int oadiffCount, int iaciCount, int iaciTotalCount, int iacoCount, int iacoTotalCount, int iadiffCount, int faciCount, int faciTotalCount, int facoCount, int facoTotalCount, int fadiffCount, List<BudgetCashflowItemDto> oaci, List<BudgetCashflowItemDto> oaciTotal, List<BudgetCashflowItemDto> oaco, List<BudgetCashflowItemDto> oacoTotal, List<BudgetCashflowItemDto> oadiff, List<BudgetCashflowItemDto> iaci, List<BudgetCashflowItemDto> iaciTotal, List<BudgetCashflowItemDto> iaco, List<BudgetCashflowItemDto> iacoTotal, List<BudgetCashflowItemDto> iadiff, List<BudgetCashflowItemDto> faci, List<BudgetCashflowItemDto> faciTotal, List<BudgetCashflowItemDto> faco, List<BudgetCashflowItemDto> facoTotal, List<BudgetCashflowItemDto> fadiff, List<BudgetCashflowItemDto> worstCases, List<BudgetCashflowItemDto> cashdiff)
        {
            var table = new PdfPTable(13)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 2f, 2f, 1f, 1f, 15f, 5f, 10f, 10f, 10f, 5f, 10f, 10f, 10f });

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var unitName = "";
            if (unit != null)
                unitName = unit.Name;

            var operatingActivitiesCashInRowCount = 2 + oaciCount + oaciTotalCount;
            var operatingActivitiesCashOutRowCount = 6 + oacoCount + oacoTotalCount;
            var operatingActivitiesRowsCount = 2 + oaciCount + oaciTotalCount + 6 + oacoCount + oacoTotalCount + oadiffCount;
            var investingActivitiesCashInRowCount = 1 + iaciCount + iaciTotalCount;
            var investingActivitiesCashOutRowCount = 1 + iacoCount + iacoTotalCount;
            var investingActivitiesRowsCount = 1 + iaciCount + iaciTotalCount + 1 + iacoCount + iacoTotalCount + iadiffCount;
            var financingActivitiesCashInRowsCount = 2 + faciCount + faciTotalCount;
            var financingActivitiesCashOutRowsCount = 3 + facoCount + facoTotalCount;
            var financingActivitiesRowsCount = 2 + faciCount + faciTotalCount + 3 + facoCount + facoTotalCount + fadiffCount;

            cell.BackgroundColor = new BaseColor(197, 90, 17);
            cell.Colspan = 5;
            cell.Rowspan = 3;
            cell.Phrase = new Phrase("KETERANGAN", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(146, 208, 80);
            cell.Colspan = 3;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("BEST CASE", _normalBoldFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(146, 208, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("ACTUAL", _normalBoldFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(255, 0, 0);
            cell.Colspan = 3;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("WORST CASE", _normalBoldFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(255, 0, 0);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("ACTUAL", _normalBoldFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 3;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(unitName, _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 2;
            cell.Phrase = new Phrase("Rp. (000)", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 3;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(unitName, _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 2;
            cell.Phrase = new Phrase("Rp. (000)", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("MATA UANG", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("NOMINAL VALAS", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("NOMINAL IDR", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("MATA UANG", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("NOMINAL VALAS", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(23, 50, 80);
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("NOMINAL IDR", _normalBoldWhiteFont);
            table.AddCell(cell);

            cell.BackgroundColor = new BaseColor(255, 255, 255);
            cell.Rotation = 90;
            cell.Colspan = 1;
            cell.Rowspan = operatingActivitiesRowsCount;
            cell.Phrase = new Phrase("OPERATING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Rowspan = operatingActivitiesCashInRowCount;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Pendapatan Operasional:", _smallerBoldFont);
            table.AddCell(cell);

            var isOthersSales = false;
            int firstOaci = 0;
            foreach (var item in oaci)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.OthersSales && !isOthersSales)
                {
                    isOthersSales = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pendapatan Operasional Lain-lain:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cell);

                var oaciLabel = "";
                if ((int)item.LayoutOrder != firstOaci)
                {
                    oaciLabel = item.LayoutOrder.ToDescriptionString();
                    firstOaci = (int)item.LayoutOrder;
                }

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(oaciLabel, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstOaciTotal = true;
            foreach (var item in oaciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = operatingActivitiesCashOutRowCount;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("HPP/Biaya Produksi:", _smallerBoldFont);
            table.AddCell(cell);

            var isMarketingSalaryCost = false;
            var isStillMarketingSalaryCost = false;
            var isGeneralAdministrativeExternalOutcomeVATCalculation = false;
            var isGeneralAdministrativeSalaryCost = false;
            var isOthersOperationalCost = false;
            int firstOaco = 0;
            foreach (var item in oaco)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost && !isMarketingSalaryCost)
                {
                    isMarketingSalaryCost = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase(" ", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.MarketingSalaryCost && !isStillMarketingSalaryCost)
                {
                    isStillMarketingSalaryCost = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Penjualan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeExternalOutcomeVATCalculation && !isGeneralAdministrativeExternalOutcomeVATCalculation)
                {
                    isGeneralAdministrativeExternalOutcomeVATCalculation = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Administrasi & Umum:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeSalaryCost && !isGeneralAdministrativeSalaryCost)
                {
                    isGeneralAdministrativeSalaryCost = true;

                    cell.Colspan = 1;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("", _smallerBoldFont);
                    table.AddCell(cell);

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 10;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya umum dan administrasi", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.OthersOperationalCost && !isOthersOperationalCost)
                {
                    isOthersOperationalCost = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Biaya Operasional Lain-lain:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cell);

                var oacoLabel = "";
                if((int)item.LayoutOrder != firstOaco)
                {
                    oacoLabel = item.LayoutOrder.ToDescriptionString();
                    firstOaco = (int)item.LayoutOrder;
                }

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(oacoLabel, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstOacoTotal = true;
            foreach (var item in oacoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstOaDiff = true;
            foreach (var item in oadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.Colspan = 1;
            cell.Rowspan = investingActivitiesRowsCount;
            cell.Phrase = new Phrase("INVESTING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Rowspan = investingActivitiesCashInRowCount;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Penerimaan dari Investasi:", _smallerBoldFont);
            table.AddCell(cell);

            int firstIaci = 0;
            foreach (var item in iaci)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                var iaciLabel = "";
                if ((int)item.LayoutOrder != firstIaci)
                {
                    iaciLabel = item.LayoutOrder.ToDescriptionString();
                    firstIaci = (int)item.LayoutOrder;
                }

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(iaciLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstIaciTotal = true;
            foreach (var item in iaciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = investingActivitiesCashOutRowCount;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Pembayaran pembelian asset tetap:", _smallerBoldFont);
            table.AddCell(cell);

            int firstIaco = 0;
            foreach (var item in iaco)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cell);

                var iacoLabel = "";
                if ((int)item.LayoutOrder != firstIaco)
                {
                    iacoLabel = item.LayoutOrder.ToDescriptionString();
                    firstIaco = (int)item.LayoutOrder;
                }
                    
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                if (item.LayoutOrder.ToDescriptionString() == "Deposito")
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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstIacoTotal = true;
            foreach (var item in iacoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstIaDiff = true;
            foreach (var item in iadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.Colspan = 1;
            cell.Rowspan = financingActivitiesRowsCount;
            cell.Phrase = new Phrase("FINANCING ACTIVITIES", _smallBoldFont);
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Rowspan = financingActivitiesCashInRowsCount;
            cell.Phrase = new Phrase("CASH IN", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Penerimaan dari Pendanaan:", _smallerBoldFont);
            table.AddCell(cell);

            var isCashInAffiliates = false;
            int firstFaci = 0;
            foreach (var item in faci)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashInAffiliates && !isCashInAffiliates)
                {
                    isCashInAffiliates = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Penerimaan lain-lain dari pendanaan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cell);

                var faciLabel = "";
                if ((int)item.LayoutOrder != firstFaci)
                {
                    faciLabel = item.LayoutOrder.ToDescriptionString();
                    firstFaci = (int)item.LayoutOrder;
                }

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                if (item.LayoutOrder.ToDescriptionString() == "Pencairan pinjaman (Loan Withdrawal)")
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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstFaciTotal = true;
            foreach (var item in faciTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Rotation = 90;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = financingActivitiesCashOutRowsCount;
            cell.Phrase = new Phrase("CASH OUT", _smallBoldFont);
            table.AddCell(cell);

            cell.Rotation = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 11;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("Pembayaran angsuran dan bunga Pinjaman:", _smallerBoldFont);
            table.AddCell(cell);

            var isCashOutBankInterest = false;
            var isCashOutAffiliates = false;
            int firstFaco = 0;
            foreach (var item in faco)
            {
                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutBankAdministrationFee && !isCashOutBankInterest)
                {
                    isCashOutBankInterest = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pembayaran Biaya Administrasi Bank:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                if (item.LayoutOrder == BudgetCashflowCategoryLayoutOrder.CashOutAffiliates && !isCashOutAffiliates)
                {
                    isCashOutAffiliates = true;

                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 11;
                    cell.Rowspan = 1;
                    cell.Phrase = new Phrase("Pengeluaran lain-lain dari Pendanaan:", _smallerBoldFont);
                    table.AddCell(cell);
                }

                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.LayoutOrder == item.LayoutOrder);
                if (worstCase == null)
                    worstCase = new BudgetCashflowItemDto();

                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase("", _smallerFont);
                table.AddCell(cell);

                var facoLabel = "";
                if ((int)item.LayoutOrder != firstFaco)
                {
                    facoLabel = item.LayoutOrder.ToDescriptionString();
                    firstFaco = (int)item.LayoutOrder;
                }

                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 2;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(facoLabel, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", worstCase.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstFacoTotal = true;
            foreach (var item in facoTotal)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

            bool firstFaDiff = true;
            foreach (var item in fadiff)
            {
                var currencyCode = "";
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                    currencyCode = currency.Code;

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
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

            double eqBestCaseCurrencyNominal = 0;
            double eqBestCaseNominal = 0;
            double eqBestCaseActualNominal = 0;
            double eqCurrencyNominal = 0;
            double eqNominal = 0;
            double eqActualNominal = 0;

            bool firstCashDiff = true;
            foreach (var item in cashdiff)
            {
                var currencyCode = "";
                double currencyRate = 0;
                var currency = _currencies.FirstOrDefault(element => element.Id == item.CurrencyId);
                if (currency != null)
                {
                    currencyCode = currency.Code;
                    currencyRate = currency.Rate;
                }

                double idrBestCaseCurrencyNominal = item.BestCaseCurrencyNominal * currencyRate;
                double idrBestCaseNominal = item.BestCaseNominal * currencyRate;
                double idrBestCaseActualNominal = item.BestCaseActualNominal * currencyRate;
                double idrCurrencyNominal = item.CurrencyNominal * currencyRate;
                double idrNominal = item.Nominal * currencyRate;
                double idrActualNominal = item.ActualNominal * currencyRate;

                eqBestCaseCurrencyNominal += idrBestCaseCurrencyNominal;
                eqBestCaseNominal += idrBestCaseNominal;
                eqBestCaseActualNominal += idrBestCaseActualNominal;
                eqCurrencyNominal += idrCurrencyNominal;
                eqNominal += idrNominal;
                eqActualNominal += idrActualNominal;

                var cashdiffLabel = firstCashDiff ? "TOTAL SURPLUS/DEFISIT KAS" : "";
                firstCashDiff = false;

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 5;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(cashdiffLabel, _smallerBoldFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseCurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.BestCaseActualNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(currencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.CurrencyNominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.Nominal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(string.Format("{0:n}", item.ActualNominal), _smallerFont);
                table.AddCell(cell);
            }
            
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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerFont);
            table.AddCell(cell);

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

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", 0), _smallerFont);
            table.AddCell(cell);

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
            cell.Colspan = 8;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(" ", _smallerBoldFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 5;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("TOTAL SURPLUS (DEFISIT) EQUIVALENT", _smallerBoldFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("IDR", _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqBestCaseCurrencyNominal), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqBestCaseNominal), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqBestCaseActualNominal), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase("IDR", _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqCurrencyNominal), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqNominal), _smallerFont);
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 1;
            cell.Rowspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:n}", eqActualNominal), _smallerFont);
            table.AddCell(cell);

            document.Add(table);
        }

    }
}
