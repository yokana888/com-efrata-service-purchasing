using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.PPHBankExpenditureNoteDataUtil
{
    public class PPHBankExpenditureNoteDataUtil
    {
        private readonly PPHBankExpenditureNoteFacade Facade;
        private readonly PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        public PPHBankExpenditureNoteDataUtil(PPHBankExpenditureNoteFacade Facade, PurchasingDocumentAcceptanceDataUtil pdaDataUtil)
        {
            this.Facade = Facade;
            this.pdaDataUtil = pdaDataUtil;
        }

        public async Task<PPHBankExpenditureNoteItem> GetItemNewData()
        {
            PurchasingDocumentExpedition purchasingDocumentExpedition = await Task.Run(() => this.pdaDataUtil.GetCashierTestData());

            return new PPHBankExpenditureNoteItem()
            {
                Id = 0,
                PurchasingDocumentExpeditionId = purchasingDocumentExpedition.Id,
                UnitPaymentOrderNo = purchasingDocumentExpedition.UnitPaymentOrderNo,
            };
        }

        public async Task<PPHBankExpenditureNote> GetNewData()
        {
            PurchasingDocumentExpedition purchasingDocumentExpedition1 = await Task.Run(() => this.pdaDataUtil.GetCashierTestData());
            PurchasingDocumentExpedition purchasingDocumentExpedition2 = await Task.Run(() => this.pdaDataUtil.GetCashierTestData());
            
            List<PPHBankExpenditureNoteItem> Items = new List<PPHBankExpenditureNoteItem>()
            {
                new PPHBankExpenditureNoteItem()
                {
                    PurchasingDocumentExpeditionId = purchasingDocumentExpedition1.Id,
                    UnitPaymentOrderNo = purchasingDocumentExpedition1.UnitPaymentOrderNo,
                },
                new PPHBankExpenditureNoteItem()
                {
                    PurchasingDocumentExpeditionId = purchasingDocumentExpedition2.Id,
                    UnitPaymentOrderNo = purchasingDocumentExpedition2.UnitPaymentOrderNo,
                }
            };

            PPHBankExpenditureNote TestData = new PPHBankExpenditureNote()
            {
                Date = DateTimeOffset.UtcNow,
                BankAccountNumber = "100020003000",
                BankAccountName = "BankAccountName",
                BankCode = "BankCode",
                BankId = "1",
                BankName = "BankName",
                BGNo = "BGNo",
                IncomeTaxId = "IncomeTaxId",
                IncomeTaxName = "IncomeTaxName",
                IncomeTaxRate = 2,
                Items = Items,
                No = "No",
                TotalDPP = 1100000,
                TotalIncomeTax = 100000,
                Currency = "USD"
            };

            return TestData;
        }

        public async Task<PPHBankExpenditureNote> GetTestData()
        {
            PPHBankExpenditureNote model = await GetNewData();
            await Facade.Create(model, "Unit Test");
            return await Facade.ReadById(model.Id);
        }
    }
}
