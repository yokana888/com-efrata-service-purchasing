using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentClosingDateFacades
{
    public class GarmentClosingDateFacade : IGarmentClosingDateFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentClosingDate> dbSet;
        public readonly IServiceProvider serviceProvider;
        private string USER_AGENT = "Facade";

        public GarmentClosingDateFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentClosingDate>();
            this.serviceProvider = serviceProvider;
        }
        public Tuple<List<GarmentClosingDate>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentClosingDate> Query = this.dbSet;

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentClosingDate>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentClosingDate> pageable = new Pageable<GarmentClosingDate>(Query, Page - 1, Size);
            List<GarmentClosingDate> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public async Task<int> Create(GarmentClosingDate m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (m.Id != 0)
                    {
                        var exist = dbSet.Where(a => a.Id == m.Id).AsNoTracking().Single();
                        exist.CloseDate = m.CloseDate;
                        EntityExtension.FlagForUpdate(m, user, USER_AGENT);

                        dbSet.Update(m);
                    }
                    else
                    {

                        EntityExtension.FlagForCreate(m, user, USER_AGENT);


                        this.dbSet.Add(m);
                    }


                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }
    }
}
