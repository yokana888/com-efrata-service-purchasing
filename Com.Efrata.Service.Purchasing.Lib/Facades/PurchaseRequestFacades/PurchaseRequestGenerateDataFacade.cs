using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
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


namespace Com.Efrata.Service.Purchasing.Lib.Facades.PurchaseRequestFacades
{
    public class PurchaseRequestGenerateDataFacade
    {
        private readonly PurchasingDbContext DbContext;
        private readonly DbSet<PurchaseRequest> DbSet;
        private IdentityService IdentityService;
      
        public PurchaseRequestGenerateDataFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<PurchaseRequest>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<PurchaseRequestGenerateDataViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in DbContext.InternalPurchaseOrders
                         join b in DbContext.InternalPurchaseOrderItems on a.Id equals b.POId
                         join d in DbContext.PurchaseRequestItems on b.PRItemId equals d.Id
                         join c in DbContext.PurchaseRequests on d.PurchaseRequestId equals c.Id
                         where a.IsDeleted == false && b.IsDeleted == false &&
                               c.IsDeleted == false && d.IsDeleted == false &&
                               c.IsPosted == true &&
                               c.Date.AddHours(offset).Date >= DateFrom.Date &&
                               c.Date.AddHours(offset).Date <= DateTo.Date                             
                         select new PurchaseRequestGenerateDataViewModel
                         {
                             PRNo = c.No,
                             PRDate = c.Date,
                             PRExpectedDeliveryDate = c.ExpectedDeliveryDate,
                             PRReceiptDate = a.CreatedUtc,
                             Budget = c.BudgetCode,
                             UnitName = c.DivisionName + "-" + c.UnitName,
                             Category = c.CategoryCode,
                             PRRemark = c.Remark,
                             ProductCode = d.ProductCode,
                             ProductName = d.ProductName,
                             Quantity = d.Quantity,
                             UOMUnit = d.Uom,
                             Remark = d.Remark,
                             UserCreated = c.CreatedBy,
                             IsPosted = c.IsPosted ? "SUDAH " : "BELUM",
                         }
                         );
            return Query;
        }

        //public Tuple<List<PurchaseRequestGenerateDataViewModel>, int> GetDisplayReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)

        //{
        //    var Query = GetReportQuery(dateFrom, dateTo, offset);

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        Query = Query.OrderBy(b => b.UnitName).ThenBy(b => b.PRNo); 
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];
        //    }

        //    var q = Query.ToList();
        //    var index = 0;
        //    foreach (PurchaseRequestGenerateDataViewModel a in q)
        //    {
        //        index++;
        //    }

        //    Pageable<PurchaseRequestGenerateDataViewModel> pageable = new Pageable<PurchaseRequestGenerateDataViewModel>(Query, page - 1, size);
        //    List<PurchaseRequestGenerateDataViewModel> Data = pageable.Data.ToList<PurchaseRequestGenerateDataViewModel>();
        //    int TotalData = pageable.TotalCount;

        //    return Tuple.Create(Data, TotalData);
        //}
        
        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.UnitName).ThenBy(b => b.PRNo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PURCHASE REQUEST", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL PURCHASE REQUEST", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DIMINTA", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL TERIMA PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BUDGET", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BAGIAN / UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE KATEGORI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "USER INPUT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "STATUS POST", DataType = typeof(string) });
   
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string PRDate = item.PRDate == new DateTime(1970, 1, 1) ? "-" : item.PRDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string ExpectedDate = item.PRExpectedDeliveryDate == null ? "-" : item.PRExpectedDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string ReceiptDate = item.PRReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.PRReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.PRNo, PRDate, ExpectedDate, ReceiptDate, item.Budget, item.UnitName, item.Category, item.PRRemark, item.ProductCode, item.ProductName, item.Quantity, item.UOMUnit, item.Remark, item.UserCreated, item.IsPosted);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);

        }
    }
}
