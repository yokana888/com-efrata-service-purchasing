using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel
{
    public class InternalPurchaseOrderCorrection : StandardEntity<long>
    {
        public long UnitPaymentCorrectionId { get; set; }
        public long UnitPaymentCorrectionItemId { get; set; }


        public DateTimeOffset CorrectionDate { get; set; }

        [MaxLength(128)]
        public string CorrectionNo { get; set; }

        public double CorrectionQuantity { get; set; }

        public double CorrectionPriceTotal { get; set; }

        [MaxLength(4000)]
        public string CorrectionRemark { get; set; }

        public virtual long POFulfillmentId { get; set; }
        [ForeignKey("POFulfillmentId")]
        public virtual InternalPurchaseOrderFulFillment InternalPurchaseOrderFulFillment { get; set; }
    }
}
