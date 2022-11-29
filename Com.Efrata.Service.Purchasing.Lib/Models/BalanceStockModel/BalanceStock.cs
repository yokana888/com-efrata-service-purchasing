using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BalanceStockModel
{
    public class BalanceStock
    {
        [Key]
        [Column(TypeName = "varchar(30)")]
        public string BalanceStockId { get; set; }
        public int? StockId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string EPOID { get; set; }
        public int? EPOItemId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string RO { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string ArticleNo { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string SmallestUnitQty { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string PeriodeMonth { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string PeriodeYear { get; set; }
        public double? OpenStock { get; set; }
        public double? DebitStock { get; set; }
        public double? CreditStock { get; set; }
        public double? CloseStock { get; set; }
        public decimal? OpenPrice { get; set; }
        public decimal? DebitPrice { get; set; }
        public decimal? CreditPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        [Column(TypeName = "Datetime")]
        public DateTime? CreateDate { get; set; }

    }
}
