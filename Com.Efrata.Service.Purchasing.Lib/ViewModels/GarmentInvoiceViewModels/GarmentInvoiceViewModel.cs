using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels
{
    public class GarmentInvoiceViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string invoiceNo { get; set; }
        public SupplierViewModel supplier { get; set; }
        public DateTimeOffset? invoiceDate { get; set; }
        public CurrencyViewModel currency { get; set; }
        public string vatNo { get; set; }
        public string incomeTaxNo { get; set; }
        public bool useVat { get; set; }
        public bool useIncomeTax { get; set; }
        public bool isPayVat { get; set; }
        public bool isPayTax { get; set; }
        public DateTimeOffset? incomeTaxDate { get; set; }
		public long incomeTaxId { get; set; }
		public string incomeTaxName { get; set; }
		public double incomeTaxRate { get; set; }
        public long vatId { get; set; }
        public double vatRate { get; set; }
        public bool hasInternNote { get; set; }
        public DateTimeOffset? vatDate { get; set; }
        public double totalAmount { get; set; }
        public string poSerialNumber { get; set; }
		public string npn { get; set; }
		public string nph { get; set; }
		public List<GarmentInvoiceItemViewModel> items { get; set; }
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrWhiteSpace(invoiceNo))
			{
				yield return new ValidationResult("No is required", new List<string> { "invoiceNo" });
			}else
			{
				PurchasingDbContext purchasingDbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
				if (purchasingDbContext.GarmentInvoices.Where(DO => DO.InvoiceNo.Equals(invoiceNo) && DO.Id != Id && DO.InvoiceDate.ToOffset((new TimeSpan(7, 0, 0))) == invoiceDate && DO.SupplierId == supplier.Id).Count() > 0)
				{
					yield return new ValidationResult("No is already exist", new List<string> { "no" });
				}
			}

			if (invoiceDate.Equals(DateTimeOffset.MinValue) || invoiceDate == null)
			{
				yield return new ValidationResult("Date is required", new List<string> { "invoiceDate" });
			} 
            else if(invoiceDate > DateTimeOffset.Now.Date)
            {
                yield return new ValidationResult("Date should not be more than today", new List<string> { "invoiceDate" });
            }
			if ( currency == null)
			{
				yield return new ValidationResult("Currency is required", new List<string> { "currency" });
			}
			if (supplier == null)
			{
				yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
			}
			if (useVat == true)
			{
				if (string.IsNullOrWhiteSpace(vatNo) || vatNo == null)
				{
					yield return new ValidationResult("No is required", new List<string> { "vatNo" });
				}
				if (vatDate.Equals(DateTimeOffset.MinValue) || vatDate == null)
				{
					yield return new ValidationResult("Date is required", new List<string> { "vatDate" });
				}
                if (vatId == 0 || vatRate == 0)
                {
                    yield return new ValidationResult("Vat is required", new List<string> { "vat" });
                }
			}
			if (useIncomeTax == true)
			{
				if (string.IsNullOrWhiteSpace(incomeTaxNo) || incomeTaxNo == null)
				{
					yield return new ValidationResult("No is required", new List<string> { "incomeTaxNo" });
				}
				if (incomeTaxDate.Equals(DateTimeOffset.MinValue) || incomeTaxDate == null)
				{
					yield return new ValidationResult("Date is required", new List<string> { "incomeTaxDate" });
				}
                if (string.IsNullOrWhiteSpace(incomeTaxName) || incomeTaxName == null)
                {
                    yield return new ValidationResult("incomeTax is required", new List<string> { "incomeTax" });
                }
            }
			int itemErrorCount = 0;
			int detailErrorCount = 0;

			if (this.items == null || this.items.Count==0)
			{
				yield return new ValidationResult("DeliveryOrder is required", new List<string> { "itemscount" });
			}
			else
			{
				//Enhance Jason Sept 2021
				List<string> arrDoNo = new List<string>();
				foreach(var detailItem in items)
                {
					if (detailItem.deliveryOrder != null)
                    {
						arrDoNo.Add(detailItem.deliveryOrder.doNo);
					}
				}

				string itemError = "[";
				foreach (var item in items)
				{
					itemError += "{";
					
					if (item.deliveryOrder == null)
					{
						itemErrorCount++;
						itemError += "deliveryOrder: 'No deliveryOrder selected', ";
					}

					//Enhance Jason Sept 2019 : Delivery Order No Validation
					if (item.deliveryOrder != null)
                    {
						//Check Duplicate Delivery Order No for 1 Invoice
						//if(arrDoNo.FindAll(e => e == item.deliveryOrder.doNo).Count > 1)
      //                  {
						//	itemErrorCount++;
						//	itemError += "deliveryOrder: 'there is duplication of deliveryOrderNo " + item.deliveryOrder.doNo + "', ";
						//}

                        if (items.ToList().Where(x=>x.deliveryOrder.doNo == item.deliveryOrder.doNo && x.deliveryOrder.doDate == item.deliveryOrder.doDate && x.deliveryOrder.arrivalDate == item.deliveryOrder.arrivalDate).Count() > 1)
                        {
                            itemErrorCount++;
                            itemError += "deliveryOrder: 'there is duplication of deliveryOrderNo " + item.deliveryOrder.doNo + "', ";
                        }

                        //Check if Delivery Order No for Specific Supplier is Existed
                        PurchasingDbContext purchasingDbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
						var detailData = purchasingDbContext.GarmentInvoiceItems.Where(w => w.DeliveryOrderNo == item.deliveryOrder.doNo && w.DODate == item.deliveryOrder.doDate && w.ArrivalDate == item.deliveryOrder.arrivalDate && w.IsDeleted == false).Select(s => new { s.Id, s.InvoiceId, s.DeliveryOrderNo});
						if (detailData.ToList().Count > 0)
                        {
							foreach (var itemDetail in detailData)
                            {
								var headerData = purchasingDbContext.GarmentInvoices.Where(w => w.Id == itemDetail.InvoiceId && w.SupplierId == supplier.Id && w.IsDeleted == false).Select(s => new { s.InvoiceNo });
								if(headerData.ToList().Count > 0)
                                {
									foreach (var itemHeader in headerData)
                                    {
										itemErrorCount++;
										itemError += "deliveryOrder: 'deliveryOrderNo " + item.deliveryOrder.doNo + " already existed on Invoice No " + itemHeader.InvoiceNo.ToString()  + "', ";
									}
								}
							}
                        }
						
					}

					if (item.details == null || item.details.Count.Equals(0))
					{
						itemErrorCount++;
						itemError += "detailscount: 'Details is required', ";
					}
					else
					{
						string detailError = "[";

						foreach (var detail in item.details)
						{
							detailError += "{";
						    
							if (detail.doQuantity == 0)
							{
								detailErrorCount++;
								detailError += "doQuantity: 'DOQuantity can not 0', ";
							}
							
							detailError += "}, ";
						}

						detailError += "]";

						if (detailErrorCount > 0)
						{
							itemErrorCount++;
							itemError += $"details: {detailError}, ";
						}
					}

					itemError += "}, ";
				}

				itemError += "]";
				
				if (itemErrorCount > 0)
					yield return new ValidationResult(itemError, new List<string> { "items" });
			}
		}
	}
}
