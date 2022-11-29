using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel
{
    public class GarmentSupplierBalanceDebt : BaseModel
    {
        [MaxLength(255)]
        public long SupplierId { get; set; }
        [MaxLength(255)]
        public string SupplierCode { get; set; }
        [MaxLength(1000)]
        public string SupplierName { get; set; }
        public long Year { get; set; }
        public bool? Import { get; set; }
        public double TotalValas { get; set; }
        public double TotalAmountIDR { get; set; }
        public double DOCurrencyRate { get; set; }
        public long? DOCurrencyId { get; set; }
        public string DOCurrencyCode { get; set; }
        public string CodeRequirment { get; set; }

        public virtual ICollection<GarmentSupplierBalanceDebtItem> Items { get; set; }
    }
}
