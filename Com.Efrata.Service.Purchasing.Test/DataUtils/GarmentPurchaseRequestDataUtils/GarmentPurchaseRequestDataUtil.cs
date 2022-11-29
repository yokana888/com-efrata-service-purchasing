using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils
{
    public class GarmentPurchaseRequestDataUtil
    {
        private readonly GarmentPurchaseRequestFacade facade;

        public GarmentPurchaseRequestDataUtil(GarmentPurchaseRequestFacade facade)
        {
            this.facade = facade;
        }

        public GarmentPurchaseRequest GetNewData()
        {
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentPurchaseRequest
            {
                RONo = $"RO{nowTicksA}",
                MDStaff = $"MDStaff{nowTicksA}",
                BuyerId = "1",
                BuyerCode = $"BuyerCode{nowTicksA}",
                BuyerName = $"BuyerName{nowTicksA}",

                Article = $"Article{nowTicksA}",

                Date = DateTimeOffset.Now,
                ShipmentDate = DateTimeOffset.Now,

                UnitId = $"{nowTicksA}",
                UnitCode = $"UnitCode{nowTicksA}",
                UnitName = $"UnitName{nowTicksA}",

                Remark = $"Remark{nowTicksA}",
                CreatedBy = "CreatedBy",

                PRType = $"PRType{nowTicks}",
                SCId = nowTicks,
                SCNo = $"SCNo{nowTicks}",
                IsValidated = true,
                ValidatedBy = nowTicksA,
                ValidatedDate = DateTimeOffset.Now,

                SectionName = $"SectionName{nowTicksA}",
                ApprovalPR = $"ApprovalPR{nowTicksA}",
                IsValidatedMD1 = true,
                IsValidatedPurchasing = true,
                IsValidatedMD2 = true,
                ValidatedMD2Date = DateTimeOffset.Now,

                Items = new List<GarmentPurchaseRequestItem>
                {
                    new GarmentPurchaseRequestItem
                    {
                        PO_SerialNumber = $"PO_SerialNumber{nowTicksA}",

                        ProductId = "1",
                        ProductCode = $"ProductCode{nowTicksA}",
                        ProductName = $"ProductName{nowTicksA}",

                        Quantity = rnd.Next(2, 100),
                        BudgetPrice = rnd.Next(2, 100),

                        UomId = $"{nowTicksA}",
                        UomUnit = $"UomUnit{nowTicksA}",

                        CategoryId = $"{nowTicksA}",
                        CategoryName = $"CategoryName{nowTicksA}",

                        ProductRemark = $"ProductRemark{nowTicksA}",

                        PriceUomId = nowTicks,
                        PriceUomUnit = $"PriceUomUnit{nowTicks}",
                        PriceConversion = 1
                    },
                    new GarmentPurchaseRequestItem
                    {
                        PO_SerialNumber = $"PO_SerialNumber{nowTicksB}",

                        ProductId = $"{nowTicksB}",
                        ProductCode = $"ProductCode{nowTicksB}",
                        ProductName = $"ProductName{nowTicksB}",

                        Quantity = rnd.Next(2, 100),
                        BudgetPrice = rnd.Next(2, 100),

                        UomId = $"{nowTicksB}",
                        UomUnit = $"UomUnit{nowTicksB}",

                        CategoryId = $"{nowTicksB}",
                        CategoryName = $"CategoryName{nowTicksB}",

                        ProductRemark = $"ProductRemark{nowTicksB}",

                        PriceUomId = nowTicks,
                        PriceUomUnit = $"PriceUomUnit{nowTicks}",
                        PriceConversion = 1
                    }
                }
            };
        }

        public GarmentPurchaseRequest CopyData(GarmentPurchaseRequest data)
        {
            return new GarmentPurchaseRequest
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

                PRNo = data.PRNo,
                PRType = data.PRType,
                RONo = data.RONo,
                MDStaff = data.MDStaff,
                SCId = data.SCId,
                SCNo = data.SCNo,
                BuyerId = data.BuyerId,
                BuyerCode = data.BuyerCode,
                BuyerName = data.BuyerName,
                Article = data.Article,
                Date = data.Date,
                ExpectedDeliveryDate = data.ExpectedDeliveryDate,
                ShipmentDate = data.ShipmentDate,
                UnitId = data.UnitId,
                UnitCode = data.UnitCode,
                UnitName = data.UnitName,
                IsPosted = data.IsPosted,
                IsUsed = data.IsUsed,
                IsValidated = data.IsValidated,
                Remark = data.Remark,
                ValidatedBy = data.ValidatedBy,
                ValidatedDate = data.ValidatedDate,
            };
        }

        public GarmentPurchaseRequestItem CopyDataItem(GarmentPurchaseRequestItem data)
        {
            return new GarmentPurchaseRequestItem
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

                PO_SerialNumber = data.PO_SerialNumber,
                ProductId = data.ProductId,
                ProductCode = data.ProductCode,
                ProductName = data.ProductName,
                Quantity = data.Quantity,
                BudgetPrice = data.BudgetPrice,
                UomId = data.UomId,
                UomUnit = data.UomUnit,
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName,
                ProductRemark = data.ProductRemark,
                Status = data.Status,
                IsUsed = data.IsUsed,
                PriceUomId = data.PriceUomId,
                PriceUomUnit = data.PriceUomUnit,
                PriceConversion = data.PriceConversion,
            };
        }

        public async Task<GarmentPurchaseRequest> GetTestData(GarmentPurchaseRequest data = null)
        {
            data = data ?? GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<List<GarmentInternalPurchaseOrder>> GetTestDataByTags(GarmentPurchaseRequest garmentPurchaseRequest = null)
        {
            var testData = await GetTestData(garmentPurchaseRequest);
            return facade.ReadByTags($"#{testData.UnitName}#{testData.BuyerName}", DateTimeOffset.MinValue, DateTimeOffset.MinValue);
        }

    }
}
