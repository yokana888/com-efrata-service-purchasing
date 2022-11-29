using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCentralBillExpenditureViewModel;
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

namespace Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCentralBillExpenditureFacades
{
    public class MonitoringCentralBillExpenditureFacade : IMonitoringCentralBillExpenditureFacade
    {
        //private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public MonitoringCentralBillExpenditureFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        #region MonitoringCentralBillExpenditureAll
        public Tuple<List<MonitoringCentralBillExpenditureViewModel>, int> GetMonitoringKeluarBonPusatReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringKeluarBonPusatByUserReportQuery(dateFrom, dateTo, jnsBC, offset, page, size);

            return Tuple.Create(Query, TotalCountReport);
        }
        public MemoryStream GenerateExcelMonitoringKeluarBonPusat(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringKeluarBonPusatByUserReportQuery(dateFrom, dateTo, jnsBC, offset, 1, int.MaxValue);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
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
                result.Rows.Add("", "", "", "", "", "", "", "","", "", "", "", "", "", "", "", "", "", "", "", "", "","", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string BillDate = item.BillDate == new DateTime(1970, 1, 1) ? "-" : item.BillDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDate = item.BeaCukaiDate == new DateTime(1970, 1, 1) ? "-" : item.BeaCukaiDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ArrivalDate = item.ArrivalDate == new DateTime(1970, 1, 1) ? "-" : item.ArrivalDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxDate = item.IncomeTaxDate == new DateTime(1970, 1, 1) ? "-" : item.IncomeTaxDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string INDate = item.INDate == new DateTime(1970, 1, 1) ? "-" : item.INDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ReceiptDate = item.ReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

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
                        index, item.BillNo, BillDate, item.PaymentBill, item.CustomsType, item.BeaCukaiNo, BCDate, item.CodeRequirement, item.PaymentType, item.BuyerName, item.ProductType, item.ProductFrom,
                        item.SupplierCode, item.SupplierName, item.Article, item.RONo, item.DONo, ArrivalDate, item.InvoiceNo, item.IncomeTaxNo, IncomeTaxDate, item.EPONo, item.ProductCode,
                        item.ProductName, item.ProductRemark, DOQuantity, item.UOMUnit, PricePerDealUnit, PriceTotal, Conversion, SmallQuantity, item.SmallUOMUnit,
                        item.InternNo, INDate, item.URNNo, ReceiptDate, item.UnitName, ReceiptQuantity, item.URNUOMUnit, URNPricePerDealUnit, URNPriceTotal, URNConversion, URNSmallQuantity, item.URNSmallUOMUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }

        #endregion
        #region MonitoringCentralBillExpenditureByUser
        public int TotalCountReport { get; set; } = 0;
        private List<MonitoringCentralBillExpenditureViewModel> GetMonitoringKeluarBonPusatByUserReportQuery(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int offset, int page, int size)
        {

            //DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : dateFrom.GetValueOrDefault();
            //DateTime d2 = dateTo == null ? DateTime.Now : dateTo.GetValueOrDefault();

            DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            offset = 7;

            List<MonitoringCentralBillExpenditureViewModel> listDO = new List<MonitoringCentralBillExpenditureViewModel>();

            #region join query
            var Query = (from
                        //DeliveryOrders
                         a in dbContext.GarmentDeliveryOrders
                         join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                         join c in dbContext.GarmentDeliveryOrderDetails on b.Id equals c.GarmentDOItemId                          
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
                         URN != null && URN.URNType == "PEMBELIAN"
                         && (string.IsNullOrWhiteSpace(jnsBC) ? true : (jnsBC == "BCDL" ? d.BeacukaiNo.Substring(0, 4) == "BCDL" : d.BeacukaiNo.Substring(0, 4) != "BCDL"))
                         &&
                         ((d1 != new DateTime(1970, 1, 1) && URN != null) ? (URN.ReceiptDate >= d1 && URN.ReceiptDate <= d2) : true)
                                                                             
                         select new SelectedId
                         {
                             URNDate = URN == null ? DateTimeOffset.MinValue : URN.ReceiptDate,
                             URNNo = URN.URNNo,
                             BCId = d == null ? 0 : d.Id,
                             DOId = a.Id,
                             DOItemId = b.Id,
                             DODetailId = c.Id,
                             EPOId = e.Id,
                             POId = g.Id,
                             POItemId = f.Id,
                             INNo = a.InternNo, 
                             INVId = HInv  == null ? 0 : HInv.Id,
                             INVItemId = Inv == null ? 0 : Inv.Id,                            
                             URNId = URN == null ? 0 : URN.Id,
                             URNItemId = IURN == null ? 0 : IURN.Id,
                         });
            #endregion

            TotalCountReport = Query.Distinct().OrderByDescending(o => o.URNDate).Count();
            var queryResult = Query.Distinct().OrderByDescending(o => o.URNDate).Skip((page - 1) * size).Take(size).ToList();
            var deliveryOrderIds = queryResult.Select(s => s.DOId).Distinct().ToList();            
            var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.BillNo, s.PaymentBill, s.DOCurrencyRate, s.PaymentMethod, s.SupplierCode, s.SupplierName, s.DONo, s.ArrivalDate, s.InternNo }).ToList();
            var deliveryOrderItemIds = queryResult.Select(s => s.DOItemId).Distinct().ToList();
            var deliveryOrderItems = dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id, s.EPONo}).ToList();
            var deliveryOrderDetailIds = queryResult.Select(s => s.DODetailId).Distinct().ToList();
            var deliveryOrderDetails = dbContext.GarmentDeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.POSerialNumber, s.CodeRequirment, s.ProductCode, s.ProductName, s.DOQuantity, s.UomUnit, s.PricePerDealUnit, s.PriceTotal, s.Conversion, s.SmallQuantity, s.SmallUomUnit }).ToList();
            var beaCukaiIds = queryResult.Select(s => s.BCId).Distinct().ToList();
            var beaCukais = dbContext.GarmentBeacukais.Where(w => beaCukaiIds.Contains(w.Id)).Select(s => new { s.Id, s.BillNo, s.ArrivalDate, s.BeacukaiDate, s.CustomsType, s.BeacukaiNo}).ToList();
            var purchaseOrderExternalIds = queryResult.Select(s => s.EPOId).Distinct().ToList();
            var purchaseOrderExternals = dbContext.GarmentExternalPurchaseOrders.Where(w => purchaseOrderExternalIds.Contains(w.Id)).Select(s => new { s.Id, s.SupplierName, s.SupplierImport}).ToList();
            var purchaseOrderInternalIds = queryResult.Select(s => s.POId).Distinct().ToList();
            var purchaseOrderInternals = dbContext.GarmentInternalPurchaseOrders.Where(w => purchaseOrderInternalIds.Contains(w.Id)).Select(s => new { s.Id, s.BuyerName, s.Article, s.RONo, s.UnitName }).ToList();
            var purchaseOrderInternalItemIds = queryResult.Select(s => s.POItemId).Distinct().ToList();
            var purchaseOrderInternalItems = dbContext.GarmentInternalPurchaseOrderItems.Where(w => purchaseOrderInternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductRemark}).ToList();
            var invoiceIds = queryResult.Select(s => s.INVId).Distinct().ToList();
            var invoices = dbContext.GarmentInvoices.Where(w => invoiceIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNo, s.IncomeTaxNo, s.IncomeTaxDate}).ToList();
            var internNoteIds = queryResult.Select(s => s.INNo).Distinct().ToList();
            var internNotes = dbContext.GarmentInternNotes.Where(w => internNoteIds.Contains(w.INNo)).Select(s => new { s.Id, s.INNo, s.INDate}).ToList();
            var unitReceiptNoteIds = queryResult.Select(s => s.URNId).Distinct().ToList();
            var unitReceiptNotes = dbContext.GarmentUnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate, s.UnitName }).ToList();
            var unitReceiptNoteItemIds = queryResult.Select(s => s.URNItemId).Distinct().ToList();
            var unitReceiptNoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.UomUnit, s.PricePerDealUnit, s.Conversion, s.SmallQuantity, s.SmallUomUnit}).ToList();

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

                MonitoringCentralBillExpenditureViewModel monitoringcentralbillexpenditureViewModel = new MonitoringCentralBillExpenditureViewModel();

                monitoringcentralbillexpenditureViewModel.index = i;
                monitoringcentralbillexpenditureViewModel.BillNo = deliveryOrder.BillNo;
                monitoringcentralbillexpenditureViewModel.PaymentBill = deliveryOrder.PaymentBill;            
                monitoringcentralbillexpenditureViewModel.BillDate = beaCukai.ArrivalDate == null ? beaCukai.BeacukaiDate : beaCukai.ArrivalDate.GetValueOrDefault();
                monitoringcentralbillexpenditureViewModel.CustomsType = beaCukai == null ? "-" : beaCukai.CustomsType;
                monitoringcentralbillexpenditureViewModel.BeaCukaiNo = beaCukai == null ? "-" : beaCukai.BeacukaiNo;
                monitoringcentralbillexpenditureViewModel.BeaCukaiDate = beaCukai == null ? new DateTime(1970, 1, 1) : beaCukai.BeacukaiDate;
                monitoringcentralbillexpenditureViewModel.PaymentType = deliveryOrder.PaymentMethod;
                monitoringcentralbillexpenditureViewModel.CodeRequirement = deliveryOrderDetail.CodeRequirment;
                monitoringcentralbillexpenditureViewModel.BuyerName = purchaseOrderInternal.BuyerName;
                monitoringcentralbillexpenditureViewModel.ProductType = beaCukai.CustomsType == "BC 262" ? "JASA" : "BARANG";
                monitoringcentralbillexpenditureViewModel.ProductFrom = purchaseOrderExternal.SupplierImport ? "IMPORT" : "LOKAL";
                monitoringcentralbillexpenditureViewModel.SupplierCode = deliveryOrder.SupplierCode;
                monitoringcentralbillexpenditureViewModel.SupplierName = deliveryOrder.SupplierName;
                monitoringcentralbillexpenditureViewModel.Article = purchaseOrderInternal.Article;
                monitoringcentralbillexpenditureViewModel.RONo = purchaseOrderInternal.RONo;
                monitoringcentralbillexpenditureViewModel.DONo = deliveryOrder.DONo;
                monitoringcentralbillexpenditureViewModel.ArrivalDate = deliveryOrder.ArrivalDate;
                monitoringcentralbillexpenditureViewModel.InvoiceNo = invoice == null ? "" : invoice.InvoiceNo;
                monitoringcentralbillexpenditureViewModel.IncomeTaxNo = invoice == null ? "" : invoice.IncomeTaxNo;
                monitoringcentralbillexpenditureViewModel.IncomeTaxDate = invoice == null ? new DateTime(1970, 1, 1) : invoice.IncomeTaxDate;
                monitoringcentralbillexpenditureViewModel.EPONo = deliveryOrderDetail.POSerialNumber;
                monitoringcentralbillexpenditureViewModel.ProductCode = deliveryOrderDetail.ProductCode;
                monitoringcentralbillexpenditureViewModel.ProductName = deliveryOrderDetail.ProductName;
                monitoringcentralbillexpenditureViewModel.ProductRemark = purchaseOrderInternalItem.ProductRemark;
                monitoringcentralbillexpenditureViewModel.DOQuantity = deliveryOrderDetail.DOQuantity;
                monitoringcentralbillexpenditureViewModel.UOMUnit = deliveryOrderDetail.UomUnit;
                monitoringcentralbillexpenditureViewModel.PricePerDealUnit = (double)deliveryOrder.DOCurrencyRate * deliveryOrderDetail.PricePerDealUnit;
                monitoringcentralbillexpenditureViewModel.PriceTotal = (double)deliveryOrder.DOCurrencyRate * deliveryOrderDetail.PriceTotal;
                monitoringcentralbillexpenditureViewModel.Conversion = deliveryOrderDetail.Conversion;
                monitoringcentralbillexpenditureViewModel.SmallQuantity = deliveryOrderDetail.SmallQuantity;
                monitoringcentralbillexpenditureViewModel.SmallUOMUnit = deliveryOrderDetail.SmallUomUnit;
                monitoringcentralbillexpenditureViewModel.InternNo = deliveryOrder.InternNo;
                monitoringcentralbillexpenditureViewModel.INDate = internNote == null ? new DateTime(1970, 1, 1) : internNote.INDate;
                monitoringcentralbillexpenditureViewModel.URNNo = unitReceiptNote == null ? "-" : unitReceiptNote.URNNo;
                monitoringcentralbillexpenditureViewModel.ReceiptDate = unitReceiptNote == null ? new DateTime(1970, 1, 1) : unitReceiptNote.ReceiptDate;
                monitoringcentralbillexpenditureViewModel.UnitName = unitReceiptNote == null ? "-" : unitReceiptNote.UnitName;
                monitoringcentralbillexpenditureViewModel.ReceiptQuantity = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.ReceiptQuantity;
                monitoringcentralbillexpenditureViewModel.URNUOMUnit = unitReceiptNoteItem == null ? "-" : unitReceiptNoteItem.UomUnit;
                monitoringcentralbillexpenditureViewModel.URNPricePerDealUnit = unitReceiptNoteItem == null ? 0 : (decimal)deliveryOrder.DOCurrencyRate * unitReceiptNoteItem.PricePerDealUnit;
                monitoringcentralbillexpenditureViewModel.URNPriceTotal = unitReceiptNoteItem == null ? 0 : (decimal)deliveryOrder.DOCurrencyRate * unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity;
                monitoringcentralbillexpenditureViewModel.URNConversion = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.Conversion;
                monitoringcentralbillexpenditureViewModel.URNSmallQuantity = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.SmallQuantity;
                monitoringcentralbillexpenditureViewModel.URNSmallUOMUnit = unitReceiptNoteItem == null ? "-" : unitReceiptNoteItem.SmallUomUnit;

                listDO.Add(monitoringcentralbillexpenditureViewModel);
                i++;
            }
            return listDO;
        }


        public Tuple<List<MonitoringCentralBillExpenditureViewModel>, int> GetMonitoringKeluarBonPusatByUserReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Data = GetMonitoringKeluarBonPusatByUserReportQuery(dateFrom, dateTo, jnsBC, offset, page, size);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
    
            return Tuple.Create(Data, TotalCountReport);
        }

        public MemoryStream GenerateExcelMonitoringKeluarBonPusatByUser(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset)
        {
            var Query = GetMonitoringKeluarBonPusatByUserReportQuery(dateFrom, dateTo, jnsBC, offset, 1, int.MaxValue);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
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
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;

                    string BillDate = item.BillDate == new DateTime(1970, 1, 1) ? "-" : item.BillDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDate = item.BeaCukaiDate == new DateTime(1970, 1, 1) ? "-" : item.BeaCukaiDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ArrivalDate = item.ArrivalDate == new DateTime(1970, 1, 1) ? "-" : item.ArrivalDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxDate = item.IncomeTaxDate == new DateTime(1970, 1, 1) ? "-" : item.IncomeTaxDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string INDate = item.INDate == new DateTime(1970, 1, 1) ? "-" : item.INDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ReceiptDate = item.ReceiptDate == new DateTime(1970, 1, 1) ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

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
                        index, item.BillNo, BillDate, item.PaymentBill, item.CustomsType, item.BeaCukaiNo, BCDate, item.CodeRequirement, item.PaymentType, item.BuyerName, item.ProductType, item.ProductFrom,
                        item.SupplierCode, item.SupplierName, item.Article, item.RONo, item.DONo, ArrivalDate, item.InvoiceNo, item.IncomeTaxNo, IncomeTaxDate, item.EPONo, item.ProductCode,
                        item.ProductName, item.ProductRemark, DOQuantity, item.UOMUnit, PricePerDealUnit, PriceTotal, Conversion, SmallQuantity, item.SmallUOMUnit,
                        item.InternNo, INDate, item.URNNo, ReceiptDate, item.UnitName, ReceiptQuantity, item.URNUOMUnit, URNPricePerDealUnit, URNPriceTotal, URNConversion, URNSmallQuantity, item.URNSmallUOMUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

    }

    public class SelectedId
    {
        public DateTimeOffset URNDate { get; set; }
        public long POId { get; set; }
        public long POItemId { get; set; }
        public long EPOId { get; set; }
        public string URNNo { get; set; }
        public long BCId { get; set; }
        public string INNo { get; set; }
        public long DOId { get; set; }
        public long DOItemId { get; set; }
        public long DODetailId { get; set; }
        public long URNId { get; set; }
        public long URNItemId { get; set; }
        public long INVId { get; set; }
        public long INVItemId { get; set; }
    }
}
