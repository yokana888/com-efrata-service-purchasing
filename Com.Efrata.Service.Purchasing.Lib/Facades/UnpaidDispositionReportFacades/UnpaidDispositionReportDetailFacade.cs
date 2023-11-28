using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnpaidDispositionReport;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnpaidDispositionReportFacades
{
    public class UnpaidDispositionReportDetailFacade : IUnpaidDispositionReportDetailFacade
    {
        private readonly PurchasingDbContext _dbContext;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IdentityService _identityService;
        private const string IDRCurrencyCode = "IDR";
        private readonly List<CategoryDto> _categories;

        public UnpaidDispositionReportDetailFacade(IServiceProvider serviceProvider)
        {
            var cache = serviceProvider.GetService<IDistributedCache>();
            var jsonCategories = cache.GetString(MemoryCacheConstant.Categories);

            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
            _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
            _identityService = serviceProvider.GetService<IdentityService>();

            _categories = JsonConvert.DeserializeObject<List<CategoryDto>>(jsonCategories, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        private async Task<UnpaidDispositionReportDetailViewModel> GetReportData(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            //var dateStart = dateFrom.GetValueOrDefault().ToUniversalTime();
            var dateEnd = (dateTo.HasValue ? dateTo.Value : DateTime.MaxValue).ToUniversalTime();

            var query = from pdItems in _dbContext.PurchasingDispositionItems

                        join pds in _dbContext.PurchasingDispositions on pdItems.PurchasingDispositionId equals pds.Id into joinPurchasingDispositions
                        from pd in joinPurchasingDispositions.DefaultIfEmpty()

                        join pdDetailItems in _dbContext.PurchasingDispositionDetails on pdItems.Id equals pdDetailItems.PurchasingDispositionItemId into joinPurchasingDispositionDetails
                        from pdDetailItem in joinPurchasingDispositionDetails.DefaultIfEmpty()

                        join urnItems in _dbContext.UnitReceiptNoteItems on pdItems.EPOId equals urnItems.EPOId.ToString() into joinUnitReceiptNoteItems
                        from urnItem in joinUnitReceiptNoteItems.DefaultIfEmpty()

                        join upoItems in _dbContext.UnitPaymentOrderItems on urnItem.Id equals upoItems.URNId into joinUnitPaymentOrderItems
                        from upoItem in joinUnitPaymentOrderItems.DefaultIfEmpty()

                        join upos in _dbContext.UnitPaymentOrders on upoItem.UPOId equals upos.Id into joinUnitPaymentOrders
                        from upo in joinUnitPaymentOrders.DefaultIfEmpty()

                        join epos in _dbContext.ExternalPurchaseOrders on pdItems.EPOId equals epos.Id.ToString() into joinExternalPurchaseOrders
                        from epo in joinExternalPurchaseOrders.DefaultIfEmpty()

                        where pd.PaymentDueDate <= dateEnd && pd.IsPaid == false && epo.SupplierIsImport == isImport
                        select new
                        {
                            pdItems.Id,

                            pd.CreatedUtc,
                            pd.IsPaid,
                            pd.DispositionNo,
                            pd.PaymentDueDate,
                            pd.DivisionId,
                            pd.DivisionCode,
                            pd.DivisionName,
                            pd.CurrencyId,
                            pd.CurrencyCode,
                            pd.CurrencyRate,
                            pd.CategoryId,
                            pd.CategoryCode,
                            pd.CategoryName,
                            pd.SupplierName,
                            pd.DPP,
                            pd.IncomeTaxBy,
                            pd.IncomeTaxValue,
                            pd.VatValue,

                            pdDetailItem.UnitId,
                            pdDetailItem.UnitCode,
                            pdDetailItem.UnitName,

                            urnItem.UnitReceiptNote.URNNo,

                            upoItem.UnitPaymentOrder.UPONo,
                            upoItem.UnitPaymentOrder.InvoiceNo,

                            epo.SupplierIsImport
                        };

            query = query.GroupBy(x => x.Id).Select(y => y.First());

            if (!isForeignCurrency && !isImport)
                query = query.Where(x => x.CurrencyCode == "IDR");
            else if (isForeignCurrency)
                query = query.Where(x => x.CurrencyCode != "IDR");

            if (accountingUnitId > 0)
            {
                var unitFilterIds = await _currencyProvider.GetUnitsIdsByAccountingUnitId(accountingUnitId);
                if (unitFilterIds.Count() > 0)
                    query = query.Where(x => unitFilterIds.Contains(x.UnitId));
            }

            //var categoryFilterIds = await _currencyProvider.GetCategoryIdsByAccountingCategoryId(accountingCategoryId);
            //if (categoryFilterIds.Count() > 0)
            //    query = query.Where(x => categoryFilterIds.Contains(x.CategoryId));

            if (categoryId > 0)
                query = query.Where(x => x.CategoryId == categoryId.ToString());

            if (divisionId > 0)
                query = query.Where(x => x.DivisionId == divisionId.ToString());

            var queryResult = query.OrderByDescending(x => x.PaymentDueDate).ToList();

            var unitIds = queryResult.Select(item =>
            {
                int.TryParse(item.UnitId, out var unitId);
                return unitId;
            }).Distinct().ToList();
            var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
            var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

            var itemCategoryIds = queryResult.Select(item =>
            {
                int.TryParse(item.CategoryId, out var itemCategoryId);
                return itemCategoryId;
            }).Distinct().ToList();
            var categories = await _currencyProvider.GetCategoriesByCategoryIds(itemCategoryIds);
            var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(itemCategoryIds);

            var reportResult = new UnpaidDispositionReportDetailViewModel();
            foreach (var item in queryResult)
            {
                int.TryParse(item.UnitId, out var unitId);
                var unit = units.FirstOrDefault(element => element.Id == unitId);
                var accountingUnit = new AccountingUnit();
                if (unit != null)
                {
                    accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
                }

                //int.TryParse(item.CategoryId, out var itemCategoryId);
                //var category = categories.FirstOrDefault(element => element.Id == itemCategoryId);
                //var accountingCategory = new AccountingCategory();
                //if (category != null)
                //{
                //    accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
                //}

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == item.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                double total = 0;
                double totalCurrency = 0;
                if (item.IncomeTaxBy == "Supplier")
                {
                    total = item.DPP + item.VatValue - item.IncomeTaxValue;
                    totalCurrency = (item.DPP + item.VatValue - item.IncomeTaxValue) * item.CurrencyRate;
                }
                else
                {
                    total = item.DPP + item.VatValue;
                    totalCurrency = (item.DPP + item.VatValue) * item.CurrencyRate;
                }


                var reportItem = new DispositionReport()
                {
                    DispositionNo = item.DispositionNo,
                    DispositionDate = item.CreatedUtc,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    CategoryCode = item.CategoryCode,
                    CurrencyId = item.CurrencyId,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = (decimal)item.CurrencyRate,
                    //AccountingCategoryName = accountingCategory.Name,
                    //AccountingCategoryCode = accountingCategory.Code,
                    //AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
                    DPP = (decimal)item.DPP,
                    DPPCurrency = (decimal)(item.DPP * item.CurrencyRate),
                    InvoiceNo = item.InvoiceNo,
                    VAT = (decimal)item.VatValue,
                    Total = (decimal)total,
                    TotalCurrency = (decimal)totalCurrency,
                    SupplierName = item.SupplierName,
                    UnitId = item.UnitId,
                    UnitName = item.UnitName,
                    UnitCode = item.UnitCode,
                    AccountingUnitName = accountingUnit.Name,
                    AccountingUnitCode = accountingUnit.Code,
                    UPONo = item.UPONo,
                    URNNo = item.URNNo,
                    IncomeTax = (decimal)item.IncomeTaxValue,
                    IncomeTaxBy = item.IncomeTaxBy,
                    PaymentDueDate = item.PaymentDueDate.Date,
                    DivisionName = item.DivisionName,
                    CategoryLayoutIndex = categoryLayoutIndex,
                };

                reportResult.Reports.Add(reportItem);
            }

            reportResult.UnitSummaries = reportResult.Reports
                        .GroupBy(report => new { report.AccountingUnitName, report.CurrencyCode })
                        .Select(report => new Summary()
                        {
                            Name = report.Key.AccountingUnitName,
                            CurrencyCode = report.Key.CurrencyCode,
                            SubTotal = report.Sum(sum => sum.Total),
                            SubTotalCurrency = report.Sum(sum => sum.TotalCurrency),
                            //AccountingLayoutIndex = report.Select(item => item.AccountingLayoutIndex).FirstOrDefault()
                        })
                        .OrderBy(report => report.Name).ToList();

            reportResult.CurrencySummaries = reportResult.Reports
                .GroupBy(report => new { report.CurrencyCode })
                .Select(report => new Summary()
                {
                    CurrencyCode = report.Key.CurrencyCode,
                    SubTotal = report.Sum(sum => sum.Total),
                    SubTotalCurrency = report.Sum(sum => sum.TotalCurrency)
                }).OrderBy(order => order.CurrencyCode).ToList();

            reportResult.CategorySummaries = reportResult.Reports
                .GroupBy(report => new { report.CategoryName, report.CategoryId, report.CurrencyCode })
                .Select(report => new Summary()
                {
                    CategoryName = report.Key.CategoryName,
                    CategoryId = report.Key.CategoryId,
                    CurrencyCode = report.Key.CurrencyCode,
                    SubTotal = report.Sum(sum => sum.Total)                    
                })
                .OrderBy(order => order.CategoryName).ToList();

            reportResult.Reports = reportResult.Reports.OrderBy(order => order.CategoryLayoutIndex).ToList();
            reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.TotalCurrency);
            reportResult.UnitSummaryTotal = reportResult.UnitSummaries.Sum(categorySummary => categorySummary.SubTotalCurrency);

            return reportResult;
        }

        public async Task<MemoryStream> GenerateExcel(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            var dueDateString = $"{dateTo:dd/MM/yyyy}";
            if (dateTo == DateTimeOffset.MaxValue)
                dueDateString = "-";

            var result = await GetReport(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);

            var unitName = "SEMUA UNIT";
            var divisionName = "SEMUA DIVISI";
            var separator = " - ";

            if (accountingUnitId > 0 && divisionId == 0)
            {
                var summary = result.Reports.FirstOrDefault();
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
                var summary = result.Reports.FirstOrDefault();
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
                var summary = result.Reports.FirstOrDefault();
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

            var reportDataTable = GetFormatReportExcel();

            var unitDataTable = new DataTable();
            if (isForeignCurrency || isImport)
            {
                unitDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
                unitDataTable.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(string) });
                unitDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
            }
            else
            {
                unitDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
                unitDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });
            }
            
            var currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });

            var categoryDataTable = new DataTable();
            if (isForeignCurrency || isImport)
            {
                categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Category", DataType = typeof(string) });
                categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(string) });
                categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
            }
            else
            {
                categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Category", DataType = typeof(string) });
                categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });
            }

            int space = 0;
            if (result.Reports.Count > 0)
            {
                var data = result.Reports.GroupBy(x => x.CategoryName);
                int i = 1;
                foreach (var reports in data)
                {
                    var totalCurrencies = new Dictionary<string, decimal>();
                    foreach (var v in reports)
                    {
                        reportDataTable.Rows.Add(i.ToString(), v.DispositionDate.GetValueOrDefault().ToString("dd/MM/yyyy"), v.DispositionNo, v.URNNo, v.UPONo, v.InvoiceNo, v.SupplierName, v.CategoryName, v.AccountingUnitName, v.PaymentDueDate.GetValueOrDefault().ToString("dd/MM/yyyy"), v.CurrencyCode, string.Format("{0:n}", v.Total));
                        i++;

                        // Currency summary
                        if (totalCurrencies.ContainsKey(v.CurrencyCode))
                        {
                            totalCurrencies[v.CurrencyCode] += v.Total;
                        }
                        else
                        {
                            totalCurrencies.Add(v.CurrencyCode, v.Total);
                        }
                    }

                    foreach (var totalCurrency in totalCurrencies)
                    {
                        reportDataTable.Rows.Add("", "", "", "", "", "", "", "", "", "Jumlah", totalCurrency.Key, string.Format("{0:n}", totalCurrency.Value));
                        space++;
                    }

                }

                List<Summary> summaries = new List<Summary>();

                foreach (var unitSummary in result.UnitSummaries)
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

                List<Summary> categSummaries = new List<Summary>();

                foreach (var categorySummary in result.CategorySummaries)
                {
                    if (categSummaries.Any(x => x.CategoryName == categorySummary.CategoryName))
                        categSummaries.Add(new Summary
                        {
                            CategoryName = "",
                            CurrencyCode = categorySummary.CurrencyCode,
                            SubTotal = categorySummary.SubTotal,
                            SubTotalCurrency = categorySummary.SubTotalCurrency
                            
                        });
                    else
                    {
                        categSummaries.Add(categorySummary);
                    }
                }

                foreach (var unitSummary in summaries)
                {
                    if (isForeignCurrency || isImport)
                        unitDataTable.Rows.Add(unitSummary.Name, unitSummary.CurrencyCode, unitSummary.SubTotal);
                    else
                        unitDataTable.Rows.Add(unitSummary.Name, unitSummary.SubTotalCurrency);
                }

                foreach(var currencySummary in result.CurrencySummaries)
                    currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.SubTotal);

                foreach (var categorySummary in categSummaries)
                {
                    if (isForeignCurrency || isImport)
                        categoryDataTable.Rows.Add(categorySummary.CategoryName, categorySummary.CurrencyCode, categorySummary.SubTotal);
                    else
                        categoryDataTable.Rows.Add(categorySummary.CategoryName, categorySummary.SubTotal);
                }


            }

            using (var package = new ExcelPackage())
            {
                var company = "PT EFRATA GARMINDO UTAMA";
                var title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) LOKAL";
                if (isForeignCurrency)
                    title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) LOKAL VALAS";
                else if (isImport)
                    title = "LAPORAN DISPOSISI BELUM DIBAYAR (DETAIL) IMPOR";
                var period = $"PERIODE S.D. {dueDateString}";

                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A3"].Value = unitName + separator + divisionName;
                worksheet.Cells["A4"].Value = period;
                worksheet.Cells["A5"].LoadFromDataTable(reportDataTable, true);
                worksheet.Cells[$"A{5 + 3 + result.Reports.Count + space}"].LoadFromDataTable(unitDataTable, true);
                worksheet.Cells[$"A{5 + result.Reports.Count + space + 3 + result.UnitSummaries.Count + 3}"].LoadFromDataTable(currencyDataTable, true);
                worksheet.Cells[$"H{5 + 3 + result.Reports.Count + space}"].LoadFromDataTable(categoryDataTable, true);

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private DataTable GetFormatReportExcel()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No SPB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No BP", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jatuh Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Saldo", DataType = typeof(string) });

            return dt;
        }

        public Task<UnpaidDispositionReportDetailViewModel> GetReport(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            return GetReportData(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);
        }
    }
}
