using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class FormDetailDto
    {
        public int Id { get; set; }
        public int ROId { get; set; }
        public string RONo { get; set; }
        public string IPONo { get; set; }
        public int IPOId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public double QTYOrder { get; set; }
        public string QTYUnit { get; set; }
        public double QTYRemains { get; set; }
        public double PricePerQTY { get; set; }
        public double PriceTotal { get; set; }
        public double QTYPaid { get; set; }
        public double PaidPrice { get; set; }
        public double PercentageOverQTY { get; set; }
        public int EPO_POId { get; set; }
        public int GarmentDispositionPurchaseItemId { get; set; }
        public double DispositionAmountPaid { get; set; }
        public double DispositionAmountCreated { get; set; }
        public double DispositionQuantityCreated { get; set; }
        public double DispositionQuantityPaid { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
    }
}
