using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel
{
    public class BankExpenditureNoteItemModel : BaseModel
    {
        public long UnitPaymentOrderItemId { get; set; }
        public double Price { get; set; }
        [MaxLength(255)]
        public string ProductCode { get; set; }
        [MaxLength(255)]
        public string ProductId { get; set; }
        [MaxLength(255)]
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        [MaxLength(255)]
        public string UnitCode { get; set; }
        [MaxLength(255)]
        public string UnitId { get; set; }
        [MaxLength(255)]
        public string UnitName { get; set; }
        [MaxLength(255)]
        public string Uom { get; set; }
        public long URNId { get; set; }
        [MaxLength(255)]
        public string URNNo { get; set; }
        public double PaidPrice { get; set; }
        public virtual long BankExpenditureNoteDetailId { get; set; }
        [ForeignKey("BankExpenditureNoteDetailId")]
        public virtual BankExpenditureNoteDetailModel BankExpenditureNoteDetail { get; set; }
    }
}