using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public interface IGarmentDebtBalanceService
    {
        Task<int> CreateFromCustoms(CustomsFormDto form);
        Task<int> UpdateFromInvoice(int deliveryOrderId, InvoiceFormDto form);
        Task<int> UpdateFromInternalNote(int deliveryOrderId, InternalNoteFormDto form);
        Task<int> UpdateFromBankExpenditureNote(int deliveryOrderId, BankExpenditureNoteFormDto form);

        Task<int> RemoveCustoms(int deliveryOrderId);
        Task<int> EmptyInvoice(int deliveryOrderId);
        Task<int> EmptyInternalNote(int deliveryOrderId);
        Task<int> EmptyBankExpenditureNote(int deliveryOrderId);

        List<DispositionDto> GetDispositions(string keyword);
    }
}
