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

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades.Reports
{
    public class TopTenGarmentPurchaseFacade : IGarmentTopTenPurchaseSupplier
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentExternalPurchaseOrder> dbSet;

        public TopTenGarmentPurchaseFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentExternalPurchaseOrder>();
        }
       
        public IQueryable<TopTenGarmenPurchasebySupplierViewModel> GetTotalGarmentPurchaseBySupplierReportQuery(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            //var Total = (from a in dbContext.GarmentExternalPurchaseOrders
            //             join b in dbContext.GarmentExternalPurchaseOrderItems on a.Id equals b.GarmentEPOId
            //             join c in dbContext.GarmentInternalPurchaseOrders on b.POId equals c.Id
            //             where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true
            //             && c.UnitId == (string.IsNullOrWhiteSpace(unit) ? c.UnitId : unit)
            //             && a.SupplierImport == jnsSpl
            //             && a.PaymentMethod == (string.IsNullOrWhiteSpace(payMtd) ? a.PaymentMethod : payMtd)
            //             && (string.IsNullOrWhiteSpace(category) ? true : (category == "BAHAN PENDUKUNG" ? (b.ProductName != "FABRIC" && b.ProductName != "INTERLINING") : b.ProductName == category))
            //             && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
            //             && a.OrderDate.AddHours(offset).Date <= DateTo.Date
            //             select b.DealQuantity * b.PricePerDealUnit * a.CurrencyRate).Sum();



            var Query1 = (from a in dbContext.GarmentExternalPurchaseOrders
                         join b in dbContext.GarmentExternalPurchaseOrderItems on a.Id equals b.GarmentEPOId
                         join c in dbContext.GarmentInternalPurchaseOrders on b.POId equals c.Id
                         where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && a.IsCanceled == false && a.IsPosted == true &&
                         b.DealQuantity != 0
                         && c.UnitId == (string.IsNullOrWhiteSpace(unit) ? c.UnitId : unit)
                         && a.SupplierImport == jnsSpl//(jnsSpl.HasValue ? jnsSpl : a.SupplierImport)
                         && a.PaymentMethod == (string.IsNullOrWhiteSpace(payMtd) ? a.PaymentMethod : payMtd)
                         && (string.IsNullOrWhiteSpace(category) ? true : (category == "BAHAN PENDUKUNG" ? (b.ProductName != "FABRIC" && b.ProductName != "INTERLINING") : b.ProductName == category))
                         && a.OrderDate.AddHours(offset).Date >= DateFrom.Date
                         && a.OrderDate.AddHours(offset).Date <= DateTo.Date
                         && a.SupplierCode != "GDG"
                         group new { DealQuantity = b.DealQuantity, PricePerDealUnit = b.PricePerDealUnit, Rate = a.CurrencyRate } by new { a.SupplierName, c.UnitName, b.ProductName, a.CurrencyCode, b.DealUomUnit, a.PaymentMethod } into G
                         select new
                         {
                             SupplierName = G.Key.SupplierName,
                             Amount = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit), 2),
                             Currencycode = G.Key.CurrencyCode,
                             Amount1 = (Decimal)Math.Round(G.Sum(c => c.DealQuantity * c.PricePerDealUnit * c.Rate), 2),
                             ProductName = G.Key.ProductName == "FABRIC" ? "BAHAN BAKU" : G.Key.ProductName == "INTERLINING" ? "INTERLINING" : "BAHAN PENDUKUNG" 
                         });

            var Query = (from a in Query1
                         group a by new { a.SupplierName, a.ProductName } into G
                         orderby G.Sum(a => a.Amount1) descending
                         select new TopTenGarmenPurchasebySupplierViewModel
                         {
                             SupplierName = G.Key.SupplierName,
                             Amount = G.Sum(a => a.Amount),
                             CurrencyCode = G.FirstOrDefault().Currencycode,
                             AmountIDR = G.Sum(a => a.Amount1),
                             ProductName = G.FirstOrDefault().ProductName
                         }
                         );

            return Query.OrderByDescending(b => b.AmountIDR).Take(10);
        }

        public List<TopTenGarmenPurchasebySupplierViewModel> GetTopTenGarmentPurchaseSupplierReport(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetTotalGarmentPurchaseBySupplierReportQuery(unit, jnsSpl, payMtd, category, dateFrom, dateTo, offset);
           // Query = Query.OrderByDescending(b => b.Amount);
            return Query.ToList();
        }


        public MemoryStream GenerateExcelTopTenGarmentPurchaseSupplier(string unit, bool jnsSpl, string payMtd, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetTotalGarmentPurchaseBySupplierReportQuery(unit, jnsSpl, payMtd, category, dateFrom, dateTo, offset);
            DataTable result = new DataTable();

            if (jnsSpl == false)
            {
                result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
                result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
                result.Columns.Add(new DataColumn() { ColumnName = "Nominal", DataType = typeof(decimal) });
                result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bahan", DataType = typeof(String) });
            }
            else
            {
                result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
                result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
                result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
                result.Columns.Add(new DataColumn() { ColumnName = "Nominal VALAS", DataType = typeof(decimal) });
                result.Columns.Add(new DataColumn() { ColumnName = "Nominal IDR", DataType = typeof(decimal) });
                result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bahan", DataType = typeof(String) });
            }

            if (Query.ToArray().Count() == 0)
                if (jnsSpl == false)
                {
                    result.Rows.Add("", "", 0, ""); // to allow column name to be generated properly for empty data as template
                }
                else
                {
                    result.Rows.Add("", "", "", 0, 0, ""); // to allow column name to be generated properly for empty data as template
                }
            else
            {
                    int index = 0;
                    foreach (var item in Query)
                    {
                        index++;
                        if (jnsSpl == false)
                        {
                            result.Rows.Add(index, item.SupplierName, (Decimal)Math.Round((item.AmountIDR), 2), item.ProductName);
                        }
                        else
                        {
                            result.Rows.Add(index, item.SupplierName, item.CurrencyCode, (Decimal)Math.Round((item.Amount), 2), (Decimal)Math.Round((item.AmountIDR), 2), item.ProductName);
                        }
                    }
                }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }
    }
}
