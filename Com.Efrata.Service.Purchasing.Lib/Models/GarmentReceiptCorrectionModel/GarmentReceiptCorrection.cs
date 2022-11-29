using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel
{
    public class GarmentReceiptCorrection : BaseModel
    {
        [MaxLength(255)]
        public string CorrectionType { get; set; }
        [MaxLength(255)]
        public string CorrectionNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public long URNId { get; set; }
        [MaxLength(255)]
        public string URNNo { get; set; }
        public long UnitId { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(255)]
        public string UnitName { get; set; }
        public long StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(500)]
        public string StorageName { get; set; }
        public string Remark { get; set; }

        public virtual ICollection<GarmentReceiptCorrectionItem> Items { get; set; }

    }
}
