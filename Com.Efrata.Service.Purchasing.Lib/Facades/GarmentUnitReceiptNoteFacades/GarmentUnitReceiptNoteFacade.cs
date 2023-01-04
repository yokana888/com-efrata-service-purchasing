using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentUnitReceiptNotePDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using System.Data;
using System.Globalization;
using System.Net.Http;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUenUrnChangeDateHistory;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades
{
    public class GarmentUnitReceiptNoteFacade : IGarmentUnitReceiptNoteFacade
    {
        private readonly string USER_AGENT = "Facade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentUnitReceiptNote> dbSet;
        private readonly DbSet<GarmentDeliveryOrderDetail> dbSetGarmentDeliveryOrderDetail;
        private readonly DbSet<GarmentExternalPurchaseOrder> dbSetGarmentExternalPurchaseOrder;
        private readonly DbSet<GarmentExternalPurchaseOrderItem> dbSetGarmentExternalPurchaseOrderItems;
        private readonly DbSet<GarmentInternalPurchaseOrderItem> dbSetGarmentInternalPurchaseOrderItems;
        private readonly DbSet<GarmentInventoryDocument> dbSetGarmentInventoryDocument;
        private readonly DbSet<GarmentInventoryMovement> dbSetGarmentInventoryMovement;
        private readonly DbSet<GarmentInventorySummary> dbSetGarmentInventorySummary;
        private readonly DbSet<GarmentDeliveryOrder> dbsetGarmentDeliveryOrder;
        private readonly DbSet<GarmentUnitDeliveryOrder> dbSetGarmentUnitDeliveryOrder;
        private readonly DbSet<GarmentUnitExpenditureNote> dbSetGarmentUnitExpenditureNote;
        private readonly DbSet<GarmentDOItems> dbSetGarmentDOItems;
        private readonly DbSet<GarmentUenUrnChangeDateHistory> dbSetUenUrnChangeDate;
        private readonly DbSet<GarmentBeacukai> dbSetBC;
        private readonly DbSet<GarmentBeacukaiItem> dbSetBCI;

        private readonly IMapper mapper;

        public GarmentUnitReceiptNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentUnitReceiptNote>();
            dbSetGarmentDeliveryOrderDetail = dbContext.Set<GarmentDeliveryOrderDetail>();
            dbSetGarmentExternalPurchaseOrder = dbContext.Set<GarmentExternalPurchaseOrder>();
            dbSetGarmentExternalPurchaseOrderItems = dbContext.Set<GarmentExternalPurchaseOrderItem>();
            dbSetGarmentInternalPurchaseOrderItems = dbContext.Set<GarmentInternalPurchaseOrderItem>();
            dbSetGarmentInventoryDocument = dbContext.Set<GarmentInventoryDocument>();
            dbSetGarmentInventoryMovement = dbContext.Set<GarmentInventoryMovement>();
            dbSetGarmentInventorySummary = dbContext.Set<GarmentInventorySummary>();
            dbsetGarmentDeliveryOrder = dbContext.Set<GarmentDeliveryOrder>();
            dbSetGarmentUnitDeliveryOrder = dbContext.Set<GarmentUnitDeliveryOrder>();
            dbSetGarmentUnitExpenditureNote = dbContext.Set<GarmentUnitExpenditureNote>();
            dbSetGarmentDOItems = dbContext.Set<GarmentDOItems>();
            dbSetUenUrnChangeDate = dbContext.Set<GarmentUenUrnChangeDateHistory>();
            dbSetBC = dbContext.Set<GarmentBeacukai>();
            dbSetBCI = dbContext.Set<GarmentBeacukaiItem>();

            mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitReceiptNote> Query = dbSet;
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentUnitReceiptNote>.ConfigureFilter(Query, FilterDictionary);

            Query = Query.Select(m => new GarmentUnitReceiptNote
            {
                Id = m.Id,
                URNNo = m.URNNo,
                UnitCode = m.UnitCode,
                UnitId = m.UnitId,
                UnitName = m.UnitName,
                ReceiptDate = m.ReceiptDate,
                SupplierName = m.SupplierName,
                DONo = m.DONo,
                DOId = m.DOId,
                StorageName = m.StorageName,
                StorageId = m.StorageId,
                StorageCode = m.StorageCode,
                DRNo = m.DRNo,
                URNType = m.URNType,
                UENNo = m.UENNo,
                DOCurrencyRate = m.DOCurrencyRate,
                Items = m.Items.Select(i => new GarmentUnitReceiptNoteItem
                {
                    Id = i.Id,
                    RONo = i.RONo,
                    ProductCode = i.ProductCode,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductRemark = i.ProductRemark,
                    OrderQuantity = i.OrderQuantity,
                    ReceiptQuantity = i.ReceiptQuantity,
                    SmallQuantity = i.SmallQuantity,
                    UomId = i.UomId,
                    UomUnit = i.UomUnit,
                    Conversion = i.Conversion,
                    DODetailId = i.DODetailId,
                    EPOItemId = i.EPOItemId,
                    POItemId = i.POItemId,
                    PRItemId = i.PRItemId,
                    POSerialNumber = i.POSerialNumber,
                    SmallUomId = i.SmallUomId,
                    SmallUomUnit = i.SmallUomUnit,
                    PricePerDealUnit = i.PricePerDealUnit,
                    DesignColor = i.DesignColor,
                    ReceiptCorrection = i.ReceiptCorrection,
                    CorrectionConversion = i.CorrectionConversion
                }).ToList(),
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc,
                CreatedUtc = m.CreatedUtc
            });

            List<string> searchAttributes = new List<string>()
            {
                "URNNo", "UnitName", "SupplierName", "DONo","URNType", "DRNo", "UENNo"
            };

            Query = QueryHelper<GarmentUnitReceiptNote>.ConfigureSearch(Query, searchAttributes, Keyword);



            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentUnitReceiptNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentUnitReceiptNote> pageable = new Pageable<GarmentUnitReceiptNote>(Query, Page - 1, Size);
            List<GarmentUnitReceiptNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(Data.Select(s => new
            {
                s.Id,
                s.URNNo,
                s.DOId,
                s.DRNo,
                s.URNType,
                s.UENNo,
                Unit = new { Name = s.UnitName, Id = s.UnitId, Code = s.UnitCode },
                Storage = new { name = s.StorageName, _id = s.StorageId, code = s.StorageCode },
                s.ReceiptDate,
                Supplier = new { Name = s.SupplierName },
                s.DONo,
                s.DOCurrencyRate,
                Items = new List<GarmentUnitReceiptNoteItem>(s.Items),
                s.CreatedBy,
                s.LastModifiedUtc,
                s.CreatedUtc
            }));

            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public GarmentUnitReceiptNoteViewModel ReadById(int id)
        {
            var model = dbSet.IgnoreQueryFilters().Where(m => m.Id == id && ((m.IsDeleted == true && m.DeletedAgent == "LUCIA") || (m.IsDeleted == false)))
                            .Include(m => m.Items)
                            .FirstOrDefault();
            var viewModel = mapper.Map<GarmentUnitReceiptNoteViewModel>(model);

            viewModel.IsInvoice = dbContext.GarmentDeliveryOrders.Where(gdo => gdo.Id == viewModel.DOId).Select(gdo => gdo.IsInvoice).FirstOrDefault();

            var dataDo = dbsetGarmentDeliveryOrder.Where(gdo => gdo.Id == viewModel.DOId).Include(x => x.Items).FirstOrDefault();
            long epoId = 0;
            if(dataDo != null)
            {
                epoId = dataDo.Items.Select(x => x.EPOId).FirstOrDefault();
            }

            foreach (var item in viewModel.Items)
            {
                if (epoId > 0)
                {
                    item.PaymentType = dbSetGarmentExternalPurchaseOrder.Where(x => x.Id == epoId).FirstOrDefault().PaymentType;
                    item.PaymentMethod = dbSetGarmentExternalPurchaseOrder.Where(x => x.Id == epoId).FirstOrDefault().PaymentMethod;
                }
                item.Buyer = new BuyerViewModel
                {
                    Name = dbContext.GarmentPurchaseRequests.Where(m => m.Id == item.PRId).Select(m => m.BuyerName).FirstOrDefault()
                };
                item.Article = dbContext.GarmentExternalPurchaseOrderItems.Where(m => m.Id == item.EPOItemId).Select(m => m.Article).FirstOrDefault();
            }

            return viewModel;
        }

        public GarmentDOItems ReadDOItemsByURNItemId(int id)
        {
            var model = dbSetGarmentDOItems.IgnoreQueryFilters().Where(i => i.URNItemId == id && ((i.IsDeleted == true && i.DeletedAgent == "LUCIA") || (i.IsDeleted == false)))
                            .FirstOrDefault();

            return model;
        }

        public MemoryStream GeneratePdf(GarmentUnitReceiptNoteViewModel garmentUnitReceiptNote)
        {
            return GarmentUnitReceiptNotePDFTemplate.GeneratePdfTemplate(serviceProvider, garmentUnitReceiptNote);
        }

        public async Task<int> Create(GarmentUnitReceiptNote garmentUnitReceiptNote)
        {
            int Created = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(garmentUnitReceiptNote, identityService.Username, USER_AGENT);
                    garmentUnitReceiptNote.URNNo = await GenerateNo(garmentUnitReceiptNote);
                    garmentUnitReceiptNote.IsStorage = true;

                    Dictionary<long, double> doCurrencies = new Dictionary<long, double>();

                    if (garmentUnitReceiptNote.URNType == "PEMBELIAN")
                    {
                        var garmentDeliveryOrder = dbsetGarmentDeliveryOrder.First(d => d.Id == garmentUnitReceiptNote.DOId);
                        garmentUnitReceiptNote.DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate;
                    }

                    if (garmentUnitReceiptNote.DOId>0)
                    {
                        var garmentDeliveryOrderUpdate = dbsetGarmentDeliveryOrder.First(d => d.Id == garmentUnitReceiptNote.DOId);
                        garmentDeliveryOrderUpdate.IsCustoms = true;
                        EntityExtension.FlagForUpdate(garmentDeliveryOrderUpdate, identityService.Username, USER_AGENT);
                    }

                    foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                    {
                        if (garmentUnitReceiptNote.URNType == "GUDANG SISA")
                        {
                            var doDetail = dbSetGarmentDeliveryOrderDetail.IgnoreQueryFilters().Where(i => (i.IsDeleted == true && i.DeletedAgent == "LUCIA") || (i.IsDeleted == false)).OrderByDescending(a => a.DOQuantity).FirstOrDefault(a => a.POSerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.DODetailId = doDetail == null ? 0 : doDetail.Id;

                            var epoItem = dbSetGarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(i => (i.IsDeleted == true && i.DeletedAgent == "LUCIA") || (i.IsDeleted == false)).First(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.PRId = epoItem.PRId;
                            garmentUnitReceiptNoteItem.EPOItemId = epoItem.Id;
                            garmentUnitReceiptNoteItem.PRNo = epoItem.PRNo;
                            garmentUnitReceiptNoteItem.RONo = epoItem.RONo;
                            garmentUnitReceiptNoteItem.POId = epoItem.POId;

                            var poItem = dbSetGarmentInternalPurchaseOrderItems.FirstOrDefault(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.POItemId = poItem == null ? 0 : poItem.Id;

                            var prItem = dbContext.GarmentPurchaseRequestItems.FirstOrDefault(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.PRItemId = prItem == null ? 0 : prItem.Id;

                        }
                        else if(garmentUnitReceiptNote.URNType == "SISA SUBCON")
                        {
                            var doDetail = dbSetGarmentDeliveryOrderDetail.IgnoreQueryFilters().Where(i => (i.IsDeleted == true && i.DeletedAgent == "LUCIA") || (i.IsDeleted == false)).OrderByDescending(a => a.DOQuantity).FirstOrDefault(a => a.POSerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.DODetailId = doDetail == null ? 0 : doDetail.Id;

                            var epoItem = dbSetGarmentExternalPurchaseOrderItems.IgnoreQueryFilters().Where(i => (i.IsDeleted == true && i.DeletedAgent == "LUCIA") || (i.IsDeleted == false)).First(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.PRId = epoItem.PRId;
                            garmentUnitReceiptNoteItem.EPOItemId = epoItem.Id;
                            garmentUnitReceiptNoteItem.PRNo = epoItem.PRNo;
                            garmentUnitReceiptNoteItem.RONo = epoItem.RONo;
                            garmentUnitReceiptNoteItem.POId = epoItem.POId;

                            var poItem = dbSetGarmentInternalPurchaseOrderItems.FirstOrDefault(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.POItemId = poItem == null ? 0 : poItem.Id;

                            var prItem = dbContext.GarmentPurchaseRequestItems.FirstOrDefault(a => a.PO_SerialNumber == garmentUnitReceiptNoteItem.POSerialNumber);
                            garmentUnitReceiptNoteItem.PRItemId = prItem == null ? 0 : prItem.Id;
                            
                        }

                        garmentUnitReceiptNoteItem.DOCurrencyRate = garmentUnitReceiptNote.DOCurrencyRate != null && garmentUnitReceiptNote.URNType == "PEMBELIAN" ?
                        (double)garmentUnitReceiptNote.DOCurrencyRate : garmentUnitReceiptNoteItem.DOCurrencyRate;
                        if (garmentUnitReceiptNoteItem.DOCurrencyRate == 0)
                        {
                            throw new Exception("DOCurrencyRate tidak boleh 0");
                        }
                        garmentUnitReceiptNoteItem.CorrectionConversion = garmentUnitReceiptNoteItem.Conversion;
                        EntityExtension.FlagForCreate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                        garmentUnitReceiptNoteItem.ReceiptCorrection = garmentUnitReceiptNoteItem.ReceiptQuantity;


                        //*per 10-06-21 yg PROSES tidak update receiptqty di DO

                        if (garmentUnitReceiptNote.URNType == "PEMBELIAN")
                        {
                            var garmentDeliveryOrderDetail = dbSetGarmentDeliveryOrderDetail.First(d => d.Id == garmentUnitReceiptNoteItem.DODetailId);
                            EntityExtension.FlagForUpdate(garmentDeliveryOrderDetail, identityService.Username, USER_AGENT);
                            garmentDeliveryOrderDetail.ReceiptQuantity = (double)((decimal)garmentDeliveryOrderDetail.ReceiptQuantity + garmentUnitReceiptNoteItem.ReceiptQuantity);

                            var garmentExternalPurchaseOrderItem = dbSetGarmentExternalPurchaseOrderItems.First(d => d.Id == garmentUnitReceiptNoteItem.EPOItemId);
                            EntityExtension.FlagForUpdate(garmentExternalPurchaseOrderItem, identityService.Username, USER_AGENT);
                            garmentExternalPurchaseOrderItem.ReceiptQuantity = (double)((decimal)garmentExternalPurchaseOrderItem.ReceiptQuantity + garmentUnitReceiptNoteItem.ReceiptQuantity);

                            if (garmentUnitReceiptNoteItem.POItemId != 0)
                            {
                                var garmentInternalPurchaseOrderItem = dbSetGarmentInternalPurchaseOrderItems.First(d => d.Id == garmentUnitReceiptNoteItem.POItemId);
                                EntityExtension.FlagForUpdate(garmentInternalPurchaseOrderItem, identityService.Username, USER_AGENT);
                                garmentInternalPurchaseOrderItem.Status = "Barang sudah diterima Unit";
                            }
                        }

                        var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitReceiptNoteItem.ProductId && s.StorageId == garmentUnitReceiptNote.StorageId && s.UomId == garmentUnitReceiptNoteItem.SmallUomId);

                        var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitReceiptNote, garmentUnitReceiptNoteItem, garmentInventorySummaryExisting);
                        dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                        if (garmentInventorySummaryExisting == null)
                        {
                            var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitReceiptNote, garmentUnitReceiptNoteItem, garmentInventoryMovement);
                            dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                        }
                        else
                        {
                            EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                        }

                        await dbContext.SaveChangesAsync();
                    }


                    if (garmentUnitReceiptNote.URNType == "PROSES")
                    {
                        await UpdateDR(garmentUnitReceiptNote.DRId, true, garmentUnitReceiptNote.UnitCode);
                    }
                    else if (garmentUnitReceiptNote.URNType == "GUDANG SISA")
                    {
                        await UpdateExpenditure(garmentUnitReceiptNote.ExpenditureId, true, garmentUnitReceiptNote.Category);
                    }
                    else if(garmentUnitReceiptNote.URNType == "SISA SUBCON")
                    {
                        var garmentUnitExpenditureNote = dbContext.GarmentUnitExpenditureNotes.Single(a => a.Id == garmentUnitReceiptNote.UENId);
                        garmentUnitExpenditureNote.IsReceived = true;
                    }

                    var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitReceiptNote);
                    dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                    dbSet.Add(garmentUnitReceiptNote);
                    Created = await dbContext.SaveChangesAsync();


                    if (garmentUnitReceiptNote.URNType == "PEMBELIAN" || garmentUnitReceiptNote.URNType == "GUDANG SISA" || garmentUnitReceiptNote.URNType == "SISA SUBCON")
                    {
                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentDOItems garmentDOItems = new GarmentDOItems
                            {
                                DOItemNo = await GenerateNoDOItems(garmentUnitReceiptNote),
                                UnitId = garmentUnitReceiptNote.UnitId,
                                UnitCode = garmentUnitReceiptNote.UnitCode,
                                UnitName = garmentUnitReceiptNote.UnitName,
                                StorageCode = garmentUnitReceiptNote.StorageCode,
                                StorageId = garmentUnitReceiptNote.StorageId,
                                StorageName = garmentUnitReceiptNote.StorageName,
                                POId = garmentUnitReceiptNoteItem.POId,
                                POItemId = garmentUnitReceiptNoteItem.POItemId,
                                POSerialNumber = garmentUnitReceiptNoteItem.POSerialNumber,
                                ProductCode = garmentUnitReceiptNoteItem.ProductCode,
                                ProductId = garmentUnitReceiptNoteItem.ProductId,
                                ProductName = garmentUnitReceiptNoteItem.ProductName,
                                DesignColor = garmentUnitReceiptNoteItem.DesignColor,
                                SmallQuantity = garmentUnitReceiptNoteItem.SmallQuantity,
                                SmallUomId = garmentUnitReceiptNoteItem.SmallUomId,
                                SmallUomUnit = garmentUnitReceiptNoteItem.SmallUomUnit,
                                RemainingQuantity = garmentUnitReceiptNoteItem.SmallQuantity,
                                DetailReferenceId = garmentUnitReceiptNoteItem.DODetailId,
                                URNItemId = garmentUnitReceiptNoteItem.Id,
                                DOCurrencyRate = garmentUnitReceiptNoteItem.DOCurrencyRate,
                                EPOItemId = garmentUnitReceiptNoteItem.EPOItemId,
                                PRItemId = garmentUnitReceiptNoteItem.PRItemId,
                                RO = garmentUnitReceiptNoteItem.RONo,
                                CustomsCategory= garmentUnitReceiptNoteItem.CustomsCategory
                            };
                            EntityExtension.FlagForCreate(garmentDOItems, identityService.Username, USER_AGENT);
                            dbSetGarmentDOItems.Add(garmentDOItems);
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    if (garmentUnitReceiptNote.URNType == "PROSES")
                    {
                        //await UpdateDR(garmentUnitReceiptNote.DRId, true);
                        var GarmentDR = GetDR(garmentUnitReceiptNote.DRId, garmentUnitReceiptNote.UnitCode);
                        var GarmentUnitDO = dbContext.GarmentUnitDeliveryOrders.AsNoTracking().Single(a => a.Id == GarmentDR.UnitDOId);
                        List<GarmentUnitDeliveryOrderItem> unitDOItems = new List<GarmentUnitDeliveryOrderItem>();
                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentDOItems garmentDOItems = new GarmentDOItems
                            {
                                DOItemNo = await GenerateNoDOItems(garmentUnitReceiptNote),
                                UnitId = garmentUnitReceiptNote.UnitId,
                                UnitCode = garmentUnitReceiptNote.UnitCode,
                                UnitName = garmentUnitReceiptNote.UnitName,
                                StorageCode = garmentUnitReceiptNote.StorageCode,
                                StorageId = garmentUnitReceiptNote.StorageId,
                                StorageName = garmentUnitReceiptNote.StorageName,
                                POId = garmentUnitReceiptNoteItem.POId,
                                POItemId = garmentUnitReceiptNoteItem.POItemId,
                                POSerialNumber = garmentUnitReceiptNoteItem.POSerialNumber,
                                ProductCode = garmentUnitReceiptNoteItem.ProductCode,
                                ProductId = garmentUnitReceiptNoteItem.ProductId,
                                ProductName = garmentUnitReceiptNoteItem.ProductName,
                                DesignColor = garmentUnitReceiptNoteItem.DesignColor,
                                SmallQuantity = garmentUnitReceiptNoteItem.SmallQuantity,
                                SmallUomId = garmentUnitReceiptNoteItem.SmallUomId,
                                SmallUomUnit = garmentUnitReceiptNoteItem.SmallUomUnit,
                                RemainingQuantity = GarmentUnitDO.UnitDOFromId != 0 ? 0 : garmentUnitReceiptNoteItem.SmallQuantity,
                                DetailReferenceId = garmentUnitReceiptNoteItem.DODetailId,
                                URNItemId = garmentUnitReceiptNoteItem.Id,
                                DOCurrencyRate = garmentUnitReceiptNoteItem.DOCurrencyRate,
                                EPOItemId = garmentUnitReceiptNoteItem.EPOItemId,
                                PRItemId = garmentUnitReceiptNoteItem.PRItemId,
                                RO = garmentUnitReceiptNoteItem.RONo,

                            };
                            EntityExtension.FlagForCreate(garmentDOItems, identityService.Username, USER_AGENT);
                            dbSetGarmentDOItems.Add(garmentDOItems);
                            await dbContext.SaveChangesAsync();
                        }

                        if (GarmentUnitDO.UnitDOFromId != 0)
                        {
                            GarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(dbContext, serviceProvider);
                            GarmentUnitExpenditureNoteFacade.GarmentUnitExpenditureNoteFacade garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade.GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);
                            var GarmentUnitDOFrom = dbContext.GarmentUnitDeliveryOrders.AsNoTracking().Single(a => a.Id == GarmentUnitDO.UnitDOFromId);
                            foreach (var item in garmentUnitReceiptNote.Items)
                            {
                                GarmentUnitDeliveryOrderItem garmentUnitDeliveryOrderItem = new GarmentUnitDeliveryOrderItem
                                {
                                    URNId = garmentUnitReceiptNote.Id,
                                    URNNo = garmentUnitReceiptNote.URNNo,
                                    URNItemId = item.Id,
                                    DODetailId = item.DODetailId,
                                    EPOItemId = item.EPOItemId,
                                    POItemId = item.POItemId,
                                    POSerialNumber = item.POSerialNumber,
                                    PRItemId = item.PRItemId,
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    ProductRemark = item.ProductRemark,
                                    RONo = item.RONo,
                                    Quantity = (double)item.SmallQuantity,
                                    UomId = item.SmallUomId,
                                    UomUnit = item.SmallUomUnit,
                                    PricePerDealUnit = (double)item.PricePerDealUnit,
                                    DesignColor = item.DesignColor,
                                    DefaultDOQuantity = (double)item.SmallQuantity,
                                    DOCurrencyRate = item.DOCurrencyRate,
                                    ReturQuantity = 0
                                };
                                unitDOItems.Add(garmentUnitDeliveryOrderItem);
                                EntityExtension.FlagForCreate(garmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);
                            }
                            var rono = garmentUnitReceiptNote.Items.First().RONo;
                            var pr = dbContext.GarmentPurchaseRequests.AsNoTracking().FirstOrDefault(p => p.RONo == rono);
                            GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
                            {
                                UnitDOType = "TRANSFER",
                                UnitDODate = garmentUnitReceiptNote.ReceiptDate,
                                UnitRequestCode = GarmentUnitDOFrom.UnitSenderCode,
                                UnitRequestId = GarmentUnitDOFrom.UnitSenderId,
                                UnitRequestName = GarmentUnitDOFrom.UnitSenderName,
                                UnitSenderId = garmentUnitReceiptNote.UnitId,
                                UnitSenderName = garmentUnitReceiptNote.UnitName,
                                UnitSenderCode = garmentUnitReceiptNote.UnitCode,
                                StorageId = garmentUnitReceiptNote.StorageId,
                                StorageCode = garmentUnitReceiptNote.StorageCode,
                                StorageName = garmentUnitReceiptNote.StorageName,
                                RONo = rono,
                                Article = pr.Article,
                                IsUsed = true,
                                StorageRequestCode = GarmentUnitDOFrom.StorageCode,
                                StorageRequestId = GarmentUnitDOFrom.StorageId,
                                StorageRequestName = GarmentUnitDOFrom.StorageName,
                                Items = unitDOItems
                            };
                            garmentUnitDeliveryOrder.UnitDONo = await garmentUnitDeliveryOrderFacade.GenerateNo(garmentUnitDeliveryOrder);
                            EntityExtension.FlagForCreate(garmentUnitDeliveryOrder, identityService.Username, USER_AGENT);

                            dbSetGarmentUnitDeliveryOrder.Add(garmentUnitDeliveryOrder);
                            await dbContext.SaveChangesAsync();

                            List<GarmentUnitExpenditureNoteItem> uenItems = new List<GarmentUnitExpenditureNoteItem>();
                            foreach (var unitDOItem in garmentUnitDeliveryOrder.Items)
                            {
                                var poItem = dbContext.GarmentInternalPurchaseOrderItems.AsNoTracking().Single(a => a.Id == unitDOItem.POItemId);
                                var po = dbContext.GarmentInternalPurchaseOrders.AsNoTracking().Single(a => a.Id == poItem.GPOId);
                                var urnItem = dbContext.GarmentUnitReceiptNoteItems.AsNoTracking().Single(a => a.Id == unitDOItem.URNItemId);
                                GarmentUnitExpenditureNoteItem garmentUnitExpenditureNoteItem = new GarmentUnitExpenditureNoteItem
                                {
                                    UnitDOItemId = unitDOItem.Id,
                                    URNItemId = unitDOItem.URNItemId,
                                    DODetailId = unitDOItem.DODetailId,
                                    EPOItemId = unitDOItem.EPOItemId,
                                    POItemId = unitDOItem.POItemId,
                                    PRItemId = unitDOItem.PRItemId,
                                    POSerialNumber = unitDOItem.POSerialNumber,
                                    ProductId = unitDOItem.ProductId,
                                    ProductName = unitDOItem.ProductName,
                                    ProductCode = unitDOItem.ProductCode,
                                    ProductRemark = unitDOItem.ProductRemark,
                                    RONo = unitDOItem.RONo,
                                    Quantity = unitDOItem.Quantity,
                                    UomId = unitDOItem.UomId,
                                    UomUnit = unitDOItem.UomUnit,
                                    PricePerDealUnit = unitDOItem.PricePerDealUnit,
                                    FabricType = unitDOItem.FabricType,
                                    BuyerId = Convert.ToInt64(po.BuyerId),
                                    BuyerCode = po.BuyerCode,
                                    BasicPrice = (decimal)(unitDOItem.PricePerDealUnit * unitDOItem.DOCurrencyRate),
                                    Conversion = urnItem.Conversion,
                                    ReturQuantity = 0,
                                    DOCurrencyRate = unitDOItem.DOCurrencyRate
                                };
                                uenItems.Add(garmentUnitExpenditureNoteItem);
                                EntityExtension.FlagForCreate(garmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);

                            }
                            GarmentUnitExpenditureNote garmentUnitExpenditureNote = new GarmentUnitExpenditureNote
                            {
                                ExpenditureDate = garmentUnitDeliveryOrder.UnitDODate,
                                ExpenditureType = "TRANSFER",
                                ExpenditureTo = "GUDANG LAIN",
                                UnitDOId = garmentUnitDeliveryOrder.Id,
                                UnitDONo = garmentUnitDeliveryOrder.UnitDONo,
                                UnitSenderId = garmentUnitDeliveryOrder.UnitSenderId,
                                UnitSenderCode = garmentUnitDeliveryOrder.UnitSenderCode,
                                UnitSenderName = garmentUnitDeliveryOrder.UnitSenderName,
                                StorageId = garmentUnitDeliveryOrder.StorageId,
                                StorageCode = garmentUnitDeliveryOrder.StorageCode,
                                StorageName = garmentUnitDeliveryOrder.StorageName,
                                UnitRequestCode = garmentUnitDeliveryOrder.UnitRequestCode,
                                UnitRequestId = garmentUnitDeliveryOrder.UnitRequestId,
                                UnitRequestName = garmentUnitDeliveryOrder.UnitRequestName,
                                StorageRequestCode = garmentUnitDeliveryOrder.StorageRequestCode,
                                StorageRequestId = garmentUnitDeliveryOrder.StorageRequestId,
                                StorageRequestName = garmentUnitDeliveryOrder.StorageRequestName,
                                IsTransfered = true,
                                Items = uenItems
                            };
                            garmentUnitExpenditureNote.UENNo = await garmentUnitExpenditureNoteFacade.GenerateNo(garmentUnitExpenditureNote);
                            EntityExtension.FlagForCreate(garmentUnitExpenditureNote, identityService.Username, USER_AGENT);

                            dbSetGarmentUnitExpenditureNote.Add(garmentUnitExpenditureNote);
                            await dbContext.SaveChangesAsync();

                            var garmentInventoryDocumentOut = garmentUnitExpenditureNoteFacade.GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOut);

                            List<GarmentUnitReceiptNoteItem> urnItems = new List<GarmentUnitReceiptNoteItem>();

                            foreach (var uenItem in uenItems)
                            {
                                var garmentInventorySummaryExistingBUK = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == uenItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageId && s.UomId == uenItem.UomId);

                                var garmentInventoryMovement = garmentUnitExpenditureNoteFacade.GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, uenItem, garmentInventorySummaryExistingBUK, "OUT");
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                                if (garmentInventorySummaryExistingBUK == null)
                                {
                                    var garmentInventorySummary = garmentUnitExpenditureNoteFacade.GenerateGarmentInventorySummary(garmentUnitExpenditureNote, uenItem, garmentInventoryMovement);
                                    dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExistingBUK, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExistingBUK.Quantity = garmentInventoryMovement.After;
                                }

                                await dbContext.SaveChangesAsync();

                                var pritem = dbContext.GarmentPurchaseRequestItems.AsNoTracking().FirstOrDefault(p => p.Id == uenItem.PRItemId);
                                var prHeader = dbContext.GarmentPurchaseRequests.AsNoTracking().FirstOrDefault(p => p.Id == pritem.GarmentPRId);
                                var poItem = dbContext.GarmentInternalPurchaseOrderItems.AsNoTracking().FirstOrDefault(p => p.Id == uenItem.POItemId);
                                var urnitem = dbContext.GarmentUnitReceiptNoteItems.AsNoTracking().FirstOrDefault(a => a.Id == uenItem.URNItemId);
                                var unitDOitem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == uenItem.UnitDOItemId);

                                GarmentUnitReceiptNoteItem garmentURNItem = new GarmentUnitReceiptNoteItem
                                {
                                    DODetailId = uenItem.DODetailId,
                                    EPOItemId = uenItem.EPOItemId,
                                    PRItemId = uenItem.PRItemId,
                                    PRId = prHeader.Id,
                                    PRNo = prHeader.PRNo,
                                    POId = poItem.GPOId,
                                    POItemId = uenItem.POItemId,
                                    POSerialNumber = uenItem.POSerialNumber,
                                    ProductId = uenItem.ProductId,
                                    ProductCode = uenItem.ProductCode,
                                    ProductName = uenItem.ProductName,
                                    ProductRemark = uenItem.ProductRemark,
                                    RONo = uenItem.RONo,
                                    ReceiptQuantity = (decimal)uenItem.Quantity / uenItem.Conversion,
                                    UomId = urnitem.UomId,
                                    UomUnit = urnitem.UomUnit,
                                    PricePerDealUnit = (decimal)uenItem.PricePerDealUnit,
                                    DesignColor = unitDOitem.DesignColor,
                                    IsCorrection = false,
                                    Conversion = uenItem.Conversion,
                                    SmallQuantity = (decimal)uenItem.Quantity,
                                    SmallUomId = uenItem.UomId,
                                    SmallUomUnit = uenItem.UomUnit,
                                    ReceiptCorrection = (decimal)uenItem.Quantity / uenItem.Conversion,
                                    CorrectionConversion = uenItem.Conversion,
                                    OrderQuantity = 0,
                                    DOCurrencyRate = uenItem.DOCurrencyRate != null ? (double)uenItem.DOCurrencyRate : 0,
                                    UENItemId = uenItem.Id
                                };
                                urnItems.Add(garmentURNItem);
                                EntityExtension.FlagForCreate(garmentURNItem, identityService.Username, USER_AGENT);
                            }

                            GarmentUnitReceiptNote garmentUrn = new GarmentUnitReceiptNote
                            {
                                URNType = "GUDANG LAIN",
                                UnitId = garmentUnitExpenditureNote.UnitRequestId,
                                UnitCode = garmentUnitExpenditureNote.UnitRequestCode,
                                UnitName = garmentUnitExpenditureNote.UnitRequestName,
                                UENId = garmentUnitExpenditureNote.Id,
                                UENNo = garmentUnitExpenditureNote.UENNo,
                                ReceiptDate = garmentUnitExpenditureNote.ExpenditureDate,
                                IsStorage = true,
                                StorageId = garmentUnitExpenditureNote.StorageRequestId,
                                StorageCode = garmentUnitExpenditureNote.StorageRequestCode,
                                StorageName = garmentUnitExpenditureNote.StorageRequestName,
                                IsCorrection = false,
                                IsUnitDO = false,
                                Items = urnItems
                            };
                            garmentUrn.URNNo = await GenerateNo(garmentUrn);
                            EntityExtension.FlagForCreate(garmentUrn, identityService.Username, USER_AGENT);

                            dbSet.Add(garmentUrn);

                            var garmentInventoryDocument2 = GenerateGarmentInventoryDocument(garmentUrn);
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocument2);

                            foreach (var gurnItem in urnItems)
                            {
                                var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == gurnItem.ProductId && s.StorageId == garmentUrn.StorageId && s.UomId == gurnItem.SmallUomId);

                                var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUrn, gurnItem, garmentInventorySummaryExisting);
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                                if (garmentInventorySummaryExisting == null)
                                {
                                    var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUrn, gurnItem, garmentInventoryMovement);
                                    dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                                }

                                await dbContext.SaveChangesAsync();
                            }

                            foreach (var garmentUnitReceiptNoteItem in garmentUrn.Items)
                            {
                                GarmentDOItems garmentDOItems = new GarmentDOItems
                                {
                                    DOItemNo = await GenerateNoDOItems(garmentUrn),
                                    UnitId = garmentUrn.UnitId,
                                    UnitCode = garmentUrn.UnitCode,
                                    UnitName = garmentUrn.UnitName,
                                    StorageCode = garmentUrn.StorageCode,
                                    StorageId = garmentUrn.StorageId,
                                    StorageName = garmentUrn.StorageName,
                                    POId = garmentUnitReceiptNoteItem.POId,
                                    POItemId = garmentUnitReceiptNoteItem.POItemId,
                                    POSerialNumber = garmentUnitReceiptNoteItem.POSerialNumber,
                                    ProductCode = garmentUnitReceiptNoteItem.ProductCode,
                                    ProductId = garmentUnitReceiptNoteItem.ProductId,
                                    ProductName = garmentUnitReceiptNoteItem.ProductName,
                                    DesignColor = garmentUnitReceiptNoteItem.DesignColor,
                                    SmallQuantity = garmentUnitReceiptNoteItem.SmallQuantity,
                                    SmallUomId = garmentUnitReceiptNoteItem.SmallUomId,
                                    SmallUomUnit = garmentUnitReceiptNoteItem.SmallUomUnit,
                                    RemainingQuantity = garmentUnitReceiptNoteItem.SmallQuantity,
                                    DetailReferenceId = garmentUnitReceiptNoteItem.DODetailId,
                                    URNItemId = garmentUnitReceiptNoteItem.Id,
                                    DOCurrencyRate = garmentUnitReceiptNoteItem.DOCurrencyRate,
                                    EPOItemId = garmentUnitReceiptNoteItem.EPOItemId,
                                    PRItemId = garmentUnitReceiptNoteItem.PRItemId,
                                    RO = garmentUnitReceiptNoteItem.RONo,

                                };
                                EntityExtension.FlagForCreate(garmentDOItems, identityService.Username, USER_AGENT);
                                dbSetGarmentDOItems.Add(garmentDOItems);
                                await dbContext.SaveChangesAsync();
                            }

                            await dbContext.SaveChangesAsync();
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

        private async Task UpdateDR(string DRId, bool isUsed, string unitCode)
        {
            string drUri = unitCode=="SMP1" ? "garment-sample-delivery-returns" : "delivery-returns";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = await httpClient.GetAsync($"{APIEndpoint.GarmentProduction}{drUri}/{DRId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                GarmentDeliveryReturnViewModel viewModel = JsonConvert.DeserializeObject<GarmentDeliveryReturnViewModel>(result.GetValueOrDefault("data").ToString());
                viewModel.IsUsed = isUsed;
                foreach (var item in viewModel.Items)
                {
                    item.QuantityUENItem = item.Quantity + 1;
                    item.RemainingQuantityPreparingItem = item.Quantity + 1;
                    item.IsSave = true;
                }

                //var httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
                var response2 = await httpClient.PutAsync($"{APIEndpoint.GarmentProduction}{drUri}/{DRId}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, General.JsonMediaType));
                var content2 = await response2.Content.ReadAsStringAsync();
                response2.EnsureSuccessStatusCode();
            }

        }

        private GarmentDeliveryReturnViewModel GetDR(string DRId, string UnitCode)
        {
            string drUri = UnitCode=="SMP1" ?"garment-sample-delivery-returns" : "delivery-returns";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.GarmentProduction}{drUri}/{DRId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                GarmentDeliveryReturnViewModel viewModel = JsonConvert.DeserializeObject<GarmentDeliveryReturnViewModel>(result.GetValueOrDefault("data").ToString());

                return viewModel;
            }
            else
            {
                return null;
            }
        }

        private async Task UpdateExpenditure(long ExpenditureId, bool isUsed, string type)
        {
            string uri = type == "FABRIC" ? "garment/leftover-warehouse-expenditures/fabric" : "garment/leftover-warehouse-expenditures/accessories";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = await httpClient.GetAsync($"{APIEndpoint.Inventory}{uri}/{ExpenditureId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                if (type != "FABRIC")
                {
                    GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel = JsonConvert.DeserializeObject<GarmentLeftoverWarehouseExpenditureAccessoriesViewModel>(result.GetValueOrDefault("data").ToString());
                    viewModel.IsUsed = isUsed;

                    var response2 = await httpClient.PutAsync($"{APIEndpoint.Inventory}{uri}/{ExpenditureId}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, General.JsonMediaType));
                    var content2 = await response2.Content.ReadAsStringAsync();
                    response2.EnsureSuccessStatusCode();
                }
                else
                {
                    GarmentLeftoverWarehouseExpenditureFabricViewModel viewModel = JsonConvert.DeserializeObject<GarmentLeftoverWarehouseExpenditureFabricViewModel>(result.GetValueOrDefault("data").ToString());
                    viewModel.IsUsed = isUsed;

                    var response2 = await httpClient.PutAsync($"{APIEndpoint.Inventory}{uri}/{ExpenditureId}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, General.JsonMediaType));
                    var content2 = await response2.Content.ReadAsStringAsync();
                    response2.EnsureSuccessStatusCode();
                }

            }

        }

        //private GarmentLeftoverWarehouseExpenditureAccessoriesViewModel GetAccExpenditure(long ExpenditureId)
        //{
        //    string uri = "garment/leftover-warehouse-expenditures/accessories";
        //    IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

        //    var response = httpClient.GetAsync($"{APIEndpoint.GarmentProduction}{uri}/{ExpenditureId}").Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = response.Content.ReadAsStringAsync().Result;
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        //        GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel = JsonConvert.DeserializeObject<GarmentLeftoverWarehouseExpenditureAccessoriesViewModel>(result.GetValueOrDefault("data").ToString());

        //        return viewModel;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        public async Task<int> Update(int id, GarmentUnitReceiptNote garmentUnitReceiptNote)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldGarmentUnitReceiptNote = dbSet
                        .Include(d => d.Items)
                        .Single(m => m.Id == id);

                    // Gudang berubah
                    if (garmentUnitReceiptNote.StorageId != oldGarmentUnitReceiptNote.StorageId)
                    {
                        foreach (var oldGarmentUnitReceiptNoteItem in oldGarmentUnitReceiptNote.Items)
                        {
                            // Buat OUT untuk Gudang yang lama
                            var oldGarmentInventorySummary = dbSetGarmentInventorySummary.Single(s => s.ProductId == oldGarmentUnitReceiptNoteItem.ProductId && s.StorageId == oldGarmentUnitReceiptNote.StorageId && s.UomId == oldGarmentUnitReceiptNoteItem.SmallUomId);

                            var garmentInventoryMovementOut = GenerateGarmentInventoryMovement(oldGarmentUnitReceiptNote, oldGarmentUnitReceiptNoteItem, oldGarmentInventorySummary, "OUT");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementOut);

                            EntityExtension.FlagForUpdate(oldGarmentInventorySummary, identityService.Username, USER_AGENT);
                            oldGarmentInventorySummary.Quantity = garmentInventoryMovementOut.After;

                            // Buat IN untuk Gudang yang baru
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == oldGarmentUnitReceiptNoteItem.ProductId && s.StorageId == garmentUnitReceiptNote.StorageId && s.UomId == oldGarmentUnitReceiptNoteItem.SmallUomId);

                            var garmentInventoryMovementIn = GenerateGarmentInventoryMovement(garmentUnitReceiptNote, oldGarmentUnitReceiptNoteItem, garmentInventorySummaryExisting, "IN");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementIn);

                            if (garmentInventorySummaryExisting == null)
                            {
                                var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitReceiptNote, oldGarmentUnitReceiptNoteItem, garmentInventoryMovementIn);
                                dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovementIn.After;
                            }

                            await dbContext.SaveChangesAsync();
                        }

                        var garmentInventoryDocumentOut = GenerateGarmentInventoryDocument(oldGarmentUnitReceiptNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOut);

                        var garmentInventoryDocumentIn = GenerateGarmentInventoryDocument(garmentUnitReceiptNote, "IN");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentIn);

                        oldGarmentUnitReceiptNote.StorageId = garmentUnitReceiptNote.StorageId;
                        oldGarmentUnitReceiptNote.StorageCode = garmentUnitReceiptNote.StorageCode;
                        oldGarmentUnitReceiptNote.StorageName = garmentUnitReceiptNote.StorageName;
                    }

                    EntityExtension.FlagForUpdate(oldGarmentUnitReceiptNote, identityService.Username, USER_AGENT);
                    foreach (var oldGarmentUnitReceiptNoteItem in oldGarmentUnitReceiptNote.Items)
                    {
                        EntityExtension.FlagForUpdate(oldGarmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                    }
                    oldGarmentUnitReceiptNote.Remark = garmentUnitReceiptNote.Remark;

                    var garmentDeliveryOrder = dbsetGarmentDeliveryOrder.First(d => d.Id == garmentUnitReceiptNote.DOId);

                    garmentUnitReceiptNote.DOCurrencyRate = garmentDeliveryOrder.DOCurrencyRate;

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

        public async Task<int> Delete(int id, string deletedReason)
        {
            int Deleted = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var garmentUnitReceiptNote = dbSet.Include(m => m.Items).Single(m => m.Id == id);

                    garmentUnitReceiptNote.DeletedReason = deletedReason;
                    EntityExtension.FlagForDelete(garmentUnitReceiptNote, identityService.Username, USER_AGENT);
                    if (garmentUnitReceiptNote.DOId > 0)
                    {
                        var garmentDeliveryOrderUpdate = dbsetGarmentDeliveryOrder.First(d => d.Id == garmentUnitReceiptNote.DOId);
                        garmentDeliveryOrderUpdate.IsCustoms = false;
                        EntityExtension.FlagForUpdate(garmentDeliveryOrderUpdate, identityService.Username, USER_AGENT);

                    }

                    foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                    {
                        EntityExtension.FlagForDelete(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);

                        //update per 10-06-21
                        if (garmentUnitReceiptNote.URNType != "PROSES" && garmentUnitReceiptNote.URNType != "GUDANG SISA" && garmentUnitReceiptNote.URNType != "SISA SUBCON")
                        {
                            var garmentDeliveryOrderDetail = dbSetGarmentDeliveryOrderDetail.First(d => d.Id == garmentUnitReceiptNoteItem.DODetailId);
                            EntityExtension.FlagForUpdate(garmentDeliveryOrderDetail, identityService.Username, USER_AGENT);
                            garmentDeliveryOrderDetail.ReceiptQuantity = (double)((decimal)garmentDeliveryOrderDetail.ReceiptQuantity - garmentUnitReceiptNoteItem.ReceiptQuantity);

                            var garmentExternalPurchaseOrderItem = dbSetGarmentExternalPurchaseOrderItems.First(d => d.Id == garmentUnitReceiptNoteItem.EPOItemId);
                            EntityExtension.FlagForUpdate(garmentExternalPurchaseOrderItem, identityService.Username, USER_AGENT);
                            garmentExternalPurchaseOrderItem.ReceiptQuantity = (double)((decimal)garmentExternalPurchaseOrderItem.ReceiptQuantity - garmentUnitReceiptNoteItem.ReceiptQuantity);

                            if (garmentExternalPurchaseOrderItem.ReceiptQuantity == 0)
                            {
                                var garmentInternalPurchaseOrderItem = dbSetGarmentInternalPurchaseOrderItems.First(d => d.Id == garmentUnitReceiptNoteItem.POItemId);
                                if (garmentExternalPurchaseOrderItem.DOQuantity > 0 && garmentExternalPurchaseOrderItem.DOQuantity < garmentExternalPurchaseOrderItem.DealQuantity)
                                {
                                    garmentInternalPurchaseOrderItem.Status = "Barang sudah datang parsial";
                                }
                                else if (garmentExternalPurchaseOrderItem.DOQuantity > 0 && garmentExternalPurchaseOrderItem.DOQuantity >= garmentExternalPurchaseOrderItem.DealQuantity)
                                {
                                    garmentInternalPurchaseOrderItem.Status = "Barang sudah datang semua";
                                }
                            }
                        }

                    }

                    if (garmentUnitReceiptNote.IsStorage && garmentUnitReceiptNote.URNType != "PROSES")
                    {
                        var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitReceiptNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitReceiptNoteItem.ProductId && s.StorageId == garmentUnitReceiptNote.StorageId && s.UomId == garmentUnitReceiptNoteItem.SmallUomId);

                            var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitReceiptNote, garmentUnitReceiptNoteItem, garmentInventorySummaryExisting, "OUT");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                            if (garmentInventorySummaryExisting != null)
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                            }
                        }
                    }

                    if (garmentUnitReceiptNote.URNType == "PEMBELIAN" || garmentUnitReceiptNote.URNType == "PROSES" || garmentUnitReceiptNote.URNType == "GUDANG SISA" || garmentUnitReceiptNote.URNType == "SISA SUBCON")
                    {
                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentDOItems garmentDOItems = dbSetGarmentDOItems.FirstOrDefault(a => a.URNItemId == garmentUnitReceiptNoteItem.Id);
                            if (garmentDOItems != null)
                                EntityExtension.FlagForDelete(garmentDOItems, identityService.Username, USER_AGENT);
                        }
                    }


                    if (garmentUnitReceiptNote.URNType == "GUDANG SISA")
                    {
                        await UpdateExpenditure(garmentUnitReceiptNote.ExpenditureId, false, garmentUnitReceiptNote.Category);
                    }
                    else if (garmentUnitReceiptNote.URNType == "SISA SUBCON")
                    {
                        var garmentUnitExpenditureNote = dbContext.GarmentUnitExpenditureNotes.Single(a => a.Id == garmentUnitReceiptNote.UENId);
                        garmentUnitExpenditureNote.IsReceived = false;
                    }

                    if (garmentUnitReceiptNote.URNType == "PROSES")
                    {
                        await UpdateDR(garmentUnitReceiptNote.DRId, false, garmentUnitReceiptNote.UnitCode);
                        var GarmentDR = GetDR(garmentUnitReceiptNote.DRId, garmentUnitReceiptNote.UnitCode);
                        var GarmentUnitDO = dbContext.GarmentUnitDeliveryOrders.AsNoTracking().Single(a => a.Id == GarmentDR.UnitDOId);
                        if (GarmentUnitDO.UnitDOFromId != 0)
                        {
                            var garmentUnitDOItem = dbContext.GarmentUnitDeliveryOrderItems.FirstOrDefault(x => x.URNId == garmentUnitReceiptNote.Id);
                            var unitDO = dbContext.GarmentUnitDeliveryOrders.Include(m => m.Items).Single(a => a.Id == garmentUnitDOItem.UnitDOId);
                            EntityExtension.FlagForDelete(unitDO, identityService.Username, USER_AGENT);
                            foreach (var uDOItem in unitDO.Items)
                            {
                                EntityExtension.FlagForDelete(uDOItem, identityService.Username, USER_AGENT);
                            }

                            var garmentExpenditureNote = dbContext.GarmentUnitExpenditureNotes.Include(m => m.Items).Single(x => x.UnitDOId == unitDO.Id);
                            EntityExtension.FlagForDelete(garmentExpenditureNote, identityService.Username, USER_AGENT);
                            GarmentUnitExpenditureNoteFacade.GarmentUnitExpenditureNoteFacade garmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade.GarmentUnitExpenditureNoteFacade(serviceProvider, dbContext);




                            //var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitReceiptNote, "OUT");
                            //dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                            //foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                            //{
                            //    var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitReceiptNoteItem.ProductId && s.StorageId == garmentUnitReceiptNote.StorageId && s.UomId == garmentUnitReceiptNoteItem.SmallUomId);

                            //    var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitReceiptNote, garmentUnitReceiptNoteItem, garmentInventorySummaryExisting, "OUT");
                            //    dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                            //    if (garmentInventorySummaryExisting != null)
                            //    {
                            //        EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            //        garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                            //    }

                            //    await dbContext.SaveChangesAsync();
                            //}

                            var gURN = dbSet.Include(m => m.Items).Single(x => x.UENId == garmentExpenditureNote.Id);
                            EntityExtension.FlagForDelete(gURN, identityService.Username, USER_AGENT);

                            var garmentInventoryDocument1 = GenerateGarmentInventoryDocument(gURN, "OUT");
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocument1);

                            foreach (var gURNItem in gURN.Items)
                            {
                                EntityExtension.FlagForDelete(gURNItem, identityService.Username, USER_AGENT);

                                var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == gURNItem.ProductId && s.StorageId == gURN.StorageId && s.UomId == gURNItem.SmallUomId);

                                var garmentInventoryMovement = GenerateGarmentInventoryMovement(gURN, gURNItem, garmentInventorySummaryExisting, "OUT");
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                                if (garmentInventorySummaryExisting != null)
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                                }
                            }

                            var garmentInventoryDocumentOut = garmentUnitExpenditureNoteFacade.GenerateGarmentInventoryDocument(garmentExpenditureNote);
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOut);

                            foreach (var uenItem in garmentExpenditureNote.Items)
                            {
                                EntityExtension.FlagForDelete(uenItem, identityService.Username, USER_AGENT);

                                var garmentInventorySummaryExistingBUK = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == uenItem.ProductId && s.StorageId == garmentExpenditureNote.StorageId && s.UomId == uenItem.UomId);

                                var garmentInventoryMovement = garmentUnitExpenditureNoteFacade.GenerateGarmentInventoryMovement(garmentExpenditureNote, uenItem, garmentInventorySummaryExistingBUK);
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                                if (garmentInventorySummaryExistingBUK == null)
                                {
                                    var garmentInventorySummary = garmentUnitExpenditureNoteFacade.GenerateGarmentInventorySummary(garmentExpenditureNote, uenItem, garmentInventoryMovement);
                                    dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExistingBUK, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExistingBUK.Quantity = garmentInventoryMovement.After;
                                }

                                await dbContext.SaveChangesAsync();
                            }
                        }

                        var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitReceiptNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitReceiptNoteItem.ProductId && s.StorageId == garmentUnitReceiptNote.StorageId && s.UomId == garmentUnitReceiptNoteItem.SmallUomId);

                            var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitReceiptNote, garmentUnitReceiptNoteItem, garmentInventorySummaryExisting, "OUT");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                            if (garmentInventorySummaryExisting != null)
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                            }
                        }

                    }

                    Deleted = await dbContext.SaveChangesAsync();
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

        private GarmentInventorySummary GenerateGarmentInventorySummary(GarmentUnitReceiptNote garmentUnitReceiptNote, GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem, GarmentInventoryMovement garmentInventoryMovement)
        {
            var garmentInventorySummary = new GarmentInventorySummary();
            EntityExtension.FlagForCreate(garmentInventorySummary, identityService.Username, USER_AGENT);
            do
            {
                garmentInventorySummary.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventorySummary.Any(m => m.No == garmentInventorySummary.No));

            garmentInventorySummary.ProductId = garmentUnitReceiptNoteItem.ProductId;
            garmentInventorySummary.ProductCode = garmentUnitReceiptNoteItem.ProductCode;
            garmentInventorySummary.ProductName = garmentUnitReceiptNoteItem.ProductName;

            garmentInventorySummary.StorageId = garmentUnitReceiptNote.StorageId;
            garmentInventorySummary.StorageCode = garmentUnitReceiptNote.StorageCode;
            garmentInventorySummary.StorageName = garmentUnitReceiptNote.StorageName;

            garmentInventorySummary.Quantity = garmentInventoryMovement.After;

            garmentInventorySummary.UomId = garmentUnitReceiptNoteItem.SmallUomId;
            garmentInventorySummary.UomUnit = garmentUnitReceiptNoteItem.SmallUomUnit;

            garmentInventorySummary.StockPlanning = 0;

            return garmentInventorySummary;
        }

        private GarmentInventoryMovement GenerateGarmentInventoryMovement(GarmentUnitReceiptNote garmentUnitReceiptNote, GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem, GarmentInventorySummary garmentInventorySummary, string type = "IN")
        {
            var garmentInventoryMovement = new GarmentInventoryMovement();
            EntityExtension.FlagForCreate(garmentInventoryMovement, identityService.Username, USER_AGENT);
            do
            {
                garmentInventoryMovement.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventoryMovement.Any(m => m.No == garmentInventoryMovement.No));

            garmentInventoryMovement.Date = garmentInventoryMovement.CreatedUtc;

            garmentInventoryMovement.ReferenceNo = garmentUnitReceiptNote.URNNo;
            garmentInventoryMovement.ReferenceType = string.Concat("Bon Terima Unit - ", garmentUnitReceiptNote.UnitName);

            garmentInventoryMovement.ProductId = garmentUnitReceiptNoteItem.ProductId;
            garmentInventoryMovement.ProductCode = garmentUnitReceiptNoteItem.ProductCode;
            garmentInventoryMovement.ProductName = garmentUnitReceiptNoteItem.ProductName;

            garmentInventoryMovement.StorageId = garmentUnitReceiptNote.StorageId;
            garmentInventoryMovement.StorageCode = garmentUnitReceiptNote.StorageCode;
            garmentInventoryMovement.StorageName = garmentUnitReceiptNote.StorageName;

            garmentInventoryMovement.StockPlanning = 0;

            garmentInventoryMovement.Before = garmentInventorySummary == null ? 0 : garmentInventorySummary.Quantity;
            garmentInventoryMovement.Quantity = garmentUnitReceiptNoteItem.SmallQuantity * ((type ?? "").ToUpper() == "OUT" ? -1 : 1);
            garmentInventoryMovement.After = garmentInventoryMovement.Before + garmentInventoryMovement.Quantity;

            garmentInventoryMovement.UomId = garmentUnitReceiptNoteItem.SmallUomId;
            garmentInventoryMovement.UomUnit = garmentUnitReceiptNoteItem.SmallUomUnit;

            garmentInventoryMovement.Remark = garmentUnitReceiptNoteItem.ProductRemark;

            garmentInventoryMovement.Type = (type ?? "").ToUpper() == "IN" ? "IN" : "OUT";

            return garmentInventoryMovement;
        }

        private GarmentInventoryDocument GenerateGarmentInventoryDocument(GarmentUnitReceiptNote garmentUnitReceiptNote, string type = "IN")
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

            garmentInventoryDocument.ReferenceNo = garmentUnitReceiptNote.URNNo;
            garmentInventoryDocument.ReferenceType = string.Concat("Bon Terima Unit - ", garmentUnitReceiptNote.UnitName);

            garmentInventoryDocument.Type = (type ?? "").ToUpper() == "IN" ? "IN" : "OUT";

            garmentInventoryDocument.StorageId = garmentUnitReceiptNote.StorageId;
            garmentInventoryDocument.StorageCode = garmentUnitReceiptNote.StorageCode;
            garmentInventoryDocument.StorageName = garmentUnitReceiptNote.StorageName;

            garmentInventoryDocument.Remark = garmentUnitReceiptNote.Remark;

            garmentInventoryDocument.Date = DateTimeOffset.Now;

            foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
            {
                var garmentInventoryDocumentItem = new GarmentInventoryDocumentItem();
                EntityExtension.FlagForCreate(garmentInventoryDocumentItem, identityService.Username, USER_AGENT);

                garmentInventoryDocumentItem.ProductId = garmentUnitReceiptNoteItem.ProductId;
                garmentInventoryDocumentItem.ProductCode = garmentUnitReceiptNoteItem.ProductCode;
                garmentInventoryDocumentItem.ProductName = garmentUnitReceiptNoteItem.ProductName;

                garmentInventoryDocumentItem.Quantity = garmentUnitReceiptNoteItem.SmallQuantity;

                garmentInventoryDocumentItem.UomId = garmentUnitReceiptNoteItem.SmallUomId;
                garmentInventoryDocumentItem.UomUnit = garmentUnitReceiptNoteItem.SmallUomUnit;

                garmentInventoryDocumentItem.ProductRemark = garmentUnitReceiptNoteItem.ProductRemark;

                garmentInventoryDocument.Items.Add(garmentInventoryDocumentItem);
            }

            return garmentInventoryDocument;
        }

        public async Task<string> GenerateNo(GarmentUnitReceiptNote garmentUnitReceiptNote)
        {
            string Year = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");
            string Day = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd");

            string no = string.Concat("BUM", garmentUnitReceiptNote.UnitCode, Year, Month, Day);
            int Padding = 3;

            var lastNo = await dbSet.Where(w => w.URNNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.URNNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.URNNo.Replace(no, string.Empty)) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public async Task<string> GenerateNoDOItems(GarmentUnitReceiptNote garmentUnitReceiptNote)
        {
            string Year = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");
            string Day = garmentUnitReceiptNote.ReceiptDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd");

            string no = string.Concat("DOI", garmentUnitReceiptNote.UnitCode, Year, Month);
            int Padding = 5;

            var lastNo = await dbSetGarmentDOItems.Where(w => w.DOItemNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.DOItemNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.DOItemNo.Replace(no, string.Empty)) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public List<object> ReadForUnitDO(string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitReceiptNote> Query = dbSet;
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            long unitId = 0;
            long storageId = 0;
            bool hasUnitFilter = FilterDictionary.ContainsKey("UnitId") && long.TryParse(FilterDictionary["UnitId"], out unitId);
            bool hasStorageFilter = FilterDictionary.ContainsKey("StorageId") && long.TryParse(FilterDictionary["StorageId"], out storageId);
            bool isPROSES = FilterDictionary.ContainsKey("Type") && FilterDictionary["Type"] == "PROSES";

            var readForUnitDO = Query.Where(x =>
                    (!hasUnitFilter ? true : x.UnitId == unitId) &&
                    (!hasStorageFilter ? true : x.StorageId == storageId) &&
                    x.IsDeleted == false &&
                    x.Items.Any(i => i.RONo.Contains((Keyword ?? "").Trim()) && (isPROSES && (i.RONo.EndsWith("S") || i.RONo.EndsWith("M")) ? false : true))
                )
                .SelectMany(x => x.Items
                .Where(i => ((i.ReceiptCorrection * i.CorrectionConversion) - i.OrderQuantity > 0) && i.RONo.Contains((Keyword ?? "").Trim()) && (isPROSES && (i.RONo.EndsWith("S") || i.RONo.EndsWith("M")) ? false : true))
                .Select(y => new
                {
                    x.URNNo,
                    y.URNId,
                    y.Id,
                    y.RONo,
                    y.DODetailId,
                    y.EPOItemId,
                    y.POItemId,
                    y.PRItemId,
                    y.ProductId,
                    y.ProductName,
                    y.ProductCode,
                    y.ProductRemark,
                    y.OrderQuantity,
                    y.SmallQuantity,
                    y.SmallUomId,
                    y.SmallUomUnit,
                    y.DesignColor,
                    y.POSerialNumber,
                    y.PricePerDealUnit,
                    y.ReceiptCorrection,
                    y.Conversion,
                    y.CorrectionConversion,
                    Article = dbContext.GarmentExternalPurchaseOrderItems.Where(m => m.Id == y.EPOItemId).Select(d => d.Article).FirstOrDefault()
                })).ToList();
            var coba = readForUnitDO.GroupBy(g => g.RONo);
            var test = coba.Select(c => new
            {
                Article = c.Select(s => s.Article).FirstOrDefault(),
                RONo = c.Key,
                Items = c.ToList()
            });
            List<object> result = new List<object>(test);
            return result;
        }

        public List<object> ReadForUnitDOHeader(string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitReceiptNote> Query = dbSet;
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            long unitId = 0;
            long storageId = 0;
            bool hasUnitFilter = FilterDictionary.ContainsKey("UnitId") && long.TryParse(FilterDictionary["UnitId"], out unitId);
            bool isPROSES = FilterDictionary.ContainsKey("Type") && FilterDictionary["Type"] == "PROSES";
            bool hasRONoFilter = FilterDictionary.ContainsKey("RONo");
            bool hasStorageFilter = FilterDictionary.ContainsKey("StorageId") && long.TryParse(FilterDictionary["StorageId"], out storageId);
            string RONo = hasRONoFilter ? (FilterDictionary["RONo"] ?? "").Trim() : "";

            var readForUnitDO = Query.Where(x =>
                    (!hasUnitFilter ? true : x.UnitId == unitId) &&
                    (!hasStorageFilter ? true : x.StorageId == storageId) &&
                    x.IsDeleted == false &&
                    x.Items.Any(i => i.RONo.Contains((Keyword ?? "").Trim()) && ((i.ReceiptCorrection * i.CorrectionConversion) - i.OrderQuantity > 0) && (hasRONoFilter ? (i.RONo != RONo) : true))
                )
                .SelectMany(x => x.Items.Select(y => new
                {
                    x.URNNo,
                    y.URNId,
                    y.Id,
                    y.RONo,
                    y.DODetailId,
                    y.EPOItemId,
                    y.POItemId,
                    y.PRItemId,
                    y.ProductId,
                    y.ProductName,
                    y.ProductCode,
                    y.ProductRemark,
                    y.OrderQuantity,
                    y.SmallQuantity,
                    y.DesignColor,
                    y.SmallUomId,
                    y.SmallUomUnit,
                    y.POSerialNumber,
                    y.PricePerDealUnit,
                    y.ReceiptCorrection,
                    y.Conversion,
                    y.CorrectionConversion,
                    Article = dbContext.GarmentExternalPurchaseOrderItems.Where(m => m.Id == y.EPOItemId).Select(d => d.Article).FirstOrDefault()
                })).ToList();
            List<object> result = new List<object>(readForUnitDO);
            return result;
        }

        public List<object> ReadURNItem(string Keyword = null, string Filter = "{}")
        {
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            bool hasDONoFilter = FilterDictionary.ContainsKey("DONo");
            bool hasUnitCodeFilter = FilterDictionary.ContainsKey("UnitCode");
            bool hasStorageCodeFilter = FilterDictionary.ContainsKey("StorageCode");
            string DONo = hasDONoFilter ? (FilterDictionary["DONo"] ?? "").Trim() : "";
            string UnitCode = hasUnitCodeFilter ? (FilterDictionary["UnitCode"] ?? "").Trim() : "";
            string StorageCode = hasStorageCodeFilter ? (FilterDictionary["StorageCode"] ?? "").Trim() : "";
            var dataDO = (from a in dbContext.GarmentDeliveryOrders
                          join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                          join c in dbContext.GarmentDeliveryOrderDetails on b.Id equals c.GarmentDOItemId
                          where a.DONo == DONo
                          select new { DOId = a.Id, c.Id }).Distinct().ToList();
            var doDetailIds = dataDO.Select(a => a.Id).Distinct().ToList();
            var query = (from y in dbContext.GarmentUnitReceiptNoteItems
                         join x in dbContext.GarmentUnitReceiptNotes on y.URNId equals x.Id
                         //join m in dbContext.GarmentExternalPurchaseOrderItems on y.EPOItemId equals m.Id
                         where x.UnitCode == UnitCode && x.StorageCode == StorageCode && doDetailIds.Contains(y.DODetailId)
                         select new
                         {
                             URNId = x.Id,
                             URNItemId = y.Id,
                             y.EPOItemId
                         }).ToList();

            var epoItemIds = query.Select(s => s.EPOItemId).ToList().Distinct().ToList();
            var epoItems = dbContext.GarmentExternalPurchaseOrderItems.Where(u => epoItemIds.Contains(u.Id))
                .Select(s => new { s.Id, s.Article }).ToList();

            var urnIds = query.Select(s => s.URNId).ToList().Distinct().ToList();
            var URNs = dbContext.GarmentUnitReceiptNotes.Where(u => urnIds.Contains(u.Id))
                .Select(s => new { s.Id, s.DOId, s.DONo, s.URNNo, s.DOCurrencyRate }).ToList();

            var urnItemIds = query.Select(s => s.URNItemId).ToList().Distinct().ToList();
            var urnItems = dbContext.GarmentUnitReceiptNoteItems.Where(u => urnItemIds.Contains(u.Id))
                .Select(y => new
                {
                    y.URNId,
                    y.Id,
                    y.RONo,
                    y.DODetailId,
                    y.EPOItemId,
                    y.POItemId,
                    y.PRItemId,
                    y.ProductId,
                    y.ProductName,
                    y.ProductCode,
                    y.ProductRemark,
                    y.OrderQuantity,
                    y.SmallQuantity,
                    y.DesignColor,
                    y.SmallUomId,
                    y.SmallUomUnit,
                    y.POSerialNumber,
                    y.PricePerDealUnit,
                    y.Conversion,
                    y.UomUnit,
                    y.UomId,
                    y.ReceiptCorrection,
                    y.CorrectionConversion,
                    y.DOCurrencyRate
                }).ToList();

            List<object> ListData = new List<object>();
            foreach (var item in query)
            {
                var urn = URNs.FirstOrDefault(f => f.Id.Equals(item.URNId));
                var urnItem = urnItems.FirstOrDefault(f => f.Id.Equals(item.URNItemId));
                var epoItem = epoItems.FirstOrDefault(f => f.Id.Equals(item.EPOItemId));
                string doNo = "";
                long doId = 0;
               // double doCurrencyRate = 0;
                if (urn.DOId == 0)
                {
                    var URN = URNs.FirstOrDefault(a => a.DONo == DONo);
                    doNo = URN.DONo;
                    doId = URN.DOId;
                    //doCurrencyRate = (double)URN.DOCurrencyRate;
                }
                ListData.Add(new
                {
                    DOId = doId == 0 ? urn.DOId : doId,
                    DONo = doNo == "" ? urn.DONo : doNo,
                    urn.URNNo,
                    urnItem.URNId,
                    urnItem.Id,
                    urnItem.RONo,
                    urnItem.DODetailId,
                    urnItem.EPOItemId,
                    urnItem.POItemId,
                    urnItem.PRItemId,
                    urnItem.ProductId,
                    urnItem.ProductName,
                    urnItem.ProductCode,
                    urnItem.ProductRemark,
                    urnItem.OrderQuantity,
                    urnItem.SmallQuantity,
                    urnItem.DesignColor,
                    urnItem.SmallUomId,
                    urnItem.SmallUomUnit,
                    urnItem.POSerialNumber,
                    urnItem.PricePerDealUnit,
                   // DOCurrencyRate = doCurrencyRate == 0 ? urn.DOCurrencyRate : doCurrencyRate,
                    urnItem.Conversion,
                    urnItem.UomUnit,
                    urnItem.UomId,
                    urnItem.ReceiptCorrection,
                    urnItem.CorrectionConversion,
                    epoItem.Article,
                    urnItem.DOCurrencyRate
                });
            }

            return ListData;
        }

        public List<object> ReadItemByRO(string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitReceiptNote> Query = this.dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "RONo",
            };

            IQueryable<GarmentUnitReceiptNoteItem> QueryItem = dbContext.GarmentUnitReceiptNoteItems;

            QueryItem = QueryHelper<GarmentUnitReceiptNoteItem>.ConfigureSearch(QueryItem, searchAttributes, Keyword);
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            long unitId = 0;
            long storageId = 0;
            bool hasUnitFilter = FilterDictionary.ContainsKey("UnitId") && long.TryParse(FilterDictionary["UnitId"], out unitId);
            bool hasRONoFilter = FilterDictionary.ContainsKey("RONo");
            bool hasStorageFilter = FilterDictionary.ContainsKey("StorageId") && long.TryParse(FilterDictionary["StorageId"], out storageId);
            string RONo = hasRONoFilter ? (FilterDictionary["RONo"] ?? "").Trim() : "";
            //QueryItem = QueryHelper<GarmentUnitReceiptNoteItem>.ConfigureFilter(QueryItem, FilterDictionary);

            var data = (from y in QueryItem
                        join x in Query on y.URNId equals x.Id
                        where
                        (!hasUnitFilter ? true : x.UnitId == unitId) &&
                        (!hasStorageFilter ? true : x.StorageId == storageId) &&
                        (!hasRONoFilter ? true : y.RONo == RONo)
                        select new
                        {
                            x.DOId,
                            x.DONo,
                            x.URNNo,
                            y.URNId,
                            y.Id,
                            y.RONo,
                            y.DODetailId,
                            y.EPOItemId,
                            y.POItemId,
                            y.PRItemId,
                            y.ProductId,
                            y.ProductName,
                            y.ProductCode,
                            y.ProductRemark,
                            y.OrderQuantity,
                            y.SmallQuantity,
                            y.DesignColor,
                            y.SmallUomId,
                            y.SmallUomUnit,
                            y.POSerialNumber,
                            y.PricePerDealUnit,
                            x.DOCurrencyRate,
                            y.Conversion,
                            y.UomUnit,
                            y.UomId,
                            y.ReceiptCorrection,
                            y.CorrectionConversion,
                            Article = dbContext.GarmentExternalPurchaseOrderItems.Where(m => m.Id == y.EPOItemId).Select(d => d.Article).FirstOrDefault()
                        }).ToList();
            List<object> ListData = new List<object>(data);
            return ListData;
        }
        //
        public List<object> ReadDataByDO(string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitReceiptNote> Query = this.dbSet;
            IQueryable<GarmentBeacukai> QueryBC = this.dbSetBC;
            IQueryable<GarmentBeacukaiItem> QueryBCI = this.dbSetBCI;

            List<string> searchAttributes = new List<string>()
            {
                "DONo",
            };

            Query = QueryHelper<GarmentUnitReceiptNote>.ConfigureSearch(Query, searchAttributes, Keyword);
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

            long unitId = 0;
            bool hasUnitFilter = FilterDictionary.ContainsKey("UnitId") && long.TryParse(FilterDictionary["UnitId"], out unitId);

            var data = (from x in Query
                        join b in QueryBCI on x.DOId equals b.GarmentDOId
                        join a in QueryBC on b.BeacukaiId equals a.Id
                        where x.URNType == "PEMBELIAN" &&
                        (!hasUnitFilter ? true : x.UnitId == unitId)
                        select new
                        {
                            x.DOId,
                            x.DONo,
                            x.Id,
                            x.UnitId,
                            x.UnitCode,
                            x.UnitName,
                            x.URNNo,
                            a.BeacukaiNo,
                            a.CustomsType,
                        }).ToList();
            List<object> ListData = new List<object>(data);
            return ListData;
        }
        //
        #region Flow Detail Penerimaan

        private List<GarmentCategoryViewModel> GetProductCodes(int page, int size, string order, string filter)
        {
            //var param = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
            if (httpClient != null)
            {
                var garmentSupplierUri = APIEndpoint.Core + $"master/garment-categories";
                string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
                string uri = garmentSupplierUri + queryUri;
                var response = httpClient.GetAsync($"{uri}").Result.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                List<GarmentCategoryViewModel> viewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<GarmentCategoryViewModel> viewModel = null;
                return viewModel;
            }
        }

        public IQueryable<FlowDetailPenerimaanViewModels> GetReportQueryFlow(DateTime? dateFrom, DateTime? dateTo, string unit, string category, int offset, int page, int size)

        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Status = new[] { "" };
            //switch (category)
            //{
            //    case "Bahan Baku":
            //        Status = new[] { "FABRIC", "SUBKON" };
            //        break;
            //    case "Bahan Pendukung":
            //        Status = new[] { "APPLICATION", "BADGES", "BUNGEE CORD", "BUCKLE", "BENANG HANGTAG", "BUTTON", "COLLAR BONE", "CARE LABEL",
            //        "DRAWSTRING", "ELASTIC", "EMBROIDERY", "LAIN-LAIN", "GROSS GRAIN", "GESPER", "HOOK & BAR", "HOOK & EYE",
            //        "INTERLINING", "KNACKS", "ID LABEL", "LABEL", "LACE", "MESS GUSSET", "METAL IGOT", "NECK LABEL",
            //        "PL PACKING", "PADDING", "PEN KRAH", "POLYCORD", "PLISKET", "POLYWOSHER", "PIPING", "PULLER","QUILTING","RIBBON","RIB","RING","STRAPPING BAND",
            //        "SLEEVE HEADER", "SIZE LABEL", "SAMPLE MATERIAL", "SHOULDER PAD", "SPONGE FOAM", "SPINDLE", "STOPPER", "SEWING THREAD","TAPE / DRYTEX","TRIMMING GROOMET","TASSEL","VELCRO","VITTER BAND",
            //        "WADDING", "WAPPEN", "WRAPBAND", "WASH", "ZIPPER","PROCESS",
            //        };
            //        break;
            //    case "Bahan Embalase":
            //        Status = new[] { "ATTENTION NAME", "POLYBAG", "BACK CB", "BENANG KENUR", "BELT", "BIS NAME","BEARING STAMP","BUTTERFLY","CABLE TIES",
            //            "COLLAR CB", "CUFF STUD", "CLIPS", "DOCUMENT", "DOLL", "LAIN - LAIN","FOAM HANGER","FELT","GADGET","GLUE","GARMENT",
            //            "HANDLING", "HANGER", "HOOK", "HEAT TRANSFER", "ISOLASI", "INNER BOX","STAMPED INK","INSERT TAG","KLEM SENG","KARET GELANG","LACKBAND",
            //            "LICENSE SEAL", "LOOP", "INSERT CD/LAYER", "MACHINE", "MOULD", "METAL SLIDER","OUTER BOX","PLASTIC COLLAR","PIN","PLASTIC","PALLET",
            //            "PAPER", "PRINT", "TALI", "SILICA BAG", "SHAVING", "SILICA GEL","GARMENT SAMPLE","SHIPPING MARK","STUDS TRANSFER","SEAL TAG","STICKER",
            //            "STAMP", "STRING", "STATIONARY", "SWATCH CARD", "SIZE CHIP", "TAG","GARMENT TEST","TIE / DASI","TISSUE PAPER","TIGER TAIL",
            //        };
            //        break;
            //    default:
            //        Status = new[] { "" };
            //        break;
            //}
          

            List<FlowDetailPenerimaanViewModels> Data = new List<FlowDetailPenerimaanViewModels>();
			if (unit == "SMP1")
			{
				var productname = (category == null || category == "undefined" ? "" : category);
				category = (category == null || category == "undefined"  ? "": category);

				var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");

				var categories1 = category == "BB" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : category == "BP" ? categories.Where(x => x.CodeRequirement == "BP").Select(x => x.Name).ToArray(): category == "" || category == "undefined" ? categories.Select(x => x.Name).ToArray() : categories.Where(x => x.CodeRequirement == "BE").Select(x => x.Name).ToArray();

				var Query = (from a in dbContext.GarmentUnitReceiptNotes
							 join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
							 join e in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals e.Id
							 join f in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on e.GarmentEPOId equals f.Id
							 join c in dbContext.GarmentInternalPurchaseOrders on e.POId equals c.Id into PO
							 from cc in PO.DefaultIfEmpty()
							 join g in dbContext.GarmentUnitExpenditureNotes on a.UENId equals g.Id into uen
							 from gg in uen.DefaultIfEmpty()
							 where a.IsDeleted == false
								&& b.IsDeleted == false
								&& categories1.Contains(b.ProductName)
								&& a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
								&& a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
								&& a.UnitCode == "SMP1"
							 select new FlowDetailPenerimaanViewModels
							 {
								 kdbarang = b.ProductCode,
								 nmbarang = b.ProductName,
								 nopo = b.POSerialNumber,
								 keterangan = b.ProductRemark,
								 noro = b.RONo,
								 artikel = e.Article,
								 kdbuyer = cc != null ? cc.BuyerCode : "-",
								 nobukti = a.URNNo,
								 tanggal = a.CreatedUtc,
								 jumlahbeli = a.URNType == "PEMBELIAN" ? decimal.ToDouble(b.ReceiptQuantity) : a.URNType == "PROSES" ? decimal.ToDouble(b.ReceiptQuantity) : decimal.ToDouble(b.ReceiptQuantity),
								 satuanbeli = a.URNType == "PEMBELIAN" ? e.DealUomUnit : a.URNType == "PROSES" ? b.UomUnit : b.UomUnit,
								 jumlahterima = Math.Round(decimal.ToDouble(b.ReceiptQuantity) * decimal.ToDouble(b.Conversion), 2),
								 satuanterima = b.SmallUomUnit,
								 jumlah = ((decimal.ToDouble(b.PricePerDealUnit) / (b.Conversion == 0 ? 1 : decimal.ToDouble(b.Conversion))) * b.DOCurrencyRate) * (decimal.ToDouble(b.ReceiptQuantity) * decimal.ToDouble(b.Conversion)),
								 asal = a.URNType == "PROSES" ? a.URNType : a.URNType == "PEMBELIAN" ? "Pembelian Eksternal" : gg.UnitSenderName,
								 Jenis = a.URNType,
								 tipepembayaran = f.PaymentMethod == "FREE FROM BUYER" || f.PaymentMethod == "CMT" || f.PaymentMethod == "CMT / IMPORT" ? "BY" : "BL"

							 });

				var index = 1;
				foreach (var item in Query)
				{

					Data.Add(
						   new FlowDetailPenerimaanViewModels
						   {
							   no = index++,
							   kdbarang = item.kdbarang,
							   nmbarang = item.nmbarang,
							   nopo = item.nopo,
							   keterangan = item.keterangan,
							   noro = item.noro,
							   artikel = item.artikel,
							   kdbuyer = item.kdbuyer,
							   asal = item.asal,
							   Jenis = item.Jenis,
							   nobukti = item.nobukti,
							   tanggal = item.tanggal,
							   jumlahbeli = item.jumlahbeli,
							   satuanbeli = item.satuanbeli,
							   jumlahterima = (double)item.jumlahterima,
							   satuanterima = item.satuanterima,
							   jumlah = item.jumlah,
							   tipepembayaran = item.tipepembayaran

						   });

				}
			}
			else
			{
				var productname = (category == "SUBKON" ? "SUBKON" : "");
				category = (category == "SUBKON" ? "BB" : category);

				var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");

				var categories1 = category == "BB" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : category == "BP" ? categories.Where(x => x.CodeRequirement == "BP").Select(x => x.Name).ToArray() : categories.Where(x => x.CodeRequirement == "BE").Select(x => x.Name).ToArray();

				var Query = (from a in dbContext.GarmentUnitReceiptNotes
							 join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
							 join e in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on b.EPOItemId equals e.Id
							 join f in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on e.GarmentEPOId equals f.Id
							 join c in dbContext.GarmentInternalPurchaseOrders on e.POId equals c.Id into PO
							 from cc in PO.DefaultIfEmpty()
							 join g in dbContext.GarmentUnitExpenditureNotes on a.UENId equals g.Id into uen
							 from gg in uen.DefaultIfEmpty()
							 where a.IsDeleted == false
								&& b.IsDeleted == false
								&& categories1.Contains(b.ProductName)
								&& a.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
								&& a.CreatedUtc.AddHours(offset).Date <= DateTo.Date
								&& a.UnitCode == (string.IsNullOrWhiteSpace(unit) ? a.UnitCode : unit)
							 select new FlowDetailPenerimaanViewModels
							 {
								 kdbarang = b.ProductCode,
								 nmbarang = b.ProductName,
								 nopo = b.POSerialNumber,
								 keterangan = b.ProductRemark,
								 noro = b.RONo,
								 artikel = e.Article,
								 kdbuyer = cc != null ? cc.BuyerCode : "-",
								 nobukti = a.URNNo,
								 tanggal = a.CreatedUtc,
								 jumlahbeli = a.URNType == "PEMBELIAN" ? decimal.ToDouble(b.ReceiptQuantity) : a.URNType == "PROSES" ? decimal.ToDouble(b.ReceiptQuantity) : decimal.ToDouble(b.ReceiptQuantity),
								 satuanbeli = a.URNType == "PEMBELIAN" ? e.DealUomUnit : a.URNType == "PROSES" ? b.UomUnit : b.UomUnit,
								 jumlahterima = Math.Round(decimal.ToDouble(b.ReceiptQuantity) * decimal.ToDouble(b.Conversion), 2),
								 satuanterima = b.SmallUomUnit,
								 jumlah = ((decimal.ToDouble(b.PricePerDealUnit) / (b.Conversion == 0 ? 1 : decimal.ToDouble(b.Conversion))) * b.DOCurrencyRate) * (decimal.ToDouble(b.ReceiptQuantity) * decimal.ToDouble(b.Conversion)),
								 asal = a.URNType == "PROSES" ? a.URNType : a.URNType == "PEMBELIAN" ? "Pembelian Eksternal" : gg.UnitSenderName,
								 Jenis = a.URNType,
								 tipepembayaran = f.PaymentMethod == "FREE FROM BUYER" || f.PaymentMethod == "CMT" || f.PaymentMethod == "CMT / IMPORT" ? "BY" : "BL"

							 });

				var index = 1;
				foreach (var item in Query)
				{

					Data.Add(
						   new FlowDetailPenerimaanViewModels
						   {
							   no = index++,
							   kdbarang = item.kdbarang,
							   nmbarang = item.nmbarang,
							   nopo = item.nopo,
							   keterangan = item.keterangan,
							   noro = item.noro,
							   artikel = item.artikel,
							   kdbuyer = item.kdbuyer,
							   asal = item.asal,
							   Jenis = item.Jenis,
							   nobukti = item.nobukti,
							   tanggal = item.tanggal,
							   jumlahbeli = item.jumlahbeli,
							   satuanbeli = item.satuanbeli,
							   jumlahterima = (double)item.jumlahterima,
							   satuanterima = item.satuanterima,
							   jumlah = item.jumlah,
							   tipepembayaran = item.tipepembayaran

						   });

				}
			}

            return Data.AsQueryable();
        }


        public Tuple<List<FlowDetailPenerimaanViewModels>, int> GetReportFlow(DateTime? dateFrom, DateTime? dateTo, string unit, string category, int page, int size, string Order, int offset)

        {
            var Query = GetReportQueryFlow(dateFrom, dateTo, unit, category, offset, page, size);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.no);
            }


            //Pageable<FlowDetailPenerimaanViewModels> pageable = new Pageable<FlowDetailPenerimaanViewModels>(Query, page - 1, size);
            //List<FlowDetailPenerimaanViewModels> Data = pageable.Data.ToList<FlowDetailPenerimaanViewModels>();
            //int TotalData = pageable.TotalCount;

            return Tuple.Create(Query.ToList(), Query.Count());
        }

        public MemoryStream GenerateExcelFlowForUnit(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, int offset, string unitname)
        {
            var Query = GetReportQueryFlow(dateFrom, dateTo, unit, category, offset, 1, int.MaxValue);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No R/O", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jenis", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bukti", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Beli", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });

            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };


            double ReceiptQtyTotal = 0;
            double PurchaseQtyTotal = 0;
            if (Query.ToArray().Count() == 0)
            {
                //result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, "", 0, ""); // to allow column name to be generated properly for empty data as template
                result.Rows.Add(0, "", "", "", "", "", "", "", "", "", "", 0, "", 0, ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int index = 0;
                foreach (FlowDetailPenerimaanViewModels data in Query)
                {
                    index++;
                    string tgl = data.tanggal == null ? "-" : data.tanggal.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, data.kdbarang, data.nmbarang, data.nopo, data.keterangan, data.noro, data.artikel, data.kdbuyer, data.nobukti, data.asal, tgl, data.jumlahbeli, data.satuanbeli, data.jumlahterima, data.satuanterima);
                    ReceiptQtyTotal += data.jumlahterima;
                    PurchaseQtyTotal += data.jumlahbeli;
                }

            }
            ExcelPackage package = new ExcelPackage();
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(DateTo.Month);
            var sheet = package.Workbook.Worksheets.Add("Report");

            var col = (char)('A' + result.Columns.Count);
            string tglawal = DateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN FLOW PENERIMAAN {0}", categoryname ==null || categoryname =="undefined" ? "":categoryname);
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("KONFEKSI : {0}", (string.IsNullOrWhiteSpace(unitname) ? "ALL" : unitname));
            sheet.Cells[$"A3:{col}3"].Merge = true;

            sheet.Cells["A5"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            var a = Query.Count();
            sheet.Cells[$"A{6 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Merge = true;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"L{6 + a}"].Value = PurchaseQtyTotal;
            sheet.Cells[$"L{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"M{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"N{6 + a}"].Value = ReceiptQtyTotal;
            sheet.Cells[$"N{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"O{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (result, "Report", mergeCells) }, true);
        }

        public MemoryStream GenerateExcelLow(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, int offset, string unitname)
        {
            var Query = GetReportQueryFlow(dateFrom, dateTo, unit, category, offset, 1, int.MaxValue);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No R/O", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jenis", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bukti", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Beli", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Beli", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Metode Pembayaran", DataType = typeof(String) });


            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };


            double ReceiptQtyTotal = 0;
            double PriceReceiptTotal = 0;
            double PurchaseQtyTotal = 0;
            if (Query.ToArray().Count() == 0)
            {
                //result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", 0, "", 0, "", 0, ""); // to allow column name to be generated properly for empty data as template
                result.Rows.Add(0, "", "", "", "", "", "", "", "", "", "", 0, "", 0, "", 0, ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int index = 0;
                foreach (FlowDetailPenerimaanViewModels data in Query)
                {
                    index++;
                    string tgl = data.tanggal == null ? "-" : data.tanggal.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, data.kdbarang, data.nmbarang, data.nopo, data.keterangan, data.noro, data.artikel, data.kdbuyer, data.asal, data.nobukti, tgl, data.jumlahbeli, data.satuanbeli, data.jumlahterima, data.satuanterima, data.jumlah, data.tipepembayaran);
                    ReceiptQtyTotal += data.jumlahterima;
                    PurchaseQtyTotal += data.jumlahbeli;
                    PriceReceiptTotal += (double)data.jumlah;
                }

            }
            ExcelPackage package = new ExcelPackage();
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(DateTo.Month);
            var sheet = package.Workbook.Worksheets.Add("Report");

            var col = (char)('A' + result.Columns.Count);
            string tglawal = DateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN REKAP PENERIMAAN SAMPLE{0}", categoryname);
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("KONFEKSI : {0}", (string.IsNullOrWhiteSpace(unitname) ? "ALL" : unitname));
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A5"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);

            var a = Query.Count();
            sheet.Cells[$"A{6 + a}"].Value = "T O T A L  . . . . . . . . . . . . . . .";
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Merge = true;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.Font.Bold = true;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{6 + a}:K{6 + a}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"L{6 + a}"].Value = PurchaseQtyTotal;
            sheet.Cells[$"L{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"M{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"N{6 + a}"].Value = ReceiptQtyTotal;
            sheet.Cells[$"N{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"O{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"P{6 + a}"].Value = PriceReceiptTotal;
            sheet.Cells[$"P{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            sheet.Cells[$"Q{6 + a}"].Style.Border.BorderAround(ExcelBorderStyle.Medium);


            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (result, "Report", mergeCells) }, true);
        }

        #endregion

        public Tuple<List<GarmentUnitReceiptNoteINReportViewModel>, int> GetReportIN(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset)

        {
            var Query = GetReportQueryIN(dateFrom, dateTo, type, offset);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.NoBUM).ThenBy(b => b.NoPO).ThenBy(b => b.NoSuratJalan).ThenBy(b => b.UNit).ThenBy(b => b.TanggalMasuk).ThenBy(b => b.TanggalBuatBon).ThenBy(b => b.Gudang).ThenBy(b => b.Supplier).ThenBy(b => b.KodeBarang).ThenBy(b => b.NamaBarang);
            }


            Pageable<GarmentUnitReceiptNoteINReportViewModel> pageable = new Pageable<GarmentUnitReceiptNoteINReportViewModel>(Query, page - 1, size);
            List<GarmentUnitReceiptNoteINReportViewModel> Data = pageable.Data.ToList<GarmentUnitReceiptNoteINReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public IQueryable<GarmentUnitReceiptNoteINReportViewModel> GetReportQueryIN(DateTime? dateFrom, DateTime? dateTo, string type, int offset)
        {
            var GudangBP = new[] { "GUDANG ACCESORIES", "GUDANG EMBALANCE", "GUDANG INTERLINING" };
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var coderequirement = new[] { "BP", "BE" };

            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");

            var categories1 = type == "FABRIC" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : type == "NON FABRIC" ? categories.Where(x => coderequirement.Contains(x.CodeRequirement)).Select(x => x.Name).ToArray() : categories.Select(x => x.Name).ToArray();

            var Data1 = type == "FABRIC" ? (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                        join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                        join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                        join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                        //join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RONo equals e.RONo
                        where a.IsDeleted == false && b.IsDeleted == false
                        //&& (type == "FABRIC" ? b.ProductName == "FABRIC" : type == "NON FABRIC" ? b.ProductName != "FABRIC" : b.ProductName == b.ProductName)
                        && categories1.Contains(a.ProductName)
                        && a.ProductName != "PROCESS"
                        && b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                        && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                        select new GarmentUnitReceiptNoteINReportViewModel
                        {
                            NoSuratJalan = b.DONo,
                            NoBUM = b.URNNo,
                            UNit = b.UnitName,
                            TanggalMasuk = b.ReceiptDate,
                            TanggalBuatBon = b.CreatedUtc,
                            Gudang = b.StorageName,
                            AsalTerima = b.URNType,
                            NoPO = a.POSerialNumber,
                            Keterangan = a.ProductRemark,
                            NoRO = a.RONo,
                            JumlahDiterima = Convert.ToDouble(a.ReceiptQuantity),
                            Satuan = a.UomUnit,
                            JumlahKecil = Convert.ToDouble(a.ReceiptQuantity * a.Conversion),
                            NamaBarang = a.ProductName,
                            KodeBarang = a.ProductCode,
                            Supplier = b.SupplierName
                        })
                        :
                        (from a in (from aa in dbContext.GarmentUnitReceiptNoteItems select aa)
                         join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                         join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                         join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                         //join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RONo equals e.RONo
                         where a.IsDeleted == false && b.IsDeleted == false
                         //&& (type == "FABRIC" ? b.ProductName == "FABRIC" : type == "NON FABRIC" ? b.ProductName != "FABRIC" : b.ProductName == b.ProductName)
                         && categories1.Contains(a.ProductName)
                         && a.ProductName != "PROCESS"
                         && b.CreatedUtc.AddHours(offset).Date >= DateFrom.Date
                         && b.CreatedUtc.AddHours(offset).Date <= DateTo.Date
                         select new GarmentUnitReceiptNoteINReportViewModel
                         {
                             NoSuratJalan = b.DONo,
                             NoBUM = b.URNNo,
                             UNit = b.UnitName,
                             TanggalMasuk = b.ReceiptDate,
                             TanggalBuatBon = b.CreatedUtc,
                             Gudang = b.StorageName,
                             AsalTerima = b.URNType,
                             NoPO = a.POSerialNumber,
                             Keterangan = a.ProductRemark,
                             NoRO = a.RONo,
                             JumlahDiterima = Convert.ToDouble(a.ReceiptQuantity),
                             Satuan = a.UomUnit,
                             JumlahKecil = Convert.ToDouble(a.ReceiptQuantity * a.Conversion),
                             NamaBarang = a.ProductName,
                             KodeBarang = a.ProductCode,
                             Supplier = b.SupplierName
                         });

            if (type == "FABRIC")
            {
                Data1 = Data1.Where(x => (x.KodeBarang != "APL001") && (x.KodeBarang != "EMB001") && (x.KodeBarang != "GMT001") && (x.KodeBarang != "PRN001") && (x.KodeBarang != "SMP001") && (x.KodeBarang != "WSH001"));
            }else if(type == "NON FABRIC")
            {
                Data1 = Data1.Where(x => (x.KodeBarang != "APL001") && (x.KodeBarang != "EMB001") && (x.KodeBarang != "GMT001") && (x.KodeBarang != "PRN001") && (x.KodeBarang != "SMP001") && (x.KodeBarang != "WSH001") && (x.KodeBarang != "QLT001") && (x.KodeBarang != "SMT001"));

            }




            var Query = Data1;
            //var Query = type == "FABRIC" ? from a in dbContext.GarmentUnitReceiptNotes
            //                               join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
            //                               where a.IsDeleted == false && b.IsDeleted == false
            //                               && a.StorageName == "GUDANG BAHAN BAKU"
            //                               && a.LastModifiedUtc.Date >= DateFrom.Date
            //                               && a.LastModifiedUtc.Date <= DateTo.Date
            //                               select new GarmentUnitReceiptNoteINReportViewModel
            //                               {
            //                                   NoSuratJalan = a.DONo,
            //                                   NoBUM = a.URNNo,
            //                                   UNit = a.UnitName,
            //                                   TanggalMasuk = a.ReceiptDate,
            //                                   TanggalBuatBon = a.CreatedUtc,
            //                                   Gudang = a.StorageName,
            //                                   AsalTerima = a.URNType,
            //                                   NoPO = b.POSerialNumber,
            //                                   Keterangan = b.ProductRemark,
            //                                   NoRO = b.RONo,
            //                                   JumlahDiterima = Convert.ToDouble(b.ReceiptQuantity),
            //                                   Satuan = b.UomUnit,
            //                                   JumlahKecil = Convert.ToDouble(b.SmallQuantity),
            //                                   NamaBarang = b.ProductName,
            //                                   KodeBarang = b.ProductCode,
            //                                   Supplier = a.SupplierName
            //                               }
            //            : type == "NON FABRIC" ? from a in dbContext.GarmentUnitReceiptNotes
            //                                     join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
            //                                     where a.IsDeleted == false && b.IsDeleted == false
            //                                     && a.StorageName != "GUDANG BAHAN BAKU"
            //                                     && a.LastModifiedUtc.Date >= DateFrom.Date
            //                                     && a.LastModifiedUtc.Date <= DateTo.Date
            //                                     select new GarmentUnitReceiptNoteINReportViewModel
            //                                     {
            //                                         NoSuratJalan = a.DONo,
            //                                         NoBUM = a.URNNo,
            //                                         UNit = a.UnitName,
            //                                         TanggalMasuk = a.ReceiptDate,
            //                                         TanggalBuatBon = a.CreatedUtc,
            //                                         Gudang = a.StorageName,
            //                                         AsalTerima = a.URNType,
            //                                         NoPO = b.POSerialNumber,
            //                                         Keterangan = b.ProductRemark,
            //                                         NoRO = b.RONo,
            //                                         JumlahDiterima = Convert.ToDouble(b.ReceiptQuantity),
            //                                         Satuan = b.UomUnit,
            //                                         JumlahKecil = Convert.ToDouble(b.SmallQuantity),
            //                                         NamaBarang = b.ProductName,
            //                                         KodeBarang = b.ProductCode,
            //                                         Supplier = a.SupplierName
            //                                     }
            //                                     : from a in dbContext.GarmentUnitReceiptNotes
            //                                       join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
            //                                       where a.IsDeleted == false && b.IsDeleted == false
            //                                       && a.StorageName == a.StorageName
            //                                       && a.LastModifiedUtc.Date >= DateFrom.Date
            //                                       && a.LastModifiedUtc.Date <= DateTo.Date
            //                                       select new GarmentUnitReceiptNoteINReportViewModel
            //                                       {
            //                                           NoSuratJalan = a.DONo,
            //                                           NoBUM = a.URNNo,
            //                                           UNit = a.UnitName,
            //                                           TanggalMasuk = a.ReceiptDate,
            //                                           TanggalBuatBon = a.CreatedUtc,
            //                                           Gudang = a.StorageName,
            //                                           AsalTerima = a.URNType,
            //                                           NoPO = b.POSerialNumber,
            //                                           Keterangan = b.ProductRemark,
            //                                           NoRO = b.RONo,
            //                                           JumlahDiterima = Convert.ToDouble(b.ReceiptQuantity),
            //                                           Satuan = b.UomUnit,
            //                                           JumlahKecil = Convert.ToDouble(b.SmallQuantity),
            //                                           NamaBarang = b.ProductName,
            //                                           KodeBarang = b.ProductCode,
            //                                           Supplier = a.SupplierName
            //                                       };
            return Query.AsQueryable();

        }

        public MemoryStream GenerateExcelMonIN(DateTime? dateFrom, DateTime? dateTo, string category, int offset)
        {
            var Query = GetReportQueryIN(dateFrom, dateTo, category, offset);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUM", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Masuk", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Buat Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Gudang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Diterima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Kecil", DataType = typeof(String) });


            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int index = 0;
                foreach (GarmentUnitReceiptNoteINReportViewModel data in Query)
                {
                    index++;
                    string tgl1 = data.TanggalMasuk == null ? "-" : data.TanggalMasuk.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string tgl2 = data.TanggalBuatBon == null ? "-" : data.TanggalBuatBon.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, data.NoBUM, data.NoPO, data.NoSuratJalan, data.UNit, tgl1, data.TanggalBuatBon, data.Gudang, data.Supplier, data.AsalTerima, data.KodeBarang, data.NamaBarang, data.Keterangan, data.NoRO, data.JumlahDiterima, data.Satuan, data.JumlahKecil);

                }

            }

            return Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (result, "Report", mergeCells) }, true);
        }

        //public async Task<int> UrnDateRevise(List<long> ids, string user, DateTime reviseDate)
        //{
        //    int Updated = 0;
        //    using (var transaction = this.dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            List<GarmentUenUrnChangeDateHistory> histories = new List<GarmentUenUrnChangeDateHistory>();
        //            //var Ids = listURN.Select(d => d.Id).ToList();
        //            //var Id = listURN.Single().Id;
        //            var listData = this.dbSet
        //                .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
        //                .ToList();


        //            listData.ForEach(m =>
        //            {
        //                EntityExtension.FlagForUpdate(m, user, "Facade");
        //                m.CreatedUtc = reviseDate;

        //                //GarmentUenUrnChangeDateHistory changeDateHistory = new GarmentUenUrnChangeDateHistory
        //                //{
        //                //    DateOld = m.CreatedUtc,
        //                //    DateNow = reviseDate,
        //                //    DocumentNo = m.URNNo,

        //                //};

        //                //EntityExtension.FlagForCreate(changeDateHistory, user, "Facade");
        //                ////dbSetUenUrnChangeDate.Add(changeDateHistory);

        //                //histories.Add(changeDateHistory);
        //                Updated = await dbContext.SaveChangesAsync();

        //            });


        //            transaction.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    return Updated;
        //}

        //public int UrnDateRevise(List<long> ids, string user, DateTime reviseDate)
        //{
        //    int Updated = 0;
        //    using (var transaction = this.dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            //var Ids = ListEPO.Select(d => d.Id).ToList();
        //            var listData = this.dbSet
        //                .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
        //                .Include(d => d.Items)
        //                .ToList();
        //            listData.ForEach(m =>
        //            {
        //                EntityExtension.FlagForUpdate(m, user, "Facade");


        //                GarmentUenUrnChangeDateHistory changeDateHistory = new GarmentUenUrnChangeDateHistory
        //                {
        //                    DateOld = m.CreatedUtc,
        //                    DateNow = reviseDate,
        //                    DocumentNo = m.URNNo,

        //                };

        //                m.CreatedUtc = reviseDate;

        //                EntityExtension.FlagForCreate(changeDateHistory, user, "Facade");
        //                dbSetUenUrnChangeDate.Add(changeDateHistory);

        //            });

        //            Updated = dbContext.SaveChanges();
        //            transaction.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            transaction.Rollback();
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    return Updated;
        //}
        public int UrnDateRevise(List<long> ids, string user, DateTime reviseDate)
        {
            int Updated = 0;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {

                    //var listData = this.dbSet
                    //    .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
                    //    .Include(d => d.Items)
                    //    .ToList();
                    var listdata = (from a in dbContext.GarmentDOItems
                                    join b in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals b.Id
                                    join c in dbContext.GarmentUnitReceiptNotes on b.URNId equals c.Id
                                    where ids.Contains(c.Id)
                                    select a).Distinct().ToList();

                    var listdata2 = (from a in dbContext.GarmentDOItems
                                     join b in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals b.Id
                                     join c in dbContext.GarmentUnitReceiptNotes on b.URNId equals c.Id
                                     where ids.Contains(c.Id)
                                     select b).Distinct().ToList();

                    var listdata3 = (from a in dbContext.GarmentDOItems
                                     join b in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals b.Id
                                     join c in dbContext.GarmentUnitReceiptNotes on b.URNId equals c.Id
                                     where ids.Contains(c.Id)
                                     select c).Distinct().ToList();

                    listdata.ForEach(c =>
                    {
                        EntityExtension.FlagForUpdate(c, user, "Facade");
                        c.CreatedUtc = reviseDate;
                    });

                    listdata2.ForEach(c =>
                    {
                        EntityExtension.FlagForUpdate(c, user, "Facade");
                        c.CreatedUtc = reviseDate;                     
                    });

                    listdata3.ForEach(c =>
                    {
                        EntityExtension.FlagForUpdate(c, user, "Facade");


                        GarmentUenUrnChangeDateHistory changeDateHistory = new GarmentUenUrnChangeDateHistory
                        {
                            DateOld = c.CreatedUtc,
                            DateNow = reviseDate,
                            DocumentNo = c.URNNo,

                        };

                        c.CreatedUtc = reviseDate;

                        EntityExtension.FlagForCreate(changeDateHistory, user, "Facade");
                        dbSetUenUrnChangeDate.Add(changeDateHistory);

                    });

                    Updated = dbContext.SaveChanges();
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




    }
}
