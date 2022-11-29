using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Microsoft.EntityFrameworkCore;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel
{
    public class UnitPaymentCorrectionNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string uPCNo { get; set; }
        public DateTimeOffset correctionDate { get; set; }
        public string correctionType { get; set; }
        public long uPOId { get; set; }
        public string uPONo { get; set; }
        public SupplierViewModel supplier { get; set; }
        public DivisionViewModel division { get; set; }
        public CategoryViewModel category { get; set; }
        public string invoiceCorrectionNo { get; set; }
        public DateTimeOffset? invoiceCorrectionDate { get; set; }
        public bool useVat { get; set; }
        public string vatTaxCorrectionNo { get; set; }
        public DateTimeOffset? vatTaxCorrectionDate { get; set; }
        public bool useIncomeTax { get; set; }
        public string incomeTaxCorrectionNo { get; set; }
        public DateTimeOffset? incomeTaxCorrectionDate { get; set; }
        public string releaseOrderNoteNo { get; set; }
        public DateTimeOffset? dueDate { get; set; }
        public string remark { get; set; }
        public string returNoteNo { get; set; }
        public List<UnitPaymentCorrectionNoteItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //PurchasingDbContext dbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
            if (this.uPONo == null)
            {
                yield return new ValidationResult("No. Surat Perintah Bayar is required", new List<string> { "uPOId" });
            }
            //if (this.correctionDate.Equals(DateTimeOffset.MinValue) || this.correctionDate == null)
            //{
            //    yield return new ValidationResult("Date is required", new List<string> { "correctionDate" });
            //}
            if ((correctionType==null || correctionType=="Jumlah") && this.releaseOrderNoteNo == null)
            {
                yield return new ValidationResult("No. Bon Keluar is required", new List<string> { "releaseOrderNoteNo" });
            }
            if (supplier == null)
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

                foreach (UnitPaymentCorrectionNoteItemViewModel item in items)
                {
                    itemError += "{";

                    if (item.product == null || string.IsNullOrWhiteSpace(item.product._id))
                    {
                        itemErrorCount++;
                        itemError += "product: 'Product is required', ";
                    }
                    if (correctionType != "Jumlah")
                    {
                        if (item.pricePerDealUnitAfter < 0)
                        {
                            itemErrorCount++;
                            itemError += "pricePerDealUnitAfter: 'Price should not be less than 0'";
                        }
                        if (item.priceTotalAfter < 0)
                        {
                            itemErrorCount++;
                            itemError += "priceTotalAfter: 'Price Total should not be less than 0'";
                        }
                    }
                    else
                    {
                        if (item.quantity > item.quantityCheck)
                        {
                            itemErrorCount++;
                            itemError += "quantity: 'Quantity should not be more than UnitPaymentOrderQuantity'";
                        }
                        if (item.quantity <= 0)
                        {
                            itemErrorCount++;
                            itemError += "quantity: 'quantity should not be less than or equal 0'";
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
