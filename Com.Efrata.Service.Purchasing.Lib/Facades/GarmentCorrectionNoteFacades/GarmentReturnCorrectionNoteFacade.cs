using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades
{
    public class GarmentReturnCorrectionNoteFacade : IGarmentReturnCorrectionNoteFacade
    {
        private string USER_AGENT = "Facade";
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentCorrectionNote> dbSet;

        public GarmentReturnCorrectionNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
            dbSet = dbContext.Set<GarmentCorrectionNote>();
        }

        public SupplierViewModel GetSupplier(long supplierId)
        {
            string supplierUri = "master/garment-suppliers";
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

        public Tuple<List<GarmentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentCorrectionNote> Query = dbSet;

            Query = Query.Where(m => m.CorrectionType.StartsWith("Retur"));

            Query = Query.Select(m => new GarmentCorrectionNote
            {
                Id = m.Id,
                CorrectionNo = m.CorrectionNo,
                CorrectionType = m.CorrectionType,
                CorrectionDate = m.CorrectionDate,
                SupplierName = m.SupplierName,
                DONo = m.DONo,
                UseIncomeTax = m.UseIncomeTax,
                UseVat = m.UseVat,
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc
            });

            List<string> searchAttributes = new List<string>()
            {
                "CorrectionNo", "CorrectionType", "SupplierName", "DONo"
            };

            Query = QueryHelper<GarmentCorrectionNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentCorrectionNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentCorrectionNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentCorrectionNote> pageable = new Pageable<GarmentCorrectionNote>(Query, Page - 1, Size);
            List<GarmentCorrectionNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public GarmentCorrectionNote ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                .FirstOrDefault();
            return model;
        }

        public async Task<int> Create(GarmentCorrectionNote garmentCorrectionNote, bool isImport, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(garmentCorrectionNote, user, USER_AGENT);
                    var supplier = GetSupplier(garmentCorrectionNote.SupplierId);
                    var supplierImport = false;
                    if (supplier != null)
                    {
                        supplierImport = supplier.Import;
                    }
                    garmentCorrectionNote.CorrectionNo = await GenerateNo(garmentCorrectionNote, supplierImport, clientTimeZoneOffset);
                    garmentCorrectionNote.TotalCorrection = garmentCorrectionNote.Items.Sum(i => i.PriceTotalAfter);

                    if (garmentCorrectionNote.UseIncomeTax == true)
                    {
                        garmentCorrectionNote.NKPH = await GenerateNKPH(garmentCorrectionNote, clientTimeZoneOffset);
                    }
                    else
                    {
                        garmentCorrectionNote.NKPH = "";
                    }
                    if (garmentCorrectionNote.UseVat == true)
                    {
                        garmentCorrectionNote.NKPN = await GenerateNKPN(garmentCorrectionNote, clientTimeZoneOffset);
                    }
                    else
                    {
                        garmentCorrectionNote.NKPN = "";
                    }

                    var garmentDeliveryOrder = dbContext.GarmentDeliveryOrders.First(d => d.Id == garmentCorrectionNote.DOId);
                    garmentDeliveryOrder.IsCorrection = true;

                    EntityExtension.FlagForUpdate(garmentDeliveryOrder, user, USER_AGENT);

                    foreach (var item in garmentCorrectionNote.Items)
                    {
                        item.Quantity = item.Quantity * (-1);
                        item.PriceTotalAfter = item.Quantity * item.PricePerDealUnitAfter;
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                        var garmentDeliveryOrderDetail = dbContext.GarmentDeliveryOrderDetails.First(d => d.Id == item.DODetailId);
                        var epoDetail = dbContext.GarmentExternalPurchaseOrderItems.First(d => d.Id == garmentDeliveryOrderDetail.EPOItemId);
                        //garmentDeliveryOrderDetail.QuantityCorrection = ((double)item.Quantity * (-1)) + garmentDeliveryOrderDetail.QuantityCorrection;
                        //garmentDeliveryOrderDetail.PriceTotalCorrection = garmentDeliveryOrderDetail.QuantityCorrection * garmentDeliveryOrderDetail.PricePerDealUnitCorrection;
                        garmentDeliveryOrderDetail.ReturQuantity = garmentDeliveryOrderDetail.ReturQuantity + ((double)item.Quantity*(-1));
                        garmentDeliveryOrderDetail.PriceTotalCorrection = (garmentDeliveryOrderDetail.QuantityCorrection - garmentDeliveryOrderDetail.ReturQuantity) * garmentDeliveryOrderDetail.PricePerDealUnitCorrection;

                        epoDetail.DOQuantity=epoDetail.DOQuantity + (double)item.Quantity ;
                        EntityExtension.FlagForUpdate(garmentDeliveryOrderDetail, user, USER_AGENT);
                    }

                    dbSet.Add(garmentCorrectionNote);

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

        public async Task<string> GenerateNo(GarmentCorrectionNote model, bool isImport, int clientTimeZoneOffset)
        {
            DateTimeOffset dateTimeOffsetNow = DateTimeOffset.Now;
            string Month = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Year = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Supplier = isImport ? "I" : "L";

            string no = $"NK{Year}{Month}";
            int Padding = 4;

            var lastNo = await this.dbSet.Where(w => w.CorrectionNo.StartsWith(no) && w.CorrectionNo.EndsWith(Supplier) && !w.IsDeleted).OrderByDescending(o => o.CorrectionNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0') + Supplier;
            }
            else
            {
                int.TryParse(lastNo.CorrectionNo.Replace(no, "").Replace(Supplier, ""), out int lastno1);
                int lastNoNumber = lastno1 + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0') + Supplier;
            }
        }
        public async Task<string> GenerateNKPN(GarmentCorrectionNote model, int clientTimeZoneOffset)
        {
            DateTimeOffset dateTimeOffsetNow = DateTimeOffset.Now;
            string Month = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Year = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");

            string no = $"NKPN{Year}{Month}";
            int Padding = 4;

            var lastNo = await this.dbSet.Where(w => w.NKPN.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.NKPN).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.NKPN.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }
        public async Task<string> GenerateNKPH(GarmentCorrectionNote model, int clientTimeZoneOffset)
        {
            DateTimeOffset dateTimeOffsetNow = DateTimeOffset.Now;
            string Month = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Year = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");

            string no = $"NKPH{Year}{Month}";
            int Padding = 4;

            var lastNo = await this.dbSet.Where(w => w.NKPH.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.NKPH).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.NKPH.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }
    }
}
