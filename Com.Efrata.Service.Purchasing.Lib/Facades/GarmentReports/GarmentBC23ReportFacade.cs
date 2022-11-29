using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentBC23ReportFacade : IGarmentBC23ReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public GarmentBC23ReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        //private List<SupplierViewModel> GetSuppliers(string code)
        //{
        //    string supplierUri = "master/garment-suppliers";
        //    IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
        //    if (httpClient != null)
        //    {
        //        var response = httpClient.GetAsync($"{APIEndpoint.Core}{supplierUri}/byCodes?code={code}").Result.Content.ReadAsStringAsync();
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
        //        List<SupplierViewModel> viewModel = JsonConvert.DeserializeObject<List<SupplierViewModel>>(result.GetValueOrDefault("data").ToString());
        //        return viewModel;
        //    }
        //    else
        //    {
        //        List<SupplierViewModel> viewModel = null;
        //        return viewModel;
        //    }
        //}

        private List<GarmentSupplierViewModel> GetSuppliers(string codes)
        {
            var param = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            if (httpClient != null)
            {
                var garmentSupplierUri = APIEndpoint.Core + $"master/garment-suppliers/byCodes";

                var httpResponse = httpClient.SendAsync(HttpMethod.Get, garmentSupplierUri, param).Result.Content.ReadAsStringAsync();
                //var response = httpClient.GetAsync($"{APIEndpoint.Core}master/garment-suppliers/byId?{param}").Result.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpResponse.Result);
                List<GarmentSupplierViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentSupplierViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<GarmentSupplierViewModel> viewModel = null;
                return viewModel;
            }
        }

        public List<GarmentBC23ReportViewModel> GetQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1070, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            List<GarmentBC23ReportViewModel> viewModels = new List<GarmentBC23ReportViewModel>();

            var Query = from a in (from a in dbContext.GarmentBeacukais
                                   where a.BeacukaiDate.Date >= DateFrom.Date
                                   && a.BeacukaiDate.Date <= DateTo.Date && !a.BeacukaiNo.Contains("BCDL")
                                   && a.CustomsType == "BC 23"
                                   select a)
                        join b in dbContext.GarmentBeacukaiItems on a.Id equals b.BeacukaiId
                        select new
                        {
                            a.BeacukaiNo,
                            a.BeacukaiDate,
                            a.SupplierId,
                            a.SupplierCode,
                            a.SupplierName,
                            a.CurrencyCode,
                            b.TotalAmount
                        };

            //var codes = string.Join(",", Query.Select(x => x.SupplierCode).Distinct());
            //HashSet<long> supplierIds = Query.Select(m => m.SupplierId).Distinct().ToHashSet();
            var code1 = string.Join(",", Query.Select(x => x.SupplierCode).ToList());
            var suppliers = GetSuppliers(code1);

            foreach(var i in Query)
            {
                var supplier = suppliers.FirstOrDefault(x => x.id == i.SupplierId);
                viewModels.Add(new GarmentBC23ReportViewModel
                {
                    BCDate = i.BeacukaiDate,
                    BeacukaiNo = i.BeacukaiNo,
                    Country = supplier == null ? "-" : supplier.address,
                    CurrencyCode = i.CurrencyCode,
                    SupplierCode = i.SupplierCode,
                    SupplierName = i.SupplierName,
                    TotalAmount = (double)i.TotalAmount
                });
            }

            return viewModels;

        }

        public Tuple<List<GarmentBC23ReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string order, int offset)
        {
            var Query = GetQuery(dateFrom, dateTo);
            Query = Query.OrderBy(x=>x.BeacukaiNo).ToList();
            //Console.WriteLine(Query);
            Pageable<GarmentBC23ReportViewModel> pageable = new Pageable<GarmentBC23ReportViewModel>(Query, page - 1, size);
            List<GarmentBC23ReportViewModel> Data = pageable.Data.ToList<GarmentBC23ReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GetXLs(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Data = GetQuery(dateFrom, dateTo);
            Data = Data.OrderBy(x => x.BeacukaiNo).ToList();
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Suppler", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Negara", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nominal", DataType = typeof(double) });

                int index = 0;
                foreach (var item in Data)
                {
                    index++;
                    string date = item.BCDate == new DateTime(1970, 1, 1) || item.BCDate.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.BCDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    // result.Rows.Add(index, item.supplierCode, item.supplierName, item.no, supplierDoDate, date, item.ePONo, item.productCode, item.productName, item.productRemark, item.dealQuantity, item.dOQuantity, item.remainingQuantity, item.uomUnit);
                    result.Rows.Add(index, item.BeacukaiNo, date, item.SupplierCode, item.SupplierName, item.Country, item.CurrencyCode, item.TotalAmount);
                }
            

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }





    }
}
