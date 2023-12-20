using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
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
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade
{
    public class UnitPaymentPriceCorrectionNoteFacade : IUnitPaymentPriceCorrectionNoteFacade
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

        public UnitPaymentPriceCorrectionNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            dbSet = dbContext.Set<UnitPaymentCorrectionNote>();
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
                }).Where(s => s.CorrectionType == "Harga Satuan" || s.CorrectionType == "Harga Total").OrderByDescending(j => j.LastModifiedUtc);

            List<string> searchAttributes = new List<string>()
            {
                "UPCNo", "UPONo", "SupplierName","InvoiceCorrectionNo"
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

        async Task<string> GenerateNo(UnitPaymentCorrectionNote model, bool supplierImport, int clientTimeZoneOffset)
        {
            string Year = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string supplier_imp = "NRL";
            if (supplierImport == true)
            {
                supplier_imp = "NRI";
            }
            string divisionName = model.DivisionName;
            string division_name = "T";
            if (divisionName.ToUpper() == "EFRATA")
            {
                division_name = "G";
            }

            string no = $"{Year}-{Month}-{division_name}-{supplier_imp}-";
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.UPCNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.UPCNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.UPCNo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public async Task<int> Create(UnitPaymentCorrectionNote model, bool supplierImport, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

                    //override correctionDate to created date
                    model.CorrectionDate = DateTimeOffset.Now.ToUniversalTime();

                    model.UPCNo = await GenerateNo(model, supplierImport, clientTimeZoneOffset);
                    if (model.useVat == true)
                    {
                        model.ReturNoteNo = await GeneratePONo(model, clientTimeZoneOffset);
                    }
                    UnitPaymentOrder unitPaymentOrder = this.dbContext.UnitPaymentOrders.FirstOrDefault(s => s.Id == model.UPOId);
                    unitPaymentOrder.IsCorrection = true;

                    foreach (var item in model.Items)
                    {

                        UnitPaymentOrderDetail upoDetail = dbContext.UnitPaymentOrderDetails.FirstOrDefault(s => s.Id == item.UPODetailId);
                        item.PricePerDealUnitBefore = upoDetail.PricePerDealUnit;
                        

                        if (upoDetail.PriceTotal != upoDetail.PriceTotalCorrection)
                        {
                            item.PriceTotalBefore = upoDetail.PriceTotalCorrection;
                        }
                        else
                        {
                            item.PriceTotalBefore = upoDetail.PriceTotal;
                        }

                        upoDetail.PriceTotalCorrection = item.PriceTotalAfter;

                        upoDetail.PricePerDealUnitCorrection = item.PricePerDealUnitAfter;
                        

                        if (item.PriceTotalAfter > 0 && item.PricePerDealUnitAfter <= 0)
                        {
                            upoDetail.PricePerDealUnitCorrection = upoDetail.PricePerDealUnit;
                        }

                        EntityExtension.FlagForCreate(item, username, USER_AGENT);
                    }

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    Created += await AddCorrections(model, username);

                    await AutoCreateJournalTransaction(model);
                    await AutoCreateCreditorAccount(model);

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

        //private async Task ReverseJournalTransaction(string referenceNo)
        //{
        //    string journalTransactionUri = $"journal-transactions/reverse-transactions/{referenceNo}";
        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(new object()).ToString(), Encoding.UTF8, General.JsonMediaType));

        //    response.EnsureSuccessStatusCode();
        //}

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

        async Task<string> GeneratePONo(UnitPaymentCorrectionNote model, int clientTimeZoneOffset)
        {
            string Year = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");



            string no = $"{Year}-{Month}-NR-";
            int Padding = 3;

            var lastNo = await this.dbSet.Where(w => w.ReturNoteNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.ReturNoteNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.ReturNoteNo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public SupplierViewModel GetSupplier(string supplierId)
        {
            string supplierUri = "master/suppliers";
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            if (httpClient != null)
            {
                var resu = httpClient.GetAsync($"{APIEndpoint.Core}{supplierUri}/{supplierId}").Result;
                var response = resu.Content.ReadAsStringAsync();
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
        public UnitReceiptNote GetUrn(string urnNo)
        {
            var urnModel = dbContext.UnitReceiptNotes.SingleOrDefault(s => s.IsDeleted == false && s.URNNo == urnNo);//_urnFacade.ReadById((int)item.URNId);
            return urnModel;
        }

        public IQueryable<UnitPaymentPriceCorrectionNoteReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.UnitPaymentCorrectionNotes
                         join i in dbContext.UnitPaymentCorrectionNoteItems on a.Id equals i.UPCId
                         where a.IsDeleted == false
                             && i.IsDeleted == false
                             && (a.CorrectionType == "Harga Total" || a.CorrectionType == "Harga Satuan")
                             && a.CorrectionDate.AddHours(offset).Date >= DateFrom.Date
                             && a.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                         select new UnitPaymentPriceCorrectionNoteReportViewModel
                         {
                             upcNo = a.UPCNo,
                             epoNo = i.EPONo,
                             upoNo = a.UPONo,
                             prNo = i.PRNo,
                             vatTaxCorrectionNo = a.VatTaxCorrectionNo,
                             vatTaxCorrectionDate = a.VatTaxCorrectionDate,
                             correctionDate = a.CorrectionDate,
                             correctionType = a.CorrectionType,
                             pricePerDealUnitAfter = i.PricePerDealUnitAfter,
                             priceTotalAfter = i.PriceTotalAfter,
                             supplier = a.SupplierName,
                             productCode = i.ProductCode,
                             productName = i.ProductName,
                             uom = i.UomUnit,
                             quantity = i.Quantity,
                             user = a.CreatedBy,
                             LastModifiedUtc = i.LastModifiedUtc
                         });
            return Query;
        }

        public Tuple<List<UnitPaymentPriceCorrectionNoteReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            }


            Pageable<UnitPaymentPriceCorrectionNoteReportViewModel> pageable = new Pageable<UnitPaymentPriceCorrectionNoteReportViewModel>(Query, page - 1, size);
            List<UnitPaymentPriceCorrectionNoteReportViewModel> Data = pageable.Data.ToList<UnitPaymentPriceCorrectionNoteReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Debet", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Debet", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No SPB", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO Eksternal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Purchase Request", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Faktur Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Faktur Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Satuan Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Total Koreksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "User Input", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string correctionDate = item.correctionDate == null ? "-" : item.correctionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    DateTimeOffset date = item.vatTaxCorrectionDate ?? new DateTime(1970, 1, 1);
                    string vatDate = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, item.upcNo, correctionDate, item.upoNo, item.epoNo, item.prNo, item.vatTaxCorrectionNo, vatDate, item.supplier, item.correctionType, item.productCode, item.productName, item.quantity, item.uom, item.pricePerDealUnitAfter, item.priceTotalAfter, item.user);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public IQueryable<UnitPaymentCorrectionNoteGenerateDataViewModel> GetDataReportQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = (from a in dbContext.UnitPaymentCorrectionNotes
                         join b in dbContext.UnitPaymentCorrectionNoteItems on a.Id equals b.UPCId
                         join c in dbContext.UnitReceiptNotes on b.URNNo equals c.URNNo
                         join d in dbContext.UnitPaymentOrders on a.UPOId equals d.Id

                         where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false &&
                               a.CorrectionDate.AddHours(offset).Date >= DateFrom.Date &&
                               a.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                         select new UnitPaymentCorrectionNoteGenerateDataViewModel
                         {
                             UPCNo = a.UPCNo,
                             UPCDate = a.CorrectionDate,
                             CorrectionType = a.CorrectionType,
                             UPONo = a.UPONo,
                             InvoiceCorrectionNo = a.InvoiceCorrectionNo,
                             InvoiceCorrectionDate = a.InvoiceCorrectionDate.GetValueOrDefault(),
                             VatTaxCorrectionNo = a.VatTaxCorrectionNo,
                             VatTaxCorrectionDate = a.VatTaxCorrectionDate.GetValueOrDefault(),
                             IncomeTaxCorrectionNo = a.IncomeTaxCorrectionNo,
                             IncomeTaxCorrectionDate = a.InvoiceCorrectionDate.GetValueOrDefault(),
                             SupplierCode = a.SupplierCode,
                             SupplierName = a.SupplierName,
                             SupplierAddress = "",
                             ReleaseOrderNoteNo = a.ReleaseOrderNoteNo,
                             UPCRemark = a.Remark,
                             EPONo = b.EPONo,
                             PRNo = b.PRNo,
                             AccountNo = "-",
                             ProductCode = b.ProductCode,
                             ProductName = b.ProductName,
                             Quantity = b.Quantity,
                             UOMUnit = b.UomUnit,
                             PricePerDealUnit = b.PricePerDealUnitAfter,
                             CurrencyCode = b.CurrencyCode,
                             CurrencyRate = b.CurrencyRate,
                             PriceTotal = b.PriceTotalAfter,
                             URNNo = b.URNNo,
                             URNDate = c.ReceiptDate,
                             UserCreated = a.CreatedBy,
                             UseVat = a.useVat ? "YA " : "TIDAK",
                             UseIncomeTax = a.useIncomeTax ? "YA " : "TIDAK",
                             VatRate = d.VatRate,
                         }
                         );
            return Query;
        }

        public MemoryStream GenerateDataExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetDataReportQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.UPCNo);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR NOTA KOREKSI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL NOTA KOREKSI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "JENIS RETUR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR NOTA KREDIT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR INVOICE KOREKSI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL INVOICE KOREKSI", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "FAKTUR PAJAK KOREKSI PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL FAKTUR PAJAK KOREKSI PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "FAKTUR PAJAK KOREKSI PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL FAKTUR PAJAK KOREKSI PPH", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "KODE SUPPLIER", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "ALAMAT SUPPLIER", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR SURAT PENGANTAR", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KETERANGAN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PO EXTERNAL", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR PURCHASE REQUEST", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR ACCOUNT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "KODE BARANG", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "NAMA BARANG", DataType = typeof(String) });

            result.Columns.Add(new DataColumn() { ColumnName = "JUMLAH BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "SATUAN BARANG", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "HARGA SATUAN BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "MATA UANG", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "RATE", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "HARGA TOTAL BARANG", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "NOMOR BON TERIMA UNIT", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "TANGGAL BON TERIMA UNIT", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "USER INPUT", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RATE PPN", DataType = typeof(double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, "", "", 0, "", "", "", "", "", 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string UPCDate = item.UPCDate == new DateTime(1970, 1, 1) ? "-" : item.UPCDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string InvoiceCorrectionDate = item.InvoiceCorrectionDate == DateTimeOffset.MinValue ? "-" : item.InvoiceCorrectionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string VatTaxCorrectionDate = item.VatTaxCorrectionDate == DateTimeOffset.MinValue ? "-" : item.VatTaxCorrectionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string IncomeTaxCorrectionDate = item.IncomeTaxCorrectionDate == DateTimeOffset.MinValue ? "-" : item.IncomeTaxCorrectionDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));
                    string URNDate = item.URNDate == null ? "-" : item.URNDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.UPCNo, UPCDate, item.CorrectionType, item.UPONo, item.InvoiceCorrectionNo, InvoiceCorrectionDate, item.VatTaxCorrectionNo, VatTaxCorrectionDate, item.IncomeTaxCorrectionNo,
                                    IncomeTaxCorrectionDate, item.SupplierCode, item.SupplierName, item.SupplierAddress, item.ReleaseOrderNoteNo, item.UPCRemark, item.EPONo, item.PRNo, item.AccountNo, item.ProductCode,
                                    item.ProductName, item.Quantity, item.UOMUnit, item.PricePerDealUnit, item.CurrencyCode, item.CurrencyRate, item.PriceTotal, item.URNNo, URNDate, item.UserCreated, item.UseVat, item.UseIncomeTax, item.VatRate);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Sheet1") }, true);
        }

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


    }

    public class PreviousCorrection
    {
        public int UPODetailId { get; set; }
        public int URNItemId { get; set; }
        public double QuantityCorrection { get; set; }
        public double PricePerDealCorrection { get; set; }
        public double PriceTotalCorrection { get; set; }
    }
}
