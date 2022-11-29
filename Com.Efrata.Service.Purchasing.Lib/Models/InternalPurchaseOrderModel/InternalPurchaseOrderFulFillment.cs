using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel
{
    public class InternalPurchaseOrderFulFillment : StandardEntity<long>
    {
        public InternalPurchaseOrderFulFillment()
        {
            Corrections = new List<InternalPurchaseOrderCorrection>();
        }

        public long DeliveryOrderId { get; set; }
        public long DeliveryOrderItemId { get; set; }
        public long DeliveryOrderDetailId { get; set; }

        public long UnitReceiptNoteId { get; set; }
        public long UnitReceiptNoteItemId { get; set; }

        public long UnitPaymentOrderId { get; set; }
        public long UnitPaymentOrderItemId { get; set; }
        public long UnitPaymentOrderDetailId { get; set; }


        //DO
        [MaxLength(64)]
        public string DeliveryOrderNo { get; set; }

        public double DeliveryOrderDeliveredQuantity { get; set; }

        public DateTimeOffset DeliveryOrderDate { get; set; }

        public DateTimeOffset SupplierDODate { get; set; }


        // URN
        [MaxLength(64)]
        public string UnitReceiptNoteNo { get; set; }

        public DateTimeOffset UnitReceiptNoteDate { get; set; }

        public double UnitReceiptNoteDeliveredQuantity { get; set; }

        [MaxLength(128)]
        public string UnitReceiptNoteUom { get; set; }
        
        [MaxLength(64)]
        public string UnitReceiptNoteUomId { get; set; }


        //UPO
        public DateTimeOffset InvoiceDate { get; set; }

        [MaxLength(64)]
        public string InvoiceNo { get; set; }

        public DateTimeOffset InterNoteDate { get; set; }

        [MaxLength(64)]
        public string InterNoteNo { get; set; }

        public double InterNoteValue { get; set; }

        public DateTimeOffset InterNoteDueDate { get; set; }

        public bool UnitPaymentOrderUseVat { get; set; }

        public DateTimeOffset UnitPaymentOrderVatDate { get; set; }

        public string UnitPaymentOrderVatNo { get; set; }

        public bool UnitPaymentOrderUseIncomeTax { get; set; }

        public DateTimeOffset UnitPaymentOrderIncomeTaxDate { get; set; }

        public string UnitPaymentOrderIncomeTaxNo { get; set; }

        public double UnitPaymentOrderIncomeTaxRate { get; set; }

        public virtual ICollection<InternalPurchaseOrderCorrection> Corrections { get; set; }

        public virtual long POItemId { get; set; }
        [ForeignKey("POItemId")]
        public virtual InternalPurchaseOrderItem InternalPurchaseOrderItem { get; set; }
    }
}
