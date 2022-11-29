using Com.Efrata.Service.Purchasing.Lib.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class PurchasingDocumentAcceptanceViewModel : IValidatableObject
    {
        /* public DateTimeOffset? ReceiptDate { get; set; } */
        public string Role { get; set; }
        public List<PurchasingDocumentAcceptanceItem> PurchasingDocumentExpedition { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            /*
                if (this.ReceiptDate == null)
                {
                    yield return new ValidationResult("Receipt Date is required", new List<string> { "ReceiptDate" });
                }
                else if (this.ReceiptDate > DateTimeOffset.UtcNow)
                {
                    yield return new ValidationResult("Receipt Date must be lower or equal than today's date", new List<string> { "ReceiptDate" });
                }
            */

            if (this.PurchasingDocumentExpedition.Count.Equals(0))
            {
                yield return new ValidationResult("Purchasing Document Expeditions is required", new List<string> { "PurchaseDocumentExpeditionCollection" });
            }
        }
    }

    public class PurchasingDocumentAcceptanceItem
    {
        public int Id { get; set; }
        public string UnitPaymentOrderNo { get; set; }
    }
}
