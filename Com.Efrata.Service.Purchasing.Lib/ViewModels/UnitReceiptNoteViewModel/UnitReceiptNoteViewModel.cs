using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel
{
    public class UnitReceiptNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public bool _deleted { get; set; }
        public SupplierViewModel supplier { get; set; }
        public string no { get; set; }
        public DateTimeOffset date { get; set; }
        public UnitViewModel unit { get; set; }
        public string pibNo { get; set; }
        public string incomeTaxNo { get; set; }
        public string doNo { get; set; }
        public string doId { get; set; }
        public List<UnitReceiptNoteItemViewModel> items { get; set; }

        public StorageViewModel storage { get; set; }

        public bool isStorage { get; set; }
        public string remark { get; set; }

        public bool isPaid { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "date" });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(doId))
                {
                    DeliveryOrderFacade Service = (DeliveryOrderFacade)validationContext.GetService(typeof(DeliveryOrderFacade));
                    DeliveryOrder sj = Service.ReadById(Convert.ToInt32(doId)).Item1;
                    if (this.date < sj.DODate.ToOffset((new TimeSpan(7, 0, 0))).Date)
                    {
                        yield return new ValidationResult("Date should not be less than delivery order date", new List<string> { "date" });
                    }
                }
                
            }
            
            if (this.isStorage)
            {
                if (storage == null||string.IsNullOrWhiteSpace(storage._id))
                {
                    yield return new ValidationResult("Storage is required", new List<string> { "storage" });
                }
            }
            if (this.unit == null || string.IsNullOrWhiteSpace(unit._id))
            {
                yield return new ValidationResult("Unit is required", new List<string> { "unitId" });
            }
            if (this.supplier == null || string.IsNullOrWhiteSpace(supplier._id))
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }

            int itemErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";

                foreach (UnitReceiptNoteItemViewModel item in items)
                {
                    itemError += "{";

                    if (item.product == null || string.IsNullOrWhiteSpace(item.product._id))
                    {
                        itemErrorCount++;
                        itemError += "product: 'Product is required', ";
                    }
                    //else
                    //{
                    //    var itemsExist = items.Where(i => i.product != null && item.product != null && i.product._id.Equals(item.product._id)).Count();
                    //    if (itemsExist > 1)
                    //    {
                    //        itemErrorCount++;
                    //        itemError += "product: 'Product is duplicate', ";
                    //    }
                    //}

                    if (item.deliveredQuantity <= 0)
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