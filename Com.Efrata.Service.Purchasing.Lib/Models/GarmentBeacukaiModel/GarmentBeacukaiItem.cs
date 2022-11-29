using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel
{
	public class GarmentBeacukaiItem : StandardEntity<long>
	{
		public long GarmentDOId { get; set; }
		public string GarmentDONo { get; set; }
		public DateTimeOffset ArrivalDate { get; set; }
		public DateTimeOffset DODate { get; set; }
		public double TotalQty { get; set; }
		public decimal TotalAmount { get; set; }
		public virtual long BeacukaiId { get; set; }
		[ForeignKey("BeacukaiId")]
		public virtual GarmentBeacukai GarmentBeacukai { get; set; }
	}
}
