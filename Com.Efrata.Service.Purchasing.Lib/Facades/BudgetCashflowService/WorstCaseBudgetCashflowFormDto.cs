using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class WorstCaseBudgetCashflowFormDto : IValidatableObject
    {
        public DateTimeOffset DueDate { get; set; }
        public int UnitId { get; set; }
        public ICollection<WorstCaseBudgetCashflowItemFormDto> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitId <= 0)
            {
                yield return new ValidationResult("Unit harus diisi", new List<string> { "Unit" });
            }

            if (DueDate == null)
            {
                yield return new ValidationResult("Tanggal harus diisi", new List<string> { "Date" });
            }

            var itemErrorCount = 0;
            if (Items == null || Items.Count <= 0)
            {
                yield return new ValidationResult("Data Worst Case harus diisi", new List<string> { "Item" });
            }
            else
            {
                string itemError = "[";

                foreach (var item in Items)
                {
                    itemError += "{";

                    //if (item.Currency == null || item.Currency.Id <= 0)
                    //{
                    //    itemErrorCount++;
                    //    itemError += "Currency: 'Mata Uang harus diisi', ";
                    //}
                    //else
                    //{
                    //    if (item.Currency.Code != "IDR" && item.CurrencyNominal <= 0)
                    //    {
                    //        itemErrorCount++;
                    //        itemError += "CurrencyNominal: 'Nominal Valas harus lebih dari 0', ";
                    //    }
                    //}

                    //if (item.Nominal <= 0)
                    //{
                    //    itemErrorCount++;
                    //    itemError += "Nominal: 'Nominal harus lebih dari 0', ";
                    //}

                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "Items" });
            }
        }
    }
}