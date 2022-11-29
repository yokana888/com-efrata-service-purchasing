using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.Models;
using Com.Efrata.Service.Purchasing.Lib.Migrations;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderReturFacades
{
    public class GarmentUnitDeliveryOrderReturFacade : IGarmentUnitDeliveryOrderReturFacade
    {
        private string USER_AGENT = "Facade";

        public readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentUnitDeliveryOrder> dbSet;
        private readonly IMapper mapper;

        public GarmentUnitDeliveryOrderReturFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentUnitDeliveryOrder>();
            mapper = serviceProvider == null ? null : (IMapper)serviceProvider.GetService(typeof(IMapper));
        }
        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitDeliveryOrder> Query = dbSet;

            if (!string.IsNullOrWhiteSpace(identityService.Username))
            {
                Query = Query.Where(x => x.CreatedBy == identityService.Username);
            }

            Query = Query.Where(x => x.UnitDOType == "RETUR").Select(m => new GarmentUnitDeliveryOrder
            {
                Id = m.Id,
                UnitDONo = m.UnitDONo,
                UnitDODate = m.UnitDODate,
                UnitDOType = m.UnitDOType,
                UnitSenderCode = m.UnitSenderCode,
                UnitSenderName = m.UnitSenderName,
                StorageName = m.StorageName,
                StorageCode = m.StorageCode,
                DONo=m.DONo,
                IsUsed = m.IsUsed,
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc
            });

            List<string> searchAttributes = new List<string>()
            {
                "UnitDONo", "UnitDOType","StorageName","UnitSenderCode","UnitSenderName","DONo"
            };

            Query = QueryHelper<GarmentUnitDeliveryOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentUnitDeliveryOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentUnitDeliveryOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentUnitDeliveryOrder> pageable = new Pageable<GarmentUnitDeliveryOrder>(Query, Page - 1, Size);
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(pageable.Data.Select(s => new
            {
                s.Id,
                s.UnitDONo,
                s.UnitDODate,
                s.UnitDOType,
                s.StorageName,
                s.CreatedBy,
                s.LastModifiedUtc,
                s.UnitSenderName,
                s.DONo
            }));

            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }
        public async Task<int> Create(GarmentUnitDeliveryOrder garmentUnitDeliveryOrder)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    garmentUnitDeliveryOrder.Items = garmentUnitDeliveryOrder.Items.Where(x => x.IsSave).ToList();

                    EntityExtension.FlagForCreate(garmentUnitDeliveryOrder, identityService.Username, USER_AGENT);

                    garmentUnitDeliveryOrder.UnitDONo = await GenerateNo(garmentUnitDeliveryOrder);

                    foreach (var garmentUnitDeliveryOrderItem in garmentUnitDeliveryOrder.Items)
                    {
                        EntityExtension.FlagForCreate(garmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);

                        GarmentDeliveryOrderDetail doDetail = dbContext.GarmentDeliveryOrderDetails.Single(s => s.Id.Equals(garmentUnitDeliveryOrderItem.DODetailId));
                        
                        doDetail.ReturQuantity = doDetail.ReturQuantity + garmentUnitDeliveryOrderItem.ReturQuantity;

                        GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == garmentUnitDeliveryOrderItem.URNItemId);
                        EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                        garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity + (decimal)garmentUnitDeliveryOrderItem.Quantity;

                        GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == garmentUnitDeliveryOrderItem.URNItemId);
                        if (garmentDOItems != null)
                            garmentDOItems.RemainingQuantity -= (decimal)garmentUnitDeliveryOrderItem.Quantity;
                    }

                    dbSet.Add(garmentUnitDeliveryOrder);

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

        async Task<string> GenerateNo(GarmentUnitDeliveryOrder model)
        {
            DateTimeOffset dateTimeOffset = model.UnitDODate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0));
            string Month = dateTimeOffset.ToString("MM");
            string Year = dateTimeOffset.ToString("yy");
            string Day = dateTimeOffset.ToString("dd");

            string no = string.Concat("DO", model.UnitSenderCode, Year, Month, Day);
            int Padding = 3;

            var lastDataByNo = await dbSet.Where(w => w.UnitDONo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.UnitDONo).FirstOrDefaultAsync();

            if (lastDataByNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int.TryParse(lastDataByNo.UnitDONo.Replace(no, ""), out int lastNoNumber);
                return string.Concat(no, (lastNoNumber + 1).ToString().PadLeft(Padding, '0'));
            }
        }

        public async Task<int> Delete(int id)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var garmentUnitDeliveryOrder = dbSet
                        .Include(m => m.Items)
                        .SingleOrDefault(m => m.Id == id);

                    EntityExtension.FlagForDelete(garmentUnitDeliveryOrder, identityService.Username, USER_AGENT);
                    foreach (var garmentUnitDeliveryOrderItem in garmentUnitDeliveryOrder.Items)
                    {
                        EntityExtension.FlagForDelete(garmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);

                        GarmentDeliveryOrderDetail doDetail = dbContext.GarmentDeliveryOrderDetails.Single(s => s.Id.Equals(garmentUnitDeliveryOrderItem.DODetailId));
                        
                        doDetail.ReturQuantity = doDetail.ReturQuantity - garmentUnitDeliveryOrderItem.ReturQuantity;

                        GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == garmentUnitDeliveryOrderItem.URNItemId);
                        EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                        garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)garmentUnitDeliveryOrderItem.Quantity;

                        GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == garmentUnitDeliveryOrderItem.URNItemId);
                        if (garmentDOItems != null)
                            garmentDOItems.RemainingQuantity += (decimal)garmentUnitDeliveryOrderItem.Quantity;
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

        

        public GarmentUnitDeliveryOrder ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
               .Include(m => m.Items)
               .FirstOrDefault();
            return model;
        }

        public async Task<int> Update(int id, GarmentUnitDeliveryOrder garmentUnitDeliveryOrder)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    garmentUnitDeliveryOrder.Items = garmentUnitDeliveryOrder.Items.Where(x => x.IsSave).ToList();

                    var oldGarmentUnitDeliveryOrder = dbSet
                        .Include(d => d.Items)
                        //.AsNoTracking()
                        .Single(m => m.Id == id);

                    EntityExtension.FlagForUpdate(oldGarmentUnitDeliveryOrder, identityService.Username, USER_AGENT);

                    foreach (var garmentUnitDeliveryOrderItem in garmentUnitDeliveryOrder.Items)
                    {
                        if (garmentUnitDeliveryOrderItem.Id != 0)
                        {
                            var oldGarmentUnitDeliveryOrderItem = oldGarmentUnitDeliveryOrder.Items.FirstOrDefault(i => i.Id == garmentUnitDeliveryOrderItem.Id);

                            EntityExtension.FlagForUpdate(oldGarmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);
                            GarmentDeliveryOrderDetail doDetail = dbContext.GarmentDeliveryOrderDetails.Single(s => s.Id.Equals(garmentUnitDeliveryOrderItem.DODetailId));

                            doDetail.ReturQuantity = doDetail.ReturQuantity - oldGarmentUnitDeliveryOrderItem.ReturQuantity+ garmentUnitDeliveryOrderItem.ReturQuantity;

                            GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == oldGarmentUnitDeliveryOrderItem.URNItemId);
                            EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                            garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)oldGarmentUnitDeliveryOrderItem.Quantity + (decimal)garmentUnitDeliveryOrderItem.Quantity;

                            GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == garmentUnitDeliveryOrderItem.URNItemId);
                            if (garmentDOItems != null)
                                garmentDOItems.RemainingQuantity = garmentDOItems.RemainingQuantity + (decimal)oldGarmentUnitDeliveryOrderItem.Quantity - (decimal)garmentUnitDeliveryOrderItem.Quantity;

                            oldGarmentUnitDeliveryOrderItem.Quantity = garmentUnitDeliveryOrderItem.Quantity;
                            oldGarmentUnitDeliveryOrderItem.ReturQuantity = garmentUnitDeliveryOrderItem.ReturQuantity;
                            oldGarmentUnitDeliveryOrderItem.DefaultDOQuantity = garmentUnitDeliveryOrderItem.DefaultDOQuantity;

                        }
                        else
                        {
                            EntityExtension.FlagForCreate(garmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);
                            oldGarmentUnitDeliveryOrder.Items.Add(garmentUnitDeliveryOrderItem);
                            GarmentDeliveryOrderDetail doDetail = dbContext.GarmentDeliveryOrderDetails.Single(s => s.Id.Equals(garmentUnitDeliveryOrderItem.DODetailId));

                            doDetail.ReturQuantity = doDetail.ReturQuantity + garmentUnitDeliveryOrderItem.ReturQuantity;

                            GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == garmentUnitDeliveryOrderItem.URNItemId);
                            EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                            garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity + (decimal)garmentUnitDeliveryOrderItem.Quantity;

                            GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == garmentUnitDeliveryOrderItem.URNItemId);
                            if (garmentDOItems != null)
                                garmentDOItems.RemainingQuantity = garmentDOItems.RemainingQuantity - (decimal)garmentUnitDeliveryOrderItem.Quantity;

                        }
                    }

                    foreach (var oldGarmentUnitDeliveryOrderItem in oldGarmentUnitDeliveryOrder.Items)
                    {
                        var newGarmentUnitDeliveryOrderItem = garmentUnitDeliveryOrder.Items.FirstOrDefault(i => i.Id == oldGarmentUnitDeliveryOrderItem.Id);
                        if (newGarmentUnitDeliveryOrderItem == null)
                        {
                            EntityExtension.FlagForDelete(oldGarmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);
                            GarmentDeliveryOrderDetail doDetail = dbContext.GarmentDeliveryOrderDetails.Single(s => s.Id.Equals(oldGarmentUnitDeliveryOrderItem.DODetailId));

                            doDetail.ReturQuantity = doDetail.ReturQuantity - oldGarmentUnitDeliveryOrderItem.ReturQuantity;

                            GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == oldGarmentUnitDeliveryOrderItem.URNItemId);
                            EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                            garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)oldGarmentUnitDeliveryOrderItem.Quantity;

                            GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == oldGarmentUnitDeliveryOrderItem.URNItemId);
                            if (garmentDOItems != null)
                                garmentDOItems.RemainingQuantity = garmentDOItems.RemainingQuantity + (decimal)oldGarmentUnitDeliveryOrderItem.Quantity;

                        }
                    }

                    //dbSet.Update(garmentUnitDeliveryOrder);

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
    }
}
