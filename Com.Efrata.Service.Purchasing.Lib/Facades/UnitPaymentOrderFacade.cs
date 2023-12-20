using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
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
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System.Net.Http;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Microsoft.Extensions.Caching.Distributed;

namespace Com.Efrata.Service.Purchasing.Lib.Facades
{
    public class UnitPaymentOrderFacade : IUnitPaymentOrderFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<UnitPaymentOrder> dbSet;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _cacheManager;
        private readonly ICurrencyProvider _currencyProvider;
        private string USER_AGENT = "Facade";

        public UnitPaymentOrderFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<UnitPaymentOrder>();
            _serviceProvider = serviceProvider;
            _cacheManager = serviceProvider.GetService<IDistributedCache>();
            _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
        }

        //private List<IdCOAResult> Units => _cacheManager.Get(MemoryCacheConstant.Units, entry => { return new List<IdCOAResult>(); });
        //private List<IdCOAResult> Divisions => _cacheManager.Get(MemoryCacheConstant.Divisions, entry => { return new List<IdCOAResult>(); });
        //private List<CategoryCOAResult> Categories => _cacheManager.Get(MemoryCacheConstant.Categories, entry => { return new List<CategoryCOAResult>(); });

        private string _jsonCategories => _cacheManager.GetString(MemoryCacheConstant.Categories);
        private string _jsonUnits => _cacheManager.GetString(MemoryCacheConstant.Units);
        private string _jsonDivisions => _cacheManager.GetString(MemoryCacheConstant.Divisions);

        public Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UPONo", "DivisionName", "SupplierName", "Items.URNNo", "Items.DONo"
            };

            Query = QueryHelper<UnitPaymentOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitPaymentOrder
            {
                Id = s.Id,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                SupplierName = s.SupplierName,
                CategoryCode = s.CategoryCode,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                Date = s.Date,
                UPONo = s.UPONo,
                DueDate = s.DueDate,
                UseIncomeTax = s.UseIncomeTax,
                UseVat = s.UseVat,
                CurrencyCode = s.CurrencyCode,
                CurrencyDescription = s.CurrencyDescription,
                CurrencyId = s.CurrencyId,
                CurrencyRate = s.CurrencyRate,
                ImportInfo = s.ImportInfo,
                Items = s.Items.Select(i => new UnitPaymentOrderItem
                {
                    URNNo = i.URNNo,
                    DONo = i.DONo,
                    Details = i.Details.ToList()
                }).ToList(),
                CreatedBy = s.CreatedBy,
                IsPosted = s.IsPosted,
                LastModifiedUtc = s.LastModifiedUtc,
            });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentOrder> pageable = new Pageable<UnitPaymentOrder>(Query, Page - 1, Size);
            List<UnitPaymentOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public UnitPaymentOrder ReadById(int id)
        {
            var Result = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details)
                .FirstOrDefault();

            return Result;
        }

        public List<UnitPaymentOrder> ReadByEPONo(string no)
        {
            var Result = dbSet
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details).Where(m => m.Items.Any(d => d.Details.Any(f => f.EPONo == no)))
                .ToList();
            return Result;
        }

        public async Task<int> Create(UnitPaymentOrder model, string user, bool isImport, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, user, USER_AGENT);
                    model.Date = DateTimeOffset.UtcNow;
                    //model.UPONo = await GenerateNo(model, isImport, clientTimeZoneOffset);
                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);
                        foreach (var detail in item.Details)
                        {
                            SetPOItemIdEPONo(detail);
                            EntityExtension.FlagForCreate(detail, user, USER_AGENT);
                        }
                        SetPaid(item, true, user);
                    }

                    SetDueDate(model);

                    this.dbSet.Add(model);

                    Created = await dbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        foreach (var detail in item.Details)
                        {
                            SetStatus(detail, user);
                        }
                    }

                    model.Position = (int)ExpeditionPosition.PURCHASING_DIVISION;

                    model.UPONo = await GenerateNo(model, isImport, clientTimeZoneOffset);
                    Created += await dbContext.SaveChangesAsync();

                    await UpdateCreditorAccount(model);
                    Created += await EditFulfillment(model, user);
                    if (model.UseVat)
                    {
                        await AutoCreateJournalTransaction(model);
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

        public async Task<int> Update(int id, UnitPaymentOrder model, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingModel = this.dbSet.AsNoTracking()
                        .Include(d => d.Items)
                            .ThenInclude(d => d.Details)
                        .SingleOrDefault(m => m.Id == id && !m.IsDeleted);

                    if (existingModel != null && id == model.Id)
                    {
                        EntityExtension.FlagForUpdate(model, user, USER_AGENT);

                        foreach (var item in model.Items)
                        {
                            if (item.Id == 0)
                            {
                                EntityExtension.FlagForCreate(item, user, USER_AGENT);
                                foreach (var detail in item.Details)
                                {
                                    SetPOItemIdEPONo(detail);
                                    EntityExtension.FlagForCreate(detail, user, USER_AGENT);
                                }
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                                foreach (var detail in item.Details)
                                {
                                    EntityExtension.FlagForUpdate(detail, user, USER_AGENT);
                                }
                            }

                            SetPaid(item, true, user);
                        }

                        SetDueDate(model);

                        this.dbContext.Update(model);

                        foreach (var existingItem in existingModel.Items)
                        {
                            var newItem = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                            if (newItem == null)
                            {
                                EntityExtension.FlagForDelete(existingItem, user, USER_AGENT);
                                this.dbContext.UnitPaymentOrderItems.Update(existingItem);
                                foreach (var existingDetail in existingItem.Details)
                                {
                                    EntityExtension.FlagForDelete(existingDetail, user, USER_AGENT);
                                    this.dbContext.UnitPaymentOrderDetails.Update(existingDetail);
                                }

                                SetPaid(existingItem, false, user);
                            }
                        }

                        Updated = await dbContext.SaveChangesAsync();

                        foreach (var item in model.Items)
                        {
                            foreach (var detail in item.Details)
                            {
                                SetStatus(detail, user);
                            }
                        }

                        Updated += await dbContext.SaveChangesAsync();

                        await UpdateCreditorAccount(model);
                        Updated += await EditFulfillment(model, user);

                        if (model.UseVat)
                        {
                            await ReverseJournalTransaction(model.UPONo);
                            await AutoCreateJournalTransaction(model);
                        }

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

        public async Task<int> Delete(int id, string user)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = this.dbSet
                        .Include(d => d.Items)
                            .ThenInclude(d => d.Details)
                        .SingleOrDefault(m => m.Id == id && !m.IsDeleted);

                    EntityExtension.FlagForDelete(model, user, USER_AGENT);

                    foreach (var item in model.Items)
                    {
                        EntityExtension.FlagForDelete(item, user, USER_AGENT);
                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForDelete(detail, user, USER_AGENT);
                        }

                        SetPaid(item, false, user);
                    }

                    Deleted = await dbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        foreach (var detail in item.Details)
                        {
                            SetStatus(detail, user);
                        }
                    }

                    Deleted += await dbContext.SaveChangesAsync();
                    await DeleteCreditorAccount(model);
                    Deleted += await RollbackFulfillment(model, user);

                    if (model.UseVat)
                        await ReverseJournalTransaction(model.UPONo);
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

        async Task<string> GenerateNo(UnitPaymentOrder model, bool isImport, int clientTimeZoneOffset)
        {
            string Year = model.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Supplier = isImport ? "NKI" : "NKL";
            string TG = "";
            if (model.DivisionName.ToUpper().Equals("EFRATA"))
            {
                TG = "G-";
            }
            else if (model.DivisionName.ToUpper().Equals("UMUM") ||
                model.DivisionName.ToUpper().Equals("SPINNING") ||
                model.DivisionName.ToUpper().Equals("DYEING & PRINTING") ||
                model.DivisionName.ToUpper().Equals("UTILITY") ||
                model.DivisionName.ToUpper().Equals("WEAVING") ||
                model.DivisionName.ToUpper().Equals("TRADING"))
            {
                TG = "T-";
            }

            string no = $"{Year}-{Month}-{TG}{Supplier}-";
            int Padding = isImport ? 3 : 4;

            var lastNo = await dbSet.Where(w => !string.IsNullOrWhiteSpace(w.UPONo) && w.UPONo.StartsWith(no) && !w.UPONo.EndsWith("L") && !w.IsDeleted).OrderByDescending(o => o.UPONo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = int.Parse(lastNo.UPONo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        private void SetPOItemIdEPONo(UnitPaymentOrderDetail detail)
        {
            ExternalPurchaseOrderDetail EPODetail = dbContext.ExternalPurchaseOrderDetails.First(m => m.Id == detail.EPODetailId);
            detail.POItemId = EPODetail.POItemId;

            detail.EPONo = dbContext.ExternalPurchaseOrders.First(m => m.Items.Any(i => i.Id == EPODetail.EPOItemId)).EPONo;
        }

        private void SetPaid(UnitPaymentOrderItem item, bool isPaid, string username)
        {
            UnitReceiptNote unitReceiptNote = dbContext.UnitReceiptNotes.Include(a => a.Items).First(m => m.Id == item.URNId);

            foreach (var itemURN in unitReceiptNote.Items)
            {
                var detail = item.Details.FirstOrDefault(a => a.URNItemId == itemURN.Id);
                if (detail != null)
                {
                    itemURN.IsPaid = isPaid;
                }
            }
            bool flagIsPaid = true;
            foreach (var itemURNPaid in unitReceiptNote.Items)
            {
                if (itemURNPaid.IsPaid == false)
                {
                    flagIsPaid = false;
                }
            }
            unitReceiptNote.IsPaid = flagIsPaid;
            EntityExtension.FlagForUpdate(unitReceiptNote, username, USER_AGENT);
        }

        private void SetStatus(UnitPaymentOrderDetail detail, string username)
        {
            ExternalPurchaseOrderDetail EPODetail = dbContext.ExternalPurchaseOrderDetails.First(m => m.Id == detail.EPODetailId);
            InternalPurchaseOrderItem POItem = dbContext.InternalPurchaseOrderItems.First(m => m.Id == EPODetail.POItemId);

            List<long> EPODetailIds = dbContext.ExternalPurchaseOrderDetails.Where(m => m.POItemId == POItem.Id).Select(m => m.Id).ToList();
            List<long> URNItemIds = dbContext.UnitReceiptNoteItems.Where(m => EPODetailIds.Contains(m.EPODetailId)).Select(m => m.Id).ToList();

            var totalReceiptQuantity = dbContext.UnitPaymentOrderDetails.AsNoTracking().Where(m => m.IsDeleted == false && URNItemIds.Contains(m.URNItemId)).Sum(m => m.ReceiptQuantity);
            if (totalReceiptQuantity > 0)
            {
                if (totalReceiptQuantity < EPODetail.DealQuantity)
                {
                    POItem.Status = "Sudah dibuat SPB sebagian";
                }
                else
                {
                    POItem.Status = "Sudah dibuat SPB semua";
                }
            }
            else if (totalReceiptQuantity == 0)
            {
                if (EPODetail.DOQuantity >= EPODetail.DealQuantity)
                {
                    if (EPODetail.ReceiptQuantity < EPODetail.DealQuantity)
                    {
                        POItem.Status = "Barang sudah diterima Unit parsial";
                    }
                    else
                    {
                        POItem.Status = "Barang sudah diterima Unit semua";
                    }
                }
                else
                {
                    POItem.Status = "Barang sudah diterima Unit parsial";
                }
            }
            EntityExtension.FlagForUpdate(POItem, username, USER_AGENT);
        }

        private void SetDueDate(UnitPaymentOrder model)
        {
            List<DateTimeOffset> DueDates = new List<DateTimeOffset>();
            foreach (var item in model.Items)
            {
                var unitReceiptNoteDate = dbContext.UnitReceiptNotes.First(m => m.Id == item.URNId).ReceiptDate;
                foreach (var detail in item.Details)
                {
                    var PaymentDueDays = dbContext.ExternalPurchaseOrders.First(m => m.EPONo.Equals(detail.EPONo)).PaymentDueDays;
                    DueDates.Add(unitReceiptNoteDate.AddDays(Double.Parse(PaymentDueDays ?? "0")));
                }
            }
            model.DueDate = DueDates.Max();
        }

        public Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadSpb(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UPONo", "DivisionName", "SupplierName", "Items.URNNo", "Items.DONo"
            };

            Query = QueryHelper<UnitPaymentOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitPaymentOrder
            {
                Id = s.Id,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                SupplierName = s.SupplierName,
                CategoryId = s.CategoryId,
                CategoryCode = s.CategoryCode,
                CategoryName = s.CategoryName,
                CurrencyId = s.CurrencyId,
                CurrencyCode = s.CurrencyCode,
                CurrencyRate = s.CurrencyRate,
                CurrencyDescription = s.CurrencyDescription,
                PaymentMethod = s.PaymentMethod,
                InvoiceNo = s.InvoiceNo,
                InvoiceDate = s.InvoiceDate,
                PibNo = s.PibNo,
                UseIncomeTax = s.UseIncomeTax,
                IncomeTaxId = s.IncomeTaxId,
                IncomeTaxName = s.IncomeTaxName,
                IncomeTaxRate = s.IncomeTaxRate,
                IncomeTaxNo = s.IncomeTaxNo,
                IncomeTaxDate = s.IncomeTaxDate,
                UseVat = s.UseVat,
                VatNo = s.VatNo,
                VatDate = s.VatDate,
                Remark = s.Remark,
                DueDate = s.DueDate,
                Date = s.Date,
                UPONo = s.UPONo,
                Items = s.Items.Select(i => new UnitPaymentOrderItem
                {
                    UPOId = i.UPOId,
                    URNId = i.URNId,
                    URNNo = i.URNNo,
                    DOId = i.DOId,
                    DONo = i.DONo,
                    Details = i.Details.Select(j => new UnitPaymentOrderDetail
                    {
                        Id = j.Id,
                        UPOItemId = j.UPOItemId,
                        URNItemId = j.URNItemId,
                        EPONo = j.EPONo,
                        PRId = j.PRId,
                        PRNo = j.PRNo,
                        PRItemId = j.PRItemId,
                        ProductId = j.ProductId,
                        ProductCode = j.ProductCode,
                        ProductName = j.ProductName,
                        ProductRemark = j.ProductRemark,
                        ReceiptQuantity = j.ReceiptQuantity,
                        UomId = j.UomId,
                        UomUnit = j.UomUnit,
                        PricePerDealUnit = j.PricePerDealUnit,
                        PriceTotal = j.PriceTotal,
                        PricePerDealUnitCorrection = j.PricePerDealUnitCorrection,
                        PriceTotalCorrection = j.PriceTotalCorrection,
                        QuantityCorrection = j.QuantityCorrection,
                        //Duedate = s.DueDate,
                    }).ToList()
                }).ToList(),
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Position = s.Position
            });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentOrder> pageable = new Pageable<UnitPaymentOrder>(Query, Page - 1, Size);
            List<UnitPaymentOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadPositionFiltered(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UPONo", "DivisionName", "SupplierName", "Items.URNNo", "Items.DONo"
            };

            Query = QueryHelper<UnitPaymentOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitPaymentOrder
            {
                Id = s.Id,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                SupplierName = s.SupplierName,
                CategoryId = s.CategoryId,
                CategoryCode = s.CategoryCode,
                CategoryName = s.CategoryName,
                CurrencyId = s.CurrencyId,
                CurrencyCode = s.CurrencyCode,
                CurrencyRate = s.CurrencyRate,
                CurrencyDescription = s.CurrencyDescription,
                PaymentMethod = s.PaymentMethod,
                InvoiceNo = s.InvoiceNo,
                InvoiceDate = s.InvoiceDate,
                PibNo = s.PibNo,
                UseIncomeTax = s.UseIncomeTax,
                IncomeTaxId = s.IncomeTaxId,
                IncomeTaxName = s.IncomeTaxName,
                IncomeTaxRate = s.IncomeTaxRate,
                IncomeTaxNo = s.IncomeTaxNo,
                IncomeTaxDate = s.IncomeTaxDate,
                UseVat = s.UseVat,
                VatNo = s.VatNo,
                VatDate = s.VatDate,
                Remark = s.Remark,
                DueDate = s.DueDate,
                Date = s.Date,
                UPONo = s.UPONo,
                Position = s.Position,
                Items = s.Items.Select(i => new UnitPaymentOrderItem
                {
                    UPOId = i.UPOId,
                    URNId = i.URNId,
                    URNNo = i.URNNo,
                    DOId = i.DOId,
                    DONo = i.DONo,
                    Details = i.Details.Select(j => new UnitPaymentOrderDetail
                    {
                        Id = j.Id,
                        UPOItemId = j.UPOItemId,
                        URNItemId = j.URNItemId,
                        EPONo = j.EPONo,
                        PRId = j.PRId,
                        PRNo = j.PRNo,
                        PRItemId = j.PRItemId,
                        ProductId = j.ProductId,
                        ProductCode = j.ProductCode,
                        ProductName = j.ProductName,
                        ProductRemark = j.ProductRemark,
                        ReceiptQuantity = j.ReceiptQuantity,
                        UomId = j.UomId,
                        UomUnit = j.UomUnit,
                        PricePerDealUnit = j.PricePerDealUnit,
                        PriceTotal = j.PriceTotal,
                        PricePerDealUnitCorrection = j.PricePerDealUnitCorrection,
                        PriceTotalCorrection = j.PriceTotalCorrection,
                        QuantityCorrection = j.QuantityCorrection,
                        //Duedate = s.DueDate,
                    }).ToList()
                }).ToList(),
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
            });

            Dictionary<string, List<int>> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(Filter);
            if (FilterDictionary.Keys.FirstOrDefault() == "position")
            {
                List<int> filteredPosition = FilterDictionary.GetValueOrDefault("position");
                Query = Query.Where(x => filteredPosition.Contains(x.Position));
            }

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentOrder> pageable = new Pageable<UnitPaymentOrder>(Query, Page - 1, Size);
            List<UnitPaymentOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> ReadSpbForVerification(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentOrder> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UPONo", "DivisionName", "SupplierName", "Items.URNNo", "Items.DONo"
            };

            Query = QueryHelper<UnitPaymentOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Where(a => a.Position == 1 || a.Position == 6).Select(s => new UnitPaymentOrder
            {
                Id = s.Id,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                SupplierName = s.SupplierName,
                CategoryId = s.CategoryId,
                CategoryCode = s.CategoryCode,
                CategoryName = s.CategoryName,
                CurrencyId = s.CurrencyId,
                CurrencyCode = s.CurrencyCode,
                CurrencyRate = s.CurrencyRate,
                CurrencyDescription = s.CurrencyDescription,
                PaymentMethod = s.PaymentMethod,
                InvoiceNo = s.InvoiceNo,
                InvoiceDate = s.InvoiceDate,
                PibNo = s.PibNo,
                UseIncomeTax = s.UseIncomeTax,
                IncomeTaxId = s.IncomeTaxId,
                IncomeTaxName = s.IncomeTaxName,
                IncomeTaxRate = s.IncomeTaxRate,
                IncomeTaxNo = s.IncomeTaxNo,
                IncomeTaxDate = s.IncomeTaxDate,
                IncomeTaxBy = s.IncomeTaxBy,
                UseVat = s.UseVat,
                VatId = s.VatId,
                VatRate = s.VatRate,
                VatNo = s.VatNo,
                VatDate = s.VatDate,
                Remark = s.Remark,
                DueDate = s.DueDate,
                Date = s.Date,
                UPONo = s.UPONo,
                Items = s.Items.Select(i => new UnitPaymentOrderItem
                {
                    UPOId = i.UPOId,
                    URNId = i.URNId,
                    URNNo = i.URNNo,
                    DOId = i.DOId,
                    DONo = i.DONo,
                    Details = i.Details.Select(j => new UnitPaymentOrderDetail
                    {
                        Id = j.Id,
                        UPOItemId = j.UPOItemId,
                        URNItemId = j.URNItemId,
                        EPONo = j.EPONo,
                        PRId = j.PRId,
                        PRNo = j.PRNo,
                        PRItemId = j.PRItemId,
                        ProductId = j.ProductId,
                        ProductCode = j.ProductCode,
                        ProductName = j.ProductName,
                        ProductRemark = j.ProductRemark,
                        ReceiptQuantity = j.ReceiptQuantity,
                        UomId = j.UomId,
                        UomUnit = j.UomUnit,
                        PricePerDealUnit = j.PricePerDealUnit,
                        PriceTotal = j.PriceTotal,
                        PricePerDealUnitCorrection = j.PricePerDealUnitCorrection,
                        PriceTotalCorrection = j.PriceTotalCorrection,
                        QuantityCorrection = j.QuantityCorrection,
                        //Duedate = s.DueDate,
                    }).ToList()
                }).ToList(),
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Position = s.Position
            });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentOrder> pageable = new Pageable<UnitPaymentOrder>(Query, Page - 1, Size);
            List<UnitPaymentOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        #region ForPDF

        public UnitReceiptNote GetUnitReceiptNote(long URNId)
        {
            return dbContext.UnitReceiptNotes.Single(m => m.Id == URNId);
        }

        public ExternalPurchaseOrder GetExternalPurchaseOrder(string EPONo)
        {
            return dbContext.ExternalPurchaseOrders.Single(m => m.EPONo.Equals(EPONo));
        }

        #endregion

        #region MonitoringAll
        public IQueryable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel> GetReportQueryAll(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.UnitPaymentOrders
                         join b in dbContext.UnitPaymentOrderItems on a.Id equals b.UPOId
                         join c in dbContext.UnitPaymentOrderDetails on b.Id equals c.UPOItemId
                         join d in dbContext.PurchaseRequests on c.PRId equals d.Id
                         join e in dbContext.UnitReceiptNotes on b.URNNo equals e.URNNo
                         join f in dbContext.ExternalPurchaseOrders on c.EPONo equals f.EPONo

                         //Conditions
                         where a.IsDeleted == false
                                && b.IsDeleted == false
                                && c.IsDeleted == false
                                && d.IsDeleted == false
                                && e.IsDeleted == false
                                && f.IsDeleted == false
                                && a.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? a.SupplierId : supplierId)
                                && a.UPONo == (string.IsNullOrWhiteSpace(noSPB) ? a.UPONo : noSPB)
                                && e.UnitId == (string.IsNullOrWhiteSpace(unitId) ? e.UnitId : unitId)
                                && a.Date.AddHours(offset).Date >= DateFrom.Date
                                && a.Date.AddHours(offset).Date <= DateTo.Date

                         select new ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel
                         {
                             tglspb = a.Date,
                             nospb = a.UPONo,
                             namabrg = c.ProductName,
                             satuan = c.UomUnit,
                             jumlah = c.ReceiptQuantity,
                             hrgsat = c.PricePerDealUnit,
                             jumlahhrg = c.PriceTotal,
                             ppn = a.UseVat == true ? (c.PriceTotal * (a.VatRate / 100)) : 0,
                             total = c.PriceTotal + (a.UseVat == true ? (c.PriceTotal * (a.VatRate / 100)) : 0),
                             pph = (a.IncomeTaxRate * c.PriceTotal) / 100,
                             tglpr = d.Date,
                             nopr = c.PRNo,
                             tglbon = e.ReceiptDate,
                             nobon = b.URNNo,
                             tglinv = a.InvoiceDate,
                             noinv = a.InvoiceNo,
                             kodesupplier = a.SupplierCode,
                             supplier = a.SupplierName,
                             div = a.DivisionName,
                             adm = a.CreatedBy,
                             term = a.PaymentMethod,
                             matauang = a.CurrencyCode,
                             kategori = a.CategoryName,
                             unit = e.UnitName,
                             jt = e.ReceiptDate.AddDays(Convert.ToDouble(f.PaymentDueDays)),
                             qtycorrection = c.QuantityCorrection,
                             pricecorrection = c.PricePerDealUnitCorrection,
                             totalpricecorrection = c.PriceTotalCorrection,



                         });
            return Query;
        }

        public Tuple<List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel>, int> GetReportAll(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQueryAll(unitId, supplierId, noSPB, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.no);
            }

            Pageable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel> pageable = new Pageable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel>(Query, page - 1, size);
            List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel> Data = pageable.Data.ToList<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQueryAll(unitId, supplierId,noSPB, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.tglspb);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TglSPB", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NoSPB", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NamaBrg", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jml", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "HrgSat", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "JmlHrg", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ppn", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pph", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "TglPR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NoPR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TglBon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NoBon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TglInv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NoInv", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KdSupp", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supp", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Div", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "ADM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Term Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "MtUang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "QtyKoreksi", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "HargaKoreksi", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "TotalKoreksi", DataType = typeof(Double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, 0, 0, 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string tglspb = item.tglspb == null ? "-" : item.tglspb.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    string tglpr = item.tglpr == null ? "-" : item.tglpr.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    string tglbon = item.tglbon == null ? "-" : item.tglbon.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    string tglinv = item.tglinv == null ? "-" : item.tglinv.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    string jt = item.jt == null ? "-" : item.jt.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, tglspb, item.nospb, item.namabrg, item.satuan, item.jumlah, item.hrgsat, item.jumlahhrg, item.ppn, item.total, item.pph, tglpr, item.nopr, tglbon
                       , item.nobon, tglinv, item.noinv, jt, item.kodesupplier, item.supplier, item.unit, item.div, item.adm, item.term, item.matauang, item.kategori, item.qtycorrection, item.pricecorrection, item.totalpricecorrection);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

        public IQueryable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderGenerateDataViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in dbContext.UnitPaymentOrders
                         join b in dbContext.UnitPaymentOrderItems on a.Id equals b.UPOId
                         join c in dbContext.UnitPaymentOrderDetails on b.Id equals c.UPOItemId
                         join d in dbContext.UnitReceiptNotes on b.URNId equals d.Id
                         where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && d.IsDeleted == false &&
                               a.Date.AddHours(offset).Date >= DateFrom.Date &&
                               a.Date.AddHours(offset).Date <= DateTo.Date
                         select new ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderGenerateDataViewModel
                         {
                             UPONo = a.UPONo,
                             UPODate = a.Date,
                             SupplierCode = a.SupplierCode,
                             SupplierName = a.SupplierName,
                             CategoryName = a.CategoryName,
                             InvoiceNo = a.InvoiceNo,
                             InvoiceDate = a.InvoiceDate,
                             DueDate = a.DueDate,
                             UPORemark = a.Remark,
                             UseVat = a.UseVat ? "YA " : "TIDAK",
                             VatNo = a.VatNo,
                             VatDate = a.VatDate,
                             VatRate = a.VatRate,
                             UseIncomeTax = a.UseIncomeTax ? "YA " : "TIDAK",
                             IncomeTaxName = a.IncomeTaxName,
                             IncomeTaxRate = a.IncomeTaxRate,
                             IncomeTaxNo = a.IncomeTaxNo,
                             IncomeTaxDate = a.IncomeTaxDate,
                             EPONO = c.EPONo,
                             PRNo = c.PRNo,
                             AccountNo = "",
                             IncludedPPN = "TIDAK",
                             Printed = "-",
                             ProductCode = c.ProductCode,
                             ProductName = c.ProductName,
                             ReceiptQty = c.ReceiptQuantity,
                             UOMUnit = c.UomUnit,
                             PricePerDealUnit = c.PricePerDealUnit,
                             CurrencyCode = a.CurrencyCode,
                             CurrencyRate = a.CurrencyRate,
                             PriceTotal = c.PriceTotal,
                             URNNo = b.URNNo,
                             URNDate = d.ReceiptDate,
                             UserCreated = a.CreatedBy,
                             PaymentMethod = a.PaymentMethod,
                         }
                         );
            return Query;
        }

        public MemoryStream GenerateDataExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.UPONo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR NOTA KREDIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL NOTA KREDIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KATEGORI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR INVOICE", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL INVOICE", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL JATUH TEMPO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR FAKTUR PAJAK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL FAKTUR PAJAK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JENIS PPH", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "% PPH", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR FAKTUR PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL FAKTUR PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PO EXTERNAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PURCHASE REQUEST", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR ACCOUNT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "HARGA SATUAN BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "INCLUDED PPN(Y / N)", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "MATA UANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RATE", DataType = typeof(double) });

            result.Columns.Add(new DataColumn() { ColumnName = "HARGA TOTAL BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON TERIMA UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BON TERIMA UNIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PRINTED_FLAG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "USER INPUT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TERM", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "RATE PPN", DataType = typeof(double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", "", "", "", "", 0, "", 0, "", "", 0, 0, "", "", "", "", "", 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string UPODate = item.UPODate == new DateTime(1970, 1, 1) ? "-" : item.UPODate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string InvoiceDate = item.InvoiceDate == null ? "-" : item.InvoiceDate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string DueDate = item.DueDate == new DateTime(1970, 1, 1) ? "-" : item.DueDate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string VatDate = item.VatDate == DateTimeOffset.MinValue ? "-" : item.VatDate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxDate = item.IncomeTaxDate == DateTimeOffset.MinValue ? "-" : item.IncomeTaxDate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string URNDate = item.URNDate == null ? "-" : item.URNDate.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.UPONo, UPODate, item.SupplierCode, item.SupplierName, item.CategoryName, item.InvoiceNo, InvoiceDate,
                                    DueDate, item.UPORemark, item.UseVat, item.VatNo, VatDate, item.UseIncomeTax, item.IncomeTaxName,
                                    item.IncomeTaxRate, item.IncomeTaxNo, IncomeTaxDate, item.EPONO, item.PRNo, item.AccountNo, item.ProductCode,
                                    item.ProductName, item.ReceiptQty, item.UOMUnit, item.PricePerDealUnit, item.IncludedPPN, item.CurrencyCode, item.CurrencyRate,
                                    item.PriceTotal, item.URNNo, URNDate, item.Printed, item.UserCreated, item.PaymentMethod, item.VatRate);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }

        private async Task<int> EditFulfillment(UnitPaymentOrder model, string username)
        {
            var internalPOFacade = _serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;

            foreach (var item in model.Items)
            {
                foreach (var detail in item.Details)
                {
                    var fulfillment = await dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.UnitReceiptNoteItemId == detail.URNItemId);

                    if (fulfillment != null)
                    {
                        fulfillment.UnitPaymentOrderDetailId = detail.Id;
                        fulfillment.UnitPaymentOrderId = model.Id;
                        fulfillment.UnitPaymentOrderItemId = item.Id;
                        fulfillment.InvoiceDate = model.InvoiceDate;
                        fulfillment.InvoiceNo = model.InvoiceNo;
                        fulfillment.InterNoteDate = model.Date;
                        fulfillment.InterNoteNo = model.UPONo;
                        fulfillment.InterNoteValue = detail.PriceTotal;
                        fulfillment.InterNoteDueDate = model.DueDate;
                        fulfillment.UnitPaymentOrderUseVat = model.UseVat;
                        fulfillment.UnitPaymentOrderUseIncomeTax = model.UseIncomeTax;
                        fulfillment.UnitPaymentOrderIncomeTaxDate = DateTimeOffset.MinValue;
                        fulfillment.UnitPaymentOrderIncomeTaxNo = model.IncomeTaxNo;
                        fulfillment.UnitPaymentOrderIncomeTaxRate = model.IncomeTaxRate;
                        fulfillment.UnitPaymentOrderVatDate = model.VatDate;
                        fulfillment.UnitPaymentOrderVatNo = model.VatNo;

                        count += await internalPOFacade.UpdateFulfillmentAsync(fulfillment.Id, fulfillment, username);
                    }
                }

            }


            return count;
        }

        private async Task<int> RollbackFulfillment(UnitPaymentOrder model, string username)
        {
            var internalPOFacade = _serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;
            foreach (var item in model.Items)
            {
                foreach (var detail in item.Details)
                {
                    var fulfillment = await dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.UnitPaymentOrderId == model.Id && x.UnitPaymentOrderItemId == item.Id && x.UnitPaymentOrderDetailId == detail.Id);

                    if (fulfillment != null)
                    {
                        fulfillment.UnitPaymentOrderDetailId = 0;
                        fulfillment.UnitPaymentOrderId = 0;
                        fulfillment.UnitPaymentOrderItemId = 0;
                        fulfillment.InvoiceDate = DateTimeOffset.MinValue;
                        fulfillment.InvoiceNo = null;
                        fulfillment.InterNoteDate = DateTimeOffset.MinValue;
                        fulfillment.InterNoteNo = null;
                        fulfillment.InterNoteValue = 0;
                        fulfillment.InterNoteDueDate = DateTimeOffset.MinValue;
                        fulfillment.UnitPaymentOrderUseVat = false;
                        fulfillment.UnitPaymentOrderUseIncomeTax = false;
                        fulfillment.UnitPaymentOrderIncomeTaxDate = DateTimeOffset.MinValue;
                        fulfillment.UnitPaymentOrderIncomeTaxNo = null;
                        fulfillment.UnitPaymentOrderIncomeTaxRate = 0;
                        fulfillment.UnitPaymentOrderVatDate = DateTimeOffset.MinValue;
                        fulfillment.UnitPaymentOrderVatNo = null;

                        count += await internalPOFacade.UpdateFulfillmentAsync(fulfillment.Id, fulfillment, username);
                    }
                }

            }
            return count;
        }

        private async Task UpdateCreditorAccount(UnitPaymentOrder model)
        {
            List<CreditorAccountViewModel> data = new List<CreditorAccountViewModel>();

            foreach (var item in model.Items)
            {
                var purchaseRequestNos = item.Details.Select(entity => entity.PRNo).ToList();
                var externalPurchaseOrderNos = item.Details.Select(entity => entity.EPONo).ToList();
                var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(entity => externalPurchaseOrderNos.Contains(entity.EPONo));
                var purchaseRequests = dbContext.PurchaseRequests.Where(entity => purchaseRequestNos.Contains(entity.No));
                var categoryIds = purchaseRequests.Select(element => element.CategoryId).Distinct().ToList();
                var tax = externalPurchaseOrders.GroupBy(element => new { element.UseVat, element.UseIncomeTax, element.IncomeTaxId, element.IncomeTaxName, element.IncomeTaxRate, element.IncomeTaxBy }).Select(element => new {
                    element.Key.UseVat,
                    element.Key.UseIncomeTax,
                    element.Key.IncomeTaxId,
                    element.Key.IncomeTaxName,
                    element.Key.IncomeTaxRate,
                    element.Key.IncomeTaxBy,
                    EpoNos = element.Select(s => s.EPONo)
                }).ToList();
                string epoNos = "";

                if (categoryIds.Count > 1)
                {
                    List<string> newExternalPONos = new List<string>();

                    foreach (var epo in categoryIds)
                    {
                        var newPurchaseRequests = purchaseRequests.Where(entity => entity.CategoryId == epo).Select(entity => entity.No).ToList();
                        var newExternalPOItems = dbContext.ExternalPurchaseOrderItems.Where(entity => newPurchaseRequests.Contains(entity.PRNo)).Select(entity => entity.EPOId).ToList();
                        var newExternalPOs = dbContext.ExternalPurchaseOrders.Where(entity => newExternalPOItems.Contains(entity.Id) && externalPurchaseOrderNos.Contains(entity.EPONo));
                        newExternalPONos = newExternalPOs.Select(entity => entity.EPONo).ToList();
                        //epoNos = string.Join('\n', newExternalPONos.Select(entity => $"- {entity}"));
                    }

                    epoNos = string.Join('\n', newExternalPONos.Select(entity => $"- {entity}"));
                }
                else
                {
                    if (tax.Count > 1)
                    {
                        List<string> newExternalPONos = new List<string>();

                        foreach (var epo in tax)
                        {
                            var newExternalPOItems = dbContext.ExternalPurchaseOrderItems.Where(entity => epo.EpoNos.Contains(entity.PRNo)).Select(entity => entity.EPOId).ToList();
                            var newExternalPOs = dbContext.ExternalPurchaseOrders.Where(entity => newExternalPOItems.Contains(entity.Id) && externalPurchaseOrderNos.Contains(entity.EPONo));
                            newExternalPONos = newExternalPOs.Select(entity => entity.EPONo).ToList();
                            //epoNos = string.Join('\n', newExternalPONos.Select(entity => $"- {entity}"));
                        }

                        epoNos = string.Join('\n', newExternalPONos.Select(entity => $"- {entity}"));
                    }
                    else
                    {
                        epoNos = string.Join('\n', externalPurchaseOrderNos.Select(entity => $"- {entity}"));
                    }
                }

                data.Add(new CreditorAccountViewModel()
                {
                    Code = item.URNNo,
                    SupplierCode = model.SupplierCode,
                    InvoiceNo = model.InvoiceNo,
                    ExternalPurchaseOrderNo = epoNos
                });
            }

            var postedData = new
            {
                MemoNo = model.UPONo,
                model.InvoiceNo,
                MemoDate = model.Date,
                CreditorAccounts = data
            };

            string creditorAccountUri = "creditor-account/unit-payment-order";
            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{creditorAccountUri}", new StringContent(JsonConvert.SerializeObject(postedData).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        private async Task DeleteCreditorAccount(UnitPaymentOrder model)
        {
            List<CreditorAccountViewModel> data = new List<CreditorAccountViewModel>();

            foreach (var item in model.Items)
            {
                data.Add(new CreditorAccountViewModel()
                {
                    Code = item.URNNo,
                    SupplierCode = model.SupplierCode,
                    InvoiceNo = model.InvoiceNo
                });
            }

            var postedData = new
            {
                CreditorAccounts = data
            };

            string creditorAccountUri = "creditor-account/unit-payment-order";
            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{creditorAccountUri}", new StringContent(JsonConvert.SerializeObject(postedData).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        private async Task AutoCreateJournalTransaction(UnitPaymentOrder model)
        {
            var journalTransactionToPost = new JournalTransaction()
            {
                Date = model.Date,
                Description = "Surat Perintah Bayar",
                ReferenceNo = model.UPONo,
                Status = "POSTED",
                Items = new List<JournalTransactionItem>(),
                Remark = $"{model.SupplierCode} - {model.SupplierName}"
            };

            var journalDebitItems = new List<JournalTransactionItem>();
            var journalCreditItems = new List<JournalTransactionItem>();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var divisions = JsonConvert.DeserializeObject<List<IdCOAResult>>(_jsonDivisions ?? "[]", jsonSerializerSettings);
            var units = JsonConvert.DeserializeObject<List<IdCOAResult>>(_jsonUnits ?? "[]", jsonSerializerSettings);
            var categories = JsonConvert.DeserializeObject<List<CategoryCOAResult>>(_jsonCategories ?? "[]", jsonSerializerSettings);

            //journal
            var inVATCOA = "1509.00";

            int.TryParse(model.DivisionId, out var divisionId);
            var division = divisions.FirstOrDefault(entity => entity.Id == divisionId);
            if (division == null || string.IsNullOrWhiteSpace(division.COACode))
            {
                division = new IdCOAResult()
                {
                    COACode = "0"
                };
            }

            var urnIds = model.Items.Select(item => item.URNId).ToList();
            var unitReceiptNotes = dbContext.UnitReceiptNotes.Include(entity => entity.Items).Where(entity => urnIds.Contains(entity.Id)).ToList();

            var prIds = unitReceiptNotes.SelectMany(entity => entity.Items).Select(item => item.PRId).ToList();
            var purchaseRequests = dbContext.PurchaseRequests.Include(entity => entity.Items).Where(entity => prIds.Contains(entity.Id)).ToList();

            foreach (var item in model.Items)
            {
                var unitReceiptNote = unitReceiptNotes.FirstOrDefault(entity => entity.Id == item.URNId);

                var externalPurchaseOrderIds = unitReceiptNote.Items.Select(s => s.EPOId).ToList();
                var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(w => externalPurchaseOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.IncomeTaxId, s.UseIncomeTax, s.IncomeTaxName, s.IncomeTaxRate, s.CurrencyCode, s.CurrencyRate }).ToList();

                int.TryParse(unitReceiptNote.UnitId, out var unitId);
                var unit = units.FirstOrDefault(entity => entity.Id == unitId);
                if (unit == null || string.IsNullOrWhiteSpace(unit.COACode))
                {
                    unit = new IdCOAResult()
                    {
                        COACode = "00"
                    };
                }

                var urnItemIds = item.Details.Select(element => element.URNItemId).ToList();
                var unitReceiptNoteItems = unitReceiptNote.Items.Where(element => urnItemIds.Contains(element.Id)).ToList();
                foreach (var detail in item.Details)
                {
                    var urnItem = unitReceiptNoteItems.FirstOrDefault(element => element.Id == detail.URNItemId);
                    var purchaseRequest = purchaseRequests.FirstOrDefault(entity => entity.Id == detail.PRId);
                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(entity => entity.Id == urnItem.EPOId);

                    var currency = await _currencyProvider.GetCurrencyByCurrencyCode(externalPurchaseOrder.CurrencyCode);
                    var currencyRate = currency != null ? (decimal)currency.Rate.GetValueOrDefault() : (decimal)externalPurchaseOrder.CurrencyRate;

                    var grandTotal = Convert.ToDouble(detail.ReceiptQuantity * detail.PricePerDealUnit * (double)currencyRate);

                    int.TryParse(purchaseRequest.CategoryId, out var categoryId);
                    var category = categories.FirstOrDefault(entity => entity.Id == categoryId);
                    if (category == null)
                    {
                        category = new CategoryCOAResult()
                        {
                            ImportDebtCOA = "9999.00",
                            LocalDebtCOA = "9999.00",
                            PurchasingCOA = "9999.00",
                            StockCOA = "9999.00"
                        };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(category.ImportDebtCOA))
                        {
                            category.ImportDebtCOA = "9999.00";
                        }
                        if (string.IsNullOrEmpty(category.LocalDebtCOA))
                        {
                            category.LocalDebtCOA = "9999.00";
                        }
                        if (string.IsNullOrEmpty(category.PurchasingCOA))
                        {
                            category.PurchasingCOA = "9999.00";
                        }
                        if (string.IsNullOrEmpty(category.StockCOA))
                        {
                            category.StockCOA = "9999.00";
                        }
                    }

                    var totalVAT = model.VatRate * grandTotal;
                    journalCreditItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = unitReceiptNote.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Credit = (decimal)totalVAT
                    });

                    journalDebitItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{inVATCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Debit = (decimal)totalVAT,
                        Remark = model.VatNo
                    });
                }
            }


            journalDebitItems = journalDebitItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
            {
                COA = new COA()
                {
                    Code = s.Key
                },
                Debit = s.Sum(sum => Math.Round(sum.Debit.GetValueOrDefault(), 4)),
                Credit = 0,
                Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
            }).ToList();
            journalTransactionToPost.Items.AddRange(journalDebitItems);

            journalCreditItems = journalCreditItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
            {
                COA = new COA()
                {
                    Code = s.Key
                },
                Debit = 0,
                Credit = s.Sum(sum => Math.Round(sum.Credit.GetValueOrDefault(), 4)),
                Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
            }).ToList();
            journalTransactionToPost.Items.AddRange(journalCreditItems);

            if (journalTransactionToPost.Items.Any(item => item.COA.Code.Split(".").FirstOrDefault().Equals("9999")))
                journalTransactionToPost.Status = "DRAFT";

            string journalTransactionUri = "journal-transactions";
            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(journalTransactionToPost).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        private async Task ReverseJournalTransaction(string referenceNo)
        {
            string journalTransactionUri = $"journal-transactions/reverse-transactions/{referenceNo}";
            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(new object()).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        private List<long> GetUPOIds(List<long> poExtIds)
        {
            if (poExtIds.Count > 0)
            {
                var epoItemIds = dbContext.ExternalPurchaseOrderItems.Where(entity => poExtIds.Contains(entity.EPOId)).Select(entity => entity.Id).ToList();
                var epoDetailIds = dbContext.ExternalPurchaseOrderDetails.Where(entity => epoItemIds.Contains(entity.EPOItemId)).Select(entity => entity.Id).ToList();
                var upoItemIds = dbContext.UnitPaymentOrderDetails.Where(entity => epoDetailIds.Contains(entity.EPODetailId)).Select(entity => entity.UPOItemId).ToList();
                return dbContext.UnitPaymentOrderItems.Where(entity => upoItemIds.Contains(entity.Id)).Select(entity => entity.UPOId).ToList();
            }
            else
            {
                poExtIds = dbContext.ExternalPurchaseOrders.Where(entity => entity.POCashType == "VB").Select(entity => entity.Id).ToList();
                var epoItemIds = dbContext.ExternalPurchaseOrderItems.Where(entity => poExtIds.Contains(entity.EPOId)).Select(entity => entity.Id).ToList();
                var epoDetailIds = dbContext.ExternalPurchaseOrderDetails.Where(entity => epoItemIds.Contains(entity.EPOItemId)).Select(entity => entity.Id).ToList();
                var upoItemIds = dbContext.UnitPaymentOrderDetails.Where(entity => epoDetailIds.Contains(entity.EPODetailId)).Select(entity => entity.UPOItemId).ToList();
                return dbContext.UnitPaymentOrderItems.Where(entity => upoItemIds.Contains(entity.Id)).Select(entity => entity.UPOId).ToList();
            }
        }

        public Tuple<List<UnitPaymentOrder>, int, Dictionary<string, string>> Read(List<long> poExtIds, int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentOrder> Query = this.dbSet;

            var upoIds = GetUPOIds(poExtIds);
            Query = Query.Where(entity => upoIds.Contains(entity.Id));

            List<string> searchAttributes = new List<string>()
            {
                "UPONo", "DivisionName", "SupplierName", "Items.URNNo", "Items.DONo"
            };

            Query = QueryHelper<UnitPaymentOrder>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitPaymentOrder
            {
                Id = s.Id,
                DivisionId = s.DivisionId,
                DivisionCode = s.DivisionCode,
                DivisionName = s.DivisionName,
                SupplierId = s.SupplierId,
                SupplierCode = s.SupplierCode,
                SupplierName = s.SupplierName,
                CategoryCode = s.CategoryCode,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                Date = s.Date,
                UPONo = s.UPONo,
                DueDate = s.DueDate,
                UseIncomeTax = s.UseIncomeTax,
                UseVat = s.UseVat,
                CurrencyCode = s.CurrencyCode,
                CurrencyDescription = s.CurrencyDescription,
                CurrencyId = s.CurrencyId,
                CurrencyRate = s.CurrencyRate,
                Items = s.Items.Select(i => new UnitPaymentOrderItem
                {
                    URNNo = i.URNNo,
                    DONo = i.DONo,
                    Details = i.Details.ToList()
                }).ToList(),
                CreatedBy = s.CreatedBy,
                IsPosted = s.IsPosted,
                LastModifiedUtc = s.LastModifiedUtc,
            });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentOrder>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentOrder> pageable = new Pageable<UnitPaymentOrder>(Query, Page - 1, Size);
            List<UnitPaymentOrder> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }
        //
        #region MonitoringPPN_&_PPH
        public IQueryable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel> GetReportQueryTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            DateTime taxDateFrom = taxdateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)taxdateFrom;
            DateTime taxDateTo = taxdateTo == null ? DateTime.Now : (DateTime)taxdateTo;

            var Query = (from a in dbContext.UnitPaymentOrders
                         join b in dbContext.UnitPaymentOrderItems on a.Id equals b.UPOId
                         join c in dbContext.UnitPaymentOrderDetails on b.Id equals c.UPOItemId
                         where a.IsDeleted == false
                               && b.IsDeleted == false
                               && c.IsDeleted == false
                               && (a.UseIncomeTax == true || a.UseVat == true)
                               && a.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? a.SupplierId : supplierId)
                               && a.VatNo == (string.IsNullOrWhiteSpace(taxno) ? a.VatNo : taxno)
                               && a.Date.AddHours(offset).Date >= DateFrom.Date
                               && a.Date.AddHours(offset).Date <= DateTo.Date
                               && a.VatDate.AddHours(offset).Date >= taxDateFrom.Date
                               && a.VatDate.AddHours(offset).Date <= taxDateTo.Date

                         group new { Total = c.PriceTotal } by new { a.UPONo, a.Date, a.VatNo, a.VatDate, a.SupplierCode, a.SupplierName, a.UseIncomeTax, a.UseVat, a.IncomeTaxRate, a.VatRate } into G


                         select new ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel
                         {
                             tglspb = G.Key.Date,
                             nospb = G.Key.UPONo,
                             tglppn = G.Key.VatDate,
                             noppn = G.Key.VatNo,
                             supplier = G.Key.SupplierCode + " - " + G.Key.SupplierName,
                             amountspb = Math.Round(G.Sum(c => c.Total), 2),
                             amountppn = G.Key.UseVat == false ? 0 : Math.Round((G.Sum(c => c.Total) * (G.Key.VatRate / 100)), 2),
                             amountpph = G.Key.UseIncomeTax == false ? 0 : Math.Round((G.Sum(c => c.Total) * (G.Key.IncomeTaxRate / 100)), 2),
                         });
            return Query;
        }

        public Tuple<List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel>, int> GetReportTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQueryTax(supplierId, taxno, dateFrom, dateTo, taxdateFrom, taxdateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.nospb);
            }

            Pageable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel> pageable = new Pageable<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel>(Query, page - 1, size);
            List<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel> Data = pageable.Data.ToList<ViewModels.UnitPaymentOrderViewModel.UnitPaymentOrderTaxReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int offset)
        {
            var Query = GetReportQueryTax(supplierId, taxno, dateFrom, dateTo, taxdateFrom, taxdateTo, offset);
            Query = Query.OrderByDescending(b => b.tglspb);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Faktur Pajak", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Seri pajak", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No SPB", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl SPB", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai DPP", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPN", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai PPH", DataType = typeof(Double) });


            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string tglspb = item.tglspb == null ? "-" : item.tglspb.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));
                    string tglpjk = item.tglppn == null ? "-" : item.tglppn.GetValueOrDefault().ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd-MM-yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(index, tglpjk, item.noppn, item.supplier, item.nospb, tglspb, item.amountspb, item.amountppn, item.amountpph);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion
    }
}
