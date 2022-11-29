using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPOMasterDistributionDataUtils
{
    public class BasicDataUtil
    {
        private readonly GarmentPOMasterDistributionFacade facade;
        private readonly GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil;

        public BasicDataUtil(GarmentPOMasterDistributionFacade facade, GarmentDeliveryOrderDataUtil garmentDeliveryOrderDataUtil)
        {
            this.facade = facade;
            this.garmentDeliveryOrderDataUtil = garmentDeliveryOrderDataUtil;
        }

        public async Task<GarmentPOMasterDistribution> GetNewData(GarmentDeliveryOrder garmentDeliveryOrder = null)
        {
            garmentDeliveryOrder = await garmentDeliveryOrderDataUtil.GetTestData4(garmentDeliveryOrder);
            return new GarmentPOMasterDistribution
            {
                DOId = garmentDeliveryOrder.Id,
                DONo = garmentDeliveryOrder.DONo,
                Items = garmentDeliveryOrder.Items.SelectMany(i => i.Details.Select(d => new GarmentPOMasterDistributionItem
                {
                    DOItemId = i.Id,
                    DODetailId = d.Id,
                    SCId = 1,
                    Details = new List<GarmentPOMasterDistributionDetail>
                    {
                        new GarmentPOMasterDistributionDetail {
                            CostCalculationId = 1,
                            RONo = "RONo",
                            POSerialNumber = "POSerialNumber",
                            ProductId = d.ProductId,
                            ProductCode = d.ProductCode,
                            QuantityCC = (decimal)d.DOQuantity,
                            UomCCId = int.Parse(d.UomId),
                            UomCCUnit = d.UomUnit,
                            Conversion = 1,
                            Quantity = (decimal)d.DOQuantity,
                            UomId = int.Parse(d.UomId),
                            UomUnit = d.UomUnit
                        }
                    }
                })).ToList()
            };
        }

        internal GarmentPOMasterDistribution CopyData(GarmentPOMasterDistribution data)
        {
            return new GarmentPOMasterDistribution
            {
                UId = data.UId,

                Id = data.Id,
                Active = data.Active,
                CreatedUtc = data.CreatedUtc,
                CreatedBy = data.CreatedBy,
                CreatedAgent = data.CreatedAgent,
                LastModifiedUtc = data.LastModifiedUtc,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAgent = data.LastModifiedAgent,
                IsDeleted = data.IsDeleted,
                DeletedUtc = data.DeletedUtc,
                DeletedBy = data.DeletedBy,
                DeletedAgent = data.DeletedAgent,

                DOId = data.DOId,
                DONo = data.DONo,
                DODate = data.DODate,
                SupplierId = data.SupplierId,
                SupplierName = data.SupplierName
            };
        }

        internal GarmentPOMasterDistributionItem CopyDataItem(GarmentPOMasterDistributionItem data)
        {
            return new GarmentPOMasterDistributionItem
            {
                UId = data.UId,

                Id = data.Id,
                Active = data.Active,
                CreatedUtc = data.CreatedUtc,
                CreatedBy = data.CreatedBy,
                CreatedAgent = data.CreatedAgent,
                LastModifiedUtc = data.LastModifiedUtc,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAgent = data.LastModifiedAgent,
                IsDeleted = data.IsDeleted,
                DeletedUtc = data.DeletedUtc,
                DeletedBy = data.DeletedBy,
                DeletedAgent = data.DeletedAgent,

                DOItemId = data.DOItemId,
                DODetailId = data.DODetailId,

                SCId = data.SCId
            };
        }

        internal GarmentPOMasterDistributionDetail CopyDataDetail(GarmentPOMasterDistributionDetail data)
        {
            return new GarmentPOMasterDistributionDetail
            {
                UId = data.UId,

                Id = data.Id,
                Active = data.Active,
                CreatedUtc = data.CreatedUtc,
                CreatedBy = data.CreatedBy,
                CreatedAgent = data.CreatedAgent,
                LastModifiedUtc = data.LastModifiedUtc,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAgent = data.LastModifiedAgent,
                IsDeleted = data.IsDeleted,
                DeletedUtc = data.DeletedUtc,
                DeletedBy = data.DeletedBy,
                DeletedAgent = data.DeletedAgent,

                CostCalculationId = data.CostCalculationId,
                RONo = data.RONo,
                POSerialNumber = data.POSerialNumber,
                ProductId = data.ProductId,
                ProductCode = data.ProductCode,
                QuantityCC = data.QuantityCC,
                UomCCId = data.UomCCId,
                UomCCUnit = data.UomCCUnit,
                Conversion = data.Conversion,
                Quantity = data.Quantity,
                UomId = data.UomId,
                UomUnit = data.UomUnit
            };
        }

        public async Task<GarmentPOMasterDistribution> GetTestData(GarmentPOMasterDistribution data = null)
        {
            data = data ?? await GetNewData();
            await facade.Create(data);
            return data;
        }
    }
}
