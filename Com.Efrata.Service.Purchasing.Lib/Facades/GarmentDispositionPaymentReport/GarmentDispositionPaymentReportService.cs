using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPaymentReport
{
    public class GarmentDispositionPaymentReportService : IGarmentDispositionPaymentReportService
    {
        private readonly PurchasingDbContext _dbContext;

        public GarmentDispositionPaymentReportService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
        }

        public List<GarmentDispositionPaymentReportDto> GetReportByDate(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var dispositions = _dbContext.GarmentDispositionPurchases.Where(entity => entity.CreatedUtc >= startDate.DateTime && entity.CreatedUtc <= endDate.DateTime).ToList();

            var result = new List<GarmentDispositionPaymentReportDto>();
            if (dispositions.Count > 0)
            {
                var dispositionIds = dispositions.Select(element => element.Id).ToList();
                var dispositionItems = _dbContext.GarmentDispositionPurchaseItems.Where(entity => dispositionIds.Contains(entity.GarmentDispositionPurchaseId)).ToList();
                var dispositionItemIds = dispositionItems.Select(element => element.Id).ToList();
                var dispositionDetails = _dbContext.GarmentDispositionPurchaseDetailss.Where(entity => dispositionItemIds.Contains(entity.GarmentDispositionPurchaseItemId)).ToList();
                var epoIds = dispositionItems.Select(element => (long)element.EPOId).ToList();
                var externalPurchaseOrders = _dbContext.GarmentExternalPurchaseOrders.Where(entity => epoIds.Contains(entity.Id)).ToList();
                var deliveryOrderItems = _dbContext.GarmentDeliveryOrderItems.Where(entity => epoIds.Contains(entity.EPOId)).ToList();
                var deliveryOrderItemIds = deliveryOrderItems.Select(element => element.Id).ToList();
                var deliveryOrderIds = deliveryOrderItems.Select(element => element.GarmentDOId).ToList();
                var deliveryOrderDetails = _dbContext.GarmentDeliveryOrderDetails.Where(entity => deliveryOrderItemIds.Contains(entity.GarmentDOItemId)).ToList();
                var deliveryOrderDetailIds = deliveryOrderDetails.Select(entity => entity.Id).ToList();
                var deliveryOrders = _dbContext.GarmentDeliveryOrders.Where(entity => deliveryOrderIds.Contains(entity.Id)).ToList();
                var customItems = _dbContext.GarmentBeacukaiItems.Where(entity => deliveryOrderIds.Contains(entity.GarmentDOId)).ToList();
                var customIds = customItems.Select(entity => entity.BeacukaiId).ToList();
                var customs = _dbContext.GarmentBeacukais.Where(entity => customIds.Contains(entity.Id)).ToList();
                var unitReceiptNoteItems = _dbContext.GarmentUnitReceiptNoteItems.Where(entity => deliveryOrderDetailIds.Contains(entity.DODetailId)).ToList();
                var unitReceiptNoteIds = unitReceiptNoteItems.Select(element => element.URNId).ToList();
                var unitReceiptNotes = _dbContext.GarmentUnitReceiptNotes.Where(entity => unitReceiptNoteIds.Contains(entity.Id)).ToList();
                var internalNoteDetails = _dbContext.GarmentInternNoteDetails.Where(entity => deliveryOrderIds.Contains(entity.DOId)).ToList();
                var internalNoteItemIds = internalNoteDetails.Select(element => element.GarmentItemINId).ToList();
                var internalNoteItems = _dbContext.GarmentInternNoteItems.Where(entity => internalNoteItemIds.Contains(entity.Id)).ToList();
                var internalNoteIds = internalNoteItems.Select(element => element.GarmentINId).ToList();
                var internalNotes = _dbContext.GarmentInternNotes.Where(entity => internalNoteIds.Contains(entity.Id)).ToList();

                foreach (var dispositionItem in dispositionItems)
                {
                    var disposition = dispositions.FirstOrDefault(element => element.Id == dispositionItem.GarmentDispositionPurchaseId);
                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(element => element.Id == dispositionItem.EPOId);
                    var selectedDispoositionDetails = dispositionDetails.Where(element => element.GarmentDispositionPurchaseItemId == dispositionItem.Id).ToList();
                    var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(element => element.EPOId == dispositionItem.EPOId);
                    if (deliveryOrderItem == null)
                        deliveryOrderItem = new GarmentDeliveryOrderItem();

                    var deliveryOrder = deliveryOrders.FirstOrDefault(element => deliveryOrderItem.GarmentDOId == element.Id);
                    if (deliveryOrder == null)
                        deliveryOrder = new GarmentDeliveryOrder();

                    var selectedDeliveryOrderDetails = deliveryOrderDetails.Where(element => element.GarmentDOItemId == deliveryOrderItem.Id).ToList();
                    var selectedDeliveryOrderDetailIds = selectedDeliveryOrderDetails.Select(element => element.Id).ToList();

                    var customItem = customItems.FirstOrDefault(element => element.GarmentDOId == deliveryOrder.Id);
                    if (customItem == null)
                        customItem = new GarmentBeacukaiItem();

                    var custom = customs.FirstOrDefault(element => customItem.BeacukaiId == element.Id);
                    if (custom == null)
                        custom = new GarmentBeacukai();

                    var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(element => selectedDeliveryOrderDetailIds.Contains(element.DODetailId));
                    if (unitReceiptNoteItem == null)
                        unitReceiptNoteItem = new GarmentUnitReceiptNoteItem();

                    var unitReceiptNote = unitReceiptNotes.FirstOrDefault(element => element.Id == unitReceiptNoteItem.URNId);
                    if (unitReceiptNote == null)
                        unitReceiptNote = new GarmentUnitReceiptNote();

                    var internalNoteDetail = internalNoteDetails.FirstOrDefault(element => element.DOId == deliveryOrder.Id);
                    if (internalNoteDetail == null)
                        internalNoteDetail = new GarmentInternNoteDetail();

                    var internalNoteItem = internalNoteItems.FirstOrDefault(element => element.Id == internalNoteDetail.GarmentItemINId);
                    if (internalNoteItem == null)
                        internalNoteItem = new GarmentInternNoteItem();

                    var internalNote = internalNotes.FirstOrDefault(element => element.Id == internalNoteItem.GarmentINId);
                    if (internalNote == null)
                        internalNote = new GarmentInternNote();

                    var customDate = (DateTimeOffset?)null;
                    if (custom.Id > 0)
                        customDate = custom.BeacukaiDate;

                    var internalNoteDate = (DateTimeOffset?)null;
                    if (internalNote.Id > 0)
                        internalNoteDate = internalNote.INDate;

                    result.Add(new GarmentDispositionPaymentReportDto(
                        disposition.Id,
                        disposition.DispositionNo,
                        disposition.CreatedUtc,
                        disposition.DueDate,
                        disposition.InvoiceProformaNo,
                        disposition.SupplierId,
                        disposition.SupplierCode,
                        disposition.SupplierName,
                        disposition.CurrencyId,
                        dispositionItem.CurrencyCode,
                        dispositionItem.CurrencyRate,
                        disposition.Dpp,
                        disposition.VAT,
                        disposition.IncomeTax,
                        disposition.OtherCost,
                        disposition.Amount,
                        0,
                        disposition.Category,
                        disposition.Category,
                        externalPurchaseOrder != null? (int)externalPurchaseOrder.Id:0,
                        externalPurchaseOrder != null? externalPurchaseOrder.EPONo:string.Empty,
                        selectedDispoositionDetails.Sum(sum => sum.QTYPaid),
                        deliveryOrder.Id > 0 ? (int)deliveryOrder.Id : 0,
                        deliveryOrder.Id > 0 ? deliveryOrder.DONo : "",
                        deliveryOrder.Id > 0 ? selectedDeliveryOrderDetails.Sum(sum => sum.DOQuantity) : 0,
                        deliveryOrder.Id > 0 ? deliveryOrder.PaymentBill : "",
                        deliveryOrder.Id > 0 ? deliveryOrder.BillNo : "",
                        custom.Id > 0 ? (int)custom.Id : 0,
                        custom.Id > 0 ? custom.BeacukaiNo : "",
                        customDate,
                        unitReceiptNote.Id > 0 ? (int)unitReceiptNote.Id : 0,
                        unitReceiptNote.Id > 0 ? unitReceiptNote.URNNo : "",
                        internalNote.Id > 0 ? (int)internalNote.Id : 0,
                        internalNote.Id > 0 ? internalNote.INNo : "",
                        internalNoteDate,
                        disposition.CreatedBy
                        ));
                }
            }

            return result;
        }

        public List<GarmentDispositionPaymentReportDto> GetReportByDispositionIds(List<int> dispositionIds)
        {
            var dispositions = _dbContext.GarmentDispositionPurchases.Where(entity => dispositionIds.Contains(entity.Id)).ToList();

            var result = new List<GarmentDispositionPaymentReportDto>();
            if (dispositions.Count > 0)
            {
                //var dispositionIds = dispositions.Select(element => element.Id).ToList();
                var dispositionItems = _dbContext.GarmentDispositionPurchaseItems.Where(entity => dispositionIds.Contains(entity.GarmentDispositionPurchaseId)).ToList();
                var dispositionItemIds = dispositionItems.Select(element => element.Id).ToList();
                var dispositionDetails = _dbContext.GarmentDispositionPurchaseDetailss.Where(entity => dispositionItemIds.Contains(entity.GarmentDispositionPurchaseItemId)).ToList();
                var epoIds = dispositionItems.Select(element => (long)element.EPOId).ToList();
                var externalPurchaseOrders = _dbContext.GarmentExternalPurchaseOrders.Where(entity => epoIds.Contains(entity.Id)).ToList();
                var deliveryOrderItems = _dbContext.GarmentDeliveryOrderItems.Where(entity => epoIds.Contains(entity.EPOId)).ToList();
                var deliveryOrderItemIds = deliveryOrderItems.Select(element => element.Id).ToList();
                //var dispositionItemIds = dispositionItems.Select(entity => entity.Id).ToList();
                var dispositionItemDetails = _dbContext.GarmentDispositionPurchaseDetailss.Where(entity => dispositionItemIds.Contains(entity.GarmentDispositionPurchaseItemId)).ToList();
                var deliveryOrderIds = deliveryOrderItems.Select(element => element.GarmentDOId).ToList();
                var deliveryOrderDetails = _dbContext.GarmentDeliveryOrderDetails.Where(entity => deliveryOrderItemIds.Contains(entity.GarmentDOItemId)).ToList();
                var deliveryOrderDetailIds = deliveryOrderDetails.Select(entity => entity.Id).ToList();
                var deliveryOrders = _dbContext.GarmentDeliveryOrders.Where(entity => deliveryOrderIds.Contains(entity.Id)).ToList();
                var customItems = _dbContext.GarmentBeacukaiItems.Where(entity => deliveryOrderIds.Contains(entity.GarmentDOId)).ToList();
                var customIds = customItems.Select(entity => entity.BeacukaiId).ToList();
                var customs = _dbContext.GarmentBeacukais.Where(entity => customIds.Contains(entity.Id)).ToList();
                var unitReceiptNoteItems = _dbContext.GarmentUnitReceiptNoteItems.Where(entity => deliveryOrderDetailIds.Contains(entity.DODetailId)).ToList();
                var unitReceiptNoteIds = unitReceiptNoteItems.Select(element => element.URNId).ToList();
                var unitReceiptNotes = _dbContext.GarmentUnitReceiptNotes.Where(entity => unitReceiptNoteIds.Contains(entity.Id)).ToList();
                var internalNoteDetails = _dbContext.GarmentInternNoteDetails.Where(entity => deliveryOrderIds.Contains(entity.DOId)).ToList();
                var internalNoteItemIds = internalNoteDetails.Select(element => element.GarmentItemINId).ToList();
                var internalNoteItems = _dbContext.GarmentInternNoteItems.Where(entity => internalNoteItemIds.Contains(entity.Id)).ToList();
                var internalNoteIds = internalNoteItems.Select(element => element.GarmentINId).ToList();
                var internalNotes = _dbContext.GarmentInternNotes.Where(entity => internalNoteIds.Contains(entity.Id)).ToList();

                foreach (var dispositionItem in dispositionItems)
                {
                    var disposition = dispositions.FirstOrDefault(element => element.Id == dispositionItem.GarmentDispositionPurchaseId);
                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(element => element.Id == dispositionItem.EPOId);
                    var selectedDispoositionDetails = dispositionDetails.Where(element => element.GarmentDispositionPurchaseItemId == dispositionItem.Id).ToList();
                    var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(element => element.EPOId == dispositionItem.EPOId);
                    if (deliveryOrderItem == null)
                        deliveryOrderItem = new GarmentDeliveryOrderItem();

                    var deliveryOrder = deliveryOrders.FirstOrDefault(element => deliveryOrderItem.GarmentDOId == element.Id);
                    if (deliveryOrder == null)
                        deliveryOrder = new GarmentDeliveryOrder();

                    var selectedDeliveryOrderDetails = deliveryOrderDetails.Where(element => element.GarmentDOItemId == deliveryOrderItem.Id).ToList();
                    var selectedDeliveryOrderDetailIds = selectedDeliveryOrderDetails.Select(element => element.Id).ToList();

                    var customItem = customItems.FirstOrDefault(element => element.GarmentDOId == deliveryOrder.Id);
                    if (customItem == null)
                        customItem = new GarmentBeacukaiItem();

                    var custom = customs.FirstOrDefault(element => customItem.BeacukaiId == element.Id);
                    if (custom == null)
                        custom = new GarmentBeacukai();

                    var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(element => selectedDeliveryOrderDetailIds.Contains(element.DODetailId));
                    if (unitReceiptNoteItem == null)
                        unitReceiptNoteItem = new GarmentUnitReceiptNoteItem();

                    var unitReceiptNote = unitReceiptNotes.FirstOrDefault(element => element.Id == unitReceiptNoteItem.URNId);
                    if (unitReceiptNote == null)
                        unitReceiptNote = new GarmentUnitReceiptNote();

                    var internalNoteDetail = internalNoteDetails.FirstOrDefault(element => element.DOId == deliveryOrder.Id);
                    if (internalNoteDetail == null)
                        internalNoteDetail = new GarmentInternNoteDetail();

                    var internalNoteItem = internalNoteItems.FirstOrDefault(element => element.Id == internalNoteDetail.GarmentItemINId);
                    if (internalNoteItem == null)
                        internalNoteItem = new GarmentInternNoteItem();

                    var internalNote = internalNotes.FirstOrDefault(element => element.Id == internalNoteItem.GarmentINId);
                    if (internalNote == null)
                        internalNote = new GarmentInternNote();

                    var customDate = (DateTimeOffset?)null;
                    if (custom.Id > 0)
                        customDate = custom.BeacukaiDate;

                    var internalNoteDate = (DateTimeOffset?)null;
                    if (internalNote.Id > 0)
                        internalNoteDate = internalNote.INDate;

                    var sumDispositionPaid = dispositionItemDetails.Where(element => element.GarmentDispositionPurchaseItemId == dispositionItem.Id).Sum(detail => detail.QTYPaid);


                    result.Add(new GarmentDispositionPaymentReportDto(
                        disposition.Id,
                        disposition.DispositionNo,
                        disposition.CreatedUtc,
                        disposition.DueDate,
                        disposition.InvoiceProformaNo,
                        disposition.SupplierId,
                        disposition.SupplierCode,
                        disposition.SupplierName,
                        disposition.CurrencyId,
                        dispositionItem.CurrencyCode,
                        dispositionItem.CurrencyRate,
                        disposition.Dpp,
                        disposition.VAT,
                        disposition.IncomeTax,
                        disposition.OtherCost,
                        disposition.Amount,
                        0,
                        disposition.Category,
                        disposition.Category,
                        //(int)externalPurchaseOrder.Id,
                        //externalPurchaseOrder.EPONo,
                        externalPurchaseOrder != null ? (int)externalPurchaseOrder.Id : 0,
                        externalPurchaseOrder != null ? externalPurchaseOrder.EPONo : string.Empty,
                        //dispositionItem.DispositionQuantityPaid,
                        sumDispositionPaid,
                        deliveryOrder.Id > 0 ? (int)deliveryOrder.Id : 0,
                        deliveryOrder.Id > 0 ? deliveryOrder.DONo : "",
                        deliveryOrder.Id > 0 ? selectedDeliveryOrderDetails.Sum(sum => sum.DOQuantity) : 0,
                        deliveryOrder.Id > 0 ? deliveryOrder.PaymentBill : "",
                        deliveryOrder.Id > 0 ? deliveryOrder.BillNo : "",
                        custom.Id > 0 ? (int)custom.Id : 0,
                        custom.Id > 0 ? custom.BeacukaiNo : "",
                        customDate,
                        unitReceiptNote.Id > 0 ? (int)unitReceiptNote.Id : 0,
                        unitReceiptNote.Id > 0 ? unitReceiptNote.URNNo : "",
                        internalNote.Id > 0 ? (int)internalNote.Id : 0,
                        internalNote.Id > 0 ? internalNote.INNo : "",
                        internalNoteDate,
                        disposition.CreatedBy
                        ));
                }
            }

            return result;
        }
    }
}
