using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class BankExpenditureNoteFormDto
    {
        public BankExpenditureNoteFormDto()
        {

        }

        public BankExpenditureNoteFormDto(int bankExpenditureNoteId, string bankExpenditureNoteNo, double bankExpenditureNoteInvoiceAmount, double currencyBankExpenditureNoteInvoiceAmount)
        {
            BankExpenditureNoteId = bankExpenditureNoteId;
            BankExpenditureNoteNo = bankExpenditureNoteNo;
            BankExpenditureNoteInvoiceAmount = bankExpenditureNoteInvoiceAmount;
            CurrencyBankExpenditureNoteInvoiceAmount = currencyBankExpenditureNoteInvoiceAmount;
        }

        public int BankExpenditureNoteId { get; private set; }
        public string BankExpenditureNoteNo { get; private set; }
        public double BankExpenditureNoteInvoiceAmount { get; private set; }
        public double CurrencyBankExpenditureNoteInvoiceAmount { get; private set; }
    }
}
