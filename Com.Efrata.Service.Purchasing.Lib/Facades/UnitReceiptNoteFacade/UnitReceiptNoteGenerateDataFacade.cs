using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
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


namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade
{
    public class UnitReceiptNoteGenerateDataFacade
    {
        private readonly PurchasingDbContext DbContext;
        private readonly DbSet<UnitReceiptNoteFacade> DbSet;
        private IdentityService IdentityService;
      
        public UnitReceiptNoteGenerateDataFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<UnitReceiptNoteFacade>();
            this.IdentityService = serviceProvider.GetService<IdentityService>();
        }

        public IQueryable<UnitReceiptNoteGenerateDataViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in DbContext.UnitReceiptNotes
                         join b in DbContext.UnitReceiptNoteItems on a.Id equals b.URNId
                         where a.IsDeleted == false && b.IsDeleted == false &&
                               a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date &&
                               a.ReceiptDate.AddHours(offset).Date <= DateTo.Date                             
                         select new UnitReceiptNoteGenerateDataViewModel
                         {
                             URNNo = a.URNNo,
                             URNDate = a.ReceiptDate,
                             UnitName = a.DivisionName + "-" + a.UnitName,
                             SupplierCode = a.SupplierCode,
                             SupplierName = a.SupplierName,
                             DONo = a.DONo,
                             URNRemark = a.Remark,
                             ProductCode = b.ProductCode,
                             ProductName = b.ProductName,
                             Quantity = b.ReceiptQuantity,
                             UOMUnit = b.Uom,
                             Remark = b.ProductRemark,
                         }
                         );
            return Query;
        }

        //public Tuple<List<UnitReceiptNoteGenerateDataViewModel>, int> GetDisplayReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)

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
        //    foreach (UnitReceiptNoteGenerateDataViewModel a in q)
        //    {
        //        index++;
        //    }

        //    Pageable<UnitReceiptNoteGenerateDataViewModel> pageable = new Pageable<UnitReceiptNoteGenerateDataViewModel>(Query, page - 1, size);
        //    List<UnitReceiptNoteGenerateDataViewModel> Data = pageable.Data.ToList<UnitReceiptNoteGenerateDataViewModel>();
        //    int TotalData = pageable.TotalCount;

        //    return Tuple.Create(Data, TotalData);
        //}
        
        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.URNNo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON TERIMA UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BON TERIMA UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BAGIAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR SURAT JALAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH TERIMA", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN BARANG", DataType = typeof(String) });
            
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string URNDate = item.URNDate == new DateTime(1970, 1, 1) ? "-" : item.URNDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
          
                    result.Rows.Add(item.URNNo, URNDate, item.UnitName, item.SupplierCode, item.SupplierName, item.DONo, item.URNRemark, item.ProductCode, item.ProductName, item.Quantity, item.UOMUnit, item.Remark);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);

        }
    }
}
