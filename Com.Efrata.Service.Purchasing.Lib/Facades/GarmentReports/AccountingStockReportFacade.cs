using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.CostCalculationGarment;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Com.Moonlay.NetCore.Lib;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class AccountingStockReportFacade : IAccountingStockReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public AccountingStockReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        public Tuple<List<AccountingStockReportViewModel>, int> GetStockReport(int offset, string unitcode, string planPo, string tipebarang, int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo)
        {
            
            List<AccountingStockReportViewModel> Query = GetStockQuery(tipebarang, unitcode, planPo, dateFrom, dateTo, offset);
            //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

            //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
            Pageable<AccountingStockReportViewModel> pageable = new Pageable<AccountingStockReportViewModel>(Query, page - 1, size);
            List<AccountingStockReportViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }
        public List<AccountingStockReportViewModel> GetStockQuery(string ctg, string unitcode, string planPo, DateTime? datefrom, DateTime? dateto, int offset)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            string filter = (string.IsNullOrWhiteSpace(ctg) ? "{}" : "{" + "'" + "CodeRequirement" + "'" + ":" + "'" + ctg + "'" + "}");

            var categories = GetProductCodes(1, int.MaxValue, "{}", filter);

            var categories1 = categories.Select(x => x.Name).ToArray();

            //var categories1 = ctg == "BB" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : ctg == "BP" ? categories.Where(x => x.CodeRequirement == "BP").Select(x => x.Name).ToArray() : ctg == "BE" ? categories.Where(x => x.CodeRequirement == "BE").Select(x => x.Name).ToArray() : categories.Select(x=>x.Name).ToArray();

            //string filter = ctg == "BB" ? "{" + "'" + "ProductType" + "'" + ":" + "'FABRIC'" + "}" : "{" + "'" + "ProductType" + "'" + ":" + "'NON FABRIC'" + "}";

            //var product = GetProductCode(1, int.MaxValue, "{}", filter);

            //var Codes = product.Where(x => categories1.Contains(x.Name)).ToList();

            //var lastdate = dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault() == null ? new DateTime(1970, 1, 1) : dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();


            //var lastdate = ctg == "BB" ? dbContext.BalanceStocks.Where(x => x.PeriodeYear == "2018").OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault() : dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();
            var lastdate = dbContext.GarmentStockOpnames.OrderByDescending(x => x.Date).Select(x => x.Date).FirstOrDefault();

            var BalanceStock = (from a in dbContext.GarmentStockOpnames
                                join b in dbContext.GarmentStockOpnameItems on a.Id equals b.GarmentStockOpnameId
                                join c in dbContext.GarmentDOItems on b.DOItemId equals c.Id
                                join d in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals d.Id
                                join f in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals f.Id
                                join h in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on f.GarmentEPOId equals h.Id
                                join g in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RO equals g.RONo into PR
                                from prs in PR.DefaultIfEmpty()
                                where a.IsDeleted == false && b.IsDeleted == false
                                && a.Date.Date == lastdate.Date
                                && c.CreatedUtc.Year <= DateTo.Date.Year
                                && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                                && categories1.Contains(b.ProductName)
                                && b.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? b.POSerialNumber : planPo)
                                select new AccountingStockTempViewModel
                                {
                                    ProductCode = b.ProductCode.Trim(),
                                    //ProductName = b.ProductName,
                                    RO = b.RO,
                                    Buyer = prs != null ? prs.BuyerCode.Trim() : "-",
                                    PlanPo = b.POSerialNumber.Trim(),
                                    NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                                    BeginningBalanceQty = b.Quantity,
                                    BeginningBalanceUom = b.SmallUomUnit.Trim(),
                                    BeginningBalancePrice = Math.Round((((d.PricePerDealUnit) / (d.Conversion == 0 ? 1 : (d.Conversion))) * (decimal)d.DOCurrencyRate) * (b.Quantity), 2, MidpointRounding.AwayFromZero),
                                    ReceiptCorrectionQty = 0,
                                    ReceiptPurchaseQty = 0,
                                    ReceiptProcessQty = 0,
                                    ReceiptKon2AQty = 0,
                                    ReceiptKon2BQty = 0,
                                    ReceiptKon2CQty = 0,
                                    ReceiptKon1AQty = 0,
                                    ReceiptKon1BQty = 0,
                                    ReceiptCorrectionPrice = 0,
                                    ReceiptPurchasePrice = 0,
                                    ReceiptProcessPrice = 0,
                                    ReceiptKon2APrice = 0,
                                    ReceiptKon2BPrice = 0,
                                    ReceiptKon2CPrice = 0,
                                    ReceiptKon1APrice = 0,
                                    ReceiptKon1BPrice = 0,
                                    ExpendReturQty = 0,
                                    ExpendRestQty = 0,
                                    ExpendProcessQty = 0,
                                    ExpendSampleQty = 0,
                                    ExpendKon2AQty = 0,
                                    ExpendKon2BQty = 0,
                                    ExpendKon2CQty = 0,
                                    ExpendKon1AQty = 0,
                                    ExpendKon1BQty = 0,
                                    ExpendReturPrice = 0,
                                    ExpendRestPrice = 0,
                                    ExpendProcessPrice = 0,
                                    ExpendSamplePrice = 0,
                                    ExpendKon2APrice = 0,
                                    ExpendKon2BPrice = 0,
                                    ExpendKon2CPrice = 0,
                                    ExpendKon1APrice = 0,
                                    ExpendKon1BPrice = 0,
                                    EndingBalanceQty = 0,
                                    EndingBalancePrice = 0,
                                    ExpendOtherPrice=0,
                                    ExpendOtherQty=0,
                                    ExpendSubconPrice=0,
                                    ExpendSubconQty=0,
                                    ExpendTransferPrice=0,
                                    ExpendTransferQty=0
                                }).GroupBy(x => new { x.ProductCode, x.RO, x.Buyer, x.PlanPo, x.NoArticle, x.BeginningBalanceUom }, (key, group) => new AccountingStockTempViewModel
                                {
                                    ProductCode = key.ProductCode,
                                    //ProductName = b.ProductName,
                                    RO = key.RO,
                                    Buyer = key.Buyer,
                                    PlanPo = key.PlanPo,
                                    NoArticle = key.NoArticle,
                                    BeginningBalanceQty = Math.Round(group.Sum(x => x.BeginningBalanceQty), 2),
                                    BeginningBalanceUom = key.BeginningBalanceUom,
                                    BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                                    ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                                    ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                                    ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                                    ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                                    ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                                    ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                                    ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                                    ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                                    ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                                    ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                                    ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                                    ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                                    ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                                    ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                                    ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                                    ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                                    ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                                    ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                                    ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                                    ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                                    ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                                    ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                                    ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                                    ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                                    ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                                    ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                                    ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                                    ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                                    ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                                    ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                                    ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                                    ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                                    ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                                    ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                                    EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                                    EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                                    ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                                    ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                                    ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                                    ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                                    ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                                    ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                                });
            //var BalanceStock = (from a in dbContext.BalanceStocks
            //                    join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on (long)a.EPOItemId equals b.Id
            //                    join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
            //                    join e in dbContext.GarmentUnitReceiptNoteItems on (long)a.EPOItemId equals e.EPOItemId
            //                    join f in dbContext.GarmentUnitReceiptNotes on e.URNId equals f.Id
            //                    join g in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RO equals g.RONo
            //                    //join h in Codes on b.ProductCode equals h.Code
            //                    where a.CreateDate.Value.Date == lastdate
            //                    && f.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? f.UnitCode : unitcode)
            //                    && f.URNType == "PEMBELIAN"
            //                    && categories1.Contains(b.ProductName)

            //                    select new AccountingStockTempViewModel
            //                    {
            //                        ProductCode = b.ProductCode,
            //                        //ProductName = b.ProductName,
            //                        RO = a.RO,
            //                        Buyer = g.BuyerCode,
            //                        PlanPo = b.PO_SerialNumber,
            //                        NoArticle = g.Article,
            //                        BeginningBalanceQty = Convert.ToDecimal(a.CloseStock),
            //                        BeginningBalanceUom = b.SmallUomUnit,
            //                        BeginningBalancePrice = Convert.ToDouble(a.ClosePrice),
            //                        ReceiptCorrectionQty = 0,
            //                        ReceiptPurchaseQty = 0,
            //                        ReceiptProcessQty = 0,
            //                        ReceiptKon2AQty = 0,
            //                        ReceiptKon2BQty = 0,
            //                        ReceiptKon2CQty = 0,
            //                        ReceiptKon1AQty = 0,
            //                        ReceiptKon1BQty = 0,
            //                        ReceiptCorrectionPrice = 0,
            //                        ReceiptPurchasePrice = 0,
            //                        ReceiptProcessPrice = 0,
            //                        ReceiptKon2APrice = 0,
            //                        ReceiptKon2BPrice = 0,
            //                        ReceiptKon2CPrice = 0,
            //                        ReceiptKon1APrice = 0,
            //                        ReceiptKon1BPrice = 0,
            //                        ExpendReturQty = 0,
            //                        ExpendRestQty = 0,
            //                        ExpendProcessQty = 0,
            //                        ExpendSampleQty = 0,
            //                        ExpendKon2AQty = 0,
            //                        ExpendKon2BQty = 0,
            //                        ExpendKon2CQty = 0,
            //                        ExpendKon1AQty = 0,
            //                        ExpendKon1BQty = 0,
            //                        ExpendReturPrice = 0,
            //                        ExpendRestPrice = 0,
            //                        ExpendProcessPrice = 0,
            //                        ExpendSamplePrice = 0,
            //                        ExpendKon2APrice = 0,
            //                        ExpendKon2BPrice = 0,
            //                        ExpendKon2CPrice = 0,
            //                        ExpendKon1APrice = 0,
            //                        ExpendKon1BPrice = 0,
            //                        EndingBalanceQty = 0,
            //                        EndingBalancePrice = 0,
            //                    }).Distinct();
            var SATerima = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems  select aa)
                            join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                            join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                            join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                            join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                            from prs in PR.DefaultIfEmpty()
                                //join h in Codes on a.ProductCode equals h.Code
                            where a.IsDeleted == false && b.IsDeleted == false
                              &&
                              b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                              && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                              && b.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? b.UnitCode : unitcode)
                              && categories1.Contains(a.ProductName)
                              && a.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? a.POSerialNumber : planPo)
                            select new AccountingStockTempViewModel
                            {
                                ProductCode = a.ProductCode.Trim(),
                                //ProductName = a.ProductName,
                                RO = a.RONo,
                                Buyer = prs != null ? prs.BuyerCode.Trim() : "-",
                                PlanPo = a.POSerialNumber.Trim(),
                                NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                                BeginningBalanceQty = Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero),
                                BeginningBalanceUom = a.SmallUomUnit.Trim(),
                                BeginningBalancePrice = Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion))) * (decimal)a.DOCurrencyRate * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero),
                                ReceiptCorrectionQty = 0,
                                ReceiptPurchaseQty = 0,
                                ReceiptProcessQty = 0,
                                ReceiptKon2AQty = 0,
                                ReceiptKon2BQty = 0,
                                ReceiptKon2CQty = 0,
                                ReceiptKon1AQty = 0,
                                ReceiptKon1BQty = 0,
                                ReceiptCorrectionPrice = 0,
                                ReceiptPurchasePrice = 0,
                                ReceiptProcessPrice = 0,
                                ReceiptKon2APrice = 0,
                                ReceiptKon2BPrice = 0,
                                ReceiptKon2CPrice = 0,
                                ReceiptKon1APrice = 0,
                                ReceiptKon1BPrice = 0,
                                ExpendReturQty = 0,
                                ExpendRestQty = 0,
                                ExpendProcessQty = 0,
                                ExpendSampleQty = 0,
                                ExpendKon2AQty = 0,
                                ExpendKon2BQty = 0,
                                ExpendKon2CQty = 0,
                                ExpendKon1AQty = 0,
                                ExpendKon1BQty = 0,
                                ExpendReturPrice = 0,
                                ExpendRestPrice = 0,
                                ExpendProcessPrice = 0,
                                ExpendSamplePrice = 0,
                                ExpendKon2APrice = 0,
                                ExpendKon2BPrice = 0,
                                ExpendKon2CPrice = 0,
                                ExpendKon1APrice = 0,
                                ExpendKon1BPrice = 0,
                                EndingBalanceQty = 0,
                                EndingBalancePrice = 0,
                                ExpendOtherPrice = 0,
                                ExpendOtherQty = 0,
                                ExpendSubconPrice = 0,
                                ExpendSubconQty = 0,
                                ExpendTransferPrice = 0,
                                ExpendTransferQty = 0
                            }).GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                            {
                                ProductCode = key.ProductCode,
                                //ProductName = key.ProductName,
                                RO = key.RO,
                                Buyer = key.Buyer,
                                PlanPo = key.PlanPo,
                                NoArticle = key.NoArticle,
                                BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                                BeginningBalanceUom = key.BeginningBalanceUom,
                                BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                                ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                                ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                                ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                                ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                                ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                                ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                                ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                                ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                                ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                                ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                                ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                                ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                                ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                                ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                                ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                                ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                                ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                                ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                                ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                                ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                                ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                                ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                                ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                                ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                                ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                                ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                                ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                                ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                                ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                                ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                                ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                                ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                                ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                                ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                                EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                                EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                                ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                                ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                                ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                                ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                                ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                                ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                            }).ToList();


            var SAKeluar = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                            join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                            join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                            join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                            //join h in Codes on a.ProductCode equals h.Code
                            join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                            from prs in PR.DefaultIfEmpty()
                            join f in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals f.Id into urnitems
                            from urnitem in urnitems.DefaultIfEmpty()
                            where
                            a.IsDeleted == false && b.IsDeleted == false
                               &&
                               b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                               && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                               && 
                               b.UnitSenderCode == (string.IsNullOrWhiteSpace(unitcode) ? b.UnitSenderCode : unitcode)
                               && categories1.Contains(a.ProductName)
                               && a.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? a.POSerialNumber : planPo)
                            select new AccountingStockTempViewModel
                            {
                                ProductCode = a.ProductCode.Trim(),
                                //ProductName = a.ProductName,
                                RO = a.RONo,
                                Buyer = a.BuyerCode == null ? "-" : a.BuyerCode.Trim(),
                                PlanPo = a.POSerialNumber.Trim(),
                                NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                                BeginningBalanceQty = a.UomUnit == "YARD" && ctg == "BB" ? Convert.ToDecimal(a.Quantity * -1 * 0.9144) : (b.ExpenditureType == "EXTERNAL" && a.UomUnit != "PCS" && ctg != "BB") ? Convert.ToDecimal(a.Quantity) * urnitem.Conversion * -1 : -1 * Convert.ToDecimal(a.Quantity),
                                BeginningBalanceUom = a.UomUnit == "YARD" && ctg == "BB" ? "MT" : b.ExpenditureType == "EXTERNAL" ? urnitem.SmallUomUnit : a.UomUnit.Trim(),
                                BeginningBalancePrice = Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? Convert.ToDecimal(a.Quantity * 0.9144) : b.ExpenditureType == "EXTERNAL" ? Convert.ToDecimal(a.Quantity * (urnitem != null ? (double)urnitem.Conversion : 1)) : (decimal)a.Quantity) * (decimal)(a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate, 2,MidpointRounding.AwayFromZero) * -1,
                                ReceiptCorrectionQty = 0,
                                ReceiptPurchaseQty = 0,
                                ReceiptProcessQty = 0,
                                ReceiptKon2AQty = 0,
                                ReceiptKon2BQty = 0,
                                ReceiptKon2CQty = 0,
                                ReceiptKon1AQty = 0,
                                ReceiptKon1BQty = 0,
                                ReceiptCorrectionPrice = 0,
                                ReceiptPurchasePrice = 0,
                                ReceiptProcessPrice = 0,
                                ReceiptKon2APrice = 0,
                                ReceiptKon2BPrice = 0,
                                ReceiptKon2CPrice = 0,
                                ReceiptKon1APrice = 0,
                                ReceiptKon1BPrice = 0,
                                ExpendReturQty = 0,
                                ExpendRestQty = 0,
                                ExpendProcessQty = 0,
                                ExpendSampleQty = 0,
                                ExpendKon2AQty = 0,
                                ExpendKon2BQty = 0,
                                ExpendKon2CQty = 0,
                                ExpendKon1AQty = 0,
                                ExpendKon1BQty = 0,
                                ExpendReturPrice = 0,
                                ExpendRestPrice = 0,
                                ExpendProcessPrice = 0,
                                ExpendSamplePrice = 0,
                                ExpendKon2APrice = 0,
                                ExpendKon2BPrice = 0,
                                ExpendKon2CPrice = 0,
                                ExpendKon1APrice = 0,
                                ExpendKon1BPrice = 0,
                                EndingBalanceQty = 0,
                                EndingBalancePrice = 0,
                                ExpendOtherPrice = 0,
                                ExpendOtherQty = 0,
                                ExpendSubconPrice = 0,
                                ExpendSubconQty = 0,
                                ExpendTransferPrice = 0,
                                ExpendTransferQty = 0
                            })
                            .GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                            {
                                ProductCode = key.ProductCode,
                                //ProductName = key.ProductName,
                                RO = key.RO,
                                Buyer = key.Buyer,
                                PlanPo = key.PlanPo,
                                NoArticle = key.NoArticle,
                                BeginningBalanceQty = Math.Round(group.Sum(x => x.BeginningBalanceQty), 2),
                                BeginningBalanceUom = key.BeginningBalanceUom,
                                BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                                ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                                ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                                ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                                ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                                ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                                ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                                ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                                ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                                ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                                ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                                ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                                ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                                ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                                ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                                ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                                ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                                ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                                ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                                ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                                ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                                ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                                ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                                ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                                ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                                ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                                ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                                ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                                ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                                ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                                ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                                ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                                ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                                ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                                ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                                EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                                EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                                ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                                ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                                ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                                ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                                ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                                ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                            }).ToList();

            var SAKoreksi = (from a in dbContext.GarmentUnitReceiptNotes
                             join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                             join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                             join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                             join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                             join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                             join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                             from prs in PR.DefaultIfEmpty()
                                 //join h in Codes on b.ProductCode equals h.Code
                             where
                             a.IsDeleted == false && b.IsDeleted == false
                             &&
                             g.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                             && g.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                             && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                             && categories1.Contains(b.ProductName)
                             && b.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? b.POSerialNumber : planPo)
                             select new AccountingStockTempViewModel
                             {
                                 ProductCode = b.ProductCode.Trim(),
                                 //ProductName = b.ProductName,
                                 RO = b.RONo,
                                 Buyer = prs != null ? prs.BuyerCode.Trim() : "-",
                                 PlanPo = b.POSerialNumber.Trim(),
                                 NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                                 BeginningBalanceQty = Math.Round((decimal)e.SmallQuantity, 2, MidpointRounding.AwayFromZero),
                                 BeginningBalanceUom = b.SmallUomUnit.Trim(),
                                 BeginningBalancePrice = Convert.ToDecimal(Math.Round(((e.PricePerDealUnit / (e.Conversion == 0 ? 1 : e.Conversion)) * b.DOCurrencyRate) * (e.SmallQuantity), 2, MidpointRounding.AwayFromZero)),
                                 ReceiptCorrectionQty = 0,
                                 ReceiptPurchaseQty = 0,
                                 ReceiptProcessQty = 0,
                                 ReceiptKon2AQty = 0,
                                 ReceiptKon2BQty = 0,
                                 ReceiptKon2CQty = 0,
                                 ReceiptKon1AQty = 0,
                                 ReceiptKon1BQty = 0,
                                 ReceiptCorrectionPrice = 0,
                                 ReceiptPurchasePrice = 0,
                                 ReceiptProcessPrice = 0,
                                 ReceiptKon2APrice = 0,
                                 ReceiptKon2BPrice = 0,
                                 ReceiptKon2CPrice = 0,
                                 ReceiptKon1APrice = 0,
                                 ReceiptKon1BPrice = 0,
                                 ExpendReturQty = 0,
                                 ExpendRestQty = 0,
                                 ExpendProcessQty = 0,
                                 ExpendSampleQty = 0,
                                 ExpendKon2AQty = 0,
                                 ExpendKon2BQty = 0,
                                 ExpendKon2CQty = 0,
                                 ExpendKon1AQty = 0,
                                 ExpendKon1BQty = 0,
                                 ExpendReturPrice = 0,
                                 ExpendRestPrice = 0,
                                 ExpendProcessPrice = 0,
                                 ExpendSamplePrice = 0,
                                 ExpendKon2APrice = 0,
                                 ExpendKon2BPrice = 0,
                                 ExpendKon2CPrice = 0,
                                 ExpendKon1APrice = 0,
                                 ExpendKon1BPrice = 0,
                                 EndingBalanceQty = 0,
                                 EndingBalancePrice = 0,
                                 ExpendOtherPrice = 0,
                                 ExpendOtherQty = 0,
                                 ExpendSubconPrice = 0,
                                 ExpendSubconQty = 0,
                                 ExpendTransferPrice = 0,
                                 ExpendTransferQty = 0
                             }).GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                             {
                                 ProductCode = key.ProductCode,
                                 //ProductName = key.ProductName,
                                 RO = key.RO,
                                 Buyer = key.Buyer,
                                 PlanPo = key.PlanPo,
                                 NoArticle = key.NoArticle,
                                 BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                                 BeginningBalanceUom = key.BeginningBalanceUom,
                                 BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                                 ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                                 ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                                 ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                                 ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                                 ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                                 ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                                 ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                                 ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                                 ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                                 ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                                 ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                                 ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                                 ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                                 ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                                 ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                                 ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                                 ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                                 ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                                 ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                                 ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                                 ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                                 ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                                 ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                                 ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                                 ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                                 ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                                 ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                                 ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                                 ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                                 ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                                 ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                                 ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                                 ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                                 ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                                 EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                                 EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                                 ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                                 ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                                 ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                                 ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                                 ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                                 ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                             }).ToList();

            var SaldoAwal1 = BalanceStock.Concat(SATerima).Concat(SAKeluar).Concat(SAKoreksi).AsEnumerable();
            var SaldoAwal = SaldoAwal1.GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, /*x.Buyer,*/ x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
            {
                ProductCode = key.ProductCode,
                //ProductName = key.ProductName,
                RO = key.RO,
                Buyer = group.FirstOrDefault().Buyer,
                PlanPo = key.PlanPo,
                NoArticle = key.NoArticle,
                BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                BeginningBalanceUom = key.BeginningBalanceUom,
                //BeginningBalancePrice = group.Sum(x => x.BeginningBalanceQty),
                BeginningBalancePrice = group.Sum(x => x.BeginningBalanceQty) == 0 ? 0: group.Sum(x => x.BeginningBalancePrice), //  BeginningBalanceQty = 0, then BeginningBalancePrice = 0
                ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
            }).ToList();

            var Terima = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems  select aa)
                          join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                          join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                          join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                          join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                          from prs in PR.DefaultIfEmpty()
                          join g in dbContext.GarmentUnitExpenditureNotes on b.UENId equals g.Id into UEN
                          from dd in UEN.DefaultIfEmpty()
                          //join h in Codes on a.ProductCode equals h.Code
                          where a.IsDeleted == false && b.IsDeleted == false
                            &&
                            b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                            && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                            && b.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? b.UnitCode : unitcode)
                            && categories1.Contains(a.ProductName)
                            && a.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? a.POSerialNumber : planPo)
                          select new AccountingStockTempViewModel
                        {
                            ProductCode = a.ProductCode.Trim(),
                            //ProductName = a.ProductName,
                            RO = a.RONo,
                            Buyer = prs != null ? prs.BuyerCode.Trim() : "-",
                            PlanPo = a.POSerialNumber.Trim(),
                            NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                            BeginningBalanceQty = 0,
                            BeginningBalanceUom = a.SmallUomUnit.Trim(),
                            BeginningBalancePrice = 0,
                            ReceiptCorrectionQty = 0,
                            ReceiptPurchaseQty = b.URNType == "PEMBELIAN" ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            ReceiptProcessQty = b.URNType == "PROSES" ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2AQty = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2A")*/ dd.UnitSenderCode == "C2A" ) ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2BQty = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2B")*/ dd.UnitSenderCode == "C2B") ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2CQty = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2C")*/ dd.UnitSenderCode == "C2C") ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon1AQty = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C1A")*/ dd.UnitSenderCode == "C1A") ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon1BQty = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C1B")*/ dd.UnitSenderCode == "C1B") ? Math.Round(a.ReceiptQuantity * a.Conversion, 2, MidpointRounding.AwayFromZero) : 0,
                            ReceiptCorrectionPrice = 0,
                            ReceiptPurchasePrice = b.URNType == "PEMBELIAN" ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            ReceiptProcessPrice = b.URNType == "PROSES" ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2APrice = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2A")*/ dd.UnitSenderCode == "C2A") ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2BPrice = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2B")*/ dd.UnitSenderCode == "C2B") ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon2CPrice = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C2C")*/ dd.UnitSenderCode == "C2C") ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon1APrice = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C1A")*/ dd.UnitSenderCode == "C1A") ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            //ReceiptKon1BPrice = b.URNType == "GUDANG LAIN" && (/*b.UId == null ?*/ /*b.UENNo.Contains("C1B")*/ dd.UnitSenderCode == "C1B") ? Math.Round(((a.PricePerDealUnit / (a.Conversion == 0 ? 1 : a.Conversion)) * (decimal)a.DOCurrencyRate) * (a.ReceiptQuantity * a.Conversion), 2, MidpointRounding.AwayFromZero) : 0,
                            ExpendReturQty = 0,
                            ExpendRestQty = 0,
                            ExpendProcessQty = 0,
                            ExpendSampleQty = 0,
                            ExpendKon2AQty = 0,
                            ExpendKon2BQty = 0,
                            ExpendKon2CQty = 0,
                            ExpendKon1AQty = 0,
                            ExpendKon1BQty = 0,
                            ExpendReturPrice = 0,
                            ExpendRestPrice = 0,
                            ExpendProcessPrice = 0,
                            ExpendSamplePrice = 0,
                            ExpendKon2APrice = 0,
                            ExpendKon2BPrice = 0,
                            ExpendKon2CPrice = 0,
                            ExpendKon1APrice = 0,
                            ExpendKon1BPrice = 0,
                            EndingBalanceQty = 0,
                            EndingBalancePrice = 0,
                            ExpendOtherPrice = 0,
                            ExpendOtherQty = 0,
                            ExpendSubconPrice = 0,
                            ExpendSubconQty = 0,
                            ExpendTransferPrice = 0,
                            ExpendTransferQty = 0
                          }).GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                        {
                            ProductCode = key.ProductCode,
                            //ProductName = key.ProductName,
                            RO = key.RO,
                            Buyer = key.Buyer,
                            PlanPo = key.PlanPo,
                            NoArticle = key.NoArticle,
                            BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                            BeginningBalanceUom = key.BeginningBalanceUom,
                            BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                            ReceiptCorrectionQty = Math.Round(group.Sum(x => x.ReceiptCorrectionQty), 2),
                            ReceiptPurchaseQty = Math.Round(group.Sum(x => x.ReceiptPurchaseQty), 2),
                            ReceiptProcessQty = Math.Round(group.Sum(x => x.ReceiptProcessQty), 2),
                            ReceiptKon2AQty = Math.Round(group.Sum(x => x.ReceiptKon2AQty), 2),
                            ReceiptKon2BQty = Math.Round(group.Sum(x => x.ReceiptKon2BQty), 2),
                            ReceiptKon2CQty = Math.Round(group.Sum(x => x.ReceiptKon2CQty), 2),
                            ReceiptKon1AQty = Math.Round(group.Sum(x => x.ReceiptKon1AQty), 2),
                            ReceiptKon1BQty = Math.Round(group.Sum(x => x.ReceiptKon1BQty), 2),
                            ReceiptCorrectionPrice = Math.Round(group.Sum(x => x.ReceiptCorrectionPrice), 2),
                            ReceiptPurchasePrice = Math.Round(group.Sum(x => x.ReceiptPurchasePrice), 2),
                            ReceiptProcessPrice = Math.Round(group.Sum(x => x.ReceiptProcessPrice), 2),
                            ReceiptKon2APrice = Math.Round(group.Sum(x => x.ReceiptKon2APrice), 2),
                            ReceiptKon2BPrice = Math.Round(group.Sum(x => x.ReceiptKon2BPrice), 2),
                            ReceiptKon2CPrice = Math.Round(group.Sum(x => x.ReceiptKon2CPrice), 2),
                            ReceiptKon1APrice = Math.Round(group.Sum(x => x.ReceiptKon1APrice), 2),
                            ReceiptKon1BPrice = Math.Round(group.Sum(x => x.ReceiptKon1BPrice), 2),
                            ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                            ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                            ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                            ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                            ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                            ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                            ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                            ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                            ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                            ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                            ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                            ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                            ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                            ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                            ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                            ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                            ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                            ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                            EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                            EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                              ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                              ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                              ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                              ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                              ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                              ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                          });
            
            var Keluar = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                          join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                          join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                          join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                          //join h in Codes on a.ProductCode equals h.Code
                          join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                          from prs in PR.DefaultIfEmpty()
                          join f in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals f.Id into urnitems
                          from urnitem in urnitems.DefaultIfEmpty()
                          where a.IsDeleted == false && b.IsDeleted == false
                             &&
                             b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                             && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                             && b.UnitSenderCode == (string.IsNullOrWhiteSpace(unitcode) ? b.UnitSenderCode : unitcode)
                             && categories1.Contains(a.ProductName)
                             && a.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? a.POSerialNumber : planPo)
                          select new AccountingStockTempViewModel
                         {
                             ProductCode = a.ProductCode.Trim(),
                             //ProductName = a.ProductName,
                             RO = a.RONo,
                             Buyer = a.BuyerCode == null ? "-" : a.BuyerCode.Trim(),
                             PlanPo = a.POSerialNumber.Trim(),
                             NoArticle = prs != null ? prs.Article.TrimEnd() : "-",
                             BeginningBalanceQty = 0,
                             BeginningBalanceUom = a.UomUnit == "YARD" && ctg == "BB" ? "MT" : b.ExpenditureType == "EXTERNAL" ? urnitem.SmallUomUnit : a.UomUnit.Trim(),
                             BeginningBalancePrice = 0,
                             ReceiptCorrectionQty = 0,
                             ReceiptPurchaseQty = 0,
                             ReceiptProcessQty = 0,
                             ReceiptKon2AQty = 0,
                             ReceiptKon2BQty = 0,
                             ReceiptKon2CQty = 0,
                             ReceiptKon1AQty = 0,
                             ReceiptKon1BQty = 0,
                             ReceiptCorrectionPrice = 0,
                             ReceiptPurchasePrice = 0,
                             ReceiptProcessPrice = 0,
                             ReceiptKon2APrice = 0,
                             ReceiptKon2BPrice = 0,
                             ReceiptKon2CPrice = 0,
                             ReceiptKon1APrice = 0,
                             ReceiptKon1BPrice = 0,
                             ExpendReturQty = b.ExpenditureType == "EXTERNAL" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? Convert.ToDecimal(a.Quantity * 0.9144) : (a.UomUnit != "PCS" && ctg != "BB") ? Convert.ToDecimal(a.Quantity * (urnitem != null ? (double)urnitem.Conversion : 1)) : Convert.ToDecimal(a.Quantity)), 2) : 0,
                             ExpendRestQty = b.ExpenditureType == "SISA" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             ExpendProcessQty = b.ExpenditureType == "PROSES" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             ExpendSampleQty = b.ExpenditureType == "SAMPLE" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendKon2AQty = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2A" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendKon2BQty = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendKon2CQty = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2C/EX. K4" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendKon1AQty = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1A/EX. K3" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendKon1BQty = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                             //ExpendReturPrice = b.ExpenditureType == "EXTERNAL" ? (a.UomUnit == "YARD" && ctg == "BB" ? Convert.ToDecimal(a.Quantity * 0.9144) : Convert.ToDecimal(a.Quantity * (urnitem != null ? (double)urnitem.Conversion : 1)) * (decimal)a.PricePerDealUnit * Convert.ToDecimal(a.DOCurrencyRate)) : 0,
                             ExpendReturPrice = b.ExpenditureType == "EXTERNAL" ? (a.UomUnit == "YARD" && ctg == "BB" ? Convert.ToDecimal(a.Quantity * 0.9144) : (a.UomUnit != "PCS" && ctg != "BB") ? Convert.ToDecimal(a.Quantity * (urnitem != null ? (double)urnitem.Conversion : 1)) : Convert.ToDecimal(a.Quantity)) * (decimal)(a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * Convert.ToDecimal(a.DOCurrencyRate) : 0,
                              //ExpendRestPrice = b.ExpenditureType == "SISA" ? (a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate : 0,
                              //ExpendProcessPrice = b.ExpenditureType == "PROSES" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendSamplePrice = b.ExpenditureType == "SAMPLE" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendKon2APrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2A" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendKon2BPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendKon2CPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2C/EX. K4" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendKon1APrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1A/EX. K3" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              //ExpendKon1BPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * ((double)a.BasicPrice / (a.Conversion == 0 ? 1 : (double)a.Conversion)), 2) : 0,
                              ExpendRestPrice = b.ExpenditureType == "SISA" ? (a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate : 0,
                              ExpendProcessPrice = b.ExpenditureType == "PROSES" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              ExpendSamplePrice = b.ExpenditureType == "SAMPLE" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              //ExpendKon2APrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2A" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              //ExpendKon2BPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              //ExpendKon2CPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 2C/EX. K4" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              //ExpendKon1APrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1A/EX. K3" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              //ExpendKon1BPrice = (b.ExpenditureType == "GUDANG LAIN" || b.ExpenditureType == "TRANSFER") && b.UnitRequestName == "CENTRAL 1B" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              EndingBalanceQty = 0,
                              EndingBalancePrice = 0,
                              ExpendTransferQty = b.ExpenditureType == "TRANSFER" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                              ExpendOtherQty = b.ExpenditureType == "LAIN-LAIN" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                              ExpendSubconQty= b.ExpenditureType == "SUBCON" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity), 2) : 0,
                              ExpendTransferPrice= b.ExpenditureType == "TRANSFER" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              ExpendSubconPrice= b.ExpenditureType == "SUBCON" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                              ExpendOtherPrice= b.ExpenditureType == "LAIN-LAIN" ? Math.Round((a.UomUnit == "YARD" && ctg == "BB" ? a.Quantity * 0.9144 : a.Quantity) * (a.PricePerDealUnit / (double)(a.Conversion == 0 ? 1 : a.Conversion)) * (double)a.DOCurrencyRate, 2) : 0,
                          }).GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                         {
                             ProductCode = key.ProductCode,
                             //ProductName = key.ProductName,
                             RO = key.RO,
                             Buyer = key.Buyer,
                             PlanPo = key.PlanPo,
                             NoArticle = key.NoArticle,
                             BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                             BeginningBalanceUom = key.BeginningBalanceUom,
                             BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                             ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                             ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                             ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                             ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                             ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                             ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                             ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                             ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                             ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                             ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                             ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                             ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                             ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                             ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                             ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                             ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                             ExpendReturQty = Math.Round(group.Sum(x => x.ExpendReturQty), 2),
                             ExpendRestQty = Math.Round(group.Sum(x => x.ExpendRestQty), 2),
                             ExpendProcessQty = Math.Round(group.Sum(x => x.ExpendProcessQty), 2),
                             ExpendSampleQty = Math.Round(group.Sum(x => x.ExpendSampleQty), 2),
                             ExpendKon2AQty = Math.Round(group.Sum(x => x.ExpendKon2AQty), 2),
                             ExpendKon2BQty = Math.Round(group.Sum(x => x.ExpendKon2BQty), 2),
                             ExpendKon2CQty = Math.Round(group.Sum(x => x.ExpendKon2CQty), 2),
                             ExpendKon1AQty = Math.Round(group.Sum(x => x.ExpendKon1AQty), 2),
                             ExpendKon1BQty = Math.Round(group.Sum(x => x.ExpendKon1BQty), 2),
                             ExpendReturPrice = Math.Round(group.Sum(x => x.ExpendReturPrice), 2),
                             ExpendRestPrice = Math.Round(group.Sum(x => x.ExpendRestPrice), 2),
                             ExpendProcessPrice = Math.Round(group.Sum(x => x.ExpendProcessPrice), 2),
                             ExpendSamplePrice = Math.Round(group.Sum(x => x.ExpendSamplePrice), 2),
                             ExpendKon2APrice = Math.Round(group.Sum(x => x.ExpendKon2APrice), 2),
                             ExpendKon2BPrice = Math.Round(group.Sum(x => x.ExpendKon2BPrice), 2),
                             ExpendKon2CPrice = Math.Round(group.Sum(x => x.ExpendKon2CPrice), 2),
                             ExpendKon1APrice = Math.Round(group.Sum(x => x.ExpendKon1APrice), 2),
                             ExpendKon1BPrice = Math.Round(group.Sum(x => x.ExpendKon1BPrice), 2),
                             EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                             EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                              ExpendOtherPrice = Math.Round(group.Sum(x => x.ExpendOtherPrice),2),
                              ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                              ExpendSubconPrice = Math.Round(group.Sum(x => x.ExpendSubconPrice),2),
                              ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                              ExpendTransferPrice = Math.Round(group.Sum(x => x.ExpendTransferPrice),2),
                              ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                          });

            var Koreksi = (from a in dbContext.GarmentUnitReceiptNotes
                           join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                           join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                           join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                           join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                           join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                           join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                           from prs in PR.DefaultIfEmpty()
                               //join h in Codes on b.ProductCode equals h.Code
                           where
                           a.IsDeleted == false && b.IsDeleted == false
                           &&
                           g.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                           && g.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                           && a.UnitCode == (string.IsNullOrWhiteSpace(unitcode) ? a.UnitCode : unitcode)
                           && categories1.Contains(b.ProductName)
                           && b.POSerialNumber == (string.IsNullOrWhiteSpace(planPo) ? b.POSerialNumber : planPo)
                           select new AccountingStockTempViewModel
                           {
                               ProductCode = b.ProductCode.Trim(),
                               //ProductName = b.ProductName,
                               RO = b.RONo,
                               Buyer = prs != null ? prs.BuyerCode.Trim() : "-",
                               PlanPo = b.POSerialNumber.Trim(),
                               NoArticle = prs != null ? prs.Article.Trim() : "-",
                               BeginningBalanceQty = 0,
                               BeginningBalanceUom = b.SmallUomUnit.Trim(),
                               BeginningBalancePrice = 0,
                               ReceiptCorrectionQty = Math.Round((decimal)e.SmallQuantity, 2),
                               ReceiptPurchaseQty = 0,
                               ReceiptProcessQty = 0,
                               ReceiptKon2AQty = 0,
                               ReceiptKon2BQty = 0,
                               ReceiptKon2CQty = 0,
                               ReceiptKon1AQty = 0,
                               ReceiptKon1BQty = 0,
                               ReceiptCorrectionPrice = Math.Round((((decimal)e.PricePerDealUnit / ((decimal)e.Conversion == 0 ? 1 : (decimal)e.Conversion)) * (decimal)b.DOCurrencyRate) * ((decimal)e.SmallQuantity), 2),
                               ReceiptPurchasePrice = 0,
                               ReceiptProcessPrice = 0,
                               ReceiptKon2APrice = 0,
                               ReceiptKon2BPrice = 0,
                               ReceiptKon2CPrice = 0,
                               ReceiptKon1APrice = 0,
                               ReceiptKon1BPrice = 0,
                               ExpendReturQty = 0,
                               ExpendRestQty = 0,
                               ExpendProcessQty = 0,
                               ExpendSampleQty = 0,
                               ExpendKon2AQty = 0,
                               ExpendKon2BQty = 0,
                               ExpendKon2CQty = 0,
                               ExpendKon1AQty = 0,
                               ExpendKon1BQty = 0,
                               ExpendReturPrice = 0,
                               ExpendRestPrice = 0,
                               ExpendProcessPrice = 0,
                               ExpendSamplePrice = 0,
                               ExpendKon2APrice = 0,
                               ExpendKon2BPrice = 0,
                               ExpendKon2CPrice = 0,
                               ExpendKon1APrice = 0,
                               ExpendKon1BPrice = 0,
                               EndingBalanceQty = 0,
                               EndingBalancePrice = 0,
                               ExpendOtherPrice = 0,
                               ExpendOtherQty = 0,
                               ExpendSubconPrice = 0,
                               ExpendSubconQty = 0,
                               ExpendTransferPrice = 0,
                               ExpendTransferQty = 0,
                           }).GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, x.Buyer, x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
                           {
                               ProductCode = key.ProductCode,
                               //ProductName = key.ProductName,
                               RO = key.RO,
                               Buyer = key.Buyer,
                               PlanPo = key.PlanPo,
                               NoArticle = key.NoArticle,
                               BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                               BeginningBalanceUom = key.BeginningBalanceUom,
                               BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                               ReceiptCorrectionQty = Math.Round(group.Sum(x => x.ReceiptCorrectionQty),2),
                               ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                               ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                               ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                               ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                               ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                               ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                               ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                               ReceiptCorrectionPrice = Math.Round(group.Sum(x => x.ReceiptCorrectionPrice), 2),
                               ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                               ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                               ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                               ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                               ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                               ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                               ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                               ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                               ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                               ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                               ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                               ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                               ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                               ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                               ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                               ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                               ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                               ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                               ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                               ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                               ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                               ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                               ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                               ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                               ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                               EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                               EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                               ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                               ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                               ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                               ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                               ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                               ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
                           });

            var SaldoAkhir1 = Terima.Concat(Keluar).Concat(Koreksi).AsEnumerable();
            var SaldoAkhir = SaldoAkhir1.GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, /*x.Buyer,*/ x.NoArticle, x.PlanPo, x.RO }, (key, group) => new AccountingStockTempViewModel
            {
                ProductCode = key.ProductCode,
                //ProductName = key.ProductName,
                RO = key.RO,
                Buyer = group.FirstOrDefault().Buyer,
                PlanPo = key.PlanPo,
                NoArticle = key.NoArticle,
                BeginningBalanceQty = group.Sum(x => x.BeginningBalanceQty),
                BeginningBalanceUom = key.BeginningBalanceUom,
                BeginningBalancePrice = group.Sum(x => x.BeginningBalancePrice),
                ReceiptCorrectionQty = group.Sum(x => x.ReceiptCorrectionQty),
                ReceiptPurchaseQty = group.Sum(x => x.ReceiptPurchaseQty),
                ReceiptProcessQty = group.Sum(x => x.ReceiptProcessQty),
                ReceiptKon2AQty = group.Sum(x => x.ReceiptKon2AQty),
                ReceiptKon2BQty = group.Sum(x => x.ReceiptKon2BQty),
                ReceiptKon2CQty = group.Sum(x => x.ReceiptKon2CQty),
                ReceiptKon1AQty = group.Sum(x => x.ReceiptKon1AQty),
                ReceiptKon1BQty = group.Sum(x => x.ReceiptKon1BQty),
                ReceiptCorrectionPrice = group.Sum(x => x.ReceiptCorrectionPrice),
                ReceiptPurchasePrice = group.Sum(x => x.ReceiptPurchasePrice),
                ReceiptProcessPrice = group.Sum(x => x.ReceiptProcessPrice),
                ReceiptKon2APrice = group.Sum(x => x.ReceiptKon2APrice),
                ReceiptKon2BPrice = group.Sum(x => x.ReceiptKon2BPrice),
                ReceiptKon2CPrice = group.Sum(x => x.ReceiptKon2CPrice),
                ReceiptKon1APrice = group.Sum(x => x.ReceiptKon1APrice),
                ReceiptKon1BPrice = group.Sum(x => x.ReceiptKon1BPrice),
                ExpendReturQty = group.Sum(x => x.ExpendReturQty),
                ExpendRestQty = group.Sum(x => x.ExpendRestQty),
                ExpendProcessQty = group.Sum(x => x.ExpendProcessQty),
                ExpendSampleQty = group.Sum(x => x.ExpendSampleQty),
                ExpendKon2AQty = group.Sum(x => x.ExpendKon2AQty),
                ExpendKon2BQty = group.Sum(x => x.ExpendKon2BQty),
                ExpendKon2CQty = group.Sum(x => x.ExpendKon2CQty),
                ExpendKon1AQty = group.Sum(x => x.ExpendKon1AQty),
                ExpendKon1BQty = group.Sum(x => x.ExpendKon1BQty),
                ExpendReturPrice = group.Sum(x => x.ExpendReturPrice),
                ExpendRestPrice = group.Sum(x => x.ExpendRestPrice),
                ExpendProcessPrice = group.Sum(x => x.ExpendProcessPrice),
                ExpendSamplePrice = group.Sum(x => x.ExpendSamplePrice),
                ExpendKon2APrice = group.Sum(x => x.ExpendKon2APrice),
                ExpendKon2BPrice = group.Sum(x => x.ExpendKon2BPrice),
                ExpendKon2CPrice = group.Sum(x => x.ExpendKon2CPrice),
                ExpendKon1APrice = group.Sum(x => x.ExpendKon1APrice),
                ExpendKon1BPrice = group.Sum(x => x.ExpendKon1BPrice),
                EndingBalanceQty = group.Sum(x => x.EndingBalanceQty),
                EndingBalancePrice = group.Sum(x => x.EndingBalancePrice),
                ExpendOtherPrice = group.Sum(x => x.ExpendOtherPrice),
                ExpendOtherQty = group.Sum(x => x.ExpendOtherQty),
                ExpendSubconPrice = group.Sum(x => x.ExpendSubconPrice),
                ExpendSubconQty = group.Sum(x => x.ExpendSubconQty),
                ExpendTransferPrice = group.Sum(x => x.ExpendTransferPrice),
                ExpendTransferQty = group.Sum(x => x.ExpendTransferQty)
            }).ToList();

            var Stock = SaldoAwal.Concat(SaldoAkhir).AsEnumerable();
            var SaldoAkhirs = Stock.GroupBy(x => new { x.ProductCode, /*x.ProductName,*/ x.BeginningBalanceUom, /*x.Buyer,*/ x.NoArticle, x.PlanPo, x.RO }, (key, data) => new AccountingStockTempViewModel
            {
                ProductCode = key.ProductCode,
                //ProductName = key.ProductName,
                RO = key.RO,
                Buyer = data.FirstOrDefault().Buyer,
                PlanPo = key.PlanPo,
                NoArticle = key.NoArticle,
                BeginningBalanceQty = data.Sum(x => x.BeginningBalanceQty),
                BeginningBalanceUom = data.FirstOrDefault().BeginningBalanceUom,
                BeginningBalancePrice = data.Sum(x => x.BeginningBalancePrice),
                ReceiptCorrectionQty = data.Sum(x => x.ReceiptCorrectionQty),
                ReceiptPurchaseQty = data.Sum(x => x.ReceiptPurchaseQty),
                ReceiptProcessQty = data.Sum(x => x.ReceiptProcessQty),
                ReceiptKon2AQty = data.Sum(x => x.ReceiptKon2AQty),
                ReceiptKon2BQty = data.Sum(x => x.ReceiptKon2BQty),
                ReceiptKon2CQty = data.Sum(x => x.ReceiptKon2CQty),
                ReceiptKon1AQty = data.Sum(x => x.ReceiptKon1AQty),
                ReceiptKon1BQty = data.Sum(x => x.ReceiptKon1BQty),
                ReceiptCorrectionPrice = data.Sum(x => x.ReceiptCorrectionPrice),
                ReceiptPurchasePrice = data.Sum(x => x.ReceiptPurchasePrice),
                ReceiptProcessPrice = data.Sum(x => x.ReceiptProcessPrice),
                ReceiptKon2APrice = data.Sum(x => x.ReceiptKon2APrice),
                ReceiptKon2BPrice = data.Sum(x => x.ReceiptKon2BPrice),
                ReceiptKon2CPrice = data.Sum(x => x.ReceiptKon2CPrice),
                ReceiptKon1APrice = data.Sum(x => x.ReceiptKon1APrice),
                ReceiptKon1BPrice = data.Sum(x => x.ReceiptKon1BPrice),
                ExpendReturQty = data.Sum(x => x.ExpendReturQty),
                ExpendRestQty = data.Sum(x => x.ExpendRestQty),
                ExpendProcessQty = data.Sum(x => x.ExpendProcessQty),
                ExpendSampleQty = data.Sum(x => x.ExpendSampleQty),
                ExpendKon2AQty = data.Sum(x => x.ExpendKon2AQty),
                ExpendKon2BQty = data.Sum(x => x.ExpendKon2BQty),
                ExpendKon2CQty = data.Sum(x => x.ExpendKon2CQty),
                ExpendKon1AQty = data.Sum(x => x.ExpendKon1AQty),
                ExpendKon1BQty = data.Sum(x => x.ExpendKon1BQty),
                ExpendReturPrice = data.Sum(x => x.ExpendReturPrice),
                ExpendRestPrice = data.Sum(x => x.ExpendRestPrice),
                ExpendProcessPrice = data.Sum(x => x.ExpendProcessPrice),
                ExpendSamplePrice = data.Sum(x => x.ExpendSamplePrice),
                ExpendKon2APrice = data.Sum(x => x.ExpendKon2APrice),
                ExpendKon2BPrice = data.Sum(x => x.ExpendKon2BPrice),
                ExpendKon2CPrice = data.Sum(x => x.ExpendKon2CPrice),
                ExpendKon1APrice = data.Sum(x => x.ExpendKon1APrice),
                ExpendKon1BPrice = data.Sum(x => x.ExpendKon1BPrice),
                EndingBalanceQty = Math.Round((decimal)data.Sum(x => x.BeginningBalanceQty) + (decimal)data.Sum(x => x.ReceiptCorrectionQty) + (decimal)data.Sum(a => a.ReceiptPurchaseQty) + (decimal)data.Sum(a => a.ReceiptProcessQty) + (decimal)data.Sum(a => a.ReceiptKon2AQty) + (decimal)data.Sum(a => a.ReceiptKon2BQty) + (decimal)data.Sum(a => a.ReceiptKon2CQty) + (decimal)data.Sum(a => a.ReceiptKon1AQty) + (decimal)data.Sum(a => a.ReceiptKon1BQty) - ((decimal)data.Sum(a => a.ExpendReturQty) + (decimal)data.Sum(a => a.ExpendSampleQty) + (decimal)data.Sum(a => a.ExpendRestQty) + (decimal)data.Sum(a => a.ExpendProcessQty) + (decimal)data.Sum(a => a.ExpendKon2AQty) + (decimal)data.Sum(a => a.ExpendKon2BQty) + (decimal)data.Sum(a => a.ExpendKon2CQty) + (decimal)data.Sum(a => a.ExpendKon1AQty) + (decimal)data.Sum(a => a.ExpendKon1BQty)), 2),
                EndingBalancePrice = Math.Round((double)data.Sum(a => a.BeginningBalancePrice) + (double)data.Sum(a => a.ReceiptCorrectionPrice) + (double)data.Sum(a => a.ReceiptPurchasePrice) + (double)data.Sum(a => a.ReceiptProcessPrice) + (double)data.Sum(a => a.ReceiptKon2APrice) + (double)data.Sum(a => a.ReceiptKon2BPrice) + (double)data.Sum(a => a.ReceiptKon2CPrice) + (double)data.Sum(a => a.ReceiptKon1APrice) + (double)data.Sum(a => a.ReceiptKon1BPrice) - ((double)data.Sum(a => a.ExpendReturPrice) + (double)data.Sum(a => a.ExpendRestPrice) + (double)data.Sum(a => a.ExpendProcessPrice) + (double)data.Sum(a => a.ExpendSamplePrice) + (double)data.Sum(a => a.ExpendKon2APrice) + (double)data.Sum(a => a.ExpendKon2BPrice) + (double)data.Sum(a => a.ExpendKon2CPrice) + (double)data.Sum(a => a.ExpendKon1APrice) + (double)data.Sum(a => a.ExpendKon1BPrice)), 2),
                ExpendOtherPrice = data.Sum(x => x.ExpendOtherPrice),
                ExpendOtherQty = data.Sum(x => x.ExpendOtherQty),
                ExpendSubconPrice = data.Sum(x => x.ExpendSubconPrice),
                ExpendSubconQty = data.Sum(x => x.ExpendSubconQty),
                ExpendTransferPrice = data.Sum(x => x.ExpendTransferPrice),
                ExpendTransferQty = data.Sum(x => x.ExpendTransferQty)
            }).ToList();

            List<AccountingStockReportViewModel> stockReportViewModels = new List<AccountingStockReportViewModel>();

            var productCodes = string.Join(",", SaldoAkhirs.Select(x => x.ProductCode).Distinct().ToList());

            var Codes = GetProductCode(productCodes);


            foreach (var i in SaldoAkhirs)
            {
                //var BeginningBalanceQty = i.BeginningBalanceQty > 0 ? i.BeginningBalanceQty : 0;
                //var BeginningBalancePrice = (i.BeginningBalanceQty > 0 && i.BeginningBalancePrice > 0) ? i.BeginningBalancePrice : 0;
                //var EndingBalanceQty = i.EndingBalanceQty > 0 ? i.EndingBalanceQty : 0;
                //var EndingBalancePrice = (i.EndingBalanceQty > 0 && i.EndingBalancePrice > 0) ? i.EndingBalancePrice : 0;
                var remark = Codes.FirstOrDefault(x => x.Code == i.ProductCode);

                var Composition = remark == null ? "-" : remark.Composition;
                var Width = remark == null ? "-" : remark.Width;
                var Const = remark == null ? "-" : remark.Const;
                var Yarn = remark == null ? "-" : remark.Yarn;
                var Name = remark == null ? "-" : remark.Name;

                stockReportViewModels.Add(new AccountingStockReportViewModel
                {
                    ProductCode = i.ProductCode,
                    ProductName = ctg == "BB" ? string.Concat(Composition, "", Width, "", Const, "", Yarn) : Name,
                    RO = i.RO,
                    Buyer = i.Buyer,
                    PlanPo = i.PlanPo,
                    NoArticle = i.NoArticle,
                    BeginningBalanceQty = i.BeginningBalanceQty,
                    BeginningBalanceUom = i.BeginningBalanceUom,
                    BeginningBalancePrice = Convert.ToDouble(i.BeginningBalancePrice),
                    //BeginningBalancePrice = i.BeginningBalanceQty == 0? 0: Convert.ToDouble(i.BeginningBalancePrice),
                    ReceiptCorrectionQty = i.ReceiptCorrectionQty,
                    ReceiptPurchaseQty = i.ReceiptPurchaseQty,
                    ReceiptProcessQty = i.ReceiptProcessQty,
                    ReceiptKon2AQty = i.ReceiptKon2AQty,
                    ReceiptKon2BQty = i.ReceiptKon2BQty,
                    ReceiptKon2CQty = i.ReceiptKon2CQty,
                    ReceiptKon1AQty = i.ReceiptKon1AQty,
                    ReceiptKon1BQty = i.ReceiptKon1BQty,
                    ReceiptCorrectionPrice = i.ReceiptCorrectionPrice,
                    ReceiptPurchasePrice = i.ReceiptPurchasePrice,
                    ReceiptProcessPrice = i.ReceiptProcessPrice,
                    ReceiptKon2APrice = i.ReceiptKon2APrice,
                    ReceiptKon2BPrice = i.ReceiptKon2BPrice,
                    ReceiptKon2CPrice = i.ReceiptKon2CPrice,
                    ReceiptKon1APrice = i.ReceiptKon1APrice,
                    ReceiptKon1BPrice = i.ReceiptKon1BPrice,
                    ExpendReturQty = Convert.ToDouble(i.ExpendReturQty),
                    ExpendRestQty = i.ExpendRestQty,
                    ExpendProcessQty = i.ExpendProcessQty,
                    ExpendSampleQty = i.ExpendSampleQty,
                    ExpendKon2AQty = i.ExpendKon2AQty,
                    ExpendKon2BQty = i.ExpendKon2BQty,
                    ExpendKon2CQty = i.ExpendKon2CQty,
                    ExpendKon1AQty = i.ExpendKon1AQty,
                    ExpendKon1BQty = i.ExpendKon1BQty,
                    ExpendReturPrice = Convert.ToDouble(i.ExpendReturPrice),
                    ExpendRestPrice = i.ExpendRestPrice,
                    ExpendProcessPrice = i.ExpendProcessPrice,
                    ExpendSamplePrice = i.ExpendSamplePrice,
                    ExpendKon2APrice = i.ExpendKon2APrice,
                    ExpendKon2BPrice = i.ExpendKon2BPrice,
                    ExpendKon2CPrice = i.ExpendKon2CPrice,
                    ExpendKon1APrice = i.ExpendKon1APrice,
                    ExpendKon1BPrice = i.ExpendKon1BPrice,
                    EndingBalanceQty = i.EndingBalanceQty,
                    EndingBalancePrice = i.EndingBalancePrice,
                    ExpendOtherPrice=i.ExpendOtherPrice,
                    ExpendSubconPrice=i.ExpendSubconPrice,
                    ExpendTransferPrice=i.ExpendTransferPrice,
                    ExpendSubconQty=i.ExpendSubconQty,
                    ExpendOtherQty=i.ExpendOtherQty,
                    ExpendTransferQty=i.ExpendTransferQty
                });

                //i.BeginningBalanceQty = i.BeginningBalanceQty > 0 ? i.BeginningBalanceQty : 0;
                //i.BeginningBalancePrice = (i.BeginningBalanceQty > 0 && i.BeginningBalancePrice > 0) ? i.BeginningBalancePrice : 0;
                //i.EndingBalanceQty = i.EndingBalanceQty > 0 ? i.EndingBalanceQty : 0;                               
                //i.EndingBalancePrice = (i.EndingBalanceQty > 0 && i.EndingBalancePrice > 0) ? i.EndingBalancePrice : 0;


            }

            stockReportViewModels = stockReportViewModels.Where(x => (x.ProductCode != "EMB001") && (x.ProductCode != "WSH001") && (x.ProductCode != "PRC001") && (x.ProductCode != "APL001") && (x.ProductCode != "QLT001") && (x.ProductCode != "SMT001") && (x.ProductCode != "GMT001") && (x.ProductCode != "PRN001") && (x.ProductCode != "SMP001")).ToList();
            stockReportViewModels = stockReportViewModels.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();

            //stockReportViewModels = stockReportViewModels.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();

            if (unitcode != "SMP1")
            {
                stockReportViewModels = stockReportViewModels.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice != 0) || (x.EndingBalanceQty != 0) || (x.ExpendKon1APrice != 0) || (x.ExpendKon1AQty != 0) ||
                (x.ExpendKon1BPrice != 0) || (x.ExpendKon1BQty != 0) || (x.ExpendKon2APrice != 0) || (x.ExpendKon2AQty != 0) || (x.ExpendKon2BPrice != 0) || (x.ExpendKon2BQty != 0) || (x.ExpendKon2CPrice != 0) || (x.ExpendKon2CQty != 0) ||
                (x.ExpendProcessPrice != 0) || (x.ExpendProcessQty != 0) || (x.ExpendRestPrice != 0) || (x.ExpendRestQty != 0) || (x.ExpendReturPrice != 0) || (x.ExpendReturQty != 0) || (x.ExpendSamplePrice != 0) || (x.ExpendSampleQty != 0) ||
                (x.ReceiptCorrectionPrice != 0) || (x.ReceiptCorrectionQty != 0) || (x.ReceiptKon1APrice != 0) || (x.ReceiptKon1AQty != 0) || (x.ReceiptKon1BPrice != 0) || (x.ReceiptKon1BQty != 0) || (x.ReceiptKon2APrice != 0) || (x.ReceiptKon2AQty != 0)
                || (x.ReceiptKon2BPrice != 0) || (x.ReceiptKon2BQty != 0) || (x.ReceiptKon2CPrice != 0) || (x.ReceiptKon2CQty != 0) || (x.ReceiptProcessPrice != 0) || (x.ReceiptProcessQty != 0) || (x.ReceiptPurchasePrice != 0) || (x.ReceiptPurchaseQty != 0)).ToList();

            }
            else
            {
                stockReportViewModels = stockReportViewModels.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice != 0) || (x.EndingBalanceQty != 0) || (x.ExpendKon1APrice != 0) || (x.ExpendKon1AQty != 0) ||
                (x.ExpendKon1BPrice != 0) || (x.ExpendKon1BQty != 0) || (x.ExpendKon2APrice != 0) || (x.ExpendKon2AQty != 0) || (x.ExpendKon2BPrice != 0) || (x.ExpendKon2BQty != 0) || (x.ExpendKon2CPrice != 0) || (x.ExpendKon2CQty != 0) ||
                (x.ExpendRestPrice != 0) || (x.ExpendRestQty != 0) || (x.ExpendReturPrice != 0) || (x.ExpendReturQty != 0) || (x.ExpendSamplePrice != 0) || (x.ExpendSampleQty != 0) ||
                (x.ReceiptCorrectionPrice != 0) || (x.ReceiptCorrectionQty != 0) || (x.ReceiptKon1APrice != 0) || (x.ReceiptKon1AQty != 0) || (x.ReceiptKon1BPrice != 0) || (x.ReceiptKon1BQty != 0) || (x.ReceiptKon2APrice != 0) || (x.ReceiptKon2AQty != 0)
                || (x.ReceiptKon2BPrice != 0) || (x.ReceiptKon2BQty != 0) || (x.ReceiptKon2CPrice != 0) || (x.ReceiptKon2CQty != 0) || (x.ReceiptProcessPrice != 0) || (x.ReceiptProcessQty != 0) || (x.ReceiptPurchasePrice != 0) || (x.ReceiptPurchaseQty != 0) ||
                (x.ExpendTransferQty != 0) || (x.ExpendTransferPrice != 0) || (x.ExpendSubconQty != 0) || (x.ExpendSubconPrice != 0) || (x.ExpendOtherQty != 0) || (x.ExpendOtherPrice != 0)).ToList();

            }
            var total = new AccountingStockReportViewModel
            {
                ProductCode = "TOTAL",
                ProductName = "",
                RO = "",
                Buyer = "",
                PlanPo = "",
                NoArticle = "",
                BeginningBalanceQty = Math.Round(SaldoAkhirs.Sum(X => X.BeginningBalanceQty), 2),
                BeginningBalanceUom = "",
                BeginningBalancePrice = Convert.ToDouble(Math.Round(SaldoAkhirs.Sum(X => X.BeginningBalancePrice), 2)),
                ReceiptCorrectionQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptCorrectionQty), 2),
                ReceiptPurchaseQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptPurchaseQty), 2),
                ReceiptProcessQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptProcessQty), 2),
                ReceiptKon2AQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2AQty), 2),
                ReceiptKon2BQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2BQty), 2),
                ReceiptKon2CQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2CQty), 2),
                ReceiptKon1AQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon1AQty), 2),
                ReceiptKon1BQty = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon1BQty), 2),
                ReceiptCorrectionPrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptCorrectionPrice), 2),
                ReceiptPurchasePrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptPurchasePrice), 2),
                ReceiptProcessPrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptProcessPrice), 2),
                ReceiptKon2APrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2APrice), 2),
                ReceiptKon2BPrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2BPrice), 2),
                ReceiptKon2CPrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon2CPrice), 2),
                ReceiptKon1APrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon1APrice), 2),
                ReceiptKon1BPrice = Math.Round(SaldoAkhirs.Sum(X => X.ReceiptKon1BPrice), 2),
                ExpendReturQty = Convert.ToDouble(Math.Round(SaldoAkhirs.Sum(X => X.ExpendReturQty), 2)),
                ExpendRestQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendRestQty), 2),
                ExpendProcessQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendProcessQty), 2),
                ExpendSampleQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendSampleQty), 2),
                ExpendKon2AQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2AQty), 2),
                ExpendKon2BQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2BQty), 2),
                ExpendKon2CQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2CQty), 2),
                ExpendKon1AQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon1AQty), 2),
                ExpendKon1BQty = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon1BQty), 2),
                ExpendReturPrice = Convert.ToDouble(Math.Round(SaldoAkhirs.Sum(X => X.ExpendReturPrice), 2)),
                ExpendRestPrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendRestPrice), 2),
                ExpendProcessPrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendProcessPrice), 2),
                ExpendSamplePrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendSamplePrice), 2),
                ExpendKon2APrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2APrice), 2),
                ExpendKon2BPrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2BPrice), 2),
                ExpendKon2CPrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon2CPrice), 2),
                ExpendKon1APrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon1APrice), 2),
                ExpendKon1BPrice = Math.Round(SaldoAkhirs.Sum(X => X.ExpendKon1BPrice), 2),
                EndingBalanceQty = Math.Round(SaldoAkhirs.Sum(X => X.EndingBalanceQty), 2),
                EndingBalancePrice = Math.Round(SaldoAkhirs.Sum(X => X.EndingBalancePrice), 2),
                ExpendOtherPrice = Math.Round(SaldoAkhirs.Sum(x => x.ExpendOtherPrice), 2),
                ExpendOtherQty = Math.Round(SaldoAkhirs.Sum(x => x.ExpendOtherQty),2),
                ExpendSubconPrice = Math.Round(SaldoAkhirs.Sum(x => x.ExpendSubconPrice), 2),
                ExpendSubconQty = Math.Round(SaldoAkhirs.Sum(x => x.ExpendSubconQty),2),
                ExpendTransferPrice = Math.Round(SaldoAkhirs.Sum(x => x.ExpendTransferPrice), 2),
                ExpendTransferQty = Math.Round(SaldoAkhirs.Sum(x => x.ExpendTransferQty),2)

            };




            stockReportViewModels.Add(total);

            return stockReportViewModels;

        }
        public MemoryStream GenerateExcelAStockReport(string ctg, string categoryname, string planPo, string unitcode, string unitname, DateTime? datefrom, DateTime? dateto, int offset)
        {
            var Query = GetStockQuery(ctg, unitcode, planPo, datefrom, dateto, offset);
            //Query = Query.Where(x => (x.BeginningBalanceQty != 0) || (x.BeginningBalancePrice != 0) || (x.EndingBalancePrice > 0) || (x.EndingBalanceQty > 0) || (x.ExpendKon1APrice > 0) || (x.ExpendKon1AQty > 0) ||
            //(x.ExpendKon1BPrice > 0) || (x.ExpendKon1BQty > 0) || (x.ExpendKon2APrice > 0) || (x.ExpendKon2AQty > 0) || (x.ExpendKon2BPrice > 0) || (x.ExpendKon2BQty > 0) || (x.ExpendKon2CPrice > 0) || (x.ExpendKon2CQty > 0) ||
            //(x.ExpendProcessPrice > 0) || (x.ExpendProcessQty > 0) || (x.ExpendRestPrice > 0) || (x.ExpendRestQty > 0) || (x.ExpendReturPrice > 0) || (x.ExpendReturQty > 0) || (x.ExpendSamplePrice > 0) || (x.ExpendSampleQty > 0) ||
            //(x.ReceiptCorrectionPrice > 0) || (x.ReceiptCorrectionQty > 0) || (x.ReceiptKon1APrice > 0) || (x.ReceiptKon1AQty > 0) || (x.ReceiptKon1BPrice > 0) || (x.ReceiptKon1BQty > 0) || (x.ReceiptKon2APrice > 0) || (x.ReceiptKon2AQty > 0)
            //|| (x.ReceiptKon2BPrice > 0) || (x.ReceiptKon2BQty > 0) || (x.ReceiptKon2CPrice > 0) || (x.ReceiptKon2CQty > 0) || (x.ReceiptProcessPrice > 0) || (x.ReceiptProcessQty > 0) || (x.ReceiptPurchasePrice > 0) || (x.ReceiptPurchaseQty > 0)).ToList();
            //Query = Query.OrderBy(x => x.ProductCode).ThenBy(x => x.PlanPo).ToList();
            Query.RemoveAt(Query.Count() - 1);

            #region Pemasukan
            double SaldoAwalQtyTotal = Query.Sum(x => Convert.ToDouble(x.BeginningBalanceQty));
            double SaldoAwalPriceTotal = Query.Sum(x => Convert.ToDouble(x.BeginningBalancePrice));
            double KoreksiQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptCorrectionQty));
            double KoreksiPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptCorrectionPrice));
            double PEMBELIANQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptPurchaseQty));
            double PEMBELIANPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptPurchasePrice));
            double PROSESQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptProcessQty));
            double PROSESPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptProcessPrice));
            //double Konfeksi2AQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2AQty));
            //double Konfeksi2APriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2APrice));
            //double KONFEKSI2BQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2BQty));
            //double KONFEKSI2BPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2BPrice));
            //double KONFEKSI2CQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2CQty));
            //double KONFEKSI2CPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon2CPrice));
            //double KONFEKSI1BQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon1BQty));
            //double KONFEKSI1BPriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon1BPrice));
            //double KONFEKSI1AQtyTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon1AQty));
            //double KONFEKSI1APriceTotal = Query.Sum(x => Convert.ToDouble(x.ReceiptKon1APrice));
            #endregion
            #region Pengeluaran
            double? ReturQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendReturQty));
            double? ReturJumlahTotal = Query.Sum(x => Convert.ToDouble(x.ExpendReturPrice));
            double? SisaQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendRestQty));
            double? SisaPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendRestPrice));
            double? ExpendPROSESQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendProcessQty));
            double? ExpendPROSESPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendProcessPrice));
            double? SAMPLEQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendSampleQty));
            double? SAMPLEPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendSamplePrice));
            //double? ExpendKONFEKSI2AQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2AQty));
            //double? ExpendKonfeksi2APriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2APrice));
            //double? ExpendKONFEKSI2BQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2BQty));
            //double? ExpendKONFEKSI2BPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2BPrice));
            //double? ExpendKONFEKSI2CQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2CQty));
            //double? ExpendKONFEKSI2CPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon2CPrice));
            //double? ExpendKONFEKSI1BQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon1BQty));
            //double? ExpendKONFEKSI1BPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon1BPrice));
            //double? ExpendKONFEKSI1AQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon1AQty));
            //double? ExpendKONFEKSI1APriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendKon1APrice));
            double? ExpendOTHERPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendOtherPrice));
            double? ExpendOTHERQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendOtherQty));
            double? ExpendSUBCONPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendSubconPrice));
            double? ExpendSUBCONQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendSubconQty));
            double? ExpendTRANSFERPriceTotal = Query.Sum(x => Convert.ToDouble(x.ExpendTransferPrice));
            double? ExpendTRANSFERQtyTotal = Query.Sum(x => Convert.ToDouble(x.ExpendTransferQty));
            #endregion
            double? EndingQty = Query.Sum(x => Convert.ToDouble(x.EndingBalanceQty));
            double? EndingTotal = Query.Sum(x => Convert.ToDouble(x.EndingBalancePrice));
            DataTable result = new DataTable();
            var headers = new string[] { "No", "Kode", "No RO", "PlanPO", "No Artikel", "Nama Barang", "Buyer", "Saldo Awal", "Saldo Awal1", "Saldo Awal2", "P E M A S U K A N", "P E M B E L I A N1", "P E M B E L I A N2", "P E M B E L I A N3", "P E M B E L I A N4", "P E M B E L I A N5", "P E M B E L I A N6", /*"P E M B E L I A N7", "P E M B E L I A N8", "P E M B E L I A N9", "P E M B E L I A N10", "P E M B E L I A N11", "P E M B E L I A N12", "P E M B E L I A N13", "P E M B E L I A N14", "P E M B E L I A N15",*/ "P E N G E L U A R A N", "P E N G E L U A R A N1", "P E N G E L U A R A N2", "P E N G E L U A R A N3", "P E N G E L U A R A N4", "P E N G E L U A R A N5", "P E N G E L U A R A N6",/* "P E N G E L U A R A N7", "P E N G E L U A R A N8", "P E N G E L U A R A N9", "P E N G E L U A R A N10", "P E N G E L U A R A N11", "P E N G E L U A R A N12", "P E N G E L U A R A N13", "P E N G E L U A R A N14", "P E N G E L U A R A N15", "P E N G E L U A R A N16", "P E N G E L U A R A N17",*/ "Saldo Akhir", "Saldo Akhir 1" };
            if (unitcode == "SMP1")
            {
                headers= new string[] { "No", "Kode", "No RO", "PlanPO", "No Artikel", "Nama Barang", "Buyer", "Saldo Awal", "Saldo Awal1", "Saldo Awal2", "P E M A S U K A N", "P E M B E L I A N1", "P E M B E L I A N2", "P E M B E L I A N3", "P E M B E L I A N4", "P E M B E L I A N5", "P E M B E L I A N6", "P E M B E L I A N7", "P E M B E L I A N8", "P E M B E L I A N9", "P E M B E L I A N10", "P E M B E L I A N11", "P E M B E L I A N12", "P E M B E L I A N13", "P E M B E L I A N14", "P E M B E L I A N15", "P E N G E L U A R A N", "P E N G E L U A R A N1", "P E N G E L U A R A N2", "P E N G E L U A R A N3", "P E N G E L U A R A N4", "P E N G E L U A R A N5", "P E N G E L U A R A N6", "P E N G E L U A R A N7", "P E N G E L U A R A N8", "P E N G E L U A R A N9", "P E N G E L U A R A N10", "P E N G E L U A R A N11", "P E N G E L U A R A N12", "P E N G E L U A R A N13", "P E N G E L U A R A N14", "P E N G E L U A R A N15", "P E N G E L U A R A N16", "P E N G E L U A R A N17", "P E N G E L U A R A N18", "P E N G E L U A R A N19", "Saldo Akhir", "Saldo Akhir 1" };
            }
            var headers2 = new string[] { "Koreksi", "Pembelian", "Proses", /*"KONFEKSI 2A", "KONFEKSI 2B", "KONFEKSI 2C/EX.K4", "KONFEKSI 1A/EX.K3",  "KONFEKSI 1B",*/ "Retur", "Sisa", "Proses", "Sample"/*, "KONFEKSI 2A", "KONFEKSI 2B", "KONFEKSI 2C/EX.K4", "KONFEKSI 1A/EX. K3", "KONFEKSI 1B"*/ };
            var subheaders = new string[] { "Jumlah", "Sat", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp"/*, "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp", "Qty", "Rp" */};
            for (int i = 0; i < 7; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            }
            result.Columns.Add(new DataColumn() { ColumnName = headers[7], DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = headers[8], DataType = typeof(string) });
            for (int i = 9; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(Double) });
            }
            var index = 1;
            if(unitcode!= "SMP1")
            {
                foreach (var item in Query)
                {
                    var ReceiptPurchaseQty = unitcode == "C2A" ? item.ReceiptPurchaseQty + item.ReceiptKon2AQty : unitcode == "C2B" ? item.ReceiptPurchaseQty + item.ReceiptKon2BQty : unitcode == "C2C" ? item.ReceiptPurchaseQty + item.ReceiptKon2CQty : unitcode == "C1B" ? item.ReceiptPurchaseQty + item.ReceiptKon1BQty : unitcode == "C1A" ? item.ReceiptPurchaseQty + item.ReceiptKon1AQty : item.ReceiptPurchaseQty + item.ReceiptKon2AQty + item.ReceiptKon2BQty + item.ReceiptKon2CQty + item.ReceiptKon1BQty + item.ReceiptKon1AQty;
                    var ReceiptPurchasePrice = unitcode == "C2A" ? item.ReceiptPurchasePrice + item.ReceiptKon2APrice : unitcode == "C2B" ? item.ReceiptPurchasePrice + item.ReceiptKon2BPrice : unitcode == "C2C" ? item.ReceiptPurchasePrice + item.ReceiptKon2CPrice : unitcode == "C1B" ? item.ReceiptPurchasePrice + item.ReceiptKon1BPrice : unitcode == "C1A" ? item.ReceiptPurchasePrice + item.ReceiptKon1APrice : item.ReceiptPurchasePrice + item.ReceiptKon2APrice + item.ReceiptKon2BPrice + item.ReceiptKon2CPrice + item.ReceiptKon1BPrice + item.ReceiptKon1APrice;
                    //var ReceiptKon2AQty = unitcode == "C2A" ? 0 : item.ReceiptKon2AQty;
                    //var ReceiptKon2APrice = unitcode == "C2A" ? 0 : item.ReceiptKon2APrice;
                    //var ReceiptKon2BPrice = unitcode == "C2B" ? 0 : item.ReceiptKon2BPrice;
                    //var ReceiptKon2BQty = unitcode == "C2B" ? 0 : item.ReceiptKon2BQty;
                    //var ReceiptKon2CPrice = unitcode == "C2C" ? 0 : item.ReceiptKon2CPrice;
                    //var ReceiptKon2CQty = unitcode == "C2C" ? 0 : item.ReceiptKon2CQty;
                    //var ReceiptKon1BPrice = unitcode == "C1B" ? 0 : item.ReceiptKon1BPrice;
                    //var ReceiptKon1BQty = unitcode == "C1B" ? 0 : item.ReceiptKon1BQty;
                    //var ReceiptKon1AQty = unitcode == "C1A" ? 0 : item.ReceiptKon1AQty;
                    //var ReceiptKon1APrice = unitcode == "C1A" ? 0 : item.ReceiptKon1APrice;
                    var ReceiptCorrection = item.ReceiptCorrectionPrice;

                    result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Buyer,
                        Convert.ToDouble(item.BeginningBalanceQty), item.BeginningBalanceUom,
                        Convert.ToDouble(item.BeginningBalancePrice),
                        Convert.ToDouble(item.ReceiptCorrectionQty),
                        Convert.ToDouble(item.ReceiptCorrectionPrice),
                        Convert.ToDouble(ReceiptPurchaseQty),
                        Convert.ToDouble(ReceiptPurchasePrice),
                        Convert.ToDouble(item.ReceiptProcessQty),
                        Convert.ToDouble(item.ReceiptProcessPrice),
                        //Convert.ToDouble(ReceiptKon2AQty),
                        //Convert.ToDouble(ReceiptKon2APrice),
                        //Convert.ToDouble(ReceiptKon2BQty),
                        //Convert.ToDouble(ReceiptKon2BPrice),
                        //Convert.ToDouble(ReceiptKon2CQty),
                        //Convert.ToDouble(ReceiptKon2CPrice),
                        //Convert.ToDouble(ReceiptKon1AQty),
                        //Convert.ToDouble(ReceiptKon1APrice),
                        //Convert.ToDouble(ReceiptKon1BQty),
                        //Convert.ToDouble(ReceiptKon1BPrice),
                        Convert.ToDouble(item.ExpendReturQty),
                        item.ExpendReturPrice,
                        item.ExpendRestQty,
                        item.ExpendRestPrice,
                        item.ExpendProcessQty,
                        item.ExpendProcessPrice,
                        item.ExpendSampleQty,
                        item.ExpendSamplePrice,
                        //item.ExpendKon2AQty,
                        //item.ExpendKon2APrice,
                        //item.ExpendKon2BQty,
                        //item.ExpendKon2BPrice,
                        //item.ExpendKon2CQty,
                        //item.ExpendKon2CPrice,
                        //item.ExpendKon1AQty,
                        //item.ExpendKon1APrice,
                        //item.ExpendKon1BQty,
                        //item.ExpendKon1BPrice,
                        Convert.ToDouble(item.EndingBalanceQty),
                        Convert.ToDouble(item.EndingBalancePrice));
                }
            }
            else
            {
                foreach (var item in Query)
                {
                    var ReceiptPurchaseQty = unitcode == "C2A" ? item.ReceiptPurchaseQty + item.ReceiptKon2AQty : unitcode == "C2B" ? item.ReceiptPurchaseQty + item.ReceiptKon2BQty : unitcode == "C2C" ? item.ReceiptPurchaseQty + item.ReceiptKon2CQty : unitcode == "C1B" ? item.ReceiptPurchaseQty + item.ReceiptKon1BQty : unitcode == "C1A" ? item.ReceiptPurchaseQty + item.ReceiptKon1AQty : item.ReceiptPurchaseQty + item.ReceiptKon2AQty + item.ReceiptKon2BQty + item.ReceiptKon2CQty + item.ReceiptKon1BQty + item.ReceiptKon1AQty;
                    var ReceiptPurchasePrice = unitcode == "C2A" ? item.ReceiptPurchasePrice + item.ReceiptKon2APrice : unitcode == "C2B" ? item.ReceiptPurchasePrice + item.ReceiptKon2BPrice : unitcode == "C2C" ? item.ReceiptPurchasePrice + item.ReceiptKon2CPrice : unitcode == "C1B" ? item.ReceiptPurchasePrice + item.ReceiptKon1BPrice : unitcode == "C1A" ? item.ReceiptPurchasePrice + item.ReceiptKon1APrice : item.ReceiptPurchasePrice + item.ReceiptKon2APrice + item.ReceiptKon2BPrice + item.ReceiptKon2CPrice + item.ReceiptKon1BPrice + item.ReceiptKon1APrice;
                    var ReceiptKon2AQty = unitcode == "C2A" ? 0 : item.ReceiptKon2AQty;
                    var ReceiptKon2APrice = unitcode == "C2A" ? 0 : item.ReceiptKon2APrice;
                    var ReceiptKon2BPrice = unitcode == "C2B" ? 0 : item.ReceiptKon2BPrice;
                    var ReceiptKon2BQty = unitcode == "C2B" ? 0 : item.ReceiptKon2BQty;
                    var ReceiptKon2CPrice = unitcode == "C2C" ? 0 : item.ReceiptKon2CPrice;
                    var ReceiptKon2CQty = unitcode == "C2C" ? 0 : item.ReceiptKon2CQty;
                    var ReceiptKon1BPrice = unitcode == "C1B" ? 0 : item.ReceiptKon1BPrice;
                    var ReceiptKon1BQty = unitcode == "C1B" ? 0 : item.ReceiptKon1BQty;
                    var ReceiptKon1AQty = unitcode == "C1A" ? 0 : item.ReceiptKon1AQty;
                    var ReceiptKon1APrice = unitcode == "C1A" ? 0 : item.ReceiptKon1APrice;
                    var ReceiptCorrection = item.ReceiptCorrectionPrice;

                    result.Rows.Add(index++, item.ProductCode, item.RO, item.PlanPo, item.NoArticle, item.ProductName, item.Buyer,
                        Convert.ToDouble(item.BeginningBalanceQty), item.BeginningBalanceUom,
                        Convert.ToDouble(item.BeginningBalancePrice),
                        Convert.ToDouble(item.ReceiptCorrectionQty),
                        Convert.ToDouble(item.ReceiptCorrectionPrice),
                        Convert.ToDouble(ReceiptPurchaseQty),
                        Convert.ToDouble(ReceiptPurchasePrice),
                        Convert.ToDouble(item.ReceiptProcessQty),
                        Convert.ToDouble(item.ReceiptProcessPrice),
                        Convert.ToDouble(ReceiptKon2AQty),
                        Convert.ToDouble(ReceiptKon2APrice),
                        Convert.ToDouble(ReceiptKon2BQty),
                        Convert.ToDouble(ReceiptKon2BPrice),
                        Convert.ToDouble(ReceiptKon2CQty),
                        Convert.ToDouble(ReceiptKon2CPrice),
                        Convert.ToDouble(ReceiptKon1AQty),
                        Convert.ToDouble(ReceiptKon1APrice),
                        Convert.ToDouble(ReceiptKon1BQty),
                        Convert.ToDouble(ReceiptKon1BPrice),
                        Convert.ToDouble(item.ExpendReturQty),
                        item.ExpendReturPrice,
                        item.ExpendRestQty,
                        item.ExpendRestPrice,
                        item.ExpendSampleQty,
                        item.ExpendSamplePrice,
                        item.ExpendSubconQty,
                        item.ExpendSubconPrice,
                        item.ExpendOtherQty,
                        item.ExpendOtherPrice,
                        item.ExpendKon2AQty,
                        item.ExpendKon2APrice,
                        item.ExpendKon2BQty,
                        item.ExpendKon2BPrice,
                        item.ExpendKon2CQty,
                        item.ExpendKon2CPrice,
                        item.ExpendKon1AQty,
                        item.ExpendKon1APrice,
                        item.ExpendKon1BQty,
                        item.ExpendKon1BPrice,
                        Convert.ToDouble(item.EndingBalanceQty),
                        Convert.ToDouble(item.EndingBalancePrice));
                }
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;
            var col1 =('G');
            string tglawal = DateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //CultureInfo Id = new CultureInfo("id-ID");
            sheet.Cells[$"A1:{col1}1"].Value = string.Format("LAPORAN FLOW {0}", categoryname);
            sheet.Cells[$"A1:{col1}1"].Merge = true;
            sheet.Cells[$"A1:{col1}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col1}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col1}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col1}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col1}2"].Merge = true;
            sheet.Cells[$"A2:{col1}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col1}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col1}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col1}3"].Value = string.Format("KONFEKSI : {0}", unitname);
            sheet.Cells[$"A3:{col1}3"].Merge = true;
            sheet.Cells[$"A3:{col1}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col1}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col1}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["H4"].Value = headers[7];
            sheet.Cells["H4:J5"].Merge = true;
            sheet.Cells["H4:J5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["K4"].Value = headers[10];
            sheet.Cells["K4:P4"].Merge = true;
            sheet.Cells["K4:P4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            if (unitcode != "SMP1")
            {
                sheet.Cells["Q4"].Value = headers[17];
                sheet.Cells["Q4:X4"].Merge = true;
                sheet.Cells["Q4:X4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["Y4"].Value = headers[24];
                sheet.Cells["Y4:Z5"].Merge = true;
                sheet.Cells["Y4:Z5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            }
            else
            {
                sheet.Cells["AA4"].Value = headers[26];
                sheet.Cells["AA4:AT4"].Merge = true;
                sheet.Cells["AA4:AT4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AU4"].Value = headers[44];
                sheet.Cells["AU4:AV5"].Merge = true;
                sheet.Cells["AU4:AV5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            }

            sheet.Cells["K5"].Value = headers2[0];
            sheet.Cells["K5:L5"].Merge = true;
            sheet.Cells["K5:L5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["M5"].Value = headers2[1];
            sheet.Cells["M5:N5"].Merge = true;
            sheet.Cells["M5:N5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["O5"].Value = headers2[2];
            sheet.Cells["O5:P5"].Merge = true;
            sheet.Cells["O5:P5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["Q5"].Value = headers2[3];
            sheet.Cells["Q5:R5"].Merge = true;
            sheet.Cells["Q5:R5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["S5"].Value = headers2[4];
            sheet.Cells["S5:T5"].Merge = true;
            sheet.Cells["S5:T5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["U5"].Value = headers2[5];
            sheet.Cells["U5:V5"].Merge = true;
            sheet.Cells["U5:V5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells["W5"].Value = headers2[6];
            sheet.Cells["W5:X5"].Merge = true;
            sheet.Cells["W5:X5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            //sheet.Cells["Y5"].Value = headers2[7];
            //sheet.Cells["Y5:Z5"].Merge = true;
            //sheet.Cells["Y5:Z5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            //sheet.Cells["AA5"].Value = headers2[8];
            //sheet.Cells["AA5:AB5"].Merge = true;
            //sheet.Cells["AA5:AB5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            //sheet.Cells["AC5"].Value = headers2[9];
            //sheet.Cells["AC5:AD5"].Merge = true;
            //sheet.Cells["AC5:AD5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            if (unitcode != "SMP1")
            {
                //sheet.Cells["AE5"].Value = headers2[10];
                //sheet.Cells["AE5:AF5"].Merge = true;
                //sheet.Cells["AE5:AF5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AG5"].Value = headers2[11];
                //sheet.Cells["AG5:AH5"].Merge = true;
                //sheet.Cells["AG5:AH5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AI5"].Value = headers2[12];
                //sheet.Cells["AI5:AJ5"].Merge = true;
                //sheet.Cells["AI5:AJ5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AK5"].Value = headers2[13];
                //sheet.Cells["AK5:AL5"].Merge = true;
                //sheet.Cells["AK5:AL5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AM5"].Value = headers2[14];
                //sheet.Cells["AM5:AN5"].Merge = true;
                //sheet.Cells["AM5:AN5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AO5"].Value = headers2[15];
                //sheet.Cells["AO5:AP5"].Merge = true;
                //sheet.Cells["AO5:AP5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells["AQ5"].Value = headers2[16];
                //sheet.Cells["AQ5:AR5"].Merge = true;
                //sheet.Cells["AQ5:AR5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            }
            else
            {
                sheet.Cells["AE5"].Value = headers2[11];
                sheet.Cells["AE5:AF5"].Merge = true;
                sheet.Cells["AE5:AF5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AG5"].Value = "Subcon";
                sheet.Cells["AG5:AH5"].Merge = true;
                sheet.Cells["AG5:AH5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AI5"].Value = "Lain-lain";
                sheet.Cells["AI5:AJ5"].Merge = true;
                sheet.Cells["AI5:AJ5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AK5"].Value = headers2[12];
                sheet.Cells["AK5:AL5"].Merge = true;
                sheet.Cells["AK5:AL5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AM5"].Value = headers2[13];
                sheet.Cells["AM5:AN5"].Merge = true;
                sheet.Cells["AM5:AN5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AO5"].Value = headers2[14];
                sheet.Cells["AO5:AP5"].Merge = true;
                sheet.Cells["AO5:AP5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AQ5"].Value = headers2[15];
                sheet.Cells["AQ5:AR5"].Merge = true;
                sheet.Cells["AQ5:AR5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["AS5"].Value = headers2[16];
                sheet.Cells["AS5:AT5"].Merge = true;
                sheet.Cells["AS5:AT5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            }

            foreach (var i in Enumerable.Range(0, 7))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}4"].Value = headers[i];
                sheet.Cells[$"{col}4:{col}6"].Merge = true;
                sheet.Cells[$"{col}4:{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            }

            for (var i = 0; i < 19; i++)
            {
                var col = (char)('H' + i);
                sheet.Cells[$"{col}6"].Value = subheaders[i];
                sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            }

            
            for (var i = 19; unitcode!="SMP1" ? i < 19 : i < 31; i++)
            {
                var col = (char)('A' + i - 19);
                sheet.Cells[$"A{col}6"].Value = subheaders[i];
                sheet.Cells[$"A{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            }
            sheet.Cells["A4:AS6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A4:AS6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A4:AS6"].Style.Font.Bold = true;
            sheet.Cells[$"A{result.Rows.Count + 7}:G{result.Rows.Count + 7}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{result.Rows.Count + 7}:G{result.Rows.Count + 7}"].Merge = true;
            sheet.Cells[$"A{result.Rows.Count + 7}:G{result.Rows.Count + 7}"].Style.Font.Bold = true;
            sheet.Cells[$"A{7 + result.Rows.Count}:G{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"A{result.Rows.Count + 7}:G{result.Rows.Count + 7}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{result.Rows.Count + 7}:G{result.Rows.Count + 7}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells[$"H{7 + result.Rows.Count}"].Value = string.Format("{0:0,0.00}", SaldoAwalQtyTotal);
            sheet.Cells[$"H{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"I{7 + result.Rows.Count}"].Value = "";
            sheet.Cells[$"I{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"J{7 + result.Rows.Count}"].Value = SaldoAwalPriceTotal;
            sheet.Cells[$"J{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"K{7 + result.Rows.Count}"].Value = KoreksiQtyTotal;
            sheet.Cells[$"K{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"L{7 + result.Rows.Count}"].Value = KoreksiPriceTotal;
            sheet.Cells[$"L{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"M{7 + result.Rows.Count}"].Value = PEMBELIANQtyTotal;
            sheet.Cells[$"M{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"N{7 + result.Rows.Count}"].Value = PEMBELIANPriceTotal;
            sheet.Cells[$"N{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"O{7 + result.Rows.Count}"].Value = PROSESQtyTotal;
            sheet.Cells[$"O{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"P{7 + result.Rows.Count}"].Value = PROSESPriceTotal;
            sheet.Cells[$"P{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            sheet.Cells[$"Q{7 + result.Rows.Count}"].Value = ReturQtyTotal;
            sheet.Cells[$"Q{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"R{7 + result.Rows.Count}"].Value = ReturJumlahTotal;
            sheet.Cells[$"R{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"S{7 + result.Rows.Count}"].Value = SisaQtyTotal;
            sheet.Cells[$"S{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"T{7 + result.Rows.Count}"].Value = SisaPriceTotal;
            sheet.Cells[$"T{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            //sheet.Cells[$"Q{7 + result.Rows.Count}"].Value = Konfeksi2AQtyTotal;
            //sheet.Cells[$"Q{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"R{7 + result.Rows.Count}"].Value = Konfeksi2APriceTotal;
            //sheet.Cells[$"R{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"S{7 + result.Rows.Count}"].Value = KONFEKSI2BQtyTotal;
            //sheet.Cells[$"S{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"T{7 + result.Rows.Count}"].Value = KONFEKSI2BPriceTotal;
            //sheet.Cells[$"T{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"U{7 + result.Rows.Count}"].Value = KONFEKSI2CQtyTotal;
            //sheet.Cells[$"U{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"V{7 + result.Rows.Count}"].Value = KONFEKSI2CPriceTotal;
            //sheet.Cells[$"V{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"W{7 + result.Rows.Count}"].Value = KONFEKSI1AQtyTotal;
            //sheet.Cells[$"W{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"X{7 + result.Rows.Count}"].Value = KONFEKSI1APriceTotal;
            //sheet.Cells[$"X{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"Y{7 + result.Rows.Count}"].Value = KONFEKSI1BQtyTotal;
            //sheet.Cells[$"Y{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"Z{7 + result.Rows.Count}"].Value = KONFEKSI1BPriceTotal;
            //sheet.Cells[$"Z{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            //sheet.Cells[$"AA{7 + result.Rows.Count}"].Value = ReturQtyTotal;
            //sheet.Cells[$"AA{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"AB{7 + result.Rows.Count}"].Value = ReturJumlahTotal;
            //sheet.Cells[$"AB{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"AC{7 + result.Rows.Count}"].Value = SisaQtyTotal;
            //sheet.Cells[$"AC{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"AD{7 + result.Rows.Count}"].Value = SisaPriceTotal;
            //sheet.Cells[$"AD{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            if (unitcode != "SMP1")
            {
                sheet.Cells[$"U{7 + result.Rows.Count}"].Value = ExpendPROSESQtyTotal;
                sheet.Cells[$"U{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"V{7 + result.Rows.Count}"].Value = ExpendPROSESPriceTotal;
                sheet.Cells[$"V{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"W{7 + result.Rows.Count}"].Value = SAMPLEQtyTotal;
                sheet.Cells[$"W{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"X{7 + result.Rows.Count}"].Value = SAMPLEPriceTotal;
                sheet.Cells[$"X{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AI{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2AQtyTotal;
                //sheet.Cells[$"AI{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AJ{7 + result.Rows.Count}"].Value = ExpendKonfeksi2APriceTotal;
                //sheet.Cells[$"AJ{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AK{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2BQtyTotal;
                //sheet.Cells[$"AK{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AL{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2BPriceTotal;
                //sheet.Cells[$"AL{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AM{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2CQtyTotal;
                //sheet.Cells[$"AM{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AN{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2CPriceTotal;
                //sheet.Cells[$"AN{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AO{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1AQtyTotal;
                //sheet.Cells[$"AO{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AP{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1APriceTotal;
                //sheet.Cells[$"AP{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AQ{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1BQtyTotal;
                //sheet.Cells[$"AQ{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AR{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1BPriceTotal;
                //sheet.Cells[$"AR{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"Y{7 + result.Rows.Count}"].Value = EndingQty;
                sheet.Cells[$"Y{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"Z{7 + result.Rows.Count}"].Value = EndingTotal;
                sheet.Cells[$"Z{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
            else
            {
                sheet.Cells[$"AE{7 + result.Rows.Count}"].Value = SAMPLEQtyTotal;
                sheet.Cells[$"AE{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AF{7 + result.Rows.Count}"].Value = SAMPLEPriceTotal;
                sheet.Cells[$"AF{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AG{7 + result.Rows.Count}"].Value = ExpendSUBCONQtyTotal;
                sheet.Cells[$"AG{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AH{7 + result.Rows.Count}"].Value = ExpendSUBCONPriceTotal;
                sheet.Cells[$"AH{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AI{7 + result.Rows.Count}"].Value = ExpendOTHERQtyTotal;
                sheet.Cells[$"AI{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AJ{7 + result.Rows.Count}"].Value = ExpendOTHERPriceTotal;
                sheet.Cells[$"AJ{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

                //sheet.Cells[$"AK{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2AQtyTotal;
                //sheet.Cells[$"AK{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AL{7 + result.Rows.Count}"].Value = ExpendKonfeksi2APriceTotal;
                //sheet.Cells[$"AL{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AM{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2BQtyTotal;
                //sheet.Cells[$"AM{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AN{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2BPriceTotal;
                //sheet.Cells[$"AN{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AO{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2CQtyTotal;
                //sheet.Cells[$"AO{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AP{7 + result.Rows.Count}"].Value = ExpendKONFEKSI2CPriceTotal;
                //sheet.Cells[$"AP{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AQ{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1AQtyTotal;
                //sheet.Cells[$"AQ{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AR{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1APriceTotal;
                //sheet.Cells[$"AR{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AS{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1BQtyTotal;
                //sheet.Cells[$"AS{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //sheet.Cells[$"AT{7 + result.Rows.Count}"].Value = ExpendKONFEKSI1BPriceTotal;
                //sheet.Cells[$"AT{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AU{7 + result.Rows.Count}"].Value = EndingQty;
                sheet.Cells[$"AU{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[$"AV{7 + result.Rows.Count}"].Value = EndingTotal;
                sheet.Cells[$"AV{7 + result.Rows.Count}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }


            var widths = new int[] { 5, 10, 20, 15, 7, 20, 20, 10, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10/*, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 */};
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }
        private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
        {
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            //if (httpClient != null)
            //{
            //    var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            //    string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            //    string uri = garmentSupplierUri + queryUri;
            //    var response = httpClient.GetAsync($"{uri}").Result.Content.ReadAsStringAsync();
            //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
            //    List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());
            //    return viewModel;
            //}
            //else
            //{
            //    List<GarmentCategoryViewModel> viewModel = new List<GarmentCategoryViewModel>();
            //    return viewModel;
            //}

            var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            string uri = garmentSupplierUri + queryUri;
            var httpResponse = httpClient.GetAsync($"{uri}").Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentCategoryViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentCategoryViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                List<GarmentCategoryViewModel> viewModel = new List<GarmentCategoryViewModel>();
                return viewModel;
            }
        }

        private List<GarmentProductViewModel> GetProductCode(string codes)
        {
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");

            var garmentProductionUri = APIEndpoint.Core + $"master/garmentProducts/byCode";
            var httpResponse = httpClient.SendAsync(HttpMethod.Get, garmentProductionUri, httpContent).Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentProductViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentProductViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentProductViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                List<GarmentProductViewModel> viewModel = new List<GarmentProductViewModel>();
                return viewModel;
            }
        }

        //public List<GarmentProductViewModel> GetRemark(string itemcode)
        //{
        //    var param = new StringContent(JsonConvert.SerializeObject(itemcode), Encoding.UTF8, "application/json");
        //    string expenditureUri = APIEndpoint.Core + $"master/garmentProducts/byCode";

        //    IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

        //    var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
        //    if (httpResponse.IsSuccessStatusCode)
        //    {
        //        var content = httpResponse.Content.ReadAsStringAsync().Result;
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

        //        List<GarmentProductViewModel> viewModel;
        //        if (result.GetValueOrDefault("data") == null)
        //        {
        //            viewModel = null;
        //        }
        //        else
        //        {
        //            viewModel = JsonConvert.DeserializeObject<List<GarmentProductViewModel>>(result.GetValueOrDefault("data").ToString());

        //        }
        //        return viewModel;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

    }
}
