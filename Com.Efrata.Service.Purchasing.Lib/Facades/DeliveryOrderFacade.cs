using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Efrata.Service.Purchasing.Lib.Facades
{
    public class DeliveryOrderFacade : IDeliveryOrderFacade
    {

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<DeliveryOrder> dbSet;
        public readonly IServiceProvider serviceProvider;

        private string USER_AGENT = "Facade";

        public DeliveryOrderFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<DeliveryOrder>();
            this.serviceProvider = serviceProvider;
        }

        public Tuple<List<DeliveryOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<DeliveryOrder> Query = this.dbSet.Include(x => x.Items).ThenInclude(x => x.Details);

            //Query = Query.Select(s => new DeliveryOrder
            //{
            //    Id = s.Id,
            //    UId = s.UId,
            //    DONo = s.DONo,
            //    DODate = s.DODate,
            //    ArrivalDate = s.ArrivalDate,
            //    SupplierName = s.SupplierName,
            //    SupplierId = s.SupplierId,
            //    IsClosed = s.IsClosed,
            //    CreatedBy = s.CreatedBy,
            //    LastModifiedUtc = s.LastModifiedUtc,
            //    Items = s.Items.Select(i => new DeliveryOrderItem
            //    {
            //        EPOId = i.EPOId,
            //        EPONo = i.EPONo,
            //        Details = i.Details.ToList()
            //    }).ToList()
            //});

            List<string> searchAttributes = new List<string>()
            {
                "DONo", "SupplierName", "Items.EPONo"
            };

            //Query = QueryHelper<DeliveryOrder>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dan dengan searchAttributes ada "." maka case sensitive, kalo tanpa "." tidak masalah
            Query = QueryHelper<DeliveryOrder>.ConfigureSearch(Query, searchAttributes, Keyword, true); // bisa make ToLower()

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<DeliveryOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<DeliveryOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<DeliveryOrder> pageable = new Pageable<DeliveryOrder>(Query, Page - 1, Size);
            List<DeliveryOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<DeliveryOrder, List<long>> ReadById(int id)
        {
            var Result = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details)
                .FirstOrDefault();

            List<long> unitReceiptNoteIds = dbContext.UnitReceiptNotes.Where(m => m.DOId == id && m.IsDeleted == false).Select(m => m.Id).ToList();

            return Tuple.Create(Result, unitReceiptNoteIds);
        }

        public async Task<int> Create(DeliveryOrder model, string username)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForCreate(item, username, USER_AGENT);

                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForCreate(detail, username, USER_AGENT);

                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == detail.EPODetailId);

                            if (externalPurchaseOrderDetail != null)
                            {
                                externalPurchaseOrderDetail.DOQuantity += detail.DOQuantity;
                                EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, username, USER_AGENT);
                                SetStatus(externalPurchaseOrderDetail, detail, username);
                            }

                        }
                    }

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    Created += await AddFulfillmentAsync(model, username);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    while (e.InnerException != null) e = e.InnerException;
                    throw e;
                }
            }

            return Created;
        }

        public async Task<int> Update(int id, DeliveryOrder model, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingModel = this.dbSet.AsNoTracking()
                        .Include(d => d.Items)
                            .ThenInclude(d => d.Details)
                            .AsNoTracking()
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                    if (existingModel != null && id == model.Id)
                    {
                        EntityExtension.FlagForUpdate(model, user, USER_AGENT);

                        foreach (var item in model.Items.ToList())
                        {
                            var existingItem = existingModel.Items.SingleOrDefault(m => m.Id == item.Id);
                            List<DeliveryOrderItem> duplicateDeliveryOrderItems = model.Items.Where(i => i.EPOId == item.EPOId && i.Id != item.Id).ToList();

                            if (item.Id == 0)
                            {
                                if (duplicateDeliveryOrderItems.Count <= 0)
                                {
                                    if (model.Items.Count(i => i.EPOId == item.EPOId && !string.IsNullOrWhiteSpace(i.CreatedBy)) < 1)
                                    {
                                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                                        var itemDetails = new List<DeliveryOrderDetail>();

                                        foreach (var duplicateItem in model.Items.Where(i => i.EPOId == item.EPOId).ToList())
                                        {
                                            foreach (var duplicateDetail in duplicateItem.Details.ToList())
                                            {
                                                if (itemDetails.Count(d => d.PRId.Equals(duplicateDetail.PRId) && d.ProductId.Equals(duplicateDetail.ProductId)) < 1)
                                                {
                                                    EntityExtension.FlagForCreate(duplicateDetail, user, USER_AGENT);

                                                    itemDetails.Add(duplicateDetail);
                                                }
                                                else
                                                {
                                                    var oldDetail = itemDetails.FirstOrDefault(d => d.PRId.Equals(duplicateDetail.PRId) && d.ProductId.Equals(duplicateDetail.ProductId));
                                                    if (oldDetail != null)
                                                    {
                                                        oldDetail.DOQuantity += duplicateDetail.DOQuantity;
                                                        oldDetail.ProductRemark = String.Concat(oldDetail.ProductRemark, Environment.NewLine, duplicateDetail.ProductRemark).Trim();
                                                    }

                                                }
                                            }
                                        }

                                        item.Details = itemDetails;

                                        foreach (var detail in item.Details.ToList())
                                        {
                                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == detail.EPODetailId);

                                            if (externalPurchaseOrderDetail != null)
                                            {
                                                externalPurchaseOrderDetail.DOQuantity += detail.DOQuantity;
                                                EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);

                                                SetStatus(externalPurchaseOrderDetail, detail, user);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        model.Items.Remove(item);
                                    }
                                }
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(item, user, USER_AGENT);

                                if (duplicateDeliveryOrderItems.Count > 0)
                                {
                                    foreach (var detail in item.Details.ToList())
                                    {
                                        if (detail.Id != 0)
                                        {
                                            EntityExtension.FlagForUpdate(detail, user, USER_AGENT);

                                            foreach (var duplicateItem in duplicateDeliveryOrderItems.ToList())
                                            {
                                                foreach (var duplicateDetail in duplicateItem.Details.ToList())
                                                {
                                                    if (detail.PRId.Equals(duplicateDetail.PRId) && detail.ProductId.Equals(duplicateDetail.ProductId))
                                                    {
                                                        detail.DOQuantity += duplicateDetail.DOQuantity;
                                                        detail.ProductRemark = String.Concat(detail.ProductRemark, Environment.NewLine, duplicateDetail.ProductRemark).Trim();
                                                    }
                                                    else if (item.Details.Count(d => d.PRId.Equals(duplicateDetail.PRId) && d.ProductId.Equals(duplicateDetail.ProductId)) < 1)
                                                    {
                                                        double duplicateDetailDOQuantity = 0;
                                                        string duplicateDetailProductRemark = string.Empty;
                                                        foreach (var duplicateItemAgain in duplicateDeliveryOrderItems.ToList())
                                                        {
                                                            var duplicateDetailAgain = duplicateItemAgain.Details.SingleOrDefault(d => d.PRId.Equals(duplicateDetail.PRId) && d.ProductId.Equals(duplicateDetail.ProductId));
                                                            duplicateDetailDOQuantity += duplicateDetailAgain.DOQuantity;
                                                            duplicateDetailProductRemark = String.Concat(duplicateDetailProductRemark, Environment.NewLine, duplicateDetailAgain.ProductRemark).Trim();
                                                        }
                                                        duplicateDetail.DOQuantity = duplicateDetailDOQuantity;
                                                        duplicateDetail.ProductRemark = duplicateDetailProductRemark;

                                                        EntityExtension.FlagForCreate(duplicateDetail, user, USER_AGENT);
                                                        item.Details.Add(duplicateDetail);

                                                        ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == duplicateDetail.EPODetailId);

                                                        if (externalPurchaseOrderDetail != null)
                                                        {
                                                            externalPurchaseOrderDetail.DOQuantity += duplicateDetail.DOQuantity;

                                                            EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);
                                                            SetStatus(externalPurchaseOrderDetail, duplicateDetail, user);
                                                        }

                                                    }
                                                }
                                                model.Items.Remove(duplicateItem);
                                            }

                                            var existingDetail = existingItem.Details.SingleOrDefault(m => m.Id == detail.Id);
                                            if (existingDetail != null)
                                            {
                                                ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == detail.EPODetailId);
                                                externalPurchaseOrderDetail.DOQuantity = externalPurchaseOrderDetail.DOQuantity - existingDetail.DOQuantity + detail.DOQuantity;

                                                EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);
                                                SetStatus(externalPurchaseOrderDetail, detail, user);
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
                                            EntityExtension.FlagForUpdate(detail, user, USER_AGENT);

                                            var existingDetail = existingItem.Details.SingleOrDefault(m => m.Id == detail.Id);

                                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == detail.EPODetailId);

                                            if (externalPurchaseOrderDetail != null)
                                            {
                                                externalPurchaseOrderDetail.DOQuantity = externalPurchaseOrderDetail.DOQuantity - existingDetail.DOQuantity + detail.DOQuantity;
                                                EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);
                                                SetStatus(externalPurchaseOrderDetail, detail, user);
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        this.dbContext.Update(model);

                        foreach (var existingItem in existingModel.Items)
                        {
                            var newItem = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                            if (newItem == null)
                            {
                                EntityExtension.FlagForDelete(existingItem, user, USER_AGENT);
                                this.dbContext.DeliveryOrderItems.Update(existingItem);
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    EntityExtension.FlagForDelete(existingDetail, user, USER_AGENT);
                                    dbContext.Entry(existingDetail).State = EntityState.Modified;
                                    //this.dbContext.DeliveryOrderDetails.Update(existingDetail);

                                    ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == existingDetail.EPODetailId);

                                    if (externalPurchaseOrderDetail != null)
                                    {
                                        externalPurchaseOrderDetail.DOQuantity -= existingDetail.DOQuantity;
                                        EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);
                                        SetStatus(externalPurchaseOrderDetail, existingDetail, user);
                                    }

                                }
                            }
                            else
                            {
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    var newDetail = newItem.Details.FirstOrDefault(d => d.Id == existingDetail.Id);
                                    if (newDetail == null)
                                    {
                                        EntityExtension.FlagForDelete(existingDetail, user, USER_AGENT);
                                        dbContext.Entry(existingDetail).State = EntityState.Modified;
                                        //this.dbContext.DeliveryOrderDetails.Update(existingDetail);

                                        ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == existingDetail.EPODetailId);
                                        if (externalPurchaseOrderDetail != null)
                                        {
                                            externalPurchaseOrderDetail.DOQuantity -= existingDetail.DOQuantity;
                                            EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, user, USER_AGENT);
                                            SetStatus(externalPurchaseOrderDetail, existingDetail, user);
                                        }

                                    }
                                }
                            }
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        var updatedModel = this.dbSet.AsNoTracking()
                           .Include(d => d.Items)
                               .ThenInclude(d => d.Details)
                           .SingleOrDefault(pr => pr.Id == model.Id && !pr.IsDeleted);
                        Updated += await EditFulfillmentAsync(updatedModel, user);
                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Invalid Id");
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    while (e.InnerException != null) e = e.InnerException;
                    throw e;
                }
            }

            return Updated;
        }



        public int Delete(int id, string username)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = this.dbSet
                        .Include(d => d.Items)
                            .ThenInclude(d => d.Details)
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                    EntityExtension.FlagForDelete(model, username, USER_AGENT);

                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForDelete(item, username, USER_AGENT);
                        foreach (var detail in item.Details)
                        {
                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.SingleOrDefault(m => m.Id == detail.EPODetailId);
                            externalPurchaseOrderDetail.DOQuantity -= detail.DOQuantity;
                            EntityExtension.FlagForUpdate(externalPurchaseOrderDetail, username, USER_AGENT);
                            SetStatus(externalPurchaseOrderDetail, detail, username);

                            EntityExtension.FlagForDelete(detail, username, USER_AGENT);
                        }
                    }

                    Deleted = dbContext.SaveChanges();
                    Deleted += RemoveFulfillment(model, username);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    while (e.InnerException != null) e = e.InnerException;
                    throw e;
                }
            }

            return Deleted;
        }

        private void SetStatus(ExternalPurchaseOrderDetail externalPurchaseOrderDetail, DeliveryOrderDetail detail, string username)
        {
            if (externalPurchaseOrderDetail.ReceiptQuantity == 0)
            {
                if (dbContext.UnitPaymentOrderDetails.FirstOrDefault(d => d.POItemId == externalPurchaseOrderDetail.POItemId) == null)
                {

                    //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.SingleOrDefault(i => i.Id == detail.PRItemId);
                    InternalPurchaseOrderItem internalPurchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.SingleOrDefault(i => i.Id == detail.POItemId);

                    if (externalPurchaseOrderDetail.DOQuantity == 0)
                    {
                        //purchaseRequestItem.Status = "Sudah diorder ke Supplier";
                        internalPurchaseOrderItem.Status = "Sudah diorder ke Supplier";

                        //EntityExtension.FlagForUpdate(purchaseRequestItem, username, USER_AGENT);
                        EntityExtension.FlagForUpdate(internalPurchaseOrderItem, username, USER_AGENT);
                    }
                    else if (externalPurchaseOrderDetail.DOQuantity > 0 && externalPurchaseOrderDetail.DOQuantity < externalPurchaseOrderDetail.DealQuantity)
                    {
                        //purchaseRequestItem.Status = "Barang sudah datang parsial";
                        internalPurchaseOrderItem.Status = "Barang sudah datang parsial";

                        //EntityExtension.FlagForUpdate(purchaseRequestItem, username, USER_AGENT);
                        EntityExtension.FlagForUpdate(internalPurchaseOrderItem, username, USER_AGENT);
                    }
                    else if (externalPurchaseOrderDetail.DOQuantity > 0 && externalPurchaseOrderDetail.DOQuantity >= externalPurchaseOrderDetail.DealQuantity)
                    {
                        //purchaseRequestItem.Status = "Barang sudah datang semua";
                        internalPurchaseOrderItem.Status = "Barang sudah datang semua";

                        //EntityExtension.FlagForUpdate(purchaseRequestItem, username, USER_AGENT);
                        EntityExtension.FlagForUpdate(internalPurchaseOrderItem, username, USER_AGENT);
                    }

                }
            }
        }

        public List<DeliveryOrder> ReadBySupplier(string Keyword = null, string unitId = "", string supplierId = "")
        {
            IQueryable<DeliveryOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "DONo", "SupplierName", "Items.EPONo"
            };

            Query = QueryHelper<DeliveryOrder>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah

            Query = Query
                .Where(m => m.IsClosed == false && m.IsDeleted == false && m.SupplierId == supplierId)
                .Select(s => new DeliveryOrder
                {
                    Id = s.Id,
                    UId = s.UId,
                    DONo = s.DONo,
                    DODate = s.DODate,
                    ArrivalDate = s.ArrivalDate,
                    SupplierName = s.SupplierName,
                    SupplierId = s.SupplierId,
                    IsClosed = s.IsClosed,
                    CreatedBy = s.CreatedBy,
                    LastModifiedUtc = s.LastModifiedUtc,
                    Items = s.Items.Select(i => new DeliveryOrderItem
                    {
                        EPOId = i.EPOId,
                        EPONo = i.EPONo,
                        DOId = i.DOId,
                        Details = i.Details
                                .Select(d => new DeliveryOrderDetail
                                {
                                    Id = d.Id,
                                    POItemId = d.POItemId,
                                    PRItemId = d.PRItemId,
                                    PRId = d.PRId,
                                    PRNo = d.PRNo,
                                    ProductId = d.ProductId,
                                    ProductCode = d.ProductCode,
                                    ProductName = d.ProductName,
                                    DealQuantity = d.DealQuantity,
                                    DOQuantity = d.DOQuantity,
                                    ProductRemark = d.ProductRemark,
                                    UnitId = d.UnitId,
                                    EPODetailId = d.EPODetailId,
                                    DOItemId = d.DOItemId,
                                    ReceiptQuantity = d.ReceiptQuantity,
                                    UomId = d.UomId,
                                    UomUnit = d.UomUnit
                                }).Where(d => d.UnitId == unitId)
                                .ToList()
                    })
                        .Where(i => i.Details.Count > 0)
                        .ToList()
                })
                .Where(m => m.Items.Count > 0);

            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //Query = QueryHelper<DeliveryOrder>.ConfigureFilter(Query, FilterDictionary);

            return Query.ToList();
        }

        public IQueryable<DeliveryOrderReportViewModel> GetReportQuery(string no, string supplierId, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.DeliveryOrders
                         join i in dbContext.DeliveryOrderItems on a.Id equals i.DOId
                         join j in dbContext.DeliveryOrderDetails on i.Id equals j.DOItemId
                         join l in dbContext.ExternalPurchaseOrderDetails on j.EPODetailId equals l.Id
                         where a.IsDeleted == false
                             && i.IsDeleted == false
                             && j.IsDeleted == false
                             && l.IsDeleted == false
                             && a.DONo == (string.IsNullOrWhiteSpace(no) ? a.DONo : no)
                             && a.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? a.SupplierId : supplierId)
                             && a.DODate.AddHours(offset).Date >= DateFrom.Date
                             && a.DODate.AddHours(offset).Date <= DateTo.Date
                         select new DeliveryOrderReportViewModel
                         {
                             no = a.DONo,
                             supplierDoDate = a.DODate == null ? new DateTime(1970, 1, 1) : a.DODate,
                             date = a.ArrivalDate,
                             supplierName = a.SupplierName,
                             supplierCode = a.SupplierCode,
                             ePONo = i.EPONo,
                             productCode = j.ProductCode,
                             productName = j.ProductName,
                             productRemark = j.ProductRemark,
                             dealQuantity = l.DealQuantity,
                             dOQuantity = j.DOQuantity,
                             remainingQuantity = l.DealQuantity,
                             uomUnit = j.UomUnit,
                             LastModifiedUtc = j.LastModifiedUtc,
                             CreatedUtc = j.CreatedUtc,
                             ePODetailId = j.EPODetailId
                         });
            Dictionary<string, double> q = new Dictionary<string, double>();
            List<DeliveryOrderReportViewModel> urn = new List<DeliveryOrderReportViewModel>();
            foreach (DeliveryOrderReportViewModel data in Query.ToList())
            {
                double value;
                if (q.TryGetValue(data.productCode + data.ePONo + data.ePODetailId, out value))
                {
                    q[data.productCode + data.ePONo + data.ePODetailId] -= data.dOQuantity;
                    data.remainingQuantity = q[data.productCode + data.ePONo + data.ePODetailId];
                    urn.Add(data);
                }
                else
                {
                    q[data.productCode + data.ePONo + data.ePODetailId] = data.remainingQuantity - data.dOQuantity;
                    data.remainingQuantity = q[data.productCode + data.ePONo + data.ePODetailId];
                    urn.Add(data);
                }
            }
            return Query = urn.AsQueryable();
        }

        public Tuple<List<DeliveryOrderReportViewModel>, int> GetReport(string no, string supplierId, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(no, supplierId, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.supplierDoDate).ThenByDescending(b => b.CreatedUtc);
            }


            Pageable<DeliveryOrderReportViewModel> pageable = new Pageable<DeliveryOrderReportViewModel>(Query, page - 1, size);
            List<DeliveryOrderReportViewModel> Data = pageable.Data.ToList<DeliveryOrderReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(string no, string supplierId, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(no, supplierId, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.supplierDoDate).ThenByDescending(b => b.CreatedUtc);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR SURAT JALAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL SURAT JALAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL DATANG BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NO PO EXTERNAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "DESKRIPSI BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG YANG DIMINTA", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG YANG DATANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SISA QTY", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, 0, 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.date == null ? "-" : item.date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string supplierDoDate = item.supplierDoDate == new DateTime(1970, 1, 1) ? "-" : item.supplierDoDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.supplierCode, item.supplierName, item.no, supplierDoDate, date, item.ePONo, item.productCode, item.productName, item.productRemark, item.dealQuantity, item.dOQuantity, item.remainingQuantity, item.uomUnit);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        private async Task<int> AddFulfillmentAsync(DeliveryOrder model, string username)
        {
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;
            foreach (var item in model.Items)
            {
                foreach (var detail in item.Details)
                {
                    var fulfillment = new InternalPurchaseOrderFulFillment()
                    {
                        DeliveryOrderDate = model.ArrivalDate,
                        DeliveryOrderDeliveredQuantity = detail.DOQuantity,
                        DeliveryOrderDetailId = detail.Id,
                        DeliveryOrderId = model.Id,
                        DeliveryOrderItemId = item.Id,
                        DeliveryOrderNo = model.DONo,
                        SupplierDODate = model.DODate,
                        POItemId = detail.POItemId
                    };


                    count += await internalPOFacade.CreateFulfillmentAsync(fulfillment, username);

                }
            }
            return count;
        }

        private async Task<int> EditFulfillmentAsync(DeliveryOrder model, string username)
        {
            int count = 0;
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            var dbFulfillments = dbContext.InternalPurchaseOrderFulfillments.AsNoTracking().Where(x => x.DeliveryOrderId == model.Id);
            var localFulfilments = model.Items.SelectMany(x => x.Details);

            var addedFulfillments = localFulfilments.Where(x => !dbFulfillments.Any(y => y.DeliveryOrderItemId == x.DOItemId && y.DeliveryOrderDetailId == x.Id && y.POItemId == x.POItemId));
            var updatedFulfillments = localFulfilments.Where(x => dbFulfillments.Any(y => y.DeliveryOrderItemId == x.DOItemId && y.DeliveryOrderDetailId == x.Id && y.POItemId == x.POItemId));
            var deletedFulfillments = dbFulfillments.Where(x => !localFulfilments.Any(y => y.DOItemId == x.DeliveryOrderItemId && y.Id == x.DeliveryOrderDetailId && y.POItemId == x.POItemId));

            foreach (var item in updatedFulfillments)
            {
                var dbItem = dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                    .FirstOrDefault(x => x.DeliveryOrderId == model.Id && x.DeliveryOrderItemId == item.DOItemId && x.DeliveryOrderDetailId == item.Id && item.POItemId == x.POItemId);

                dbItem.DeliveryOrderDate = model.ArrivalDate;
                dbItem.SupplierDODate = model.DODate;
                dbItem.DeliveryOrderDeliveredQuantity = item.DOQuantity;
                dbItem.DeliveryOrderNo = model.DONo;
                dbItem.POItemId = item.POItemId;

                count += await internalPOFacade.UpdateFulfillmentAsync(dbItem.Id, dbItem, username);

            }

            foreach (var item in addedFulfillments)
            {
                var fulfillment = new InternalPurchaseOrderFulFillment()
                {
                    DeliveryOrderDate = model.ArrivalDate,
                    DeliveryOrderDeliveredQuantity = item.DOQuantity,
                    DeliveryOrderDetailId = item.Id,
                    DeliveryOrderId = model.Id,
                    DeliveryOrderItemId = item.DOItemId,
                    DeliveryOrderNo = model.DONo,
                    SupplierDODate = model.DODate,
                    POItemId = item.POItemId
                };

                count += await internalPOFacade.CreateFulfillmentAsync(fulfillment, username);
            }

            foreach (var item in deletedFulfillments)
            {
                count += internalPOFacade.DeleteFulfillment(item.Id, username);
            }

            return count;
        }

        private int RemoveFulfillment(DeliveryOrder model, string username)
        {
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;
            foreach (var item in model.Items)
            {
                foreach (var detail in item.Details)
                {
                    var fulfillment = dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                        .FirstOrDefault(x => x.DeliveryOrderId == model.Id && x.DeliveryOrderItemId == item.Id && x.DeliveryOrderDetailId == detail.Id);
                    if (fulfillment != null)
                        count += internalPOFacade.DeleteFulfillment(fulfillment.Id, username);

                }
            }
            return count;
        }
    }


}