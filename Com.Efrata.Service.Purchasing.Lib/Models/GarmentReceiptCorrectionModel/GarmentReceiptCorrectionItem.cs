using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel
{
    public class GarmentReceiptCorrectionItem : StandardEntity<long>
    {
        public long CorrectionId { get; set; }
        [ForeignKey("CorrectionId")]
        public virtual GarmentReceiptCorrection GarmentReceiptCorrection { get; set; }
        public long URNItemId { get; set; }
        public long DODetailId { get; set; }
        public long EPOItemId { get; set; }
        public long POItemId { get; set; }
        public long PRItemId { get; set; }
        [MaxLength(500)]
        public string POSerialNumber { get; set; }
        public long ProductId { get; set; }
        [MaxLength(500)]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        [MaxLength(255)]
        public string RONo { get; set; }
        public double Conversion { get; set; }
        public double CorrectionConversion { get; set; }
        public double Quantity { get; set; }
        public long UomId { get; set; }
        public string UomUnit { get; set; }
        public double CorrectionQuantity { get; set; }
        public double SmallQuantity { get; set; }
        public long SmallUomId { get; set; }
        public string SmallUomUnit { get; set; }
        public double PricePerDealUnit { get; set; }
        [MaxLength(500)]
        public string DesignColor { get; set; }

    }
}
