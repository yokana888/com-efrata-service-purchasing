using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel
{
    public class UnitPaymentOrder : BaseModel
    {
        [MaxLength(255)]
        public string UPONo { get; set; }

        /*Division*/
        [MaxLength(255)]
        public string DivisionId { get; set; }
        [MaxLength(255)]
        public string DivisionCode { get; set; }
        [MaxLength(1000)]
        public string DivisionName { get; set; }

        /*Supplier*/
        [MaxLength(255)]
        public string SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        [MaxLength(1000)]
        public string SupplierAddress { get; set; }

        public DateTimeOffset Date { get; set; }

        /*Category*/
        [MaxLength(255)]
        public string CategoryId { get; set; }
        [MaxLength(255)]
        public string CategoryCode { get; set; }
        [MaxLength(1000)]
        public string CategoryName { get; set; }

        /*Currency*/
        [MaxLength(255)]
        public string CurrencyId { get; set; }
        [MaxLength(255)]
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        [MaxLength(1000)]
        public string CurrencyDescription { get; set; }

        [MaxLength(255)]
        public string PaymentMethod { get; set; } //
        [MaxLength(255)]
        public string InvoiceNo { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }

        [MaxLength(255)]
        public string PibNo { get; set; }
        public DateTimeOffset PibDate { get; set; }
        public double ImportDuty { get; set; }
        public double TotalIncomeTaxAmount { get; set; }
        public double TotalVatAmount { get; set; }

        public bool UseIncomeTax { get; set; }
        [MaxLength(255)]
        public string IncomeTaxId { get; set; }
        [MaxLength(1000)]
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        [MaxLength(255)]
        public string IncomeTaxNo { get; set; }
        public DateTimeOffset? IncomeTaxDate { get; set; }
        [MaxLength(255)]
        public string IncomeTaxBy { get; set; }

        public string VatId { get; set; }
        public double VatRate { get; set; }
        public bool UseVat { get; set; }
        [MaxLength(255)]
        public string VatNo { get; set; }
        public DateTimeOffset VatDate { get; set; }

        public string Remark { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public bool IsCorrection { get; set; }
        public bool IsPaid { get; set; }

        public int Position { get; set; }

        public bool IsPosted { get; set; }

        public bool IsCreatedVB { get; set; }
        public string ImportInfo { get; set; }

        public virtual ICollection<UnitPaymentOrderItem> Items { get; set; }
    }
}
