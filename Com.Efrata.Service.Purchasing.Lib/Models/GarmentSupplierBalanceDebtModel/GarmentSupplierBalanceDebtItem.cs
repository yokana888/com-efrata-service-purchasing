using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel
{
    public class GarmentSupplierBalanceDebtItem : BaseModel
    {
        [MaxLength(50)]
        public string BillNo { get; set; }
        public double Valas { get; set; }
        public double IDR { get; set; }
        public DateTimeOffset ArrivalDate { get; set; }
        public string InternNo { get; set; }
        public string DONo { get; set; }
        public long DOId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }


        public virtual long GarmentDebtId { get; set; }
        [ForeignKey("GarmentDebtId")]
        public virtual GarmentSupplierBalanceDebt GarmentSupplierBalanceDebt { get; set; }
    }
}
