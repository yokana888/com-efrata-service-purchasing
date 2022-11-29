using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class MutationBeacukaiFacade : IMutationBeacukaiFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public MutationBeacukaiFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }
        #region BB
        public Tuple<List<MutationBBCentralViewModel>, int> GetReportBBCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<MutationBBCentralViewModel> Query = GetCentralItemBBReport(dateFrom, dateTo, offset).ToList();
            //Query = Query.OrderBy(x => x.ItemCode).ToList();

            Pageable<MutationBBCentralViewModel> pageable = new Pageable<MutationBBCentralViewModel>(Query, page - 1, size);
            List<MutationBBCentralViewModel> Data = pageable.Data.ToList<MutationBBCentralViewModel>();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }

        private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
        {
            ////var param = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");
            //IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            ////httpClient.GetAsync($"{APIEndpoint.Core}{currencyUri}/{currencyCode}").Result
            //if (httpClient != null)
            //{
            //    var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            //    string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            //    string uri = garmentSupplierUri + queryUri;
            //    //var response = httpClient.GetAsync($"{uri}").Result;
            //    var response = httpClient.GetAsync($"{uri}").Result;
            //    var response2  =  response.Content.ReadAsStringAsync();
            //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
            //    List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());
            //    return viewModel;
            //}
            //else
            //{
            //    List<GarmentCategoryViewModel> viewModel = null;
            //    return viewModel;
            //}

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            string uri = garmentSupplierUri + queryUri;
            var response = httpClient.GetAsync($"{APIEndpoint.Core}/master/garment-categories?page={page}&size={size}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                return viewModel ;
            }
            else
            {
                return null;
            }
        }

        private List<GarmentLeftoverWarehouseReportExpenditureViewModel> GetReportLeftOver(DateTime dateFrom, DateTime dateTo, string receiptType)
        {
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            //var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            //string queryUri = "?dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&page=1 &size=" + int.MaxValue + "&receiptType=" + receiptType;
            //string uri = garmentSupplierUri + queryUri;

            //string dateFromConv = dateFrom.ToString("MM/dd/yyyy");
            //string dateToConv = dateTo.ToString("MM/dd/yyyy");
            //var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-expenditures/report-expenditures?dateFrom={dateFromConv}&dateTo={dateToConv}&page=1&size={int.MaxValue}&receiptType={receiptType}").Result;
            var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-expenditures/report-expenditures?dateFrom={dateFrom}&dateTo={dateTo}&page=1&size={int.MaxValue}&receiptType={receiptType}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<GarmentLeftoverWarehouseReportExpenditureViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentLeftoverWarehouseReportExpenditureViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        private List<GarmentLeftoverWarehouseStockMonitoringViewModel> GetReportLeftOverStock(DateTime dateFrom, DateTime dateTo, string unit)
        {
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            //var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            //string queryUri = "?dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&page=1 &size=" + int.MaxValue + "&receiptType=" + receiptType;
            //string uri = garmentSupplierUri + queryUri;
            

            //string dateFromConv = dateFrom.ToString("MM/dd/yyyy");
            //string dateToConv = dateTo.ToString("MM/dd/yyyy");
            //var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-stocks/report/fabric?dateFrom={dateFromConv}&dateTo={dateToConv}&order={order}&page=1&size={int.MaxValue}&unit={unit}").Result;

            var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-stocks/report/fabric?dateFrom={dateFrom}&dateTo={dateTo}&page=1&size={int.MaxValue}&unit={unit}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<GarmentLeftoverWarehouseStockMonitoringViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentLeftoverWarehouseStockMonitoringViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        private List<ReceiptMonitoringViewModel> GetReportLeftOverReceipt(DateTime dateFrom, DateTime dateTo, string receiptType)
        {

            
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            //var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
            //string queryUri = "?dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&page=1 &size=" + int.MaxValue + "&receiptType=" + receiptType;
            //string uri = garmentSupplierUri + queryUri;
            //string dateFromConv = dateFrom.ToString("MM/dd/yyyy");
            //string dateToConv = dateTo.ToString("MM/dd/yyyy");
            //var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-receipts/monitoring?dateFrom={dateFromConv}&dateTo={dateToConv}&page=1&size={int.MaxValue}&type={receiptType}").Result;
            var response = httpClient.GetAsync($"{APIEndpoint.Inventory}/garment/leftover-warehouse-receipts/monitoring?dateFrom={dateFrom}&dateTo={dateTo}&page=1&size={int.MaxValue}&type={receiptType}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<ReceiptMonitoringViewModel> viewModel = JsonConvert.DeserializeObject<List<ReceiptMonitoringViewModel>>(result.GetValueOrDefault("data").ToString()); ;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        public List<MutationBBCentralViewModel> GetCentralItemBBReport(DateTime? datefrom, DateTime? dateto, int offset)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            var pengeluaran = new[] { "PROSES", "SAMPLE", "EXTERNAL", "SUBCON" };
            var pemasukan = new[] { "PROSES", "PEMBELIAN", "SISA SUBCON" };

            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");
            var coderequirement = new[] { "BP", "BE" };
            //var categories1 = categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray();
            var categories1 = new[] { "FABRIC", "SUBKON" };


            List<MutationBBCentralViewModelTemp> saldoawalreceipt = new List<MutationBBCentralViewModelTemp>();
            List<MutationBBCentralViewModelTemp> saldoawalexpenditure = new List<MutationBBCentralViewModelTemp>();
            List<MutationBBCentralViewModelTemp> saldoawalreceiptcorrection = new List<MutationBBCentralViewModelTemp>();

            #region Balance
            //var lastdate = dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault() == null ? new DateTime(1970, 1, 1) : dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();
            var lastdate = dbContext.GarmentStockOpnames.OrderByDescending(x => x.Date).Select(x => x.Date).FirstOrDefault();

            //var BalanceStock = (from a in dbContext.BalanceStocks
            //                    join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on (long)a.EPOItemId equals b.Id
            //                    join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
            //                    join e in dbContext.GarmentUnitReceiptNoteItems on (long)a.EPOItemId equals e.EPOItemId
            //                    join f in dbContext.GarmentUnitReceiptNotes on e.URNId equals f.Id
            //                    join g in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RO equals g.RONo
            //                    where a.CreateDate.Value.Date == lastdate
            //                    && f.URNType == "PEMBELIAN"
            //                    && categories1.Contains(b.ProductName)
            //                    select new MutationBBCentralViewModelTemp
            //                    {
            //                        AdjustmentQty = 0,
            //                        BeginQty = (double)a.CloseStock,
            //                        ExpenditureQty = 0,
            //                        ItemCode = b.ProductCode,
            //                        ItemName = b.ProductName,
            //                        LastQty = 0,
            //                        OpnameQty = 0,
            //                        ReceiptQty = 0,
            //                        SupplierType = c.SupplierImport == false ? "LOKAL" : "IMPORT",
            //                        UnitQtyName = b.DealUomUnit

            //                    }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
            //                    {
            //                        AdjustmentQty = group.Sum(x => x.AdjustmentQty),
            //                        BeginQty = group.Sum(x => x.BeginQty),
            //                        ExpenditureQty = group.Sum(x => x.ExpenditureQty),
            //                        ItemCode = key.ItemCode,
            //                        ItemName = key.ItemName,
            //                        LastQty = group.Sum(x => x.LastQty),
            //                        OpnameQty = group.Sum(x => x.OpnameQty),
            //                        ReceiptQty = group.Sum(x => x.ReceiptQty),
            //                        SupplierType = key.SupplierType,
            //                        UnitQtyName = key.UnitQtyName

            //                    });

            var BalanceStock = (from a in dbContext.GarmentStockOpnames
                               join b in dbContext.GarmentStockOpnameItems on a.Id equals b.GarmentStockOpnameId
                               join i in dbContext.GarmentDOItems on b.DOItemId equals i.Id
                               join c in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals c.Id
                               join g in dbContext.GarmentUnitReceiptNotes on c.URNId equals g.Id
                               join d in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on c.EPOItemId equals d.Id
                               join e in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on d.GarmentEPOId equals e.Id
                               join h in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RO equals h.RONo into PR
                           from prs in PR.DefaultIfEmpty()
                               where a.Date.Date == lastdate.Date
                               && i.CreatedUtc.Year <= DateTo.Date.Year
                               && a.IsDeleted == false && b.IsDeleted == false
                               && categories1.Contains(b.ProductName)
                               //&& pemasukan.Contains(g.URNType)
                                select new MutationBBCentralViewModelTemp
                               {
                                   AdjustmentQty = 0,
                                   BeginQty = Math.Round((double)b.Quantity,2),
                                   ExpenditureQty = 0,
                                   ItemCode = b.ProductCode,
                                   ItemName = b.ProductName,
                                   LastQty = 0,
                                   OpnameQty = 0,
                                   ReceiptQty = 0,
                                   SupplierType = e.SupplierImport == false ? "LOKAL" : "IMPORT",
                                   UnitQtyName = b.SmallUomUnit

                               }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                               {
                                   AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                   BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                   ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                   ItemCode = key.ItemCode,
                                   ItemName = key.ItemName,
                                   LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                   OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                   ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                   SupplierType = key.SupplierType,
                                   UnitQtyName = key.UnitQtyName

                               }).ToList();

            var ReceiptBalance = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                                  join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                                  join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                                  join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                  join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                                  from prs in PR.DefaultIfEmpty()
                                  where
                                     a.IsDeleted == false && b.IsDeleted == false
                                     &&
                                     b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                     && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                     && categories1.Contains(a.ProductName)
                                  //&& pemasukan.Contains(b.URNType)
                                  select new MutationBBCentralViewModelTemp
                                  {
                                      AdjustmentQty = 0,
                                      BeginQty = Math.Round((double)(a.ReceiptQuantity * a.Conversion), 2),
                                      ExpenditureQty = 0,
                                      ItemCode = a.ProductCode,
                                      ItemName = a.ProductName,
                                      LastQty = 0,
                                      OpnameQty = 0,
                                      ReceiptQty = 0,
                                      SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                      UnitQtyName = a.SmallUomUnit
                                  }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                  {
                                      AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                      BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                      ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                      ItemCode = key.ItemCode,
                                      ItemName = key.ItemName,
                                      LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                      OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                      ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                      SupplierType = key.SupplierType,
                                      UnitQtyName = key.UnitQtyName

                                  }).ToList();

            


            var ExpenditureBalance = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                                      join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                                      join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                                      join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                      //join h in Codes on a.ProductCode equals h.Code
                                      join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                                      from prs in PR.DefaultIfEmpty()
                                      where
                                      a.IsDeleted == false && b.IsDeleted == false
                                       &&
                                       b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                       && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                       && categories1.Contains(a.ProductName)
                                      //&& pengeluaran.Contains(b.ExpenditureType)
                                      select new MutationBBCentralViewModelTemp
                                      {
                                          AdjustmentQty = 0,
                                          BeginQty = a.UomUnit == "YARD" ? (double)a.Quantity * -1 * 0.9144 : -1 * (double)a.Quantity,
                                          ExpenditureQty = 0,
                                          ItemCode = a.ProductCode,
                                          ItemName = a.ProductName,
                                          LastQty = 0,
                                          OpnameQty = 0,
                                          ReceiptQty = 0,
                                          SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                          UnitQtyName = a.UomUnit == "YARD" ? "MT" : a.UomUnit

                                      }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                      {
                                          AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                          BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                          ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                          ItemCode = key.ItemCode,
                                          ItemName = key.ItemName,
                                          LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                          OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                          ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                          SupplierType = key.SupplierType,
                                          UnitQtyName = key.UnitQtyName

                                      }).ToList();

            

            var ReceiptCorrectionBalance = (from a in dbContext.GarmentUnitReceiptNotes
                                            join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                                            join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                            join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                            join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                                            join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                                            join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                                            from prs in PR.DefaultIfEmpty()
                                            where
                                            a.IsDeleted == false && b.IsDeleted == false
                                               &&
                                               g.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                               && g.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                               && categories1.Contains(b.ProductName)
                                            //&& pemasukan.Contains(a.URNType)
                                            select new MutationBBCentralViewModelTemp
                                            {
                                                AdjustmentQty = 0,
                                                BeginQty = (double)e.SmallQuantity,
                                                ExpenditureQty = 0,
                                                ItemCode = b.ProductCode,
                                                ItemName = b.ProductName,
                                                LastQty = 0,
                                                OpnameQty = 0,
                                                ReceiptQty = 0,
                                                SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                                UnitQtyName = b.SmallUomUnit
                                            }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                            {
                                                AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                                BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                                ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                                ItemCode = key.ItemCode,
                                                ItemName = key.ItemName,
                                                LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                                OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                                ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                                SupplierType = key.SupplierType,
                                                UnitQtyName = key.UnitQtyName

                                            }).ToList();

            //var leftoverbalance = GetReportLeftOverStock(new DateTime(2022, 1, 1), new DateTime(2022, 1, 1),"");
            var leftoverbalance = GetReportLeftOverStock(DateFrom, DateTo, "");
            var leftoverBalanceRemove = leftoverbalance.Where(x => x.ProductCode != "").AsQueryable();

            var totalQtyBefore = leftoverBalanceRemove.Sum(x => x.BeginingbalanceQty);

            var EPO = (from a in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters()
                       join b in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on a.GarmentEPOId equals b.Id
                       select new
                       {
                           PO_SerialNumber = a.PO_SerialNumber,
                           SupplierType = b.SupplierImport


                       }).GroupBy(x => new { x.PO_SerialNumber }).Select(d => new
                       {
                           PO_SerialNumber = d.Key.PO_SerialNumber,
                           SupplierType = d.FirstOrDefault().SupplierType

                       }).ToList();
                

            var leftoverbalanceQuery = (from a in leftoverBalanceRemove
                                        join b in EPO on a.PONo equals b.PO_SerialNumber
                                        //join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.PONo equals b.PO_SerialNumber

                                        //join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id

                                        //where  b.IsDeleted == false
                                        select new MutationBBCentralViewModelTemp
                                        {
                                           PONo = a.PONo,
                                           AdjustmentQty = 0,
                                           BeginQty = (double)a.BeginingbalanceQty,
                                           ExpenditureQty = 0,
                                           ItemCode = a.ProductCode,
                                           ItemName = a.ProductName,
                                           LastQty = 0,
                                           OpnameQty = 0,
                                           ReceiptQty = 0,
                                           SupplierType = b.SupplierType ==false? "LOKAL" : "IMPORT",
                                           //SupplierType =  "IMPORT",
                                           UnitQtyName = a.UomUnit
                                       }).GroupBy(x => new { x.PONo, x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                       {
                                           AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                           BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                           ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                           ItemCode = key.ItemCode,
                                           ItemName = key.ItemName,
                                           LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                           OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                           ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                           SupplierType = key.SupplierType,
                                           UnitQtyName = key.UnitQtyName

                                       }).ToList();

            //var totalQty = leftoverbalanceQuery.Sum(x => x.BeginQty);


            #endregion
            var SAwal = BalanceStock.Union(ReceiptBalance).Union(ExpenditureBalance).Union(ReceiptCorrectionBalance).Union(leftoverbalanceQuery).AsEnumerable();
            var SaldoAwal = SAwal.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
            {
                AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                BeginQty = group.Sum(x => x.BeginQty),
                ExpenditureQty = group.Sum(x => x.ExpenditureQty),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = group.Sum(x => x.LastQty),
                OpnameQty = group.Sum(x => x.OpnameQty),
                ReceiptQty = group.Sum(x => x.ReceiptQty),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName

            }).ToList();
            #region filtered
            var Receipt = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                           join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                           join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                           join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                           //join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                           //from prs in PR.DefaultIfEmpty()
                               //join h in Codes on a.ProductCode equals h.Code
                           where a.IsDeleted == false && b.IsDeleted == false
                               &&
                               b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                               && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                               && categories1.Contains(a.ProductName)
                               && pemasukan.Contains(b.URNType)

                           //group new { a, b, c, d } by new { b.ProductCode, b.ProductName, b.SmallUomUnit, d.SupplierImport } into data
                           select new MutationBBCentralViewModelTemp
                           {
                               AdjustmentQty = 0,
                               BeginQty = 0,
                               ExpenditureQty = 0,
                               ItemCode = a.ProductCode,
                               ItemName = a.ProductName,
                               LastQty = 0,
                               OpnameQty = 0,
                               ReceiptQty = (double)(a.ReceiptQuantity * a.Conversion),
                               SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                               UnitQtyName = a.SmallUomUnit
                           }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                           {
                               AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                               BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                               ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                               ItemCode = key.ItemCode,
                               ItemName = key.ItemName,
                               LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                               OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                               ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                               SupplierType = key.SupplierType,
                               UnitQtyName = key.UnitQtyName

                           }).ToList();


            
            var Expenditure = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                               join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                               join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                               join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                               //join h in Codes on a.ProductCode equals h.Code
                               join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                               from prs in PR.DefaultIfEmpty()
                               where a.IsDeleted == false && b.IsDeleted == false
                                    &&
                                    b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                                    && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                                    && categories1.Contains(a.ProductName)
                                    && pengeluaran.Contains(b.ExpenditureType)
                               select new MutationBBCentralViewModelTemp
                               {
                                   AdjustmentQty = 0,
                                   BeginQty = 0,
                                   ExpenditureQty = a.UomUnit == "YARD" ? (double)a.Quantity * 0.9144 : (double)a.Quantity,
                                   ItemCode = a.ProductCode,
                                   ItemName = a.ProductName,
                                   LastQty = 0,
                                   OpnameQty = 0,
                                   ReceiptQty = 0,
                                   SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                   UnitQtyName = a.UomUnit == "YARD" ? "MT" : a.UomUnit
                               }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                               {
                                   AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                   BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                   ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                   ItemCode = key.ItemCode,
                                   ItemName = key.ItemName,
                                   LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                   OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                   ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty),2),
                                   SupplierType = key.SupplierType,
                                   UnitQtyName = key.UnitQtyName

                               }).ToList();


            var ReceiptCorrection = (from a in dbContext.GarmentUnitReceiptNotes
                                     join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                                     join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                     join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                     join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                                     join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                                     join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                                     from prs in PR.DefaultIfEmpty()
                                         //join h in Codes on b.ProductCode equals h.Code
                                     where a.IsDeleted == false && b.IsDeleted == false
                                          &&
                                          g.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                                          && g.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                                          && categories1.Contains(b.ProductName)
                                          && pemasukan.Contains(a.URNType)
                                     select new MutationBBCentralViewModelTemp
                                     {
                                         AdjustmentQty = Math.Round(e.SmallQuantity,2),
                                         BeginQty = 0,
                                         ExpenditureQty = 0,
                                         ItemCode = b.ProductCode,
                                         ItemName = b.ProductName,
                                         LastQty = 0,
                                         OpnameQty = 0,
                                         ReceiptQty = 0,
                                         SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                         UnitQtyName = b.SmallUomUnit
                                     }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                     {
                                         AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                         BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                         ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                         ItemCode = key.ItemCode,
                                         ItemName = key.ItemName,
                                         LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                         OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                         ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                         SupplierType = key.SupplierType,
                                         UnitQtyName = key.UnitQtyName

                                     }).ToList();

            var leftoverreceipt = GetReportLeftOverReceipt(DateFrom, DateTo, "FABRIC");
            var leftoverexpenditure = GetReportLeftOver(DateFrom, DateTo, "FABRIC");

            var EPOExpend = (from a in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters()
                       join b in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on a.GarmentEPOId equals b.Id
                       //where a.IsDeleted == false && b.IsDeleted == false 
                       select new
                       {
                           PO_SerialNumber = a.PO_SerialNumber,
                           SupplierType = b.SupplierImport


                       }).GroupBy(x => new { x.PO_SerialNumber }).Select(d => new
                       {
                           PO_SerialNumber = d.Key.PO_SerialNumber,
                           SupplierType = d.FirstOrDefault().SupplierType

                       });


            var leftoverexpenditurefiltered = (from a in leftoverexpenditure
                                   // join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.PONo equals b.PO_SerialNumber
                                   //join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
                                   //where b.IsDeleted == false && c.IsDeleted == false
                                   //&& a.ExpenditureDestination == "JUAL LOKAL"

                                   join b in EPOExpend on a.PONo equals b.PO_SerialNumber
                                   where  a.ExpenditureDestination == "JUAL LOKAL"
                                   
                                   select new MutationBBCentralViewModelTemp
                                   {
                                       AdjustmentQty = 0,
                                       BeginQty = 0,
                                       ExpenditureQty = a.Quantity,
                                       ItemCode = a.Product.Code,
                                       ItemName = a.Product.Name,
                                       LastQty = 0,
                                       OpnameQty = 0,
                                       ReceiptQty = 0,
                                       //SupplierType = b.SupplierImport == false ? "LOKAL" : "IMPORT",
                                       SupplierType = b.SupplierType == false ? "LOKAL" : "IMPORT",
                                       UnitQtyName = a.Uom.Unit
                                   }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                   {
                                       AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                       BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                       ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                       ItemCode = key.ItemCode,
                                       ItemName = key.ItemName,
                                       LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                       OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                       ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                       SupplierType = key.SupplierType,
                                       UnitQtyName = key.UnitQtyName

                                   }).ToList();

            var leftoverreceiptfiltered = (from a in leftoverreceipt
                                    //       join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.POSerialNumber equals b.PO_SerialNumber
                                    //join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
                                    //where b.IsDeleted == false && c.IsDeleted == false

                                    join b in EPOExpend on a.POSerialNumber equals b.PO_SerialNumber
                                           where a.Product.Code != null
                                           
                                     select new MutationBBCentralViewModelTemp
                                    {
                                        AdjustmentQty = 0,
                                        BeginQty = 0,
                                        ExpenditureQty = 0,
                                        ItemCode = a.Product.Code,
                                        ItemName = a.Product.Name,
                                        LastQty = 0,
                                        OpnameQty = 0,
                                        ReceiptQty = a.Quantity,
                                        //SupplierType = c.SupplierImport == false ? "LOKAL" : "IMPORT",
                                        SupplierType = b.SupplierType == false ? "LOKAL" : "IMPORT",
                                        UnitQtyName = a.Uom.Unit
                                    }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
                                    {
                                        AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                        BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                        ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                        ItemCode = key.ItemCode,
                                        ItemName = key.ItemName,
                                        LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                        OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                        ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                        SupplierType = key.SupplierType,
                                        UnitQtyName = key.UnitQtyName

                                    }).ToList();

            //var SFiltered = Receipt.Union(Expenditure).Union(ReceiptCorrection).AsEnumerable();
            var SFiltered = Receipt.Union(Expenditure).Union(ReceiptCorrection).Union(leftoverexpenditurefiltered).Union(leftoverreceiptfiltered).AsEnumerable(); 

            //var SFiltered1 = Receipt.Union(Expenditure).Union(ReceiptCorrection).AsEnumerable();
            //var SFiltered2 = leftoverexpenditurefiltered.Union(leftoverreceiptfiltered).AsEnumerable();

            //var SfilteredUnion1 = SFiltered1.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
            //{
            //    AdjustmentQty = group.Sum(x => x.AdjustmentQty),
            //    BeginQty = group.Sum(x => x.BeginQty),
            //    ExpenditureQty = group.Sum(x => x.ExpenditureQty),
            //    ItemCode = key.ItemCode,
            //    ItemName = key.ItemName,
            //    LastQty = group.Sum(x => x.LastQty),
            //    OpnameQty = group.Sum(x => x.OpnameQty),
            //    ReceiptQty = group.Sum(x => x.ReceiptQty),
            //    SupplierType = key.SupplierType,
            //    UnitQtyName = key.UnitQtyName

            //});

            //var SfilteredUnion2 = SFiltered2.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
            //{
            //    AdjustmentQty = group.Sum(x => x.AdjustmentQty),
            //    BeginQty = group.Sum(x => x.BeginQty),
            //    ExpenditureQty = group.Sum(x => x.ExpenditureQty),
            //    ItemCode = key.ItemCode,
            //    ItemName = key.ItemName,
            //    LastQty = group.Sum(x => x.LastQty),
            //    OpnameQty = group.Sum(x => x.OpnameQty),
            //    ReceiptQty = group.Sum(x => x.ReceiptQty),
            //    SupplierType = key.SupplierType,
            //    UnitQtyName = key.UnitQtyName

            //});


            //var SFilteredUnion = SFiltered1.Union(SfilteredUnion2).AsEnumerable();




            var SaldoFilterd = SFiltered.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBBCentralViewModelTemp
            {
                AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                BeginQty = group.Sum(x => x.BeginQty),
                ExpenditureQty = group.Sum(x => x.ExpenditureQty),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = group.Sum(x => x.LastQty),
                OpnameQty = group.Sum(x => x.OpnameQty),
                ReceiptQty = group.Sum(x => x.ReceiptQty),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName

            }).ToList();

            var dataUnion = SaldoAwal.Union(SaldoFilterd).AsEnumerable();

            //var dataUnion = dataUnion1.ToList();
            #endregion



            var mutation1 = dataUnion.GroupBy(x => new { x.ItemCode, x.ItemName, x.UnitQtyName, x.SupplierType }, (key, group) => new MutationBBCentralViewModel
            {
                AdjustmentQty = Math.Round(Convert.ToDouble(group.Sum(x => x.AdjustmentQty)), 2),
                BeginQty = Math.Round(Convert.ToDouble(group.Sum(x => x.BeginQty)), 2),
                ExpenditureQty = Math.Round(Convert.ToDouble(group.Sum(x => x.ExpenditureQty)), 2),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = Math.Round(Convert.ToDouble(group.Sum(x => x.BeginQty + x.ReceiptQty + x.AdjustmentQty - x.ExpenditureQty - x.OpnameQty)), 2),
                ReceiptQty = Math.Round(Convert.ToDouble(group.Sum(x => x.ReceiptQty)), 2),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName,
                OpnameQty = 0,
                Diff = 0
            });

            var mutation = mutation1.OrderBy(x => x.ItemCode).ToList();

            //var mm = new MutationBBCentralViewModel();

            //mm.AdjustmentQty = Math.Round(mutation.Sum(x => x.AdjustmentQty), 2);
            //mm.BeginQty = Math.Round(mutation.Sum(x => x.BeginQty), 2);
            //mm.ExpenditureQty = Math.Round(mutation.Sum(x => x.ExpenditureQty), 2);
            //mm.ItemCode = "";
            //mm.ItemName = "";
            //mm.LastQty = Math.Round(mutation.Sum(x => x.LastQty), 2);
            //mm.ReceiptQty = Math.Round(mutation.Sum(x => x.ReceiptQty), 2);
            //mm.SupplierType = "";
            //mm.UnitQtyName = "";
            //mm.OpnameQty = 0;
            //mm.Diff = 0;

            var mutation2 = new List<MutationBBCentralViewModel>();

            foreach (var i in mutation)
            {
                var AdjustmentQty = i.AdjustmentQty > 0 ? i.AdjustmentQty : 0;
                var BeginQty = i.BeginQty > 0 ? i.BeginQty : 0;
                var ExpenditureQty = i.ExpenditureQty > 0 ? i.ExpenditureQty : 0;
                var LastQty = i.LastQty > 0 ? i.LastQty : 0;
                var OpnameQty = i.OpnameQty > 0 ? i.OpnameQty : 0;
                var Diff = i.Diff > 0 ? i.Diff : 0;

                mutation2.Add(new MutationBBCentralViewModel
                {
                    AdjustmentQty = i.AdjustmentQty,
                    BeginQty = BeginQty,
                    ExpenditureQty = i.ExpenditureQty,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    LastQty = LastQty,
                    ReceiptQty = i.ReceiptQty,
                    SupplierType = i.SupplierType,
                    UnitQtyName = i.UnitQtyName,
                    OpnameQty = i.OpnameQty,
                    Diff = i.Diff

                });
            }

            

            mutation2 = mutation2.Where(x => x.AdjustmentQty > 0 || x.BeginQty > 0 || x.Diff > 0 || x.ExpenditureQty > 0 || x.LastQty > 0 || x.OpnameQty > 0 || x.ReceiptQty > 0).ToList();
            //mutation2 = mutation2.Where(x => x.LastQty > 0).ToList();

            
            //disableSUMMARYallData
            //mutation2.Add(new MutationBBCentralViewModel {
            //    AdjustmentQty = mm.AdjustmentQty,
            //    BeginQty = mm.BeginQty,
            //    ExpenditureQty = mm.ExpenditureQty,
            //    ItemCode = mm.ItemCode,
            //    ItemName = mm.ItemName,
            //    LastQty = mm.LastQty,
            //    ReceiptQty = mm.ReceiptQty,
            //    SupplierType = mm.SupplierType,
            //    UnitQtyName = mm.UnitQtyName,
            //    OpnameQty = mm.OpnameQty,
            //    Diff = mm.Diff
            //});

            return mutation2;

        }

        public MemoryStream GenerateExcelBBCentral(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetCentralItemBBReport(dateFrom, dateTo, offset);
            //Query = Query.OrderBy(b => b.ItemCode).ToList();
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Gudang", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            //if (Query.ToArray().Count() == 0)
            //    result.Rows.Add("", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            //else
            var index = 1;
            foreach (var item in Query)
            {
                //result.Rows.Add((item.ItemCode), item.ItemName, item.SupplierType, item.UnitQtyName, item.BeginQty, item.ReceiptQty, item.ExpenditureQty, item.AdjustmentQty, item.LastQty, item.OpnameQty, item.Diff);
                result.Rows.Add(index++,item.ItemCode, item.ItemName, item.UnitQtyName, item.BeginQty, item.ReceiptQty, item.ExpenditureQty, item.LastQty, "Gudang AG2");

            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }
        #endregion 
        #region BP
        public Tuple<List<MutationBPCentralViewModel>, int> GetReportBPCentral(int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<MutationBPCentralViewModel> Query = GetCentralItemBPReport(dateFrom, dateTo, offset).ToList();
            //Query = Query.OrderBy(x => x.ItemCode).ToList();

            Pageable<MutationBPCentralViewModel> pageable = new Pageable<MutationBPCentralViewModel>(Query, page - 1, size);
            List<MutationBPCentralViewModel> Data = pageable.Data.ToList<MutationBPCentralViewModel>();
            int TotalData = pageable.TotalCount;
            //int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }

        public List<MutationBPCentralViewModel> GetCentralItemBPReport(DateTime? datefrom, DateTime? dateto, int offset)
        {
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            //var pengeluaran = new[] { "PROSES", "SAMPLE", "EXTERNAL" };
            //var pemasukan = new[] { "PROSES", "PEMBELIAN" };

            var pengeluaran = new[] { "PROSES", "SAMPLE", "EXTERNAL", "SUBCON" };
            var pemasukan = new[] { "PROSES", "PEMBELIAN", "SISA SUBCON" };

            //var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");
            var coderequirement = new[] { "BP", "BE" };
            //var categories1 = categories.Where(x => coderequirement.Contains(x.CodeRequirement)).Select(x => x.Name).ToArray();
            var categories1 = new[] { "FABRIC", "SUBKON" };

            #region Balance
            //var lastdate = dbContext.BalanceStocks.OrderByDescending(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();

            var lastdate = dbContext.GarmentStockOpnames.OrderByDescending(x => x.Date).Select(x => x.Date).FirstOrDefault();

            //var BalanceStock = (from a in dbContext.BalanceStocks
            //                    join b in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on (long)a.EPOItemId equals b.Id
            //                    join c in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on b.GarmentEPOId equals c.Id
            //                    join e in dbContext.GarmentUnitReceiptNoteItems on (long)a.EPOItemId equals e.EPOItemId
            //                    join f in dbContext.GarmentUnitReceiptNotes on e.URNId equals f.Id
            //                    join g in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RO equals g.RONo
            //                    where a.CreateDate.Value.Date == lastdate
            //                    && f.URNType == "PEMBELIAN"
            //                    && categories1.Contains(b.ProductName)

            //                    select new MutationBPCentralViewModelTemp
            //                    {
            //                        //AdjustmentQty = 0,
            //                        BeginQty = (double)a.CloseStock,
            //                        ExpenditureQty = 0,
            //                        ItemCode = b.ProductCode,
            //                        ItemName = b.ProductName,
            //                        //LastQty = 0,
            //                        //OpnameQty = 0,
            //                        ReceiptQty = 0,
            //                        SupplierType = c.SupplierImport,
            //                        UnitQtyName = b.DealUomUnit

            //                    }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModelTemp
            //                    {
            //                        //AdjustmentQty = group.Sum(x => x.AdjustmentQty),
            //                        BeginQty = group.Sum(x => x.BeginQty),
            //                        ExpenditureQty = group.Sum(x => x.ExpenditureQty),
            //                        ItemCode = key.ItemCode,
            //                        ItemName = key.ItemName,
            //                        //LastQty = group.Sum(x => x.LastQty),
            //                        //OpnameQty = group.Sum(x => x.OpnameQty),
            //                        ReceiptQty = group.Sum(x => x.ReceiptQty),
            //                        SupplierType = key.SupplierType,
            //                        UnitQtyName = key.UnitQtyName

            //                    });

            var BalanceStock = (from a in dbContext.GarmentStockOpnames
                                join b in dbContext.GarmentStockOpnameItems on a.Id equals b.GarmentStockOpnameId
                                join i in dbContext.GarmentDOItems on b.DOItemId equals i.Id
                                join c in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals c.Id
                                join g in dbContext.GarmentUnitReceiptNotes on c.URNId equals g.Id
                                join e in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on c.EPOItemId equals e.Id
                                join f in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on e.GarmentEPOId equals f.Id
                                join h in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RO equals h.RONo into PR
                                from prs in PR.DefaultIfEmpty()
                                where a.Date.Date == lastdate.Date
                                && i.CreatedUtc.Year <= DateTo.Date.Year
                                && a.IsDeleted == false && b.IsDeleted == false
                                && categories1.Contains(b.ProductName)
                                //&& pemasukan.Contains(g.URNType)
                                select new MutationBPCentralViewModel
                                {
                                    AdjustmentQty = 0,
                                    BeginQty = (double)b.Quantity,
                                    ExpenditureQty = 0,
                                    ItemCode = b.ProductCode,
                                    ItemName = b.ProductName,
                                    LastQty = 0,
                                    OpnameQty = 0,
                                    ReceiptQty = 0,
                                    SupplierType = f.SupplierImport == false ? "LOKAL" : "IMPORT",
                                    UnitQtyName = b.SmallUomUnit
                                }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                                {
                                    AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                                    BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                    ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                    ItemCode = key.ItemCode,
                                    ItemName = key.ItemName,
                                    LastQty = group.Sum(x => x.LastQty),
                                    OpnameQty = group.Sum(x => x.OpnameQty),
                                    ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                    SupplierType = key.SupplierType,
                                    UnitQtyName = key.UnitQtyName

                                });


            var ReceiptBalance = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                                  join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                                  join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                                  join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                  join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                                  from prs in PR.DefaultIfEmpty()
                                  where
                                     a.IsDeleted == false && b.IsDeleted == false
                                     &&
                                     b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                     && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                     && categories1.Contains(a.ProductName)
                                  //&& pemasukan.Contains(b.URNType)
                                  select new MutationBPCentralViewModel
                                  {
                                      AdjustmentQty = 0,
                                      BeginQty = Math.Round((double)(a.ReceiptQuantity * a.Conversion),2),
                                      ExpenditureQty = 0,
                                      ItemCode = a.ProductCode,
                                      ItemName = a.ProductName,
                                      OpnameQty = 0,
                                      ReceiptQty = 0,
                                      SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                      UnitQtyName = a.SmallUomUnit
                                  }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                                  {
                                      AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                                      BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                      ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                      ItemCode = key.ItemCode,
                                      ItemName = key.ItemName,
                                      LastQty = group.Sum(x => x.LastQty),
                                      OpnameQty = group.Sum(x => x.OpnameQty),
                                      ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                      SupplierType = key.SupplierType,
                                      UnitQtyName = key.UnitQtyName

                                  });




            var ExpenditureBalance = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                                      join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                                      join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                                      join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                      //join h in Codes on a.ProductCode equals h.Code
                                      join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                                      from prs in PR.DefaultIfEmpty()
                                      where
                                      a.IsDeleted == false && b.IsDeleted == false
                                       &&
                                       b.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                       && b.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                       && categories1.Contains(a.ProductName)
                                      //&& pengeluaran.Contains(b.ExpenditureType)
                                      select new MutationBPCentralViewModel
                                      {
                                          AdjustmentQty = 0,
                                          BeginQty = Math.Round(-1 * (double)a.Quantity,2),
                                          ExpenditureQty = 0,
                                          ItemCode = a.ProductCode,
                                          ItemName = a.ProductName,
                                          LastQty = 0,
                                          OpnameQty = 0,
                                          ReceiptQty = 0,
                                          SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                          UnitQtyName = a.UomUnit == "PACKS" ? "PCS" : a.UomUnit

                                      }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                                      {
                                          AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                                          BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                          ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                          ItemCode = key.ItemCode,
                                          ItemName = key.ItemName,
                                          LastQty = group.Sum(x => x.LastQty),
                                          OpnameQty = group.Sum(x => x.OpnameQty),
                                          ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                          SupplierType = key.SupplierType,
                                          UnitQtyName = key.UnitQtyName

                                      });

            var ReceiptCorrectionBalance = (from a in dbContext.GarmentUnitReceiptNotes
                                            join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                                            join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                            join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                            join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                                            join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                                            join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                                            from prs in PR.DefaultIfEmpty()
                                            where
                                            a.IsDeleted == false && b.IsDeleted == false
                                               &&
                                               g.CreatedUtc.AddHours(offset).Date >= lastdate.Date
                                               && g.CreatedUtc.AddHours(offset).Date < DateFrom.Date
                                               && categories1.Contains(b.ProductName)
                                            //&& pemasukan.Contains(a.URNType)
                                            select new MutationBPCentralViewModel
                                            {
                                                AdjustmentQty = 0,
                                                BeginQty = Math.Round((double)e.SmallQuantity,2),
                                                ExpenditureQty = 0,
                                                ItemCode = b.ProductCode,
                                                ItemName = b.ProductName,
                                                LastQty = 0,
                                                OpnameQty = 0,
                                                ReceiptQty = 0,
                                                SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                                UnitQtyName = b.SmallUomUnit
                                            }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                                            {
                                                AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                                                BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                                ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                                ItemCode = key.ItemCode,
                                                ItemName = key.ItemName,
                                                LastQty = Math.Round(group.Sum(x => x.LastQty), 2),
                                                OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                                                ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                                SupplierType = key.SupplierType,
                                                UnitQtyName = key.UnitQtyName

                                            });



            #endregion
            var SAwal = BalanceStock.Concat(ReceiptBalance).Concat(ExpenditureBalance).Concat(ReceiptCorrectionBalance).AsEnumerable();
            var SaldoAwal = SAwal.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
            {
                AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                BeginQty = group.Sum(x => x.BeginQty),
                ExpenditureQty = group.Sum(x => x.ExpenditureQty),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = group.Sum(x => x.LastQty),
                OpnameQty = group.Sum(x => x.OpnameQty),
                ReceiptQty = group.Sum(x => x.ReceiptQty),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName

            }).ToList();
            #region filtered
            var Receipt = (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                           join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                           join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                           join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                           //join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                           //from prs in PR.DefaultIfEmpty()
                               //join h in Codes on a.ProductCode equals h.Code
                           where a.IsDeleted == false && b.IsDeleted == false
                               &&
                               b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                               && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                               && categories1.Contains(a.ProductName)
                                && pemasukan.Contains(b.URNType)

                           //group new { a, b, c, d } by new { b.ProductCode, b.ProductName, b.SmallUomUnit, d.SupplierImport } into data
                           select new MutationBPCentralViewModel
                           {
                               AdjustmentQty = 0,
                               BeginQty = 0,
                               ExpenditureQty = 0,
                               ItemCode = a.ProductCode,
                               ItemName = a.ProductName,
                               LastQty = 0,
                               OpnameQty = 0,
                               ReceiptQty = Math.Round((double)(a.ReceiptQuantity * a.Conversion),2),
                               SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                               UnitQtyName = a.SmallUomUnit
                           }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                           {
                               AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                               BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                               ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                               ItemCode = key.ItemCode,
                               ItemName = key.ItemName,
                               LastQty = group.Sum(x => x.LastQty),
                               OpnameQty = group.Sum(x => x.OpnameQty),
                               ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                               SupplierType = key.SupplierType,
                               UnitQtyName = key.UnitQtyName

                           });

            //Receipt = Receipt.Where(x => (x.ItemCode != "APL001") && (x.ItemCode != "EMB001") && (x.ItemCode != "GMT001") && (x.ItemCode != "PRN001") && (x.ItemCode != "SMP001") && (x.ItemCode != "WSH001"));


            var Expenditure = (from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                               join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                               join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                               join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                               //join h in Codes on a.ProductCode equals h.Code
                               join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on a.RONo equals e.RONo into PR
                               from prs in PR.DefaultIfEmpty()
                               where a.IsDeleted == false && b.IsDeleted == false
                                    &&
                                    b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                                    && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                                    && categories1.Contains(a.ProductName)
                                    && pengeluaran.Contains(b.ExpenditureType)
                               select new MutationBPCentralViewModel
                               {
                                   AdjustmentQty = 0,
                                   BeginQty = 0,
                                   ExpenditureQty = Math.Round((double)a.Quantity,2),
                                   ItemCode = a.ProductCode,
                                   ItemName = a.ProductName,
                                   LastQty = 0,
                                   OpnameQty = 0,
                                   ReceiptQty = 0,
                                   SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                   UnitQtyName = a.UomUnit == "YARD" ? "MT" : a.UomUnit
                               }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                               {
                                   AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                                   BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                   ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                   ItemCode = key.ItemCode,
                                   ItemName = key.ItemName,
                                   LastQty = group.Sum(x => x.LastQty),
                                   OpnameQty = group.Sum(x => x.OpnameQty),
                                   ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                   SupplierType = key.SupplierType,
                                   UnitQtyName = key.UnitQtyName

                               });


            var ReceiptCorrection = (from a in dbContext.GarmentUnitReceiptNotes
                                     join b in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa) on a.Id equals b.URNId
                                     join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals c.Id
                                     join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                                     join e in dbContext.GarmentReceiptCorrectionItems on b.Id equals e.URNItemId
                                     join g in dbContext.GarmentReceiptCorrections on e.CorrectionId equals g.Id
                                     join f in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select new { gg.BuyerCode, gg.Article, gg.RONo }).Distinct() on b.RONo equals f.RONo into PR
                                     from prs in PR.DefaultIfEmpty()
                                         //join h in Codes on b.ProductCode equals h.Code
                                     where a.IsDeleted == false && b.IsDeleted == false
                                          &&
                                          g.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                                          && g.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                                          && categories1.Contains(b.ProductName)
                                          && pemasukan.Contains(a.URNType)
                                     //where b.ProductCode == "TC07547"
                                     select new MutationBPCentralViewModel
                                     {
                                         AdjustmentQty = Math.Round(e.SmallQuantity, 2),
                                         BeginQty = 0,
                                         ExpenditureQty = 0,
                                         ItemCode = b.ProductCode,
                                         ItemName = b.ProductName,
                                         LastQty = 0,
                                         OpnameQty = 0,
                                         ReceiptQty = 0,
                                         SupplierType = d.SupplierImport == false ? "LOKAL" : "IMPORT",
                                         UnitQtyName = b.SmallUomUnit
                                     }).GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
                                     {
                                         AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                                         BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                                         ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                                         ItemCode = key.ItemCode,
                                         ItemName = key.ItemName,
                                         LastQty = group.Sum(x => x.LastQty),
                                         OpnameQty = group.Sum(x => x.OpnameQty),
                                         ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                                         SupplierType = key.SupplierType,
                                         UnitQtyName = key.UnitQtyName

                                     });
            #endregion

            var SFiltered = Receipt.Concat(Expenditure).Concat(ReceiptCorrection).AsEnumerable();
            var SaldoFilterd = SFiltered.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
            {
                AdjustmentQty = group.Sum(x => x.AdjustmentQty),
                BeginQty = group.Sum(x => x.BeginQty),
                ExpenditureQty = group.Sum(x => x.ExpenditureQty),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = group.Sum(x => x.LastQty),
                OpnameQty = group.Sum(x => x.OpnameQty),
                ReceiptQty = group.Sum(x => x.ReceiptQty),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName

            }).ToList();

            var dataUnion = SaldoAwal.Concat(SaldoFilterd).AsEnumerable();

            var mutationgroup = dataUnion.GroupBy(x => new { x.ItemCode, x.ItemName, x.SupplierType, x.UnitQtyName }, (key, group) => new MutationBPCentralViewModel
            {
                AdjustmentQty = Math.Round(group.Sum(x => x.AdjustmentQty), 2),
                BeginQty = Math.Round(group.Sum(x => x.BeginQty), 2),
                ExpenditureQty = Math.Round(group.Sum(x => x.ExpenditureQty), 2),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                LastQty = Math.Round(group.Sum(x => x.BeginQty) + group.Sum(x => x.ReceiptQty) - group.Sum(x => x.ExpenditureQty) + group.Sum(x => x.AdjustmentQty) + group.Sum(x => x.OpnameQty), 2),
                ReceiptQty = Math.Round(group.Sum(x => x.ReceiptQty), 2),
                SupplierType = key.SupplierType,
                UnitQtyName = key.UnitQtyName,
                OpnameQty = Math.Round(group.Sum(x => x.OpnameQty), 2),
                Diff = Math.Round(group.Sum(x => x.Diff), 2)
            });



            var mm = new MutationBPCentralViewModel();

            var bpmutation = mutationgroup.Where(x => (x.ItemCode != "APL001") && (x.ItemCode != "EMB001") && (x.ItemCode != "GMT001") && (x.ItemCode != "PRN001") && (x.ItemCode != "SMP001") && (x.ItemCode != "WSH001") && (x.ItemCode != "QLT001") && (x.ItemCode != "SMT001")).ToList();

            mm.AdjustmentQty = Math.Round(bpmutation.Sum(x => x.AdjustmentQty), 2);
            mm.BeginQty = Math.Round(bpmutation.Sum(x => x.BeginQty), 2);
            mm.ExpenditureQty = Math.Round(bpmutation.Sum(x => x.ExpenditureQty), 2);
            mm.ItemCode = "";
            mm.ItemName = "";
            mm.LastQty = Math.Round(bpmutation.Sum(x => x.LastQty), 2);
            mm.ReceiptQty = Math.Round(bpmutation.Sum(x => x.ReceiptQty), 2);
            mm.SupplierType = "";
            mm.UnitQtyName = "";
            mm.OpnameQty = 0;
            mm.Diff = 0;

            List<MutationBPCentralViewModel> mutations = new List<MutationBPCentralViewModel>();

            foreach (var item in mutationgroup.Where(x => (x.ItemCode != "APL001") && (x.ItemCode != "EMB001") && (x.ItemCode != "GMT001") && (x.ItemCode != "PRN001") && (x.ItemCode != "SMP001") && (x.ItemCode != "WSH001") && (x.ItemCode != "QLT001") && (x.ItemCode != "SMT001")).ToList())
            {
                //var AdjustmentQty = item.AdjustmentQty > 0 ? item.AdjustmentQty : 0;
                var BeginQty = item.BeginQty > 0 ? item.BeginQty : 0;
                var ExpenditureQty = item.ExpenditureQty > 0 ? item.ExpenditureQty : 0;
                var LastQty = item.LastQty > 0 ? item.LastQty : 0;
                var ReceiptQty = item.ReceiptQty > 0 ? item.ReceiptQty : 0;
                var OpnameQty = item.OpnameQty > 0 ? item.OpnameQty : 0;
                var Diff = item.Diff > 0 ? item.Diff : 0;

                MutationBPCentralViewModel mutation = new MutationBPCentralViewModel()
                {
                    AdjustmentQty = item.AdjustmentQty,
                    BeginQty = BeginQty,
                    ExpenditureQty = ExpenditureQty,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    LastQty = LastQty,
                    ReceiptQty = ReceiptQty,
                    SupplierType = item.SupplierType,
                    UnitQtyName = item.UnitQtyName,
                    OpnameQty = 0,
                    Diff = 0
                };

                mutations.Add(mutation);
            }

            


            mutations = mutations.OrderBy(x => x.ItemCode).ToList();

            mutations = mutations.Where(x => x.ItemCode != "APL001" || x.ItemCode != "EMB001" || x.ItemCode != "GMT001" || x.ItemCode != "PRN001" || x.ItemCode != "SMP001" || x.ItemCode != "WSH001" || x.ItemCode != "QLT001" || x.ItemCode != "SMT001").ToList();

            mutations = mutations.Where(x => x.AdjustmentQty > 0 || x.BeginQty > 0 || x.Diff > 0 || x.ExpenditureQty > 0 || x.LastQty > 0 || x.OpnameQty > 0 || x.ReceiptQty > 0).ToList();
            //mutations = mutations.Where(x => x.AdjustmentQty != 0 || x.BeginQty != 0 || x.Diff != 0 || x.ExpenditureQty != 0 || x.LastQty != 0 || x.OpnameQty != 0 || x.ReceiptQty != 0).ToList();
            //mutations = mutations.Where(x => x.LastQty > 0).ToList();

            //foreach(var i in mutations)
            //{
            //    i.BeginQty = i.BeginQty > 0 ? i.BeginQty : 0;
            //    i.LastQty = i.LastQty > 0 ? i.LastQty : 0;
            //}



            mutations.Add(new MutationBPCentralViewModel
            {
                AdjustmentQty = mm.AdjustmentQty,
                BeginQty = mm.BeginQty,
                ExpenditureQty = mm.ExpenditureQty,
                ItemCode = mm.ItemCode,
                ItemName = mm.ItemName,
                LastQty = mm.LastQty,
                ReceiptQty = mm.ReceiptQty,
                SupplierType = mm.SupplierType,
                UnitQtyName = mm.UnitQtyName,
                OpnameQty = mm.OpnameQty,
                Diff = mm.Diff
            });

            return mutations;



        }

        public MemoryStream GenerateExcelBPCentral(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetCentralItemBPReport(dateFrom, dateTo, offset);
            //Query = Query.OrderBy(b => b.ItemCode).ToList();
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
                foreach (var item in Query)
                {
                    result.Rows.Add((item.ItemCode), item.ItemName, item.SupplierType, item.UnitQtyName, item.BeginQty, item.ReceiptQty, item.ExpenditureQty, item.AdjustmentQty, item.LastQty, item.OpnameQty, item.Diff);
                }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }

        #endregion


    }

}
