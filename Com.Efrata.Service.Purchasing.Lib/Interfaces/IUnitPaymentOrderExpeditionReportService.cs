using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IUnitPaymentOrderExpeditionReportService
    {
        Task<UnitPaymentOrderExpeditionReportWrapper> GetReport(string no, string supplierCode, string divisionCode, int status, DateTimeOffset dateFrom, DateTimeOffset dateTo, string order, int page, int size);
        Task<MemoryStream> GetExcel(string no, string supplierCode, string divisionCode, int status, DateTimeOffset dateFrom, DateTimeOffset dateTo, string order);
    }
}
