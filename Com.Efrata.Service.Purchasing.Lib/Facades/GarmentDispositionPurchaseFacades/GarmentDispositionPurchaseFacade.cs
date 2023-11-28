using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades
{
    public class GarmentDispositionPurchaseFacade : IGarmentDispositionPurchaseFacade
    {
        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentDispositionPurchase> dbSet;
        public readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly IMapper mapper;


        public GarmentDispositionPurchaseFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider, IdentityService identityService, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.GarmentDispositionPurchases;
            this.serviceProvider = serviceProvider;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.mapper = mapper;
        }

        public async Task<int> Post(FormDto model)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FixingVatAndIncomeTax();
                    GarmentDispositionPurchase dataModel = mapper.Map<FormDto, GarmentDispositionPurchase>(model);
                    EntityExtension.FlagForCreate(dataModel, identityService.Username, USER_AGENT);
                    var DispositionNo = await GenerateNo(identityService.TimezoneOffset);
                    dataModel.DispositionNo = DispositionNo;
                    dataModel.Position = Enums.PurchasingGarmentExpeditionPosition.Purchasing;
                    dataModel.GarmentDispositionPurchaseItems.ForEach(s=> {
                        EntityExtension.FlagForCreate(s, identityService.Username, USER_AGENT);

                        //var DispoDetail = this.dbContext.GarmentDispositionPurchaseDetailss.Where(a => a.GarmentDispositionPurchaseItemId == dataModel.Id).FirstOrDefault();
                        //var EPOItems1 = this.dbContext.GarmentExternalPurchaseOrderItems.Where(a => a.Id == DispoDetail.EPO_POId).FirstOrDefault();
                        //var EPO = this.dbContext.GarmentExternalPurchaseOrders.Where(a => a.Id == EPOItems1.GarmentEPOId).FirstOrDefault();

                        //s.VatId = EPO.VatId;
                        //s.VatRate = EPO.VatRate;
                        s.IsDispositionCreated = true;
                        s.GarmentDispositionPurchaseDetails.ForEach(t =>
                        {
                            if(t.QTYRemains<= 0)
                            {
                                var EPOItems = this.dbContext.GarmentExternalPurchaseOrderItems.Where(a => a.Id == t.EPO_POId).FirstOrDefault();
                                EPOItems.IsDispositionCreatedAll = true;
                                EntityExtension.FlagForUpdate(EPOItems, identityService.Username, USER_AGENT);
                                var afterUpdateModel = this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems);
                            }
                            EntityExtension.FlagForCreate(t, identityService.Username, USER_AGENT);
                        });
                    });

                    var afterAddModel = dbContext.GarmentDispositionPurchases.Add(dataModel);
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

        async Task<string> GenerateNo(int clientTimeZoneOffset)
        {
            DateTimeOffset Now = DateTime.UtcNow;
            string Year = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");

            string no = $"{Year}-{Month}-EGU" ;
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.DispositionNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.DispositionNo).FirstOrDefaultAsync();
            no = $"{no}";

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.DispositionNo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public async Task<int> Delete(int id)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dataModel = dbSet.Include(s=> s.GarmentDispositionPurchaseItems).ThenInclude(s=> s.GarmentDispositionPurchaseDetails).FirstOrDefault(s => s.Id == id);

                    EntityExtension.FlagForDelete(dataModel, identityService.Username, USER_AGENT);

                    var afterDeletedModel = dbContext.GarmentDispositionPurchases.Update(dataModel);

                    dataModel.GarmentDispositionPurchaseItems.ForEach(t =>
                    {
                        var EPOItems1 = this.dbContext.GarmentExternalPurchaseOrders.AsNoTracking().Where(a => a.Id == t.EPOId).FirstOrDefault();
                        EPOItems1.IsDispositionPaidCreatedAll = false;
                        EntityExtension.FlagForUpdate(EPOItems1, identityService.Username, USER_AGENT);
                        var afterUpdateModel1 = this.dbContext.GarmentExternalPurchaseOrders.Update(EPOItems1);

                        t.GarmentDispositionPurchaseDetails.ForEach(s =>
                        {
                            var EPOItems2 = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.Id == s.EPO_POId).FirstOrDefault();
                            EPOItems2.IsDispositionCreatedAll = false;
                            EntityExtension.FlagForUpdate(EPOItems2, identityService.Username, USER_AGENT);
                            var afterUpdateModel2 = this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems2);
                        });
                    });
                    
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

        public async Task<FormDto> GetFormById(int id,bool isVerifiedAmountCalculated= false)
        {
            var dataModel = await dbSet
                .AsNoTracking()
                    .Include(p => p.GarmentDispositionPurchaseItems)
                        .ThenInclude(p => p.GarmentDispositionPurchaseDetails)
                //.Select(s=> {
                //    s.GarmentDispositionPurchaseItems = s.GarmentDispositionPurchaseItems.Select(t => {
                //        t.VerifiedAmount = dbContext.GarmentDispositionPurchaseItems.Where(j => j.EPOId == t.EPOId).Sum(j => (j.VATAmount + j.GarmentDispositionPurchaseDetails.Sum(a => a.PaidPrice)) - j.IncomeTaxAmount)
                //        return t;
                //    }).ToList();
                //    return s;
                //})
                .Where(d => d.Id.Equals(id))
                .FirstOrDefaultAsync();

            var listEPOID = dataModel.GarmentDispositionPurchaseItems.Select(s => s.EPOId).ToList();
            var listItemsWithSameEPOID = dbContext.GarmentDispositionPurchaseItems.Where(s => listEPOID.Contains(s.EPOId));

            dataModel.GarmentDispositionPurchaseItems = dataModel.GarmentDispositionPurchaseItems.Select(s => {
                s.VerifiedAmount = isVerifiedAmountCalculated? listItemsWithSameEPOID.Where(t => t.EPOId == s.EPOId).Sum(t=> t.VerifiedAmount):s.VerifiedAmount;
                return s;
            }).ToList(); 

            var model = mapper.Map<GarmentDispositionPurchase, FormDto>(dataModel);
            model.FixingVatAndIncomeTaxView();
            var modelFixing = model.Items.Select(s => {
                var epo = ReadByEPOWithDisposition(s.EPOId, model.SupplierId, model.CurrencyCode);
                s.IsPayIncomeTax = epo.IsPayIncomeTax;
                s.IsPayVat = epo.IsPayVAT;
                return s;
                });
            model.Items = modelFixing.ToList();
            return model;
        }

        public async Task<DispositionPurchaseIndexDto> GetAll(string keyword, int page, int size,string filter, string order)
        {

            IQueryable<GarmentDispositionPurchase> Query = dbSet;

            if (!string.IsNullOrWhiteSpace(identityService.Username))
            {
                Query = Query.Where(x => x.CreatedBy == identityService.Username);
            }

            Query = Query
                .AsNoTracking()
                    .Include(p => p.GarmentDispositionPurchaseItems)
                        .ThenInclude(p => p.GarmentDispositionPurchaseDetails)
                        //.Where(s=> s.Position == PurchasingGarmentExpeditionPosition.Purchasing)
                        .AsQueryable();

            if (keyword != null)
                Query = Query.Where(s => s.DispositionNo.Contains(keyword) || s.SupplierName.Contains(keyword));

            //var countData = dataModel.Count();

            //var dataList = await dataModel.ToListAsync();


            //var Query = dataModel;

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo"
            };

            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureSearch(Query, searchAttributes, keyword);

            
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            //override by user
            //FilterDictionary.Add("CreatedBy", identityService.Username);
            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);

            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentDispositionPurchase> pageable = new Pageable<GarmentDispositionPurchase>(Query, page - 1, size);
            List<GarmentDispositionPurchase> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            var model = mapper.Map<List<GarmentDispositionPurchase>, List<DispositionPurchaseTableDto>>(Data.ToList());

            var indexModel = new DispositionPurchaseIndexDto(model, page, TotalData);
            return indexModel;
        }

        public DispositionPurchaseReportIndexDto GetReport(int supplierId, string username, DateTimeOffset? dateForm, DateTimeOffset? dateTo, int size, int page)
        {
            var dataModel = dbSet
                .AsNoTracking()
                    .Include(p => p.GarmentDispositionPurchaseItems)
                        .ThenInclude(p => p.GarmentDispositionPurchaseDetails)
                        //.Where(s=> s.Position == PurchasingGarmentExpeditionPosition.Purchasing)
                        .AsQueryable();

            if (supplierId != 0)
                dataModel = dataModel.Where(s => s.SupplierId == supplierId);

            if (!string.IsNullOrEmpty(username))
                dataModel = dataModel.Where(s => s.CreatedBy == username);

            if (dateForm.HasValue)
                dataModel = dataModel.Where(s => s.CreatedUtc >= dateForm.GetValueOrDefault());

            if (dateTo.HasValue)
                dataModel = dataModel.Where(s => s.CreatedUtc <= dateTo.GetValueOrDefault());

            var countData = dataModel.Count();

            var dataList = dataModel.ToList();


            var Query = dataList.AsQueryable();

            if (page != 0 && size != 0)
            {
                Pageable<GarmentDispositionPurchase> pageable = new Pageable<GarmentDispositionPurchase>(Query, page - 1, size);
                List<GarmentDispositionPurchase> Data = pageable.Data.ToList();

                int TotalData = pageable.TotalCount;

                var model = mapper.Map<List<GarmentDispositionPurchase>, List<DispositionPurchaseReportTableDto>>(Data.ToList());

                var indexModel = new DispositionPurchaseReportIndexDto(model, page, countData);
                return indexModel;
            }
            else
            {
                var model = mapper.Map<List<GarmentDispositionPurchase>, List<DispositionPurchaseReportTableDto>>(Query.ToList());

                var indexModel = new DispositionPurchaseReportIndexDto(model, 1, countData);
                return indexModel;
            }
        }

        public async Task<DispositionUserIndexDto> GetListUsers (string keyword)
        {
            var dataModel = dbSet.Select(s=> s.CreatedBy).Distinct()
                        .AsQueryable();

            if (keyword != null)
                dataModel = dataModel.Where(s => s.Contains(keyword));

            var countData = dataModel.Count();

            var resultList = dataModel.Select(s => new DispositionUserDto { Name = s, Username = s });
            var result = new DispositionUserIndexDto(resultList.ToList(), 1, resultList.Count());
            return result;
        }

        public Tuple<List<FormDto>, int, Dictionary<string, string>> Read(PurchasingGarmentExpeditionPosition position, int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", int supplierId=0)
        {
            IQueryable<GarmentDispositionPurchase> Query = this.dbSet;

            if (position != PurchasingGarmentExpeditionPosition.Invalid)
                Query = Query.Where(s => s.Position == position);

            if (supplierId != 0)
                Query = Query.Where(s => s.SupplierId == supplierId);

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo"
            };

            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentDispositionPurchase>.ConfigureOrder(Query, OrderDictionary);
            //Query = Query
            //    .Select(s => new FormDto
            //    {
            //        DispositionNo = s.DispositionNo,
            //        Id = s.Id,
            //        SupplierCode = s.SupplierCode,
            //        SupplierId = s.SupplierId,
            //        SupplierName = s.SupplierName,
            //        Bank = s.Bank,
            //        CurrencyCode = s.CurrencyCode,
            //        CurrencyId = s.CurrencyId,
            //        CurrencyRate = s.CurrencyRate,
            //        ConfirmationOrderNo = s.ConfirmationOrderNo,
            //        //InvoiceNo = s.InvoiceNo,
            //        PaymentMethod = s.PaymentMethod,
            //        PaymentDueDate = s.PaymentDueDate,
            //        CreatedBy = s.CreatedBy,
            //        LastModifiedUtc = s.LastModifiedUtc,
            //        CreatedUtc = s.CreatedUtc,
            //        Amount = s.Amount,
            //        Calculation = s.Calculation,
            //        //Investation = s.Investation,
            //        Position = s.Position,
            //        ProformaNo = s.ProformaNo,
            //        Remark = s.Remark,
            //        UId = s.UId,
            //        CategoryCode = s.CategoryCode,
            //        CategoryId = s.CategoryId,
            //        CategoryName = s.CategoryName,
            //        DPP = s.DPP,
            //        IncomeTaxValue = s.IncomeTaxValue,
            //        VatValue = s.VatValue,
            //        IncomeTaxBy = s.IncomeTaxBy,
            //        DivisionCode = s.DivisionCode,
            //        DivisionId = s.DivisionId,
            //        DivisionName = s.DivisionName,
            //        PaymentCorrection = s.PaymentCorrection,
            //        Items = s.Items.Select(x => new PurchasingDispositionItem()
            //        {
            //            EPOId = x.EPOId,
            //            EPONo = x.EPONo,
            //            Id = x.Id,
            //            IncomeTaxId = x.IncomeTaxId,
            //            IncomeTaxName = x.IncomeTaxName,
            //            IncomeTaxRate = x.IncomeTaxRate,
            //            UseVat = x.UseVat,
            //            UseIncomeTax = x.UseIncomeTax,
            //            UId = x.UId,

            //            Details = x.Details.Select(y => new PurchasingDispositionDetail()
            //            {

            //                UId = y.UId,

            //                DealQuantity = y.DealQuantity,
            //                DealUomId = y.DealUomId,
            //                DealUomUnit = y.DealUomUnit,
            //                Id = y.Id,
            //                PaidPrice = y.PaidPrice,
            //                PaidQuantity = y.PaidQuantity,
            //                PricePerDealUnit = y.PricePerDealUnit,
            //                PriceTotal = y.PriceTotal,
            //                PRId = y.PRId,
            //                PRNo = y.PRNo,
            //                ProductCode = y.ProductCode,
            //                ProductId = y.ProductId,
            //                ProductName = y.ProductName,
            //                PurchasingDispositionItem = y.PurchasingDispositionItem,
            //                PurchasingDispositionItemId = y.PurchasingDispositionItemId,
            //                UnitCode = y.UnitCode,
            //                UnitId = y.UnitId,
            //                UnitName = y.UnitName
            //            }).ToList()
            //        }).ToList()
            //    });
            Pageable<GarmentDispositionPurchase> pageable = new Pageable<GarmentDispositionPurchase>(Query, Page - 1, Size);
            List<FormDto> listMap = mapper.Map<List<FormDto>>(pageable.Data.ToList());
            //List<PurchasingDisposition> Data = pageable.Data.ToList<PurchasingDisposition>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(listMap, TotalData, OrderDictionary);
        }
        public async Task<List<FormDto>> ReadByDispositionNo(string dispositionNo, int page, int size)
        {
            var dataModel = dbSet
                .AsNoTracking()
                    .Include(p => p.GarmentDispositionPurchaseItems)
                        .ThenInclude(p => p.GarmentDispositionPurchaseDetails).AsQueryable();

            if (dispositionNo != null)
                dataModel = dataModel.Where(s => s.DispositionNo.Contains(dispositionNo));


            var dataList = await dataModel.OrderBy(s => s.DispositionNo).ToListAsync();

            var model = mapper.Map<List<GarmentDispositionPurchase>, List<FormDto>>(dataList);
            return model;
        }

        public async Task<int> Update(FormEditDto model)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    //validation
                    var dataExist = this.dbContext.GarmentDispositionPurchases.AsNoTracking().Include(s=> s.GarmentDispositionPurchaseItems).ThenInclude(s=>s.GarmentDispositionPurchaseDetails).FirstOrDefault(s => s.Id == model.Id);
                    if (dataExist == null)
                    {
                        throw new Exception("Data Not Found");
                    }
                    GarmentDispositionPurchase dataModel = mapper.Map<FormEditDto, GarmentDispositionPurchase>(model);
                    //dataModel.GarmentDispositionPurchaseItems.ForEach(s => {
                    foreach (var s in dataModel.GarmentDispositionPurchaseItems)
                    {
                        //createNew Items
                        if (s.Id == 0)
                        {
                            EntityExtension.FlagForCreate(s, identityService.Username, USER_AGENT);
                            s.IsDispositionCreated = true;
                            s.GarmentDispositionPurchaseId = dataModel.Id;
                            var afterCreateItem = this.dbContext.GarmentDispositionPurchaseItems.Add(s);
                            //s.GarmentDispositionPurchaseDetails.ForEach(t =>
                            foreach (var t in s.GarmentDispositionPurchaseDetails)
                            {
                                if (t.QTYRemains <= 0)
                                {
                                    var EPOItems1 = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.Id == t.EPO_POId).FirstOrDefault();
                                    EPOItems1.IsDispositionCreatedAll = true;
                                    EntityExtension.FlagForUpdate(EPOItems1, identityService.Username, USER_AGENT);
                                    var afterUpdateModel1 = this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems1);
                                    //dbContext.SaveChanges();

                                }

                                if (t.Id <= 0)
                                {
                                    EntityExtension.FlagForCreate(t, identityService.Username, USER_AGENT);
                                    t.GarmentDispositionPurchaseItemId = afterCreateItem.Entity.Id;
                                    var afterCreateDetail = this.dbContext.GarmentDispositionPurchaseDetailss.Add(t);
                                    //this.dbContext.SaveChanges();
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(t, identityService.Username, USER_AGENT);
                                    var afterCreateDetail = this.dbContext.GarmentDispositionPurchaseDetailss.Update(t);
                                    //this.dbContext.SaveChanges();
                                }
                                
                            }
                            this.dbContext.SaveChanges();
                        }
                        else//updatet data if items Exist
                        {
                            EntityExtension.FlagForUpdate(s, identityService.Username, USER_AGENT);
                            var afterCreateItem = this.dbContext.GarmentDispositionPurchaseItems.Update(s);
                            this.dbContext.SaveChanges();
                            //s.GarmentDispositionPurchaseDetails.ForEach(t =>
                            foreach (var t in s.GarmentDispositionPurchaseDetails)
                            {
                                if (t.QTYRemains <= 0)
                                {
                                    var EPOItems2 = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.Id == t.EPO_POId).FirstOrDefault();
                                    EPOItems2.IsDispositionCreatedAll = true;
                                    EntityExtension.FlagForUpdate(EPOItems2, identityService.Username, USER_AGENT);
                                    this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems2);
                                    dbContext.SaveChanges();

                                }
                                else
                                {
                                    var EPOItems3 = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.Id == t.EPO_POId).FirstOrDefault();
                                    EPOItems3.IsDispositionCreatedAll = false;
                                    EntityExtension.FlagForUpdate(EPOItems3, identityService.Username, USER_AGENT);
                                    this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems3);
                                    dbContext.SaveChanges();

                                }
                                if (t.Id == 0)
                                {
                                    EntityExtension.FlagForCreate(t, identityService.Username, USER_AGENT);
                                    t.GarmentDispositionPurchaseItemId = afterCreateItem.Entity.Id;
                                    var afterCreateDetail = this.dbContext.GarmentDispositionPurchaseDetailss.Add(t);
                                    this.dbContext.SaveChanges();
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(t, identityService.Username, USER_AGENT);
                                    var afterCreateDetail = this.dbContext.GarmentDispositionPurchaseDetailss.Update(t);
                                    this.dbContext.SaveChanges();
                                }
                                //});
                            }
                        }

                        //deleted detail when not exist anymore
                        var detailsPerItems = dataExist.GarmentDispositionPurchaseItems.SelectMany(j => j.GarmentDispositionPurchaseDetails).Where(j => j.GarmentDispositionPurchaseItemId == s.Id);
                        var detailsFormPerItem = s.GarmentDispositionPurchaseDetails.Select(j => j.Id).ToList();
                        var deletedDetails = detailsPerItems.Where(j => !detailsFormPerItem.Contains(j.Id)).ToList();

                        //deletedDetails.ForEach(j =>
                        foreach (var j in deletedDetails)
                        {
                            var EPOItems = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.Id == j.EPO_POId).FirstOrDefault();
                            EPOItems.IsDispositionCreatedAll = false;
                            EntityExtension.FlagForUpdate(EPOItems, identityService.Username, USER_AGENT);
                            this.dbContext.GarmentExternalPurchaseOrderItems.Update(EPOItems);
                            dbContext.SaveChanges();

                            EntityExtension.FlagForDelete(j, identityService.Username, USER_AGENT);
                            this.dbContext.GarmentDispositionPurchaseDetailss.Update(j);
                            dbContext.SaveChanges();

                            //});
                        }
                        //});
                    }
                    //deleted items 
                    var dataformItems = dataModel.GarmentDispositionPurchaseItems.Select(t => t.Id).ToList();
                    var deletedItems = dataExist.GarmentDispositionPurchaseItems.Where(s => !dataformItems.Contains(s.Id)).ToList();
                    //deletedItems.ForEach(t =>
                    foreach (var t in deletedItems)
                    {
                        var EPOItems = this.dbContext.GarmentDispositionPurchaseItems.AsNoTracking().Where(a => a.Id == t.Id).FirstOrDefault();
                        EntityExtension.FlagForDelete(EPOItems, identityService.Username, USER_AGENT);
                        var afterDeletedItems = this.dbContext.GarmentDispositionPurchaseItems.Update(EPOItems);

                        var EPO = this.dbContext.GarmentExternalPurchaseOrderItems.AsNoTracking().Where(a => a.GarmentEPOId == t.EPOId).ToList();
                        foreach (var item in EPO)
                        {
                            item.IsDispositionCreatedAll = false;
                            EntityExtension.FlagForUpdate(item, identityService.Username, USER_AGENT);
                            this.dbContext.GarmentExternalPurchaseOrderItems.Update(item);
                        }
                        
                        dbContext.SaveChanges();


                        //});
                    }
                    var modelParentUpdate = this.dbContext.GarmentDispositionPurchases.FirstOrDefault(t => t.Id == dataModel.Id);
                    modelParentUpdate.Amount = dataModel.Amount;
                    modelParentUpdate.Bank = dataModel.Bank;
                    modelParentUpdate.Category = dataModel.Category;
                    modelParentUpdate.ConfirmationOrderNo = dataModel.ConfirmationOrderNo;
                    modelParentUpdate.CurrencyDate = dataModel.CurrencyDate;
                    modelParentUpdate.CurrencyId = dataModel.CurrencyId;
                    modelParentUpdate.CurrencyName = dataModel.CurrencyName;
                    modelParentUpdate.Description = dataModel.Description;
                    modelParentUpdate.DispositionNo = dataModel.DispositionNo;
                    modelParentUpdate.Dpp = dataModel.Dpp;
                    modelParentUpdate.DueDate = dataModel.DueDate;
                    modelParentUpdate.IncomeTax = dataModel.IncomeTax;
                    modelParentUpdate.InvoiceProformaNo = dataModel.InvoiceProformaNo;
                    modelParentUpdate.OtherCost = dataModel.OtherCost;
                    modelParentUpdate.PaymentType = dataModel.PaymentType;
                    modelParentUpdate.SupplierCode = dataModel.SupplierCode;
                    modelParentUpdate.SupplierId = dataModel.SupplierId;
                    modelParentUpdate.SupplierIsImport = dataModel.SupplierIsImport;
                    modelParentUpdate.SupplierName = dataModel.SupplierName;
                    modelParentUpdate.VAT = dataModel.VAT;

                    var afterUpdateModel = dbContext.GarmentDispositionPurchases.Update(modelParentUpdate);
                    Updated = dbContext.SaveChanges();

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

        public GarmentExternalPurchaseOrderViewModel ReadByEPOWithDisposition(int EPOid, int supplierId, string currencyCode)
        {
            //var EPObyId = this.dbContext.GarmentExternalPurchaseOrders.Where(p => p.Id == EPOid && p.SupplierId == supplierId && p.CurrencyId == currencyId)
            var EPObyId = this.dbContext.GarmentExternalPurchaseOrders.Where(p => p.Id == EPOid)
                .Include(p => p.Items)
                .FirstOrDefault();
            EPObyId.Items = EPObyId.Items.Where(s => s.IsDispositionCreatedAll == false).ToList();
            //var POIds = EPObyId.SelectMany(s=> s.Items).Select(s => (long)s.POId).ToList();
            var POIds = EPObyId.Items.Select(s => (long)s.POId).ToList();


            var IPOByEPO = this.dbContext.GarmentInternalPurchaseOrders.Where(s => POIds.Contains(s.Id)).ToList();
            //var IPOUnits = IPOByEPO.Select(s => new { s.UnitId, s.UnitCode, s.UnitName });

            GarmentExternalPurchaseOrderViewModel viewModel = mapper.Map<GarmentExternalPurchaseOrderViewModel>(EPObyId);

            //get disposition
            var searchDisposition = this.dbContext.GarmentDispositionPurchases
                .Include(s => s.GarmentDispositionPurchaseItems)
                .ThenInclude(s=> s.GarmentDispositionPurchaseDetails)
                .Where(s => s.GarmentDispositionPurchaseItems.Any(t => t.EPOId == EPOid && t.CurrencyCode == currencyCode)
                && s.SupplierId == supplierId
                //&& s.CurrencyId == currencyId
                ).ToList();

            
            var dispositionPaid = searchDisposition.SelectMany(s=> s.GarmentDispositionPurchaseItems).Where(s => s.IsDispositionPaid).Select(s=> new {Item = s, TotalPaidPrice = s.GarmentDispositionPurchaseDetails.Sum(t=> t.PaidPrice) }).ToList();
            var dispositionCreated = searchDisposition.SelectMany(s=> s.GarmentDispositionPurchaseItems).Where(s => s.IsDispositionCreated).Select(s => new { Item = s, TotalPaidPrice = s.GarmentDispositionPurchaseDetails.Sum(t => t.PaidPrice) }).ToList();
            //viewModel.ForEach(Model => { 

            viewModel.DispositionAmountCreated = dispositionCreated != null ? dispositionCreated.Sum(s => (s.Item.IncomeTaxAmount + s.Item.VATAmount + s.TotalPaidPrice)):0;
            
            viewModel.DispositionAmountPaid = dispositionPaid != null ? dispositionPaid.Sum(s => (s.Item.IncomeTaxAmount + s.Item.VATAmount + s.TotalPaidPrice)):0;
            viewModel.DispositionQuantityCreated = dispositionCreated!= null ? dispositionCreated
                .SelectMany(t => t.Item.GarmentDispositionPurchaseDetails).Sum(t => t.QTYPaid):0;
            viewModel.DispositionQuantityPaid = dispositionPaid!= null ? dispositionPaid
                .SelectMany(t => t.Item.GarmentDispositionPurchaseDetails).Sum(t => t.QTYPaid):0;
            //foreach Unit

            viewModel.Items.ForEach(t =>
            {
                var getIPO = IPOByEPO.Where(s => s.Id == t.POId).FirstOrDefault();
                t.UnitId = getIPO.UnitId;
                t.UnitName = getIPO.UnitName;
                t.UnitCode = getIPO.UnitCode;

                var searchDispositionIPO = this.dbContext.GarmentDispositionPurchases
                .Include(s => s.GarmentDispositionPurchaseItems)
                .ThenInclude(s => s.GarmentDispositionPurchaseDetails)
                .Where(s => s.GarmentDispositionPurchaseItems.Any(j => j.GarmentDispositionPurchaseDetails.Any(d => d.IPOId == t.POId) && j.CurrencyCode == currencyCode)
                && s.SupplierId == supplierId
                //&& s.CurrencyId == currencyId
                );

                var dispositionPaidIPO = searchDispositionIPO.SelectMany(d => d.GarmentDispositionPurchaseItems).Where(d => d.IsDispositionPaid).Select(s => new { Item = s, TotalPaidPrice = s.GarmentDispositionPurchaseDetails.Where(a=> a.IPOId == a.IPOId).Sum(a => a.PaidPrice) }).ToList();
                var dispositionCreatedIPO = searchDispositionIPO.SelectMany(d => d.GarmentDispositionPurchaseItems).Where(d => d.IsDispositionCreated).Select(s => new { Item = s, TotalPaidPrice = s.GarmentDispositionPurchaseDetails.Where(a => a.IPOId == a.IPOId).Sum(a => a.PaidPrice) }).ToList();

                t.DispositionAmountCreated = dispositionCreatedIPO!= null ? dispositionCreatedIPO.Sum(d => (d.Item.IncomeTaxAmount + d.Item.VATAmount + d.TotalPaidPrice)):0;
                t.DispositionAmountPaid = dispositionPaidIPO != null ? dispositionPaidIPO.Sum(d => (d.Item.IncomeTaxAmount + d.Item.VATAmount + d.TotalPaidPrice)):0;
                t.DispositionQuantityCreated = dispositionCreatedIPO != null ? dispositionCreatedIPO
                    .SelectMany(j => j.Item.GarmentDispositionPurchaseDetails).Where(d => d.IPOId == t.POId).Sum(j => j.QTYPaid):0;
                t.DispositionQuantityPaid = dispositionPaidIPO != null ? dispositionPaidIPO
                    .SelectMany(j => j.Item.GarmentDispositionPurchaseDetails).Where(d => d.IPOId == t.POId).Sum(j => j.QTYPaid):0;

            });
            //});

            return viewModel;
        }

        public Task<int> SetIsPaidTrue(string dispositionNo, string user)
        {
            var model = dbContext.GarmentDispositionPurchases.FirstOrDefault(entity => entity.DispositionNo == dispositionNo);
            model.IsPaymentPaid = true;
            EntityExtension.FlagForUpdate(model, user, "Facade");
            dbContext.GarmentDispositionPurchases.Update(model);
            return dbContext.SaveChangesAsync();

        }

        public List<GarmentDispositionPurchase> GetGarmentDispositionPurchase()
        {
            var model = dbContext.GarmentDispositionPurchases.ToList();

            return model;
        }
    }
}
