using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil
{
    public class SendToVerificationDataUtil
    {
        private readonly PurchasingDocumentExpeditionFacade Facade;

        public SendToVerificationDataUtil(PurchasingDocumentExpeditionFacade Facade)
        {
            this.Facade = Facade;
        }
        public PurchasingDocumentExpedition GetNewData(UnitPaymentOrder unitPaymentOrder = null)
        {
            List<PurchasingDocumentExpeditionItem> Items = new List<PurchasingDocumentExpeditionItem>()
            {
                new PurchasingDocumentExpeditionItem()
                {
                    ProductId = "ProductId",
                    ProductCode = "ProductCode",
                    ProductName = "ProductName",
                    Price = 10000,
                    Quantity = 5,
                    Uom = "MTR",
                    UnitId = "UnitId",
                    UnitCode = "UnitCode",
                    UnitName = "UnitName"
                }
            };

            PurchasingDocumentExpedition TestData = new PurchasingDocumentExpedition()
            {
                SendToVerificationDivisionDate = DateTimeOffset.UtcNow,
                UnitPaymentOrderNo = unitPaymentOrder == null ? Guid.NewGuid().ToString() : unitPaymentOrder.UPONo,
                UPODate = unitPaymentOrder == null ? DateTimeOffset.UtcNow : unitPaymentOrder.Date,
                DueDate = DateTimeOffset.UtcNow,
                InvoiceNo = "Invoice",
                PaymentMethod = "CASH",
                SupplierCode = "Supplier",
                SupplierName = "Supplier",
                DivisionCode = "Division",
                DivisionName = "Division",
                IncomeTax = 20000,
                Vat = 100000,
                IncomeTaxId = "IncomeTaxId",
                IncomeTaxName = "IncomeTaxName",
                IncomeTaxRate = 2,
                TotalPaid = 1000000,
                Currency = "IDR",
                Items = Items,
            };

            return TestData;
        }

        public async Task<PurchasingDocumentExpedition> GetTestData(PurchasingDocumentExpedition purchasingDocumentExpedition = null)
        {
            PurchasingDocumentExpedition model = purchasingDocumentExpedition ?? GetNewData();
            await Facade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");
            return model;
        }
    }
}
