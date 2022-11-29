using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentClosingDateViewModels
{
    public class GarmentClosingDateViewModel : BaseViewModel, IValidatableObject
    {
        public DateTimeOffset CloseDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CloseDate.Equals(DateTimeOffset.MinValue) || CloseDate == null)
            {
                yield return new ValidationResult("CloseDate harus diisi", new List<string> { "CloseDate" });
            }
        }
    }
}
