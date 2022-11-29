using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel
{
    public class UnitPaymentOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string no { get; set; }
        public DivisionViewModel division { get; set; }
        public SupplierViewModel supplier { get; set; }
        public DateTimeOffset? date { get; set; }
        public CategoryViewModel category { get; set; }
        public CurrencyViewModel currency { get; set; }
        public string paymentMethod { get; set; }
        public string invoiceNo { get; set; }
        public DateTimeOffset? invoiceDate { get; set; }
        public string pibNo { get; set; }
        public DateTimeOffset? pibDate { get; set; }
        public double? importDuty { get; set; }
        public double? totalIncomeTaxAmount { get; set; }
        public double? totalVatAmount { get; set; }

        public bool useIncomeTax { get; set; }
        public IncomeTaxViewModel incomeTax { get; set; }
        public string incomeTaxNo { get; set; }
        public DateTimeOffset? incomeTaxDate { get; set; }
        public string incomeTaxBy { get; set; }

        public VatTaxViewModel vatTax { get; set; }

        public bool useVat { get; set; }
        public string vatNo { get; set; }
        public DateTimeOffset? vatDate { get; set; }

        public string remark { get; set; }
        public string importInfo { get; set; }
        public DateTimeOffset dueDate { get; set; }
        public bool IsCorrection { get; set; }
        public bool isPaid { get; set; }

        public int position { get; set; }

        public bool isPosted { get; set; }

        public List<UnitPaymentOrderItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (date.Equals(DateTimeOffset.MinValue) || date == null)
            //{
            //    yield return new ValidationResult("Date is required", new List<string> { "date" });
            //}
            if (division == null)
            {
                yield return new ValidationResult("Division is required", new List<string> { "division" });
            }
            if (supplier == null)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }
            if (category == null)
            {
                yield return new ValidationResult("Category is required", new List<string> { "category" });
            }
            if (currency == null)
            {
                yield return new ValidationResult("Currency is required", new List<string> { "currency" });
            }
            if (string.IsNullOrWhiteSpace(invoiceNo))
            {
                yield return new ValidationResult("InvoiceNo is required", new List<string> { "invoiceNo" });
            }
            if (date.Equals(DateTimeOffset.MinValue) || invoiceDate == null)
            {
                yield return new ValidationResult("InvoiceDate is required", new List<string> { "invoiceDate" });
            }

            if (useIncomeTax)
            {
                if (incomeTax == null || string.IsNullOrWhiteSpace(incomeTax._id))
                {
                    yield return new ValidationResult("IncomeTax is required", new List<string> { "incomeTax" });
                }
                if (string.IsNullOrWhiteSpace(incomeTaxNo))
                {
                    yield return new ValidationResult("IncomeTaxNo is required", new List<string> { "incomeTaxNo" });
                }
                if (incomeTaxDate == null || incomeTaxDate.Equals(DateTimeOffset.MinValue))
                {
                    yield return new ValidationResult("IncomeTaxDate is required", new List<string> { "incomeTaxDate" });
                }
                if (string.IsNullOrWhiteSpace(incomeTaxBy))
                {
                    yield return new ValidationResult("IncomeTaxBy is required", new List<string> { "incomeTaxBy" });
                }
            }

            if (useVat)
            {
                if (string.IsNullOrWhiteSpace(vatNo))
                {
                    yield return new ValidationResult("VatNo is required", new List<string> { "vatNo" });
                }
                if (vatDate == null || vatDate.Equals(DateTimeOffset.MinValue))
                {
                    yield return new ValidationResult("VatDate is required", new List<string> { "vatDate" });
                }
            }

            if (items == null || items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";
                int itemErrorCount = 0;

                foreach (var item in items)
                {
                    itemError += "{";
                    if (item.unitReceiptNote == null || item.unitReceiptNote._id == 0)
                    {
                        itemErrorCount++;
                        itemError += "unitReceiptNote: 'No UnitReceiptNote selected', ";
                    }
                    else if(items.Where(i => i.unitReceiptNote != null && item.unitReceiptNote != null && i.unitReceiptNote._id == item.unitReceiptNote._id).Count() > 1)
                    {
                        itemErrorCount++;
                        itemError += "unitReceiptNote: 'UnitReceiptNote is already used', ";
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
