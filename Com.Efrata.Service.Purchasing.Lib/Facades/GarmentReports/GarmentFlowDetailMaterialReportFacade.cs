using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.NetCore.Lib;
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
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
	public class GarmentFlowDetailMaterialReportFacade : IGarmentFlowDetailMaterialReport
	{
		private readonly PurchasingDbContext dbContext;
		public readonly IServiceProvider serviceProvider;
		private readonly DbSet<GarmentUnitExpenditureNote> dbSet;


		public GarmentFlowDetailMaterialReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
		{
			this.serviceProvider = serviceProvider;
			this.dbContext = dbContext;
			this.dbSet = dbContext.Set<GarmentUnitExpenditureNote>();
		}

		private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
		{
			//var param = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");
			IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
			if (httpClient != null)
			{
				//var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
				var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories/join-by-product";
				string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
				string uri = garmentSupplierUri + queryUri;
				var response = httpClient.GetAsync($"{uri}").Result.Content.ReadAsStringAsync();
				Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
				List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());
				return viewModel;
			}
			else
			{
				List<GarmentCategoryViewModel> viewModel = null;
				return viewModel;
			}
		}

		public IQueryable<GarmentFlowDetailMaterialViewModel> GetQuery(string category, string productcode, string unit, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int offset)
		{
            //DateTimeOffset dateFrom = DateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)DateFrom;
            //DateTimeOffset dateTo = DateTo == null ? new DateTime(2100, 1, 1) : (DateTimeOffset)DateTo;

            string filter = (string.IsNullOrWhiteSpace(category) ? "{}" : "{" + "'" + "CodeRequirement" + "'" + ":" + "'" + category + "'" + "}");

            var categories = GetProductCodes(1, int.MaxValue, "{}", filter);

			var categories1 =  categories.Select(x => x.Name).ToHashSet();

			if(unit == "SMP1")
			{
				var Query = (from a in dbContext.GarmentUnitExpenditureNoteItems
							 join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id

							 join e in dbContext.GarmentUnitDeliveryOrderItems on a.UnitDOItemId equals e.Id
							 join d in dbContext.GarmentUnitDeliveryOrders on e.UnitDOId equals d.Id
							 join g in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals g.Id
							 join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on g.EPOItemId equals c.Id
							 where
							 a.IsDeleted == false && b.IsDeleted == false &&
							 categories1.Contains(a.ProductName) &&
							 b.CreatedUtc.Date >= DateFrom
							 && b.CreatedUtc.Date <= DateTo
							 && b.UnitSenderCode == "SMP1"

							 orderby a.CreatedUtc descending
							 select new GarmentFlowDetailMaterialViewModel
							 {
								 ProductCode = a.ProductCode,
								 ProductName = e.ProductName == "" ? a.ProductName : e.ProductName,
								 POSerialNumber = a.POSerialNumber,
								 ProductRemark = a.ProductRemark,
								 RONo = a.RONo,
								 Article = c.Article,
								 BuyerCode = a.BuyerCode,
								 RONoDO = d.RONo,
								 ArticleDO = d.Article,
								 UnitDOType = d.UnitDOType,
								 UENNo = b.UENNo,
								 ExpenditureDate = b.CreatedUtc,
								 Quantity = a.Quantity,
								 UomUnit = a.UomUnit,
								 Total = (a.BasicPrice / (a.Conversion == 0 ? 1 : a.Conversion)) * Convert.ToDecimal(a.Quantity),
								 UnitDestination = (b.ExpenditureType == "TRANSFER" || b.ExpenditureType == "GUDANG LAIN") ? b.UnitRequestName : b.ExpenditureType == "EXTERNAL" ? "RETUR" : b.ExpenditureType

							 });
				return Query.AsQueryable();
			}
			else
			{
				var Query = (from a in dbContext.GarmentUnitExpenditureNoteItems
							 join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id

							 join e in dbContext.GarmentUnitDeliveryOrderItems on a.UnitDOItemId equals e.Id
							 join d in dbContext.GarmentUnitDeliveryOrders on e.UnitDOId equals d.Id
							 join g in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals g.Id
							 join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on g.EPOItemId equals c.Id
							 //join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on g.EPOItemId equals c.Id
							 //join f in categories on a.ProductCode equals f.Code
							 where
							 a.IsDeleted == false && b.IsDeleted == false &&
							 categories1.Contains(a.ProductName) &&
							 //f.CodeRequirement == (string.IsNullOrWhiteSpace(category) ? f.CodeRequirement : category)
							 //(string.IsNullOrWhiteSpace(category) ? a.ProductCode == a.ProductCode : categories1.Contains(a.ProductCode))
							 //&& f.ProductCode.Substring(0, 3) == (string.IsNullOrWhiteSpace(productcode) ? f.ProductCode.Substring(0, 3) : productcode)
							 b.CreatedUtc.Date >= DateFrom
							 && b.CreatedUtc.Date <= DateTo
							 && b.UnitSenderCode == (string.IsNullOrWhiteSpace(unit) ? b.UnitSenderCode : unit)

							 orderby a.CreatedUtc descending
							 select new GarmentFlowDetailMaterialViewModel
							 {
								 ProductCode = a.ProductCode,
								 ProductName = e.ProductName == "" ? a.ProductName : e.ProductName,
								 POSerialNumber = a.POSerialNumber,
								 ProductRemark = a.ProductRemark,
								 RONo = a.RONo,
								 Article = c.Article,
								 BuyerCode = a.BuyerCode,
								 RONoDO = d.RONo,
								 ArticleDO = d.Article,
								 UnitDOType = d.UnitDOType,
								 UENNo = b.UENNo,
								 ExpenditureDate = b.CreatedUtc,
								 Quantity = a.Quantity,
								 UomUnit = a.UomUnit,
								 Total = (a.BasicPrice / (a.Conversion == 0 ? 1 : a.Conversion)) * Convert.ToDecimal(a.Quantity),
								 UnitDestination = (b.ExpenditureType == "TRANSFER" || b.ExpenditureType == "GUDANG LAIN") ? b.UnitRequestName : b.ExpenditureType == "EXTERNAL" ? "RETUR" : b.ExpenditureType

							 });
				return Query.AsQueryable();
			}
           

            //Query = string.IsNullOrWhiteSpace(category) ? Query : Query.Where(x => categories1.Contains(x.ProductName)).Select(x => x);


            
        }

        public Tuple<List<GarmentFlowDetailMaterialViewModel>, int> GetReport(string category, string productcode, string unit, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int offset, string order, int page, int size)
        {
            var Query = GetQuery(category, productcode, unit, DateFrom, DateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //	Query = Query.OrderByDescending(b => b.poExtDate);
            //}

            //Pageable<GarmentFlowDetailMaterialViewModel> pageable = new Pageable<GarmentFlowDetailMaterialViewModel>(Query, page - 1, size);
            //List<GarmentFlowDetailMaterialViewModel> Data = pageable.Data.ToList<GarmentFlowDetailMaterialViewModel>();
            //int TotalData = pageable.TotalCount;

            return Tuple.Create(Query.ToList(), Query.Count());
        }


        public MemoryStream GenerateExcel(string category, string productcode, string categoryname, string unit, string unitname, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            var Query = GetQuery(category, productcode, unit, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.CreatedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            double ExpendQtyTotal = 0;
            decimal ExpendPriceTotal = 0;

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. R/O", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Untuk RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Untuk Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No.Bukti", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "","", 0, "", 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    ExpendQtyTotal += item.Quantity;
                    ExpendPriceTotal += item.Total;
                    index++;
                    string tanggal = item.ExpenditureDate.Value.AddHours(offset).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.ProductCode, item.ProductName, item.POSerialNumber, item.ProductRemark, item.RONo,
                        item.Article, item.BuyerCode, item.RONoDO, item.ArticleDO, item.UENNo, item.UnitDestination, tanggal, NumberFormat(item.Quantity),
                        item.UomUnit, NumberFormat((double)item.Total));
                }
            }
            ExcelPackage package = new ExcelPackage();
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom.Value.DateTime;
            //DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo.Value.DateTime;
            CultureInfo Id = new CultureInfo("id-ID");

            var sheet = package.Workbook.Worksheets.Add("Report");

            var col = (char)('A' + result.Columns.Count);
            string tglawal = dateFrom.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = dateTo.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN REKAP PENGELUARAN {0}", string.IsNullOrWhiteSpace(productcode) ? categoryname : "INTERLINING");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("KONFEKSI : {0}", unitname);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A5"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            var a = Query.Count();
            sheet.Cells[$"A{6 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Merge = true;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"N{6 + a}"].Value = NumberFormat(ExpendQtyTotal);
            sheet.Cells[$"N{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"P{6 + a}"].Value = NumberFormat((double)ExpendPriceTotal);
            sheet.Cells[$"P{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"O{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public MemoryStream GenerateExcelForUnit(string category, string productcode, string categoryname, string unit, string unitname, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            var Query = GetQuery(category, productcode, unit, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.CreatedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            double ExpendQtyTotal = 0;
            decimal ExpendPriceTotal = 0;

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. R/O", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Untuk RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Untuk Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No.Bukti", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    ExpendQtyTotal += item.Quantity;
                    ExpendPriceTotal += item.Total;
                    index++;
                    string tanggal = item.ExpenditureDate.Value.AddHours(offset).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.ProductCode, item.ProductName, item.POSerialNumber, item.ProductRemark, item.RONo,
                        item.Article, item.BuyerCode, item.RONoDO, item.ArticleDO, item.UnitDestination, item.UENNo, tanggal, NumberFormat(item.Quantity),
                        item.UomUnit);
                }
            }
            ExcelPackage package = new ExcelPackage();
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom.Value.DateTime;
            //DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo.Value.DateTime;
            CultureInfo Id = new CultureInfo("id-ID");

            var sheet = package.Workbook.Worksheets.Add("Report");

            var col = (char)('A' + result.Columns.Count);
            string tglawal = dateFrom.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = dateTo.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN REKAP PENGELUARAN {0}", string.IsNullOrWhiteSpace(productcode) ? categoryname : "INTERLINING");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("KONFEKSI : {0}", unitname);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A5"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            var a = Query.Count();
            sheet.Cells[$"A{6 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Merge = true;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{6 + a}:M{6 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"N{6 + a}"].Value = NumberFormat(ExpendQtyTotal);
            sheet.Cells[$"N{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"O{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //sheet.Cells[$"{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        String NumberFormat(double? numb)
        {

            var number = string.Format("{0:0,0.00}", numb);

            return number;
        }
    }
}
