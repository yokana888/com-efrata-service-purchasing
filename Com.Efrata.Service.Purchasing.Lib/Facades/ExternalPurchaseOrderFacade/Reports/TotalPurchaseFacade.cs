
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel.Reports;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade.Reports
{
	public class TotalPurchaseFacade
	{
		private readonly PurchasingDbContext dbContext;
		public readonly IServiceProvider serviceProvider;
		private readonly DbSet<ExternalPurchaseOrder> dbSet;

		public TotalPurchaseFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
		{
			this.serviceProvider = serviceProvider;
			this.dbContext = dbContext;
			this.dbSet = dbContext.Set<ExternalPurchaseOrder>();
		}
		#region BySupplier
		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseBySupplierReportQuery(string division, string unit, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			var Total = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where  a.IsDeleted==false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled ==false && a.IsPosted== true &&
						  a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit) && d.CategoryId == (string.IsNullOrWhiteSpace(category) ? d.CategoryId : category)
						 && a.DivisionId == (string.IsNullOrWhiteSpace(division) ? a.DivisionId : division)
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 select c.DealQuantity * c.PricePerDealUnit * a.CurrencyRate).Sum();
			var Query = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where  a.IsDeleted==false && b.IsDeleted == false && c.IsDeleted ==false & d.IsDeleted==false && a.IsCanceled == false &&  a.IsPosted == true &&
						 c.DealQuantity !=0 && a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit) && d.CategoryId == (string.IsNullOrWhiteSpace(category) ? d.CategoryId : category)
						 && a.DivisionId == (string.IsNullOrWhiteSpace(division) ? a.DivisionId : division) 
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 group new { DealQuantity = c.DealQuantity , PricePerDealUnit = c.PricePerDealUnit,Rate=a.CurrencyRate} by new { a.SupplierName, a.UnitName, d.CategoryName ,a.DivisionName} into G
						 select new TotalPurchaseBySupplierViewModel
						 {
							 supplierName =G.Key.SupplierName,
							 unitName = G.Key.UnitName,
							 categoryName = G.Key.CategoryName,
							 divisionName=G.Key.DivisionName,
							 amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate), 2),
							 total =  (Decimal)Math.Round(Total,2)
						 });
			return Query;
		}

		public   IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseBySupplierReport(string division, string unit, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseBySupplierReportQuery(division,unit, category , dateFrom, dateTo, offset);
			Query = Query.OrderBy(b => b.supplierName).ThenBy(b=>b.unitName).ThenBy(b=>b.categoryName);
			return Query;
		}

		public MemoryStream GenerateExcelTotalPurchaseBySupplier(string division, string unit, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseBySupplierReportQuery(division,unit, category , dateFrom, dateTo, offset);
			DataTable result = new DataTable();
		
			result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah(Rp)", DataType = typeof(Decimal) });
			result.Columns.Add(new DataColumn() { ColumnName = "%", DataType = typeof(Decimal) });

			decimal Total = 0;
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("", "","", "", "",0 ,0); // to allow column name to be generated properly for empty data as template
			else
			{
				int index = 0;
				foreach (var item in Query)
				{
					index++;
					Total = item.total;
						result.Rows.Add(index, item.supplierName, item.divisionName,item.unitName,item.categoryName, (Decimal)Math.Round((item.amount), 2), (Decimal)Math.Round((item.amount / item.total),2));
				}
				result.Rows.Add("", "Total Pembelian", "", "","", Total, 100);
			}

			return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
		}
		#endregion
		#region ByCategories
		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByCategoriesReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			var Total = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true 
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 select c.DealQuantity * c.PricePerDealUnit * a.CurrencyRate).Sum();
			var Query = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true 
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 group new { DealQuantity = c.DealQuantity, PricePerDealUnit = c.PricePerDealUnit ,Rate = a.CurrencyRate} by new { d.CategoryName } into G
						 select new TotalPurchaseBySupplierViewModel
						 {
							 categoryName = G.Key.CategoryName,
							 amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate), 2),
							 total = (Decimal)Math.Round(Total, 2)
						 });
			return Query;
		}

		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByCategoriesReport( DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByCategoriesReportQuery( dateFrom, dateTo, offset);
			Query = Query.OrderBy(b => b.categoryName);
			return Query;
		}

		public MemoryStream GenerateExcelTotalPurchaseByCategories( DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByCategoriesReportQuery(dateFrom, dateTo, offset);
			DataTable result = new DataTable();

			result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Rp", DataType = typeof(Decimal) });
			result.Columns.Add(new DataColumn() { ColumnName = "%", DataType = typeof(Decimal) });

			decimal Total = 0;
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("", "", 0, 0); // to allow column name to be generated properly for empty data as template
			else
			{
				int index = 0;
				foreach (var item in Query)
				{
					index++;
					Total = item.total;
					result.Rows.Add(index, item.categoryName, (Decimal)Math.Round((item.amount), 2), (Decimal)Math.Round((item.amount / item.total), 2));
				}
				result.Rows.Add("", "Total Pembelian", Total, 100);
			}

			return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
		}
		#endregion
		#region ByUnit
		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByUnitsReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			var Total = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 select c.DealQuantity * c.PricePerDealUnit*a.CurrencyRate).Sum();
			var Query = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 group new { DealQuantity = c.DealQuantity, PricePerDealUnit = c.PricePerDealUnit,Rate=a.CurrencyRate } by new { d.DivisionName ,d.DivisionId} into G
						 select new TotalPurchaseBySupplierViewModel
						 {
							 divisionName = G.Key.DivisionName,
							 divisionId=G.Key.DivisionId,
							 amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit *c.Rate), 2),
							 total = (Decimal)Math.Round(Total, 2)
						 });
			return Query;
		}

		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByUnitsReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByUnitsReportQuery(dateFrom, dateTo, offset);
			Query = Query.OrderBy(b => b.divisionName);
			return Query;
		}
		public IQueryable<TotalPurchaseBySupplierViewModel> GetDetailTotalPurchaseByUnitsReportByDivisionIdReport(string divisionId,DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			var Total = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.DivisionId == divisionId && a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 select c.DealQuantity * c.PricePerDealUnit * a.CurrencyRate).Sum();
			var Query = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.DivisionId == divisionId && a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 group new { DealQuantity = c.DealQuantity, PricePerDealUnit = c.PricePerDealUnit,Rate=a.CurrencyRate } by new { d.UnitName } into G
						 select new TotalPurchaseBySupplierViewModel
						 {
							 unitName = G.Key.UnitName,
							 amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate), 2),
							 total = (Decimal)Math.Round(Total, 2)
						 });
			return Query.OrderBy(a=>a.unitName);
		}

		public MemoryStream GenerateExcelTotalPurchaseByUnits(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByUnitsReportQuery(dateFrom, dateTo, offset);
			DataTable result = new DataTable();

			result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Rp", DataType = typeof(Decimal) });
			result.Columns.Add(new DataColumn() { ColumnName = "%", DataType = typeof(Decimal) });

			decimal Total = 0;
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("", "", 0, 0); // to allow column name to be generated properly for empty data as template
			else
			{
				int index = 0;
				foreach (var item in Query)
				{
					index++;
					Total = item.total;
					result.Rows.Add(index, item.divisionName, (Decimal)Math.Round((item.amount), 2), (Decimal)Math.Round((item.amount / item.total), 2));
				}
				result.Rows.Add("", "Total Pembelian", Total, 100);
			}

			return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
		}
		#endregion
		#region ByUnitCategories
		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByUnitCategoriesReportQuery(string division, string categoryId, string currencyCode, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			var Total = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.DivisionId == (string.IsNullOrWhiteSpace(division)? a.DivisionId :division)
						 && d.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? d.CategoryId : categoryId)
						 && a.CurrencyId == (string.IsNullOrWhiteSpace(currencyCode) ? a.CurrencyId : currencyCode)
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 select c.DealQuantity * c.PricePerDealUnit * a.CurrencyRate).Sum();
			var Query = (from a in dbContext.ExternalPurchaseOrders
						 join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
						 join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
						 join d in dbContext.InternalPurchaseOrders on b.POId equals d.Id
						 //Conditions
						 where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false & d.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
						 && a.DivisionId == (string.IsNullOrWhiteSpace(division) ? a.DivisionId : division)
						 && d.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? d.CategoryId :categoryId)
						 && a.CurrencyId == (string.IsNullOrWhiteSpace(currencyCode) ? a.CurrencyId : currencyCode)
						 && a.OrderDate.AddHours(offset).Date >= DateFrom.Date 
						 && a.OrderDate.AddHours(offset).Date <= DateTo.Date
						 group new { DealQuantity = c.DealQuantity, PricePerDealUnit = c.PricePerDealUnit ,Rate=a.CurrencyRate} by new { d.CategoryName ,a.UnitName,a.CurrencyCode,a.DivisionName} into G
						 select new TotalPurchaseBySupplierViewModel
						 {
							 categoryName = G.Key.CategoryName,
							 divisionName=G.Key.DivisionName,
							 unitName=G.Key.UnitName,
							 currencyCode=G.Key.CurrencyCode,
							 amountIDR= (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate ), 2),
							 amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit), 2),
							 total = (Decimal)Math.Round(Total, 2)
						 });
			return Query;
		}

		public IQueryable<TotalPurchaseBySupplierViewModel> GetTotalPurchaseByUnitCategoriesReport(string divisionId, string categoryId, string currencyCode, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByUnitCategoriesReportQuery( divisionId,  categoryId,  currencyCode, dateFrom, dateTo, offset);
			Query = Query.OrderBy(b => b.divisionName).ThenBy(b=>b.unitName).ThenBy(b=>b.categoryName);
			return Query;
		}

		public MemoryStream GenerateExcelTotalPurchaseByUnitCategories(string divisionId,string categoryId,string currencyCode,DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetTotalPurchaseByUnitCategoriesReportQuery(divisionId, categoryId, currencyCode, dateFrom, dateTo, offset);
			DataTable result = new DataTable();

			result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Harga Per Mata Uang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Rp", DataType = typeof(Decimal) });
			result.Columns.Add(new DataColumn() { ColumnName = "%", DataType = typeof(Decimal) });

			decimal Total = 0;
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("", "","","","","", 0, 0); // to allow column name to be generated properly for empty data as template
			else
			{
				int index = 0;
				foreach (var item in Query)
				{
					index++;
					Total = item.total;
					result.Rows.Add(index,item.divisionName,item.unitName, item.categoryName,item.currencyCode, (Decimal)Math.Round((item.amount), 2), (Decimal)Math.Round((item.amountIDR), 2), (Decimal)Math.Round((item.amountIDR / item.total), 2));
				}
				result.Rows.Add("","","","","", "Total Pembelian", Total, 100);
			}

			return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
		}
		#endregion
	}
}
