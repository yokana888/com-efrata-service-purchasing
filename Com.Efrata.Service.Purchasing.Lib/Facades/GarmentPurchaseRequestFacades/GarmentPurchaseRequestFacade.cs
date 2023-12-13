using System.Linq.Dynamic.Core;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentPurchaseRequestPDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using System.IO;
using System.Data;
using Com.Efrata.Service.Purchasing.Lib.Services;
using System.Globalization;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades
{
    public class GarmentPurchaseRequestFacade : IGarmentPurchaseRequestFacade
    {
        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentPurchaseRequest> dbSet;
        private readonly IServiceProvider serviceProvider;

        private readonly string GarmentPreSalesContractUri = "merchandiser/garment-pre-sales-contracts/";

        public GarmentPurchaseRequestFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentPurchaseRequest>();
        }

        public Tuple<List<GarmentPurchaseRequest>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentPurchaseRequest> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "PRType", "SCNo", "PRNo", "RONo", "BuyerCode", "BuyerName", "UnitName", "Article", "SectionName", "ApprovalPR"
            };

            Query = QueryHelper<GarmentPurchaseRequest>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentPurchaseRequest>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary != null && OrderDictionary.Count != 0)
            {
                var order = OrderDictionary.First();
                switch (order.Key)
                {
                    case "Date":
                        if (order.Value == "asc")
                        {
                            Query = Query.OrderBy(o => (o.PRType == "MASTER" || o.PRType == "SAMPLE") ? o.ValidatedMD2Date : o.Date);
                        }
                        else
                        {
                            Query = Query.OrderByDescending(o => (o.PRType == "MASTER" || o.PRType == "SAMPLE") ? o.ValidatedMD2Date : o.Date);
                        }
                        break;
                    case "Status":
                        if (order.Value == "asc")
                        {
                            Query = Query.OrderBy(o => (o.PRType == "MASTER" || o.PRType == "SAMPLE") ? (o.IsValidatedMD1 && o.IsValidatedMD2 && o.IsValidatedPurchasing) : true);
                        }
                        else
                        {
                            Query = Query.OrderByDescending(o => (o.PRType == "MASTER" || o.PRType == "SAMPLE") ? (o.IsValidatedMD1 && o.IsValidatedMD2 && o.IsValidatedPurchasing) : true);
                        }
                        break;
                    default:
                        Query = QueryHelper<GarmentPurchaseRequest>.ConfigureOrder(Query, OrderDictionary);
                        break;
                }
            }
            else
            {
                Query = QueryHelper<GarmentPurchaseRequest>.ConfigureOrder(Query, OrderDictionary);
            }

            Query = Query.Select(s => new GarmentPurchaseRequest
            {
                Id = s.Id,
                UId = s.UId,
                RONo = s.RONo,
                MDStaff = s.MDStaff,
                PRNo = s.PRNo,
                Article = s.Article,
                Date = s.Date,
                ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                ShipmentDate = s.ShipmentDate,
                BuyerId = s.BuyerId,
                BuyerCode = s.BuyerCode,
                BuyerName = s.BuyerName,
                UnitId = s.UnitId,
                UnitCode = s.UnitCode,
                UnitName = s.UnitName,
                IsPosted = s.IsPosted,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,

                PRType = s.PRType,
                SCId = s.SCId,
                SCNo = s.SCNo,

                IsValidated = s.IsValidated,
                IsValidatedMD1 = s.IsValidatedMD1,
                IsValidatedMD2 = s.IsValidatedMD2,
                IsValidatedPurchasing = s.IsValidatedPurchasing,

                ValidatedDate = s.ValidatedDate,
                ValidatedMD1Date = s.ValidatedMD1Date,
                ValidatedMD2Date = s.ValidatedMD2Date,
                ValidatedPurchasingDate = s.ValidatedPurchasingDate,
                SectionName=s.SectionName,
                ApprovalPR = s.ApprovalPR

            });

            Pageable<GarmentPurchaseRequest> pageable = new Pageable<GarmentPurchaseRequest>(Query, Page - 1, Size);
            List<GarmentPurchaseRequest> Data = pageable.Data.ToList<GarmentPurchaseRequest>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

		public ReadResponse<dynamic> ReadDynamic(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}", string Select = "{}", string Search = "[]")
		{
			IQueryable<GarmentPurchaseRequest> Query = this.dbSet;

			List<string> SearchAttributes = JsonConvert.DeserializeObject<List<string>>(Search);
			if (SearchAttributes.Count == 0)
			{
				SearchAttributes = new List<string>() { "PRType", "SCNo", "PRNo", "RONo", "BuyerCode", "BuyerName", "UnitName", "Article" };
			}
			Query = QueryHelper<GarmentPurchaseRequest>.ConfigureSearch(Query, SearchAttributes, Keyword, SearchWith: "StartsWith");

			Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
			Query = QueryHelper<GarmentPurchaseRequest>.ConfigureFilter(Query, FilterDictionary);

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			Query = QueryHelper<GarmentPurchaseRequest>.ConfigureOrder(Query, OrderDictionary);

			Dictionary<string, string> SelectDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Select);
			IQueryable SelectedQuery = Query;
			if (SelectDictionary.Count > 0)
			{
				SelectedQuery = QueryHelper<GarmentPurchaseRequest>.ConfigureSelect(Query, SelectDictionary);
			}

			int TotalData = SelectedQuery.Count();

			List<dynamic> Data = SelectedQuery
				.Skip((Page - 1) * Size)
				.Take(Size)
				.ToDynamicList();

			return new ReadResponse<dynamic>(Data, TotalData, OrderDictionary);
		}

		public GarmentPurchaseRequest ReadById(int id)
        {
            var a = this.dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        public GarmentPurchaseRequest ReadByRONo(string rono)
        {
            var a = this.dbSet.Where(p => p.RONo.Equals(rono))
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(GarmentPurchaseRequest m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    if (string.IsNullOrWhiteSpace(m.RONo))
                    {
                        m.RONo = GenerateRONo(m, clientTimeZoneOffset);
                    }

                    if (m.Items.Count(i => string.IsNullOrWhiteSpace(i.PO_SerialNumber)) > 0)
                    {
                        GeneratePOSerialNumber(m, clientTimeZoneOffset);
                    }

                    m.PRNo = $"PR{m.RONo}";

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                        item.Status = "Belum diterima Pembelian";
                    }

                    this.dbSet.Add(m);

                    Created = await dbContext.SaveChangesAsync();

                    if (m.PRType == "MASTER" || m.PRType == "SAMPLE")
                    {
                        await SetIsPR(m.SCId, true);
                    }

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

        private string GenerateRONo(GarmentPurchaseRequest m, int timeZone)
        {
            DateTimeOffset now = m.Date.ToOffset(new TimeSpan(timeZone, 0, 0));
            string y = now.ToString("yy");

            var unitCode = new List<string> { null, "EFR" }.IndexOf(m.UnitCode);
            if (unitCode < 1)
            {
                throw new Exception("UnitCode format is invalid when Generate RONo");
            }

            var prefix = string.Concat("EFR", y, unitCode);
            var padding = 5;
            var suffix = string.Empty;

            if (m.PRType == "MASTER")
            {
                suffix = "M";
            }
            else if (m.PRType == "SAMPLE")
            {
                suffix = "S";
            }
            else
            {
                throw new Exception("PRType only accepting \"MASTER\" and \"SAMPLE\" in order to generate RONo.");
            }

            var lastRONo = dbSet.Where(w => !string.IsNullOrWhiteSpace(w.RONo) && w.RONo.Length == prefix.Length + padding + suffix.Length && w.RONo.StartsWith(prefix) && w.RONo.EndsWith(suffix))
                .OrderByDescending(o => o.RONo)
                .Select(s => int.Parse(s.RONo.Substring(prefix.Length, padding)))
                .FirstOrDefault();

            var RONo = $"{prefix}{(lastRONo + 1).ToString($"D{padding}")}{suffix}";

            return RONo;
        }

        private void GeneratePOSerialNumber(GarmentPurchaseRequest m, int timeZone)
        {
            DateTimeOffset now = m.Date.ToOffset(new TimeSpan(timeZone, 0, 0));
            string y = now.ToString("yy");

            var unitCode = new List<string> { null, "EFR" }.IndexOf(m.UnitCode);
            if (unitCode < 1)
            {
                throw new Exception("UnitCode format is invalid when Generate POSerialnumber");
            }

            var prefix = string.Concat(y, unitCode);
            var padding = 6;
            var suffix = string.Empty;

            if (m.PRType == "MASTER")
            {
                suffix = "M";
            }
            else if (m.PRType == "SAMPLE")
            {
                suffix = "S";
            }
            else
            {
                throw new Exception("PRType only accepting \"MASTER\" and \"SAMPLE\" in order to generate POSerialNumber.");
            }

            var prefixPM = string.Concat("PM", prefix);
            var prefixPA = string.Concat("PA", prefix);
            int lasPM = 0, lasPA = 0;

            if (m.Items.Count(i => i.Id == 0 && i.CategoryName == "FABRIC") > 0)
            {
                lasPM = dbContext.GarmentPurchaseRequestItems
                    .Where(w => !string.IsNullOrWhiteSpace(w.PO_SerialNumber) && w.PO_SerialNumber.Length == prefixPM.Length + padding + suffix.Length && w.PO_SerialNumber.StartsWith(prefixPM) && w.PO_SerialNumber.EndsWith(suffix))
                    .OrderByDescending(o => o.PO_SerialNumber)
                    .Select(s => int.Parse(s.PO_SerialNumber.Substring(prefixPM.Length, padding)))
                    .FirstOrDefault();
            }

            if (m.Items.Count(i => i.Id == 0 && i.CategoryName != "FABRIC") > 0)
            {
                lasPA = dbContext.GarmentPurchaseRequestItems
                    .Where(w => !string.IsNullOrWhiteSpace(w.PO_SerialNumber) && w.PO_SerialNumber.Length == prefixPA.Length + padding + suffix.Length && w.PO_SerialNumber.StartsWith(prefixPA) && w.PO_SerialNumber.EndsWith(suffix))
                    .OrderByDescending(o => o.PO_SerialNumber)
                    .Select(s => int.Parse(s.PO_SerialNumber.Substring(prefixPA.Length, padding)))
                    .FirstOrDefault();
            }

            foreach (var item in m.Items.Where(i => i.Id == 0 && string.IsNullOrWhiteSpace(i.PO_SerialNumber)))
            {
                if (item.CategoryName == "FABRIC")
                {
                    item.PO_SerialNumber = $"{prefixPM}{(++lasPM).ToString($"D{padding}")}{suffix}";
                }
                else
                {
                    item.PO_SerialNumber = $"{prefixPA}{(++lasPA).ToString($"D{padding}")}{suffix}";
                }
            }
        }

        public async Task<int> Update(int id, GarmentPurchaseRequest m, string user, int clientTimeZoneOffset = 7)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldM = this.dbSet
                        .Include(d => d.Items)
                        .SingleOrDefault(pr => pr.Id == id);

                    if (oldM != null && oldM.Id == id)
                    {
                        EntityExtension.FlagForUpdate(oldM, user, USER_AGENT);

                        if (m.PRType == "MASTER" || m.PRType == "SAMPLE")
                        {
                            oldM.Article = m.Article;
                            oldM.Date = m.Date;
                            oldM.ShipmentDate = m.ShipmentDate;
                            oldM.Remark = m.Remark;
                        }
                        oldM.IsUsed = m.IsUsed;

                        foreach (var oldItem in oldM.Items)
                        {
                            var newItem = m.Items.FirstOrDefault(i => i.Id.Equals(oldItem.Id));
                            if (newItem == null)
                            {
                                EntityExtension.FlagForDelete(oldItem, user, USER_AGENT);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(oldItem, user, USER_AGENT);

                                oldItem.ProductRemark = newItem.ProductRemark;
                                oldItem.Quantity = newItem.Quantity;
                                oldItem.BudgetPrice = newItem.BudgetPrice;
                                oldItem.PriceUomId = newItem.PriceUomId;
                                oldItem.PriceUomUnit = newItem.PriceUomUnit;
                                oldItem.PriceConversion = newItem.PriceConversion;
								oldItem.UomId = newItem.UomId;
								oldItem.UomUnit = newItem.UomUnit;
                            }
                        }

                        if (m.Items.Count(i => i.Id == 0 && string.IsNullOrWhiteSpace(i.PO_SerialNumber)) > 0)
                        {
                            GeneratePOSerialNumber(m, clientTimeZoneOffset);
                        }

                        foreach (var item in m.Items.Where(i => i.Id == 0))
                        {
                            EntityExtension.FlagForCreate(item, user, USER_AGENT);
                            item.Status = "Belum diterima Pembelian";

                            oldM.Items.Add(item);
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

        public List<GarmentInternalPurchaseOrder> ReadByTags(string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo)
        {
            IQueryable<GarmentPurchaseRequest> Models = this.dbSet.AsQueryable();

            if (shipmentDateFrom != DateTimeOffset.MinValue && shipmentDateTo != DateTimeOffset.MinValue)
            {
                Models = Models.Where(m => m.ShipmentDate >= shipmentDateFrom && m.ShipmentDate <= shipmentDateTo);
            }

            string[] stringKeywords = new string[3];

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

            Models = Models
                .Where(m =>
                    (string.IsNullOrWhiteSpace(stringKeywords[0]) || m.UnitName.ToLower().Contains(stringKeywords[0])) &&
                    (string.IsNullOrWhiteSpace(stringKeywords[1]) || m.BuyerName.ToLower().Contains(stringKeywords[1])) &&
                    m.Items.Any(i => i.IsUsed == false) &&
                    m.IsUsed == false &&
                    m.IsValidated == true
                    )
                .Select(m => new GarmentPurchaseRequest
                {
                    Id = m.Id,
                    Date = m.Date,
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
                    Items = m.Items
                        .Where(i =>
                            i.IsUsed == false &&
                            (string.IsNullOrWhiteSpace(stringKeywords[2]) || i.CategoryName.ToLower().Contains(stringKeywords[2]))
                            )
                        .ToList(),
                })
                .Where(m => m.Items.Count() > 0);

            var IPOModels = new List<GarmentInternalPurchaseOrder>();

            foreach (var model in Models)
            {
                foreach (var item in model.Items)
                {
                    var IPOModel = new GarmentInternalPurchaseOrder
                    {
                        PRId = model.Id,
                        PRDate = model.Date,
                        PRNo = model.PRNo,
                        RONo = model.RONo,
                        BuyerId = model.BuyerId,
                        BuyerCode = model.BuyerCode,
                        BuyerName = model.BuyerName,
                        Article = model.Article,
                        ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                        ShipmentDate = model.ShipmentDate,
                        UnitId = model.UnitId,
                        UnitCode = model.UnitCode,
                        UnitName = model.UnitName,
                        //IsPosted = false,
                        //IsClosed = false,
                        //Remark = "",
                        Items = new List<GarmentInternalPurchaseOrderItem>
                        {
                            new GarmentInternalPurchaseOrderItem
                            {
                                GPRItemId = item.Id,
                                PO_SerialNumber = item.PO_SerialNumber,
                                ProductId = item.ProductId,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                Quantity = item.Quantity,
                                BudgetPrice = item.BudgetPrice,
                                UomId = item.UomId,
                                UomUnit = item.UomUnit,
                                CategoryId = item.CategoryId,
                                CategoryName = item.CategoryName,
                                ProductRemark = item.ProductRemark,
                                //Status = "PO Internal belum diorder"
                            }
                        }
                    };
                    IPOModels.Add(IPOModel);
                }
            }

            return IPOModels;
        }

        public async Task<int> Delete(int id, string user)
        {
            int Deleted = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet
                        .Include(m => m.Items)
                        .SingleOrDefault(m => m.Id == id);

                    EntityExtension.FlagForDelete(data, user, USER_AGENT);
                    foreach (var item in data.Items)
                    {
                        EntityExtension.FlagForDelete(item, user, USER_AGENT);
                    }

                    if (data.PRType == "MASTER" || data.PRType == "SAMPLE")
                    {
                        var countPreSCinOtherPR = dbSet.Count(w => w.Id != data.Id && w.SCId == data.SCId);
                        if (countPreSCinOtherPR == 0)
                        {
                            await SetIsPR(data.SCId, false);
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

        public async Task<int> PRPost(List<long> listId, string user)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var listData = dbSet.Include(i => i.Items)
                        .Where(w => listId.Contains(w.Id))
                        .ToList();

                    foreach (var data in listData)
                    {
                        EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                        data.IsPosted = true;

                        foreach (var item in data.Items)
                        {
                            EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                        }
                    }

                    Updated = await dbContext.SaveChangesAsync();

                    if (Updated < 1)
                    {
                        throw new Exception("No data updated");
                    }

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

        public async Task<int> PRUnpost(long id, string user)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Include(i => i.Items)
                        .Where(w => w.Id == id)
                        .Single();

                    EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                    data.IsPosted = false;

                    data.IsValidatedMD1 = false;
                    data.IsValidatedMD2 = false;
                    data.IsValidatedPurchasing = false;
                    data.IsValidated = false;
                    data.ValidatedMD1By = null;
                    data.ValidatedMD2By = null;
                    data.ValidatedPurchasingBy = null;
                    data.ValidatedBy = null;
                    data.ValidatedMD1Date = DateTimeOffset.MinValue;
                    data.ValidatedMD2Date = DateTimeOffset.MinValue;
                    data.ValidatedPurchasingDate = DateTimeOffset.MinValue;
                    data.ValidatedDate = DateTimeOffset.MinValue;

                    foreach (var item in data.Items)
                    {
                        EntityExtension.FlagForUpdate(item, user, USER_AGENT);
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

        public async Task<int> PRApprove(long id, string user)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Include(i => i.Items)
                        .Where(w => w.Id == id)
                        .Single();

                    EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                    data.IsValidated = true;
                    data.ValidatedBy = user;
                    data.ValidatedDate = DateTimeOffset.Now;

                        foreach (var item in data.Items)
                        {
                            EntityExtension.FlagForUpdate(item, user, USER_AGENT);
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

        public async Task<int> Patch(long id, JsonPatchDocument<GarmentPurchaseRequest> jsonPatch, string user)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Where(d => d.Id == id)
                        //.Include(i => i.Items)
                        .Single();

                    EntityExtension.FlagForUpdate(data, user, USER_AGENT);

                    //if (data.Items != null)
                    //{
                    //    foreach (var item in data.Items)
                    //    {
                    //        EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                    //    }
                    //}

                    jsonPatch.ApplyTo(data);

                    Updated = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }

        public async Task<int> PRUnApprove(long id, string user)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Include(i => i.Items)
                        .Where(w => w.Id == id)
                        .Single();

                    EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                    data.IsValidated = false;
                    data.ValidatedBy = user;
                    data.ValidatedDate = DateTimeOffset.Now;

                    foreach (var item in data.Items)
                    {
                        EntityExtension.FlagForUpdate(item, user, USER_AGENT);
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

        public MemoryStream GeneratePdf(GarmentPurchaseRequestViewModel viewModel)
        {
            return GarmentPurchaseRequestPDFTemplate.Generate(serviceProvider, viewModel);
        }

        private async Task SetIsPR(long scId, bool isPR)
        {
            var stringContentRequest = JsonConvert.SerializeObject(new List<object>
                        {
                            new { op = "replace", path = "/ispr", value = isPR }
                        });
            var httpContentRequest = new StringContent(stringContentRequest, Encoding.UTF8, General.JsonMediaType);

            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = await httpClient.PatchAsync(string.Concat(APIEndpoint.Sales, GarmentPreSalesContractUri, scId), httpContentRequest);

            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentResponse) ?? new Dictionary<string, object>();

                throw new Exception(string.Concat("Error from '", GarmentPreSalesContractUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }

        #region monitoringpurchasealluser
        public List<GarmentPurchaseRequest> ReadName(string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentPurchaseRequest> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "CreatedBy",
            };

            Query = QueryHelper<GarmentPurchaseRequest>.ConfigureSearch(Query, searchAttributes, Keyword); // kalo search setelah Select dengan .Where setelahnya maka case sensitive, kalo tanpa .Where tidak masalah

            Query = Query
                .Where(m => m.IsPosted == true && m.IsDeleted == false && m.CreatedBy.Contains(Keyword))
                .Select(s => new GarmentPurchaseRequest
                {
                    CreatedBy = s.CreatedBy
                }).Distinct();

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentPurchaseRequest>.ConfigureFilter(Query, FilterDictionary);

            return Query.ToList();
        }

        //public IQueryable<MonitoringPurchaseAllUserViewModel> GetMonitoringPurchaseAllReportQuery(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, int offset)
        //{


        //    DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
        //    DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;
        //    offset = 7;

        //    List<MonitoringPurchaseAllUserViewModel> listEPO = new List<MonitoringPurchaseAllUserViewModel>();
        //    var Query = (from
        //                  a in dbContext.GarmentPurchaseRequests
        //                 join b in dbContext.GarmentPurchaseRequestItems on a.Id equals b.GarmentPRId
        //                 //internalPO
        //                 join l in dbContext.GarmentInternalPurchaseOrderItems on b.Id equals l.GPRItemId into ll
        //                 from ipoitem in ll.DefaultIfEmpty()
        //                 join c in dbContext.GarmentInternalPurchaseOrders on ipoitem.GPOId equals c.Id into d
        //                 from ipo in d.DefaultIfEmpty()

        //                     //eksternalpo
        //                 join e in dbContext.GarmentExternalPurchaseOrderItems on ipo.Id equals e.POId into f
        //                 from epo in f.DefaultIfEmpty()

        //                 join g in dbContext.GarmentExternalPurchaseOrders on epo.GarmentEPOId equals g.Id into gg
        //                 from epos in gg.DefaultIfEmpty()
        //                     //do
        //                 join h in dbContext.GarmentDeliveryOrderDetails on epo.Id equals h.EPOItemId into ii
        //                 from dodetail in ii.DefaultIfEmpty()

        //                 join j in dbContext.GarmentDeliveryOrderItems on dodetail.GarmentDOItemId equals j.Id into hh
        //                 from doitem in hh.DefaultIfEmpty()

        //                 join k in dbContext.GarmentDeliveryOrders on doitem.GarmentDOId equals k.Id into kk
        //                 from dos in kk.DefaultIfEmpty()


        //                     //bc
        //                 join bcs in dbContext.GarmentBeacukaiItems on dos.Id equals bcs.GarmentDOId into bb
        //                 from beacukai in bb.DefaultIfEmpty()
        //                 join m in dbContext.GarmentBeacukais on beacukai.BeacukaiId equals m.Id into n
        //                 from bc in n.DefaultIfEmpty()
        //                     // //urn
        //                 join q in dbContext.GarmentUnitReceiptNoteItems on dodetail.Id equals q.DODetailId into qq
        //                 from unititem in qq.DefaultIfEmpty()

        //                 join o in dbContext.GarmentUnitReceiptNotes on unititem.URNId equals o.Id into p
        //                 from receipt in p.DefaultIfEmpty()

        //                     // //inv
        //                 join invd in dbContext.GarmentInvoiceDetails on dodetail.Id equals invd.DODetailId into rr
        //                 from invoicedetail in rr.DefaultIfEmpty()
        //                 join inv in dbContext.GarmentInvoiceItems on invoicedetail.InvoiceItemId equals inv.Id into r
        //                 from invoiceitem in r.DefaultIfEmpty()
        //                 join s in dbContext.GarmentInvoices on invoiceitem.InvoiceId equals s.Id into ss
        //                 from inv in ss.DefaultIfEmpty()
        //                     // //intern
        //                 join w in dbContext.GarmentInternNoteDetails on invoicedetail.Id equals w.InvoiceDetailId into ww
        //                 from internotedetail in ww.DefaultIfEmpty()
        //                 join t in dbContext.GarmentInternNoteItems on internotedetail.GarmentItemINId equals t.Id into u
        //                 from intern in u.DefaultIfEmpty()
        //                 join v in dbContext.GarmentInternNotes on intern.GarmentINId equals v.Id into vv
        //                 from internnote in vv.DefaultIfEmpty()

        //                     // //corr
        //                 join y in dbContext.GarmentCorrectionNoteItems on dodetail.Id equals y.DODetailId into oo
        //                 from corrItem in oo.DefaultIfEmpty()

        //                 join x in dbContext.GarmentCorrectionNotes on corrItem.GCorrectionId equals x.Id into cor
        //                 from correction in cor.DefaultIfEmpty()

        //                 where
        //                 ipoitem.IsDeleted == false && ipo.IsDeleted == false && epo.IsDeleted == false && epos.IsDeleted == false && intern.IsDeleted == false && internnote.IsDeleted == false && internotedetail.IsDeleted == false
        //                 && inv.IsDeleted == false && invoiceitem.IsDeleted == false && receipt.IsDeleted == false && unititem.IsDeleted == false && bc.IsDeleted == false

        //                  && a.IsDeleted == false && b.IsDeleted == false && ipo.IsDeleted == false && ipoitem.IsDeleted == false

        //                 && a.UnitId == (string.IsNullOrWhiteSpace(unit) ? a.UnitId : unit)
        //                  && a.Article == (string.IsNullOrWhiteSpace(article) ? a.Article : article) &&
        //                 epos.EPONo == (string.IsNullOrWhiteSpace(epono) ? epos.EPONo : epono)
        //                 && b.PO_SerialNumber == (string.IsNullOrWhiteSpace(poSerialNumber) ? b.PO_SerialNumber : poSerialNumber)
        //                 && dos.DONo == (string.IsNullOrWhiteSpace(doNo) ? dos.DONo : doNo)
        //                 && epos.SupplierId.ToString() == (string.IsNullOrWhiteSpace(supplier) ? epos.SupplierId.ToString() : supplier)
        //                 && a.RONo == (string.IsNullOrWhiteSpace(roNo) ? a.RONo : roNo)
        //                 && ipo.CreatedBy == (string.IsNullOrWhiteSpace(username) ? ipo.CreatedBy : username)
        //                 && b.IsUsed == (ipoStatus == "BELUM" ? false : ipoStatus == "SUDAH" ? true : b.IsUsed)
        //                 && ipoitem.Status == (string.IsNullOrWhiteSpace(status) ? ipoitem.Status : status)
        //                 && ((d1 != new DateTime(1970, 1, 1)) ? (a.Date.Date >= d1 && a.Date.Date <= d2) : true)

        //                 select new MonitoringPurchaseAllUserViewModel
        //                 {

        //                     poextNo = epos != null ? epos.EPONo : "",
        //                     poExtDate = epos != null ? epos.OrderDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     deliveryDate = epos != null ? epos.DeliveryDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     supplierCode = epos != null ? epos.SupplierCode : "",
        //                     supplierName = epos != null ? epos.SupplierName : "",
        //                     prNo = a.PRNo,
        //                     poSerialNumber = b.PO_SerialNumber,
        //                     prDate = a.Date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
        //                     PrDate = a.Date,
        //                     unitName = a.UnitName,
        //                     buyerCode = a.BuyerCode,
        //                     buyerName = a.BuyerName,
        //                     ro = a.RONo,
        //                     article = a.Article,
        //                     shipmentDate = a.ShipmentDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
        //                     productCode = b.ProductCode,
        //                     productName = b.ProductName,
        //                     prProductRemark = b.ProductRemark,
        //                     poProductRemark = epo != null ? epo.Remark : "",
        //                     poDealQty = epo != null ? epo.DealQuantity : 0,
        //                     poDealUomUnit = epo != null ? epo.DealUomUnit : "",
        //                     prBudgetPrice = b.BudgetPrice,
        //                     poPricePerDealUnit = epo != null ? epo.PricePerDealUnit : 0,
        //                     incomeTaxRate = epos != null ? (epos.IncomeTaxRate).ToString() : "",
        //                     totalNominalPO = epo != null ? String.Format("{0:N2}", (epo.DealQuantity * epo.PricePerDealUnit)) : "",
        //                     TotalNominalPO = epo != null ? (epo.DealQuantity * epo.PricePerDealUnit) : 0,
        //                     poCurrencyCode = epos != null ? epos.CurrencyCode : "",
        //                     poCurrencyRate = epos != null ? epos.CurrencyRate : 0,
        //                     totalNominalRp = epo != null && epos != null ? String.Format("{0:N2}", (epo.DealQuantity * epo.PricePerDealUnit * epos.CurrencyRate)).ToString() : "",
        //                     TotalNominalRp = epo != null && epos != null ? epo.DealQuantity * epo.PricePerDealUnit * epos.CurrencyRate : 0,
        //                     ipoDate = ipo != null ? ipo.CreatedUtc.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     username = ipo != null ? ipo.CreatedBy : "",
        //                     useIncomeTax = epos != null ? epos.IsIncomeTax.Equals(true) ? "YA" : "TIDAK" : "",
        //                     useVat = epos != null ? epos.IsUseVat.Equals(true) ? "YA" : "TIDAK" : "",
        //                     useInternalPO = b.IsUsed == true ? "SUDAH" : "BELUM",
        //                     status = ipoitem != null ? ipoitem.Status : "",
        //                     doNo = dos != null ? dos.DONo : "",
        //                     doDate = dos != null ? dos.DODate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     arrivalDate = dos != null ? dos.ArrivalDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     doQty = dodetail != null ? dodetail.DOQuantity : 0,
        //                     doUomUnit = dodetail != null ? dodetail.UomUnit : "",
        //                     remainingDOQty = dodetail != null ? dodetail.DealQuantity - dodetail.DOQuantity : 0,
        //                     bcNo = bc != null ? bc.BeacukaiNo : "",
        //                     bcDate = bc != null ? bc.BeacukaiDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     receiptNo = receipt != null ? receipt.URNNo : "",
        //                     receiptDate = receipt != null ? receipt.ReceiptDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     receiptQty = unititem != null ? String.Format("{0:N2}", unititem.ReceiptQuantity) : "",
        //                     ReceiptQty = unititem != null ? unititem.ReceiptQuantity : 0,
        //                     receiptUomUnit = unititem != null ? unititem.UomUnit : "",
        //                     invoiceNo = inv != null ? inv.InvoiceNo : "",
        //                     invoiceDate = inv != null ? inv.InvoiceDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     incomeTaxDate = inv != null ? inv.IncomeTaxDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     incomeTaxNo = inv != null ? inv.IncomeTaxNo : "",
        //                     incomeTaxType = inv != null ? inv.IncomeTaxName : "",
        //                     incomeTaxtRate = inv != null ? (inv.IncomeTaxRate).ToString() : "",
        //                     incomeTaxtValue = inv != null && inv.IsPayTax == true ? String.Format("{0:N2}", dodetail.DOQuantity * dodetail.PricePerDealUnit * inv.IncomeTaxRate / 100) : "",
        //                     vatNo = inv != null ? inv.VatNo : "",
        //                     vatDate = inv != null ? inv.VatDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     vatValue = inv != null && inv.IsPayTax == true ? String.Format("{0:N2}", dodetail.DOQuantity * dodetail.PricePerDealUnit * 10 / 100) : "",
        //                     internNo = internnote != null ? internnote.INNo : "",
        //                     internDate = internnote != null ? internnote.INDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     maturityDate = internotedetail != null ? internotedetail.PaymentDueDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     internTotal = internotedetail != null ? String.Format("{0:N2}", internotedetail.Quantity * internotedetail.PricePerDealUnit) : "",
        //                     InternTotal = internotedetail != null ? internotedetail.Quantity * internotedetail.PricePerDealUnit : 0,
        //                     dodetailId = corrItem != null ? corrItem.DODetailId : 0,
        //                     correctionNoteNo = correction != null ? correction.CorrectionNo : "",
        //                     correctionDate = correction != null ? correction.CorrectionDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "",
        //                     correctionRemark = correction != null ? correction.CorrectionType : "",
        //                     correctionTotal = correction == null ? 0 : correction.CorrectionType == "Harga Total" ? corrItem.PriceTotalAfter - corrItem.PriceTotalBefore : correction.CorrectionType == "Harga Satuan" ? (corrItem.PricePerDealUnitAfter - corrItem.PricePerDealUnitBefore) * corrItem.Quantity : correction.CorrectionType == "Jumlah" ? corrItem.PriceTotalAfter : 0,


        //                 }).Distinct().OrderBy(s => s.PrDate);
        //    int i = 1;
        //    foreach (var item in Query.Distinct())
        //    {
        //        listEPO.Add(
        //            new MonitoringPurchaseAllUserViewModel
        //            {
        //                index = i,
        //                poextNo = item.poextNo,
        //                poExtDate = item.poExtDate,
        //                deliveryDate = item.deliveryDate,
        //                supplierCode = item.supplierCode,
        //                supplierName = item.supplierName,
        //                prNo = item.prNo,
        //                poSerialNumber = item.poSerialNumber,
        //                prDate = item.prDate,
        //                PrDate = item.PrDate,
        //                unitName = item.unitName,
        //                buyerCode = item.buyerCode,
        //                buyerName = item.buyerName,
        //                ro = item.ro,
        //                article = item.article,
        //                shipmentDate = item.shipmentDate,
        //                productCode = item.productCode,
        //                productName = item.productName,
        //                prProductRemark = item.prProductRemark,
        //                poProductRemark = item.poProductRemark,
        //                poDealQty = item.poDealQty,
        //                poDealUomUnit = item.poDealUomUnit,
        //                prBudgetPrice = item.prBudgetPrice,
        //                poPricePerDealUnit = item.poPricePerDealUnit,
        //                totalNominalPO = item.totalNominalPO,
        //                TotalNominalPO = item.TotalNominalPO,
        //                poCurrencyCode = item.poCurrencyCode,
        //                poCurrencyRate = item.poCurrencyRate,
        //                totalNominalRp = item.totalNominalRp,
        //                TotalNominalRp = item.TotalNominalRp,
        //                incomeTaxRate = item.incomeTaxRate,
        //                ipoDate = item.ipoDate,
        //                username = item.username,
        //                useIncomeTax = item.useIncomeTax,
        //                useVat = item.useVat,
        //                useInternalPO = item.useInternalPO,
        //                incomeTaxtRate = item.incomeTaxtRate,
        //                status = item.status,
        //                doNo = item.doNo,
        //                doDate = item.doDate,
        //                arrivalDate = item.arrivalDate,
        //                doQty = item.doQty,
        //                doUomUnit = item.doUomUnit,
        //                remainingDOQty = item.remainingDOQty,
        //                bcNo = item.bcNo,
        //                bcDate = item.bcDate,
        //                receiptNo = item.receiptNo,
        //                receiptDate = item.receiptDate,
        //                receiptQty = item.receiptQty,
        //                ReceiptQty = item.ReceiptQty,
        //                receiptUomUnit = item.receiptUomUnit,
        //                invoiceNo = item.invoiceNo,
        //                invoiceDate = item.invoiceDate,
        //                incomeTaxDate = item.incomeTaxDate,
        //                incomeTaxNo = item.incomeTaxNo,
        //                incomeTaxType = item.incomeTaxType,
        //                incomeTaxtValue = item.incomeTaxtValue,
        //                vatNo = item.vatNo,
        //                vatDate = item.vatDate,
        //                vatValue = item.vatValue,
        //                internNo = item.internNo,
        //                internDate = item.internDate,
        //                internTotal = item.internTotal,
        //                InternTotal = item.InternTotal,
        //                maturityDate = item.maturityDate,
        //                dodetailId = item.dodetailId,
        //                correctionNoteNo = item.correctionNoteNo,
        //                correctionDate = item.correctionDate,
        //                correctionRemark = item.correctionRemark,
        //                correctionTotal = item.correctionTotal,

        //            }
        //            );



        //        i++;
        //    }
        //    Dictionary<long, string> qry = new Dictionary<long, string>();
        //    Dictionary<long, string> qryDate = new Dictionary<long, string>();
        //    Dictionary<long, string> qryQty = new Dictionary<long, string>();
        //    Dictionary<long, string> qryType = new Dictionary<long, string>();
        //    List<MonitoringPurchaseAllUserViewModel> listData = new List<MonitoringPurchaseAllUserViewModel>();

        //    var index = 0;
        //    List<string> corNo = new List<string>();
        //    foreach (MonitoringPurchaseAllUserViewModel data in listEPO.OrderByDescending(s => s.prDate))
        //    {
        //        string value;
        //        if (data.dodetailId != 0)
        //        {
        //            //string correctionDate = data.correctionDate == new DateTime(1970, 1, 1) ? "-" : data.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
        //            if (data.correctionNoteNo != null)
        //            {
        //                if (qry.TryGetValue(data.dodetailId, out value))
        //                {
        //                    var isexist = (from a in corNo
        //                                   where a == data.correctionNoteNo
        //                                   select a).FirstOrDefault();
        //                    if (isexist == null)
        //                    {
        //                        qry[data.dodetailId] += (index).ToString() + ". " + data.correctionNoteNo + "\n";
        //                        qryType[data.dodetailId] += (index).ToString() + ". " + data.correctionRemark + "\n";
        //                        qryDate[data.dodetailId] += (index).ToString() + ". " + data.correctionDate + "\n";
        //                        qryQty[data.dodetailId] += (index).ToString() + ". " + String.Format("{0:N2}", data.correctionTotal) + "\n";
        //                        index++;
        //                        corNo.Add(data.correctionNoteNo);
        //                    }

        //                }
        //                else
        //                {
        //                    index = 1;
        //                    qry[data.dodetailId] = (index).ToString() + ". " + data.correctionNoteNo + "\n";
        //                    qryType[data.dodetailId] = (index).ToString() + ". " + data.correctionRemark + "\n";
        //                    qryDate[data.dodetailId] = (index).ToString() + ". " + data.correctionDate + "\n";
        //                    qryQty[data.dodetailId] = (index).ToString() + ". " + String.Format("{0:N2}", data.correctionTotal) + "\n";
        //                    corNo.Add(data.correctionNoteNo);
        //                    index++;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            listData.Add(data);
        //        }

        //    }
        //    foreach (var corrections in qry.Distinct())
        //    {
        //        foreach (MonitoringPurchaseAllUserViewModel data in Query.ToList())
        //        {
        //            if (corrections.Key == data.dodetailId)
        //            {
        //                data.correctionNoteNo = qry[data.dodetailId];
        //                data.correctionRemark = qryType[data.dodetailId];
        //                data.valueCorrection = (qryQty[data.dodetailId]);
        //                data.correctionDate = qryDate[data.dodetailId];
        //                listData.Add(data);
        //                break;
        //            }
        //        }
        //    }

        //    //var op = qry;
        //    return listData.AsQueryable().OrderBy(s => s.PrDate);
        //    //return listEPO.AsQueryable();

        //}


        public async Task<List<MonitoringPurchaseAllUserViewModel>> GetMonitoringPurchaseReport(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order, int offset)
        {
            var Query = await GetMonitoringPurchaseByUserReportQuery(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo,dateFromEx, dateToEx, offset, page, size);
	 
			return Query;
		}
        public async Task<MemoryStream> GenerateExcelPurchase(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order, int offset)
        {
            var Query = await GetMonitoringPurchaseByUserReportQuery(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, offset, 1, int.MaxValue);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Ref.PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dibuat PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NO RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Shipment GMT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Target Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kena PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kena PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = " PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Term Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tempo", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Const", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Yarn", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Width", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Composition", DataType = typeof(String) });
			result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang (PR)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang (PO EKS)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Budget", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Budget", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total Beli", DataType = typeof(Double) });
			result.Columns.Add(new DataColumn() { ColumnName = "Total Budget", DataType = typeof(Double) });
			result.Columns.Add(new DataColumn() { ColumnName = "MT UANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kurs", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total RP", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima PO Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Datang Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Datang", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Pusat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Kecil", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Qty Sisa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Terima Unit", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NO PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Intern", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jatuh Tempo", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff Pembelian", DataType = typeof(String) });


            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", "", "", "","","","","", "", "", 0, 0, "", 0,0, 0, 0, "", "", 0, "", "", "", "", 0, "", "", "", "", "", "", "", 0, "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;

                    result.Rows.Add(index, item.prNo, item.prDate, item.unitName, item.poSerialNumber, item.useInternalPO, item.ro, item.article, item.buyerCode, item.buyerName, item.shipmentDate, item.poextNo, item.poExtDate, item.deliveryDate, item.useVat, item.useIncomeTax, item.incomeTaxRate, 
                        item.paymentMethod, item.paymentType, 
                        item.paymentDueDays, item.supplierCode, item.supplierName, item.SupplierImport, item.status, item.productCode, item.productName,item.consts,item.yarn,item.width,item.composition, item.prProductRemark, item.poProductRemark, item.poDefaultQty, item.poDealQty,
                        item.poDealUomUnit, item.prBudgetPrice, item.poPricePerDealUnit, item.TotalNominalPO,item.prBudgetPrice * item.poDefaultQty, item.poCurrencyCode, item.poCurrencyRate, item.TotalNominalRp, item.ipoDate, item.doNo,
                        item.doDate, item.arrivalDate, item.doQty, item.doUomUnit, item.Bon, item.BonSmall, item.bcNo, item.bcDate, item.receiptNo, item.receiptDate, item.ReceiptQty, item.receiptUomUnit,
                        item.invoiceNo, item.invoiceDate, item.vatNo, item.vatDate, item.vatValue, item.incomeTaxType, item.incomeTaxtRate, item.incomeTaxNo, item.incomeTaxDate, item.incomeTaxtValue,
                        item.internNo, item.internDate, item.InternTotal, item.maturityDate, item.correctionNoteNo, item.correctionDate, item.valueCorrection, item.correctionRemark, item.username);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        #endregion
        #region monitoring purchase
        public int TotalCountReport { get; set; } = 0;

		private async Task<List<GarmentProductViewModel>> GetProducts(List<string> _code)
		{

			var listCode = string.Join(",", _code.Distinct());
			var http = serviceProvider.GetService<IHttpClientService>();
			 
			List<GarmentProductViewModel> result = new List<GarmentProductViewModel>();
			var Uri = APIEndpoint.Core + $"master/garmentProducts/fabricByCode/";

			var httpContent = new StringContent(JsonConvert.SerializeObject(listCode), Encoding.UTF8, "application/json");

			var httpResponse = await http.SendAsync(HttpMethod.Get, Uri, httpContent);
			if (httpResponse.IsSuccessStatusCode)
			{
				var contentString = await httpResponse.Content.ReadAsStringAsync();
				Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
				var dataString = content.GetValueOrDefault("data").ToString();
				var data = JsonConvert.DeserializeObject<List<GarmentProductViewModel>>(dataString);
				result = data;
			}
			return result;
		}
		private async Task<List<MonitoringPurchaseAllUserViewModel>> GetMonitoringPurchaseByUserReportQuery(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int offset, int page, int size)
        {


            DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            DateTimeOffset d3 = dateFromEx == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFromEx;
            DateTimeOffset d4 = dateToEx == null ? DateTimeOffset.Now : (DateTimeOffset)dateToEx;
            offset = 7;

            List<MonitoringPurchaseAllUserViewModel> listEPO = new List<MonitoringPurchaseAllUserViewModel>();

            #region join query
            var Query = (from
                          a in dbContext.GarmentPurchaseRequests
                         join b in dbContext.GarmentPurchaseRequestItems on a.Id equals b.GarmentPRId
                         //internalPO
                         join l in dbContext.GarmentInternalPurchaseOrderItems on b.Id equals l.GPRItemId into ll
                         from ipoitem in ll.DefaultIfEmpty()
                         join c in dbContext.GarmentInternalPurchaseOrders on ipoitem.GPOId equals c.Id into d
                         from ipo in d.DefaultIfEmpty()

                             //eksternalpo
                         join e in dbContext.GarmentExternalPurchaseOrderItems on ipo.Id equals e.POId into f
                         from epo in f.DefaultIfEmpty()

                         join g in dbContext.GarmentExternalPurchaseOrders on epo.GarmentEPOId equals g.Id into gg
                         from epos in gg.DefaultIfEmpty()
                             //do
                         join h in dbContext.GarmentDeliveryOrderDetails on epo.Id equals h.EPOItemId into ii
                         from dodetail in ii.DefaultIfEmpty()

                         join j in dbContext.GarmentDeliveryOrderItems on dodetail.GarmentDOItemId equals j.Id into hh
                         from doitem in hh.DefaultIfEmpty()

                         join k in dbContext.GarmentDeliveryOrders on doitem.GarmentDOId equals k.Id into kk
                         from dos in kk.DefaultIfEmpty()


                             //bc
                         join bcs in dbContext.GarmentBeacukaiItems on dos.Id equals bcs.GarmentDOId into bb
                         from beacukai in bb.DefaultIfEmpty()
                         join m in dbContext.GarmentBeacukais on beacukai.BeacukaiId equals m.Id into n
                         from bc in n.DefaultIfEmpty()
                             //urn
                         join q in dbContext.GarmentUnitReceiptNoteItems on dodetail.Id equals q.DODetailId into qq
                         from unititem in qq.DefaultIfEmpty()

                         join o in dbContext.GarmentUnitReceiptNotes on unititem.URNId equals o.Id into p
                         from receipt in p.DefaultIfEmpty()

                             //inv
                         join invd in dbContext.GarmentInvoiceDetails on dodetail.Id equals invd.DODetailId into rr
                         from invoicedetail in rr.DefaultIfEmpty()
                         join inv in dbContext.GarmentInvoiceItems on invoicedetail.InvoiceItemId equals inv.Id into r
                         from invoiceitem in r.DefaultIfEmpty()
                         join s in dbContext.GarmentInvoices on invoiceitem.InvoiceId equals s.Id into ss
                         from inv in ss.DefaultIfEmpty()
                             //intern
                         join w in dbContext.GarmentInternNoteDetails on invoicedetail.Id equals w.InvoiceDetailId into ww
                         from internotedetail in ww.DefaultIfEmpty()
                         join t in dbContext.GarmentInternNoteItems on internotedetail.GarmentItemINId equals t.Id into u
                         from intern in u.DefaultIfEmpty()
                         join v in dbContext.GarmentInternNotes on intern.GarmentINId equals v.Id into vv
                         from internnote in vv.DefaultIfEmpty()

                             //    //corr
                             //join y in dbContext.GarmentCorrectionNoteItems on dodetail.Id equals y.DODetailId into oo
                             //from corrItem in oo.DefaultIfEmpty()

                             //join x in dbContext.GarmentCorrectionNotes on corrItem.GCorrectionId equals x.Id into cor
                             //from correction in cor.DefaultIfEmpty()

                         where
                         ipoitem.IsDeleted == false && ipo.IsDeleted == false && epo.IsDeleted == false && epos.IsDeleted == false && intern.IsDeleted == false && internnote.IsDeleted == false && internotedetail.IsDeleted == false
                         && inv.IsDeleted == false && invoiceitem.IsDeleted == false && receipt.IsDeleted == false && unititem.IsDeleted == false && bc.IsDeleted == false
                          && a.IsDeleted == false && b.IsDeleted == false && ipo.IsDeleted == false && ipoitem.IsDeleted == false


                         && (unit == null || (unit != null && unit != "" && a.UnitId == unit))
                         && (article == null || (article != null && article != "" && a.Article == article))
                         && (roNo == null || (roNo != null && roNo != "" && a.RONo == roNo))
                         && (a.Date.Date >= d1 && a.Date.Date <= d2)
                         //&& (epos.OrderDate >= d3 && epos.OrderDate <= d4)
                          //&& ((d1 != new DateTime(1970, 1, 1)) ? (a.Date.Date >= d1 && a.Date.Date <= d2) : true)

                          && ((d3 != new DateTime(1970, 1, 1)) ? (epos.OrderDate >= d3 && epos.OrderDate <= d4) : true)

                          && (poSerialNumber == null || (poSerialNumber != null && poSerialNumber != "" && b.PO_SerialNumber == poSerialNumber))
                          && b.IsUsed == (ipoStatus != "BELUM" && (ipoStatus == "SUDAH" || b.IsUsed))

                          && (username == null || (username != null && username != "" && ipo.CreatedBy == username))
                          && (status == null || (status != null && status != "" && ipoitem.Status == status))

                          && (epono == null || (epono != null && epono != "" && epos.EPONo == epono))
                          && (supplier == null || (supplier != null && supplier != "" && epos.SupplierId.ToString() == supplier))

                          && (doNo == null || (doNo != null && doNo != "" && dos.DONo == doNo))
                          && (receipt != null ? receipt.URNType == "PEMBELIAN" : true)

                         //orderby a.Date descending

                         select new SelectedId
                         {
                             PRDate = a.Date,
                             PRId = a.Id,
                             PRItemId = b.Id,
                             POId = ipo == null ? 0 : ipo.Id,
                             POItemId = ipoitem == null ? 0 : ipoitem.Id,
                             EPOId = epos == null ? 0 : epos.Id,
                             EPOItemId = epo == null ? 0 : epo.Id,
                             DOId = dos == null ? 0 : dos.Id,
                             DOItemId = doitem == null ? 0 : doitem.Id,
                             DODetailId = dodetail == null ? 0 : dodetail.Id,
                             BCId = bc == null ? 0 : bc.Id,
                             BCItemId = beacukai == null ? 0 : beacukai.Id,
                             URNId = receipt == null ? 0 : receipt.Id,
                             URNItemId = unititem == null ? 0 : unititem.Id,
                             INVId = inv == null ? 0 : inv.Id,
                             INVItemId = invoiceitem == null ? 0 : invoiceitem.Id,
                             INVDetailId = invoicedetail == null ? 0 : invoicedetail.Id,
                             INId = internnote == null ? 0 : internnote.Id,
                             INItemId = intern == null ? 0 : intern.Id,
                             INDetailId = internotedetail == null ? 0 : internotedetail.Id
                         });
            #endregion

            TotalCountReport = Query.Distinct().OrderByDescending(o => o.PRDate).Count();
            var queryResult = Query.Distinct().OrderByDescending(o => o.PRDate).Skip((page - 1) * size).Take(size).ToList();

            var purchaseRequestIds = queryResult.Select(s => s.PRId).Distinct().ToList();
            var purchaseRequests = dbContext.GarmentPurchaseRequests.Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.PRNo, s.Date, s.UnitName, s.BuyerCode, s.BuyerName, s.RONo, s.Article, s.ShipmentDate, s.IsUsed }).ToList();
            var purchaseRequestItemIds = queryResult.Select(s => s.PRItemId).Distinct().ToList();
            var purchaseRequestItems = dbContext.GarmentPurchaseRequestItems.Where(w => purchaseRequestItemIds.Contains(w.Id)).Select(s => new { s.Id, s.PO_SerialNumber, s.ProductCode, s.ProductName, s.ProductRemark, s.BudgetPrice, s.IsUsed }).ToList();

            var purchaseOrderInternalIds = queryResult.Select(s => s.POId).Distinct().ToList();
            var purchaseOrderInternals = dbContext.GarmentInternalPurchaseOrders.Where(w => purchaseOrderInternalIds.Contains(w.Id)).Select(s => new { s.Id, s.CreatedUtc, s.CreatedBy }).ToList();
            var purchaseOrderInternalItemIds = queryResult.Select(s => s.POItemId).Distinct().ToList();
            var purchaseOrderInternalItems = dbContext.GarmentInternalPurchaseOrderItems.Where(w => purchaseOrderInternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Status }).ToList();

            var purchaseOrderExternalIds = queryResult.Select(s => s.EPOId).Distinct().ToList();
            var purchaseOrderExternals = dbContext.GarmentExternalPurchaseOrders.Where(w => purchaseOrderExternalIds.Contains(w.Id)).Select(s => new { s.Id, s.EPONo, s.OrderDate, s.DeliveryDate, s.SupplierCode, s.SupplierName, s.CurrencyCode, s.CurrencyRate, s.IncomeTaxRate, s.IsIncomeTax, s.IsUseVat, s.PaymentMethod, s.PaymentType, s.PaymentDueDays, s.SupplierImport }).ToList();
            var purchaseOrderExternalItemIds = queryResult.Select(s => s.EPOItemId).Distinct().ToList();
            var purchaseOrderExternalItems = dbContext.GarmentExternalPurchaseOrderItems.Where(w => purchaseOrderExternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Remark, s.DealQuantity, s.DealUomUnit, s.PricePerDealUnit, s.DefaultQuantity }).ToList();

            var deliveryOrderIds = queryResult.Select(s => s.DOId).Distinct().ToList();
            var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.DONo, s.DODate, s.ArrivalDate, s.BillNo, s.PaymentBill }).ToList();
            //var deliveryOrderItemIds = queryResult.Select(s => s.DOItemId).Distinct().ToList();
            //var deliveryOrderItems = dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            var deliveryOrderDetailIds = queryResult.Select(s => s.DODetailId).Distinct().ToList();
            var deliveryOrderDetails = dbContext.GarmentDeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.DOQuantity, s.UomUnit, s.DealQuantity, s.PricePerDealUnit }).ToList();

            var customIds = queryResult.Select(s => s.BCId).Distinct().ToList();
            var customs = dbContext.GarmentBeacukais.Where(w => customIds.Contains(w.Id)).Select(s => new { s.Id, s.BeacukaiNo, s.BeacukaiDate }).ToList();
            //var customItemIds = queryResult.Select(s => s.BCItemId).Distinct().ToList();
            //var customItems = dbContext.GarmentBeacukaiItems.Where(w => customItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();

            var unitReceiptNoteIds = queryResult.Select(s => s.URNId).Distinct().ToList();
            var unitReceiptNotes = dbContext.GarmentUnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate }).ToList();
            var unitReceiptNoteItemIds = queryResult.Select(s => s.URNItemId).Distinct().ToList();
            var unitReceiptNoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.UomUnit }).ToList();

            var invoiceIds = queryResult.Select(s => s.INVId).Distinct().ToList();
            var invoices = dbContext.GarmentInvoices.Where(w => invoiceIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNo, s.InvoiceDate, s.IncomeTaxDate, s.IncomeTaxNo, s.IncomeTaxName, s.IncomeTaxRate, s.IsPayTax, s.VatDate, s.VatNo, s.VatRate }).ToList();
            //var invoiceItemIds = queryResult.Select(s => s.INVItemId).Distinct().ToList();
            //var invoiceItems = dbContext.GarmentInvoiceItems.Where(w => invoiceItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            //var invoiceDetailIds = queryResult.Select(s => s.INVDetailId).Distinct().ToList();
            //var invoiceDetails = dbContext.GarmentInvoiceDetails.Where(w => invoiceDetailIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();

            var internNoteIds = queryResult.Select(s => s.INId).Distinct().ToList();
            var internNotes = dbContext.GarmentInternNotes.Where(w => internNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.INNo, s.INDate }).ToList();
            //var internNoteItemIds = queryResult.Select(s => s.INItemId).Distinct().ToList();
            //var internNoteItems = dbContext.GarmentInternNoteItems.Where(w => internNoteItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            var internNoteDetailIds = queryResult.Select(s => s.INDetailId).Distinct().ToList();
            var internNoteDetails = dbContext.GarmentInternNoteDetails.Where(w => internNoteDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.Quantity, s.PricePerDealUnit, s.PaymentDueDate }).ToList();

            var corrections = dbContext.GarmentCorrectionNotes.Where(w => deliveryOrderIds.Contains(w.DOId)).Select(s => new { s.Id, s.DOId, s.CorrectionNo, s.CorrectionType, s.CorrectionDate }).ToList();
            var correctionIds = corrections.Select(s => s.Id).ToList();
            var correctionItems = dbContext.GarmentCorrectionNoteItems.Where(w => correctionIds.Contains(w.GCorrectionId)).Select(s => new { s.GCorrectionId, s.DODetailId, s.PriceTotalAfter, s.PriceTotalBefore, s.Quantity }).ToList();

            int i = ((page - 1) * size) + 1;
			List<string> listCode = new List<string>();
			
			foreach (var item in queryResult)
			{
				var purchaseRequestItem = purchaseRequestItems.FirstOrDefault(f => f.Id.Equals(item.PRItemId));
				listCode.Add(purchaseRequestItem.ProductCode);
			}
			listCode.Distinct();
			var products = await GetProducts(listCode);
			foreach (var item in queryResult)
			{


				var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
				var purchaseRequestItem = purchaseRequestItems.FirstOrDefault(f => f.Id.Equals(item.PRItemId));

				var purchaseOrderInternal = purchaseOrderInternals.FirstOrDefault(f => f.Id.Equals(item.POId));
				var purchaseOrderInternalItem = purchaseOrderInternalItems.FirstOrDefault(f => f.Id.Equals(item.POItemId));

				var purchaseOrderExternal = purchaseOrderExternals.FirstOrDefault(f => f.Id.Equals(item.EPOId));
				var purchaseOrderExternalItem = purchaseOrderExternalItems.FirstOrDefault(f => f.Id.Equals(item.EPOItemId));

				var deliveryOrder = deliveryOrders.FirstOrDefault(f => f.Id.Equals(item.DOId));
				//var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(f => f.Id.Equals(item.DOItemId));
				var deliveryOrderDetail = deliveryOrderDetails.FirstOrDefault(f => f.Id.Equals(item.DODetailId));

				var custom = customs.FirstOrDefault(f => f.Id.Equals(item.BCId));
				//var customItem = customItems.FirstOrDefault(f => f.Id.Equals(item.BCItemId));

				var unitReceiptNote = unitReceiptNotes.FirstOrDefault(f => f.Id.Equals(item.URNId));
				var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(f => f.Id.Equals(item.URNItemId));

				var invoice = invoices.FirstOrDefault(f => f.Id.Equals(item.INVId));
				//var invoiceItem = invoiceItems.FirstOrDefault(f => f.Id.Equals(item.INVItemId));
				//var invoiceDetail = invoiceDetails.FirstOrDefault(f => f.Id.Equals(item.INVDetailId));

				var internNote = internNotes.FirstOrDefault(f => f.Id.Equals(item.INId));
				//var internNoteItem = internNoteItems.FirstOrDefault(f => f.Id.Equals(item.INItemId));
				var internNoteDetail = internNoteDetails.FirstOrDefault(f => f.Id.Equals(item.INDetailId));

				var selectedCorrections = corrections.Where(w => w.DOId.Equals(item.DOId)).ToList();
				var selectedCorrectionIds = selectedCorrections.Select(s => s.Id).ToList();
				var selectedCorrectionItems = correctionItems.Where(w => selectedCorrectionIds.Contains(w.GCorrectionId)).ToList();

				var correctionNoList = new List<string>();
				var correctionDateList = new List<string>();
				var correctionRemarkList = new List<string>();
				var correctionNominalList = new List<string>();
				int j = 1;
				foreach (var selectedCorrection in selectedCorrections)
				{

					var selectedCorrectionItem = selectedCorrectionItems.FirstOrDefault(f => f.GCorrectionId.Equals(selectedCorrection.Id) && f.DODetailId.Equals(item.DODetailId));
					if (selectedCorrectionItem != null)
					{
						correctionNoList.Add($"{j}. {selectedCorrection.CorrectionNo}");
						correctionRemarkList.Add($"{j}. {selectedCorrection.CorrectionType}");
						correctionDateList.Add($"{j}. {selectedCorrection.CorrectionDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}");

						switch (selectedCorrection.CorrectionType)
						{
							case "Harga Total":
								correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore)}");
								break;
							case "Harga Satuan":
								correctionNominalList.Add($"{j}. {string.Format("{0:N2}", (selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore) * selectedCorrectionItem.Quantity)}");
								break;
							case "Jumlah":
								correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter)}");
								break;
							default:
								break;
						}
						j++;
					}
				}


				listEPO.Add(
					new MonitoringPurchaseAllUserViewModel
					{
						index = i,
						poextNo = purchaseOrderExternal?.EPONo,
						poExtDate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.OrderDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						deliveryDate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.DeliveryDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						supplierCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierCode,
						supplierName = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierName,
						prNo = purchaseRequest.PRNo,
						poSerialNumber = purchaseRequestItem.PO_SerialNumber,
						prDate = purchaseRequest.Date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						PrDate = purchaseRequest.Date,
						unitName = purchaseRequest.UnitName,
						buyerCode = purchaseRequest.BuyerCode,
						buyerName = purchaseRequest.BuyerName,
						ro = purchaseRequest.RONo,
						article = purchaseRequest.Article,
						shipmentDate = purchaseRequest.ShipmentDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						productCode = purchaseRequestItem.ProductCode,
						productName = purchaseRequestItem.ProductName,
						prProductRemark = purchaseRequestItem.ProductRemark,
						poProductRemark = purchaseOrderExternalItem == null ? "" : purchaseOrderExternalItem.Remark,
						poDefaultQty = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DefaultQuantity,
						poDealQty = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity,
						poDealUomUnit = purchaseOrderExternalItem == null ? "" : purchaseOrderExternalItem.DealUomUnit,
						prBudgetPrice = purchaseRequestItem.BudgetPrice,
						poPricePerDealUnit = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.PricePerDealUnit,
						totalNominalPO = purchaseOrderExternalItem == null ? "" : string.Format("{0:N2}", purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit),
						TotalNominalPO = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit,
						poCurrencyCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.CurrencyCode,
						poCurrencyRate = purchaseOrderExternal == null ? 0 : purchaseOrderExternal.CurrencyRate,
						totalNominalRp = purchaseOrderExternalItem == null && purchaseOrderExternal == null ? "" : string.Format("{0:N2}", purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit * purchaseOrderExternal.CurrencyRate),
						TotalNominalRp = purchaseOrderExternal == null && purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit * purchaseOrderExternal.CurrencyRate,
						incomeTaxRate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IncomeTaxRate?.ToString(),
						paymentMethod = purchaseOrderExternal == null ? "" : purchaseOrderExternal.PaymentMethod?.ToString(),
						paymentType = purchaseOrderExternal == null ? "" : purchaseOrderExternal.PaymentType?.ToString(),
						paymentDueDays = purchaseOrderExternal == null ? 0 : purchaseOrderExternal.PaymentDueDays,
						ipoDate = purchaseOrderInternal == null ? "" : purchaseOrderInternal.CreatedUtc.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						username = purchaseOrderInternal == null ? "" : purchaseOrderInternal.CreatedBy,
						useIncomeTax = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IsIncomeTax ? "YA" : "TIDAK",
						useVat = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IsUseVat ? "YA" : "TIDAK",
						useInternalPO = purchaseRequestItem.IsUsed ? "YA" : "TIDAK",
						status = purchaseOrderInternalItem == null ? "" : purchaseOrderInternalItem.Status,
						doNo = deliveryOrder == null ? "" : deliveryOrder.DONo,
						doDate = deliveryOrder == null ? "" : deliveryOrder.DODate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						arrivalDate = deliveryOrder == null ? "" : deliveryOrder.ArrivalDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						doQty = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DOQuantity,
						doUomUnit = deliveryOrderDetail == null ? "" : deliveryOrderDetail.UomUnit,
						remainingDOQty = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DealQuantity - deliveryOrderDetail.DOQuantity,
						bcNo = custom == null ? "" : custom.BeacukaiNo,
						bcDate = custom == null ? "" : custom.BeacukaiDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						receiptNo = unitReceiptNote == null ? "" : unitReceiptNote.URNNo,
						receiptDate = unitReceiptNote == null ? "" : unitReceiptNote.ReceiptDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						receiptQty = unitReceiptNoteItem == null ? "" : string.Format("{0:N2}", unitReceiptNoteItem.ReceiptQuantity),
						ReceiptQty = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.ReceiptQuantity,
						receiptUomUnit = unitReceiptNoteItem == null ? "" : unitReceiptNoteItem.UomUnit,
						invoiceNo = invoice == null ? "" : invoice.InvoiceNo,
						invoiceDate = invoice == null ? "" : invoice.InvoiceDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						incomeTaxDate = invoice == null ? "" : invoice.IncomeTaxDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						incomeTaxNo = invoice == null ? "" : invoice.IncomeTaxNo,
						incomeTaxType = invoice == null ? "" : invoice.IncomeTaxName,
						incomeTaxtRate = invoice == null ? "" : invoice.IncomeTaxRate.ToString(),
						incomeTaxtValue = invoice != null && invoice.IsPayTax ? string.Format("{0:N2}", deliveryOrderDetail.DOQuantity * deliveryOrderDetail.PricePerDealUnit * invoice.IncomeTaxRate / 100) : "",
						vatNo = invoice == null ? "" : invoice.VatNo,
						vatDate = invoice == null ? "" : invoice.VatDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						vatValue = invoice != null && invoice.IsPayTax ? string.Format("{0:N2}", deliveryOrderDetail.DOQuantity * deliveryOrderDetail.PricePerDealUnit * (invoice.VatRate / 100)) : "",
						internNo = internNote == null ? "" : internNote.INNo,
						internDate = internNote == null ? "" : internNote.INDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						internTotal = internNoteDetail == null ? "" : string.Format("{0:N2}", internNoteDetail.Quantity * internNoteDetail.PricePerDealUnit),
						InternTotal = internNoteDetail == null ? 0 : internNoteDetail.Quantity * internNoteDetail.PricePerDealUnit,
						maturityDate = internNoteDetail == null ? "" : internNoteDetail.PaymentDueDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
						dodetailId = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.Id,
						correctionNoteNo = string.Join("\n", correctionNoList),
						correctionDate = string.Join("\n", correctionDateList),
						correctionRemark = string.Join("\n", correctionRemarkList),
						valueCorrection = string.Join("\n", correctionNominalList),
						Bon = deliveryOrder == null ? "" : deliveryOrder.BillNo,
						BonSmall = deliveryOrder == null ? "" : deliveryOrder.PaymentBill,
						SupplierImport = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierImport == true ? "IMPORT" : "LOCAL"
						
					});
				i++;
			}
			
			//foreach (var corrections in qry.Distinct())
			//{
			//    foreach (MonitoringPurchaseAllUserViewModel data in Query.ToList())
			//    {
			//        if (corrections.Key == data.dodetailId)
			//        {
			//            data.correctionNoteNo = qry[data.dodetailId];
			//            data.correctionRemark = qryType[data.dodetailId];
			//            data.valueCorrection = (qryQty[data.dodetailId]);
			//            data.correctionDate = qryDate[data.dodetailId];
			//            listData.Add(data);
			//            break;
			//        }
			//    }
			//}

			//var op = qry;
			foreach(var item in listEPO)
			{
				item.yarn = products.Where(s => s.Code == item.productCode).Select(s => s.Yarn).FirstOrDefault();
				item.width = products.Where(s => s.Code == item.productCode).Select(s => s.Width).FirstOrDefault();
				item.consts = products.Where(s => s.Code == item.productCode).Select(s => s.Const).FirstOrDefault();
				item.composition = products.Where(s => s.Code == item.productCode).Select(s => s.Composition).FirstOrDefault();
				item.Total = TotalCountReport;
			}
			return listEPO;
            //return listEPO.AsQueryable();

        }

        /// <summary>
        /// optimezed for GetMonitoringPurchaseByUserReportQuery 
        /// </summary>
        /// <param name="epono"></param>
        /// <param name="unit"></param>
        /// <param name="roNo"></param>
        /// <param name="article"></param>
        /// <param name="poSerialNumber"></param>
        /// <param name="username"></param>
        /// <param name="doNo"></param>
        /// <param name="ipoStatus"></param>
        /// <param name="supplier"></param>
        /// <param name="status"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dateFromEx"></param>
        /// <param name="dateToEx"></param>
        /// <param name="offset"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private List<MonitoringPurchaseAllUserViewModel> GetMonitoringPurchaseByUserReportQueryOptimized(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int offset, int page, int size)
        {


            DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            DateTimeOffset d3 = dateFromEx == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFromEx;
            DateTimeOffset d4 = dateToEx == null ? DateTimeOffset.Now : (DateTimeOffset)dateToEx;
            offset = 7;

            List<MonitoringPurchaseAllUserViewModel> listEPO = new List<MonitoringPurchaseAllUserViewModel>();

            #region join query
            var Query = (from
                          a in dbContext.GarmentPurchaseRequests
                         join b in dbContext.GarmentPurchaseRequestItems on a.Id equals b.GarmentPRId
                         //internalPO
                         join l in dbContext.GarmentInternalPurchaseOrderItems on b.Id equals l.GPRItemId into ll
                         from ipoitem in ll.DefaultIfEmpty()
                         join c in dbContext.GarmentInternalPurchaseOrders on ipoitem.GPOId equals c.Id into d
                         from ipo in d.DefaultIfEmpty()

                             //eksternalpo
                         join e in dbContext.GarmentExternalPurchaseOrderItems on ipo.Id equals e.POId into f
                         from epo in f.DefaultIfEmpty()

                         join g in dbContext.GarmentExternalPurchaseOrders on epo.GarmentEPOId equals g.Id into gg
                         from epos in gg.DefaultIfEmpty()
                             //do
                         join h in dbContext.GarmentDeliveryOrderDetails on epo.Id equals h.EPOItemId into ii
                         from dodetail in ii.DefaultIfEmpty()

                         join j in dbContext.GarmentDeliveryOrderItems on dodetail.GarmentDOItemId equals j.Id into hh
                         from doitem in hh.DefaultIfEmpty()

                         join k in dbContext.GarmentDeliveryOrders on doitem.GarmentDOId equals k.Id into kk
                         from dos in kk.DefaultIfEmpty()


                             //bc
                         join bcs in dbContext.GarmentBeacukaiItems on dos.Id equals bcs.GarmentDOId into bb
                         from beacukai in bb.DefaultIfEmpty()
                         join m in dbContext.GarmentBeacukais on beacukai.BeacukaiId equals m.Id into n
                         from bc in n.DefaultIfEmpty()
                             //urn
                         join q in dbContext.GarmentUnitReceiptNoteItems on dodetail.Id equals q.DODetailId into qq
                         from unititem in qq.DefaultIfEmpty()

                         join o in dbContext.GarmentUnitReceiptNotes on unititem.URNId equals o.Id into p
                         from receipt in p.DefaultIfEmpty()

                             //inv
                         join invd in dbContext.GarmentInvoiceDetails on dodetail.Id equals invd.DODetailId into rr
                         from invoicedetail in rr.DefaultIfEmpty()
                         join inv in dbContext.GarmentInvoiceItems on invoicedetail.InvoiceItemId equals inv.Id into r
                         from invoiceitem in r.DefaultIfEmpty()
                         join s in dbContext.GarmentInvoices on invoiceitem.InvoiceId equals s.Id into ss
                         from inv in ss.DefaultIfEmpty()
                             //intern
                         join w in dbContext.GarmentInternNoteDetails on invoicedetail.Id equals w.InvoiceDetailId into ww
                         from internotedetail in ww.DefaultIfEmpty()
                         join t in dbContext.GarmentInternNoteItems on internotedetail.GarmentItemINId equals t.Id into u
                         from intern in u.DefaultIfEmpty()
                         join v in dbContext.GarmentInternNotes on intern.GarmentINId equals v.Id into vv
                         from internnote in vv.DefaultIfEmpty()

                             //    //corr
                             //join y in dbContext.GarmentCorrectionNoteItems on dodetail.Id equals y.DODetailId into oo
                             //from corrItem in oo.DefaultIfEmpty()

                             //join x in dbContext.GarmentCorrectionNotes on corrItem.GCorrectionId equals x.Id into cor
                             //from correction in cor.DefaultIfEmpty()

                         where
                         ipoitem.IsDeleted == false && ipo.IsDeleted == false && epo.IsDeleted == false && epos.IsDeleted == false && intern.IsDeleted == false && internnote.IsDeleted == false && internotedetail.IsDeleted == false
                         && inv.IsDeleted == false && invoiceitem.IsDeleted == false && receipt.IsDeleted == false && unititem.IsDeleted == false && bc.IsDeleted == false
                          && a.IsDeleted == false && b.IsDeleted == false && ipo.IsDeleted == false && ipoitem.IsDeleted == false


                         && (unit == null || (unit != null && unit != "" && a.UnitId == unit))
                         && (article == null || (article != null && article != "" && a.Article == article))
                         && (roNo == null || (roNo != null && roNo != "" && a.RONo == roNo))
                         && (a.Date.Date >= d1 && a.Date.Date <= d2)
                          //&& (epos.OrderDate >= d3 && epos.OrderDate <= d4)
                          //&& ((d1 != new DateTime(1970, 1, 1)) ? (a.Date.Date >= d1 && a.Date.Date <= d2) : true)

                          && ((d3 != new DateTime(1970, 1, 1)) ? (epos.OrderDate >= d3 && epos.OrderDate <= d4) : true)

                          && (poSerialNumber == null || (poSerialNumber != null && poSerialNumber != "" && b.PO_SerialNumber == poSerialNumber))
                          && b.IsUsed == (ipoStatus != "BELUM" && (ipoStatus == "SUDAH" || b.IsUsed))

                          && (username == null || (username != null && username != "" && ipo.CreatedBy == username))
                          && (status == null || (status != null && status != "" && ipoitem.Status == status))

                          && (epono == null || (epono != null && epono != "" && epos.EPONo == epono))
                          && (supplier == null || (supplier != null && supplier != "" && epos.SupplierId.ToString() == supplier))

                          && (doNo == null || (doNo != null && doNo != "" && dos.DONo == doNo))
                          && (receipt != null ? receipt.URNType == "PEMBELIAN" : true)

                         //orderby a.Date descending

                         select new SelectedId
                         {
                             PRDate = a.Date,
                             PRId = a.Id,
                             PRItemId = b.Id,
                             POId = ipo == null ? 0 : ipo.Id,
                             POItemId = ipoitem == null ? 0 : ipoitem.Id,
                             EPOId = epos == null ? 0 : epos.Id,
                             EPOItemId = epo == null ? 0 : epo.Id,
                             DOId = dos == null ? 0 : dos.Id,
                             DOItemId = doitem == null ? 0 : doitem.Id,
                             DODetailId = dodetail == null ? 0 : dodetail.Id,
                             BCId = bc == null ? 0 : bc.Id,
                             BCItemId = beacukai == null ? 0 : beacukai.Id,
                             URNId = receipt == null ? 0 : receipt.Id,
                             URNItemId = unititem == null ? 0 : unititem.Id,
                             INVId = inv == null ? 0 : inv.Id,
                             INVItemId = invoiceitem == null ? 0 : invoiceitem.Id,
                             INVDetailId = invoicedetail == null ? 0 : invoicedetail.Id,
                             INId = internnote == null ? 0 : internnote.Id,
                             INItemId = intern == null ? 0 : intern.Id,
                             INDetailId = internotedetail == null ? 0 : internotedetail.Id
                         });
            #endregion

            TotalCountReport = Query.Distinct().OrderByDescending(o => o.PRDate).Count();
            var queryResult = Query.Distinct().OrderByDescending(o => o.PRDate).Skip((page - 1) * size).Take(size).ToList();

            var purchaseRequestIds = queryResult.Select(s => s.PRId).Distinct().ToList();
            var purchaseRequests = dbContext.GarmentPurchaseRequests.Include(s=> s.Items).Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.PRNo, s.Date, s.UnitName, s.BuyerCode, s.BuyerName, s.RONo, s.Article, s.ShipmentDate, s.IsUsed }).ToList();
            var purchaseRequestItemIds = queryResult.Select(s => s.PRItemId).Distinct().ToList();
            var purchaseRequestItems = dbContext.GarmentPurchaseRequestItems.Where(w => purchaseRequestItemIds.Contains(w.Id)).Select(s => new { s.Id, s.PO_SerialNumber, s.ProductCode, s.ProductName, s.ProductRemark, s.BudgetPrice, s.IsUsed }).ToList();

            var purchaseOrderInternalIds = queryResult.Select(s => s.POId).Distinct().ToList();
            var purchaseOrderInternals = dbContext.GarmentInternalPurchaseOrders.Where(w => purchaseOrderInternalIds.Contains(w.Id)).Select(s => new { s.Id, s.CreatedUtc, s.CreatedBy }).ToList();
            var purchaseOrderInternalItemIds = queryResult.Select(s => s.POItemId).Distinct().ToList();
            var purchaseOrderInternalItems = dbContext.GarmentInternalPurchaseOrderItems.Where(w => purchaseOrderInternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Status }).ToList();

            var purchaseOrderExternalIds = queryResult.Select(s => s.EPOId).Distinct().ToList();
            var purchaseOrderExternals = dbContext.GarmentExternalPurchaseOrders.Where(w => purchaseOrderExternalIds.Contains(w.Id)).Select(s => new { s.Id, s.EPONo, s.OrderDate, s.DeliveryDate, s.SupplierCode, s.SupplierName, s.CurrencyCode, s.CurrencyRate, s.IncomeTaxRate, s.IsIncomeTax, s.IsUseVat, s.PaymentMethod, s.PaymentType, s.PaymentDueDays, s.SupplierImport }).ToList();
            var purchaseOrderExternalItemIds = queryResult.Select(s => s.EPOItemId).Distinct().ToList();
            var purchaseOrderExternalItems = dbContext.GarmentExternalPurchaseOrderItems.Where(w => purchaseOrderExternalItemIds.Contains(w.Id)).Select(s => new { s.Id, s.Remark, s.DealQuantity, s.DealUomUnit, s.PricePerDealUnit, s.DefaultQuantity }).ToList();

            var deliveryOrderIds = queryResult.Select(s => s.DOId).Distinct().ToList();
            var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(w => deliveryOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.DONo, s.DODate, s.ArrivalDate, s.BillNo, s.PaymentBill }).ToList();
            //var deliveryOrderItemIds = queryResult.Select(s => s.DOItemId).Distinct().ToList();
            //var deliveryOrderItems = dbContext.GarmentDeliveryOrderItems.Where(w => deliveryOrderItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            var deliveryOrderDetailIds = queryResult.Select(s => s.DODetailId).Distinct().ToList();
            var deliveryOrderDetails = dbContext.GarmentDeliveryOrderDetails.Where(w => deliveryOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.DOQuantity, s.UomUnit, s.DealQuantity, s.PricePerDealUnit }).ToList();

            var customIds = queryResult.Select(s => s.BCId).Distinct().ToList();
            var customs = dbContext.GarmentBeacukais.Where(w => customIds.Contains(w.Id)).Select(s => new { s.Id, s.BeacukaiNo, s.BeacukaiDate }).ToList();
            //var customItemIds = queryResult.Select(s => s.BCItemId).Distinct().ToList();
            //var customItems = dbContext.GarmentBeacukaiItems.Where(w => customItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();

            var unitReceiptNoteIds = queryResult.Select(s => s.URNId).Distinct().ToList();
            var unitReceiptNotes = dbContext.GarmentUnitReceiptNotes.Where(w => unitReceiptNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.URNNo, s.ReceiptDate }).ToList();
            var unitReceiptNoteItemIds = queryResult.Select(s => s.URNItemId).Distinct().ToList();
            var unitReceiptNoteItems = dbContext.GarmentUnitReceiptNoteItems.Where(w => unitReceiptNoteItemIds.Contains(w.Id)).Select(s => new { s.Id, s.ReceiptQuantity, s.UomUnit }).ToList();

            var invoiceIds = queryResult.Select(s => s.INVId).Distinct().ToList();
            var invoices = dbContext.GarmentInvoices.Where(w => invoiceIds.Contains(w.Id)).Select(s => new { s.Id, s.InvoiceNo, s.InvoiceDate, s.IncomeTaxDate, s.IncomeTaxNo, s.IncomeTaxName, s.IncomeTaxRate, s.IsPayTax, s.VatDate, s.VatNo }).ToList();
            //var invoiceItemIds = queryResult.Select(s => s.INVItemId).Distinct().ToList();
            //var invoiceItems = dbContext.GarmentInvoiceItems.Where(w => invoiceItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            //var invoiceDetailIds = queryResult.Select(s => s.INVDetailId).Distinct().ToList();
            //var invoiceDetails = dbContext.GarmentInvoiceDetails.Where(w => invoiceDetailIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();

            var internNoteIds = queryResult.Select(s => s.INId).Distinct().ToList();
            var internNotes = dbContext.GarmentInternNotes.Where(w => internNoteIds.Contains(w.Id)).Select(s => new { s.Id, s.INNo, s.INDate }).ToList();
            //var internNoteItemIds = queryResult.Select(s => s.INItemId).Distinct().ToList();
            //var internNoteItems = dbContext.GarmentInternNoteItems.Where(w => internNoteItemIds.Contains(w.Id)).Select(s => new { s.Id}).ToList();
            var internNoteDetailIds = queryResult.Select(s => s.INDetailId).Distinct().ToList();
            var internNoteDetails = dbContext.GarmentInternNoteDetails.Where(w => internNoteDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.Quantity, s.PricePerDealUnit, s.PaymentDueDate }).ToList();

            var corrections = dbContext.GarmentCorrectionNotes.Where(w => deliveryOrderIds.Contains(w.DOId)).Select(s => new { s.Id, s.DOId, s.CorrectionNo, s.CorrectionType, s.CorrectionDate }).ToList();
            var correctionIds = corrections.Select(s => s.Id).ToList();
            var correctionItems = dbContext.GarmentCorrectionNoteItems.Where(w => correctionIds.Contains(w.GCorrectionId)).Select(s => new { s.GCorrectionId, s.DODetailId, s.PriceTotalAfter, s.PriceTotalBefore, s.Quantity }).ToList();

            int i = ((page - 1) * size) + 1;
            foreach (var item in queryResult)
            {
                var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
                var purchaseRequestItem = purchaseRequestItems.FirstOrDefault(f => f.Id.Equals(item.PRItemId));

                var purchaseOrderInternal = purchaseOrderInternals.FirstOrDefault(f => f.Id.Equals(item.POId));
                var purchaseOrderInternalItem = purchaseOrderInternalItems.FirstOrDefault(f => f.Id.Equals(item.POItemId));

                var purchaseOrderExternal = purchaseOrderExternals.FirstOrDefault(f => f.Id.Equals(item.EPOId));
                var purchaseOrderExternalItem = purchaseOrderExternalItems.FirstOrDefault(f => f.Id.Equals(item.EPOItemId));

                var deliveryOrder = deliveryOrders.FirstOrDefault(f => f.Id.Equals(item.DOId));
                //var deliveryOrderItem = deliveryOrderItems.FirstOrDefault(f => f.Id.Equals(item.DOItemId));
                var deliveryOrderDetail = deliveryOrderDetails.FirstOrDefault(f => f.Id.Equals(item.DODetailId));

                var custom = customs.FirstOrDefault(f => f.Id.Equals(item.BCId));
                //var customItem = customItems.FirstOrDefault(f => f.Id.Equals(item.BCItemId));

                var unitReceiptNote = unitReceiptNotes.FirstOrDefault(f => f.Id.Equals(item.URNId));
                var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(f => f.Id.Equals(item.URNItemId));

                var invoice = invoices.FirstOrDefault(f => f.Id.Equals(item.INVId));
                //var invoiceItem = invoiceItems.FirstOrDefault(f => f.Id.Equals(item.INVItemId));
                //var invoiceDetail = invoiceDetails.FirstOrDefault(f => f.Id.Equals(item.INVDetailId));

                var internNote = internNotes.FirstOrDefault(f => f.Id.Equals(item.INId));
                //var internNoteItem = internNoteItems.FirstOrDefault(f => f.Id.Equals(item.INItemId));
                var internNoteDetail = internNoteDetails.FirstOrDefault(f => f.Id.Equals(item.INDetailId));

                var selectedCorrections = corrections.Where(w => w.DOId.Equals(item.DOId)).ToList();
                var selectedCorrectionIds = selectedCorrections.Select(s => s.Id).ToList();
                var selectedCorrectionItems = correctionItems.Where(w => selectedCorrectionIds.Contains(w.GCorrectionId)).ToList();

                var correctionNoList = new List<string>();
                var correctionDateList = new List<string>();
                var correctionRemarkList = new List<string>();
                var correctionNominalList = new List<string>();
                int j = 1;
                foreach (var selectedCorrection in selectedCorrections)
                {

                    var selectedCorrectionItem = selectedCorrectionItems.FirstOrDefault(f => f.GCorrectionId.Equals(selectedCorrection.Id) && f.DODetailId.Equals(item.DODetailId));
                    if (selectedCorrectionItem != null)
                    {
                        correctionNoList.Add($"{j}. {selectedCorrection.CorrectionNo}");
                        correctionRemarkList.Add($"{j}. {selectedCorrection.CorrectionType}");
                        correctionDateList.Add($"{j}. {selectedCorrection.CorrectionDate.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}");

                        switch (selectedCorrection.CorrectionType)
                        {
                            case "Harga Total":
                                correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore)}");
                                break;
                            case "Harga Satuan":
                                correctionNominalList.Add($"{j}. {string.Format("{0:N2}", (selectedCorrectionItem.PriceTotalAfter - selectedCorrectionItem.PriceTotalBefore) * selectedCorrectionItem.Quantity)}");
                                break;
                            case "Jumlah":
                                correctionNominalList.Add($"{j}. {string.Format("{0:N2}", selectedCorrectionItem.PriceTotalAfter)}");
                                break;
                            default:
                                break;
                        }
                        j++;
                    }
                }

                listEPO.Add(
                    new MonitoringPurchaseAllUserViewModel
                    {
                        index = i,
                        poextNo = purchaseOrderExternal?.EPONo,
                        poExtDate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.OrderDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        deliveryDate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.DeliveryDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        supplierCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierCode,
                        supplierName = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierName,
                        prNo = purchaseRequest.PRNo,
                        poSerialNumber = purchaseRequestItem.PO_SerialNumber,
                        prDate = purchaseRequest.Date.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        PrDate = purchaseRequest.Date,
                        unitName = purchaseRequest.UnitName,
                        buyerCode = purchaseRequest.BuyerCode,
                        buyerName = purchaseRequest.BuyerName,
                        ro = purchaseRequest.RONo,
                        article = purchaseRequest.Article,
                        shipmentDate = purchaseRequest.ShipmentDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        productCode = purchaseRequestItem.ProductCode,
                        productName = purchaseRequestItem.ProductName,
                        prProductRemark = purchaseRequestItem.ProductRemark,
                        poProductRemark = purchaseOrderExternalItem == null ? "" : purchaseOrderExternalItem.Remark,
                        poDefaultQty = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DefaultQuantity,
                        poDealQty = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity,
                        poDealUomUnit = purchaseOrderExternalItem == null ? "" : purchaseOrderExternalItem.DealUomUnit,
                        prBudgetPrice = purchaseRequestItem.BudgetPrice,
                        poPricePerDealUnit = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.PricePerDealUnit,
                        totalNominalPO = purchaseOrderExternalItem == null ? "" : string.Format("{0:N2}", purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit),
                        TotalNominalPO = purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit,
                        poCurrencyCode = purchaseOrderExternal == null ? "" : purchaseOrderExternal.CurrencyCode,
                        poCurrencyRate = purchaseOrderExternal == null ? 0 : purchaseOrderExternal.CurrencyRate,
                        totalNominalRp = purchaseOrderExternalItem == null && purchaseOrderExternal == null ? "" : string.Format("{0:N2}", purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit * purchaseOrderExternal.CurrencyRate),
                        TotalNominalRp = purchaseOrderExternal == null && purchaseOrderExternalItem == null ? 0 : purchaseOrderExternalItem.DealQuantity * purchaseOrderExternalItem.PricePerDealUnit * purchaseOrderExternal.CurrencyRate,
                        incomeTaxRate = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IncomeTaxRate?.ToString(),
                        paymentMethod = purchaseOrderExternal == null ? "" : purchaseOrderExternal.PaymentMethod?.ToString(),
                        paymentType = purchaseOrderExternal == null ? "" : purchaseOrderExternal.PaymentType?.ToString(),
                        paymentDueDays = purchaseOrderExternal == null ? 0 : purchaseOrderExternal.PaymentDueDays,
                        ipoDate = purchaseOrderInternal == null ? "" : purchaseOrderInternal.CreatedUtc.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        username = purchaseOrderInternal == null ? "" : purchaseOrderInternal.CreatedBy,
                        useIncomeTax = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IsIncomeTax ? "YA" : "TIDAK",
                        useVat = purchaseOrderExternal == null ? "" : purchaseOrderExternal.IsUseVat ? "YA" : "TIDAK",
                        useInternalPO = purchaseRequestItem.IsUsed ? "YA" : "TIDAK",
                        status = purchaseOrderInternalItem == null ? "" : purchaseOrderInternalItem.Status,
                        doNo = deliveryOrder == null ? "" : deliveryOrder.DONo,
                        doDate = deliveryOrder == null ? "" : deliveryOrder.DODate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        arrivalDate = deliveryOrder == null ? "" : deliveryOrder.ArrivalDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        doQty = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DOQuantity,
                        doUomUnit = deliveryOrderDetail == null ? "" : deliveryOrderDetail.UomUnit,
                        remainingDOQty = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.DealQuantity - deliveryOrderDetail.DOQuantity,
                        bcNo = custom == null ? "" : custom.BeacukaiNo,
                        bcDate = custom == null ? "" : custom.BeacukaiDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        receiptNo = unitReceiptNote == null ? "" : unitReceiptNote.URNNo,
                        receiptDate = unitReceiptNote == null ? "" : unitReceiptNote.ReceiptDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        receiptQty = unitReceiptNoteItem == null ? "" : string.Format("{0:N2}", unitReceiptNoteItem.ReceiptQuantity),
                        ReceiptQty = unitReceiptNoteItem == null ? 0 : unitReceiptNoteItem.ReceiptQuantity,
                        receiptUomUnit = unitReceiptNoteItem == null ? "" : unitReceiptNoteItem.UomUnit,
                        invoiceNo = invoice == null ? "" : invoice.InvoiceNo,
                        invoiceDate = invoice == null ? "" : invoice.InvoiceDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        incomeTaxDate = invoice == null ? "" : invoice.IncomeTaxDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        incomeTaxNo = invoice == null ? "" : invoice.IncomeTaxNo,
                        incomeTaxType = invoice == null ? "" : invoice.IncomeTaxName,
                        incomeTaxtRate = invoice == null ? "" : invoice.IncomeTaxRate.ToString(),
                        incomeTaxtValue = invoice != null && invoice.IsPayTax ? string.Format("{0:N2}", deliveryOrderDetail.DOQuantity * deliveryOrderDetail.PricePerDealUnit * invoice.IncomeTaxRate / 100) : "",
                        vatNo = invoice == null ? "" : invoice.VatNo,
                        vatDate = invoice == null ? "" : invoice.VatDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        vatValue = invoice != null && invoice.IsPayTax ? string.Format("{0:N2}", deliveryOrderDetail.DOQuantity * deliveryOrderDetail.PricePerDealUnit * 0.1) : "",
                        internNo = internNote == null ? "" : internNote.INNo,
                        internDate = internNote == null ? "" : internNote.INDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        internTotal = internNoteDetail == null ? "" : string.Format("{0:N2}", internNoteDetail.Quantity * internNoteDetail.PricePerDealUnit),
                        InternTotal = internNoteDetail == null ? 0 : internNoteDetail.Quantity * internNoteDetail.PricePerDealUnit,
                        maturityDate = internNoteDetail == null ? "" : internNoteDetail.PaymentDueDate.AddHours(offset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                        dodetailId = deliveryOrderDetail == null ? 0 : deliveryOrderDetail.Id,
                        correctionNoteNo = string.Join("\n", correctionNoList),
                        correctionDate = string.Join("\n", correctionDateList),
                        correctionRemark = string.Join("\n", correctionRemarkList),
                        valueCorrection = string.Join("\n", correctionNominalList),
                        Bon = deliveryOrder == null ? "" : deliveryOrder.BillNo,
                        BonSmall = deliveryOrder == null ? "" : deliveryOrder.PaymentBill,
                        SupplierImport = purchaseOrderExternal == null ? "" : purchaseOrderExternal.SupplierImport == true ? "IMPORT" : "LOCAL"
                    });
                i++;
            }
            //foreach (var corrections in qry.Distinct())
            //{
            //    foreach (MonitoringPurchaseAllUserViewModel data in Query.ToList())
            //    {
            //        if (corrections.Key == data.dodetailId)
            //        {
            //            data.correctionNoteNo = qry[data.dodetailId];
            //            data.correctionRemark = qryType[data.dodetailId];
            //            data.valueCorrection = (qryQty[data.dodetailId]);
            //            data.correctionDate = qryDate[data.dodetailId];
            //            listData.Add(data);
            //            break;
            //        }
            //    }
            //}

            //var op = qry;
            return listEPO;
            //return listEPO.AsQueryable();

        }


        public async Task<List<MonitoringPurchaseAllUserViewModel>> GetMonitoringPurchaseByUserReport(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order, int offset)
        {
            var Data = await GetMonitoringPurchaseByUserReportQuery(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx,dateToEx, offset, page, size);

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //	Query = Query.OrderByDescending(b => b.prDate);
            //}

            //Pageable<MonitoringPurchaseAllUserViewModel> pageable = new Pageable<MonitoringPurchaseAllUserViewModel>(Query, page - 1, size);
            //List<MonitoringPurchaseAllUserViewModel> Data = pageable.Data.ToList<MonitoringPurchaseAllUserViewModel>();
            //int TotalData = pageable.TotalCount;

            return Data;
        }

        public async Task<MemoryStream> GenerateExcelByUserPurchase(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order, int offset)
        {
            var Query = await GetMonitoringPurchaseByUserReportQuery(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, offset, 1, int.MaxValue);
            //Query = Query.OrderBy(b => b.PrDate);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Ref.PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dibuat PO Internal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NO RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Shipment GMT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Target Datang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kena PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kena PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = " PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Term Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tempo", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang (PR)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang (PO EKS)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Budget", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Budget", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "MT UANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kurs", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total RP", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima PO Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Datang Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Datang", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. Bon Kecil", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Qty Sisa", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Terima Unit", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NO PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Intern", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jatuh Tempo", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff Pembelian", DataType = typeof(String) });


            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "","", "", "", "", "", "", 0, 0, "", 0, 0, 0, "", "", 0, "", "", "", "", 0, "", "", "", "","","", "", 0, "", "", "", "", 0, "", "", "", "", "", "", "", "", 0, "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;

                    result.Rows.Add(index, item.prNo, item.prDate, item.unitName, item.poSerialNumber, item.useInternalPO, item.ro, item.article, item.buyerCode, item.buyerName, item.shipmentDate, item.poextNo, item.poExtDate, item.deliveryDate, item.useVat, item.useIncomeTax, item.incomeTaxRate, item.paymentMethod, item.paymentType, item.paymentDueDays,item.supplierCode, item.supplierName, item.SupplierImport, item.status, item.productCode, item.productName, item.prProductRemark, item.poProductRemark, item.poDefaultQty, item.poDealQty,
                        item.poDealUomUnit, item.prBudgetPrice, item.poPricePerDealUnit, item.TotalNominalPO, item.poCurrencyCode, item.poCurrencyRate, item.TotalNominalRp, item.ipoDate, item.doNo,
                        item.doDate, item.arrivalDate, item.doQty, item.doUomUnit, item.Bon, item.BonSmall, item.bcNo, item.bcDate, item.receiptNo, item.receiptDate, item.ReceiptQty, item.receiptUomUnit,
                        item.invoiceNo, item.invoiceDate, item.vatNo, item.vatDate, item.vatValue, item.incomeTaxType, item.incomeTaxtRate, item.incomeTaxNo, item.incomeTaxDate, item.incomeTaxtValue,
                        item.internNo, item.internDate, item.InternTotal, item.maturityDate, item.correctionNoteNo, item.correctionDate, item.valueCorrection, item.correctionRemark, item.username);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

        public GarmentPreSalesContractViewModel GetGarmentPreSalesContract(long Id)
        {
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync(string.Concat(APIEndpoint.Sales, GarmentPreSalesContractUri, Id)).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content) ?? new Dictionary<string, object>();
            if (response.IsSuccessStatusCode)
            {
                GarmentPreSalesContractViewModel data = JsonConvert.DeserializeObject<GarmentPreSalesContractViewModel>(result.GetValueOrDefault("data").ToString());
                return data;
            }
            else
            {
                throw new Exception(string.Concat("Error from '", GarmentPreSalesContractUri, "' : ", (string)result.GetValueOrDefault("error") ?? "- ", ". Message : ", (string)result.GetValueOrDefault("message") ?? "- ", ". Status : ", response.StatusCode, "."));
            }
        }

        public List<GarmentInternalPurchaseOrder> ReadByTagsOptimized(string tags, DateTimeOffset shipmentDateFrom, DateTimeOffset shipmentDateTo)
        {
            IQueryable<GarmentPurchaseRequest> Models = this.dbSet.AsNoTracking().AsQueryable();

            if (shipmentDateFrom != DateTimeOffset.MinValue && shipmentDateTo != DateTimeOffset.MinValue)
            {
                Models = Models.Where(m => m.ShipmentDate >= shipmentDateFrom && m.ShipmentDate <= shipmentDateTo);
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

            Models = Models
                .Where(m =>
                    (string.IsNullOrWhiteSpace(stringKeywords[0]) || m.UnitName.ToLower().Contains(stringKeywords[0])) &&
                    (string.IsNullOrWhiteSpace(stringKeywords[1]) || m.BuyerName.ToLower().Contains(stringKeywords[1])) &&
                    (string.IsNullOrWhiteSpace(stringKeywords[3]) || m.PRNo.ToLower().Contains(stringKeywords[3])) &&
                    m.IsUsed == false &&
                    m.IsValidated == true
                    )
                .Select(m => new GarmentPurchaseRequest
                {
                    Id = m.Id,
                    Date = m.Date,
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
                    Items = m.Items
                        .Where(i =>
                            i.IsUsed == false &&
                            (string.IsNullOrWhiteSpace(stringKeywords[2]) || i.CategoryName.ToLower().Contains(stringKeywords[2]))
                            ).ToList(),
                });


            var IPOModels = new List<GarmentInternalPurchaseOrder>();

            var data = Models.ToList();

            foreach (var model in data)
            {
                if (model.Items.Count() > 0)
                {
                    foreach (var item in model.Items)
                    {
                        var IPOModel = new GarmentInternalPurchaseOrder
                        {
                            PRId = model.Id,
                            PRDate = model.Date,
                            PRNo = model.PRNo,
                            RONo = model.RONo,
                            BuyerId = model.BuyerId,
                            BuyerCode = model.BuyerCode,
                            BuyerName = model.BuyerName,
                            Article = model.Article,
                            ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                            ShipmentDate = model.ShipmentDate,
                            UnitId = model.UnitId,
                            UnitCode = model.UnitCode,
                            UnitName = model.UnitName,
                            //IsPosted = false,
                            //IsClosed = false,
                            //Remark = "",
                            Items = new List<GarmentInternalPurchaseOrderItem>
                            {
                                new GarmentInternalPurchaseOrderItem
                                {
                                    GPRItemId = item.Id,
                                    PO_SerialNumber = item.PO_SerialNumber,
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    Quantity = item.Quantity,
                                    BudgetPrice = item.BudgetPrice,
                                    UomId = item.UomId,
                                    UomUnit = item.UomUnit,
                                    CategoryId = item.CategoryId,
                                    CategoryName = item.CategoryName,
                                    ProductRemark = item.ProductRemark,
                                    //Status = "PO Internal belum diorder"
                                }
                            }
                        };
                        IPOModels.Add(IPOModel);
                    }
                }
            }

            return IPOModels;
        }

	}

    public class SelectedId
    {
        public DateTimeOffset PRDate { get; set; }
        public long PRId { get; set; }
        public long PRItemId { get; set; }
        public long POId { get; set; }
        public long POItemId { get; set; }
        public long EPOId { get; set; }
        public long EPOItemId { get; set; }
        public long DOId { get; set; }
        public long DOItemId { get; set; }
        public long DODetailId { get; set; }
        public long BCId { get; set; }
        public long BCItemId { get; set; }
        public long URNId { get; set; }
        public long URNItemId { get; set; }
        public long INVId { get; set; }
        public long INVItemId { get; set; }
        public long INVDetailId { get; set; }
        public long INId { get; set; }
        public long INItemId { get; set; }
        public long INDetailId { get; set; }


    }
}
