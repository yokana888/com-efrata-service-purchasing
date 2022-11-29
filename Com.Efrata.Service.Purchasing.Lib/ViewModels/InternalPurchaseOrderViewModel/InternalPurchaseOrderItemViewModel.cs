using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class InternalPurchaseOrderItemViewModel : BaseViewModel//, IValidatableObject
    {
        public string prItemId { get; set; }
        public ProductViewModel product { get; set; }
        public double quantity { get; set; }
        public string productRemark { get; set; }
        public string status { get; set; }
        public long poId { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
