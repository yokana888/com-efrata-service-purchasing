using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades
{
    public class GarmentReceiptCorrectionFacade : IGarmentReceiptCorrectionFacade
    {
        private string USER_AGENT = "Facade";

        public readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentReceiptCorrection> dbSet;
        private readonly DbSet<GarmentInventoryDocument> dbSetGarmentInventoryDocument;
        private readonly DbSet<GarmentInventoryMovement> dbSetGarmentInventoryMovement;
        private readonly DbSet<GarmentInventorySummary> dbSetGarmentInventorySummary;
        private readonly IMapper mapper;

        public GarmentReceiptCorrectionFacade(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentReceiptCorrection>();

            dbSetGarmentInventoryDocument= dbContext.Set< GarmentInventoryDocument>();
            dbSetGarmentInventoryMovement = dbContext.Set<GarmentInventoryMovement>();
            dbSetGarmentInventorySummary = dbContext.Set<GarmentInventorySummary>();

            mapper = serviceProvider == null ? null : (IMapper)serviceProvider.GetService(typeof(IMapper));
        }

        public Tuple<List<GarmentReceiptCorrection>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentReceiptCorrection> Query = this.dbSet.Include(m => m.Items);
            
            Query = Query.Select(m => new GarmentReceiptCorrection
            {
                Id = m.Id,
                CorrectionNo=m.CorrectionNo,
                CorrectionDate=m.CorrectionDate,
                URNNo = m.URNNo,
                UnitName = m.UnitName,
                StorageName=m.StorageName,
                Items = m.Items.ToList(),
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc,
                CorrectionType=m.CorrectionType,

            });

            List<string> searchAttributes = new List<string>()
            {
                "CorrectionNo", "URNNo","UnitName","StorageName"
            };

            Query = QueryHelper<GarmentReceiptCorrection>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentReceiptCorrection>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentReceiptCorrection>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentReceiptCorrection> pageable = new Pageable<GarmentReceiptCorrection>(Query, Page - 1, Size);
            List<GarmentReceiptCorrection> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public GarmentReceiptCorrection ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                 .Include(m => m.Items)
                 .FirstOrDefault();
            return model;
        }

        public async Task<int> Create(GarmentReceiptCorrection m, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    m.CorrectionNo = await GenerateNo(m, clientTimeZoneOffset);
                    m.CorrectionDate = DateTimeOffset.Now;

                    if(m.CorrectionType == "Konversi")
                    {
                        GarmentReceiptCorrection outCorrection = new GarmentReceiptCorrection
                        {
                            CorrectionDate=m.CorrectionDate,
                            CorrectionNo=m.CorrectionNo,
                            Remark=m.Remark,
                            StorageCode=m.StorageCode,
                            StorageId=m.StorageId,
                            StorageName=m.StorageName,
                            CorrectionType=m.CorrectionType,
                            UnitCode=m.UnitCode,
                            UnitName=m.UnitName,
                            UnitId=m.UnitId,
                            URNId=m.URNId,
                            URNNo=m.URNNo,
                        };
                        List<GarmentReceiptCorrectionItem> itemsOut = new List<GarmentReceiptCorrectionItem>();
                        foreach (var outs in m.Items)
                        {
                            GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.FirstOrDefault(a => a.Id == outs.URNItemId);
                            double SmallQuantityOut= (double)garmentUnitReceiptNoteItem.ReceiptCorrection * (double)garmentUnitReceiptNoteItem.CorrectionConversion;
                            GarmentReceiptCorrectionItem outItem = new GarmentReceiptCorrectionItem
                            {

                                PricePerDealUnit= outs.PricePerDealUnit,
                                POItemId= outs.POItemId,
                                Conversion= outs.Conversion,
                                SmallQuantity= SmallQuantityOut,
                                DODetailId= outs.DODetailId,
                                ProductCode= outs.ProductCode,
                                ProductId= outs.ProductId,
                                POSerialNumber= outs.POSerialNumber,
                                ProductName= outs.ProductName,
                                PRItemId= outs.PRItemId,
                                ProductRemark= outs.ProductRemark,
                                Quantity= outs.Quantity,
                                CorrectionConversion= outs.CorrectionConversion,
                                CorrectionId=outs.CorrectionId,
                                CorrectionQuantity= outs.CorrectionQuantity,
                                DesignColor= outs.DesignColor,
                                EPOItemId= outs.EPOItemId,
                                RONo= outs.RONo,
                                SmallUomId= outs.SmallUomId,
                                SmallUomUnit= outs.SmallUomUnit,
                                UomId= outs.UomId,
                                UomUnit= outs.UomUnit,
                                URNItemId= outs.URNItemId
                            };
                            itemsOut.Add(outItem);
                            var garmentInventorySummaryExistingOut = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == outItem.ProductId && s.StorageId == outCorrection.StorageId && s.UomId == outItem.SmallUomId);
                            
                            var garmentInventoryMovementOut = GenerateGarmentInventoryMovement(outCorrection, outItem, garmentInventorySummaryExistingOut, "OUT");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementOut);

                            if (garmentInventorySummaryExistingOut == null)
                            {
                                var garmentInventorySummaryOut = GenerateGarmentInventorySummary(outCorrection, outItem, garmentInventoryMovementOut);
                                dbSetGarmentInventorySummary.Add(garmentInventorySummaryOut);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExistingOut, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExistingOut.Quantity = garmentInventoryMovementOut.After;
                            }

                            

                            await dbContext.SaveChangesAsync();
                        }
                        outCorrection.Items = itemsOut;

                        var invOut = GenerateGarmentInventoryDocumentConv(outCorrection, "OUT");
                        dbSetGarmentInventoryDocument.Add(invOut);
                       

                    }
                    var type = "IN";
                    foreach (var item in m.Items)
                    {
                        
                        GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.FirstOrDefault(a => a.Id == item.URNItemId);
                        GarmentDOItems garmentDOItems = dbContext.GarmentDOItems.SingleOrDefault(x => x.URNItemId == item.URNItemId);

                        if (item.CorrectionQuantity < 0)
                        {
                            type = "OUT";
                        }

                        if (m.CorrectionType == "Jumlah")
                        {
                            item.CorrectionConversion = 0;
                            garmentUnitReceiptNoteItem.ReceiptCorrection += (decimal)item.CorrectionQuantity;

                            var garmentInventoryDocument = GenerateGarmentInventoryDocument(m, item, type);
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                            if(garmentDOItems!=null)
                                garmentDOItems.RemainingQuantity += (decimal)item.SmallQuantity;

                        }
                        else
                        {
                            //decimal qty = (garmentUnitReceiptNoteItem.ReceiptCorrection - (garmentUnitReceiptNoteItem.OrderQuantity / garmentUnitReceiptNoteItem.CorrectionConversion))* garmentUnitReceiptNoteItem.CorrectionConversion;
                            //decimal newQty = (garmentUnitReceiptNoteItem.ReceiptCorrection - (garmentUnitReceiptNoteItem.OrderQuantity / garmentUnitReceiptNoteItem.CorrectionConversion)) * (decimal)item.CorrectionConversion;
                            //decimal diff = (newQty - qty)/(decimal)item.CorrectionConversion;
                            item.SmallQuantity =(double) garmentUnitReceiptNoteItem.ReceiptCorrection * item.CorrectionConversion;
                            //garmentUnitReceiptNoteItem.ReceiptCorrection += diff;
                            garmentUnitReceiptNoteItem.CorrectionConversion = (decimal)item.CorrectionConversion;

                            if (garmentDOItems != null)
                                garmentDOItems.RemainingQuantity = garmentDOItems.RemainingQuantity + ((garmentUnitReceiptNoteItem.CorrectionConversion * garmentUnitReceiptNoteItem.ReceiptCorrection) - garmentDOItems.RemainingQuantity);
                        }
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);

                        var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == item.ProductId && s.StorageId == m.StorageId && s.UomId == item.SmallUomId);
                        
                        
                        var garmentInventoryMovement = GenerateGarmentInventoryMovement(m, item, garmentInventorySummaryExisting,type);
                        dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                        if (garmentInventorySummaryExisting == null)
                        {
                            var garmentInventorySummary = GenerateGarmentInventorySummary(m, item, garmentInventoryMovement);

                            dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                        }
                        else
                        {
                            EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                        }

                        

                        await dbContext.SaveChangesAsync();
                    }

                    if(m.CorrectionType != "Jumlah")
                    {
                        var garmentInventoryDocumentConv = GenerateGarmentInventoryDocumentConv(m, type);
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentConv);
                    }

                    this.dbSet.Add(m);

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

        async Task<string> GenerateNo(GarmentReceiptCorrection model, int clientTimeZoneOffset)
        {
            DateTimeOffset dateTimeOffsetNow = DateTimeOffset.Now;
            string Month = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Year = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Day= dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd");

            string no = $"CB{Year}{Month}{Day}";
            int Padding = 4;

            var lastNo = await this.dbSet.Where(w => w.CorrectionNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.CorrectionNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                //int lastNoNumber = Int32.Parse(lastNo.INNo.Replace(no, "")) + 1;
                int.TryParse(lastNo.CorrectionNo.Replace(no, ""), out int lastno1);
                int lastNoNumber = lastno1 + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');

            }
        }


        private GarmentInventorySummary GenerateGarmentInventorySummary(GarmentReceiptCorrection garmentReceiptCorrection, GarmentReceiptCorrectionItem garmentReceiptCorrectionItem, GarmentInventoryMovement garmentInventoryMovement)
        {
            var garmentInventorySummary = new GarmentInventorySummary();
            EntityExtension.FlagForCreate(garmentInventorySummary, identityService.Username, USER_AGENT);
            do
            {
                garmentInventorySummary.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventorySummary.Any(m => m.No == garmentInventorySummary.No));

            garmentInventorySummary.ProductId = garmentReceiptCorrectionItem.ProductId;
            garmentInventorySummary.ProductCode = garmentReceiptCorrectionItem.ProductCode;
            garmentInventorySummary.ProductName = garmentReceiptCorrectionItem.ProductName;

            garmentInventorySummary.StorageId = garmentReceiptCorrection.StorageId;
            garmentInventorySummary.StorageCode = garmentReceiptCorrection.StorageCode;
            garmentInventorySummary.StorageName = garmentReceiptCorrection.StorageName;

            garmentInventorySummary.Quantity = garmentInventoryMovement.After;

            garmentInventorySummary.UomId = garmentReceiptCorrectionItem.SmallUomId;
            garmentInventorySummary.UomUnit = garmentReceiptCorrectionItem.SmallUomUnit;

            garmentInventorySummary.StockPlanning = 0;

            return garmentInventorySummary;
        }

        private GarmentInventoryMovement GenerateGarmentInventoryMovement(GarmentReceiptCorrection garmentReceiptCorrection, GarmentReceiptCorrectionItem garmentReceiptCorrectionItem, GarmentInventorySummary garmentInventorySummary, string type )
        {
            var garmentInventoryMovement = new GarmentInventoryMovement();
            EntityExtension.FlagForCreate(garmentInventoryMovement, identityService.Username, USER_AGENT);
            do
            {
                garmentInventoryMovement.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventoryMovement.Any(m => m.No == garmentInventoryMovement.No));

            garmentInventoryMovement.Date = garmentInventoryMovement.CreatedUtc;

            garmentInventoryMovement.ReferenceNo = garmentReceiptCorrection.CorrectionNo;
            garmentInventoryMovement.ReferenceType = string.Concat("Koreksi Bon - ", garmentReceiptCorrection.UnitName);

            garmentInventoryMovement.ProductId = garmentReceiptCorrectionItem.ProductId;
            garmentInventoryMovement.ProductCode = garmentReceiptCorrectionItem.ProductCode;
            garmentInventoryMovement.ProductName = garmentReceiptCorrectionItem.ProductName;

            garmentInventoryMovement.StorageId = garmentReceiptCorrection.StorageId;
            garmentInventoryMovement.StorageCode = garmentReceiptCorrection.StorageCode;
            garmentInventoryMovement.StorageName = garmentReceiptCorrection.StorageName;

            garmentInventoryMovement.StockPlanning = 0;

            garmentInventoryMovement.Before = garmentInventorySummary == null ? 0 : garmentInventorySummary.Quantity;
            garmentInventoryMovement.Quantity = (type ?? "").ToUpper() == "OUT" && garmentReceiptCorrectionItem.SmallQuantity>0 ? (decimal)garmentReceiptCorrectionItem.SmallQuantity*(-1) : (decimal)garmentReceiptCorrectionItem.SmallQuantity;
            garmentInventoryMovement.After = garmentInventoryMovement.Before + garmentInventoryMovement.Quantity;

            garmentInventoryMovement.UomId = garmentReceiptCorrectionItem.SmallUomId;
            garmentInventoryMovement.UomUnit = garmentReceiptCorrectionItem.SmallUomUnit;

            garmentInventoryMovement.Remark = garmentReceiptCorrectionItem.ProductRemark;

            garmentInventoryMovement.Type = type;

            return garmentInventoryMovement;
        }

        private GarmentInventoryDocument GenerateGarmentInventoryDocument(GarmentReceiptCorrection garmentReceiptCorrection, GarmentReceiptCorrectionItem garmentReceiptCorrectionItem, string type)
        {
            var garmentInventoryDocument = new GarmentInventoryDocument
            {
                Items = new List<GarmentInventoryDocumentItem>()
            };
            EntityExtension.FlagForCreate(garmentInventoryDocument, identityService.Username, USER_AGENT);
            do
            {
                garmentInventoryDocument.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventoryDocument.Any(m => m.No == garmentInventoryDocument.No));

            garmentInventoryDocument.ReferenceNo = garmentReceiptCorrection.CorrectionNo;
            garmentInventoryDocument.ReferenceType = string.Concat("Koreksi Bon - ", garmentReceiptCorrection.UnitName);

            garmentInventoryDocument.Type = type;
            garmentInventoryDocument.Date = DateTime.Now;
            garmentInventoryDocument.StorageId = garmentReceiptCorrection.StorageId;
            garmentInventoryDocument.StorageCode = garmentReceiptCorrection.StorageCode;
            garmentInventoryDocument.StorageName = garmentReceiptCorrection.StorageName;

            garmentInventoryDocument.Remark = garmentReceiptCorrection.Remark;

            //1 item 1 doc
            var garmentInventoryDocumentItem = new GarmentInventoryDocumentItem();
            EntityExtension.FlagForCreate(garmentInventoryDocumentItem, identityService.Username, USER_AGENT);

            garmentInventoryDocumentItem.ProductId = garmentReceiptCorrectionItem.ProductId;
            garmentInventoryDocumentItem.ProductCode = garmentReceiptCorrectionItem.ProductCode;
            garmentInventoryDocumentItem.ProductName = garmentReceiptCorrectionItem.ProductName;

            garmentInventoryDocumentItem.Quantity = garmentReceiptCorrectionItem.SmallQuantity >= 0 ? (decimal)garmentReceiptCorrectionItem.SmallQuantity : (decimal)garmentReceiptCorrectionItem.SmallQuantity * (-1);

            garmentInventoryDocumentItem.UomId = garmentReceiptCorrectionItem.SmallUomId;
            garmentInventoryDocumentItem.UomUnit = garmentReceiptCorrectionItem.SmallUomUnit;

            garmentInventoryDocumentItem.ProductRemark = garmentReceiptCorrectionItem.ProductRemark;

            garmentInventoryDocument.Items.Add(garmentInventoryDocumentItem);

            //foreach (var garmentReceiptCorrectionItem in garmentReceiptCorrection.Items)
            //{
            //    var garmentInventoryDocumentItem = new GarmentInventoryDocumentItem();
            //    EntityExtension.FlagForCreate(garmentInventoryDocumentItem, identityService.Username, USER_AGENT);

            //    garmentInventoryDocumentItem.ProductId = garmentReceiptCorrectionItem.ProductId;
            //    garmentInventoryDocumentItem.ProductCode = garmentReceiptCorrectionItem.ProductCode;
            //    garmentInventoryDocumentItem.ProductName = garmentReceiptCorrectionItem.ProductName;

            //    garmentInventoryDocumentItem.Quantity = (type ?? "").ToUpper() == "IN" ? (decimal) garmentReceiptCorrectionItem.SmallQuantity : (decimal)garmentReceiptCorrectionItem.SmallQuantity*(-1);

            //    garmentInventoryDocumentItem.UomId = garmentReceiptCorrectionItem.SmallUomId;
            //    garmentInventoryDocumentItem.UomUnit = garmentReceiptCorrectionItem.SmallUomUnit;

            //    garmentInventoryDocumentItem.ProductRemark = garmentReceiptCorrectionItem.ProductRemark;

            //    garmentInventoryDocument.Items.Add(garmentInventoryDocumentItem);
            //}

            return garmentInventoryDocument;
        }

        private GarmentInventoryDocument GenerateGarmentInventoryDocumentConv(GarmentReceiptCorrection garmentReceiptCorrection, string type)
        {
            var garmentInventoryDocument = new GarmentInventoryDocument
            {
                Items = new List<GarmentInventoryDocumentItem>()
            };
            EntityExtension.FlagForCreate(garmentInventoryDocument, identityService.Username, USER_AGENT);
            do
            {
                garmentInventoryDocument.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventoryDocument.Any(m => m.No == garmentInventoryDocument.No));

            garmentInventoryDocument.ReferenceNo = garmentReceiptCorrection.CorrectionNo;
            garmentInventoryDocument.ReferenceType = string.Concat("Koreksi Bon - ", garmentReceiptCorrection.UnitName);

            garmentInventoryDocument.Type = type;
            garmentInventoryDocument.Date = DateTime.Now;
            garmentInventoryDocument.StorageId = garmentReceiptCorrection.StorageId;
            garmentInventoryDocument.StorageCode = garmentReceiptCorrection.StorageCode;
            garmentInventoryDocument.StorageName = garmentReceiptCorrection.StorageName;

            garmentInventoryDocument.Remark = garmentReceiptCorrection.Remark;

            foreach (var garmentReceiptCorrectionItem in garmentReceiptCorrection.Items)
            {
                var garmentInventoryDocumentItem = new GarmentInventoryDocumentItem();
                EntityExtension.FlagForCreate(garmentInventoryDocumentItem, identityService.Username, USER_AGENT);

                garmentInventoryDocumentItem.ProductId = garmentReceiptCorrectionItem.ProductId;
                garmentInventoryDocumentItem.ProductCode = garmentReceiptCorrectionItem.ProductCode;
                garmentInventoryDocumentItem.ProductName = garmentReceiptCorrectionItem.ProductName;

                garmentInventoryDocumentItem.Quantity = garmentReceiptCorrectionItem.SmallQuantity>=0 ? (decimal)garmentReceiptCorrectionItem.SmallQuantity : (decimal)garmentReceiptCorrectionItem.SmallQuantity * (-1);

                garmentInventoryDocumentItem.UomId = garmentReceiptCorrectionItem.SmallUomId;
                garmentInventoryDocumentItem.UomUnit = garmentReceiptCorrectionItem.SmallUomUnit;

                garmentInventoryDocumentItem.ProductRemark = garmentReceiptCorrectionItem.ProductRemark;

                garmentInventoryDocument.Items.Add(garmentInventoryDocumentItem);
            }

            return garmentInventoryDocument;
        }
    }
}
