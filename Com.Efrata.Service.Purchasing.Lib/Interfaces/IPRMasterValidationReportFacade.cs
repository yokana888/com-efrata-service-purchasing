using Com.Efrata.Service.Purchasing.Lib.ViewModels.PRMasterValidationReportViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
	public interface IPRMasterValidationReportFacade
	{
       Tuple<List<PRMasterValidationReportViewModel>, int> GetDisplayReport(string unit, string sectionName, DateTime? dateFrom, DateTime? dateTo, string Order, int offset);

       MemoryStream GenerateExcel(string unit, string sectionName, DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
