using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel
{
    public class DeliveryOrderViewModel : BaseViewModel, IValidatableObject
    {
        //public DeliveryOrderViewModel()
        //{
        //    items = new List<DeliveryOrderItemViewModel>();
        //    unitReceiptNoteIds = new List<long>();
        //}

        public string UId { get; set; }
        public string no { get; set; }
        public DateTimeOffset? supplierDoDate { get; set; } // DODate
        public DateTimeOffset? date { get; set; } // ArrivalDate

        public SupplierViewModel supplier { get; set; }

        public string remark { get; set; }
        public bool isClosed { get; set; }
        public List<DeliveryOrderItemViewModel> items { get; set; }

        public List<long> unitReceiptNoteIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrWhiteSpace(no))
            {
                yield return new ValidationResult("No is required", new List<string> { "no" });
            }
            else
            {
                if(supplier != null)
                {
                    PurchasingDbContext purchasingDbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
                    if (purchasingDbContext.DeliveryOrders.Where(DO => DO.DONo.Equals(no) && DO.Id != this._id && DO.DODate.ToOffset((new TimeSpan(7, 0, 0))) == supplierDoDate && DO.SupplierId == supplier._id && DO.ArrivalDate.ToOffset((new TimeSpan(7, 0, 0))) == date).Count() > 0)
                    {
                        yield return new ValidationResult("No is already exist", new List<string> { "no" });
                    }
                }
               
            }
            if (date.Equals(DateTimeOffset.MinValue) || date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "date" });
            }
            if (supplierDoDate.Equals(DateTimeOffset.MinValue) || supplierDoDate == null)
            {
                yield return new ValidationResult("SupplierDoDate is required", new List<string> { "supplierDoDate" });
            }
            if (supplier == null)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }

            int itemErrorCount = 0;
            int detailErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("PurchaseOrderExternal is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";

                foreach (var item in items)
                {
                    itemError += "{";

                    if (item.purchaseOrderExternal == null || item.purchaseOrderExternal._id == 0)
                    {
                        itemErrorCount++;
                        itemError += "purchaseOrderExternal: 'No PurchaseOrderExternal selected', ";
                    }
                    else if (items.Count(i => i.purchaseOrderExternal._id == item.purchaseOrderExternal._id) > 1 && _id == 0)
                    {
                        itemErrorCount++;
                        itemError += "purchaseOrderExternal: 'Data sudah ada', ";
                    }
                    else if (item.fulfillments == null || item.fulfillments.Count.Equals(0))
                    {
                        itemErrorCount++;
                        itemError += "fulfillmentscount: 'PurchaseRequest is required', ";
                    }
                    else
                    {
                        string detailError = "[";

                        foreach (var detail in item.fulfillments)
                        {
                            detailError += "{";

                            //var duplicateItems = items.Where(i => i.purchaseOrderExternal._id == item.purchaseOrderExternal._id && i._id != item._id).ToList();
                            //var duplicateDetails = duplicateItems.Where(i => i.fulfillments.Any(f => f.purchaseOrder.purchaseRequest._id == detail.purchaseOrder.purchaseRequest._id && f.product._id == detail.product._id)).ToList();
                            //if (duplicateDetails.Count > 0)
                            //{
                            //    detailErrorCount++;
                            //    detailError += "product: 'Data sudah ada', ";
                            //}

                            if (detail.deliveredQuantity == 0)
                            {
                                detailErrorCount++;
                                detailError += "deliveredQuantity: 'DeliveredQuantity can not 0', ";
                            }

                            detailError += "}, ";
                        }

                        detailError += "]";

                        if(detailErrorCount > 0)
                        {
                            itemErrorCount++;
                            itemError += $"fulfillments: {detailError}, ";
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
