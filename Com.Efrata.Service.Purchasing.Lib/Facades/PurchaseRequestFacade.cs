using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
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
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades
{
    public class PurchaseRequestFacade
    {
        #region DUMMY_DATA
        private List<PurchaseRequest> DUMMY_DATA = new List<PurchaseRequest>()
        {
            new PurchaseRequest()
            {
                Id = 1,
                Active = true,
                BudgetId = "BudgetId-1",
                BudgetCode = "BudgetCode-1",
                BudgetName = "BudgetName-1",
                CategoryId = "CategoryId-1",
                CategoryCode = "CategoryCode-1",
                CategoryName = "CategoryName-1",
                CreatedAgent = "Dummy-1",
                CreatedBy = "Dummy-1",
                CreatedUtc = DateTime.UtcNow,
                Date = DateTimeOffset.UtcNow,
                DeletedAgent = "",
                DeletedBy = "",
                DivisionId = "DivisionId-1",
                DivisionCode = "DivisionCode-1",
                DivisionName = "DivisionName-1",
                ExpectedDeliveryDate = DateTimeOffset.UtcNow,
                Internal = false,
                IsDeleted = false,
                IsPosted = false,
                IsUsed = false,
                LastModifiedAgent = "Dummy-1",
                LastModifiedBy = "Dummy-1",
                LastModifiedUtc = DateTime.UtcNow,
                No = "No-1",
                Remark = "Remark-1",
                Status = Enums.PurchaseRequestStatus.CREATED,
                UId = "8ad231fk1049201da",
                UnitId = "UnitId-1",
                UnitCode = "UnitCode-1",
                UnitName = "UnitName-1",
                Items = new List<PurchaseRequestItem>()
                {
                    new PurchaseRequestItem()
                    {
                        Id = 1,
                        Active = true,
                        IsDeleted = false,
                        PurchaseRequestId = 1,
                        CreatedAgent = "Dummy-1",
                        CreatedBy = "Dummy-1",
                        CreatedUtc = DateTime.UtcNow,
                        LastModifiedAgent = "Dummy-1",
                        LastModifiedBy = "Dummy-1",
                        LastModifiedUtc = DateTime.UtcNow.AddDays(1),
                        DeletedAgent = "",
                        DeletedBy = "",
                        ProductId = "ProductId-1",
                        ProductCode = "ProductCode-1",
                        ProductName = "ProductName-1",
                        Quantity = 10,
                        Remark = "Remark-1",
                        Uom = "MTR"
                    },
                    new PurchaseRequestItem()
                    {
                        Id = 2,
                        Active = true,
                        IsDeleted = false,
                        PurchaseRequestId = 1,
                        CreatedAgent = "Dummy-1",
                        CreatedBy = "Dummy-1",
                        CreatedUtc = DateTime.UtcNow,
                        LastModifiedAgent = "Dummy-1",
                        LastModifiedBy = "Dummy-1",
                        LastModifiedUtc = DateTime.UtcNow,
                        DeletedAgent = "",
                        DeletedBy = "",
                        ProductId = "ProductId-2",
                        ProductCode = "ProductCode-2",
                        ProductName = "ProductName-2",
                        Quantity = 10,
                        Remark = "Remark-2",
                        Uom = "PCS"
                    }
                }
            },
            new PurchaseRequest()
            {
                Id = 2,
                Active = true,
                BudgetId = "BudgetId-2",
                BudgetCode = "BudgetCode-2",
                BudgetName = "BudgetName-2",
                CategoryId = "CategoryId-2",
                CategoryCode = "CategoryCode-2",
                CategoryName = "CategoryName-2",
                CreatedAgent = "Dummy-2",
                CreatedBy = "Dummy-2",
                CreatedUtc = DateTime.UtcNow,
                Date = DateTimeOffset.UtcNow,
                DeletedAgent = "",
                DeletedBy = "",
                DivisionId = "DivisionId-2",
                DivisionCode = "DivisionCode-2",
                DivisionName = "DivisionName-2",
                ExpectedDeliveryDate = DateTimeOffset.UtcNow,
                Internal = true,
                IsDeleted = false,
                IsPosted = false,
                IsUsed = false,
                LastModifiedAgent = "Dummy-2",
                LastModifiedBy = "Dummy-2",
                LastModifiedUtc = DateTime.UtcNow,
                No = "No-2",
                Remark = "Remark-2",
                Status = Enums.PurchaseRequestStatus.CREATED,
                UId = "8ad231fk1049201daf32",
                UnitId = "UnitId-2",
                UnitCode = "UnitCode-2",
                UnitName = "UnitName-2",
                Items = new List<PurchaseRequestItem>()
                {
                    new PurchaseRequestItem()
                    {
                        Id = 3,
                        Active = true,
                        IsDeleted = false,
                        PurchaseRequestId = 2,
                        CreatedAgent = "Dummy-3",
                        CreatedBy = "Dummy-3",
                        CreatedUtc = DateTime.UtcNow,
                        LastModifiedAgent = "Dummy-3",
                        LastModifiedBy = "Dummy-3",
                        LastModifiedUtc = DateTime.UtcNow,
                        DeletedAgent = "",
                        DeletedBy = "",
                        ProductId = "ProductId-3",
                        ProductCode = "ProductCode-3",
                        ProductName = "ProductName-3",
                        Quantity = 10,
                        Remark = "Remark-3",
                        Uom = "BUAH"
                    },
                    new PurchaseRequestItem()
                    {
                        Id = 4,
                        Active = true,
                        IsDeleted = false,
                        PurchaseRequestId = 2,
                        CreatedAgent = "Dummy-4",
                        CreatedBy = "Dummy-4",
                        CreatedUtc = DateTime.UtcNow,
                        LastModifiedAgent = "Dummy-4",
                        LastModifiedBy = "Dummy-4",
                        LastModifiedUtc = DateTime.UtcNow,
                        DeletedAgent = "",
                        DeletedBy = "",
                        ProductId = "ProductId-4",
                        ProductCode = "ProductCode-4",
                        ProductName = "ProductName-4",
                        Quantity = 10,
                        Remark = "Remark-4",
                        Uom = "P"
                    }
                }
            }
        };
        #endregion

        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<PurchaseRequest> dbSet;

        public PurchaseRequestFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<PurchaseRequest>();
        }

        //public List<PurchaseRequestViewModel> Read()
        //{
        //    return mapper.Map<List<PurchaseRequestViewModel>>(DUMMY_DATA);
        //}

        public Tuple<List<PurchaseRequest>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PurchaseRequest> Query = this.dbSet;

            Query = Query.Select(s => new PurchaseRequest
            {
                Id = s.Id,
                UId = s.UId,
                No = s.No,
                Date = s.Date,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                UnitName = s.UnitName,
                DivisionName = s.DivisionName,
                CategoryName = s.CategoryName,
                IsPosted = s.IsPosted,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc
            });

            List<string> searchAttributes = new List<string>()
            {
                "No", "UnitName", "CategoryName", "DivisionName"
            };

            Query = QueryHelper<PurchaseRequest>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchaseRequest>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<PurchaseRequest>.ConfigureOrder(Query, OrderDictionary);

            Pageable<PurchaseRequest> pageable = new Pageable<PurchaseRequest>(Query, Page - 1, Size);
            List<PurchaseRequest> Data = pageable.Data.ToList<PurchaseRequest>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public PurchaseRequest ReadById(int id)
        {
            var a = this.dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        //public int Create(PurchaseRequest m)
        //{
        //    int Result = 0;

        //    /* TODO EF Operation */

        //    Result = 1;

        //    return Result;
        //}

        public async Task<int> Create(PurchaseRequest m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    m.No = await GenerateNo(m, clientTimeZoneOffset);

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                        item.Status = "Belum diterima Pembelian";
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

        public async Task<int> Update(int id, PurchaseRequest purchaseRequest, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet.AsNoTracking()
                        .Include(d => d.Items)
                        .Single(pr => pr.Id == id && !pr.IsDeleted);

                    if (m != null && id == purchaseRequest.Id)
                    {

                        EntityExtension.FlagForUpdate(purchaseRequest, user, USER_AGENT);

                        foreach (var item in purchaseRequest.Items)
                        {
                            if (item.Id == 0)
                            {
                                EntityExtension.FlagForCreate(item, user, USER_AGENT);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                            }
                        }

                        this.dbContext.Update(purchaseRequest);

                        foreach (var item in m.Items)
                        {
                            PurchaseRequestItem purchaseRequestItem = purchaseRequest.Items.FirstOrDefault(i => i.Id.Equals(item.Id));
                            if (purchaseRequestItem == null)
                            {
                                EntityExtension.FlagForDelete(item, user, USER_AGENT);
                                this.dbContext.PurchaseRequestItems.Update(item);
                            }
                        }

                        Updated = await dbContext.SaveChangesAsync();
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
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                    EntityExtension.FlagForDelete(m, user, USER_AGENT);

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForDelete(item, user, USER_AGENT);
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

        public int PRPost(List<PurchaseRequest> ListPurchaseRequest, string user)
        {
            int Updated = 0;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var Ids = ListPurchaseRequest.Select(d => d.Id).ToList();
                    var listData = this.dbSet
                        .Where(m => Ids.Contains(m.Id) && !m.IsDeleted)
                        .Include(d => d.Items)
                        .ToList();
                    listData.ForEach(m =>
                    {
                        EntityExtension.FlagForUpdate(m, user, USER_AGENT);
                        m.IsPosted = true;

                        foreach (var item in m.Items)
                        {
                            EntityExtension.FlagForUpdate(item, user, USER_AGENT);
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

        public int PRUnpost(int id, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet
                        .Include(d => d.Items)
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                    EntityExtension.FlagForUpdate(m, user, USER_AGENT);
                    m.IsPosted = false;

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForUpdate(item, user, USER_AGENT);
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

        async Task<string> GenerateNo(PurchaseRequest model, int clientTimeZoneOffset)
        {
            string Year = model.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");

            string no = $"PR-{model.BudgetCode}-{model.UnitCode}-{model.CategoryCode}-{Year}-{Month}-";
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.No.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.No).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.No.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public Tuple<List<PurchaseRequest>, int, Dictionary<string, string>> ReadModelPosted(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<PurchaseRequest> Query = this.dbSet;

            Query = Query.Select(s => new PurchaseRequest
            {
                Id = s.Id,
                UId = s.UId,
                No = s.No,
                Date = s.Date,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                UnitName = s.UnitName,
                UnitId = s.UnitId,
                UnitCode = s.UnitCode,
                BudgetCode = s.BudgetCode,
                BudgetId = s.BudgetId,
                BudgetName = s.BudgetName,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                CategoryCode = s.CategoryCode,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                IsPosted = s.IsPosted,
                Remark = s.Remark,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Items = s.Items
                    .Select(
                        q => new PurchaseRequestItem
                        {
                            Id=q.Id,
                            PurchaseRequestId = q.PurchaseRequestId,
                            ProductId = q.ProductId,
                            ProductCode = q.ProductCode,
                            ProductName = q.ProductName,
                            Uom = q.Uom,
                            UomId = q.UomId,
                            Status = q.Status,
                            Quantity = q.Quantity,
                            Remark = q.Remark
                        })
                    .Where(j => j.PurchaseRequestId.Equals(s.Id))
                    .ToList()
            }).Where(s => s.IsPosted == true && this.dbContext.InternalPurchaseOrders.Count(m=> m.PRNo.Equals(s.No))<=0);

            List<string> searchAttributes = new List<string>()
            {
                "No", "UnitName", "CategoryName", "DivisionName"
            };

            Query = QueryHelper<PurchaseRequest>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<PurchaseRequest>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<PurchaseRequest>.ConfigureOrder(Query, OrderDictionary);

            Pageable<PurchaseRequest> pageable = new Pageable<PurchaseRequest>(Query, Page - 1, Size);
            List<PurchaseRequest> Data = pageable.Data.ToList<PurchaseRequest>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        #region Monitoring By User
        public IQueryable<PurchaseRequestReportViewModel> GetReportQuery(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, string productId, DateTime? dateFrom, DateTime? dateTo, int offset, string username)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            
            var Query = (from a in dbContext.PurchaseRequests
                         join b in dbContext.PurchaseRequestItems on a.Id equals b.PurchaseRequestId
                         //PO
                         join c in dbContext.InternalPurchaseOrderItems on b.Id equals c.PRItemId into d
                         from poItem in d.DefaultIfEmpty()
                         join i in dbContext.InternalPurchaseOrders on poItem.POId equals i.Id into j
                         from po in j.DefaultIfEmpty()
                         //EPO
                         join e in dbContext.ExternalPurchaseOrderItems on poItem.POId equals e.POId into f
                         from epoItem in f.DefaultIfEmpty()
                         join g in dbContext.ExternalPurchaseOrders on epoItem.EPOId equals g.Id into h
                         from epo in h.DefaultIfEmpty()
                         join k in dbContext.ExternalPurchaseOrderDetails on poItem.Id equals k.POItemId into l
                         from epoDetail in l.DefaultIfEmpty()
                             //Conditions
                         where a.IsDeleted == false
                             && b.IsDeleted==false
                             && poItem.IsDeleted == false
                             && po.IsDeleted == false
                             && epoItem.IsDeleted == false
                             && epo.IsDeleted == false
                             && epoDetail.IsDeleted==false
                             && a.CreatedBy== (string.IsNullOrWhiteSpace(username) ? a.CreatedBy : username)
                             && poItem.Quantity!=0
                             && a.No == (string.IsNullOrWhiteSpace(no) ? a.No : no)
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && a.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? a.CategoryId : categoryId)
                             && a.BudgetId == (string.IsNullOrWhiteSpace(budgetId) ? a.BudgetId : budgetId)
                             && b.Status == (string.IsNullOrWhiteSpace(prStatus) ? b.Status : prStatus)
                             && poItem.Status == (string.IsNullOrWhiteSpace(poStatus) ? poItem.Status : poStatus)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                             && b.ProductId == (string.IsNullOrWhiteSpace(productId) ? b.ProductId : productId)
                         select new PurchaseRequestReportViewModel
                         {
                             no=a.No,
                             budget=a.BudgetName,
                             category=a.CategoryName,
                             unit= a.DivisionName +" - " + a.UnitName,
                             date=a.Date,
                             expectedDeliveryDatePR=a.ExpectedDeliveryDate == null ? new DateTime(1970, 1, 1) : a.ExpectedDeliveryDate,
                             expectedDeliveryDatePO=epo.DeliveryDate==null ? new DateTime(1970, 1, 1) : epo.DeliveryDate,
                             poStatus=poItem.Status,
                             prStatus=b.Status,
                             productCode=b.ProductCode,
                             productName=b.ProductName,
                             uom=b.Uom,
                             quantity=b.Quantity,
                             dealQuantity=epoDetail==null ?0: epoDetail.DealQuantity,
                             dealUom= epoDetail == null? "-":epoDetail.DealUomUnit,
                             LastModifiedUtc=b.LastModifiedUtc
                         });
            return Query;
        }

        public Tuple<List<PurchaseRequestReportViewModel>, int> GetReport(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, string productId, DateTime? dateFrom, DateTime? dateTo , int page, int size, string Order,int offset, string username)
        {
            var Query = GetReportQuery(no, unitId, categoryId, budgetId, prStatus, poStatus, productId, dateFrom, dateTo, offset, username);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<PurchaseRequestReportViewModel> pageable = new Pageable<PurchaseRequestReportViewModel>(Query, page - 1, size);
            List<PurchaseRequestReportViewModel> Data = pageable.Data.ToList<PurchaseRequestReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, string productId, DateTime? dateFrom, DateTime? dateTo, int offset, string username)
        {
            var Query = GetReportQuery(no, unitId, categoryId, budgetId, prStatus, poStatus, productId, dateFrom, dateTo, offset, username);
            Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Diminta", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Diminta", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Deal PO Eksternal", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Deal PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status Barang", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "","","", "", "", "", "", 0,"","","", 0, "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.date == null ? "-" : item.date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string prDate = item.expectedDeliveryDatePR == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePR.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string epoDate = item.expectedDeliveryDatePO == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePO.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.unit, item.budget, item.category, date, (item.no), item.productCode, item.productName, item.quantity, item.uom, prDate, epoDate,item.dealQuantity,item.dealUom, item.prStatus,item.poStatus);
                }
            }
                
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

        #region Duration PR-POInt
        public IQueryable<PurchaseRequestPurchaseOrderDurationReportViewModel> GetPRDurationReportQuery(string unit, string duration,  DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
			int start = 0;
			int end = 0;
			if (duration == "8-14 hari")
			{
				start = 8;
				end = 14;
			}
			else if (duration == "15-30 hari")
			{
				start = 15;
				end = 30;
			}
			else
			{
				start = 31;
				end = 1000;
			}
			List<PurchaseRequestPurchaseOrderDurationReportViewModel> listPRDUration = new List<PurchaseRequestPurchaseOrderDurationReportViewModel>();
			var Query = (from a in dbContext.PurchaseRequests
						 join b in dbContext.PurchaseRequestItems on a.Id equals b.PurchaseRequestId
						 join c in dbContext.InternalPurchaseOrders on a.No equals c.PRNo
						 join d in dbContext.InternalPurchaseOrderItems on c.Id equals d.POId 
						 //Conditions
						 where d.Quantity>0 && a.IsDeleted == false  && b.Id == d.PRItemId && b.IsDeleted== false && c.IsDeleted==false && d.IsDeleted == false
						 &&  a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit)
						  && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
						  && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
						 select new PurchaseRequestPurchaseOrderDurationReportViewModel
						 {
							 prNo = a.No,
							 prDate = a.Date,
							 prCreatedDate = a.CreatedUtc,
							 unit = a.UnitName,
							 category = a.CategoryName,
							 productUom = b.Uom,
							 poDate = c.CreatedUtc,
							 productCode = b.ProductCode,
							 productName = b.ProductName,
							 productQuantity = d.Quantity,
							 staff = c.CreatedBy,
							 division = a.DivisionName,
							 budget = a.BudgetName
						

		});
			foreach (var item in Query)
			{
				var poDate = new DateTimeOffset(item.poDate.Date, TimeSpan.Zero); 
				var prCreatedDate = new DateTimeOffset(item.prCreatedDate.Date, TimeSpan.Zero); 
				 
				var datediff = ((TimeSpan)( poDate - prCreatedDate)).Days;
				PurchaseRequestPurchaseOrderDurationReportViewModel _new = new PurchaseRequestPurchaseOrderDurationReportViewModel
				{
					prNo = item.prNo,
					prDate = item.prDate,
					prCreatedDate = item.prCreatedDate,
					unit = item.unit,
					category = item.category,
					productUom = item.productUom,
					poDate = item.poDate,
					productCode = item.productCode,
					productName = item.productName,
					productQuantity = item.productQuantity,
					dateDiff = datediff,
					staff = item.staff,
					division = item.division,
					budget = item.budget,
				};
				listPRDUration.Add(_new);
			}
			return listPRDUration.Where(s=>s.dateDiff >=start && s.dateDiff <=end ).AsQueryable();
		 
		}
 

		public Tuple<List<PurchaseRequestPurchaseOrderDurationReportViewModel>, int> GetPRDurationReport(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
		{
			var Query = GetPRDurationReportQuery(unit,duration, dateFrom, dateTo, offset);

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			if (OrderDictionary.Count.Equals(0))
			{
				Query = Query.OrderByDescending(b => b.prCreatedDate);
			}
			else
			{
				string Key = OrderDictionary.Keys.First();
				string OrderType = OrderDictionary[Key];

				Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
			}

			Pageable<PurchaseRequestPurchaseOrderDurationReportViewModel> pageable = new Pageable<PurchaseRequestPurchaseOrderDurationReportViewModel>(Query, page - 1, size);
			List<PurchaseRequestPurchaseOrderDurationReportViewModel> Data = pageable.Data.ToList<PurchaseRequestPurchaseOrderDurationReportViewModel>();
			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData);
		}
		public MemoryStream GenerateExcelPRDuration(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			var Query = GetPRDurationReportQuery(unit, duration, dateFrom, dateTo, offset);
			Query = Query.OrderByDescending(b => b.prCreatedDate);
			DataTable result = new DataTable();
			//No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


			result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Buat Purchase Request", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Nomor PR", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima PO Internal", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Selisih Tanggal PR - PO Internal (hari)", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Nama Staff Pembelian", DataType = typeof(string) });
		
			if (Query.ToArray().Count() == 0)
				result.Rows.Add("","", "", "", "", "", "", "", "", "", "", "", "", "",""); // to allow column name to be generated properly for empty data as template
			else
			{
				int index = 0;
				foreach (var item in Query)
				{
					index++;
					string prDate = item.prDate == null ? "-" : item.prDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
					string prCreatedDate = item.prCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.prCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
					string poDate = item.poDate == new DateTime(1970, 1, 1) ? "-" : item.poDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
					result.Rows.Add(index,prDate,prCreatedDate,item.prNo,item.division, item.unit, item.budget, item.category, item.productCode, (item.productName),item.productQuantity, item.productUom, poDate, item.dateDiff, item.staff);
				}
			}

			return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
		}
        #endregion

        #region Duration PR-POEks
        public IQueryable<PurchaseRequestPurchaseOrderDurationReportViewModel> GetPREPODurationReportQuery(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            int start = 0;
            int end = 0;
            if (duration == "8-14 hari")
            {
                start = 8;
                end = 14;
            }
            else if (duration == "15-30 hari")
            {
                start = 15;
                end = 30;
            }
            else
            {
                start = 31;
                end = 1000;
            }
            List<PurchaseRequestPurchaseOrderDurationReportViewModel> listPRDUration = new List<PurchaseRequestPurchaseOrderDurationReportViewModel>();
            var Query = (from a in dbContext.PurchaseRequests
                         join b in dbContext.PurchaseRequestItems on a.Id equals b.PurchaseRequestId
                         join c in dbContext.ExternalPurchaseOrderItems on a.Id equals c.PRId
                         join d in dbContext.ExternalPurchaseOrders on c.EPOId equals d.Id
                         join e in dbContext.ExternalPurchaseOrderDetails on c.Id equals e.EPOItemId
                         join h in dbContext.InternalPurchaseOrders on c.POId equals h.Id into i
                         from po in i.DefaultIfEmpty()
                         join f in dbContext.InternalPurchaseOrderItems on po.Id equals f.POId into g
                         from poItem in g.DefaultIfEmpty()
                             //Conditions
                         where a.IsDeleted == false 
                            && b.Id == e.PRItemId 
                            && b.IsDeleted == false 
                            && c.IsDeleted == false 
                            && d.IsDeleted == false 
                            && e.IsDeleted == false
                            && a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit)
                            && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                            && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new PurchaseRequestPurchaseOrderDurationReportViewModel
                         {
                             prNo = a.No,
                             prDate = a.Date,
                             prCreatedDate = a.CreatedUtc,
                             unit = a.UnitName,
                             category = a.CategoryName,
                             division = a.DivisionName,
                             budget = a.BudgetName,
                             productCode = e.ProductCode,
                             productName = e.ProductName,
                             dealQuantity = e.DealQuantity,
                             dealUomUnit = e.DealUomUnit,
                             pricePerDealUnit = e.PricePerDealUnit,
                             supplierCode = d.SupplierCode,
                             supplierName = d.SupplierName,
                             poDate = po.CreatedUtc,
                             orderDate = d.OrderDate,
                             ePOCreatedDate = d.CreatedUtc,
                             deliveryDate = d.DeliveryDate,
                             ePONo = d.EPONo,
                             staff = d.CreatedBy,
                         }).Distinct();
            foreach (var item in Query)
            {
                var ePODate = new DateTimeOffset(item.ePOCreatedDate.Date, TimeSpan.Zero);
                var prCreatedDate = new DateTimeOffset(item.prCreatedDate.Date, TimeSpan.Zero);

                var datediff = ((TimeSpan)(ePODate - prCreatedDate)).Days;
                PurchaseRequestPurchaseOrderDurationReportViewModel _new = new PurchaseRequestPurchaseOrderDurationReportViewModel
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
                    dateDiff = datediff,
                    staff = item.staff,
                };
                listPRDUration.Add(_new);
            }
            return listPRDUration.Where(s => s.dateDiff >= start && s.dateDiff <= end).AsQueryable();

        }


        public Tuple<List<PurchaseRequestPurchaseOrderDurationReportViewModel>, int> GetPREPODurationReport(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetPREPODurationReportQuery(unit, duration, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.prCreatedDate);
            }
            //else
            //{
            //    string Key = OrderDictionary.Keys.First();
            //    string OrderType = OrderDictionary[Key];

            //    Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            //}

            Pageable<PurchaseRequestPurchaseOrderDurationReportViewModel> pageable = new Pageable<PurchaseRequestPurchaseOrderDurationReportViewModel>(Query, page - 1, size);
            List<PurchaseRequestPurchaseOrderDurationReportViewModel> Data = pageable.Data.ToList<PurchaseRequestPurchaseOrderDurationReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcelPREPODuration(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetPREPODurationReportQuery(unit, duration, dateFrom, dateTo, offset);
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
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih Tanggal PR - PO Eksternal (hari)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Staff Pembelian", DataType = typeof(string) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
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
                    result.Rows.Add(index, prDate, prCreatedDate, item.prNo, item.division, item.unit, item.budget, item.category, item.productCode, item.productName, item.dealQuantity, item.dealUomUnit, item.pricePerDealUnit, item.supplierCode, item.supplierName, poDate, orderDate, ePOCreatedDate, deliveryDate, item.ePONo, item.dateDiff, item.staff);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion
    }
}