using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel
{
    public class GarmentDeliveryOrder : BaseModel
    {
		public long CustomsId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }
        public DateTimeOffset DODate { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }

        /* Supplier */
        [MaxLength(255)]
        public long SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        [MaxLength(255)]
        public string Country { get; set; }

        public string ShipmentType { get; set; }
        public string ShipmentNo { get; set; }

        public string Remark { get; set; }
        public bool IsClosed { get; set; }
        public bool IsCustoms { get; set; }
        public bool IsInvoice { get; set; }
        public string InternNo { get; set; }
        [MaxLength(50)]
        public string BillNo { get; set; }
        [MaxLength(50)]
        public string PaymentBill { get; set; }
        public double TotalAmount { get; set; }

        public bool? IsCorrection { get; set; }

        public bool? UseVat { get; set; }
        public bool? UseIncomeTax { get; set; }
        public bool? IsPayVAT { get; set; }
        public bool? IsPayIncomeTax { get; set; }

        [MaxLength(255)]
        public int? IncomeTaxId { get; set; }
        [MaxLength(255)]
        public string IncomeTaxName { get; set; }
        public double? IncomeTaxRate { get; set; }

        [MaxLength(255)]
        public int? VatId { get; set; }
        public double? VatRate { get; set; }

        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public long? DOCurrencyId { get; set; }
        public string DOCurrencyCode { get; set; }
        public double? DOCurrencyRate { get; set; }
        public string CustomsCategory { get; set; }


        public virtual IEnumerable<GarmentDeliveryOrderItem> Items { get; set; }
        public bool SupplierIsImport { get; set; }
    }
}
