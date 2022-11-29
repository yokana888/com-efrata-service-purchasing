using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities
{
    public abstract class BaseViewModel
    {
        public long _id { get; set; }
        public long Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
    }
}
