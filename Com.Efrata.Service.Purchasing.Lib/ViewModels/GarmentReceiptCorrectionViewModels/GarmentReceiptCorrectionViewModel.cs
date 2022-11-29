using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionViewModels
{
    public class GarmentReceiptCorrectionViewModel : BaseViewModel, IValidatableObject
    {
        public string CorrectionType { get; set; }
        public string CorrectionNo { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public long URNId { get; set; }
        public string URNNo { get; set; }
        public UnitViewModel Unit { get; set; }
        public IntegrationViewModel.StorageViewModel Storage { get; set; }
        public string Remark { get; set; }
        public List<GarmentReceiptCorrectionItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(URNNo))
            {
                yield return new ValidationResult("Nomor Bon tidak boleh kosong", new List<string> { "URNNo" });
            }
            if (Items == null || Items.Count(i => i.IsSave) <= 0)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
            else
            {
                string itemError = "[";
                int itemErrorCount = 0;

                foreach (var item in Items)
                {
                    itemError += "{";

                    if (item.IsSave)
                    {
                        if (CorrectionType.ToUpper() == "JUMLAH")
                        {
                            if (item.CorrectionQuantity == 0)
                            {
                                itemErrorCount++;
                                itemError += "CorrectionQuantity: 'Jumlah Koreksi tidak boleh sama dengan 0', ";
                            }
                            else if (item.CorrectionQuantity + item.QuantityCheck < 0)
                            {
                                itemErrorCount++;
                                itemError += $"CorrectionQuantity: 'Jumlah Koreksi tidak boleh < -{item.QuantityCheck}', ";
                            }
                        }
                        else if (CorrectionType.ToUpper() == "KONVERSI")
                        {
                            double qty = item.Quantity * item.CorrectionConversion;
                            if (item.CorrectionConversion <= 0)
                            {
                                itemErrorCount++;
                                itemError += "CorrectionConversion: 'Konversi tidak boleh kurang dari 0', ";
                            }
                            else if (qty < item.OrderQuantity)
                            {
                                itemErrorCount++;
                                itemError += "CorrectionConversion: 'Konversi tidak Sesuai. Stok minus', ";
                            }
                        }
                    }
                    

                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "Items" });
            }
        }
    }
}
