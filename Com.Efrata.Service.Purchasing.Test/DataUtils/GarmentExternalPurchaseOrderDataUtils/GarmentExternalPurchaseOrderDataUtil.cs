using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils
{
    public class GarmentExternalPurchaseOrderDataUtil
    {
        private readonly GarmentExternalPurchaseOrderFacade facade;
        private readonly GarmentInternalPurchaseOrderDataUtil garmentPurchaseOrderDataUtil;

        public GarmentExternalPurchaseOrderDataUtil(GarmentExternalPurchaseOrderFacade facade, GarmentInternalPurchaseOrderDataUtil garmentPurchaseOrderDataUtil)
        {
            this.facade = facade;
            this.garmentPurchaseOrderDataUtil = garmentPurchaseOrderDataUtil;
        }

        public async Task<GarmentExternalPurchaseOrder> GetNewDataFabric()
        {
            var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
            return new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "CMT",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = DateTimeOffset.Now,

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = false,
                IsOverBudget = false,
                IsPosted = false,
                IsIncomeTax=true,
                IsUseVat=true,
                VatId = "1",
                VatRate = "10",

                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        PO_SerialNumber = datas[0].Items.First().PO_SerialNumber,
                        POId=(int)datas[0].Id,
                        PONo=datas[0].PONo,
                        PRNo=datas[0].PRNo,
                        PRId=(int)datas[0].PRId,
                        ProductId = 1,
                        ProductCode = "ProductCode1",
                        ProductName = "FABRIC",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=datas[0].RONo,

                        Remark = "ProductRemark",
                        IsOverBudget=true,
                        OverBudgetRemark="TestRemarkOB",

                        ReceiptQuantity = 0,
                        DOQuantity = 5,

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",
                        SmallQuantity = 50
                    }
                }
            };
        }

        public async Task<GarmentExternalPurchaseOrder> GetNewDataFabric2()
        {
            var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
            return new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "FREE FROM BUYER",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTimeOffset(),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = false,
                IsOverBudget = false,
                IsPosted = false,


                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)datas[0].Id,
                        PONo=datas[0].PONo,
                        PRNo=datas[0].PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "ProductCode1",
                        ProductName = "FABRIC",

                        DealQuantity = 20,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo="RONo123",

                        Remark = "ProductRemark",
                        IsOverBudget=true,
                        OverBudgetRemark="TestRemarkOB",

                        ReceiptQuantity = 0,
                        DOQuantity = 5,

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",
                    }
                }
            };
        }

        public async Task<GarmentExternalPurchaseOrder> GetNewDataACC()
        {
            var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
            return new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "ACCESORIES",


                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTimeOffset(),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = true,
                IsOverBudget = true,
                IsPosted = false,


                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)datas[0].Id,
                        PONo=datas[0].PONo,
                        PRNo=datas[0].PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "ProductCode1",
                        ProductName = "ProductName1",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=datas[0].RONo,

                        Remark = "ProductRemark"
                    }
                }
            };
        }

        public async Task<GarmentExternalPurchaseOrder> GetDataForDo()
        {
            var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
            return new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "pay",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTimeOffset(),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = false,
                IsOverBudget = false,
                IsPosted = false,


                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)datas[0].Id,
                        PONo=datas[0].PONo,
                        PRNo=datas[0].PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "ProductCode1",
                        ProductName = "ProductName1",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=datas[0].RONo,

                        Remark = "ProductRemark",
                        IsOverBudget=true,
                        OverBudgetRemark="TestRemarkOB",

                        ReceiptQuantity = 0,
                        DOQuantity = 1,

                    }
                }
            };
        }

        public async Task<GarmentExternalPurchaseOrder> GetDataForDo2(List<GarmentInternalPurchaseOrder> garmentInternalPurchaseOrders = null)
        {
            var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags(garmentInternalPurchaseOrders));
            var epo = new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "pay",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTimeOffset(),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = false,
                IsOverBudget = false,
                IsPosted = false,


                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>()
            };

            foreach (var data in datas)
            {
                foreach (var item in data.Items)
                {
                    epo.Items.Add(new GarmentExternalPurchaseOrderItem
                    {
                        PO_SerialNumber = item.PO_SerialNumber ?? "PO_SerialNumber1",
                        POId = (int)data.Id,
                        PONo = data.PONo,
                        PRNo = data.PRNo,
                        PRId = (int)data.PRId,
                        ProductId = 1,
                        ProductCode = "ProductCode1",
                        ProductName = "ProductName1",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity = 5,
                        DefaultUomId = 1,
                        DefaultUomUnit = "unit1",

                        UsedBudget = 1,

                        PricePerDealUnit = 1,
                        Conversion = 1,
                        RONo = data.RONo,

                        Remark = "ProductRemark",
                        IsOverBudget = true,
                        OverBudgetRemark = "TestRemarkOB",

                        ReceiptQuantity = 0,
                        DOQuantity = 0,

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",
                    });
                }
            }
            return epo;
        }

        public async Task<GarmentExternalPurchaseOrder> GetTestDataFabric()
        {
            var data = await GetNewDataFabric();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentExternalPurchaseOrder> GetTestData_DOCurrency()
        {
            var data = await GetNewDataFabric();
            data.CurrencyRate = 0;
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentExternalPurchaseOrder> GetTestDataFabric2()
        {
            var data = await GetNewDataFabric2();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentExternalPurchaseOrder> GetTestDataAcc()
        {
            var data = await GetNewDataACC();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentExternalPurchaseOrder> GetTestDataForDo()
        {
            var data = await GetDataForDo();
            await facade.Create(data, "Unit Test");
            return data;
        }
        public async Task<GarmentExternalPurchaseOrder> GetTestDataForDo2(GarmentExternalPurchaseOrder data = null)
        {
            data = data ?? await GetDataForDo2();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetNewTotalData()
        {
            var result = await garmentPurchaseOrderDataUtil.GetTestData();
            var data = result.FirstOrDefault();
            var GarmentExternalPurchaseOrder = new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "T/T PAYMENT",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTime(1970, 1, 1),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = true,
                IsOverBudget = true,
                IsPosted = true,
                IsCanceled = false,
                IsDeleted = false,

                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        IsDeleted = false,
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)data.Id,
                        PONo=data.PONo,
                        PRNo=data.PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "FAB001",
                        ProductName = "FABRIC",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=data.RONo,

                        Remark = "ProductRemark"
                    }
                }
            };
            return (GarmentExternalPurchaseOrder, data);
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetTestData()
        {
            var data = await GetNewTotalData();
            await facade.Create(data.garmentExternalPurchaseOrder, "Unit Test");
            return data;
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetNewTotalData1()
        {
            var result = await garmentPurchaseOrderDataUtil.GetTestData();
            var data = result.FirstOrDefault();
            var GarmentExternalPurchaseOrder = new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = false,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "T/T PAYMENT",
                PaymentType = "payType",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTime(1970, 1, 1),

                CurrencyId = 1,
                CurrencyCode = "currency1",
                CurrencyRate = 1,

                IsApproved = true,
                IsOverBudget = true,
                IsPosted = true,
                IsCanceled = false,
                IsDeleted = false,

                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        IsDeleted = false,
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)data.Id,
                        PONo=data.PONo,
                        PRNo=data.PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "FAB001",
                        ProductName = "FABRIC",

                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=data.RONo,

                        Remark = "ProductRemark"
                    }
                }
            };
            return (GarmentExternalPurchaseOrder, data);
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetTestData1()
        {
            var data = await GetNewTotalData1();
            await facade.Create(data.garmentExternalPurchaseOrder, "Unit Test");
            return data;
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetNewData_VBRequestPOExternal()
        {
            var result = await garmentPurchaseOrderDataUtil.GetTestData();
            var data = result.FirstOrDefault();
            var GarmentExternalPurchaseOrder = new GarmentExternalPurchaseOrder
            {
                SupplierId = 1,
                SupplierCode = "Supplier1",
                SupplierImport = true,
                SupplierName = "supplier1",

                Category = "FABRIC",
                DarkPerspiration = "dark",
                WetRubbing = "wet",
                DryRubbing = "dry",
                LightMedPerspiration = "light",
                Washing = "wash",
                Shrinkage = "shrink",
                QualityStandardType = "quality",
                PieceLength = "piece",
                PaymentMethod = "T/T PAYMENT",
                PaymentType = "CASH",
                IncomeTaxId = "1",
                IncomeTaxName = "income1",
                IncomeTaxRate = "1",

                DeliveryDate = new DateTimeOffset(),
                OrderDate = new DateTime(1970, 1, 1),
                EPONo= "PO700100001",
                CurrencyId = 1,
                CurrencyCode = "IDR",
                CurrencyRate = 1,

                IsApproved = true,
                IsOverBudget = true,
                IsPosted = true,
                IsCanceled = false,
                IsDeleted = false,

                Remark = "Remark1",

                Items = new List<GarmentExternalPurchaseOrderItem>
                {
                    new GarmentExternalPurchaseOrderItem
                    {
                        IsDeleted = false,
                        PO_SerialNumber = "PO_SerialNumber1",
                        POId=(int)data.Id,
                        PONo=data.PONo,
                        PRNo=data.PRNo,
                        PRId=1,
                        ProductId = 1,
                        ProductCode = "FAB001",
                        ProductName = "FABRIC",
                        
                        DealQuantity = 5,
                        BudgetPrice = 5,

                        DealUomId = 1,
                        DealUomUnit = "UomUnit1",

                        DefaultQuantity=5,
                        DefaultUomId=1,
                        DefaultUomUnit="unit1",

                        SmallUomId = 1,
                        SmallUomUnit = "UomUnit1",

                        UsedBudget=1,

                        PricePerDealUnit=1,
                        Conversion=1,
                        RONo=data.RONo,

                        Remark = "ProductRemark"
                    }
                }
            };
            return (GarmentExternalPurchaseOrder, data);
        }

        public async Task<(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder, GarmentInternalPurchaseOrder garmentInternalPurchaseOrder)> GetTestData_VBRequestPOExternal()
        {
            var data = await GetNewData_VBRequestPOExternal();
            await facade.Create(data.garmentExternalPurchaseOrder, "Unit Test");
            return data;
        }
    }
}
