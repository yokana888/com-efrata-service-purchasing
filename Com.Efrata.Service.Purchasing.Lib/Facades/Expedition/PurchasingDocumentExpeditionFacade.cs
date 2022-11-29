using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Expedition
{
    public interface IPurchasingDocumentExpeditionFacade : IReadable, IDeleteable
    {
        Task<PurchasingDocumentExpedition> ReadModelById(int id);
        Task<int> DeleteByUPONo(string unitPaymentOrderNo);
        Task<int> SendToVerification(object list, string username);
        Task<int> PurchasingDocumentAcceptance(PurchasingDocumentAcceptanceViewModel data, string username);
        Task<int> DeletePurchasingDocumentAcceptance(int id);
        Task<int> UnitPaymentOrderVerification(PurchasingDocumentExpedition data, string username);
        void UpdateUnitPaymentOrderPosition(List<string> unitPaymentOrders, ExpeditionPosition position, string username);
    }

    public class PurchasingDocumentExpeditionFacade : IPurchasingDocumentExpeditionFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<PurchasingDocumentExpedition> dbSet;
        private readonly DbSet<UnitPaymentOrder> unitPaymentOrderDbSet;

        //var unitPaymentOrderDbSet = dbContext.Set<UnitPaymentOrder>();


        public PurchasingDocumentExpeditionFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<PurchasingDocumentExpedition>();
            unitPaymentOrderDbSet = dbContext.Set<UnitPaymentOrder>();
        }

        public Tuple<List<object>, int, Dictionary<string, string>> Read(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            IQueryable<PurchasingDocumentExpedition> Query = this.dbContext.PurchasingDocumentExpeditions;

            Query = Query
                .Select(s => new PurchasingDocumentExpedition
                {
                    Id = s.Id,
                    UnitPaymentOrderNo = s.UnitPaymentOrderNo,
                    UPODate = s.UPODate,
                    DueDate = s.DueDate,
                    InvoiceNo = s.InvoiceNo,
                    PaymentMethod = s.PaymentMethod,
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    CategoryCode = s.CategoryCode,
                    CategoryName = s.CategoryName,
                    DivisionCode = s.DivisionCode,
                    DivisionName = s.DivisionName,
                    TotalPaid = s.TotalPaid,
                    Currency = s.Currency,
                    Position = s.Position,
                    IsPaid = s.IsPaid,
                    IsPaidPPH = s.IsPaidPPH,
                    VerifyDate = s.VerifyDate,
                    LastModifiedUtc = s.LastModifiedUtc,
                    Active = s.Active,
                    IsDeleted = s.IsDeleted,
                    Vat = s.Vat,
                    IncomeTax = s.IncomeTax
                });

            List<string> searchAttributes = new List<string>()
            {
                "UnitPaymentOrderNo", "InvoiceNo", "SupplierName", "DivisionName"
            };

            Query = QueryHelper<PurchasingDocumentExpedition>.ConfigureSearch(Query, searchAttributes, keyword);

            if (filter.Contains("verificationFilter"))
            {
                filter = "{}";
                List<ExpeditionPosition> positions = new List<ExpeditionPosition> { ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, ExpeditionPosition.SEND_TO_CASHIER_DIVISION };
                Query = Query.Where(p => positions.Contains(p.Position));
            }

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            Query = QueryHelper<PurchasingDocumentExpedition>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<PurchasingDocumentExpedition>.ConfigureOrder(Query, OrderDictionary);

            Pageable<PurchasingDocumentExpedition> pageable = new Pageable<PurchasingDocumentExpedition>(Query, page - 1, size);
            List<PurchasingDocumentExpedition> Data = pageable.Data.ToList<PurchasingDocumentExpedition>();
            int TotalData = pageable.TotalCount;

            List<object> list = new List<object>();
            list.AddRange(
               Data.Select(s => new
               {
                   Id = s.Id,
                   UnitPaymentOrderNo = s.UnitPaymentOrderNo,
                   UPODate = s.UPODate,
                   InvoiceNo = s.InvoiceNo,
                   PaymentMethod = s.PaymentMethod,
                   DueDate = s.DueDate,
                   SupplierName = s.SupplierName,
                   CategoryName = s.CategoryName,
                   DivisionName = s.DivisionName,
                   TotalPaid = s.TotalPaid,
                   Currency = s.Currency,
                   Position = s.Position,
                   VerifyDate = s.VerifyDate,
                   _LastModifiedUtc = s.LastModifiedUtc,
                   s.Active,
                   s.IsDeleted,
                   s.IncomeTax,
                   s.Vat
               }).ToList()
            );

            return Tuple.Create(list, TotalData, OrderDictionary);
        }

        public async Task<PurchasingDocumentExpedition> ReadModelById(int id)
        {
            return await this.dbContext.PurchasingDocumentExpeditions
                .AsNoTracking()
                .Include(p => p.Items)
                .Where(d => d.Id.Equals(id) && d.IsDeleted.Equals(false))
                .FirstOrDefaultAsync();
        }

        public async Task<int> Delete(int id)
        {
            int Count = 0;

            if (this.dbContext.PurchasingDocumentExpeditions.Count(p => p.Id == id && p.IsDeleted == false).Equals(0))
            {
                return 0;
            }

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    IdentityService identityService = serviceProvider.GetService<IdentityService>();
                    PurchasingDocumentExpedition purchasingDocumentExpedition = dbContext.PurchasingDocumentExpeditions.Single(p => p.Id == id && p.Position == ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION);

                    ICollection<PurchasingDocumentExpeditionItem> Items = new List<PurchasingDocumentExpeditionItem>(this.dbContext.PurchasingDocumentExpeditionItems.Where(p => p.PurchasingDocumentExpeditionId.Equals(id)));

                    foreach (PurchasingDocumentExpeditionItem item in Items)
                    {
                        EntityExtension.FlagForDelete(item, identityService.Username, "Facade");
                        this.dbContext.PurchasingDocumentExpeditionItems.Update(item);
                    }

                    EntityExtension.FlagForDelete(purchasingDocumentExpedition, identityService.Username, "Facade");
                    this.dbSet.Update(purchasingDocumentExpedition);
                    Count = await this.dbContext.SaveChangesAsync();

                    UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.PURCHASING_DIVISION, identityService.Username);

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

        public async Task<int> DeleteByUPONo(string unitPaymentOrderNo)
        {
            int Count = 0;

            if (this.dbSet.Count(p => p.IsDeleted == false && p.UnitPaymentOrderNo == unitPaymentOrderNo).Equals(0))
            {
                return 0;
            }

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    IdentityService identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
                    dbContext.PurchasingDocumentExpeditions
                        .Where(p => p.UnitPaymentOrderNo == unitPaymentOrderNo)
                        .ToList()
                        .ForEach(p =>
                        {
                            p.IsDeleted = true;
                            p.LastModifiedAgent = "Service";
                            p.LastModifiedBy = identityService.Username;
                            p.LastModifiedUtc = DateTime.UtcNow;
                            p.DeletedAgent = "Service";
                            p.DeletedBy = identityService.Username;
                            p.DeletedUtc = DateTime.UtcNow;
                        });

                    Count = await dbContext.SaveChangesAsync();
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

        public async Task<int> SendToVerification(object list, string username)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
             {
                try
                {
                    List<string> unitPaymentOrders = new List<string>();


                    foreach (PurchasingDocumentExpedition purchasingDocumentExpedition in (List<PurchasingDocumentExpedition>)list)
                    {

                        unitPaymentOrders.Add(purchasingDocumentExpedition.UnitPaymentOrderNo);

                        var existing = this.dbContext.PurchasingDocumentExpeditions
                                                                .Include(d => d.Items)
                                                                .FirstOrDefault(p => p.UnitPaymentOrderNo == purchasingDocumentExpedition.UnitPaymentOrderNo && p.IsDeleted == false);

                        purchasingDocumentExpedition.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
                        purchasingDocumentExpedition.Active = true;
                        purchasingDocumentExpedition.SendToVerificationDivisionBy = username;

                        foreach (PurchasingDocumentExpeditionItem purchasingDocumentExpeditionItem in purchasingDocumentExpedition.Items)
                        {

                            EntityExtension.FlagForCreate(purchasingDocumentExpeditionItem, username, "Facade");

                        }

                        EntityExtension.FlagForCreate(purchasingDocumentExpedition, username, "Facade");


                        this.dbSet.Add(purchasingDocumentExpedition);

                        //if (existing != null)
                        //{
                        //    existing.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
                        //    existing.Active = true;
                        //    existing.SendToVerificationDivisionBy = username;
                        //    EntityExtension.FlagForUpdate(existing, username, "Facade");
                        //    this.dbSet.Update(existing);
                        //}
                        //else if (existing == null)
                        //{
                        //    purchasingDocumentExpedition.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
                        //    purchasingDocumentExpedition.Active = true;
                        //    purchasingDocumentExpedition.SendToVerificationDivisionBy = username;

                        //    foreach (PurchasingDocumentExpeditionItem purchasingDocumentExpeditionItem in purchasingDocumentExpedition.Items)
                        //    {

                        //        EntityExtension.FlagForCreate(purchasingDocumentExpeditionItem, username, "Facade");

                        //    }

                        //    EntityExtension.FlagForCreate(purchasingDocumentExpedition, username, "Facade");


                        //    this.dbSet.Add(purchasingDocumentExpedition);
                        //}
                        Created = await dbContext.SaveChangesAsync();

                    }



                    UpdateUnitPaymentOrderPosition(unitPaymentOrders, ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION, username);

                    transaction.Commit();
                }
                catch (ServiceValidationExeption e)
                {
                    transaction.Rollback();
                    throw new ServiceValidationExeption(e.ValidationContext, e.ValidationResults);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        public async Task<int> PurchasingDocumentAcceptance(PurchasingDocumentAcceptanceViewModel data, string username)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> unitPaymentOrders = new List<string>();

                    #region Verification
                    if (data.Role.Equals("VERIFICATION"))
                    {
                        foreach (PurchasingDocumentAcceptanceItem item in data.PurchasingDocumentExpedition)
                        {
                            unitPaymentOrders.Add(item.UnitPaymentOrderNo);

                            PurchasingDocumentExpedition model = new PurchasingDocumentExpedition
                            {
                                Id = item.Id,
                                VerificationDivisionBy = username,
                                VerificationDivisionDate = DateTimeOffset.UtcNow,
                                Position = ExpeditionPosition.VERIFICATION_DIVISION,
                            };

                            EntityExtension.FlagForUpdate(model, username, "Facade");
                            //dbContext.Attach(model);
                            dbContext.Entry(model).Property(x => x.VerificationDivisionBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.VerificationDivisionDate).IsModified = true;
                            dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(unitPaymentOrders, ExpeditionPosition.VERIFICATION_DIVISION, username);
                    }
                    #endregion Verification
                    #region Cashier
                    else if (data.Role.Equals("CASHIER"))
                    {
                        foreach (PurchasingDocumentAcceptanceItem item in data.PurchasingDocumentExpedition)
                        {
                            unitPaymentOrders.Add(item.UnitPaymentOrderNo);

                            PurchasingDocumentExpedition model = new PurchasingDocumentExpedition
                            {
                                Id = item.Id,
                                CashierDivisionBy = username,
                                CashierDivisionDate = DateTimeOffset.UtcNow,
                                Position = ExpeditionPosition.CASHIER_DIVISION,
                            };

                            EntityExtension.FlagForUpdate(model, username, "Facade");
                            //dbContext.Attach(model);
                            dbContext.Entry(model).Property(x => x.CashierDivisionBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.CashierDivisionDate).IsModified = true;
                            dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(unitPaymentOrders, ExpeditionPosition.CASHIER_DIVISION, username);
                    }
                    #endregion Cashier
                    #region Accounting
                    else if (data.Role.Equals("ACCOUNTING"))
                    {
                        foreach (PurchasingDocumentAcceptanceItem item in data.PurchasingDocumentExpedition)
                        {
                            unitPaymentOrders.Add(item.UnitPaymentOrderNo);

                            PurchasingDocumentExpedition model = new PurchasingDocumentExpedition
                            {
                                Id = item.Id,
                                AccountingDivisionBy = username,
                                AccountingDivisionDate = DateTimeOffset.UtcNow,
                                Position = ExpeditionPosition.FINANCE_DIVISION,
                            };

                            EntityExtension.FlagForUpdate(model, username, "Facade");
                            //dbContext.Attach(model);
                            dbContext.Entry(model).Property(x => x.AccountingDivisionBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.AccountingDivisionDate).IsModified = true;
                            dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(unitPaymentOrders, ExpeditionPosition.FINANCE_DIVISION, username);
                    }
                    #endregion Accounting
                    /*
                    #region Finance
                    else if (data.Role.Equals("FINANCE"))
                    {
                        foreach (PurchasingDocumentAcceptanceItem item in data.PurchasingDocumentExpedition)
                        {
                            unitPaymentOrders.Add(item.UnitPaymentOrderNo);

                            PurchasingDocumentExpedition model = new PurchasingDocumentExpedition
                            {
                                Id = item.Id,
                                FinanceDivisionBy = username,
                                FinanceDivisionDate = DateTimeOffset.UtcNow,
                                Position = ExpeditionPosition.FINANCE_DIVISION,
                            };

                            EntityExtension.FlagForUpdate(model, username, "Facade");
                            dbContext.Attach(model);
                            dbContext.Entry(model).Property(x => x.FinanceDivisionBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.FinanceDivisionDate).IsModified = true;
                            dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                            dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;
                        }

                        Updated = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(unitPaymentOrders, ExpeditionPosition.FINANCE_DIVISION);
                    }
                    #endregion Finance
                    */

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

        public async Task<int> DeletePurchasingDocumentAcceptance(int id)
        {
            int Count = 0;

            if (this.dbContext.PurchasingDocumentExpeditions.Count(p => p.Id == id && p.IsDeleted == false).Equals(0))
            {
                return 0;
            }

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    PurchasingDocumentExpedition model;
                    PurchasingDocumentExpedition purchasingDocumentExpedition = this.dbSet.AsNoTracking().Single(p => p.Id == id);
                    IdentityService identityService = serviceProvider.GetService<IdentityService>();

                    if (purchasingDocumentExpedition.Position == ExpeditionPosition.VERIFICATION_DIVISION)
                    {
                        model = new PurchasingDocumentExpedition
                        {
                            Id = id,
                            VerificationDivisionBy = null,
                            VerificationDivisionDate = null,
                            Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION,
                        };

                        EntityExtension.FlagForUpdate(model, identityService.Username, "Facade");
                        dbContext.Attach(model);
                        dbContext.Entry(model).Property(x => x.VerificationDivisionBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.VerificationDivisionDate).IsModified = true;
                        dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;

                        Count = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION, identityService.Username);
                    }
                    else if (purchasingDocumentExpedition.Position == ExpeditionPosition.CASHIER_DIVISION)
                    {
                        model = new PurchasingDocumentExpedition
                        {
                            Id = id,
                            CashierDivisionBy = null,
                            CashierDivisionDate = null,
                            Position = ExpeditionPosition.SEND_TO_CASHIER_DIVISION,
                        };

                        EntityExtension.FlagForUpdate(model, identityService.Username, "Facade");
                        dbContext.Attach(model);
                        dbContext.Entry(model).Property(x => x.CashierDivisionBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.CashierDivisionDate).IsModified = true;
                        dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;

                        Count = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_CASHIER_DIVISION, identityService.Username);
                    }
                    /*
                    else if (purchasingDocumentExpedition.Position == ExpeditionPosition.FINANCE_DIVISION)
                    {
                        model = new PurchasingDocumentExpedition
                        {
                            Id = id,
                            FinanceDivisionBy = null,
                            SendToFinanceDivisionDate = null,
                            Position = ExpeditionPosition.SEND_TO_FINANCE_DIVISION,
                        };

                        EntityExtension.FlagForUpdate(model, identityService.Username, "Facade");
                        dbContext.Attach(model);
                        dbContext.Entry(model).Property(x => x.FinanceDivisionBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.SendToFinanceDivisionDate).IsModified = true;
                        dbContext.Entry(model).Property(x => x.Position).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(model).Property(x => x.LastModifiedUtc).IsModified = true;

                        Count = await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_FINANCE_DIVISION);
                    }
                    */

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

        public async Task<int> UnitPaymentOrderVerification(PurchasingDocumentExpedition data, string username)
        {
            int Count = 0;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    PurchasingDocumentExpedition purchasingDocumentExpedition;

                    if (!data.Id.Equals(0))
                    {
                        purchasingDocumentExpedition = this.dbSet.AsNoTracking().Single(d => d.Id == data.Id);
                    }
                    else
                    {
                        purchasingDocumentExpedition = this.dbSet.AsNoTracking().Single(d => d.UnitPaymentOrderNo == data.UnitPaymentOrderNo && d.IsDeleted == false && d.Active == true);
                    }

                    if (data.Position.Equals(ExpeditionPosition.SEND_TO_PURCHASING_DIVISION))
                    {

                        purchasingDocumentExpedition.UnitPaymentOrderNo = data.UnitPaymentOrderNo;
                        purchasingDocumentExpedition.VerifyDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToPurchasingDivisionDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToPurchasingDivisionBy = username;
                        purchasingDocumentExpedition.Position = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION;
                        purchasingDocumentExpedition.Active = false;
                        purchasingDocumentExpedition.NotVerifiedReason = data.NotVerifiedReason;

                        EntityExtension.FlagForUpdate(purchasingDocumentExpedition, username, "Facade");
                        dbContext.Attach(purchasingDocumentExpedition);

                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToPurchasingDivisionDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToPurchasingDivisionBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Active).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.VerifyDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Position).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.NotVerifiedReason).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedUtc).IsModified = true;

                        await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, username);
                    }
                    else if (data.Position.Equals(ExpeditionPosition.SEND_TO_CASHIER_DIVISION))
                    {

                        purchasingDocumentExpedition.VerifyDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToCashierDivisionDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToCashierDivisionBy = username;
                        purchasingDocumentExpedition.Position = ExpeditionPosition.SEND_TO_CASHIER_DIVISION;
                        purchasingDocumentExpedition.Active = true;

                        EntityExtension.FlagForUpdate(purchasingDocumentExpedition, username, "Facade");
                        dbContext.Attach(purchasingDocumentExpedition);

                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToCashierDivisionDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToCashierDivisionBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Active).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.VerifyDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Position).IsModified = true;

                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedUtc).IsModified = true;

                        await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_CASHIER_DIVISION, username);

                    }

                    else if (data.Position.Equals(ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION))
                    {

                        purchasingDocumentExpedition.VerifyDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToAccountingDivisionDate = data.VerifyDate;
                        purchasingDocumentExpedition.SendToAccountingDivisionBy = username;
                        purchasingDocumentExpedition.Position = ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION;
                        purchasingDocumentExpedition.Active = true;


                        EntityExtension.FlagForUpdate(purchasingDocumentExpedition, username, "Facade");

                        dbContext.Attach(purchasingDocumentExpedition);

                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToAccountingDivisionDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.SendToAccountingDivisionBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Active).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.VerifyDate).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.Position).IsModified = true;

                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedAgent).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedBy).IsModified = true;
                        dbContext.Entry(purchasingDocumentExpedition).Property(x => x.LastModifiedUtc).IsModified = true;

                        await dbContext.SaveChangesAsync();
                        UpdateUnitPaymentOrderPosition(new List<string>() { purchasingDocumentExpedition.UnitPaymentOrderNo }, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, username);

                    }

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

        public void UpdateUnitPaymentOrderPosition(List<string> unitPaymentOrders, ExpeditionPosition position, string username)
        {
            //foreach(var item in unitPaymentOrders)
            //{
            //    var upo = dbContext.UnitPaymentOrders.FirstOrDefault(x => x.UPONo == item);
            //    if(upo != null)
            //    {

            //        upo.Position = (int) position;
            //        EntityExtension.FlagForUpdate(upo, username, "Facade");
            //    }
            //}

            //dbContext.SaveChanges();


            //unitPaymentOrder

            var unitPaymentOrderModels = unitPaymentOrderDbSet.Where(w => unitPaymentOrders.Contains(w.UPONo)).ToList();
            foreach (var unitPaymentOrder in unitPaymentOrderModels)
            {
                unitPaymentOrder.Position = (int)position;
                EntityExtension.FlagForUpdate(unitPaymentOrder, username, "Facade");
            }

            dbContext.SaveChanges();

            //unitPaymentOrderModels.UpdateRa



            //string unitPaymentOrderUri = "unit-payment-orders/update/position";

            //var data = new
            //{
            //    position = position,
            //    unitPaymentOrders = unitPaymentOrders
            //};

            //IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            //var response = httpClient.PutAsync($"{APIEndpoint.Purchasing}{unitPaymentOrderUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            //response.EnsureSuccessStatusCode();
        }
    }
}
