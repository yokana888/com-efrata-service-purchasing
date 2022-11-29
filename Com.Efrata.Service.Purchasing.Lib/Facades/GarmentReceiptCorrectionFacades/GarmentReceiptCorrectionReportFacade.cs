using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionNoteViewModel;
using Microsoft.EntityFrameworkCore;
using Com.Moonlay.NetCore.Lib;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Globalization;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades
{
    public class GarmentReceiptCorrectionReportFacade:IGarmentReceiptCorrectionReportFacade
    {
        private string USER_AGENT = "Facade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentCorrectionNote> dbSet;

        public GarmentReceiptCorrectionReportFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentCorrectionNote>();
        }


        private SupplierViewModel GetSupplier(long supplierId)
        {
            string supplierUri = "master/garment-suppliers";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Core}{supplierUri}/{supplierId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                SupplierViewModel viewModel = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                return null;
            }
        }
        public IQueryable<GarmentReceiptCorrectionReportViewModel> GetQuery(string unit, string category, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order)
        {
            //GarmentCorrectionNote garmentCorrectionNote = new GarmentCorrectionNote();
            //var garmentCorrectionNotes = dbContext.Set<GarmentCorrectionNote>().AsQueryable();

            

            var Query = (from b in dbContext.GarmentReceiptCorrectionItems 
                         join a in dbContext.GarmentReceiptCorrections on b.CorrectionId equals a.Id into i from a in i.DefaultIfEmpty()

                         join c in dbContext.GarmentCorrectionNoteItems on b.DODetailId equals c.DODetailId into j from c in j.DefaultIfEmpty()

                         join d in dbContext.GarmentCorrectionNotes on c.GCorrectionId equals d.Id into k from d in k.DefaultIfEmpty()

                         join e in dbContext.GarmentDeliveryOrders on d.DOId equals e.Id into l from e in l.DefaultIfEmpty()

                         join f in dbContext.GarmentUnitReceiptNotes on a.URNId equals f.Id into m from f in m.DefaultIfEmpty()

                         join g in dbContext.GarmentExternalPurchaseOrderItems on b.EPOItemId equals g.Id into n from g in n.DefaultIfEmpty()
                         join h in dbContext.GarmentUnitReceiptNoteItems on f.Id equals h.URNId into o from h in o.DefaultIfEmpty()

                         join z in dbContext.GarmentDeliveryOrderDetails on b.DODetailId equals z.Id into p from z in p.DefaultIfEmpty()

                         where
                         a.IsDeleted == false
                         &&b.IsDeleted == false
                         && c.IsDeleted == false
                         && d.IsDeleted == false
                         && e.IsDeleted == false
                         && f.IsDeleted == false
                         && g.IsDeleted == false
                         && h.IsDeleted == false
                        && a.UnitCode == (string.IsNullOrWhiteSpace(unit) ? a.UnitCode : unit)
                        && a.CorrectionDate.Date >= dateFrom
                        && a.CorrectionDate.Date <= dateTo
                         && z.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? z.CodeRequirment : category)
                         orderby

                         a.CorrectionDate descending
                        
                         select new 
                         {
                             CorrectionNo = a.CorrectionNo,
                             CorrectionDate = a.CorrectionDate,
                             CorrectionNote = d.CorrectionType == "Jumlah" ? d.CorrectionNo : "-",
                             URNNo = a.URNNo,
                             BillNo = e.BillNo,
                             SupplierId = f.SupplierId,
                             //Supplier = GetSupplier(f.SupplierId),
                               
                             //Supplier = supplier.import == true && supplier.code != "GDG" ? "Pembelian Import" :
                             //           supplier.import == false && supplier.code != "GDG" ? "Pembelian Local" :
                             //           "GDG Pbl Gmt",
                             SupplierCode = f.SupplierCode,
                             RONo = b.RONo,
                             Article = g.Article,
                             DONo = f.DONo,
                             POSerialNumber = b.POSerialNumber,
                             ProductCode = b.ProductCode,
                             ProductName = b.ProductName,
                             ProductRemark = b.ProductRemark,
                             Quantity = b.Quantity,
                             UomUnit = b.UomUnit,
                             Conversion =b.Conversion,
                             SmallQuantity = b.SmallQuantity,
                             SmallUomUnit = b.SmallUomUnit
                              
                         }).Distinct();

            List<GarmentReceiptCorrectionReportViewModel> Data = new List<GarmentReceiptCorrectionReportViewModel>();
            foreach (var item in Query)
            {

              var supplier = GetSupplier(item.SupplierId);
                Data.Add(
                    
                    new GarmentReceiptCorrectionReportViewModel
                {
                    CorrectionNo = item.CorrectionNo,
                    CorrectionDate = item.CorrectionDate,
                    CorrectionNote = item.CorrectionNote,
                    URNNo = item.URNNo,
                    BillNo = item.BillNo,
                    SupplierId = item.SupplierId,
                    Supplier = supplier.import == true && supplier.code != "GDG" ? "Pembelian Import" :
                               supplier.import == false && supplier.code != "GDG" ? "Pembelian Local" :
                               "GDG Pbl Gmt",
                    SupplierCode = item.SupplierCode,
                    RONo = item.RONo,
                    Article = item.Article,
                    DONo = item.DONo,
                    POSerialNumber = item.POSerialNumber,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    ProductRemark = item.ProductRemark,
                    Quantity = item.Quantity,
                    UomUnit = item.UomUnit,
                    Conversion = item.Conversion,
                    SmallQuantity = item.SmallQuantity,
                    SmallUomUnit = item.SmallUomUnit


                });

            }

            

            return Data.AsQueryable();
        }

        public Tuple<List<GarmentReceiptCorrectionReportViewModel>, int> GetReport(string unit, string category, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order, int page, int size)
        {
            var Query = GetQuery(unit, category, dateFrom, dateTo, order);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //	Query = Query.OrderByDescending(b => b.poExtDate);
            //}

            Pageable<GarmentReceiptCorrectionReportViewModel> pageable = new Pageable<GarmentReceiptCorrectionReportViewModel>(Query, page - 1, size);
            List<GarmentReceiptCorrectionReportViewModel> Data = pageable.Data.ToList<GarmentReceiptCorrectionReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }



        //public async Task<GarmentReceiptCorrectionNoteWrapper> GetReport(string unit, string category,  DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order, int page, int size)
        //{
        //    var Query = GetQuery(unit, category, dateFrom, dateTo, order);

        //    return new GarmentReceiptCorrectionNoteWrapper()
        //    {
        //        Total = await Query.CountAsync(),
        //        Data = await Query
        //        .Skip((page - 1) * size)
        //        .Take(size)
        //        .ToListAsync()
        //    };


        //}

        public MemoryStream GenerateExcel(string unit, string category, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string order)
        {
            var Query = GetQuery(unit, category, dateFrom, dateTo, order);
            Query = Query.OrderByDescending(b => b.CorrectionDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Koreksi Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TGL Koreksi Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode SPL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Article", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Konversi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "QTY Konversi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Konversi", DataType = typeof(string) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, 0, 0 ); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string Corr = item.CorrectionDate == null ? "-" : item.CorrectionDate.Value.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.CorrectionNo, Corr, item.CorrectionNote, item.URNNo, item.BillNo, 
                        item.Supplier, item.SupplierCode, item.RONo, item.Article, item.DONo, item.POSerialNumber, item.ProductCode, item.ProductName,
                        item.ProductRemark, item.Quantity, item.UomUnit,item.Conversion, item.SmallQuantity, item.SmallUomUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }


    }
}
