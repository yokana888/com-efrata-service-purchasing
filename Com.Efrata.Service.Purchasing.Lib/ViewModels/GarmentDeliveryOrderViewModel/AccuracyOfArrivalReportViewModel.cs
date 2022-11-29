using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel
{
    public class AccuracyOfArrivalReportViewModel : BaseViewModel
    {
        public SupplierViewModel supplier { get; set; } //SJ
        public string poSerialNumber { get; set; } //SJ
        public DateTimeOffset prDate { get; set; } //date PR
        public DateTimeOffset poDate { get; set; } //createdDate PO
        public DateTimeOffset epoDate { get; set; } //date EPO
        public GarmentProductViewModel product { get; set; } //SJ
        public string article { get; set; } // article EPO
        public string roNo { get; set; } //SJ
        public DateTimeOffset shipmentDate { get; set; } //PR
        public DateTimeOffset doDate { get; set; } //dodate SJ
        public string status { get; set; } //OK ? NotOK
        public string staff { get; set; } //CreatedBy SJ
        public string category { get; set; } //GarmentCategory Based On Product 
        public string doNo { get; set; } //SJ
        public int dateDiff { get; set; }
        public string ok_notOk { get; set; }
        public int percentOk_notOk { get; set; }
        public int jumlah { get; set; }
        public int jumlahOk { get; set; }

        public string paymentMethod { get; set; }
        public string paymentType { get; set; }
    }
}
