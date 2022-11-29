using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentSupplierBalanceDebtViewModel
{
    public class GarmentSupplierBalanceDebtViewModel : BaseViewModel, IValidatableObject
    {
        public SupplierViewModel supplier { get; set; }
        public double? totalValas { get; set; }
        public double? totalAmountIDR { get; set; }
        public CurrencyViewModel currency { get; set; }
        public long? Year { get; set; }
        public string codeRequirment { get; set; }
        public double dOCurrencyRate { get; set; }
        public List<GarmentSupplierBalanceDebtItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (dOCurrencyRate == 0)
            {
                yield return new ValidationResult("Kurs is required", new List<string> { "CurrencyRate" });
            }
            if (supplier == null)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }
            if (currency == null)
            {
                yield return new ValidationResult("Currency is required", new List<string> { "currency" });
            }
            if (this.items == null || items.Count <= 0)
            {
                yield return new ValidationResult("BP is required", new List<string> { "itemscount" });
            }
        }
    }
}
