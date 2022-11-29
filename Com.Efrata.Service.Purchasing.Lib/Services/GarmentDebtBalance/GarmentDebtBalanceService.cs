using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class GarmentDebtBalanceService : IGarmentDebtBalanceService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PurchasingDbContext _dbContext;

        public GarmentDebtBalanceService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
        }

        public async Task<int> CreateFromCustoms(CustomsFormDto form)
        {
            var uri = "garment-debt-balances/customs";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{uri}", new StringContent(JsonConvert.SerializeObject(form).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> EmptyBankExpenditureNote(int deliveryOrderId)
        {
            var uri = "garment-debt-balances/remove-bank-expenditure-note/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(new { }).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> EmptyInternalNote(int deliveryOrderId)
        {
            var uri = "garment-debt-balances/remove-internal-note/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(new { }).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> EmptyInvoice(int deliveryOrderId)
        {
            var uri = "garment-debt-balances/remove-invoice/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(new { }).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public List<DispositionDto> GetDispositions(string keyword)
        {
            var result = new List<DispositionDto>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var dispositions = _dbContext.GarmentDispositionPurchases.Where(entity => entity.DispositionNo.Contains(keyword)).Select(entity => new { entity.Id, entity.DispositionNo }).ToList();

                if (dispositions.Count > 0)
                {
                    var dispositionIds = dispositions.Select(element => element.Id).ToList();
                    var dispositionItems = _dbContext.GarmentDispositionPurchaseItems.Where(entity => dispositionIds.Contains(entity.GarmentDispositionPurchaseId)).Select(entity => new { entity.Id, entity.GarmentDispositionPurchaseId, entity.EPOId }).ToList();
                    var epoIds = dispositionItems.Select(element => (long)element.EPOId).ToList();

                    var deliveryOrderItems = _dbContext.GarmentDeliveryOrderItems.Where(entity => epoIds.Contains(entity.EPOId)).Select(element => new { element.Id, element.GarmentDOId, element.EPOId }).ToList();
                    var deliveryOrderItemIds = deliveryOrderItems.Select(element => element.Id).ToList();
                    var deliveryOrderIds = deliveryOrderItems.Select(element => element.GarmentDOId).ToList();
                    var deliveryOrders = _dbContext.GarmentDeliveryOrders.Where(entity => deliveryOrderIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.DONo, entity.BillNo, entity.PaymentBill, entity.SupplierCode, entity.SupplierName, entity.DOCurrencyCode, entity.DOCurrencyRate, entity.TotalAmount }).ToList();
                    var deliveryOrderDetails = _dbContext.GarmentDeliveryOrderDetails.Where(entity => deliveryOrderItemIds.Contains(entity.GarmentDOItemId)).Select(entity => new { entity.Id, entity.GarmentDOItemId }).ToList();
                    var deliveryOrderDetailIds = deliveryOrderDetails.Select(element => element.Id).ToList();
                    var customsDocumentItems = _dbContext.GarmentBeacukaiItems.Where(entity => deliveryOrderIds.Contains(entity.GarmentDOId)).Select(entity => new { entity.Id, entity.GarmentDOId }).ToList();
                    var internalNoteDetails = _dbContext.GarmentInternNoteDetails.Where(entity => deliveryOrderIds.Contains(entity.DOId)).Select(entity => new { entity.Id, entity.GarmentItemINId, entity.DOId }).ToList();
                    var internalNoteItemIds = internalNoteDetails.Select(element => element.GarmentItemINId).ToList();
                    var internalNoteItems = _dbContext.GarmentInternNoteItems.Where(entity => internalNoteItemIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.GarmentINId }).ToList();
                    var internalNoteIds = internalNoteItems.Select(element => element.GarmentINId).ToList();
                    var internalNotes = _dbContext.GarmentInternNotes.Where(entity => internalNoteIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.INNo }).ToList();

                    foreach (var disposition in dispositions)
                    {
                        var dto = new DispositionDto(disposition.Id, disposition.DispositionNo, new List<MemoDetail>());

                        var dispositionEPOIds = dispositionItems.Where(element => element.GarmentDispositionPurchaseId == disposition.Id).Select(element => (long)element.EPOId).ToList();
                        var dispositionDOIds = deliveryOrderItems.Where(element => dispositionEPOIds.Contains(element.EPOId)).Select(element => element.GarmentDOId).ToList();
                        var dispositionDeliveryOrders = deliveryOrders.Where(element => dispositionDOIds.Contains(element.Id)).ToList();

                        foreach (var dispositionDeliveryOrder in dispositionDeliveryOrders)
                        {
                            var purchasingAmount = dispositionDeliveryOrder.TotalAmount;

                            var currencyRate = dispositionDeliveryOrder.DOCurrencyRate.GetValueOrDefault();
                            if (currencyRate <= 0 )
                            {
                                currencyRate = 1;
                            }

                            if (customsDocumentItems.Any(element => element.GarmentDOId == dispositionDeliveryOrder.Id))
                            {
                                var internalNoteNo = "";
                                var internalNoteDetail = internalNoteDetails.FirstOrDefault(element => element.DOId == dispositionDeliveryOrder.Id);
                                if (internalNoteDetail != null)
                                {
                                    var internalNoteItem = internalNoteItems.FirstOrDefault(element => element.Id == internalNoteDetail.GarmentItemINId);
                                    var internalNote = internalNotes.FirstOrDefault(element => element.Id == internalNoteItem.GarmentINId);
                                    internalNoteNo = internalNote.INNo;

                                }
                                dto.MemoDetails.Add(new MemoDetail((int)dispositionDeliveryOrder.Id, dispositionDeliveryOrder.DONo, dispositionDeliveryOrder.SupplierCode, dispositionDeliveryOrder.SupplierName, internalNoteNo, dispositionDeliveryOrder.BillNo, dispositionDeliveryOrder.PaymentBill, dispositionDeliveryOrder.DOCurrencyCode, currencyRate, dispositionDeliveryOrder.TotalAmount));
                            }
                        }
                        dto.OrderByInternalNoteNo();
                        result.Add(dto);
                    }
                }
            }

            return result.Where(element => element.MemoDetails.Count > 0).ToList();
        }

        public async Task<int> RemoveCustoms(int deliveryOrderId)
        {
            var uri = "garment-debt-balances/remove-customs/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(new { }).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> UpdateFromBankExpenditureNote(int deliveryOrderId, BankExpenditureNoteFormDto form)
        {
            var uri = "garment-debt-balances/bank-expenditure-note/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(form).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> UpdateFromInternalNote(int deliveryOrderId, InternalNoteFormDto form)
        {
            var uri = "garment-debt-balances/internal-note/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(form).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }

        public async Task<int> UpdateFromInvoice(int deliveryOrderId, InvoiceFormDto form)
        {
            var uri = "garment-debt-balances/invoice/";
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{uri}{deliveryOrderId}", new StringContent(JsonConvert.SerializeObject(form).ToString(), Encoding.UTF8, General.JsonMediaType));

            return (int)response.StatusCode;
        }
    }
}
