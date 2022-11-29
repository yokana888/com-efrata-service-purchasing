using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentSupplierBalanceDebtFacades
{
    public class GarmentSupplierBalanceDebtFacade : IBalanceDebtFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentSupplierBalanceDebt> dbSet;
        private readonly DbSet<GarmentDeliveryOrder> dbSetDO;
        private string USER_AGENT = "Facade";

        public GarmentSupplierBalanceDebtFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentSupplierBalanceDebt>();
            this.dbSetDO = dbContext.Set<GarmentDeliveryOrder>();
        }
        public Tuple<List<GarmentSupplierBalanceDebt>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentSupplierBalanceDebt> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "SupplierName", "Import"
            };

            Query = QueryHelper<GarmentSupplierBalanceDebt>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentSupplierBalanceDebt>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentSupplierBalanceDebt>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentSupplierBalanceDebt> pageable = new Pageable<GarmentSupplierBalanceDebt>(Query, Page - 1, Size);
            List<GarmentSupplierBalanceDebt> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }
        public async Task<int> Create(GarmentSupplierBalanceDebt m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                        m.TotalValas += item.Valas;
                        m.TotalAmountIDR += item.IDR;
                    }

                    this.dbSet.Add(m);

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
        public GarmentSupplierBalanceDebt ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                .FirstOrDefault();
            return model;
        }

        public ReadResponse<dynamic> ReadLoader(int Page = 1, int Size = 25, string Order = "{}", int year = 0, string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]")
        {
            IQueryable<GarmentDeliveryOrder> Query = dbSetDO;

            List<string> SearchAttributes = JsonConvert.DeserializeObject<List<string>>(Search);
            if (SearchAttributes.Count.Equals(0))
            {
                SearchAttributes = new List<string>() { "DONo" };
            }
            Query = QueryHelper<GarmentDeliveryOrder>.ConfigureSearch(Query, SearchAttributes, Keyword, SearchWith: "StartsWith");

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentDeliveryOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentDeliveryOrder>.ConfigureOrder(Query, OrderDictionary);

            Dictionary<string, string> SelectDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Select);
            var SelectedQuery = QueryHelper<GarmentDeliveryOrder>.ConfigureSelect(Query, SelectDictionary);

            int TotalData = SelectedQuery.Count();
            int days = DateTime.DaysInMonth(year, 12);
            DateTimeOffset filter = new DateTimeOffset(new DateTime(year, 12, days));
            SelectedQuery = SelectedQuery.Where("ArrivalDate <= @0", filter);


            List<dynamic> Data = SelectedQuery
                .Skip((Page - 1) * Size)
                .Take(Size)
                .ToDynamicList();

            return new ReadResponse<dynamic>(Data, TotalData, OrderDictionary);
        }
    }
}
