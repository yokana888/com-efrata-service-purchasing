using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports.GarmentReportCMTFacade;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.PurchasingDispositionFacades
{
    public class PurchasingDispositionFacade : IPurchasingDispositionFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<PurchasingDisposition> dbSet;

        public PurchasingDispositionFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<PurchasingDisposition>();
        }

        public Tuple<List<PurchasingDisposition>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PurchasingDisposition> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo","SupplierName","CurrencyCode","DivisionName","CategoryName"
            };

            Query = QueryHelper<PurchasingDisposition>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchasingDisposition>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<PurchasingDisposition>.ConfigureOrder(Query, OrderDictionary);
            Query = Query
                .Select(s => new PurchasingDisposition
                {
                    DispositionNo = s.DispositionNo,
                    Id = s.Id,
                    SupplierCode = s.SupplierCode,
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    Bank = s.Bank,
                    CurrencyCode = s.CurrencyCode,
                    CurrencyId = s.CurrencyId,
                    CurrencyRate = s.CurrencyRate,
                    ConfirmationOrderNo = s.ConfirmationOrderNo,
                    //InvoiceNo = s.InvoiceNo,
                    PaymentMethod = s.PaymentMethod,
                    PaymentDueDate = s.PaymentDueDate,
                    CreatedBy = s.CreatedBy,
                    LastModifiedUtc = s.LastModifiedUtc,
                    CreatedUtc = s.CreatedUtc,
                    Amount = s.Amount,
                    Calculation = s.Calculation,
                    //Investation = s.Investation,
                    Position = s.Position,
                    ProformaNo = s.ProformaNo,
                    Remark = s.Remark,
                    UId = s.UId,
                    CategoryCode = s.CategoryCode,
                    CategoryId = s.CategoryId,
                    CategoryName = s.CategoryName,
                    DPP = s.DPP,
                    IncomeTaxValue = s.IncomeTaxValue,
                    VatValue = s.VatValue,
                    IncomeTaxBy = s.IncomeTaxBy,
                    DivisionCode = s.DivisionCode,
                    DivisionId = s.DivisionId,
                    DivisionName = s.DivisionName,
                    PaymentCorrection = s.PaymentCorrection,
                    Items = s.Items.Select(x => new PurchasingDispositionItem()
                    {
                        EPOId = x.EPOId,
                        EPONo = x.EPONo,
                        Id = x.Id,
                        IncomeTaxId = x.IncomeTaxId,
                        IncomeTaxName = x.IncomeTaxName,
                        IncomeTaxRate = x.IncomeTaxRate,
                        UseVat = x.UseVat,
                        UseIncomeTax = x.UseIncomeTax,
                        UId = x.UId,

                        Details = x.Details.Select(y => new PurchasingDispositionDetail()
                        {

                            UId = y.UId,

                            DealQuantity = y.DealQuantity,
                            DealUomId = y.DealUomId,
                            DealUomUnit = y.DealUomUnit,
                            Id = y.Id,
                            PaidPrice = y.PaidPrice,
                            PaidQuantity = y.PaidQuantity,
                            PricePerDealUnit = y.PricePerDealUnit,
                            PriceTotal = y.PriceTotal,
                            PRId = y.PRId,
                            PRNo = y.PRNo,
                            ProductCode = y.ProductCode,
                            ProductId = y.ProductId,
                            ProductName = y.ProductName,
                            PurchasingDispositionItem = y.PurchasingDispositionItem,
                            PurchasingDispositionItemId = y.PurchasingDispositionItemId,
                            UnitCode = y.UnitCode,
                            UnitId = y.UnitId,
                            UnitName = y.UnitName
                        }).ToList()
                    }).ToList()
                });
            Pageable<PurchasingDisposition> pageable = new Pageable<PurchasingDisposition>(Query, Page - 1, Size);
            List<PurchasingDisposition> Data = pageable.Data.ToList<PurchasingDisposition>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<PurchasingDisposition>, int, Dictionary<string, string>> ReadOptimized(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PurchasingDisposition> Query = this.dbSet.AsNoTracking().Select(s => new PurchasingDisposition
            {
                DispositionNo = s.DispositionNo,
                Id = s.Id,
                SupplierCode = s.SupplierCode,
                SupplierId = s.SupplierId,
                SupplierName = s.SupplierName,
                Bank = s.Bank,
                CurrencyCode = s.CurrencyCode,
                CurrencyId = s.CurrencyId,
                CurrencyRate = s.CurrencyRate,
                ConfirmationOrderNo = s.ConfirmationOrderNo,
                //InvoiceNo = s.InvoiceNo,
                PaymentMethod = s.PaymentMethod,
                PaymentDueDate = s.PaymentDueDate,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                CreatedUtc = s.CreatedUtc,
                Amount = s.Amount,
                Calculation = s.Calculation,
                //Investation = s.Investation,
                Position = s.Position,
                ProformaNo = s.ProformaNo,
                Remark = s.Remark,
                UId = s.UId,
                CategoryCode = s.CategoryCode,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                DPP = s.DPP,
                IncomeTaxValue = s.IncomeTaxValue,
                VatValue = s.VatValue,
                IncomeTaxBy = s.IncomeTaxBy,
                DivisionCode = s.DivisionCode,
                DivisionId = s.DivisionId,
                DivisionName = s.DivisionName,
                PaymentCorrection = s.PaymentCorrection,
                Items = s.Items.Select(x => new PurchasingDispositionItem()
                {
                    EPOId = x.EPOId,
                    EPONo = x.EPONo,
                    Id = x.Id,
                    IncomeTaxId = x.IncomeTaxId,
                    IncomeTaxName = x.IncomeTaxName,
                    IncomeTaxRate = x.IncomeTaxRate,
                    UseVat = x.UseVat,
                    UseIncomeTax = x.UseIncomeTax,
                    UId = x.UId,
                    Details = x.Details.Select(y => new PurchasingDispositionDetail()
                    {

                        UId = y.UId,
                        DealQuantity = y.DealQuantity,
                        DealUomId = y.DealUomId,
                        DealUomUnit = y.DealUomUnit,
                        Id = y.Id,
                        PaidPrice = y.PaidPrice,
                        PaidQuantity = y.PaidQuantity,
                        PricePerDealUnit = y.PricePerDealUnit,
                        PriceTotal = y.PriceTotal,
                        PRId = y.PRId,
                        PRNo = y.PRNo,
                        ProductCode = y.ProductCode,
                        ProductId = y.ProductId,
                        ProductName = y.ProductName,
                        PurchasingDispositionItem = y.PurchasingDispositionItem,
                        PurchasingDispositionItemId = y.PurchasingDispositionItemId,
                        UnitCode = y.UnitCode,
                        UnitId = y.UnitId,
                        UnitName = y.UnitName
                    }).ToList()
                }).ToList()
            });

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo","SupplierName","CurrencyCode","DivisionName","CategoryName"
            };

            Query = QueryHelper<PurchasingDisposition>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchasingDisposition>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<PurchasingDisposition>.ConfigureOrder(Query, OrderDictionary);

            Pageable<PurchasingDisposition> pageable = new Pageable<PurchasingDisposition>(Query, Page - 1, Size);
            List<PurchasingDisposition> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public PurchasingDisposition ReadModelById(int id)
        {
            var a = this.dbSet.Where(d => d.Id.Equals(id) && d.IsDeleted.Equals(false))
                .Include(p => p.Items)
                .ThenInclude(p => p.Details)
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(PurchasingDisposition m, string user, int clientTimeZoneOffset)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, "Facade");
                    m.DispositionNo = await GenerateNo(m, clientTimeZoneOffset);
                    m.Position = 1;
                    if (m.IncomeTaxBy == "Supplier")
                    {
                        m.Amount = (m.DPP + m.VatValue - m.IncomeTaxValue) + m.PaymentCorrection;
                    }
                    else
                    {
                        m.Amount = m.DPP + m.VatValue + m.PaymentCorrection;
                    }
                    
                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id.ToString() == detail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                            epoDetail.DispositionQuantity += detail.PaidQuantity;
                            EntityExtension.FlagForCreate(detail, user, "Facade");
                        }
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

        async Task<string> GenerateNo(PurchasingDisposition model, int clientTimeZoneOffset)
        {
            DateTimeOffset Now = DateTime.UtcNow;
            string Year = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");

            string no = model.DivisionName == "EFRATA" ? $"{Year}-{Month}-G" : $"{Year}-{Month}-T";
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

        public int Delete(int id, string user)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .SingleOrDefault(epo => epo.Id == id && !epo.IsDeleted);

                    EntityExtension.FlagForDelete(m, user, "Facade");

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForDelete(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id.ToString() == detail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                            epoDetail.DispositionQuantity -= detail.PaidQuantity;
                            EntityExtension.FlagForDelete(detail, user, "Facade");
                        }
                    }

                    Deleted = dbContext.SaveChanges();
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

        public async Task<int> Update(int id, PurchasingDisposition purchasingDisposition, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingModel = this.dbSet.AsNoTracking()
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(epo => epo.Id == id && !epo.IsDeleted);

                    foreach (var oldIem in existingModel.Items)
                    {
                        foreach (var oldDetail in oldIem.Details)
                        {
                            ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id.ToString() == oldDetail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                            epoDetail.DispositionQuantity -= oldDetail.PaidQuantity;
                        }
                    }

                    if (existingModel != null && id == purchasingDisposition.Id)
                    {
                        if (purchasingDisposition.IncomeTaxBy == "Supplier")
                        {
                            purchasingDisposition.Amount = purchasingDisposition.DPP + purchasingDisposition.VatValue + purchasingDisposition.PaymentCorrection;
                        }
                        else
                        {
                            purchasingDisposition.Amount = purchasingDisposition.DPP + purchasingDisposition.VatValue - purchasingDisposition.IncomeTaxValue + purchasingDisposition.PaymentCorrection;
                        }
                        
                        EntityExtension.FlagForUpdate(purchasingDisposition, user, "Facade");

                        foreach (var item in purchasingDisposition.Items.ToList())
                        {
                            var existingItem = existingModel.Items.SingleOrDefault(m => m.Id == item.Id);
                            List<PurchasingDispositionItem> duplicateDispositionItems = purchasingDisposition.Items.Where(i => i.EPOId == item.EPOId && i.Id != item.Id).ToList();

                            if (item.Id == 0)
                            {
                                if (duplicateDispositionItems.Count <= 0)
                                {

                                    EntityExtension.FlagForCreate(item, user, "Facade");

                                    foreach (var detail in item.Details)
                                    {
                                        ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id.ToString() == detail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                                        epoDetail.DispositionQuantity += detail.PaidQuantity;
                                        EntityExtension.FlagForCreate(detail, user, "Facade");
                                    }

                                }
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(item, user, "Facade");

                                if (duplicateDispositionItems.Count > 0)
                                {
                                    //foreach (var detail in item.Details.ToList())
                                    //{
                                    //    if (detail.Id != 0)
                                    //    {

                                    //        EntityExtension.FlagForUpdate(detail, user, "Facade");

                                    //        foreach (var duplicateItem in duplicateDispositionItems.ToList())
                                    //        {
                                    //            foreach (var duplicateDetail in duplicateItem.Details.ToList())
                                    //            {
                                    //                if (item.Details.Count(d => d.EPODetailId.Equals(duplicateDetail.EPODetailId)) < 1)
                                    //                {
                                    //                    ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id == detail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                                    //                    epoDetail.DispositionQuantity += detail.PaidQuantity;
                                    //                    EntityExtension.FlagForCreate(duplicateDetail, user, "Facade");
                                    //                    item.Details.Add(duplicateDetail);

                                    //                }
                                    //            }
                                    //            purchasingDisposition.Items.Remove(duplicateItem);
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    foreach (var detail in item.Details)
                                    {
                                        if (detail.Id != 0)
                                        {
                                            ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.Where(s => s.Id.ToString() == detail.EPODetailId && s.IsDeleted == false).FirstOrDefault();
                                            epoDetail.DispositionQuantity += detail.PaidQuantity;
                                            EntityExtension.FlagForUpdate(detail, user, "Facade");
                                        }
                                    }
                                }
                            }
                        }

                        this.dbContext.Update(purchasingDisposition);

                        foreach (var existingItem in existingModel.Items)
                        {
                            var newItem = purchasingDisposition.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                            if (newItem == null)
                            {
                                EntityExtension.FlagForDelete(existingItem, user, "Facade");

                                this.dbContext.PurchasingDispositionItems.Update(existingItem);
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    EntityExtension.FlagForDelete(existingDetail, user, "Facade");

                                    this.dbContext.PurchasingDispositionDetails.Update(existingDetail);
                                }
                            }
                            else
                            {
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    var newDetail = newItem.Details.FirstOrDefault(d => d.Id == existingDetail.Id);
                                    if (newDetail == null)
                                    {
                                        EntityExtension.FlagForDelete(existingDetail, user, "Facade");

                                        this.dbContext.PurchasingDispositionDetails.Update(existingDetail);

                                    }
                                }
                            }
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        transaction.Commit();

                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Updated;
        }

        public List<PurchasingDisposition> ReadDisposition(string Keyword = null, string Filter = "{}", string epoId = "")
        {
            IQueryable<PurchasingDisposition> Query = this.dbSet.Include(x => x.Items).ThenInclude(x => x.Details);

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo","SupplierName","Items.EPONo","CurrencyCode"
            };

            Query = QueryHelper<PurchasingDisposition>.ConfigureSearch(Query, searchAttributes, Keyword);
            Query = Query.Where(x => x.IsDeleted == false && x.Items.Count() > 0
                && x.Items.Any(y => y.Details.Count() > 0 && y.EPOId == epoId && y.Details.Any(z => z.IsDeleted == false)));
            //Query = Query
            //    .Where(m => m.IsDeleted == false)
            //    .Select(s => new PurchasingDisposition
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
            //        PaymentCorrection=s.PaymentCorrection,
            //        Items = s.Items
            //            .Select(i => new PurchasingDispositionItem
            //            {
            //                Id = i.Id,
            //                EPOId = i.EPOId,
            //                Details = i.Details
            //                    .Where(d => d.IsDeleted == false)
            //                    .ToList()
            //            })
            //            .Where(i => i.Details.Count > 0 && i.EPOId == epoId)
            //            .ToList()
            //    })
            //    .Where(m => m.Items.Count > 0);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchasingDisposition>.ConfigureFilter(Query, FilterDictionary);

            return Query.ToList();
        }

        public IQueryable<PurchasingDisposition> ReadByDisposition(string Keyword = null, string Filter = "{}")

        {
            IQueryable<PurchasingDisposition> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "DispositionNo"
            };

            Query = QueryHelper<PurchasingDisposition>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchasingDisposition>.ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>("{}");

            Query = QueryHelper<PurchasingDisposition>.ConfigureOrder(Query, OrderDictionary).Include(m => m.Items)
                .ThenInclude(i => i.Details).Where(s => s.IsDeleted == false && (s.Position == 1 || s.Position == 6));

            return Query;
        }

        public async Task<int> UpdatePosition(PurchasingDispositionUpdatePositionPostedViewModel data, string user)
        {
            int updated = 0;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                foreach (var dispositionNo in data.PurchasingDispositionNoes)
                {
                    PurchasingDisposition purchasingDisposition = dbSet.FirstOrDefault(x => x.DispositionNo == dispositionNo);

                    purchasingDisposition.Position = (int)data.Position;
                    EntityExtension.FlagForUpdate(purchasingDisposition, user, "Facade");

                }
                updated = await dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            return updated;
        }

        public List<PurchasingDispositionViewModel> GetTotalPaidPrice(List<PurchasingDispositionViewModel> data)
        {
            var dbSet = dbContext.PurchasingDispositionDetails.Select(entity => new { entity.ProductId, entity.PRId, entity.PaidPrice }).AsNoTracking().ToList();

            foreach (var purchasingDisposition in data)
            {
                foreach (var item in purchasingDisposition.Items)
                {
                    foreach (var detail in item.Details)
                    {
                        detail.TotalPaidPrice = dbSet.Where(x => x.ProductId == detail.Product._id && x.PRId == detail.PRId).Sum(x => x.PaidPrice);
                    }
                }
            }
            return data;
        }

        public Task<int> SetIsPaidTrue(string dispositionNo, string user)
        {
            var model = dbContext.PurchasingDispositions.FirstOrDefault(entity => entity.DispositionNo == dispositionNo);
            model.IsPaid = true;
            EntityExtension.FlagForUpdate(model, user, "Facade");
            dbContext.PurchasingDispositions.Update(model);
            return dbContext.SaveChangesAsync();
        }

        public DispositionMemoLoaderDto GetDispositionMemoLoader(int dispositionId)
        {
            var disposition = dbContext.PurchasingDispositions.FirstOrDefault(entity => entity.Id == dispositionId);

            var result = (DispositionMemoLoaderDto)null;
            if (disposition != null)
            {
                var dispositionItems = dbContext.PurchasingDispositionItems.Where(entity => entity.PurchasingDispositionId == dispositionId).ToList();
                var dispositionItemIds = dispositionItems.Select(element => element.Id).ToList();
                var dispositionDetails = dbContext.PurchasingDispositionDetails.Where(entity => dispositionItemIds.Contains(entity.PurchasingDispositionItemId)).ToList();

                foreach (var dispositionItem in dispositionItems)
                {
                    var upoItemIds = dbContext.UnitPaymentOrderDetails.Where(entity => entity.EPONo == dispositionItem.EPONo).Select(entity => entity.UPOItemId).ToList();
                    var upoIds = dbContext.UnitPaymentOrderItems.Where(entity => upoItemIds.Contains(entity.Id)).Select(entity => entity.UPOId).ToList();
                    var unitPaymentOrder = dbContext.UnitPaymentOrders.FirstOrDefault(entity => upoIds.Contains(entity.Id));

                    var upoDto = new UnitPaymentOrderDto(0, null, DateTimeOffset.Now);
                    var urnDto = new List<UnitReceiptNoteDto>();
                    if (unitPaymentOrder != null)
                    {
                        upoDto = new UnitPaymentOrderDto((int)unitPaymentOrder.Id, unitPaymentOrder.UPONo, unitPaymentOrder.Date);
                        var urnIds = dbContext.UnitPaymentOrderItems.Where(entity => entity.UPOId == unitPaymentOrder.Id).Select(entity => entity.URNId).ToList();
                        var unitReceiptNotes = dbContext.UnitReceiptNotes.Where(entity => urnIds.Contains(entity.Id)).ToList();
                        urnDto = unitReceiptNotes.Select(element => new UnitReceiptNoteDto((int)element.Id, element.URNNo, element.ReceiptDate)).ToList();

                    }

                    var dpp = dispositionDetails.Sum(element => element.PaidPrice);
                    var incomeTaxAmount = (double)0;
                    var vatAmount = (double)0;

                    if (dispositionItem.UseVat)
                        vatAmount = dpp * 0.1;

                    if (dispositionItem.UseIncomeTax)
                        incomeTaxAmount = dpp * dispositionItem.IncomeTaxRate / 100;

                    var productName = string.Join('\n', dispositionDetails.Where(element => element.PurchasingDispositionItemId == dispositionItem.Id).Select(element => $"{element.ProductCode} - {element.ProductName}"));

                    var purchaseAmountCurrency = dpp;
                    if (disposition.CurrencyCode == "IDR")
                        purchaseAmountCurrency = 0;
                    var purchaseAmount = (dpp + vatAmount - incomeTaxAmount) * disposition.CurrencyRate;
                    result = new DispositionMemoLoaderDto(upoDto, urnDto, purchaseAmount, purchaseAmountCurrency, productName);
                }

                return result;
            }
            else
            {
                return result;
            }
        }

        public ReadResponse<UnitPaymentOrderMemoLoaderDto> GetUnitPaymentOrderMemoLoader(string keyword, int divisionId, bool supplierIsImport, string currencyCode)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var bankExpenditureIds = dbContext.BankExpenditureNotes.Where(entity => entity.SupplierImport == supplierIsImport).Select(entity => entity.Id).ToList();
                var existingUpoNos = dbContext.BankExpenditureNoteDetails.Where(entity => bankExpenditureIds.Contains(entity.BankExpenditureNoteId) && entity.UnitPaymentOrderNo.Contains(keyword)).Select(entity => entity.UnitPaymentOrderNo).ToList();

                var query = dbContext.UnitPaymentOrders.AsQueryable();

                if (divisionId > 0)
                {
                    query = query.Where(entity => entity.DivisionId == divisionId.ToString());
                }

                if (!string.IsNullOrWhiteSpace(currencyCode))
                {
                    query = query.Where(entity => entity.CurrencyCode == currencyCode);
                }

                var unitPaymentOrders = query.Where(entity => existingUpoNos.Contains(entity.UPONo)).Take(10).ToList();
                var upoNos = unitPaymentOrders.Select(element => element.UPONo).ToList();
                var upoIds = unitPaymentOrders.Select(element => element.Id).ToList();
                var expenditureDetails = dbContext.BankExpenditureNoteDetails.Where(entity => upoNos.Contains(entity.UnitPaymentOrderNo)).ToList();
                var unitReceiptNoteIds = dbContext.UnitPaymentOrderItems.Where(entity => upoIds.Contains(entity.UPOId)).Select(entity => entity.URNId).ToList();
                var unitReceiptNotes = dbContext.UnitReceiptNotes.Where(entity => unitReceiptNoteIds.Contains(entity.Id)).ToList();
                var unitReceiptNoteItems = dbContext.UnitReceiptNoteItems.Where(entity => unitReceiptNoteIds.Contains(entity.URNId)).ToList();

                var data = new List<UnitPaymentOrderMemoLoaderDto>();

                foreach (var unitPaymentOrder in unitPaymentOrders)
                {
                    var expenditureIds = expenditureDetails.Where(element => element.UnitPaymentOrderNo == unitPaymentOrder.UPONo).Select(element => element.BankExpenditureNoteId).ToList();
                    var expenditureDetail = expenditureDetails.Where(element => element.UnitPaymentOrderNo == unitPaymentOrder.UPONo).FirstOrDefault();
                    var expenditure = dbContext.BankExpenditureNotes.FirstOrDefault(entity => expenditureIds.Contains(entity.Id));
                    var urnIds = dbContext.UnitPaymentOrderItems.Where(entity => entity.UPOId == unitPaymentOrder.Id).Select(entity => entity.URNId).ToList();
                    var upoUnitReceiptNotes = unitReceiptNotes.Where(entity => urnIds.Contains(entity.Id)).Select(element => new UnitReceiptNoteDto((int)element.Id, element.URNNo, element.ReceiptDate)).ToList();

                    var purchaseAmountCurrency = expenditure.GrandTotal;

                    var dpp = expenditureDetail.TotalPaid;
                    var incomeTaxAmount = (double)0;
                    var vatAmount = (double)0;

                    if (unitPaymentOrder.UseVat)
                        vatAmount = dpp * 0.1;

                    if (unitPaymentOrder.UseIncomeTax)
                        incomeTaxAmount = dpp * unitPaymentOrder.IncomeTaxRate / 100;

                    if (expenditure.CurrencyCode == "IDR")
                        purchaseAmountCurrency = 0;

                    var purchaseAmount = (dpp + vatAmount - incomeTaxAmount) * expenditure.CurrencyRate;
                    var productName = string.Join('\n', unitReceiptNoteItems.Where(element => urnIds.Contains(element.URNId)).Select(element => $"{element.ProductCode} - {element.ProductName}"));
                    if (expenditure != null)
                    {
                        data.Add(new UnitPaymentOrderMemoLoaderDto(new ExpenditureDto((int)expenditure.Id, expenditure.DocumentNo, expenditure.Date), new SupplierDto(expenditure.SupplierId, expenditure.SupplierCode, expenditure.SupplierName), productName, new UnitPaymentOrderDto((int)unitPaymentOrder.Id, unitPaymentOrder.UPONo, unitPaymentOrder.Date), upoUnitReceiptNotes, purchaseAmountCurrency, purchaseAmount, 0, 0));
                    }
                }

                return new ReadResponse<UnitPaymentOrderMemoLoaderDto>(data, 10, new Dictionary<string, string>());

            }
            else
            {
                return new ReadResponse<UnitPaymentOrderMemoLoaderDto>(new List<UnitPaymentOrderMemoLoaderDto>(), 0, new Dictionary<string, string>());
            }
        }
    }
}
