using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel
{
    public class PurchasingDispositionDetail :BaseModel
    {

        public string EPODetailId { get; set; }
        public string PRId { get; set; }
        public string PRNo { get; set; }

        //public string CategoryId { get; set; }
        //public string CategoryName { get; set; }
        //public string CategoryCode { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public double DealQuantity { get; set; }
        public string DealUomUnit { get; set; }
        public string DealUomId { get; set; }
        public double PaidQuantity { get; set; }
        public double PricePerDealUnit { get; set; }
        public double PriceTotal { get; set; }
        public double PaidPrice { get; set; }

        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public string UnitId { get; set; }

        //public string DivisionName { get; set; }
        //public string DivisionCode { get; set; }
        //public string DivisionId { get; set; }

        public virtual long PurchasingDispositionItemId { get; set; }
        [ForeignKey("PurchasingDispositionItemId")]
        public virtual PurchasingDispositionItem PurchasingDispositionItem { get; set; }
    }
}
