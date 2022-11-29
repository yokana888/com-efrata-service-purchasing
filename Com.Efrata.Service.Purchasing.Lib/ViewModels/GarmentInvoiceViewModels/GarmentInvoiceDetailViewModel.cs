using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels
{
    public class GarmentInvoiceDetailViewModel : BaseViewModel
	{
        public long ePOId { get; set; }
        public string ePONo { get; set; }
        public long pOId { get; set; }
        public long pRItemId { get; set; }
        public string pRNo { get; set; }
        public string roNo { get; set; }
        public GarmentProductViewModel product { get; set; }
        public UomViewModel uoms { get; set; }
        public double doQuantity { get; set; }
        public double pricePerDealUnit { get; set; }
        public long dODetailId { get; set; }
        public int paymentDueDays { get; set; }
        public bool useVat { get; set; }
        public bool useIncomeTax { get; set; }
        public string pOSerialNumber { get; set; }
    }
}
