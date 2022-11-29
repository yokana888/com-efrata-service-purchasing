using Com.Efrata.Service.Purchasing.Lib.Configs.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.BankDocumentNumber;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Efrata.Service.Purchasing.Lib.Models.BalanceStockModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel;
using Com.Efrata.Service.Purchasing.Lib.Models.BudgetCashflowWorstCaseModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUenUrnChangeDateHistory;
//using Com.Efrata.Service.Purchasing.Lib.Models.ImportValueModel;

namespace Com.Efrata.Service.Purchasing.Lib
{
    public class PurchasingDbContext : StandardDbContext
    {
        public PurchasingDbContext(DbContextOptions<PurchasingDbContext> options) : base(options)
        {
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                Database.SetCommandTimeout(1000 * 60 * 20);
        }

        public DbSet<PurchasingDocumentExpedition> PurchasingDocumentExpeditions { get; set; }
        public DbSet<PurchasingDocumentExpeditionItem> PurchasingDocumentExpeditionItems { get; set; }
        public DbSet<PPHBankExpenditureNote> PPHBankExpenditureNotes { get; set; }
        public DbSet<PPHBankExpenditureNoteItem> PPHBankExpenditureNoteItems { get; set; }


        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }

        public DbSet<InternalPurchaseOrder> InternalPurchaseOrders { get; set; }
        public DbSet<InternalPurchaseOrderItem> InternalPurchaseOrderItems { get; set; }
        public DbSet<InternalPurchaseOrderFulFillment> InternalPurchaseOrderFulfillments { get; set; }
        public DbSet<InternalPurchaseOrderCorrection> InternalPurchaseOrderCorrections { get; set; }

        public DbSet<ExternalPurchaseOrder> ExternalPurchaseOrders { get; set; }
        public DbSet<ExternalPurchaseOrderItem> ExternalPurchaseOrderItems { get; set; }
        public DbSet<ExternalPurchaseOrderDetail> ExternalPurchaseOrderDetails { get; set; }

        public DbSet<BankExpenditureNoteModel> BankExpenditureNotes { get; set; }
        public DbSet<BankExpenditureNoteItemModel> BankExpenditureNoteItems { get; set; }
        public DbSet<BankExpenditureNoteDetailModel> BankExpenditureNoteDetails { get; set; }

