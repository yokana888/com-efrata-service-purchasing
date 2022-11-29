using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO
{
    public class PurchaseMonitoringService : IPurchaseMonitoringService
    {
        private readonly PurchasingDbContext _dbContext;
        private readonly DbSet<PurchaseRequest> _purchaseRequestDbSet;
        private readonly DbSet<PurchaseRequestItem> _purchaseRequestItemDbSet;
        private readonly DbSet<InternalPurchaseOrder> _internalPurchaseOrderDbSet;
        private readonly DbSet<InternalPurchaseOrderItem> _internalPurchaseOrderItemDbSet;
        private readonly DbSet<InternalPurchaseOrderFulFillment> _internalPurchaseOrderFulfillmentDbSet;
        private readonly DbSet<ExternalPurchaseOrder> _externalPurchaseOrderDbSet;
        private readonly DbSet<ExternalPurchaseOrderItem> _externalPurchaseOrderItemDbSet;
        private readonly DbSet<ExternalPurchaseOrderDetail> _externalPurchaseOrderDetailDbSet;
        private readonly DbSet<DeliveryOrder> _deliveryOderDbSet;
        private readonly DbSet<DeliveryOrderItem> _deliveryOrderItemDbSet;
        private readonly DbSet<DeliveryOrderDetail> _deliveryOrderDetailDbSet;
        private readonly DbSet<UnitReceiptNote> _unitReceiptNoteDbSet;
        private readonly DbSet<UnitReceiptNoteItem> _unitReceiptNoteItemDbSet;
        private readonly DbSet<UnitPaymentOrder> _unitPaymentOrderDbSet;
        private readonly DbSet<UnitPaymentOrderItem> _unitPaymentOrderItemDbSet;
        private readonly DbSet<UnitPaymentOrderDetail> _unitPaymentOrderDetailDbSet;
        private readonly DbSet<UnitPaymentCorrectionNoteItem> _correctionItemDbSet;

        public PurchaseMonitoringService(PurchasingDbContext dbContext)
        {
            _dbContext = dbContext;
            //_dbContext.Database.SetCommandTimeout(1000 * 60 * 2);
            //if (_dbContext.Database.IsSqlServer())
            //    _dbContext.Database.SetCommandTimeout(1000 * 60 * 2);

            _purchaseRequestDbSet = dbContext.Set<PurchaseRequest>();
            _purchaseRequestItemDbSet = dbContext.Set<PurchaseRequestItem>();

            _internalPurchaseOrderDbSet = dbContext.Set<InternalPurchaseOrder>();
            _internalPurchaseOrderItemDbSet = dbContext.Set<InternalPurchaseOrderItem>();
            _internalPurchaseOrderFulfillmentDbSet = dbContext.Set<InternalPurchaseOrderFulFillment>();

            _externalPurchaseOrderDbSet = dbContext.Set<ExternalPurchaseOrder>();
            _externalPurchaseOrderItemDbSet = dbContext.Set<ExternalPurchaseOrderItem>();
            _externalPurchaseOrderDetailDbSet = dbContext.Set<ExternalPurchaseOrderDetail>();

            _deliveryOderDbSet = dbContext.Set<DeliveryOrder>();
            _deliveryOrderItemDbSet = dbContext.Set<DeliveryOrderItem>();
            _deliveryOrderDetailDbSet = dbContext.Set<DeliveryOrderDetail>();

            _unitReceiptNoteDbSet = dbContext.Set<UnitReceiptNote>();
            _unitReceiptNoteItemDbSet = dbContext.Set<UnitReceiptNoteItem>();

            _unitPaymentOrderDbSet = dbContext.Set<UnitPaymentOrder>();
            _unitPaymentOrderItemDbSet = dbContext.Set<UnitPaymentOrderItem>();
            _unitPaymentOrderDetailDbSet = dbContext.Set<UnitPaymentOrderDetail>();

            _correctionItemDbSet = dbContext.Set<UnitPaymentCorrectionNoteItem>();
        }
        public int TotalCountReport { get; set; } = 0;
        public List<PurchaseMonitoringReportViewModel> GetReportQuery(string unitId, string categoryId, string divisionId, string budgetId, long prId, string createdBy, string status, DateTimeOffset startDate, DateTimeOffset endDate, DateTime? startDatePO, DateTime? endDatePO, long poExtId, string supplierId, int page, int size)
        {
            DateTime StartDatePO = startDatePO == null ? DateTime.MinValue : (DateTime)startDatePO;
            DateTime EndDatePO = endDatePO == null ? DateTime.Now : (DateTime)endDatePO;
            var purchaseRequestItems = _purchaseRequestItemDbSet.Include(prItem => prItem.PurchaseRequest).Where(w => w.PurchaseRequest.Date >= startDate && w.PurchaseRequest.Date <= endDate);
            purchaseRequestItems = FilterPurchaseRequest(unitId, categoryId, divisionId, budgetId, prId, purchaseRequestItems);
            var internalPurchaseOrderFulfillments = _internalPurchaseOrderFulfillmentDbSet.AsQueryable();
            var internalPurchaseOrderItems = _internalPurchaseOrderItemDbSet.Include(ipoItem => ipoItem.InternalPurchaseOrder).AsQueryable();
            var externalPurchaseOrderDetails = _externalPurchaseOrderDetailDbSet.Include(epoDetail => epoDetail.ExternalPurchaseOrderItem).ThenInclude(epoItem => epoItem.ExternalPurchaseOrder).AsQueryable();
            //externalPurchaseOrderDetails = externalPurchaseOrderDetails.Where(x => x.ExternalPurchaseOrderItem.ExternalPurchaseOrder.OrderDate >= startDatePO && x.ExternalPurchaseOrderItem.ExternalPurchaseOrder.OrderDate <= endDatePO);
            //var d = externalPurchaseOrderDetails.Count();
            //var deliveryOrderDetails = _deliveryOrderDetailDbSet.Include(doDetail => doDetail.DeliveryOrderItem).ThenInclude(doItem => doItem.DeliveryOrder).AsQueryable();
            //var unitReceiptNoteItems = _unitReceiptNoteItemDbSet.Include(urnItem => urnItem.UnitReceiptNote).AsQueryable();
            //var unitPaymentOrderDetails = _unitPaymentOrderDetailDbSet.Include(upoDetail => upoDetail.UnitPaymentOrderItem).ThenInclude(upoItem => upoItem.UnitPaymentOrder).AsQueryable();
            List<PurchaseMonitoringReportViewModel>reportdata = new List<PurchaseMonitoringReportViewModel>();
            var query = (from a in _dbContext.InternalPurchaseOrders
                         join b in _dbContext.InternalPurchaseOrderItems on a.Id equals b.POId
                         //PR
                         join d in _dbContext.PurchaseRequestItems on b.PRItemId equals d.Id
                         join c in _dbContext.PurchaseRequests on d.PurchaseRequestId equals c.Id
                         //EPO
                         join e in _dbContext.ExternalPurchaseOrderItems on a.Id equals e.POId into f
                         from epoitem in f.DefaultIfEmpty()
                         join k in _dbContext.ExternalPurchaseOrderDetails on b.Id equals k.POItemId into l
                         from epodetail in l.DefaultIfEmpty()
                         join g in _dbContext.ExternalPurchaseOrders on epoitem.EPOId equals g.Id into h
                         from epo in h.DefaultIfEmpty()
                             //DO
                         join yy in _dbContext.DeliveryOrderDetails on epodetail.Id equals yy.EPODetailId into zz
                         from doDetail in zz.DefaultIfEmpty()
                         join m in _dbContext.DeliveryOrderItems on doDetail.DOItemId equals m.Id into n
                         from doItem in n.DefaultIfEmpty()
                         join o in _dbContext.DeliveryOrders on doItem.DOId equals o.Id into p
                         from DO in p.DefaultIfEmpty()
                             //    //URN
                         join s in _dbContext.UnitReceiptNoteItems on doDetail.Id equals s.DODetailId into t
                         from urnItem in t.DefaultIfEmpty()
                         join q in _dbContext.UnitReceiptNotes on urnItem.URNId equals q.Id into r
                         from urn in r.DefaultIfEmpty()
                             //    //UPO
                         join u in _dbContext.UnitPaymentOrderItems on urn.Id equals u.URNId into v
                         from upoItem in v.DefaultIfEmpty()
                         join w in _dbContext.UnitPaymentOrders on upoItem.UPOId equals w.Id into x
                         from upo in x.DefaultIfEmpty()
                         join y in _dbContext.UnitPaymentOrderDetails on urnItem.Id equals y.URNItemId into z
                         from upoDetail in z.DefaultIfEmpty()
                         where
                          a.CreatedBy == (string.IsNullOrWhiteSpace(createdBy) ? a.CreatedBy : createdBy)
                          && (poExtId > 0 ? epo.Id == poExtId : true )
                          && b.Status == (string.IsNullOrWhiteSpace(status) ? b.Status : status)
                          && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                          && a.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? a.CategoryId : categoryId)
                          && a.DivisionId == (string.IsNullOrWhiteSpace(divisionId) ? a.DivisionId : divisionId)
                          && a.BudgetId == (string.IsNullOrWhiteSpace(budgetId) ? a.BudgetId : budgetId)
                          && (!string.IsNullOrWhiteSpace(supplierId) ? epo.SupplierId == supplierId : true) 
                          && a.PRId == (prId > 0 ? prId.ToString() : a.PRId)
                          && a.PRDate.Date >= startDate.Date 
                          && a.PRDate.Date <= endDate.Date
                          && epo.OrderDate.Date >= StartDatePO.Date
                          && epo.OrderDate.Date <= EndDatePO.Date
                         select new
                         {
                             PurchaseRequestId = c.Id,
                             PurchaseRequestItemId = d.Id,
                             InternalPurchaseOrderId = a.Id,
                             InternalPurchaseOrderLastModifiedDate = a.LastModifiedUtc,
                             InternalPurchaseOrderItemId = b.Id,
                             ExternalPurchaseOrderId = epo != null ? epo.Id : 0,
                             ExternalPurchaseOrderItemId = epoitem != null ? epoitem.Id : 0,
                             ExternalPurchaseOrderDetailId = epodetail != null ? epodetail.Id : 0,
                             DOId = DO == null ? 0 : DO.Id,
                             DOItemId = doItem == null ? 0 : doItem.Id,
                             DODetailId = doDetail == null ? 0 : doDetail.Id,
                             URNId = urn == null ? 0 : urn.Id,
                             URNItemId = urnItem == null ? 0 : urnItem.Id,
                             UPOId = upo == null ? 0 : upo.Id,
                             UPOItemId = upoItem == null ? 0 : upoItem.Id,
                             UPODetailId = upoDetail == null ? 0 : upoDetail.Id,
                             SupplierId = epo == null ? "0" : epo.SupplierId,
                         }).OrderByDescending(b=>b.InternalPurchaseOrderLastModifiedDate).Distinct().ToList();

            //if (!string.IsNullOrWhiteSpace(createdBy))
            //    query = query.Where(w => w.InternalPurchaseOrderStaff == createdBy).ToList();

            //if (poExtId > 0)
            //    query = query.Where(w => w.ExternalPurchaseOrderId == poExtId).ToList();

            //if (!string.IsNullOrWhiteSpace(supplierId))
            //    query = query.Where(w => w.SupplierId.Equals(supplierId)).ToList();

            //if (!string.IsNullOrWhiteSpace(status))
            //    query = query.Where(w => w.InternalPurchaseOrderStatus.Equals(status)).ToList();

            TotalCountReport = query.Distinct().Count();
            var queryresult = query.Distinct().Skip((page - 1) * size).Take(size).ToList();

            var purchaserequestsIds = queryresult.Select(x => x.PurchaseRequestId).Distinct().ToList();
            var purchaserequests = _dbContext.PurchaseRequests.Where(x => purchaserequestsIds.Contains(x.Id)).Select(x => new { x.Id, x.Date, x.No, x.CreatedUtc, x.CategoryId, x.CategoryName, x.DivisionId, x.DivisionName, x.BudgetId, x.BudgetName }).ToList();
            var purchaserequestitemIds = queryresult.Select(x => x.PurchaseRequestItemId).Distinct().ToList();
            var purchaserequestItems = _dbContext.PurchaseRequestItems.Where(x => purchaserequestitemIds.Contains(x.Id)).Select(s => new { s.Id, s.ProductId, s.ProductName, s.ProductCode }).ToList();
            var internalpurchaseorderIds = queryresult.Select(x => x.InternalPurchaseOrderId).Distinct().ToList();
            var internalpurchaseorders = _dbContext.InternalPurchaseOrders.Where(x => internalpurchaseorderIds.Contains(x.Id)).Select(s => new { s.Id, s.PONo, s.CreatedUtc, s.CreatedBy, s.LastModifiedUtc, s.ExpectedDeliveryDate }).ToList();
            var internalpurchaseorderitemIds = queryresult.Select(x => x.InternalPurchaseOrderItemId).Distinct().ToList();
            var internalpurchaseorderitems = _dbContext.InternalPurchaseOrderItems.Where(x => internalpurchaseorderitemIds.Contains(x.Id)).Select(s => new { s.Id, s.Status }).ToList();
            var externalpurchaseorderIds = queryresult.Select(x => x.ExternalPurchaseOrderId).Distinct().ToList();
            var externalpurchaseorders = _dbContext.ExternalPurchaseOrders.Where(x => externalpurchaseorderIds.Contains(x.Id)).Select(x => new { x.Id, x.IsPosted, x.CurrencyId, x.CurrencyCode, x.CurrencyRate, x.CreatedUtc, x.OrderDate, x.DeliveryDate, x.EPONo, x.SupplierId, x.SupplierCode, x.SupplierName, x.PaymentDueDays, x.Remark }).ToList();
            var externalpurchaseorderitemIds = queryresult.Select(x => x.ExternalPurchaseOrderItemId).Distinct().ToList();
            var externalpurchaseorderitems = _dbContext.ExternalPurchaseOrderItems.Where(x => externalpurchaseorderitemIds.Contains(x.Id)).Select(x => new { x.Id }).ToList();
            var externalpurchaseorderdetailIds = queryresult.Select(x => x.ExternalPurchaseOrderDetailId).Distinct().ToList();
            var externalpurchaseorderdetails = _dbContext.ExternalPurchaseOrderDetails.Where(x => externalpurchaseorderdetailIds.Contains(x.Id)).Select(x => new { x.Id, x.DealQuantity, x.DealUomId, x.DealUomUnit, x.PricePerDealUnit }).ToList();
            var deliveryOrderIds = queryresult.Select(s => s.DOId).Distinct().ToList();
            var deliveryOrders = _dbContext.DeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.DONo, s.DODate, s.ArrivalDate }).ToList();
            var unitReceiptNoteIds = queryresult.Select(s => s.URNId).Distinct().ToList();
            var unitReceiptNotes = _dbContext.UnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate }).ToList();
            var unitReceiptNoteItemIds = queryresult.Select(s => s.URNItemId).Distinct().ToList();
            var unitReceiptNoteItems = _dbContext.UnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.Uom, s.UomId }).ToList();
            var deliveryOrderItemIds = queryresult.Select(s => s.DOItemId).Distinct().ToList();
            var deliveryOrderItems = _dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id }).ToList();
            var deliveryOrderDetailIds = queryresult.Select(s => s.DODetailId).Distinct().ToList();
            var deliveryOrderDetails = _dbContext.DeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.DOQuantity, s.UomUnit }).ToList();

            var unitPaymentOrderIds = queryresult.Select(s => s.UPOId).Distinct().ToList();
            var unitPaymentOrders = _dbContext.UnitPaymentOrders.Where(w => unitPaymentOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceDate, s.InvoiceNo, s.Date, s.UPONo, s.DueDate, s.UseVat, s.VatDate, s.VatNo, s.UseIncomeTax, s.IncomeTaxNo, s.IncomeTaxDate, s.IncomeTaxRate, s.VatRate }).ToList();
            var unitPaymentOrderItemIds = queryresult.Select(s => s.UPOItemId).Distinct().ToList();
            var unitPaymentOrderItems = _dbContext.UnitPaymentOrderItems.Where(w => unitPaymentOrderItemIds.Contains(w.Id)).Select(s => new { s.Id }).ToList();
            var unitPaymentOrderDetailIds = queryresult.Select(s => s.UPODetailId).Distinct().ToList();
            var unitPaymentOrderDetails = _dbContext.UnitPaymentOrderDetails.Where(w => unitReceiptNoteItemIds.Contains(w.URNItemId)).Select(s => new { s.Id, s.PriceTotal }).ToList();

            foreach (var data in queryresult)
            {
                var purchaserequest = purchaserequests.FirstOrDefault(x => x.Id.Equals(data.PurchaseRequestId));
                var purchaserequestItem = purchaserequestItems.FirstOrDefault(x => x.Id.Equals(data.PurchaseRequestItemId));
                var internalpurchaseorder = internalpurchaseorders.FirstOrDefault(x => x.Id.Equals(data.InternalPurchaseOrderId));
                var internalpurchaseorderitem = internalpurchaseorderitems.FirstOrDefault(x => x.Id.Equals(data.InternalPurchaseOrderItemId));
                var externalpurchaseorder = externalpurchaseorders.FirstOrDefault(x => x.Id.Equals(data.ExternalPurchaseOrderId));
                var externalpurchaseorderitem = externalpurchaseorderitems.FirstOrDefault(x => x.Id.Equals(data.ExternalPurchaseOrderItemId));
                var externalpurchaseorderdetail = externalpurchaseorderdetails.FirstOrDefault(x => x.Id.Equals(data.ExternalPurchaseOrderDetailId));
                var deliveryOrder = deliveryOrders.FirstOrDefault(f => f.Id.Equals(data.DOId));
                var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(f => f.Id.Equals(data.DOItemId));
                var deliveryOrderDetail = deliveryOrderDetails.FirstOrDefault(f => f.Id.Equals(data.DODetailId));

                var unitReceiptNote = unitReceiptNotes.FirstOrDefault(f => f.Id.Equals(data.URNId));
                var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(f => f.Id.Equals(data.URNItemId));

                var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(f => f.Id.Equals(data.UPOId));
                var unitPaymentOrderItem = unitPaymentOrderItems.FirstOrDefault(f => f.Id.Equals(data.UPOItemId));
                var unitPaymentOrderDetail = unitPaymentOrderDetails.FirstOrDefault(f => f.Id.Equals(data.UPODetailId));

                reportdata.Add(new PurchaseMonitoringReportViewModel
                {
                    PurchaseRequestId = purchaserequest.Id,
                    PurchaseRequestNo = purchaserequest.No,
                    PurchaseRequestDate = purchaserequest.Date.Date,
                    PurchaseRequestCreatedDate = purchaserequest.CreatedUtc,
                    CategoryId = purchaserequest.CategoryId,
                    CategoryName = purchaserequest.CategoryName,
                    DivisionId = purchaserequest.DivisionId,
                    DivisionName = purchaserequest.DivisionName,
                    BudgetId = purchaserequest.BudgetId,
                    BudgetName = purchaserequest.BudgetName,
                    PurchaseRequestItemId = purchaserequestItem.Id,
                    ProductId = purchaserequestItem.ProductId,
                    ProductName = purchaserequestItem.ProductName,
                    ProductCode = purchaserequestItem.ProductCode,
                    OrderQuantity = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.DealQuantity : 0,
                    UOMId = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.DealUomId : "",
                    UOMUnit = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.DealUomUnit : "-",
                    Price = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.PricePerDealUnit : 0,
                    PriceTotal = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.PricePerDealUnit * externalpurchaseorderdetail.DealQuantity : 0,
                    CurrencyId = externalpurchaseorder != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.CurrencyId : "",
                    CurrencyCode = externalpurchaseorder != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.CurrencyCode : "",
                    CurrencyRate = externalpurchaseorder != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.CurrencyRate : 0,
                    InternalPurchaseOrderId = internalpurchaseorder == null ? 0 : internalpurchaseorder.Id,
                    InternalPurchaseOrderNo = internalpurchaseorder == null ? "" : internalpurchaseorder.PONo,
                    InternalPurchaseOrderCreatedDate = internalpurchaseorder == null ? (DateTime?)null : internalpurchaseorder.CreatedUtc,
                    InternalPurchaseOrderLastModifiedDate = internalpurchaseorder == null ? (DateTime?)null : internalpurchaseorder.LastModifiedUtc,
                    InternalPurchaseOrderStaff = internalpurchaseorder != null ? internalpurchaseorder.CreatedBy : "",
                    InternalPurchaseOrderItemId = internalpurchaseorderitem == null ? 0 : internalpurchaseorderitem.Id,
                    InternalPurchaseOrderStatus = internalpurchaseorderitem != null ? internalpurchaseorderitem.Status : "",
                    ExternalPurchaseOrderId = externalpurchaseorder != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.Id : 0,
                    ExternalPurchaseOrderCreatedDate = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.CreatedUtc : (DateTime?)null,
                    ExternalPurchaseOrderDate = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.OrderDate.Date : (DateTime?)null,
                    ExternalPurchaseOrderExpectedDeliveryDate = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted && internalpurchaseorder != null ? internalpurchaseorder.ExpectedDeliveryDate.Date : (DateTime?)null,
                    ExternalPurchaseOrderDeliveryDate = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted && internalpurchaseorder != null ? externalpurchaseorder.DeliveryDate.Date : (DateTime?)null,
                    ExternalPurchaseOrderNo = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.EPONo : "",
                    ExternalPurchaseOrderQuantity = externalpurchaseorderdetail != null ? externalpurchaseorderdetail.DealQuantity : 0,
                    ExternalPurchaseOrderUomUnit = externalpurchaseorderdetail != null ? externalpurchaseorderdetail.DealUomUnit : "-",
                    SupplierId = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.SupplierId : "",
                    SupplierCode = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.SupplierCode : "",
                    SupplierName = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.SupplierName : "",
                    ExternalPurchaseOrderPaymentDueDays = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.PaymentDueDays : "",
                    ExternalPurchaseOrderRemark = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorder.Remark : "",
                    ExternalPurchaseOrderItemId = externalpurchaseorderitem != null && externalpurchaseorder.IsPosted ? externalpurchaseorderitem.Id : 0,
                    ExternalPurchaseOrderDetailId = externalpurchaseorderdetail != null && externalpurchaseorder.IsPosted ? externalpurchaseorderdetail.Id : 0,
                    DeliveryOrderId = deliveryOrder == null ? 0 : deliveryOrder.Id,
                    DeliveryOrderDate = deliveryOrder != null ? deliveryOrder.DODate.Date : (DateTime?)null,
                    DeliveryOrderArrivalDate = deliveryOrder != null ? deliveryOrder.ArrivalDate.Date : (DateTime?)null,
                    DeliveryOrderNo = deliveryOrder != null ? deliveryOrder.DONo : "",
                    DeliveryOrderItemId = deliveryOrderItem == null ? 0 : deliveryOrderItem.Id,
                    DelveryOrderDetailId = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.Id,
                    doQuantity = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DOQuantity,
                    doUom = deliveryOrderDetail == null ? "-" : deliveryOrderDetail.UomUnit,
                    UnitReceiptNoteId = unitReceiptNote == null ? 0 : unitReceiptNote.Id,
                    UnitReceiptNoteDate = unitReceiptNote != null && unitReceiptNote.ReceiptDate.Date != DateTime.MinValue ? unitReceiptNote.ReceiptDate.DateTime : (DateTime?)null,
                    UnitReceiptNoteNo = unitReceiptNote != null ? unitReceiptNote.URNNo : "",
                    UnitReceiptNoteItemId = unitReceiptNoteItem != null ? unitReceiptNoteItem.Id : 0,
                    UnitReceiptNoteQuantity = unitReceiptNoteItem != null ? unitReceiptNoteItem.ReceiptQuantity : 0,
                    UnitReceiptNoteUomId = unitReceiptNoteItem != null ? unitReceiptNoteItem.UomId : "",
                    UnitReceiptNoteUomUnit = unitReceiptNoteItem != null ? unitReceiptNoteItem.Uom : "",
                    UnitPaymentOrderId = unitPaymentOrder == null ? 0 : unitPaymentOrder.Id,
                    UnitPaymentOrderInvoiceDate = unitPaymentOrder != null && unitPaymentOrder.InvoiceDate.Date != DateTime.MinValue ? unitPaymentOrder.InvoiceDate.Date : (DateTime?)null,
                    UnitPaymentOrderInvoiceNo = unitPaymentOrder != null ? unitPaymentOrder.InvoiceNo : "",
                    UnitPaymentOrderDate = unitPaymentOrder != null && unitPaymentOrder.Date.Date != DateTime.MinValue ? unitPaymentOrder.Date.Date : (DateTime?)null,
                    UnitPaymentOrderNo = unitPaymentOrder != null ? unitPaymentOrder.UPONo : "",
                    UnitPaymentOrderDueDate = unitPaymentOrder != null && unitPaymentOrder.DueDate.Date != DateTime.MinValue ? unitPaymentOrder.DueDate.Date : (DateTime?)null,
                    UnitPaymentOrderItemId = unitPaymentOrderItem == null ? 0 : unitPaymentOrderItem.Id,
                    UnitPaymentOrderDetailId = unitPaymentOrderDetail == null ? 0 : unitPaymentOrderDetail.Id,
                    UnitPaymentOrderTotalPrice = unitPaymentOrderDetail != null ? unitPaymentOrderDetail.PriceTotal : 0,
                    UnitPaymentOrderVATDate = unitPaymentOrder != null && unitPaymentOrder.UseVat && unitPaymentOrder.VatDate.Date != DateTime.MinValue ? unitPaymentOrder.VatDate.Date : (DateTime?)null,
                    UnitPaymentOrderVATNo = unitPaymentOrder != null && unitPaymentOrder.UseVat ? unitPaymentOrder.VatNo : "",
                    UnitPaymentOrderVAT = unitPaymentOrder != null && unitPaymentOrder.UseVat ? (unitPaymentOrder.VatRate / 100) * unitPaymentOrderDetail.PriceTotal : 0,
                    UnitPaymentOrderIncomeTaxDate = unitPaymentOrder != null && unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxDate.Value.Date != DateTime.MinValue ? unitPaymentOrder.IncomeTaxDate.Value.Date : (DateTime?)null,
                    UnitPaymentOrderIncomeTaxNo = unitPaymentOrder != null && unitPaymentOrder.UseIncomeTax ? unitPaymentOrder.IncomeTaxNo : "",
                    UnitPaymentOrderIncomeTax = unitPaymentOrder != null && unitPaymentOrder.UseIncomeTax ? unitPaymentOrder.IncomeTaxRate * unitPaymentOrderDetail.PriceTotal : 0
                });
            }

            return reportdata;
        }

        public async Task<List<PurchaseMonitoringReportViewModel>> MapCorrections(List<PurchaseMonitoringReportViewModel> data)
        {
            var upoDetailIds = data.Select(datum => datum.UnitPaymentOrderDetailId).ToList();
            var corrections = await _correctionItemDbSet
                .Include(item => item.UnitPaymentCorrectionNote)
                .Where(item => upoDetailIds.Contains(item.UPODetailId))
                .Select(item => new
                {
                    item.UnitPaymentCorrectionNote.UPCNo,
                    item.UnitPaymentCorrectionNote.CorrectionType,
                    item.UnitPaymentCorrectionNote.CorrectionDate,
                    item.PriceTotalAfter,
                    item.PriceTotalBefore,
                    item.Quantity,
                    item.UPODetailId
                }).ToListAsync();

            //var result = new List<PurchaseMonitoringReportViewModel>();

            foreach (var correction in corrections)
            {
                var datum = data.FirstOrDefault(f => f.UnitPaymentOrderDetailId == correction.UPODetailId);

                if (datum != null)
                {
                    datum.CorrectionDate += $"- {correction.CorrectionDate.ToString("dd MMMM yyyy")}\n";
                    datum.CorrectionNo += $"- {correction.UPCNo}\n";
                    datum.CorrectionType += $"- {correction.CorrectionType}\n";

                    switch (correction.CorrectionType)
                    {
                        case "Harga Total":
                            datum.CorrectionNominal += $"- {string.Format("{0:N2}", correction.PriceTotalAfter - correction.PriceTotalBefore)}\n";
                            break;
                        case "Harga Satuan":
                            datum.CorrectionNominal += $"- {string.Format("{0:N2}", (correction.PriceTotalAfter - correction.PriceTotalBefore) * correction.Quantity)}\n";
                            break;
                        case "Jumlah":
                            datum.CorrectionNominal += $"- {string.Format("{0:N2}", correction.PriceTotalAfter)}\n";
                            break;
                        default:
                            break;
                    }
                }
            }

            return data;

        }

        public async Task<ReportFormatter> GetReport(string unitId, string categoryId, string divisionId, string budgetId, long prId, string createdBy, string status, DateTimeOffset startDate, DateTimeOffset endDate, DateTime? startDatePO, DateTime? endDatePO, long poExtId, string supplierId, int page, int size)
        {
            var query = GetReportQuery(unitId, categoryId, divisionId, budgetId, prId, createdBy, status, startDate, endDate, startDatePO, endDatePO, poExtId, supplierId, page, size);

            //var result = await query.OrderByDescending(order => order.InternalPurchaseOrderLastModifiedDate).Skip((page - 1) * size).Take(size).ToListAsync();
            query = await MapCorrections(query);

            return new ReportFormatter()
            {
                Data = query,
                Total = TotalCountReport
            };
        }

        private IQueryable<PurchaseRequestItem> FilterPurchaseRequest(string unitId, string categoryId, string divisionId, string budgetId, long prId, IQueryable<PurchaseRequestItem> purchaseRequestItems)
        {
            if (!string.IsNullOrWhiteSpace(unitId))
                purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.UnitId.Equals(unitId));

            if (!string.IsNullOrWhiteSpace(categoryId))
                purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.CategoryId.Equals(categoryId));

            if (!string.IsNullOrWhiteSpace(divisionId))
                purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.DivisionId.Equals(divisionId));

            if (!string.IsNullOrWhiteSpace(budgetId))
                purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.BudgetId.Equals(budgetId));

            if (prId > 0)
                purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.Id.Equals(prId));

            //if (!string.IsNullOrWhiteSpace(createdBy))
            //    purchaseRequestItems = purchaseRequestItems.Where(w => w.PurchaseRequest.CreatedBy.Equals(createdBy));

            return purchaseRequestItems;
        }

        public async Task<MemoryStream> GenerateExcel(string unitId, string categoryId, string divisionId, string budgetId, long prId, string createdBy, string status, DateTimeOffset startDate, DateTimeOffset endDate, DateTime? startDatePO, DateTime? endDatePO, long poExtId, string supplierId, int timezoneOffset)
        {

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembuatan PR", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Purchase Request", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang PR", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang PR", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang PO", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang PO", DataType = typeof(string) });

            result.Columns.Add(new DataColumn() { ColumnName = "Harga Barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Internal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Eksternal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembuatan PO Eksternal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Diminta Datang PO Eksternal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Target Datang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Datang Barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Surat Jalan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Surat Jalan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon Terima Unit", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Terima Unit", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Bon", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Bon", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tempo Pembayaran", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Intern", DataType = typeof(string) });

            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Intern", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Nota Intern", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Jatuh Tempo", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PPN", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPN", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPN", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PPH", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPH", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPH", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Koreksi", DataType = typeof(string) });

            result.Columns.Add(new DataColumn() { ColumnName = "No Koreksi", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Koreksi", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Koreksi", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff Pembelian", DataType = typeof(string) });

            var query = GetReportQuery(unitId, categoryId, divisionId, budgetId, prId, createdBy, status, startDate, endDate, startDatePO, endDatePO, poExtId, supplierId, 1, int.MaxValue);
            var queryResult = query.ToList();

            if (queryResult.Count == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", 0, "", "", "", "", "", "", 0, "", "", "", "", "", "", 0, "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                var upoDetailIds = queryResult.Select(item => item.UnitPaymentOrderDetailId).ToList();
                var corrections = await _correctionItemDbSet
                    .Include(item => item.UnitPaymentCorrectionNote)
                    .Where(item => upoDetailIds.Contains(item.UPODetailId))
                    .Select(item => new
                    {
                        item.UnitPaymentCorrectionNote.UPCNo,
                        item.UnitPaymentCorrectionNote.CorrectionType,
                        item.UnitPaymentCorrectionNote.CorrectionDate,
                        item.PriceTotalAfter,
                        item.PriceTotalBefore,
                        item.Quantity,
                        item.UPODetailId
                    }).ToListAsync();

                int index = 0;
                foreach (var item in queryResult)
                {
                    index++;

                    var selectedCorrections = corrections.Where(correction => correction.UPODetailId == item.UnitPaymentOrderDetailId).ToList();

                    item.CorrectionDate = string.Join("\n", selectedCorrections.Select(correction => $"- {correction.CorrectionDate.ToString("dd MMMM yyyy")}"));
                    item.CorrectionNo = string.Join("\n", selectedCorrections.Select(correction => $"- {correction.UPCNo}"));
                    item.CorrectionType = string.Join("\n", selectedCorrections.Select(correction => $"- {correction.CorrectionType}"));
                    item.CorrectionNominal = string.Join("\n", selectedCorrections.Select(correction =>
                    {
                        var nominalResult = "";
                        switch (correction.CorrectionType)
                        {
                            case "Harga Total":
                                nominalResult = $"- {string.Format("{0:N2}", correction.PriceTotalAfter - correction.PriceTotalBefore)}";
                                break;
                            case "Harga Satuan":
                                nominalResult = $"- {string.Format("{0:N2}", (correction.PriceTotalAfter - correction.PriceTotalBefore) * correction.Quantity)}";
                                break;
                            case "Jumlah":
                                nominalResult = $"- {string.Format("{0:N2}", correction.PriceTotalAfter)}";
                                break;
                            default:
                                break;
                        }
                        return nominalResult;
                    }));
                    //string prDate = item.prDate == null ? "-" : item.prDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string prCreatedDate = item.createdDatePR == null ? "-" : item.createdDatePR.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string receiptDatePO = item.receivedDatePO == null ? "-" : item.receivedDatePO.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDate = item.epoDate == new DateTime(1970, 1, 1) ? "-" : item.epoDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoCreatedDate = item.epoCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.epoCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoExpectedDeliveryDate = item.epoExpectedDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.epoExpectedDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDeliveryDate = item.epoDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.epoDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string doDate = item.doDate == new DateTime(1970, 1, 1) ? "-" : item.doDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string doDeliveryDate = item.doDeliveryDate == new DateTime(1970, 1, 1) ? "-" : item.doDeliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string urnDate = item.urnDate == new DateTime(1970, 1, 1) ? "-" : item.urnDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string invoiceDate = item.invoiceDate == new DateTime(1970, 1, 1) ? "-" : item.invoiceDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string upoDate = item.upoDate == new DateTime(1970, 1, 1) ? "-" : item.upoDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string dueDate = item.dueDate == new DateTime(1970, 1, 1) ? "-" : item.dueDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string vatDate = item.vatDate == new DateTime(1970, 1, 1) ? "-" : item.vatDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));

                    //string correctionDate = item.correctionDate == new DateTime(1970, 1, 1) ? "-" : item.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).Tostring("dd MMM yyyy", new CultureInfo("id-ID"));

                    var internalPurchaseOrderCreatedDate = item.InternalPurchaseOrderCreatedDate.HasValue ? item.InternalPurchaseOrderCreatedDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var externalPurchaseOrderDate = item.ExternalPurchaseOrderDate.HasValue ? item.ExternalPurchaseOrderDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var externalPurchaseOrderCreatedDate = item.ExternalPurchaseOrderCreatedDate.HasValue ? item.ExternalPurchaseOrderCreatedDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var externalPurchaseOrderExpectedDeliveryDate = item.ExternalPurchaseOrderExpectedDeliveryDate.HasValue ? item.ExternalPurchaseOrderExpectedDeliveryDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var externalPurchaseOrderDeliveryDate = item.ExternalPurchaseOrderDeliveryDate.HasValue ? item.ExternalPurchaseOrderDeliveryDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var deliveryOrderDate = item.DeliveryOrderDate.HasValue ? item.DeliveryOrderDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var deliveryOrderArrivalDate = item.DeliveryOrderArrivalDate.HasValue ? item.DeliveryOrderArrivalDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var unitReceiptNoteDate = item.UnitReceiptNoteDate.HasValue ? item.UnitReceiptNoteDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var unitPaymentOrderDate = item.UnitPaymentOrderDate.HasValue ? item.UnitPaymentOrderDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var unitPaymentOrderDueDate = item.UnitPaymentOrderDueDate.HasValue ? item.UnitPaymentOrderDueDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var unitPaymentOrderVATDate = item.UnitPaymentOrderVATDate.HasValue ? item.UnitPaymentOrderVATDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";
                    var unitPaymentOrderIncomeTaxDate = item.UnitPaymentOrderIncomeTaxDate.HasValue ? item.UnitPaymentOrderIncomeTaxDate.Value.AddHours(timezoneOffset).ToString("dd MMMM yyyy") : "";


                    result.Rows.Add(index.ToString(), item.PurchaseRequestDate.ToString("dd MMMM yyyy"), item.PurchaseRequestCreatedDate.ToString("dd MMMM yyyy"), item.PurchaseRequestNo, item.CategoryName, item.DivisionName, item.BudgetName, item.ProductName, item.ProductCode, item.OrderQuantity, item.UOMUnit, item.ExternalPurchaseOrderQuantity, item.ExternalPurchaseOrderUomUnit,
                        item.Price, item.PriceTotal, item.CurrencyCode, item.SupplierCode, item.SupplierName, internalPurchaseOrderCreatedDate, externalPurchaseOrderDate, externalPurchaseOrderCreatedDate, externalPurchaseOrderExpectedDeliveryDate, externalPurchaseOrderDeliveryDate, item.ExternalPurchaseOrderNo, deliveryOrderDate,
                        deliveryOrderArrivalDate, item.DeliveryOrderNo, item.doQuantity, item.doUom, unitReceiptNoteDate, item.UnitReceiptNoteNo, item.UnitReceiptNoteQuantity, item.UnitReceiptNoteUomUnit, item.ExternalPurchaseOrderPaymentDueDays, item.UnitPaymentOrderInvoiceDate, item.UnitPaymentOrderInvoiceNo, unitPaymentOrderDate,
                        item.UnitPaymentOrderNo, item.UnitPaymentOrderTotalPrice, unitPaymentOrderDueDate, unitPaymentOrderVATDate, item.UnitPaymentOrderVATNo, item.UnitPaymentOrderVAT, unitPaymentOrderIncomeTaxDate, item.UnitPaymentOrderIncomeTaxNo, item.UnitPaymentOrderIncomeTax, item.CorrectionDate,
                        item.CorrectionNo, item.CorrectionNominal, item.CorrectionType, item.ExternalPurchaseOrderRemark, item.InternalPurchaseOrderStatus, item.InternalPurchaseOrderStaff);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "sheet 1") }, true);
        }

    }


    public interface IPurchaseMonitoringService
    {
        Task<ReportFormatter> GetReport(string unitId, string categoryId, string divisionId, string budgetId, long prId, string createdBy, string status, DateTimeOffset startDate, DateTimeOffset endDate, DateTime? startDatePO, DateTime? endDatePO, long poExtId, string supplierId, int page, int size);
        Task<MemoryStream> GenerateExcel(string unitId, string categoryId, string divisionId, string budgetId, long prId, string createdBy, string status, DateTimeOffset startDate, DateTimeOffset endDate, DateTime? startDatePO, DateTime? endDatePO, long poExtId, string supplierId, int timezoneOffset);
    }

    public class ReportFormatter
    {
        public ReportFormatter()
        {

        }

        public List<PurchaseMonitoringReportViewModel> Data { get; set; }
        public int Total { get; set; }
    }

    public class PurchaseMonitoringReportViewModel
    {
        public PurchaseMonitoringReportViewModel()
        {
            CorrectionDate = "";
            CorrectionNo = "";
            CorrectionNominal = "";
            CorrectionType = "";
        }

        public long PurchaseRequestId { get; set; }
        public string PurchaseRequestNo { get; set; }
        public DateTime PurchaseRequestDate { get; set; }
        public DateTime PurchaseRequestCreatedDate { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string DivisionId { get; set; }
        public string DivisionName { get; set; }
        public string BudgetId { get; set; }
        public string BudgetName { get; set; }
        public long PurchaseRequestItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public double OrderQuantity { get; set; }
        public string UOMId { get; set; }
        public string UOMUnit { get; set; }
        public double Price { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public long InternalPurchaseOrderId { get; set; }
        public DateTime? InternalPurchaseOrderCreatedDate { get; set; }
        public DateTime? InternalPurchaseOrderLastModifiedDate { get; set; }
        public string InternalPurchaseOrderStaff { get; set; }
        public long InternalPurchaseOrderItemId { get; set; }
        public string InternalPurchaseOrderStatus { get; set; }
        public long ExternalPurchaseOrderId { get; set; }
        public DateTime? ExternalPurchaseOrderCreatedDate { get; set; }
        public DateTime? ExternalPurchaseOrderDate { get; set; }
        public DateTime? ExternalPurchaseOrderExpectedDeliveryDate { get; set; }
        public string ExternalPurchaseOrderNo { get; set; }
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ExternalPurchaseOrderPaymentDueDays { get; set; }
        public string ExternalPurchaseOrderRemark { get; set; }
        public double ExternalPurchaseOrderQuantity { get; set; }
        public string ExternalPurchaseOrderUomUnit { get; set; }
        public long ExternalPurchaseOrderItemId { get; set; }
        public long ExternalPurchaseOrderDetailId { get; set; }
        public long DeliveryOrderId { get; set; }
        public DateTime? DeliveryOrderDate { get; set; }
        public DateTime? DeliveryOrderArrivalDate { get; set; }
        public string DeliveryOrderNo { get; set; }
        public long DeliveryOrderItemId { get; set; }
        public long DelveryOrderDetailId { get; set; }
        public double doQuantity { get; set; }
        public string doUom { get; set; }
        public long UnitReceiptNoteId { get; set; }
        public DateTime? UnitReceiptNoteDate { get; set; }
        public string UnitReceiptNoteNo { get; set; }
        public long UnitReceiptNoteItemId { get; set; }
        public double UnitReceiptNoteQuantity { get; set; }
        public string UnitReceiptNoteUomId { get; set; }
        public string UnitReceiptNoteUomUnit { get; set; }
        public long UnitPaymentOrderId { get; set; }
        public DateTime? UnitPaymentOrderInvoiceDate { get; set; }
        public string UnitPaymentOrderInvoiceNo { get; set; }
        public DateTime? UnitPaymentOrderDate { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public DateTime? UnitPaymentOrderDueDate { get; set; }
        public long UnitPaymentOrderItemId { get; set; }
        public long UnitPaymentOrderDetailId { get; set; }
        public double UnitPaymentOrderTotalPrice { get; set; }
        public DateTime? UnitPaymentOrderVATDate { get; set; }
        public string UnitPaymentOrderVATNo { get; set; }
        public double UnitPaymentOrderVAT { get; set; }
        public DateTime? UnitPaymentOrderIncomeTaxDate { get; set; }
        public string UnitPaymentOrderIncomeTaxNo { get; set; }
        public double UnitPaymentOrderIncomeTax { get; set; }
        public string CorrectionDate { get; set; }
        public string CorrectionNo { get; set; }
        public string CorrectionType { get; set; }
        public string CorrectionNominal { get; set; }
        public double PriceTotal { get; internal set; }
        public DateTime? ExternalPurchaseOrderDeliveryDate { get; internal set; }
        public string InternalPurchaseOrderNo { get; internal set; }
    }


}
