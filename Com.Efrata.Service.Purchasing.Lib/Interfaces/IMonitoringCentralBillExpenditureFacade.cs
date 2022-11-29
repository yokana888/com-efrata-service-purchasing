using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCentralBillExpenditureViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
	public interface IMonitoringCentralBillExpenditureFacade
	{
        Tuple<List<MonitoringCentralBillExpenditureViewModel>, int> GetMonitoringKeluarBonPusatReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelMonitoringKeluarBonPusat(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);

        Tuple<List<MonitoringCentralBillExpenditureViewModel>, int> GetMonitoringKeluarBonPusatByUserReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelMonitoringKeluarBonPusatByUser(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
    }
}
