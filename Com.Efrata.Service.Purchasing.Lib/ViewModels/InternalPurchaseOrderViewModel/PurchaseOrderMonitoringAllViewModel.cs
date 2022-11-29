using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class PurchaseOrderMonitoringAllViewModel : BaseViewModel
    {
        public int index { get; set; }
        public string prNo { get; set; }
        public string createdDatePR { get; set; }
        public string prDate { get; set; }
        public string category { get; set; }
        public string budget { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public double? pricePerDealUnit { get; set; }
        public double? priceTotal { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string receivedDatePO { get; set; }
        public string division { get; set; }

        //EPO
        public string epoDate { get; set; }
        public string epoCreatedDate { get; set; }
        public string epoExpectedDeliveryDate { get; set; }
        public string epoDeliveryDate { get; set; }
        public string epoNo { get; set; }
        public string currencyCode { get; set; }
        public double epoQty { get; set; }
        public string Uomepo { get; set; }

        //DO
        public string doDate { get; set; }
        public string doDeliveryDate { get; set; }
        public string doNo { get; set; }
        public long doDetailId { get; set; }
        public double doQuantity { get; set; }
        public string doUom { get; set; }

        //URN
        public string urnDate { get; set; }
        public string urnNo { get; set; }
        public double urnQuantity { get; set; }
        public string urnUom { get; set; }
        public string urnProductCode { get; set; }

        public string paymentDueDays { get; set; }
        public string invoiceDate { get; set; }
        public string invoiceNo { get; set; }

        //UPO
        public string upoDate { get; set; }
        public string upoNo { get; set; }
        public double upoPriceTotal { get; set; }
        public string dueDate { get; set; }

        //vat
        public string vatDate { get; set; }
        public string vatNo { get; set; }
        public string vatValue { get; set; }

        //incomeTax
        public DateTimeOffset? incomeTaxDate { get; set; }
        public string incomeTaxNo { get; set; }
        public double incomeTaxValue { get; set; }

        //correction
        public string correctionDate { get; set; }
        public string correctionNo { get; set; }
        public string correctionType { get; set; }
        public double priceBefore { get; set; }
        public double priceAfter { get; set; }
        public double priceTotalAfter { get; set; }
        public double priceTotalBefore { get; set; }
        public double qtyCorrection { get; set; }
        public string valueCorrection { get; set; }
        public string correctionRemark { get; set; }
        public string correctionDates { get; set; }
        public string correctionQtys { get; set; }

        public string remark { get; set; }
        public string status { get; set; }
        public string staff { get; set; }
    }
}
