
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDailyPurchasingReportViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDailyPurchasingReportFacade
{
    public class GarmentDailyPurchasingReportFacade : IGarmentDailyPurchasingReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public GarmentDailyPurchasingReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }
        #region GarmentDailyPurchasingAll
        public IEnumerable<GarmentDailyPurchasingReportViewModel> GetGarmentDailyPurchasingReportQuery(string unitName, bool supplierType, string supplierName, DateTime? dateFrom, DateTime? dateTo, string jnsbc, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            IQueryable<GarmentDailyPurchasingTempViewModel> d1 = from a in dbContext.GarmentDeliveryOrders
                                                                 join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                                                 join c in dbContext.GarmentDeliveryOrderDetails on b.Id equals c.GarmentDOItemId
                                                                 join d in dbContext.GarmentBeacukais on a.CustomsId equals d.Id
                                                                 join e in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals e.Id
                                                                 join f in dbContext.GarmentInternalPurchaseOrders on c.POId equals f.Id
                                                                 where c.DOQuantity != 0
                                                                 && c.UnitId == (string.IsNullOrWhiteSpace(unitName) ? c.UnitId : unitName)
                                                                 && e.SupplierImport == supplierType
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? a.SupplierCode.Substring(0, 2) == "DL" : a.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && d.ArrivalDate >= DateFrom.Date && d.ArrivalDate <= DateTo.Date
                                                                 && (string.IsNullOrWhiteSpace(jnsbc) ? true : (jnsbc == "BCDL" ? d.BeacukaiNo.Substring(0, 4) == "BCDL" : d.BeacukaiNo.Substring(0, 4) != "BCDL"))
                                                                 && a.SupplierCode != "GDG"

                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = a.SupplierName,
                                                                     UnitName = f.UnitName,
                                                                     BCNo = d.BeacukaiNo,
                                                                     BCType = d.CustomsType,
                                                                     NoteNo = a.BillNo,
                                                                     BonKecil = a.PaymentBill,
                                                                     DONo = a.DONo,
                                                                     INNo = a.InternNo,
                                                                     ProductName = c.ProductName,
                                                                     JnsBrg = c.CodeRequirment,
                                                                     Quantity = (decimal)c.DOQuantity,
                                                                     Satuan = c.UomUnit,
                                                                     Kurs = (double)a.DOCurrencyRate,
                                                                     Amount = c.PriceTotal,
                                                                     CurrencyCode = a.DOCurrencyCode,
                                                                     AmountIDR = c.PriceTotal * (double)a.DOCurrencyRate,
                                                                 };

            IQueryable<GarmentDailyPurchasingTempViewModel> d2 = from gc in dbContext.GarmentCorrectionNotes
                                                                 join gci in dbContext.GarmentCorrectionNoteItems on gc.Id equals gci.GCorrectionId
                                                                 join ipo in dbContext.GarmentInternalPurchaseOrders on gci.POId equals ipo.Id
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on gci.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on gci.EPOId equals epo.Id
                                                                 where gci.Quantity != 0
                                                                 && ipo.UnitId == (string.IsNullOrWhiteSpace(unitName) ? ipo.UnitId : unitName)
                                                                 && epo.SupplierImport == supplierType
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? gc.SupplierCode.Substring(0, 2) == "DL" : gc.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && gc.CorrectionDate.AddHours(offset).Date >= DateFrom.Date && gc.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                                                                 && gc.SupplierCode != "GDG"

                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = gc.SupplierName,
                                                                     UnitName = ipo.UnitName,
                                                                     BCNo = "-",
                                                                     BCType = "-",
                                                                     NoteNo = gc.CorrectionNo,
                                                                     BonKecil = gdo.PaymentBill,
                                                                     DONo = gc.DONo,
                                                                     INNo = gdo.InternNo,
                                                                     ProductName = gci.ProductName,
                                                                     JnsBrg = gdd.CodeRequirment,
                                                                     Quantity = (decimal)gci.Quantity,
                                                                     Satuan = gci.UomIUnit,
                                                                     Kurs = (double)gdo.DOCurrencyRate,
                                                                     Amount = (double)(gc.CorrectionType == "Jumlah" || gc.CorrectionType == "Retur" ? gci.PriceTotalAfter : (gci.PriceTotalAfter - gci.PriceTotalBefore)),
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     AmountIDR = (double)(gc.CorrectionType == "Jumlah" || gc.CorrectionType == "Retur" ? gci.PriceTotalAfter * (decimal)gdo.DOCurrencyRate : (gci.PriceTotalAfter - gci.PriceTotalBefore) * (decimal)gdo.DOCurrencyRate),
                                                                 };

            IQueryable<GarmentDailyPurchasingTempViewModel> d3 = from gc in dbContext.GarmentCorrectionNotes
                                                                 join gci in dbContext.GarmentCorrectionNoteItems on gc.Id equals gci.GCorrectionId
                                                                 join ipo in dbContext.GarmentInternalPurchaseOrders on gci.POId equals ipo.Id
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on gci.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on gci.EPOId equals epo.Id
                                                                 where gci.Quantity != 0
                                                                 && ipo.UnitId == (string.IsNullOrWhiteSpace(unitName) ? ipo.UnitId : unitName)
                                                                 && epo.SupplierImport == supplierType
                                                                 && gc.UseVat == true
                                                                 && gc.NKPN != null
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? gc.SupplierCode.Substring(0, 2) == "DL" : gc.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && gc.CorrectionDate.AddHours(offset).Date >= DateFrom.Date && gc.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                                                                 && gc.SupplierCode != "GDG"

                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = gc.SupplierName,
                                                                     UnitName = ipo.UnitName,
                                                                     BCNo = "-",
                                                                     BCType = "-",
                                                                     NoteNo = gc.NKPN,
                                                                     BonKecil = gdo.PaymentBill,
                                                                     DONo = gc.DONo,
                                                                     INNo = gdo.InternNo,
                                                                     ProductName = gci.ProductName,
                                                                     JnsBrg = "PPN",//gdd.CodeRequirment,
                                                                     Quantity = (decimal)gci.Quantity,
                                                                     Satuan = gci.UomIUnit,
                                                                     Kurs = (double)gdo.DOCurrencyRate,
                                                                     Amount = (double)(gci.PriceTotalAfter - gci.PriceTotalBefore) * 10 / 100,
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     AmountIDR = ((double)(gci.PriceTotalAfter - gci.PriceTotalBefore)) * (double)gdo.DOCurrencyRate * 10 / 100,
                                                                 };

            IQueryable<GarmentDailyPurchasingTempViewModel> d4 = from gc in dbContext.GarmentCorrectionNotes
                                                                 join gci in dbContext.GarmentCorrectionNoteItems on gc.Id equals gci.GCorrectionId
                                                                 join ipo in dbContext.GarmentInternalPurchaseOrders on gci.POId equals ipo.Id
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on gci.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on gci.EPOId equals epo.Id
                                                                 where gci.Quantity != 0
                                                                 && ipo.UnitId == (string.IsNullOrWhiteSpace(unitName) ? ipo.UnitId : unitName)
                                                                 && epo.SupplierImport == supplierType
                                                                 && gc.UseIncomeTax == true
                                                                 && gc.NKPH != null
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? gc.SupplierCode.Substring(0, 2) == "DL" : gc.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && gc.CorrectionDate.AddHours(offset).Date >= DateFrom.Date && gc.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                                                                 && gc.SupplierCode != "GDG"

                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = gc.SupplierName,
                                                                     UnitName = ipo.UnitName,
                                                                     BCNo = "-",
                                                                     BCType = "-",
                                                                     NoteNo = gc.NKPH,
                                                                     BonKecil = gdo.PaymentBill,
                                                                     DONo = gc.DONo,
                                                                     INNo = gdo.InternNo,
                                                                     ProductName = gci.ProductName,
                                                                     JnsBrg = "PPH", //gdd.CodeRequirment,
                                                                     Quantity = (decimal)gci.Quantity,
                                                                     Satuan = gci.UomIUnit,
                                                                     Kurs = (double)gdo.DOCurrencyRate,
                                                                     Amount = (double)(gci.PriceTotalAfter - gci.PriceTotalBefore) * ((double)gc.IncomeTaxRate / 100),
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     AmountIDR = ((double)(gci.PriceTotalAfter - gci.PriceTotalBefore) * ((double)gc.IncomeTaxRate / 100)) * (double)gdo.DOCurrencyRate * 10 / 100,
                                                                 };

            IQueryable<GarmentDailyPurchasingTempViewModel> d5 = from inv in dbContext.GarmentInvoices
                                                                 join invi in dbContext.GarmentInvoiceItems on inv.Id equals invi.InvoiceId
                                                                 join invd in dbContext.GarmentInvoiceDetails on invi.Id equals invd.InvoiceItemId
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on invd.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on invd.EPOId equals epo.Id
                                                                 join ipo in dbContext.GarmentInternalPurchaseOrders on invd.IPOId equals ipo.Id
                                                                 where invd.DOQuantity != 0
                                                                 && ipo.UnitId == (string.IsNullOrWhiteSpace(unitName) ? ipo.UnitId : unitName)
                                                                 && epo.SupplierImport == supplierType
                                                                 && inv.IsPayTax == true
                                                                 && inv.UseVat == true
                                                                 && inv.NPN != null
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? inv.SupplierCode.Substring(0, 2) == "DL" : inv.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && inv.InvoiceDate.AddHours(offset).Date >= DateFrom.Date && inv.InvoiceDate.AddHours(offset).Date <= DateTo.Date
                                                                 && inv.SupplierCode != "GDG"

                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = inv.SupplierName,
                                                                     UnitName = ipo.UnitName,
                                                                     BCNo = "-",
                                                                     BCType = "-",
                                                                     NoteNo = inv.NPN,
                                                                     BonKecil = gdo.PaymentBill,
                                                                     DONo = gdo.DONo,
                                                                     INNo = gdo.InternNo,
                                                                     ProductName = invd.ProductName,
                                                                     JnsBrg = "PPN",//gdd.CodeRequirment,
                                                                     Quantity = (decimal)invd.DOQuantity,
                                                                     Satuan = invd.UomUnit,
                                                                     Kurs = (double)gdo.DOCurrencyRate,
                                                                     Amount = invd.DOQuantity * invd.PricePerDealUnit * 10 / 100,
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     AmountIDR = (invd.DOQuantity * invd.PricePerDealUnit * 10 / 100) * (double)gdo.DOCurrencyRate * 10 / 100,
                                                                 };
            IQueryable<GarmentDailyPurchasingTempViewModel> d6 = from inv in dbContext.GarmentInvoices
                                                                 join invi in dbContext.GarmentInvoiceItems on inv.Id equals invi.InvoiceId
                                                                 join invd in dbContext.GarmentInvoiceDetails on invi.Id equals invd.InvoiceItemId
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on invd.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on invd.EPOId equals epo.Id
                                                                 join ipo in dbContext.GarmentInternalPurchaseOrders on invd.IPOId equals ipo.Id
                                                                 where invd.DOQuantity != 0
                                                                 && ipo.UnitId == (string.IsNullOrWhiteSpace(unitName) ? ipo.UnitId : unitName)
                                                                 && epo.SupplierImport == supplierType
                                                                 && inv.IsPayTax == true
                                                                 && inv.UseIncomeTax == true
                                                                 && inv.NPH != null
                                                                 && (string.IsNullOrWhiteSpace(supplierName) ? true : (supplierName == "EFRATA GARMINDO UTAMA" ? inv.SupplierCode.Substring(0, 2) == "DL" : inv.SupplierCode.Substring(0, 2) != "DL"))
                                                                 && inv.InvoiceDate.AddHours(offset).Date >= DateFrom.Date && inv.InvoiceDate.AddHours(offset).Date <= DateTo.Date
                                                                 && inv.SupplierCode != "GDG"
                                                                 select new GarmentDailyPurchasingTempViewModel
                                                                 {
                                                                     SuplName = inv.SupplierName,
                                                                     UnitName = ipo.UnitName,
                                                                     BCNo = "-",
                                                                     BCType = "-",
                                                                     NoteNo = inv.NPH,
                                                                     BonKecil = gdo.PaymentBill,
                                                                     DONo = gdo.DONo,
                                                                     INNo = gdo.InternNo,
                                                                     ProductName = invd.ProductName,
                                                                     JnsBrg = "PPH",//gdd.CodeRequirment,
                                                                     Quantity = (decimal)invd.DOQuantity,
                                                                     Satuan = invd.UomUnit,
                                                                     Kurs = (double)gdo.DOCurrencyRate,
                                                                     Amount = invd.DOQuantity * invd.PricePerDealUnit * inv.IncomeTaxRate / 100,
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     AmountIDR = (invd.DOQuantity * invd.PricePerDealUnit * inv.IncomeTaxRate / 100) * (double)gdo.DOCurrencyRate * 10 / 100,
                                                                 };
            List<GarmentDailyPurchasingTempViewModel> CombineData = d1.Union(d2).Union(d3).Union(d4).Union(d5).Union(d6).ToList();

            var Query = from data in CombineData
                        group data by new { data.SuplName, data.BCNo, data.BCType, data.NoteNo, data.BonKecil, data.DONo, data.INNo, data.UnitName, data.ProductName, data.Satuan, data.JnsBrg, data.CurrencyCode, data.Kurs } into groupData
                        select new GarmentDailyPurchasingReportViewModel
                        {
                            SupplierName = groupData.Key.SuplName,
                            UnitName = groupData.Key.UnitName,
                            BCNo = groupData.Key.BCNo,
                            BCType = groupData.Key.BCType,
                            BillNo = groupData.Key.NoteNo,
                            PaymentBill = groupData.Key.BonKecil,
                            DONo = groupData.Key.DONo,
                            InternNo = groupData.Key.INNo,
                            ProductName = groupData.Key.ProductName,
                            CodeRequirement = groupData.Key.JnsBrg,
                            UOMUnit = groupData.Key.Satuan,
                            Quantity = groupData.Sum(s => (double)s.Quantity),
                            Amount = Math.Round(groupData.Sum(s => Math.Round(s.Amount, 2)), 2),
                            CurrencyCode = groupData.Key.CurrencyCode,
                            Rate = groupData.Key.Kurs,
                            Amount6 = Math.Round(groupData.Sum(s => Math.Round((s.Amount * s.Kurs), 2)), 2)
                        };
            return Query.AsQueryable();
        }

        public Tuple<List<GarmentDailyPurchasingReportViewModel>, int> GetGDailyPurchasingReport(string unitName, bool supplierType, string supplierName, DateTime? dateFrom, DateTime? dateTo, string jnsbc, int offset)
        {
            List<GarmentDailyPurchasingReportViewModel> result = GetGarmentDailyPurchasingReportQuery(unitName, supplierType, supplierName, dateFrom, dateTo, jnsbc, offset).ToList();
            return Tuple.Create(result, result.Count);
        }

        public MemoryStream GenerateExcelGDailyPurchasingReport(string unitName, bool supplierType, string supplierName, DateTime? dateFrom, DateTime? dateTo, string jnsbc, int offset)
        {
            Tuple<List<GarmentDailyPurchasingReportViewModel>, int> Data = this.GetGDailyPurchasingReport(unitName, supplierType, supplierName, dateFrom, dateTo, jnsbc, offset);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bukti Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "DPP VLS", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "DPP IDR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bahan Embalase (Rp)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bahan Pendukung (Rp)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bahan Baku (Rp)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Proses (Rp)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPN (Rp)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPH (Rp)", DataType = typeof(String) });

            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            if (Data.Item2 == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                Dictionary<string, List<GarmentDailyPurchasingReportViewModel>> dataBySupplier = new Dictionary<string, List<GarmentDailyPurchasingReportViewModel>>();
                Dictionary<string, double> subTotalBESupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalBPSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalBBSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalPRCSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalPPNSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalPPHSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalDPPSupplier = new Dictionary<string, double>();
                Dictionary<string, double> subTotalDPP1Supplier = new Dictionary<string, double>();

                foreach (GarmentDailyPurchasingReportViewModel data in Data.Item1)
                {
                    string SupplierName = data.PaymentBill;
                    double Amount1 = 0, Amount2 = 0, Amount3 = 0, Amount4 = 0, Amount5 = 0, Amount6 = 0, Amount7 = 0, Amount8 = 0;

                    switch (data.CodeRequirement)
                    {
                        case "BE":
                            Amount1 = data.Amount6;
                            Amount2 = 0;
                            Amount3 = 0;
                            Amount4 = 0;
                            Amount5 = 0;
                            Amount6 = 0;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                        case "BP":
                            Amount1 = 0;
                            Amount2 = data.Amount6;
                            Amount3 = 0;
                            Amount4 = 0;
                            Amount5 = 0;
                            Amount6 = 0;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                        case "BB":
                            Amount1 = 0;
                            Amount2 = 0;
                            Amount3 = data.Amount6;
                            Amount4 = 0;
                            Amount5 = 0;
                            Amount6 = 0;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                        case "PPN":
                            Amount1 = 0;
                            Amount2 = 0;
                            Amount3 = 0;
                            Amount4 = 0;
                            Amount5 = data.Amount6;
                            Amount6 = 0;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                        case "PPH":
                            Amount1 = 0;
                            Amount2 = 0;
                            Amount3 = 0;
                            Amount4 = 0;
                            Amount5 = 0;
                            Amount6 = data.Amount6;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                        default:
                            Amount1 = 0;
                            Amount2 = 0;
                            Amount3 = 0;
                            Amount4 = data.Amount6;
                            Amount5 = 0;
                            Amount6 = 0;
                            Amount7 = data.Amount;
                            Amount8 = data.Amount6;
                            break;
                    }

                    //if (data.BillNo.Contains("NPN") || data.BillNo.Contains("NKPN"))
                    //{
                    //    Amount1 = 0;
                    //    Amount2 = 0;
                    //    Amount3 = 0;
                    //    Amount4 = 0;
                    //    Amount5 = data.Amount6;
                    //    Amount6 = 0;
                    //    Amount7 = data.Amount;
                    //    Amount8 = data.Amount6;
                    //}
                    //if (data.BillNo.Contains("NPH") || data.BillNo.Contains("NKPH"))
                    //{
                    //    Amount1 = 0;
                    //    Amount2 = 0;
                    //    Amount3 = 0;
                    //    Amount4 = 0;
                    //    Amount5 = 0;
                    //    Amount6 = data.Amount6;
                    //    Amount7 = data.Amount;
                    //    Amount8 = data.Amount6;
                    //}

                    if (!dataBySupplier.ContainsKey(SupplierName)) dataBySupplier.Add(SupplierName, new List<GarmentDailyPurchasingReportViewModel> { });
                    dataBySupplier[SupplierName].Add(new GarmentDailyPurchasingReportViewModel
                    {

                        SupplierName = data.SupplierName,
                        UnitName = data.UnitName,
                        BCNo = data.BCNo,
                        BCType = data.BCType,
                        BillNo = data.BillNo,
                        PaymentBill = data.PaymentBill,
                        DONo = data.DONo,
                        InternNo = data.InternNo,
                        ProductName = data.ProductName,
                        CodeRequirement = data.CodeRequirement,
                        UOMUnit = data.UOMUnit,
                        Quantity = data.Quantity,
                        CurrencyCode = data.CurrencyCode,
                        Rate = data.Rate,
                        Amount = Amount1,
                        Amount1 = Amount2,
                        Amount2 = Amount3,
                        Amount3 = Amount4,
                        Amount4 = Amount5,
                        Amount5 = Amount6,
                        Amount6 = Amount7,
                        Amount7 = Amount8,
                    });

                    if (!subTotalBESupplier.ContainsKey(SupplierName))
                    {
                        subTotalBESupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalBPSupplier.ContainsKey(SupplierName))
                    {
                        subTotalBPSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalBBSupplier.ContainsKey(SupplierName))
                    {
                        subTotalBBSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalPRCSupplier.ContainsKey(SupplierName))
                    {
                        subTotalPRCSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalPPNSupplier.ContainsKey(SupplierName))
                    {
                        subTotalPPNSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalPPHSupplier.ContainsKey(SupplierName))
                    {
                        subTotalPPHSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalDPPSupplier.ContainsKey(SupplierName))
                    {
                        subTotalDPPSupplier.Add(SupplierName, 0);
                    };

                    if (!subTotalDPP1Supplier.ContainsKey(SupplierName))
                    {
                        subTotalDPP1Supplier.Add(SupplierName, 0);
                    };

                    subTotalBESupplier[SupplierName] += Amount1;
                    subTotalBPSupplier[SupplierName] += Amount2;
                    subTotalBBSupplier[SupplierName] += Amount3;
                    subTotalPRCSupplier[SupplierName] += Amount4;
                    subTotalPPNSupplier[SupplierName] += Amount5;
                    subTotalPPHSupplier[SupplierName] += Amount6;
                    subTotalDPPSupplier[SupplierName] += Amount7;
                    subTotalDPP1Supplier[SupplierName] += Amount8;

                }

                double totalBE = 0;
                double totalBP = 0;
                double totalBB = 0;
                double totalPRC = 0;
                double totalPPN = 0;
                double totalPPH = 0;
                double totalDPP = 0;
                double totalDPP1 = 0;


                int rowPosition = 7;

                foreach (KeyValuePair<string, List<GarmentDailyPurchasingReportViewModel>> SupplName in dataBySupplier)
                {
                    string splCode = "";
                    string mtUang = "";
                    int index = 0;
                    foreach (GarmentDailyPurchasingReportViewModel data in SupplName.Value)
                    {
                        index++;
                        result.Rows.Add(index, data.SupplierName, data.UnitName, data.BillNo, data.PaymentBill, data.BCNo, data.BCType, data.DONo, data.InternNo, data.ProductName, data.Quantity, data.UOMUnit, Math.Round(data.Amount6, 2), Math.Round(data.Amount7, 2), data.CurrencyCode, data.Rate, Math.Round(data.Amount, 2), Math.Round(data.Amount1, 2), Math.Round(data.Amount2, 2), Math.Round(data.Amount3, 2), Math.Round(data.Amount4, 2), Math.Round(data.Amount5, 2));
                        rowPosition += 1;
                        splCode = data.PaymentBill;
                        mtUang = data.CurrencyCode;
                    }

                    result.Rows.Add("SUB TOTAL", "", "", "", "", "", "NO BON PUSAT KECIL", ".", ".", ":", splCode, ":", Math.Round(subTotalDPPSupplier[SupplName.Key], 2), Math.Round(subTotalDPP1Supplier[SupplName.Key], 2), mtUang, "", Math.Round(subTotalBESupplier[SupplName.Key], 2), Math.Round(subTotalBPSupplier[SupplName.Key], 2), Math.Round(subTotalBBSupplier[SupplName.Key], 2), Math.Round(subTotalPRCSupplier[SupplName.Key], 2), Math.Round(subTotalPPNSupplier[SupplName.Key], 2), Math.Round(subTotalPPHSupplier[SupplName.Key], 2));

                    rowPosition += 1;
                    mergeCells.Add(($"A{rowPosition}:D{rowPosition}", OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom));

                    totalBE += subTotalBESupplier[SupplName.Key];
                    totalBP += subTotalBPSupplier[SupplName.Key];
                    totalBB += subTotalBBSupplier[SupplName.Key];
                    totalPRC += subTotalPRCSupplier[SupplName.Key];
                    totalPPN += subTotalPPNSupplier[SupplName.Key];
                    totalPPH += subTotalPPHSupplier[SupplName.Key];
                    totalDPP += subTotalDPPSupplier[SupplName.Key];
                    totalDPP1 += subTotalDPP1Supplier[SupplName.Key];
                }

                result.Rows.Add("TOTAL    ", "", "", "", "", "", "", "", "", "", "", "", Math.Round(totalDPP1, 2), Math.Round(totalDPP, 2), "", "", Math.Round(totalBE, 2), Math.Round(totalBP, 2), Math.Round(totalBB, 2), Math.Round(totalPRC, 2), Math.Round(totalPPN, 2), Math.Round(totalPPH, 2));
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                rowPosition += 1;
                mergeCells.Add(($"A{rowPosition}:D{rowPosition}", OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom));
            }

            ExcelPackage package = new ExcelPackage();
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(DateTo.Month);
            var sheet = package.Workbook.Worksheets.Add("Report");

            #region Kop Table
            var col = (char)('A' + result.Columns.Count);
            sheet.Cells[$"A1:{col}1"].Value = "PT. EFRATA GARMINDO UTAMA";
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = "BUKU HARIAN PEMBELIAN GARMENT";
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("BULAN {0} {1}", Month, DateTo.Year);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A4:{col}4"].Value = string.Format("SUPPLIER {0}", supplierType == true ? "IMPORT" : "LOCAL");
            sheet.Cells[$"A4:{col}4"].Merge = true;
            sheet.Cells[$"A4:{col}4"].Style.Font.Bold = true;
            sheet.Cells[$"A4:{col}4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A4:{col}4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A5:{col}5"].Value = string.Format("KONFEKSI {0}", string.IsNullOrWhiteSpace(unitName) ? "ALL" : unitName);
            sheet.Cells[$"A5:{col}5"].Merge = true;
            sheet.Cells[$"A5:{col}5"].Style.Font.Bold = true;
            sheet.Cells[$"A5:{col}5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A5:{col}5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            #endregion

            foreach (var i in Enumerable.Range(0, result.Columns.Count))
            {
                var colheader = (char)('A' + i);
                sheet.Cells[$"{colheader}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            }
            sheet.Cells["A7"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);
            foreach ((string cells, Enum hAlign, Enum vAlign) in mergeCells)
            {
                sheet.Cells[cells].Merge = true;
                sheet.Cells[cells].Style.HorizontalAlignment = (OfficeOpenXml.Style.ExcelHorizontalAlignment)hAlign;
                sheet.Cells[cells].Style.VerticalAlignment = (OfficeOpenXml.Style.ExcelVerticalAlignment)vAlign;
            }
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (result, "Report", mergeCells) }, true);
        }
        #endregion
    }
}
