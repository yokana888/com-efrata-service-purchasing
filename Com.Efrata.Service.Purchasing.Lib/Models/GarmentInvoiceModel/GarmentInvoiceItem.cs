using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel
{
    public class GarmentInvoiceItem : StandardEntity<long>
    {

        public long DeliveryOrderId { get; set; }
        public string DeliveryOrderNo { get; set; }
        public DateTimeOffset DODate { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }
        public double TotalAmount { get; set; }
		public string PaymentType { get; set; }
		public string PaymentMethod { get; set; }
		
		public virtual ICollection<GarmentInvoiceDetail> Details { get; set; }
        public virtual long InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual GarmentInvoice GarmentInvoice { get; set; }
    }
}
