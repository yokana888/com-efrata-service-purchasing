using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
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
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades
{
    public class GarmentInternalPurchaseOrderFacade : IGarmentInternalPurchaseOrderFacade
    {
        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentInternalPurchaseOrder> dbSet;

        public GarmentInternalPurchaseOrderFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentInternalPurchaseOrder>();
        }

        public Tuple<List<GarmentInternalPurchaseOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentInternalPurchaseOrder> Query = this.dbSet.Include(x => x.Items);

            List<string> searchAttributes = new List<string>()
            {
                "PRNo", "RONo", "BuyerName", "Items.ProductName", "Items.UomUnit", "CreatedBy"
            };

            Query = QueryHelper<GarmentInternalPurchaseOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentInternalPurchaseOrder>.ConfigureFilter(Query, FilterDictionary);

            //Query = Query.Select(m => new GarmentInternalPurchaseOrder
            //{
            //    Id = m.Id,
            //    PRNo = m.PRNo,
            //    RONo = m.RONo,
            //    Article = m.Article,
            //    ShipmentDate = m.ShipmentDate,
            //    BuyerId = m.BuyerId,
            //    BuyerCode = m.BuyerCode,
            //    BuyerName = m.BuyerName,
            //    Items = m.Items.Select(i => new GarmentInternalPurchaseOrderItem
            //    {
            //        ProductId = i.ProductId,
            //        ProductCode = i.ProductCode,
            //        ProductName = i.ProductName,
            //        ProductRemark = i.ProductRemark,
            //        Quantity = i.Quantity,
            //        UomId = i.UomId,
            //        UomUnit = i.UomUnit
            //    }).ToList(),
            //    CreatedBy = m.CreatedBy,
            //    IsPosted = m.IsPosted,
            //    LastModifiedUtc = m.LastModifiedUtc
            //});

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count > 0 && OrderDictionary.Keys.First().Contains("."))
            {
                string Key = OrderDictionary.Keys.First();
                string SubKey = Key.Split(".")[1];
                string OrderType = OrderDictionary[Key];

                Query = Query
                    .Select(m => new
                    {
                        Data = m,
                        ProductName = m.Items.Select(i => i.ProductName).OrderBy(o => o).FirstOrDefault(),
                        Quantity = m.Items.Select(i => i.Quantity).OrderBy(o => o).FirstOrDefault(),
                        UomUnit = m.Items.Select(i => i.UomUnit).OrderBy(o => o).FirstOrDefault(),
                    })
                    .OrderBy(string.Concat(SubKey, " ", OrderType))
                    .Select(m => m.Data);
            }
            else
            {
                Query = QueryHelper<GarmentInternalPurchaseOrder>.ConfigureOrder(Query, OrderDictionary);
            }

            Pageable<GarmentInternalPurchaseOrder> pageable = new Pageable<GarmentInternalPurchaseOrder>(Query, Page - 1, Size);
            List<GarmentInternalPurchaseOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public GarmentInternalPurchaseOrder ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                .FirstOrDefault();
            return model;
        }

        public bool CheckDuplicate(GarmentInternalPurchaseOrder model)
        {
            var countPOIntByPRAndRefNo = dbSet.Count(m => m.PRNo == model.PRNo && m.Items.Any(i => i.PO_SerialNumber == model.Items.Single().PO_SerialNumber));
            return countPOIntByPRAndRefNo > 1;
        }

        public async Task<int> CreateMultiple(List<GarmentInternalPurchaseOrder> ListModel, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var model in ListModel)
                    {
                        EntityExtension.FlagForCreate(model, user, USER_AGENT);

                        do
                        {
                            model.PONo = CodeGenerator.Generate();
                        }
                        while (ListModel.Count(m => m.PONo == model.PONo) > 1 || dbSet.Any(m => m.PONo.Equals(model.PONo)));
                        model.IsPosted = false;
                        model.IsClosed = false;

                        foreach (var item in model.Items)
                        {
                            EntityExtension.FlagForCreate(item, user, USER_AGENT);

                            item.Status = "PO Internal belum diorder";
                            item.RemainingBudget = item.BudgetPrice * item.Quantity;

                            var garmentPurchaseRequestItem = dbContext.GarmentPurchaseRequestItems.Single(i => i.Id == item.GPRItemId);
                            garmentPurchaseRequestItem.IsUsed = true;
                            garmentPurchaseRequestItem.Status = "Sudah diterima Pembelian";
                            EntityExtension.FlagForUpdate(garmentPurchaseRequestItem, user, USER_AGENT);

                            var garmentPurchaseRequest = dbContext.GarmentPurchaseRequests.Include(m => m.Items).Single(i => i.Id == model.PRId);
                            garmentPurchaseRequest.IsUsed = garmentPurchaseRequest.Items.All(i => i.IsUsed == true);
                            EntityExtension.FlagForUpdate(garmentPurchaseRequest, user, USER_AGENT);
                        }

                        dbSet.Add(model);
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

        public async Task<int> Split(int id, GarmentInternalPurchaseOrder model, string user, int clientTimeZoneOffset = 7)
        {
            int Splited = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldModel = dbSet.SingleOrDefault(m => m.Id == id);

                    EntityExtension.FlagForUpdate(oldModel, user, USER_AGENT);
                    foreach (var oldItem in oldModel.Items)
                    {
                        EntityExtension.FlagForUpdate(oldItem, user, USER_AGENT);
                        var newQuantity = model.Items.Single(i => i.Id == oldItem.Id).Quantity;
                        oldItem.Quantity -= newQuantity;
                    }

                    model.Id = 0;
                    foreach (var item in model.Items)
                    {
                        item.Id = 0;
                    }

                    EntityExtension.FlagForCreate(model, user, USER_AGENT);
                    do
                    {
                        model.PONo = CodeGenerator.Generate();
                    }
                    while (dbSet.Any(m => m.PONo.Equals(model.PONo)));

                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);
                    }

                    dbSet.Add(model);

                    Splited = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Splited;
        }

        public async Task<int> Delete(int id, string username)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = this.dbSet
                        .Include(d => d.Items)
                        .SingleOrDefault(m => m.Id == id && !m.IsDeleted);

                    EntityExtension.FlagForDelete(model, username, USER_AGENT);
                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForDelete(item, username, USER_AGENT);

                        if (!CheckDuplicate(model))
                        {
                            var garmentPurchaseRequestItem = dbContext.GarmentPurchaseRequestItems.Single(i => i.Id == item.GPRItemId);
                            garmentPurchaseRequestItem.IsUsed = false;
                            garmentPurchaseRequestItem.Status = "Belum diterima Pembelian";
                            EntityExtension.FlagForUpdate(garmentPurchaseRequestItem, username, USER_AGENT);

                            var garmentPurchaseRequest = dbContext.GarmentPurchaseRequests.Single(i => i.Id == model.PRId);
                            garmentPurchaseRequest.IsUsed = false;
                            EntityExtension.FlagForUpdate(garmentPurchaseRequest, username, USER_AGENT);
                        }
                    }

                    Deleted = await dbContext.SaveChangesAsync();

                    await dbContext.SaveChangesAsync();
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

        public List<GarmentInternalPurchaseOrder> ReadByTags(string category, string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo, string username)
        {
            IQueryable<GarmentInternalPurchaseOrder> Models = this.dbSet.AsQueryable();

            if (shipmentDateFrom != DateTimeOffset.MinValue && shipmentDateTo != DateTimeOffset.MinValue)
            {
                Models = Models.Where(m => m.ShipmentDate.AddHours(7).Date >= shipmentDateFrom.Date && m.ShipmentDate.AddHours(7).Date <= shipmentDateTo.Date);
            }

            string[] stringKeywords = new string[4];

            if (tags != null)
            {
                List<string> Keywords = new List<string>();

                if (tags.Contains("#"))
                {
                    Keywords = tags.Split("#").ToList();
                    Keywords.RemoveAt(0);
                    Keywords = Keywords.Take(stringKeywords.Length).ToList();
                }
                else
                {
                    Keywords.Add(tags);
                }

                for (int n = 0; n < Keywords.Count; n++)
                {
                    stringKeywords[n] = Keywords[n].Trim().ToLower();
                }
            }
            string filterCategory = "";
            if (category.ToLower() == "fabric")
            {
                filterCategory = category.ToLower();
            }
            else
            {
                filterCategory = stringKeywords[2];
            }

            Models = Models
                .Where(m =>
                    (string.IsNullOrWhiteSpace(stringKeywords[0]) || m.UnitName.ToLower().Contains(stringKeywords[0])) &&
                    (string.IsNullOrWhiteSpace(stringKeywords[1]) || m.BuyerName.ToLower().Contains(stringKeywords[1])) &&
                    (string.IsNullOrWhiteSpace(stringKeywords[3]) || m.PRNo.ToLower().Contains(stringKeywords[3])) &&
                    //m.Items.Any(i => i.IsUsed == false) &&
                    m.IsPosted == false && m.CreatedBy==username
                    )
                .Select(m => new GarmentInternalPurchaseOrder
                {
                    Id = m.Id,
                    PONo = m.PONo,
                    PRDate = m.PRDate,
                    PRNo = m.PRNo,
                    RONo = m.RONo,
                    BuyerId = m.BuyerId,
                    BuyerCode = m.BuyerCode,
                    BuyerName = m.BuyerName,
                    Article = m.Article,
                    ExpectedDeliveryDate = m.ExpectedDeliveryDate,
                    ShipmentDate = m.ShipmentDate,
                    UnitId = m.UnitId,
                    UnitCode = m.UnitCode,
                    UnitName = m.UnitName,
                    PRId=m.PRId,
                    Items = m.Items
                        .Where(i =>
                                category.ToLower() == "fabric" ? i.CategoryName.ToLower().Contains("fabric") : ((string.IsNullOrWhiteSpace(stringKeywords[2]) || i.CategoryName.ToLower().Contains(stringKeywords[2])) && i.CategoryName.ToLower() != "fabric")
                            //(string.IsNullOrWhiteSpace(filterCategory) || i.CategoryName.ToLower().Contains(filterCategory) 
                            //|| string.IsNullOrWhiteSpace(stringKeywords[2]) || i.CategoryName.ToLower().Contains(stringKeywords[2])) 
                            )
                        .ToList()
                })
                .Where(m => m.Items.Count > 0);


            return Models.ToList();
        }
		public List<GarmentInternalPurchaseOrder> ReadName(string Keyword = null, string Filter = "{}")
		{
			//IQueryable<GarmentInternalPurchaseOrder> Query = this.dbSet;

			//List<string> searchAttributes = new List<string>()
			//{
			//	"CreatedBy",
			//};

			//Query = QueryHelper<GarmentInternalPurchaseOrder>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah

			var Query = this.dbSet
				.Where(x => !x.IsDeleted && x.CreatedBy.Contains(Keyword))
				.Select(s => new GarmentInternalPurchaseOrder
				{
					CreatedBy = s.CreatedBy
				}).Distinct();

			Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
			Query = QueryHelper<GarmentInternalPurchaseOrder>.ConfigureFilter(Query, FilterDictionary);

			return Query.ToList();
		}
		#region Duration InternalPO-POEks
		public IQueryable<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> GetIPOEPODurationReportQuery(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            int start = 0;
            int end = 0;

            if(duration == "0-7 hari")
            {
                start = 0;
                end = 7;
            }
            else if (duration == "8-14 hari")
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
            List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> listIPODUration = new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>();
            var Query = (from a in dbContext.GarmentInternalPurchaseOrders
                         join b in dbContext.GarmentInternalPurchaseOrderItems on a.Id equals b.GPOId
                         join c in dbContext.GarmentPurchaseRequestItems on b.GPRItemId equals c.Id
                         join d in dbContext.GarmentPurchaseRequests on c.GarmentPRId equals d.Id
                         join e in dbContext.GarmentExternalPurchaseOrderItems on a.Id equals e.POId
                         join f in dbContext.GarmentExternalPurchaseOrders on e.GarmentEPOId equals f.Id
                         //Conditions
                         where a.IsDeleted == false
                            && b.IsDeleted == false
                            && c.IsDeleted == false
                            && d.IsDeleted == false
                            && e.IsDeleted == false
                            && f.IsDeleted == false
                            && a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit)
                            && a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                            && a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel
                         {
                             roNo=a.RONo,
                             artikelNo=a.Article,
                             buyerName=a.BuyerName,
                             poIntNo=a.PONo,
                             poEksNo=f.EPONo,
                             planPO=b.PO_SerialNumber,
                             poEksCreatedDate=f.CreatedUtc,
                             poIntCreatedDate=a.CreatedUtc,
                             unit = a.UnitName,
                             category = b.CategoryName,
                             productCode = e.ProductCode,
                             productName = e.ProductName,
                             supplierCode = f.SupplierCode,
                             supplierName = f.SupplierName,
                             expectedDate=f.DeliveryDate,
                             productPrice=e.PricePerDealUnit,
                             productQuantity=e.DealQuantity,
                             productUom=e.DealUomUnit,
                             staff = f.CreatedBy,
                         }).Distinct();
            foreach (var item in Query)
            {
                var ePODate = new DateTimeOffset(item.poEksCreatedDate.Date, TimeSpan.Zero);
                var poCreatedDate = new DateTimeOffset(item.poIntCreatedDate.Date, TimeSpan.Zero);

                var datediff = ((TimeSpan)(ePODate - poCreatedDate)).Days;
                GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel _new = new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel
                {
                    roNo = item.roNo,
                    artikelNo = item.artikelNo,
                    buyerName = item.buyerName,
                    poIntNo = item.poIntNo,
                    poEksNo = item.poEksNo,
                    planPO = item.planPO,
                    poEksCreatedDate = item.poEksCreatedDate,
                    poIntCreatedDate = item.poIntCreatedDate,
                    unit = item.unit,
                    category = item.category,
                    productCode = item.productCode,
                    productName = item.productName,
                    supplierCode = item.supplierCode,
                    supplierName = item.supplierName,
                    expectedDate = item.expectedDate,
                    productPrice = item.productPrice,
                    productQuantity = item.productQuantity,
                    productUom = item.productUom,
                    dateDiff = datediff,
                    staff = item.staff,
                };
                listIPODUration.Add(_new);
            }
            return listIPODUration.Where(s => s.dateDiff >= start && s.dateDiff <= end).AsQueryable();
        }
           
        public Tuple<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>, int> GetIPOEPODurationReport(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetIPOEPODurationReportQuery(unit, duration, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.poIntCreatedDate);
            }
            //else
            //{
            //    string Key = OrderDictionary.Keys.First();
            //    string OrderType = OrderDictionary[Key];

            //    Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            //}

            Pageable<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> pageable = new Pageable<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>(Query, page - 1, size);
            List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> Data = pageable.Data.ToList<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public MemoryStream GenerateExcelIPOEPODuration(string unit, string duration, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetIPOEPODurationReportQuery(unit, duration, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.poIntCreatedDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Plan PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Article / Style", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Target Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih Tanggal PO Internal - PO Eksternal (hari)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Staff Pembelian", DataType = typeof(string) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string poDate = item.poIntCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.poIntCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string orderDate = item.expectedDate == new DateTime(1970, 1, 1) ? "-" : item.expectedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ePOCreatedDate = item.poEksCreatedDate == new DateTime(1970, 1, 1) ? "-" : item.poEksCreatedDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    
                    result.Rows.Add(index, item.poIntNo, item.roNo, item.planPO, item.artikelNo, item.buyerName, item.unit, item.category,item.productCode,
                        item.productName,item.productQuantity,item.productUom,item.productPrice, item.supplierCode,item.supplierName, poDate,ePOCreatedDate, orderDate, 
                        item.poEksNo, item.dateDiff, item.staff);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion
    }
}
