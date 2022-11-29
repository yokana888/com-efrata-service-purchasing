using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel
{
    public class PurchaseRequestViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string no { get; set; }
        public DateTimeOffset? date { get; set; }
        public DateTimeOffset? expectedDeliveryDate { get; set; }
        public BudgetViewModel budget { get; set; }
        public UnitViewModel unit { get; set; }
        public CategoryViewModel category { get; set; }
        public bool isPosted { get; set; }
        public bool isUsed { get; set; }
        public string remark { get; set; }
        public bool @internal { get; set; }
        public List<PurchaseRequestItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "date" });
            }
            if (this.expectedDeliveryDate.Equals(DateTimeOffset.MinValue) || this.expectedDeliveryDate == null)
            {
                yield return new ValidationResult("ExpectedDeliveryDate is required", new List<string> { "expectedDeliveryDate" });
            }
            else if (this.date != null && this.date > this.expectedDeliveryDate)
            {
                yield return new ValidationResult("Date is greater than expected delivery date", new List<string> { "expectedDeliveryDate" });
            }
            if (this.budget == null)
            {
                yield return new ValidationResult("Budget is required", new List<string> { "budget" });
            }
            if (this.unit == null)
            {
                yield return new ValidationResult("Unit is required", new List<string> { "unit" });
            }
            if (this.category == null)
            {
                yield return new ValidationResult("Category is required", new List<string> { "category" });
            }

            int itemErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";

                foreach (PurchaseRequestItemViewModel item in items)
                {
                    itemError += "{";

                    if (item.product == null || string.IsNullOrWhiteSpace(item.product._id))
                    {
                        itemErrorCount++;
                        itemError += "product: 'Product is required', ";
                    }
                    else
                    {
                        var itemsExist = items.Where(i => i.product != null && item.product != null && i.product._id.Equals(item.product._id)).Count();
                        if (itemsExist > 1)
                        {
                            itemErrorCount++;
                            itemError += "product: 'Product is duplicate', ";
                        }
                    }

                    if (item.quantity <= 0)
                    {
                        itemErrorCount++;
                        itemError += "quantity: 'Quantity should be more than 0'";
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
