using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote
{
    public class BankExpenditureNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public AccountBankViewModel Bank { get; set; }
        public string BGCheckNumber { get; set; }
        public DateTimeOffset? Date { get; set; }
        public List<BankExpenditureNoteDetailViewModel> Details { get; set; }
        public string DocumentNo { get; set; }
        public NewSupplierViewModel Supplier { get; set; }
        public double GrandTotal { get; set; }

        public string CurrencyCode { get; set; }
        public int CurrencyId { get; set; }
        public double CurrencyRate { get; set; }
        public bool IsPosted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Details == null || Details.Count.Equals(0))
            {
                yield return new ValidationResult("Minimal 1 Surat Perintah Bayar", new List<string> { "Details" });
            }

            if (Bank == null || Bank.Id <= 0)
            {
                yield return new ValidationResult("Bank harus diisi", new List<string> { "Bank" });
            }

            if (Supplier == null || Supplier._id <= 0)
            {
                yield return new ValidationResult("Supplier harus diisi", new List<string> { "Supplier" });
            }

            if (Date == null)
            {
                yield return new ValidationResult("Tanggal harus diisi", new List<string> { "Date" });
            }
            else if (Date > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal harus lebih kecil dari atau sama dengan tanggal sekarang", new List<string> { "Date" });
            }
        }
    }
}
