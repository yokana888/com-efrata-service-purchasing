using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using System;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IUnitPaymentOrderUnpaidReportFacade
    {
        Task<ReadResponse<object>> GetReport(int Size, int Page, string Order, string UnitPaymentOrderNo, string SupplierCode, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Offset);
    }
}
