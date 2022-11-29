using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Globalization;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO
{
    public class PurchaseOrderMonitoringAllFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<InternalPurchaseOrder> dbSet;

        public PurchaseOrderMonitoringAllFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            if(this.dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                this.dbContext.Database.SetCommandTimeout(600);
            }
            this.dbSet = dbContext.Set<InternalPurchaseOrder>();
        }

        //public IQueryable<PurchaseOrderMonitoringAllViewModel> GetReportQuery(string prNo, string supplierId, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, string status, int offset, string user)
        //{
        //    DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
        //    DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

        //    var Query = (from a in dbContext.InternalPurchaseOrders
        //                 join b in dbContext.InternalPurchaseOrderItems on a.Id equals b.POId
        //                 //PR
        //                 join d in dbContext.PurchaseRequestItems on b.PRItemId equals d.Id
        //                 join c in dbContext.PurchaseRequests on d.PurchaseRequestId equals c.Id
        //                 //EPO
        //                 join e in dbContext.ExternalPurchaseOrderItems on b.POId equals e.POId into f
        //                 from epoItem in f.DefaultIfEmpty()
        //                 join k in dbContext.ExternalPurchaseOrderDetails on b.Id equals k.POItemId into l
        //                 from epoDetail in l.DefaultIfEmpty()
        //                 join g in dbContext.ExternalPurchaseOrders on epoItem.EPOId equals g.Id into h
        //                 from epo in h.DefaultIfEmpty()
        //                     //DO
        //                 join yy in dbContext.DeliveryOrderDetails on epoDetail.Id equals yy.EPODetailId into zz
        //                 from doDetail in zz.DefaultIfEmpty()
        //                 join m in dbContext.DeliveryOrderItems on doDetail.DOItemId equals m.Id into n
        //                 from doItem in n.DefaultIfEmpty()
        //                 join o in dbContext.DeliveryOrders on doItem.DOId equals o.Id into p
        //                 from DO in p.DefaultIfEmpty()
        //                     //URN
        //                 join s in dbContext.UnitReceiptNoteItems on doDetail.Id equals s.DODetailId into t
        //                 from urnItem in t.DefaultIfEmpty()
        //                 join q in dbContext.UnitReceiptNotes on urnItem.URNId equals q.Id into r
        //                 from urn in r.DefaultIfEmpty()
        //                     //UPO
        //                 join u in dbContext.UnitPaymentOrderItems on urn.Id equals u.URNId into v
        //                 from upoItem in v.DefaultIfEmpty()
        //                 join w in dbContext.UnitPaymentOrders on upoItem.UPOId equals w.Id into x
        //                 from upo in x.DefaultIfEmpty()
        //                 join y in dbContext.UnitPaymentOrderDetails on urnItem.Id equals y.URNItemId into z
        //                 from upoDetail in z.DefaultIfEmpty()
        //                     //Correction
        //                 join cc in dbContext.UnitPaymentCorrectionNoteItems on upoDetail.Id equals cc.UPODetailId into dd
        //                 from corrItem in dd.DefaultIfEmpty()
        //                 join aa in dbContext.UnitPaymentCorrectionNotes on corrItem.UPCId equals aa.Id into bb
        //                 from corr in bb.DefaultIfEmpty()
        //                 where a.IsDeleted == false && b.IsDeleted == false
        //                     && c.IsDeleted == false
        //                     && d.IsDeleted == false
        //                     && epo.IsDeleted == false && epoDetail.IsDeleted == false && epoItem.IsDeleted == false
        //                     && DO.IsDeleted == false && doItem.IsDeleted == false && doDetail.IsDeleted == false
        //                     && urn.IsDeleted == false && urnItem.IsDeleted == false
        //                     && upo.IsDeleted == false && upoItem.IsDeleted == false && upoDetail.IsDeleted == false
        //                     && corr.IsDeleted == false && corrItem.IsDeleted == false
        //                     && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
        //                     && a.PRNo == (string.IsNullOrWhiteSpace(prNo) ? a.PRNo : prNo)
        //                     && a.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? a.CategoryId : categoryId)
        //                     && a.BudgetId == (string.IsNullOrWhiteSpace(budgetId) ? a.BudgetId : budgetId)
        //                     && epo.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? epo.SupplierId : supplierId)
        //                     && epo.EPONo == (string.IsNullOrWhiteSpace(epoNo) ? epo.EPONo : epoNo)
        //                     && b.Status == (string.IsNullOrWhiteSpace(status) ? b.Status : status)
        //                     && a.CreatedBy == (string.IsNullOrWhiteSpace(staff) ? a.CreatedBy : staff)
        //                     && a.PRDate.AddHours(offset).Date >= DateFrom.Date
        //                     && a.PRDate.AddHours(offset).Date <= DateTo.Date
        //                     && b.Quantity > 0
        //                     && a.CreatedBy == (string.IsNullOrWhiteSpace(user) ? a.CreatedBy : user)

        //                 select new PurchaseOrderMonitoringAllViewModel
        //                 {
        //                     createdDatePR = c.CreatedUtc,
        //                     prNo = a.PRNo,
        //                     prDate = a.PRDate,
        //                     category = a.CategoryName,
        //                     budget = a.BudgetName,
        //                     productName = b.ProductName,
        //                     productCode = b.ProductCode,
        //                     quantity = epoDetail != null ? epoDetail.DealQuantity : 0 ,
        //                     uom = epoDetail != null  ? epoDetail.DealUomUnit : "-" ,
        //                     pricePerDealUnit = epoDetail != null ? epo.IsPosted==true? epoDetail.PricePerDealUnit : 0 : 0,
        //                     priceTotal = epoDetail != null ? epo.IsPosted == true ? epoDetail.DealQuantity * epoDetail.PricePerDealUnit : 0 :0,
        //                     supplierCode = epo!=null ? epo.IsPosted == true?epo.SupplierCode : "-" : "-",
        //                     supplierName = epo != null ? epo.IsPosted == true ? epo.SupplierName : "-" : "-",
        //                     receivedDatePO = a.CreatedUtc,
        //                     epoDate = epo != null ? epo.IsPosted == true ? epo.OrderDate : new DateTime(1970, 1, 1) : new DateTime(1970, 1, 1) ,
        //                     epoCreatedDate = epo != null ? epo.IsPosted == true ?  epo.CreatedUtc : new DateTime(1970, 1, 1) : new DateTime(1970, 1, 1) ,
        //                     epoExpectedDeliveryDate = a.ExpectedDeliveryDate,
        //                     epoDeliveryDate = epo != null ? epo.IsPosted == true ?  epo.DeliveryDate : new DateTime(1970, 1, 1) : new DateTime(1970, 1, 1) ,
        //                     epoNo = epo != null ? epo.IsPosted == true?epo.EPONo : "-" : "-",
        //                     doDate = DO == null ? new DateTime(1970, 1, 1) : DO.DODate,
        //                     doDeliveryDate = DO == null ? new DateTime(1970, 1, 1) : DO.ArrivalDate,
        //                     doNo = DO.DONo ?? "-",
        //                     doDetailId = corrItem == null ? 0 : upoDetail.Id,
        //                     urnDate = urn == null ? new DateTime(1970, 1, 1) : urn.ReceiptDate,
        //                     urnNo = urn.URNNo ?? "-",
        //                     urnQuantity = urnItem == null ? 0 : urnItem.ReceiptQuantity,
        //                     urnUom = urnItem.Uom ?? "-",
        //                     paymentDueDays = epo != null && epo.IsPosted == true?epo.PaymentDueDays : "-",
        //                     invoiceDate = upo == null ? new DateTime(1970, 1, 1) : upo.InvoiceDate,
        //                     invoiceNo = upo.InvoiceNo ?? "-",
        //                     upoDate = upo == null ? new DateTime(1970, 1, 1) : upo.Date,
        //                     upoNo = upo.UPONo ?? "-",
        //                     upoPriceTotal = upoDetail == null ? 0 : upoDetail.PriceTotal,
        //                     dueDate = upo == null ? new DateTime(1970, 1, 1) : upo.DueDate,
        //                     vatDate = upo != null ? upo.UseVat ? upo.VatDate : new DateTime(1970, 1, 1) : new DateTime(1970, 1, 1),
        //                     vatNo = upo.VatNo ?? "-",
        //                     vatValue = upoDetail != null && upo!=null ? upo.UseVat ? 0.1 * upoDetail.PriceTotal : 0 : 0,
        //                     incomeTaxDate = upo == null && !upo.UseIncomeTax ? null : upo.IncomeTaxDate,
        //                     incomeTaxNo = upo.IncomeTaxNo ?? null,
        //                     incomeTaxValue = upoDetail != null && upo != null ? upo.UseIncomeTax ? (upo.IncomeTaxRate * upoDetail.PriceTotal / 100) : 0 : 0,
        //                     correctionDate = corr == null ? new DateTime(1970, 1, 1) : corr.CorrectionDate,
        //                     correctionNo = corr.UPCNo ?? null,
        //                     correctionType = corr.CorrectionType ?? null,
        //                     valueCorrection = corrItem == null ? 0 : corr.CorrectionType == "Harga Total" ? corrItem.PriceTotalAfter - corrItem.PriceTotalBefore : corr.CorrectionType == "Harga Satuan" ? (corrItem.PricePerDealUnitAfter - corrItem.PricePerDealUnitBefore) * corrItem.Quantity : corr.CorrectionType == "Jumlah" ? corrItem.PriceTotalAfter * -1 : 0,
        //                     priceAfter = corrItem == null ? 0 : corrItem.PricePerDealUnitAfter,
        //                     priceBefore = corrItem == null ? 0 : corrItem.PricePerDealUnitBefore,
        //                     priceTotalAfter = corrItem == null ? 0 : corrItem.PriceTotalAfter,
        //                     priceTotalBefore = corrItem == null ? 0 : corrItem.PricePerDealUnitBefore,
        //                     qtyCorrection = corrItem == null ? 0 : corrItem.Quantity,
        //                     remark = epo != null ?epo.Remark : "",
        //                     status = b.Status,
        //                     staff = a.CreatedBy,
        //                     LastModifiedUtc = a.LastModifiedUtc,
        //                 });


        //    Dictionary<long, string> qry = new Dictionary<long, string>();
        //    Dictionary<long, string> qryDate = new Dictionary<long, string>();
        //    Dictionary<long, string> qryQty = new Dictionary<long, string>();
        //    Dictionary<long, string> qryType = new Dictionary<long, string>();
        //    List<PurchaseOrderMonitoringAllViewModel> listData = new List<PurchaseOrderMonitoringAllViewModel>();

        //    var index = 0;
        //    foreach (PurchaseOrderMonitoringAllViewModel data in Query.ToList())
        //    {
        //        string value;
        //        if (data.doDetailId != 0)
        //        {
        //            string correctionDate = data.correctionDate == new DateTime(1970, 1, 1) ? "-" : data.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
        //            if (data.correctionNo != null)
        //            {
        //                if (qry.TryGetValue(data.doDetailId, out value))
        //                {
        //                    qry[data.doDetailId] += (index).ToString() + ". " + data.correctionNo + "\n";
        //                    qryType[data.doDetailId] += (index).ToString() + ". " + data.correctionType + "\n";
        //                    qryDate[data.doDetailId] += (index).ToString() + ". " + correctionDate + "\n";
        //                    qryQty[data.doDetailId] += (index).ToString() + ". " + String.Format("{0:N2}", data.valueCorrection) + "\n";
        //                    index++;
        //                }
        //                else
        //                {
        //                    index = 1;
        //                    qry[data.doDetailId] = (index).ToString() + ". " + data.correctionNo + "\n";
        //                    qryType[data.doDetailId] = (index).ToString() + ". " + data.correctionType + "\n";
        //                    qryDate[data.doDetailId] = (index).ToString() + ". " + correctionDate + "\n";
        //                    qryQty[data.doDetailId] = (index).ToString() + ". " + String.Format("{0:N2}", data.valueCorrection) + "\n";
        //                    index++;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            listData.Add(data);
        //        }

        //    }
        //    foreach(var corrections in qry)
        //    {
        //        foreach(PurchaseOrderMonitoringAllViewModel data in Query.ToList())
        //        {
        //            if( corrections.Key==data.doDetailId)
        //            {
        //                data.correctionNo = qry[data.doDetailId];
        //                data.correctionType= qryType[data.doDetailId];
        //                data.correctionQtys = qryQty[data.doDetailId];
        //                data.correctionDates = qryDate[data.doDetailId];
        //                listData.Add(data);
        //                break;
        //            }
        //        }
        //    }

        //    var op = qry;
        //    return Query=listData.AsQueryable();
        //    //return Query;

        //}

       public int TotalCountReport { get; set; } = 0;

        List<PurchaseOrderMonitoringAllViewModel> listEPO2 = new List<PurchaseOrderMonitoringAllViewModel>();

        private List<PurchaseOrderMonitoringAllViewModel> GetReportQuery(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status, int page, int size, int offset, string user)
        {
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
                DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
                DateTime DateFromPO = dateFromPO == null ? new DateTime(1970, 1, 1) : (DateTime)dateFromPO;
                DateTime DateToPO = dateToPO == null ? DateTime.Now : (DateTime)dateToPO;
                DateTime date = new DateTime(1970, 1, 1);
                offset = 7;

                List<PurchaseOrderMonitoringAllViewModel> listEPO = new List<PurchaseOrderMonitoringAllViewModel>();

                var Query = (from a in dbContext.InternalPurchaseOrders
                             join b in dbContext.InternalPurchaseOrderItems on a.Id equals b.POId
                             //PR
                             join d in dbContext.PurchaseRequestItems on b.PRItemId equals d.Id
                             join c in dbContext.PurchaseRequests on d.PurchaseRequestId equals c.Id
                             //EPO
                             join e in dbContext.ExternalPurchaseOrderItems on b.POId equals e.POId into f
                             from epoItem in f.DefaultIfEmpty()
                             join k in dbContext.ExternalPurchaseOrderDetails on b.Id equals k.POItemId into l
                             from epoDetail in l.DefaultIfEmpty()
                             join g in dbContext.ExternalPurchaseOrders on epoItem.EPOId equals g.Id into h
                             from epo in h.DefaultIfEmpty()
                                 //DO
                             join yy in dbContext.DeliveryOrderDetails on epoDetail.Id equals yy.EPODetailId into zz
                             from doDetail in zz.DefaultIfEmpty()
                             join m in dbContext.DeliveryOrderItems on doDetail.DOItemId equals m.Id into n
                             from doItem in n.DefaultIfEmpty()
                             join o in dbContext.DeliveryOrders on doItem.DOId equals o.Id into p
                             from DO in p.DefaultIfEmpty()
                                 //URN
                             join s in dbContext.UnitReceiptNoteItems on doDetail.Id equals s.DODetailId into t
                             from urnItem in t.DefaultIfEmpty()
                             join q in dbContext.UnitReceiptNotes on urnItem.URNId equals q.Id into r
                             from urn in r.DefaultIfEmpty()
                                 //UPO
                             join u in dbContext.UnitPaymentOrderItems on urn.Id equals u.URNId into v
                             from upoItem in v.DefaultIfEmpty()
                             join w in dbContext.UnitPaymentOrders on upoItem.UPOId equals w.Id into x
                             from upo in x.DefaultIfEmpty()
                             join y in dbContext.UnitPaymentOrderDetails on urnItem.Id equals y.URNItemId into z
                             from upoDetail in z.DefaultIfEmpty()
                                 //    //Correction
                                 //join cc in dbContext.UnitPaymentCorrectionNoteItems on upoDetail.Id equals cc.UPODetailId into dd
                                 //from corrItem in dd.DefaultIfEmpty()
                                 //join aa in dbContext.UnitPaymentCorrectionNotes on corrItem.UPCId equals aa.Id into bb
                                 //from corr in bb.DefaultIfEmpty()
                             where
                             a.IsDeleted == false && b.IsDeleted == false
                                 && c.IsDeleted == false
                                 && d.IsDeleted == false
                                 && epo.IsDeleted == false && epoDetail.IsDeleted == false && epoItem.IsDeleted == false
                                 && DO.IsDeleted == false && doItem.IsDeleted == false && doDetail.IsDeleted == false
                                 && urn.IsDeleted == false && urnItem.IsDeleted == false
                                 && upo.IsDeleted == false && upoItem.IsDeleted == false && upoDetail.IsDeleted == false
                                 //&& corr.IsDeleted == false && corrItem.IsDeleted == false
                                 &&
                                 a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                                 && a.PRNo == (string.IsNullOrWhiteSpace(prNo) ? a.PRNo : prNo)
                                 && a.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? a.CategoryId : categoryId)
                                 && a.BudgetId == (string.IsNullOrWhiteSpace(budgetId) ? a.BudgetId : budgetId)
                                 && a.DivisionCode == (string.IsNullOrWhiteSpace(divisionCode) ? a.DivisionCode : divisionCode)
                             && epo.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? epo.SupplierId : supplierId)
                             && epo.EPONo == (string.IsNullOrWhiteSpace(epoNo) ? epo.EPONo : epoNo)
                             && b.Status == (string.IsNullOrWhiteSpace(status) ? b.Status : status)
                             && a.CreatedBy == (string.IsNullOrWhiteSpace(staff) ? a.CreatedBy : staff)
                             && a.PRDate.AddHours(offset).Date >= DateFrom.Date
                             && a.PRDate.AddHours(offset).Date <= DateTo.Date
                             && b.Quantity > 0
                             && a.CreatedBy == (string.IsNullOrWhiteSpace(user) ? a.CreatedBy : user)
                             && epo.OrderDate.AddHours(offset).Date >= DateFromPO.Date
                             && epo.OrderDate.AddHours(offset).Date <= DateToPO.Date


                             select new SelectedId
                             {
                                 LastModifiedUtc = a.LastModifiedUtc,
                                 POId = a.Id,
                                 POItemId = b.Id,
                                 PRId = c.Id,
                                 PRItemId = d.Id,
                                 EPOId = epo == null ? 0 : epo.Id,
                                 EPOItemId = epoItem == null ? 0 : epoItem.Id,
                                 EPODetailId = epoDetail == null ? 0 : epoDetail.Id,
                                 DOId = DO == null ? 0 : DO.Id,
                                 DOItemId = doItem == null ? 0 : doItem.Id,
                                 DODetailId = doDetail == null ? 0 : doDetail.Id,
                                 URNId = urn == null ? 0 : urn.Id,
                                 URNItemId = urnItem == null ? 0 : urnItem.Id,
                                 UPOId = upo == null ? 0 : upo.Id,
                                 UPOItemId = upoItem == null ? 0 : upoItem.Id,
                                 UPODetailId = upoDetail == null ? 0 : upoDetail.Id,
                                 //UPCId = corr == null ? 0 : corr.Id,
                                 //UPCItemId = corrItem == null ? 0 : corrItem.Id
                             });

                //TotalCountReport = Query.Distinct().Count();

                //var tt = Query.ToList();
                var queryResult = Query.Distinct().OrderByDescending(o => o.LastModifiedUtc).Skip((page - 1) * size).Take(size).ToList();
                TotalCountReport = Query.Distinct().Count();
                //var queryResult = Query.Distinct().OrderByDescending(o => o.LastModifiedUtc).ToList();

                var purchaseOrderInternalIds = queryResult.Select(s => s.POId).Distinct().ToList();
                var purchaseOrderInternals = dbContext.InternalPurchaseOrders.Where(w => purchaseOrderInternalIds.Contains(w.Id)).Select(s => new { s.Id, s.CreatedUtc, s.CreatedBy, s.PRNo, s.PRDate, s.CategoryName, s.BudgetName, s.LastModifiedUtc, s.ExpectedDeliveryDate, s.DivisionName }).ToList();
                var purchaseOrderInternalItemIds = queryResult.Select(s => s.POItemId).Distinct().ToList();
                var purchaseOrderInternalItems = dbContext.InternalPurchaseOrderItems.Where(w => purchaseOrderInternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Status, s.ProductName, s.ProductCode }).ToList();

                var purchaseRequestIds = queryResult.Select(s => s.PRId).Distinct().ToList();
                var purchaseRequests = dbContext.PurchaseRequests.Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.CreatedUtc }).ToList();
                //var purchaseRequestItemIds = queryResult.Select(s => s.PRItemId).Distinct().ToList();
                //var purchaseRequestItems = dbContext.PurchaseRequestItems.Where(w => purchaseRequestItemIds.Contains(w.Id)).Select(s => new { s.Id }).ToList();

                var purchaseOrderExternalIds = queryResult.Select(s => s.EPOId).Distinct().ToList();
                var purchaseOrderExternals = dbContext.ExternalPurchaseOrders.Where(w => purchaseOrderExternalIds.Contains(w.Id)).Select(s => new { s.Id, s.IsPosted, s.OrderDate, s.DeliveryDate, s.CurrencyCode, s.SupplierCode, s.SupplierName, s.EPONo, s.CreatedUtc, s.PaymentDueDays, s.Remark }).ToList();
                //var purchaseOrderExternalItemIds = queryResult.Select(s => s.EPOItemId).Distinct().ToList();
                //var purchaseOrderExternalItems = dbContext.ExternalPurchaseOrderItems.Where(w => purchaseOrderExternalItemIds.Contains(w.Id)).Select(s => new { s.Id }).ToList();
                var purchaseOrderExternalDetailIds = queryResult.Select(s => s.EPODetailId).Distinct().ToList();
                var purchaseOrderExternalDetails = dbContext.ExternalPurchaseOrderDetails.Where(w => purchaseOrderExternalDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.DealQuantity, s.DealUomUnit, s.PricePerDealUnit }).ToList();

                var unitReceiptNoteIds = queryResult.Select(s => s.URNId).Distinct().ToList();
                var unitReceiptNotes = dbContext.UnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate }).ToList();
                var unitReceiptNoteItemIds = queryResult.Select(s => s.URNItemId).Distinct().ToList();
                var unitReceiptNoteItems = dbContext.UnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.Uom }).ToList();

                var deliveryOrderIds = queryResult.Select(s => s.DOId).Distinct().ToList();
                var deliveryOrders = dbContext.DeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.DONo, s.DODate, s.ArrivalDate }).ToList();
                //var deliveryOrderItemIds = queryResult.Select(s => s.DOItemId).Distinct().ToList();
                //var deliveryOrderItems = dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
                var deliveryOrderDetailIds = queryResult.Select(s => s.DODetailId).Distinct().ToList();
                var deliveryOrderDetails = dbContext.DeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.DOQuantity, s.UomUnit }).ToList();

                var unitPaymentOrderIds = queryResult.Select(s => s.UPOId).Distinct().ToList();
                var unitPaymentOrders = dbContext.UnitPaymentOrders.Where(w => unitPaymentOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceDate, s.InvoiceNo, s.Date, s.UPONo, s.DueDate, s.UseVat, s.VatDate, s.VatNo, s.UseIncomeTax, s.IncomeTaxNo, s.IncomeTaxDate, s.IncomeTaxRate, s.VatRate }).ToList();
                //var unitPaymentOrderItemIds = queryResult.Select(s => s.UPOItemId).Distinct().ToList();
                //var unitPaymentOrderItems = dbContext.UnitPaymentOrderItems.Where(w => unitPaymentOrderItemIds.Contains(w.Id)).Select(s => new { s.Id }).ToList();
                var unitPaymentOrderDetailIds = queryResult.Select(s => s.UPODetailId).Distinct().ToList();
                var unitPaymentOrderDetails = dbContext.UnitPaymentOrderDetails.Where(w => unitReceiptNoteItemIds.Contains(w.URNItemId)).Select(s => new { s.Id, s.PriceTotal }).ToList();

                //var unitPaymentCorrectionNoteIds = queryResult.Select(s => s.UPCId).Distinct().ToList();
                //var unitPaymentCorrectionNotes = dbContext.UnitPaymentCorrectionNotes.Where(w => unitPaymentCorrectionNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.CorrectionDate, s.CorrectionType, s.UPCNo }).ToList();
                //var unitPaymentCorrectionNoteItemIds = queryResult.Select(s => s.UPCItemId).Distinct().ToList();
                //var unitPaymentCorrectionNoteItems = dbContext.UnitPaymentCorrectionNoteItems.Where(w => unitPaymentCorrectionNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.PriceTotalAfter, s.PriceTotalBefore, s.PricePerDealUnitAfter, s.PricePerDealUnitBefore, s.Quantity }).ToList();

                var corrections = dbContext.UnitPaymentCorrectionNotes.Where(w => unitPaymentOrderIds.Contains(w.UPOId)).Select(s => new { s.Id, s.CorrectionDate, s.CorrectionType, s.UPOId, s.UPCNo }).ToList();
                var correctionIds = corrections.Select(s => s.Id).ToList();
                var correctionItems = dbContext.UnitPaymentCorrectionNoteItems.Where(w => correctionIds.Contains(w.UPCId)).Select(s => new { s.Id, s.UPCId, s.UPODetailId, s.PriceTotalAfter, s.PriceTotalBefore, s.PricePerDealUnitAfter, s.PricePerDealUnitBefore, s.Quantity }).ToList();



                int i = ((page - 1) * size);
                foreach (var item in queryResult)
                {

                    var purchaseOrderInternal = purchaseOrderInternals.FirstOrDefault(f => f.Id.Equals(item.POId));
                    var purchaseOrderInternalItem = purchaseOrderInternalItems.FirstOrDefault(f => f.Id.Equals(item.POItemId));

                    var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
                    //var purchaseRequestItem = purchaseRequestItems.FirstOrDefault(f => f.Id.Equals(item.PRItemId));

                    var purchaseOrderExternal = purchaseOrderExternals.FirstOrDefault(f => f.Id.Equals(item.EPOId));
                    //var purchaseOrderExternalItem = purchaseOrderExternalItems.FirstOrDefault(f => f.Id.Equals(item.EPOItemId));
                    var purchaseOrderExternalDetail = purchaseOrderExternalDetails.FirstOrDefault(f => f.Id.Equals(item.EPODetailId));

                    var deliveryOrder = deliveryOrders.FirstOrDefault(f => f.Id.Equals(item.DOId));
                    //var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(f => f.Id.Equals(item.DOItemId));
                    var deliveryOrderDetail = deliveryOrderDetails.FirstOrDefault(f => f.Id.Equals(item.DODetailId));

                    var unitReceiptNote = unitReceiptNotes.FirstOrDefault(f => f.Id.Equals(item.URNId));
                    var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(f => f.Id.Equals(item.URNItemId));

                    var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.Id.Equals(item.UPOId));
                    //var unitPaymentOrderItem = unitPaymentOrderItems.FirstOrDefault(f => f.Id.Equals(item.UPOItemId));
                    var unitPaymentOrderDetail = unitPaymentOrderDetails.FirstOrDefault(f => f.Id.Equals(item.UPODetailId));

                    var selectedCorrections = corrections.Where(w => w.UPOId.Equals(item.UPOId)).ToList();
                    var selectedCorrectionIds = selectedCorrections.Select(s => s.Id).ToList();
                    var selectedCorrectionItems = correctionItems.Where(w => selectedCorrectionIds.Contains(w.UPCId)).ToList();
                    var corrItems = selectedCorrectionItems.Select(s => s.Id).ToList();


                    var correctionNoList = new List<string>();
                    var correctionDateList = new List<string>();
                    var correctionRemarkList = new List<string>();
                    var correctionNominalList = new List<string>();
                    int j = 1;
                    if (selectedCorrections.Count > 0)
                    {
                        foreach (var selectedCorrection in selectedCorrections)
                        {

                            var selectedCorrectionItem = selectedCorrectionItems.FirstOrDefault(f => f.UPCId.Equals(selectedCorrection.Id) && f.UPODetailId.Equals(item.UPODetailId));
                            if (selectedCorrectionItem != null)
                            {
                                correctionNoList.Add($"{j}. {selectedCorrection.UPCNo}");
                                correctionRemarkList.Add($"{j}. {selectedCorrection.CorrectionType}");
                                correctionDateList.Add($"{j}. {selectedCorrection.CorrectionDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}");

                                switch (selectedCorrection.CorrectionType)
                                {
                                    case "Harga Total":
                                        correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore)}");
                                        break;
                                    case "Harga Satuan":
                                        correctionNominalList.Add($"{j}. {string.Format("{0:N2}", (selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore) * selectedCorrectionItem.Quantity)}");
                                        break;
                                    case "Jumlah":
                                        correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter)}");
                                        break;
                                    default:
                                        break;
                                }
                                j++;
                            }
                        }
                    }

                    listEPO.Add(
                                        new PurchaseOrderMonitoringAllViewModel
                                        {
                                            index = i,
                                            createdDatePR = purchaseRequest.CreatedUtc.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            prDate = purchaseOrderInternal.PRDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            prNo = purchaseOrderInternal.PRNo,
                                            category = purchaseOrderInternal.CategoryName,
                                            budget = purchaseOrderInternal.BudgetName,
                                            productCode = purchaseOrderInternalItem.ProductCode,
                                            productName = purchaseOrderInternalItem.ProductName,
                                            quantity = purchaseOrderExternalDetail != null ? purchaseOrderExternalDetail.DealQuantity : 0,
                                            uom = purchaseOrderExternalDetail != null ? purchaseOrderExternalDetail.DealUomUnit : "-",
                                            pricePerDealUnit = purchaseOrderExternalDetail != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternalDetail.PricePerDealUnit : 0 : 0,
                                            priceTotal = purchaseOrderExternalDetail != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternalDetail.DealQuantity * purchaseOrderExternalDetail.PricePerDealUnit : 0 : 0,
                                            currencyCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.CurrencyCode,
                                            supplierCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierCode,
                                            supplierName = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierName,
                                            receivedDatePO = purchaseOrderInternal.CreatedUtc.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            epoDate = purchaseOrderExternal != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternal.OrderDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            epoCreatedDate = purchaseOrderExternal != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternal.CreatedUtc.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            epoExpectedDeliveryDate = purchaseOrderInternal.ExpectedDeliveryDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            epoDeliveryDate = purchaseOrderExternal != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternal.DeliveryDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            epoNo = purchaseOrderExternal != null ? purchaseOrderExternal.IsPosted == true ? purchaseOrderExternal.EPONo : "-" : "-",
                                            doDate = deliveryOrder == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : deliveryOrder.DODate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            doDeliveryDate = deliveryOrder == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : deliveryOrder.ArrivalDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            doNo = deliveryOrder == null ? "-" : deliveryOrder.DONo,
                                            doDetailId = selectedCorrectionItems.Count == 0 ? 0 : unitPaymentOrderDetail.Id,
                                            doQuantity = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DOQuantity,
                                            doUom = deliveryOrderDetail == null ? "-" : deliveryOrderDetail.UomUnit,
                                            urnProductCode = "",
                                            priceTotalBefore = 0,
                                            urnDate = unitReceiptNote == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : unitReceiptNote.ReceiptDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            urnNo = unitReceiptNote == null ? "-" : unitReceiptNote.URNNo,
                                            urnQuantity = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.ReceiptQuantity,
                                            urnUom = unitReceiptNoteItem == null ? "-" : unitReceiptNoteItem.Uom,
                                            paymentDueDays = purchaseOrderExternal != null && purchaseOrderExternal.IsPosted == true ? purchaseOrderExternal.PaymentDueDays : "-",
                                            invoiceDate = unitPaymentOrder == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : unitPaymentOrder.InvoiceDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            invoiceNo = unitPaymentOrder == null ? "-" : unitPaymentOrder.InvoiceNo,
                                            upoDate = unitPaymentOrder == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : unitPaymentOrder.Date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            upoNo = unitPaymentOrder == null ? "-" : unitPaymentOrder.UPONo,
                                            upoPriceTotal = unitPaymentOrderDetail == null ? 0 : unitPaymentOrderDetail.PriceTotal,
                                            dueDate = unitPaymentOrder == null ? date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : unitPaymentOrder.DueDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            vatDate = unitPaymentOrder != null ? unitPaymentOrder.UseVat ? unitPaymentOrder.VatDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                                            vatNo = unitPaymentOrder == null ? "-" : unitPaymentOrder.VatNo,
                                            vatValue = unitPaymentOrderDetail != null && unitPaymentOrder != null ? unitPaymentOrder.UseVat ? string.Format("{0:N2}", (unitPaymentOrder.VatRate / 100) * unitPaymentOrderDetail.PriceTotal) : "" : "",
                                            incomeTaxDate = unitPaymentOrder != null ? unitPaymentOrder.UseIncomeTax ? unitPaymentOrder.IncomeTaxDate : null : null,
                                            incomeTaxNo = unitPaymentOrder == null ? null : unitPaymentOrder.IncomeTaxNo,
                                            incomeTaxValue = unitPaymentOrderDetail != null && unitPaymentOrder != null ? unitPaymentOrder.UseIncomeTax ? (unitPaymentOrder.IncomeTaxRate * unitPaymentOrderDetail.PriceTotal / 100) : 0 : 0,
                                            correctionDate = correctionDateList.Count > 0 ? string.Join("\n", correctionDateList) : "",
                                            correctionNo = correctionNoList.Count > 0 ? string.Join("\n", correctionNoList) : "",
                                            correctionType = correctionRemarkList.Count > 0 ? string.Join("\n", correctionRemarkList) : "",
                                            valueCorrection = correctionNominalList.Count > 0 ? string.Join("\n", correctionNominalList) : "",
                                            remark = purchaseOrderExternal != null ? purchaseOrderExternal.Remark : "",
                                            status = purchaseOrderInternalItem.Status,
                                            staff = purchaseOrderInternal.CreatedBy,
                                            division = purchaseOrderInternal.DivisionName,
                                            correctionRemark = "",
                                            LastModifiedUtc = purchaseOrderInternal.LastModifiedUtc,
                                            epoQty = purchaseOrderExternalDetail != null ? purchaseOrderExternalDetail.DealQuantity : 0,
                                            Uomepo = purchaseOrderExternalDetail != null ? purchaseOrderExternalDetail.DealUomUnit : "-"
                                        });
                    listEPO2 = listEPO;

                    //i++;
                }

                return listEPO.Distinct().ToList();
        }

        public Tuple<List<PurchaseOrderMonitoringAllViewModel>, int> GetReport(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status, int page, int size, string Order, int offset, string user)
        {

            var Data = GetReportQuery(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo, dateFromPO, dateToPO, status, page, size, offset, user);

            //List<PurchaseOrderMonitoringAllViewModel> Query = Data.ToList();
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //    Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            //}


            //Pageable<PurchaseOrderMonitoringAllViewModel> pageable = new Pageable<PurchaseOrderMonitoringAllViewModel>(Query, page - 1, size);
            //List<PurchaseOrderMonitoringAllViewModel> DataQuery = pageable.Data.ToList<PurchaseOrderMonitoringAllViewModel>();
            //int TotalData = Query.Count();

            return Tuple.Create(Data, TotalCountReport);
        }

        public MemoryStream GenerateExcel(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status, int offset, string user)
        {
            var Query = GetReportQuery(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo, dateFromPO, dateToPO, status, 1, int.MaxValue, offset, user);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembuatan PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang PR", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang PO", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang PO", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Harga Barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembuatan PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Diminta Datang PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Target Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Datang Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Surat Jalan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Bon", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tempo Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Intern", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Nota Intern", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Jatuh Tempo", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPH", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Koreksi", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "No Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff Pembelian", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", 0, "", "", "", "", "", "", 0, "", "", "", "", "", "", 0, "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    //string prDate = item.prDate == null ? "-" : item.prDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string prCreatedDate = item.createdDatePR == null ? "-" : item.createdDatePR.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string receiptDatePO = item.receivedDatePO == null ? "-" : item.receivedDatePO.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDate = item.epoDate == new DateTime(1970, 1, 1) ? "-" : item.epoDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoCreatedDate = item.epoCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.epoCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoExpectedDeliveryDate = item.epoExpectedDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.epoExpectedDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDeliveryDate = item.epoDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.epoDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string doDate = item.doDate == new DateTime(1970, 1, 1) ? "-" : item.doDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string doDeliveryDate = item.doDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.doDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string urnDate = item.urnDate == new DateTime(1970, 1, 1) ? "-" : item.urnDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string invoiceDate = item.invoiceDate == new DateTime(1970, 1, 1) ? "-" : item.invoiceDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string upoDate = item.upoDate == new DateTime(1970, 1, 1) ? "-" : item.upoDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string dueDate = item.dueDate == new DateTime(1970, 1, 1) ? "-" : item.dueDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string vatDate = item.vatDate == new DateTime(1970, 1, 1) ? "-" : item.vatDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    DateTimeOffset date = item.incomeTaxDate ?? new DateTime(1970, 1, 1);
                    string incomeTaxDate = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string correctionDate = item.correctionDate == new DateTime(1970, 1, 1) ? "-" : item.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(index.ToString(), item.prDate, item.createdDatePR, item.prNo, item.category, item.division, item.budget, item.productName, item.productCode, item.quantity, item.uom, item.epoQty, item.Uomepo,
                        item.pricePerDealUnit, item.priceTotal, item.currencyCode, item.supplierCode, item.supplierName, item.receivedDatePO, item.epoDate, item.epoCreatedDate, item.epoExpectedDeliveryDate, item.epoDeliveryDate, item.epoNo, item.doDate,
                        item.doDeliveryDate, item.doNo, item.doQuantity, item.doUom, item.urnDate, item.urnNo, item.urnQuantity, item.urnUom, item.paymentDueDays, item.invoiceDate, item.invoiceNo, item.upoDate,
                        item.upoNo, item.upoPriceTotal, item.dueDate, item.vatDate, item.vatNo, item.vatValue, incomeTaxDate, item.incomeTaxNo, item.incomeTaxValue, item.correctionDates,
                        item.correctionNo, item.correctionQtys, item.correctionType, item.remark, item.status, item.staff);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public class SelectedId
        {
            public DateTimeOffset LastModifiedUtc { get; set; }
            public long POId { get; set; }
            public long POItemId { get; set; }
            public long PRId { get; set; }
            public long PRItemId { get; set; }
            public long EPOId { get; set; }
            public long EPOItemId { get; set; }
            public long EPODetailId { get; set; }
            public long DOId { get; set; }
            public long DOItemId { get; set; }
            public long DODetailId { get; set; }
            public long URNId { get; set; }
            public long URNItemId { get; set; }
            public long UPOId { get; set; }
            public long UPOItemId { get; set; }
            public long UPODetailId { get; set; }
            //public long UPCId { get; set; }
            //public long UPCItemId { get; set; }


        }


        #region PurchaseOrderStaffReport
        public IQueryable<PurchaseOrderStaffReportViewModel> GetReportQueryStaff(DateTime? dateFrom, DateTime? dateTo, string divisi, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;



            List<PurchaseOrderStaffReportViewModel> listAccuracyOfArrival = new List<PurchaseOrderStaffReportViewModel>();
            var QueryJoin = from a in dbContext.InternalPurchaseOrders
                            join b in dbContext.InternalPurchaseOrderItems on a.Id equals b.POId
                            join c in dbContext.ExternalPurchaseOrderItems on a.Id equals c.POId
                            join d in dbContext.ExternalPurchaseOrders on c.EPOId equals d.Id
                            join e in dbContext.DeliveryOrderItems on c.EPOId equals e.EPOId
                            join f in dbContext.DeliveryOrders on e.DOId equals f.Id


                            where a.IsDeleted == false
                                && d.IsDeleted == false
                                && f.IsDeleted == false
                               && ((DateFrom != new DateTime(1970, 1, 1)) ? (d.DeliveryDate.AddHours(offset).Date >= DateFrom && d.DeliveryDate.AddHours(offset).Date <= DateTo) : true)
                                && a.DivisionId == (string.IsNullOrWhiteSpace(divisi) ? a.DivisionId : divisi)

                            select new PurchaseOrderStaffReportViewModel
                            {
                                user = a.CreatedBy,
                                divisi = a.DivisionName,
                                unit = a.UnitName,
                                nopr = a.PRNo,
                                nmbarang = b.ProductName,
                                nmsupp = d.SupplierName,
                                tgltarget = d.DeliveryDate.AddHours(offset),
                                tgldatang = f.ArrivalDate.AddHours(offset),
                                tglpoint = a.CreatedUtc,
                                tglpoeks = d.OrderDate.AddHours(offset),
                                tgpr = a.PRDate,

                            };
            var Query = (from QueryJo in QueryJoin
                         group QueryJo by QueryJo.nopr into g

                         select new PurchaseOrderStaffReportViewModel
                         {
                             nopr = g.Key,
                             tgpr = g.Select(gg => gg.tgpr).FirstOrDefault(),
                             nmbarang = g.Select(gg => gg.nmbarang).FirstOrDefault(),
                             user = g.Select(gg => gg.user).FirstOrDefault(),
                             divisi = g.Select(gg => gg.divisi).FirstOrDefault(),
                             unit = g.Select(gg => gg.unit).FirstOrDefault(),
                             nmsupp = g.Select(gg => gg.nmsupp).FirstOrDefault(),
                             tgltarget = g.Select(gg => gg.tgltarget).FirstOrDefault(),
                             tgldatang = g.Select(gg => gg.tgldatang).FirstOrDefault(),
                             tglpoint = g.Select(gg => gg.tglpoint).FirstOrDefault(),
                             tglpoeks = g.Select(gg => gg.tglpoeks).FirstOrDefault(),

                         }).Distinct();



            Query = Query.OrderByDescending(b => b.user).ThenByDescending(b => b.tgltarget);


            foreach (var item in Query)
            {
                var tgldatang = new DateTimeOffset(item.tgldatang.Date, TimeSpan.Zero);
                var tgltarget = new DateTimeOffset(item.tgltarget.Date, TimeSpan.Zero);
                var tglpoint = new DateTimeOffset(item.tglpoint.Date, TimeSpan.Zero);
                var tglpoeks = new DateTimeOffset(item.tglpoeks.Date, TimeSpan.Zero);
                var selisih = ((TimeSpan)(tgldatang - tgltarget)).Days;
                var selisih2 = ((TimeSpan)(tglpoeks - tglpoint)).Days;


                PurchaseOrderStaffReportViewModel _new = new PurchaseOrderStaffReportViewModel
                {
                    user = item.user,
                    divisi = item.divisi,
                    unit = item.unit,
                    selisih = selisih,
                    selisih2 = selisih2,
                    nmbarang = item.nmbarang,
                    nmsupp = item.nmsupp,
                    nopr = item.nopr,
                    tgltarget = item.tgltarget,
                    tgldatang = item.tgldatang,
                    tglpoint = item.tglpoint,
                    tglpoeks = item.tglpoeks,
                    tgpr = item.tgpr,
                    jumpr = 1,


                };


                listAccuracyOfArrival.Add(_new);
            }
            return listAccuracyOfArrival.OrderByDescending(b => b.user).ThenByDescending(b => b.tgltarget).AsQueryable();
        }

        public Tuple<List<PurchaseOrderStaffReportViewModel>, int> GetReportStaff(DateTime? dateFrom, DateTime? dateTo, string divisi, int offset)
        {

            var QueryStaff = GetReportQueryStaff(dateFrom, dateTo, divisi, offset);

            List<PurchaseOrderStaffReportViewModel> Data = new List<PurchaseOrderStaffReportViewModel>();
            List<PurchaseOrderStaffReportViewModel> Data2 = new List<PurchaseOrderStaffReportViewModel>();

            //var SuppTemp = "";


            var Group = (from a in QueryStaff
                         group a by a.user into g

                         select new PurchaseOrderStaffReportViewModel
                         {
                             user = g.Key,
                             jumpr = g.Sum(gg => gg.jumpr),
                         }).Distinct();


            foreach (var item in Group.OrderByDescending(b => b.user).ThenByDescending(b => b.jumpr))
            {


                PurchaseOrderStaffReportViewModel _new = new PurchaseOrderStaffReportViewModel
                {
                    user = item.user,
                    jumpr = item.jumpr,


                };
                Data.Add(_new);
            }


            return Tuple.Create(Data, Data.Count);
        }



        public Tuple<List<PurchaseOrderStaffReportViewModel>, int> GetReportsubStaff(DateTime? dateFrom, DateTime? dateTo, string divisi, string staff, int offset)
        {

            var QueryStaff = GetReportQueryStaff(dateFrom, dateTo, divisi, offset);

            List<PurchaseOrderStaffReportViewModel> Data = new List<PurchaseOrderStaffReportViewModel>();

            foreach (var item in QueryStaff.Where(b => b.user == staff).OrderByDescending(b => b.tgltarget))
            {

                PurchaseOrderStaffReportViewModel _new = new PurchaseOrderStaffReportViewModel
                {
                    user = item.user,
                    divisi = item.divisi,
                    unit = item.unit,
                    selisih = item.selisih,
                    selisih2 = item.selisih2,
                    nmbarang = item.nmbarang,
                    nmsupp = item.nmsupp,
                    nopr = item.nopr,
                    tgltarget = item.tgltarget,
                    tgldatang = item.tgldatang,
                    tglpoint = item.tglpoint,
                    tglpoeks = item.tglpoeks,
                    tgpr = item.tgpr,
                    jumpr = item.jumpr,
                };
                Data.Add(_new);

            }

            return Tuple.Create(Data, Data.Count);
        }


        public MemoryStream GenerateExcelSarmut(DateTime? dateFrom, DateTime? dateTo, string divisi, string staff, int offset)
        {
            var QueryStaff = GetReportQueryStaff(dateFrom, dateTo, divisi, offset);
            List<PurchaseOrderStaffReportViewModel> Data = new List<PurchaseOrderStaffReportViewModel>();

            foreach (var item in QueryStaff.Where(b => b.user == staff).OrderByDescending(b => b.tgltarget))
            {

                PurchaseOrderStaffReportViewModel _new = new PurchaseOrderStaffReportViewModel
                {
                    user = item.user,
                    divisi = item.divisi,
                    unit = item.unit,
                    selisih = item.selisih,
                    selisih2 = item.selisih2,
                    nmbarang = item.nmbarang,
                    nmsupp = item.nmsupp,
                    nopr = item.nopr,
                    tgltarget = item.tgltarget,
                    tgldatang = item.tgldatang,
                    tglpoint = item.tglpoint,
                    tglpoeks = item.tglpoeks,
                    tgpr = item.tgpr,
                    jumpr = item.jumpr,
                };
                Data.Add(_new);

            }

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff Pembelian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima PO Int", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima PO Eks", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih 1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Target", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Kedatangan ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih 2", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });


            if (Data.ToArray().Count() == 0)

                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
            else
            {
                int index = 0;
                foreach (var item in Data)
                {
                    index++;
                    string gpoint = item.tglpoint == null ? "-" : item.tglpoint.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string gpoeks = item.tglpoeks == null ? "-" : item.tglpoeks.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string gtarget = item.tgltarget == null ? "-" : item.tgltarget.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string gdatang = item.tgldatang == null ? "-" : item.tgldatang.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(index, item.divisi, item.user, item.nopr, item.nmbarang, item.nmsupp, gpoint, gpoeks, item.selisih2, gtarget, gdatang, item.selisih, item.unit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        #endregion
    }
}
