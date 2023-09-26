using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
//using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
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
using System.IO;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades
{
    public class BankExpenditureNoteFacade : IBankExpenditureNoteFacade, IReadByIdable<BankExpenditureNoteModel>
    {
        private const string V = "Operasional";
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<BankExpenditureNoteModel> dbSet;
        private readonly DbSet<BankExpenditureNoteDetailModel> detailDbSet;
        private readonly DbSet<BankExpenditureNoteItemModel> itemDbSet;
        private readonly DbSet<UnitPaymentOrder> unitPaymentOrderDbSet;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IBankDocumentNumberGenerator bankDocumentNumberGenerator;
        public readonly IServiceProvider serviceProvider;


        private readonly string USER_AGENT = "Facade";
        private readonly string CREDITOR_ACCOUNT_URI = "creditor-account/bank-expenditure-note/list";

        public BankExpenditureNoteFacade(PurchasingDbContext dbContext, IBankDocumentNumberGenerator bankDocumentNumberGenerator, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            this.bankDocumentNumberGenerator = new BankDocumentNumberGenerator(dbContext);
            dbSet = dbContext.Set<BankExpenditureNoteModel>();
            detailDbSet = dbContext.Set<BankExpenditureNoteDetailModel>();
            itemDbSet = dbContext.Set<BankExpenditureNoteItemModel>();
            unitPaymentOrderDbSet = dbContext.Set<UnitPaymentOrder>();
            _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
            this.serviceProvider = serviceProvider;
        }

        public async Task<BankExpenditureNoteModel> ReadById(int id)
        {
            return await this.dbContext.BankExpenditureNotes
                .AsNoTracking()
                    .Include(p => p.Details)
                        .ThenInclude(p => p.Items)
                .Where(d => d.Id.Equals(id) && d.IsDeleted.Equals(false))
                .FirstOrDefaultAsync();
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<BankExpenditureNoteModel> Query = this.dbSet;

            var queryItems = Query.Select(x => x.Details.Select(y => y.Items).ToList());

            Query = Query
                .Select(s => new BankExpenditureNoteModel
                {
                    Id = s.Id,
                    CreatedUtc = s.CreatedUtc,
                    LastModifiedUtc = s.LastModifiedUtc,
                    BankName = s.BankName,
                    BankAccountName = s.BankAccountName,
                    BankAccountNumber = s.BankAccountNumber,
                    DocumentNo = s.DocumentNo,
                    SupplierName = s.SupplierName,
                    GrandTotal = s.GrandTotal,
                    BankCurrencyCode = s.BankCurrencyCode,
                    CurrencyRate = s.CurrencyRate,
                    IsPosted = s.IsPosted,
                    Date = s.Date,
                    Details = s.Details.Where(x => x.BankExpenditureNoteId == s.Id).Select(a => new BankExpenditureNoteDetailModel
                    {
                        SupplierName = a.SupplierName,
                        UnitPaymentOrderNo = a.UnitPaymentOrderNo,
                        Items = a.Items.Where(b => b.BankExpenditureNoteDetailId == a.Id).ToList()
                    }).ToList()
                });

            List<string> searchAttributes = new List<string>()
            {
                "DocumentNo", "BankName", "SupplierName","BankCurrencyCode"
            };

            Query = QueryHelper<BankExpenditureNoteModel>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<BankExpenditureNoteModel>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<BankExpenditureNoteModel>.ConfigureOrder(Query, OrderDictionary);

            Pageable<BankExpenditureNoteModel> pageable = new Pageable<BankExpenditureNoteModel>(Query, Page - 1, Size);
            List<BankExpenditureNoteModel> Data = pageable.Data.ToList();

            List<object> list = new List<object>();
            list.AddRange(
               Data.Select(s => new
               {
                   s.Id,
                   s.DocumentNo,
                   s.CreatedUtc,
                   s.BankName,
                   s.BankAccountName,
                   s.BankAccountNumber,
                   s.SupplierName,
                   s.GrandTotal,
                   s.BankCurrencyCode,
                   s.IsPosted,
                   s.Date,
                   Details = s.Details.Select(sl => new { sl.SupplierName, sl.UnitPaymentOrderNo, sl.Items }).ToList(),
               }).ToList()
            );

            int TotalData = pageable.TotalCount;

            return new ReadResponse<object>(list, TotalData, OrderDictionary);
        }

        public async Task<int> Update(int id, BankExpenditureNoteModel model, IdentityService identityService)
        {
            int Updated = 0;
            string username = identityService.Username;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForUpdate(model, username, USER_AGENT);
                    dbContext.Entry(model).Property(x => x.GrandTotal).IsModified = true;
                    dbContext.Entry(model).Property(x => x.BGCheckNumber).IsModified = true;
                    dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                    dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                    dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;

                    foreach (var detail in model.Details)
                    {
                        if (detail.Id == 0)
                        {
                            var paidFlag = true;

                            var query = dbContext.BankExpenditureNoteDetails.Where(x => x.UnitPaymentOrderNo == detail.UnitPaymentOrderNo).ToList();

                            if (query.Count == 0)
                            {
                                if (detail.SupplierPayment != detail.TotalPaid)
                                {
                                    paidFlag = false;
                                }

                                detail.UPOIndex = 1;
                            }
                            else
                            {
                                if ((query.Sum(x => x.SupplierPayment) + detail.SupplierPayment) != detail.TotalPaid)
                                {
                                    paidFlag = false;
                                }

                                detail.UPOIndex = query.Last().UPOIndex + 1;
                            }

                            EntityExtension.FlagForCreate(detail, username, USER_AGENT);
                            dbContext.BankExpenditureNoteDetails.Add(detail);

                            var pdeExisting = dbContext.PurchasingDocumentExpeditions.FirstOrDefault(entity => entity.BankExpenditureNoteNo == model.DocumentNo && entity.UnitPaymentOrderNo == detail.UnitPaymentOrderNo);
                            if (pdeExisting != null)
                            {
                                pdeExisting.IsPaid = paidFlag;
                                pdeExisting.BankExpenditureNoteNo = model.DocumentNo;
                                pdeExisting.BankExpenditureNoteDate = model.Date;
                                pdeExisting.AmountPaid = detail.AmountPaid;
                                pdeExisting.SupplierPayment = detail.SupplierPayment;

                                EntityExtension.FlagForUpdate(pdeExisting, username, USER_AGENT);

                                dbContext.PurchasingDocumentExpeditions.Update(pdeExisting);
                            }

                            //PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                            //{
                            //    Id = (int)detail.UnitPaymentOrderId,
                            //    IsPaid = paidFlag,
                            //    BankExpenditureNoteNo = model.DocumentNo,
                            //    BankExpenditureNoteDate = model.Date,
                            //    AmountPaid = detail.AmountPaid,
                            //    SupplierPayment = detail.SupplierPayment
                            //};

                            //EntityExtension.FlagForUpdate(pde, username, USER_AGENT);
                            //dbContext.Attach(pde);
                            //dbContext.Entry(pde).Property(x => x.IsPaid).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteNo).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.AmountPaid).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.SupplierPayment).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteDate).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedAgent).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedBy).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedUtc).IsModified = true;

                            foreach (var item in detail.Items)
                            {
                                EntityExtension.FlagForCreate(item, username, USER_AGENT);
                            }
                        }
                        else
                        {
                            var paidFlag = true;

                            var query = dbContext.BankExpenditureNoteDetails.Where(x => x.UnitPaymentOrderNo == detail.UnitPaymentOrderNo).ToList();

                            if (query.Count == 0)
                            {
                                if (detail.SupplierPayment != detail.TotalPaid)
                                {
                                    paidFlag = false;
                                }
                            }
                            else
                            {
                                var previousAmountPaid = query.SkipLast(1);

                                if ((previousAmountPaid.Sum(x => x.SupplierPayment) + detail.SupplierPayment) != detail.TotalPaid)
                                {
                                    paidFlag = false;
                                }
                            }

                            EntityExtension.FlagForUpdate(detail, username, USER_AGENT);
                            dbContext.Entry(detail).Property(x => x.SupplierPayment).IsModified = true;

                            var pdeExisting = dbContext.PurchasingDocumentExpeditions.FirstOrDefault(entity => entity.BankExpenditureNoteNo == model.DocumentNo && entity.UnitPaymentOrderNo == detail.UnitPaymentOrderNo);
                            if (pdeExisting != null)
                            {
                                pdeExisting.IsPaid = paidFlag;
                                pdeExisting.BankExpenditureNoteNo = model.DocumentNo;
                                pdeExisting.BankExpenditureNoteDate = model.Date;
                                pdeExisting.AmountPaid = detail.AmountPaid;
                                pdeExisting.SupplierPayment = detail.SupplierPayment;

                                EntityExtension.FlagForUpdate(pdeExisting, username, USER_AGENT);

                                dbContext.PurchasingDocumentExpeditions.Update(pdeExisting);
                            }

                            //    PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                            //    {
                            //        Id = (int)detail.UnitPaymentOrderId,
                            //        IsPaid = paidFlag,
                            //        BankExpenditureNoteNo = model.DocumentNo,
                            //        BankExpenditureNoteDate = model.Date
                            //    };

                            //    EntityExtension.FlagForUpdate(pde, username, USER_AGENT);
                            //    dbContext.Entry(pde).Property(x => x.IsPaid).IsModified = true;
                            //    dbContext.Entry(pde).Property(x => x.BankExpenditureNoteNo).IsModified = true;
                            //    dbContext.Entry(pde).Property(x => x.BankExpenditureNoteDate).IsModified = true;
                            //    dbContext.Entry(pde).Property(x => x.LastModifiedAgent).IsModified = true;
                            //    dbContext.Entry(pde).Property(x => x.LastModifiedBy).IsModified = true;
                            //    dbContext.Entry(pde).Property(x => x.LastModifiedUtc).IsModified = true;
                        }
                    }

                    foreach (var detail in dbContext.BankExpenditureNoteDetails.AsNoTracking().Where(p => p.BankExpenditureNoteId == model.Id))
                    {
                        BankExpenditureNoteDetailModel detailModel = model.Details.FirstOrDefault(prop => prop.Id.Equals(detail.Id));

                        if (detailModel == null)
                        {
                            EntityExtension.FlagForDelete(detail, username, USER_AGENT);

                            foreach (var item in dbContext.BankExpenditureNoteItems.AsNoTracking().Where(p => p.BankExpenditureNoteDetailId == detail.Id))
                            {
                                EntityExtension.FlagForDelete(item, username, USER_AGENT);
                                dbContext.BankExpenditureNoteItems.Update(item);
                            }

                            dbContext.BankExpenditureNoteDetails.Update(detail);

                            var pdeExisting = dbContext.PurchasingDocumentExpeditions.FirstOrDefault(entity => entity.BankExpenditureNoteNo == model.DocumentNo && entity.UnitPaymentOrderNo == detail.UnitPaymentOrderNo);
                            if (pdeExisting != null)
                            {
                                pdeExisting.IsPaid = false;
                                pdeExisting.BankExpenditureNoteNo = null;
                                pdeExisting.BankExpenditureNoteDate = null;
                                pdeExisting.AmountPaid = 0;
                                pdeExisting.SupplierPayment = 0;

                                EntityExtension.FlagForUpdate(pdeExisting, username, USER_AGENT);

                                dbContext.PurchasingDocumentExpeditions.Update(pdeExisting);
                            }

                            //PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                            //{
                            //    Id = (int)detail.UnitPaymentOrderId,
                            //    IsPaid = false,
                            //    BankExpenditureNoteNo = null,
                            //    BankExpenditureNoteDate = null,
                            //    SupplierPayment = 0,
                            //    AmountPaid = 0
                            //};

                            //EntityExtension.FlagForUpdate(pde, username, USER_AGENT);
                            ////dbContext.Attach(pde);
                            //dbContext.Entry(pde).Property(x => x.IsPaid).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteNo).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteDate).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.AmountPaid).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.SupplierPayment).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedAgent).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedBy).IsModified = true;
                            //dbContext.Entry(pde).Property(x => x.LastModifiedUtc).IsModified = true;
                        }
                    }

                    Updated = await dbContext.SaveChangesAsync();
                    //DeleteDailyBankTransaction(model.DocumentNo, identityService);
                    //CreateDailyBankTransaction(model, identityService);
                    //UpdateCreditorAccount(model, identityService);
                    //ReverseJournalTransaction(model);
                    //CreateJournalTransaction(model, identityService);
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

        public async Task<int> Create(BankExpenditureNoteModel model, IdentityService identityService)
        {
            int Created = 0;
            string username = identityService.Username;
            TimeSpan timeOffset = new TimeSpan(identityService.TimezoneOffset, 0, 0);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

                    model.DocumentNo = await bankDocumentNumberGenerator.GenerateDocumentNumber("K", model.BankCode, username, model.Date.ToOffset(timeOffset).Date);

                    if (model.BankCurrencyCode != "IDR")
                    {
                        var BICurrency = await GetBICurrency(model.CurrencyCode, model.Date);
                        model.CurrencyRate = BICurrency.Rate.GetValueOrDefault();
                    }

                    foreach (var detail in model.Details)
                    {
                        var paidFlag = true;

                        var query = dbContext.BankExpenditureNoteDetails.Where(x => x.UnitPaymentOrderNo == detail.UnitPaymentOrderNo).ToList();

                        if (query.Count == 0)
                        {
                            if (detail.SupplierPayment != detail.TotalPaid)
                            {
                                paidFlag = false;
                            }

                            detail.UPOIndex = 1;
                        }
                        else
                        {
                            if ((query.Sum(x => x.SupplierPayment) + detail.SupplierPayment) != detail.TotalPaid)
                            {
                                paidFlag = false;
                            }

                            detail.UPOIndex = query.Last().UPOIndex + 1;
                        }

                        EntityExtension.FlagForCreate(detail, username, USER_AGENT);

                        var pdeSPB = dbContext.PurchasingDocumentExpeditions.Include(item => item.Items).LastOrDefault(entity => entity.UnitPaymentOrderNo == detail.UnitPaymentOrderNo);
                        
                        if (pdeSPB != null && string.IsNullOrWhiteSpace(pdeSPB.BankExpenditureNoteNo))
                        {
                            // update pde
                            pdeSPB.IsPaid = paidFlag;
                            pdeSPB.BankExpenditureNoteNo = model.DocumentNo;
                            pdeSPB.BankExpenditureNoteDate = model.Date;
                            pdeSPB.AmountPaid = detail.AmountPaid;
                            pdeSPB.SupplierPayment = detail.SupplierPayment;

                            EntityExtension.FlagForUpdate(pdeSPB, username, USER_AGENT);

                            dbContext.PurchasingDocumentExpeditions.Update(pdeSPB);
                        }
                        else
                        {
                            // copy pdeSPB -> seusaikan dengan pembayaran bank baru

                            pdeSPB.IsPaid = true;

                            EntityExtension.FlagForUpdate(pdeSPB, username, USER_AGENT);

                            dbContext.PurchasingDocumentExpeditions.Update(pdeSPB);

                            PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                            {
                                AccountingDivisionBy = pdeSPB.AccountingDivisionBy,
                                AccountingDivisionDate = pdeSPB.AccountingDivisionDate,
                                Active = pdeSPB.Active,
                                AmountPaid = detail.AmountPaid,
                                CreatedAgent = pdeSPB.CreatedAgent,
                                SendToAccountingDivisionBy = pdeSPB.SendToAccountingDivisionBy,
                                SendToAccountingDivisionDate = pdeSPB.SendToAccountingDivisionDate,
                                BankExpenditureNoteDate = model.Date,
                                BankExpenditureNoteNo = model.DocumentNo,
                                BankExpenditureNotePPHDate = pdeSPB.BankExpenditureNotePPHDate,
                                BankExpenditureNotePPHNo = pdeSPB.BankExpenditureNotePPHNo,
                                CashierDivisionBy = pdeSPB.CashierDivisionBy,
                                CashierDivisionDate = pdeSPB.CashierDivisionDate,
                                CategoryCode = pdeSPB.CategoryCode,
                                CategoryName = pdeSPB.CategoryName,
                                CreatedBy = pdeSPB.CreatedBy,
                                CreatedUtc = model.CreatedUtc,
                                Currency = pdeSPB.Currency,
                                DivisionCode = pdeSPB.DivisionCode,
                                DivisionName = pdeSPB.DivisionName,
                                DueDate = pdeSPB.DueDate,
                                IncomeTax = pdeSPB.IncomeTax,
                                IncomeTaxBy = pdeSPB.IncomeTaxBy,
                                IncomeTaxId = pdeSPB.IncomeTaxId,
                                IncomeTaxName = pdeSPB.IncomeTaxName,
                                IncomeTaxRate = pdeSPB.IncomeTaxRate,
                                InvoiceNo = pdeSPB.InvoiceNo,
                                IsDeleted = pdeSPB.IsDeleted,
                                IsPaid = paidFlag,
                                IsPaidPPH = pdeSPB.IsPaidPPH,
                                NotVerifiedReason = pdeSPB.NotVerifiedReason,
                                PaymentMethod = pdeSPB.PaymentMethod,
                                Position = pdeSPB.Position,
                                SendToCashierDivisionBy = pdeSPB.SendToCashierDivisionBy,
                                SendToCashierDivisionDate = pdeSPB.SendToCashierDivisionDate,
                                SendToPurchasingDivisionBy = pdeSPB.SendToPurchasingDivisionBy,
                                SendToPurchasingDivisionDate = pdeSPB.SendToPurchasingDivisionDate,
                                SendToVerificationDivisionBy = pdeSPB.SendToVerificationDivisionBy,
                                SendToVerificationDivisionDate = pdeSPB.SendToVerificationDivisionDate,
                                SupplierCode = pdeSPB.SupplierCode,
                                SupplierName = pdeSPB.SupplierName,
                                SupplierPayment = detail.SupplierPayment,
                                TotalPaid = pdeSPB.TotalPaid,
                                UnitPaymentOrderNo = pdeSPB.UnitPaymentOrderNo,
                                UPODate = pdeSPB.UPODate,
                                URNId = pdeSPB.URNId,
                                URNNo = pdeSPB.URNNo,
                                Vat = pdeSPB.Vat,
                                VerificationDivisionBy = pdeSPB.VerificationDivisionBy,
                                VerificationDivisionDate = pdeSPB.VerificationDivisionDate,
                                VerifyDate = pdeSPB.VerifyDate
                            };

                            List<PurchasingDocumentExpeditionItem> pdeItems = new List<PurchasingDocumentExpeditionItem>();

                            foreach (var item in pdeSPB.Items)
                            {
                                PurchasingDocumentExpeditionItem expeditionItem = new PurchasingDocumentExpeditionItem
                                {
                                    Active = item.Active,
                                    CreatedAgent = item.CreatedAgent,
                                    CreatedBy = item.CreatedBy,
                                    CreatedUtc = item.CreatedUtc,
                                    DeletedAgent = item.DeletedAgent,
                                    DeletedBy = item.DeletedBy,
                                    DeletedUtc = item.DeletedUtc,
                                    IsDeleted = item.IsDeleted,
                                    LastModifiedAgent = item.LastModifiedAgent,
                                    LastModifiedBy = item.LastModifiedBy,
                                    LastModifiedUtc = item.LastModifiedUtc,
                                    Price = item.Price,
                                    ProductCode = item.ProductCode,
                                    ProductId = item.ProductId,
                                    ProductName = item.ProductName,
                                    PurchasingDocumentExpedition = pde,
                                    Quantity = item.Quantity,
                                    UnitCode = item.UnitCode,
                                    UnitId = item.UnitId,
                                    UnitName = item.UnitName,
                                    Uom = item.Uom,
                                    URNId = item.URNId,
                                    URNNo = item.URNNo
                                };

                                pdeItems.Add(expeditionItem);
                            }

                            pde.Items = pdeItems;

                            EntityExtension.FlagForCreate(pde, username, USER_AGENT);

                            dbContext.PurchasingDocumentExpeditions.Add(pde);


                        }

                        //PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                        //{
                        //    Id = (int)pdeId,
                        //    IsPaid = paidFlag,
                        //    BankExpenditureNoteNo = model.DocumentNo,
                        //    BankExpenditureNoteDate = model.Date,
                        //    AmountPaid = detail.AmountPaid,
                        //    SupplierPayment = detail.SupplierPayment
                        //};

                        //EntityExtension.FlagForUpdate(pde, username, USER_AGENT);
                        //dbContext.Attach(pde);
                        //dbContext.Entry(pde).Property(x => x.IsPaid).IsModified = true;
                        //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteNo).IsModified = true;
                        //dbContext.Entry(pde).Property(x => x.BankExpenditureNoteDate).IsModified = true;
                        //dbContext.Entry(pde).Property(x => x.LastModifiedAgent).IsModified = true;
                        //dbContext.Entry(pde).Property(x => x.LastModifiedBy).IsModified = true;
                        //dbContext.Entry(pde).Property(x => x.LastModifiedUtc).IsModified = true;

                        foreach (var item in detail.Items.OrderBy(x => x.URNNo))
                        {
                            EntityExtension.FlagForCreate(item, username, USER_AGENT);
                        }
                    }

                    dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    //await CreateJournalTransaction(model, identityService);
                    //CreateDailyBankTransaction(model, identityService);
                    //CreateCreditorAccount(model, identityService);
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

        private async Task CreateJournalTransaction(BankExpenditureNoteModel model, IdentityService identityService)
        {
            var upoNos = model.Details.Select(detail => detail.UnitPaymentOrderNo).ToList();
            var unitPaymentOrders = dbContext.UnitPaymentOrders.Where(unitPaymentOrder => upoNos.Contains(unitPaymentOrder.UPONo)).ToList();
            var currency = await _currencyProvider.GetCurrencyByCurrencyCodeDate(model.CurrencyCode, model.Date);
            if (currency == null)
            {
                currency = new Currency() { Rate = model.CurrencyRate };
            }
            double totalPayment = 0;
            //var currency = await GetBICurrency(model.CurrencyCode, model.Date);

            //if (currency == null)
            //{
            //    currency = new GarmentCurrency() { Rate = model.CurrencyRate };
            //}

            var items = new List<JournalTransactionItem>();
            foreach (var detail in model.Details)
            {
                var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(element => element.UPONo == detail.UnitPaymentOrderNo);
                var currencyRate = currency.Rate;
                if (unitPaymentOrder == null)
                    unitPaymentOrder = new UnitPaymentOrder();
                var unitSummaries = detail.Items.GroupBy(g => new { g.URNNo, g.UnitCode }).Select(s => new
                {
                    UnitCode = s.Key.UnitCode,
                    URNNo = s.Key.URNNo,
                    //Total = s.Sum(sm => sm.Price * currency.Rate)
                    //Total = s.Sum(sm => sm.Price)
                });

                if (unitSummaries.Count() > 1)
                {
                    var nominal = (decimal)0;
                    var Remaining = detail.SupplierPayment;
                    foreach (var unitSummary in detail.Items)
                    {
                        var paidPrice = itemDbSet.Where(bank => bank.URNNo == unitSummary.URNNo && bank.Price == unitSummary.Price).Sum(x => x.PaidPrice);
                        if (unitSummary.Price <= paidPrice)
                        {
                            continue;
                        }
                        else
                        {
                            var vatAmount = unitPaymentOrder.UseVat ? unitSummary.Price * 0.1 : 0;
                            var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? unitSummary.Price * unitPaymentOrder.IncomeTaxRate / 100 : 0;
                            var dpp = unitSummary.Price + vatAmount - incomeTaxAmount;

                            if (Remaining >= dpp)
                            {
                                if (paidPrice != 0)
                                {
                                    dpp -= paidPrice;
                                }
                            }
                            else
                            {
                                if (Remaining <= 0)
                                {
                                    continue;
                                }

                                dpp = Remaining;
                            }
                            //var vatAmount = unitPaymentOrder.UseVat ? dpp * 0.1 : 0;
                            //var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? dpp * unitPaymentOrder.IncomeTaxRate / 100 : 0;

                            //var debit = dpp + vatAmount - incomeTaxAmount;
                            Remaining -= dpp;

                            var debit = dpp;
                            if (model.CurrencyCode != "IDR")
                            {
                                //debit = (dpp + vatAmount - incomeTaxAmount) * model.CurrencyRate
                                debit = (dpp) * currencyRate.GetValueOrDefault();
                            }
                            nominal = decimal.Add(nominal, Convert.ToDecimal(debit));

                            var item = new JournalTransactionItem()
                            {
                                COA = new COA()
                                {
                                    Code = COAGenerator.GetDebtCOA(model.SupplierImport, detail.DivisionName, unitSummary.UnitCode)
                                },
                                Debit = Convert.ToDecimal(debit),
                                Remark = detail.UnitPaymentOrderNo + " / " + detail.InvoiceNo
                            };

                            items.Add(item);
                            totalPayment += dpp;
                        }
                    }
                }
                else
                {
                    var nominal = (decimal)0;
                    foreach (var unitSummary in unitSummaries)
                    {
                        var dpp = detail.SupplierPayment;
                        //var vatAmount = unitPaymentOrder.UseVat ? dpp * 0.1 : 0;
                        //var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? dpp * unitPaymentOrder.IncomeTaxRate / 100 : 0;

                        //var debit = dpp + vatAmount - incomeTaxAmount;
                        var debit = dpp;
                        if (model.CurrencyCode != "IDR")
                        {
                            //debit = (dpp + vatAmount - incomeTaxAmount) * model.CurrencyRate;
                            debit = (dpp) * currencyRate.GetValueOrDefault();
                        }
                        nominal = decimal.Add(nominal, Convert.ToDecimal(debit));

                        var item = new JournalTransactionItem()
                        {
                            COA = new COA()
                            {
                                Code = COAGenerator.GetDebtCOA(model.SupplierImport, detail.DivisionName, unitSummary.UnitCode)
                            },
                            Debit = Convert.ToDecimal(debit),
                            Remark = detail.UnitPaymentOrderNo + " / " + detail.InvoiceNo
                        };

                        items.Add(item);
                        totalPayment += dpp;
                    }
                }
            }

            //items = items.GroupBy(g => g.COA.Code).Select(s => new JournalTransactionItem()
            //{
            //    COA = s.First().COA,
            //    Debit = s.Sum(sm => Math.Round(sm.Debit.GetValueOrDefault(), 4))
            //}).ToList();

            var credit = totalPayment * model.CurrencyRate;
            var creditRound = Math.Round(Convert.ToDecimal(credit), 2);
            var debitRound = Math.Round(items.Sum(s => s.Debit.GetValueOrDefault()), 2);

            if (creditRound != debitRound)
            {
                if (creditRound > debitRound)
                {
                    var differenceRate = (decimal)credit - items.Sum(s => s.Debit.GetValueOrDefault());

                    var differentJournalItem = new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = "7131.00.0.00",
                        },
                        Debit = differenceRate,
                        Remark = "Pelunasan Hutang"
                    };
                    items.Add(differentJournalItem);
                }
                else
                {
                    var differenceRate = items.Sum(s => s.Debit.GetValueOrDefault()) - (decimal)credit;

                    var differentJournalItem = new JournalTransactionItem()
                    {
                        COA = new COA()
                        {
                            Code = "7131.00.0.00",
                        },
                        Credit = differenceRate,
                        Remark = "Pelunasan Hutang"
                    };
                    items.Add(differentJournalItem);
                }
                
                var bankJournalItem = new JournalTransactionItem()
                {
                    COA = new COA()
                    {
                        Code = model.BankAccountCOA
                    },
                    Credit = (decimal)credit,
                    Remark = "Bayar Hutang " + model.SupplierName
                };
                items.Add(bankJournalItem);
            }
            else
            {
                var bankJournalItem = new JournalTransactionItem()
                {
                    COA = new COA()
                    {
                        Code = model.BankAccountCOA
                    },
                    //Credit = items.Sum(s => Math.Round(s.Debit.GetValueOrDefault(), 4))
                    Credit = items.Sum(s => s.Debit.GetValueOrDefault()),
                    Remark = "Bayar Hutang " + model.SupplierName
                };
                items.Add(bankJournalItem);
            }
            
            var modelToPost = new JournalTransaction()
            {
                Date = model.Date,
                Description = "Bukti Pengeluaran Bank",
                ReferenceNo = model.DocumentNo,
                Remark = "",
                Items = items
            };

            string journalTransactionUri = "journal-transactions";
            //var httpClient = new HttpClientService(identityService);
            var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = await httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(modelToPost).ToString(), Encoding.UTF8, General.JsonMediaType));
            response.EnsureSuccessStatusCode();
        }

        //private async Task<GarmentCurrency> GetGarmentCurrency(string codeCurrency)
        //{
        //    string date = DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        //    string queryString = $"code={codeCurrency}&stringDate={date}";

        //    var http = serviceProvider.GetService<IHttpClientService>();
        //    var response = await http.GetAsync(APIEndpoint.Core + $"master/garment-currencies/single-by-code-date?{queryString}");

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

        //    var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

        //    return result.data;
        //}

        private async Task<GarmentCurrency> GetBICurrency(string codeCurrency, DateTimeOffset date)
        {
            string stringDate = date.ToString("yyyy/MM/dd HH:mm:ss");
            string queryString = $"code={codeCurrency}&stringDate={stringDate}";

            var http = serviceProvider.GetService<IHttpClientService>();
            var response = await http.GetAsync(APIEndpoint.Core + $"master/bi-currencies/single-by-code-date?{queryString}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

            return result.data;
        }

        //private void ReverseJournalTransaction(BankExpenditureNoteModel model)
        //{
        //    foreach (var detail in model.Details)
        //    {
        //        //string journalTransactionUri = $"journal-transactions/reverse-transactions/{model.DocumentNo + "/" + detail.UnitPaymentOrderNo}";
        //        string journalTransactionUri = $"journal-transactions/reverse-transactions/{model.DocumentNo}";
        //        var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
        //        var response = httpClient.PostAsync($"{APIEndpoint.Finance}{journalTransactionUri}", new StringContent(JsonConvert.SerializeObject(new object()).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
        //        response.EnsureSuccessStatusCode();
        //    }
        //}

        public async Task<int> Delete(int Id, IdentityService identityService)
        {
            int Count = 0;
            string username = identityService.Username;

            if (dbSet.Count(p => p.Id == Id && p.IsDeleted == false).Equals(0))
            {
                return 0;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    BankExpenditureNoteModel bankExpenditureNote = dbContext.BankExpenditureNotes.Include(entity => entity.Details).Single(p => p.Id == Id);

                    ICollection<BankExpenditureNoteDetailModel> Details = new List<BankExpenditureNoteDetailModel>(dbContext.BankExpenditureNoteDetails.Where(p => p.BankExpenditureNoteId.Equals(Id)));

                    foreach (var detail in Details)
                    {
                        ICollection<BankExpenditureNoteItemModel> Items = new List<BankExpenditureNoteItemModel>(dbContext.BankExpenditureNoteItems.Where(p => p.BankExpenditureNoteDetailId.Equals(detail.Id)));

                        foreach (var item in Items)
                        {
                            EntityExtension.FlagForDelete(item, username, USER_AGENT);
                            dbContext.BankExpenditureNoteItems.Update(item);
                        }

                        EntityExtension.FlagForDelete(detail, username, USER_AGENT);
                        dbContext.BankExpenditureNoteDetails.Update(detail);

                        PurchasingDocumentExpedition pde = new PurchasingDocumentExpedition
                        {
                            Id = (int)detail.UnitPaymentOrderId,
                            IsPaid = false,
                            BankExpenditureNoteNo = null,
                            BankExpenditureNoteDate = null
                        };

                        EntityExtension.FlagForUpdate(pde, username, USER_AGENT);
                        //dbContext.Attach(pde);
                        dbContext.Entry(pde).Property(x => x.IsPaid).IsModified = true;
                        dbContext.Entry(pde).Property(x => x.BankExpenditureNoteNo).IsModified = true;
                        dbContext.Entry(pde).Property(x => x.BankExpenditureNoteDate).IsModified = true;
                        dbContext.Entry(pde).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(pde).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(pde).Property(x => x.LastModifiedUtc).IsModified = true;
                    }

                    EntityExtension.FlagForDelete(bankExpenditureNote, username, USER_AGENT);
                    dbSet.Update(bankExpenditureNote);
                    Count = await dbContext.SaveChangesAsync();
                    //DeleteDailyBankTransaction(bankExpenditureNote.DocumentNo, identityService);
                    //DeleteCreditorAccount(bankExpenditureNote, identityService);
                    //ReverseJournalTransaction(bankExpenditureNote);
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Count;
        }

        public ReadResponse<object> GetAllByPosition(int Page, int Size, string Order, string Keyword, string Filter)
        {
            var query = dbContext.PurchasingDocumentExpeditions.AsQueryable();


            query = query.Include(i => i.Items);

            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                List<string> searchAttributes = new List<string>()
                {
                    "UnitPaymentOrderNo", "SupplierName", "DivisionName", "SupplierCode", "InvoiceNo"
                };

                query = QueryHelper<PurchasingDocumentExpedition>.ConfigureSearch(query, searchAttributes, Keyword);
            }

            if (Filter.Contains("verificationFilter"))
            {
                Filter = "{}";
                List<ExpeditionPosition> positions = new List<ExpeditionPosition> { ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, ExpeditionPosition.SEND_TO_CASHIER_DIVISION };
                query = query.Where(p => positions.Contains(p.Position));
            }

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            query = QueryHelper<PurchasingDocumentExpedition>.ConfigureFilter(query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            query = QueryHelper<PurchasingDocumentExpedition>.ConfigureOrder(query, OrderDictionary);

            Pageable<PurchasingDocumentExpedition> pageable = new Pageable<PurchasingDocumentExpedition>(query, Page - 1, Size);
            List<PurchasingDocumentExpedition> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> list = new List<object>();
            list.AddRange(Data.Select(s => new
            {
                UnitPaymentOrderId = s.Id,
                s.UnitPaymentOrderNo,
                s.UPODate,
                s.DueDate,
                s.InvoiceNo,
                s.SupplierCode,
                s.SupplierName,
                s.CategoryCode,
                s.CategoryName,
                s.DivisionCode,
                s.DivisionName,
                s.Vat,
                s.IncomeTax,
                s.IncomeTaxBy,
                s.IsPaid,
                s.TotalPaid,
                s.Currency,
                s.PaymentMethod,
                s.URNId,
                s.URNNo,
                AmountPaid = (detailDbSet.Where(x => x.UnitPaymentOrderNo == s.UnitPaymentOrderNo).ToList().Count == 0 ? 0 : detailDbSet.Where(x => x.UnitPaymentOrderNo == s.UnitPaymentOrderNo).Sum(x => x.SupplierPayment)),
                IsPosted = dbSet.Where(p => p.DocumentNo == s.BankExpenditureNoteNo).LastOrDefault() != null ? dbSet.Where(p => p.DocumentNo == s.BankExpenditureNoteNo).LastOrDefault().IsPosted : true,
                Items = s.Items.Select(sl => new
                {
                    UnitPaymentOrderItemId = sl.Id,
                    sl.UnitId,
                    sl.UnitCode,
                    sl.UnitName,
                    sl.ProductId,
                    sl.ProductCode,
                    sl.ProductName,
                    sl.Quantity,
                    sl.Uom,
                    sl.Price,
                    sl.URNId,
                    sl.URNNo
                }).ToList()
            }));

            return new ReadResponse<object>(list, TotalData, OrderDictionary);
        }

        public ReadResponse<object> GetReport(int Size, int Page, string DocumentNo, string UnitPaymentOrderNo, string InvoiceNo, string SupplierCode, string DivisionCode, string PaymentMethod, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Offset)
        {
            IQueryable<BankExpenditureNoteReportViewModel> Query;
            TimeSpan offset = new TimeSpan(7, 0, 0);

            DateFrom = DateFrom.HasValue ? DateFrom : DateTimeOffset.MinValue;
            DateTo = DateTo.HasValue ? DateTo : DateTimeOffset.Now;

            Query = (from a in dbContext.BankExpenditureNotes
                     join b in dbContext.BankExpenditureNoteDetails on a.Id equals b.BankExpenditureNoteId
                     //join c in dbContext.PurchasingDocumentExpeditions on new { BankExpenditureNoteNo = b.BankExpenditureNote.DocumentNo, b.UnitPaymentOrderNo } equals new { c.BankExpenditureNoteNo, c.UnitPaymentOrderNo }
                     join d in dbContext.UnitPaymentOrders on b.UnitPaymentOrderNo equals d.UPONo
                     //where c.InvoiceNo == (InvoiceNo ?? c.InvoiceNo)
                     //   && c.SupplierCode == (SupplierCode ?? c.SupplierCode)
                     //   && c.UnitPaymentOrderNo == (UnitPaymentOrderNo ?? c.UnitPaymentOrderNo)
                     //   && c.DivisionCode == (DivisionCode ?? c.DivisionCode)
                     //   && !c.PaymentMethod.ToUpper().Equals("CASH")
                     //   && c.IsPaid
                     //   && c.PaymentMethod == (PaymentMethod ?? c.PaymentMethod)
                     where a.IsPosted
                     orderby a.DocumentNo, b.Id
                     select new BankExpenditureNoteReportViewModel
                     {
                         Id = a.Id,
                         DocumentNo = a.DocumentNo,
                         Currency = a.BankCurrencyCode,
                         Date = a.Date,
                         SupplierCode = b.SupplierCode,
                         SupplierName = b.SupplierName,
                         CategoryName = b.CategoryName == null ? "-" : b.CategoryName,
                         DivisionName = b.DivisionName,
                         PaymentMethod = d.PaymentMethod,
                         UnitPaymentOrderNo = b.UnitPaymentOrderNo,
                         BankName = string.Concat(a.BankAccountName, " - ", a.BankName, " - ", a.BankAccountNumber, " - ", a.BankCurrencyCode),
                         DPP = b.TotalPaid - b.Vat,
                         VAT = b.Vat,
                         TotalPaid = b.SupplierPayment,
                         InvoiceNumber = b.InvoiceNo,
                         DivisionCode = b.DivisionCode,
                         TotalDPP = b.TotalPaid - b.Vat,
                         TotalPPN = b.Vat,
                         DifferenceNominal = (b.TotalPaid) - (b.AmountPaid + b.SupplierPayment)
                     });

            //if (DateFrom == null || DateTo == null)
            //{
            //    Query = (from a in dbContext.BankExpenditureNotes
            //             join b in dbContext.BankExpenditureNoteDetails on a.Id equals b.BankExpenditureNoteId
            //             join c in dbContext.PurchasingDocumentExpeditions on new { BankExpenditureNoteNo = b.BankExpenditureNote.DocumentNo, b.UnitPaymentOrderNo } equals new { c.BankExpenditureNoteNo, c.UnitPaymentOrderNo }
            //             //where c.InvoiceNo == (InvoiceNo ?? c.InvoiceNo)
            //             //   && c.SupplierCode == (SupplierCode ?? c.SupplierCode)
            //             //   && c.UnitPaymentOrderNo == (UnitPaymentOrderNo ?? c.UnitPaymentOrderNo)
            //             //   && c.DivisionCode == (DivisionCode ?? c.DivisionCode)
            //             //   && !c.PaymentMethod.ToUpper().Equals("CASH")
            //             //   && c.IsPaid
            //             //   && c.PaymentMethod == (PaymentMethod ?? c.PaymentMethod)
            //             where a.IsPosted
            //             orderby a.DocumentNo
            //             select new BankExpenditureNoteReportViewModel
            //             {
            //                 Id = a.Id,
            //                 DocumentNo = a.DocumentNo,
            //                 Currency = a.BankCurrencyCode,
            //                 Date = a.Date,
            //                 SupplierCode = c.SupplierCode,
            //                 SupplierName = c.SupplierName,
            //                 CategoryName = c.CategoryName == null ? "-" : c.CategoryName,
            //                 DivisionName = c.DivisionName,
            //                 PaymentMethod = c.PaymentMethod,
            //                 UnitPaymentOrderNo = b.UnitPaymentOrderNo,
            //                 BankName = string.Concat(a.BankAccountName, " - ", a.BankName, " - ", a.BankAccountNumber, " - ", a.BankCurrencyCode),
            //                 DPP = c.TotalPaid - c.Vat,
            //                 VAT = c.Vat,
            //                 TotalPaid = c.TotalPaid,
            //                 InvoiceNumber = c.InvoiceNo,
            //                 DivisionCode = c.DivisionCode,
            //                 TotalDPP = c.TotalPaid - c.Vat,
            //                 TotalPPN = c.Vat
            //             }
            //          );
            //}
            //else
            //{
            //    Query = (from a in dbContext.BankExpenditureNotes
            //             join b in dbContext.BankExpenditureNoteDetails on a.Id equals b.BankExpenditureNoteId
            //             join c in dbContext.PurchasingDocumentExpeditions on new { BankExpenditureNoteNo = b.BankExpenditureNote.DocumentNo, b.UnitPaymentOrderNo } equals new { c.BankExpenditureNoteNo, c.UnitPaymentOrderNo }
            //             //where c.InvoiceNo == (InvoiceNo ?? c.InvoiceNo)
            //             //   && c.SupplierCode == (SupplierCode ?? c.SupplierCode)
            //             //   && c.UnitPaymentOrderNo == (UnitPaymentOrderNo ?? c.UnitPaymentOrderNo)
            //             //   && c.DivisionCode == (DivisionCode ?? c.DivisionCode)
            //             //   && !c.PaymentMethod.ToUpper().Equals("CASH")
            //             //   && c.IsPaid
            //             //   && c.PaymentMethod == (PaymentMethod ?? c.PaymentMethod)
            //             where a.IsPosted
            //             orderby a.DocumentNo
            //             select new BankExpenditureNoteReportViewModel
            //             {
            //                 Id = a.Id,
            //                 DocumentNo = a.DocumentNo,
            //                 Currency = a.BankCurrencyCode,
            //                 Date = a.Date,
            //                 SupplierCode = c.SupplierCode,
            //                 SupplierName = c.SupplierName,
            //                 CategoryName = c.CategoryName == null ? "-" : c.CategoryName,
            //                 DivisionName = c.DivisionName,
            //                 PaymentMethod = c.PaymentMethod,
            //                 UnitPaymentOrderNo = b.UnitPaymentOrderNo,
            //                 BankName = string.Concat(a.BankAccountName, " - ", a.BankName, " - ", a.BankAccountNumber, " - ", a.BankCurrencyCode),
            //                 DPP = c.TotalPaid - c.Vat,
            //                 VAT = c.Vat,
            //                 TotalPaid = c.TotalPaid,
            //                 InvoiceNumber = c.InvoiceNo,
            //                 DivisionCode = c.DivisionCode,
            //                 TotalDPP = c.TotalPaid - c.Vat,
            //                 TotalPPN = c.Vat,
            //             }
            //          );
            //}

            Query = Query.Where(entity => entity.Date.AddHours(Offset) >= DateFrom.GetValueOrDefault() && entity.Date.AddHours(Offset) <= DateTo.GetValueOrDefault().AddDays(1).AddSeconds(-1));
            // override duplicate 
            Query = Query.GroupBy(
                key => new { key.Id, key.BankName, key.CategoryName, key.Currency, key.Date, key.DivisionCode, key.DivisionName, key.DocumentNo, key.DPP, key.InvoiceNumber, key.PaymentMethod, key.SupplierCode, key.SupplierName, key.TotalDPP, key.TotalPaid, key.TotalPPN, key.VAT, key.UnitPaymentOrderNo, key.DifferenceNominal },
                value => value,
                (key, value) => new BankExpenditureNoteReportViewModel
                {
                    Id = key.Id,
                    DocumentNo = key.DocumentNo,
                    Currency = key.Currency,
                    Date = key.Date,
                    SupplierCode = key.SupplierCode,
                    SupplierName = key.SupplierName,
                    CategoryName = key.CategoryName == null ? "-" : key.CategoryName,
                    DivisionName = key.DivisionName,
                    PaymentMethod = key.PaymentMethod,
                    UnitPaymentOrderNo = key.UnitPaymentOrderNo,
                    BankName = key.BankName,
                    DPP = key.DPP,
                    VAT = key.VAT,
                    TotalPaid = key.TotalPaid,
                    InvoiceNumber = key.InvoiceNumber,
                    DivisionCode = key.DivisionCode,
                    TotalDPP = key.TotalDPP,
                    TotalPPN = key.TotalPPN,
                    DifferenceNominal = key.DifferenceNominal
                });
            if (!string.IsNullOrWhiteSpace(DocumentNo))
                Query = Query.Where(entity => entity.DocumentNo == DocumentNo);

            if (!string.IsNullOrWhiteSpace(UnitPaymentOrderNo))
                Query = Query.Where(entity => entity.UnitPaymentOrderNo == UnitPaymentOrderNo);

            if (!string.IsNullOrWhiteSpace(InvoiceNo))
                Query = Query.Where(entity => entity.InvoiceNumber == InvoiceNo);

            if (!string.IsNullOrWhiteSpace(SupplierCode))
                Query = Query.Where(entity => entity.SupplierCode == SupplierCode);

            if (!string.IsNullOrWhiteSpace(PaymentMethod))
                Query = Query.Where(entity => entity.PaymentMethod == PaymentMethod);

            if (!string.IsNullOrWhiteSpace(DivisionCode))
                Query = Query.Where(entity => entity.DivisionCode == DivisionCode);

            Pageable<BankExpenditureNoteReportViewModel> pageable = new Pageable<BankExpenditureNoteReportViewModel>(Query, Page - 1, Size);
            List<object> data = pageable.Data.ToList<object>();

            return new ReadResponse<object>(data, pageable.TotalCount, new Dictionary<string, string>());
        }

        public async Task CreateDailyBankTransaction(BankExpenditureNoteModel model, IdentityService identityService)
        {
            var nominal = model.GrandTotal;
            var nominalValas = 0.0;

            if (model.CurrencyCode != "IDR")
            {
                nominalValas = model.GrandTotal;
                nominal = model.GrandTotal * model.CurrencyRate;
            }

            DailyBankTransactionViewModel modelToPost = new DailyBankTransactionViewModel()
            {
                Bank = new ViewModels.NewIntegrationViewModel.AccountBankViewModel()
                {
                    Id = model.BankId,
                    Code = model.BankCode,
                    AccountName = model.BankAccountName,
                    AccountNumber = model.BankAccountNumber,
                    BankCode = model.BankCode,
                    BankName = model.BankName,
                    Currency = new ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                    {
                        Code = model.BankCurrencyCode,
                        Id = model.BankCurrencyId,
                    }
                },
                Date = model.Date,
                Nominal = nominal,
                CurrencyRate = model.CurrencyRate,
                ReferenceNo = model.DocumentNo,
                ReferenceType = "Bayar Hutang",
                Remark = model.CurrencyCode != "IDR" ? $"Pembayaran atas {model.BankCurrencyCode} dengan nominal {string.Format("{0:n}", model.GrandTotal)} dan kurs {model.CurrencyCode}" : "",
                SourceType = "Operasional",
                Status = "OUT",
                Supplier = new NewSupplierViewModel()
                {
                    _id = model.SupplierId,
                    code = model.SupplierCode,
                    name = model.SupplierName
                },
                IsPosted = true,
                NominalValas = nominalValas
            };

            //if (model.BankCurrencyCode != "IDR")
            //    modelToPost.NominalValas = model.GrandTotal * model.CurrencyRate;

            string dailyBankTransactionUri = "daily-bank-transactions";
            //var httpClient = new HttpClientService(identityService);
            var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.PostAsync($"{APIEndpoint.Finance}{dailyBankTransactionUri}", new StringContent(JsonConvert.SerializeObject(modelToPost).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        //public void DeleteDailyBankTransaction(string documentNo, IdentityService identityService)
        //{
        //    string dailyBankTransactionUri = "daily-bank-transactions/by-reference-no/";
        //    //var httpClient = new HttpClientService(identityService);
        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = httpClient.DeleteAsync($"{APIEndpoint.Finance}{dailyBankTransactionUri}{documentNo}").Result;
        //    response.EnsureSuccessStatusCode();
        //}

        //private void CreateCreditorAccount(BankExpenditureNoteModel model, IdentityService identityService)
        //{
        //    List<CreditorAccountViewModel> postedData = new List<CreditorAccountViewModel>();
        //    foreach (var item in model.Details)
        //    {
        //        CreditorAccountViewModel viewModel = new CreditorAccountViewModel()
        //        {
        //            Code = model.DocumentNo,
        //            Date = model.Date,
        //            Id = (int)model.Id,
        //            InvoiceNo = item.InvoiceNo,
        //            Mutation = model.CurrencyCode != "IDR" ? item.SupplierPayment * model.CurrencyRate : item.SupplierPayment,
        //            SupplierCode = model.SupplierCode,
        //            SupplierName = model.SupplierName,
        //            MemoNo = item.UnitPaymentOrderNo
        //        };
        //        postedData.Add(viewModel);
        //    }


        //    var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = httpClient.PostAsync($"{APIEndpoint.Finance}{CREDITOR_ACCOUNT_URI}", new StringContent(JsonConvert.SerializeObject(postedData).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
        //    response.EnsureSuccessStatusCode();
        //}

        private async Task CreateCreditorAccountv2(BankExpenditureNoteModel model, IdentityService identityService)
        {
            var upoNos = model.Details.Select(detail => detail.UnitPaymentOrderNo).ToList();
            var unitPaymentOrders = dbContext.UnitPaymentOrders.Where(unitPaymentOrder => upoNos.Contains(unitPaymentOrder.UPONo)).ToList();
            var currency = await GetBICurrency(model.CurrencyCode, model.Date);

            if (currency == null)
            {
                currency = new GarmentCurrency() { Rate = model.CurrencyRate };
            }

            List<CreditorAccountViewModel> postedData = new List<CreditorAccountViewModel>();
            foreach (var detail in model.Details)
            {
                var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(element => element.UPONo == detail.UnitPaymentOrderNo);

                if (unitPaymentOrder == null)
                    unitPaymentOrder = new UnitPaymentOrder();
                var unitSummaries = detail.Items.GroupBy(g => new { g.URNNo, g.UnitCode }).Select(s => new
                {
                    UnitCode = s.Key.UnitCode,
                    URNNo = s.Key.URNNo
                });

                if (unitSummaries.Count() > 1)
                {
                    var Remaining = detail.SupplierPayment;
                    foreach (var unitSummary in detail.Items)
                    {
                        var paidPrice = itemDbSet.Where(bank => bank.URNNo == unitSummary.URNNo && bank.Price == unitSummary.Price).Sum(x => x.PaidPrice);
                        if (unitSummary.Price <= paidPrice)
                        {
                            continue;
                        }
                        else
                        {
                            var vatAmount = unitPaymentOrder.UseVat ? unitSummary.Price * 0.1 : 0;
                            var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? unitSummary.Price * unitPaymentOrder.IncomeTaxRate / 100 : 0;
                            var dpp = unitSummary.Price + vatAmount - incomeTaxAmount;

                            if (Remaining >= dpp)
                            {
                                if (paidPrice != 0)
                                {
                                    dpp -= paidPrice;
                                }

                                var unitReceiptNotes = itemDbSet.Where(bank => bank.URNNo == unitSummary.URNNo && bank.Price == unitSummary.Price && bank.BankExpenditureNoteDetailId == unitSummary.BankExpenditureNoteDetailId).ToList();

                                foreach (var urnno in unitReceiptNotes)
                                {
                                    urnno.PaidPrice = dpp;

                                    EntityExtension.FlagForUpdate(unitSummary, identityService.Username, USER_AGENT);
                                    itemDbSet.Update(unitSummary);
                                }
                            }
                            else
                            {
                                if (Remaining <= 0)
                                {
                                    continue;
                                }

                                dpp = Remaining;

                                var unitReceiptNotes = itemDbSet.Where(bank => bank.URNNo == unitSummary.URNNo && bank.Price == unitSummary.Price && bank.BankExpenditureNoteDetailId == unitSummary.BankExpenditureNoteDetailId).ToList();

                                foreach (var urnno in unitReceiptNotes)
                                {
                                    urnno.PaidPrice = Remaining;

                                    EntityExtension.FlagForUpdate(unitSummary, identityService.Username, USER_AGENT);
                                    itemDbSet.Update(unitSummary);
                                }
                            }

                            Remaining -= dpp;

                            var mutation = dpp;
                            if (model.CurrencyCode != "IDR")
                            {
                                mutation = (dpp) * model.CurrencyRate;
                            }

                            CreditorAccountViewModel viewModel = new CreditorAccountViewModel()
                            {
                                Code = model.DocumentNo,
                                Date = model.Date,
                                Id = (int)model.Id,
                                InvoiceNo = detail.InvoiceNo,
                                Mutation = mutation,
                                SupplierCode = model.SupplierCode,
                                SupplierName = model.SupplierName,
                                MemoNo = detail.UnitPaymentOrderNo,
                                UnitReceiptNoteNo = unitSummary.URNNo
                            };

                            postedData.Add(viewModel);
                        }
                    }
                }
                else
                {
                    foreach (var unitSummary in unitSummaries)
                    {
                        var dpp = detail.SupplierPayment;
                        //var vatAmount = unitPaymentOrder.UseVat ? dpp * 0.1 : 0;
                        //var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? dpp * unitPaymentOrder.IncomeTaxRate / 100 : 0;

                        //var mutation = dpp + vatAmount - incomeTaxAmount;
                        var mutation = dpp;
                        if (model.CurrencyCode != "IDR")
                        {
                            //mutation = (dpp + vatAmount - incomeTaxAmount) * model.CurrencyRate;
                            mutation = (dpp) * model.CurrencyRate;
                        }

                        CreditorAccountViewModel viewModel = new CreditorAccountViewModel()
                        {
                            Code = model.DocumentNo,
                            Date = model.Date,
                            Id = (int)model.Id,
                            InvoiceNo = detail.InvoiceNo,
                            Mutation = mutation,
                            SupplierCode = model.SupplierCode,
                            SupplierName = model.SupplierName,
                            MemoNo = detail.UnitPaymentOrderNo,
                            UnitReceiptNoteNo = unitSummary.URNNo
                        };

                        postedData.Add(viewModel);
                    }
                }
            }

            var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.PostAsync($"{APIEndpoint.Finance}{CREDITOR_ACCOUNT_URI}", new StringContent(JsonConvert.SerializeObject(postedData).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            response.EnsureSuccessStatusCode();
        }

        //private void UpdateCreditorAccount(BankExpenditureNoteModel model, IdentityService identityService)
        //{
        //    List<CreditorAccountViewModel> postedData = new List<CreditorAccountViewModel>();
        //    foreach (var item in model.Details)
        //    {
        //        CreditorAccountViewModel viewModel = new CreditorAccountViewModel()
        //        {
        //            Code = model.DocumentNo,
        //            Date = model.Date,
        //            Id = (int)model.Id,
        //            InvoiceNo = item.InvoiceNo,
        //            Mutation = item.TotalPaid,
        //            SupplierCode = model.SupplierCode,
        //            SupplierName = model.SupplierName
        //        };
        //        postedData.Add(viewModel);
        //    }


        //    var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = httpClient.PutAsync($"{APIEndpoint.Finance}{CREDITOR_ACCOUNT_URI}", new StringContent(JsonConvert.SerializeObject(postedData).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
        //    response.EnsureSuccessStatusCode();

        //}

        //private void DeleteCreditorAccount(BankExpenditureNoteModel model, IdentityService identityService)
        //{
        //    var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
        //    var response = httpClient.DeleteAsync($"{APIEndpoint.Finance}{CREDITOR_ACCOUNT_URI}/{model.DocumentNo}").Result;
        //    response.EnsureSuccessStatusCode();

        //}

        public List<ExpenditureInfo> GetByPeriod(int month, int year, int timeoffset)
        {
            if (month == 0 && year == 0)
            {
                return dbSet.Select(s => new ExpenditureInfo() { DocumentNo = s.DocumentNo, BankName = s.BankName, BGCheckNumber = s.BGCheckNumber }).ToList();
            }
            else
            {
                return dbSet.Where(w => w.Date.AddHours(timeoffset).Month.Equals(month) && w.Date.AddHours(timeoffset).Year.Equals(year)).Select(s => new ExpenditureInfo() { DocumentNo = s.DocumentNo, BankName = s.BankName, BGCheckNumber = s.BGCheckNumber }).ToList();
            }

        }

        public async Task<string> Posting(List<long> ids)
        {
            var models = dbContext.BankExpenditureNotes.Include(entity => entity.Details).ThenInclude(detail => detail.Items).Where(entity => ids.Contains(entity.Id)).ToList();
            var identityService = serviceProvider.GetService<IdentityService>();
            var upoNos = models.SelectMany(model => model.Details).Select(detail => detail.UnitPaymentOrderNo).ToList();
            var unitPaymentOrders = dbContext
                .UnitPaymentOrders
                .Where(upo => upoNos.Contains(upo.UPONo))
                .ToList()
                .Select(upo =>
                {
                    upo.IsPaid = true;
                    EntityExtension.FlagForUpdate(upo, identityService.Username, USER_AGENT);
                    return upo;
                })
                .ToList();
            dbContext.UnitPaymentOrders.UpdateRange(unitPaymentOrders);

            var result = "";

            //var bankExpenditureNoteNos = models.Select(element => element.DocumentNo).ToList();
            //var expeditions = dbContext
            //    .PurchasingDocumentExpeditions
            //    .Where(pde => bankExpenditureNoteNos.Contains(pde.BankExpenditureNoteNo))
            //    .ToList()
            //    .Select(pde =>
            //    {
            //        pde.IsPaid = true;
            //        EntityExtension.FlagForUpdate(pde, identityService.Username, USER_AGENT);
            //        return pde;
            //    })
            //    .ToList();
            //dbContext.PurchasingDocumentExpeditions.UpdateRange(expeditions);

            foreach (var model in models)
            {
                if (model.IsPosted)
                {
                    result += "Nomor " + model.DocumentNo + ", ";
                }
                else
                {
                    model.IsPosted = true;
                    await CreateJournalTransaction(model, identityService);
                    await CreateDailyBankTransaction(model, identityService);
                    //CreateCreditorAccount(model, identityService);
                    await CreateCreditorAccountv2(model, identityService);
                    EntityExtension.FlagForUpdate(model, identityService.Username, USER_AGENT);
                    await dbContext.SaveChangesAsync();
                }
            }

            return result;
        }

        public MemoryStream GeneratePdfTemplate(BankExpenditureNoteModel model, int clientTimeZoneOffset)
        {
            const int MARGIN = 15;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A4, MARGIN, MARGIN, MARGIN, MARGIN);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            Dictionary<string, double> units = new Dictionary<string, double>();
            model.Details = model.Details.OrderBy(o => o.SupplierName).ToList();

            #region Header

            PdfPTable headerTable = new PdfPTable(2);
            headerTable.SetWidths(new float[] { 10f, 10f });
            headerTable.WidthPercentage = 100;
            PdfPTable headerTable1 = new PdfPTable(1);
            PdfPTable headerTable2 = new PdfPTable(2);
            headerTable2.SetWidths(new float[] { 15f, 40f });
            headerTable2.WidthPercentage = 100;

            PdfPCell cellHeader1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody = new PdfPCell() { Border = Rectangle.NO_BORDER };

            PdfPCell cellHeaderCS2 = new PdfPCell() { Border = Rectangle.NO_BORDER, Colspan = 2 };


            cellHeaderCS2.Phrase = new Phrase("BUKTI PENGELUARAN BANK", bold_font);
            cellHeaderCS2.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(cellHeaderCS2);

            cellHeaderBody.Phrase = new Phrase("PT. EFRATA GARMINDO UTAMA", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("Banaran, Grogol, Sukoharjo, Jawa Tengah", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("57552" + "Telp (+62 271)719911, (+62 21)2900977", normal_font);
            headerTable1.AddCell(cellHeaderBody);

            cellHeader1.AddElement(headerTable1);
            headerTable.AddCell(cellHeader1);

            cellHeaderCS2.Phrase = new Phrase("", bold_font);
            headerTable2.AddCell(cellHeaderCS2);

            cellHeaderBody.Phrase = new Phrase("Tanggal", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + model.Date.AddHours(clientTimeZoneOffset).ToString("dd MMMM yyyy"), normal_font);
            headerTable2.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("NO", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + model.DocumentNo, normal_font);
            headerTable2.AddCell(cellHeaderBody);

            List<string> supplier = model.Details.Select(m => m.SupplierName).Distinct().ToList();
            cellHeaderBody.Phrase = new Phrase("Dibayarkan ke", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + (supplier.Count > 0 ? supplier[0] : "-"), normal_font);
            headerTable2.AddCell(cellHeaderBody);

            for (int i = 1; i < supplier.Count; i++)
            {
                cellHeaderBody.Phrase = new Phrase("", normal_font);
                headerTable2.AddCell(cellHeaderBody);
                cellHeaderBody.Phrase = new Phrase(": " + supplier[i], normal_font);
                headerTable2.AddCell(cellHeaderBody);
            }

            cellHeaderBody.Phrase = new Phrase("Bank", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + model.BankAccountName + " - A/C : " + model.BankAccountNumber, normal_font);
            headerTable2.AddCell(cellHeaderBody);

            cellHeader2.AddElement(headerTable2);
            headerTable.AddCell(cellHeader2);

            cellHeaderCS2.Phrase = new Phrase("", normal_font);
            headerTable.AddCell(cellHeaderCS2);

            document.Add(headerTable);

            #endregion Header

            var upoNos = model.Details.Select(detail => detail.UnitPaymentOrderNo).ToList();
            var unitPaymentOrders = dbContext.UnitPaymentOrders.Where(unitPaymentOrder => upoNos.Contains(unitPaymentOrder.UPONo)).ToList();
            var currency = GetBICurrency(model.CurrencyCode, model.Date).Result;

            if (currency == null)
            {
                currency = new GarmentCurrency() { Rate = model.CurrencyRate };
            }

            int index = 1;
            double total = 0;
            bool sameCurrency = true;
            if (model.BankCurrencyCode != "IDR" || model.CurrencyCode == "IDR")
            {
                #region BodyNonIdr

                PdfPTable bodyTable = new PdfPTable(8);
                PdfPCell bodyCell = new PdfPCell();

                float[] widthsBody = new float[] { 5f, 10f, 10f, 10f, 8f, 7f, 15f, 7f };
                bodyTable.SetWidths(widthsBody);
                bodyTable.WidthPercentage = 100;

                bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                bodyCell.Phrase = new Phrase("No.", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("No. SPB", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Kategori Barang", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Divisi", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Unit", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Mata Uang", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Jumlah", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Pembayaran SPB ke-", bold_font);
                bodyTable.AddCell(bodyCell);

                foreach (BankExpenditureNoteDetailModel detail in model.Details)
                {
                    var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(element => element.UPONo == detail.UnitPaymentOrderNo);

                    if (unitPaymentOrder == null)
                        unitPaymentOrder = new UnitPaymentOrder();

                    double remaining = detail.SupplierPayment;
                    double previousPayment = detail.AmountPaid;

                    var unitSummaries = detail.Items.GroupBy(g => new { g.URNNo, g.UnitCode }).Select(s => new
                    {
                        UnitCode = s.Key.UnitCode,
                        URNNo = s.Key.URNNo
                    });

                    //var items = detail.Items
                    //    .GroupBy(m => new { m.UnitCode, m.UnitName })
                    //    .Select(s => new
                    //    {
                    //        s.First().UnitCode,
                    //        s.First().UnitName,
                    //        s.First().Price,
                    //        Total = s.Sum(d => detail.Vat == 0 ? d.Price : d.Price * 1.1)
                    //    });
                    if (unitSummaries.Count() > 1)
                    {
                        foreach (var item in detail.Items)
                        {
                            var vatAmount = unitPaymentOrder.UseVat ? item.Price * 0.1 : 0;
                            var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? item.Price * unitPaymentOrder.IncomeTaxRate / 100 : 0;
                            var dpp = item.Price + vatAmount - incomeTaxAmount;

                            if (remaining <= 0)
                            {
                                continue;
                            }

                            if (previousPayment > 0)
                            {
                                if (previousPayment >= dpp)
                                {
                                    previousPayment -= dpp;

                                    continue;
                                }
                                else
                                {
                                    dpp -= previousPayment;

                                    previousPayment -= dpp;
                                }
                            }

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                            bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            bodyCell.Phrase = new Phrase(detail.UnitPaymentOrderNo, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.CategoryName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.DivisionName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(item.UnitCode, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.Currency, normal_font);
                            bodyTable.AddCell(bodyCell);

                            if (remaining >= dpp)
                            {
                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", dpp), normal_font);
                                bodyTable.AddCell(bodyCell);

                                if (units.ContainsKey(item.UnitCode))
                                {
                                    units[item.UnitCode] += dpp;
                                }
                                else
                                {
                                    units.Add(item.UnitCode, dpp);
                                }

                                total += dpp;
                                remaining -= dpp;

                            }
                            else
                            {
                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", remaining), normal_font);
                                bodyTable.AddCell(bodyCell);

                                if (units.ContainsKey(item.UnitCode))
                                {
                                    units[item.UnitCode] += remaining;
                                }
                                else
                                {
                                    units.Add(item.UnitCode, remaining);
                                }

                                total += remaining;
                                remaining -= remaining;
                            }

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(detail.UPOIndex.ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);
                        }
                    }
                    else
                    {
                        foreach (var unitSummary in unitSummaries)
                        {
                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                            bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            bodyCell.Phrase = new Phrase(detail.UnitPaymentOrderNo, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.CategoryName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.DivisionName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(unitSummary.UnitCode, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.Currency, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", remaining), normal_font);
                            bodyTable.AddCell(bodyCell);

                            if (units.ContainsKey(unitSummary.UnitCode))
                            {
                                units[unitSummary.UnitCode] += remaining;
                            }
                            else
                            {
                                units.Add(unitSummary.UnitCode, remaining);
                            }

                            total += remaining;

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(detail.UPOIndex.ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);
                        }
                    }

                }

                bodyCell.Colspan = 4;
                bodyCell.Border = Rectangle.NO_BORDER;
                bodyCell.Phrase = new Phrase("", normal_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Colspan = 1;
                bodyCell.Border = Rectangle.BOX;
                bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                bodyCell.Phrase = new Phrase("Total", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Colspan = 1;
                bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                bodyCell.Phrase = new Phrase(model.BankCurrencyCode, bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", total), bold_font);
                bodyTable.AddCell(bodyCell);

                document.Add(bodyTable);

                #endregion BodyNonIdr
            }
            else
            {
                sameCurrency = false;
                #region BodyIdr
                PdfPTable bodyTable = new PdfPTable(9);
                PdfPCell bodyCell = new PdfPCell();

                float[] widthsBody = new float[] { 5f, 10f, 10f, 10f, 8f, 7f, 10f, 10f, 7f };
                bodyTable.SetWidths(widthsBody);
                bodyTable.WidthPercentage = 100;

                bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                bodyCell.Phrase = new Phrase("No.", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("No. SPB", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Kategori Barang", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Divisi", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Unit", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Mata Uang", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Jumlah", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Jumlah (IDR)", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Phrase = new Phrase("Pembayaran SPB ke-", bold_font);
                bodyTable.AddCell(bodyCell);

                foreach (BankExpenditureNoteDetailModel detail in model.Details)
                {
                    var unitPaymentOrder = unitPaymentOrders.FirstOrDefault(element => element.UPONo == detail.UnitPaymentOrderNo);

                    if (unitPaymentOrder == null)
                        unitPaymentOrder = new UnitPaymentOrder();

                    double remaining = detail.SupplierPayment;
                    double previousPayment = detail.AmountPaid;

                    var unitSummaries = detail.Items.GroupBy(g => new { g.URNNo, g.UnitCode }).Select(s => new
                    {
                        UnitCode = s.Key.UnitCode,
                        URNNo = s.Key.URNNo
                    });

                    //var items = detail.Items
                    //    .GroupBy(m => new { m.UnitCode, m.UnitName })
                    //    .Select(s => new
                    //    {
                    //        s.First().UnitCode,
                    //        s.First().UnitName,
                    //        s.First().Price,
                    //        Total = s.Sum(d => detail.Vat == 0 ? d.Price : d.Price * 1.1)
                    //    });
                    if (unitSummaries.Count() > 1)
                    {
                        foreach (var item in detail.Items)
                        {
                            var vatAmount = unitPaymentOrder.UseVat ? item.Price * 0.1 : 0;
                            var incomeTaxAmount = unitPaymentOrder.UseIncomeTax && unitPaymentOrder.IncomeTaxBy.ToUpper() == "SUPPLIER" ? item.Price * unitPaymentOrder.IncomeTaxRate / 100 : 0;
                            var dpp = item.Price + vatAmount - incomeTaxAmount;

                            if (remaining <= 0)
                            {
                                continue;
                            }

                            if (previousPayment > 0)
                            {
                                if (previousPayment >= dpp)
                                {
                                    previousPayment -= dpp;

                                    continue;
                                }
                                else
                                {
                                    dpp -= previousPayment;

                                    previousPayment -= dpp;
                                }
                            }

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                            bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            bodyCell.Phrase = new Phrase(detail.UnitPaymentOrderNo, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.CategoryName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.DivisionName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(item.UnitCode, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.Currency, normal_font);
                            bodyTable.AddCell(bodyCell);

                            if (remaining >= dpp)
                            {
                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", dpp), normal_font);
                                bodyTable.AddCell(bodyCell);

                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", dpp * model.CurrencyRate), normal_font);
                                bodyTable.AddCell(bodyCell);

                                if (units.ContainsKey(item.UnitCode))
                                {
                                    units[item.UnitCode] += dpp * model.CurrencyRate;
                                }
                                else
                                {
                                    units.Add(item.UnitCode, dpp * model.CurrencyRate);
                                }

                                total += dpp;
                                remaining -= dpp;

                            }
                            else
                            {
                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", remaining), normal_font);
                                bodyTable.AddCell(bodyCell);

                                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", remaining * model.CurrencyRate), normal_font);
                                bodyTable.AddCell(bodyCell);

                                if (units.ContainsKey(item.UnitCode))
                                {
                                    units[item.UnitCode] += remaining * model.CurrencyRate;
                                }
                                else
                                {
                                    units.Add(item.UnitCode, remaining * model.CurrencyRate);
                                }

                                total += remaining;
                                remaining -= remaining;
                            }

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(detail.UPOIndex.ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);
                        }
                    }
                    else
                    {
                        foreach (var unitSummary in unitSummaries)
                        {
                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                            bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            bodyCell.Phrase = new Phrase(detail.UnitPaymentOrderNo, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.CategoryName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.DivisionName, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(unitSummary.UnitCode, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.Phrase = new Phrase(detail.Currency, normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", remaining), normal_font);
                            bodyTable.AddCell(bodyCell);

                            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", (remaining * model.CurrencyRate)), normal_font);
                            bodyTable.AddCell(bodyCell);

                            if (units.ContainsKey(unitSummary.UnitCode))
                            {
                                units[unitSummary.UnitCode] += (remaining * model.CurrencyRate);
                            }
                            else
                            {
                                units.Add(unitSummary.UnitCode, (remaining * model.CurrencyRate));
                            }

                            total += remaining;

                            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            bodyCell.Phrase = new Phrase(detail.UPOIndex.ToString(), normal_font);
                            bodyTable.AddCell(bodyCell);
                        }
                    }
                }

                bodyCell.Colspan = 4;
                bodyCell.Border = Rectangle.NO_BORDER;
                bodyCell.Phrase = new Phrase("", normal_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Colspan = 1;
                bodyCell.Border = Rectangle.BOX;
                bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                bodyCell.Phrase = new Phrase("Total", bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.Colspan = 1;
                bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                bodyCell.Phrase = new Phrase(model.BankCurrencyCode, bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", total), bold_font);
                bodyTable.AddCell(bodyCell);

                bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                bodyCell.Phrase = new Phrase(string.Format("{0:n4}", total * model.CurrencyRate), bold_font);
                bodyTable.AddCell(bodyCell);

                document.Add(bodyTable);

                #endregion BodyIdr
            }



            #region BodyFooter

            PdfPTable bodyFooterTable = new PdfPTable(6);
            bodyFooterTable.SetWidths(new float[] { 3f, 6f, 2f, 6f, 10f, 10f });
            bodyFooterTable.WidthPercentage = 100;

            PdfPCell bodyFooterCell = new PdfPCell() { Border = Rectangle.NO_BORDER };

            bodyFooterCell.Colspan = 1;
            bodyFooterCell.Phrase = new Phrase("");
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 1;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase("Rincian per bagian:", normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 4;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyFooterCell.Phrase = new Phrase("");
            bodyFooterTable.AddCell(bodyFooterCell);
            total = model.CurrencyId > 0 && sameCurrency == false ? total * model.CurrencyRate : total;
            foreach (var unit in units)
            {
                bodyFooterCell.Colspan = 1;
                bodyFooterCell.Phrase = new Phrase("");
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(unit.Key, normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(model.BankCurrencyCode, normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(string.Format("{0:n4}", unit.Value), normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Colspan = 2;
                bodyFooterCell.Phrase = new Phrase("");
                bodyFooterTable.AddCell(bodyFooterCell);
            }

            bodyFooterCell.Colspan = 1;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase("");
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Phrase = new Phrase("Terbilang", normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyFooterCell.Phrase = new Phrase(": " + model.BankCurrencyCode, normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 3;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase(NumberToTextIDN.terbilang(total), normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);


            document.Add(bodyFooterTable);
            document.Add(new Paragraph("\n"));

            #endregion BodyFooter

            #region Footer

            PdfPTable footerTable = new PdfPTable(2);
            PdfPCell cellFooter = new PdfPCell() { Border = Rectangle.NO_BORDER };

            float[] widthsFooter = new float[] { 10f, 5f };
            footerTable.SetWidths(widthsFooter);
            footerTable.WidthPercentage = 100;

            cellFooter.Phrase = new Phrase("Dikeluarkan dengan cek/BG No. : " + model.BGCheckNumber, normal_font);
            footerTable.AddCell(cellFooter);

            cellFooter.Phrase = new Phrase("", normal_font);
            footerTable.AddCell(cellFooter);

            PdfPTable signatureTable = new PdfPTable(2);
            PdfPCell signatureCell = new PdfPCell() { HorizontalAlignment = Element.ALIGN_CENTER };
            signatureCell.Phrase = new Phrase("Bag. Keuangan", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureCell.HorizontalAlignment = Element.ALIGN_CENTER;
            signatureCell.Phrase = new Phrase("Bag. Akuntansi", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureTable.AddCell(new PdfPCell()
            {
                Phrase = new Phrase("---------------------------", normal_font),
                FixedHeight = 40,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            signatureTable.AddCell(new PdfPCell()
            {
                Phrase = new Phrase("---------------------------", normal_font),
                FixedHeight = 40,
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            footerTable.AddCell(new PdfPCell(signatureTable));

            cellFooter.Phrase = new Phrase("", normal_font);
            footerTable.AddCell(cellFooter);
            document.Add(footerTable);

            #endregion Footer

            document.Close();

            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }

    public class ExpenditureInfo
    {
        public string DocumentNo { get; set; }
        public string BankName { get; set; }
        public string BGCheckNumber { get; set; }
    }
}
