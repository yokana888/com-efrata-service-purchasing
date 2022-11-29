using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Moonlay.NetCore.Lib.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Services
{
    public class ValidateService : IValidateService
    {
        private readonly IServiceProvider serviceProvider;

        public ValidateService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Validate(dynamic viewModel)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider, null);

            if (!Validator.TryValidateObject(viewModel, validationContext, validationResults, true))
                throw new ServiceValidationExeption(validationContext, validationResults);
        }
    }
}
