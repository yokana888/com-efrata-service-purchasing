using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentNoteItemsInvoiceDetailsDto
    {
        public long? DOId { get; set; }
        public string DONo { get; set; }

        public long? EPOId { get; set; }
        public string EPONo { get; set; }

        public string POSerialNumber { get; set; }
        public string RONo { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentType { get; set; }
        public int? PaymentDueDays { get; set; }
        public DateTimeOffset? PaymentDueDate { get; set; }
        public DateTimeOffset? DODate { get; set; }
        public int? InvoiceDetailId { get; set; }

        /*Product*/
        public string ProductCode { get; set; }
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public double? Quantity { get; set; }

        /* Unit */
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        /*UOM*/
        public long? UOMId { get; set; }
        public string UOMUnit { get; set; }

        public double? PricePerDealUnit { get; set; }
        public double? PriceTotal { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public double? InvoiceTotalAmount { get; set; }
        public Models.GarmentInvoiceModel.GarmentInvoiceDetail GarmentInvoiceDetail { get; set; }
        public Models.GarmentDeliveryOrderModel.GarmentDeliveryOrder GarmentDeliveryOrder { get; set; }
    }
}
