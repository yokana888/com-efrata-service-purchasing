using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel
{
    public class GarmentUnitDeliveryOrder : BaseModel
    {
        [MaxLength(255)]
        public string UnitDOType { get; set; }
        [MaxLength(255)]
        public string UnitDONo { get; set; }
        public DateTimeOffset UnitDODate { get; set; }

        public long UnitRequestId { get; set; }
        [MaxLength(255)]
        public string UnitRequestCode { get; set; }
        [MaxLength(1000)]
        public string UnitRequestName { get; set; }

        public long UnitSenderId { get; set; }
        [MaxLength(255)]
        public string UnitSenderCode { get; set; }
        [MaxLength(1000)]
        public string UnitSenderName { get; set; }

        public long StorageId { get; set; }
        [MaxLength(255)]
        public string StorageCode { get; set; }
        [MaxLength(1000)]
        public string StorageName { get; set; }

        [MaxLength(255)]
        public string RONo { get; set; }
        [MaxLength(1000)]
        public string Article { get; set; }
        public bool IsUsed { get; set; }

        public long DOId { get; set; }
        [MaxLength(255)]
        public string DONo { get; set; }

        public long CorrectionId { get; set; }
        [MaxLength(255)]
        public string CorrectionNo { get; set; }


        public long StorageRequestId { get; set; }
        [MaxLength(255)]
        public string StorageRequestCode { get; set; }
        [MaxLength(1000)]
        public string StorageRequestName { get; set; }

        public long UENFromId { get; set; }
        [MaxLength(255)]
        public string UENFromNo { get; set; }
        public long UnitDOFromId { get; set; }
        [MaxLength(255)]
        public string UnitDOFromNo { get; set; }

        [MaxLength(4000)]
        public string OtherDescription { get; set; }
        public long SupplierReceiptId { get; set; }
        [MaxLength(255)]
        public string SupplierReceiptCode { get; set; }
        [MaxLength(255)]
        public string SupplierReceiptName { get; set; }
        
        

        public virtual List<GarmentUnitDeliveryOrderItem> Items { get; set; }
    }
}
