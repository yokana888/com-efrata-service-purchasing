using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel
{
    public class PurchaseRequestItem : StandardEntity<long>
    {
        /* Product */
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(4000)]
        public string ProductName { get; set; }
        [MaxLength(255)]
        public string Uom { get; set; }
        public string UomId { get; set; }
        public string Status { get; set; }

        public double Quantity { get; set; }
        public string Remark { get; set; }

        public virtual long PurchaseRequestId { get; set; }
        [ForeignKey("PurchaseRequestId")]
        public virtual PurchaseRequest PurchaseRequest { get; set; }
    }
}
