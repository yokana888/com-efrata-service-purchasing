using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentExpenditureGood;
using System.Net.Http;
using Newtonsoft.Json;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentFinishingOut;
using System.IO;
using System.Data;
using OfficeOpenXml;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.BeacukaiAdded;
using System.Globalization;
using OfficeOpenXml.Style;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentCuttingOut;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentPreparing;
using Com.Moonlay.NetCore.Lib;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using static Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports.TraceableInBeacukaiViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentSampleExpenditureGood;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class TraceableBeacukaiFacade : ITraceableBeacukaiFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;

        public TraceableBeacukaiFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
        }

        #region Masuk

        public List<GarmentExpenditureGoodViewModel> GetExpenditureGood(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"expenditure-goods/traceable-by-ro";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentExpenditureGoodViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentExpenditureGoodViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentExpenditureGoodViewModel>();
            }
        }

        public List<GarmentSampleExpenditureGoodViewModel> GetSampleExpenditureGood(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"garment-sample-expenditure-goods/traceable-by-ro";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentSampleExpenditureGoodViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentSampleExpenditureGoodViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentSampleExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentSampleExpenditureGoodViewModel>();
            }
        }

        public List<GarmentCuttingOutViewModel> GetCuttingOut(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"cutting-outs/for-traceable";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentCuttingOutViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentCuttingOutViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentCuttingOutViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentCuttingOutViewModel>();
            }
        }

        public List<GarmentCuttingOutViewModel> GetSampleCuttingOut(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"garment-sample-cutting-outs/for-traceable";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentCuttingOutViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentCuttingOutViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentCuttingOutViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentCuttingOutViewModel>();
            }
        }

        public List<GarmentFinishingOutViewModel> GetFinishingOut(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"finishing-outs/for-traceable";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentFinishingOutViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentFinishingOutViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentFinishingOutViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentFinishingOutViewModel>();
            }
        }

        public List<GarmentFinishingOutViewModel> GetSampleFinishingOut(string RONo)
        {
            var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"garment-sample-finishing-outs/traceable-by-ro";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentFinishingOutViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentFinishingOutViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentFinishingOutViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentFinishingOutViewModel>();
            }
        }

        public List<GarmentForTraceableIN> GetFullGarment(string UENItemId)
        {
            var param = new StringContent(JsonConvert.SerializeObject(UENItemId), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"finishing-outs/for-traceable-full-garment";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentForTraceableIN> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentForTraceableIN>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentForTraceableIN>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentForTraceableIN>();
            }
        }

        public List<GarmentForTraceableIN> GetFullGarmentSample(string UENItemId)
        {
            var param = new StringContent(JsonConvert.SerializeObject(UENItemId), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"finishing-outs/for-traceable-full-garment-sample";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentForTraceableIN> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentForTraceableIN>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentForTraceableIN>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentForTraceableIN>();
            }
        }

        public List<BeacukaiAddedViewModel> GetPEB(string invoice)
        {
            var param = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            string shippingInvoiceUri = APIEndpoint.CustomsReport + $"customs-reports/getPEB";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, shippingInvoiceUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<BeacukaiAddedViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<BeacukaiAddedViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<BeacukaiAddedViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<BeacukaiAddedViewModel>();
            }
        }

        public Tuple<List<TraceableInBeacukaiViewModel>, int> GetReportTraceableIN(string filter, string tipe, string tipebc)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<TraceableInBeacukaiViewModel> Query = GetTraceableInQuery(filter, tipe, tipebc);
            //Query = Query.OrderBy(x => x.ItemCode).ToList();
            int TotalData = Query.Count;
            //int TotalData = Data.Count();
            return Tuple.Create(Query, TotalData);
        }


        public List<TraceableInBeacukaiViewModel> GetTraceableInQuery(string filter, string tipe, string tipebc)
        {
            //var Query = (from a in (from aa in dbContext.GarmentBeacukais
            //                        where aa.BeacukaiNo == filter && aa.CustomsType == tipebc
            //                        select aa)
            //             join t in dbContext.GarmentDeliveryOrders on a.Id equals t.CustomsId
            //             join c in dbContext.GarmentDeliveryOrderItems on t.Id equals c.GarmentDOId
            //             join d in dbContext.GarmentDeliveryOrderDetails on c.Id equals d.GarmentDOItemId
            //             join e in dbContext.GarmentUnitReceiptNoteItems on d.Id equals e.DODetailId
            //             join r in dbContext.GarmentUnitReceiptNotes on e.URNId equals r.Id
            //             join f in dbContext.GarmentUnitDeliveryOrderItems on e.Id equals f.URNItemId into unitdoitem
            //             from ff in unitdoitem.DefaultIfEmpty()
            //             join g in dbContext.GarmentUnitDeliveryOrders on ff.UnitDOId equals g.Id into unitdo
            //             from gg in unitdo.DefaultIfEmpty()
            //             join h in dbContext.GarmentUnitExpenditureNoteItems on ff.Id equals h.UnitDOItemId into uenitem
            //             from hh in uenitem.DefaultIfEmpty()
            //             join i in dbContext.GarmentUnitExpenditureNotes on hh.UENId equals i.Id into uen
            //             from ii in uen.DefaultIfEmpty()
            //             where r.URNType == "PEMBELIAN"
            //             select new TraceableInBeacukaiViewModelTemp
            //             {
            //                 BCDate = a.BeacukaiDate,
            //                 BCNo = a.BeacukaiNo,
            //                 BCType = a.CustomsType,
            //                 BJQty = 0,
            //                 BonNo = a.BillNo,
            //                 ItemCode = d.ProductCode,
            //                 ItemName = d.ProductName,
            //                 ReceiptQty = Math.Round((double)(e.ReceiptQuantity * e.Conversion), 2),
            //                 QtyBUK = ii != null ? (ii.ExpenditureType == "PROSES" ? hh.Quantity : 0) : 0,
            //                 SampleQtyOut = ii != null ? (ii.ExpenditureType == "SAMPLE" ? hh.Quantity : 0) : 0,
            //                 ROJob = gg.UnitDOType == "PROSES" ? gg.RONo : "rojob-",
            //                 ROSample = gg.UnitDOType == "SAMPLE" ? gg.RONo : "rosamp-",
            //                 SatuanBUK = hh != null ? hh.UomUnit : "satbuk-",
            //                 SatuanReceipt = e.SmallUomUnit,
            //                 UnitDOType = gg.UnitDOType,
            //                 EksporQty = 0,
            //                 SampleQty = ii != null ? (ii.ExpenditureType == "SAMPLE" ? hh.Quantity : 0) : 0,
            //             });

            //New Query
            //var expendType = new string[] { "PROSES", "SAMPLE" };

            //var Query = (from a in dbContext.GarmentBeacukais
            //             join b in dbContext.GarmentBeacukaiItems on a.Id equals b.BeacukaiId
            //             join c in dbContext.GarmentUnitReceiptNotes on b.GarmentDONo equals c.DONo into urn
            //             from cc in urn.DefaultIfEmpty()
            //             join d in dbContext.GarmentUnitReceiptNoteItems on cc.Id equals d.URNId into urnitem
            //             from dd in urnitem.DefaultIfEmpty()
            //             join g in dbContext.GarmentUnitDeliveryOrderItems on dd.Id equals g.URNItemId into unitdoitem
            //             from gg in unitdoitem.DefaultIfEmpty()
            //             join h in dbContext.GarmentUnitDeliveryOrders on gg.UnitDOId equals h.Id into unitdo
            //             from hh in unitdo.DefaultIfEmpty()
            //             join e in dbContext.GarmentUnitExpenditureNoteItems on gg.Id equals e.UnitDOItemId into uenitem
            //             from ee in uenitem.DefaultIfEmpty()
            //             join f in dbContext.GarmentUnitExpenditureNotes on ee.UENId equals f.Id into uen
            //             from ff in uen.DefaultIfEmpty()
            //             where cc.URNType == "PEMBELIAN" && expendType.Contains(ff.ExpenditureType)
            //            && a.BeacukaiNo == filter && a.CustomsType == tipebc
            //             select new TraceableInBeacukaiViewModelTemp
            //             {

            //                 BCDate = a.BeacukaiDate,
            //                 BCNo = a.BeacukaiNo,
            //                 BCType = a.CustomsType,
            //                 BonNo = a.BillNo,
            //                 ItemCode = ee.ProductCode,
            //                 ItemName = ee.ProductName,
            //                 ReceiptQty = Math.Round((double)(dd.ReceiptQuantity * dd.Conversion), 2),
            //                 SatuanReceipt = dd.SmallUomUnit,
            //                 QtyBUK = ee != null ? (ff.ExpenditureType == "PROSES" ? ee.Quantity : 0) : 0,
            //                 SampleQtyOut = ee != null ? (ff.ExpenditureType == "SAMPLE" ? ee.Quantity : 0) : 0,
            //                 SatuanBUK = ee != null ? ee.UomUnit : "satbuk-",
            //                 ROJob = hh.UnitDOType == "PROSES" ? hh.RONo : "rojob-",
            //                 ROSample = hh.UnitDOType == "SAMPLE" ? hh.RONo : "rosamp-",
            //                 //ROJob = ff.ExpenditureType == "PROSES" ? ee.RONo : "rojob-",
            //                 //ROSample = ff.ExpenditureType == "SAMPLE" ? ee.RONo : "rosamp-",
            //                 ExpenditureType = ff.ExpenditureType,
            //                 EksporQty = 0,
            //                 SampleQty = ee != null ? (ff.ExpenditureType == "SAMPLE" ? ee.Quantity : 0) : 0,
            //                 UENItemId = ee.Id
            //             });

            //Second Query
            var queryReceipt = (from a in (from aa in dbContext.GarmentBeacukais
                                           where aa.BeacukaiNo == filter 
                                           //&& aa.CustomsType == tipebc
                                           select aa)
                                join b in dbContext.GarmentBeacukaiItems on a.Id equals b.BeacukaiId
                                join c in dbContext.GarmentUnitReceiptNotes on b.GarmentDONo equals c.DONo into urn
                                from cc in urn.DefaultIfEmpty()
                                join d in dbContext.GarmentUnitReceiptNoteItems on cc.Id equals d.URNId into urnitem
                                from dd in urnitem.DefaultIfEmpty()
                                where cc.URNType == "PEMBELIAN"
                                select new TraceableInForToReceipt
                                {
                                    BCDate = a.BeacukaiDate,
                                    BCNo = a.BeacukaiNo,
                                    BCType = a.CustomsType,
                                    BonNo = a.BillNo,
                                    ItemCode = dd.ProductCode,
                                    ItemName = dd.ProductName,
                                    POSerialNumber = dd.POSerialNumber,
                                    ReceiptQty = Math.Round((double)(dd.ReceiptQuantity * dd.Conversion), 2),
                                    SatuanReceipt = dd.SmallUomUnit,
                                    URNItemId = dd.Id
                                });
            var ReceiptValue = queryReceipt.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BonNo, x.ItemCode, x.ItemName, x.SatuanReceipt, x.POSerialNumber }, (key, group) => new TraceableInBeacukaiViewModelTemp
            {
                BCDate = key.BCDate,
                BCNo = key.BCNo,
                BCType = key.BCType,
                BonNo = key.BonNo,
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                ReceiptQty = group.Sum(x => x.ReceiptQty),
                SatuanReceipt = key.SatuanReceipt,
                PO = key.POSerialNumber
            });

            var POReceipt = queryReceipt.Select(x => x.POSerialNumber).Distinct();

            var expendType = new string[] { "PROSES", "SAMPLE" };
            var queryExpend = from g in (from gg in dbContext.GarmentUnitDeliveryOrderItems
                                         where POReceipt.Contains(gg.POSerialNumber)
                                         select gg)
                              join h in dbContext.GarmentUnitDeliveryOrders on g.UnitDOId equals h.Id into unitdo
                              from hh in unitdo.DefaultIfEmpty()
                              join e in dbContext.GarmentUnitExpenditureNoteItems on g.Id equals e.UnitDOItemId into uenitem
                              from ee in uenitem.DefaultIfEmpty()
                              join f in dbContext.GarmentUnitExpenditureNotes on ee.UENId equals f.Id into uen
                              from ff in uen.DefaultIfEmpty()
                              where expendType.Contains(ff.ExpenditureType)
                              select new TraceableInForToExpend
                              {
                                  QtyBUK = ee != null ? (ff.ExpenditureType == "PROSES" ? ee.Quantity : 0) : 0,
                                  SampleQtyOut = ee != null ? (ff.ExpenditureType == "SAMPLE" ? ee.Quantity : 0) : 0,
                                  SatuanBUK = ee != null ? ee.UomUnit : "satbuk-",
                                  ROJob = hh.UnitDOType == "PROSES" ? hh.RONo : "rojob-",
                                  ROSample = hh.UnitDOType == "SAMPLE" ? hh.RONo : "rosamp-",
                                  //ROJob = ff.ExpenditureType == "PROSES" ? ee.RONo : "rojob-",
                                  //ROSample = ff.ExpenditureType == "SAMPLE" ? ee.RONo : "rosamp-",
                                  ExpenditureType = ff.ExpenditureType,
                                  //EksporQty = 0,
                                  SampleQty = ee != null ? (ff.ExpenditureType == "SAMPLE" ? ee.Quantity : 0) : 0,
                                  ItemCode = ee.ProductCode,
                                  ItemName = ee.ProductName,
                                  UnitDOType = hh.UnitDOType,
                                  POSerialNumber = ee.POSerialNumber,
                                  UENItemId = ee.Id,

                              };

            var ExpendValue = queryExpend.GroupBy(x => new { x.SatuanBUK, x.ROSample, x.ExpenditureType, x.ItemCode, x.ItemName, x.POSerialNumber, x.ROJob, x.UnitDOType }, (key, group) => new TraceableInBeacukaiViewModelTemp
            {
                SampleQtyOut = group.Sum(x => x.SampleQtyOut),
                QtyBUK = group.Sum(x => x.QtyBUK),
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                //ROJob = key.ROJob,
                ROSample = key.ROSample,
                SatuanBUK = key.SatuanBUK,
                SampleQty = group.Sum(x => x.SampleQty),
                ExpenditureType = key.ExpenditureType,
                PO = key.POSerialNumber,
                ROJob = key.ROJob,
                UnitDOType = key.UnitDOType

            });

            List<TraceableInBeacukaiViewModelTemp> queryyss = new List<TraceableInBeacukaiViewModelTemp>();
            foreach (var a in ExpendValue)
            {
                var match = ReceiptValue.FirstOrDefault(x => x.PO == a.PO);

                var querrr = new TraceableInBeacukaiViewModelTemp
                {
                    BCDate = match.BCDate,
                    BCNo = match.BCNo,
                    BCType = match.BCType,
                    BonNo = match.BonNo,
                    ReceiptQty = match.ReceiptQty,
                    SatuanReceipt = match.SatuanReceipt,

                    SampleQtyOut = a.SampleQtyOut,
                    QtyBUK = a.QtyBUK,
                    ItemCode = a.ItemCode,
                    ItemName = a.ItemName,
                    //ROJob = key.ROJob,
                    ROSample = a.ROSample,
                    SatuanBUK = a.SatuanBUK,
                    SampleQty = a.SampleQty,
                    ExpenditureType = a.ExpenditureType,
                    ROJob = a.ROJob,
                    UnitDOType = a.UnitDOType
                };

                queryyss.Add(querrr);



            }

            var queryNow = ReceiptValue.Union(ExpendValue).AsEnumerable();

            //QuerySecond
            var Query = queryyss.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BonNo, x.ItemCode, x.SatuanReceipt, x.ItemName, x.SatuanBUK, x.ROJob, x.ROSample, x.UnitDOType, x.ReceiptQty, x.SampleQtyOut, x.SampleQty, x.QtyBUK }, (key, group) => new TraceableInBeacukaiViewModelTemp
            {
                BCDate = key.BCDate,
                BCNo = key.BCNo,
                BCType = key.BCType,
                BonNo = key.BonNo,
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                //ReceiptQty = group.Sum(x => x.ReceiptQty),
                ReceiptQty = key.ReceiptQty,
                SatuanReceipt = key.SatuanReceipt,
                //SampleQtyOut = group.Sum(x => x.SampleQtyOut),
                //QtyBUK = group.Sum(x => x.QtyBUK),
                SampleQtyOut = key.SampleQtyOut,
                QtyBUK = key.QtyBUK,

                ROJob = key.ROJob,
                ROSample = key.ROSample,
                SatuanBUK = key.SatuanBUK,
                //SampleQty = group.Sum(x => x.SampleQty),
                SampleQty = key.SampleQty,
                UnitDOType = key.UnitDOType
                //ExpenditureType = key.ExpenditureType
            });

            //FirstQuery
            //var Query = Querys.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BonNo, x.ItemCode, x.SatuanReceipt, x.ItemName, x.SatuanBUK, x.ROJob, x.ROSample, x.UnitDOType,x.ReceiptQty,x.SampleQtyOut,x.QtyBUK,x.SampleQty }, (key, group) => new TraceableInBeacukaiViewModelTemp
            //{
            //    BCDate = key.BCDate,
            //    BCNo = key.BCNo,
            //    BCType = key.BCType,
            //    BonNo = key.BonNo,
            //    ItemCode = key.ItemCode,
            //    ItemName = key.ItemName,
            //    //ReceiptQty = group.Sum(x => x.ReceiptQty),
            //    ReceiptQty = key.ReceiptQty,
            //    SatuanReceipt = key.SatuanReceipt,
            //    //SampleQtyOut = group.Sum(x => x.SampleQtyOut),
            //    //QtyBUK = group.Sum(x => x.QtyBUK),
            //    SampleQtyOut = key.SampleQtyOut,
            //    QtyBUK = key.QtyBUK,

            //    ROJob = key.ROJob,
            //    ROSample = key.ROSample,
            //    SatuanBUK = key.SatuanBUK,
            //    //SampleQty = group.Sum(x => x.SampleQty),
            //    SampleQty = key.SampleQty,
            //    UnitDOType = key.UnitDOType
            //    //ExpenditureType = key.ExpenditureType

            //});

            var Query2 = Query.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.ROJob).ThenBy(x => x.ROSample).ToList();

            //var roSample = string.Join(",", Query2.Select(x => x.ROSample).Distinct().ToList());
            //var ro = string.Join(",", Query2.Select(x => x.ROJob).Distinct().ToList());

            var UenItem = string.Join(",", queryExpend.Where(x => x.ExpenditureType == "PROSES").Select(x => x.UENItemId).Distinct().ToList());
            var UenItemSample = string.Join(",", queryExpend.Where(x => x.ExpenditureType == "SAMPLE").Select(x => x.UENItemId).Distinct().ToList());

            List<TraceableInDataBeacukaiViewModel> Data2 = new List<TraceableInDataBeacukaiViewModel>();
            List<BeacukaiAddedViewModel> PEBs = new List<BeacukaiAddedViewModel>();
            //List<GarmentExpenditureGoodViewModel> expendituregoods = new List<GarmentExpenditureGoodViewModel>();
            //List<GarmentSampleExpenditureGoodViewModel> expendituregoodsSample = new List<GarmentSampleExpenditureGoodViewModel>();

            //firstQuery
            //var garment = GetExpenditureGood(ro);
            //var garmentSample = GetSampleExpenditureGood(roSample);

            //if (garment != null)
            //{
            //    //expendituregoods = GetExpenditureGood(ro);
            //    var invoices = string.Join(",", garment.Select(x => x.Invoice).Distinct().ToList());
            //    var PEBS = GetPEB(invoices);

            //    foreach (var p in PEBS)
            //    {
            //        PEBs.Add(p);
            //    }
            //}

            //if (garmentSample != null)
            //{

            //    var invoicess = string.Join(",", garmentSample.Select(x => x.Invoice).Distinct().ToList());
            //    var PEBSS = GetPEB(invoicess);
            //    foreach (var p in PEBSS)
            //    {
            //        PEBs.Add(p);
            //    }
            //}

            //second Query
            var garment = GetFullGarment(UenItem);
            var garmentSample = GetFullGarmentSample(UenItemSample);
            if (UenItem.Count() > 0)
            {
                //expendituregoods = GetExpenditureGood(ro);
                var invoices = string.Join(",", garment.Select(x => x.Invoice).Distinct().ToList());
                var PEBS = GetPEB(invoices);

                foreach (var p in PEBS)
                {
                    PEBs.Add(p);
                }
            }

            if (UenItemSample.Count() > 0)
            {

                var invoicess = string.Join(",", garmentSample.Select(x => x.Invoice).Distinct().ToList());
                var PEBSS = GetPEB(invoicess);
                foreach (var p in PEBSS)
                {
                    PEBs.Add(p);
                }
            }

            //var Data1 = (from a in Query2
            //             join expend in expendituregoods on a.ROJob equals expend.RONo into expendgood
            //             from bb in expendgood.DefaultIfEmpty()
            //                 //join peb in PEBs on bb.Invoice equals peb.BonNo.Trim() into bcout
            //                 //from cc in bcout.DefaultIfEmpty()
            //                 where a.UnitDOType != "SAMPLE"
            //             select new TraceableInDataBeacukaiViewModel
            //             {
            //                 BCDate = a.BCDate,
            //                 BCNo = a.BCNo,
            //                 BCType = a.BCType,
            //                 BJQty = a.BJQty,
            //                 BonNo = a.BonNo,
            //                 SampleQtyOut = a.SampleQtyOut,
            //                 QtyBUK = a.QtyBUK,
            //                 ItemCode = a.ItemCode,
            //                 ItemName = a.ItemName,
            //                 ReceiptQty = a.ReceiptQty,
            //                 ROJob = a.ROJob,
            //                 SatuanBUK = a.SatuanBUK,
            //                 SampleQty = bb == null ? 0 : bb.ExpenditureType == "SAMPLE" ? bb.TotalQuantity : 0,
            //                 Invoice = bb == null ? "invo-" : bb.Invoice != null ? bb.Invoice : "invo-",
            //                 EksporQty = bb != null ? (bb.ExpenditureType =="EXPORT" ? bb.TotalQuantity : 0) : 0,
            //                 UnitDOType = a.UnitDOType
            //             }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo,  x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType,x.SampleQtyOut,x.EksporQty,x.ROJob }, (key, group) => new TraceableInDataBeacukaiViewModel
            //             {
            //                 BCDate = key.BCDate,
            //                 BCNo = key.BCNo,
            //                 BCType = key.BCType,
            //                 BJQty = key.BJQty,
            //                 BonNo = key.BonNo,
            //                 SampleQtyOut = key.SampleQtyOut,
            //                 QtyBUK = key.QtyBUK,
            //                 ItemCode = key.ItemCode,
            //                 ItemName = key.ItemName,
            //                 ReceiptQty = key.ReceiptQty,
            //                 ROJob = key.ROJob,
            //                 SatuanBUK = key.SatuanBUK,
            //                 SampleQty = group.Sum(x => x.SampleQty),
            //                 Invoice = key.Invoice,
            //                 EksporQty = key.EksporQty,
            //                 UnitDOType = key.UnitDOType
            //             })
            //             //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK).
            //             .ToList();

            //foreach (var x in Data1)
            //{
            //    Data2.Add(x);
            //}
            //First Query
            //var Data3 = (from a in Query2
            //             join expend in garmentSample on a.ROSample equals expend.RONo into expendgood
            //             from bb in expendgood.DefaultIfEmpty()
            //                 //join peb in PEBs on bb.Invoice equals peb.BonNo.Trim() into bcout
            //                 //from cc in bcout.DefaultIfEmpty()
            //             where a.UnitDOType == "SAMPLE"
            //             select new TraceableInDataBeacukaiViewModel
            //             {
            //                 BCDate = a.BCDate,
            //                 BCNo = a.BCNo,
            //                 BCType = a.BCType,
            //                 BJQty = a.BJQty,
            //                 BonNo = a.BonNo,
            //                 //BUK = a.BUK,
            //                 //SampleOut = a.SampleOut,
            //                 //BUM = a.BUM,
            //                 QtyBUK = a.QtyBUK,
            //                 SampleQtyOut = a.SampleQtyOut,
            //                 ItemCode = a.ItemCode,
            //                 ItemName = a.ItemName,
            //                 //PO = a.PO,
            //                 ReceiptQty = a.ReceiptQty,
            //                 //ROJob = a.ROJob,
            //                 ROSample = a.ROSample,
            //                 SatuanBUK = a.SatuanBUK,
            //                 //SatuanReceipt = a.SatuanReceipt,
            //                 SampleQty = bb == null ? 0 : bb.TotalQuantity,
            //                 Invoice = bb == null ? "invo-" : bb.Invoice != null ? bb.Invoice : "invo-",
            //                 //PEB = cc != null ? cc.BCNo : "peb-",
            //                 //PEBDate = cc != null ? cc.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
            //                 EksporQty = bb != null ? (bb.ExpenditureType == "EXPORT" ? bb.TotalQuantity : 0) : 0,
            //                 UnitDOType = a.UnitDOType
            //             }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType, x.SampleQtyOut, x.EksporQty, x.ROSample }, (key, group) => new TraceableInDataBeacukaiViewModel
            //             {
            //                 BCDate = key.BCDate,
            //                 BCNo = key.BCNo,
            //                 BCType = key.BCType,
            //                 BJQty = key.BJQty,
            //                 BonNo = key.BonNo,
            //                 //BUK = key.BUK,
            //                 //SampleOut = key.SampleOut,
            //                 //BUM = key.BUM,
            //                 QtyBUK = key.QtyBUK,
            //                 ItemCode = key.ItemCode,
            //                 ItemName = key.ItemName,
            //                 SampleQtyOut = key.SampleQtyOut,
            //                 //PO = key.PO,
            //                 ReceiptQty = key.ReceiptQty,
            //                 //ROJob = key.ROJob,
            //                 ROSample = key.ROSample,
            //                 SatuanBUK = key.SatuanBUK,
            //                 //SatuanReceipt = key.SatuanReceipt,
            //                 SampleQty = group.Sum(x => x.SampleQty),
            //                 Invoice = key.Invoice,
            //                 //PEB = cc != null ? cc.BCNo : "peb-",
            //                 //PEBDate = cc != null ? cc.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
            //                 //EksporQty = key.EksporQty
            //                 //EksporQty = group.Sum(x => x.EksporQty)
            //                 EksporQty = key.EksporQty,
            //                 UnitDOType = key.UnitDOType
            //             })
            //           //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK)
            //           .ToList();

            //foreach (var x in Data3)
            //{
            //    Data2.Add(x);
            //}

            //var Data1 = (from a in Query2
            //             join expend in garment on a.ROJob equals expend.RONo into expendgood
            //             from bb in expendgood.DefaultIfEmpty()
            //                 //join peb in PEBs on bb.Invoice equals peb.BonNo.Trim() into bcout
            //                 //from cc in bcout.DefaultIfEmpty()
            //             where a.UnitDOType != "SAMPLE"
            //             select new TraceableInBeacukaiViewModelTemp
            //             {
            //                 BCDate = a.BCDate,
            //                 BCNo = a.BCNo,
            //                 BCType = a.BCType,
            //                 BJQty = a.BJQty,
            //                 BonNo = a.BonNo,
            //                 SampleQtyOut = a.SampleQtyOut,
            //                 QtyBUK = a.QtyBUK,
            //                 ItemCode = a.ItemCode,
            //                 ItemName = a.ItemName,
            //                 ReceiptQty = a.ReceiptQty,
            //                 ROJob = a.ROJob,
            //                 SatuanBUK = a.SatuanBUK,
            //                 UnitDOType = a.UnitDOType,

            //                 SampleQty = bb == null ? 0 : bb.ExpenditureType == "SAMPLE" ? bb.TotalQuantity : 0,
            //                 Invoice = bb == null ? "invo-" : bb.Invoice != null ? bb.Invoice : "invo-",
            //                 EksporQty = bb != null ? (bb.ExpenditureType == "EXPORT" ? bb.TotalQuantity : 0) : 0,

            //                 //BJQty = (finishingout == null ? 0 : finishingout.totalQty),
            //                 //WIP = (cutting != null ? cutting.TotalCuttingOutQuantity : 0) - (finishingout != null ? finishingout.totalQty : 0),
            //                 //BJQty = garment != null ? (bb.FinishingTo == "GUDANG JADI" ? bb.FinishingOutQuantity : 0) : 0,
            //                 //WIP = garment != null ? (bb.CutOutType == "SEWING" ? bb.CutOutQuantity : 0) - (bb.FinishingTo == "GUDANG JADI" ? bb.FinishingOutQuantity : 0):0

            //             }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType, x.SampleQtyOut, x.EksporQty, x.ROJob }, (key, group) => new TraceableInDataBeacukaiViewModel
            //             {
            //                 BCDate = key.BCDate,
            //                 BCNo = key.BCNo,
            //                 BCType = key.BCType,
            //                 BJQty = key.BJQty,
            //                 BonNo = key.BonNo,
            //                 SampleQtyOut = key.SampleQtyOut,
            //                 QtyBUK = key.QtyBUK,
            //                 ItemCode = key.ItemCode,
            //                 ItemName = key.ItemName,
            //                 ReceiptQty = key.ReceiptQty,
            //                 ROJob = key.ROJob,
            //                 SatuanBUK = key.SatuanBUK,
            //                 SampleQty = group.Sum(x => x.SampleQty),
            //                 Invoice = key.Invoice,
            //                 EksporQty = key.EksporQty,
            //                 UnitDOType = key.UnitDOType
            //             })
            //             //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK).
            //             .ToList();

            //foreach (var x in Data1)
            //{
            //    Data2.Add(x);
            //}

            //Second Query
            var Data3 = (from a in Query2
                         join expend in garmentSample on a.ROSample equals expend.RoJob into expendgood
                         from bb in expendgood.DefaultIfEmpty()
                             //join peb in PEBs on bb.Invoice equals peb.BonNo.Trim() into bcout
                             //from cc in bcout.DefaultIfEmpty()
                         where a.UnitDOType == "SAMPLE"
                         select new TraceableInDataBeacukaiViewModel
                         {
                             BCDate = a.BCDate,
                             BCNo = a.BCNo,
                             BCType = a.BCType,
                             BJQty = a.BJQty,
                             BonNo = a.BonNo,
                             //BUK = a.BUK,
                             //SampleOut = a.SampleOut,
                             //BUM = a.BUM,
                             QtyBUK = a.QtyBUK,
                             SampleQtyOut = a.SampleQtyOut,
                             ItemCode = a.ItemCode,
                             ItemName = a.ItemName,
                             //PO = a.PO,
                             ReceiptQty = a.ReceiptQty,
                             //ROJob = a.ROJob,
                             ROSample = a.ROSample,
                             SatuanBUK = a.SatuanBUK,
                             //SatuanReceipt = a.SatuanReceipt,
                             SampleQty = bb == null ? 0 : bb.ExpenditureQuantity,
                             Invoice = bb == null ? "invo-" : bb.Invoice != null ? bb.Invoice : "invo-",
                             //PEB = cc != null ? cc.BCNo : "peb-",
                             //PEBDate = cc != null ? cc.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
                             EksporQty = bb != null ? (bb.ExpenditureType == "EXPORT" ? bb.ExpenditureQuantity : 0) : 0,
                             UnitDOType = a.UnitDOType
                         }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType, x.SampleQtyOut, x.EksporQty, x.ROSample }, (key, group) => new TraceableInDataBeacukaiViewModel
                         {
                             BCDate = key.BCDate,
                             BCNo = key.BCNo,
                             BCType = key.BCType,
                             BJQty = key.BJQty,
                             BonNo = key.BonNo,
                             //BUK = key.BUK,
                             //SampleOut = key.SampleOut,
                             //BUM = key.BUM,
                             QtyBUK = key.QtyBUK,
                             ItemCode = key.ItemCode,
                             ItemName = key.ItemName,
                             SampleQtyOut = key.SampleQtyOut,
                             //PO = key.PO,
                             ReceiptQty = key.ReceiptQty,
                             //ROJob = key.ROJob,
                             ROSample = key.ROSample,
                             SatuanBUK = key.SatuanBUK,
                             //SatuanReceipt = key.SatuanReceipt,
                             SampleQty = group.Sum(x => x.SampleQty),
                             Invoice = key.Invoice,
                             //PEB = cc != null ? cc.BCNo : "peb-",
                             //PEBDate = cc != null ? cc.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
                             //EksporQty = key.EksporQty
                             //EksporQty = group.Sum(x => x.EksporQty)
                             EksporQty = key.EksporQty,
                             UnitDOType = key.UnitDOType
                         })
                         //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK)
                         .ToList();

            foreach (var x in Data3)
            {
                Data2.Add(x);
            }

            var Data1 = (from a in Query2
                         join expend in garment on a.ROJob equals expend.RoJob into expendgood
                         from bb in expendgood.DefaultIfEmpty()
                             //join peb in PEBs on bb.Invoice equals peb.BonNo.Trim() into bcout
                             //from cc in bcout.DefaultIfEmpty()
                         where a.UnitDOType != "SAMPLE"
                         select new TraceableInBeacukaiViewModelTemp
                         {
                             BCDate = a.BCDate,
                             BCNo = a.BCNo,
                             BCType = a.BCType,
                             BJQty = a.BJQty,
                             BonNo = a.BonNo,
                             SampleQtyOut = a.SampleQtyOut,
                             QtyBUK = a.QtyBUK,
                             ItemCode = a.ItemCode,
                             ItemName = a.ItemName,
                             ReceiptQty = a.ReceiptQty,
                             ROJob = a.ROJob,
                             SatuanBUK = a.SatuanBUK,
                             UnitDOType = a.UnitDOType,

                             SampleQty = bb == null ? 0 : bb.ExpenditureType == "SAMPLE" ? bb.ExpenditureQuantity : 0,
                             Invoice = bb == null ? "invo-" : bb.Invoice != null ? bb.Invoice : "invo-",
                             EksporQty = bb != null ? (bb.ExpenditureType == "EXPORT" ? bb.ExpenditureQuantity : 0) : 0,

                             //BJQty = (finishingout == null ? 0 : finishingout.totalQty),
                             //WIP = (cutting != null ? cutting.TotalCuttingOutQuantity : 0) - (finishingout != null ? finishingout.totalQty : 0),
                             //BJQty = garment != null ? (bb.FinishingTo == "GUDANG JADI" ? bb.FinishingOutQuantity : 0) : 0,
                             //WIP = garment != null ? (bb.CutOutType == "SEWING" ? bb.CutOutQuantity : 0) - (bb.FinishingTo == "GUDANG JADI" ? bb.FinishingOutQuantity : 0):0

                         }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType, x.SampleQtyOut, x.EksporQty, x.ROJob }, (key, group) => new TraceableInDataBeacukaiViewModel
                         {
                             BCDate = key.BCDate,
                             BCNo = key.BCNo,
                             BCType = key.BCType,
                             BJQty = key.BJQty,
                             BonNo = key.BonNo,
                             SampleQtyOut = key.SampleQtyOut,
                             QtyBUK = key.QtyBUK,
                             ItemCode = key.ItemCode,
                             ItemName = key.ItemName,
                             ReceiptQty = key.ReceiptQty,
                             ROJob = key.ROJob,
                             SatuanBUK = key.SatuanBUK,
                             SampleQty = group.Sum(x => x.SampleQty),
                             Invoice = key.Invoice,
                             EksporQty = key.EksporQty,
                             UnitDOType = key.UnitDOType
                         })
                         //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK).
                         .ToList();

            foreach (var x in Data1)
            {
                Data2.Add(x);
            }

            var data22 = Data2.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.Invoice, x.UnitDOType, x.SampleQtyOut, x.ROSample, x.ROJob }, (key, group) => new TraceableInBeacukaiViewModelTemp
            {
                BCDate = key.BCDate,
                BCNo = key.BCNo,
                BCType = key.BCType,
                BJQty = key.BJQty,
                BonNo = key.BonNo,
                //BUK = key.BUK,
                //SampleOut = key.SampleOut,
                //BUM = key.BUM,
                QtyBUK = key.QtyBUK,
                SampleQtyOut = key.SampleQtyOut,
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                //PO = key.PO,
                ReceiptQty = key.ReceiptQty,
                ROJob = key.ROJob,
                ROSample = key.ROSample,
                SatuanBUK = key.SatuanBUK,
                //SatuanReceipt = key.SatuanReceipt,
                SampleQty = key.SampleQty,
                Invoice = key.Invoice,
                EksporQty = group.Sum(x => x.EksporQty),
                UnitDOType = key.UnitDOType
            }).ToList();
            //var DataTrace = Data2.Select((a, coba) => new
            //{
            //    rown = coba + 1,
            //    BCDate = a.BCDate,
            //    BCNo = a.BCNo,
            //    BCType = a.BCType,
            //    BJQty = a.BJQty,
            //    BonNo = a.BonNo,
            //    BUK = a.BUK,
            //    BUM = a.BUM,
            //    EksporQty = a.EksporQty,
            //    QtyBUK = a.QtyBUK,
            //    ItemCode = a.ItemCode,
            //    ItemName = a.ItemName,
            //    PO = a.PO,
            //    ReceiptQty = a.ReceiptQty,
            //    ROJob = a.ROJob,
            //    SatuanBUK = a.SatuanBUK,
            //    SatuanReceipt = a.SatuanReceipt

            //    //SampleQty = bb == null ? 0 : bb.ExpenditureType == "SAMPLE" ? bb.TotalQuantity : 0,
            //    //Invoice = bb == null ? "" : bb.Invoice,

            //}).ToList();

            List<TraceableInBeacukaiViewModelTemp> traceableIn1 = new List<TraceableInBeacukaiViewModelTemp>();

            //traceableIn1 = data22;
            List<TraceableInBeacukaiViewModel> traceableIn2 = new List<TraceableInBeacukaiViewModel>();

            var DistinctMasukperPO = Query.Select(x => new { x.ReceiptQty, x.BonNo, x.ItemCode }).GroupBy(x => new { x.BonNo, x.ReceiptQty, x.ItemCode }, (key, group) => new {
                key.BonNo,
                key.ItemCode,
                key.ReceiptQty
            }).ToList();

            //var DistinctKeluarperPO = Query.Select(x => new { x.QtyBUK, x.PO }).GroupBy(x => new { x.PO, x.QtyBUK }, (key, group) => new
            //{
            //    key.PO,
            //    key.QtyBUK

            //}).ToList();

            var groupMasukperPO = DistinctMasukperPO.GroupBy(x => new { x.BonNo, x.ItemCode }, (key, group) => new
            {
                ItemCode = key.ItemCode,
                BonNo = key.BonNo,
                ReceiptQty = group.Sum(x => x.ReceiptQty)
            }).ToList();

            var groupKeluarperPO = Query.GroupBy(x => new { x.BonNo, x.ItemCode }, (key, group) => new
            {
                ItemCode = key.ItemCode,
                BonNo = key.BonNo,
                QtyBUK = group.Sum(x => x.QtyBUK),
                SampleQtyOut = group.Sum(x => x.SampleQtyOut)
            }).ToList();

            var groupKeluarperExport1 = data22.Select(x => new { x.BonNo, x.ItemCode, x.EksporQty, x.SampleQty }).GroupBy(x => new { x.BonNo, x.ItemCode, x.EksporQty, x.SampleQty }, (key, group) => new
            {
                //SampleOut = key.SampleOut,
                key.BonNo,
                key.ItemCode,
                key.EksporQty,
                key.SampleQty
            }).ToList();

            var groupKeluarperExport = groupKeluarperExport1.GroupBy(x => new { x.BonNo, x.ItemCode }, (key, group) => new
            {
                //SampleOut = key.SampleOut,
                BonNo = key.BonNo,
                ItemCode = key.ItemCode,
                EksporQty = group.Sum(x => x.EksporQty),
                SampleQty = group.Sum(x => x.SampleQty)
            }).ToList();

            //var ListQtyMasukPerPO = DistinctMasukperPO.GroupBy(x => new { x.PO, x.ReceiptQty }, (key, group) => new
            //{
            //    PO = key.PO,
            //    ReceiptQty = group.Sum(x=>x.ReceiptQty)
            //});

            //var ListQtyKeluarperPO = DistinctKeluarperPO.GroupBy(x => new { x.PO }, (key, group) => new
            //{
            //    PO = key.PO,
            //    QtyBUK = group.Sum(x=>x.QtyBUK)
            //});

            //var Sisa = (from aa in ListQtyMasukPerPO
            //            join bb in ListQtyKeluarperPO on aa.PO equals bb.PO
            //            select new
            //            {
            //                PO = aa.PO,
            //                Sisa = aa.ReceiptQty - bb.QtyBUK
            //            }).ToList();

            //First Query
            //var cuttingOut = GetCuttingOut(ro);
            //var finouts = GetFinishingOut(ro);

            //var samplecuttingOut = GetSampleCuttingOut(roSample);
            //var samplefinouts = GetSampleFinishingOut(roSample);

            //foreach (var i in data22)
            //{
            //    if (i.UnitDOType == "SAMPLE")
            //    {
            //        //var samplecuttingIn = GetSampleCuttingOut(roSample);
            //        ////var finouts = GetFinishingOut(ro);
            //        //var samplefinouts = GetSampleFinishingOut(roSample);

            //        var subconout = samplefinouts.FirstOrDefault(x => x.roJob == i.ROSample && x.finishingInType == "PEMBELIAN");
            //        var finishingout = samplefinouts.FirstOrDefault(x => x.roJob == i.ROSample && x.finishingTo == "GUDANG JADI");
            //        //double nol = 0;
            //        //finishingout = finishingout == null ? finishingout.totalQty = Convert.ToDouble(nol) : finishingout;

            //        var cutting = samplecuttingOut.FirstOrDefault(x => x.RONo == i.ROSample && x.CuttingOutType == "SEWING");

            //        var sisa1 = groupMasukperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
            //        var sisa2 = groupKeluarperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
            //        //var PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);

            //        var eksQty = groupKeluarperExport.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
            //        //var PEBQty = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
            //        var wip = (cutting != null ? cutting.TotalCuttingOutQuantity : 0) - (finishingout != null ? finishingout.totalQty : 0);

            //        var trace1 = new TraceableInBeacukaiViewModelTemp
            //        {
            //            BCDate = i.BCDate,
            //            BCNo = i.BCNo,
            //            BCType = i.BCType,

            //            BonNo = i.BonNo,
            //            //BUK = i.BUK,
            //            //SampleOut = i.SampleOut,
            //            SampleQtyOut = sisa2.SampleQtyOut,
            //            //BUM = i.BUM,
            //            Invoice = i.Invoice,
            //            ItemCode = i.ItemCode,
            //            ItemName = i.ItemName,
            //            //PEB = PEB != null ? PEB.BCNo : "peb-",
            //            //PEBDate = PEB != null ? PEB.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
            //            //PO = i.PO,
            //            QtyBUK = sisa2.QtyBUK,
            //            ReceiptQty = sisa1.ReceiptQty,
            //            //ROJob = i.ROJob,
            //            //ROSample = i.ROSample,
            //            SampleQty = eksQty.SampleQty,
            //            SatuanBUK = i.SatuanBUK,
            //            SatuanReceipt = i.SatuanReceipt,
            //            Sisa = Math.Round(sisa1.ReceiptQty - sisa2.SampleQtyOut - sisa2.QtyBUK, 2),
            //            SubkonOutQty = 0,
            //            BJQty = (finishingout == null ? 0 : finishingout.totalQty),
            //            WIP = wip >= 0 ? wip : 0,

            //            EksporQty = eksQty.EksporQty
            //        };


            //        traceableIn1.Add(trace1);

            //    }
            //    else
            //    {
            //        //var cuttingIn = GetCuttingOut(ro);
            //        //var finouts = GetFinishingOut(ro);
            //        var subconout = finouts.FirstOrDefault(x => x.roJob == i.ROJob && x.finishingInType == "PEMBELIAN");
            //        var finishingout = finouts.FirstOrDefault(x => x.roJob == i.ROJob && x.finishingTo == "GUDANG JADI");
            //        //double nol = 0;
            //        //finishingout = finishingout == null ? finishingout.totalQty = Convert.ToDouble(nol) : finishingout;


            //        var cutting = cuttingOut.FirstOrDefault(x => x.RONo == i.ROJob && x.CuttingOutType == "SEWING");

            //        var sisa1 = groupMasukperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
            //        var sisa2 = groupKeluarperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
            //        //var PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
            //        //var PEBQty = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
            //        var eksQty = groupKeluarperExport.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);

            //        var wip = (cutting != null ? cutting.TotalCuttingOutQuantity : 0) - (finishingout != null ? finishingout.totalQty : 0);

            //        var trace1 = new TraceableInBeacukaiViewModelTemp
            //        {
            //            BCDate = i.BCDate,
            //            BCNo = i.BCNo,
            //            BCType = i.BCType,
            //            BJQty = (finishingout == null ? 0 : finishingout.totalQty) + (subconout == null ? 0 : subconout.totalQty),
            //            BonNo = i.BonNo,
            //            //BUK = i.BUK,
            //            //SampleOut = i.SampleOut,
            //            //BUM = i.BUM,
            //            Invoice = i.Invoice,
            //            ItemCode = i.ItemCode,
            //            ItemName = i.ItemName,
            //            //PEB = PEB != null ? PEB.BCNo : "peb-",
            //            //PEBDate = PEB != null ? PEB.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
            //            //PO = i.PO,
            //            QtyBUK = sisa2.QtyBUK,
            //            SampleQtyOut = sisa2.SampleQtyOut,
            //            ReceiptQty = sisa1.ReceiptQty,
            //            //ROJob = i.ROJob,
            //            //ROSample = i.ROSample,
            //            SampleQty = eksQty.SampleQty,
            //            SatuanBUK = i.SatuanBUK,
            //            SatuanReceipt = i.SatuanReceipt,
            //            Sisa = Math.Round(sisa1.ReceiptQty - sisa2.SampleQtyOut - sisa2.QtyBUK, 2),
            //            SubkonOutQty = subconout == null ? 0 : subconout.totalQty,
            //            //ProduksiQty = ((cutting != null && finishingout != null) ? cutting.TotalCuttingOutQuantity - finishingout.totalQty : 0),
            //            EksporQty = eksQty.EksporQty,

            //            WIP = wip >= 0 ? wip : 0,

            //        };

            //        traceableIn1.Add(trace1);
            //    }
            //}

            //Second Query
            foreach (var i in data22)
            {
                if (i.UnitDOType == "SAMPLE")
                {
                    //var samplecuttingIn = GetSampleCuttingOut(roSample);
                    ////var finouts = GetFinishingOut(ro);
                    //var samplefinouts = GetSampleFinishingOut(roSample);

                    var subconout = garmentSample.FirstOrDefault(x => x.RoJob == i.ROSample && x.FinishingInType == "PEMBELIAN");
                    var finishingout = garmentSample.FirstOrDefault(x => x.RoJob == i.ROSample && x.FinishingTo == "GUDANG JADI");
                    //double nol = 0;
                    //finishingout = finishingout == null ? finishingout.totalQty = Convert.ToDouble(nol) : finishingout;

                    var cutting = garmentSample.FirstOrDefault(x => x.RoJob == i.ROSample && x.CutOutType == "SEWING");

                    var sisa1 = groupMasukperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                    var sisa2 = groupKeluarperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                    //var PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);

                    var eksQty = groupKeluarperExport.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                    //var PEBQty = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);


                    var trace1 = new TraceableInBeacukaiViewModelTemp
                    {
                        BCDate = i.BCDate,
                        BCNo = i.BCNo,
                        BCType = i.BCType,

                        BonNo = i.BonNo,
                        //BUK = i.BUK,
                        //SampleOut = i.SampleOut,
                        SampleQtyOut = sisa2.SampleQtyOut,
                        //BUM = i.BUM,
                        Invoice = i.Invoice,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        //PEB = PEB != null ? PEB.BCNo : "peb-",
                        //PEBDate = PEB != null ? PEB.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
                        //PO = i.PO,
                        QtyBUK = sisa2.QtyBUK,
                        ReceiptQty = sisa1.ReceiptQty,
                        //ROJob = i.ROJob,
                        //ROSample = i.ROSample,
                        SampleQty = eksQty.SampleQty,
                        SatuanBUK = i.SatuanBUK,
                        SatuanReceipt = i.SatuanReceipt,
                        Sisa = Math.Round(sisa1.ReceiptQty - sisa2.SampleQtyOut - sisa2.QtyBUK, 2),
                        SubkonOutQty = 0,
                        BJQty = (finishingout == null ? 0 : finishingout.FinishingOutQuantity),
                        WIP = (cutting != null ? cutting.CutOutQuantity : 0) - (finishingout != null ? finishingout.FinishingOutQuantity : 0),

                        EksporQty = eksQty.EksporQty
                    };


                    traceableIn1.Add(trace1);

                }
                else
                {
                    //var cuttingIn = GetCuttingOut(ro);
                    //var finouts = GetFinishingOut(ro);
                    var subconout = garment.FirstOrDefault(x => x.RoJob == i.ROJob && x.FinishingInType == "PEMBELIAN");
                    var finishingout = garment.FirstOrDefault(x => x.RoJob == i.ROJob && x.FinishingTo == "GUDANG JADI");
                    //double nol = 0;
                    //finishingout = finishingout == null ? finishingout.totalQty = Convert.ToDouble(nol) : finishingout;


                    var cutting = garment.FirstOrDefault(x => x.RoJob == i.ROJob && x.CutOutType == "SEWING");

                    var sisa1 = groupMasukperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                    var sisa2 = groupKeluarperPO.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                    //var PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
                    //var PEBQty = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
                    var eksQty = groupKeluarperExport.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);


                    var trace1 = new TraceableInBeacukaiViewModelTemp
                    {
                        BCDate = i.BCDate,
                        BCNo = i.BCNo,
                        BCType = i.BCType,
                        BJQty = (finishingout == null ? 0 : finishingout.FinishingOutQuantity) + (subconout == null ? 0 : subconout.FinishingOutQuantity),
                        BonNo = i.BonNo,
                        //BUK = i.BUK,
                        //SampleOut = i.SampleOut,
                        //BUM = i.BUM,
                        Invoice = i.Invoice,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        //PEB = PEB != null ? PEB.BCNo : "peb-",
                        //PEBDate = PEB != null ? PEB.BCDate : new DateTimeOffset(new DateTime(1970, 1, 1)),
                        //PO = i.PO,
                        QtyBUK = sisa2.QtyBUK,
                        SampleQtyOut = sisa2.SampleQtyOut,
                        ReceiptQty = sisa1.ReceiptQty,
                        //ROJob = i.ROJob,
                        //ROSample = i.ROSample,
                        SampleQty = eksQty.SampleQty,
                        SatuanBUK = i.SatuanBUK,
                        SatuanReceipt = i.SatuanReceipt,
                        Sisa = Math.Round(sisa1.ReceiptQty - sisa2.SampleQtyOut - sisa2.QtyBUK, 2),
                        SubkonOutQty = subconout == null ? 0 : subconout.FinishingOutQuantity,
                        //ProduksiQty = ((cutting != null && finishingout != null) ? cutting.TotalCuttingOutQuantity - finishingout.totalQty : 0),
                        EksporQty = eksQty.EksporQty,

                        WIP = (cutting != null ? cutting.CutOutQuantity : 0) - (finishingout != null ? finishingout.FinishingOutQuantity : 0),

                    };

                    traceableIn1.Add(trace1);
                }
            }

            var groupBJQty = traceableIn1.Select(x => new { x.BJQty, x.BonNo, x.ItemCode, x.WIP }).GroupBy(x => new { x.BonNo, x.BJQty, x.ItemCode, x.WIP }, (key, group) => new
            {
                key.ItemCode,
                key.BonNo,
                key.WIP,
                key.BJQty
            }).ToList();

            var sumBJQty = groupBJQty.GroupBy(x => new { x.BonNo, x.ItemCode }, (key, group) => new
            {
                ItemCode = key.ItemCode,
                BonNo = key.BonNo,
                BJQty = group.Sum(x => x.BJQty),
                WIP = group.Sum(x => x.WIP)
            }).ToList();

            foreach (var i in traceableIn1)
            {
                var PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == i.Invoice);
                var resulBJ = sumBJQty.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);
                //var resulBJ = sumBJQty.FirstOrDefault(x => x.BonNo == i.BonNo && x.ItemCode == i.ItemCode);

                i.WIP = resulBJ.WIP;
                i.BJQty = resulBJ.BJQty;
                i.PEB = PEB != null ? PEB.BCNo : "peb-";
            };
            //traceableIn1 = traceableIn1.OrderBy(x => x.ROJob).ThenBy(x => x.Invoice).ToList();


            //traceableIn1 = traceableIn1.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode)
            //    .ThenBy(x => x.ItemName).ThenBy(x => x.BUM).ThenBy(x => x.ReceiptQty).ThenBy(x => x.SatuanReceipt).ThenBy(x => x.BUK).ThenBy(x => x.QtyBUK).ThenBy(x => x.Sisa).ThenBy(x => x.SatuanBUK).ThenBy(x => x.ProduksiQty)
            //    .ThenBy(x => x.BJQty).ThenBy(x => x.Invoice).ToList();





            var traceableIn11 = traceableIn1.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.QtyBUK, x.ItemCode, x.ItemName, x.ReceiptQty, x.SatuanBUK, x.SampleQty, x.EksporQty, x.PEB, x.Sisa, x.SampleQtyOut, x.WIP }, (key, group) => new TraceableInBeacukaiViewModelTemp
            {
                BCDate = key.BCDate,
                BCNo = key.BCNo,
                BCType = key.BCType,
                BJQty = key.BJQty,
                BonNo = key.BonNo,
                //BUK = key.BUK,
                //SampleOut = key.SampleOut,
                //BUM = key.BUM,
                QtyBUK = key.QtyBUK,
                SampleQtyOut = key.SampleQtyOut,
                ItemCode = key.ItemCode,
                ItemName = key.ItemName,
                //PO = key.PO,
                ReceiptQty = key.ReceiptQty,
                //ROJob = key.ROJob,
                //ROSample = key.ROSample,
                SatuanBUK = key.SatuanBUK,
                //SatuanReceipt = key.SatuanReceipt,
                SampleQty = key.SampleQty,
                //Invoice = key.Invoice,
                EksporQty = key.EksporQty,
                PEB = key.PEB,
                //PEBDate = key.PEBDate,
                Sisa = key.Sisa,
                WIP = key.WIP
                //SubkonOutQty = key.SubkonOutQty,
                //ProduksiQty = key.ProduksiQty
            });

            var trace21 = traceableIn11.Select((i, order) => new
            {
                rown = order + 1,
                BCDate = i.BCDate,
                BCNo = i.BCNo,
                BCType = i.BCType,
                BJQty = i.BJQty,
                BonNo = i.BonNo,
                //BUK = i.BUK,
                //SampleOut = i.SampleOut,
                //BUM = i.BUM,
                //Invoice = i.Invoice,
                ItemCode = i.ItemCode,
                ItemName = i.ItemName,
                PEB = i.PEB,
                //PEBDate = new DateTimeOffset(new DateTime(1970, 1, 1)),
                PO = i.PO,
                //ProduksiQty = cutting == null ? 0 : cutting.Sum(x => x.TotalCuttingInQuantity),
                EksporQty = i.EksporQty,
                QtyBUK = i.QtyBUK,
                SampleQtyOut = i.SampleQtyOut,
                ReceiptQty = i.ReceiptQty,
                //ROJob = i.ROJob,
                //ROSample = i.ROSample,
                SampleQty = i.SampleQty,
                SatuanBUK = i.SatuanBUK,
                //SatuanReceipt = i.SatuanReceipt,
                Sisa = i.Sisa,
                WIP = i.WIP
                //SubkonOutQty = i.SubkonOutQty,
                //ProduksiQty = i.ProduksiQty
            });

            var expendorder = data22.OrderBy(x => x.PEB).Select((a, order) => new
            {
                rown = order + 1,
                Invoice = a.Invoice,
                PEB = PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice) == null ? "peb-" : PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice).BCNo,
                PEBDate = PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice) == null ? new DateTimeOffset(new DateTime(1970, 1, 1)) : PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice).BCDate,
                EksporQty = PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice) == null ? 0 : PEBs.FirstOrDefault(x => x.BonNo.Trim() == a.Invoice).Quantity
            }).ToList();

            var traceable = (from a in trace21
                             join r in expendorder on a.rown equals r.rown
                             select new TraceableInBeacukaiViewModelTemp
                             {
                                 BCDate = a.BCDate,
                                 BCNo = a.BCNo,
                                 BCType = a.BCType,
                                 BJQty = a.BJQty,
                                 BonNo = a.BonNo,
                                 //BUK = a.BUK,
                                 //SampleOut = a.SampleOut,
                                 //BUM = a.BUM,
                                 count = 0,
                                 EksporQty = a.EksporQty,
                                 //ExType = i.ExType,
                                 //Invoice = r.Invoice,
                                 ItemCode = a.ItemCode,
                                 ItemName = a.ItemName,
                                 PEB = a.PEB,
                                 //PEBDate = r.PEBDate,
                                 //PO = a.PO,
                                 //ProduksiQty = a.ProduksiQty,
                                 QtyBUK = a.QtyBUK,
                                 SampleQtyOut = a.SampleQtyOut,
                                 ReceiptQty = a.ReceiptQty,
                                 //ROJob = a.ROJob,
                                 //ROSample = a.ROSample,
                                 SampleQty = a.SampleQty,
                                 SatuanBUK = a.SatuanBUK,
                                 //SatuanReceipt = a.SatuanReceipt,
                                 Sisa = a.Sisa,
                                 WIP = a.WIP
                                 //SubkonOutQty = a.SubkonOutQty
                             }).
                             //.OrderBy(x => x.BCType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate).ThenBy(x => x.BonNo).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.ItemCode).ThenBy(x => x.ItemName).ThenBy(x => x.ReceiptQty).ThenBy(x => x.BUK).ThenBy(x=>x.BJQty).ThenBy(x => x.EksporQty).ThenBy(x => x.BJQty).ThenBy(x => x.Invoice);
                             GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.BJQty, x.BonNo, x.count, x.EksporQty, x.ItemCode, x.ItemName, x.PEB, x.QtyBUK, x.SampleQtyOut, x.ReceiptQty, x.SampleQty, x.SatuanBUK, x.Sisa, x.WIP }, (key, group) => new TraceableInBeacukaiViewModelTemp
                             {
                                 BCDate = key.BCDate,
                                 BCNo = key.BCNo,
                                 BCType = key.BCType,
                                 BJQty = key.BJQty,
                                 BonNo = key.BonNo,
                                 count = key.count,
                                 EksporQty = key.EksporQty,
                                 ItemCode = key.ItemCode,
                                 ItemName = key.ItemName,
                                 PEB = key.PEB,
                                 QtyBUK = key.QtyBUK,
                                 SampleQtyOut = key.SampleQtyOut,
                                 ReceiptQty = key.ReceiptQty,
                                 SampleQty = key.SampleQty,
                                 SatuanBUK = key.SatuanBUK,
                                 Sisa = key.Sisa,
                                 WIP = key.WIP
                             });


            //{
            //    BCDate =key.BCDate
            //});



            var traceableInBeacukaiViews = traceable.ToArray();
            var index = 0;
            foreach (TraceableInBeacukaiViewModelTemp a in traceableInBeacukaiViews)
            {
                TraceableInBeacukaiViewModelTemp dup = Array.Find(traceableInBeacukaiViews, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == a.BonNo);
                if (dup != null)
                {
                    if (dup.count == 0)
                    {
                        index++;
                        dup.count = index;
                    }
                }
                a.count = dup.count;
            }

            foreach (var i in traceableInBeacukaiViews.ToList())
            {
                var trace1 = new TraceableInBeacukaiViewModel
                {
                    BCDate = i.BCDate,
                    BCNo = i.BCNo,
                    BCType = i.BCType,
                    BJQty = i.BJQty == 0 ? "BJQty0" : i.BJQty.ToString(),
                    BonNo = i.BonNo,
                    //BUK = i.BUK == null ? "buk-" : i.BUK.ToString(),
                    SampleOut = i.SampleOut == "" ? "sampout-" : i.SampleOut,
                    //BUM = i.BUM,
                    //Invoice = i.Invoice,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    PEB = i.PEB,
                    //PEBDate = i.PEBDate,
                    //PO = i.PO,
                    QtyBUK = i.QtyBUK == 0 ? "QtyBUK0" : i.QtyBUK.ToString(),
                    SampleQtyOut = i.SampleQtyOut.ToString(),
                    ReceiptQty = i.ReceiptQty.ToString(),
                    //ROJob = i.ROJob == null ? "rojob-":i.ROJob,
                    //ROSample = i.ROJob == null ? "rosamp-" : i.ROSample,
                    SampleQty = i.SampleQty == 0 ? "SampleQty0" : i.SampleQty.ToString(),
                    SatuanBUK = i.SatuanBUK,
                    //SatuanReceipt = i.SatuanReceipt,
                    Sisa = i.Sisa == 0 ? "Sisa0" : i.Sisa.ToString(),
                    //SubkonOutQty = i.SubkonOutQty == 0 ? "SubkonOutQty0" : i.SubkonOutQty.ToString(),
                    //ProduksiQty = i.ProduksiQty == 0 ? "ProduksiQty0" : i.ProduksiQty.ToString(),
                    count = i.count,
                    EksporQty = i.EksporQty == 0 ? "EksporQty0" : i.EksporQty.ToString(),
                    ExType = "",
                    WIP = i.WIP.ToString()

                };

                traceableIn2.Add(trace1);
            }

            //traceableIn2.GroupBy(x => new { x.BCDate, x.BCNo, x.BCType,, x.BonNo, x.BUK, x.BUM, x.QtyBUK, x.ItemCode, x.ItemName, x.PO, x.ReceiptQty, x.ROJob, x.SatuanBUK, x.SatuanReceipt, x.SampleQty, x.Invoice, x.SampleOut, x.UnitDOType }, (key, group) => new TraceableInBeacukaiViewModel


            return traceableIn2.Distinct().ToList();

        }

        public MemoryStream GetTraceableInExcel(string filter, string tipe, string tipebc)
        {
            var Query = GetTraceableInQuery(filter, tipe, tipebc);
            //Query.OrderBy(x => x.BCNo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });

            //result.Columns.Add(new DataColumn() { ColumnName = "PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Keluar Proses", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "ROJob", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Keluar Sample", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "ROSample", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Subcon", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar Proses", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar Sample", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sisa", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "WIP (PCS)", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "BJ Qty", DataType = typeof(Double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BC Keluar", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tgl BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ekspor Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sample Qty", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0,
                     0,0, "","", 0, 0, "", 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                //var docNo = Query.ToArray();
                //var q = Query.ToList();
                //var index = 0;
                //foreach (TraceableInBeacukaiViewModel a in q)
                //{
                //    TraceableInBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == o.BonNo);
                //    if (dup != null)
                //    {
                //        if (dup.count == 0)
                //        {
                //            index++;
                //            dup.count = index;
                //        }
                //    }
                //    a.count = dup.count;
                //}
                //Query = q;
                //var index = 0;
                foreach (var item in Query)
                {
                    string bcdate = item.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : item.BCDate.DateTime.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string pebdate = item.PEBDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : item.PEBDate.Value.DateTime.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string rojob = item.ROJob == "rojob-" ? "-" : item.ROJob;
                    //string rosamp = item.ROSample == "rosamp-" ? "-" : item.ROSample;
                    //string sampleOut = item.SampleOut == "samout-" ? "-" : item.SampleOut;
                    //string buk = item.BUK == "buk-" ? "-" : item.BUK;
                    string satuanbuk = item.SatuanBUK == "satbuk-" ? "-" : item.SatuanBUK;
                    //string invoice = item.Invoice == "invo-" ? "-" : item.Invoice;
                    string PEB = item.PEB == "peb-" ? "-" : item.PEB;
                    double qtybuk = item.QtyBUK == "QtyBUK0" ? 0 : Convert.ToDouble(item.QtyBUK);
                    double sisa = item.Sisa == "Sisa0" ? 0 : Convert.ToDouble(item.Sisa);
                    //double subcon = item.Subcon ==  ? 0 : Convert.ToDouble(item.Sisa);
                    //double produksiQty = item.ProduksiQty == "ProduksiQty0" ? 0 : Convert.ToDouble(item.ProduksiQty);
                    double bJQty = item.BJQty == "BJQty0" ? 0 : Convert.ToDouble(item.BJQty);
                    double eksporQty = item.EksporQty == "EksporQty0" ? 0 : Convert.ToDouble(item.EksporQty);
                    double sampleQty = item.SampleQty == "SampleQty0" ? 0 : Convert.ToDouble(item.SampleQty);
                    result.Rows.Add(item.count, item.BCType, item.BCNo, bcdate, item.BonNo,
                        item.ItemCode, item.ItemName,
                        item.ReceiptQty,
                        qtybuk,item.SampleQtyOut, sisa, satuanbuk,item.WIP, bJQty
                        , PEB, eksporQty, sampleQty);

                }
            }
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                //sheet.Cells["C1:D1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //sheet.Cells["E1:F1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                Dictionary<string, int> bcnospan = new Dictionary<string, int>();
                Dictionary<string, int> bonspan = new Dictionary<string, int>();
                Dictionary<string, int> rojobspan = new Dictionary<string, int>();
                Dictionary<string, int> pospan = new Dictionary<string, int>();
                Dictionary<string, int> itemcodespan = new Dictionary<string, int>();
                Dictionary<string, int> itemnamespan = new Dictionary<string, int>();
                Dictionary<string, int> qtyreceiptspan = new Dictionary<string, int>();
                Dictionary<string, int> qtybukspan = new Dictionary<string, int>();
                Dictionary<string, int> qtysampleoutspan = new Dictionary<string, int>();
                Dictionary<string, int> satuanreceiptspan = new Dictionary<string, int>();
                Dictionary<string, int> nobukspan = new Dictionary<string, int>();
                Dictionary<string, int> sisaspan = new Dictionary<string, int>();
                Dictionary<string, int> satuanbukspan = new Dictionary<string, int>();
                Dictionary<string, int> produksiQtyspan = new Dictionary<string, int>();
                Dictionary<string, int> bjquantityspan = new Dictionary<string, int>();
                Dictionary<string, int> invoicespan = new Dictionary<string, int>();
                Dictionary<string, int> pebnospan = new Dictionary<string, int>();
                Dictionary<string, int> pebdatespan = new Dictionary<string, int>();
                Dictionary<string, int> eksporqtyspan = new Dictionary<string, int>();
                Dictionary<string, int> samppleqtyspan = new Dictionary<string, int>();
                Dictionary<string, int> nobuksampspan = new Dictionary<string, int>();
                Dictionary<string, int> rosampspan = new Dictionary<string, int>();
                Dictionary<string, int> wipspan = new Dictionary<string, int>();

                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (bcnospan.TryGetValue(a.BCType + a.BCNo + a.BCDate, out value))
                    {
                        bcnospan[a.BCType + a.BCNo + a.BCDate]++;
                    }
                    else
                    {
                        bcnospan[a.BCType + a.BCNo + a.BCDate] = 1;
                    }

                    ////FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                    //if (bonspan.TryGetValue(a.BonNo, out value))
                    //{
                    //    bonspan[a.BonNo]++;
                    //}
                    //else
                    //{
                    //    bonspan[a.BonNo] = 1;
                    //}

                    //if (rojobspan.TryGetValue(a.ROJob + a.ROSample, out value))
                    //{
                    //    rojobspan[a.ROJob + a.ROSample]++;
                    //}
                    //else
                    //{
                    //    rojobspan[a.ROJob + a.ROSample] = 1;
                    //}
                    if (itemcodespan.TryGetValue(a.ItemCode + a.BonNo, out value))
                    {
                        itemcodespan[a.ItemCode + a.BonNo]++;
                    }
                    else
                    {
                        itemcodespan[a.ItemCode + a.BonNo] = 1;
                    }
                    if (itemnamespan.TryGetValue(a.ItemName + a.BonNo, out value))
                    {
                        itemnamespan[a.ItemName + a.BonNo]++;
                    }
                    else
                    {
                        itemnamespan[a.ItemName + a.BonNo] = 1;
                    }

                    if (qtyreceiptspan.TryGetValue(a.ReceiptQty + "bum" + a.ItemCode, out value))
                    {
                        qtyreceiptspan[a.ReceiptQty + "bum" + a.ItemCode]++;
                    }
                    else
                    {
                        qtyreceiptspan[a.ReceiptQty + "bum" + a.ItemCode] = 1;
                    }
                    if (qtybukspan.TryGetValue(a.QtyBUK + "buk" + a.ItemCode, out value))
                    {
                        qtybukspan[a.QtyBUK + "buk" + a.ItemCode]++;
                    }
                    else
                    {
                        qtybukspan[a.QtyBUK + "buk" + a.ItemCode] = 1;
                    }
                    if (qtysampleoutspan.TryGetValue(a.SampleQtyOut + "samp" + a.ItemCode, out value))
                    {
                        qtysampleoutspan[a.SampleQtyOut + "samp" + a.ItemCode]++;
                    }
                    else
                    {
                        qtysampleoutspan[a.SampleQtyOut + "samp" + a.ItemCode] = 1;
                    }
                    //if (satuanreceiptspan.TryGetValue(a.SatuanReceipt + "uomreceipt", out value))
                    //{
                    //    satuanreceiptspan[a.SatuanReceipt + "uomreceipt"]++;
                    //}
                    //else
                    //{
                    //    satuanreceiptspan[a.SatuanReceipt + "uomreceipt"] = 1;
                    //}
                    //if (nobukspan.TryGetValue(a.BUK + a.QtyBUK +a.SampleOut + a.PO, out value))
                    //{
                    //    nobukspan[a.BUK + a.QtyBUK + a.SampleOut + a.PO]++;
                    //}
                    //else
                    //{
                    //    nobukspan[a.BUK + a.QtyBUK + a.SampleOut + a.PO] = 1;
                    //}
                    if (sisaspan.TryGetValue(a.Sisa.ToString() + "sisa" + a.ItemCode + a.ReceiptQty, out value))
                    {
                        sisaspan[a.Sisa.ToString() + "sisa" + a.ItemCode + a.ReceiptQty]++;
                    }
                    else
                    {
                        sisaspan[a.Sisa.ToString() + "sisa" + a.ItemCode + a.ReceiptQty] = 1;
                    }
                    if (satuanbukspan.TryGetValue(a.SatuanBUK + "uombuk", out value))
                    {
                        satuanbukspan[a.SatuanBUK + "uombuk"]++;
                    }
                    else
                    {
                        satuanbukspan[a.SatuanBUK + "uombuk"] = 1;
                    }
                    if (wipspan.TryGetValue(a.WIP + "wip" + a.ItemCode, out value))
                    {
                        wipspan[a.WIP + "wip" + a.ItemCode]++;
                    }
                    else
                    {
                        wipspan[a.WIP + "wip" + a.ItemCode] = 1;
                    }
                    //if (produksiQtyspan.TryGetValue(a.ProduksiQty.ToString() + "proqty" + a.ROJob, out value))
                    //{
                    //    produksiQtyspan[a.ProduksiQty.ToString() + "proqty" + a.ROJob]++;
                    //}
                    //else
                    //{
                    //    produksiQtyspan[a.ProduksiQty.ToString() + "proqty" + a.ROJob] = 1;
                    //}
                    if (bjquantityspan.TryGetValue(a.BJQty.ToString() + a.ItemCode, out value))
                    {
                        bjquantityspan[a.BJQty.ToString() + a.ItemCode]++;
                    }
                    else
                    {
                        bjquantityspan[a.BJQty.ToString() + a.ItemCode] = 1;
                    }
                    //if (invoicespan.TryGetValue(a.Invoice + a.ROJob + a.ROSample+a.BUK+a.SampleOut, out value))
                    //{
                    //    invoicespan[a.Invoice + a.ROJob + a.ROSample + a.BUK + a.SampleOut]++;
                    //}
                    //else
                    //{
                    //    invoicespan[a.Invoice + a.ROJob + a.ROSample + a.BUK + a.SampleOut] = 1;
                    //}
                    if (pebnospan.TryGetValue(a.PEB + a.ItemCode + a.BonNo, out value))
                    {
                        pebnospan[a.PEB + a.ItemCode + a.BonNo]++;
                    }
                    else
                    {
                        pebnospan[a.PEB + a.ItemCode + a.BonNo] = 1;
                    }

                    //if (pebdatespan.TryGetValue(a.PEBDate.ToString() + a.ROJob + a.Invoice, out value))
                    //{
                    //    pebdatespan[a.PEBDate.ToString() + a.ROJob + a.Invoice]++;
                    //}
                    //else
                    //{
                    //    pebdatespan[a.PEBDate.ToString() + a.ROJob + a.Invoice] = 1;
                    //}

                    if (eksporqtyspan.TryGetValue(a.EksporQty.ToString() + a.ItemCode + a.BJQty.ToString(), out value))
                    {
                        eksporqtyspan[a.EksporQty.ToString()  + a.ItemCode + a.BJQty.ToString()]++;
                    }
                    else
                    {
                        eksporqtyspan[a.EksporQty.ToString()  + a.ItemCode + a.BJQty.ToString()] = 1;
                    }

                    if (samppleqtyspan.TryGetValue(a.SampleQty.ToString() + a.ItemCode, out value))
                    {
                        samppleqtyspan[a.SampleQty.ToString()  + a.ItemCode]++;
                    }
                    else
                    {
                        samppleqtyspan[a.SampleQty.ToString() + a.ItemCode] = 1;
                    }

                    ////if (pospan.TryGetValue(a.PO + a.ROJob, out value))
                    ////{
                    ////    pospan[a.PO + a.ROJob]++;
                    ////}
                    ////else
                    ////{
                    ////    pospan[a.PO + a.ROJob] = 1;
                    ////}
                    //if (pospan.TryGetValue(a.PO , out value))
                    //{
                    //    pospan[a.PO ]++;
                    //}
                    //else
                    //{
                    //    pospan[a.PO ] = 1;
                    //}

                    //if (nobuksampspan.TryGetValue(a.SampleOut + a.QtyBUK, out value))
                    //{
                    //    nobuksampspan[a.SampleOut + a.QtyBUK]++;
                    //}
                    //else
                    //{
                    //    nobuksampspan[a.SampleOut + a.QtyBUK] = 1;
                    //}

                    //if (rosampspan.TryGetValue(a.ROSample + a.PO, out value))
                    //{
                    //    rosampspan[a.ROSample + a.PO]++;
                    //}
                    //else
                    //{
                    //    rosampspan[a.ROSample + a.PO] = 1;
                    //}

                }

                int index = 2;
                foreach (KeyValuePair<string, int> b in bcnospan)
                {
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += b.Value;
                }

                //index = 2;
                //foreach (KeyValuePair<string, int> c in bonspan)
                //{
                //    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //    index += c.Value;
                //}
                index = 2;
                foreach (KeyValuePair<string, int> c in itemcodespan)
                {
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                ////index = 2;
                ////foreach (KeyValuePair<string, int> c in pospan)
                ////{
                ////    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Merge = true;
                ////    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                ////    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                ////    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                ////    index += c.Value;
                ////}
                index = 2;
                foreach (KeyValuePair<string, int> c in itemnamespan)
                {
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in qtyreceiptspan)
                {
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in qtybukspan)
                {
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in qtysampleoutspan)
                {
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in sisaspan)
                {
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                    //sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in satuanbukspan)
                {
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in wipspan)
                {
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }


                index = 2;
                foreach (KeyValuePair<string, int> c in bjquantityspan)
                {
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                ////////index = 2;
                ////////foreach (KeyValuePair<string, int> c in satuanbukspan)
                ////////{
                ////////    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                ////////    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                ////////    index += c.Value;
                ////////}
                index = 2;
                foreach (KeyValuePair<string, int> c in pebnospan)
                {
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in eksporqtyspan)
                {
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in samppleqtyspan)
                {
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                //index = 2;
                //foreach (KeyValuePair<string, int> c in satuanreceiptspan)
                //{
                //    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in produksiQtyspan)
                //{
                //    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in bjquantityspan)
                //{
                //    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in invoicespan)
                //{
                //    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in invoicespan)
                //{
                //    sheet.Cells["V" + index + ":V" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["V" + index + ":V" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in invoicespan)
                //{
                //    sheet.Cells["W" + index + ":W" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["W" + index + ":W" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                //index = 2;
                //foreach (KeyValuePair<string, int> c in eksporqtyspan)
                //{
                //    sheet.Cells["X" + index + ":X" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["X" + index + ":X" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        #endregion
        #region Keluar
        public List<GarmentExpenditureGoodViewModel> GetRono(string invoice)
        {
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var param = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            string shippingInvoiceUri = APIEndpoint.GarmentProduction + $"expenditure-goods/byInvoice";

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, shippingInvoiceUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentExpenditureGoodViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentExpenditureGoodViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentExpenditureGoodViewModel>();
            }

        }

        public List<BeacukaiAddedViewModelbyBCNo> GetPEBbyBCNo(string bcno)
        {
            //var param = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            string shippingInvoiceUri = APIEndpoint.CustomsReport + $"customs-reports/getPEB/byBCNo?bcno={bcno}";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.GetAsync(shippingInvoiceUri).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<BeacukaiAddedViewModelbyBCNo> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<BeacukaiAddedViewModelbyBCNo>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<BeacukaiAddedViewModelbyBCNo>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<BeacukaiAddedViewModelbyBCNo>();
            }
        }

        public List<GarmentPreparingViewModel> GetPreparingByRo(string ro)
        {
            var param = new StringContent(JsonConvert.SerializeObject(ro), Encoding.UTF8, "application/json");
            string shippingInvoiceUri = APIEndpoint.GarmentProduction + $"preparings/byRONO?RO={ro}";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, shippingInvoiceUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentPreparingViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<GarmentPreparingViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<GarmentPreparingViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<GarmentPreparingViewModel>();
            }
        }

        public List<TraceableOutBeacukaiViewModel> getQueryTraceableOut(string bcno)
        {

            var PEB = GetPEBbyBCNo(bcno.Trim());

            //var filterexpend = new
            //{
            //    invoice = PEB.Select(x=>x.BonNo.Trim()).FirstOrDefault()
            //};

            string invoices = string.Join(",", PEB.Select(x => x.BonNo));

            var expend = GetRono(invoices);

            var Query = (from a in PEB
                         join b in expend on a.BonNo.Trim() equals b.Invoice.Trim()
                         select new TraceableOutBeacukaiViewModel
                         {
                             BCDate = a.BCDate,
                             BCNo = a.BCNo,
                             BCType = a.BCType,
                             BuyerName = b.Buyer.Name,
                             ComodityName = b.Comodity.Name,
                             ExpenditureDate = b.ExpenditureDate,
                             ExpenditureGoodId = b.ExpenditureGoodNo,
                             ExpenditureNo = b.Invoice,
                             Qty = b.TotalQuantity,
                             RO = b.RONo,
                             UnitQtyName = "PCS"

                         }).GroupBy(x => new { x.ExpenditureDate, x.ExpenditureGoodId, x.ComodityName, x.ExpenditureNo, x.BuyerName, x.BCType, x.BCNo, x.BCDate, x.RO, x.UnitQtyName }, (key, group) => new TraceableOutBeacukaiViewModel
                         {
                             BCDate = key.BCDate,
                             BCNo = key.BCNo,
                             BCType = key.BCType,
                             BuyerName = key.BuyerName,
                             ComodityName = key.ComodityName,
                             ExpenditureDate = key.ExpenditureDate,
                             ExpenditureGoodId = key.ExpenditureGoodId,
                             ExpenditureNo = key.ExpenditureNo,
                             Qty = group.Sum(x => x.Qty),
                             RO = key.RO,
                             UnitQtyName = key.UnitQtyName

                         }).ToList();

            var RO = string.Join(",", Query.Select(x => x.RO).Distinct().ToList());
            var listRo = Query.Select(x => x.RO).Distinct().ToList();

            var expend2 = GetPreparingByRo(RO);

            var rinciandetil = (from a in dbContext.GarmentUnitDeliveryOrderItems
                                join b in dbContext.GarmentUnitDeliveryOrders on a.UnitDOId equals b.Id
                                join c in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals c.UnitDOItemId
                                join d in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals d.Id
                                join i in dbContext.GarmentUnitReceiptNotes on d.URNId equals i.Id
                                join e in dbContext.GarmentDeliveryOrderDetails on d.DODetailId equals e.Id
                                join f in dbContext.GarmentDeliveryOrderItems on e.GarmentDOItemId equals f.Id
                                join g in dbContext.GarmentDeliveryOrders on f.GarmentDOId equals g.Id
                                join h in dbContext.GarmentBeacukais on g.CustomsId equals h.Id
                                where listRo.Contains(b.RONo)
                                && i.URNType == "PEMBELIAN"
                                select new TraceableOutBeacukaiDetailViewModel
                                {
                                    BCDate = h.BeacukaiDate.DateTime,
                                    BCNo = h.BeacukaiNo,
                                    BCType = h.CustomsType,
                                    DestinationJob = b.RONo,
                                    ItemCode = d.ProductCode,
                                    ItemName = d.ProductName,
                                    SmallestQuantity = c.Quantity,
                                    UnitQtyName = c.UomUnit
                                }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.DestinationJob, x.ItemCode, x.ItemName, x.UnitQtyName }, (key, group) => new TraceableOutBeacukaiDetailViewModel
                                {
                                    BCDate = key.BCDate,
                                    BCNo = key.BCNo,
                                    BCType = key.BCType,
                                    DestinationJob = key.DestinationJob,
                                    ItemCode = key.ItemCode,
                                    ItemName = key.ItemName,
                                    SmallestQuantity = group.Sum(x => x.SmallestQuantity),
                                    UnitQtyName = key.UnitQtyName

                                }).ToList();

            var item = Query.Select(x => x.RO).Distinct().ToList();

            //var data = Query.Where(x => item.Contains(x.RO)).Select(s => new TraceableOutBeacukaiViewModel()).Take(1);
            var data = Query.Where(x => item.Contains(x.RO)).Take(1).ToList();
            //var rinci = data.rincian;

            foreach (var a in data)
            {
                //foreach (var i in rinciandetil)
                //{

                //    a.rincian.Add(i);

                //}
                //var rinci = rinciandetil.Where(x => x.DestinationJob == a.RO).ToList();

                a.rincian = rinciandetil;
            };




            //var cc = 

            //foreach(var g in rinciandetil)
            //{
            //    var QtyExpend = expend2.FirstOrDefault(x => x.RONo == g.DestinationJob && x.ProductCode == g.ItemCode);

            //    g.SmallestQuantity = QtyExpend == null ? g.SmallestQuantity : QtyExpend.Quantity;
            //}

            //foreach (var i in Query)
            //{
            //    var rinci = rinciandetil.Where(x => x.DestinationJob == i.RO).ToList();

            //    i.rincian = rinci;

            //}


            return Query;
        }

        //public List<TraceableOutBeacukaiDetailViewModel> getQueryDetail(string RO)
        //{
        //    var expend = GetPreparingByRo(RO);
        //    //var expenditems = new List<GarmentPreparingItemViewModel>();
        //    //foreach(var i in expend)
        //    //{
        //    //    foreach(var n in i.Items)
        //    //    {
        //    //        expenditems.Add(new GarmentPreparingItemViewModel
        //    //        {
        //    //            BasicPrice = n.BasicPrice,
        //    //            DesignColor = n.DesignColor,
        //    //            FabricType = n.FabricType,
        //    //            Id = n.Id,
        //    //            LastModifiedBy = n.LastModifiedBy,
        //    //            LastModifiedDate = n.LastModifiedDate,
        //    //            Quantity = n.Quantity,
        //    //            RemainingQuantity = n.RemainingQuantity,
        //    //            ROSource = n.ROSource,
        //    //            UENItemId = n.UENItemId
        //    //        });
        //    //    }
        //    //}

        //    //var Query = (from a in dbContext.GarmentUnitDeliveryOrderItems
        //    //             join b in dbContext.GarmentUnitDeliveryOrders on a.UnitDOId equals b.Id
        //    //             join c in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals c.UnitDOItemId
        //    //             join d in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals d.Id
        //    //             join e in dbContext.GarmentDeliveryOrderDetails on d.DODetailId equals e.Id
        //    //             join f in dbContext.GarmentDeliveryOrderItems on e.GarmentDOItemId equals f.Id
        //    //             join g in dbContext.GarmentDeliveryOrders on f.GarmentDOId equals g.Id
        //    //             join h in dbContext.GarmentBeacukais on g.CustomsId equals h.Id
        //    //             where b.RONo == (string.IsNullOrEmpty(RO) ? b.RONo : RO)
        //    //             select new TraceableOutBeacukaiDetailViewModel
        //    //             {
        //    //                 BCDate = h.BeacukaiDate.DateTime,
        //    //                 BCNo = h.BeacukaiNo,
        //    //                 BCType = h.CustomsType,
        //    //                 DestinationJob = b.RONo,
        //    //                 ItemCode = d.ProductCode,
        //    //                 ItemName = d.ProductName,
        //    //                 SmallestQuantity = c.Quantity,
        //    //                 UnitQtyName = c.UomUnit
        //    //             }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.DestinationJob, x.ItemCode, x.ItemName, x.UnitQtyName }, (key, group) => new TraceableOutBeacukaiDetailViewModel
        //    //             {
        //    //                 BCDate = key.BCDate,
        //    //                 BCNo = key.BCNo,
        //    //                 BCType = key.BCType,
        //    //                 DestinationJob = key.DestinationJob,
        //    //                 ItemCode = key.ItemCode,
        //    //                 ItemName = key.ItemName,
        //    //                 SmallestQuantity = group.Sum(x => x.SmallestQuantity),
        //    //                 UnitQtyName = key.UnitQtyName

        //    //             }).ToList();

        //    var Query = (from a in expend
        //                 join c in dbContext.GarmentUnitExpenditureNoteItems on a.UENItemId equals c.Id
        //                 join d in dbContext.GarmentUnitReceiptNoteItems on c.URNItemId equals d.Id
        //                 join e in dbContext.GarmentDeliveryOrderDetails on d.DODetailId equals e.Id
        //                 join f in dbContext.GarmentDeliveryOrderItems on e.GarmentDOItemId equals f.Id
        //                 join g in dbContext.GarmentDeliveryOrders on f.GarmentDOId equals g.Id
        //                 join h in dbContext.GarmentBeacukais on g.CustomsId equals h.Id
        //                 //where b.RONo == (string.IsNullOrEmpty(RO) ? b.RONo : RO)
        //                 select new TraceableOutBeacukaiDetailViewModel
        //                 {
        //                     BCDate = h.BeacukaiDate.DateTime,
        //                     BCNo = h.BeacukaiNo,
        //                     BCType = h.CustomsType,
        //                     DestinationJob = RO,
        //                     ItemCode = d.ProductCode,
        //                     ItemName = d.ProductName,
        //                     SmallestQuantity = a.Quantity - a.RemainingQuantity,
        //                     UnitQtyName = c.UomUnit
        //                 }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.DestinationJob, x.ItemCode, x.ItemName, x.UnitQtyName }, (key, group) => new TraceableOutBeacukaiDetailViewModel
        //                 {
        //                     BCDate = key.BCDate,
        //                     BCNo = key.BCNo,
        //                     BCType = key.BCType,
        //                     DestinationJob = key.DestinationJob,
        //                     ItemCode = key.ItemCode,
        //                     ItemName = key.ItemName,
        //                     SmallestQuantity = group.Sum(x => x.SmallestQuantity),
        //                     UnitQtyName = key.UnitQtyName

        //                 }).ToList();



        //    return Query;
        //}


        public MemoryStream GetExceltraceOut(string bcno)
        {
            var index2 = 0;
            var query = getQueryTraceableOut(bcno);
            var satuan = "-";

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "no", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal keluar", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no. bon", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "nama barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jumlah barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no invoice", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "buyer", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jenis", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "nomor", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(string) });

            DataTable result2 = new DataTable();
            result2.Columns.Add(new DataColumn() { ColumnName = "Nomor22", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Nama Barang 2", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pemakaian", DataType = typeof(Double) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Satuan 2", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "jumlah budget", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Tanggal 2", DataType = typeof(String) });


            //if (query.ToArray().Count() == 0)
            //    result.Rows.Add("", "", "", "", 0, "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            //else
            //{

            var index = 0;
            foreach (var item in query)
            {
                index++;
                string ExpenditureDate = item.ExpenditureDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(item.ExpenditureDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                string BCDate = item.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(item.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                result.Rows.Add(index, ExpenditureDate, item.ExpenditureGoodId, item.ComodityName, item.Qty, item.UnitQtyName, item.ExpenditureNo, item.BuyerName, item.BCType, item.BCNo, BCDate, item.RO);

                if (item.rincian != null)
                {
                    foreach (var detail in item.rincian)
                    {
                        index2++;
                        string BCDate2 = detail.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(detail.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        result2.Rows.Add(index2, detail.DestinationJob, detail.ItemCode, detail.ItemName, detail.SmallestQuantity, detail.UnitQtyName, detail.BCType, detail.BCNo, BCDate2);

                    }
                }

            }


            //}


            //foreach (var detail in query)
            //{
            //    //var querydetail = getQueryDetail(detail.RO);




            //    //if (querydetail.ToArray().Count() == 0)
            //    //    result2.Rows.Add("", "", "", "", 0, "", "", "", ""); // to allow column name to be generated properly for empty data as template
            //    //else
            //    //{


            //        foreach (var item in querydetail)
            //        {
            //            index2++;
            //            string BCDate = item.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(item.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            //            result2.Rows.Add(index2, item.DestinationJob, item.ItemCode, item.ItemName, item.SmallestQuantity, item.UnitQtyName, item.BCType, item.BCNo, BCDate);

            //        }
            //    //}

            //}


            ExcelPackage package = new ExcelPackage();



            var sheet = package.Workbook.Worksheets.Add("Data");


            var Tittle = new string[] { "Monitoring Pengeluaran Hasil Produksi" };
            var headers = new string[] { "No", "Tanggal Keluar", "No BON", "Nama Barang", "Jumlah Barang", "Satuan", "No. Invoice", "Buyer", "Dokumen", "Dokumen1", "Dokumen2", "RO" };
            var subHeaders = new string[] { "Jenis", "Nomor", "Tanggal" };

            //for (int i = 0; i < headers.Length; i++)
            //{
            //    result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            //}



            sheet.Cells["A5"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells["A2"].Value = Tittle[0];
            sheet.Cells["A2:L2"].Merge = true;

            sheet.Cells["I3"].Value = headers[8];
            sheet.Cells["I3:K3"].Merge = true;

            foreach (var i in Enumerable.Range(0, 8))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}3"].Value = headers[i];
                sheet.Cells[$"{col}3:{col}4"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 3))
            {
                var col = (char)('I' + i);
                sheet.Cells[$"{col}4"].Value = subHeaders[i];
            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                var col = (char)('L' + i);
                sheet.Cells[$"{col}3"].Value = headers[i + 11];
                sheet.Cells[$"{col}3:{col}4"].Merge = true;
            }
            sheet.Cells["A1:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A1:L4"].Style.Font.Bold = true;
            //-----------


            var countdata = query.Count();

            sheet.Cells[$"A{countdata + 11}"].LoadFromDataTable(result2, false, OfficeOpenXml.Table.TableStyles.Light16);

            var Tittle1 = new string[] { "PERINCIAN PEMAKAIAN BAHAN BAKU DAN BAHAN PENOLONG" };
            var headers1 = new string[] { "No", "No. RO", "Kode Barang", "Nama Barang", "Jumlah Pemakaian", "Satuan", "Dokumen", "Dokumen1", "Dokumen2" };
            var subHeaders1 = new string[] { "Jenis", "Nomor", "Tanggal" };

            sheet.Cells[$"A{countdata + 8}"].Value = Tittle1[0];
            sheet.Cells[$"A{countdata + 8}:I{countdata + 8}"].Merge = true;

            sheet.Cells[$"G{countdata + 9}"].Value = headers1[6];
            sheet.Cells[$"G{countdata + 9}:I{countdata + 9}"].Merge = true;

            foreach (var i in Enumerable.Range(0, 6))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}{countdata + 9}"].Value = headers1[i];
                sheet.Cells[$"{col}{countdata + 9}:{col}{countdata + 10}"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 3))
            {
                var col = (char)('G' + i);
                sheet.Cells[$"{col}{countdata + 10}"].Value = subHeaders1[i];
            }


            sheet.Cells[$"A{countdata + 8}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.Font.Bold = true;

            var widths = new int[] { 5, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        #endregion

        public List<TraceableInWithBUMBeacukaiViewModel> Read(string bum)
        {
            var Query = (from t in dbContext.GarmentDeliveryOrders
                         join c in dbContext.GarmentDeliveryOrderItems on t.Id equals c.GarmentDOId
                         join d in dbContext.GarmentDeliveryOrderDetails on c.Id equals d.GarmentDOItemId
                         join e in dbContext.GarmentUnitReceiptNoteItems on d.Id equals e.DODetailId
                         join r in dbContext.GarmentUnitReceiptNotes on e.URNId equals r.Id
                         join f in dbContext.GarmentUnitDeliveryOrderItems on e.Id equals f.URNItemId into unitdoitem
                         from ff in unitdoitem.DefaultIfEmpty()
                         join g in dbContext.GarmentUnitDeliveryOrders on ff.UnitDOId equals g.Id into unitdo
                         from gg in unitdo.DefaultIfEmpty()
                         join h in dbContext.GarmentUnitExpenditureNoteItems on ff.Id equals h.UnitDOItemId into uenitem
                         from hh in uenitem.DefaultIfEmpty()
                         join i in dbContext.GarmentUnitExpenditureNotes on hh.UENId equals i.Id into uen
                         from ii in uen.DefaultIfEmpty()
                         where r.URNType == "PEMBELIAN" && r.URNNo==bum
                         select new TraceableInWithBUMBeacukaiViewModel
                         {
                             //BCDate = a.BeacukaiDate,
                             //BCNo = a.BeacukaiNo,
                             //BCType = a.CustomsType,
                             //BJQty = 0,
                             //BonNo = a.BillNo,
                             URNNo = r.URNNo,
                             SmallQty = e == null ? 0 : e.SmallQuantity,
                             SatuanReceipt = e.SmallUomUnit,
                             UnitDONo = gg.UnitDONo,
                             ROJob = gg.RONo,
                             UnitDOType = gg.UnitDOType,
                             DOQuantity = ff == null ? 0 : ff.Quantity,
                             DOUomUnit = ff.UomUnit,
                             BUK = ii.UENNo,
                             QtyBUK = hh == null ? 0 : hh.Quantity ,
                             UENType = ii.ExpenditureType,
                             SatuanBUK = hh.UomUnit,
                             //ItemName = d.ProductName,
                             PO = d.POSerialNumber
                             //ReceiptQty = Math.Round((double)(e.ReceiptQuantity * e.Conversion), 2),

                             //ROSample = gg.UnitDOType == "SAMPLE" ? gg.RONo : "rosamp-",
                             //SatuanBUK = hh != null ? hh.UomUnit : "satbuk-",
                             //SatuanReceipt = e.SmallUomUnit,
                             //SampleQty = ii != null ? (ii.ExpenditureType == "SAMPLE" ? hh.Quantity : 0) : 0,
                             //UnitDOType = gg.UnitDOType
                         }).OrderBy(x=>x.URNNo).ThenBy(x=>x.PO).ThenBy(x => x.ROJob).ThenBy(x => x.PO).ThenBy(x => x.UnitDONo).ThenBy(x => x.BUK)
                         .ToList();

            //List<string> searchAttributes = new List<string>()
            //{
            //    "URNNo"
            //};

            //Query = QueryHelper<TraceableInWithBUMBeacukaiViewModel>.ConfigureSearch(Query, searchAttributes, Keyword);

            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //Query = QueryHelper<TraceableInWithBUMBeacukaiViewModel>.ConfigureFilter(Query, FilterDictionary);

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //Query = QueryHelper<TraceableInWithBUMBeacukaiViewModel>.ConfigureOrder(Query, OrderDictionary);

            //Pageable<TraceableInWithBUMBeacukaiViewModel> pageable = new Pageable<TraceableInWithBUMBeacukaiViewModel>(Query, Page - 1, Size);
            //int TotalData = pageable.TotalCount;

            //List<object> ListData = new List<object>();
            //ListData.AddRange(pageable.Data.Select(s => new
            //{
            //    s.URNNo,
            //    s.SmallQty,
            //    s.SatuanReceipt,
            //    s.UnitDONo,
            //    s.ROJob,
            //    s.UnitDOType,
            //    s.DOQuantity,
            //    s.DOUomUnit,
            //    s.BUK,
            //    s.QtyBUK,
            //    s.UENType,
            //    s.SatuanBUK
            //}));

            //return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
            //return new ReadResponse<object>(ListData, TotalData);
            return Query;

        }

        public MemoryStream GetExceltracebyBUM(string bum)
        {
            var query = Read(bum);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BON", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Terima", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "DONo", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "DO Type ", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "DO Qty", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "BUK No ", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty BUK", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Pengeluaran", DataType = typeof(string) });

            var index = 0;
            foreach (var i in query)
            {
                index++;
                result.Rows.Add(index,i.URNNo,i.PO,i.SmallQty,i.SatuanReceipt,i.UnitDONo,i.ROJob,i.UnitDOType,i.DOQuantity,i.DOUomUnit,i.BUK,i.QtyBUK,i.SatuanBUK,i.UENType);
            }


            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);

                Dictionary<string, int> nobonspan = new Dictionary<string, int>();
                Dictionary<string, int> pospan = new Dictionary<string, int>();
                Dictionary<string, int> qtyreceiptspan = new Dictionary<string, int>();

                var docNo = query.ToArray();
                int value;
                foreach (var a in query)
                {
                    if (nobonspan.TryGetValue(a.URNNo , out value))
                    {
                        nobonspan[a.URNNo]++;
                    }
                    else
                    {
                        nobonspan[a.URNNo] = 1;
                    }
                    if (pospan.TryGetValue(a.PO, out value))
                    {
                        pospan[a.PO]++;
                    }
                    else
                    {
                        pospan[a.PO] = 1;
                    }
                    if (qtyreceiptspan.TryGetValue(a.PO + a.SmallQty, out value))
                    {
                        qtyreceiptspan[a.PO+ a.SmallQty]++;
                    }
                    else
                    {
                        qtyreceiptspan[a.PO+ a.SmallQty] = 1;
                    }
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in nobonspan)
                {
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in pospan)
                {
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in qtyreceiptspan)
                {
                    sheet.Cells["D" + index + ":D" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

    }
}
