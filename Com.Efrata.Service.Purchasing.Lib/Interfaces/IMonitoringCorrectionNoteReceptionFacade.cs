using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCorrectionNoteReceptionViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IMonitoringCorrectionNoteReceptionFacade
    {
        Tuple<List<MonitoringCorrectionNoteReceptionViewModel>, int> GetMonitoringTerimaNKReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelMonitoringTerimaNK(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);

        Tuple<List<MonitoringCorrectionNoteReceptionViewModel>, int> GetMonitoringTerimaNKByUserReport(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelMonitoringTerimaNKByUser(DateTime? dateFrom, DateTime? dateTo, string jnsBC, int page, int size, string Order, int offset);
    }
}
