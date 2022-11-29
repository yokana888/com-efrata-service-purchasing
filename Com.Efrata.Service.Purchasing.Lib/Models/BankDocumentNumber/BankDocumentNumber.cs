using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.BankDocumentNumber
{
    public class BankDocumentNumber : StandardEntity<long>
    {
        [MaxLength(255)]
        public string BankCode { get; set; }
        [MaxLength(25)]
        public string Type { get; set; } //K or M
        public int LastDocumentNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
