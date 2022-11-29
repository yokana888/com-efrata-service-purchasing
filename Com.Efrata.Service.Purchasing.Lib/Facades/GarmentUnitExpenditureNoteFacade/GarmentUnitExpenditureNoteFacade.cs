using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUenUrnChangeDateHistory;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade
{
    public class GarmentUnitExpenditureNoteFacade : IGarmentUnitExpenditureNoteFacade
    {
        private readonly string USER_AGENT = "Facade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentUnitExpenditureNote> dbSet;
        private readonly DbSet<GarmentUnitExpenditureNoteItem> dbSetItem;
        private readonly DbSet<GarmentUnitDeliveryOrder> dbSetGarmentUnitDeliveryOrder;
        private readonly DbSet<GarmentUnitDeliveryOrderItem> dbSetGarmentUnitDeliveryOrderItem;
        private readonly DbSet<GarmentInventoryDocument> dbSetGarmentInventoryDocument;
        private readonly DbSet<GarmentInventoryMovement> dbSetGarmentInventoryMovement;
        private readonly DbSet<GarmentInventorySummary> dbSetGarmentInventorySummary;
        private readonly DbSet<GarmentUnitReceiptNoteItem> dbSetGarmentUnitReceiptNoteItem;
        private readonly DbSet<GarmentCorrectionNote> dbSetGarmentCorrectionNote;
        private readonly DbSet<GarmentExternalPurchaseOrder> dbSetGarmentExternalPurchaseOrder;
        private readonly DbSet<GarmentUnitReceiptNote> dbSetGarmentUnitReceiptNote;
        private readonly DbSet<GarmentDOItems> dbSetGarmentDOItems;
        private readonly DbSet<GarmentUenUrnChangeDateHistory> dbSetUenUrnChangeDate;

        //private GarmentReturnCorrectionNoteFacade garmentReturnCorrectionNoteFacade;


        private readonly IMapper mapper;

        public GarmentUnitExpenditureNoteFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentUnitExpenditureNote>();
            dbSetItem = dbContext.Set<GarmentUnitExpenditureNoteItem>();
            dbSetGarmentInventoryDocument = dbContext.Set<GarmentInventoryDocument>();
            dbSetGarmentInventoryMovement = dbContext.Set<GarmentInventoryMovement>();
            dbSetGarmentInventorySummary = dbContext.Set<GarmentInventorySummary>();
            dbSetGarmentUnitDeliveryOrder = dbContext.Set<GarmentUnitDeliveryOrder>();
            dbSetGarmentUnitDeliveryOrderItem = dbContext.Set<GarmentUnitDeliveryOrderItem>();
            dbSetGarmentUnitReceiptNoteItem = dbContext.Set<GarmentUnitReceiptNoteItem>();
            dbSetGarmentCorrectionNote= dbContext.Set<GarmentCorrectionNote>();
            dbSetGarmentExternalPurchaseOrder= dbContext.Set<GarmentExternalPurchaseOrder>();
            dbSetGarmentUnitReceiptNote= dbContext.Set<GarmentUnitReceiptNote>();
            dbSetGarmentDOItems= dbContext.Set<GarmentDOItems>();
            dbSetUenUrnChangeDate = dbContext.Set<GarmentUenUrnChangeDateHistory>();

            mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            //this.garmentReturnCorrectionNoteFacade = (GarmentReturnCorrectionNoteFacade)serviceProvider.GetService(typeof(GarmentReturnCorrectionNoteFacade));
        }

        public async Task<int> Create(GarmentUnitExpenditureNote garmentUnitExpenditureNote)
        {
            int Created = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    garmentUnitExpenditureNote.IsPreparing = false;
                    EntityExtension.FlagForCreate(garmentUnitExpenditureNote, identityService.Username, USER_AGENT);
                    garmentUnitExpenditureNote.UENNo = await GenerateNo(garmentUnitExpenditureNote);
                    var garmentUnitDeliveryOrder = dbSetGarmentUnitDeliveryOrder.First(d => d.Id == garmentUnitExpenditureNote.UnitDOId);
                    EntityExtension.FlagForUpdate(garmentUnitDeliveryOrder, identityService.Username, USER_AGENT);
                    garmentUnitDeliveryOrder.IsUsed = true;

                    var UnitDO = dbContext.GarmentUnitDeliveryOrders.Include(d=>d.Items).FirstOrDefault(d => d.Id.Equals(garmentUnitExpenditureNote.UnitDOId));
                    foreach (var unitDOItem in UnitDO.Items)
                    {
                        var unitExSaved = garmentUnitExpenditureNote.Items.FirstOrDefault(d => d.UnitDOItemId == unitDOItem.Id);
                        if (unitExSaved == null || unitExSaved.IsSave == false)
                        {
                            GarmentDOItems garmentDOItems = dbSetGarmentDOItems.FirstOrDefault(u => u.URNItemId == unitDOItem.URNItemId);
                            garmentDOItems.RemainingQuantity += (decimal)unitDOItem.Quantity;

                            var garmentUnitReceiptNoteItem = dbSetGarmentUnitReceiptNoteItem.FirstOrDefault(u => u.Id == unitDOItem.URNItemId);
                            EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                            garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)unitDOItem.Quantity;
                            unitDOItem.Quantity = 0;
                        }
                    }
                    var garmentUnitExpenditureNoteItems = garmentUnitExpenditureNote.Items.Where(x => x.IsSave).ToList();
                    foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNoteItems)
                    {
                        EntityExtension.FlagForCreate(garmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);

                        var garmentUnitDeliveryOrderItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(s => s.Id == garmentUnitExpenditureNoteItem.UnitDOItemId);
                        var garmentUnitReceiptNoteItem = dbSetGarmentUnitReceiptNoteItem.FirstOrDefault(u => u.Id == garmentUnitExpenditureNoteItem.URNItemId);
                        if (garmentUnitDeliveryOrderItem != null && garmentUnitReceiptNoteItem != null)
                        {
                            if (garmentUnitDeliveryOrderItem.Quantity != garmentUnitExpenditureNoteItem.Quantity)
                            {
                                EntityExtension.FlagForUpdate(garmentUnitDeliveryOrderItem, identityService.Username, USER_AGENT);
                                garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - ((decimal)garmentUnitDeliveryOrderItem.Quantity - (decimal)garmentUnitExpenditureNoteItem.Quantity);
                                garmentUnitDeliveryOrderItem.Quantity = garmentUnitExpenditureNoteItem.Quantity;

                                GarmentDOItems garmentDOItems = dbSetGarmentDOItems.FirstOrDefault(u => u.URNItemId == garmentUnitDeliveryOrderItem.URNItemId);
                                garmentDOItems.RemainingQuantity = garmentDOItems.RemainingQuantity + ((decimal)garmentUnitDeliveryOrderItem.Quantity - (decimal)garmentUnitExpenditureNoteItem.Quantity);
                            }
                            garmentUnitExpenditureNoteItem.DOCurrencyRate = garmentUnitDeliveryOrderItem.DOCurrencyRate == null ? 0 : garmentUnitDeliveryOrderItem.DOCurrencyRate;
                            if (garmentUnitExpenditureNoteItem.DOCurrencyRate == 0)
                            {
                                throw new Exception("garmentUnitExpenditureNoteItem.DOCurrencyRate tidak boleh 0");
                            }

                            garmentUnitExpenditureNoteItem.Conversion = garmentUnitReceiptNoteItem.Conversion;
                            //var basicPrice = (garmentUnitExpenditureNoteItem.PricePerDealUnit * Math.Round(garmentUnitExpenditureNoteItem.Quantity / (double)garmentUnitExpenditureNoteItem.Conversion, 2) * garmentUnitExpenditureNoteItem.DOCurrencyRate) / (double)garmentUnitExpenditureNoteItem.Conversion;
                            var basicPrice = (garmentUnitExpenditureNoteItem.PricePerDealUnit * garmentUnitExpenditureNoteItem.DOCurrencyRate);
                            garmentUnitExpenditureNoteItem.BasicPrice = Math.Round((decimal)basicPrice, 4);
                        }
                    }

                    //var garmentUENIsSaveFalse = garmentUnitExpenditureNote.Items.Where(d => d.IsSave == false).ToList();

                    //if (garmentUENIsSaveFalse.Count > 0)
                    //{
                    //    foreach (var itemFalseIsSave in garmentUENIsSaveFalse)
                    //    {
                    //        var garmentUnitDeliveryOrderItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(s => s.Id == itemFalseIsSave.UnitDOItemId);
                    //        var garmentUnitReceiptNoteItem = dbSetGarmentUnitReceiptNoteItem.FirstOrDefault(u => u.Id == itemFalseIsSave.URNItemId);
                    //        EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                    //        garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)itemFalseIsSave.Quantity;
                    //        garmentUnitDeliveryOrderItem.Quantity = 0;
                    //    }
                    //}
                    

                    if (garmentUnitExpenditureNote.ExpenditureType != "TRANSFER")
                    {
                        var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                        foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                        {
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "OUT");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                            if (garmentInventorySummaryExisting == null)
                            {
                                var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovement);
                                dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                            }
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    GarmentCorrectionNote Correction = null;
                    bool suppType = true;
                    List<GarmentExternalPurchaseOrder> newEPOList = new List<GarmentExternalPurchaseOrder>();

                    dbSet.Add(garmentUnitExpenditureNote);

                    Created = await dbContext.SaveChangesAsync();

                    if (garmentUnitExpenditureNote.ExpenditureType == "EXTERNAL" && garmentUnitExpenditureNote.ExpenditureTo== "PEMBELIAN")
                    {
                        List<long> epoItemIds = new List<long>();
                        List<long> epoIds = new List<long>();
                        GarmentDeliveryOrder garmentDeliveryOrder = dbContext.GarmentDeliveryOrders.FirstOrDefault(d => d.Id.Equals(garmentUnitDeliveryOrder.DOId));
                        
                        List<GarmentCorrectionNoteItem> correctionNoteItems = new List<GarmentCorrectionNoteItem>();
                        decimal totalPrice = 0;
                        foreach (var item in garmentUnitExpenditureNote.Items)
                        {
                            GarmentUnitDeliveryOrderItem garmentUnitDeliveryOrderItem = dbContext.GarmentUnitDeliveryOrderItems.FirstOrDefault(d => d.Id == item.UnitDOItemId);
                            GarmentDeliveryOrderDetail garmentDeliveryOrderDetail = dbContext.GarmentDeliveryOrderDetails.FirstOrDefault(d => d.Id.Equals(item.DODetailId));
                            GarmentDeliveryOrderItem garmentDeliveryOrderItem = dbContext.GarmentDeliveryOrderItems.FirstOrDefault(d => d.Id.Equals(garmentDeliveryOrderDetail.GarmentDOItemId));

                            double priceTotalAfter = Math.Round((Math.Round(garmentUnitDeliveryOrderItem.ReturQuantity,2) *(double) garmentDeliveryOrderDetail.PricePerDealUnitCorrection),2);
                            garmentDeliveryOrderDetail.PriceTotalCorrection = (garmentDeliveryOrderDetail.QuantityCorrection - garmentDeliveryOrderDetail.ReturQuantity) * garmentDeliveryOrderDetail.PricePerDealUnitCorrection;

                            GarmentCorrectionNoteItem correctionNoteItem = new GarmentCorrectionNoteItem
                            {
                                POId = garmentDeliveryOrderDetail.POId,
                                EPOId = garmentDeliveryOrderItem.EPOId,
                                DODetailId = item.DODetailId,
                                EPONo = garmentDeliveryOrderItem.EPONo,
                                PRId = garmentDeliveryOrderDetail.PRId,
                                POSerialNumber = garmentDeliveryOrderDetail.POSerialNumber,
                                RONo = garmentDeliveryOrderDetail.RONo,
                                PricePerDealUnitAfter = (decimal)garmentDeliveryOrderDetail.PricePerDealUnitCorrection,
                                PricePerDealUnitBefore = (decimal)garmentDeliveryOrderDetail.PricePerDealUnitCorrection,
                                PriceTotalBefore = (decimal)garmentDeliveryOrderDetail.PriceTotalCorrection,
                                PriceTotalAfter = (decimal)priceTotalAfter,
                                Quantity = (decimal)garmentUnitDeliveryOrderItem.ReturQuantity,
                                UomId = (long)garmentUnitDeliveryOrderItem.ReturUomId,
                                UomIUnit = garmentUnitDeliveryOrderItem.ReturUomUnit,
                                PRNo = garmentDeliveryOrderDetail.PRNo,
                                ProductCode = item.ProductCode,
                                ProductId = item.ProductId,
                                ProductName = item.ProductName
                            };
                            correctionNoteItems.Add(correctionNoteItem);
                            totalPrice += correctionNoteItem.PriceTotalAfter;

                            epoItemIds.Add(garmentUnitDeliveryOrderItem.EPOItemId);
                            if (!epoIds.Contains(garmentDeliveryOrderItem.EPOId))
                            {
                                epoIds.Add(garmentDeliveryOrderItem.EPOId);
                            }
                        }

                        List<string> listEpoNo = new List<string>();

                        foreach (var epoId in epoIds)
                        {
                            GarmentExternalPurchaseOrder garmentExternalPurchaseOrder = dbContext.GarmentExternalPurchaseOrders.FirstOrDefault(p => p.Id.Equals(epoId));
                            
                            List<long> garmentExternalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.Where(p => p.GarmentEPOId.Equals(epoId)).Select(p => p.Id).ToList();
                            List<GarmentExternalPurchaseOrderItem> epoItems = new List<GarmentExternalPurchaseOrderItem>();
                            foreach (var garmentExternalPurchaseOrderItemId in garmentExternalPurchaseOrderItems)
                            {
                                if (epoItemIds.Contains(garmentExternalPurchaseOrderItemId))
                                {
                                    var BUKItem = garmentUnitExpenditureNote.Items.FirstOrDefault(a => a.EPOItemId == garmentExternalPurchaseOrderItemId);
                                    GarmentExternalPurchaseOrderItem garmentExternalPurchaseOrderItem = dbContext.GarmentExternalPurchaseOrderItems.FirstOrDefault(p => p.Id.Equals(garmentExternalPurchaseOrderItemId));
                                    GarmentExternalPurchaseOrderItem newItem = new GarmentExternalPurchaseOrderItem
                                    {
                                        BudgetPrice = garmentExternalPurchaseOrderItem.BudgetPrice,
                                        Conversion = garmentExternalPurchaseOrderItem.Conversion,
                                        DealQuantity = BUKItem.Quantity/(double) BUKItem.Conversion,
                                        DealUomId = garmentExternalPurchaseOrderItem.DealUomId,
                                        DealUomUnit = garmentExternalPurchaseOrderItem.DealUomUnit,
                                        DefaultQuantity = BUKItem.Quantity / (double)BUKItem.Conversion,
                                        DefaultUomId = garmentExternalPurchaseOrderItem.DefaultUomId,
                                        DefaultUomUnit = garmentExternalPurchaseOrderItem.DefaultUomUnit,
                                        DOQuantity = 0,
                                        IsOverBudget = false,
                                        OverBudgetRemark = garmentExternalPurchaseOrderItem.OverBudgetRemark,
                                        POId = garmentExternalPurchaseOrderItem.POId,
                                        PONo = garmentExternalPurchaseOrderItem.PONo,
                                        PO_SerialNumber = garmentExternalPurchaseOrderItem.PO_SerialNumber + "-R",
                                        PricePerDealUnit = garmentExternalPurchaseOrderItem.PricePerDealUnit,
                                        PRId = garmentExternalPurchaseOrderItem.PRId,
                                        PRNo = garmentExternalPurchaseOrderItem.PRNo,
                                        ProductCode = garmentExternalPurchaseOrderItem.ProductCode,
                                        ProductId = garmentExternalPurchaseOrderItem.ProductId,
                                        ProductName = garmentExternalPurchaseOrderItem.ProductName,
                                        ReceiptQuantity = 0,
                                        Remark = garmentExternalPurchaseOrderItem.Remark,
                                        RONo = garmentExternalPurchaseOrderItem.RONo,
                                        SmallQuantity = BUKItem.Quantity,
                                        SmallUomId = garmentExternalPurchaseOrderItem.SmallUomId,
                                        SmallUomUnit = garmentExternalPurchaseOrderItem.SmallUomUnit,
                                        UsedBudget = 0,
                                        Article = garmentExternalPurchaseOrderItem.Article,
                                        UENItemId = BUKItem.Id

                                    };

                                    if (!(garmentExternalPurchaseOrder.PaymentMethod == "CMT" || garmentExternalPurchaseOrder.PaymentMethod == "FREE FROM BUYER") || !(garmentExternalPurchaseOrder.PaymentType == "FREE" || garmentExternalPurchaseOrder.PaymentType == "EX MASTER FREE"))
                                    {
                                        newItem.UsedBudget = BUKItem.Quantity / (double)BUKItem.Conversion * garmentExternalPurchaseOrderItem.PricePerDealUnit * garmentExternalPurchaseOrder.BudgetRate;
                                    }

                                    epoItems.Add(newItem);
                                }
                            }
                            var EPONo = garmentExternalPurchaseOrder.EPONo + "-R";
                            GarmentExternalPurchaseOrder existEPO = dbContext.GarmentExternalPurchaseOrders.Where(p => p.EPONo.StartsWith(EPONo)).OrderByDescending(o => o.EPONo).FirstOrDefault();
                            if(listEpoNo.Count > 0)
                            {
                                string lastEpo = listEpoNo.Last();
                                var LastEPONo= lastEpo.Substring(lastEpo.IndexOf("R")+1);
                                int lastNo= Int32.Parse(LastEPONo) + 1;
                                EPONo = EPONo + lastNo.ToString();
                                listEpoNo.Add(EPONo);
                            }
                            else if (existEPO != null )
                            {
                                var LastEPONo = existEPO.EPONo.Substring(existEPO.EPONo.IndexOf("R") + 1);
                                int lastNo = Int32.Parse(LastEPONo) + 1;
                                EPONo = EPONo + lastNo.ToString();
                                //int lastNoNumber = Int32.Parse(existEPO.EPONo.Replace(EPONo, string.Empty)) + 1;
                                //EPONo = EPONo + lastNoNumber.ToString();
                                listEpoNo.Add(EPONo);
                            }
                            else
                            {
                                EPONo = EPONo + "1";
                                listEpoNo.Add(EPONo);
                            }
                            GarmentExternalPurchaseOrder newEpo = new GarmentExternalPurchaseOrder
                            {
                                CreatedBy = garmentExternalPurchaseOrder.CreatedBy,
                                EPONo = EPONo,
                                Category = garmentExternalPurchaseOrder.Category,
                                CurrencyCode = garmentExternalPurchaseOrder.CurrencyCode,
                                CurrencyId = garmentExternalPurchaseOrder.CurrencyId,
                                CurrencyRate = garmentExternalPurchaseOrder.CurrencyRate,
                                DarkPerspiration = garmentExternalPurchaseOrder.DarkPerspiration,
                                DeliveryDate = garmentExternalPurchaseOrder.DeliveryDate,
                                DryRubbing = garmentExternalPurchaseOrder.DryRubbing,
                                FreightCostBy = garmentExternalPurchaseOrder.FreightCostBy,
                                IncomeTaxId = garmentExternalPurchaseOrder.IncomeTaxId,
                                IncomeTaxName = garmentExternalPurchaseOrder.IncomeTaxName,
                                IncomeTaxRate = garmentExternalPurchaseOrder.IncomeTaxRate,
                                IsApproved = garmentExternalPurchaseOrder.IsApproved,
                                IsCanceled = garmentExternalPurchaseOrder.IsCanceled,
                                IsClosed = garmentExternalPurchaseOrder.IsClosed,
                                IsIncomeTax = garmentExternalPurchaseOrder.IsIncomeTax,
                                IsOverBudget = false,
                                IsPosted = true,
                                IsUseVat = garmentExternalPurchaseOrder.IsUseVat,
                                LightMedPerspiration = garmentExternalPurchaseOrder.LightMedPerspiration,
                                OrderDate = garmentExternalPurchaseOrder.OrderDate,
                                PaymentDueDays = garmentExternalPurchaseOrder.PaymentDueDays,
                                PaymentMethod = garmentExternalPurchaseOrder.PaymentMethod,
                                PaymentType = garmentExternalPurchaseOrder.PaymentType,
                                PieceLength = garmentExternalPurchaseOrder.PieceLength,
                                QualityStandardType = garmentExternalPurchaseOrder.QualityStandardType,
                                Remark = garmentExternalPurchaseOrder.Remark,
                                Shrinkage = garmentExternalPurchaseOrder.Shrinkage,
                                SupplierCode = garmentExternalPurchaseOrder.SupplierCode,
                                SupplierId = garmentExternalPurchaseOrder.SupplierId,
                                SupplierImport = garmentExternalPurchaseOrder.SupplierImport,
                                SupplierName = garmentExternalPurchaseOrder.SupplierName,
                                Washing = garmentExternalPurchaseOrder.Washing,
                                WetRubbing = garmentExternalPurchaseOrder.WetRubbing,
                                Items = epoItems,
                                UENId = garmentUnitExpenditureNote.Id,
                                BudgetRate = garmentExternalPurchaseOrder.BudgetRate,
                                VatId = garmentExternalPurchaseOrder.VatId,
                                VatRate = garmentExternalPurchaseOrder.VatRate
                            };

                            suppType = garmentExternalPurchaseOrder.SupplierImport;

                            newEPOList.Add(newEpo);
                        }

                        Correction = new GarmentCorrectionNote
                        {
                            CorrectionDate = garmentUnitExpenditureNote.ExpenditureDate,
                            CorrectionType = "Retur",
                            DOId = garmentDeliveryOrder.Id,
                            DONo = garmentDeliveryOrder.DONo,
                            CurrencyCode = garmentDeliveryOrder.DOCurrencyCode,
                            CurrencyId = (long)garmentDeliveryOrder.DOCurrencyId,
                            IncomeTaxId = (long)garmentDeliveryOrder.IncomeTaxId,
                            IncomeTaxName = garmentDeliveryOrder.IncomeTaxName,
                            IncomeTaxRate = (decimal)garmentDeliveryOrder.IncomeTaxRate,
                            SupplierCode = garmentDeliveryOrder.SupplierCode,
                            SupplierId = garmentDeliveryOrder.SupplierId,
                            SupplierName = garmentDeliveryOrder.SupplierName,
                            UseIncomeTax = (bool)garmentDeliveryOrder.UseIncomeTax,
                            UseVat = (bool)garmentDeliveryOrder.UseVat,
                            TotalCorrection = totalPrice,
                            Items = correctionNoteItems
                        };

                        //await this.garmentReturnCorrectionNoteFacade.Create(Correction, suppType, identityService.Username);
                        #region Correction
                        var epoFacade = new GarmentExternalPurchaseOrderFacades.GarmentExternalPurchaseOrderFacade(serviceProvider, dbContext);
                        var correctionFacade = new GarmentReturnCorrectionNoteFacade(this.serviceProvider, dbContext);

                        EntityExtension.FlagForCreate(Correction, identityService.Username, USER_AGENT);

                        Correction.CorrectionNo = await correctionFacade.GenerateNo(Correction, suppType, 7);
                        //Correction.TotalCorrection = (garmentCorrectionNote.Items.Sum(i => i.PriceTotalAfter)) * (-1);
                        Correction.NKPH = "";
                        Correction.NKPN = "";
                        if (Correction.UseIncomeTax == true)
                        {
                            Correction.NKPH = await correctionFacade.GenerateNKPH(Correction, 7);
                        }
                        if (Correction.UseVat == true)
                        {
                            Correction.NKPN = await correctionFacade.GenerateNKPN(Correction, 7);
                        }

                        garmentDeliveryOrder.IsCorrection = true;

                        EntityExtension.FlagForUpdate(garmentDeliveryOrder, identityService.Username, USER_AGENT);

                        foreach (var item in correctionNoteItems)
                        {
                            item.Quantity = item.Quantity * (-1);
                            item.PriceTotalAfter = item.PriceTotalAfter *(-1);
                            EntityExtension.FlagForCreate(item, identityService.Username, USER_AGENT);

                            //var garmentDeliveryOrderDetail = dbContext.GarmentDeliveryOrderDetails.First(d => d.Id == item.DODetailId);
                            //var epoDetail = dbContext.GarmentExternalPurchaseOrderItems.First(d => d.Id == garmentDeliveryOrderDetail.EPOItemId);
                            ////garmentDeliveryOrderDetail.QuantityCorrection = ((double)item.Quantity * (-1)) + garmentDeliveryOrderDetail.QuantityCorrection;
                            ////garmentDeliveryOrderDetail.PriceTotalCorrection = garmentDeliveryOrderDetail.QuantityCorrection * garmentDeliveryOrderDetail.PricePerDealUnitCorrection;
                            ////18/02/20 *update by mb Nila
                            ////garmentDeliveryOrderDetail.ReturQuantity = garmentDeliveryOrderDetail.ReturQuantity + ((double)item.Quantity * (-1));

                            //epoDetail.DOQuantity = epoDetail.DOQuantity + (double)item.Quantity;
                            //EntityExtension.FlagForUpdate(garmentDeliveryOrderDetail, identityService.Username, USER_AGENT);
                        }
                        dbSetGarmentCorrectionNote.Add(Correction);
                        #endregion

                        #region EPO
                        foreach (var epo in newEPOList)
                        {
                            var prevCreatedBy = epo.CreatedBy;

                            EntityExtension.FlagForCreate(epo, identityService.Username, USER_AGENT);
                            epo.CreatedBy = prevCreatedBy;

                            foreach (var item in epo.Items)
                            {
                                EntityExtension.FlagForCreate(item, identityService.Username, USER_AGENT);
                                item.CreatedBy = prevCreatedBy;
                            }
                            dbSetGarmentExternalPurchaseOrder.Add(epo);
                        }

                        #endregion

                        await dbContext.SaveChangesAsync();

                        garmentUnitDeliveryOrder.CorrectionId = Correction.Id;
                        garmentUnitDeliveryOrder.CorrectionNo = Correction.CorrectionNo;

                    }

                    if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" || (garmentUnitExpenditureNote.ExpenditureType == "SAMPLE" && garmentUnitExpenditureNote.UnitSenderCode != "SMP1"))
                    {
                        GarmentUnitReceiptNoteFacade garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(this.serviceProvider, dbContext);
                        GarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(dbContext, this.serviceProvider);
                        var oldGarmentUnitDO = dbSetGarmentUnitDeliveryOrder.AsNoTracking().First(d => d.Id == garmentUnitExpenditureNote.UnitDOId);

                        List<GarmentUnitReceiptNoteItem> urnItems = new List<GarmentUnitReceiptNoteItem>();
                        foreach (var item in garmentUnitExpenditureNote.Items)
                        {
                            GarmentPurchaseRequestItem garmentPurchaseRequestItem = item.PRItemId == 0 ? null : dbContext.GarmentPurchaseRequestItems.AsNoTracking().FirstOrDefault(a => a.Id == item.PRItemId);
                            GarmentPurchaseRequest garmentPurchaseRequest = item.PRItemId == 0 ? null : dbContext.GarmentPurchaseRequests.AsNoTracking().FirstOrDefault(a => a.Id == garmentPurchaseRequestItem.GarmentPRId);
                            GarmentInternalPurchaseOrderItem garmentInternalPurchaseOrderItem = dbContext.GarmentInternalPurchaseOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == item.POItemId);
                            GarmentUnitDeliveryOrderItem garmentUnitDeliveryOrderItem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == item.UnitDOItemId);
                            GarmentUnitReceiptNoteItem OldurnItem = dbContext.GarmentUnitReceiptNoteItems.AsNoTracking().FirstOrDefault(a => a.Id == item.URNItemId);
                            //var prId = garmentPurchaseRequestItem == null ? 0 : garmentPurchaseRequestItem.GarmentPRId;
                            GarmentUnitReceiptNoteItem urnItem = new GarmentUnitReceiptNoteItem
                            {
                                DODetailId = item.DODetailId,
                                EPOItemId = item.EPOItemId,
                                PRId = garmentPurchaseRequestItem == null ? 0 : garmentPurchaseRequestItem.GarmentPRId,
                                PRNo = garmentPurchaseRequestItem == null ? null : garmentPurchaseRequest.PRNo,
                                PRItemId = item.PRItemId,
                                POId = garmentInternalPurchaseOrderItem == null ? 0 : garmentInternalPurchaseOrderItem.GPOId,
                                POItemId = item.POItemId,
                                POSerialNumber = item.POSerialNumber,
                                ProductId = item.ProductId,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductRemark = item.ProductRemark,
                                RONo = item.RONo,
                                ReceiptQuantity = (decimal)item.Quantity / item.Conversion,
                                UomId = OldurnItem.UomId,
                                UomUnit = OldurnItem.UomUnit,
                                PricePerDealUnit = (decimal)item.PricePerDealUnit,
                                DesignColor = garmentUnitDeliveryOrderItem.DesignColor,
                                IsCorrection = false,
                                Conversion = item.Conversion,
                                SmallQuantity = (decimal)item.Quantity,
                                SmallUomId = item.UomId,
                                SmallUomUnit = item.UomUnit,
                                ReceiptCorrection = (decimal)item.Quantity / item.Conversion,
                                OrderQuantity = (decimal)item.Quantity,
                                CorrectionConversion = item.Conversion,
                                UENItemId = item.Id,
                                DOCurrencyRate = item.DOCurrencyRate != null ? (double)item.DOCurrencyRate : 0
                            };
                            urnItems.Add(urnItem);
                            EntityExtension.FlagForCreate(urnItem, identityService.Username, USER_AGENT);

                        }

                        GarmentUnitReceiptNote garmentUnitReceiptNote = new GarmentUnitReceiptNote
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
                            IsUnitDO = true,
                            Items = urnItems
                        };
                        garmentUnitReceiptNote.URNNo = await garmentUnitReceiptNoteFacade.GenerateNo(garmentUnitReceiptNote);
                        EntityExtension.FlagForCreate(garmentUnitReceiptNote, identityService.Username, USER_AGENT);
                        dbSetGarmentUnitReceiptNote.Add(garmentUnitReceiptNote);

                        await dbContext.SaveChangesAsync();

                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentDOItems garmentDOItems = new GarmentDOItems
                            {
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
                                RemainingQuantity = 0,
                                DetailReferenceId = garmentUnitReceiptNoteItem.DODetailId,
                                URNItemId = garmentUnitReceiptNoteItem.Id,
                                DOCurrencyRate = garmentUnitReceiptNoteItem.DOCurrencyRate,
                                EPOItemId = garmentUnitReceiptNoteItem.EPOItemId,
                                PRItemId = garmentUnitReceiptNoteItem.PRItemId,
                                RO = garmentUnitReceiptNoteItem.RONo,

                            };
                            garmentDOItems.DOItemNo = await garmentUnitReceiptNoteFacade.GenerateNoDOItems(garmentUnitReceiptNote);

                            EntityExtension.FlagForCreate(garmentDOItems, identityService.Username, USER_AGENT);
                            dbSetGarmentDOItems.Add(garmentDOItems);
                            await dbContext.SaveChangesAsync();
                        }

                        List<GarmentUnitDeliveryOrderItem> unitDOItems = new List<GarmentUnitDeliveryOrderItem>();
                        foreach (var urnItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentUnitExpenditureNoteItem gUenItem = garmentUnitExpenditureNote.Items.FirstOrDefault(a => a.Id == urnItem.UENItemId);

                            GarmentUnitDeliveryOrderItem garmentUnitDOItems = new GarmentUnitDeliveryOrderItem
                            {
                                URNId = garmentUnitReceiptNote.Id,
                                URNNo = garmentUnitReceiptNote.URNNo,
                                URNItemId = urnItem.Id,
                                DODetailId = urnItem.DODetailId,
                                EPOItemId = urnItem.EPOItemId,
                                POItemId = urnItem.POItemId,
                                PRItemId = urnItem.PRItemId,
                                POSerialNumber = urnItem.POSerialNumber,
                                ProductId = urnItem.ProductId,
                                ProductCode = urnItem.ProductCode,
                                ProductName = urnItem.ProductName,
                                ProductRemark = urnItem.ProductRemark,
                                RONo = urnItem.RONo,
                                Quantity = (double)urnItem.SmallQuantity,
                                UomId = urnItem.SmallUomId,
                                UomUnit = urnItem.SmallUomUnit,
                                PricePerDealUnit = (double)urnItem.PricePerDealUnit,
                                DesignColor = urnItem.DesignColor,
                                DefaultDOQuantity = (double)urnItem.SmallQuantity,
                                DOCurrencyRate = gUenItem.DOCurrencyRate,
                                ReturQuantity = 0,
                                FabricType = gUenItem.FabricType,

                            };
                            unitDOItems.Add(garmentUnitDOItems);
                            EntityExtension.FlagForCreate(garmentUnitDOItems, identityService.Username, USER_AGENT);
                        }

                        GarmentUnitDeliveryOrder garmentUnitDO = new GarmentUnitDeliveryOrder
                        {
                            UnitDOType = garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" ? "PROSES" : "SAMPLE",
                            UnitDODate = garmentUnitExpenditureNote.ExpenditureDate,
                            UnitRequestId = garmentUnitExpenditureNote.UnitRequestId,
                            UnitRequestCode = garmentUnitExpenditureNote.UnitRequestCode,
                            UnitRequestName = garmentUnitExpenditureNote.UnitRequestName,
                            UnitSenderId = garmentUnitExpenditureNote.UnitRequestId,
                            UnitSenderCode = garmentUnitExpenditureNote.UnitRequestCode,
                            UnitSenderName = garmentUnitExpenditureNote.UnitRequestName,
                            StorageId = garmentUnitExpenditureNote.StorageRequestId,
                            StorageCode = garmentUnitExpenditureNote.StorageRequestCode,
                            StorageName = garmentUnitExpenditureNote.StorageRequestName,
                            RONo = oldGarmentUnitDO.RONo,
                            Article = oldGarmentUnitDO.Article,
                            IsUsed = true,
                            Items = unitDOItems,
                            UENFromId = garmentUnitExpenditureNote.Id,
                            UENFromNo = garmentUnitExpenditureNote.UENNo,
                            UnitDOFromId = oldGarmentUnitDO.Id,
                            UnitDOFromNo = oldGarmentUnitDO.UnitDONo
                        };

                        garmentUnitDO.UnitDONo = await garmentUnitDeliveryOrderFacade.GenerateNo(garmentUnitDO);
                        EntityExtension.FlagForCreate(garmentUnitDO, identityService.Username, USER_AGENT);

                        dbSetGarmentUnitDeliveryOrder.Add(garmentUnitDO);

                        await dbContext.SaveChangesAsync();

                        List<GarmentUnitExpenditureNoteItem> garmentUENItems = new List<GarmentUnitExpenditureNoteItem>();
                        foreach (var unitDOItem in garmentUnitDO.Items)
                        {
                            GarmentPurchaseRequest garmentPurchaseRequest = dbContext.GarmentPurchaseRequests.FirstOrDefault(a => a.RONo == unitDOItem.RONo);
                            //GarmentInternalPurchaseOrderItem gpoItem = dbContext.GarmentInternalPurchaseOrderItems.FirstOrDefault(a => a.Id == unitDOItem.POItemId);
                            //GarmentInternalPurchaseOrder garmentInternalPurchaseOrder = dbContext.GarmentInternalPurchaseOrders.FirstOrDefault(a => a.Id == gpoItem.GPOId);

                            GarmentUnitReceiptNoteItem gUrnItem = garmentUnitReceiptNote.Items.FirstOrDefault(a => a.Id == unitDOItem.URNItemId);
                            GarmentUnitExpenditureNoteItem gUenItem1 = garmentUnitExpenditureNote.Items.FirstOrDefault(a => a.Id == gUrnItem.UENItemId);

                            GarmentUnitExpenditureNoteItem uenItem = new GarmentUnitExpenditureNoteItem
                            {
                                UnitDOItemId = unitDOItem.Id,
                                URNItemId = unitDOItem.URNItemId,
                                DODetailId = unitDOItem.DODetailId,
                                EPOItemId = unitDOItem.EPOItemId,
                                POItemId = unitDOItem.POItemId,
                                PRItemId = unitDOItem.PRItemId,
                                POSerialNumber = unitDOItem.POSerialNumber,
                                ProductId = unitDOItem.ProductId,
                                ProductCode = unitDOItem.ProductCode,
                                ProductName = unitDOItem.ProductName,
                                ProductRemark = unitDOItem.ProductRemark,
                                RONo = unitDOItem.RONo,
                                Quantity = unitDOItem.Quantity,
                                UomId = unitDOItem.UomId,
                                UomUnit = unitDOItem.UomUnit,
                                PricePerDealUnit = unitDOItem.PricePerDealUnit,
                                FabricType = unitDOItem.FabricType,
                                BuyerId = garmentPurchaseRequest == null ? 0 : long.Parse(garmentPurchaseRequest.BuyerId),
                                BuyerCode = garmentPurchaseRequest == null ? null : garmentPurchaseRequest.BuyerCode,
                                DOCurrencyRate = unitDOItem.DOCurrencyRate,
                                BasicPrice = gUenItem1.BasicPrice,
                                Conversion = gUenItem1.Conversion

                            };
                            garmentUENItems.Add(uenItem);

                            EntityExtension.FlagForCreate(uenItem, identityService.Username, USER_AGENT);
                        }

                        GarmentUnitExpenditureNote uen = new GarmentUnitExpenditureNote
                        {
                            ExpenditureDate = garmentUnitExpenditureNote.ExpenditureDate,
                            ExpenditureType = garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" ? "PROSES" : "SAMPLE",
                            ExpenditureTo = garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" ? "PROSES" : "SAMPLE",
                            UnitDOId = garmentUnitDO.Id,
                            UnitDONo = garmentUnitDO.UnitDONo,
                            UnitSenderId = garmentUnitDO.UnitSenderId,
                            UnitSenderCode = garmentUnitDO.UnitSenderCode,
                            UnitSenderName = garmentUnitDO.UnitSenderName,
                            StorageId = garmentUnitDO.StorageId,
                            StorageCode = garmentUnitDO.StorageCode,
                            StorageName = garmentUnitDO.StorageName,
                            UnitRequestId = garmentUnitDO.UnitRequestId,
                            UnitRequestCode = garmentUnitDO.UnitRequestCode,
                            UnitRequestName = garmentUnitDO.UnitRequestName,
                            IsTransfered = true,
                            Items = garmentUENItems
                        };
                        uen.UENNo = await GenerateNo(uen);
                        EntityExtension.FlagForCreate(uen, identityService.Username, USER_AGENT);

                        dbSet.Add(uen);

                        #region Inventory

                        var garmentInventoryDocumentTransferOutStorage = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentTransferOutStorage);

                        var garmentInventoryDocumentTransferInStorageRequest = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "IN");
                        garmentInventoryDocumentTransferInStorageRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                        garmentInventoryDocumentTransferInStorageRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                        garmentInventoryDocumentTransferInStorageRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                        garmentInventoryDocumentTransferInStorageRequest.ReferenceNo = uen.UENNo;
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentTransferInStorageRequest);

                        var garmentInventoryDocumentTransferOutStorageRequest = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        garmentInventoryDocumentTransferOutStorageRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                        garmentInventoryDocumentTransferOutStorageRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                        garmentInventoryDocumentTransferOutStorageRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                        garmentInventoryDocumentTransferOutStorageRequest.ReferenceNo = uen.UENNo;
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentTransferOutStorageRequest);

                        foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                        {
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "OUT");
                            garmentInventoryMovement.Date = garmentUnitExpenditureNote.ExpenditureDate;
                            garmentInventoryMovement.ReferenceNo = garmentUnitExpenditureNote.UENNo;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                            if (garmentInventorySummaryExisting == null)
                            {
                                var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovement);
                                dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                            }

                            var garmentInventorySummaryExistingRequest = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageRequestId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovementRequestIn = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExistingRequest, "IN");
                            garmentInventoryMovementRequestIn.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                            garmentInventoryMovementRequestIn.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                            garmentInventoryMovementRequestIn.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                            garmentInventoryMovementRequestIn.After = garmentInventoryMovementRequestIn.Before + (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            garmentInventoryMovementRequestIn.ReferenceNo = uen.UENNo;
                            garmentInventoryMovementRequestIn.Date = uen.ExpenditureDate;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementRequestIn);

                            var garmentInventoryMovementRequestOut = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExistingRequest, "OUT");
                            garmentInventoryMovementRequestOut.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                            garmentInventoryMovementRequestOut.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                            garmentInventoryMovementRequestOut.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                            garmentInventoryMovementRequestOut.Before = garmentInventoryMovementRequestIn.After;
                            garmentInventoryMovementRequestOut.After = garmentInventoryMovementRequestOut.Before - (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            garmentInventoryMovementRequestOut.ReferenceNo = uen.UENNo;
                            garmentInventoryMovementRequestOut.Date = uen.ExpenditureDate;
                            //if (garmentInventorySummaryExistingRequest == null || garmentInventorySummaryExistingRequest.Quantity == 0)
                            //{
                            //    garmentInventoryMovementRequestOut.Before = garmentInventoryMovementRequestIn.After;
                            //    garmentInventoryMovementRequestOut.After = garmentInventoryMovementRequestIn.Before;
                            //}
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementRequestOut);

                            if (garmentInventorySummaryExistingRequest == null)
                            {
                                var garmentInventorySummaryRequest = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovementRequestOut);
                                garmentInventorySummaryRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                                garmentInventorySummaryRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                                garmentInventorySummaryRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                                dbSetGarmentInventorySummary.Add(garmentInventorySummaryRequest);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExistingRequest, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExistingRequest.Quantity = garmentInventoryMovementRequestOut.After;
                            }

                            #endregion Inventory

                            Created = await dbContext.SaveChangesAsync();
                        }

                        //Created = await dbContext.SaveChangesAsync();
                    }
                    else if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER SAMPLE")
                    {
                        GarmentUnitReceiptNoteFacade garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(this.serviceProvider, dbContext);
                        GarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(dbContext, this.serviceProvider);
                        var oldGarmentUnitDO = dbSetGarmentUnitDeliveryOrder.AsNoTracking().First(d => d.Id == garmentUnitExpenditureNote.UnitDOId);

                        List<GarmentUnitReceiptNoteItem> urnItems = new List<GarmentUnitReceiptNoteItem>();
                        foreach (var item in garmentUnitExpenditureNote.Items)
                        {
                            GarmentPurchaseRequestItem garmentPurchaseRequestItem = item.PRItemId == 0 ? null : dbContext.GarmentPurchaseRequestItems.AsNoTracking().FirstOrDefault(a => a.Id == item.PRItemId);
                            GarmentPurchaseRequest garmentPurchaseRequest = item.PRItemId == 0 ? null : dbContext.GarmentPurchaseRequests.AsNoTracking().FirstOrDefault(a => a.Id == garmentPurchaseRequestItem.GarmentPRId);
                            GarmentInternalPurchaseOrderItem garmentInternalPurchaseOrderItem = dbContext.GarmentInternalPurchaseOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == item.POItemId);
                            GarmentUnitDeliveryOrderItem garmentUnitDeliveryOrderItem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == item.UnitDOItemId);
                            GarmentUnitReceiptNoteItem OldurnItem = dbContext.GarmentUnitReceiptNoteItems.AsNoTracking().FirstOrDefault(a => a.Id == item.URNItemId);
                            //var prId = garmentPurchaseRequestItem == null ? 0 : garmentPurchaseRequestItem.GarmentPRId;
                            GarmentUnitReceiptNoteItem urnItem = new GarmentUnitReceiptNoteItem
                            {
                                DODetailId = item.DODetailId,
                                EPOItemId = item.EPOItemId,
                                PRId = garmentPurchaseRequestItem == null ? 0 : garmentPurchaseRequestItem.GarmentPRId,
                                PRNo = garmentPurchaseRequestItem == null ? null : garmentPurchaseRequest.PRNo,
                                PRItemId = item.PRItemId,
                                POId = garmentInternalPurchaseOrderItem == null ? 0 : garmentInternalPurchaseOrderItem.GPOId,
                                POItemId = item.POItemId,
                                POSerialNumber = item.POSerialNumber,
                                ProductId = item.ProductId,
                                ProductCode = item.ProductCode,
                                ProductName = item.ProductName,
                                ProductRemark = item.ProductRemark,
                                RONo = item.RONo,
                                ReceiptQuantity = (decimal)item.Quantity / item.Conversion,
                                UomId = OldurnItem.UomId,
                                UomUnit = OldurnItem.UomUnit,
                                PricePerDealUnit = (decimal)item.PricePerDealUnit,
                                DesignColor = garmentUnitDeliveryOrderItem.DesignColor,
                                IsCorrection = false,
                                Conversion = item.Conversion,
                                SmallQuantity = (decimal)item.Quantity,
                                SmallUomId = item.UomId,
                                SmallUomUnit = item.UomUnit,
                                ReceiptCorrection = (decimal)item.Quantity / item.Conversion,
                                OrderQuantity = (decimal)item.Quantity,
                                CorrectionConversion = item.Conversion,
                                UENItemId = item.Id,
                                DOCurrencyRate = item.DOCurrencyRate != null ? (double)item.DOCurrencyRate : 0
                            };
                            urnItems.Add(urnItem);
                            EntityExtension.FlagForCreate(urnItem, identityService.Username, USER_AGENT);

                        }

                        GarmentUnitReceiptNote garmentUnitReceiptNote = new GarmentUnitReceiptNote
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
                            IsUnitDO = true,
                            Items = urnItems
                        };
                        garmentUnitReceiptNote.URNNo = await garmentUnitReceiptNoteFacade.GenerateNo(garmentUnitReceiptNote);
                        EntityExtension.FlagForCreate(garmentUnitReceiptNote, identityService.Username, USER_AGENT);
                        dbSetGarmentUnitReceiptNote.Add(garmentUnitReceiptNote);

                        await dbContext.SaveChangesAsync();

                        foreach (var garmentUnitReceiptNoteItem in garmentUnitReceiptNote.Items)
                        {
                            GarmentDOItems garmentDOItems = new GarmentDOItems
                            {
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
                                RemainingQuantity = garmentUnitReceiptNoteItem.ReceiptQuantity,
                                DetailReferenceId = garmentUnitReceiptNoteItem.DODetailId,
                                URNItemId = garmentUnitReceiptNoteItem.Id,
                                DOCurrencyRate = garmentUnitReceiptNoteItem.DOCurrencyRate,
                                EPOItemId = garmentUnitReceiptNoteItem.EPOItemId,
                                PRItemId = garmentUnitReceiptNoteItem.PRItemId,
                                RO = garmentUnitReceiptNoteItem.RONo,

                            };
                            garmentDOItems.DOItemNo = await garmentUnitReceiptNoteFacade.GenerateNoDOItems(garmentUnitReceiptNote);

                            EntityExtension.FlagForCreate(garmentDOItems, identityService.Username, USER_AGENT);
                            dbSetGarmentDOItems.Add(garmentDOItems);
                            await dbContext.SaveChangesAsync();

                            #region Inventory

                            var garmentInventoryDocumentTransferOutStorage = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentTransferOutStorage);

                            var garmentInventoryDocumentTransferInStorageRequest = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "IN");
                            garmentInventoryDocumentTransferInStorageRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                            garmentInventoryDocumentTransferInStorageRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                            garmentInventoryDocumentTransferInStorageRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                            garmentInventoryDocumentTransferInStorageRequest.ReferenceNo = garmentUnitExpenditureNote.UENNo;
                            dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentTransferInStorageRequest);


                            foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                            {
                                var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                                var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "OUT");
                                garmentInventoryMovement.Date = garmentUnitExpenditureNote.ExpenditureDate;
                                garmentInventoryMovement.ReferenceNo = garmentUnitExpenditureNote.UENNo;
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                                if (garmentInventorySummaryExisting == null)
                                {
                                    var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovement);
                                    dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExisting.Quantity = garmentInventoryMovement.After;
                                }

                                var garmentInventorySummaryExistingRequest = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageRequestId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                                var garmentInventoryMovementRequestIn = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExistingRequest, "IN");
                                garmentInventoryMovementRequestIn.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                                garmentInventoryMovementRequestIn.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                                garmentInventoryMovementRequestIn.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                                garmentInventoryMovementRequestIn.After = garmentInventoryMovementRequestIn.Before + (decimal)garmentUnitExpenditureNoteItem.Quantity;
                                garmentInventoryMovementRequestIn.ReferenceNo = garmentUnitExpenditureNote.UENNo;
                                garmentInventoryMovementRequestIn.Date = garmentUnitExpenditureNote.ExpenditureDate;
                                dbSetGarmentInventoryMovement.Add(garmentInventoryMovementRequestIn);


                                if (garmentInventorySummaryExistingRequest == null)
                                {
                                    var garmentInventorySummaryRequest = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovementRequestIn);
                                    garmentInventorySummaryRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                                    garmentInventorySummaryRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                                    garmentInventorySummaryRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                                    dbSetGarmentInventorySummary.Add(garmentInventorySummaryRequest);
                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(garmentInventorySummaryExistingRequest, identityService.Username, USER_AGENT);
                                    garmentInventorySummaryExistingRequest.Quantity = garmentInventoryMovementRequestIn.After;
                                }
                            }

                            #endregion Inventory

                            Created = await dbContext.SaveChangesAsync();
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

        public async Task<int> Delete(int id)
        {
            int Deleted = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var garmentUnitExpenditureNote = dbSet.Include(m => m.Items).Single(m => m.Id == id);

                    EntityExtension.FlagForDelete(garmentUnitExpenditureNote, identityService.Username, USER_AGENT);

                    var garmentUnitDeliveryOrder = dbSetGarmentUnitDeliveryOrder.FirstOrDefault(d => d.Id == garmentUnitExpenditureNote.UnitDOId);
                    if (garmentUnitDeliveryOrder != null)
                    {
                        EntityExtension.FlagForUpdate(garmentUnitDeliveryOrder, identityService.Username, USER_AGENT);
                        garmentUnitDeliveryOrder.IsUsed = false;
                        

                    }
                    foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                    {
                        EntityExtension.FlagForDelete(garmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);
                        var garmentUnitDOItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(d => d.Id == garmentUnitExpenditureNoteItem.UnitDOItemId);
                        if (garmentUnitDOItem != null)
                        {
                            garmentUnitDOItem.Quantity = garmentUnitExpenditureNoteItem.Quantity;
                        }

                    }

                    var garmentInventoryDocument = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "IN");
                    garmentInventoryDocument.Date = DateTimeOffset.Now;
                    dbSetGarmentInventoryDocument.Add(garmentInventoryDocument);

                    foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                    {
                        var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.FirstOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageId && s.UomId == garmentUnitExpenditureNoteItem.UomId);
                        
                        var garmentInventoryMovement = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "IN");
                        dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                        if (garmentInventorySummaryExisting != null)
                        {
                            EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            garmentInventorySummaryExisting.Quantity = garmentInventorySummaryExisting.Quantity + (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            garmentInventoryMovement.After = garmentInventorySummaryExisting.Quantity;
                        }
                    }

                    if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" || (garmentUnitExpenditureNote.ExpenditureType == "SAMPLE" && garmentUnitExpenditureNote.UnitSenderCode != "SMP1"))
                    {
                        var urn = dbSetGarmentUnitReceiptNote.Include(a=>a.Items).FirstOrDefault(a => a.UENId == id);
                        EntityExtension.FlagForDelete(urn, identityService.Username, USER_AGENT);
                        foreach(var urnItem in urn.Items)
                        {
                            EntityExtension.FlagForDelete(urnItem, identityService.Username, USER_AGENT);
                        }
                        var unitDOItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(a => a.URNId == urn.Id);
                        var unitDO = dbSetGarmentUnitDeliveryOrder.Include(a=>a.Items).FirstOrDefault(a => a.Id == unitDOItem.UnitDOId);
                        EntityExtension.FlagForDelete(unitDO, identityService.Username, USER_AGENT);

                        foreach(var uDOItem in unitDO.Items)
                        {
                            EntityExtension.FlagForDelete(uDOItem, identityService.Username, USER_AGENT);
                        }

                        var uen = dbSet.Include(a=>a.Items).FirstOrDefault(a => a.UnitDOId == unitDO.Id);
                        EntityExtension.FlagForDelete(uen, identityService.Username, USER_AGENT);

                        foreach (var uenItem in uen.Items)
                        {
                            EntityExtension.FlagForDelete(uenItem, identityService.Username, USER_AGENT);
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

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentUnitExpenditureNote> Query = dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UENNo", "UnitDONo", "ExpenditureType", "ExpenditureTo", "CreatedBy"
            };

            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureOrder(Query, OrderDictionary);

            Query = Query.Select(m => new GarmentUnitExpenditureNote
            {
                Id = m.Id,
                UENNo = m.UENNo,
                UnitDONo = m.UnitDONo,
                ExpenditureDate = m.ExpenditureDate,
                ExpenditureTo = m.ExpenditureTo,
                ExpenditureType = m.ExpenditureType,
                UnitDOId = m.UnitDOId,
                UnitRequestId = m.UnitRequestId,
                UnitRequestCode = m.UnitRequestCode,
                UnitRequestName = m.UnitRequestName,
                UnitSenderId = m.UnitSenderId,
                UnitSenderCode = m.UnitSenderCode,
                UnitSenderName = m.UnitSenderName,
                Items = m.Items.Select(i => new GarmentUnitExpenditureNoteItem
                {
                    Id = i.Id,
                    UENId = i.UENId,
                    ProductId = i.ProductId,
                    ProductCode = i.ProductCode,
                    ProductName = i.ProductName,
                    RONo = i.RONo,
                    Quantity = i.Quantity,
                    UomId = i.UomId,
                    UomUnit = i.UomUnit,
                    ReturQuantity = i.ReturQuantity,
                    UnitDOItemId = i.UnitDOItemId,
                    FabricType = i.FabricType,
                    ProductRemark = i.ProductRemark
                }).ToList(),
                CreatedAgent = m.CreatedAgent,
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc,
                CreatedUtc = m.CreatedUtc
            });

            Pageable<GarmentUnitExpenditureNote> pageable = new Pageable<GarmentUnitExpenditureNote>(Query, Page - 1, Size);
            List<GarmentUnitExpenditureNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(Data.Select(s => new
            {
                s.Id,
                s.UENNo,
                s.ExpenditureDate,
                s.ExpenditureTo,
                s.ExpenditureType,
                s.UnitDONo,
                s.CreatedAgent,
                s.CreatedBy,
                s.LastModifiedUtc,
                s.UnitDOId,
                s.CreatedUtc,
                s.UnitRequestId,
                s.UnitRequestCode,
                s.UnitRequestName,
                s.UnitSenderId,
                s.UnitSenderCode,
                s.UnitSenderName,
                s.Items
            }));

            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public ReadResponse<object> ReadLoader(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}",ConditionType conditionType=ConditionType.ENUM_INT)
        {
            IQueryable<GarmentUnitExpenditureNote> Query = dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "UENNo", "UnitDONo", "ExpenditureType", "ExpenditureTo", "CreatedBy"
            };

            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            //List<object> FilterDictionary = JsonConvert.DeserializeObject<List<object>>(Filter);
            List<FilterViewModel> filter = FilterViewModel.ConvertJsonAsList(Filter, conditionType);
            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureFilter(Query, filter);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureOrder(Query, OrderDictionary);

            Query = Query.Select(m => new GarmentUnitExpenditureNote
            {
                Id = m.Id,
                UENNo = m.UENNo,
                UnitDONo = m.UnitDONo,
                ExpenditureDate = m.ExpenditureDate,
                ExpenditureTo = m.ExpenditureTo,
                ExpenditureType = m.ExpenditureType,
                UnitDOId = m.UnitDOId,
                Items = m.Items.Select(i => new GarmentUnitExpenditureNoteItem
                {
                    Id = i.Id,
                    UENId = i.UENId,
                    ProductId = i.ProductId,
                    ProductCode = i.ProductCode,
                    ProductName = i.ProductName,
                    RONo = i.RONo,
                    Quantity = i.Quantity,
                    UomId = i.UomId,
                    UomUnit = i.UomUnit,
                    ReturQuantity = i.ReturQuantity,
                    UnitDOItemId = i.UnitDOItemId,
                    FabricType = i.FabricType,
                    ProductRemark = i.ProductRemark
                }).ToList(),
                CreatedAgent = m.CreatedAgent,
                CreatedBy = m.CreatedBy,
                LastModifiedUtc = m.LastModifiedUtc
            });

            Pageable<GarmentUnitExpenditureNote> pageable = new Pageable<GarmentUnitExpenditureNote>(Query, Page - 1, Size);
            List<GarmentUnitExpenditureNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(Data.Select(s => new
            {
                s.Id,
                s.UENNo,
                s.ExpenditureDate,
                s.ExpenditureTo,
                s.ExpenditureType,
                s.UnitDONo,
                s.CreatedAgent,
                s.CreatedBy,
                s.LastModifiedUtc,
                s.UnitDOId,
                s.Items
            }));

            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public GarmentUnitExpenditureNoteViewModel ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                            .Include(m => m.Items)
                            .FirstOrDefault();
            var viewModel = mapper.Map<GarmentUnitExpenditureNoteViewModel>(model);

            return viewModel;
        }

		public async Task<int> Update(int id, GarmentUnitExpenditureNote garmentUnitExpenditureNote)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var itemIsSaveFalse = garmentUnitExpenditureNote.Items.Where(i => i.IsSave == false).ToList();
                    garmentUnitExpenditureNote.Items = garmentUnitExpenditureNote.Items.Where(x => x.IsSave).ToList();

                    var oldGarmentUnitExpenditureNote = dbSet
                        .Include(d => d.Items)
                        .Single(m => m.Id == id);

                    foreach(var uncheck in itemIsSaveFalse)
                    {
                        var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.FirstOrDefault(s => s.ProductId == uncheck.ProductId && s.StorageId == oldGarmentUnitExpenditureNote.StorageId && s.UomId == uncheck.UomId);

                        var garmentInventoryMovement = GenerateGarmentInventoryMovement(oldGarmentUnitExpenditureNote, uncheck, garmentInventorySummaryExisting, "IN");
                        dbSetGarmentInventoryMovement.Add(garmentInventoryMovement);

                        if (garmentInventorySummaryExisting != null)
                        {
                            EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            garmentInventorySummaryExisting.Quantity = garmentInventorySummaryExisting.Quantity + (decimal)uncheck.Quantity;
                            garmentInventoryMovement.After = garmentInventorySummaryExisting.Quantity;
                        }
                    }



                    if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" || (garmentUnitExpenditureNote.ExpenditureType == "SAMPLE" && garmentUnitExpenditureNote.UnitSenderCode != "SMP1"))
                    {
                        var garmentInventoryDocumentIn = GenerateGarmentInventoryDocument(oldGarmentUnitExpenditureNote, "IN");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentIn);

                        var garmentInventoryDocumentOut = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOut);

                        var garmentInventoryDocumentInRequest = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "IN");
                        garmentInventoryDocumentInRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                        garmentInventoryDocumentInRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                        garmentInventoryDocumentInRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentInRequest);

                        var garmentInventoryDocumentOutRequest = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        garmentInventoryDocumentOutRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                        garmentInventoryDocumentOutRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                        garmentInventoryDocumentOutRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOutRequest);

                        foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                        {
                            var oldGarmentUnitExpenditureNoteItem = oldGarmentUnitExpenditureNote.Items.Single(i => i.Id == garmentUnitExpenditureNoteItem.Id);

                            //Buat IN untuk gudang yang mengeluarkan
                            var oldGarmentInventorySummaryExisting = dbSetGarmentInventorySummary.Single(s => s.ProductId == oldGarmentUnitExpenditureNoteItem.ProductId && s.StorageId == oldGarmentUnitExpenditureNote.StorageId && s.UomId == oldGarmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovementIn = GenerateGarmentInventoryMovement(oldGarmentUnitExpenditureNote, oldGarmentUnitExpenditureNoteItem, oldGarmentInventorySummaryExisting, "IN");
                            garmentInventoryMovementIn.After = garmentInventoryMovementIn.Before + (decimal)oldGarmentUnitExpenditureNoteItem.Quantity;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementIn);

                            var garmentInventoryMovementOut = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, oldGarmentInventorySummaryExisting, "OUT");
                            garmentInventoryMovementOut.Before = garmentInventoryMovementIn.After;
                            garmentInventoryMovementOut.After = garmentInventoryMovementOut.Before - (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementOut);

                            if (oldGarmentInventorySummaryExisting != null)
                            {
                                EntityExtension.FlagForUpdate(oldGarmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                oldGarmentInventorySummaryExisting.Quantity = garmentInventoryMovementOut.After;
                            }

                            //Buat OUT untuk gudang yang mengeluarkan
                            var garmentInventorySummaryExisting = dbSetGarmentInventorySummary.SingleOrDefault(s => s.ProductId == garmentUnitExpenditureNoteItem.ProductId && s.StorageId == garmentUnitExpenditureNote.StorageRequestId && s.UomId == garmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovementInRequest = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "IN");
                            garmentInventoryMovementInRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                            garmentInventoryMovementInRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                            garmentInventoryMovementInRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                            garmentInventoryMovementInRequest.After = garmentInventoryMovementInRequest.Before + (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementInRequest);

                            var garmentInventoryMovementOutRequest = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventorySummaryExisting, "OUT");
                            garmentInventoryMovementOutRequest.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                            garmentInventoryMovementOutRequest.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                            garmentInventoryMovementOutRequest.StorageName = garmentUnitExpenditureNote.StorageRequestName;
                            garmentInventoryMovementOutRequest.Before = garmentInventoryMovementInRequest.After;
                            garmentInventoryMovementOutRequest.After = garmentInventoryMovementOutRequest.Before - (decimal)garmentUnitExpenditureNoteItem.Quantity;
                            if (garmentInventorySummaryExisting == null || garmentInventorySummaryExisting.Quantity == 0)
                            {
                                garmentInventoryMovementOutRequest.Before = garmentInventoryMovementInRequest.After;
                                garmentInventoryMovementOutRequest.After = garmentInventoryMovementInRequest.Before;
                            }
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementOutRequest);

                            if (garmentInventorySummaryExisting == null)
                            {
                                var garmentInventorySummary = GenerateGarmentInventorySummary(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, garmentInventoryMovementOutRequest);
                                garmentInventorySummary.StorageId = garmentUnitExpenditureNote.StorageRequestId;
                                garmentInventorySummary.StorageCode = garmentUnitExpenditureNote.StorageRequestCode;
                                garmentInventorySummary.StorageName = garmentUnitExpenditureNote.StorageRequestName;

                                dbSetGarmentInventorySummary.Add(garmentInventorySummary);
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(garmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                                garmentInventorySummaryExisting.Quantity = garmentInventoryMovementOutRequest.After;
                            }
                        }
                    }
                    else
                    {
                        var garmentInventoryDocumentIn = GenerateGarmentInventoryDocument(oldGarmentUnitExpenditureNote, "IN");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentIn);

                        var garmentInventoryDocumentOut = GenerateGarmentInventoryDocument(garmentUnitExpenditureNote, "OUT");
                        dbSetGarmentInventoryDocument.Add(garmentInventoryDocumentOut);

                        foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
                        {
                            var oldGarmentUnitExpenditureNoteItem = oldGarmentUnitExpenditureNote.Items.Single(i => i.Id == garmentUnitExpenditureNoteItem.Id);

                            //Buat IN untuk gudang yang mengeluarkan
                            var oldGarmentInventorySummaryExisting = dbSetGarmentInventorySummary.Single(s => s.ProductId == oldGarmentUnitExpenditureNoteItem.ProductId && s.StorageId == oldGarmentUnitExpenditureNote.StorageId && s.UomId == oldGarmentUnitExpenditureNoteItem.UomId);

                            var garmentInventoryMovementIn = GenerateGarmentInventoryMovement(oldGarmentUnitExpenditureNote, oldGarmentUnitExpenditureNoteItem, oldGarmentInventorySummaryExisting, "IN");
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementIn);
                            
                            var garmentInventoryMovementOut = GenerateGarmentInventoryMovement(garmentUnitExpenditureNote, garmentUnitExpenditureNoteItem, oldGarmentInventorySummaryExisting, "OUT");
                            garmentInventoryMovementOut.Before = garmentInventoryMovementIn.After;
                            garmentInventoryMovementOut.After = garmentInventoryMovementOut.Before -(decimal) garmentUnitExpenditureNoteItem.Quantity;
                            dbSetGarmentInventoryMovement.Add(garmentInventoryMovementOut);

                            EntityExtension.FlagForUpdate(oldGarmentInventorySummaryExisting, identityService.Username, USER_AGENT);
                            oldGarmentInventorySummaryExisting.Quantity = garmentInventoryMovementOut.After;
                        }
                    }

                    EntityExtension.FlagForUpdate(oldGarmentUnitExpenditureNote, identityService.Username, USER_AGENT);

                    foreach (var oldGarmentUnitExpenditureNoteItem in oldGarmentUnitExpenditureNote.Items)
                    {
                        var newGarmentUnitExpenditureNoteItem = garmentUnitExpenditureNote.Items.FirstOrDefault(i => i.Id == oldGarmentUnitExpenditureNoteItem.Id);
                        if (newGarmentUnitExpenditureNoteItem == null)
                        {
                            var coba = dbContext.GarmentUnitExpenditureNotes.AsNoTracking().FirstOrDefault(d => d.Id == id);
                            coba.Items = itemIsSaveFalse;
                            
                            
                            EntityExtension.FlagForDelete(oldGarmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);

                            var garmentUnitDeliveryOrderItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(s => s.Id == oldGarmentUnitExpenditureNoteItem.UnitDOItemId);
                            garmentUnitDeliveryOrderItem.Quantity = 0;

                            oldGarmentUnitExpenditureNoteItem.DOCurrencyRate = garmentUnitDeliveryOrderItem.DOCurrencyRate.GetValueOrDefault();

                            GarmentUnitReceiptNoteItem garmentUnitReceiptNoteItem = dbContext.GarmentUnitReceiptNoteItems.Single(s => s.Id == oldGarmentUnitExpenditureNoteItem.URNItemId);
                            EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                            garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - (decimal)oldGarmentUnitExpenditureNoteItem.Quantity;
                            oldGarmentUnitExpenditureNoteItem.Conversion = garmentUnitReceiptNoteItem.Conversion;
                        }
                        else
                        {
                            EntityExtension.FlagForUpdate(oldGarmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);

                            var garmentUnitDeliveryOrderItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(s => s.Id == oldGarmentUnitExpenditureNoteItem.UnitDOItemId);
                            var garmentUnitReceiptNoteItem = dbSetGarmentUnitReceiptNoteItem.FirstOrDefault(u => u.Id == oldGarmentUnitExpenditureNoteItem.URNItemId);

                            if (garmentUnitDeliveryOrderItem != null && garmentUnitReceiptNoteItem != null)
                            {
                                EntityExtension.FlagForUpdate(garmentUnitReceiptNoteItem, identityService.Username, USER_AGENT);
                                garmentUnitReceiptNoteItem.OrderQuantity = garmentUnitReceiptNoteItem.OrderQuantity - ((decimal)oldGarmentUnitExpenditureNoteItem.Quantity - (decimal)newGarmentUnitExpenditureNoteItem.Quantity);
                                garmentUnitDeliveryOrderItem.Quantity = newGarmentUnitExpenditureNoteItem.Quantity;
                            }
                            oldGarmentUnitExpenditureNoteItem.Quantity = garmentUnitExpenditureNote.Items.FirstOrDefault(i => i.Id == oldGarmentUnitExpenditureNoteItem.Id).Quantity;
                            oldGarmentUnitExpenditureNoteItem.DOCurrencyRate = garmentUnitDeliveryOrderItem.DOCurrencyRate.GetValueOrDefault();

                            if (oldGarmentUnitExpenditureNoteItem.DOCurrencyRate == 0)
                            {
                                throw new Exception("oldGarmentUnitExpenditureNoteItem.DOCurrencyRate tidak boleh 0");
                            }

                            oldGarmentUnitExpenditureNoteItem.Conversion = garmentUnitReceiptNoteItem.Conversion;
                            oldGarmentUnitExpenditureNoteItem.ItemStatus = newGarmentUnitExpenditureNoteItem.ItemStatus;
                        }
                        var basicPrice = (oldGarmentUnitExpenditureNoteItem.PricePerDealUnit * Math.Round(oldGarmentUnitExpenditureNoteItem.Quantity / (double)oldGarmentUnitExpenditureNoteItem.Conversion, 2) * oldGarmentUnitExpenditureNoteItem.DOCurrencyRate) / (double)oldGarmentUnitExpenditureNoteItem.Conversion;
                        oldGarmentUnitExpenditureNoteItem.BasicPrice = Math.Round((decimal)basicPrice, 4);
                    }

                    //dbSet.Update(garmentUnitExpenditureNote);

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

        public GarmentInventorySummary GenerateGarmentInventorySummary(GarmentUnitExpenditureNote garmentUnitExpenditureNote, GarmentUnitExpenditureNoteItem garmentUnitExpenditureNoteItem, GarmentInventoryMovement garmentInventoryMovement)
        {
            var garmentInventorySummary = new GarmentInventorySummary();
            EntityExtension.FlagForCreate(garmentInventorySummary, identityService.Username, USER_AGENT);
            do
            {
                garmentInventorySummary.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventorySummary.Any(m => m.No == garmentInventorySummary.No));

            garmentInventorySummary.ProductId = garmentUnitExpenditureNoteItem.ProductId;
            garmentInventorySummary.ProductCode = garmentUnitExpenditureNoteItem.ProductCode;
            garmentInventorySummary.ProductName = garmentUnitExpenditureNoteItem.ProductName;

            garmentInventorySummary.StorageId = garmentUnitExpenditureNote.StorageId;
            garmentInventorySummary.StorageCode = garmentUnitExpenditureNote.StorageCode;
            garmentInventorySummary.StorageName = garmentUnitExpenditureNote.StorageName;

            garmentInventorySummary.Quantity = garmentInventoryMovement.After;

            garmentInventorySummary.UomId = garmentUnitExpenditureNoteItem.UomId;
            garmentInventorySummary.UomUnit = garmentUnitExpenditureNoteItem.UomUnit;

            garmentInventorySummary.StockPlanning = 0;

            return garmentInventorySummary;
        }

        public GarmentInventoryMovement GenerateGarmentInventoryMovement(GarmentUnitExpenditureNote garmentUnitExpenditureNote, GarmentUnitExpenditureNoteItem garmentUnitExpenditureNoteItem, GarmentInventorySummary garmentInventorySummary, string type = "IN")
        {
            var garmentInventoryMovement = new GarmentInventoryMovement();
            EntityExtension.FlagForCreate(garmentInventoryMovement, identityService.Username, USER_AGENT);
            do
            {
                garmentInventoryMovement.No = CodeGenerator.Generate();
            }
            while (dbSetGarmentInventoryMovement.Any(m => m.No == garmentInventoryMovement.No));

            garmentInventoryMovement.Date = garmentInventoryMovement.CreatedUtc;

            garmentInventoryMovement.ReferenceNo = garmentUnitExpenditureNote.UENNo;
            garmentInventoryMovement.ReferenceType = string.Concat("Bon Pengeluaran Unit - ", garmentUnitExpenditureNote.UnitSenderName);

            garmentInventoryMovement.ProductId = garmentUnitExpenditureNoteItem.ProductId;
            garmentInventoryMovement.ProductCode = garmentUnitExpenditureNoteItem.ProductCode;
            garmentInventoryMovement.ProductName = garmentUnitExpenditureNoteItem.ProductName;

            garmentInventoryMovement.Type = (type ?? "").ToUpper() == "IN" ? "IN" : "OUT";

            garmentInventoryMovement.StorageId = garmentUnitExpenditureNote.StorageId;
            garmentInventoryMovement.StorageCode = garmentUnitExpenditureNote.StorageCode;
            garmentInventoryMovement.StorageName = garmentUnitExpenditureNote.StorageName;

            garmentInventoryMovement.StockPlanning = 0;
            if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER")
            {
                garmentInventoryMovement.Before = garmentInventorySummary == null ? 0 : garmentInventorySummary.Quantity;
                garmentInventoryMovement.Quantity = (decimal)garmentUnitExpenditureNoteItem.Quantity * (type.ToUpper() == "OUT" ? -1 : 1);
                garmentInventoryMovement.After = garmentInventorySummary == null || garmentInventorySummary.Quantity == 0  ? garmentInventoryMovement.Quantity : garmentInventoryMovement.Before + garmentInventoryMovement.Quantity;
            }
            else
            {
                garmentInventoryMovement.Before = garmentInventorySummary == null ? 0 : garmentInventorySummary.Quantity;
                garmentInventoryMovement.Quantity = (decimal)garmentUnitExpenditureNoteItem.Quantity * ((type ?? "").ToUpper() == "OUT" ? -1 : 1);
                garmentInventoryMovement.After = garmentInventoryMovement.Before + garmentInventoryMovement.Quantity;
            }

            garmentInventoryMovement.UomId = garmentUnitExpenditureNoteItem.UomId;
            garmentInventoryMovement.UomUnit = garmentUnitExpenditureNoteItem.UomUnit;

            garmentInventoryMovement.Remark = garmentUnitExpenditureNoteItem.ProductRemark;

            return garmentInventoryMovement;
        }

        public GarmentInventoryDocument GenerateGarmentInventoryDocument(GarmentUnitExpenditureNote garmentUnitExpenditureNote, string type = "IN")
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

            garmentInventoryDocument.Date = garmentUnitExpenditureNote.ExpenditureDate;
            garmentInventoryDocument.ReferenceNo = garmentUnitExpenditureNote.UENNo;
            garmentInventoryDocument.ReferenceType = string.Concat("Bon Pengeluaran Unit - ", garmentUnitExpenditureNote.UnitSenderName);

            garmentInventoryDocument.Type = (type ?? "").ToUpper() == "IN" ? "IN" : "OUT";

            garmentInventoryDocument.StorageId = garmentUnitExpenditureNote.StorageId;
            garmentInventoryDocument.StorageCode = garmentUnitExpenditureNote.StorageCode;
            garmentInventoryDocument.StorageName = garmentUnitExpenditureNote.StorageName;
            
            garmentInventoryDocument.Remark = "";

            foreach (var garmentUnitExpenditureNoteItem in garmentUnitExpenditureNote.Items)
            {

                var garmentInventoryDocumentItem = new GarmentInventoryDocumentItem();
                EntityExtension.FlagForCreate(garmentInventoryDocumentItem, identityService.Username, USER_AGENT);

                garmentInventoryDocumentItem.ProductId = garmentUnitExpenditureNoteItem.ProductId;
                garmentInventoryDocumentItem.ProductCode = garmentUnitExpenditureNoteItem.ProductCode;
                garmentInventoryDocumentItem.ProductName = garmentUnitExpenditureNoteItem.ProductName;

                garmentInventoryDocumentItem.Quantity = (decimal)garmentUnitExpenditureNoteItem.Quantity;

                garmentInventoryDocumentItem.UomId = garmentUnitExpenditureNoteItem.UomId;
                garmentInventoryDocumentItem.UomUnit = garmentUnitExpenditureNoteItem.UomUnit;

                garmentInventoryDocumentItem.ProductRemark = garmentUnitExpenditureNoteItem.ProductRemark;

                garmentInventoryDocument.Items.Add(garmentInventoryDocumentItem);
            }

            return garmentInventoryDocument;
        }

        public async Task<string> GenerateNo(GarmentUnitExpenditureNote garmentUnitExpenditureNote)
        {
            string Year = garmentUnitExpenditureNote.ExpenditureDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("yy");
            string Month = garmentUnitExpenditureNote.ExpenditureDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("MM");
            string Day = garmentUnitExpenditureNote.ExpenditureDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd");

            string no = "";
            if (garmentUnitExpenditureNote.ExpenditureType == "PROSES" || garmentUnitExpenditureNote.ExpenditureType == "SAMPLE" || garmentUnitExpenditureNote.ExpenditureType == "SISA" || garmentUnitExpenditureNote.ExpenditureType == "SUBCON")// || garmentUnitExpenditureNote.ExpenditureType == "EXTERNAL")
            {
                no = string.Concat("BUK", garmentUnitExpenditureNote.UnitRequestCode, Year, Month, Day);
            }else if (garmentUnitExpenditureNote.ExpenditureType == "TRANSFER" || garmentUnitExpenditureNote.ExpenditureType == "EXTERNAL" || garmentUnitExpenditureNote.ExpenditureType == "TRANSFER SAMPLE" || garmentUnitExpenditureNote.ExpenditureType == "LAIN-LAIN")
            {
                no = string.Concat("BUK", garmentUnitExpenditureNote.UnitSenderCode, Year, Month, Day);

            }
            int Padding = 3;

            var lastNo = await dbSet.Where(w => w.UENNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.UENNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.UENNo.Replace(no, string.Empty)) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public ReadResponse<object> ReadForGPreparing(int Page = 1, int Size = 10, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            IQueryable<GarmentUnitExpenditureNote> Query = dbSet
                .Where(x => x.UENNo.Contains(Keyword ?? "") && x.IsPreparing == false && x.Items.Any(i => i.ProductName == "FABRIC"))
                .Select(m => new GarmentUnitExpenditureNote
                {
                    Id = m.Id,
                    UENNo = m.UENNo,
                    ExpenditureDate = m.ExpenditureDate,
                    ExpenditureType = m.ExpenditureType,
                    UnitDOId = m.UnitDOId,
                    UnitDONo = m.UnitDONo,

                    UnitSenderId = m.UnitSenderId,
                    UnitSenderCode = m.UnitSenderCode,
                    UnitSenderName = m.UnitSenderName,
                    UnitRequestId = m.UnitRequestId,
                    UnitRequestCode = m.UnitRequestCode,
                    UnitRequestName = m.UnitRequestName,
                    StorageId = m.StorageId,
                    StorageCode = m.StorageCode,
                    StorageName = m.StorageName,
                    StorageRequestId = m.StorageRequestId,
                    StorageRequestCode = m.StorageRequestCode,
                    StorageRequestName = m.StorageRequestName,
                    IsPreparing = m.IsPreparing,
                    LastModifiedUtc = m.LastModifiedUtc,
                    Items = m.Items.Where(x => x.ProductName == "FABRIC").Select(i => new GarmentUnitExpenditureNoteItem
                    {
                        Id = i.Id,
                        UnitDOItemId = i.UnitDOItemId,
                        URNItemId = i.URNItemId,
                        ProductId = i.ProductId,
                        ProductCode = i.ProductCode,
                        ProductName = i.ProductName,
                        ProductRemark = i.ProductRemark,

                        PRItemId = i.PRItemId,
                        EPOItemId = i.EPOItemId,
                        DODetailId = i.DODetailId,
                        POItemId = i.POItemId,
                        POSerialNumber = i.POSerialNumber,
                        PricePerDealUnit = i.PricePerDealUnit,
                        Quantity = i.Quantity,
                        RONo = i.RONo,
                        UomId = i.UomId,
                        UomUnit = i.UomUnit,
                        FabricType = i.FabricType,
                        DOCurrencyRate = i.DOCurrencyRate,
                        Conversion = i.Conversion,
                        BasicPrice = i.BasicPrice,
                        ReturQuantity = i.ReturQuantity,
                        CustomsCategory=i.CustomsCategory
                    }).OrderByDescending(i => i.LastModifiedUtc).ToList()
                });

            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentUnitExpenditureNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentUnitExpenditureNote> pageable = new Pageable<GarmentUnitExpenditureNote>(Query, Page - 1, Size);
            List<GarmentUnitExpenditureNote> DataModel = pageable.Data.ToList();
            int Total = pageable.TotalCount;

            //List<GarmentUnitExpenditureNote> DataViewModel = mapper.Map<List<GarmentUnitExpenditureNote>>(DataModel);

            List<dynamic> listData = new List<dynamic>();
            listData.AddRange(
                DataModel.Select(s => new
                {
                    s.Id,
                    s.UENNo,
                    s.ExpenditureDate,
                    s.ExpenditureType,
                    s.UnitDOId,
                    s.UnitDONo,
                    s.UnitSenderId,
                    s.UnitSenderCode,
                    s.UnitSenderName,
                    s.StorageId,
                    s.StorageCode,
                    s.StorageName,
                    s.UnitRequestId,
                    s.UnitRequestCode,
                    s.UnitRequestName,
                    s.StorageRequestId,
                    s.StorageRequestCode,
                    s.StorageRequestName,
                    s.CreatedBy,
                    s.LastModifiedUtc,
                    Items = s.Items.Select(i => new
                    {
                        i.Id,
                        i.UnitDOItemId,
                        i.URNItemId,
                        i.DODetailId,
                        i.EPOItemId,
                        i.POItemId,
                        i.PRItemId,
                        i.POSerialNumber,

                        i.ProductId,
                        i.ProductCode,
                        i.ProductName,
                        i.ProductRemark,
                        i.RONo,
                        i.UomId,
                        i.UomUnit,
                        i.PricePerDealUnit,
                        i.FabricType,
                        i.Quantity,
                        i.DOCurrencyRate,
                        i.Conversion,
                        i.BasicPrice,
                        i.ReturQuantity,
                    }).ToList()
                }).ToList()
            );
            return new ReadResponse<object>(listData, Total, OrderDictionary);
        }

        public async Task<int> UpdateIsPreparing(int id, GarmentUnitExpenditureNote garmentUnitExpenditureNote)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldGarmentUnitExpenditureNote = dbSet
                        .Include(d => d.Items)
                        .Single(m => m.Id == id);

                    oldGarmentUnitExpenditureNote.IsPreparing = garmentUnitExpenditureNote.IsPreparing;

                    EntityExtension.FlagForUpdate(oldGarmentUnitExpenditureNote, identityService.Username, USER_AGENT);

                    foreach (var oldGarmentUnitExpenditureNoteItem in oldGarmentUnitExpenditureNote.Items)
                    {
                        EntityExtension.FlagForUpdate(oldGarmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);
                    }

                    //dbSet.Update(garmentUnitExpenditureNote);

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

        public async Task<int> UpdateReturQuantity(int id, double quantity, double quantityBefore)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldGarmentUnitExpenditureNoteItem = dbSetItem
                        .Single(m => m.Id == id);

                    oldGarmentUnitExpenditureNoteItem.ReturQuantity = oldGarmentUnitExpenditureNoteItem.ReturQuantity - quantityBefore + quantity;
                    EntityExtension.FlagForUpdate(oldGarmentUnitExpenditureNoteItem, identityService.Username, USER_AGENT);

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

        public GarmentUnitExpenditureNote ReadByUENId(int id)
        {
            var urn = dbSetGarmentUnitReceiptNote.FirstOrDefault(a => a.UENId == id);
            var unitDOItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(a => a.URNId == urn.Id);
            var unitDO = dbSetGarmentUnitDeliveryOrder.FirstOrDefault(a => a.Id == unitDOItem.UnitDOId);
            var uen = dbSet.FirstOrDefault(a => a.UnitDOId == unitDO.Id);

            return uen;
        }

		public ExpenditureROViewModel GetROAsalById(int id)
		{
			ExpenditureROViewModel viewModel = new ExpenditureROViewModel();

			var uendetail = dbSet.Include(x=>x.Items).FirstOrDefault(x => x.Items.Any(i => i.Id == id));

			var unitDO = dbSetGarmentUnitDeliveryOrder.FirstOrDefault(a => a.Id == uendetail.UnitDOId);

			foreach (var item in uendetail.Items.Where(s=>s.Id== id))
			{
				var unitDOItem = dbSetGarmentUnitDeliveryOrderItem.FirstOrDefault(a => a.Id == item.UnitDOItemId);
				viewModel = new ExpenditureROViewModel
				{
					DetailExpenditureId = item.Id,
					ROAsal = unitDOItem.RONo,
					BuyerCode= item.BuyerCode
				};
			}
			return viewModel;

		}
        //
        public IQueryable<GarmentUENViewModel> GetDataUENQuery(int id)
        {
            var Query = from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                        join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                        where a.IsDeleted == false && b.IsDeleted == false && b.Id == id
                        select new GarmentUENViewModel
                        {
                            UENId = b.Id,
                            UENNo = b.UENNo,
                            UENDate = b.ExpenditureDate,
                            UnitRequestName = b.UnitRequestName,
                            UnitSenderName = b.UnitSenderName,
                            FabricType = a.FabricType,
                            RONo = a.RONo,
                            Quntity = a.Quantity,
                            UOMUnit = a.UomUnit,
                        };

            return Query.AsQueryable();

        }
        //
        public List<GarmentUENViewModel> GetDataUEN(int id)
        {
            var Query = GetDataUENQuery(id);

            return Query.ToList();
        }
        //  
        public Tuple<List<MonitoringOutViewModel>, int> GetReportOut(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset)
        {
            var Query = GetReportQueryOut(dateFrom, dateTo, type, offset);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.UENNo).ThenBy(b => b.PONo);
            }

            
            Pageable<MonitoringOutViewModel> pageable = new Pageable<MonitoringOutViewModel>(Query, page - 1, size);
            List<MonitoringOutViewModel> Data = pageable.Data.ToList<MonitoringOutViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public IQueryable<MonitoringOutViewModel> GetReportQueryOut(DateTime? dateFrom, DateTime? dateTo, string type, int offset)
        {
            var categories = GetProductCodes(1, int.MaxValue, "{}", "{}");
            var coderequirement = new[] { "BP", "BE" };
            var categories1 = type == "FABRIC" ? categories.Where(x => x.CodeRequirement == "BB").Select(x => x.Name).ToArray() : type == "NON FABRIC" ? categories.Where(x => coderequirement.Contains(x.CodeRequirement)).Select(x => x.Name).ToArray() : categories.Select(x => x.Name).ToArray();
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Data1 = from a in (from aa in dbContext.GarmentUnitExpenditureNoteItems select aa)
                        join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
                        join c in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on a.EPOItemId equals c.Id
                        join d in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on c.GarmentEPOId equals d.Id
                        //join h in Codes on a.ProductCode equals h.Code
                        //join e in (from gg in dbContext.GarmentPurchaseRequests where gg.IsDeleted == false select gg) on a.RONo equals e.RONo
                        where a.IsDeleted == false && b.IsDeleted == false
                        //&& (type == "FABRIC" ? b.ProductName == "FABRIC" : type == "NON FABRIC" ? b.ProductName != "FABRIC" : b.ProductName == b.ProductName)
                        && categories1.Contains(a.ProductName)
                        && a.ProductName != "PROCESS"
                        && b.CreatedUtc.Date >= DateFrom.Date
                        && b.CreatedUtc.Date <= DateTo.Date
                        select new MonitoringOutViewModel
                        {
                            CreatedUtc = a.CreatedUtc,
                            ExTo = b.ExpenditureType == "EXTERNAL" ? "RETUR KE PEMBELIAN" : b.ExpenditureType,
                            ItemCode = a.ProductCode,
                            ItemName = a.ProductName,
                            PONo = a.POSerialNumber,
                            Quantity = Math.Round(a.UomUnit == "YARD" && type == "FABRIC" ? (double)a.Quantity * 0.9144 : (double)a.Quantity,2),
                            Storage = b.StorageName,
                            UENNo = b.UENNo,
                            UnitCode = b.UnitRequestCode,
                            UnitName = b.UnitRequestName,
                            UnitQtyName = a.UomUnit == "PACKS" ? "MT" : a.UomUnit,
                            ExDate = b.ExpenditureDate
                        };
            Data1 = Data1.Where(x => (x.ItemCode != "APL001") && (x.ItemCode != "EMB001") && (x.ItemCode != "GMT001") && (x.ItemCode != "PRN001") && (x.ItemCode != "SMP001") && (x.ItemCode != "WSH001") && (x.ItemCode != "QLT001") && (x.ItemCode != "SMT001"));
            var Query = Data1;
            //var Query = type == "FABRIC" ? from a in dbContext.GarmentUnitExpenditureNotes
            //                               join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
            //                               where a.IsDeleted == false && b.IsDeleted == false
            //                               && a.StorageName == "GUDANG BAHAN BAKU"
            //                               && a.LastModifiedUtc.Date >= DateFrom.Date
            //                               && a.LastModifiedUtc.Date <= DateTo.Date

            //                               select new MonitoringOutViewModel
            //                               {
            //                                   CreatedUtc = a.CreatedUtc,
            //                                   ExTo = a.ExpenditureType == "EXTERNAL" ? "RETUR KE PEMBELIAN" : a.ExpenditureType,
            //                                   ItemCode = b.ProductCode,
            //                                   ItemName = b.ProductName,
            //                                   PONo = b.POSerialNumber,
            //                                   Quantity = b.Quantity,
            //                                   Storage = a.StorageName,
            //                                   UENNo = a.UENNo,
            //                                   UnitCode = a.UnitRequestCode,
            //                                   UnitName = a.UnitRequestName,
            //                                   UnitQtyName = b.UomUnit,
            //                                   ExDate = a.ExpenditureDate
            //                               }
            //            : type == "NON FABRIC" ? from a in dbContext.GarmentUnitExpenditureNotes
            //                                     join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
            //                                     where a.IsDeleted == false && b.IsDeleted == false
            //                                     && a.StorageName != "GUDANG BAHAN BAKU"
            //                                     && a.LastModifiedUtc.Date >= DateFrom.Date
            //                                     && a.LastModifiedUtc.Date <= DateTo.Date
            //                                     select new MonitoringOutViewModel
            //                                     {
            //                                         CreatedUtc = a.CreatedUtc,
            //                                         ExTo = a.ExpenditureType == "EXTERNAL" ? "RETUR KE PEMBELIAN" : a.ExpenditureType,
            //                                         ItemCode = b.ProductCode,
            //                                         ItemName = b.ProductName,
            //                                         PONo = b.POSerialNumber,
            //                                         Quantity = b.Quantity,
            //                                         Storage = a.StorageName,
            //                                         UENNo = a.UENNo,
            //                                         UnitCode = a.UnitRequestCode,
            //                                         UnitName = a.UnitRequestName,
            //                                         UnitQtyName = b.UomUnit,
            //                                         ExDate = a.ExpenditureDate
            //                                     }
            //                                     : from a in dbContext.GarmentUnitExpenditureNotes
            //                                       join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
            //                                       where a.IsDeleted == false && b.IsDeleted == false
            //                                       && a.StorageName == a.StorageName
            //                                       && a.LastModifiedUtc.Date >= DateFrom.Date
            //                                       && a.LastModifiedUtc.Date <= DateTo.Date
            //                                       select new MonitoringOutViewModel
            //                                       {
            //                                           CreatedUtc = a.CreatedUtc,
            //                                           ExTo = a.ExpenditureType == "EXTERNAL" ? "RETUR KE PEMBELIAN" : a.ExpenditureType,
            //                                           ItemCode = b.ProductCode,
            //                                           ItemName = b.ProductName,
            //                                           PONo = b.POSerialNumber,
            //                                           Quantity = b.Quantity,
            //                                           Storage = a.StorageName,
            //                                           UENNo = a.UENNo,
            //                                           UnitCode = a.UnitRequestCode,
            //                                           UnitName = a.UnitRequestName,
            //                                           UnitQtyName = b.UomUnit,
            //                                           ExDate = a.ExpenditureDate
            //                                       };
            return Query.AsQueryable();

        }

        public MemoryStream GenerateExcelMonOut(DateTime? dateFrom, DateTime? dateTo, string category, int offset)
        {
            var Query = GetReportQueryOut(dateFrom, dateTo, category, offset);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Pengeluaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembuatan Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan Pengeluaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Gudang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            


            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "",""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int index = 0;
                foreach (MonitoringOutViewModel data in Query)
                {
                    index++;
                    string tgl1 = data.CreatedUtc == null ? "-" : data.CreatedUtc.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string tgl2 = data.ExDate == null ? "-" : data.ExDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string tgl2 = data.TanggalBuatBon == null ? "-" : data.TanggalBuatBon.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(index, data.UENNo, tgl1, tgl2, data.PONo, data.ItemCode, data.ItemName, data.UnitCode, data.UnitName, data.ExTo, data.Storage, data.Quantity, data.UnitQtyName);

                }

            }

            return Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (result, "Report", mergeCells) }, true);
        }

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

        public async Task<int> PatchOne(long id, JsonPatchDocument<GarmentUnitExpenditureNote> jsonPatch)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var data = dbSet.Where(d => d.Id == id)
                        .Single();

                    EntityExtension.FlagForUpdate(data, identityService.Username, USER_AGENT);

                    jsonPatch.ApplyTo(data);

                    Updated = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }

        public GarmentUnitExpenditureNoteItem GetBasicPriceByPOSerialNumber(string po)
        {
            var uenItem = dbContext.GarmentUnitExpenditureNoteItems.Where(a => a.POSerialNumber == po).OrderByDescending(a => a.CreatedUtc).FirstOrDefault();

            return uenItem;
        }

        //public async Task<int> UenDateRevise(List<long> ids, string user, DateTime reviseDate)
        //{
        //    int Updated = 0;
        //    using (var transaction = this.dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {

        //            //var Ids = listURN.Select(d => d.Id).ToList();
        //            //var Id = listURN.Select(x => x.Id).Single();
        //            var listData = this.dbSet
        //                .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
        //                //.Include(d => d.Items)
        //                .ToList();
        //            listData.ForEach(m =>
        //            {
        //                GarmentUenUrnChangeDateHistory changeDateHistory = new GarmentUenUrnChangeDateHistory
        //                {
        //                    DateOld = m.CreatedUtc,
        //                    DateNow = reviseDate,
        //                    DocumentNo = m.UENNo,

        //                };
        //                EntityExtension.FlagForUpdate(m, user, "Facade");
        //                m.CreatedUtc = reviseDate;
        //                EntityExtension.FlagForCreate(changeDateHistory, user, "Facade");
        //                dbSetUenUrnChangeDate.Add(changeDateHistory);
        //            });

        //            Updated = await dbContext.SaveChangesAsync();
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

        public int UenDateRevise(List<long> ids, string user, DateTime reviseDate)
        {
            int Updated = 0;
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    //var Ids = ListEPO.Select(d => d.Id).ToList();
                    var listData = this.dbSet
                        .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
                        .Include(d => d.Items)
                        .ToList();
                    listData.ForEach(m =>
                    {
                        EntityExtension.FlagForUpdate(m, user, "Facade");

                        GarmentUenUrnChangeDateHistory changeDateHistory = new GarmentUenUrnChangeDateHistory
                        {
                            DateOld = m.CreatedUtc,
                            DateNow = reviseDate,
                            DocumentNo = m.UENNo,

                        };

                        m.CreatedUtc = reviseDate;

                        

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

        public List<object> ReadLoaderProductByROJob(string Keyword = null, string Filter = "{}", int size = 50)
        {
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            bool hasRONoFilter = FilterDictionary.ContainsKey("RONo");
            string RONo = hasRONoFilter ? (FilterDictionary["RONo"] ?? "").Trim() : "";

            Keyword = (Keyword ?? "").Trim();

            IQueryable<GarmentUnitExpenditureNoteItem> Query = (from uenItem in dbContext.GarmentUnitExpenditureNoteItems
                                                                join uen in dbSet on uenItem.UENId equals uen.Id
                                                                join b in dbContext.GarmentUnitDeliveryOrders
                                                                on uen.UnitDOId equals b.Id
                                                                where b.RONo == RONo
                                                                && uenItem.IsDeleted == false
                                                                && uenItem.ProductCode.Contains(Keyword)
                                                                && uen.ExpenditureType == "SUBCON"
                                                                && uenItem.ProductName == "FABRIC"
                                                                select uenItem).AsQueryable();

            var Data = from a in Query
                       join b in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals b.Id
                       select new
                       {
                           a.ProductCode,
                           a.ProductName,
                           a.ProductId,
                           b.DesignColor
                       };

            List<object> ListData = new List<object>();
            ListData.AddRange(Data.Distinct());
            return ListData;
        }
    }
}
