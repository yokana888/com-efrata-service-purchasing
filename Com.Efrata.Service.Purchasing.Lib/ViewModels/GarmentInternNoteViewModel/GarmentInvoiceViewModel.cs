using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class GarmentInvoiceInternNoteViewModel
    {
        public GarmentInvoice GarmentInvoices { get; set; }
        public CategoryDto Category { get; set; }
        public string PaymentMethod { get; internal set; }
        public string BillsNo { get; internal set; }
        public string PaymentBills { get; internal set; }
        public string DeliveryOrdersNo { get; internal set; }
    }
}
