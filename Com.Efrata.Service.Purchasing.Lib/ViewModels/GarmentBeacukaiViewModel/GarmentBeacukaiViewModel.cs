using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel
{
	public class GarmentBeacukaiViewModel : BaseViewModel, IValidatableObject
	{
        public string UId { get; set; }
        public string beacukaiNo { get; set; }
		public string billNo { get; set; }
		public DateTimeOffset beacukaiDate { get; set; }
		public DateTimeOffset validationDate { get; set; }
		public SupplierViewModel supplier  { get; set; }
		public double packagingQty { get; set; }
		public string packaging { get; set; }
		public string customType { get; set; }
		public double bruto { get; set; }
		public double netto { get; set; }
		public CurrencyViewModel currency{ get; set; }
        public DateTimeOffset? arrivalDate { get; set; }
        public List<GarmentBeacukaiItemViewModel> items { get; set; }
        public string importValue { get; set; }
		public bool customCategory { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrWhiteSpace(beacukaiNo))
			{
				yield return new ValidationResult("No is required", new List<string> { "beacukaiNo" });
			}
			//else
			//{
			//	PurchasingDbContext purchasingDbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
			//	if (purchasingDbContext.GarmentBeacukais.Where(DO => DO.BeacukaiNo.Equals(beacukaiNo) && DO.Id != Id && DO.BeacukaiDate.ToOffset((new TimeSpan(7, 0, 0))) == beacukaiDate && DO.SupplierId == supplier.Id).Count() > 0)
			//	{
			//		yield return new ValidationResult("No is already exist", new List<string> { "no" });
			//	}
			//}

			if (beacukaiDate.Equals(DateTimeOffset.MinValue) || beacukaiDate == null)
			{
				yield return new ValidationResult("Date is required", new List<string> { "beacukaiDate" });
			}
			//if (validationDate.Equals(DateTimeOffset.MinValue) || beacukaiDate == null)
			//{
			//	yield return new ValidationResult("Validate Date is required", new List<string> { "validationDate" });
			//}
			if (currency == null )
			{
				yield return new ValidationResult("Currency is required", new List<string> { "currency" });
			}
			if (customType == null || customType =="")
			{
				yield return new ValidationResult("Type is required", new List<string> { "customType" });
			}
			if (supplier == null)
			{
				yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
			}
			if ((packagingQty == 0))
			{
				yield return new ValidationResult("Qty is required", new List<string> { "packagingQty" });
			}
			if ((netto == 0))
			{
				yield return new ValidationResult("Netto is required", new List<string> { "netto" });
			}
			if ((bruto == 0))
			{
				yield return new ValidationResult("Bruto is required", new List<string> { "bruto" });
			}
			if (string.IsNullOrWhiteSpace(packaging))
			{
				yield return new ValidationResult("Packaging is required", new List<string> { "packaging" });
			}
			if (this.items == null || this.items.Count == 0)
			{
				yield return new ValidationResult("DeliveryOrder is required", new List<string> { "itemscount" });
			}
            //else
            //{
            //    int itemErrorCount = 0;

            //    string itemError = "[";
            //    foreach (var item in items)
            //    {
            //        itemError += "{";

            //        //if (item.deliveryOrder == null)
            //        //{
            //        //	itemErrorCount++;
            //        //	itemError += "deliveryOrder: 'No deliveryOrder selected', ";
            //        //}
            //        //itemError += "}, ";
            //    }

            //    itemError += "]";

            //    if (itemErrorCount > 0)
            //        yield return new ValidationResult(itemError, new List<string> { "items" });
            //}
        }
	}
}
