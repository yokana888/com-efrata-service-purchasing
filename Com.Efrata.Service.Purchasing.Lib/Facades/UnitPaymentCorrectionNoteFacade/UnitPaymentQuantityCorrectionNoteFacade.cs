using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using System.Net.Http;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade
{
    public class UnitPaymentQuantityCorrectionNoteFacade : IUnitPaymentQuantityCorrectionNoteFacade
    {
        private string USER_AGENT = "Facade";

        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<UnitPaymentCorrectionNote> dbSet;
        private readonly IDistributedCache _cacheManager;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IEnumerable<string> SpecialCategoryCode = new List<string>()
        {
            "BP","BB","EM","S","R","E","PL","MM","SP","U"
        };

        public UnitPaymentQuantityCorrectionNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<UnitPaymentCorrectionNote>();
            _cacheManager = serviceProvider.GetService<IDistributedCache>();
            _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
        }

        private string _jsonCategories => _cacheManager.GetString(MemoryCacheConstant.Categories);
        private string _jsonUnits => _cacheManager.GetString(MemoryCacheConstant.Units);
        private string _jsonDivisions => _cacheManager.GetString(MemoryCacheConstant.Divisions);
        private string _jsonIncomeTaxes => _cacheManager.GetString(MemoryCacheConstant.IncomeTaxes);

        public Tuple<List<UnitPaymentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<UnitPaymentCorrectionNote> Query = this.dbSet;

            Query = Query
                .Select(s => new UnitPaymentCorrectionNote
                {
                    Id = s.Id,
                    UPCNo = s.UPCNo,
                    CorrectionDate = s.CorrectionDate,
                    CorrectionType = s.CorrectionType,
                    UPOId = s.UPOId,
                    UPONo = s.UPONo,
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    InvoiceCorrectionNo = s.InvoiceCorrectionNo,
                    InvoiceCorrectionDate = s.InvoiceCorrectionDate,
                    useVat = s.useVat,
                    useIncomeTax = s.useIncomeTax,
                    ReleaseOrderNoteNo = s.ReleaseOrderNoteNo,
                    DueDate = s.DueDate,
                    Items = s.Items.Select(
                        q => new UnitPaymentCorrectionNoteItem
                        {
                            Id = q.Id,
                            UPCId = q.UPCId,
                            UPODetailId = q.UPODetailId,
                            URNNo = q.URNNo,
                            EPONo = q.EPONo,
                            PRId = q.PRId,
                            PRNo = q.PRNo,
                            PRDetailId = q.PRDetailId,
                        }
                    )
                    .ToList(),
                    CreatedBy = s.CreatedBy,
                    LastModifiedUtc = s.LastModifiedUtc
                }).Where(k => k.CorrectionType == "Jumlah")
                .OrderByDescending(j => j.LastModifiedUtc);

            List<string> searchAttributes = new List<string>()
            {
                "UPCNo", "UPONo", "SupplierName", "InvoiceCorrectionNo"
            };

            Query = QueryHelper<UnitPaymentCorrectionNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<UnitPaymentCorrectionNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<UnitPaymentCorrectionNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<UnitPaymentCorrectionNote> pageable = new Pageable<UnitPaymentCorrectionNote>(Query, Page - 1, Size);
            List<UnitPaymentCorrectionNote> Data = pageable.Data.ToList<UnitPaymentCorrectionNote>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public UnitPaymentCorrectionNote ReadById(int id)
        {
            var a = this.dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        public async Task<int> Create(UnitPaymentCorrectionNote m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    //override correction Date
                    m.CorrectionDate = DateTimeOffset.Now.ToUniversalTime();

                    var supplier = GetSupplier(m.SupplierId);
                    var supplierImport = false;
                    m.SupplierNpwp = null;
                    if (supplier != null)
                    {
                        m.SupplierNpwp = supplier.npwp;
                        supplierImport = supplier.import;
                    }
                    m.UPCNo = await GenerateNo(m, clientTimeZoneOffset, supplierImport, m.DivisionName);
                    if (m.useVat == true)
                    {
                        m.ReturNoteNo = await GeneratePONo(m, clientTimeZoneOffset);
                    }
                    UnitPaymentOrder unitPaymentOrder = this.dbContext.UnitPaymentOrders.Where(s => s.Id == m.UPOId).Include(p => p.Items).ThenInclude(i => i.Details).FirstOrDefault();
                    unitPaymentOrder.IsCorrection = true;

                    foreach (var item in m.Items)
                    {
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);
                        foreach (var itemSpb in unitPaymentOrder.Items)
                        {
                            foreach (var detailSpb in itemSpb.Details)
                            {
                                if (item.UPODetailId == detailSpb.Id)
                                {
                                    if (detailSpb.QuantityCorrection <= 0)
                                    {
                                        detailSpb.QuantityCorrection = detailSpb.ReceiptQuantity;
                                    }

                                    detailSpb.QuantityCorrection = detailSpb.QuantityCorrection - item.Quantity;
                                    ExternalPurchaseOrderDetail epoDetail = dbContext.ExternalPurchaseOrderDetails.FirstOrDefault(a => a.Id.Equals(detailSpb.EPODetailId));
                                    epoDetail.DOQuantity -= item.Quantity;
                                }
                            }
                        }
                    }

                    this.dbSet.Add(m);
                    Created = await dbContext.SaveChangesAsync();
                    Created += await AddCorrections(m, user);
                    await AutoCreateJournalTransaction(m);
                    await AutoCreateCreditorAccount(m);

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

        async Task<string> GenerateNo(UnitPaymentCorrectionNote model, int clientTimeZoneOffset, bool supplierImport, string divisionName)
        {
            string Year = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string supplier_imp;
            char division_name;
            if (supplierImport == true)
            {
                supplier_imp = "NRI";
            }
            else
            {
                supplier_imp = "NRL";
            }
            if (divisionName.ToUpper() == "EFRATA")
            {
                division_name = 'G';
            }
            else
            {
                division_name = 'T';
            }


            string no = $"{Year}-{Month}-{division_name}-{supplier_imp}-";
            int Padding = 3;
            var upcno = "";

            var lastNo = await this.dbSet.Where(w => w.UPCNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.UPCNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                upcno = no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.UPCNo.Replace(no, "")) + 1;
                upcno = no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
            return upcno;
        }

        async Task<string> GeneratePONo(UnitPaymentCorrectionNote model, int clientTimeZoneOffset)
        {
            string Year = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");

            string no = $"{Year}-{Month}-NR-";
            int Padding = 3;
            var pono = "";

            var lastNo = await this.dbSet.Where(w => w.ReturNoteNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.ReturNoteNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                pono = no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.ReturNoteNo.Replace(no, "")) + 1;
                pono = no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
            return pono;
        }

        public SupplierViewModel GetSupplier(string supplierId)
        {
            string supplierUri = "master/suppliers";
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            if (httpClient != null)
            {
                var response = httpClient.GetAsync($"{APIEndpoint.Core}{supplierUri}/{supplierId}").Result.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                SupplierViewModel viewModel = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                SupplierViewModel viewModel = null;
                return viewModel;
            }

        }

        private async Task AutoCreateJournalTransaction(UnitPaymentCorrectionNote unitPaymentCorrection)
        {
            var prIds = unitPaymentCorrection.Items.Select(element => element.PRId).ToList();
            var purchaseRequests = dbContext.PurchaseRequests.Where(entity => prIds.Contains(entity.Id)).ToList();
            var epoNos = unitPaymentCorrection.Items.Select(element => element.EPONo).ToList();
            var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(entity => epoNos.Contains(entity.EPONo)).ToList();
            var urnNos = unitPaymentCorrection.Items.Select(element => element.URNNo).Distinct().ToList();
            var unitReceiptNotes = dbContext.UnitReceiptNotes.Where(element => urnNos.Contains(element.URNNo)).ToList();
            var upoDetailIds = unitPaymentCorrection.Items.Select(element => element.UPODetailId).ToList();
            var unitPaymentOrderDetails = dbContext.UnitPaymentOrderDetails.Where(entity => upoDetailIds.Contains(entity.Id)).ToList();
            var urnItemIds = unitPaymentOrderDetails.Select(element => element.URNItemId).ToList();
            var unitReceiptNoteItems = dbContext.UnitReceiptNoteItems.Where(entity => urnItemIds.Contains(entity.Id)).ToList();
            var epoDetailIds = unitReceiptNoteItems.Select(element => element.EPODetailId).ToList();
            var externalPurchaseOrderDetails = dbContext.ExternalPurchaseOrderDetails.Where(entity => epoDetailIds.Contains(entity.Id)).ToList();
            var correctionItems = dbContext.UnitPaymentCorrectionNoteItems.Where(entity => urnNos.Contains(entity.URNNo)).ToList();
            var correctionIds = correctionItems.Select(element => element.UPCId).ToList();
            var previousCorrectionNotes = dbContext.UnitPaymentCorrectionNotes.Where(entity => correctionIds.Contains(entity.Id) && entity.CorrectionDate < unitPaymentCorrection.CorrectionDate).ToList();
            var previousCorrections = new List<PreviousCorrection>();

            foreach (var previousCorrectionNote in previousCorrectionNotes.OrderBy(element => element.CorrectionDate))
            {
                var previousCorrectionItems = correctionItems.Where(element => element.UPCId == previousCorrectionNote.Id).ToList();
                foreach (var previousCorrectionItem in previousCorrectionItems)
                {
                    var upoDetail = unitPaymentOrderDetails.FirstOrDefault(element => element.Id == previousCorrectionItem.UPODetailId);
                    if (upoDetail != null)
                    {
                        var urnItem = unitReceiptNoteItems.FirstOrDefault(element => element.Id == upoDetail.URNItemId);

                        if (urnItem != null)
                        {
                            if (previousCorrectionNote.CorrectionType == "Harga Total")
                            {
                                var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == urnItem.Id);

                                if (previousCorrection == null)
                                {
                                    previousCorrection = new PreviousCorrection
                                    {
                                        URNItemId = (int)urnItem.Id,
                                        PriceTotalCorrection = previousCorrectionItem.PriceTotalAfter
                                    };
                                    previousCorrections.Add(previousCorrection);
                                }
                                else
                                    previousCorrection.PriceTotalCorrection = previousCorrectionItem.PriceTotalAfter;
                            }
                            else if (previousCorrectionNote.CorrectionType == "Harga Satuan")
                            {
                                var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == urnItem.Id);

                                if (previousCorrection == null)
                                {
                                    previousCorrection = new PreviousCorrection
                                    {
                                        URNItemId = (int)urnItem.Id,
                                        PricePerDealCorrection = previousCorrectionItem.PricePerDealUnitAfter
                                    };
                                    previousCorrections.Add(previousCorrection);
                                }
                                else
                                    previousCorrection.PricePerDealCorrection = previousCorrectionItem.PricePerDealUnitAfter;
                            }
                        }
                    }

                }
            }

            foreach (var urnNo in urnNos)
            {
                var journalTransactionToPost = new JournalTransaction()
                {
                    Date = unitPaymentCorrection.CorrectionDate,
                    Description = $"Nota Koreksi {urnNo}",
                    ReferenceNo = unitPaymentCorrection.UPCNo,
                    Status = "POSTED",
                    Items = new List<JournalTransactionItem>()
                };

                var journalDebitItems = new List<JournalTransactionItem>();
                var journalCreditItems = new List<JournalTransactionItem>();

                foreach (var unitPaymentCorrectionItem in unitPaymentCorrection.Items.Where(element => element.URNNo == urnNo))
                {
                    var unitReceiptNote = unitReceiptNotes.FirstOrDefault(entity => entity.URNNo == unitPaymentCorrectionItem.URNNo);
                    var unitPaymentOrderDetail = unitPaymentOrderDetails.FirstOrDefault(entity => entity.Id == unitPaymentCorrectionItem.UPODetailId);
                    var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(entity => entity.Id == unitPaymentOrderDetail.URNItemId);

                    var purchaseRequest = purchaseRequests.FirstOrDefault(w => w.Id == unitPaymentCorrectionItem.PRId);

                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(w => w.EPONo == unitPaymentCorrectionItem.EPONo);
                    var externalPurchaseOrderDetail = externalPurchaseOrderDetails.FirstOrDefault(entity => entity.Id == unitReceiptNoteItem.EPODetailId);

                    var jsonSerializerSettings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    var divisions = JsonConvert.DeserializeObject<List<IdCOAResult>>(_jsonDivisions ?? "[]", jsonSerializerSettings);
                    var units = JsonConvert.DeserializeObject<List<IdCOAResult>>(_jsonUnits ?? "[]", jsonSerializerSettings);
                    var categories = JsonConvert.DeserializeObject<List<CategoryCOAResult>>(_jsonCategories ?? "[]", jsonSerializerSettings);
                    var incomeTaxes = JsonConvert.DeserializeObject<List<IncomeTaxCOAResult>>(_jsonIncomeTaxes ?? "[]", jsonSerializerSettings);

                    //var purchaseRequestIds = unitReceiptNote.Items.Select(s => s.PRId).ToList();
                    //var purchaseRequests = dbContext.PurchaseRequests.Where(w => purchaseRequestIds.Contains(w.Id)).Select(s => new { s.Id, s.CategoryCode, s.CategoryId }).ToList();

                    //var externalPurchaseOrderIds = unitReceiptNote.Items.Select(s => s.EPOId).ToList();
                    //var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(w => externalPurchaseOrderIds.Contains(w.Id)).Select(s => new { s.Id, s.IncomeTaxId, s.UseIncomeTax, s.IncomeTaxName, s.IncomeTaxRate, s.CurrencyCode, s.CurrencyRate }).ToList();



                    //var externalPurchaseOrderDetailIds = unitReceiptNote.Items.Select(s => s.EPODetailId).ToList();
                    //var externalPurchaseOrderDetails = dbContext.ExternalPurchaseOrderDetails.Where(w => externalPurchaseOrderDetailIds.Contains(w.Id)).Select(s => new { s.Id, s.ProductId, TotalPrice = s.PricePerDealUnit * s.DealQuantity, s.DealQuantity }).ToList();

                    //var postMany = new List<Task<HttpResponseMessage>>();

                    //var journalTransactionsToPost = new List<JournalTransaction>();

                    int.TryParse(unitReceiptNote.DivisionId, out var divisionId);
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


                    int.TryParse(unitReceiptNote.UnitId, out var unitId);
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

                    var currencyCode = unitPaymentCorrectionItem.CurrencyCode;

                    var currency = await _currencyProvider.GetCurrencyByCurrencyCodeDate(unitPaymentCorrectionItem.CurrencyCode, unitReceiptNote.ReceiptDate);

                    var currencyRate = currency != null && currency != null ? (decimal)currency.Rate.GetValueOrDefault() : (decimal)externalPurchaseOrder.CurrencyRate;

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
                    var externalPOPriceTotal = externalPurchaseOrderDetail.PricePerDealUnit * externalPurchaseOrderDetail.DealQuantity;

                    var grandTotal = (decimal)0.0;
                    if (unitPaymentCorrection.CorrectionType == "Harga Total")
                    {
                        var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == unitReceiptNoteItem.Id);

                        if (previousCorrection == null || previousCorrection.PriceTotalCorrection == 0)
                            grandTotal = (decimal)(unitPaymentCorrectionItem.PriceTotalAfter - unitPaymentCorrectionItem.PriceTotalBefore);
                        else
                            grandTotal = (decimal)(unitPaymentCorrectionItem.PriceTotalAfter - previousCorrection.PriceTotalCorrection);

                        if (previousCorrection == null)
                        {
                            previousCorrection = new PreviousCorrection
                            {
                                URNItemId = (int)unitReceiptNoteItem.Id,
                                PriceTotalCorrection = unitPaymentCorrectionItem.PriceTotalAfter
                            };
                            previousCorrections.Add(previousCorrection);
                        }
                        else
                            previousCorrection.PriceTotalCorrection = unitPaymentCorrectionItem.PriceTotalAfter;
                        //grandTotal = (decimal)((correctionNoteItem.PriceTotalAfter - correctionNoteItem.PriceTotalBefore));
                    }
                    else if (unitPaymentCorrection.CorrectionType == "Harga Satuan")
                    {
                        var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == unitReceiptNoteItem.Id);

                        if (previousCorrection == null || previousCorrection.PricePerDealCorrection == 0)
                            grandTotal = (decimal)((unitPaymentCorrectionItem.PricePerDealUnitAfter - unitPaymentCorrectionItem.PricePerDealUnitBefore) * unitPaymentCorrectionItem.Quantity);
                        else
                            grandTotal = (decimal)((unitPaymentCorrectionItem.PricePerDealUnitAfter - previousCorrection.PricePerDealCorrection) * unitPaymentCorrectionItem.Quantity);

                        if (previousCorrection == null)
                        {
                            previousCorrection = new PreviousCorrection
                            {
                                URNItemId = (int)unitReceiptNoteItem.Id,
                                PricePerDealCorrection = unitPaymentCorrectionItem.PricePerDealUnitAfter
                            };
                            previousCorrections.Add(previousCorrection);
                        }
                        else
                            previousCorrection.PricePerDealCorrection = unitPaymentCorrectionItem.PricePerDealUnitAfter;


                        //grandTotal = (decimal)((correctionNoteItem.PricePerDealUnitAfter - correctionNoteItem.PricePerDealUnitBefore) * correctionNoteItem.Quantity);
                    }
                    else if (unitPaymentCorrection.CorrectionType == "Jumlah")
                    {
                        grandTotal = (decimal)(unitPaymentCorrectionItem.Quantity * unitPaymentCorrectionItem.PricePerDealUnitAfter * -1);
                        //grandTotal = (decimal)(correctionNoteItem.Quantity * unitPaymentOrderDetail.PricePerDealUnit);
                    }

                    if (grandTotal != 0)
                    {
                        if (unitPaymentCorrection.useIncomeTax)
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
                                    Code = unitReceiptNote.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Debit = incomeTaxTotal * (decimal)currencyRate
                            });

                            journalCreditItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{incomeTax.COACodeCredit}.{division.COACode}.{unit.COACode}"
                                },
                                Credit = incomeTaxTotal * (decimal)currencyRate
                            });
                        }

                        if (unitPaymentCorrection.useVat)
                        {
                            var inVATCOA = "1509.00";
                            var totalVAT = (decimal)0.1 * grandTotal;
                            journalCreditItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = unitReceiptNote.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Credit = totalVAT * (decimal)currencyRate
                            });

                            journalDebitItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{inVATCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Debit = totalVAT * (decimal)currencyRate
                            });
                        }

                        if (unitReceiptNote.SupplierIsImport && ((decimal)externalPOPriceTotal * currencyRate) > 100000000)
                        {
                            //Purchasing Journal Item
                            journalDebitItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Debit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
                            });

                            //Debt Journal Item
                            journalCreditItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Credit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
                            });

                            //Stock Journal Item
                            journalDebitItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{category.StockCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Debit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
                            });

                            //Purchasing Journal Item
                            journalCreditItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = $"{category.PurchasingCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Credit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
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
                                Debit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
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
                                    Debit = grandTotal * (decimal)currencyRate,
                                    Remark = $"- {unitReceiptNoteItem.ProductName}"
                                });
                            }


                            //Debt Journal Item
                            journalCreditItems.Add(new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = unitReceiptNote.SupplierIsImport ? $"{category.ImportDebtCOA}.{division.COACode}.{unit.COACode}" : $"{category.LocalDebtCOA}.{division.COACode}.{unit.COACode}"
                                },
                                Credit = grandTotal * (decimal)currencyRate,
                                Remark = $"- {unitReceiptNoteItem.ProductName}"
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
                                    Credit = grandTotal * (decimal)currencyRate,
                                    Remark = $"- {unitReceiptNoteItem.ProductName}"
                                });
                            }
                        }
                    }


                    //journalDebitItems = journalDebitItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
                    //{
                    //    COA = new COA()
                    //    {
                    //        Code = s.Key
                    //    },
                    //    Debit = s.Sum(sum => sum.Debit) > 0 ? Math.Abs(s.Sum(sum => sum.Debit.GetValueOrDefault())) : 0,
                    //    Credit = s.Sum(sum => sum.Debit) > 0 ? 0 : Math.Abs(s.Sum(sum => sum.Debit.GetValueOrDefault())),
                    //    Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
                    //}).ToList();
                    //journalTransactionToPost.Items.AddRange(journalDebitItems);

                    //journalCreditItems = journalCreditItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
                    //{
                    //    COA = new COA()
                    //    {
                    //        Code = s.Key
                    //    },
                    //    Credit = s.Sum(sum => sum.Credit) > 0 ? Math.Abs(s.Sum(sum => sum.Credit.GetValueOrDefault())) : 0,
                    //    Debit = s.Sum(sum => sum.Credit) > 0 ? 0 : Math.Abs(s.Sum(sum => sum.Credit.GetValueOrDefault())),
                    //    Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
                    //}).ToList();
                    //journalTransactionToPost.Items.AddRange(journalCreditItems);

                    //if (journalTransactionToPost.Items.Any(item => item.COA.Code.Split(".").FirstOrDefault().Equals("9999")))
                    //    journalTransactionToPost.Status = "DRAFT";

                    //if (journalTransactionToPost.Items.Count > 0)
                    //{
                    //    var journalTransactionUri = "journal-transactions";
                    //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(journalTransactionToPost).ToString(), Encoding.UTF8, General.JsonMediaType));

                    //    response.EnsureSuccessStatusCode();
                    //}
                }

                journalDebitItems = journalDebitItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
                {
                    COA = new COA()
                    {
                        Code = s.Key
                    },
                    Debit = s.Sum(sum => sum.Debit) > 0 ? Math.Abs(s.Sum(sum => sum.Debit.GetValueOrDefault())) : 0,
                    Credit = s.Sum(sum => sum.Debit) > 0 ? 0 : Math.Abs(s.Sum(sum => sum.Debit.GetValueOrDefault())),
                    Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
                }).ToList();
                journalTransactionToPost.Items.AddRange(journalDebitItems);

                journalCreditItems = journalCreditItems.GroupBy(grouping => grouping.COA.Code).Select(s => new JournalTransactionItem()
                {
                    COA = new COA()
                    {
                        Code = s.Key
                    },
                    Credit = s.Sum(sum => sum.Credit) > 0 ? Math.Abs(s.Sum(sum => sum.Credit.GetValueOrDefault())) : 0,
                    Debit = s.Sum(sum => sum.Credit) > 0 ? 0 : Math.Abs(s.Sum(sum => sum.Credit.GetValueOrDefault())),
                    Remark = string.Join("\n", s.Select(grouped => grouped.Remark).ToList())
                }).ToList();
                journalTransactionToPost.Items.AddRange(journalCreditItems);

                if (journalTransactionToPost.Items.Any(item => item.COA.Code.Split(".").FirstOrDefault().Equals("9999")))
                    journalTransactionToPost.Status = "DRAFT";

                if (journalTransactionToPost.Items.Count > 0)
                {
                    var journalTransactionUri = "journal-transactions";
                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(journalTransactionToPost).ToString(), Encoding.UTF8, General.JsonMediaType));

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private async Task AutoCreateCreditorAccount(UnitPaymentCorrectionNote unitPaymentCorrectionNote)
        {
            var prIds = unitPaymentCorrectionNote.Items.Select(element => element.PRId).ToList();
            var purchaseRequests = dbContext.PurchaseRequests.Where(entity => prIds.Contains(entity.Id)).ToList();
            var epoNos = unitPaymentCorrectionNote.Items.Select(element => element.EPONo).ToList();
            var externalPurchaseOrders = dbContext.ExternalPurchaseOrders.Where(entity => epoNos.Contains(entity.EPONo)).ToList();
            var urnNos = unitPaymentCorrectionNote.Items.Select(element => element.URNNo).Distinct().ToList();
            var unitReceiptNotes = dbContext.UnitReceiptNotes.Where(element => urnNos.Contains(element.URNNo)).ToList();
            var upoDetailIds = unitPaymentCorrectionNote.Items.Select(element => element.UPODetailId).ToList();
            var unitPaymentOrderDetails = dbContext.UnitPaymentOrderDetails.Where(entity => upoDetailIds.Contains(entity.Id)).ToList();
            var urnItemIds = unitPaymentOrderDetails.Select(element => element.URNItemId).ToList();
            var unitReceiptNoteItems = dbContext.UnitReceiptNoteItems.Where(entity => urnItemIds.Contains(entity.Id)).ToList();
            var epoDetailIds = unitReceiptNoteItems.Select(element => element.EPODetailId).ToList();
            var externalPurchaseOrderDetails = dbContext.ExternalPurchaseOrderDetails.Where(entity => epoDetailIds.Contains(entity.Id)).ToList();
            var correctionItems = dbContext.UnitPaymentCorrectionNoteItems.Where(entity => urnNos.Contains(entity.URNNo)).ToList();
            var correctionIds = correctionItems.Select(element => element.UPCId).ToList();
            var previousCorrectionNotes = dbContext.UnitPaymentCorrectionNotes.Where(entity => correctionIds.Contains(entity.Id) && entity.CorrectionDate < unitPaymentCorrectionNote.CorrectionDate).ToList();
            var previousCorrections = new List<PreviousCorrection>();
            var unitPaymentOrder = dbContext.UnitPaymentOrders.FirstOrDefault(entity => entity.Id == unitPaymentCorrectionNote.UPOId);

            foreach (var previousCorrectionNote in previousCorrectionNotes.OrderBy(element => element.CorrectionDate))
            {
                var previousCorrectionItems = correctionItems.Where(element => element.UPCId == previousCorrectionNote.Id).ToList();
                foreach (var previousCorrectionItem in previousCorrectionItems)
                {
                    var upoDetail = unitPaymentOrderDetails.FirstOrDefault(element => element.Id == previousCorrectionItem.UPODetailId);
                    if (upoDetail != null)
                    {
                        var urnItem = unitReceiptNoteItems.FirstOrDefault(element => element.Id == upoDetail.URNItemId);

                        if (urnItem != null)
                        {
                            if (previousCorrectionNote.CorrectionType == "Harga Total")
                            {
                                var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == urnItem.Id);

                                if (previousCorrection == null)
                                {
                                    previousCorrection = new PreviousCorrection
                                    {
                                        URNItemId = (int)urnItem.Id,
                                        PriceTotalCorrection = previousCorrectionItem.PriceTotalAfter
                                    };
                                    previousCorrections.Add(previousCorrection);
                                }
                                else
                                    previousCorrection.PriceTotalCorrection = previousCorrectionItem.PriceTotalAfter;
                            }
                            else if (previousCorrectionNote.CorrectionType == "Harga Satuan")
                            {
                                var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == urnItem.Id);

                                if (previousCorrection == null)
                                {
                                    previousCorrection = new PreviousCorrection
                                    {
                                        URNItemId = (int)urnItem.Id,
                                        PricePerDealCorrection = previousCorrectionItem.PricePerDealUnitAfter
                                    };
                                    previousCorrections.Add(previousCorrection);
                                }
                                else
                                    previousCorrection.PricePerDealCorrection = previousCorrectionItem.PricePerDealUnitAfter;
                            }
                        }
                    }

                }
            }

            foreach (var urnNo in urnNos)
            {
                foreach (var unitPaymentCorrectionNoteItem in unitPaymentCorrectionNote.Items.Where(element => element.URNNo == urnNo))
                {
                    var dppAmount = (decimal)0;
                    var vatAmount = (decimal)0;
                    var incomeTaxAmount = (decimal)0;
                    var unitPaymentOrderDetail = unitPaymentOrderDetails.FirstOrDefault(element => element.Id == unitPaymentCorrectionNoteItem.UPODetailId);
                    //var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(entity => entity.Id == unitPaymentCorrectionNote.UPOId);
                    var unitReceiptNoteItem = unitReceiptNoteItems.FirstOrDefault(entity => entity.Id == unitPaymentOrderDetail.URNItemId);
                    var unitReceiptNote = unitReceiptNotes.FirstOrDefault(entity => entity.Id == unitReceiptNoteItem.URNId);
                    if (unitPaymentCorrectionNote.CorrectionType == "Harga Total")
                    {
                        var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == unitReceiptNoteItem.Id);
                        if (previousCorrection == null || previousCorrection.PriceTotalCorrection == 0)
                            dppAmount = (decimal)(unitPaymentCorrectionNoteItem.PriceTotalAfter - unitPaymentCorrectionNoteItem.PriceTotalBefore);
                        else
                            dppAmount = (decimal)(unitPaymentCorrectionNoteItem.PriceTotalAfter - previousCorrection.PriceTotalCorrection);

                        if (previousCorrection == null)
                        {
                            previousCorrection = new PreviousCorrection
                            {
                                URNItemId = (int)unitReceiptNoteItem.Id,
                                PriceTotalCorrection = unitPaymentCorrectionNoteItem.PriceTotalAfter
                            };
                            previousCorrections.Add(previousCorrection);
                        }
                        else
                            previousCorrection.PriceTotalCorrection = unitPaymentCorrectionNoteItem.PriceTotalAfter;
                    }
                    else if (unitPaymentCorrectionNote.CorrectionType == "Harga Satuan")
                    {
                        var previousCorrection = previousCorrections.FirstOrDefault(element => element.URNItemId == unitReceiptNoteItem.Id);
                        if (previousCorrection == null || previousCorrection.PricePerDealCorrection == 0)
                            dppAmount = (decimal)((unitPaymentCorrectionNoteItem.PricePerDealUnitAfter - unitPaymentCorrectionNoteItem.PricePerDealUnitBefore) * unitPaymentCorrectionNoteItem.Quantity);
                        else
                            dppAmount = (decimal)((unitPaymentCorrectionNoteItem.PricePerDealUnitAfter - previousCorrection.PricePerDealCorrection) * unitPaymentCorrectionNoteItem.Quantity);

                        if (previousCorrection == null)
                        {
                            previousCorrection = new PreviousCorrection
                            {
                                URNItemId = (int)unitReceiptNoteItem.Id,
                                PricePerDealCorrection = unitPaymentCorrectionNoteItem.PricePerDealUnitAfter
                            };
                            previousCorrections.Add(previousCorrection);
                        }
                        else
                            previousCorrection.PricePerDealCorrection = unitPaymentCorrectionNoteItem.PricePerDealUnitAfter;
                    }
                    else if (unitPaymentCorrectionNote.CorrectionType == "Jumlah")
                    {
                        //if (previousCorrection.QuantityCorrection == 0)
                        dppAmount = (decimal)(unitPaymentCorrectionNoteItem.Quantity * unitPaymentCorrectionNoteItem.PricePerDealUnitAfter * -1);
                        //else
                        //dppAmount = (decimal)((unitPaymentCorrectionNoteItem.PriceTotalAfter - previousCorrection.QuantityCorrection));

                        //previousCorrection.QuantityCorrection = unitPaymentCorrectionNoteItem.PriceTotalAfter;
                    }

                    if (unitPaymentCorrectionNote.useVat)
                        vatAmount = dppAmount * (decimal)0.1;

                    if (unitPaymentCorrectionNote.useIncomeTax && unitReceiptNoteItem.IncomeTaxBy == "Supplier")
                        incomeTaxAmount = dppAmount * (decimal)unitPaymentOrder.IncomeTaxRate / 100;

                    var currency = await _currencyProvider.GetCurrencyByCurrencyCodeDate(unitPaymentCorrectionNoteItem.CurrencyCode, unitReceiptNote.ReceiptDate);

                    if (currency == null)
                    {
                        currency = new Currency() { Rate = (double)unitPaymentOrder.CurrencyRate };
                    }

                    if (dppAmount + vatAmount - incomeTaxAmount != 0)
                    {
                        var viewModel = new CreateCreditorAccountViewModel()
                        {
                            UnitPaymentCorrectionDPP = dppAmount * (decimal)currency.Rate.GetValueOrDefault(),
                            UnitPaymentCorrectionId = (int)unitPaymentCorrectionNote.Id,
                            UnitPaymentCorrectionMutation = (dppAmount + vatAmount - incomeTaxAmount) * (decimal)currency.Rate.GetValueOrDefault(),
                            UnitPaymentCorrectionNo = unitPaymentCorrectionNote.UPCNo,
                            UnitPaymentCorrectionPPN = vatAmount * (decimal)currency.Rate.GetValueOrDefault(),
                            UnitReceiptNoteNo = unitPaymentCorrectionNoteItem.URNNo,
                            UnitPaymentCorrectionDate = unitPaymentCorrectionNote.CorrectionDate
                        };

                        var uri = "creditor-account/unit-payment-correction";
                        var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                        var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{uri}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, General.JsonMediaType));

                        response.EnsureSuccessStatusCode();
                    }
                }
            }

            //var upoDetailIds = unitPaymentCorrectionNote.Items.Select(element => element.UPODetailId).ToList();
            //var unitPaymentOrderDetails = dbContext.UnitPaymentOrderDetails.Where(entity => upoDetailIds.Contains(entity.Id)).ToList();
            //foreach (var unitPaymentCorrectionNoteItem in unitPaymentCorrectionNote.Items)
            //{
            //    var dppAmount = (decimal)0;
            //    var vatAmount = (decimal)0;
            //    var unitPaymentOrderDetail = unitPaymentOrderDetails.FirstOrDefault(element => element.Id == unitPaymentCorrectionNoteItem.UPODetailId);

            //    if (unitPaymentCorrectionNote.CorrectionType == "Harga Total")
            //    {
            //        dppAmount = (decimal)(unitPaymentCorrectionNoteItem.PriceTotalAfter - unitPaymentCorrectionNoteItem.PriceTotalBefore);
            //    }
            //    else if (unitPaymentCorrectionNote.CorrectionType == "Harga Satuan")
            //    {
            //        dppAmount = (decimal)(unitPaymentCorrectionNoteItem.PricePerDealUnitAfter - unitPaymentCorrectionNoteItem.PricePerDealUnitBefore);
            //    }
            //    else if (unitPaymentCorrectionNote.CorrectionType == "Jumlah")
            //    {
            //        dppAmount = (decimal)(unitPaymentOrderDetail.QuantityCorrection * unitPaymentOrderDetail.PricePerDealUnit);
            //    }

            //    if (unitPaymentCorrectionNote.useVat)
            //        vatAmount = dppAmount * (decimal)0.1;


            //    var viewModel = new CreateCreditorAccountViewModel()
            //    {
            //        UnitPaymentCorrectionDPP = dppAmount,
            //        UnitPaymentCorrectionId = (int)unitPaymentCorrectionNote.Id,
            //        UnitPaymentCorrectionMutation = dppAmount + vatAmount,
            //        UnitPaymentCorrectionNo = unitPaymentCorrectionNote.UPCNo,
            //        UnitPaymentCorrectionPPN = vatAmount,
            //        UnitReceiptNoteNo = unitPaymentCorrectionNoteItem.URNNo,
            //        UnitPaymentCorrectionDate = unitPaymentCorrectionNote.CorrectionDate
            //    };

            //    var uri = "creditor-account/unit-payment-correction";
            //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{uri}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, General.JsonMediaType));

            //    response.EnsureSuccessStatusCode();
            //}
        }

        //private async Task ReverseJournalTransaction(string referenceNo)
        //{
        //    string journalTransactionUri = $"journal-transactions/reverse-transactions/{referenceNo}";
        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(new object()).ToString(), Encoding.UTF8, General.JsonMediaType));

        //    response.EnsureSuccessStatusCode();
        //}

        public UnitReceiptNote ReadByURNNo(string uRNNo)
        {
            var a = dbContext.UnitReceiptNotes.Where(p => p.URNNo == uRNNo)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }
        //public UnitReceiptNote ReadByURNNo(string uRNNo)
        //{
        //    return dbContext.UnitReceiptNotes.Where(m => m.URNNo == uRNNo)
        //        .Include(p => p.Items)
        //        .FirstOrDefault();
        //}

        //public async Task<int> Update(int id, UnitPaymentCorrectionNote unitPaymentCorrectionNote, string user)
        //{
        //    int Updated = 0;

        //    using (var transaction = this.dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var m = this.dbSet.AsNoTracking()
        //                .Include(d => d.Items)
        //                .Single(pr => pr.Id == id && !pr.IsDeleted);

        //            if (m != null && id == unitPaymentCorrectionNote.Id)
        //            {

        //                EntityExtension.FlagForUpdate(unitPaymentCorrectionNote, user, USER_AGENT);

        //                foreach (var item in unitPaymentCorrectionNote.Items)
        //                {
        //                    if (item.Id == 0)
        //                    {
        //                        EntityExtension.FlagForCreate(item, user, USER_AGENT);
        //                    }
        //                    else
        //                    {
        //                        EntityExtension.FlagForUpdate(item, user, USER_AGENT);
        //                    }
        //                }

        //                this.dbContext.Update(unitPaymentCorrectionNote);

        //                foreach (var item in m.Items)
        //                {
        //                    UnitPaymentCorrectionNoteItem unitPaymentCorrectionNoteItem = unitPaymentCorrectionNote.Items.FirstOrDefault(i => i.Id.Equals(item.Id));
        //                    if (unitPaymentCorrectionNoteItem == null)
        //                    {
        //                        EntityExtension.FlagForDelete(item, user, USER_AGENT);
        //                        this.dbContext.UnitPaymentCorrectionNoteItems.Update(item);
        //                    }
        //                }

        //                Updated = await dbContext.SaveChangesAsync();
        //                transaction.Commit();
        //            }
        //            else
        //            {
        //                throw new Exception("Invalid Id");
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    return Updated;
        //}

        //public int Delete(int id, string user)
        //{
        //    int Deleted = 0;

        //    using (var transaction = this.dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var m = this.dbSet
        //                .Include(d => d.Items)
        //                .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

        //            EntityExtension.FlagForDelete(m, user, USER_AGENT);

        //            foreach (var item in m.Items)
        //            {
        //                EntityExtension.FlagForDelete(item, user, USER_AGENT);
        //            }

        //            Deleted = dbContext.SaveChanges();
        //            transaction.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    return Deleted;
        //}
        #region Monitoring Correction Jumlah 
        public IQueryable<UnitPaymentQuantityCorrectionNoteReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.UnitPaymentCorrectionNotes
                         join i in dbContext.UnitPaymentCorrectionNoteItems on a.Id equals i.UPCId
                         join j in dbContext.UnitReceiptNotes on i.URNNo equals j.URNNo
                         where a.IsDeleted == false
                             && a.CorrectionType == "Jumlah"
                             && i.IsDeleted == false
                             // && (a.CorrectionType == "Harga Total" || a.CorrectionType == "Harga Satuan")
                             && a.CorrectionDate.AddHours(offset).Date >= DateFrom.Date
                             && a.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                         select new UnitPaymentQuantityCorrectionNoteReportViewModel
                         {
                             upcNo = a.UPCNo,
                             epoNo = i.EPONo,
                             upoNo = a.UPONo,
                             prNo = i.PRNo,
                             notaRetur = a.ReturNoteNo,
                             vatTaxCorrectionNo = a.VatTaxCorrectionNo,
                             vatTaxCorrectionDate = a.VatTaxCorrectionDate,
                             correctionDate = a.CorrectionDate,
                             unit = j.UnitName,
                             category = a.CategoryName,
                             supplier = a.SupplierName,
                             productCode = i.ProductCode,
                             productName = i.ProductName,
                             jumlahKoreksi = i.Quantity,
                             satuanKoreksi = i.UomUnit,
                             hargaSatuanKoreksi = i.PricePerDealUnitAfter,
                             hargaTotalKoreksi = i.PriceTotalAfter,
                             user = a.CreatedBy,
                             jenisKoreksi = a.CorrectionType,

                         });
            return Query;
        }

        public Tuple<List<UnitPaymentQuantityCorrectionNoteReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            }


            Pageable<UnitPaymentQuantityCorrectionNoteReportViewModel> pageable = new Pageable<UnitPaymentQuantityCorrectionNoteReportViewModel>(Query, page - 1, size);
            List<UnitPaymentQuantityCorrectionNoteReportViewModel> Data = pageable.Data.ToList<UnitPaymentQuantityCorrectionNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Debet", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Debet", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No SPB", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eks", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Retur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Faktur Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Faktur Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kd Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nm Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Satuan Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "User Input", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Koreksi", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, 0, "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string correctionDate = item.correctionDate == null ? "-" : item.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    DateTimeOffset date = item.vatTaxCorrectionDate ?? new DateTime(1970, 1, 1);
                    string vatDate = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.upcNo, correctionDate, item.upoNo, item.epoNo, item.prNo, item.notaRetur, item.vatTaxCorrectionNo, vatDate, item.unit, item.category, item.supplier, item.productCode, item.productName, item.jumlahKoreksi, item.satuanKoreksi, item.hargaSatuanKoreksi, item.hargaTotalKoreksi, item.user, item.jenisKoreksi);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion 

        private async Task<int> AddCorrections(UnitPaymentCorrectionNote model, string username)
        {
            var internalPOFacade = serviceProvider.GetService<InternalPurchaseOrderFacade>();
            int count = 0;
            foreach (var item in model.Items)
            {

                var fulfillment = await dbContext.InternalPurchaseOrderFulfillments.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UnitPaymentOrderId == model.UPOId && x.UnitPaymentOrderDetailId == item.UPODetailId);

                if (fulfillment != null)
                {
                    fulfillment.Corrections.Add(new InternalPurchaseOrderCorrection()
                    {
                        CorrectionDate = model.CorrectionDate,
                        CorrectionNo = model.UPCNo,
                        CorrectionPriceTotal = item.PriceTotalAfter,
                        CorrectionQuantity = item.Quantity,
                        CorrectionRemark = model.Remark,
                        UnitPaymentCorrectionId = model.Id,
                        UnitPaymentCorrectionItemId = item.Id
                    });

                    count += await internalPOFacade.UpdateFulfillmentAsync(fulfillment.Id, fulfillment, username);
                }
            }

            return count;
        }

        public async Task<CorrectionState> GetCorrectionStateByUnitPaymentOrderId(int unitPaymentOrderId)
        {
            return new CorrectionState()
            {
                IsHavingPricePerUnitCorrection = await dbSet.AnyAsync(entity => entity.UPOId == unitPaymentOrderId && entity.CorrectionType == "Harga Satuan"),
                IsHavingPriceTotalCorrection = await dbSet.AnyAsync(entity => entity.UPOId == unitPaymentOrderId && entity.CorrectionType == "Harga Total"),
                IsHavingQuantityCorrection = await dbSet.AnyAsync(entity => entity.UPOId == unitPaymentOrderId && entity.CorrectionType == "Jumlah")
            };
        }
    }
}