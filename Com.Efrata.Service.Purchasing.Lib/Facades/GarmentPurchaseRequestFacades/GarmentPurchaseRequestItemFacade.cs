using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades
{
    public class GarmentPurchaseRequestItemFacade : IGarmentPurchaseRequestItemFacade
    {
        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentPurchaseRequestItem> dbSet;

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public GarmentPurchaseRequestItemFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentPurchaseRequestItem>();
        }

        public ReadResponse<dynamic> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = null, string Search = "[]")
        {
            IQueryable<GarmentPurchaseRequestItem> Query = dbSet;

            List<string> SearchAttributes = JsonConvert.DeserializeObject<List<string>>(Search);
            if (SearchAttributes.Count < 1)
            {
                SearchAttributes = new List<string>() { "PO_SerialNumber" };
            }
            Query = QueryHelper<GarmentPurchaseRequestItem>.ConfigureSearch(Query, SearchAttributes, Keyword, WithAny:false);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentPurchaseRequestItem>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentPurchaseRequestItem>.ConfigureOrder(Query, OrderDictionary);

            IQueryable SelectedQuery = Query;
            if (!string.IsNullOrWhiteSpace(Select))
            {
                SelectedQuery = Query.Select(Select);
            }

            int totalData = SelectedQuery.Count();

            if (Size > 0)
            {
                SelectedQuery = SelectedQuery
                    .Skip((Page - 1) * Size)
                    .Take(Size);
            }

            List<dynamic> Data = SelectedQuery
                .ToDynamicList();

            return new ReadResponse<dynamic>(Data, totalData, OrderDictionary);
        }

        public async Task<int> Patch(string id, JsonPatchDocument<GarmentPurchaseRequestItem> jsonPatch)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var IDs = JsonConvert.DeserializeObject<List<long>>(id);
                    foreach (var ID in IDs)
                    {
                        var data = dbSet.Where(d => d.Id == ID)
                            .Single();

                        EntityExtension.FlagForUpdate(data, identityService.Username, USER_AGENT);

                        jsonPatch.ApplyTo(data);
                    }

                    Updated = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }
    }
}
