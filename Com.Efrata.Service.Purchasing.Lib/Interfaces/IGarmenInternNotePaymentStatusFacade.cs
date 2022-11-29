using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IGarmenInternNotePaymentStatusFacade
    {
        Tuple<List<GarmentInternNotePaymentStatusViewModel>, int> GetReport(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int page, int size, string Order, int offset);
        MemoryStream GetXLs(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int offset);
    }
}
