using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades
{
    public class GarmentCorrectionNoteFacade : IGarmentCorrectionNoteFacade
    {
        private string USER_AGENT = "Facade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentCorrectionNote> dbSet;

        public GarmentCorrectionNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentCorrectionNote>();
        }

        public Tuple<List<GarmentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentCorrectionNote> Query = dbSet;

            Query = Query.Select(m => new GarmentCorrectionNote
            {
                Id = m.Id,
                CorrectionNo = m.CorrectionNo,
                CorrectionType = m.CorrectionType,
                CorrectionDate = m.CorrectionDate,
                SupplierName = m.SupplierName,
                DONo = m.DONo,
                UseIncomeTax = m.UseIncomeTax,
                UseVat = m.UseVat,
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc
            });

            List<string> searchAttributes = new List<string>()
            {
                "CorrectionNo", "CorrectionType", "SupplierName", "DONo"
            };

            Query = QueryHelper<GarmentCorrectionNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentCorrectionNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentCorrectionNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentCorrectionNote> pageable = new Pageable<GarmentCorrectionNote>(Query, Page - 1, Size);
            List<GarmentCorrectionNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }
    }
}