        public DbSet<UnitReceiptNote> UnitReceiptNotes { get; set; }
        public DbSet<UnitReceiptNoteItem> UnitReceiptNoteItems { get; set; }

        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<DeliveryOrderItem> DeliveryOrderItems { get; set; }
        public DbSet<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }

        public DbSet<UnitPaymentOrder> UnitPaymentOrders { get; set; }
        public DbSet<UnitPaymentOrderItem> UnitPaymentOrderItems { get; set; }
        public DbSet<UnitPaymentOrderDetail> UnitPaymentOrderDetails { get; set; }

        public DbSet<BankDocumentNumber> BankDocumentNumbers { get; set; }

        public DbSet<UnitPaymentCorrectionNote> UnitPaymentCorrectionNotes { get; set; }
        public DbSet<UnitPaymentCorrectionNoteItem> UnitPaymentCorrectionNoteItems { get; set; }

        public DbSet<GarmentPurchaseRequest> GarmentPurchaseRequests { get; set; }
        public DbSet<GarmentPurchaseRequestItem> GarmentPurchaseRequestItems { get; set; }

        public DbSet<GarmentInternalPurchaseOrder> GarmentInternalPurchaseOrders { get; set; }
        public DbSet<GarmentInternalPurchaseOrderItem> GarmentInternalPurchaseOrderItems { get; set; }

        public DbSet<GarmentExternalPurchaseOrder> GarmentExternalPurchaseOrders { get; set; }
        public DbSet<GarmentExternalPurchaseOrderItem> GarmentExternalPurchaseOrderItems { get; set; }

        public DbSet<GarmentDeliveryOrder> GarmentDeliveryOrders { get; set; }
        public DbSet<GarmentDeliveryOrderItem> GarmentDeliveryOrderItems { get; set; }
        public DbSet<GarmentDeliveryOrderDetail> GarmentDeliveryOrderDetails { get; set; }
        public DbSet<GarmentInvoice> GarmentInvoices { get; set; }
        public DbSet<GarmentInvoiceItem> GarmentInvoiceItems { get; set; }
        public DbSet<GarmentInvoiceDetail> GarmentInvoiceDetails { get; set; }
        public DbSet<GarmentInternNote> GarmentInternNotes { get; set; }
        public DbSet<GarmentInternNoteItem> GarmentInternNoteItems { get; set; }
        public DbSet<GarmentInternNoteDetail> GarmentInternNoteDetails { get; set; }
        public DbSet<PurchasingDisposition> PurchasingDispositions { get; set; }
        public DbSet<PurchasingDispositionItem> PurchasingDispositionItems { get; set; }
        public DbSet<PurchasingDispositionDetail> PurchasingDispositionDetails { get; set; }

        public DbSet<GarmentCorrectionNote> GarmentCorrectionNotes { get; set; }
        public DbSet<GarmentCorrectionNoteItem> GarmentCorrectionNoteItems { get; set; }
		public DbSet<GarmentBeacukai> GarmentBeacukais { get; set; }
		public DbSet<GarmentBeacukaiItem> GarmentBeacukaiItems { get; set; }

        public DbSet<GarmentUnitReceiptNote> GarmentUnitReceiptNotes { get; set; }
        public DbSet<GarmentUnitReceiptNoteItem> GarmentUnitReceiptNoteItems { get; set; }
        public DbSet<GarmentDOItems> GarmentDOItems { get; set; }

        public DbSet<GarmentInventoryDocument> GarmentInventoryDocuments { get; set; }
        public DbSet<GarmentInventoryDocumentItem> GarmentInventoryDocumentItems { get; set; }
        public DbSet<GarmentInventoryMovement> GarmentInventoryMovements { get; set; }
        public DbSet<GarmentInventorySummary> GarmentInventorySummaries { get; set; }
        public DbSet<GarmentUnitDeliveryOrder> GarmentUnitDeliveryOrders { get; set; }
        public DbSet<GarmentUnitDeliveryOrderItem> GarmentUnitDeliveryOrderItems { get; set; }
        public DbSet<GarmentUnitExpenditureNote> GarmentUnitExpenditureNotes { get; set; }
        public DbSet<GarmentUnitExpenditureNoteItem> GarmentUnitExpenditureNoteItems { get; set; }

        public DbSet<GarmentReceiptCorrection> GarmentReceiptCorrections { get; set; }
        public DbSet<GarmentReceiptCorrectionItem> GarmentReceiptCorrectionItems { get; set; }

        public DbSet<GarmentPOMasterDistribution> GarmentPOMasterDistributions { get; set; }
        public DbSet<GarmentPOMasterDistributionItem> GarmentPOMasterDistributionItems { get; set; }
        public DbSet<GarmentPOMasterDistributionDetail> GarmentPOMasterDistributionDetails { get; set; }

        public DbSet<GarmentSupplierBalanceDebt> GarmentSupplierBalanceDebts { get; set; }
        public DbSet<GarmentSupplierBalanceDebtItem> GarmentSupplierBalanceDebtItems { get; set; }

        public DbSet<GarmentStockOpname> GarmentStockOpnames { get; set; }
        public DbSet<GarmentStockOpnameItem> GarmentStockOpnameItems { get; set; }

        public DbSet<BalanceStock> BalanceStocks { get; set; }

        public DbSet<BudgetCashflowWorstCase> BudgetCashflowWorstCases { get; set; }
        public DbSet<BudgetCashflowWorstCaseItem> BudgetCashflowWorstCaseItems { get; set; }

        public DbSet<GarmentDispositionPurchase> GarmentDispositionPurchases { get; set; }
        public DbSet<GarmentDispositionPurchaseItem> GarmentDispositionPurchaseItems { get; set; }
        public DbSet<GarmentDispositionPurchaseDetail> GarmentDispositionPurchaseDetailss { get; set; }

        public DbSet<GarmentUenUrnChangeDateHistory> GarmentUenUrnChangeDateHistories { get; set; }

        //public DbSet<ImportValue> ImportValues { get; set; }

        public DbSet<GarmentClosingDate> ClosingDate { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PurchasingDocumentExpeditionConfig());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<GarmentPurchaseRequest>()
                .HasIndex(i => i.PRNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentPurchaseRequest>()
                .HasIndex(i => i.RONo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentExternalPurchaseOrder>()
                .HasIndex(i => i.EPONo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentInternNote>()
                .HasIndex(i => i.INNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentUnitReceiptNote>()
                .HasIndex(i => i.URNNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-04 00:00:00.0000000')");

            modelBuilder.Entity<GarmentReceiptCorrection>()
                .HasIndex(i => i.CorrectionNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentUnitDeliveryOrder>()
                .HasIndex(i => i.UnitDONo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentUnitExpenditureNote>()
                .HasIndex(i => i.UENNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentCorrectionNote>()
                .HasIndex(i => i.CorrectionNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            #region Purchasing

            modelBuilder.Entity<PurchaseRequest>()
                .HasIndex(i => i.No)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            modelBuilder.Entity<ExternalPurchaseOrder>()
                .HasIndex(i => i.EPONo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            modelBuilder.Entity<UnitReceiptNote>()
                .HasIndex(i => i.URNNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            modelBuilder.Entity<UnitPaymentOrder>()
                .HasIndex(i => i.UPONo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            modelBuilder.Entity<UnitPaymentCorrectionNote>()
                .HasIndex(i => i.UPCNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            modelBuilder.Entity<GarmentDispositionPurchase>()
                .HasIndex(i => i.DispositionNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");
            #endregion

            #region indexes
            modelBuilder.Entity<GarmentInternNote>()
                .HasIndex(i => new { i.INDate, i.CurrencyId, i.CurrencyCode, i.SupplierId });

            modelBuilder.Entity<GarmentInvoice>()
                .HasIndex(i => new { i.CurrencyId, i.IncomeTaxId, i.SupplierId, i.InvoiceDate, i.IncomeTaxDate });

            modelBuilder.Entity<GarmentInvoiceItem>()
                .HasIndex(i => new { i.ArrivalDate, i.DeliveryOrderId, i.DeliveryOrderNo, i.InvoiceId });

            modelBuilder.Entity<GarmentInvoiceDetail>()
                .HasIndex(i => new { i.EPOId, i.IPOId, i.PRItemId, i.DODetailId, i.ProductId, i.UomId });

            modelBuilder.Entity<GarmentDeliveryOrder>()
                .HasIndex(i => new { i.ArrivalDate, i.CustomsId, i.DOCurrencyId, i.DODate, i.IncomeTaxId, i.SupplierId });

            modelBuilder.Entity<GarmentDeliveryOrderItem>()
                .HasIndex(i => new { i.CurrencyId, i.EPOId, i.GarmentDOId });

            modelBuilder.Entity<GarmentDeliveryOrderDetail>()
                .HasIndex(i => new { i.POId, i.PRId, i.ProductId });

            modelBuilder.Entity<GarmentExternalPurchaseOrder>()
                .HasIndex(i => new { i.CurrencyId, i.IncomeTaxId, i.SupplierId, i.UENId });

            modelBuilder.Entity<GarmentExternalPurchaseOrderItem>()
                .HasIndex(i => new { i.POId, i.PRId });

            modelBuilder.Entity<GarmentInternalPurchaseOrderItem>()
               .HasIndex(i => new { i.CategoryId, i.GPOId, i.ProductId });
            #endregion
            #region BalanceStock
            //modelBuilder.Entity<BalanceStock>()
            //    .HasIndex(i => i.BalanceStockId)
            //    .IsUnique()
            //    .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");
            modelBuilder.Entity<BalanceStock>()
                .Property(i => i.OpenPrice)
                .HasColumnType("Money");
            modelBuilder.Entity<BalanceStock>()
                .Property(i => i.DebitPrice)
                .HasColumnType("Money");
            modelBuilder.Entity<BalanceStock>()
                .Property(i => i.CreditPrice)
                .HasColumnType("Money");
            modelBuilder.Entity<BalanceStock>()
                .Property(i => i.ClosePrice)
                .HasColumnType("Money");
            #endregion
        }
    }
}
