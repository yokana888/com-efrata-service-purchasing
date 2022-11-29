using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IBalanceDebtFacade
    {
        //Task<int> UploadData(List<GarmentSupplierBalanceDebt> data, string Username);
        ReadResponse<dynamic> ReadLoader(int Page = 1, int Size = 25, string Order = "{}", int year = 0, string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]");
        Tuple<List<GarmentSupplierBalanceDebt>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        //List<string> CsvHeader { get; }
        Task<int> Create(GarmentSupplierBalanceDebt m, string user, int clientTimeZoneOffset = 7);
        GarmentSupplierBalanceDebt ReadById(int id);
        //ReadResponse<dynamic> ReadLoader(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]");
    }
}
