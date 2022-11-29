using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade
{
    public class ExternalPurchaseOrderGenerateDataFacade
    {
        private readonly PurchasingDbContext DbContext;
        private readonly DbSet<ExternalPurchaseOrder> DbSet;
        private IdentityService IdentityService;
      
        public ExternalPurchaseOrderGenerateDataFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<ExternalPurchaseOrder>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<ExternalPurchaseOrderGenerateDataViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in DbContext.ExternalPurchaseOrders
                         join b in DbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
                         join c in DbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
                         where a.IsDeleted == false && b.IsDeleted == false &&
                               c.IsDeleted == false && a.IsPosted == true &&
                               a.OrderDate.AddHours(offset).Date >= DateFrom.Date &&
                               a.OrderDate.AddHours(offset).Date <= DateTo.Date                             
                         select new ExternalPurchaseOrderGenerateDataViewModel
                         {
                             EPONo = a.EPONo,
                             EPODate = a.OrderDate,
                             SupplierCode = a.SupplierCode,
                             SupplierName = a.SupplierName,
                             DeliveryDate = a.DeliveryDate,
                             FreightCostBy = a.FreightCostBy,
                             PaymentMethod = a.PaymentMethod,
                             POCashType = a.POCashType,
                             PaymentDueDays = a.PaymentDueDays,
                             CurrencyCode = a.CurrencyCode,
                             CurrencyRate = a.CurrencyRate,
                             UseVat = a.UseVat ? "YA " : "TIDAK",
                             UseIncomeTax = a.UseIncomeTax ? "YA " : "TIDAK",
                             IncomeTaxRate = a.IncomeTaxRate,
                             Remark = a.Remark,
                             PRNo = b.PRNo,
                             ProductCode = c.ProductCode,
                             ProductName = c.ProductName,
                             DealQuantity = c.DealQuantity,
                             UOMUnit = c.DealUomUnit,
                             PricePerDealUnit = c.PricePerDealUnit,
                             Amount = c.PricePerDealUnit * c.DealQuantity,
                             UserCreated = a.CreatedBy,
                             IsPosted = a.IsPosted ? "SUDAH " : "BELUM",
                         }
                         );
            return Query;
        }

        //public Tuple<List<ExternalPurchaseOrderGenerateDataViewModel>, int> GetDisplayReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)

        //{
        //    var Query = GetReportQuery(dateFrom, dateTo, offset);

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        Query = Query.OrderBy(b => b.EPONo);
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];
        //    }

        //    var q = Query.ToList();
        //    var index = 0;
        //    foreach (ExternalPurchaseOrderGenerateDataViewModel a in q)
        //    {
        //        index++;
        //    }

        //    Pageable<ExternalPurchaseOrderGenerateDataViewModel> pageable = new Pageable<ExternalPurchaseOrderGenerateDataViewModel>(Query, page - 1, size);
        //    List<ExternalPurchaseOrderGenerateDataViewModel> Data = pageable.Data.ToList<ExternalPurchaseOrderGenerateDataViewModel>();
        //    int TotalData = pageable.TotalCount;

        //    return Tuple.Create(Data, TotalData);
        //}

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.EPONo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PO EXTERNAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PO EXTERNAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "DELIVERY", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "ONGKIR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PAYMENT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO CASH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TEMPO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "MATA UANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RATE", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "% PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PURCHASE REQUEST", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "HARGA SATUAN BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "HARGA TOTAL BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "USER INPUT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "STATUS POST", DataType = typeof(string) });
   
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", "", "", "", "", "", "", 0, "", 0, 0, "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string EPODate = item.EPODate == new DateTime(1970, 1, 1) ? "-" : item.EPODate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string DeliveryDate = item.DeliveryDate == null ? "-" : item.DeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.EPONo, EPODate, item.SupplierCode, item.SupplierName, DeliveryDate, item.FreightCostBy, item.PaymentMethod, item.POCashType, item.PaymentDueDays,
                                    item.CurrencyCode, item.CurrencyRate, item.UseVat, item.UseIncomeTax, item.IncomeTaxRate, item.Remark, item.PRNo, item.ProductCode,
                                    item.ProductName,item.DealQuantity, item.UOMUnit, item.PricePerDealUnit, item.Amount, item.UserCreated, item.IsPosted);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);

        }
    }
}
