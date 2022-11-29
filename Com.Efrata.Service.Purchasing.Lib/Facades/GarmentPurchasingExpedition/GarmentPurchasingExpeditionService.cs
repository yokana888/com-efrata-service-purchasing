using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    //TODO : Adding API for View Nomor Internal yang memiliki TaxIncome, Tax Rate dan lain lain
    public class GarmentPurchasingExpeditionService : IGarmentPurchasingExpeditionService
    {
        private const string UserAgent = "purchasing-service";
        private readonly PurchasingDbContext _dbContext;
        private readonly IdentityService _identityService;

        public GarmentPurchasingExpeditionService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
            _identityService = serviceProvider.GetService<IdentityService>();
        }

        public List<GarmentDispositionNoteDto> GetGarmentDispositionNotes(string keyword, PurchasingGarmentExpeditionPosition position)
        {
            var query = _dbContext.GarmentDispositionPurchases.AsQueryable();

            if (position > 0)
                query = query.Where(entity => entity.Position == position);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.DispositionNo.Contains(keyword));

            var result = new List<GarmentDispositionNoteDto>();
            var queryResult = query
                .Take(10)
                .ToList();
            var dispositionIds = queryResult.Select(element => element.Id).ToList();
            var dispositionItems = _dbContext.GarmentDispositionPurchaseItems.Where(entity => dispositionIds.Contains(entity.GarmentDispositionPurchaseId)).ToList();
            var dispositionItemIds = dispositionItems.Select(entity => entity.Id).ToList();
            var dispositionDetails = _dbContext.GarmentDispositionPurchaseDetailss.Where(entity => dispositionItemIds.Contains(entity.GarmentDispositionPurchaseItemId)).ToList();
            foreach (var dispositionNote in queryResult)
            {
                var items = dispositionItems.Where(element => element.GarmentDispositionPurchaseId == dispositionNote.Id).ToList();

                var resultItems = new List<GarmentDispositionNoteItemDto>();

                foreach (var item in items)
                {
                    var details = dispositionDetails.Where(element => element.GarmentDispositionPurchaseItemId == item.Id).ToList();

                    foreach (var detail in details)
                    {
                        resultItems.Add(new GarmentDispositionNoteItemDto(detail.UnitId, detail.UnitCode, detail.UnitName, detail.ProductId, detail.ProductName, detail.QTYPaid, detail.PricePerQTY));
                    }
                }  

                result.Add(new GarmentDispositionNoteDto(dispositionNote.Id, dispositionNote.DispositionNo, dispositionNote.CreatedUtc.ToUniversalTime(), dispositionNote.DueDate, dispositionNote.SupplierId, dispositionNote.SupplierCode, dispositionNote.SupplierName, dispositionNote.VAT, dispositionNote.VAT, dispositionNote.IncomeTax, dispositionNote.IncomeTax, dispositionNote.Amount, dispositionNote.Amount, dispositionNote.CurrencyId, dispositionNote.CurrencyName, 0, dispositionNote.Dpp, dispositionNote.Dpp, resultItems, dispositionNote.InvoiceProformaNo,dispositionNote.Category));
            }

            return result;
        }

        public List<GarmentDispositionNoteDto> GetGarmentDispositionNotesVerification(string keyword, PurchasingGarmentExpeditionPosition position)
        {
            var query = _dbContext.GarmentDispositionPurchases.Where(entity => entity.Position == PurchasingGarmentExpeditionPosition.Purchasing || entity.Position == PurchasingGarmentExpeditionPosition.SendToPurchasing).AsQueryable();

            if (position > 0)
                query = query.Where(entity => entity.Position == position);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.DispositionNo.Contains(keyword));

            var result = new List<GarmentDispositionNoteDto>();
            var queryResult = query
                .Take(10)
                .ToList();
            var dispositionIds = queryResult.Select(element => element.Id).ToList();
            var dispositionItems = _dbContext.GarmentDispositionPurchaseItems.Where(entity => dispositionIds.Contains(entity.GarmentDispositionPurchaseId)).ToList();
            var dispositionItemIds = dispositionItems.Select(entity => entity.Id).ToList();
            var dispositionDetails = _dbContext.GarmentDispositionPurchaseDetailss.Where(entity => dispositionItemIds.Contains(entity.GarmentDispositionPurchaseItemId)).ToList();
            foreach (var dispositionNote in queryResult)
            {
                var items = dispositionItems.Where(element => element.GarmentDispositionPurchaseId == dispositionNote.Id).ToList();

                var resultItems = new List<GarmentDispositionNoteItemDto>();

                foreach (var item in items)
                {
                    var details = dispositionDetails.Where(element => element.GarmentDispositionPurchaseItemId == item.Id).ToList();

                    foreach (var detail in details)
                    {
                        resultItems.Add(new GarmentDispositionNoteItemDto(detail.UnitId, detail.UnitCode, detail.UnitName, detail.ProductId, detail.ProductName, detail.QTYPaid, detail.PricePerQTY));
                    }
                }

                result.Add(new GarmentDispositionNoteDto(dispositionNote.Id, dispositionNote.DispositionNo, dispositionNote.CreatedUtc.ToUniversalTime(), dispositionNote.DueDate, dispositionNote.SupplierId, dispositionNote.SupplierCode, dispositionNote.SupplierName, dispositionNote.VAT, dispositionNote.VAT, dispositionNote.IncomeTax, dispositionNote.IncomeTax, dispositionNote.Amount, dispositionNote.Amount, dispositionNote.CurrencyId, dispositionNote.CurrencyName, 0, dispositionNote.Dpp, dispositionNote.Dpp, resultItems, dispositionNote.InvoiceProformaNo, dispositionNote.Category));
            }

            return result;
        }

        public List<GarmentInternalNoteDto> GetGarmentInternalNotes(string keyword, GarmentInternalNoteFilterDto filter)
        {
            //var internalNoteQuery = _dbContext.GarmentInternNotes.Where(entity => entity.Position <= PurchasingGarmentExpeditionPosition.Purchasing || entity.Position == PurchasingGarmentExpeditionPosition.SendToPurchasing);
            var internalNoteQuery = _dbContext.GarmentInternNotes.AsQueryable();
            if (filter.PositionIds == null)
                internalNoteQuery = internalNoteQuery.Where(entity => entity.Position <= PurchasingGarmentExpeditionPosition.Purchasing);
            else
                internalNoteQuery = internalNoteQuery.Where(entity => filter.PositionIds.Contains((int)entity.Position));

            if (!string.IsNullOrWhiteSpace(keyword))
                internalNoteQuery = internalNoteQuery.Where(entity => entity.INNo.Contains(keyword));

            var internalNotes = internalNoteQuery.Select(entity => new
            {
                entity.Id,
                entity.INNo,
                entity.INDate,
                entity.SupplierId,
                entity.SupplierName,
                entity.CurrencyCode,
                entity.CurrencyId
            }).Take(10).ToList();

            var internalNoteIds = internalNotes.Select(element => element.Id).ToList();
            var internalNoteItems = _dbContext.GarmentInternNoteItems.Where(entity => internalNoteIds.Contains(entity.GarmentINId)).Select(entity => new { entity.Id, entity.GarmentINId, entity.InvoiceId }).ToList();
            var internalNoteItemIds = internalNoteItems.Select(element => element.Id).ToList();
            var internalNoteDetails = _dbContext.GarmentInternNoteDetails.Where(entity => internalNoteItemIds.Contains(entity.GarmentItemINId)).Select(entity => new { entity.Id, entity.GarmentItemINId, entity.PaymentDueDate, entity.DOId, entity.PaymentType, entity.PaymentMethod, entity.PaymentDueDays }).ToList();

            var doIds = internalNoteDetails.Select(element => element.DOId).ToList();
            var corrections = _dbContext.GarmentCorrectionNotes.Where(entity => doIds.Contains(entity.DOId)).Select(entity => new { entity.Id, entity.TotalCorrection, entity.CorrectionType, entity.DOId, entity.UseIncomeTax, entity.UseVat, entity.IncomeTaxRate ,entity.VatRate});
            var correctionIds = corrections.Select(element => element.Id).ToList();
            var correctionItems = _dbContext.GarmentCorrectionNoteItems.Where(entity => correctionIds.Contains(entity.GCorrectionId)).Select(entity => new { entity.Id, entity.PricePerDealUnitAfter, entity.Quantity, entity.GCorrectionId });

            var invoiceIds = internalNoteItems.Select(element => element.InvoiceId).ToList();
            var invoices = _dbContext.GarmentInvoices.Where(entity => invoiceIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.IsPayTax, entity.IsPayVat, entity.UseIncomeTax, entity.UseVat, entity.IncomeTaxRate, entity.TotalAmount, entity.InvoiceNo, entity.VatRate }).ToList();

            var result = internalNotes.Select(internalNote =>
            {
                var selectedInternalNoteItems = internalNoteItems.Where(element => element.GarmentINId == internalNote.Id).ToList();
                var selectedInternalNoteItemIds = selectedInternalNoteItems.Select(element => element.Id).ToList();
                var selectedInvoiceIds = selectedInternalNoteItems.Select(element => element.InvoiceId).ToList();
                var internalNoteDetail = internalNoteDetails.Where(element => selectedInternalNoteItemIds.Contains(element.GarmentItemINId)).OrderByDescending(element => element.PaymentDueDate).FirstOrDefault();

                var selectedInternalNoteDetails = internalNoteDetails.Where(element => selectedInternalNoteItemIds.Contains(element.GarmentItemINId)).ToList();
                var selectedDOIds = selectedInternalNoteDetails.Select(element => element.DOId).ToList();
                var selectedCorrections = corrections.Where(element => selectedDOIds.Contains(element.DOId)).ToList();

                var amountDPP = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Sum(element => element.TotalAmount);
                var invoicesNo = string.Join('\n', invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Select(element => $"- {element.InvoiceNo}"));

                var correctionAmount = selectedCorrections.Sum(element =>
                {
                    var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);
                    var garmentInvoicesIds = internalNoteItems.Where(item => item.Id == internalNoteDetails.Where(i => i.DOId == element.DOId).Select(x => x.GarmentItemINId).FirstOrDefault()).Select(entity => entity.InvoiceId);
                    var garmentInvoicesCorrection = invoices.Where(item => garmentInvoicesIds.Contains(item.Id)).FirstOrDefault();

                    var total = 0.0;
                    if (element.CorrectionType.ToUpper() == "RETUR")
                        total = (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity);
                    else
                        total = (double)element.TotalCorrection;

                    if (garmentInvoicesCorrection.UseVat && garmentInvoicesCorrection.IsPayVat)
                        total += total * (Convert.ToDouble(element.VatRate)/100);

                    if (garmentInvoicesCorrection.UseIncomeTax && garmentInvoicesCorrection.IsPayTax)
                        total -= total * (double)(element.IncomeTaxRate / 100);

                    return total;
                });

                var totalCorrection = selectedCorrections.Sum(element =>
                {
                    var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);
                    var garmentInvoicesIds = internalNoteItems.Where(item => item.Id == internalNoteDetails.Where(i => i.DOId == element.DOId).Select(x => x.GarmentItemINId).FirstOrDefault()).Select(entity => entity.InvoiceId);
                    var garmentInvoicesCorrection = invoices.Where(item => garmentInvoicesIds.Contains(item.Id)).FirstOrDefault();

                    var total = element.CorrectionType.ToUpper() == "RETUR" ? (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity) : (double)element.TotalCorrection;

                    if (garmentInvoicesCorrection.UseVat && garmentInvoicesCorrection.IsPayVat)
                    {
                        total += element.CorrectionType.ToUpper() == "RETUR" ? (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity) * (Convert.ToDouble(element.VatRate) / 100) : (double)element.TotalCorrection * (Convert.ToDouble(element.VatRate) / 100);
                    }

                    if (garmentInvoicesCorrection.UseIncomeTax && garmentInvoicesCorrection.IsPayTax)
                    {
                        total -= element.CorrectionType.ToUpper() == "RETUR" ? (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity) * (double)(element.IncomeTaxRate / 100) : (double)element.TotalCorrection * (double)(element.IncomeTaxRate / 100);
                    }

                    return total;
                });

                var totalAmount = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Sum(element =>
                {
                    var total = element.TotalAmount;

                    if (element.UseVat && element.IsPayVat)
                    {
                        total += element.TotalAmount * (Convert.ToDouble(element.VatRate) / 100);
                    }
                    
                    if (element.UseIncomeTax && element.IsPayTax)
                    {
                        total -= element.TotalAmount * (element.IncomeTaxRate / 100);
                    }
                    
                    return total;
                });
                totalAmount += totalCorrection;

                var vatCorrection = selectedCorrections.Sum(element =>
                {
                    var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);

                    var total = 0.0;

                    if (element.UseVat)
                    {
                        total += element.CorrectionType.ToUpper() == "RETUR" ? (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity) * (Convert.ToDouble(element.VatRate) / 100) : (double)element.TotalCorrection * (Convert.ToDouble(element.VatRate) / 100);
                    }
                    
                    return total;
                });

                var vatTotal = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Sum(element =>
                {
                    var vat = 0.0;

                    if (element.UseVat && element.IsPayVat)
                    {
                        vat += element.TotalAmount * (Convert.ToDouble(element.VatRate) / 100);
                    }
                    
                    return vat;
                });

                if (vatTotal != 0)
                {
                    vatTotal += vatCorrection;
                }

                var correctionIncomeTax = selectedCorrections.Sum(element =>
                {
                    var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);

                    var total = 0.0;

                    if (element.UseIncomeTax)
                        total += element.CorrectionType.ToUpper() == "RETUR" ? (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity) * (double)(element.IncomeTaxRate / 100) : (double)element.TotalCorrection * (double)(element.IncomeTaxRate / 100);

                    return total;
                });

                var incomeTaxTotal = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Sum(element =>
                {
                    var incomeTax = 0.0;

                    if (element.UseIncomeTax && element.IsPayTax)
                    {
                        incomeTax += element.TotalAmount * (element.IncomeTaxRate / 100);
                    }

                    return incomeTax;
                });

                if (incomeTaxTotal != 0)
                {
                    incomeTaxTotal += correctionIncomeTax;
                }
                
                //var productInvoice = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Select(element =>
                //{
                //    var firstProduct = element.entity.Items;
                //    if (firstProduct != null)
                //        return firstProduct.FirstOrDefault().Details.Select(details => new { details.ProductId, details.ProductName, details.ProductCode }).FirstOrDefault();
                //    else
                //        return null;
                //});

                //var productInvoiceFirst = productInvoice.Count() <= 0 ? null : productInvoice.FirstOrDefault();
                //var productNameInvoiceFirst = productInvoiceFirst == null ? string.Empty : productInvoiceFirst.ProductName;
                //var productIdInvoiceFirst = productInvoiceFirst == null ? 0 : productInvoiceFirst.ProductId;
                //var productCodeInvoiceFirst = productInvoiceFirst == null ? string.Empty : productInvoiceFirst.ProductCode;

                var invoiceFirst = invoices.FirstOrDefault();
                return new GarmentInternalNoteDto((int)internalNote.Id, internalNote.INNo, internalNote.INDate, internalNoteDetail.PaymentDueDate, (int)internalNote.SupplierId, internalNote.SupplierName, vatTotal, incomeTaxTotal, totalAmount, (int)internalNote.CurrencyId, internalNote.CurrencyCode, amountDPP, correctionAmount, internalNoteDetail.PaymentType, internalNoteDetail.PaymentMethod, internalNoteDetail.PaymentDueDays, invoicesNo);
            }).ToList();

            return result;
        }

        public List<GarmentInternalNoteDetailsDto> GetGarmentInternNotesDetails(string keyword, GarmentInternalNoteFilterDto filter)
        {
            var invoiceInfo = _dbContext.GarmentInvoiceDetails.Include(t => t.GarmentInvoiceItem).ThenInclude(t => t.GarmentInvoice);
            var deliveryOrderInfo = _dbContext.GarmentDeliveryOrders;
            var externalPurchasOrder = _dbContext.GarmentExternalPurchaseOrders;
            var invoiceInfoHeader = _dbContext.GarmentInvoices;
            var internalNoteQuery = _dbContext.GarmentInternNotes.Include(s => s.Items).ThenInclude(s => s.Details).Where(s => !s.IsPphPaid)
                .Select(element =>
                new GarmentInternalNoteDetailsDto
                {
                    Active = element.Active,
                    DeletedAgent = element.DeletedAgent,
                    CreatedAgent = element.CreatedAgent,
                    LastModifiedAgent = element.LastModifiedAgent,
                    CreatedBy = element.CreatedBy,
                    CreatedUtc = element.CreatedUtc,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId.GetValueOrDefault(),
                    CurrencyRate = element.CurrencyRate,
                    DeletedBy = element.DeletedBy,
                    DeletedUtc = element.DeletedUtc,
                    Id = element.Id,
                    INDate = element.INDate,
                    INId = element.Id,
                    INNo = element.INNo,
                    IsCreatedVB = element.IsCreatedVB,
                    SupplierId = element.SupplierId.GetValueOrDefault(),
                    SupplierName = element.SupplierName,
                    IsDeleted = element.IsDeleted,
                    SupplierCode = element.SupplierCode,
                    LastModifiedBy = element.LastModifiedBy,
                    LastModifiedUtc = element.LastModifiedUtc,
                    Position = element.Position,
                    Remark = element.Remark,
                    UId = element.UId,
                    INDueDate =
                    element.Items == null ? DateTimeOffset.MinValue :
                        element.Items.SelectMany(item => item.Details).Count() <= 0 ? DateTimeOffset.MinValue :
                            element.Items.SelectMany(item => item.Details).OrderByDescending(detail => detail.PaymentDueDate).FirstOrDefault().PaymentDueDate,
                    TotalAmount = element.Items == null ? 0 :
                        element.Items.Sum(item => item.TotalAmount),
                    Items = element.Items == null ? new List<GarmentInternNoteItemsInvoiceDto>() :
                    element.Items.Select(elementItem =>
                        new GarmentInternNoteItemsInvoiceDto
                        {
                            Active = elementItem.Active,
                            CreatedAgent = elementItem.CreatedAgent,
                            CreatedBy = elementItem.CreatedBy,
                            LastModifiedAgent = elementItem.LastModifiedAgent,
                            TotalAmount = elementItem.TotalAmount,
                            CreatedUtc = elementItem.CreatedUtc,
                            DeletedAgent = elementItem.DeletedAgent,
                            DeletedBy = elementItem.DeletedBy,
                            DeletedUtc = elementItem.DeletedUtc,
                            Id = elementItem.Id,
                            InvoiceDate = elementItem.InvoiceDate,
                            InvoiceId = elementItem.InvoiceId,
                            InvoiceNo = elementItem.InvoiceNo,
                            IsDeleted = elementItem.IsDeleted,
                            LastModifiedBy = elementItem.LastModifiedBy,
                            LastModifiedUtc = elementItem.LastModifiedUtc,
                            GarmentInvoice = invoiceInfoHeader.FirstOrDefault(s => s.Id == elementItem.InvoiceId),
                            TotalIncomeTax = _dbContext.GarmentInvoices.FirstOrDefault(s => s.Id == elementItem.InvoiceId) == null ? 0 :
                            (_dbContext.GarmentInvoices.FirstOrDefault(s => s.Id == elementItem.InvoiceId).IncomeTaxRate / 100) * elementItem.TotalAmount,
                            Details = elementItem.Details == null ? new List<GarmentNoteItemsInvoiceDetailsDto>() :
                            elementItem.Details.Select(elementDetails =>
                            new GarmentNoteItemsInvoiceDetailsDto
                            {
                                DOId = elementDetails.DOId,
                                DONo = elementDetails.DONo,
                                EPOId = elementDetails.EPOId,
                                EPONo = elementDetails.EPONo,
                                POSerialNumber = elementDetails.POSerialNumber,
                                RONo = elementDetails.RONo,
                                PaymentMethod = elementDetails.PaymentMethod,
                                PaymentType = elementDetails.PaymentType,
                                PaymentDueDays = elementDetails.PaymentDueDays,
                                PaymentDueDate = elementDetails.PaymentDueDate,
                                DODate = elementDetails.DODate,
                                InvoiceDetailId = elementDetails.InvoiceDetailId,
                                ProductCode = elementDetails.ProductCode,
                                ProductId = elementDetails.ProductId,
                                ProductName = elementDetails.ProductName,
                                ProductCategory = externalPurchasOrder.FirstOrDefault(s => s.Id == elementDetails.EPOId).Category,
                                Quantity = elementDetails.Quantity,
                                UnitId = elementDetails.UnitId,
                                UnitCode = elementDetails.UnitCode,
                                UnitName = elementDetails.UnitName,
                                UOMId = elementDetails.UOMId,
                                UOMUnit = elementDetails.UOMUnit,
                                PricePerDealUnit = elementDetails.PricePerDealUnit,
                                PriceTotal = elementDetails.PriceTotal,
                                InvoiceNo = elementItem.InvoiceNo,
                                InvoiceId = elementItem.InvoiceId,
                                InvoiceDate = elementItem.InvoiceDate,
                                InvoiceTotalAmount = elementItem.TotalAmount,
                                GarmentInvoiceDetail = invoiceInfo.FirstOrDefault(s => s.Id == elementDetails.InvoiceDetailId),
                                GarmentDeliveryOrder = deliveryOrderInfo.FirstOrDefault(s => s.Id == elementDetails.DOId),

                            }).ToList()
                            //elementItem.Details.GroupBy(
                            //    productKey => productKey.DOId,
                            //    grpProduct => grpProduct,
                            //    (productKey, grpProduct) =>
                            //new GarmentNoteItemsInvoiceDetailsDto
                            //{
                            //    DOId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().DOId,
                            //    DONo = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().DONo,
                            //    EPOId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().EPOId,
                            //    EPONo = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().EPONo,
                            //    POSerialNumber = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().POSerialNumber,
                            //    RONo = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().RONo,
                            //    PaymentMethod = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PaymentMethod,
                            //    PaymentType = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PaymentType,
                            //    PaymentDueDays = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PaymentDueDays,
                            //    PaymentDueDate = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PaymentDueDate,
                            //    DODate = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().DODate,
                            //    InvoiceDetailId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().InvoiceDetailId,
                            //    ProductCode = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().ProductCode,
                            //    ProductId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().ProductId.GetValueOrDefault(),
                            //    ProductName = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().ProductName,
                            //    Quantity = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().Quantity,
                            //    UnitId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().UnitId,
                            //    UnitCode = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().UnitCode,
                            //    UnitName = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().UnitName,
                            //    UOMId = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().UOMId.GetValueOrDefault(),
                            //    UOMUnit = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().UOMUnit,
                            //    PricePerDealUnit = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PricePerDealUnit,
                            //    PriceTotal = grpProduct.OrderByDescending(t => t.PaymentDueDate).FirstOrDefault().PriceTotal,
                            //    InvoiceId = elementItem.InvoiceId,
                            //    InvoiceDate = elementItem.InvoiceDate,
                            //    InvoiceNo = elementItem.InvoiceNo,
                            //    InvoiceTotalAmount = elementItem.TotalAmount,
                            //}).ToList()
                        })
                    .ToList()
                }).AsQueryable();
            if (filter.PositionIds == null && filter.isPPHMenu != 1)
                internalNoteQuery = internalNoteQuery.Where(entity => entity.Position <= PurchasingGarmentExpeditionPosition.Purchasing || entity.Position == PurchasingGarmentExpeditionPosition.SendToPurchasing);
            else
            {
                if (filter.isPPHMenu != 1)
                    internalNoteQuery = internalNoteQuery.Where(entity => filter.PositionIds.Contains((int)entity.Position));
                else
                {
                    internalNoteQuery = internalNoteQuery.Where(entity => (entity.Position == PurchasingGarmentExpeditionPosition.AccountingAccepted || entity.Position == PurchasingGarmentExpeditionPosition.CashierAccepted));
                }

            }

            if (filter.IncomeTaxId != null)
                internalNoteQuery = internalNoteQuery.Where(entity => entity.Items.Any(item => filter.IncomeTaxId.Select(t => Convert.ToInt64(t)).Contains(item.GarmentInvoice.IncomeTaxId)));

            if (filter.CurrencyCode != null)
                internalNoteQuery = internalNoteQuery.Where(entity => filter.CurrencyCode.Contains(entity.CurrencyCode));

            if (!string.IsNullOrWhiteSpace(keyword))
                internalNoteQuery = internalNoteQuery.Where(entity => entity.INNo.Contains(keyword));

            //var internalNotes = internalNoteQuery.Select(entity => new
            //{
            //    entity.Id,
            //    entity.INNo,
            //    entity.INDate,
            //    entity.SupplierId,
            //    entity.SupplierName,
            //    entity.CurrencyCode,
            //    entity.CurrencyId
            //}).Take(10).ToList();

            return internalNoteQuery.ToList();
        }

        public int UpdateInternNotePosition(UpdatePositionFormDto form)
        {
            var models = _dbContext.GarmentInternNotes.Where(entity => form.Ids.Contains((int)entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.Position = form.Position;
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

                return model;
            }).ToList();

            _dbContext.GarmentInternNotes.UpdateRange(models);
            return _dbContext.SaveChanges();
        }
        public int UpdateDispositionNotePosition(UpdatePositionFormDto form)
        {
            var models = _dbContext.GarmentDispositionPurchases.Include(item => item.GarmentDispositionPurchaseItems).ThenInclude(detail => detail.GarmentDispositionPurchaseDetails).Where(entity => form.Ids.Contains((int)entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.Position = form.Position;
                if (form.Position == PurchasingGarmentExpeditionPosition.SendToCashier)
                {
                    model.GarmentDispositionPurchaseItems = model.GarmentDispositionPurchaseItems.Select(item =>
                    {
                        var previousVerifiedAmount = _dbContext.GarmentDispositionPurchaseItems.Where(t=> t.EPOId == item.EPOId).Sum(t=> t.VerifiedAmount);
                        item.VerifiedAmount = (item.VATAmount + item.GarmentDispositionPurchaseDetails.Sum(detail => detail.PaidPrice) - item.IncomeTaxAmount)+ previousVerifiedAmount;
                        return item;

                    }).ToList();
                }
                else if (form.Position == PurchasingGarmentExpeditionPosition.SendToPurchasing)
                {
                    model.GarmentDispositionPurchaseItems = model.GarmentDispositionPurchaseItems.Select(item =>
                    {
                        item.VerifiedAmount = 0;
                        return item;

                    }).ToList();
                }
                else
                {
                    model.GarmentDispositionPurchaseItems = model.GarmentDispositionPurchaseItems.Select(item =>
                    {
                        //item.VerifiedAmount = 0;
                        return item;

                    }).ToList();
                }
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

                return model;
            }).ToList();

            _dbContext.GarmentDispositionPurchases.UpdateRange(models);
            return _dbContext.SaveChanges();
        }

        public void UpdateInternNotesIsPphPaid(List<GarmentInternNoteUpdateIsPphPaidDto> listModel)
        {
            var listModelINId = listModel.Select(s => s.InternNoteId);
            var existingData = _dbContext.GarmentInternNotes.Where(entity => listModelINId.Contains(entity.Id));
            foreach (var data in existingData)
            {
                data.IsPphPaid = listModel.FirstOrDefault(s => s.InternNoteId == data.Id).IsPphPaid;
                EntityExtension.FlagForUpdate(data, "finance-service", UserAgent);
                _dbContext.GarmentInternNotes.Update(data);
            }
            _dbContext.SaveChanges();
        }
    }
}
