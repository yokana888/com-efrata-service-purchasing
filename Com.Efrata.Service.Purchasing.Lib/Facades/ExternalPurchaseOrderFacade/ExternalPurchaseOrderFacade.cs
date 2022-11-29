using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel.EPODispositionLoader;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade
{
    public class ExternalPurchaseOrderFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<ExternalPurchaseOrder> dbSet;

        public ExternalPurchaseOrderFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<ExternalPurchaseOrder>();
        }

        public Tuple<List<ExternalPurchaseOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<ExternalPurchaseOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "EPONo", "SupplierName", "DivisionName","UnitName","Items.PRNo", "POCashType", "PaymentMethod"
            };

            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query
                .Select(s => new ExternalPurchaseOrder
                {
                    Id = s.Id,
                    EPONo = s.EPONo,
                    CurrencyCode = s.CurrencyCode,
                    CurrencyRate = s.CurrencyRate,
                    CurrencyId = s.CurrencyId,
                    OrderDate = s.OrderDate,
                    DeliveryDate = s.DeliveryDate,
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    DivisionCode = s.DivisionCode,
                    DivisionName = s.DivisionName,
                    DivisionId = s.DivisionId,
                    POCashType = s.POCashType,
                    PaymentMethod = s.PaymentMethod,
                    LastModifiedUtc = s.LastModifiedUtc,
                    UnitName = s.UnitName,
                    UnitCode = s.UnitCode,
                    UnitId = s.UnitId,
                    CreatedBy = s.CreatedBy,
                    IsPosted = s.IsPosted,
                    UseVat = s.UseVat,
                    UseIncomeTax = s.UseIncomeTax,
                    IncomeTaxId = s.IncomeTaxId,
                    IncomeTaxName = s.IncomeTaxName,
                    IncomeTaxRate = s.IncomeTaxRate,
                    IncomeTaxBy = s.IncomeTaxBy,
                    IsCreateOnVBRequest = s.IsCreateOnVBRequest,
                    Items = s.Items.Select(
                        q => new ExternalPurchaseOrderItem
                        {
                            Id = q.Id,
                            POId = q.POId,
                            PRNo = q.PRNo,
                            Details = q.Details.Select(

                                r => new ExternalPurchaseOrderDetail
                                {
                                    Conversion = r.Conversion,
                                    POItemId = r.POItemId,
                                    PRItemId = r.PRItemId,
                                    PriceBeforeTax = r.PriceBeforeTax,
                                    PricePerDealUnit = r.PricePerDealUnit,
                                    ProductCode = r.ProductCode,
                                    ProductId = r.ProductId,
                                    ProductName = r.ProductName,
                                    ProductRemark = r.ProductRemark,
                                    ReceiptQuantity = r.ReceiptQuantity,
                                    DispositionQuantity = r.DispositionQuantity,
                                    DefaultUomId = r.DefaultUomId,
                                    DefaultUomUnit = r.DefaultUomUnit,
                                    DefaultQuantity = r.DefaultQuantity,
                                    DealQuantity = r.DealQuantity,
                                    DealUomId = r.DealUomId,
                                    DealUomUnit = r.DealUomUnit,
                                    IncludePpn = r.IncludePpn,

                                }
                            ).ToList()
                        }
                    )
                    .ToList()
                });



            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<ExternalPurchaseOrder> pageable = new Pageable<ExternalPurchaseOrder>(Query, Page - 1, Size);
            List<ExternalPurchaseOrder> Data = pageable.Data.ToList<ExternalPurchaseOrder>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public ExternalPurchaseOrder ReadModelById(int id)
        {
            var a = this.dbSet.Where(d => d.Id.Equals(id) && d.IsDeleted.Equals(false))
                .Include(p => p.Items)
                .ThenInclude(p => p.Details)
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(ExternalPurchaseOrder m, string user, int clientTimeZoneOffset)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, "Facade");

                    m.EPONo = await GenerateNo(m, clientTimeZoneOffset);

                    if (m.UseIncomeTax == false)
                    {
                        m.IncomeTaxBy = "";
                    }

                    foreach (var item in m.Items)
                    {

                        EntityExtension.FlagForCreate(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            //detail.PricePerDealUnit = detail.IncludePpn ? detail.PriceBeforeTax - (detail.PriceBeforeTax * (Convert.ToDouble(m.VatRate) / 100)) : detail.PriceBeforeTax;
                            detail.PricePerDealUnit = detail.IncludePpn ? (100 * detail.PriceBeforeTax) / (100 + Convert.ToDouble(m.VatRate)) : detail.PriceBeforeTax;
                            //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                            //purchaseRequestItem.Status = "Sudah diorder ke Supplier";
                            EntityExtension.FlagForCreate(detail, user, "Facade");

                            InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                            internalPurchaseOrderItem.Status = "Sudah dibuat PO Eksternal";
                        }
                        InternalPurchaseOrder internalPurchaseOrder = this.dbContext.InternalPurchaseOrders.FirstOrDefault(s => s.Id == item.POId);
                        internalPurchaseOrder.IsPosted = true;
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

        public async Task<int> Update(int id, ExternalPurchaseOrder externalPurchaseOrder, string user)
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

                    if (existingModel != null && id == externalPurchaseOrder.Id)
                    {
                        EntityExtension.FlagForUpdate(externalPurchaseOrder, user, "Facade");
                        if (externalPurchaseOrder.UseIncomeTax == false)
                        {
                            externalPurchaseOrder.IncomeTaxBy = "";
                        }
                        foreach (var item in externalPurchaseOrder.Items.ToList())
                        {
                            var existingItem = existingModel.Items.SingleOrDefault(m => m.Id == item.Id);
                            List<ExternalPurchaseOrderItem> duplicateExternalPurchaseOrderItems = externalPurchaseOrder.Items.Where(i => i.POId == item.POId && i.Id != item.Id).ToList();

                            if (item.Id == 0)
                            {
                                if (duplicateExternalPurchaseOrderItems.Count <= 0)
                                {

                                    EntityExtension.FlagForCreate(item, user, "Facade");

                                    foreach (var detail in item.Details)
                                    {
                                        detail.PricePerDealUnit = detail.IncludePpn ? (100 * detail.PriceBeforeTax) / 110 : detail.PriceBeforeTax;
                                        EntityExtension.FlagForCreate(detail, user, "Facade");
                                        //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                                        //purchaseRequestItem.Status = "Sudah diorder ke Supplier";

                                        InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                                        internalPurchaseOrderItem.Status = "Sudah dibuat PO Eksternal";

                                    }
                                    InternalPurchaseOrder internalPurchaseOrder = this.dbContext.InternalPurchaseOrders.FirstOrDefault(s => s.Id == item.POId);
                                    internalPurchaseOrder.IsPosted = true;
                                }
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(item, user, "Facade");

                                if (duplicateExternalPurchaseOrderItems.Count > 0)
                                {
                                    foreach (var detail in item.Details.ToList())
                                    {
                                        if (detail.Id != 0)
                                        {

                                            EntityExtension.FlagForUpdate(detail, user, "Facade");

                                            foreach (var duplicateItem in duplicateExternalPurchaseOrderItems.ToList())
                                            {
                                                foreach (var duplicateDetail in duplicateItem.Details.ToList())
                                                {
                                                    if (detail.ProductId.Equals(duplicateDetail.ProductId))
                                                    {
                                                        detail.PricePerDealUnit = detail.IncludePpn ? (100 * detail.PriceBeforeTax) / 110 : detail.PriceBeforeTax;
                                                    }
                                                    else if (item.Details.Count(d => d.ProductId.Equals(duplicateDetail.ProductId)) < 1)
                                                    {
                                                        EntityExtension.FlagForCreate(duplicateDetail, user, "Facade");
                                                        item.Details.Add(duplicateDetail);
                                                        duplicateDetail.PricePerDealUnit = duplicateDetail.IncludePpn ? (100 * duplicateDetail.PriceBeforeTax) / 110 : duplicateDetail.PriceBeforeTax;
                                                        //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                                                        //purchaseRequestItem.Status = "Sudah diorder ke Supplier";

                                                        InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                                                        internalPurchaseOrderItem.Status = "Sudah dibuat PO Eksternal";
                                                    }
                                                }
                                                externalPurchaseOrder.Items.Remove(duplicateItem);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var detail in item.Details)
                                    {
                                        if (detail.Id != 0)
                                        {
                                            EntityExtension.FlagForUpdate(detail, user, "Facade");
                                            detail.PricePerDealUnit = detail.IncludePpn ? (100 * detail.PriceBeforeTax) / 110 : detail.PriceBeforeTax;

                                        }
                                    }
                                }
                            }
                        }

                        this.dbContext.Update(externalPurchaseOrder);

                        foreach (var existingItem in existingModel.Items)
                        {
                            var newItem = externalPurchaseOrder.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                            if (newItem == null)
                            {
                                EntityExtension.FlagForDelete(existingItem, user, "Facade");
                                InternalPurchaseOrder internalPurchaseOrder = this.dbContext.InternalPurchaseOrders.FirstOrDefault(s => s.Id == existingItem.POId);
                                internalPurchaseOrder.IsPosted = false;
                                this.dbContext.ExternalPurchaseOrderItems.Update(existingItem);
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    EntityExtension.FlagForDelete(existingDetail, user, "Facade");
                                    //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == existingDetail.PRItemId);
                                    //purchaseRequestItem.Status = "Sudah diterima Pembelian";
                                    existingDetail.PricePerDealUnit = existingDetail.IncludePpn ? (100 * existingDetail.PriceBeforeTax) / 110 : existingDetail.PriceBeforeTax;

                                    InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == existingDetail.POItemId);
                                    internalPurchaseOrderItem.Status = "PO Internal belum diorder";
                                    this.dbContext.ExternalPurchaseOrderDetails.Update(existingDetail);
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
                                        //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == existingDetail.PRItemId);
                                        //purchaseRequestItem.Status = "Sudah diterima Pembelian";
                                        existingDetail.PricePerDealUnit = existingDetail.IncludePpn ? (100 * existingDetail.PriceBeforeTax) / 110 : existingDetail.PriceBeforeTax;

                                        InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == existingDetail.POItemId);
                                        internalPurchaseOrderItem.Status = "PO Internal belum diorder";
                                        this.dbContext.ExternalPurchaseOrderDetails.Update(existingDetail);

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
                            //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                            //purchaseRequestItem.Status = "Sudah diterima Pembelian";
                            EntityExtension.FlagForDelete(detail, user, "Facade");

                            InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                            internalPurchaseOrderItem.Status = "PO Internal belum diorder";
                        }
                        InternalPurchaseOrder internalPurchaseOrder = this.dbContext.InternalPurchaseOrders.FirstOrDefault(s => s.Id == item.POId);
                        internalPurchaseOrder.IsPosted = false;

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

        public int EPOPost(List<ExternalPurchaseOrder> ListEPO, string user)
        {
            int Updated = 0;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var Ids = ListEPO.Select(d => d.Id).ToList();
                    var listData = this.dbSet
                        .Where(m => Ids.Contains(m.Id) && !m.IsDeleted)
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .ToList();
                    listData.ForEach(m =>
                    {
                        EntityExtension.FlagForUpdate(m, user, "Facade");
                        m.IsPosted = true;

                        foreach (var item in m.Items)
                        {
                            EntityExtension.FlagForUpdate(item, user, "Facade");
                            foreach (var detail in item.Details)
                            {
                                EntityExtension.FlagForUpdate(detail, user, "Facade");
                                InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                                internalPurchaseOrderItem.Status = "Sudah diorder ke Supplier";

                                PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                                purchaseRequestItem.Status = "Sudah diorder ke Supplier";
                            }
                        }
                    });

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

        public int EPOCancel(int id, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .SingleOrDefault(epo => epo.Id == id && !epo.IsDeleted);

                    EntityExtension.FlagForUpdate(m, user, "Facade");
                    m.IsCanceled = true;

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForUpdate(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForUpdate(detail, user, "Facade");

                            InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                            internalPurchaseOrderItem.Status = "Dibatalkan";

                            //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                            //purchaseRequestItem.Status = "Dibatalkan";
                        }
                    }

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

        public int EPOClose(int id, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .SingleOrDefault(epo => epo.Id == id && !epo.IsDeleted);

                    EntityExtension.FlagForUpdate(m, user, "Facade");
                    m.IsClosed = true;

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForUpdate(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForUpdate(detail, user, "Facade");
                        }
                        InternalPurchaseOrder internalPurchaseOrder = this.dbContext.InternalPurchaseOrders.FirstOrDefault(s => s.Id == item.POId);
                        internalPurchaseOrder.IsClosed = true;
                    }

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

        public int EPOUnpost(int id, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .SingleOrDefault(epo => epo.Id == id && !epo.IsDeleted);

                    EntityExtension.FlagForUpdate(m, user, "Facade");
                    m.IsPosted = false;

                    foreach (var item in m.Items)
                    {
                        //var existPR = this.dbContext.ExternalPurchaseOrderItems.Include(a => a.Details).Where(a => a.EPOId != item.EPOId);
                        EntityExtension.FlagForUpdate(item, user, "Facade");
                        foreach (var detail in item.Details)
                        {
                            var existPR = (from a in this.dbContext.ExternalPurchaseOrderDetails
                                           join b in dbContext.ExternalPurchaseOrderItems on a.EPOItemId equals b.Id
                                           join c in dbContext.ExternalPurchaseOrders on b.EPOId equals c.Id
                                           where a.PRItemId == detail.PRItemId && a.IsDeleted == false && b.EPOId != item.EPOId && c.IsPosted == true
                                           select a).FirstOrDefault();

                            EntityExtension.FlagForUpdate(detail, user, "Facade");

                            if (existPR == null)
                            {
                                PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == detail.PRItemId);
                                purchaseRequestItem.Status = "Sudah diterima Pembelian";
                            }


                            InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == detail.POItemId);
                            internalPurchaseOrderItem.Status = "Sudah dibuat PO Eksternal";
                        }
                    }

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

        public int HideUnpost(string PONo, string user, POExternalUpdateModel model)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet.SingleOrDefault(epo => epo.EPONo == PONo);
                    EntityExtension.FlagForUpdate(m, user, "Facade");
                    m.IsCreateOnVBRequest = model.IsCreateOnVBRequest;

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

        async Task<string> GenerateNo(ExternalPurchaseOrder model, int clientTimeZoneOffset)
        {
            DateTimeOffset Now = model.OrderDate;
            string Year = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy"); ;
            string Month = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM"); ;

            string no = $"PE-{model.UnitCode}-{Year}-{Month}-";
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.EPONo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.EPONo).FirstOrDefaultAsync();
            no = $"{no}";

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.EPONo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public List<ExternalPurchaseOrder> ReadUnused(string Keyword = null, string Filter = "{}")
        {
            IQueryable<ExternalPurchaseOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "EPONo", "SupplierName", "DivisionName","UnitName"
            };

            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah

            Query = Query
                .Where(m => m.IsPosted == true && m.IsCanceled == false && m.IsClosed == false && m.IsDeleted == false)
                .Select(s => new ExternalPurchaseOrder
                {
                    Id = s.Id,
                    EPONo = s.EPONo,
                    CurrencyCode = s.CurrencyCode,
                    CurrencyRate = s.CurrencyRate,
                    OrderDate = s.OrderDate,
                    DeliveryDate = s.DeliveryDate,
                    SupplierId = s.SupplierId,
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    DivisionCode = s.DivisionCode,
                    DivisionName = s.DivisionName,
                    LastModifiedUtc = s.LastModifiedUtc,
                    UnitName = s.UnitName,
                    UnitId = s.UnitId,
                    UnitCode = s.UnitCode,
                    CreatedBy = s.CreatedBy,
                    IsPosted = s.IsPosted,
                    Items = s.Items
                        .Select(i => new ExternalPurchaseOrderItem
                        {
                            Id = i.Id,
                            PRId = i.PRId,
                            PRNo = i.PRNo,
                            UnitId = i.UnitId,
                            UnitCode = i.UnitCode,
                            UnitName = i.UnitName,
                            Details = i.Details
                                .Where(d => d.DOQuantity < d.DealQuantity && d.IsDeleted == false)
                                .Select(d => new ExternalPurchaseOrderDetail
                                {
                                    Id = d.Id,
                                    POItemId = d.POItemId,
                                    PRItemId = d.PRItemId,
                                    ProductId = d.ProductId,
                                    ProductCode = d.ProductCode,
                                    ProductName = d.ProductName,
                                    DealQuantity = d.DealQuantity,
                                    DealUomId = d.DealUomId,
                                    DealUomUnit = d.DealUomUnit,
                                    DOQuantity = d.DOQuantity,
                                    ProductRemark = d.ProductRemark,
                                })
                                .ToList()
                        })
                        .Where(i => i.Details.Count > 0)
                        .ToList()
                })
                .Where(m => m.Items.Count > 0);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureFilter(Query, FilterDictionary);

            return Query.ToList();
        }

        public List<EPOViewModel> ReadDisposition(string Keyword = null, string currencyId = "", string supplierId = "", string categoryId = "", string divisionId = "", string incomeTaxBy = "")
        {
            IQueryable<ExternalPurchaseOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "EPONo", "SupplierName", "DivisionName","UnitName"
            };

            Query = QueryHelper<ExternalPurchaseOrder>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah
            List<EPOViewModel> list = new List<EPOViewModel>();
            list = Query
                .Where(m => m.IsPosted == true && m.IsCanceled == false && m.IsClosed == false && m.IsDeleted == false
                            && m.SupplierId == supplierId && m.CurrencyId == currencyId && m.DivisionId == divisionId && m.IncomeTaxBy == (incomeTaxBy == null ? "" : incomeTaxBy))
                .Select(s => new EPOViewModel
                {
                    _id = s.Id,
                    no = s.EPONo,
                    unit = new UnitViewModel
                    {
                        _id = s.UnitId,
                        name = s.UnitName,
                        code = s.UnitCode,
                    },
                    useVat = s.UseVat,
                    useIncomeTax = s.UseIncomeTax,
                    incomeTax = new IncomeTaxViewModel
                    {
                        _id = s.IncomeTaxId,
                        name = s.IncomeTaxName,
                        rate = s.IncomeTaxRate,
                    },
                    vatTax = new VatTaxViewModel
                    {
                        _id = s.VatId,
                        rate = s.VatRate,
                    },
                    items = s.Items.Join(dbContext.InternalPurchaseOrders,
                                          i => i.POId,
                                          j => j.Id,
                                          (i, j) => new EPOItemViewModel
                                          {
                                              _id = i.Id,
                                              IsDeleted = i.IsDeleted,
                                              prId = i.PRId,
                                              poId = i.POId,
                                              prNo = i.PRNo,
                                              ipoIsdeleted = j.IsDeleted,
                                              category = new CategoryViewModel
                                              {
                                                  _id = j.CategoryId,
                                                  code = j.CategoryCode,
                                                  name = j.CategoryName
                                              },
                                              details = i.Details
                                                        .Where(d => d.IsDeleted == false)
                                                        .Select(d => new EPODetailViewModel
                                                        {
                                                            _id = d.Id,
                                                            poItemId = d.POItemId,
                                                            prItemId = d.PRItemId,
                                                            product = new ProductViewModel
                                                            {
                                                                _id = d.ProductId,
                                                                code = d.ProductCode,
                                                                name = d.ProductName,
                                                            },

                                                            dealQuantity = d.DealQuantity,
                                                            dealUom = new UomViewModel
                                                            {
                                                                _id = d.DealUomId,
                                                                unit = d.DealUomUnit,
                                                            },
                                                            doQuantity = d.DOQuantity,
                                                            dispositionQuantity = d.DispositionQuantity,
                                                            productRemark = d.ProductRemark,
                                                            priceBeforeTax = d.PriceBeforeTax,
                                                            pricePerDealUnit = d.PricePerDealUnit,

                                                        })
                                                        .ToList()
                                          })
                                          .Where(a => a.category._id == categoryId && a.ipoIsdeleted == false && a.IsDeleted == false)
                                          .ToList()

                })
                .Where(m => m.items.Count > 0).ToList();

            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //Query = QueryHelper<ExternalPurchaseOrder>.ConfigureFilter(Query, FilterDictionary);

            //foreach(var data in Query)
            //{
            //    ExternalPurchaseOrderViewModel epo = new ExternalPurchaseOrderViewModel()
            //    {

            //    };
            //    foreach (var item in data.Items)
            //    {

            //    }
            //}
            return list;
        }

        public ProductViewModel GetProduct(string productId)
        {
            string productUri = "master/products";
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{productUri}/{productId}").Result.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
            var jsonUOM = result.Single(p => p.Key.Equals("data")).Value;
            ProductViewModel viewModel = JsonConvert.DeserializeObject<ProductViewModel>(result.GetValueOrDefault("data").ToString());
            return viewModel;
        }

        #region Duration ExternalPO-DO
        public IQueryable<ExternalPurchaseDeliveryOrderDurationReportViewModel> GetEPODODurationReportQuery(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            int start = 0;
            int end = 0;
            if (duration == "31-60 hari")
            {
                start = 31;
                end = 60;
            }
            else if (duration == "61-90 hari")
            {
                start = 61;
                end = 90;
            }
            else
            {
                start = 91;
                end = 1000;
            }
            List<ExternalPurchaseDeliveryOrderDurationReportViewModel> listEPODUration = new List<ExternalPurchaseDeliveryOrderDurationReportViewModel>();
            var Query = (from a in dbContext.ExternalPurchaseOrders
                         join b in dbContext.ExternalPurchaseOrderItems on a.Id equals b.EPOId
                         join c in dbContext.ExternalPurchaseOrderDetails on b.Id equals c.EPOItemId
                         join d in dbContext.PurchaseRequests on b.PRId equals d.Id
                         join e in dbContext.InternalPurchaseOrders on b.POId equals e.Id
                         join f in dbContext.InternalPurchaseOrderItems on e.Id equals f.POId
                         join g in dbContext.DeliveryOrderItems on a.Id equals g.EPOId
                         join h in dbContext.DeliveryOrders on g.DOId equals h.Id
                         //Conditions
                         where a.IsDeleted == false
                         //&& b.Id == e.PRItemId
                         && b.IsDeleted == false
                         && c.IsDeleted == false
                         && d.IsDeleted == false
                         && e.IsDeleted == false
                         && f.IsDeleted == false
                         && g.IsDeleted == false
                         && h.IsDeleted == false
                         && a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit)
                         && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                         && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new ExternalPurchaseDeliveryOrderDurationReportViewModel
                         {
                             prNo = d.No,
                             prDate = d.Date,
                             prCreatedDate = d.CreatedUtc,
                             unit = d.UnitName,
                             category = d.CategoryName,
                             division = d.DivisionName,
                             budget = d.BudgetName,
                             productCode = c.ProductCode,
                             productName = c.ProductName,
                             dealQuantity = c.DealQuantity,
                             dealUomUnit = c.DealUomUnit,
                             pricePerDealUnit = c.PricePerDealUnit,
                             supplierCode = a.SupplierCode,
                             supplierName = a.SupplierName,
                             poDate = e.CreatedUtc,
                             orderDate = a.OrderDate,
                             ePOCreatedDate = a.CreatedUtc,
                             deliveryDate = a.DeliveryDate,
                             ePONo = a.EPONo,
                             dODate = h.DODate,
                             arrivalDate = h.ArrivalDate,
                             dONo = h.DONo,
                             staff = a.CreatedBy,
                         }).Distinct();
            foreach (var item in Query)
            {
                var ePODate = new DateTimeOffset(item.ePOCreatedDate.Date, TimeSpan.Zero);
                var doDate = new DateTimeOffset(item.dODate.Date, TimeSpan.Zero);

                var datediff = (((TimeSpan)(doDate - ePODate)).Days) + 1;
                ExternalPurchaseDeliveryOrderDurationReportViewModel _new = new ExternalPurchaseDeliveryOrderDurationReportViewModel
                {
                    prNo = item.prNo,
                    prDate = item.prDate,
                    prCreatedDate = item.prCreatedDate,
                    unit = item.unit,
                    category = item.category,
                    division = item.division,
                    budget = item.budget,
                    productCode = item.productCode,
                    productName = item.productName,
                    dealQuantity = item.dealQuantity,
                    dealUomUnit = item.dealUomUnit,
                    pricePerDealUnit = item.pricePerDealUnit,
                    supplierCode = item.supplierCode,
                    supplierName = item.supplierName,
                    poDate = item.poDate,
                    orderDate = item.orderDate,
                    ePOCreatedDate = item.ePOCreatedDate,
                    deliveryDate = item.deliveryDate,
                    ePONo = item.ePONo,
                    dODate = item.dODate,
                    arrivalDate = item.arrivalDate,
                    dONo = item.dONo,
                    dateDiff = datediff,
                    staff = item.staff,
                };
                listEPODUration.Add(_new);
            }
            return listEPODUration.Where(s => s.dateDiff >= start && s.dateDiff <= end).AsQueryable();


        }


        public Tuple<List<ExternalPurchaseDeliveryOrderDurationReportViewModel>, int> GetEPODODurationReport(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetEPODODurationReportQuery(unit, duration, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.prCreatedDate);
            }

            Pageable<ExternalPurchaseDeliveryOrderDurationReportViewModel> pageable = new Pageable<ExternalPurchaseDeliveryOrderDurationReportViewModel>(Query, page - 1, size);
            List<ExternalPurchaseDeliveryOrderDurationReportViewModel> Data = pageable.Data.ToList<ExternalPurchaseDeliveryOrderDurationReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcelEPODODuration(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetEPODODurationReportQuery(unit, duration, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.prCreatedDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Buat Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Buat PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Target Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Datang Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih Tanggal PO Eksternal - Surat Jalan (hari)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Staff Pembelian", DataType = typeof(string) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string prDate = item.prDate == null ? "-" : item.prDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string prCreatedDate = item.prCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.prCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string poDate = item.poDate == new DateTime(1970, 1, 1) ? "-" : item.poDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string orderDate = item.orderDate == new DateTime(1970, 1, 1) ? "-" : item.orderDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ePOCreatedDate = item.ePOCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.ePOCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string deliveryDate = item.deliveryDate == new DateTime(1970, 1, 1) ? "-" : item.deliveryDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string dODate = item.dODate == new DateTime(1970, 1, 1) ? "-" : item.dODate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string arrivalDate = item.arrivalDate == new DateTime(1970, 1, 1) ? "-" : item.arrivalDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, prDate, prCreatedDate, item.prNo, item.division, item.unit, item.budget, item.category, item.productCode, item.productName, item.dealQuantity, item.dealUomUnit, item.pricePerDealUnit, item.supplierCode, item.supplierName, poDate, orderDate, ePOCreatedDate, deliveryDate, item.ePONo, dODate, arrivalDate, item.dONo, item.dateDiff, item.staff);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion
    }
}
