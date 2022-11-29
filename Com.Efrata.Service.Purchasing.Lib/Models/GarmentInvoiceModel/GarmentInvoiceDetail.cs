using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel
{
    public class GarmentInvoiceDetail : StandardEntity<long>
    {
        public long EPOId { get; set; }
		public long DODetailId { get; set; }
		public string EPONo { get; set; }
        public long IPOId { get; set; }
        public long PRItemId { get; set; }
        public string PRNo { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public string RONo { get; set; }
		public string POSerialNumber { get; set; }
		public double DOQuantity { get; set; }
        public double PricePerDealUnit { get; set; }
		public int PaymentDueDays { get; set; }
		public virtual long InvoiceItemId { get; set; }
        [ForeignKey("InvoiceItemId")]
        public virtual GarmentInvoiceItem GarmentInvoiceItem { get; set; }
    }
}
