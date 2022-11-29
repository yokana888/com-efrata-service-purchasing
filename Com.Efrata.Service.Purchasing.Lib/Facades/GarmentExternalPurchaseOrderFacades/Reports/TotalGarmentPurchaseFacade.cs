
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel.Reports;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacade.Reports
{
	public class TotalGarmentPurchaseFacade : IGarmentTotalPurchaseOrderFacade
    {
		private readonly PurchasingDbContext dbContext;
		public readonly IServiceProvider serviceProvider;
		private readonly DbSet<GarmentExternalPurchaseOrder> dbSet;

		public TotalGarmentPurchaseFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
		{
			this.serviceProvider = serviceProvider;
			this.dbContext = dbContext;
			this.dbSet = dbContext.Set<GarmentExternalPurchaseOrder>();
		}
        #region BySupplier
        public IQueryable<TotalGarmentPurchaseBySupplierViewModel> GetTotalGarmentPurchaseBySupplierReportQuery(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Total = (from a in dbContext.GarmentExternalPurchaseOrders
                         join b in dbContext.GarmentExternalPurchaseOrderItems on a.Id equals b.GarmentEPOId
                         join c in dbContext.GarmentInternalPurchaseOrders on b.POId equals c.Id
                         where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
                         && c.UnitId == (string.IsNullOrWhiteSpace(unit) ? c.UnitId : unit)
                         && a.SupplierImport == jnsSpl
                         && a.PaymentMethod == (string.IsNullOrWhiteSpace(payMtd) ? a.PaymentMethod : payMtd)
                         && (string.IsNullOrWhiteSpace(category) ? true : (category == "BAHAN PENDUKUNG" ? (b.ProductName != "FABRIC" && b.ProductName != "INTERLINING") : b.ProductName == category))
                         && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
                         && a.OrderDate.AddHours(offset).Date <= DateTo.Date
                         select b.DealQuantity * b.PricePerDealUnit * a.CurrencyRate).Sum();

            var Query = (from a in dbContext.GarmentExternalPurchaseOrders
                         join b in dbContext.GarmentExternalPurchaseOrderItems on a.Id equals b.GarmentEPOId
                         join c in dbContext.GarmentInternalPurchaseOrders on b.POId equals c.Id
                         where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true &&
                         b.DealQuantity != 0
                         && c.UnitId == (string.IsNullOrWhiteSpace(unit) ? c.UnitId : unit)
                         && a.SupplierImport == jnsSpl
                         && a.PaymentMethod == (string.IsNullOrWhiteSpace(payMtd) ? a.PaymentMethod : payMtd)
                         && (string.IsNullOrWhiteSpace(category) ? true : (category == "BAHAN PENDUKUNG" ? (b.ProductName != "FABRIC" && b.ProductName != "INTERLINING") : b.ProductName == category))
                         && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
                         && a.OrderDate.AddHours(offset).Date <= DateTo.Date
                         group new { DealQuantity = b.DealQuantity, SmallQty = b.SmallQuantity, PricePerDealUnit = b.PricePerDealUnit, Rate = a.CurrencyRate } by new { a.SupplierName, c.UnitName, b.ProductName, a.CurrencyCode, b.DealUomUnit, b.SmallUomUnit, a.PaymentMethod } into G
                         select new TotalGarmentPurchaseBySupplierViewModel
                         {
                             SupplierName = G.Key.SupplierName,
                             UnitName = G.Key.UnitName,
                             CurrencyCode = G.Key.CurrencyCode,
                             PaymentMethod = G.Key.PaymentMethod,
                             UOMUnit = G.Key.DealUomUnit,
                             SmallUom = G.Key.SmallUomUnit,
                             CategoryName = G.Key.ProductName,
                             Quantity = (Decimal)Math.Round(G.Sum(c => c.DealQuantity), 2),
                             SmallQty = (Decimal)Math.Round(G.Sum(c => c.SmallQty), 2),
                             Amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit), 2),
                             AmountIDR = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate), 2)
                         });
            return Query;
        }

        public List<TotalGarmentPurchaseBySupplierViewModel> GetTotalGarmentPurchaseBySupplierReport(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetTotalGarmentPurchaseBySupplierReportQuery(unit, jnsSpl, payMtd, category, dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.SupplierName).ThenBy(b => b.UnitName).ThenBy(b => b.CategoryName).ThenBy(b => b.CurrencyCode).ThenBy(b => b.UOMUnit).ThenBy(b => b.PaymentMethod);
            return Query.ToList();
        }

        public MemoryStream GenerateExcelTotalGarmentPurchaseBySupplier(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetTotalGarmentPurchaseBySupplierReportQuery(unit, jnsSpl, payMtd, category, dateFrom, dateTo, offset);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Metode Bayar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(Decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Konversi", DataType = typeof(Decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nominal(Rp)", DataType = typeof(Decimal) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, "", 0, "", 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    result.Rows.Add(index, item.SupplierName, item.UnitName, item.CategoryName, item.PaymentMethod, item.Quantity, item.UOMUnit,item.SmallQty, item.SmallUom, (Decimal)Math.Round((item.AmountIDR), 2));
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }
        #endregion
    }
}
