using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.Expedition
{
    public class PPHBankExpenditureNote : StandardEntity, IValidatableObject
    {
        public DateTimeOffset Date { get; set; }
        public string No { get; set; }
        public string BGNo { get; set; }
        public string IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public string BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public double TotalIncomeTax { get; set; }
        public double TotalDPP { get; set; }
        public string Currency { get; set; }
        public double? CurrencyRate { get; set; }
        public bool IsPosted { get; set; }
        public virtual ICollection<PPHBankExpenditureNoteItem> Items { get; set; }
        [MaxLength(128)]
        public string DivisionName { get; set; }
        [MaxLength(128)]
        public string DivisionCode { get; set; }
        public string DivisionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
