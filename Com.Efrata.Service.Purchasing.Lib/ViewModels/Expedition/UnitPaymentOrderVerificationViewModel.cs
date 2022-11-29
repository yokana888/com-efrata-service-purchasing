using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class UnitPaymentOrderVerificationViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public DateTimeOffset? VerifyDate { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public string Reason { get; set; }
        public ExpeditionPosition SubmitPosition { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (this.VerifyDate == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "VerifyDate" });
            }
            else if (this.VerifyDate.GetValueOrDefault().Date > DateTimeOffset.UtcNow.Date)
            {
                yield return new ValidationResult("Date must be lower or equal than today's date", new List<string> { "VerifyDate" });
            }

            if (string.IsNullOrWhiteSpace(this.UnitPaymentOrderNo))
                yield return new ValidationResult("Unit Payment Order No is required", new List<string> { "UnitPaymentOrderNo" });

            if (this.SubmitPosition.Equals(ExpeditionPosition.INVALID))
                yield return new ValidationResult("Submit Position is required", new List<string> { "SubmitPosition" });

        }

        public PurchasingDocumentExpedition ToModel()
        {

            PurchasingDocumentExpedition data = new PurchasingDocumentExpedition
            {
                Id = this.Id,
                VerifyDate = this.VerifyDate,
                UnitPaymentOrderNo = this.UnitPaymentOrderNo,
                Position = this.SubmitPosition,
                NotVerifiedReason = this.Reason,
            };

            return data;
        }
    }
}
