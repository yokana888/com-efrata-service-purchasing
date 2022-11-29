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
    public class MonitoringPriceFacade
    {
        private readonly PurchasingDbContext DbContext;
        private readonly DbSet<ExternalPurchaseOrder> DbSet;
        private IdentityService IdentityService;
      
        public MonitoringPriceFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<ExternalPurchaseOrder>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<MonitoringPriceViewModel> GetDisplayQuery(string product, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in DbContext.ExternalPurchaseOrders
                         join b in DbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
                         join c in DbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
                         // Delivery Order
                         join d in DbContext.DeliveryOrderItems on a.Id equals d.EPOId into e
                         from DOItem in e.DefaultIfEmpty()
                         join f in DbContext.DeliveryOrderItems on DOItem.DOId equals f.Id into g
                         from DO in g.DefaultIfEmpty()
                         // Unit Receipt Note
                         join h in DbContext.UnitReceiptNotes on DO.Id equals h.DOId into i
                         from URN in i.DefaultIfEmpty()
                         // Unit Payment Order
                         join j in DbContext.UnitPaymentOrderItems on URN.Id equals j.URNId into k
                         from UPOItem in k.DefaultIfEmpty()
                         join l in DbContext.UnitPaymentOrders on UPOItem.UPOId equals l.Id into m
                         from UPO in m.DefaultIfEmpty()

                         where a.IsDeleted == false
                               && b.IsDeleted == false
                               && c.IsDeleted == false
                               && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
                               && a.OrderDate.AddHours(offset).Date <= DateTo.Date                             
                               && c.ProductId == (string.IsNullOrWhiteSpace(product) ? c.ProductId : product)
                       
                         select new MonitoringPriceViewModel
                         {
                             EPONo = a.EPONo,
                             EPODate = a.OrderDate,
                             UnitName = a.DivisionName + "-" + a.UnitName,
                             PRNo = b.PRNo,
                             SupplierCode = a.SupplierCode,
                             SupplierName = a.SupplierName,
                             InvoiceNo = UPO.InvoiceNo ?? "-" ,
                             InvoiceDate = UPO == null ? new DateTime(1970, 1, 1) : UPO.InvoiceDate,
                             ProductCode = c.ProductCode,
                             ProductName = c.ProductName, 
                             Quantity = String.Format("{0:n}", c.DealQuantity),
                             UOMUnit = c.DealUomUnit,
                             CurrencyCode = a.CurrencyCode,
                             CurrencyRate = String.Format("{0:n}", a.CurrencyRate),
                             PricePerDeal = String.Format("{0:n}", c.PricePerDealUnit),                           
                             Amount = String.Format("{0:n}", c.DealQuantity * c.PricePerDealUnit * a.CurrencyRate),
        }
                         );
            return Query;
        }

        public Tuple<List<MonitoringPriceViewModel>, int> GetDisplayReport(string product, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)

        {
            var Query = GetDisplayQuery(product, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.EPODate); 
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
            }

            var q = Query.ToList();
            var index = 0;
            foreach (MonitoringPriceViewModel a in q)
            {
                index++;
            }
      
            Pageable<MonitoringPriceViewModel> pageable = new Pageable<MonitoringPriceViewModel>(Query, page - 1, size);
            List<MonitoringPriceViewModel> Data = pageable.Data.ToList<MonitoringPriceViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        
        public MemoryStream GenerateExcel(string product, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetDisplayQuery(product, dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.SupplierCode).ThenBy(b => b.EPONo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Po Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Harga", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;

                    string EPODate = item.EPODate == null ? "-" : item.EPODate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));                
                    string InvoiceDate = item.InvoiceDate == new DateTime(1970, 1, 1) ? "-" : item.InvoiceDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));                   
                
                    result.Rows.Add(index, item.EPONo, EPODate, item.UnitName, item.PRNo, item.SupplierCode, item.SupplierName, item.InvoiceNo, InvoiceDate, item.ProductCode, item.ProductName, item.Quantity, item.UOMUnit, item.CurrencyCode, item.CurrencyRate, item.PricePerDeal, item.Amount);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }
    }
}
