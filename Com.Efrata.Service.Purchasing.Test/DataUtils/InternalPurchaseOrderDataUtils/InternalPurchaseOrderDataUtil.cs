using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils
{
    public class InternalPurchaseOrderDataUtil
    {
        private InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil;
        private PurchaseRequestDataUtil purchaserequestDataUtil;
        private readonly InternalPurchaseOrderFacade facade;
        //private readonly HttpClientTestService client;

        public InternalPurchaseOrderDataUtil(InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil, InternalPurchaseOrderFacade facade, PurchaseRequestDataUtil purchaserequestDataUtil)
        {
            this.internalPurchaseOrderItemDataUtil = internalPurchaseOrderItemDataUtil;
            this.purchaserequestDataUtil = purchaserequestDataUtil;
            this.facade = facade;
            //this.client = client;
        }

        public async Task<InternalPurchaseOrder> GetNewData(string user)
        {
            PurchaseRequest purchaseRequest = await purchaserequestDataUtil.GetTestDataPosted(user);

            return new InternalPurchaseOrder
            {
                IsoNo = "",
                PRId = purchaseRequest.Id.ToString(),
                PRNo = purchaseRequest.No,
                PRDate = purchaseRequest.Date,
                ExpectedDeliveryDate = purchaseRequest.ExpectedDeliveryDate,
                BudgetId = purchaseRequest.BudgetId,
                BudgetCode = purchaseRequest.BudgetCode,
                BudgetName = purchaseRequest.BudgetName,
                UnitId = purchaseRequest.UnitId,
                UnitCode = purchaseRequest.UnitCode,
                UnitName = purchaseRequest.UnitName,
                DivisionId = purchaseRequest.DivisionId,
                DivisionCode = purchaseRequest.DivisionCode,
                DivisionName = purchaseRequest.DivisionName,
                CategoryId = purchaseRequest.CategoryId,
                CategoryCode = purchaseRequest.CategoryCode,
                CategoryName = purchaseRequest.CategoryName,
                Remark = purchaseRequest.Remark,
                Items = new List<InternalPurchaseOrderItem> { internalPurchaseOrderItemDataUtil.GetNewData(purchaseRequest.Items.ToList()) }
            };
        }

        public async Task<InternalPurchaseOrder> GetNewHavingStockData(string user)
        {
            PurchaseRequest purchaseRequest = await purchaserequestDataUtil.GetTestHavingStockDataPosted(user);

            return new InternalPurchaseOrder
            {
                IsoNo = "",
                PRId = purchaseRequest.Id.ToString(),
                PRNo = purchaseRequest.No,
                PRDate = purchaseRequest.Date,
                ExpectedDeliveryDate = purchaseRequest.ExpectedDeliveryDate,
                BudgetId = purchaseRequest.BudgetId,
                BudgetCode = purchaseRequest.BudgetCode,
                BudgetName = purchaseRequest.BudgetName,
                UnitId = purchaseRequest.UnitId,
                UnitCode = purchaseRequest.UnitCode,
                UnitName = purchaseRequest.UnitName,
                DivisionId = purchaseRequest.DivisionId,
                DivisionCode = purchaseRequest.DivisionCode,
                DivisionName = purchaseRequest.DivisionName,
                CategoryId = purchaseRequest.CategoryId,
                CategoryCode = purchaseRequest.CategoryCode,
                CategoryName = purchaseRequest.CategoryName,
                Remark = purchaseRequest.Remark,
                Items = new List<InternalPurchaseOrderItem> { internalPurchaseOrderItemDataUtil.GetNewData(purchaseRequest.Items.ToList()) }
            };
        }

        public async Task<InternalPurchaseOrderViewModel> GetNewDataViewModel(string user)
        {
            PurchaseRequest purchaseRequest = await purchaserequestDataUtil.GetTestDataPosted(user);
            return new InternalPurchaseOrderViewModel
            {
                prId = purchaseRequest.Id.ToString(),
                prNo = purchaseRequest.No,
                prDate = purchaseRequest.Date,
                expectedDeliveryDate = purchaseRequest.ExpectedDeliveryDate,
                budget = new BudgetViewModel
                {
                    _id = purchaseRequest.BudgetId,
                    code = purchaseRequest.BudgetCode,
                    name = purchaseRequest.BudgetName,
                },
                unit = new UnitViewModel
                {
                    _id = purchaseRequest.UnitId,
                    code = purchaseRequest.UnitCode,
                    name = purchaseRequest.UnitName,
                    division = new DivisionViewModel
                    {
                        _id = purchaseRequest.DivisionId,
                        code = purchaseRequest.DivisionCode,
                        name = purchaseRequest.DivisionName,
                    }
                },
                category = new CategoryViewModel
                {
                    _id = purchaseRequest.CategoryId,
                    code = purchaseRequest.CategoryCode,
                    name = purchaseRequest.CategoryName,
                },
                remark = purchaseRequest.Remark,
                items = new List<InternalPurchaseOrderItemViewModel> { internalPurchaseOrderItemDataUtil.GetNewDataViewModel(purchaseRequest.Items.ToList()) }
            };
        }

        public async Task<InternalPurchaseOrder> GetTestData(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewData(user);

            await facade.Create(internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }

        public async Task<InternalPurchaseOrder> GetTestHavingStockData(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewHavingStockData(user);

            await facade.Create(internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }

        public async Task<InternalPurchaseOrder> GetTestData2(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewData(user);

            await facade.Create(internalPurchaseOrder, user);
            internalPurchaseOrder.CreatedUtc = internalPurchaseOrder.CreatedUtc.AddDays(10);
            await facade.Update(Convert.ToInt32(internalPurchaseOrder.Id), internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }
        public async Task<InternalPurchaseOrder> GetTestData3(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewData(user);

            await facade.Create(internalPurchaseOrder, user);
            internalPurchaseOrder.CreatedUtc = internalPurchaseOrder.CreatedUtc.AddDays(16);
            await facade.Update(Convert.ToInt32(internalPurchaseOrder.Id), internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }

        public async Task<InternalPurchaseOrder> GetTestData4(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewData(user);

            await facade.Create(internalPurchaseOrder, user);
            internalPurchaseOrder.CreatedUtc = internalPurchaseOrder.CreatedUtc.AddDays(-40);
            await facade.Update(Convert.ToInt32(internalPurchaseOrder.Id), internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }

        public async Task<InternalPurchaseOrder> GetTestData5(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await GetNewData(user);

            await facade.Create(internalPurchaseOrder, user);
            internalPurchaseOrder.CreatedUtc = internalPurchaseOrder.CreatedUtc.AddDays(-70);
            await facade.Update(Convert.ToInt32(internalPurchaseOrder.Id), internalPurchaseOrder, user);

            return internalPurchaseOrder;
        }
        //public PurchaseRequestViewModel GetViewModelTestData()
        //{
        //    PurchaseRequestViewModel viewModel = mapper.Map<PurchaseRequestViewModel>(GetNewData());

        //    return viewModel;
        //}
        //public PurchaseRequestViewModel GetViewModelFromModelTestData(PurchaseRequest model)
        //{
        //    PurchaseRequestViewModel viewModel = mapper.Map<PurchaseRequestViewModel>(model);

        //    return viewModel;
        //}

        public InternalPurchaseOrderFulFillment GetNewFulfillmentData(string user)
        {
            return new InternalPurchaseOrderFulFillment
            {
                DeliveryOrderDate = DateTimeOffset.UtcNow,
                DeliveryOrderDeliveredQuantity = 1,
                Corrections = new List<InternalPurchaseOrderCorrection>()
                {
                    new InternalPurchaseOrderCorrection()
                    {
                        CorrectionDate = DateTimeOffset.UtcNow,
                        CorrectionNo = "np",
                        CorrectionQuantity = 1,
                        CorrectionPriceTotal = 1,
                        CorrectionRemark = "remark",
                        UnitPaymentCorrectionId = 1,
                        UnitPaymentCorrectionItemId = 1
                    }
                },
                DeliveryOrderDetailId = 1,
                DeliveryOrderId = 1,
                DeliveryOrderNo = "no",
                DeliveryOrderItemId = 1,
                InterNoteDate = DateTimeOffset.UtcNow,
                InterNoteDueDate = DateTimeOffset.UtcNow,
                InterNoteNo = "no",
                InterNoteValue = 1,
                InvoiceDate = DateTimeOffset.UtcNow,
                InvoiceNo = "np",
                SupplierDODate = DateTimeOffset.UtcNow,
                UnitPaymentOrderDetailId = 1,
                UnitPaymentOrderId = 1,
                UnitPaymentOrderItemId = 1,
                UnitReceiptNoteDate = DateTimeOffset.UtcNow,
                UnitReceiptNoteDeliveredQuantity = 1,
                UnitReceiptNoteId = 1,
                UnitReceiptNoteItemId = 1,
                UnitReceiptNoteNo = "np",
                UnitReceiptNoteUom = "uom",
                UnitReceiptNoteUomId = "1",
                UnitPaymentOrderUseVat = true,
                UnitPaymentOrderUseIncomeTax = true,
                UnitPaymentOrderIncomeTaxDate = DateTimeOffset.UtcNow,
                UnitPaymentOrderIncomeTaxNo = "no",
                UnitPaymentOrderIncomeTaxRate = 1,
                UnitPaymentOrderVatDate = DateTimeOffset.UtcNow,
                UnitPaymentOrderVatNo = "no"
            };
        }
    }
}
