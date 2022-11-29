using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel
{
    public class GarmentUnitExpenditureNote : BaseModel
    {
        [MaxLength(255)]
        public string UENNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        [MaxLength(255)]
        public string ExpenditureType { get; set; }
        [MaxLength(1000)]
        public string ExpenditureTo { get; set; }

        public long UnitDOId { get; set; }
        [MaxLength(255)]
        public string UnitDONo { get; set; }

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

        public long UnitRequestId { get; set; }
        [MaxLength(255)]
        public string UnitRequestCode { get; set; }
        [MaxLength(1000)]
        public string UnitRequestName { get; set; }

        public long StorageRequestId { get; set; }
        [MaxLength(255)]
        public string StorageRequestCode { get; set; }
        [MaxLength(1000)]
        public string StorageRequestName { get; set; }

        public bool IsPreparing { get; set; }

        public bool IsTransfered { get; set; }

        public bool IsReceived { get; set; }

        public virtual ICollection<GarmentUnitExpenditureNoteItem> Items { get; set; }

    }
}
