using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades
{
    public class GarmentInvoiceFacade : IGarmentInvoice
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentInvoice> dbSet;
        private readonly DbSet<GarmentDeliveryOrder> dbSetDeliveryOrder;
        public readonly IServiceProvider serviceProvider;
        private readonly IGarmentDebtBalanceService _garmentDebtBalanceService;
        private string USER_AGENT = "Facade";

        public GarmentInvoiceFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentInvoice>();
            this.dbSetDeliveryOrder = dbContext.Set<GarmentDeliveryOrder>();
            this.serviceProvider = serviceProvider;
            _garmentDebtBalanceService = serviceProvider.GetService<IGarmentDebtBalanceService>();
        }
        public Tuple<List<GarmentInvoice>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentInvoice> Query = this.dbSet.Include(m => m.Items).ThenInclude(i => i.Details);

            List<string> searchAttributes = new List<string>()
            {
                "InvoiceNo", "SupplierName","Items.DeliveryOrderNo","NPN","NPH"
            };

            Query = QueryHelper<GarmentInvoice>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentInvoice>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentInvoice>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentInvoice> pageable = new Pageable<GarmentInvoice>(Query, Page - 1, Size);
            List<GarmentInvoice> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public GarmentInvoice ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                 .Include(m => m.Items)
                     .ThenInclude(i => i.Details)
                 .FirstOrDefault();
            return model;
        }

        public async Task<int> Create(GarmentInvoice model, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    double _total = 0;
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);
                    if (model.UseIncomeTax)
                    {
                        model.NPH = GenerateNPH();
                    }
                    if (model.UseVat)
                    {
                        model.NPN = GenerateNPN();
                    }
                    foreach (var item in model.Items)
                    {
                        _total += item.TotalAmount;
                        GarmentDeliveryOrder deliveryOrder = dbSetDeliveryOrder.FirstOrDefault(s => s.Id == item.DeliveryOrderId);
                        if (deliveryOrder != null)
                            deliveryOrder.IsInvoice = true;
                        EntityExtension.FlagForCreate(item, username, USER_AGENT);

                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForCreate(detail, username, USER_AGENT);
                        }
                    }
                    model.TotalAmount = _total;

                    this.dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
                        var deliveryOrder = dbSetDeliveryOrder.FirstOrDefault(s => s.Id == item.DeliveryOrderId);
                        if (deliveryOrder != null)
                        {
                            var amount = 0.0;
                            var currencyAmount = 0.0;
                            var vatAmount = 0.0;
                            var currencyVATAmount = 0.0;
                            var incomeTaxAmount = 0.0;
                            var currencyIncomeTaxAmount = 0.0;

                            if (model.CurrencyCode == "IDR")
                            {
                                amount = item.TotalAmount;
                                if (model.IsPayVat)
                                {
                                    vatAmount = item.TotalAmount * 0.1;
                                    //vatAmount = item.TotalAmount * (model.VatRate / 100);
                                }

                                if (model.IsPayTax)
                                {
                                    incomeTaxAmount = item.TotalAmount * model.IncomeTaxRate / 100;
                                }
                            }
                            else
                            {
                                amount = item.TotalAmount * deliveryOrder.DOCurrencyRate.GetValueOrDefault();
                                currencyAmount = item.TotalAmount;
                                if (model.IsPayVat)
                                {
                                    vatAmount = amount * 0.1;
                                    //vatAmount = amount * (model.VatRate / 100);
                                    currencyVATAmount = item.TotalAmount * 0.1;
                                    //currencyVATAmount = item.TotalAmount * (model.VatRate / 100);
                                }

                                if (model.IsPayTax)
                                {
                                    incomeTaxAmount = amount * model.IncomeTaxRate / 100;
                                    currencyIncomeTaxAmount = item.TotalAmount * model.IncomeTaxRate / 100;
                                }
                            }

                            await _garmentDebtBalanceService.UpdateFromInvoice((int)deliveryOrder.Id, new InvoiceFormDto((int)model.Id, model.InvoiceDate, model.InvoiceNo, amount, currencyAmount, vatAmount, incomeTaxAmount, model.IsPayVat, model.IsPayTax, currencyVATAmount, currencyIncomeTaxAmount, model.VatNo));
                        }
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

        private string GenerateNPN()
        {
            string NPN = null;
            GarmentInvoice garmentInvoice = (from data in dbSet
                                             where data.NPN != null && data.NPN.StartsWith("NPN")
                                             orderby data.NPN descending
                                             select data).FirstOrDefault();
            string year = DateTime.Now.Year.ToString().Substring(2, 2);
            string month = DateTime.Now.Month.ToString("D2");
            string day = DateTime.Now.Day.ToString("D2");
            string formatDate = year + month;
            int counterId = 0;
            if (garmentInvoice != null)
            {
                NPN = garmentInvoice.NPN;
                string months = NPN.Substring(5, 2);
                string number = NPN.Substring(7);
                if (months == DateTime.Now.Month.ToString("D2"))
                {
                    counterId = Convert.ToInt32(number) + 1;
                }
                else
                {
                    counterId = 1;
                }
            }
            else
            {
                counterId = 1;
            }
            NPN = "NPN" + formatDate + counterId.ToString("D4");

            return NPN;
        }
        private string GenerateNPH()
        {
            string NPH = null;
            GarmentInvoice garmentInvoice = (from data in dbSet
                                             where data.NPH != null && data.NPH.StartsWith("NPH")
                                             orderby data.NPH descending
                                             select data).FirstOrDefault();
            string year = DateTime.Now.Year.ToString().Substring(2, 2);
            string month = DateTime.Now.Month.ToString("D2");
            string day = DateTime.Now.Day.ToString("D2");
            string formatDate = year + month;
            int counterId = 0;
            if (garmentInvoice != null)
            {
                NPH = garmentInvoice.NPH;
                string months = NPH.Substring(5, 2);
                string number = NPH.Substring(7);
                if (months == DateTime.Now.Month.ToString("D2"))
                {
                    counterId = Convert.ToInt32(number) + 1;
                }
                else
                {
                    counterId = 1;
                }
            }
            else
            {
                counterId = 1;
            }
            NPH = "NPH" + formatDate + counterId.ToString("D4");

            return NPH;
        }
        public int Delete(int id, string username)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = this.dbSet
                        .Include(d => d.Items)
                            .ThenInclude(d => d.Details)
                        .SingleOrDefault(pr => pr.Id == id && !pr.IsDeleted);

                    EntityExtension.FlagForDelete(model, username, USER_AGENT);

                    foreach (var item in model.Items)
                    {
                        GarmentDeliveryOrder garmentDeliveryOrder = dbSetDeliveryOrder.FirstOrDefault(s => s.Id == item.DeliveryOrderId);
                        if (garmentDeliveryOrder != null)
                            garmentDeliveryOrder.IsInvoice = false;
                        EntityExtension.FlagForDelete(item, username, USER_AGENT);
                        var deleted = _garmentDebtBalanceService.EmptyInvoice((int)item.DeliveryOrderId).Result;
                        foreach (var detail in item.Details)
                        {
                            EntityExtension.FlagForDelete(detail, username, USER_AGENT);
                        }
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

        public HashSet<long> GetGarmentInvoiceId(long id)
        {
            return new HashSet<long>(dbContext.GarmentInvoiceItems.Where(d => d.GarmentInvoice.Id == id).Select(d => d.Id));
        }
        public async Task<int> Update(int id, GarmentInvoice model, string user, int clientTimeZoneOffset = 7)
        {
            int Updated = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.Items != null)
                    {
                        double total = 0;
                        HashSet<long> detailIds = GetGarmentInvoiceId(id);
                        foreach (var itemId in detailIds)
                        {
                            GarmentInvoiceItem data = model.Items.FirstOrDefault(prop => prop.Id.Equals(itemId));
                            if (data == null)
                            {
                                GarmentInvoiceItem dataItem = dbContext.GarmentInvoiceItems.FirstOrDefault(prop => prop.Id.Equals(itemId));
                                EntityExtension.FlagForDelete(dataItem, user, USER_AGENT);
                                var Details = dbContext.GarmentInvoiceDetails.Where(prop => prop.InvoiceItemId.Equals(itemId)).ToList();
                                GarmentDeliveryOrder deliveryOrder = dbContext.GarmentDeliveryOrders.FirstOrDefault(s => s.Id.Equals(dataItem.DeliveryOrderId));
                                deliveryOrder.IsInvoice = false;
                                foreach (GarmentInvoiceDetail detail in Details)
                                {

                                    EntityExtension.FlagForDelete(detail, user, USER_AGENT);
                                }

                                await _garmentDebtBalanceService.EmptyInvoice((int)dataItem.DeliveryOrderId);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                            }

                            foreach (GarmentInvoiceItem item in model.Items)
                            {
                                total += item.TotalAmount;
                                if (item.Id <= 0)
                                {
                                    GarmentDeliveryOrder garmentDeliveryOrder = dbSetDeliveryOrder.FirstOrDefault(s => s.Id == item.DeliveryOrderId);
                                    if (garmentDeliveryOrder != null)
                                        garmentDeliveryOrder.IsInvoice = true;
                                    EntityExtension.FlagForCreate(item, user, USER_AGENT);

                                    var deliveryOrder = dbSetDeliveryOrder.FirstOrDefault(s => s.Id == item.DeliveryOrderId);
                                    if (deliveryOrder != null)
                                    {
                                        var amount = 0.0;
                                        var currencyAmount = 0.0;
                                        var vatAmount = 0.0;
                                        var currencyVATAmount = 0.0;
                                        var incomeTaxAmount = 0.0;
                                        var currencyIncomeTaxAmount = 0.0;

                                        if (model.CurrencyCode == "IDR")
                                        {
                                            amount = item.TotalAmount;
                                            if (model.IsPayVat)
                                            {
                                                vatAmount = item.TotalAmount * 0.1;
                                            }

                                            if (model.IsPayTax)
                                            {
                                                incomeTaxAmount = item.TotalAmount * model.IncomeTaxRate / 100;
                                            }
                                        }
                                        else
                                        {
                                            amount = item.TotalAmount * deliveryOrder.DOCurrencyRate.GetValueOrDefault();
                                            currencyAmount = item.TotalAmount;
                                            if (model.IsPayVat)
                                            {
                                                vatAmount = amount * 0.1;
                                                currencyVATAmount = item.TotalAmount * 0.1;
                                            }

                                            if (model.IsPayTax)
                                            {
                                                incomeTaxAmount = amount * model.IncomeTaxRate / 100;
                                                currencyIncomeTaxAmount = item.TotalAmount * model.IncomeTaxRate / 100;
                                            }
                                        }

                                        await _garmentDebtBalanceService.UpdateFromInvoice((int)deliveryOrder.Id, new InvoiceFormDto((int)model.Id, model.InvoiceDate, model.InvoiceNo, amount, currencyAmount, vatAmount, incomeTaxAmount, model.IsPayVat, model.IsPayTax, currencyVATAmount, currencyIncomeTaxAmount, model.VatNo));
                                    }
                                }
                                else
                                    EntityExtension.FlagForUpdate(item, user, USER_AGENT);

                                foreach (GarmentInvoiceDetail detail in item.Details)
                                {
                                    if (item.Id <= 0)
                                    {
                                        EntityExtension.FlagForCreate(detail, user, USER_AGENT);
                                    }
                                    else
                                        EntityExtension.FlagForUpdate(detail, user, USER_AGENT);
                                }
                            }
                        }
                    }
                    EntityExtension.FlagForUpdate(model, user, USER_AGENT);
                    this.dbSet.Update(model);

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

        public GarmentInvoice ReadByDOId(int id)
        {
            var model = dbSet.Where(m => m.Items.Any(i => i.DeliveryOrderId == id))
                 .Include(m => m.Items)
                     .ThenInclude(i => i.Details)
                 .FirstOrDefault();
            return model;
        }

        public List<GarmentInvoice> ReadForInternNote(List<long> garmentInvoiceIds)
        {
            var models = dbSet.Where(m => m.Items.Any(i => garmentInvoiceIds.Contains(m.Id)))
                .Select(m => new GarmentInvoice
                {
                    Id = m.Id,
                    Items = m.Items.Select(i => new GarmentInvoiceItem
                    {
                        Details = i.Details.Select(d => new GarmentInvoiceDetail
                        {
                            Id = d.Id,
                            DODetailId = d.DODetailId
                        }).ToList()
                    }).ToList()
                }).ToList();

            return models;
        }
    }
}
