using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCorrectionNoteReceptionViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteReceptionFacades
{
    public class MonitoringCorrectionNoteReceptionFacade : IMonitoringCorrectionNoteReceptionFacade
    {
        //private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public MonitoringCorrectionNoteReceptionFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        #region MonitoringCorrectionNoteReceptionAll
        public Tuple<List<MonitoringCorrectionNoteReceptionViewModel>, int> GetMonitoringTerimaNKReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringTerimaNKByUserReportQuery(dateFrom, dateTo, jnsBC, offset, page, size);

            return Tuple.Create(Query, TotalCountReport);
        }
        public MemoryStream GenerateExcelMonitoringTerimaNK(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringTerimaNKByUserReportQuery(dateFrom, dateTo, jnsBC, offset, 1, int.MaxValue);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Harga", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bon Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bukti BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Klasifikasi", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Term Of Payment", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barang / Jasa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Article No", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Faktur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Faktur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO Pembelian", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | QTY Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Satuan Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Jumlah Harga", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Konversi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Qty Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Sat Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl BUM", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Konfeksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | QTY Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Satuan Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Jumlah Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Konversi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Qty Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Sat Stl Konv", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)

                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string CorrectionDate = item.CorrectionDate == new DateTime(1970, 1, 1) ? "-" : item.CorrectionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BillDate = item.BillDate == new DateTime(1970, 1, 1) ? "-" : item.BillDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDate = item.BeaCukaiDate == new DateTime(1970, 1, 1) ? "-" : item.BeaCukaiDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ArrivalDate = item.ArrivalDate == new DateTime(1970, 1, 1) ? "-" : item.ArrivalDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxDate = item.IncomeTaxDate == new DateTime(1970, 1, 1) ? "-" : item.IncomeTaxDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string INDate = item.INDate == new DateTime(1970, 1, 1) ? "-" : item.INDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ReceiptDate = item.ReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    string CorrectionQuantity = string.Format("{0:N2}", item.CorrectionQuantity);
                    string CorrectionAmount = string.Format("{0:N2}", item.CorrectionAmount);

                    string DOQuantity = string.Format("{0:N2}", item.DOQuantity);
                    string PricePerDealUnit = string.Format("{0:N2}", item.PricePerDealUnit);
                    string PriceTotal = string.Format("{0:N2}", item.PriceTotal);
                    string Conversion = string.Format("{0:N2}", item.Conversion);
                    string SmallQuantity = string.Format("{0:N2}", item.SmallQuantity);

                    string ReceiptQuantity = string.Format("{0:N2}", item.ReceiptQuantity);
                    string URNPricePerDealUnit = string.Format("{0:N2}", item.URNPricePerDealUnit);
                    string URNPriceTotal = string.Format("{0:N2}", item.URNPriceTotal);
                    string URNConversion = string.Format("{0:N2}", item.URNConversion);
                    string URNSmallQuantity = string.Format("{0:N2}", item.URNSmallQuantity);


                    result.Rows.Add(
                        index, item.CorrectionNo, CorrectionDate, item.CorrectionType, CorrectionQuantity, item.CorrectionUOMUnit, CorrectionAmount, item.BillNo, BillDate, item.PaymentBill,
                        item.CustomsType, item.BeaCukaiNo, BCDate, item.CodeRequirement, item.PaymentType, item.BuyerName, item.ProductType, item.ProductFrom, item.SupplierCode, item.SupplierName, item.Article,
                        item.RONo, item.DONo, ArrivalDate, item.InvoiceNo, item.IncomeTaxNo, IncomeTaxDate, item.EPONo, item.ProductCode, item.ProductName, item.ProductRemark, DOQuantity,
                        item.UOMUnit, PricePerDealUnit, PriceTotal, Conversion, SmallQuantity, item.SmallUOMUnit, item.InternNo, INDate, item.URNNo, ReceiptDate,
                        item.UnitName, ReceiptQuantity, item.URNUOMUnit, URNPricePerDealUnit, URNPriceTotal, URNConversion, URNSmallQuantity, item.URNSmallUOMUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }

        #endregion
        #region MonitoringCorrectionNoteReceptionByUser
        public int TotalCountReport { get; set; } = 0;
        private List<MonitoringCorrectionNoteReceptionViewModel> GetMonitoringTerimaNKByUserReportQuery(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int offset, int page, int size)
        {


            DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<MonitoringCorrectionNoteReceptionViewModel> listDO = new List<MonitoringCorrectionNoteReceptionViewModel>();

            #region join query
            var Query = (from
                         // Correction Note
                         n in dbContext.GarmentCorrectionNotes
                         join o in dbContext.GarmentCorrectionNoteItems on n.Id equals o.GCorrectionId
                         //DeliveryOrders
                         join c in dbContext.GarmentDeliveryOrderDetails on o.DODetailId equals c.Id
                         join b in dbContext.GarmentDeliveryOrderItems on c.GarmentDOItemId equals b.Id
                         join a in dbContext.GarmentDeliveryOrders on b.GarmentDOId equals a.Id
                         //BeaCukais
                         join d in dbContext.GarmentBeacukais on a.CustomsId equals d.Id
                         //External PO
                         join e in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals e.Id
                         //Internal PO
                         join f in dbContext.GarmentInternalPurchaseOrderItems on c.POItemId equals f.Id
                         join g in dbContext.GarmentInternalPurchaseOrders on f.GPOId equals g.Id
                         //Invoice
                         join p in dbContext.GarmentInvoiceDetails on c.Id equals p.DODetailId into nn
                         from DInv in nn.DefaultIfEmpty()
                         join h in dbContext.GarmentInvoiceItems on a.Id equals h.DeliveryOrderId into hh
                         from Inv in hh.DefaultIfEmpty()
                         join j in dbContext.GarmentInvoices on Inv.InvoiceId equals j.Id into jj
                         from HInv in jj.DefaultIfEmpty()
                             //Intern Note
                         join k in dbContext.GarmentInternNotes on a.InternNo equals k.INNo into kk
                         from NI in kk.DefaultIfEmpty()
                             //Unit Receipt Note
                         join m in dbContext.GarmentUnitReceiptNoteItems on c.Id equals m.DODetailId into mm
                         from IURN in mm.DefaultIfEmpty()
                         join l in dbContext.GarmentUnitReceiptNotes on IURN.URNId equals l.Id into ll
                         from URN in ll.DefaultIfEmpty()
                         where
                         n != null
                         && URN.URNType == (URN.URNType == null ? URN.URNType : "PEMBELIAN")
                         && (string.IsNullOrWhiteSpace(jnsBC) ? true : (jnsBC == "BCDL" ? d.BeacukaiNo.Substring(0, 4) == "BCDL" : d.BeacukaiNo.Substring(0, 4) != "BCDL"))
                         //&& d.BeacukaiNo.Substring(0, 4) == jnsBC  
                         && ((d1 != new DateTime(1970, 1, 1)) ? (n.CorrectionDate.Date >= d1.Date && n.CorrectionDate.Date <= d2.Date) : true)
                         && a.SupplierCode != "GDG"

                         select new SelectedId
                         {
                             CorrectionDate = n == null ? DateTimeOffset.MinValue : n.CorrectionDate,
                             CorrectionNo = n.CorrectionNo,
                             CorrectionId = n.Id,
                             CorrectionItemId = o.Id,
                             BCId = d == null ? 0 : d.Id,
                             DOId = a.Id,
                             DOItemId = b.Id,
                             DODetailId = c.Id,
                             EPOId = e.Id,
                             POId = g.Id,
                             POItemId = f.Id,
                             INNo = a.InternNo,
                             INVId = HInv == null ? 0 : HInv.Id,
                             INVItemId = Inv == null ? 0 : Inv.Id,
                             URNId = URN == null ? 0 : URN.Id,
                             URNItemId = IURN == null ? 0 : IURN.Id,
                         });
            #endregion

            TotalCountReport = Query.Distinct().OrderByDescending(o => o.CorrectionDate).Count();
            var queryResult = Query.Distinct().OrderByDescending(o => o.CorrectionDate).Skip((page - 1) * size).Take(size).ToList();
            var deliveryOrderIds = queryResult.Select(s => s.DOId).Distinct().ToList();
            var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.BillNo, s.PaymentBill, s.DOCurrencyRate, s.PaymentMethod, s.SupplierCode, s.SupplierName, s.DONo, s.ArrivalDate, s.InternNo }).ToList();
            var deliveryOrderItemIds = queryResult.Select(s => s.DOItemId).Distinct().ToList();
            var deliveryOrderItems = dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id, s.EPONo }).ToList();
            var deliveryOrderDetailIds = queryResult.Select(s => s.DODetailId).Distinct().ToList();
            var deliveryOrderDetails = dbContext.GarmentDeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.CodeRequirment, s.ProductCode, s.ProductName, s.DOQuantity, s.UomUnit, s.PricePerDealUnit, s.PriceTotal, s.Conversion, s.SmallQuantity, s.SmallUomUnit }).ToList();
            var beaCukaiIds = queryResult.Select(s => s.BCId).Distinct().ToList();
            var beaCukais = dbContext.GarmentBeacukais.Where(w => beaCukaiIds.Contains(w.Id)).Select(s => new { s.Id, s.BillNo, s.BeacukaiDate, s.CustomsType, s.BeacukaiNo }).ToList();
            var purchaseOrderExternalIds = queryResult.Select(s => s.EPOId).Distinct().ToList();
            var purchaseOrderExternals = dbContext.GarmentExternalPurchaseOrders.Where(w => purchaseOrderExternalIds.Contains(w.Id)).Select(s => new { s.Id, s.SupplierName, s.SupplierImport }).ToList();
            var purchaseOrderInternalIds = queryResult.Select(s => s.POId).Distinct().ToList();
            var purchaseOrderInternals = dbContext.GarmentInternalPurchaseOrders.Where(w => purchaseOrderInternalIds.Contains(w.Id)).Select(s => new { s.Id, s.BuyerName, s.Article, s.RONo, s.UnitName }).ToList();
            var purchaseOrderInternalItemIds = queryResult.Select(s => s.POItemId).Distinct().ToList();
            var purchaseOrderInternalItems = dbContext.GarmentInternalPurchaseOrderItems.Where(w => purchaseOrderInternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductRemark }).ToList();
            var invoiceIds = queryResult.Select(s => s.INVId).Distinct().ToList();
            var invoices = dbContext.GarmentInvoices.Where(w => invoiceIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNo, s.IncomeTaxNo, s.IncomeTaxDate }).ToList();
            var internNoteIds = queryResult.Select(s => s.INNo).Distinct().ToList();
            var internNotes = dbContext.GarmentInternNotes.Where(w => internNoteIds.Contains(w.INNo)).Select(s => new { s.Id, s.INNo, s.INDate }).ToList();
            var unitReceiptNoteIds = queryResult.Select(s => s.URNId).Distinct().ToList();
            var unitReceiptNotes = dbContext.GarmentUnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate, s.UnitName }).ToList();
            var unitReceiptNoteItemIds = queryResult.Select(s => s.URNItemId).Distinct().ToList();
            var unitReceiptNoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.UomUnit, s.PricePerDealUnit, s.Conversion, s.SmallQuantity, s.SmallUomUnit }).ToList();
            var correctionNoteIds = queryResult.Select(s => s.CorrectionId).Distinct().ToList();
            var correctionNotes = dbContext.GarmentCorrectionNotes.Where(w => correctionNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.CorrectionNo, s.CorrectionDate, s.CorrectionType, s.CurrencyCode }).ToList();
            var correctionNoteItemIds = queryResult.Select(s => s.CorrectionItemId).Distinct().ToList();
            var correctionNoteItems = dbContext.GarmentCorrectionNoteItems.Where(w => correctionNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Quantity, s.UomIUnit, s.PriceTotalBefore, s.PriceTotalAfter }).ToList();


            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var purchaseOrderInternal = purchaseOrderInternals.FirstOrDefault(f => f.Id.Equals(item.POId));
                var purchaseOrderInternalItem = purchaseOrderInternalItems.FirstOrDefault(f => f.Id.Equals(item.POItemId));
                var purchaseOrderExternal = purchaseOrderExternals.FirstOrDefault(f => f.Id.Equals(item.EPOId));
                var deliveryOrder = deliveryOrders.FirstOrDefault(f => f.Id.Equals(item.DOId));
                var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(f => f.Id.Equals(item.DOItemId));
                var deliveryOrderDetail = deliveryOrderDetails.FirstOrDefault(f => f.Id.Equals(item.DODetailId));
                var beaCukai = beaCukais.FirstOrDefault(f => f.Id.Equals(item.BCId));
                var invoice = invoices.FirstOrDefault(f => f.Id.Equals(item.INVId));
                var unitReceiptNote = unitReceiptNotes.FirstOrDefault(f => f.Id.Equals(item.URNId));
                var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(f => f.Id.Equals(item.URNItemId));
                var internNote = internNotes.FirstOrDefault(f => f.INNo.Equals(item.INNo));
                var correctionNote = correctionNotes.FirstOrDefault(f => f.Id.Equals(item.CorrectionId));
                var correctionNoteItem = correctionNoteItems.FirstOrDefault(f => f.Id.Equals(item.CorrectionItemId));

                MonitoringCorrectionNoteReceptionViewModel monitoringcorrectionnotereceptionViewModel = new MonitoringCorrectionNoteReceptionViewModel();

                monitoringcorrectionnotereceptionViewModel.index = i;
                monitoringcorrectionnotereceptionViewModel.CorrectionNo = correctionNote.CorrectionNo;
                monitoringcorrectionnotereceptionViewModel.CorrectionDate = correctionNote.CorrectionDate;
                monitoringcorrectionnotereceptionViewModel.CorrectionType = correctionNote.CorrectionType;
                monitoringcorrectionnotereceptionViewModel.CorrectionQuantity = correctionNote.CorrectionType == "Jumlah" || correctionNote.CorrectionType == "Retur" ? correctionNoteItem.Quantity : 0;
                monitoringcorrectionnotereceptionViewModel.CorrectionUOMUnit = correctionNoteItem.UomIUnit;
                monitoringcorrectionnotereceptionViewModel.CorrectionAmount = correctionNote.CorrectionType == "Jumlah" || correctionNote.CorrectionType == "Retur" ? (decimal)deliveryOrder.DOCurrencyRate * correctionNoteItem.PriceTotalAfter : (decimal)deliveryOrder.DOCurrencyRate * (correctionNoteItem.PriceTotalAfter - correctionNoteItem.PriceTotalBefore);
                monitoringcorrectionnotereceptionViewModel.BillNo = deliveryOrder.BillNo;
                monitoringcorrectionnotereceptionViewModel.PaymentBill = deliveryOrder.PaymentBill;
                monitoringcorrectionnotereceptionViewModel.BillDate = beaCukai == null ? new DateTime(1970, 1, 1) : beaCukai.BeacukaiDate;
                monitoringcorrectionnotereceptionViewModel.CustomsType = beaCukai == null ? "-" : beaCukai.CustomsType;
                monitoringcorrectionnotereceptionViewModel.BeaCukaiNo = beaCukai == null ? "-" : beaCukai.BeacukaiNo;
                monitoringcorrectionnotereceptionViewModel.BeaCukaiDate = beaCukai == null ? new DateTime(1970, 1, 1) : beaCukai.BeacukaiDate;
                monitoringcorrectionnotereceptionViewModel.PaymentType = deliveryOrder.PaymentMethod;
                monitoringcorrectionnotereceptionViewModel.CodeRequirement = deliveryOrderDetail.CodeRequirment;
                monitoringcorrectionnotereceptionViewModel.BuyerName = purchaseOrderInternal.BuyerName;
                monitoringcorrectionnotereceptionViewModel.ProductType = beaCukai.CustomsType == "BC 262" ? "JASA" : "BARANG";
                monitoringcorrectionnotereceptionViewModel.ProductFrom = purchaseOrderExternal.SupplierImport ? "IMPORT" : "LOKAL";
                monitoringcorrectionnotereceptionViewModel.SupplierCode = deliveryOrder.SupplierCode;
                monitoringcorrectionnotereceptionViewModel.SupplierName = deliveryOrder.SupplierName;
                monitoringcorrectionnotereceptionViewModel.Article = purchaseOrderInternal.Article;
                monitoringcorrectionnotereceptionViewModel.RONo = purchaseOrderInternal.RONo;
                monitoringcorrectionnotereceptionViewModel.DONo = deliveryOrder.DONo;
                monitoringcorrectionnotereceptionViewModel.ArrivalDate = deliveryOrder.ArrivalDate;
                monitoringcorrectionnotereceptionViewModel.InvoiceNo = invoice == null ? "" : invoice.InvoiceNo;
                monitoringcorrectionnotereceptionViewModel.IncomeTaxNo = invoice == null ? "" : invoice.IncomeTaxNo;
                monitoringcorrectionnotereceptionViewModel.IncomeTaxDate = invoice == null ? new DateTime(1970, 1, 1) : invoice.IncomeTaxDate;
                monitoringcorrectionnotereceptionViewModel.EPONo = deliveryOrderDetail.POSerialNumber;
                monitoringcorrectionnotereceptionViewModel.ProductCode = deliveryOrderDetail.ProductCode;
                monitoringcorrectionnotereceptionViewModel.ProductName = deliveryOrderDetail.ProductName;
                monitoringcorrectionnotereceptionViewModel.ProductRemark = purchaseOrderInternalItem.ProductRemark;
                monitoringcorrectionnotereceptionViewModel.DOQuantity = deliveryOrderDetail.DOQuantity;
                monitoringcorrectionnotereceptionViewModel.UOMUnit = deliveryOrderDetail.UomUnit;
                monitoringcorrectionnotereceptionViewModel.PricePerDealUnit = Math.Round((double)deliveryOrder.DOCurrencyRate * deliveryOrderDetail.PricePerDealUnit, 2);
                monitoringcorrectionnotereceptionViewModel.PriceTotal = Math.Round((double)deliveryOrder.DOCurrencyRate * deliveryOrderDetail.PriceTotal, 2);
                monitoringcorrectionnotereceptionViewModel.Conversion = deliveryOrderDetail.Conversion;
                monitoringcorrectionnotereceptionViewModel.SmallQuantity = deliveryOrderDetail.SmallQuantity;
                monitoringcorrectionnotereceptionViewModel.SmallUOMUnit = deliveryOrderDetail.SmallUomUnit;
                monitoringcorrectionnotereceptionViewModel.InternNo = deliveryOrder.InternNo;
                monitoringcorrectionnotereceptionViewModel.INDate = internNote == null ? new DateTime(1970, 1, 1) : internNote.INDate;
                monitoringcorrectionnotereceptionViewModel.URNNo = unitReceiptNote == null ? "-" : unitReceiptNote.URNNo;
                monitoringcorrectionnotereceptionViewModel.ReceiptDate = unitReceiptNote == null ? new DateTime(1970, 1, 1) : unitReceiptNote.ReceiptDate;
                monitoringcorrectionnotereceptionViewModel.UnitName = unitReceiptNote == null ? "-" : unitReceiptNote.UnitName;
                monitoringcorrectionnotereceptionViewModel.ReceiptQuantity = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.ReceiptQuantity;
                monitoringcorrectionnotereceptionViewModel.URNUOMUnit = unitReceiptNoteItem == null ? "-" : unitReceiptNoteItem.UomUnit;
                monitoringcorrectionnotereceptionViewModel.URNPricePerDealUnit = unitReceiptNoteItem == null ? 0 : Math.Round((decimal)deliveryOrder.DOCurrencyRate * unitReceiptNoteItem.PricePerDealUnit, 2);
                monitoringcorrectionnotereceptionViewModel.URNPriceTotal = unitReceiptNoteItem == null ? 0 : Math.Round((decimal)deliveryOrder.DOCurrencyRate * unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity, 2);
                monitoringcorrectionnotereceptionViewModel.URNConversion = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.Conversion;
                monitoringcorrectionnotereceptionViewModel.URNSmallQuantity = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.SmallQuantity;
                monitoringcorrectionnotereceptionViewModel.URNSmallUOMUnit = unitReceiptNoteItem == null ? "-" : unitReceiptNoteItem.SmallUomUnit;

                listDO.Add(monitoringcorrectionnotereceptionViewModel);
                i++;
            }
            return listDO;
        }


        public Tuple<List<MonitoringCorrectionNoteReceptionViewModel>, int> GetMonitoringTerimaNKByUserReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Data = GetMonitoringTerimaNKByUserReportQuery(dateFrom, dateTo, jnsBC, offset, page, size);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            return Tuple.Create(Data, TotalCountReport);
        }

        public MemoryStream GenerateExcelMonitoringTerimaNKByUser(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringTerimaNKByUserReportQuery(dateFrom, dateTo, jnsBC, offset, 1, int.MaxValue);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No. Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Harga", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bon Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bukti BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Klasifikasi", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Term Of Payment", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barang / Jasa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Article No", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Faktur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Faktur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO Pembelian", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | QTY Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Satuan Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Jumlah Harga", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Konversi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Qty Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PUSAT | Sat Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl BUM", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Konfeksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | QTY Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Satuan Sbl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Jumlah Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Konversi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Qty Stl Konv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KONF | Sat Stl Konv", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string CorrectionDate = item.CorrectionDate == new DateTime(1970, 1, 1) ? "-" : item.CorrectionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BillDate = item.BillDate == new DateTime(1970, 1, 1) ? "-" : item.BillDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDate = item.BeaCukaiDate == new DateTime(1970, 1, 1) ? "-" : item.BeaCukaiDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ArrivalDate = item.ArrivalDate == new DateTime(1970, 1, 1) ? "-" : item.ArrivalDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxDate = item.IncomeTaxDate == new DateTime(1970, 1, 1) ? "-" : item.IncomeTaxDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string INDate = item.INDate == new DateTime(1970, 1, 1) ? "-" : item.INDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ReceiptDate = item.ReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    string CorrectionQuantity = string.Format("{0:N2}", item.CorrectionQuantity);
                    string CorrectionAmount = string.Format("{0:N2}", item.CorrectionAmount);

                    string DOQuantity = string.Format("{0:N2}", item.DOQuantity);
                    string PricePerDealUnit = string.Format("{0:N2}", item.PricePerDealUnit);
                    string PriceTotal = string.Format("{0:N2}", item.PriceTotal);
                    string Conversion = string.Format("{0:N2}", item.Conversion);
                    string SmallQuantity = string.Format("{0:N2}", item.SmallQuantity);

                    string ReceiptQuantity = string.Format("{0:N2}", item.ReceiptQuantity);
                    string URNPricePerDealUnit = string.Format("{0:N2}", item.URNPricePerDealUnit);
                    string URNPriceTotal = string.Format("{0:N2}", item.URNPriceTotal);
                    string URNConversion = string.Format("{0:N2}", item.URNConversion);
                    string URNSmallQuantity = string.Format("{0:N2}", item.URNSmallQuantity);

                    result.Rows.Add(
                        index, item.CorrectionNo, CorrectionDate, item.CorrectionType, CorrectionQuantity, item.CorrectionUOMUnit, CorrectionAmount, item.BillNo, BillDate, item.PaymentBill,
                        item.CustomsType, item.BeaCukaiNo, BCDate, item.CodeRequirement, item.PaymentType, item.BuyerName, item.ProductType, item.ProductFrom, item.SupplierCode, item.SupplierName, item.Article,
                        item.RONo, item.DONo, ArrivalDate, item.InvoiceNo, item.IncomeTaxNo, IncomeTaxDate, item.EPONo, item.ProductCode, item.ProductName, item.ProductRemark, DOQuantity,
                        item.UOMUnit, PricePerDealUnit, PriceTotal, Conversion, SmallQuantity, item.SmallUOMUnit, item.InternNo, INDate, item.URNNo, ReceiptDate,
                        item.UnitName, ReceiptQuantity, item.URNUOMUnit, URNPricePerDealUnit, URNPriceTotal, URNConversion, URNSmallQuantity, item.URNSmallUOMUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

    }

    public class SelectedId
    {
        public string CorrectionNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public string BillNo { get; set; }
        public DateTimeOffset BillDate { get; set; }
        public long POId { get; set; }
        public long POItemId { get; set; }
        public long EPOId { get; set; }
        public long BCId { get; set; }
        public string INNo { get; set; }
        public long DOId { get; set; }
        public long DOItemId { get; set; }
        public long DODetailId { get; set; }
        public long URNId { get; set; }
        public long URNItemId { get; set; }
        public long INVId { get; set; }
        public long INVItemId { get; set; }
        public long CorrectionId { get; set; }
        public long CorrectionItemId { get; set; }


    }
}
