using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.Efrata.Service.Purchasing.Lib.Configs.Expedition
{
    public class PurchasingDocumentExpeditionConfig : IEntityTypeConfiguration<PurchasingDocumentExpedition>
    {
        public void Configure(EntityTypeBuilder<PurchasingDocumentExpedition> builder)
        {
            builder.Property(p => p.UnitPaymentOrderNo).HasMaxLength(255);
            builder.Property(p => p.InvoiceNo).HasMaxLength(255);
            builder.Property(p => p.SupplierCode).HasMaxLength(255);
            builder.Property(p => p.SupplierName).HasMaxLength(255);
            builder.Property(p => p.DivisionCode).HasMaxLength(255);
            builder.Property(p => p.DivisionName).HasMaxLength(255);
            builder.Property(p => p.Currency).HasMaxLength(255);
            builder.Property(p => p.SendToVerificationDivisionBy).HasMaxLength(255);
            builder.Property(p => p.VerificationDivisionBy).HasMaxLength(255);
            builder.Property(p => p.SendToCashierDivisionBy).HasMaxLength(255);
            builder.Property(p => p.SendToAccountingDivisionBy).HasMaxLength(255);
            builder.Property(p => p.SendToPurchasingDivisionBy).HasMaxLength(255);
            builder.Property(p => p.CashierDivisionBy).HasMaxLength(255);
            builder.Property(p => p.AccountingDivisionBy).HasMaxLength(255);
            builder.Property(p => p.NotVerifiedReason).HasMaxLength(255);
            builder.Property(p => p.BankExpenditureNoteNo).HasMaxLength(255);
            builder.Property(p => p.IncomeTaxBy).HasMaxLength(128);
        }
    }
}
