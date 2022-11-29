using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades
{
    public class GarmentPOMasterDistributionFacade : IGarmentPOMasterDistributionFacade
    {

        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentPOMasterDistribution> dbSet;
        private readonly IdentityService identityService;

        public GarmentPOMasterDistributionFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = this.dbContext.Set<GarmentPOMasterDistribution>();
        }

        public ReadResponse<dynamic> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]")
        {
            IQueryable<GarmentPOMasterDistribution> Query = dbSet;

            List<string> SearchAttributes = JsonConvert.DeserializeObject<List<string>>(Search);
            if (SearchAttributes.Count < 1)
            {
                SearchAttributes = new List<string>() { "DONo", "SupplierName" };
            }
            Query = QueryHelper<GarmentPOMasterDistribution>.ConfigureSearch(Query, SearchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentPOMasterDistribution>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentPOMasterDistribution>.ConfigureOrder(Query, OrderDictionary);

            Dictionary<string, string> SelectDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Select);
            IQueryable SelectedQuery = Query;
            if (SelectDictionary.Count > 0)
            {
                SelectedQuery = QueryHelper<GarmentPOMasterDistribution>.ConfigureSelect(Query, SelectDictionary);
            }

            int TotalData = SelectedQuery.Count();

            List<dynamic> Data = SelectedQuery
                .Skip((Page - 1) * Size)
                .Take(Size)
                .ToDynamicList();

            return new ReadResponse<dynamic>(Data, TotalData, OrderDictionary);
        }

        public GarmentPOMasterDistribution ReadById(long id)
        {
            var data = dbSet.AsNoTracking().Where(d => d.Id == id)
                .Include(d => d.Items)
                .ThenInclude(i => i.Details)
                .FirstOrDefault();

            return data;
        }

        public async Task<int> Create(GarmentPOMasterDistribution model)
        {
            int Created = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, identityService.Username, USER_AGENT);
                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForCreate(item, identityService.Username, USER_AGENT);
                        if (item.Details != null)
                        {
                            foreach (var detail in item.Details)
                            {
                                EntityExtension.FlagForCreate(detail, identityService.Username, USER_AGENT);
                            }
                        }
                    }

                    dbSet.Add(model);
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

        public async Task<int> Update(long id, GarmentPOMasterDistribution data)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldData = dbSet.Where(d => d.Id == id)
                        .Include(d => d.Items)
                        .ThenInclude(i => i.Details)
                        .Single();

                    EntityExtension.FlagForUpdate(oldData, identityService.Username, USER_AGENT);

                    foreach (var oldItem in oldData.Items)
                    {
                        EntityExtension.FlagForUpdate(oldItem, identityService.Username, USER_AGENT);

                        var newItem = data.Items.Single(i => i.Id == oldItem.Id);

                        if (oldItem.Details != null)
                        {
                            foreach (var oldDetail in oldItem.Details)
                            {
                                var newDetail = (newItem.Details ?? new List<GarmentPOMasterDistributionDetail>()).SingleOrDefault(i => i.Id == oldDetail.Id);

                                if (newDetail == null)
                                {
                                    EntityExtension.FlagForDelete(oldDetail, identityService.Username, USER_AGENT);
                                }
                                else
                                {
                                    oldDetail.Conversion = newDetail.Conversion;
                                    oldDetail.Quantity = newDetail.Quantity;
                                    oldDetail.OverUsageReason = newDetail.OverUsageReason;

                                    EntityExtension.FlagForUpdate(oldDetail, identityService.Username, USER_AGENT);
                                }
                            }
                        }
                    }

                    foreach (var item in data.Items)
                    {
                        var oldItem = oldData.Items.Single(i => i.Id == item.Id);

                        if (item.Details != null)
                        {
                            foreach (var detail in item.Details)
                            {
                                var oldDetail = oldItem.Details.SingleOrDefault(d => d.Id == detail.Id);

                                if (oldDetail == null)
                                {
                                    oldItem.Details.Add(detail);

                                    EntityExtension.FlagForCreate(detail, identityService.Username, USER_AGENT);
                                }
                            }
                        }
                    }

                    Updated = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Updated;
        }

        public async Task<int> Delete(long id)
        {
            int Deleted = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Where(d => d.Id == id)
                        .Include(d => d.Items)
                        .ThenInclude(i => i.Details)
                        .Single();

                    EntityExtension.FlagForDelete(data, identityService.Username, USER_AGENT);

                    foreach (var item in data.Items)
                    {
                        EntityExtension.FlagForDelete(item, identityService.Username, USER_AGENT);

                        if (item.Details != null)
                        {
                            foreach (var detail in item.Details)
                            {
                                EntityExtension.FlagForDelete(detail, identityService.Username, USER_AGENT);
                            }
                        }
                    }

                    Deleted = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Deleted;
        }

        public Dictionary<string, decimal> GetOthersQuantity(GarmentPOMasterDistributionViewModel viewModel)
        {
            var poSerialNumbers = viewModel.Items.SelectMany(i => i.Details.Where(w => !string.IsNullOrWhiteSpace(w.POSerialNumber)).Select(d => d.POSerialNumber)).ToHashSet();
            var id = viewModel.Id;
            var quantities = from p in dbSet
                             join i in dbContext.GarmentPOMasterDistributionItems on p.Id equals i.POMasterDistributionId
                             join d in dbContext.GarmentPOMasterDistributionDetails on i.Id equals d.POMasterDistributionItemId
                             where p.Id != id && poSerialNumbers.Contains(d.POSerialNumber)
                             select new { d.POSerialNumber, d.Quantity };

            return quantities.ToList().GroupBy(g => g.POSerialNumber).ToDictionary(k => k.Key, k => k.Sum(s => s.Quantity));
        }
    }
}
