using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel
{
   public class GarmentDeliveryOrderReportViewModel
    {
        public string no { get; set; }
        public DateTimeOffset supplierDoDate { get; set; } // DODate
        public DateTimeOffset date { get; set; } // ArrivalDate
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string ePONo { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string productRemark { get; set; }
        public double dealQuantity { get; set; }
        public double dOQuantity { get; set; }
        public double remainingQuantity { get; set; }
        public string uomUnit { get; set; }
        public long ePODetailId { get; set; }
        public DateTimeOffset createdUtc { get; set; }
        public string shipmentType { get; set; }
        public string shipmentNo { get; set; }
        public string prNo { get; set; }
        public string prRefNo { get; set; }
        public string roNo { get; set; }
        public double price { get; set; }
        public string doCurrencyCode { get; set; }
        public double? doCurrencyRate { get; set; }
        public string remark { get; set; }
        public string createdBy { get; set; }
        public bool isCustoms { get; set; }
        public string URNNo { get; set; }
        public DateTimeOffset URNDate { get; set; }
        public decimal urnQuantity { get; set; }
        public string urnUom { get; set; }
        public string UnitName { get; set; }
        public string EPOcreatedBy { get; set; }
        public string INNo { get; set; }
        public string TermPayment { get; set; }
        public string URNType { get; set; }
        public string BillNo { get; set; }
        public string PaymentBill { get; set; }
        public string BeacukaiNo { get; set; }
        public string BeacukaiType { get; set; }
        public DateTimeOffset BeacukaiDate { get; set; }
        public DateTimeOffset BCDate { get; set; }
        public string diffdate { get; set; }

    }
}
