using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils
{

    public class ExternalPurchaseOrderDataUtil
    {
        private InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil;
        private ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil;
        private readonly ExternalPurchaseOrderFacade facade;

        public ExternalPurchaseOrderDataUtil(ExternalPurchaseOrderFacade facade)
        {
            this.facade = facade;
        }

        public ExternalPurchaseOrderDataUtil(ExternalPurchaseOrderFacade facade, InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil, ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil)
        {
            this.facade = facade;
            this.internalPurchaseOrderDataUtil = internalPurchaseOrderDataUtil;
            this.externalPurchaseOrderItemDataUtil = externalPurchaseOrderItemDataUtil;
        }

        public async Task<ExternalPurchaseOrder> GetNewData(string user, InternalPurchaseOrder inPO = null)
        {
            InternalPurchaseOrder internalPurchaseOrder = inPO ?? await internalPurchaseOrderDataUtil.GetTestData(user);
           
            return new ExternalPurchaseOrder
            {
                CurrencyCode = "IDR",
                CurrencyId = "1",
                CurrencyRate = 0.5,
                UnitId = "1",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "1",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                FreightCostBy = "test",
                DeliveryDate = DateTime.Now.AddDays(1),
                OrderDate = DateTime.Now,
                SupplierCode = "sup",
                SupplierId = "1",
                POCashType = "VB",
                IncomeTaxName = "Final",
                IncomeTaxRate = "1.5",
                UseIncomeTax = true,
                IncomeTaxBy = "Supplier",
                SupplierName = "Supplier",
                PaymentMethod = "test",
                Remark = "Remark",
                EPONo = "EPONoTest123",
                Items = new List<ExternalPurchaseOrderItem> { externalPurchaseOrderItemDataUtil.GetNewData(internalPurchaseOrder) }
            };
        }

        public async Task<ExternalPurchaseOrder> GetNewDataValas(string user, InternalPurchaseOrder inPO = null)
        {
            InternalPurchaseOrder internalPurchaseOrder = inPO ?? await internalPurchaseOrderDataUtil.GetTestData(user);

            return new ExternalPurchaseOrder
            {
                CurrencyCode = "USD",
                CurrencyId = "1",
                CurrencyRate = 0.5,
                UnitId = "1",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "1",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                FreightCostBy = "test",
                DeliveryDate = DateTime.Now.AddDays(1),
                OrderDate = DateTime.Now,
                SupplierCode = "sup",
                SupplierId = "1",
                POCashType = "VB",
                IncomeTaxName = "Final",
                IncomeTaxRate = "1.5",
                UseIncomeTax = true,
                IncomeTaxBy = "Supplier",
                SupplierName = "Supplier",
                PaymentMethod = "test",
                Remark = "Remark",
                EPONo = "EPONoTest123",
                Items = new List<ExternalPurchaseOrderItem> { externalPurchaseOrderItemDataUtil.GetNewData(internalPurchaseOrder) }
            };
        }

        public async Task<ExternalPurchaseOrder> GetNewHavingStockData(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await internalPurchaseOrderDataUtil.GetTestHavingStockData(user);
            

            return new ExternalPurchaseOrder
            {
                CurrencyCode = "CurrencyCode",
                CurrencyId = "CurrencyId",
                CurrencyRate = 0.5,
                UnitId = "UnitId",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                FreightCostBy = "test",
                DeliveryDate = DateTime.Now.AddDays(1),
                OrderDate = DateTime.Now,
                SupplierCode = "sup",
                SupplierId = "supId",
                POCashType = "POCashType",
                IncomeTaxName = "Final",
                IncomeTaxRate = "1.5",
                UseIncomeTax = true,
                IncomeTaxBy = "Supplier",
                SupplierName = "Supplier",
                PaymentMethod = "test",
                Remark = "Remark",
                Items = new List<ExternalPurchaseOrderItem> { externalPurchaseOrderItemDataUtil.GetNewData(internalPurchaseOrder) }
            };
        }

        public async Task<ExternalPurchaseOrderViewModel> GetNewDataViewModel(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await internalPurchaseOrderDataUtil.GetTestData(user);


            return new ExternalPurchaseOrderViewModel
            {
                unit = new UnitViewModel
                {
                    _id = internalPurchaseOrder.UnitId,
                    code = internalPurchaseOrder.UnitCode,
                    name = internalPurchaseOrder.UnitName,
                    division = new DivisionViewModel
                    {
                        _id = internalPurchaseOrder.DivisionId,
                        code = internalPurchaseOrder.DivisionCode,
                        name = internalPurchaseOrder.DivisionName,
                    }
                },
                currency = new CurrencyViewModel
                {
                    code = "CurrencyCode",
                    _id = "CurrencyId",
                    rate = 0.5,
                },
                freightCostBy = "test",
                deliveryDate = DateTime.Now.AddDays(1),
                orderDate = DateTime.Now,
                supplier = new SupplierViewModel
                {
                    code = "sup",
                    _id = "supId",
                    name = "Supplier",
                },
                paymentMethod = "test",
                poCashType = "test",
                remark = "Remark",
                items = new List<ExternalPurchaseOrderItemViewModel> { externalPurchaseOrderItemDataUtil.GetNewDataViewModel(internalPurchaseOrder) }
            };
        }

        public async Task<ExternalPurchaseOrderViewModel> GetNewDuplicateDataViewModel(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await internalPurchaseOrderDataUtil.GetTestData(user);


            return new ExternalPurchaseOrderViewModel
            {
                unit = new UnitViewModel
                {
                    _id = internalPurchaseOrder.UnitId,
                    code = internalPurchaseOrder.UnitCode,
                    name = internalPurchaseOrder.UnitName,
                    division = new DivisionViewModel
                    {
                        _id = internalPurchaseOrder.DivisionId,
                        code = internalPurchaseOrder.DivisionCode,
                        name = internalPurchaseOrder.DivisionName,
                    }
                },
                currency = new CurrencyViewModel
                {
                    code = "CurrencyCode",
                    _id = "CurrencyId",
                    rate = 0.5,
                },
                freightCostBy = "test",
                deliveryDate = DateTime.Now.AddDays(1),
                orderDate = DateTime.Now,
                supplier = new SupplierViewModel
                {
                    code = "sup",
                    _id = "supId",
                    name = "Supplier",
                },
                paymentMethod = "test",
                poCashType = "test",
                remark = "Remark",
                items = new List<ExternalPurchaseOrderItemViewModel> {
                    externalPurchaseOrderItemDataUtil.GetNewDataViewModel(internalPurchaseOrder),
                    externalPurchaseOrderItemDataUtil.GetNewDataViewModel(internalPurchaseOrder),
                }
            };
        }

        public async Task<ExternalPurchaseOrder> GetTestData(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user, 7);

            return externalPurchaseOrder;
        }


        public async Task<ExternalPurchaseOrder> GetTestData2(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user, 7);
            externalPurchaseOrder.CreatedUtc = externalPurchaseOrder.CreatedUtc.AddDays(10);
            await facade.Update(Convert.ToInt32(externalPurchaseOrder.Id), externalPurchaseOrder, user);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestData3(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user, 7);
            externalPurchaseOrder.CreatedUtc = externalPurchaseOrder.CreatedUtc.AddDays(16);
            await facade.Update(Convert.ToInt32(externalPurchaseOrder.Id), externalPurchaseOrder, user);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestData4(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user, 7);
            externalPurchaseOrder.CreatedUtc = externalPurchaseOrder.CreatedUtc.AddDays(-40);
            await facade.Update(Convert.ToInt32(externalPurchaseOrder.Id), externalPurchaseOrder, user);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestData5(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user, 7);
            externalPurchaseOrder.CreatedUtc = externalPurchaseOrder.CreatedUtc.AddDays(-70);
            await facade.Update(Convert.ToInt32(externalPurchaseOrder.Id), externalPurchaseOrder, user);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestDataUnused(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);
            externalPurchaseOrder.IsPosted = true;
            foreach (var item in externalPurchaseOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    detail.DOQuantity = 0;
                    detail.DealQuantity = 2;
                }
            }

            await facade.Create(externalPurchaseOrder, user, 7);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestDataUnusedValas(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewDataValas(user);
            externalPurchaseOrder.IsPosted = true;
            foreach (var item in externalPurchaseOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    detail.DOQuantity = 0;
                    detail.DealQuantity = 2;
                }
            }

            await facade.Create(externalPurchaseOrder, user, 7);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestHavingStockDataUnused(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewHavingStockData(user);
            externalPurchaseOrder.IsPosted = true;
            foreach (var item in externalPurchaseOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    detail.DOQuantity = 0;
                    detail.DealQuantity = 2;
                }
            }

            await facade.Create(externalPurchaseOrder, user, 7);

            return externalPurchaseOrder;
        }

        public async Task<ExternalPurchaseOrder> GetTestDataMP(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);
            externalPurchaseOrder.IsPosted = true;
            foreach (var item in externalPurchaseOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    detail.ProductName = "KERTAS BURAM";
                }
            }

            await facade.Create(externalPurchaseOrder, user, 7);

            return externalPurchaseOrder;
        }

        public ExternalPurchaseOrder GetNewData_VBRequestPOExternal()
        {
            return new ExternalPurchaseOrder()
            {
                Items=new List<ExternalPurchaseOrderItem>()
                {
                    new ExternalPurchaseOrderItem()
                    {
                        Details=new List<ExternalPurchaseOrderDetail>()
                        {
                            new ExternalPurchaseOrderDetail()
                            {
                                ExternalPurchaseOrderItem=new ExternalPurchaseOrderItem()
                                {
                                    Details =new List<ExternalPurchaseOrderDetail>()
                                }
                            }
                        }
                    }
                }
            };
        }

        public async Task<ExternalPurchaseOrder> GetTestData_VBRequestPOExternal()
        {
            var data = GetNewData_VBRequestPOExternal();
            await facade.Create(data, "", 7);

            return data;
        }
    }
}


