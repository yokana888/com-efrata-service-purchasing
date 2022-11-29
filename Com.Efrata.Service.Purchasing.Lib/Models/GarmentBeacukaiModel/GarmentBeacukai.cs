using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel
{
	public class GarmentBeacukai : BaseModel
	{
		public long BCIdTemp { get; set; }
		public string BeacukaiNo { get; set; }
		public string BillNo { get; set; }
		public DateTimeOffset BeacukaiDate { get; set; }
		public DateTimeOffset ValidationDate { get; set; }
        public long SupplierId { get; set; }
		public string SupplierCode { get; set; }
		public string SupplierName { get; set; }
		public double PackagingQty { get; set; }
		public string Packaging { get; set; }
		public double Bruto { get; set; }
		public double Netto { get; set; }
		public long CurrencyId { get; set; }
		public string CurrencyCode { get; set; }
		public string CustomsType { get; set; }
		public bool CustomsCategory { get; set; }
        public string ImportValue { get; set; }
        public int ImportValueId { get; set; }
        public DateTimeOffset? ArrivalDate { get; set; }
        public virtual ICollection<GarmentBeacukaiItem> Items { get; set; }
	}
}
