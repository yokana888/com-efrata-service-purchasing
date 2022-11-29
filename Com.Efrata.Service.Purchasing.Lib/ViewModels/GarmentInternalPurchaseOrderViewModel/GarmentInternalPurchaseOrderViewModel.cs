using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel
{
    public class GarmentInternalPurchaseOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string PONo { get; set; }
        public long PRId { get; set; }
        public DateTimeOffset? PRDate { get; set; }
        public string PRNo { get; set; }
        public string RONo { get; set; }

        public BuyerViewModel Buyer { get; set; }

        public string Article { get; set; }

        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset? ShipmentDate { get; set; }

        public UnitViewModel Unit { get; set; }

        public bool IsPosted { get; set; }
        public bool IsClosed { get; set; }
        public bool HasDuplicate { get; set; } // PR sama lebih dari 1
        public string Remark { get; set; }

        public List<GarmentInternalPurchaseOrderItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
            else
            {
                string itemError = "[";
                int itemErrorCount = 0;

                foreach (var item in Items)
                {
                    itemError += "{";

                    if (item.Quantity <= 0)
                    {
                        itemErrorCount++;
                        itemError += "Quantity: 'Jumlah harus lebih dari 0', ";
                    }
                    else if (Id != 0)
                    {
                        PurchasingDbContext dbContext = validationContext == null ? null : (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
                        var oldItem = dbContext.GarmentInternalPurchaseOrderItems.SingleOrDefault(i => i.Id == item.Id);
                        if (oldItem != null)
                        {
                            if (item.Quantity >= oldItem.Quantity)
                            {
                                itemErrorCount++;
                                itemError += $"Quantity: 'Jumlah harus kurang dari jumlah sebelumnya ({oldItem.Quantity})', ";
                            }
                        }
                        else
                        {
                            itemErrorCount++;
                            itemError += "Quantity: 'Jumlah sebelumnya tidak diketahui', ";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(item.ProductRemark))
                    {
                        itemErrorCount++;
                        itemError += "ProductRemark: 'Design/Color/Keterangan tidak boleh kosong', ";
                    }


                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "Items" });
            }
        }
    }
}
