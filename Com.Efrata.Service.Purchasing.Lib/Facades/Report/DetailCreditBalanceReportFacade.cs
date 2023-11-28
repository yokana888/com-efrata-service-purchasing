using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using MongoDB.Driver;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Microsoft.EntityFrameworkCore;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseOrder;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using MongoDB.Bson;
using System.Data.SqlClient;
using System.Globalization;
using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Microsoft.Extensions.Caching.Distributed;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Report
{
    public class DetailCreditBalanceReportFacade : IDetailCreditBalanceReportFacade
    {
        private readonly PurchasingDbContext _dbContext;
        private readonly List<UnitDto> _units;
        private readonly List<AccountingUnitDto> _accountingUnits;
        private readonly List<CategoryDto> _categories;
        //private readonly IdentityService _identityService;
        //private readonly ICurrencyProvider _currencyProvider;
        //private const string IDRCurrencyCode = "IDR";

        public DetailCreditBalanceReportFacade(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
            var cache = serviceProvider.GetService<IDistributedCache>();
            var jsonUnits = cache.GetString(MemoryCacheConstant.Units);
            var jsonCategories = cache.GetString(MemoryCacheConstant.Categories);
            var jsonAccountingUnits = cache.GetString(MemoryCacheConstant.AccountingUnits);
            _units = JsonConvert.DeserializeObject<List<UnitDto>>(jsonUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            _accountingUnits = JsonConvert.DeserializeObject<List<AccountingUnitDto>>(jsonAccountingUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            _categories = JsonConvert.DeserializeObject<List<CategoryDto>>(jsonCategories, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        public async Task<DetailCreditBalanceReportViewModel> GetReportData(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.MaxValue).ToUniversalTime();

            var unitReceiptNoteItems = _dbContext.UnitReceiptNoteItems.AsQueryable();
            var unitReceiptNotes = _dbContext.UnitReceiptNotes.AsQueryable();
            var unitPaymentOrderItems = _dbContext.UnitPaymentOrderItems.AsQueryable();
            var unitPaymentOrders = _dbContext.UnitPaymentOrders.AsQueryable();
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchaseRequests = _dbContext.PurchaseRequests.AsQueryable();

            var query = from unitReceiptNoteItem in unitReceiptNoteItems

                        join unitReceiptNote in unitReceiptNotes on unitReceiptNoteItem.URNId equals unitReceiptNote.Id into urnWithItems
                        from urnWithItem in urnWithItems.DefaultIfEmpty()

                        join unitPaymentOrderItem in unitPaymentOrderItems on urnWithItem.Id equals unitPaymentOrderItem.URNId into urnUPOItems
                        from urnUPOItem in urnUPOItems.DefaultIfEmpty()

                        join unitPaymentOrder in unitPaymentOrders on urnUPOItem.UPOId equals unitPaymentOrder.Id into upoWithItems
                        from upoWithItem in upoWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on unitReceiptNoteItem.EPOId equals externalPurchaseOrder.Id into urnEPOs
                        from urnEPO in urnEPOs.DefaultIfEmpty()

                        join purchaseRequest in purchaseRequests on unitReceiptNoteItem.PRId equals purchaseRequest.Id into urnPRs
                        from urnPR in urnPRs.DefaultIfEmpty()

                            // Additional
                            //join epoDetail in _dbContext.ExternalPurchaseOrderDetails on unitReceiptNoteItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrderDetails
                            //from urnEPODetail in joinExternalPurchaseOrderDetails.DefaultIfEmpty()

                            //where urnWithItem != null && urnWithItem.ReceiptDate != null  
                            //&& urnEPO != null && urnEPO.PaymentDueDays != null
                            //&& urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)) <= d2
                            //&& upoWithItem != null && upoWithItem.IsPaid == false && urnEPO != null && urnEPO.SupplierIsImport == isImport

                            //where upoWithItem != null && !upoWithItem.IsPaid && (urnWithItem.SupplierIsImport == isImport) && urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)) <= d2

                        select new DetailCreditBalanceReport
                        {
                            CurrencyId = urnEPO.CurrencyId,
                            CurrencyCode = urnEPO.CurrencyCode,
                            CurrencyRate = urnEPO.CurrencyRate,
                            CategoryId = urnPR.CategoryId,
                            CategoryCode = urnPR.CategoryCode,
                            CategoryName = urnPR.CategoryName,
                            UnitId = urnPR.UnitId,
                            UnitCode = urnPR.UnitCode,
                            UnitName = urnPR.UnitName,
                            DivisionId = urnPR.DivisionId,
                            DivisionCode = urnPR.DivisionCode,
                            DivisionName = urnPR.DivisionName,
                            IsImport = urnWithItem.SupplierIsImport,
                            IsPaid = upoWithItem != null && upoWithItem.IsPaid,
                            DebtPrice = unitReceiptNoteItem.PricePerDealUnit,
                            DebtQuantity = unitReceiptNoteItem.ReceiptQuantity,
                            DebtTotal = unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity,
                            DueDate = urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)),
                            IncomeTaxBy = urnEPO.IncomeTaxBy,
                            UseIncomeTax = urnEPO.UseIncomeTax,
                            IncomeTaxRate = urnEPO.IncomeTaxRate,
                            UseVat = urnEPO.UseVat,

                            UPODate = urnUPOItem.UnitPaymentOrder.Date,
                            UPONo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.UPONo : "",
                            URNNo = unitReceiptNoteItem.UnitReceiptNote != null ? unitReceiptNoteItem.UnitReceiptNote.URNNo : "",
                            InvoiceNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.InvoiceNo : "",
                            SupplierName = unitReceiptNoteItem.UnitReceiptNote.SupplierName,
                            //urnPR.CategoryName,
                            //AccountingUnitName = 
                            //DueDate = urnWithItem != null && urnWithItem.ReceiptDate != null && urnEPO != null ? urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)) : DateTimeOffset.Now,
                            //urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,
                            //TotalSaldo = unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity,

                            //urnPR.CategoryId,
                            //urnPR.DivisionName,
                            //unitReceiptNoteItem.UnitReceiptNote.UnitId,
                            //urnPR.DivisionId,
                            //urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
                            //ReceiptQuantity = unitReceiptNoteItem.ReceiptQuantity,
                            //EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
                            //unitReceiptNoteItem.IncomeTaxBy,
                            //urnEPO.UseIncomeTax,
                            //urnEPO.IncomeTaxRate,
                        };

            query = query.Where(entity => !entity.IsPaid && (entity.IsImport == isImport) && entity.DueDate <= d2);

            if (categoryId > 0)
                query = query.Where(entity => entity.CategoryId == categoryId.ToString());

            //if (accountingUnitId > 0)
            //{
            //    var unitFilterIds = await _currencyProvider.GetUnitsIdsByAccountingUnitId(accountingUnitId);
            //    if (unitFilterIds.Count() > 0)
            //    {
            //        query = query.Where(unitReceiptNote => unitFilterIds.Contains(unitReceiptNote.UnitId));
            //    }
            //}

            if (accountingUnitId > 0)
            {
                var unitIds = _units.Where(unit => unit.AccountingUnitId == accountingUnitId).Select(unit => unit.Id.ToString()).ToList();
                if (unitIds.Count == 0)
                    // intentionally added to make the query returns empty data
                    unitIds.Add("0");
                query = query.Where(entity => unitIds.Contains(entity.UnitId));

            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            if (!isForeignCurrency && !isImport)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() == "IDR" && !entity.IsImport);
            else if (isForeignCurrency)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() != "IDR" && !entity.IsImport);
            else if (isImport)
                query = query.Where(entity => entity.IsImport);

            //var queryResult = query.OrderByDescending(item => item.DueDate).ToList();
            var queryResult = query.ToList();

            //var currencyTuples = queryResult.Select(item => new Tuple<string, DateTimeOffset>(item.CurrencyCode, item.Date));
            //var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

            //var unitIds = queryResult.Select(item =>
            //{
            //    int.TryParse(item.UnitId, out var unitId);
            //    return unitId;
            //}).Distinct().ToList();
            //var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
            //var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

            //var itemCategoryIds = queryResult.Select(item =>
            //{
            //    int.TryParse(item.CategoryId, out var itemCategoryId);
            //    return itemCategoryId;
            //}).Distinct().ToList();
            //var categories = await _currencyProvider.GetCategoriesByCategoryIds(itemCategoryIds);
            //var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(itemCategoryIds);


            var reportResult = new DetailCreditBalanceReportViewModel();
            foreach (var element in queryResult)
            {
                //var currency = currencies.FirstOrDefault(f => f.Code == item.CurrencyCode);
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.AccountingUnitId);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                }

                //int.TryParse(element.UnitId, out var unitId);
                //var unit = _units.FirstOrDefault(entity => entity.Id == unitId);
                //var accountingUnit = new AccountingUnit();
                //if (unit != null)
                //{
                //    accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
                //}

                //int.TryParse(item.CategoryId, out var itemCategoryId);
                //var category = categories.FirstOrDefault(element => element.Id == itemCategoryId);
                //var accountingCategory = new AccountingCategory();
                //if (category != null)
                //{
                //    accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
                //}

                //var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == item.CategoryId);
                //var categoryLayoutIndex = 0;
                //if (category != null)
                //    categoryLayoutIndex = category.ReportLayoutIndex;

                //decimal dpp = 0;
                //decimal dppCurrency = 0;
                //decimal ppn = 0;
                //decimal ppnCurrency = 0;

                //double currencyRate = 1;
                //var currencyCode = "IDR";

                //decimal totalDebt = 0;
                //decimal totalDebtIDR = 0;
                //decimal incomeTax = 0;
                //decimal.TryParse(item.IncomeTaxRate, out var incomeTaxRate);


                //if (item.UseVat)
                //    ppn = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity * 0.1);

                //if (currency != null && !currency.Code.Equals("IDR"))
                //{
                //    currencyRate = currency.Rate.GetValueOrDefault();
                //    dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
                //    dppCurrency = dpp * (decimal)currencyRate;
                //    ppnCurrency = ppn * (decimal)currencyRate;
                //    currencyCode = currency.Code;
                //}
                //else
                //    dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);

                //if (item.UseIncomeTax)
                //    incomeTax = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity) * incomeTaxRate / 100;

                //if (item.IncomeTaxBy == "Supplier")
                //{
                //    totalDebtIDR = (dpp + ppn - incomeTax) * (decimal)currencyRate;
                //    totalDebt = dpp + ppn - incomeTax;
                //}
                //else
                //{
                //    totalDebtIDR = (dpp + ppn) * (decimal)currencyRate;
                //    totalDebt = dpp + ppn;
                //}

                var reportItem = new DetailCreditBalanceReport()
                {
                    UPODate = element.UPODate,
                    UPONo = element.UPONo,
                    URNNo = element.URNNo,
                    InvoiceNo = element.InvoiceNo,
                    SupplierName = element.SupplierName,
                    CategoryName = element.CategoryName,
                    AccountingUnitName = accountingUnitName,
                    DueDate = element.DueDate,
                    CurrencyCode = element.CurrencyCode,
                    Total = (decimal)debtTotal,
                    TotalIDR = (decimal)debtTotal * (decimal)element.CurrencyRate,
                    CategoryId = element.CategoryId,
                    DivisionName = element.DivisionName,
                    CategoryLayoutIndex = categoryLayoutIndex,
                    //TotalSaldo = (decimal)item.TotalSaldo
                    //CategoryCode = item.CategoryCode,
                    //AccountingCategoryName = accountingCategory.Name,
                    //AccountingCategoryCode = accountingCategory.Code,
                    //AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
                    //CurrencyRate = (decimal)currencyRate,
                    //DONo = item.DONo,
                    //DPP = dpp,
                    //DPPCurrency = dppCurrency,
                    //VATNo = item.VatNo,
                    //IPONo = item.PONo,
                    //VAT = ppn,
                    //VATCurrency = ppnCurrency,
                    //Total = dpp * (decimal)currencyRate,
                    //ProductName = item.ProductName,
                    //SupplierCode = item.SupplierCode,
                    //UnitName = item.UnitName,
                    //UnitCode = item.UnitCode,
                    //AccountingUnitCode = accountingUnit.Code,
                    //IsUseVat = item.UseVat,
                    //PIBDate = item.PibDate,
                    //PIBNo = item.PibNo,
                    //PIBBM = (decimal)item.ImportDuty,
                    //PIBIncomeTax = (decimal)item.TotalIncomeTaxAmount,
                    //PIBVat = (decimal)item.TotalVatAmount,
                    //PIBImportInfo = item.ImportInfo,
                    //Remark = item.Remark,
                    //Quantity = item.ReceiptQuantity,
                    //IsPaid = item.IsPaid
                    //Saldo = (decimal)item.Saldo,
                };

                reportResult.Reports.Add(reportItem);
            }

            reportResult.AccountingUnitSummaries = reportResult.Reports
                        .GroupBy(report => new { report.AccountingUnitName, report.CurrencyCode })
                        .Select(report => new SummaryDCB()
                        {
                            AccountingUnitName = report.Key.AccountingUnitName,
                            CurrencyCode = report.Key.CurrencyCode,
                            SubTotal = report.Sum(sum => sum.Total),
                            SubTotalIDR = report.Sum(sum => sum.TotalIDR),
                        })
                        .OrderBy(order => order.AccountingUnitName).ToList();

            reportResult.CurrencySummaries = reportResult.Reports
                        .GroupBy(report => new { report.CurrencyCode })
                        .Select(report => new SummaryDCB()
                        {
                            CurrencyCode = report.Key.CurrencyCode,
                            SubTotal = report.Sum(sum => sum.Total),
                            SubTotalIDR = report.Sum(sum => sum.TotalIDR)
                        })
                        .OrderBy(order => order.CurrencyCode).ToList();

            reportResult.CategorySummaries = reportResult.Reports
                .GroupBy(report => new { report.CategoryName, report.CategoryId, report.CurrencyCode })
                .Select(report => new SummaryDCB()
                {
                    CategoryName = report.Key.CategoryName,
                    CategoryId = report.Key.CategoryId,
                    CurrencyCode = report.Key.CurrencyCode,
                    SubTotal = report.Sum(sum => sum.Total),
                    SubTotalIDR = report.Sum(sum => sum.TotalIDR)
                })
                .OrderBy(order => order.CategoryName).ToList();

            //reportResult.Reports = reportResult.Reports
            reportResult.Reports = reportResult.Reports
                        .GroupBy(
                            key => new
                            {
                                key.URNNo,
                                key.UPONo,
                                key.UPODate,
                                key.InvoiceNo,
                                key.SupplierName,
                                key.CategoryName,
                                key.AccountingUnitName,
                                key.DueDate,
                                key.CurrencyCode,
                                key.CategoryId,
                                key.DivisionName,
                                key.CategoryLayoutIndex
                            },
                            val => val,
                            (key, val) => new DetailCreditBalanceReport()
                            {
                                URNNo = key.URNNo,
                                UPONo = key.UPONo,
                                UPODate = key.UPODate,
                                InvoiceNo = key.InvoiceNo,
                                SupplierName = key.SupplierName,
                                CategoryName = key.CategoryName,
                                AccountingUnitName = key.AccountingUnitName,
                                DueDate = key.DueDate,
                                CurrencyCode = key.CurrencyCode,
                                Total = val.Sum(s => s.Total),
                                TotalIDR = val.Sum(s => s.TotalIDR),
                                CategoryId = key.CategoryId,
                                DivisionName = key.DivisionName,
                                CategoryLayoutIndex = key.CategoryLayoutIndex
                            })
                        .OrderBy(order => order.CategoryLayoutIndex).ToList();

            reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.Total);
            reportResult.AccountingUnitSummaryTotal = reportResult.AccountingUnitSummaries.Sum(summary => summary.SubTotal);

            return reportResult;
        }

        public Task<DetailCreditBalanceReportViewModel> GetReport(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            return GetReportData(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);
        }

        //public async Task<MemoryStream> GenerateExcel(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        //{
        //    var result = await GetReport(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);
        //    var reportDataTable = new DataTable();
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal SPB", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SPB", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No BP", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Jatuh Tempo", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Saldo", DataType = typeof(decimal) });

        //    var accountingUnitDataTable = new DataTable();
        //    accountingUnitDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
        //    accountingUnitDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });

        //    var currencyDataTable = new DataTable();
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

        //    if (result.Reports.Count > 0)
        //    {
        //        foreach (var report in result.Reports)
        //        {
        //            reportDataTable.Rows.Add(report.UPODate.ToString("dd/MM/yyyy"), report.UPONo, report.URNNo, report.InvoiceNo, report.SupplierName, report.CategoryName, report.AccountingUnitName, report.DueDate.ToString("dd/MM/yyyy"), report.CurrencyCode, report.TotalSaldo);
        //        }
        //        foreach (var accountingUnitSummary in result.AccountingUnitSummaries)
        //            accountingUnitDataTable.Rows.Add(accountingUnitSummary.AccountingUnitName, accountingUnitSummary.SubTotal);

        //        foreach (var currencySummary in result.CurrencySummaries)
        //            currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.SubTotal, currencySummary.SubTotal);
        //    }

        //    using (var package = new ExcelPackage())
        //    {
        //        var company = "PT EFRATA GARMINDO UTAMA";
        //        var sTitle = isImport ? "IMPOR" : isForeignCurrency ? "LOKAL VALAS" : "LOKAL";
        //        var title = $"LAPORAN SALDO HUTANG (DETAIL) {sTitle}";
        //        var period = $"Periode sampai {dateTo.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy}";

        //        var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
        //        worksheet.Cells["A1"].Value = company;
        //        worksheet.Cells["A2"].Value = title;
        //        worksheet.Cells["A3"].Value = period;
        //        worksheet.Cells["A4"].LoadFromDataTable(reportDataTable, true);
        //        worksheet.Cells[$"A{4 + 3 + result.Reports.Count}"].LoadFromDataTable(accountingUnitDataTable, true);
        //        worksheet.Cells[$"A{4 + result.Reports.Count + 3 + result.AccountingUnitSummaries.Count + 3}"].LoadFromDataTable(currencyDataTable, true);

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);

        //        return stream;
        //    }
        //}

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
                        reportDataTable.Rows.Add(v.UPODate.GetValueOrDefault().ToString("dd/MM/yyyy"), v.UPONo, v.URNNo, v.InvoiceNo, v.SupplierName, v.CategoryName, v.AccountingUnitName, v.DueDate.GetValueOrDefault().ToString("dd/MM/yyyy"), v.CurrencyCode, string.Format("{0:n}", v.Total));
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
                        reportDataTable.Rows.Add("", "", "", "", "", "", "", "Jumlah", totalCurrency.Key, string.Format("{0:n}", totalCurrency.Value));
                        space++;
                    }

                }

                List<SummaryDCB> summaries = new List<SummaryDCB>();

                foreach (var unitSummary in result.AccountingUnitSummaries)
                {
                    if (summaries.Any(x => x.AccountingUnitName == unitSummary.AccountingUnitName))
                        summaries.Add(new SummaryDCB
                        {
                            AccountingUnitName = "",
                            CurrencyCode = unitSummary.CurrencyCode,
                            SubTotal = unitSummary.SubTotal,
                            SubTotalIDR = unitSummary.SubTotalIDR,
                            AccountingLayoutIndex = unitSummary.AccountingLayoutIndex
                        });
                    else
                        summaries.Add(unitSummary);
                }

                List<SummaryDCB> categSummaries = new List<SummaryDCB>();

                foreach (var categorySummary in result.CategorySummaries)
                {
                    if (categSummaries.Any(x => x.CategoryName == categorySummary.CategoryName))
                        categSummaries.Add(new SummaryDCB
                        {
                            CategoryName = "",
                            CurrencyCode = categorySummary.CurrencyCode,
                            SubTotal = categorySummary.SubTotal,
                            SubTotalIDR = categorySummary.SubTotalIDR
                        });
                    else
                    {
                        categSummaries.Add(categorySummary);
                    }
                }

                foreach (var unitSummary in summaries)
                {
                    if (isForeignCurrency || isImport)
                        unitDataTable.Rows.Add(unitSummary.AccountingUnitName, unitSummary.CurrencyCode, unitSummary.SubTotal);
                    else
                        unitDataTable.Rows.Add(unitSummary.AccountingUnitName, unitSummary.SubTotalIDR);
                }

                foreach (var currencySummary in result.CurrencySummaries)
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
                var title = "LAPORAN SALDO HUTANG (DETAIL) LOKAL";
                if (isForeignCurrency)
                    title = "LAPORAN SALDO HUTANG (DETAIL) LOKAL VALAS";
                else if (isImport)
                    title = "LAPORAN SALDO HUTANG (DETAIL) IMPOR";
                //var period = $"Periode sampai {dateTo.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy}";
                var period = $"PERIODE S.D. {dueDateString}";

                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A3"].Value = unitName + separator + divisionName;
                worksheet.Cells["A4"].Value = period;
                worksheet.Cells["A5"].LoadFromDataTable(reportDataTable, true);
                worksheet.Cells[$"A{5 + 3 + result.Reports.Count + space}"].LoadFromDataTable(unitDataTable, true);
                worksheet.Cells[$"A{5 + result.Reports.Count + space + 3 + result.AccountingUnitSummaries.Count + 3}"].LoadFromDataTable(currencyDataTable, true);
                worksheet.Cells[$"F{5 + 3 + result.Reports.Count + space}"].LoadFromDataTable(categoryDataTable, true);

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private DataTable GetFormatReportExcel()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl SPB", DataType = typeof(string) });
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

    }
}