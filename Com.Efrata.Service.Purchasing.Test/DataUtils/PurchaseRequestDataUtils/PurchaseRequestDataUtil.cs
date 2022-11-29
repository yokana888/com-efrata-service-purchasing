using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils
{
    public class PurchaseRequestDataUtil
    {
        private PurchaseRequestItemDataUtil purchaseRequestItemDataUtil;
        private readonly PurchaseRequestFacade facade;

        public PurchaseRequestDataUtil(PurchaseRequestItemDataUtil purchaseRequestItemDataUtil, PurchaseRequestFacade facade)
        {
            this.purchaseRequestItemDataUtil = purchaseRequestItemDataUtil;
            this.facade = facade;
        }

        public PurchaseRequest GetNewData()
        {
            return new PurchaseRequest
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                ExpectedDeliveryDate = DateTimeOffset.Now,
                BudgetId = "1",
                BudgetCode = "BudgetCode",
                BudgetName = "BudgetName",
                UnitId = "1",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "1",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                CategoryId = "1",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",
                Remark = "Remark",
                Items = new List<PurchaseRequestItem> { purchaseRequestItemDataUtil.GetNewData() }
            };
        }

        public PurchaseRequest GetNewDataPdf()
        {
            return new PurchaseRequest
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                ExpectedDeliveryDate = DateTimeOffset.Now,
                BudgetId = "BudgetId",
                BudgetCode = "BudgetCode",
                BudgetName = "BudgetName",
                UnitId = "50",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                CategoryId = "1",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",
                Remark = "Remark",
                Items = new List<PurchaseRequestItem> { purchaseRequestItemDataUtil.GetNewData() }
            };
        }

        public PurchaseRequest GetNewDataPdf1()
        {
            return new PurchaseRequest
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                ExpectedDeliveryDate = DateTimeOffset.Now,
                BudgetId = "BudgetId",
                BudgetCode = "BudgetCode",
                BudgetName = "BudgetName",
                UnitId = "35",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                CategoryId = "1",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",
                Remark = "Remark",
                Items = new List<PurchaseRequestItem> { purchaseRequestItemDataUtil.GetNewData() }
            };
        }

        public PurchaseRequest GetNewHavingStockData()
        {
            return new PurchaseRequest
            {
                No = "No1",
                Date = DateTimeOffset.Now,
                ExpectedDeliveryDate = DateTimeOffset.Now,
                BudgetId = "BudgetId",
                BudgetCode = "BudgetCode",
                BudgetName = "BudgetName",
                UnitId = "UnitId",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                CategoryId = "1",
                CategoryCode = "BB",
                CategoryName = "CategoryName",
                Remark = "Remark",
                Items = new List<PurchaseRequestItem> { purchaseRequestItemDataUtil.GetNewData() }
            };
        }

        public PurchaseRequestViewModel GetNewDataViewModel()
        {
            return new PurchaseRequestViewModel
            {
                no = "No1",
                date = DateTimeOffset.Now,
                expectedDeliveryDate = DateTimeOffset.Now,
                budget = new BudgetViewModel
                {
                    _id = "BudgetId",
                    code = "BudgetCode",
                    name = "BudgetName",
                },
                unit = new UnitViewModel
                {
                    _id = "UnitId",
                    code = "UnitCode",
                    name = "UnitName",
                    division = new DivisionViewModel
                    {
                        _id = "DivisionId",
                        code = "DivisionCode",
                        name = "DivisionName",
                    }
                },
                category = new CategoryViewModel
                {
                    _id = "1",
                    code = "CategoryCode",
                    name = "CategoryName",
                },
                remark = "Remark",
                items = new List<PurchaseRequestItemViewModel> { purchaseRequestItemDataUtil.GetNewDataViewModel() }
            };
        }

        public async Task<PurchaseRequest> GetTestData(string user)
        {
            PurchaseRequest purchaseRequest = GetNewData();

            await facade.Create(purchaseRequest, user);

            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetTestDataPdf(string user)
        {
            PurchaseRequest purchaseRequest = GetNewDataPdf();

            await facade.Create(purchaseRequest, user);

            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetTestDataPdf1(string user)
        {
            PurchaseRequest purchaseRequest = GetNewDataPdf1();

            await facade.Create(purchaseRequest, user);

            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetTestDataPosted(string user)
        {
            PurchaseRequest purchaseRequest = GetNewData();
            purchaseRequest.IsPosted = true;
            await facade.Create(purchaseRequest, user);

            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetTestHavingStockDataPosted(string user)
        {
            PurchaseRequest purchaseRequest = GetNewHavingStockData();
            purchaseRequest.IsPosted = true;
            await facade.Create(purchaseRequest, user);

            return purchaseRequest;
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
    }
}
