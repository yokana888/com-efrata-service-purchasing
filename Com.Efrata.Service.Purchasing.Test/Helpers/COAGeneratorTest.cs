using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
    public class COAGeneratorTest
    {
        [Fact]
        public void Should_Success_Generate_UnitAndDivision()
        {
            var result = COAGenerator.GetDivisionAndUnitCOACode("", "");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_Spinning1()
        {
            var result = COAGenerator.GetDebtCOA(false, "SPINNING", "S1");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_Spinning2()
        {
            var result = COAGenerator.GetDebtCOA(false, "SPINNING", "S2");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_Spinning3()
        {
            var result = COAGenerator.GetDebtCOA(false, "SPINNING", "S3");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_SpinningMS()
        {
            var result = COAGenerator.GetDebtCOA(false, "SPINNING", "S4");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_SpinningOther()
        {
            var result = COAGenerator.GetDebtCOA(true, "SPINNING", "other");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_PurchasingWeavingKK()
        {
            var result = COAGenerator.GetPurchasingCOA("WEAVING", "W1", "EM");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_PurchasingWeavingE()
        {
            var result = COAGenerator.GetPurchasingCOA("WEAVING", "W2", "BB");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_PurchasingWeavingOther()
        {
            var result = COAGenerator.GetPurchasingCOA("WEAVING", "other", "BP");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_FinishingPrintingF1()
        {
            var result = COAGenerator.GetPurchasingCOA("DYEING&PRINTING", "F1", "BJ");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingF2()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "F2", "BB");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "BJ");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther_Chemical()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "E");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther_BahanBakar()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "MM");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther_Pelumas()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "PL");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther_SparePart()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "SP");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockFinishingPrintingOther_BahanPembantu()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "other", "BP");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentC1A()
        {
            var result = COAGenerator.GetIncomeTaxCOA("Final", "EFRATA", "C1A");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentC1B()
        {
            var result = COAGenerator.GetIncomeTaxCOA("PASAL21", "EFRATA", "C1B");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentC2A()
        {
            var result = COAGenerator.GetIncomeTaxCOA("PASAL23", "EFRATA", "EFR");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentC2B()
        {
            var result = COAGenerator.GetIncomeTaxCOA("PASAL26", "EFRATA", "C2B");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentC2C()
        {
            var result = COAGenerator.GetIncomeTaxCOA("PASAL26", "EFRATA", "C2C");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_IncomeTaxGarmentOther()
        {
            var result = COAGenerator.GetIncomeTaxCOA("PASAL26", "EFRATA", "other");
            Assert.NotNull(result);
        }
        
        [Fact]
        public void Should_Success_Get_COA_PurchasingNotExist()
        {
            var result = COAGenerator.GetPurchasingCOA("DYEING&PRINTING", "F1", "BELUMADA");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_StockNotExist()
        {
            var result = COAGenerator.GetStockCOA("DYEING&PRINTING", "F1", "BELUMADA");
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Success_Get_COA_TaxNotExist()
        {
            var result = COAGenerator.GetIncomeTaxCOA("BELUMADA", "DYEING&PRINTING", "F1");
            Assert.NotNull(result);
        }

        [Fact]
        public void Set_Purchase_Order_Delivery_Order_Duration_Report_ViewModel()
        {
            var result = new GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel()
            {
                artikelNo = "",
                buyerName = "",
                category = "",
                dateDiff = 0,
                deliveryOrderNo = "",
                doCreatedDate = DateTime.Now,
                expectedDate = DateTime.Now,
                planPO = "",
                poEksCreatedDate = DateTime.Now,
                poEksNo = "",
                poIntCreatedDate = DateTime.Now,
                poIntNo = "",
                productCode = "",
                productName = "",
                productPrice = 0,
                productQuantity = 0,
                productUom = "",
                roNo = "",
                staff = "",
                supplierCode = "",
                supplierDoDate = DateTime.Now,
                supplierName = "",
                unit = ""
            };

            Assert.NotNull(result);
        }

        [Fact]
        public void Set_Garment_External_Purchase_Order_Over_Budget_Monitoring_ViewModel()
        {
            var result = new GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel()
            {
                budgetPrice = 0,
                no = 1,
                overBudgetRemark = "",
                overBudgetValue = 0,
                overBudgetValuePercentage = 0,
                poExtDate = "",
                poExtNo = "",
                prDate = "",
                prNo = "",
                prRefNo = "",
                price = 0,
                productCode = "",
                productDesc = "",
                productName = "",
                quantity = 0,
                status = "",
                supplierCode = "",
                supplierName = "",
                totalBudgetPrice = 0,
                totalPrice = 0,
                unit = "",
                uom = "",
            };

            Assert.NotNull(result);
        }
    }
}
