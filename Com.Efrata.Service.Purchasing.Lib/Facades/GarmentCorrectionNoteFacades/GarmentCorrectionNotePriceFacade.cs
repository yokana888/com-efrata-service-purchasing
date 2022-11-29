using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades
{
    public class GarmentCorrectionNotePriceFacade : IGarmentCorrectionNotePriceFacade
    {
        private string USER_AGENT = "Facade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentCorrectionNote> dbSet;

        public GarmentCorrectionNotePriceFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentCorrectionNote>();
        }

        public Tuple<List<GarmentCorrectionNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentCorrectionNote> Query = dbSet;

            Query = Query.Where(m => (m.CorrectionType ?? "").ToUpper().StartsWith("HARGA"));

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

        public async Task<int> Create(GarmentCorrectionNote garmentCorrectionNote)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(garmentCorrectionNote, identityService.Username, USER_AGENT);
                    var supplier = GetSupplier(garmentCorrectionNote.SupplierId);
                    garmentCorrectionNote.CorrectionNo = GenerateNo("NK", garmentCorrectionNote, supplier.Import ? "I" : "L");
                    if (garmentCorrectionNote.UseVat)
                    {
                        garmentCorrectionNote.NKPN = GenerateNKPN("NKPN", garmentCorrectionNote);
                    }
                    if (garmentCorrectionNote.UseIncomeTax)
                    {
                        garmentCorrectionNote.NKPH = GenerateNKPH("NKPH", garmentCorrectionNote);
                    }

                    if (((garmentCorrectionNote.CorrectionType ?? "").ToUpper() == "HARGA SATUAN"))
                    {
                        garmentCorrectionNote.TotalCorrection = garmentCorrectionNote.Items.Sum(i => (i.PricePerDealUnitAfter - i.PricePerDealUnitBefore) * i.Quantity);
                    }
                    else if ((garmentCorrectionNote.CorrectionType ?? "").ToUpper() == "HARGA TOTAL")
                    {
                        garmentCorrectionNote.TotalCorrection = garmentCorrectionNote.Items.Sum(i => i.PriceTotalAfter - i.PriceTotalBefore);
                    }

                    var garmentDeliveryOrder = dbContext.GarmentDeliveryOrders.First(d => d.Id == garmentCorrectionNote.DOId);
                    garmentDeliveryOrder.IsCorrection = true;
                    EntityExtension.FlagForUpdate(garmentDeliveryOrder, identityService.Username, USER_AGENT);

                    foreach (var item in garmentCorrectionNote.Items)
                    {
                        EntityExtension.FlagForCreate(item, identityService.Username, USER_AGENT);

                        var garmentDeliveryOrderDetail = dbContext.GarmentDeliveryOrderDetails.First(d => d.Id == item.DODetailId);
                        if ((garmentCorrectionNote.CorrectionType ?? "").ToUpper() == "HARGA SATUAN")
                        {
                            garmentDeliveryOrderDetail.PricePerDealUnitCorrection = (double)item.PricePerDealUnitAfter;
                            //garmentDeliveryOrderDetail.PriceTotalCorrection = (double)item.PriceTotalAfter;
                            garmentDeliveryOrderDetail.PriceTotalCorrection = (garmentDeliveryOrderDetail.QuantityCorrection - garmentDeliveryOrderDetail.ReturQuantity) * garmentDeliveryOrderDetail.PricePerDealUnitCorrection;

                        }
                        else if ((garmentCorrectionNote.CorrectionType ?? "").ToUpper() == "HARGA TOTAL")
                        {
                            garmentDeliveryOrderDetail.PriceTotalCorrection = (double)item.PriceTotalAfter;
                        }
                        EntityExtension.FlagForUpdate(garmentDeliveryOrderDetail, identityService.Username, USER_AGENT);
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

        private string GenerateNo(string code, GarmentCorrectionNote garmentCorrectionNote, string suffix = "")
        {
            string Year = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");

            string no = string.Concat(code, Year, Month);
            int Padding = 4;

            var lastNo = dbSet.Where(w => (w.CorrectionNo ?? "").StartsWith(no) && (w.CorrectionNo ?? "").EndsWith(suffix) && !w.IsDeleted).OrderByDescending(o => o.CorrectionNo).FirstOrDefaultAsync().Result;

            int lastNoNumber = 0;
            if (lastNo != null)
            {
                int.TryParse(lastNo.CorrectionNo.Substring(no.Length, lastNo.CorrectionNo.Length - no.Length - suffix.Length), out lastNoNumber);
            }
            return no + (lastNoNumber + 1).ToString().PadLeft(Padding, '0') + suffix;
        }

        private string GenerateNKPN(string code, GarmentCorrectionNote garmentCorrectionNote)
        {
            string Year = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");

            string no = string.Concat(code, Year, Month);
            int Padding = 4;

            var lastData = dbSet.Where(w => (w.NKPN ?? "").StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.NKPN).FirstOrDefaultAsync().Result;

            int lastNoNumber = 0;
            if (lastData != null)
            {
                int.TryParse(lastData.NKPN.Substring(no.Length, lastData.NKPN.Length - no.Length), out lastNoNumber);
            }
            return no + (lastNoNumber + 1).ToString().PadLeft(Padding, '0');
        }

        private string GenerateNKPH(string code, GarmentCorrectionNote garmentCorrectionNote)
        {
            string Year = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentCorrectionNote.CorrectionDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");

            string no = string.Concat(code, Year, Month);
            int Padding = 4;

            var lastData = dbSet.Where(w => (w.NKPH ?? "").StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.NKPH).FirstOrDefaultAsync().Result;

            int lastNoNumber = 0;
            if (lastData != null)
            {
                int.TryParse(lastData.NKPH.Substring(no.Length, lastData.NKPH.Length - no.Length), out lastNoNumber);
            }
            return no + (lastNoNumber + 1).ToString().PadLeft(Padding, '0');
        }

        private SupplierViewModel GetSupplier(long supplierId)
        {
            string supplierUri = "master/garment-suppliers";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Core}{supplierUri}/{supplierId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                SupplierViewModel viewModel = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                return null;
            }
        }
    }
}
