using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class PurchasingToVerificationViewModel : IValidatableObject
    {
        /* public DateTimeOffset? SubmissionDate { get; set; } */
        public List<UnitPaymentOrderViewModel> UnitPaymentOrders { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            /*
                if (this.SubmissionDate == null)
                {
                    yield return new ValidationResult("Submission Date is required", new List<string> { "SubmissionDate" });
                }
                else if (this.SubmissionDate > DateTimeOffset.UtcNow)
                {
                    yield return new ValidationResult("Submission Date must be lower or equal than today's date", new List<string> { "SubmissionDate" });
                }
            */

            if (this.UnitPaymentOrders.Count.Equals(0))
            {
                yield return new ValidationResult("Unit Payment Orders is required", new List<string> { "UnitPaymentOrdersCollection" });
            }
            else
            {
                int Count = 0;
                string error = "[";

                foreach (UnitPaymentOrderViewModel unitPaymentOrder in UnitPaymentOrders)
                {
                    if (string.IsNullOrWhiteSpace(unitPaymentOrder.No))
                    {
                        Count++;
                        error += "{ UnitPaymentOrder: 'Unit Payment Order is required' }, ";
                    }
                    else if (UnitPaymentOrders.Count(prop => prop.No == unitPaymentOrder.No) > 1)
                    {
                        Count++;
                        error += "{ UnitPaymentOrder: 'Unit Payment Order must be unique' }, ";
                    }
                    else
                    {
                        error += "{},";
                    }
                }

                error += "]";

                if (Count > 0)
                {
                    yield return new ValidationResult(error, new List<string> { "UnitPaymentOrders" });
                }
            }
        }

        public object ToModel()
        {
            List<PurchasingDocumentExpedition> list = new List<PurchasingDocumentExpedition>();

            foreach (UnitPaymentOrderViewModel unitPaymentOrder in this.UnitPaymentOrders)
            {
                List<PurchasingDocumentExpeditionItem> Items = new List<PurchasingDocumentExpeditionItem>();

                foreach (UnitPaymentOrderItemViewModel item in unitPaymentOrder.Items)
                {
                    Items.Add(new PurchasingDocumentExpeditionItem()
                    {
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Uom = item.Uom,
                        Price = item.Price,
                        UnitId = item.UnitId,
                        UnitCode = item.UnitCode,
                        UnitName = item.UnitName,
                        URNId = item.URNId.GetValueOrDefault(),
                        URNNo = item.URNNo
                    });
                }

                list.Add(new PurchasingDocumentExpedition()
                {
                    SendToVerificationDivisionDate = DateTimeOffset.UtcNow,
                    UnitPaymentOrderNo = unitPaymentOrder.No,
                    UPODate = unitPaymentOrder.UPODate,
                    InvoiceNo = unitPaymentOrder.InvoiceNo,
                    PaymentMethod = unitPaymentOrder.PaymentMethod,
                    DueDate = unitPaymentOrder.DueDate,
                    SupplierCode = unitPaymentOrder.SupplierCode,
                    SupplierName = unitPaymentOrder.SupplierName,
                    CategoryCode = unitPaymentOrder.CategoryCode,
                    CategoryName = unitPaymentOrder.CategoryName,
                    DivisionCode = unitPaymentOrder.DivisionCode,
                    DivisionName = unitPaymentOrder.DivisionName,
                    IncomeTax = unitPaymentOrder.IncomeTax,
                    Vat = unitPaymentOrder.Vat,
                    IncomeTaxId = unitPaymentOrder.IncomeTaxId,
                    IncomeTaxName = unitPaymentOrder.IncomeTaxName,
                    IncomeTaxRate = unitPaymentOrder.IncomeTaxRate,
                    IncomeTaxBy = unitPaymentOrder.IncomeTaxBy,
                    TotalPaid = unitPaymentOrder.TotalPaid,
                    Currency = unitPaymentOrder.Currency,
                    Items = Items,
                    URNId = unitPaymentOrder.URNId,
                    URNNo = unitPaymentOrder.URNNo
                });
            }

            return list;
        }
    }
}
