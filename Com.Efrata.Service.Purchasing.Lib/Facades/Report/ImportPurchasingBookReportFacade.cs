using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseOrder;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Report
{
    public class ImportPurchasingBookReportFacade : IImportPurchasingBookReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<UnitReceiptNote> dbSet;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IdentityService _identityService;

        public ImportPurchasingBookReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            //MongoDbContext mongoDbContext = new MongoDbContext();
            //collection = mongoDbContext.UnitReceiptNote;
            //collectionUnitPaymentOrder = mongoDbContext.UnitPaymentOrder;

            //filterBuilder = Builders<BsonDocument>.Filter;
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<UnitReceiptNote>();
            _currencyProvider = (ICurrencyProvider)serviceProvider.GetService(typeof(ICurrencyProvider));
            _identityService = serviceProvider.GetService<IdentityService>();

        }

        //public async Task<LocalPurchasingBookReportViewModel> GetReportData(string no, string unit, string categoryCode, DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
        //    var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

        //    var query = from urnWithItem in dbContext.UnitReceiptNoteItems

        //                join pr in dbContext.PurchaseRequests on urnWithItem.PRId equals pr.Id into joinPurchaseRequest
        //                from urnPR in joinPurchaseRequest.DefaultIfEmpty()

        //                join epoDetail in dbContext.ExternalPurchaseOrderDetails on urnWithItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrder
        //                from urnEPODetail in joinExternalPurchaseOrder.DefaultIfEmpty()

        //                join upoItem in dbContext.UnitPaymentOrderItems on urnWithItem.URNId equals upoItem.URNId into joinUnitPaymentOrder
        //                from urnUPOItem in joinUnitPaymentOrder.DefaultIfEmpty()

        //                where urnWithItem.UnitReceiptNote.ReceiptDate >= d1 && urnWithItem.UnitReceiptNote.ReceiptDate <= d2 && urnWithItem.UnitReceiptNote.SupplierIsImport
        //                select new
        //                {
        //                    // PR Info
        //                    urnPR.CategoryCode,
        //                    urnPR.CategoryName,

        //                    urnWithItem.PRId,
        //                    urnWithItem.UnitReceiptNote.DOId,
        //                    urnWithItem.UnitReceiptNote.DONo,
        //                    urnWithItem.UnitReceiptNote.URNNo,
        //                    URNId = urnWithItem.UnitReceiptNote.Id,
        //                    urnWithItem.ProductName,
        //                    urnWithItem.UnitReceiptNote.ReceiptDate,
        //                    urnWithItem.UnitReceiptNote.SupplierName,
        //                    urnWithItem.UnitReceiptNote.SupplierCode,
        //                    urnWithItem.UnitReceiptNote.UnitCode,
        //                    urnWithItem.UnitReceiptNote.UnitName,
        //                    urnWithItem.EPODetailId,
        //                    urnWithItem.PricePerDealUnit,
        //                    urnWithItem.ReceiptQuantity,
        //                    urnWithItem.Uom,

        //                    // EPO Info
        //                    urnEPODetail.ExternalPurchaseOrderItem.PONo,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
        //                    EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,

        //                    // UPO Info
        //                    urnUPOItem.UnitPaymentOrder.InvoiceNo,
        //                    urnUPOItem.UnitPaymentOrder.UPONo,
        //                    urnUPOItem.UnitPaymentOrder.VatNo,
        //                    urnUPOItem.UnitPaymentOrder.PibNo
        //                };


        //    //var query = dbSet
        //    //    .Where(urn => urn.ReceiptDate >= d1.ToUniversalTime() && urn.ReceiptDate.ToUniversalTime() <= d2 && !urn.SupplierIsImport);

        //    if (!string.IsNullOrWhiteSpace(no))
        //        query = query.Where(urn => urn.URNNo == no);

        //    if (!string.IsNullOrWhiteSpace(unit))
        //        query = query.Where(urn => urn.UnitCode == unit);

        //    //var prIds = query.SelectMany(urn => urn.Items.Select(s => s.PRId)).ToList();

        //    if (!string.IsNullOrWhiteSpace(categoryCode))
        //        query = query.Where(urn => urn.CategoryCode == categoryCode);

        //    var queryResult = query.OrderByDescending(item => item.ReceiptDate).ToList();
        //    //var currencyCodes = queryResult.Select(item => item.CurrencyCode).ToList();
        //    //var receiptDates = queryResult.Select(item => item.ReceiptDate).ToList();
        //    var currencyTuples = queryResult.Select(item => new Tuple<string, DateTimeOffset>(item.CurrencyCode, item.ReceiptDate));
        //    var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

        //    var reportResult = new LocalPurchasingBookReportViewModel();
        //    foreach (var item in queryResult)
        //    {
        //        //var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
        //        //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
        //        //var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
        //        //var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
        //        var currency = currencies.FirstOrDefault(f => f.Code == item.CurrencyCode);

        //        decimal dpp = 0;
        //        decimal dppCurrency = 0;
        //        decimal ppn = 0;

        //        //default IDR
        //        double currencyRate = 1;
        //        var currencyCode = "IDR";
        //        if (currency != null && !currency.Code.Equals("IDR"))
        //        {
        //            dppCurrency = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
        //            currencyRate = currency.Rate.GetValueOrDefault();
        //            currencyCode = currency.Code;
        //        }
        //        else
        //            dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);

        //        if (item.UseVat)
        //            ppn = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity * 0.1);



        //        var reportItem = new PurchasingReport()
        //        {
        //            CategoryName = item.CategoryName,
        //            CategoryCode = item.CategoryCode,
        //            CurrencyRate = (decimal)currencyRate,
        //            DONo = item.DONo,
        //            DPP = dpp,
        //            DPPCurrency = dppCurrency,
        //            InvoiceNo = item.InvoiceNo,
        //            VATNo = item.VatNo,
        //            IPONo = item.PONo,
        //            VAT = ppn,
        //            Total = (dpp + dppCurrency + ppn) * (decimal)currencyRate,
        //            ProductName = item.ProductName,
        //            ReceiptDate = item.ReceiptDate,
        //            SupplierName = item.SupplierCode + " - " + item.SupplierName,
        //            UnitName = item.UnitName,
        //            UPONo = item.UPONo,
        //            URNNo = item.URNNo,
        //            IsUseVat = item.UseVat,
        //            CurrencyCode = currencyCode,
        //            PIBNo = item.PibNo
        //        };
        //        reportItem.PIBBM = reportItem.Total * 0.1m;

        //        reportResult.Reports.Add(reportItem);
        //    }

        //    reportResult.CategorySummaries = reportResult.Reports
        //                .GroupBy(report => new { report.CategoryCode })
        //                .Select(report => new Summary()
        //                {
        //                    Category = report.Key.CategoryCode,
        //                    SubTotal = report.Sum(sum => sum.Total)
        //                }).OrderBy(order => order.Category).ToList();
        //    reportResult.CurrencySummaries = reportResult.Reports
        //        .GroupBy(report => new { report.CurrencyCode })
        //        .Select(report => new Summary()
        //        {
        //            CurrencyCode = report.Key.CurrencyCode,
        //            SubTotal = report.Sum(sum => sum.DPP + sum.DPPCurrency + sum.VAT)
        //        }).OrderBy(order => order.CurrencyCode).ToList();
        //    reportResult.Reports = reportResult.Reports;
        //    reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.Total);
        //    reportResult.CategorySummaryTotal = reportResult.CategorySummaries.Sum(categorySummary => categorySummary.SubTotal);

        //    #region Old Query
        //    //if (prIds.Count > 0)
        //    //{
        //    //    var purchaseRequestQuery = dbContext.PurchaseRequests.AsQueryable();


        //    //    if (purchaseRequestQuery.Count() > 0)
        //    //    {
        //    //        //var purchaseRequests = purchaseRequestQuery.Select(pr => new { pr.Id, pr.CategoryName, pr.CategoryCode }).ToList();
        //    //        //prIds = purchaseRequests.Select(pr => pr.Id).ToList();
        //    //        //var categories = purchaseRequests.Select(pr => pr.CategoryCode).Distinct().ToList();

        //    //        //var urnIds = query.Select(urn => urn.Id).ToList();
        //    //        //var urnItems = dbContext.UnitReceiptNoteItems
        //    //        //    .Include(urnItem => urnItem.UnitReceiptNote)
        //    //        //    .Where(urnItem => urnIds.Contains(urnItem.URNId) && prIds.Contains(urnItem.PRId))
        //    //        //    .Select(urnItem => new
        //    //        //    {
        //    //        //        urnItem.PRId,
        //    //        //        urnItem.UnitReceiptNote.DOId,
        //    //        //        urnItem.UnitReceiptNote.DONo,
        //    //        //        urnItem.UnitReceiptNote.URNNo,
        //    //        //        URNId = urnItem.UnitReceiptNote.Id,
        //    //        //        urnItem.ProductName,
        //    //        //        urnItem.UnitReceiptNote.ReceiptDate,
        //    //        //        urnItem.UnitReceiptNote.SupplierName,
        //    //        //        urnItem.UnitReceiptNote.UnitCode,
        //    //        //        urnItem.EPODetailId,
        //    //        //        urnItem.PricePerDealUnit,
        //    //        //        urnItem.ReceiptQuantity,
        //    //        //        urnItem.Uom
        //    //        //    })
        //    //        //    .ToList();

        //    //        //var epoDetailIds = urnItems.Select(urnItem => urnItem.EPODetailId).ToList();
        //    //        //var epoItemIds = dbContext.ExternalPurchaseOrderDetails
        //    //        //    .Include(epoDetail => epoDetail.ExternalPurchaseOrderItem)
        //    //        //    .Where(epoDetail => epoDetailIds.Contains(epoDetail.Id))
        //    //        //    .Select(epoDetail => epoDetail.ExternalPurchaseOrderItem.Id)
        //    //        //    .ToList();
        //    //        var epoItems = dbContext.ExternalPurchaseOrderItems
        //    //            .Include(epoItem => epoItem.ExternalPurchaseOrder)
        //    //            .Where(epoItem => epoItemIds.Contains(epoItem.Id))
        //    //            .Select(epoItem => new
        //    //            {
        //    //                epoItem.PONo,
        //    //                epoDetailIds = epoItem.Details.Select(epoDetail => epoDetail.Id).ToList(),
        //    //                epoItem.ExternalPurchaseOrder.CurrencyCode,
        //    //                epoItem.ExternalPurchaseOrder.UseVat,
        //    //                Details = epoItem.Details.Select(epoDetail => new { epoDetail.PricePerDealUnit, epoDetail.Id }).ToList()
        //    //            })
        //    //            .ToList();

        //    //        var unitPaymentOrders = dbContext.UnitPaymentOrderItems
        //    //            .Include(upoItem => upoItem.UnitPaymentOrder)
        //    //            .Where(upoItem => urnIds.Contains(upoItem.URNId))
        //    //            .Select(upoItem => new
        //    //            {
        //    //                upoItem.URNId,
        //    //                upoItem.UnitPaymentOrder.InvoiceNo,
        //    //                upoItem.UnitPaymentOrder.UPONo,
        //    //                upoItem.UnitPaymentOrder.VatNo
        //    //            });

        //    //        var currencyCodes = epoItems.Select(epoItem => epoItem.CurrencyCode).Distinct().ToList();
        //    //        var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeList(currencyCodes);

        //    //        var reportResult = new LocalPurchasingBookReportViewModel();
        //    //        foreach (var urnItem in urnItems)
        //    //        {
        //    //            var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
        //    //            var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
        //    //            var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
        //    //            var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
        //    //            var currency = currencies.FirstOrDefault(f => f.Code.Equals(epoItem.CurrencyCode));

        //    //            decimal dpp = 0;
        //    //            decimal dppCurrency = 0;
        //    //            decimal ppn = 0;

        //    //            //default IDR
        //    //            double currencyRate = 1;
        //    //            var currencyCode = "IDR";
        //    //            if (currency != null && !currency.Code.Equals("IDR"))
        //    //            {
        //    //                dppCurrency = (decimal)(epoDetail.PricePerDealUnit * urnItem.ReceiptQuantity);
        //    //                currencyRate = currency.Rate.GetValueOrDefault();
        //    //                currencyCode = currency.Code;
        //    //            }
        //    //            else
        //    //                dpp = (decimal)(epoDetail.PricePerDealUnit * urnItem.ReceiptQuantity);

        //    //            if (epoItem.UseVat)
        //    //                ppn = (decimal)(epoDetail.PricePerDealUnit * urnItem.ReceiptQuantity * 0.1);



        //    //            var reportItem = new PurchasingReport()
        //    //            {
        //    //                CategoryName = purchaseRequest.CategoryName,
        //    //                CategoryCode = purchaseRequest.CategoryCode,
        //    //                CurrencyRate = (decimal)currencyRate,
        //    //                DONo = urnItem.DONo,
        //    //                DPP = dpp,
        //    //                DPPCurrency = dppCurrency,
        //    //                InvoiceNo = unitPaymentOrder?.InvoiceNo,
        //    //                VATNo = unitPaymentOrder?.VatNo,
        //    //                IPONo = epoItem.PONo,
        //    //                VAT = ppn,
        //    //                Total = (dpp + dppCurrency + ppn) * (decimal)currencyRate,
        //    //                ProductName = urnItem.ProductName,
        //    //                ReceiptDate = urnItem.ReceiptDate,
        //    //                SupplierName = urnItem.SupplierName,
        //    //                UnitName = urnItem.UnitCode,
        //    //                UPONo = unitPaymentOrder?.UPONo,
        //    //                URNNo = urnItem.URNNo,
        //    //                IsUseVat = epoItem.UseVat,
        //    //                CurrencyCode = currencyCode,
        //    //                Quantity = urnItem.ReceiptQuantity,
        //    //                Uom = urnItem.Uom
        //    //            };

        //    //            reportResult.Reports.Add(reportItem);
        //    //        }

        //    //        reportResult.CategorySummaries = reportResult.Reports
        //    //            .GroupBy(report => new { report.CategoryCode })
        //    //            .Select(report => new Summary()
        //    //            {
        //    //                Category = report.Key.CategoryCode,
        //    //                SubTotal = report.Sum(sum => sum.Total)
        //    //            }).OrderBy(order => order.Category).ToList();
        //    //        reportResult.CurrencySummaries = reportResult.Reports
        //    //            .GroupBy(report => new { report.CurrencyCode })
        //    //            .Select(report => new Summary()
        //    //            {
        //    //                CurrencyCode = report.Key.CurrencyCode,
        //    //                SubTotal = report.Sum(sum => sum.DPP + sum.DPPCurrency + sum.VAT)
        //    //            }).OrderBy(order => order.CurrencyCode).ToList();
        //    //        reportResult.Reports = reportResult.Reports.OrderByDescending(order => order.ReceiptDate).ToList();
        //    //        reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.Total);
        //    //        reportResult.CategorySummaryTotal = reportResult.CategorySummaries.Sum(categorySummary => categorySummary.SubTotal);

        //    //        return reportResult;
        //    //    }
        //    //}
        //    #endregion

        //    return reportResult;
        //}
        #region oldQuery
        //public async Task<LocalPurchasingBookReportViewModel> GetReportData(string no, string unitCode, string categoryCode, DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
        //    var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

        //    var query = from urnWithItem in dbContext.UnitReceiptNoteItems

        //                join pr in dbContext.PurchaseRequests on urnWithItem.PRId equals pr.Id into joinPurchaseRequest
        //                from urnPR in joinPurchaseRequest.DefaultIfEmpty()

        //                join epoDetail in dbContext.ExternalPurchaseOrderDetails on urnWithItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrder
        //                from urnEPODetail in joinExternalPurchaseOrder.DefaultIfEmpty()

        //                join upoItem in dbContext.UnitPaymentOrderItems on urnWithItem.URNId equals upoItem.URNId into joinUnitPaymentOrder
        //                from urnUPOItem in joinUnitPaymentOrder.DefaultIfEmpty()

        //                where urnWithItem.UnitReceiptNote.ReceiptDate >= d1 && urnWithItem.UnitReceiptNote.ReceiptDate <= d2 && urnWithItem.UnitReceiptNote.SupplierIsImport
        //                select new
        //                {
        //                    // PR Info
        //                    urnPR.CategoryCode,
        //                    urnPR.CategoryName,
        //                    urnPR.CategoryId,

        //                    urnWithItem.PRId,
        //                    urnWithItem.UnitReceiptNote.DOId,
        //                    urnWithItem.UnitReceiptNote.DONo,
        //                    urnWithItem.UnitReceiptNote.URNNo,
        //                    URNId = urnWithItem.UnitReceiptNote.Id,
        //                    urnWithItem.ProductName,
        //                    urnWithItem.UnitReceiptNote.ReceiptDate,
        //                    urnWithItem.UnitReceiptNote.SupplierName,
        //                    urnWithItem.UnitReceiptNote.SupplierCode,
        //                    urnWithItem.UnitReceiptNote.UnitCode,
        //                    urnWithItem.UnitReceiptNote.UnitName,
        //                    urnWithItem.UnitReceiptNote.UnitId,
        //                    urnWithItem.EPODetailId,
        //                    urnWithItem.PricePerDealUnit,
        //                    urnWithItem.ReceiptQuantity,
        //                    urnWithItem.Uom,

        //                    // EPO Info
        //                    urnEPODetail.ExternalPurchaseOrderItem.PONo,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
        //                    EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,

        //                    // UPO Info
        //                    InvoiceNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.InvoiceNo : "",
        //                    UPONo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.UPONo : "",
        //                    VatNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.VatNo : "",
        //                    PibDate = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibDate : new DateTimeOffset(),
        //                    //urnUPOItem.UnitPaymentOrder.PibDate,
        //                    PibNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibNo : "",
        //                    ImportDuty = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportDuty : 0,
        //                    TotalIncomeTaxAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount : 0,
        //                    TotalVatAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalVatAmount : 0,
        //                    ImportInfo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportInfo : "",
        //                    //urnUPOItem.UnitPaymentOrder.InvoiceNo,
        //                    //urnUPOItem.UnitPaymentOrder.UPONo,
        //                    //urnUPOItem.UnitPaymentOrder.VatNo,
        //                    //urnUPOItem.UnitPaymentOrder.PibDate,
        //                    //urnUPOItem.UnitPaymentOrder.PibNo,
        //                    //urnUPOItem.UnitPaymentOrder.ImportDuty,
        //                    //urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount,
        //                    //urnUPOItem.UnitPaymentOrder.TotalVatAmount,
        //                    //urnUPOItem.UnitPaymentOrder.ImportInfo,
        //                    urnPR.Remark
        //                };


        //    //var query = dbSet
        //    //    .Where(urn => urn.ReceiptDate >= d1.ToUniversalTime() && urn.ReceiptDate.ToUniversalTime() <= d2 && !urn.SupplierIsImport);

        //    query = query.Where(urn => urn.CurrencyCode != "IDR");

        //    if (!string.IsNullOrWhiteSpace(no))
        //        query = query.Where(urn => urn.URNNo == no);

        //    if (!string.IsNullOrWhiteSpace(unitCode))
        //        query = query.Where(urn => urn.UnitCode == unitCode);

        //    //var prIds = query.SelectMany(urn => urn.Items.Select(s => s.PRId)).ToList();

        //    if (!string.IsNullOrWhiteSpace(categoryCode))
        //        query = query.Where(urn => urn.CategoryCode == categoryCode);

        //    var queryResult = query.OrderByDescending(item => item.ReceiptDate).ToList();
        //    //var currencyCodes = queryResult.Select(item => item.CurrencyCode).ToList();
        //    //var receiptDates = queryResult.Select(item => item.ReceiptDate).ToList();
        //    var currencyTuples = queryResult.GroupBy(item => new { item.CurrencyCode, item.ReceiptDate }).Select(item => new Tuple<string, DateTimeOffset>(item.Key.CurrencyCode, item.Key.ReceiptDate));
        //    var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

        //    var unitIds = queryResult.Select(item =>
        //    {
        //        int.TryParse(item.UnitId, out var unitId);
        //        return unitId;
        //    }).Distinct().ToList();
        //    var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
        //    var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

        //    var categoryIds = queryResult.Select(item =>
        //    {
        //        int.TryParse(item.CategoryId, out var categoryId);
        //        return categoryId;
        //    }).Distinct().ToList();
        //    var categories = await _currencyProvider.GetCategoriesByCategoryIds(categoryIds);
        //    var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(categoryIds);

        //    var reportResult = new LocalPurchasingBookReportViewModel();
        //    foreach (var item in queryResult)
        //    {
        //        //var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
        //        //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
        //        //var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
        //        //var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
        //        var selectedCurrencies = currencies.Where(element => element.Code == item.CurrencyCode).ToList();
        //        var nearestDate = item.ReceiptDate + selectedCurrencies.Min(x => (x.Date - item.ReceiptDate).Duration());
        //        var currency = selectedCurrencies.FirstOrDefault(element => element.Date.Date == nearestDate.Date);

        //        if (currency == null)
        //            currency = currencies.FirstOrDefault(element => element.Code == currency.Code);

        //        int.TryParse(item.UnitId, out var unitId);
        //        var unit = units.FirstOrDefault(element => element.Id == unitId);
        //        var accountingUnit = new AccountingUnit();
        //        if (unit != null)
        //        {
        //            accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
        //            if (accountingUnit == null)
        //                accountingUnit = new AccountingUnit();
        //        }

        //        int.TryParse(item.CategoryId, out var categoryId);
        //        var category = categories.FirstOrDefault(element => element.Id == categoryId);
        //        var accountingCategory = new AccountingCategory();
        //        if (category != null)
        //        {
        //            accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
        //            if (accountingCategory == null)
        //                accountingCategory = new AccountingCategory();
        //        }

        //        decimal dpp = 0;
        //        decimal dppCurrency = 0;
        //        decimal ppn = 0;
        //        decimal ppnCurrency = 0;

        //        //default IDR
        //        double currencyRate = 1;
        //        var currencyCode = "IDR";

        //        if (item.UseVat)
        //            ppn = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity * 0.1);

        //        if (currency != null && !currency.Code.Equals("IDR"))
        //        {
        //            currencyRate = currency.Rate.GetValueOrDefault();
        //            dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
        //            dppCurrency = dpp * (decimal)currencyRate;
        //            ppnCurrency = ppn * (decimal)currencyRate;
        //            currencyCode = currency.Code;
        //        }
        //        else
        //            dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);

        //        var reportItem = new PurchasingReport()
        //        {
        //            CategoryName = item.CategoryName,
        //            CategoryCode = item.CategoryCode,
        //            AccountingCategoryName = accountingCategory.Name,
        //            AccountingCategoryCode = accountingCategory.Code,
        //            AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
        //            CurrencyRate = (decimal)currencyRate,
        //            DONo = item.DONo,
        //            DPP = dpp,
        //            DPPCurrency = dppCurrency,
        //            InvoiceNo = item.InvoiceNo,
        //            VATNo = item.VatNo,
        //            IPONo = item.PONo,
        //            VAT = ppn,
        //            VATCurrency = ppnCurrency,
        //            Total = dpp * (decimal)currencyRate,
        //            ProductName = item.ProductName,
        //            ReceiptDate = item.ReceiptDate,
        //            SupplierCode = item.SupplierCode,
        //            SupplierName = item.SupplierName,
        //            UnitName = item.UnitName,
        //            UnitCode = item.UnitCode,
        //            AccountingUnitName = accountingUnit.Name,
        //            AccountingUnitCode = accountingUnit.Code,
        //            UPONo = item.UPONo,
        //            URNNo = item.URNNo,
        //            IsUseVat = item.UseVat,
        //            CurrencyCode = currencyCode,
        //            PIBDate = item.PibDate,
        //            PIBNo = item.PibNo,
        //            PIBBM = (decimal)item.ImportDuty,
        //            PIBIncomeTax = (decimal)item.TotalIncomeTaxAmount,
        //            PIBVat = (decimal)item.TotalVatAmount,
        //            PIBImportInfo = item.ImportInfo,
        //            Remark = item.Remark,
        //            Quantity = item.ReceiptQuantity
        //        };

        //        reportResult.Reports.Add(reportItem);
        //    }

        //    reportResult.CategorySummaries = reportResult.Reports
        //                .GroupBy(report => new { report.AccountingCategoryName })
        //                .Select(report => new Summary()
        //                {
        //                    Category = report.Key.AccountingCategoryName,
        //                    SubTotal = report.Sum(sum => sum.Total),
        //                    AccountingLayoutIndex = report.Select(item => item.AccountingLayoutIndex).FirstOrDefault()
        //                }).OrderBy(order => order.AccountingLayoutIndex).ToList();
        //    reportResult.CurrencySummaries = reportResult.Reports
        //        .GroupBy(report => new { report.CurrencyCode })
        //        .Select(report => new Summary()
        //        {
        //            CurrencyCode = report.Key.CurrencyCode,
        //            SubTotal = report.Sum(sum => sum.DPP),
        //            SubTotalCurrency = report.Sum(sum => sum.Total)
        //        }).OrderBy(order => order.CurrencyCode).ToList();
        //    reportResult.Reports = reportResult.Reports;
        //    reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.Total);
        //    reportResult.CategorySummaryTotal = reportResult.CategorySummaries.Sum(categorySummary => categorySummary.SubTotalCurrency);

        //    return reportResult;
        //}

        //public async Task<LocalPurchasingBookReportViewModel> GetReportData(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        //{
        //    var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
        //    var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

        //    var query = from urnWithItem in dbContext.UnitReceiptNoteItems

        //                join pr in dbContext.PurchaseRequests on urnWithItem.PRId equals pr.Id into joinPurchaseRequest
        //                from urnPR in joinPurchaseRequest.DefaultIfEmpty()

        //                join epoDetail in dbContext.ExternalPurchaseOrderDetails on urnWithItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrder
        //                from urnEPODetail in joinExternalPurchaseOrder.DefaultIfEmpty()

        //                join upoItem in dbContext.UnitPaymentOrderItems on urnWithItem.URNId equals upoItem.URNId into joinUnitPaymentOrder
        //                from urnUPOItem in joinUnitPaymentOrder.DefaultIfEmpty()

        //                where urnWithItem.UnitReceiptNote.ReceiptDate >= d1 && urnWithItem.UnitReceiptNote.ReceiptDate <= d2 && urnWithItem.UnitReceiptNote.SupplierIsImport
        //                select new
        //                {
        //                    // PR Info
        //                    urnPR.CategoryCode,
        //                    urnPR.CategoryName,
        //                    urnPR.CategoryId,

        //                    urnWithItem.PRId,
        //                    urnWithItem.UnitReceiptNote.DOId,
        //                    urnWithItem.UnitReceiptNote.DONo,
        //                    urnWithItem.UnitReceiptNote.URNNo,
        //                    URNId = urnWithItem.UnitReceiptNote.Id,
        //                    urnWithItem.ProductName,
        //                    urnWithItem.UnitReceiptNote.ReceiptDate,
        //                    urnWithItem.UnitReceiptNote.SupplierName,
        //                    urnWithItem.UnitReceiptNote.SupplierCode,
        //                    urnWithItem.UnitReceiptNote.UnitCode,
        //                    urnWithItem.UnitReceiptNote.UnitName,
        //                    urnWithItem.UnitReceiptNote.UnitId,
        //                    urnWithItem.UnitReceiptNote.DivisionId,
        //                    urnWithItem.EPODetailId,
        //                    urnWithItem.PricePerDealUnit,
        //                    urnWithItem.ReceiptQuantity,
        //                    urnWithItem.Uom,

        //                    // EPO Info
        //                    urnEPODetail.ExternalPurchaseOrderItem.PONo,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
        //                    EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
        //                    urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,

        //                    // UPO Info
        //                    InvoiceNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.InvoiceNo : "",
        //                    UPONo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.UPONo : "",
        //                    VatNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.VatNo : "",
        //                    PibDate = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibDate : new DateTimeOffset(),
        //                    //urnUPOItem.UnitPaymentOrder.PibDate,
        //                    PibNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibNo : "",
        //                    ImportDuty = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportDuty : 0,
        //                    TotalIncomeTaxAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount : 0,
        //                    TotalVatAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalVatAmount : 0,
        //                    ImportInfo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportInfo : "",
        //                    //urnUPOItem.UnitPaymentOrder.InvoiceNo,
        //                    //urnUPOItem.UnitPaymentOrder.UPONo,
        //                    //urnUPOItem.UnitPaymentOrder.VatNo,
        //                    //urnUPOItem.UnitPaymentOrder.PibDate,
        //                    //urnUPOItem.UnitPaymentOrder.PibNo,
        //                    //urnUPOItem.UnitPaymentOrder.ImportDuty,
        //                    //urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount,
        //                    //urnUPOItem.UnitPaymentOrder.TotalVatAmount,
        //                    //urnUPOItem.UnitPaymentOrder.ImportInfo,
        //                    urnPR.Remark
        //                };


        //    //var query = dbSet
        //    //    .Where(urn => urn.ReceiptDate >= d1.ToUniversalTime() && urn.ReceiptDate.ToUniversalTime() <= d2 && !urn.SupplierIsImport);

        //    query = query.Where(urn => urn.CurrencyCode != "IDR");

        //    if (divisionId > 0)
        //        query = query.Where(urn => urn.DivisionId == divisionId.ToString());

        //    if (!string.IsNullOrWhiteSpace(no))
        //        query = query.Where(urn => urn.URNNo == no);

        //    var unitFilterIds = await _currencyProvider.GetUnitsIdsByAccountingUnitId(accountingUnitId);
        //    if (unitFilterIds.Count() > 0)
        //        query = query.Where(urn => unitFilterIds.Contains(urn.UnitId));

        //    //var prIds = query.SelectMany(urn => urn.Items.Select(s => s.PRId)).ToList();
        //    var categoryFilterIds = await _currencyProvider.GetCategoryIdsByAccountingCategoryId(accountingCategoryId);
        //    if (categoryFilterIds.Count() > 0)
        //        query = query.Where(urn => categoryFilterIds.Contains(urn.CategoryId));

        //    var queryResult = query.OrderByDescending(item => item.ReceiptDate).ToList();
        //    //var currencyCodes = queryResult.Select(item => item.CurrencyCode).ToList();
        //    //var receiptDates = queryResult.Select(item => item.ReceiptDate).ToList();
        //    var currencyTuples = queryResult.Select(item => new Tuple<string, DateTimeOffset>(item.CurrencyCode, item.ReceiptDate));
        //    var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

        //    var unitIds = queryResult.Select(item =>
        //    {
        //        int.TryParse(item.UnitId, out var unitId);
        //        return unitId;
        //    }).Distinct().ToList();
        //    var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
        //    var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

        //    var categoryIds = queryResult.Select(item =>
        //    {
        //        int.TryParse(item.CategoryId, out var categoryId);
        //        return categoryId;
        //    }).Distinct().ToList();
        //    var categories = await _currencyProvider.GetCategoriesByCategoryIds(categoryIds);
        //    var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(categoryIds);

        //    var reportResult = new LocalPurchasingBookReportViewModel();
        //    foreach (var item in queryResult)
        //    {
        //        //var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
        //        //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
        //        //var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
        //        //var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
        //        //var selectedCurrencies = currencies.Where(element => element.Code == item.CurrencyCode).ToList();
        //        //var currency = selectedCurrencies.OrderBy(entity => (entity.Date - item.ReceiptDate).Duration()).FirstOrDefault();
        //        var currency = currencies.Where(entity => entity.Date <= item.ReceiptDate && entity.Code == item.CurrencyCode).OrderByDescending(entity => entity.Date).FirstOrDefault();

        //        if (currency == null)
        //            currency = currencies.FirstOrDefault(element => element.Code == item.CurrencyCode);

        //        int.TryParse(item.UnitId, out var unitId);
        //        var unit = units.FirstOrDefault(element => element.Id == unitId);
        //        var accountingUnit = new AccountingUnit();
        //        if (unit != null)
        //        {
        //            accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
        //            if (accountingUnit == null)
        //                accountingUnit = new AccountingUnit();
        //        }

        //        int.TryParse(item.CategoryId, out var categoryId);
        //        var category = categories.FirstOrDefault(element => element.Id == categoryId);
        //        var accountingCategory = new AccountingCategory();
        //        if (category != null)
        //        {
        //            accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
        //            if (accountingCategory == null)
        //                accountingCategory = new AccountingCategory();
        //        }

        //        decimal dpp = 0;
        //        decimal dppCurrency = 0;
        //        decimal ppn = 0;
        //        decimal ppnCurrency = 0;

        //        //default IDR
        //        double currencyRate = 1;
        //        var currencyCode = "IDR";

        //        if (item.UseVat)
        //            ppn = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity * 0.1);

        //        if (currency != null && !currency.Code.Equals("IDR"))
        //        {
        //            currencyRate = currency.Rate.GetValueOrDefault();
        //            dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
        //            dppCurrency = dpp * (decimal)currencyRate;
        //            ppnCurrency = ppn * (decimal)currencyRate;
        //            currencyCode = currency.Code;
        //        }
        //        else
        //            dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);

        //        var reportItem = new PurchasingReport()
        //        {
        //            CategoryName = item.CategoryName,
        //            CategoryCode = item.CategoryCode,
        //            AccountingCategoryName = accountingCategory.Name,
        //            AccountingCategoryCode = accountingCategory.Code,
        //            AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
        //            CurrencyRate = (decimal)currencyRate,
        //            DONo = item.DONo,
        //            DPP = dpp,
        //            DPPCurrency = dppCurrency,
        //            InvoiceNo = item.InvoiceNo,
        //            VATNo = item.VatNo,
        //            IPONo = item.PONo,
        //            VAT = ppn,
        //            VATCurrency = ppnCurrency,
        //            Total = dpp * (decimal)currencyRate,
        //            ProductName = item.ProductName,
        //            ReceiptDate = item.ReceiptDate,
        //            SupplierCode = item.SupplierCode,
        //            SupplierName = item.SupplierCode + " - " + item.SupplierName,
        //            UnitName = item.UnitName,
        //            UnitCode = item.UnitCode,
        //            AccountingUnitName = accountingUnit.Name,
        //            AccountingUnitCode = accountingUnit.Code,
        //            UPONo = item.UPONo,
        //            URNNo = item.URNNo,
        //            IsUseVat = item.UseVat,
        //            CurrencyCode = currencyCode,
        //            PIBDate = item.PibDate,
        //            PIBNo = item.PibNo,
        //            PIBBM = (decimal)item.ImportDuty,
        //            PIBIncomeTax = (decimal)item.TotalIncomeTaxAmount,
        //            PIBVat = (decimal)item.TotalVatAmount,
        //            PIBImportInfo = item.ImportInfo,
        //            Remark = item.Remark,
        //            Quantity = item.ReceiptQuantity
        //        };

        //        reportResult.Reports.Add(reportItem);
        //    }

        //    reportResult.CategorySummaries = reportResult.Reports
        //                .GroupBy(report => new { report.AccountingCategoryName })
        //                .Select(report => new Summary()
        //                {
        //                    Category = report.Key.AccountingCategoryName,
        //                    SubTotal = report.Sum(sum => sum.Total),
        //                    AccountingLayoutIndex = report.Select(item => item.AccountingLayoutIndex).FirstOrDefault()
        //                }).OrderBy(order => order.AccountingLayoutIndex).ToList();
        //    reportResult.CurrencySummaries = reportResult.Reports
        //        .GroupBy(report => new { report.CurrencyCode })
        //        .Select(report => new Summary()
        //        {
        //            CurrencyCode = report.Key.CurrencyCode,
        //            SubTotal = report.Sum(sum => sum.DPP),
        //            SubTotalCurrency = report.Sum(sum => sum.Total)
        //        }).OrderBy(order => order.CurrencyCode).ToList();
        //    reportResult.Reports = reportResult.Reports;
        //    reportResult.GrandTotal = reportResult.Reports.Sum(sum => sum.Total);
        //    reportResult.CategorySummaryTotal = reportResult.CategorySummaries.Sum(categorySummary => categorySummary.SubTotalCurrency);

        //    return reportResult;
        //}
        #endregion

        public async Task<List<PurchasingReport>> GetReportDataImportPurchasing(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
            var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

            var query = from urnWithItem in dbContext.UnitReceiptNoteItems

                        join pr in dbContext.PurchaseRequests on urnWithItem.PRId equals pr.Id into joinPurchaseRequest
                        from urnPR in joinPurchaseRequest.DefaultIfEmpty()

                        join epoDetail in dbContext.ExternalPurchaseOrderDetails on urnWithItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrder
                        from urnEPODetail in joinExternalPurchaseOrder.DefaultIfEmpty()

                        join upoItem in dbContext.UnitPaymentOrderItems on urnWithItem.URNId equals upoItem.URNId into joinUnitPaymentOrder
                        from urnUPOItem in joinUnitPaymentOrder.DefaultIfEmpty()

                        where urnWithItem.UnitReceiptNote.ReceiptDate >= d1 && urnWithItem.UnitReceiptNote.ReceiptDate <= d2 && urnWithItem.UnitReceiptNote.SupplierIsImport
                        select new
                        {
                            // PR Info
                            urnPR.CategoryCode,
                            urnPR.CategoryName,
                            urnPR.CategoryId,

                            urnWithItem.PRId,
                            urnWithItem.UnitReceiptNote.DOId,
                            urnWithItem.UnitReceiptNote.DONo,
                            urnWithItem.UnitReceiptNote.URNNo,
                            URNId = urnWithItem.UnitReceiptNote.Id,
                            urnWithItem.ProductName,
                            urnWithItem.UnitReceiptNote.ReceiptDate,
                            urnWithItem.UnitReceiptNote.SupplierName,
                            urnWithItem.UnitReceiptNote.SupplierCode,
                            urnWithItem.UnitReceiptNote.UnitCode,
                            urnWithItem.UnitReceiptNote.UnitName,
                            urnWithItem.UnitReceiptNote.UnitId,
                            urnWithItem.UnitReceiptNote.DivisionId,
                            urnWithItem.EPODetailId,
                            urnWithItem.PricePerDealUnit,
                            urnWithItem.ReceiptQuantity,
                            urnWithItem.Uom,

                            // EPO Info
                            urnEPODetail.ExternalPurchaseOrderItem.PONo,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
                            EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,

                            // UPO Info
                            InvoiceNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.InvoiceNo : "",
                            UPONo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.UPONo : "",
                            VatNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.VatNo : "",
                            PibDate = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibDate : DateTimeOffset.MinValue,
                            //urnUPOItem.UnitPaymentOrder.PibDate,
                            PibNo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.PibNo : "",
                            ImportDuty = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportDuty : 0,
                            TotalIncomeTaxAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount : 0,
                            TotalVatAmount = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.TotalVatAmount : 0,
                            ImportInfo = urnUPOItem.UnitPaymentOrder != null ? urnUPOItem.UnitPaymentOrder.ImportInfo : "",
                            //urnUPOItem.UnitPaymentOrder.InvoiceNo,
                            //urnUPOItem.UnitPaymentOrder.UPONo,
                            //urnUPOItem.UnitPaymentOrder.VatNo,
                            //urnUPOItem.UnitPaymentOrder.PibDate,
                            //urnUPOItem.UnitPaymentOrder.PibNo,
                            //urnUPOItem.UnitPaymentOrder.ImportDuty,
                            //urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount,
                            //urnUPOItem.UnitPaymentOrder.TotalVatAmount,
                            //urnUPOItem.UnitPaymentOrder.ImportInfo,
                            urnPR.Remark
                        };


            //var query = dbSet
            //    .Where(urn => urn.ReceiptDate >= d1.ToUniversalTime() && urn.ReceiptDate.ToUniversalTime() <= d2 && !urn.SupplierIsImport);

            query = query.Where(urn => urn.CurrencyCode != "IDR");

            if (divisionId > 0)
                query = query.Where(urn => urn.DivisionId == divisionId.ToString());

            if (!string.IsNullOrWhiteSpace(no))
                query = query.Where(urn => urn.URNNo == no);

            var unitFilterIds = await _currencyProvider.GetUnitsIdsByAccountingUnitId(accountingUnitId);
            if (unitFilterIds.Count() > 0)
                query = query.Where(urn => unitFilterIds.Contains(urn.UnitId));

            //var prIds = query.SelectMany(urn => urn.Items.Select(s => s.PRId)).ToList();
            var categoryFilterIds = await _currencyProvider.GetCategoryIdsByAccountingCategoryId(accountingCategoryId);
            if (categoryFilterIds.Count() > 0)
                query = query.Where(urn => categoryFilterIds.Contains(urn.CategoryId));

            var queryResult = query.OrderByDescending(item => item.ReceiptDate).ToList();
            //var currencyCodes = queryResult.Select(item => item.CurrencyCode).ToList();
            //var receiptDates = queryResult.Select(item => item.ReceiptDate).ToList();
            var currencyTuples = queryResult.Select(item => new Tuple<string, DateTimeOffset>(item.CurrencyCode, item.ReceiptDate));
            var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

            var unitIds = queryResult.Select(item =>
            {
                int.TryParse(item.UnitId, out var unitId);
                return unitId;
            }).Distinct().ToList();
            var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
            var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

            var categoryIds = queryResult.Select(item =>
            {
                int.TryParse(item.CategoryId, out var categoryId);
                return categoryId;
            }).Distinct().ToList();
            var categories = await _currencyProvider.GetCategoriesByCategoryIds(categoryIds);
            var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(categoryIds);

            var reportResult = new List<PurchasingReport>();
            foreach (var item in queryResult)
            {
                //var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
                //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
                //var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
                //var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
                //var selectedCurrencies = currencies.Where(element => element.Code == item.CurrencyCode).ToList();
                //var currency = selectedCurrencies.OrderBy(entity => (entity.Date - item.ReceiptDate).Duration()).FirstOrDefault();
                var currency = currencies.Where(entity => entity.Date.ToUniversalTime() <= item.ReceiptDate && entity.Code == item.CurrencyCode).OrderByDescending(entity => entity.Date.ToUniversalTime()).FirstOrDefault();

                if (currency == null)
                    currency = currencies.FirstOrDefault(element => element.Code == item.CurrencyCode);

                int.TryParse(item.UnitId, out var unitId);
                var unit = units.FirstOrDefault(element => element.Id == unitId);
                var accountingUnit = new AccountingUnit();
                if (unit != null)
                {
                    accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
                    if (accountingUnit == null)
                        accountingUnit = new AccountingUnit();
                }

                int.TryParse(item.CategoryId, out var categoryId);
                var category = categories.FirstOrDefault(element => element.Id == categoryId);
                var accountingCategory = new AccountingCategory();
                if (category != null)
                {
                    accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
                    if (accountingCategory == null)
                        accountingCategory = new AccountingCategory();
                }

                decimal dpp = 0;
                decimal dppCurrency = 0;
                decimal ppn = 0;
                decimal ppnCurrency = 0;

                //default IDR
                double currencyRate = 1;
                var currencyCode = "IDR";
                if (item.CurrencyCode != "IDR")
                {
                    currencyCode = item.CurrencyCode;
                };

                if (item.UseVat)
                    ppn = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity * 0.1);

                if (currency != null && !currency.Code.Equals("IDR"))
                {
                    currencyRate = currency.Rate.GetValueOrDefault();
                    dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
                    dppCurrency = dpp * (decimal)currencyRate;
                    ppnCurrency = ppn * (decimal)currencyRate;
                    currencyCode = currency.Code;
                }
                else
                {
                    dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
                }
                    

                var reportItem = new PurchasingReport()
                {
                    CategoryName = item.CategoryName,
                    CategoryCode = item.CategoryCode,
                    AccountingCategoryName = accountingCategory.Name,
                    AccountingCategoryCode = accountingCategory.Code,
                    AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
                    CurrencyRate = (decimal)currencyRate,
                    DONo = item.DONo,
                    DPP = dpp,
                    DPPCurrency = dppCurrency,
                    InvoiceNo = item.InvoiceNo,
                    VATNo = item.VatNo,
                    IPONo = item.PONo,
                    VAT = ppn,
                    VATCurrency = ppnCurrency,
                    Total = dpp * (decimal)currencyRate,
                    ProductName = item.ProductName,
                    ReceiptDate = item.ReceiptDate,
                    SupplierCode = item.SupplierCode,
                    SupplierName = item.SupplierCode + " - " + item.SupplierName,
                    UnitName = item.UnitName,
                    UnitCode = item.UnitCode,
                    AccountingUnitName = accountingUnit.Name,
                    AccountingUnitCode = accountingUnit.Code,
                    UPONo = item.UPONo,
                    URNNo = item.URNNo,
                    IsUseVat = item.UseVat,
                    CurrencyCode = currencyCode,
                    PIBDate = item.PibDate == DateTimeOffset.MinValue ? (DateTimeOffset?)null : item.PibDate,
                    PIBNo = item.PibNo,
                    PIBBM = (decimal)item.ImportDuty,
                    PIBIncomeTax = (decimal)item.TotalIncomeTaxAmount,
                    PIBVat = (decimal)item.TotalVatAmount,
                    PIBImportInfo = item.ImportInfo,
                    Remark = item.Remark,
                    Quantity = item.ReceiptQuantity,
                    DataSourceSort = 1,
                    CorrectionDate = null
                };

                reportResult.Add(reportItem);
            }
            return reportResult;
        }

        public async Task<List<PurchasingReport>> GetReportDataImportPurchasingCorrection(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
            var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

            var query =
                        from upcCorrectionNoteItem in dbContext.UnitPaymentCorrectionNoteItems

                        join upcCorrectionNote in dbContext.UnitPaymentCorrectionNotes on upcCorrectionNoteItem.UPCId equals upcCorrectionNote.Id into joinupcCorrections
                        from upcCorrection in joinupcCorrections

                        join upoDetailNotes in dbContext.UnitPaymentOrderDetails on upcCorrectionNoteItem.UPODetailId equals upoDetailNotes.Id into joinUpcUpoDetails
                        from joinUpcUpoDetail in joinUpcUpoDetails.DefaultIfEmpty()

                        join urnItem in dbContext.UnitReceiptNoteItems on joinUpcUpoDetail.URNItemId equals urnItem.Id into urnItems
                        from urnWithItem in urnItems

                        join pr in dbContext.PurchaseRequests on urnWithItem.PRId equals pr.Id into joinPurchaseRequest
                        from urnPR in joinPurchaseRequest.DefaultIfEmpty()

                        join epoDetail in dbContext.ExternalPurchaseOrderDetails on urnWithItem.EPODetailId equals epoDetail.Id into joinExternalPurchaseOrder
                        from urnEPODetail in joinExternalPurchaseOrder.DefaultIfEmpty()

                            //join upoItem in dbContext.UnitPaymentOrderItems on urnWithItem.URNId equals upoItem.URNId into joinUnitPaymentOrder
                            //from urnUPOItem in joinUnitPaymentOrder.DefaultIfEmpty()

                            //join upoDetail in dbContext.UnitPaymentOrderDetails on urnEPODetail.Id equals upoDetail.EPODetailId into joinUnitPaymentOrderDetails
                            //from urnUPODetail in joinUnitPaymentOrderDetails.DefaultIfEmpty()

                            //where urnWithItem.UnitReceiptNote.ReceiptDate >= d1 && urnWithItem.UnitReceiptNote.ReceiptDate <= d2 && urnWithItem.UnitReceiptNote.SupplierIsImport
                        where upcCorrection.CorrectionDate.Date >= d1.Date && upcCorrection.CorrectionDate.Date <= d2.Date && urnWithItem.UnitReceiptNote.SupplierIsImport

                        select new
                        {
                            // PR Info
                            urnPR.CategoryCode,
                            urnPR.CategoryName,
                            urnPR.CategoryId,

                            urnWithItem.PRId,
                            urnWithItem.UnitReceiptNote.DOId,
                            urnWithItem.UnitReceiptNote.DONo,
                            urnWithItem.UnitReceiptNote.URNNo,
                            URNId = urnWithItem.UnitReceiptNote.Id,
                            urnWithItem.ProductName,
                            urnWithItem.UnitReceiptNote.ReceiptDate,
                            urnWithItem.UnitReceiptNote.SupplierName,
                            urnWithItem.UnitReceiptNote.SupplierCode,
                            urnWithItem.UnitReceiptNote.UnitCode,
                            urnWithItem.UnitReceiptNote.UnitName,
                            urnWithItem.UnitReceiptNote.UnitId,
                            urnWithItem.UnitReceiptNote.DivisionId,
                            urnWithItem.EPODetailId,
                            urnWithItem.PricePerDealUnit,
                            urnWithItem.ReceiptQuantity,
                            urnWithItem.Uom,

                            // EPO Info
                            urnEPODetail.ExternalPurchaseOrderItem.PONo,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseVat,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.UseIncomeTax,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.IncomeTaxBy,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.IncomeTaxRate,
                            EPOPricePerDealUnit = urnEPODetail.PricePerDealUnit,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyCode,
                            urnEPODetail.ExternalPurchaseOrderItem.ExternalPurchaseOrder.CurrencyRate,

                            // UPO Info
                            InvoiceNo = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.InvoiceNo : "",
                            UPONo = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.UPONo : "",
                            VatNo = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.VatNo : "",
                            PibDate = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.PibDate : new DateTimeOffset(),
                            //urnUPOItem.UnitPaymentOrder.PibDate,
                            PibNo = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.PibNo : "",
                            ImportDuty = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.ImportDuty : 0,
                            TotalIncomeTaxAmount = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.TotalIncomeTaxAmount : 0,
                            TotalVatAmount = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.TotalVatAmount : 0,
                            ImportInfo = joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder != null ? joinUpcUpoDetail.UnitPaymentOrderItem.UnitPaymentOrder.ImportInfo : "",
                            //urnUPOItem.UnitPaymentOrder.InvoiceNo,
                            //urnUPOItem.UnitPaymentOrder.UPONo,
                            //urnUPOItem.UnitPaymentOrder.VatNo,
                            //urnUPOItem.UnitPaymentOrder.PibDate,
                            //urnUPOItem.UnitPaymentOrder.PibNo,
                            //urnUPOItem.UnitPaymentOrder.ImportDuty,
                            //urnUPOItem.UnitPaymentOrder.TotalIncomeTaxAmount,
                            //urnUPOItem.UnitPaymentOrder.TotalVatAmount,
                            //urnUPOItem.UnitPaymentOrder.ImportInfo,
                            urnPR.Remark,

                            //Correction Info
                            CorrectionNo = upcCorrection.UPCNo,
                            upcCorrection.CorrectionDate,
                            upcCorrection.CorrectionType,
                            CorrectionPricePerDealUnitBefore = upcCorrectionNoteItem.PricePerDealUnitBefore,
                            CorrectionPricePerDealUnitAfter = upcCorrectionNoteItem.PricePerDealUnitAfter,
                            CorrectionPriceTotalBefore = upcCorrectionNoteItem.PriceTotalBefore,
                            CorrectionPriceTotalAfter = upcCorrectionNoteItem.PriceTotalAfter,
                            CorrectionQuantity = upcCorrectionNoteItem.Quantity,
                            CorrectionReceiptQuantity = joinUpcUpoDetail.ReceiptQuantity,
                            CorrectionQuantityCorrection = joinUpcUpoDetail.QuantityCorrection,
                            UnitReceiptNoteNo = upcCorrectionNoteItem.URNNo,
                            UPODetailId = upcCorrectionNoteItem.UPODetailId
                        };


            //var query = dbSet
            //    .Where(urn => urn.ReceiptDate >= d1.ToUniversalTime() && urn.ReceiptDate.ToUniversalTime() <= d2 && !urn.SupplierIsImport);

            query = query.Where(urn => urn.CurrencyCode != "IDR");

            if (divisionId > 0)
                query = query.Where(urn => urn.DivisionId == divisionId.ToString());

            if (!string.IsNullOrWhiteSpace(no))
                query = query.Where(urn => urn.URNNo == no);

            var unitFilterIds = await _currencyProvider.GetUnitsIdsByAccountingUnitId(accountingUnitId);
            if (unitFilterIds.Count() > 0)
                query = query.Where(urn => unitFilterIds.Contains(urn.UnitId));

            //var prIds = query.SelectMany(urn => urn.Items.Select(s => s.PRId)).ToList();
            var categoryFilterIds = await _currencyProvider.GetCategoryIdsByAccountingCategoryId(accountingCategoryId);
            if (categoryFilterIds.Count() > 0)
                query = query.Where(urn => categoryFilterIds.Contains(urn.CategoryId));

            var queryResult = query.OrderByDescending(item => item.ReceiptDate).ToList();

            var urnNos = queryResult.Select(element => element.UnitReceiptNoteNo).ToList();
            var correctionItems = dbContext.UnitPaymentCorrectionNoteItems.Where(entity => urnNos.Contains(entity.URNNo)).ToList();
            var correctionIds = correctionItems.Select(element => element.UPCId).ToList();
            var corrections = dbContext.UnitPaymentCorrectionNotes.Where(entity => correctionIds.Contains(entity.Id)).ToList();

            //var currencyCodes = queryResult.Select(item => item.CurrencyCode).ToList();
            //var receiptDates = queryResult.Select(item => item.ReceiptDate).ToList();
            var currencyTuples = queryResult.Select(item => new Tuple<string, DateTimeOffset>(item.CurrencyCode, item.ReceiptDate));
            var currencies = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

            var unitIds = queryResult.Select(item =>
            {
                int.TryParse(item.UnitId, out var unitId);
                return unitId;
            }).Distinct().ToList();
            var units = await _currencyProvider.GetUnitsByUnitIds(unitIds);
            var accountingUnits = await _currencyProvider.GetAccountingUnitsByUnitIds(unitIds);

            var categoryIds = queryResult.Select(item =>
            {   
                int.TryParse(item.CategoryId, out var categoryId);
                return categoryId;
            }).Distinct().ToList();
            var categories = await _currencyProvider.GetCategoriesByCategoryIds(categoryIds);
            var accountingCategories = await _currencyProvider.GetAccountingCategoriesByCategoryIds(categoryIds);

            var reportResult = new List<PurchasingReport>();
            foreach (var item in queryResult)
            {
                //var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(urnItem.PRId));
                //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.URNId.Equals(urnItem.URNId));
                //var epoItem = epoItems.FirstOrDefault(f => f.epoDetailIds.Contains(urnItem.EPODetailId));
                //var epoDetail = epoItem.Details.FirstOrDefault(f => f.Id.Equals(urnItem.EPODetailId));
                //var selectedCurrencies = currencies.Where(element => element.Code == item.CurrencyCode).ToList();
                //var currency = selectedCurrencies.OrderBy(entity => (entity.Date - item.ReceiptDate).Duration()).FirstOrDefault();
                var currency = currencies.Where(entity => entity.Date.ToUniversalTime() <= item.ReceiptDate && entity.Code == item.CurrencyCode).OrderByDescending(entity => entity.Date.ToUniversalTime()).FirstOrDefault();

                if (currency == null)
                    currency = new Currency()
                    {
                        Code = item.CurrencyCode,
                        Rate = item.CurrencyRate
                    };

                int.TryParse(item.UnitId, out var unitId);
                var unit = units.FirstOrDefault(element => element.Id == unitId);
                var accountingUnit = new AccountingUnit();
                if (unit != null)
                {
                    accountingUnit = accountingUnits.FirstOrDefault(element => element.Id == unit.AccountingUnitId);
                    if (accountingUnit == null)
                        accountingUnit = new AccountingUnit();
                }

                int.TryParse(item.CategoryId, out var categoryId);
                var category = categories.FirstOrDefault(element => element.Id == categoryId);
                var accountingCategory = new AccountingCategory();
                if (category != null)
                {
                    accountingCategory = accountingCategories.FirstOrDefault(element => element.Id == category.AccountingCategoryId);
                    if (accountingCategory == null)
                        accountingCategory = new AccountingCategory();
                }

                decimal dpp = 0;
                decimal dppCurrency = 0;
                decimal ppn = 0;
                decimal ppnCurrency = 0;
                decimal incomeTax = 0;
                decimal incomeTaxCurrency = 0;
                decimal total = 0;
                decimal totalCurrency = 0;

                //default IDR
                double currencyRate = 1;
                var currencyCode = "IDR";
                if (item.CurrencyCode != "IDR")
                {
                    currencyCode = item.CurrencyCode;
                };
                decimal.TryParse(item.IncomeTaxRate, out var incomeTaxRate);


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

                if (currency != null && !currency.Code.Equals("IDR"))
                {
                    currencyRate = currency.Rate.GetValueOrDefault();
                    //dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
                    dpp = (decimal)CalculateCorrectionDpp(item.CorrectionType, item.CorrectionPricePerDealUnitBefore, item.CorrectionPricePerDealUnitAfter, item.CorrectionPriceTotalBefore, item.CorrectionPriceTotalAfter, item.CorrectionQuantity, item.CorrectionReceiptQuantity, item.CorrectionQuantityCorrection, item.UPODetailId, corrections, correctionItems, item.CorrectionDate);

                    dppCurrency = dpp * (decimal)currencyRate;
                    currencyCode = currency.Code;
                }
                else
                    //dpp = (decimal)(item.EPOPricePerDealUnit * item.ReceiptQuantity);
                    dpp = (decimal)CalculateCorrectionDpp(item.CorrectionType, item.CorrectionPricePerDealUnitBefore, item.CorrectionPricePerDealUnitAfter, item.CorrectionPriceTotalBefore, item.CorrectionPriceTotalAfter, item.CorrectionQuantity, item.CorrectionReceiptQuantity, item.CorrectionQuantityCorrection, item.UPODetailId, corrections, correctionItems, item.CorrectionDate);

                if (item.UseVat)
                    ppn = (decimal)(dpp * (decimal)0.1);

                if (item.UseIncomeTax && item.IncomeTaxBy == "Supplier")
                    incomeTax = (decimal)(dpp * (incomeTaxRate / 100));

                if (item.IncomeTaxBy == "Supplier")
                {
                    total = dpp + ppn - incomeTax;
                    totalCurrency = (dpp + ppn - incomeTax) * (decimal)currencyRate;
                }
                else
                {
                    total = dpp + ppn;
                    totalCurrency = (dpp + ppn) * (decimal)currencyRate;
                }

                var reportItem = new PurchasingReport()
                {
                    CategoryName = item.CategoryName,
                    CategoryCode = item.CategoryCode,
                    AccountingCategoryName = accountingCategory.Name,
                    AccountingCategoryCode = accountingCategory.Code,
                    AccountingLayoutIndex = accountingCategory.AccountingLayoutIndex,
                    CurrencyRate = (decimal)currencyRate,
                    DONo = item.DONo,
                    DPP = dpp,
                    DPPCurrency = dppCurrency,
                    InvoiceNo = item.InvoiceNo,
                    VATNo = item.VatNo,
                    IPONo = item.PONo,
                    VAT = ppn,
                    VATCurrency = ppnCurrency,
                    Total = dpp * (decimal)currencyRate,
                    ProductName = item.ProductName,
                    ReceiptDate = item.ReceiptDate,
                    SupplierCode = item.SupplierCode,
                    SupplierName = item.SupplierCode + " - " + item.SupplierName,
                    UnitName = item.UnitName,
                    UnitCode = item.UnitCode,
                    AccountingUnitName = accountingUnit.Name,
                    AccountingUnitCode = accountingUnit.Code,
                    UPONo = item.UPONo,
                    URNNo = item.URNNo,
                    IsUseVat = item.UseVat,
                    CurrencyCode = currencyCode,
                    PIBDate = item.PibDate,
                    PIBNo = item.PibNo,
                    PIBBM = (decimal)item.ImportDuty,
                    PIBIncomeTax = (decimal)item.TotalIncomeTaxAmount,
                    PIBVat = (decimal)item.TotalVatAmount,
                    PIBImportInfo = item.ImportInfo,
                    Remark = item.Remark,
                    Quantity = item.ReceiptQuantity,
                    CorrectionDate = item.CorrectionDate,
                    CorrectionNo = item.CorrectionNo,
                    DataSourceSort = 2
                };

                reportResult.Add(reportItem);
            }
            return reportResult;
        }

        private double CalculateCorrectionDpp(string correctionType, double pricePerDealBefore, double priceperDealAfter, double priceTotalBefore, double priceTotalAfter, double quantity, double receiptQuantity, double quantityCorrection, long uPODetailId, List<Models.UnitPaymentCorrectionNoteModel.UnitPaymentCorrectionNote> corrections, List<Models.UnitPaymentCorrectionNoteModel.UnitPaymentCorrectionNoteItem> correctionItems, DateTimeOffset correctionDate)
        {
            var previousCorrection = new PreviousCorrection();

            var upcIds = correctionItems.Where(element => element.UPODetailId == uPODetailId).Select(element => element.UPCId).ToList();
            var previousCorrectionNotes = corrections.Where(element => upcIds.Contains(element.Id) && element.CorrectionDate < correctionDate).OrderBy(element => element.UPCNo).ToList();

            foreach (var previousCorrectionNote in previousCorrectionNotes)
            {
                var previousCorrectionNoteItem = correctionItems.FirstOrDefault(element => element.UPCId == previousCorrectionNote.Id && element.UPODetailId == uPODetailId);

                if (previousCorrectionNote.CorrectionType == "Harga Satuan")
                {
                    previousCorrection.PricePerDealCorrection = previousCorrectionNoteItem.PricePerDealUnitAfter;
                }
                else if (previousCorrectionNote.CorrectionType == "Harga Total")
                {
                    previousCorrection.TotalCorrection = previousCorrectionNoteItem.PriceTotalAfter;
                }
            }

            switch (correctionType)
            {
                case "Harga Satuan":
                    if (previousCorrection.PricePerDealCorrection == 0)
                        return ((priceperDealAfter - pricePerDealBefore) * receiptQuantity);
                    else
                        return ((priceperDealAfter - previousCorrection.PricePerDealCorrection) * receiptQuantity);
                case "Harga Total":
                    if (previousCorrection.TotalCorrection == 0)
                        return priceTotalAfter - priceTotalBefore;
                    else
                        return priceTotalAfter - previousCorrection.TotalCorrection;
                case "Jumlah":
                    //return (priceperDealAfter * (Math.Abs(quantityCorrection-quantity)))*-1 ;
                    return priceTotalAfter * -1;
                default:
                    return 0;
            }
        }

        public async Task<LocalPurchasingBookReportViewModel> GetReportDataV2(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            var dataReceiptNote = Task.Run(() => GetReportDataImportPurchasing(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId)).Result;
            var dataReceiptNoteCorrection = Task.Run(() => GetReportDataImportPurchasingCorrection(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId)).Result;

            var reportReceipt = new List<PurchasingReport>();
            reportReceipt.AddRange(dataReceiptNote);
            reportReceipt.AddRange(dataReceiptNoteCorrection);

            var reportResult = new LocalPurchasingBookReportViewModel();

            reportResult.Reports = reportReceipt.OrderBy(data => data.URNId).OrderBy(data => data.UPONo).ThenBy(data => data.DataSourceSort).ToList();

            reportResult.CategorySummaries = reportResult.Reports
                        .GroupBy(report => new { report.AccountingCategoryName })
                        .Select(report => new Summary()
                        {
                            Category = report.Key.AccountingCategoryName,
                            SubTotal = report.Sum(sum => sum.TotalCurrency),
                            AccountingLayoutIndex = report.Select(item => item.AccountingLayoutIndex).FirstOrDefault()
                        }).OrderBy(order => order.AccountingLayoutIndex).ToList();
            reportResult.CurrencySummaries = reportResult.Reports
                .GroupBy(report => new { report.CurrencyCode })
                .Select(report => new Summary()
                {
                    CurrencyCode = report.Key.CurrencyCode,
                    SubTotal = report.Sum(sum => sum.Total),
                    SubTotalCurrency = report.Sum(sum => sum.TotalCurrency)
                }).OrderBy(order => order.CurrencyCode).ToList();
            return reportResult;

        }

        //public Task<LocalPurchasingBookReportViewModel> GetReport(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo)
        //{
        //    return GetReportData(no, unit, category, dateFrom, dateTo);
        //}

        //public Task<LocalPurchasingBookReportViewModel> GetReport(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        //{
        //    return GetReportData(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
        //}
        public Task<LocalPurchasingBookReportViewModel> GetReportV2(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            return GetReportDataV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
        }

        //public async Task<MemoryStream> GenerateExcel(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var result = await GetReport(no, unit, category, dateFrom, dateTo);
        //    //var Data = reportResult.Reports;
        //    var reportDataTable = new DataTable();
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Bon Penerimaan", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Faktur Pajak", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SPB/NI", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB Tanggal", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB No", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB BM", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPH Impor", DataType = typeof(decimal) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPN Impor", DataType = typeof(decimal) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Ket. Nilai Import", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "DPP Valas", DataType = typeof(decimal) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(decimal) });
        //    reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

        //    var categoryDataTable = new DataTable();
        //    categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
        //    categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });

        //    var currencyDataTable = new DataTable();
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
        //    currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

        //    if (result.Reports.Count > 0)
        //    {
        //        foreach (var report in result.Reports)
        //        {
        //            reportDataTable.Rows.Add(report.ReceiptDate.ToString("dd/MM/yyyy"), report.SupplierName, report.ProductName, report.IPONo, report.DONo, report.URNNo, report.InvoiceNo, report.VATNo, report.UPONo, report.AccountingCategoryName, report.AccountingUnitName, report.PIBDate.ToString("dd/MM/yyyy"), report.PIBNo, report.PIBBM, report.PIBIncomeTax, report.PIBVat, report.PIBImportInfo, report.CurrencyCode,  report.DPP, report.CurrencyRate, report.Total);
        //        }
        //        foreach (var categorySummary in result.CategorySummaries)
        //            categoryDataTable.Rows.Add(categorySummary.Category, categorySummary.SubTotal);

        //        foreach (var currencySummary in result.CurrencySummaries)
        //            currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.SubTotal, currencySummary.SubTotalCurrency);
        //    }

        //    using (var package = new ExcelPackage())
        //    {
        //        var company = "PT EFRATA GARMINDO UTAMA";
        //        var title = "BUKU PEMBELIAN Import";
        //        var period = $"Dari {dateFrom.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy} Sampai {dateTo.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy}";

        //        var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
        //        worksheet.Cells["A1"].Value = company;
        //        worksheet.Cells["A2"].Value = title;
        //        worksheet.Cells["A3"].Value = period;
        //        worksheet.Cells["A4"].LoadFromDataTable(reportDataTable, true);
        //        worksheet.Cells[$"A{4 + 3 + result.Reports.Count}"].LoadFromDataTable(categoryDataTable, true);
        //        worksheet.Cells[$"A{4 + result.Reports.Count + 3 + result.CategorySummaries.Count + 3}"].LoadFromDataTable(currencyDataTable, true);

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);

        //        return stream;
        //    }
        //}

        public async Task<MemoryStream> GenerateExcel(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            var result = await GetReportV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
            //var Data = reportResult.Reports;
            var reportDataTable = new DataTable();
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Bon Penerimaan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No Faktur Pajak", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SPB/NI", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No. Nota Koreksi", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Koreksi", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembelian", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori Pembukuan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit Pembelian", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Unit Pembukuan", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB Tanggal", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB No", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PIB BM", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPH Impor", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "PPN Impor", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Ket. Nilai Import", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "DPP Valas", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(decimal) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

            var categoryDataTable = new DataTable();
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            categoryDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });

            var currencyDataTable = new DataTable();
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(decimal) });
            currencyDataTable.Columns.Add(new DataColumn() { ColumnName = "Total (IDR)", DataType = typeof(decimal) });

            if (result.Reports.Count > 0)
            {
                foreach (var report in result.Reports)
                {
                    var dateReceipt = report.ReceiptDate.HasValue ? report.ReceiptDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
                    var dateCorrection = report.CorrectionDate.HasValue ? report.CorrectionDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
                    var datePib = report.PIBDate.HasValue ? report.PIBDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
                    reportDataTable.Rows.Add(dateReceipt, report.SupplierName, report.ProductName, report.IPONo, report.DONo, report.URNNo, report.InvoiceNo, report.VATNo, report.UPONo, report.CorrectionNo, dateCorrection, report.AccountingCategoryName, report.CategoryName, report.AccountingUnitName, report.UnitName, datePib, report.PIBNo, report.PIBBM, report.PIBIncomeTax, report.PIBVat, report.PIBImportInfo, report.CurrencyCode, report.DPP, report.CurrencyRate, report.Total);
                }
                foreach (var categorySummary in result.CategorySummaries)
                    categoryDataTable.Rows.Add(categorySummary.Category, categorySummary.SubTotal);

                foreach (var currencySummary in result.CurrencySummaries)
                    currencyDataTable.Rows.Add(currencySummary.CurrencyCode, currencySummary.SubTotal, currencySummary.SubTotalCurrency);
            }

            using (var package = new ExcelPackage())
            {
                var company = "PT EFRATA GARMINDO UTAMA";
                var title = "BUKU PEMBELIAN Import";
                var period = $"Dari {dateFrom.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy} Sampai {dateTo.GetValueOrDefault().AddHours(_identityService.TimezoneOffset):dd/MM/yyyy}";

                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A3"].Value = period;

                #region PrintHeader
                var rowStartHeader = 4;
                var colStartHeader = 1;

                foreach (var columns in reportDataTable.Columns)
                {
                    DataColumn column = (DataColumn)columns;
                    if (column.ColumnName.Contains("PIB Tanggal"))
                    {
                        var rowStartHeaderSpan = rowStartHeader + 1;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Value = "Tanggal";
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Font.Bold = true;

                        worksheet.Cells[rowStartHeader, colStartHeader].Value = "PIB";
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 5].Merge = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 5].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    else if (column.ColumnName == "Mata Uang")
                    {
                        var rowStartHeaderSpan = rowStartHeader + 1;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Value = column.ColumnName;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Font.Bold = true;

                        worksheet.Cells[rowStartHeader, colStartHeader].Value = "Pembelian";
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 1].Merge = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 1].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader, colStartHeader + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    }
                    else if (column.ColumnName == "PIB No" ||
                        column.ColumnName == "PIB BM" ||
                        column.ColumnName == "PPH (IDR)" ||
                        column.ColumnName == "PIB BM" ||
                        column.ColumnName == "PPH Impor" ||
                        column.ColumnName == "PPN Impor" ||
                        column.ColumnName == "Ket. Nilai Import" ||
                        column.ColumnName == "DPP Valas")
                    {
                        var rowStartHeaderSpan = rowStartHeader + 1;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Value = column.ColumnName.Replace("PIB ", "");
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeaderSpan, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    else
                    {
                        worksheet.Cells[rowStartHeader, colStartHeader].Value = column.ColumnName;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Merge = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.Font.Bold = true;
                        worksheet.Cells[rowStartHeader, colStartHeader, rowStartHeader + 1, colStartHeader].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);



                    }
                    colStartHeader += 1;
                }
                #endregion

                worksheet.Cells["A6"].LoadFromDataTable(reportDataTable, false);
                for (int i = 6; i < result.Reports.Count + 6; i++)
                {
                    for (int j = 1; j <= reportDataTable.Columns.Count; j++)
                    {
                        worksheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                }
                worksheet.Cells[$"A{6 + 3 + result.Reports.Count}"].LoadFromDataTable(categoryDataTable, true, OfficeOpenXml.Table.TableStyles.Light18);
                worksheet.Cells[$"A{6 + result.Reports.Count + 3 + result.CategorySummaries.Count + 3}"].LoadFromDataTable(currencyDataTable, true, OfficeOpenXml.Table.TableStyles.Light18);

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }
    }

    public interface IImportPurchasingBookReportFacade
    {
        //Task<LocalPurchasingBookReportViewModel> GetReport(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo);
        //Task<LocalPurchasingBookReportViewModel> GetReport(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId);
        Task<LocalPurchasingBookReportViewModel> GetReportV2(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId);
        //Task<MemoryStream> GenerateExcel(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo);
        Task<MemoryStream> GenerateExcel(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId);
    }
}