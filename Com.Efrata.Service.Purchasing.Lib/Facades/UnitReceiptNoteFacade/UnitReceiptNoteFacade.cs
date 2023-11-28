

using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade
{
    public class UnitReceiptNoteFacade : IUnitReceiptNoteFacade
    {
        private string USER_AGENT = "Facade";
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly IDistributedCache _cacheManager;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly DbSet<UnitReceiptNote> dbSet;
        private readonly IEnumerable<string> SpecialCategoryCode = new List<string>()
        {
            "BP","BB","EM","S","R","E","PL","MM","SP","U"
        };

        public UnitReceiptNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            _cacheManager = serviceProvider.GetService<IDistributedCache>();
            _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<UnitReceiptNote>();
        }

        //private List<CategoryCOAResult> Categories => _cacheManager.Get(MemoryCacheConstant.Categories, entry => { return new List<CategoryCOAResult>(); });
        //private List<IdCOAResult> Units => _cacheManager.Get(MemoryCacheConstant.Units, entry => { return new List<IdCOAResult>(); });
        //private List<IdCOAResult> Divisions => _cacheManager.Get(MemoryCacheConstant.Divisions, entry => { return new List<IdCOAResult>(); });
        //private List<IncomeTaxCOAResult> IncomeTaxes => _cacheManager.Get(MemoryCacheConstant.IncomeTaxes, entry => { return new List<IncomeTaxCOAResult>(); });

        private string _jsonCategories => _cacheManager.GetString(MemoryCacheConstant.Categories);
        private string _jsonUnits => _cacheManager.GetString(MemoryCacheConstant.Units);
        private string _jsonDivisions => _cacheManager.GetString(MemoryCacheConstant.Divisions);
        private string _jsonIncomeTaxes => _cacheManager.GetString(MemoryCacheConstant.IncomeTaxes);

        public ReadResponse<UnitReceiptNote> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitReceiptNote> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "URNNo", "UnitName", "SupplierName", "DONo","Items.PRNo"
            };

            Query = QueryHelper<UnitReceiptNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitReceiptNote
            {
                Id = s.Id,
                UId = s.UId,
                URNNo = s.URNNo,
                ReceiptDate = s.ReceiptDate,
                UnitName = s.UnitName,
                DivisionName = s.DivisionName,
                SupplierName = s.SupplierName,
                DONo = s.DONo,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Items = s.Items.ToList()
            });



            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitReceiptNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitReceiptNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitReceiptNote> pageable = new Pageable<UnitReceiptNote>(Query, Page - 1, Size);
            List<UnitReceiptNote> Data = pageable.Data.ToList<UnitReceiptNote>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<UnitReceiptNote>(Data, TotalData, OrderDictionary);
        }

        public ReadResponse<UnitReceiptNote> ReadByNoFiltered(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitReceiptNote> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "URNNo", "UnitName", "SupplierName", "DONo","Items.PRNo"
            };

            Query = QueryHelper<UnitReceiptNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new UnitReceiptNote
            {
                Id = s.Id,
                UId = s.UId,
                URNNo = s.URNNo,
                ReceiptDate = s.ReceiptDate,
                UnitName = s.UnitName,
                DivisionName = s.DivisionName,
                SupplierName = s.SupplierName,
                DONo = s.DONo,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Items = s.Items.ToList()
            });



            Dictionary<string, List<string>> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Filter);
            if (FilterDictionary.Keys.FirstOrDefault() == "no")
            {
                List<string> filteredPosition = FilterDictionary.GetValueOrDefault("no");
                Query = Query.Where(x => filteredPosition.Any(y => y == x.URNNo));
            }

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitReceiptNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitReceiptNote> pageable = new Pageable<UnitReceiptNote>(Query, Page - 1, Size);
            List<UnitReceiptNote> Data = pageable.Data.ToList<UnitReceiptNote>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<UnitReceiptNote>(Data, TotalData, OrderDictionary);
        }

        public UnitReceiptNote ReadById(int id)
        {
            var a = this.dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        async Task<string> GenerateNo(UnitReceiptNote model)
        {
            string Year = model.ReceiptDate.ToString("yy");
            string Month = model.ReceiptDate.ToString("MM");


            string no = $"{Year}-{Month}-{model.URNNo}-{model.UnitCode}-";
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.URNNo.StartsWith(no) && !w.URNNo.EndsWith("L") && !w.IsDeleted).OrderByDescending(o => o.URNNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = int.Parse(lastNo.URNNo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public async Task<int> Create(UnitReceiptNote model, string user)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, user, "Facade");

                    model.URNNo = await GenerateNo(model);

                    var useVATFlag = false;
                    var currencyCode = "";
                    var paymentDuration = "";
                    if (model.Items != null)
                    {
                        foreach (var item in model.Items)
                        {

                            EntityExtension.FlagForCreate(item, user, "Facade");
                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(s => s.Id == item.EPODetailId);
                            PurchaseRequestItem prItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == externalPurchaseOrderDetail.PRItemId);
                            InternalPurchaseOrderItem poItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == externalPurchaseOrderDetail.POItemId);
                            DeliveryOrderDetail doDetail = dbContext.DeliveryOrderDetails.FirstOrDefault(s => s.Id == item.DODetailId);
                            UnitPaymentOrderDetail upoDetail = dbContext.UnitPaymentOrderDetails.FirstOrDefault(s => s.IsDeleted == false && s.POItemId == poItem.Id);

                            var poextItem = dbContext.ExternalPurchaseOrderItems.Select(s => new { s.Id, s.EPOId }).FirstOrDefault(f => f.Id.Equals(externalPurchaseOrderDetail.EPOItemId));
                            var poext = dbContext.ExternalPurchaseOrders.Select(s => new { s.Id, s.UseIncomeTax, s.UseVat, s.CurrencyCode, s.PaymentDueDays }).FirstOrDefault(f => f.Id.Equals(poextItem.EPOId));

                            paymentDuration = poext.PaymentDueDays;

                            useVATFlag = useVATFlag || poext.UseVat;
                            currencyCode = poext.CurrencyCode;

                            item.PRItemId = doDetail.PRItemId;
                            item.PricePerDealUnit = externalPurchaseOrderDetail.PricePerDealUnit;
                            doDetail.ReceiptQuantity += item.ReceiptQuantity;
                            externalPurchaseOrderDetail.ReceiptQuantity += item.ReceiptQuantity;
                            if (upoDetail == null)
                            {
                                if (externalPurchaseOrderDetail.DOQuantity >= externalPurchaseOrderDetail.DealQuantity)
                                {
                                    if (externalPurchaseOrderDetail.ReceiptQuantity < externalPurchaseOrderDetail.DealQuantity)
                                    {
                                        //prItem.Status = "Barang sudah diterima Unit parsial";
                                        poItem.Status = "Barang sudah diterima Unit parsial";
                                    }
                                    else
                                    {
                                        //prItem.Status = "Barang sudah diterima Unit semua";
                                        poItem.Status = "Barang sudah diterima Unit semua";
                                    }
                                }
                                else
                                {
                                    //prItem.Status = "Barang sudah diterima Unit parsial";
                                    poItem.Status = "Barang sudah diterima Unit parsial";
                                }
                            }

                        }
                    }

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();


                    await CreateJournalTransactions(model);
                    await CreateCreditorAccount(model, useVATFlag, currencyCode, paymentDuration);
                    if (model.IsStorage == true)
                    {
                        insertStorage(model, user, "IN");
                    }
                    await EditFulfillment(model, user);
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

        private async Task CreateCreditorAccount(UnitReceiptNote model, bool useVATFlag, string currencyCode, string paymentDuration)
        {
            var dpp = model.Items.Sum(s => s.ReceiptQuantity * s.PricePerDealUnit);
            var externalPurchaseOrderNo = model.Items.Select(entity => entity.EPONo).ToList();
            var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(entity => externalPurchaseOrderNo.Contains(entity.EPONo)).ToList();
            var externalPurchaseOrdersIds = externalPurchaseOrders.Select(element => element.Id).ToList();
            var externalPurchaseOrder = dbContext.ExternalPurchaseOrders.FirstOrDefault(entity => externalPurchaseOrderNo.Contains(entity.EPONo));

            var vatAmount = externalPurchaseOrder.UseVat ? dpp * 0.1 : 0;
            double.TryParse(externalPurchaseOrder.IncomeTaxRate, out var incomeTaxRate);
            var incomeTaxAmount = externalPurchaseOrder.UseIncomeTax && externalPurchaseOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? dpp * incomeTaxRate / 100 : 0;
            var productList = string.Join("\n", model.Items.Select(s => s.ProductName).ToList());

            var currencyTuples = new List<Tuple<string, DateTimeOffset>> { new Tuple<string, DateTimeOffset>(currencyCode, model.ReceiptDate) };

            //var currency = await _currencyProvider.GetCurrencyByCurrencyCode(currencyCode);
            var currency = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

            var currencyRate = currency != null && currency.FirstOrDefault() != null ? currency.FirstOrDefault().Rate.GetValueOrDefault() : 1;

            var creditorAccounts = new List<CreditorAccountDto>();

            if (externalPurchaseOrders.Count > 1)
            {
                var externalPOItems = dbContext.ExternalPurchaseOrderItems.Where(entity => externalPurchaseOrdersIds.Contains(entity.EPOId));
                var purchaseRequestNos = externalPOItems.Select(element => element.PRNo).ToList();
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

                if (categoryIds.Count > 1)
                {
                    foreach (var epo in categoryIds)
                    {
                        var newPurchaseRequests = purchaseRequests.Where(entity => entity.CategoryId == epo).Select(entity => entity.No).ToList();
                        var newExternalPOItems = externalPOItems.Where(entity => newPurchaseRequests.Contains(entity.PRNo)).Select(entity => entity.EPOId).ToList();
                        var newExternalPOs = externalPurchaseOrders.Where(entity => newExternalPOItems.Contains(entity.Id));
                        var newExternalPONos = newExternalPOs.Select(entity => entity.EPONo).ToList();
                        var epoItem = model.Items.Where(s => newExternalPONos.Contains(s.EPONo));
                        var newDPP = epoItem.Sum(s => s.ReceiptQuantity * s.PricePerDealUnit);
                        var newProductList = string.Join("\n", epoItem.Select(s => s.ProductName).ToList());
                        var newVatAmount = newExternalPOs.FirstOrDefault().UseVat ? newDPP * 0.1 : 0;
                        double.TryParse(newExternalPOs.FirstOrDefault().IncomeTaxRate, out var newIncomeTaxRate);
                        var newIncomeTaxAmount = newExternalPOs.FirstOrDefault().UseIncomeTax && newExternalPOs.FirstOrDefault().IncomeTaxBy.ToUpper() == "SUPPLIER" ? newDPP * newIncomeTaxRate / 100 : 0;

                        var creditorAccount = new CreditorAccountDto()
                        {
                            DPP = Convert.ToDecimal(currencyCode != "IDR" ? newDPP * currencyRate : newDPP),
                            PPN = Convert.ToDecimal(newExternalPOs.FirstOrDefault().UseVat ? 0.1 * (currencyCode != "IDR" ? newDPP * currencyRate : newDPP) : 0),
                            SupplierCode = model.SupplierCode,
                            SupplierName = model.SupplierName,
                            SupplierIsImport = model.SupplierIsImport,
                            Date = model.ReceiptDate,
                            Code = model.URNNo,
                            Currency = currencyCode,
                            CurrencyRate = Convert.ToDecimal(currencyRate),
                            PaymentDuration = paymentDuration,
                            Products = newProductList,
                            DivisionId = Convert.ToInt32(model.DivisionId),
                            DivisionCode = model.DivisionCode,
                            DivisionName = model.DivisionName,
                            UnitId = Convert.ToInt32(model.UnitId),
                            UnitCode = model.UnitCode,
                            UnitName = model.UnitName,
                            ExternalPurchaseOrderNo = string.Join('\n', epoItem.Select(item => $"- {item.EPONo}")),
                            VATAmount = Convert.ToDecimal(newVatAmount),
                            IncomeTaxAmount = Convert.ToDecimal(currencyCode != "IDR" ? newIncomeTaxAmount * currencyRate : newIncomeTaxAmount),
                            DPPCurrency = Convert.ToDecimal(currencyCode != "IDR" ? newDPP : 0)
                        };

                        creditorAccounts.Add(creditorAccount);
                    }
                }
                else
                {
                    if (tax.Count > 1)
                    {
                        foreach (var epo in tax)
                        {
                            var epoItem = model.Items.Where(s => epo.EpoNos.Contains(s.EPONo));
                            var epoNos = epoItem.Select(entity => entity.EPONo).ToList();
                            var newExternalPOs = externalPurchaseOrders.Where(entity => epoNos.Contains(entity.EPONo));
                            var newDPP = epoItem.Sum(s => s.ReceiptQuantity * s.PricePerDealUnit);
                            var newProductList = string.Join("\n", epoItem.Select(s => s.ProductName).ToList());
                            var newVatAmount = epo.UseVat ? newDPP * 0.1 : 0;
                            double.TryParse(epo.IncomeTaxRate, out var newIncomeTaxRate);
                            var newIncomeTaxAmount = epo.UseIncomeTax && epo.IncomeTaxBy.ToUpper() == "SUPPLIER" ? newDPP * newIncomeTaxRate / 100 : 0;

                            var creditorAccount = new CreditorAccountDto()
                            {
                                DPP = Convert.ToDecimal(currencyCode != "IDR" ? newDPP * currencyRate : newDPP),
                                PPN = Convert.ToDecimal(newExternalPOs.FirstOrDefault().UseVat ? 0.1 * (currencyCode != "IDR" ? newDPP * currencyRate : newDPP) : 0),
                                SupplierCode = model.SupplierCode,
                                SupplierName = model.SupplierName,
                                SupplierIsImport = model.SupplierIsImport,
                                Date = model.ReceiptDate,
                                Code = model.URNNo,
                                Currency = currencyCode,
                                CurrencyRate = Convert.ToDecimal(currencyRate),
                                PaymentDuration = paymentDuration,
                                Products = newProductList,
                                DivisionId = Convert.ToInt32(model.DivisionId),
                                DivisionCode = model.DivisionCode,
                                DivisionName = model.DivisionName,
                                UnitId = Convert.ToInt32(model.UnitId),
                                UnitCode = model.UnitCode,
                                UnitName = model.UnitName,
                                ExternalPurchaseOrderNo = string.Join('\n', epoItem.Select(item => $"- {item.EPONo}")),
                                VATAmount = Convert.ToDecimal(newVatAmount),
                                IncomeTaxAmount = Convert.ToDecimal(currencyCode != "IDR" ? newIncomeTaxAmount * currencyRate : newIncomeTaxAmount),
                                DPPCurrency = Convert.ToDecimal(currencyCode != "IDR" ? newDPP : 0)
                            };

                            creditorAccounts.Add(creditorAccount);
                        }
                    }
                    else
                    {
                        var creditorAccount = new CreditorAccountDto()
                        {
                            DPP = Convert.ToDecimal(currencyCode != "IDR" ? dpp * currencyRate : dpp),
                            PPN = Convert.ToDecimal(useVATFlag ? 0.1 * (currencyCode != "IDR" ? dpp * currencyRate : dpp) : 0),
                            SupplierCode = model.SupplierCode,
                            SupplierName = model.SupplierName,
                            SupplierIsImport = model.SupplierIsImport,
                            Date = model.ReceiptDate,
                            Code = model.URNNo,
                            Currency = currencyCode,
                            CurrencyRate = Convert.ToDecimal(currencyRate),
                            PaymentDuration = paymentDuration,
                            Products = productList,
                            DivisionId = Convert.ToInt32(model.DivisionId),
                            DivisionCode = model.DivisionCode,
                            DivisionName = model.DivisionName,
                            UnitId = Convert.ToInt32(model.UnitId),
                            UnitCode = model.UnitCode,
                            UnitName = model.UnitName,
                            ExternalPurchaseOrderNo = string.Join('\n', model.Items.Select(item => $"- {item.EPONo}")),
                            VATAmount = Convert.ToDecimal(vatAmount),
                            IncomeTaxAmount = Convert.ToDecimal(currencyCode != "IDR" ? incomeTaxAmount * currencyRate : incomeTaxAmount),
                            DPPCurrency = Convert.ToDecimal(currencyCode != "IDR" ? dpp : 0)
                        };

                        creditorAccounts.Add(creditorAccount);
                    }
                }
            }
            else
            {
                var creditorAccount = new CreditorAccountDto()
                {
                    DPP = Convert.ToDecimal(currencyCode != "IDR" ? dpp * currencyRate : dpp),
                    PPN = Convert.ToDecimal(useVATFlag ? 0.1 * (currencyCode != "IDR" ? dpp * currencyRate : dpp) : 0),
                    SupplierCode = model.SupplierCode,
                    SupplierName = model.SupplierName,
                    SupplierIsImport = model.SupplierIsImport,
                    Date = model.ReceiptDate,
                    Code = model.URNNo,
                    Currency = currencyCode,
                    CurrencyRate = Convert.ToDecimal(currencyRate),
                    PaymentDuration = paymentDuration,
                    Products = productList,
                    DivisionId = Convert.ToInt32(model.DivisionId),
                    DivisionCode = model.DivisionCode,
                    DivisionName = model.DivisionName,
                    UnitId = Convert.ToInt32(model.UnitId),
                    UnitCode = model.UnitCode,
                    UnitName = model.UnitName,
                    ExternalPurchaseOrderNo = string.Join('\n', model.Items.Select(item => $"- {item.EPONo}")),
                    VATAmount = Convert.ToDecimal(vatAmount),
                    IncomeTaxAmount = Convert.ToDecimal(currencyCode != "IDR" ? incomeTaxAmount * currencyRate : incomeTaxAmount),
                    DPPCurrency = Convert.ToDecimal(currencyCode != "IDR" ? dpp : 0)
                };

                creditorAccounts.Add(creditorAccount);
            }

            string creditorAccountUri = "creditor-account/unit-receipt-note";
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{creditorAccountUri}", new StringContent(JsonConvert.SerializeObject(creditorAccounts).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        //private async Task CreateJournalTransactions(UnitReceiptNote model)
        //{
        //    var purchaseRequestIds = model.Items.Select(s => s.PRId).ToList();
        //    var purchaseRequests = dbContext.PurchaseRequests.Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.CategoryCode }).ToList();

        //    var externalPurchaseOrderIds = model.Items.Select(s => s.EPOId).ToList();
        //    var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(w => externalPurchaseOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.UseIncomeTax, s.IncomeTaxName, s.IncomeTaxRate }).ToList();

        //    var externalPurchaseOrderDetailIds = model.Items.Select(s => s.EPODetailId).ToList();
        //    var externalPurchaseOrderDetails = dbContext.ExternalPurchaseOrderDetails.Where(w => externalPurchaseOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductId, TotalPrice = s.PricePerDealUnit * s.DealQuantity, s.DealQuantity }).ToList();

        //    //var postMany = new List<Task<HttpResponseMessage>>();

        //    var journalTransactionsToPost = new List<JournalTransaction>();

        //    foreach (var item in model.Items)
        //    {
        //        var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
        //        var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(f => f.Id.Equals(item.EPOId));

        //        if (COAGenerator.IsHavingStockCOA(purchaseRequest.CategoryCode))
        //        {
        //            if (COAGenerator.IsSparePart(purchaseRequest.CategoryCode))
        //            {
        //                if (model.SupplierIsImport)
        //                {
        //                    var externalPOPriceTotal = externalPurchaseOrderDetails.Where(w => w.ProductId.Equals(item.ProductId) && w.Id.Equals(item.EPODetailId)).Sum(s => s.TotalPrice);

        //                    if (externalPOPriceTotal > 100000000)
        //                    {
        //                        journalTransactionsToPost.Add(CreateIsSparePartJournalTransaction(item, model, externalPurchaseOrder.UseIncomeTax, double.TryParse(externalPurchaseOrder.IncomeTaxRate, out double incomeTax) ? double.Parse(externalPurchaseOrder.IncomeTaxRate) : 0, externalPurchaseOrder.IncomeTaxName, true));
        //                    }
        //                    else
        //                    {
        //                        journalTransactionsToPost.Add(CreateIsSparePartJournalTransaction(item, model, externalPurchaseOrder.UseIncomeTax, double.TryParse(externalPurchaseOrder.IncomeTaxRate, out double incomeTax) ? double.Parse(externalPurchaseOrder.IncomeTaxRate) : 0, externalPurchaseOrder.IncomeTaxName, false));
        //                    }

        //                }
        //                else
        //                {
        //                    journalTransactionsToPost.Add(CreateNormalJournalTransaction(item, model, purchaseRequest.CategoryCode, externalPurchaseOrder.UseIncomeTax, double.TryParse(externalPurchaseOrder.IncomeTaxRate, out double incomeTax) ? double.Parse(externalPurchaseOrder.IncomeTaxRate) : 0, externalPurchaseOrder.IncomeTaxName));
        //                }
        //            }
        //            else
        //            {
        //                journalTransactionsToPost.Add(CreateNormalJournalTransaction(item, model, purchaseRequest.CategoryCode, externalPurchaseOrder.UseIncomeTax, double.TryParse(externalPurchaseOrder.IncomeTaxRate, out double incomeTax) ? double.Parse(externalPurchaseOrder.IncomeTaxRate) : 0, externalPurchaseOrder.IncomeTaxName));
        //            }
        //        }
        //        else
        //        {
        //            journalTransactionsToPost.Add(CreateJournalTransactionNotHavingStock(item, model, purchaseRequest.CategoryCode, externalPurchaseOrder.UseIncomeTax, double.TryParse(externalPurchaseOrder.IncomeTaxRate, out double incomeTax) ? double.Parse(externalPurchaseOrder.IncomeTaxRate) : 0, externalPurchaseOrder.IncomeTaxName));
        //        }
        //    }

        //    foreach (var journalTransaction in journalTransactionsToPost)
        //    {
        //        if (journalTransaction.Items.Any(a => a.COA.Code.Split(".").First().Equals("9999")))
        //        {
        //            journalTransaction.Status = "DRAFT";
        //        }
        //    }

        //    string journalTransactionUri = "journal-transactions/many";
        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(journalTransactionsToPost).ToString(), Encoding.UTF8, General.JsonMediaType));

        //    response.EnsureSuccessStatusCode();
        //}

        private async Task CreateJournalTransactions(UnitReceiptNote model)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var divisions = JsonConvert.DeserializeObject<List<IdCOAResult>>(string.IsNullOrEmpty(_jsonDivisions) ? "[]": _jsonDivisions, jsonSerializerSettings);
            var units = JsonConvert.DeserializeObject<List<IdCOAResult>>(string.IsNullOrEmpty(_jsonUnits) ? "[]": _jsonUnits, jsonSerializerSettings);
            var categories = JsonConvert.DeserializeObject<List<CategoryCOAResult>>(string.IsNullOrEmpty(_jsonCategories) ? "[]" : _jsonCategories, jsonSerializerSettings);
            var incomeTaxes = JsonConvert.DeserializeObject<List<IncomeTaxCOAResult>>(string.IsNullOrEmpty(_jsonIncomeTaxes) ? "[]": _jsonIncomeTaxes, jsonSerializerSettings);

            var purchaseRequestIds = model.Items.Select(s => s.PRId).ToList();
            var purchaseRequests = dbContext.PurchaseRequests.Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.CategoryCode, s.CategoryId }).ToList();

            var externalPurchaseOrderIds = model.Items.Select(s => s.EPOId).ToList();
            var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(w => externalPurchaseOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.IncomeTaxId, s.UseIncomeTax, s.IncomeTaxName, s.IncomeTaxRate, s.CurrencyCode, s.CurrencyRate, s.IncomeTaxBy }).ToList();



            var externalPurchaseOrderDetailIds = model.Items.Select(s => s.EPODetailId).ToList();
            var externalPurchaseOrderDetails = dbContext.ExternalPurchaseOrderDetails.Where(w => externalPurchaseOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductId, TotalPrice = s.PricePerDealUnit * s.DealQuantity, s.DealQuantity }).ToList();

            //var postMany = new List<Task<HttpResponseMessage>>();

            //var journalTransactionsToPost = new List<JournalTransaction>();

            var journalTransactionToPost = new JournalTransaction()
            {
                Date = model.ReceiptDate,
                Description = "Bon Terima Unit",
                ReferenceNo = model.URNNo,
                Status = "POSTED",
                Items = new List<JournalTransactionItem>()
            };

            int.TryParse(model.DivisionId, out var divisionId);
            var division = divisions.FirstOrDefault(f => f.Id.Equals(divisionId));
            if (division == null)
            {
                division = new IdCOAResult()
                {
                    COACode = "0"
                };
            }
            else
            {
                if (string.IsNullOrEmpty(division.COACode))
                {
                    division.COACode = "0";
                }
            }


            int.TryParse(model.UnitId, out var unitId);
            var unit = units.FirstOrDefault(f => f.Id.Equals(unitId));
            if (unit == null)
            {
                unit = new IdCOAResult()
                {
                    COACode = "00"
                };
            }
            else
            {
                if (string.IsNullOrEmpty(unit.COACode))
                {
                    unit.COACode = "00";
                }
            }


            var journalDebitItems = new List<JournalTransactionItem>();
            var journalCreditItems = new List<JournalTransactionItem>();

            foreach (var item in model.Items)
            {

                var purchaseRequest = purchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
                var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(f => f.Id.Equals(item.EPOId));

                //double.TryParse(externalPurchaseOrder.IncomeTaxRate, out var incomeTaxRate);

                //var currency = await _currencyProvider.GetCurrencyByCurrencyCode(externalPurchaseOrder.CurrencyCode);
                //var currencyTupples = new Tuple<>
                var currencyTuples = new List<Tuple<string, DateTimeOffset>> { new Tuple<string, DateTimeOffset>(externalPurchaseOrder.CurrencyCode, model.ReceiptDate) };
                var currency = await _currencyProvider.GetCurrencyByCurrencyCodeDateList(currencyTuples);

                var currencyRate = currency != null && currency.FirstOrDefault() != null ? (decimal)currency.FirstOrDefault().Rate.GetValueOrDefault() : (decimal)externalPurchaseOrder.CurrencyRate;

                //if (!externalPurchaseOrder.UseIncomeTax)
                //    incomeTaxRate = 1;
                //var externalPurchaseOrderDetail = externalPurchaseOrderDetails.FirstOrDefault(f => f.Id.Equals(item.EPODetailId));
                var externalPOPriceTotal = externalPurchaseOrderDetails.Where(w => w.ProductId.Equals(item.ProductId) && w.Id.Equals(item.EPODetailId)).Sum(s => s.TotalPrice);



                int.TryParse(purchaseRequest.CategoryId, out var categoryId);
                var category = categories.FirstOrDefault(f => f.Id.Equals(categoryId));
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

                double.TryParse(externalPurchaseOrder.IncomeTaxRate, out var incomeTaxRate);
                var grandTotal = Convert.ToDecimal(item.ReceiptQuantity * item.PricePerDealUnit * (double)currencyRate);
                if (externalPurchaseOrder.UseIncomeTax && externalPurchaseOrder.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    int.TryParse(externalPurchaseOrder.IncomeTaxId, out var incomeTaxId);
                    var incomeTax = incomeTaxes.FirstOrDefault(f => f.Id.Equals(incomeTaxId));

                    if (incomeTax == null || string.IsNullOrWhiteSpace(incomeTax.COACodeCredit))
                    {
                        incomeTax = new IncomeTaxCOAResult()
                        {
                            COACodeCredit = "9999.00"
                        };
                    }

                    var incomeTaxTotal = (decimal)incomeTaxRate / 100 * grandTotal;

                    journalDebitItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = model.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Debit = incomeTaxTotal
                    });

                    journalCreditItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{incomeTax.COACodeCredit}.{division.COACode}.{unit.COACode}"
                        },
                        Credit = incomeTaxTotal
                    });
                }


                if (model.SupplierIsImport && ((decimal)externalPOPriceTotal * currencyRate) > 100000000)
                {
                    //Purchasing Journal Item
                    journalDebitItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Debit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });

                    //Debt Journal Item
                    journalCreditItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Credit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });

                    //Stock Journal Item
                    journalDebitItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{category.StockCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Debit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });

                    //Purchasing Journal Item
                    journalCreditItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Credit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });
                }
                else
                {
                    //Purchasing Journal Item
                    journalDebitItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Debit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });

                    if (SpecialCategoryCode.Contains(category.Code))
                    {
                        //Stock Journal Item
                        journalDebitItems.Add(new JournalTransactionItem()
                        {
                            COA = new COA()
                            {
                                Code = $"{category.StockCOA}.{division.COACode}.{unit.COACode}"
                            },
                            Debit = grandTotal,
                            Remark = $"- {item.ProductName}"
                        });
                    }


                    //Debt Journal Item
                    journalCreditItems.Add(new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = model.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                        },
                        Credit = grandTotal,
                        Remark = $"- {item.ProductName}"
                    });

                    if (SpecialCategoryCode.Contains(category.Code))
                    {
                        //Purchasing Journal Item
                        journalCreditItems.Add(new JournalTransactionItem()
                        {
                            COA = new COA()
                            {
                                Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                            },
                            Credit = grandTotal,
                            Remark = $"- {item.ProductName}"
                        });
                    }
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
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(journalTransactionToPost).ToString(), Encoding.UTF8, General.JsonMediaType));
            var a = response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        //private JournalTransaction CreateIsSparePartJournalTransaction(UnitReceiptNoteItem item, UnitReceiptNote model, bool useIncomeTax, double incomeTaxRate, string incomeTaxName, bool isMoreThanOneHundredMillion)
        //{
        //    var items = new List<JournalTransactionItem>()
        //    {
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = isMoreThanOneHundredMillion ? $"2303.00.{COAGenerator.GetDivisionAndUnitCOACode(model.DivisionName, model.UnitCode)}" : $"5903.00.{COAGenerator.GetDivisionAndUnitCOACode(model.DivisionName, model.UnitCode)}"
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity,
        //            Remark = item.ProductName
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetDebtCOA(model.SupplierIsImport, model.DivisionName, model.UnitCode)
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity
        //        }
        //    };

        //    if (useIncomeTax && incomeTaxRate > 0)
        //    {
        //        AddIncomeTax(items, item, model, incomeTaxRate, incomeTaxName);
        //    }

        //    var result = new JournalTransaction()
        //    {
        //        Date = DateTimeOffset.Now,
        //        Description = "Bon Terima Unit",
        //        ReferenceNo = model.URNNo,
        //        Items = items
        //    };
        //    return result;
        //}

        //private JournalTransaction CreateJournalTransactionNotHavingStock(UnitReceiptNoteItem item, UnitReceiptNote model, string categoryCode, bool useIncomeTax, double incomeTaxRate, string incomeTaxName)
        //{
        //    var items = new List<JournalTransactionItem>()
        //    {
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetCOAByCategoryCodeAndDivisionUnit(categoryCode, model.DivisionName, model.UnitCode)
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity,
        //            Remark = item.ProductName
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetDebtCOA(model.SupplierIsImport, model.DivisionName, model.UnitCode)
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity
        //        }
        //    };

        //    if (useIncomeTax && incomeTaxRate > 0)
        //    {
        //        AddIncomeTax(items, item, model, incomeTaxRate, incomeTaxName);
        //    }

        //    var result = new JournalTransaction()
        //    {
        //        Date = DateTimeOffset.Now,
        //        Description = "Bon Terima Unit",
        //        ReferenceNo = model.URNNo,
        //        Items = items
        //    };
        //    return result;
        //}

        //private JournalTransaction CreateNormalJournalTransaction(UnitReceiptNoteItem item, UnitReceiptNote model, string categoryCode, bool useIncomeTax, double incomeTaxRate, string incomeTaxName)
        //{
        //    var items = new List<JournalTransactionItem>()
        //    {
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetCOAByCategoryCodeAndDivisionUnit(categoryCode, model.DivisionName, model.UnitCode)
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetStockCOA(model.DivisionName, model.UnitCode, categoryCode)
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity,
        //            Remark = item.ProductName
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetDebtCOA(model.SupplierIsImport, model.DivisionName, model.UnitCode)
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetCOAByCategoryCodeAndDivisionUnit(categoryCode, model.DivisionName, model.UnitCode)
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity
        //        }
        //    };

        //    if (useIncomeTax && incomeTaxRate > 0)
        //    {
        //        AddIncomeTax(items, item, model, incomeTaxRate, incomeTaxName);
        //    }

        //    var result = new JournalTransaction()
        //    {
        //        Date = DateTimeOffset.Now,
        //        Description = "Bon Terima Unit",
        //        ReferenceNo = model.URNNo,
        //        Items = items
        //    };
        //    return result;
        //}

        //private void AddIncomeTax(List<JournalTransactionItem> items, UnitReceiptNoteItem item, UnitReceiptNote model, double incomeTaxRate, string incomeTaxName)
        //{
        //    items.AddRange(new List<JournalTransactionItem>()
        //    {
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA() {
        //                Code = COAGenerator.GetDebtCOA(model.SupplierIsImport, model.DivisionName, model.UnitCode)
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity * incomeTaxRate / 100
        //        },
        //        new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = COAGenerator.GetIncomeTaxCOA(incomeTaxName, model.DivisionName, model.UnitCode)
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity * incomeTaxRate / 100
        //        }
        //    });
        //}

        //public async Task CreateJournalTransactionUnitReceiptNote(UnitReceiptNote model)

        //{
        //    var items = new List<JournalTransactionItem>();

        //    var purchasingItems = new List<JournalTransactionItem>();
        //    var stockItems = new List<JournalTransactionItem>();
        //    var debtItems = new List<JournalTransactionItem>();
        //    var incomeTaxPaidItems = new List<JournalTransactionItem>();
        //    var incomeTaxItems = new List<JournalTransactionItem>();
        //    var productListRemark = new List<string>();
        //    foreach (var item in model.Items)
        //    {
        //        var purchaseRequest = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(item.PRId));
        //        var poExternalItem = dbContext.ExternalPurchaseOrderItems.FirstOrDefault(f => f.PRId.Equals(item.PRId));
        //        var poExternal = dbContext.ExternalPurchaseOrders.FirstOrDefault(f => f.Id.Equals(poExternalItem.EPOId));

        //        var purchasingCOACode = "";
        //        var stockCOACode = "";
        //        var debtCOACode = "";
        //        var incomeTaxCOACode = "";
        //        if (purchaseRequest != null)
        //        {
        //            purchasingCOACode = COAGenerator.GetPurchasingCOA(purchaseRequest.DivisionName, purchaseRequest.UnitCode, purchaseRequest.CategoryCode);
        //            stockCOACode = COAGenerator.GetStockCOA(purchaseRequest.DivisionName, purchaseRequest.UnitCode, purchaseRequest.CategoryCode);
        //            debtCOACode = COAGenerator.GetDebtCOA(model.SupplierIsImport, purchaseRequest.DivisionName, purchaseRequest.UnitCode);
        //            if (poExternal.UseIncomeTax && double.TryParse(poExternal.IncomeTaxRate, out double test) && double.Parse(poExternal.IncomeTaxRate) > 0)
        //                incomeTaxCOACode = COAGenerator.GetIncomeTaxCOA(poExternal.IncomeTaxName, purchaseRequest.DivisionName, purchaseRequest.UnitCode);
        //        }

        //        var journalPurchasingItem = new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = purchasingCOACode
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity,
        //        };
        //        purchasingItems.Add(journalPurchasingItem);

        //        var journalStockItem = new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = stockCOACode
        //            },
        //            Debit = item.PricePerDealUnit * item.ReceiptQuantity,
        //        };
        //        stockItems.Add(journalStockItem);

        //        var journalDebtItem = new JournalTransactionItem()
        //        {
        //            COA = new COA()
        //            {
        //                Code = debtCOACode
        //            },
        //            Credit = item.PricePerDealUnit * item.ReceiptQuantity
        //        };
        //        debtItems.Add(journalDebtItem);

        //        if (poExternal.UseIncomeTax && double.TryParse(poExternal.IncomeTaxRate, out double test) && double.Parse(poExternal.IncomeTaxRate) > 0)
        //        {
        //            var pphItem = new JournalTransactionItem()
        //            {
        //                COA = new COA()
        //                {
        //                    Code = incomeTaxCOACode
        //                },
        //                Credit = item.PricePerDealUnit * item.ReceiptQuantity * double.Parse(poExternal.IncomeTaxRate) / 100
        //            };
        //            incomeTaxItems.Add(pphItem);

        //            var incomeTaxPaid = new JournalTransactionItem()
        //            {
        //                COA = new COA()
        //                {
        //                    Code = debtCOACode
        //                },
        //                Debit = item.PricePerDealUnit * item.ReceiptQuantity * double.Parse(poExternal.IncomeTaxRate) / 100
        //            };
        //            incomeTaxPaidItems.Add(incomeTaxPaid);
        //        }


        //        productListRemark.Add($"- {item.ProductName}");
        //    }

        //    purchasingItems = purchasingItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Debit = s.Sum(sum => sum.Debit),
        //        Remark = string.Join("\n", productListRemark)
        //    }).ToList();
        //    items.AddRange(purchasingItems);

        //    debtItems = debtItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Credit = s.Sum(sum => sum.Credit)
        //    }).ToList();
        //    items.AddRange(debtItems);

        //    incomeTaxPaidItems = incomeTaxPaidItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Debit = s.Sum(sum => sum.Debit)
        //    }).ToList();
        //    if (incomeTaxPaidItems.Count > 0)
        //        items.AddRange(incomeTaxPaidItems);

        //    incomeTaxItems = incomeTaxItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Credit = s.Sum(sum => sum.Credit)
        //    }).ToList();
        //    if (incomeTaxItems.Count > 0)
        //        items.AddRange(incomeTaxItems);

        //    stockItems = stockItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Debit = s.Sum(sum => sum.Debit),
        //        Remark = string.Join("\n", productListRemark)
        //    }).ToList();
        //    items.AddRange(stockItems);

        //    var purchasingCreditItems = purchasingItems.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
        //    {
        //        COA = new COA()
        //        {
        //            Code = s.First().COA.Code
        //        },
        //        Credit = s.Sum(sum => sum.Debit)
        //    }).ToList();
        //    items.AddRange(purchasingCreditItems);

        //    var modelToPost = new JournalTransaction()
        //    {
        //        Date = DateTimeOffset.Now,
        //        Description = "Bon Terima Unit",
        //        ReferenceNo = model.URNNo,
        //        Items = items
        //    };

        //    string journalTransactionUri = "journal-transactions";
        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

        //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(modelToPost).ToString(), Encoding.UTF8, General.JsonMediaType));

        //    response.EnsureSuccessStatusCode();
        //}

        public async Task<int> Update(int id, UnitReceiptNote unitReceiptNote, string user)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var m = this.dbSet.AsNoTracking()
                        .Include(d => d.Items)
                        .Single(pr => pr.Id == id && !pr.IsDeleted);

                    var useIncomeTaxFlag = false;
                    if (m != null && !id.Equals(unitReceiptNote.Id))
                    {
                        if (m.IsStorage == true)
                        {
                            insertStorage(m, user, "OUT");
                        }
                        if (unitReceiptNote.IsStorage == false)
                        {
                            unitReceiptNote.StorageCode = null;
                            unitReceiptNote.StorageId = null;
                            unitReceiptNote.StorageName = null;
                        }
                        EntityExtension.FlagForUpdate(unitReceiptNote, user, USER_AGENT);
                        foreach (var item in unitReceiptNote.Items)
                        {
                            EntityExtension.FlagForUpdate(item, user, "Facade");

                            var poext = dbContext.ExternalPurchaseOrders.Select(s => new { s.Id, s.UseIncomeTax }).FirstOrDefault(f => f.Id.Equals(item.EPOId));

                            useIncomeTaxFlag = useIncomeTaxFlag || poext.UseIncomeTax;
                        }
                        this.dbContext.Update(unitReceiptNote);
                        #region UpdateStatus
                        //foreach (var item in unitReceiptNote.Items)
                        //{
                        //    if (item.Id == 0)
                        //    {
                        //        EntityExtension.FlagForCreate(item, user, USER_AGENT);
                        //        ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(s => s.Id == item.EPODetailId);
                        //        PurchaseRequestItem prItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == externalPurchaseOrderDetail.PRItemId);
                        //        InternalPurchaseOrderItem poItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == externalPurchaseOrderDetail.POItemId);
                        //        DeliveryOrderDetail doDetail = dbContext.DeliveryOrderDetails.FirstOrDefault(s => s.Id == item.DODetailId);
                        //        item.PRItemId = doDetail.PRItemId;
                        //        item.PricePerDealUnit = externalPurchaseOrderDetail.PricePerDealUnit;
                        //        doDetail.ReceiptQuantity += item.ReceiptQuantity;
                        //        externalPurchaseOrderDetail.ReceiptQuantity += item.ReceiptQuantity;
                        //        if (externalPurchaseOrderDetail.DOQuantity >= externalPurchaseOrderDetail.DealQuantity)
                        //        {
                        //            if (externalPurchaseOrderDetail.ReceiptQuantity < externalPurchaseOrderDetail.DealQuantity)
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit parsial";
                        //                poItem.Status = "Barang sudah diterima Unit parsial";
                        //            }
                        //            else
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit semua";
                        //                poItem.Status = "Barang sudah diterima Unit semua";
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //prItem.Status = "Barang sudah diterima Unit parsial";
                        //            poItem.Status = "Barang sudah diterima Unit parsial";
                        //        }
                        //    }
                        //    EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                        //}

                        //this.dbContext.Update(unitReceiptNote);

                        //foreach (var itemExist in m.Items)
                        //{
                        //    var a = itemExist;
                        //    ExternalPurchaseOrderDetail epoDetail = this.dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(s => s.Id == itemExist.EPODetailId);
                        //    //PurchaseRequestItem purchaseRequestItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.Id == externalPurchaseOrderDetail.PRItemId);
                        //    InternalPurchaseOrderItem purchaseOrderItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.Id == epoDetail.POItemId);
                        //    DeliveryOrderDetail sjDetail = dbContext.DeliveryOrderDetails.FirstOrDefault(s => s.Id == itemExist.DODetailId);
                        //    itemExist.PRItemId = sjDetail.PRItemId;
                        //    itemExist.PricePerDealUnit = epoDetail.PricePerDealUnit;
                        //    UnitReceiptNoteItem unitReceiptNoteItem = unitReceiptNote.Items.FirstOrDefault(i => i.Id.Equals(itemExist.Id));
                        //    if (unitReceiptNoteItem == null)
                        //    {
                        //        EntityExtension.FlagForDelete(itemExist, user, USER_AGENT);
                        //        this.dbContext.UnitReceiptNoteItems.Update(itemExist);
                        //        sjDetail.ReceiptQuantity -= itemExist.ReceiptQuantity;
                        //        epoDetail.ReceiptQuantity -= itemExist.ReceiptQuantity;
                        //        if (epoDetail.ReceiptQuantity == 0)
                        //        {
                        //            if (epoDetail.DOQuantity > 0 && epoDetail.DOQuantity >= epoDetail.DealQuantity)
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit semua";
                        //                purchaseOrderItem.Status = "Barang sudah datang semua";
                        //            }
                        //            else if (epoDetail.DOQuantity > 0 && epoDetail.DOQuantity < epoDetail.DealQuantity)
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit parsial";
                        //                purchaseOrderItem.Status = "Barang sudah datang parsial";
                        //            }
                        //        }
                        //        else if (epoDetail.ReceiptQuantity > 0)
                        //        {
                        //            if (epoDetail.DOQuantity >= epoDetail.DealQuantity)
                        //            {
                        //                if (epoDetail.ReceiptQuantity < epoDetail.DealQuantity)
                        //                {
                        //                    //prItem.Status = "Barang sudah diterima Unit parsial";
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit parsial";
                        //                }
                        //                else if (epoDetail.ReceiptQuantity >= epoDetail.DealQuantity)
                        //                {
                        //                    //prItem.Status = "Barang sudah diterima Unit semua";
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit semua";
                        //                }
                        //                else if (epoDetail.DOQuantity < epoDetail.DealQuantity)
                        //                {
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit parsial";
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        EntityExtension.FlagForUpdate(itemExist, user, USER_AGENT);

                        //        sjDetail.ReceiptQuantity -= itemExist.ReceiptQuantity;
                        //        epoDetail.ReceiptQuantity -= itemExist.ReceiptQuantity;
                        //        itemExist.PRItemId = sjDetail.PRItemId;
                        //        itemExist.PricePerDealUnit = epoDetail.PricePerDealUnit;
                        //        sjDetail.ReceiptQuantity += unitReceiptNoteItem.ReceiptQuantity;
                        //        epoDetail.ReceiptQuantity += unitReceiptNoteItem.ReceiptQuantity;
                        //        if (epoDetail.ReceiptQuantity == 0)
                        //        {
                        //            if (epoDetail.DOQuantity > 0 && epoDetail.DOQuantity >= epoDetail.DealQuantity)
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit semua";
                        //                purchaseOrderItem.Status = "Barang sudah datang semua";
                        //            }
                        //            else if (epoDetail.DOQuantity > 0 && epoDetail.DOQuantity < epoDetail.DealQuantity)
                        //            {
                        //                //prItem.Status = "Barang sudah diterima Unit parsial";
                        //                purchaseOrderItem.Status = "Barang sudah datang semua";
                        //            }
                        //        }
                        //        else if (epoDetail.ReceiptQuantity > 0)
                        //        {
                        //            if (epoDetail.DOQuantity >= epoDetail.DealQuantity)
                        //            {
                        //                if (epoDetail.ReceiptQuantity < epoDetail.DealQuantity)
                        //                {
                        //                    //prItem.Status = "Barang sudah diterima Unit parsial";
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit parsial";
                        //                }
                        //                else if (epoDetail.ReceiptQuantity >= epoDetail.DealQuantity)
                        //                {
                        //                    //prItem.Status = "Barang sudah diterima Unit semua";
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit semua";
                        //                }
                        //                else if (epoDetail.DOQuantity < epoDetail.DealQuantity)
                        //                {
                        //                    purchaseOrderItem.Status = "Barang sudah diterima Unit parsial";
                        //                }
                        //            }
                        //        }
                        //    }

                        //}
                        #endregion



                        Updated = await dbContext.SaveChangesAsync();

                        await ReverseJournalTransaction(m.URNNo);
                        await CreateJournalTransactions(m);
                        await UpdateCreditorAccount(unitReceiptNote, useIncomeTaxFlag);

                        if (unitReceiptNote.IsStorage == true)
                        {
                            insertStorage(unitReceiptNote, user, "IN");
                        }

                        var updatedModel = this.dbSet.AsNoTracking()
                            .Include(d => d.Items)
                            .Single(pr => pr.Id == m.Id && !pr.IsDeleted);

                        await EditFulfillment(updatedModel, user);
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

        private async Task UpdateCreditorAccount(UnitReceiptNote unitReceiptNote, bool useIncomeTaxFlag)
        {
            var dpp = unitReceiptNote.Items.Sum(s => s.ReceiptQuantity + s.PricePerDealUnit);
            var productList = string.Join("\n", unitReceiptNote.Items.Select(s => s.ProductName).ToList());

            var creditorAccount = new
            {
                DPP = dpp,
                Products = productList,
                Code = unitReceiptNote.URNNo,
                Date = unitReceiptNote.ReceiptDate,
                UseIncomeTax = useIncomeTaxFlag
            };

            string creditorAccountUri = "creditor-account/unit-receipt-note";
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{creditorAccountUri}", new StringContent(JsonConvert.SerializeObject(creditorAccount).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        private async Task ReverseJournalTransaction(string referenceNo)
        {
            string journalTransactionUri = $"journal-transactions/reverse-transactions/{referenceNo}";
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(new object()).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        public async Task<string> Delete(int id, string user)
        {
            string Message = "";

            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var spb = dbContext.UnitPaymentOrderItems.Where(entity => entity.URNId == id && !entity.IsDeleted).FirstOrDefault();

                    if (spb == null)
                    {
                        var m = this.dbSet
                        .Include(d => d.Items)
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                        EntityExtension.FlagForDelete(m, user, USER_AGENT);

                        foreach (var item in m.Items)
                        {
                            EntityExtension.FlagForDelete(item, user, USER_AGENT);

                            ExternalPurchaseOrderDetail externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(s => s.IsDeleted == false && s.Id == item.EPODetailId);
                            PurchaseRequestItem prItem = this.dbContext.PurchaseRequestItems.FirstOrDefault(s => s.IsDeleted == false && s.Id == externalPurchaseOrderDetail.PRItemId);
                            InternalPurchaseOrderItem poItem = this.dbContext.InternalPurchaseOrderItems.FirstOrDefault(s => s.IsDeleted == false && s.Id == externalPurchaseOrderDetail.POItemId);
                            DeliveryOrderDetail doDetail = dbContext.DeliveryOrderDetails.FirstOrDefault(s => s.IsDeleted == false && s.Id == item.DODetailId);
                            UnitPaymentOrderDetail upoDetail = dbContext.UnitPaymentOrderDetails.FirstOrDefault(s => s.IsDeleted == false && s.POItemId == poItem.Id);
                            doDetail.ReceiptQuantity -= item.ReceiptQuantity;
                            externalPurchaseOrderDetail.ReceiptQuantity -= item.ReceiptQuantity;
                            if (externalPurchaseOrderDetail.ReceiptQuantity == 0 && upoDetail == null)
                            {
                                if (externalPurchaseOrderDetail.DOQuantity > 0 && externalPurchaseOrderDetail.DOQuantity >= externalPurchaseOrderDetail.DealQuantity)
                                {
                                    //prItem.Status = "Barang sudah diterima Unit semua";
                                    poItem.Status = "Barang sudah datang semua";
                                }
                                else if (externalPurchaseOrderDetail.DOQuantity > 0 && externalPurchaseOrderDetail.DOQuantity < externalPurchaseOrderDetail.DealQuantity)
                                {
                                    //prItem.Status = "Barang sudah diterima Unit parsial";
                                    poItem.Status = "Barang sudah datang parsial";
                                }
                            }
                            else if (externalPurchaseOrderDetail.ReceiptQuantity > 0 && upoDetail == null)
                            {
                                if (externalPurchaseOrderDetail.DOQuantity >= externalPurchaseOrderDetail.DealQuantity)
                                {
                                    if (externalPurchaseOrderDetail.ReceiptQuantity < externalPurchaseOrderDetail.DealQuantity)
                                    {
                                        //prItem.Status = "Barang sudah diterima Unit parsial";
                                        poItem.Status = "Barang sudah diterima Unit parsial";
                                    }
                                    else if (externalPurchaseOrderDetail.ReceiptQuantity >= externalPurchaseOrderDetail.DealQuantity)
                                    {
                                        //prItem.Status = "Barang sudah diterima Unit semua";
                                        poItem.Status = "Barang sudah diterima Unit semua";
                                    }
                                    else if (externalPurchaseOrderDetail.DOQuantity < externalPurchaseOrderDetail.DealQuantity)
                                    {
                                        poItem.Status = "Barang sudah diterima Unit parsial";
                                    }
                                }
                            }
                        }

                        Deleted = dbContext.SaveChanges();

                        await ReverseJournalTransaction(m.URNNo);
                        await DeleteCreditorAccount(m.URNNo);

                        if (m.IsStorage == true)
                        {
                            insertStorage(m, user, "OUT");
                        }
                        await RollbackFulfillment(m, user);
                        transaction.Commit();
                    }
                    else
                    {
                        Message = "Tidak dapat menghapus, SPB Sudah dibuat";
                    }
                    
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Message;
        }

        private async Task DeleteCreditorAccount(string urnNo)
        {
            var creditorAccount = new
            {
                Code = urnNo
            };

            string creditorAccountUri = "creditor-account/unit-receipt-note/delete";
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PutAsync($"{APIEndpoint.Finance}{creditorAccountUri}", new StringContent(JsonConvert.SerializeObject(creditorAccount).ToString(), Encoding.UTF8, General.JsonMediaType));

            response.EnsureSuccessStatusCode();
        }

        public void insertStorage(UnitReceiptNote unitReceiptNote, string user, string type)
        {
            List<object> items = new List<object>();
            foreach (var item in unitReceiptNote.Items)
            {
                items.Add(new
                {
                    productId = item.ProductId,
                    productcode = item.ProductCode,
                    productname = item.ProductName,
                    uomId = item.UomId,
                    uom = item.Uom,
                    quantity = item.ReceiptQuantity,
                    remark = item.ProductRemark
                });
            }
            var data = new
            {
                storageId = unitReceiptNote.StorageId,
                storagecode = unitReceiptNote.StorageCode,
                storagename = unitReceiptNote.StorageName,
                referenceNo = unitReceiptNote.URNNo,
                referenceType = "Bon Terima Unit - " + unitReceiptNote.UnitName,
                type = type,
                remark = unitReceiptNote.Remark,
                date = unitReceiptNote.ReceiptDate,
                items = items
            };
            string inventoryUri = "inventory-documents";
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.PostAsync($"{APIEndpoint.Inventory}{inventoryUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();

        }

        public ReadResponse<UnitReceiptNote> ReadBySupplierUnit(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitReceiptNote> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "URNNo", "DONo",
            };

            Query = QueryHelper<UnitReceiptNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            Query = Query
                .Where(w =>
                    w.IsDeleted == false &&
                    w.IsPaid == false &&
                    dbContext.DeliveryOrderItems.Any(doItem =>
                        doItem.DOId == w.DOId &&
                        dbContext.ExternalPurchaseOrders.Any(epo =>
                            epo.Id == doItem.EPOId &&
                            epo.DivisionId.Equals(FilterDictionary.GetValueOrDefault("DivisionId") ?? "") &&
                            epo.SupplierId.Equals(FilterDictionary.GetValueOrDefault("SupplierId") ?? "") &&
                            epo.PaymentMethod.Equals(FilterDictionary.GetValueOrDefault("PaymentMethod") ?? "") &&
                            epo.CurrencyCode.Equals(FilterDictionary.GetValueOrDefault("CurrencyCode") ?? "") &&
                            epo.UseIncomeTax == Boolean.Parse(FilterDictionary.GetValueOrDefault("UseIncomeTax") ?? "false") &&
                            epo.IncomeTaxId == (FilterDictionary.GetValueOrDefault("IncomeTaxId") ?? epo.IncomeTaxId) &&
                            epo.UseVat == Boolean.Parse(FilterDictionary.GetValueOrDefault("UseVat") ?? "false") &&
                            epo.Items.Any(epoItem =>
                                dbContext.InternalPurchaseOrders.Any(po =>
                                    po.Id == epoItem.POId &&
                                    po.CategoryId.Equals(FilterDictionary.GetValueOrDefault("CategoryId") ?? "")
                                )
                            )
                        )
                    )
                )
                .Select(s => new UnitReceiptNote
                {
                    Id = s.Id,
                    UId = s.UId,
                    URNNo = s.URNNo,
                    ReceiptDate = s.ReceiptDate,
                    UnitName = s.UnitName,
                    DivisionName = s.DivisionName,
                    SupplierName = s.SupplierName,
                    DOId = s.DOId,
                    DONo = s.DONo,
                    CreatedBy = s.CreatedBy,
                    LastModifiedUtc = s.LastModifiedUtc,
                    Items = s.Items.Where(a => a.IsPaid == false).ToList()
                });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitReceiptNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitReceiptNote> pageable = new Pageable<UnitReceiptNote>(Query, Page - 1, Size);
            List<UnitReceiptNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<UnitReceiptNote>(Data, TotalData, OrderDictionary);
        }

        public IQueryable<UnitReceiptNoteReportViewModel> GetReportQuery(string urnNo, string prNo, string unitId, string categoryId, string supplierId, string divisionId, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.UnitReceiptNotes
                         join b in dbContext.UnitReceiptNoteItems on a.Id equals b.URNId
                         //EPO
                         join k in dbContext.ExternalPurchaseOrderDetails on b.EPODetailId equals k.Id
                         //PO
                         join c in dbContext.InternalPurchaseOrderItems on k.POItemId equals c.Id
                         join d in dbContext.InternalPurchaseOrders on c.POId equals d.Id
                         //Conditions
                         where a.IsDeleted == false
                             && b.IsDeleted == false
                             && k.IsDeleted == false
                             && c.IsDeleted == false
                             && d.IsDeleted == false
                             && a.URNNo == (string.IsNullOrWhiteSpace(urnNo) ? a.URNNo : urnNo)
                             && a.DivisionId == (string.IsNullOrWhiteSpace(divisionId) ? a.DivisionId : divisionId)
                             && a.UnitId == (string.IsNullOrWhiteSpace(unitId) ? a.UnitId : unitId)
                             && b.PRNo == (string.IsNullOrWhiteSpace(prNo) ? b.PRNo : prNo)
                             && d.CategoryId == (string.IsNullOrWhiteSpace(categoryId) ? d.CategoryId : categoryId)
                             && a.SupplierId == (string.IsNullOrWhiteSpace(supplierId) ? a.SupplierId : supplierId)
                             && a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                             && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                             
                         orderby a.ReceiptDate, a.CreatedUtc ascending
                         select new UnitReceiptNoteReportViewModel
                         {
                             urnNo = a.URNNo,
                             prNo = b.PRNo,
                             epoDetailId = b.EPODetailId,
                             category = d.CategoryName,
                             unit = a.DivisionName + " - " + a.UnitName,
                             supplier = a.SupplierName,
                             receiptDate = a.ReceiptDate,
                             productCode = b.ProductCode,
                             productName = b.ProductName,
                             receiptUom = b.Uom,
                             receiptQuantity = b.ReceiptQuantity,
                             DealUom = k.DealUomUnit,
                             dealQuantity = k.DealQuantity,
                             quantity = k.DealQuantity,
                             CreatedUtc = b.CreatedUtc,
                             pricePerDealUnit = b.PricePerDealUnit,
                             totalPrice = b.PricePerDealUnit * b.ReceiptQuantity

                         });
            Dictionary<string, double> q = new Dictionary<string, double>();
            List<UnitReceiptNoteReportViewModel> urn = new List<UnitReceiptNoteReportViewModel>();
            foreach (UnitReceiptNoteReportViewModel data in Query.ToList())
            {
                double value;
                if (q.TryGetValue(data.productCode + data.prNo + data.epoDetailId.ToString(), out value))
                {
                    q[data.productCode + data.prNo + data.epoDetailId.ToString()] -= data.receiptQuantity;
                    data.quantity = q[data.productCode + data.prNo + data.epoDetailId.ToString()];
                    urn.Add(data);
                }
                else
                {
                    q[data.productCode + data.prNo + data.epoDetailId.ToString()] = data.quantity - data.receiptQuantity;
                    data.quantity = q[data.productCode + data.prNo + data.epoDetailId.ToString()];
                    urn.Add(data);
                }

            }
            return Query = urn.AsQueryable();
        }

        public ReadResponse<UnitReceiptNoteReportViewModel> GetReport(string urnNo, string prNo, string unitId, string categoryId, string supplierId, string divisionId, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(urnNo, prNo, unitId, categoryId, supplierId, divisionId, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.receiptDate).ThenByDescending(a => a.CreatedUtc);
            }

            Pageable<UnitReceiptNoteReportViewModel> pageable = new Pageable<UnitReceiptNoteReportViewModel>(Query, page - 1, size);
            List<UnitReceiptNoteReportViewModel> Data = pageable.Data.ToList<UnitReceiptNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<UnitReceiptNoteReportViewModel>(Data, TotalData, OrderDictionary);
        }

        public MemoryStream GenerateExcel(string urnNo, string prNo, string unitId, string categoryId, string supplierId, string divisionId, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(urnNo, prNo, unitId, categoryId, supplierId, divisionId, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.receiptDate).ThenByDescending(a => a.CreatedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Beli", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Beli", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah (+/-/0)", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Satuan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total", DataType = typeof(double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", 0, "", 0,0,0); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.receiptDate == null ? "-" : item.receiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.unit, item.category, item.prNo, item.productName, item.productCode, item.supplier, date, item.urnNo, item.dealQuantity, item.DealUom, item.receiptQuantity, item.receiptUom, item.quantity, item.pricePerDealUnit, item.totalPrice);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public string GetPurchaseRequestCategoryCode(long prId)
        {
            return dbContext.PurchaseRequests.Where(pr => pr.Id == prId).Select(pr => pr.CategoryCode).FirstOrDefault();
        }

        public List<UnitReceiptNote> GetByListOfNo(List<string> urnNoList)
        {
            return dbSet.Where(w => urnNoList.Contains(w.URNNo)).Include(i => i.Items).ToList();

            //throw new NotImplementedException();
        }

        public async Task<List<SubLedgerUnitReceiptNoteViewModel>> GetUnitReceiptNoteForSubledger(List<string> urnNoes)
        {
            List<SubLedgerUnitReceiptNoteViewModel> result = new List<SubLedgerUnitReceiptNoteViewModel>();
            var urns = await dbSet.Where(x => urnNoes.Contains(x.URNNo)).ToListAsync();
            var upos = await dbContext.UnitPaymentOrderItems.Include(x => x.UnitPaymentOrder).Where(x => urnNoes.Contains(x.URNNo)).ToListAsync();
            foreach (var urnNo in urnNoes)
            {
                var urn = urns.FirstOrDefault(x => x.URNNo.Equals(urnNo, StringComparison.OrdinalIgnoreCase));
                var upo = upos.FirstOrDefault(x => x.URNNo.Equals(urnNo, StringComparison.OrdinalIgnoreCase));
                if (urn == null)
                    continue;
                result.Add(new SubLedgerUnitReceiptNoteViewModel()
                {
                    Supplier = urn.SupplierName,
                    UPONo = upo?.UnitPaymentOrder?.UPONo,
                    URNDate = urn.ReceiptDate,
                    URNNo = urn.URNNo
                });
            }

            return result;
        }

        private async Task<int> EditFulfillment(UnitReceiptNote model, string username)
        {
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;

            foreach (var item in model.Items)
            {
                var fulfillment = await dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DeliveryOrderDetailId == item.DODetailId);

                if (fulfillment != null)
                {
                    fulfillment.UnitReceiptNoteDate = model.ReceiptDate;
                    fulfillment.UnitReceiptNoteDeliveredQuantity = item.ReceiptQuantity;
                    fulfillment.UnitReceiptNoteId = model.Id;
                    fulfillment.UnitReceiptNoteItemId = item.Id;
                    fulfillment.UnitReceiptNoteNo = model.URNNo;
                    fulfillment.UnitReceiptNoteUom = item.Uom;
                    fulfillment.UnitReceiptNoteUomId = item.UomId;

                    count += await internalPOFacade.UpdateFulfillmentAsync(fulfillment.Id, fulfillment, username);
                }

            }


            return count;
        }

        private async Task<int> RollbackFulfillment(UnitReceiptNote model, string username)
        {
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;
            foreach (var item in model.Items)
            {
                var fulfillment = dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                          .FirstOrDefault(x => x.UnitReceiptNoteId == model.Id && x.UnitReceiptNoteItemId == item.Id);

                if (fulfillment != null)
                {
                    fulfillment.UnitReceiptNoteDate = DateTimeOffset.MinValue;
                    fulfillment.UnitReceiptNoteDeliveredQuantity = 0;
                    fulfillment.UnitReceiptNoteId = 0;
                    fulfillment.UnitReceiptNoteItemId = 0;
                    fulfillment.UnitReceiptNoteNo = null;
                    fulfillment.UnitReceiptNoteUom = null;
                    fulfillment.UnitReceiptNoteUomId = null;

                    count += await internalPOFacade.UpdateFulfillmentAsync(fulfillment.Id, fulfillment, username);
                }


            }
            return count;
        }

        public IQueryable<UnitNoteSpbViewModel> GetSpbQuery(string urnNo, string supplierName, string doNo, DateTime? dateFrom, DateTime? dateTo, int offset)
        {

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.UnitReceiptNotes
                         join b in dbContext.DeliveryOrders on a.DOId equals b.Id

                         join c in dbContext.UnitReceiptNoteItems on a.Id equals c.URNId
                         join d in dbContext.ExternalPurchaseOrders on c.EPOId equals d.Id
                         join e in dbContext.PurchaseRequests on c.PRId equals e.Id
                         join f in dbContext.UnitPaymentOrderDetails on c.Id equals f.URNItemId into k
                         from f in k.DefaultIfEmpty()
                         join g in dbContext.UnitPaymentOrderItems on f.UPOItemId equals g.Id into l
                         from g in l.DefaultIfEmpty()
                         join h in dbContext.UnitPaymentOrders on g.UPOId equals h.Id into m
                         from h in m.DefaultIfEmpty()
                             //Conditions
                         where
                             a.IsDeleted == false
                             && b.IsDeleted == false
                             && c.IsDeleted == false
                             && d.IsDeleted == false
                             && e.IsDeleted == false
                             && f.IsDeleted == false
                             && g.IsDeleted == false
                             && h.IsDeleted == false
                             && a.URNNo == (string.IsNullOrWhiteSpace(urnNo) ? a.URNNo : urnNo)
                             && a.SupplierName == (string.IsNullOrWhiteSpace(supplierName) ? a.SupplierName : supplierName)
                             && a.DONo == (string.IsNullOrWhiteSpace(doNo) ? a.DONo : doNo)
                             && a.ReceiptDate.AddHours(offset).Date >= DateFrom.Date
                             && a.ReceiptDate.AddHours(offset).Date <= DateTo.Date
                             && a.IsPaid == false

                         orderby a.ReceiptDate descending


                         select new UnitNoteSpbViewModel
                         {

                             UrnNo = a.URNNo,
                             ReceiptDate = a.ReceiptDate,
                             SupplierName = a.SupplierName,
                             PaymentDueDays = d.PaymentDueDays,
                             DONo = b.DONo,
                             DODate = b.DODate,
                             PRNo = c.PRNo,
                             PRDate = e.Date,

                             UnitName = a.UnitName,
                             CategoryName = e.CategoryName,
                             BudgetName = e.BudgetName,
                             ProductName = c.ProductName,
                             ReceiptQuantity = c.ReceiptQuantity,
                             Uom = c.Uom,
                             PricePerDealUnit = c.PricePerDealUnit,
                             TotalPrice = (c.ReceiptQuantity * c.PricePerDealUnit),
                             CurencyCode = d.CurrencyCode,
                             createdBy = b.CreatedBy,

                         });

            return Query;
        }


        public ReadResponse<UnitNoteSpbViewModel> GetSpbReport(string urnNo, string supplierName, string doNo, DateTime? dateFrom, DateTime? dateTo, int size, int page, string Order, int offset)
        {
            var Query = GetSpbQuery(urnNo, supplierName, doNo, dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(a => a.ReceiptDate).ThenByDescending(a => a.ReceiptDate);
            }

            Pageable<UnitNoteSpbViewModel> pageable = new Pageable<UnitNoteSpbViewModel>(Query, page - 1, size);
            List<UnitNoteSpbViewModel> Data = pageable.Data.ToList<UnitNoteSpbViewModel>();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<UnitNoteSpbViewModel>(Data, TotalData, OrderDictionary);
        }


        public MemoryStream GenerateExcelSpb(string urnNo, string supplierName, string doNo, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetSpbQuery(urnNo, supplierName, doNo, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(a => a.ReceiptDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Satuan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tempo Pembelian", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Budget", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Staff", DataType = typeof(String) });


            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", 0, "", "", "", "", "", "", "", "", 0, "", 0, 0, "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string receipt_date = item.ReceiptDate == null ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string pr_date = item.PRDate == null ? "-" : item.PRDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string do_date = item.DODate == null ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(index, pr_date, item.PRNo, item.ProductName, do_date, item.DONo, receipt_date, item.UrnNo, item.ReceiptQuantity, item.Uom, item.PricePerDealUnit,
                        item.TotalPrice, item.CurencyCode, item.SupplierName, item.PaymentDueDays, item.CategoryName, item.BudgetName, item.UnitName, item.createdBy);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public async Task<dynamic> GetCreditorAccountDataByURNNo(string urnNo)
        {
            var model = dbSet.Include(entity => entity.Items).Where(entity => entity.URNNo == urnNo).FirstOrDefault();

            var dpp = model.Items.Sum(s => s.ReceiptQuantity + s.PricePerDealUnit);
            var productList = string.Join("\n", model.Items.Select(s => s.ProductName).ToList());

            var item = model.Items.LastOrDefault();

            var externalPurchaseOrderDetail = this.dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(s => s.Id == item.EPODetailId);
            var poextItem = dbContext.ExternalPurchaseOrderItems.Select(s => new { s.Id, s.EPOId }).FirstOrDefault(f => f.Id.Equals(externalPurchaseOrderDetail.EPOItemId));
            var poext = dbContext.ExternalPurchaseOrders.Select(s => new { s.Id, s.UseIncomeTax, s.CurrencyCode, s.PaymentDueDays }).FirstOrDefault(f => f.Id.Equals(poextItem.EPOId));

            var currencyCode = poext.CurrencyCode;
            var currency = await _currencyProvider.GetCurrencyByCurrencyCode(currencyCode);
            var currencyRate = currency != null ? currency.Rate.GetValueOrDefault() : 1;

            var useIncomeTaxFlag = poext.UseIncomeTax;
            var paymentDuration = poext.PaymentDueDays;

            var creditorAccount = new
            {
                DPP = dpp,
                Products = productList,
                PPN = useIncomeTaxFlag ? 0.1 * dpp : 0,
                model.SupplierCode,
                model.SupplierName,
                Code = model.URNNo,
                Date = model.ReceiptDate,
                Currency = currencyCode,
                CurrencyRate = currencyRate,
                PaymentDuration = paymentDuration
            };

            return creditorAccount;
        }
    }
}

