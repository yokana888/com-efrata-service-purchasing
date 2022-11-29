using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Expedition
{
    public class UnitPaymentOrderUnpaidReportFacade : IUnitPaymentOrderUnpaidReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<PurchasingDocumentExpedition> dbSet;

        IMongoCollection<BsonDocument> collectionUPO;
        IMongoCollection<BsonDocument> collectionURN;

        public UnitPaymentOrderUnpaidReportFacade(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<PurchasingDocumentExpedition>();

            MongoDbContext mongoDbContext = new MongoDbContext();
            this.collectionUPO = mongoDbContext.UnitPaymentOrder;
            this.collectionURN = mongoDbContext.UnitReceiptNote;
        }

        public async Task<List<UnitPaymentOrderUnpaidViewModel>> GetReportMongo(string no, string supplierCode, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            string query = "{'$and' : [{_deleted : false}";
            if (!string.IsNullOrWhiteSpace(no))
            {
                query += ",{ no : '" + no + "'}";
            }
            if (!string.IsNullOrWhiteSpace(supplierCode))
            {
                query += ",{ 'supplier.code' : '" + supplierCode + "'}";
            }
            if (dateFrom != null && dateTo != null)
            {
                query += ",{'$and' : [{ dueDate : {'$gte' : new Date('" + dateFrom.ToString() + "')}}, " + "{dueDate : {'$lte' : new Date('" + dateTo.ToString() + "')}}]}";
            }
            query += "]}";
            
            var bsonDataUPO = collectionUPO.Find(query).Project(Builders<BsonDocument>.Projection
                .Include("no").Include("date").Include("currency").Include("supplier").Include("invoceNo").Include("dueDate").Include("items")).ToList();
            var listURNId = await bsonDataUPO.SelectMany(m => m["items"].AsBsonArray.AsQueryable().Select(n => n["unitReceiptNoteId"].AsBsonValue.AsObjectId.ToString())).ToDynamicListAsync();
            var aggregateURNId = listURNId.Aggregate((i, j) => i + "'),ObjectId('" + j);
            string queryURN = "{ _id: { $in: [ ObjectId('" + aggregateURNId + "')]}}";
            var bsonDataURN = await collectionURN.Find(queryURN).Project(Builders<BsonDocument>.Projection
                .Include("_id").Include("unit").Include("items")).ToListAsync();

            List<UnitPaymentOrderUnpaidViewModel> listData = new List<UnitPaymentOrderUnpaidViewModel>();
            foreach ( var data in bsonDataUPO)
            {
                var unitReceiptNoteIds = data["items"].AsBsonArray.AsQueryable().Select(m => m["unitReceiptNoteId"].AsBsonValue.AsObjectId).ToList();
                var unitReceiptNotes = bsonDataURN.Where(m => unitReceiptNoteIds.Contains(m["_id"].AsObjectId)).ToList();
                foreach (var unitReceiptNote in unitReceiptNotes)
                {
                    var itemsCount = unitReceiptNote["items"].AsBsonArray.Count;
                    var product = unitReceiptNote["items"].AsBsonArray.AsQueryable().Select(x => x["product"].AsBsonValue["name"].ToString()).ToList();
                    var qty = unitReceiptNote["items"].AsBsonArray.AsQueryable().Select(x => x["deliveredQuantity"].ToDouble()).ToList();
                    for (int i = 0; i < itemsCount; i++)
                    {
                        listData.Add(new UnitPaymentOrderUnpaidViewModel
                        {
                            UnitPaymentOrderNo = data["no"].ToString(),
                            UPODate = data["date"].ToUniversalTime(),
                            Currency = data["currency"].AsBsonValue["code"].ToString(),
                            SupplierName = data["supplier"].AsBsonValue["name"].ToString(),
                            InvoiceNo = data["invoceNo"].ToString(),
                            DueDate = data.Contains("dueDate") ? data["dueDate"].ToUniversalTime() : DateTimeOffset.MinValue,
                            ProductName = product[i],
                            Quantity = qty[i],
                            UnitName = unitReceiptNote["unit"].AsBsonValue["name"].ToString(),
                        });
                    }       
                    
                }
            }
            return listData;
        }

        public IQueryable<UnitPaymentOrderUnpaidViewModel> GetPurchasingDocumentExpedition(int Size, int Page, string UnitPaymentOrderNo, string SupplierCode, DateTimeOffset DateFrom, DateTimeOffset DateTo)
        {
            IQueryable<PurchasingDocumentExpedition> Query = this.dbContext.PurchasingDocumentExpeditions;

            Query = Query
                   .Where(p => p.IsDeleted == false &&
                          p.UnitPaymentOrderNo == (UnitPaymentOrderNo == null ? p.UnitPaymentOrderNo : UnitPaymentOrderNo) &&
                          p.SupplierCode == (SupplierCode == null ? p.SupplierCode : SupplierCode) &&
                          p.DueDate.Date >= DateFrom &&
                          p.DueDate.Date <= DateTo
                   );

            Query = Query
                .Select(s => new PurchasingDocumentExpedition
                {
                    Id = s.Id,
                    UnitPaymentOrderNo = s.UnitPaymentOrderNo,
                    UPODate = s.UPODate,
                    DueDate = s.DueDate,
                    InvoiceNo = s.InvoiceNo,
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    IsPaid = s.IsPaid,
                    IsPaidPPH = s.IsPaidPPH,
                    TotalPaid = s.TotalPaid,
                    IncomeTax = s.IncomeTax,
                    Vat = s.Vat,
                    Currency = s.Currency,
                    LastModifiedUtc = s.LastModifiedUtc
                });

            var list = (from datum in Query
                        orderby datum.LastModifiedUtc descending
                        select new UnitPaymentOrderUnpaidViewModel
                        {
                            UnitPaymentOrderNo = datum.UnitPaymentOrderNo,
                            UPODate = datum.UPODate,
                            InvoiceNo = datum.InvoiceNo,
                            SupplierName = datum.SupplierName,
                            Currency = datum.Currency,
                            IncomeTax = datum.IncomeTax,
                            DueDate = datum.DueDate,
                            DPPVat = datum.TotalPaid,
                            TotalPaid = datum.TotalPaid + datum.Vat
                        });
            return list;
        }

        public async Task<ReadResponse<object>> GetReport(int Size, int Page, string Order, string UnitPaymentOrderNo, string SupplierCode, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Offset)
        {
            if (!DateFrom.HasValue && !DateTo.HasValue)
            {
                DateFrom = DateTimeOffset.Now.AddHours(Offset).AddMonths(-1);
                DateTo = DateTimeOffset.Now.AddHours(Offset);
            }
            else if (!DateFrom.HasValue && DateTo.HasValue)
            {
                DateTo = DateTo.Value.AddHours(Offset);
                DateFrom = DateTo.Value.AddMonths(-1);
            }
            else if (DateFrom.HasValue && !DateTo.HasValue)
            {
                DateFrom = DateFrom.Value.AddHours(Offset);
                DateTo = DateFrom.Value.AddMonths(1);
            }
            else
            {
                DateFrom = DateFrom.Value.AddHours(Offset);
                DateTo = DateTo.Value.AddHours(Offset);
            }
            var listData = GetPurchasingDocumentExpedition(Size,Page,UnitPaymentOrderNo, SupplierCode, DateFrom.Value, DateTo.Value);

            List<UnitPaymentOrderUnpaidViewModel> dataMongo = await GetReportMongo(UnitPaymentOrderNo, SupplierCode, DateFrom.Value, DateTo.Value);
            
            IQueryable<UnitPaymentOrderUnpaidViewModel> resultQuery = 
                (from a in dataMongo
                join b in listData on a.UnitPaymentOrderNo equals b.UnitPaymentOrderNo into ab
                from b in ab.DefaultIfEmpty()
                select new UnitPaymentOrderUnpaidViewModel
                {
                    UnitPaymentOrderNo = a.UnitPaymentOrderNo,
                    UPODate = a.UPODate,
                    InvoiceNo = a.InvoiceNo,
                    SupplierName = a.SupplierName,
                    Currency = a.Currency,
                    IncomeTax = b == null ? 0 : b.IncomeTax,
                    DPPVat = b == null ? 0 : b.DPPVat,
                    DueDate = a.DueDate,
                    ProductName = a.ProductName,
                    Quantity = a.Quantity,
                    UnitName = a.UnitName,
                    TotalPaid = 0
                }).AsQueryable();

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count > 0)
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                string orderField = string.Concat(Key.Replace(".", ""), " ", OrderType);
                resultQuery = resultQuery.OrderBy(orderField);
            }

            Pageable<UnitPaymentOrderUnpaidViewModel> pageable = new Pageable<UnitPaymentOrderUnpaidViewModel>(resultQuery, Page - 1, Size);
            List<UnitPaymentOrderUnpaidViewModel> Data = await pageable.Data.ToDynamicListAsync<UnitPaymentOrderUnpaidViewModel>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<object>(Data.ToList<object>(), TotalData, OrderDictionary);
        }

        public void InsertToMongoUPO(BsonDocument document)
        {
            collectionUPO.InsertOne(document);
        }
        public void DeleteDataMongoUPO(string query)
        {
            collectionUPO.DeleteOne(query);
        }
        public void InsertToMongoURN(BsonDocument document)
        {
            collectionURN.InsertOne(document);
        }
        public void DeleteDataMongoURN(string query)
        {
            collectionURN.DeleteOne(query);
        }
    }
}
