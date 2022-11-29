using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel
{
    public class GarmentExternalPurchaseOrderItem : BaseModel
    {

        [MaxLength(20)]
        public string PRNo { get; set; }
        public int PRId { get; set; }

        [MaxLength(20)]
        public string PONo { get; set; }
        public int POId { get; set; }

        [MaxLength(20)]
        public string PO_SerialNumber { get; set; }

        [MaxLength(20)]
        public string RONo { get; set; }
        public int ProductId { get; set; }

        [MaxLength(20)]
        public string ProductCode { get; set; }

        [MaxLength(50)]
        public string ProductName { get; set; }

        [MaxLength(255)]
        public string Article { get; set; }
        public double DefaultQuantity { get; set; }

        [MaxLength(50)]
        public string DefaultUomUnit { get; set; }
        public int DefaultUomId { get; set; }
        public double DealQuantity { get; set; }
        public int DealUomId { get; set; }

        [MaxLength(50)]
        public string DealUomUnit { get; set; }
        public double SmallQuantity { get; set; }
        public int SmallUomId { get; set; }

        [MaxLength(50)]
        public string SmallUomUnit { get; set; }
        public double BudgetPrice { get; set; }
        public double Conversion { get; set; }
        public double UsedBudget { get; set; }
        public double PricePerDealUnit { get; set; }
        public double DOQuantity { get; set; }
        public double ReceiptQuantity { get; set; }

        [MaxLength(4000)]
        public string Remark { get; set; }

        [MaxLength(4000)]
        public string OverBudgetRemark { get; set; }
        public bool IsOverBudget { get; set; }

        public long? UENItemId { get; set; }

        public long GarmentEPOId { get; set; }
        [ForeignKey("GarmentEPOId")]
        public virtual GarmentExternalPurchaseOrder GarmentExternalPurchaseOrder { get; set; }
        public bool IsDispositionCreatedAll { get; set; }


    }
}
