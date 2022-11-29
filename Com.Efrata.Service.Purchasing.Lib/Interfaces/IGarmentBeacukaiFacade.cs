using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
	public interface IGarmentBeacukaiFacade
	{
		Tuple<List<GarmentBeacukai>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
		GarmentBeacukai ReadById(int id);
		//Task<List<ImportValueViewModel>> ReadImportValue(string keyword);
		Task<int> Create(GarmentBeacukai m, string user, int clientTimeZoneOffset = 7);
		Task<int> Update(int id, GarmentBeacukaiViewModel vm,GarmentBeacukai  m, string user, int clientTimeZoneOffset = 7);
		int Delete(int id, string username);
        List<object> ReadBCByPOSerialNumber(string Keyword = null, string Filter = "{}");
        List<object> ReadBCByPOSerialNumbers(string Keyword);

    }
}
