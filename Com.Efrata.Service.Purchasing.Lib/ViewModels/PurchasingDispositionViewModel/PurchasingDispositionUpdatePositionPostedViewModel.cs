using Com.Efrata.Service.Purchasing.Lib.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel
{
    public class PurchasingDispositionUpdatePositionPostedViewModel : IValidatableObject
    {
        public PurchasingDispositionUpdatePositionPostedViewModel()
        {
            PurchasingDispositionNoes = new List<string>();
        }

        public List<string> PurchasingDispositionNoes { get; set; }

        public ExpeditionPosition Position { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PurchasingDispositionNoes.Count == 0)
            {
                yield return new ValidationResult("Purchasing Disposition No tidak boleh kosong", new List<string> { "PurchasingDispositionNoes" });
            }
        }
    }
}
