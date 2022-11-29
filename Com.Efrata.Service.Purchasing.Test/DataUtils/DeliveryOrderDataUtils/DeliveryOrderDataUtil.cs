using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils
{
    public class DeliveryOrderDataUtil
    {
        private DeliveryOrderItemDataUtil deliveryOrderItemDataUtil;
        private ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil;
        private readonly DeliveryOrderFacade facade;
        private DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil;
        
        public DeliveryOrderDataUtil(DeliveryOrderItemDataUtil deliveryOrderItemDataUtil, DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil, ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil, DeliveryOrderFacade facade)
        {
            this.deliveryOrderItemDataUtil = deliveryOrderItemDataUtil;
            this.deliveryOrderDetailDataUtil = deliveryOrderDetailDataUtil;
            this.externalPurchaseOrderDataUtil = externalPurchaseOrderDataUtil;
            this.facade = facade;
        }

        public async Task<DeliveryOrder> GetNewData(string user)
        {
            var externalPurchaseOrder = await externalPurchaseOrderDataUtil.GetTestDataUnused(user);
            return new DeliveryOrder
            {
                DONo = DateTime.UtcNow.Ticks.ToString(),
                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,
                SupplierId = externalPurchaseOrder.SupplierId,
                SupplierCode = externalPurchaseOrder.SupplierCode,
                SupplierName = externalPurchaseOrder.SupplierName,
                Remark = "Ini Keterangan",
                Items = new List<DeliveryOrderItem> { deliveryOrderItemDataUtil.GetNewData(externalPurchaseOrder) }
            };
        }

        public async Task<DeliveryOrder> GetNewDataValas(string user)
        {
            var externalPurchaseOrder = await externalPurchaseOrderDataUtil.GetTestDataUnusedValas(user);
            return new DeliveryOrder
            {
                DONo = DateTime.UtcNow.Ticks.ToString(),
                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,
                SupplierId = externalPurchaseOrder.SupplierId,
                SupplierCode = externalPurchaseOrder.SupplierCode,
                SupplierName = externalPurchaseOrder.SupplierName,
                Remark = "Ini Keterangan",
                Items = new List<DeliveryOrderItem> { deliveryOrderItemDataUtil.GetNewData(externalPurchaseOrder) }
            };
        }

        public async Task<DeliveryOrder> GetNewHavingStockData(string user)
        {
            var externalPurchaseOrder = await externalPurchaseOrderDataUtil.GetTestHavingStockDataUnused(user);
            return new DeliveryOrder
            {
                DONo = DateTime.UtcNow.Ticks.ToString(),
                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,
                SupplierId = externalPurchaseOrder.SupplierId,
                SupplierCode = externalPurchaseOrder.SupplierCode,
                SupplierName = externalPurchaseOrder.SupplierName,
                Remark = "Ini Keterangan",
                Items = new List<DeliveryOrderItem> { deliveryOrderItemDataUtil.GetNewData(externalPurchaseOrder) }
            };
        }



        public async Task<DeliveryOrderViewModel> GetNewDataViewModel(string user)
        {
            var externalPurchaseOrder = await externalPurchaseOrderDataUtil.GetTestDataUnused(user);

            return new DeliveryOrderViewModel
            {
                no = DateTime.UtcNow.Ticks.ToString(),
                date = DateTimeOffset.Now,
                supplierDoDate = DateTimeOffset.Now,
                supplier = new SupplierViewModel
                {
                    _id = externalPurchaseOrder.SupplierId,
                    code = externalPurchaseOrder.SupplierCode,
                    name = externalPurchaseOrder.SupplierName,
                },
                remark = "Ini Ketereangan",
                items = new List<DeliveryOrderItemViewModel> { deliveryOrderItemDataUtil.GetNewDataViewModel(externalPurchaseOrder) }
            };
        }

        public async Task<DeliveryOrder> GetNewDataDummy(string user)
        {
            var externalPurchaseOrder = await externalPurchaseOrderDataUtil.GetTestDataUnused(user);
            List<ExternalPurchaseOrderItem> EPOItem = new List<ExternalPurchaseOrderItem>(externalPurchaseOrder.Items);
            List<ExternalPurchaseOrderDetail> EPODetail= new List<ExternalPurchaseOrderDetail>(EPOItem[0].Details);
            return new DeliveryOrder
            {
                DONo = DateTime.UtcNow.Ticks.ToString(),
                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,
                SupplierId = externalPurchaseOrder.SupplierId,
                SupplierCode = externalPurchaseOrder.SupplierCode,
                SupplierName = externalPurchaseOrder.SupplierName,
                Remark = "Ini Keterangan",
                Items = new List<DeliveryOrderItem>
                {
                    new DeliveryOrderItem()
                    {
                        EPOId = externalPurchaseOrder.Id,
                        EPONo = "ExternalPONo",
                        Details = new List<DeliveryOrderDetail>
                        {
                            new DeliveryOrderDetail()
                            {
                                EPODetailId = EPODetail[0].Id,
                                POItemId = EPOItem[0].POId,
                                PRId = EPOItem[0].PRId,
                                PRNo = "PRNo",
                                PRItemId = 1,
                                UnitId = "UnitId",
                                UnitCode = "UnitCode",
                                ProductId = "ProductId",
                                ProductCode = "ProductCode",
                                ProductName = "ProductName",
                                DealQuantity = 1,
                                UomId = "UomId",
                                UomUnit = "UomUnit",
                                DOQuantity = 1 - 1
                            }
                        }
                    }
                }
            };
        }

        public async Task<DeliveryOrder> GetTestData(string user)
        {
            DeliveryOrder model = await GetNewData(user);

            await facade.Create(model, user);

            return model;
        }

        public async Task<DeliveryOrder> GetTestDataValas(string user)
        {
            DeliveryOrder model = await GetNewDataValas(user);

            await facade.Create(model, user);

            return model;
        }

        public async Task<DeliveryOrder> GetTestHavingStockData(string user)
        {
            DeliveryOrder model = await GetNewHavingStockData(user);

            await facade.Create(model, user);

            return model;
        }

        public async Task<DeliveryOrder> GetTestData2(string user)
        {
            DeliveryOrder deliveryOrder = await GetNewDataDummy(user);

            await facade.Create(deliveryOrder, user);
            deliveryOrder.DODate = deliveryOrder.DODate.AddDays(40);
            await facade.Update(Convert.ToInt32(deliveryOrder.Id), deliveryOrder, user);

            return deliveryOrder;
        }

        public async Task<DeliveryOrder> GetTestData3(string user)
        {
            DeliveryOrder deliveryOrder = await GetNewDataDummy(user);

            await facade.Create(deliveryOrder, user);
            deliveryOrder.DODate = deliveryOrder.DODate.AddDays(70);
            await facade.Update(Convert.ToInt32(deliveryOrder.Id), deliveryOrder, user);

            return deliveryOrder;
        }

        public async Task<DeliveryOrder> GetTestDataDummy(string user)
        {
            DeliveryOrder model = await GetNewDataDummy(user);

            await facade.Create(model, user);

            return model;
        }
    }
}
