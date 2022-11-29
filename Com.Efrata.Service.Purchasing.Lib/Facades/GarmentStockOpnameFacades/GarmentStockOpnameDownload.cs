using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades
{
    public class GarmentStockOpnameDownload : IValidatableObject
    {
        public DateTimeOffset? date { get; set; }
        public string unit { get; set; }
        public string storage { get; set; }
        public string storageName { get; set; }

        public GarmentStockOpnameDownload(DateTimeOffset? date, string unit, string storage, string storageName)
        {
            this.date = date;
            this.unit = unit;
            this.storage = storage;
            this.storageName = storageName;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (date == null || date == DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal harus diisi", new List<string> { "date" });
            }
            else if (!string.IsNullOrWhiteSpace(unit) && !string.IsNullOrWhiteSpace(storage))
            {
                IGarmentStockOpnameFacade _facade = validationContext.GetService<IGarmentStockOpnameFacade>();
                var lastData = _facade.GetLastDataByUnitStorage(unit, storage);
                if (lastData != null)
                {
                    if (date <= lastData.Date)
                    {
                        IdentityService _identityService = validationContext.GetService<IdentityService>(); 
                        yield return new ValidationResult("Tanggal harus lebih dari " + lastData.Date.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("id-ID")), new List<string> { "date" });
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                yield return new ValidationResult("Unit harus diisi", new List<string> { "unit" });
            }

            if (string.IsNullOrWhiteSpace(storage))
            {
                yield return new ValidationResult("Storage harus diisi", new List<string> { "storage" });
            }
        }
    }
}
