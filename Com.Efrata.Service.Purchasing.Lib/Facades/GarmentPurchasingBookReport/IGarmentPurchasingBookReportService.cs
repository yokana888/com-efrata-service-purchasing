using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport
{
    public interface IGarmentPurchasingBookReportService
    {
        ReportDto GetReport(string billNo, string paymentBill, string garmentCategory, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier);
        List<AutoCompleteDto> GetBillNos(string keyword);
        List<AutoCompleteDto> GetPaymentBills(string keyword);
        List<AutoCompleteDto> GetAccountingCategories(string keyword);
        Task<MemoryStream> GenerateExcel(string billNo, string paymentBill, string garmentCategory, DateTimeOffset startDate, DateTimeOffset endDate, bool isForeignCurrency, bool isImportSupplier, int timeZone);
    }
}
