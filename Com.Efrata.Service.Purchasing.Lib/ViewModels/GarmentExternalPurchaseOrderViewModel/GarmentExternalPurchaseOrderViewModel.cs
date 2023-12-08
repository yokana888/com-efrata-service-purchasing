using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel
{
    public class GarmentExternalPurchaseOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string EPONo { get; set; }
        public SupplierViewModel Supplier { get; set; }

        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public string FreightCostBy { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public int PaymentDueDays { get; set; }
        public CurrencyViewModel Currency { get; set; }

        public bool IsIncomeTax { get; set; }
        public IncomeTaxViewModel IncomeTax { get; set; }

        public bool IsUseVat { get; set; }
        public VatViewModel Vat { get; set; }
        public string Category { get; set; }
        public string Remark { get; set; }
        public bool IsPosted { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsApproved { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsClosed { get; set; }
        public bool IsUnpost { get; set; }

        //StandardQuality
        public string QualityStandardType { get; set; }
        public string Shrinkage { get; set; }
        public string WetRubbing { get; set; }
        public string DryRubbing { get; set; }
        public string Washing { get; set; }
        public string DarkPerspiration { get; set; }
        public string LightMedPerspiration { get; set; }
        public string PieceLength { get; set; }

        public long? UENId { get; set; }

        public double BudgetRate { get; set; }

        public bool IsPayVAT { get; set; }
        public bool IsPayIncomeTax { get; set; }

        public List<GarmentExternalPurchaseOrderItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Supplier==null || Supplier.Id == 0)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "Supplier" });
            }

            if (this.Currency == null)
            {
                yield return new ValidationResult("Currency is required", new List<string> { "Currency" });
            }

            if (this.IsUseVat == true && this.Vat.Id == 0)
            {
                yield return new ValidationResult("Vat is required", new List<string> { "Vat" });
            }
            
            if (this.DeliveryDate.Equals(DateTimeOffset.MinValue) || this.DeliveryDate == null)
            {
                yield return new ValidationResult("DeliveryDate is required", new List<string> { "DeliveryDate" });
            }

            else if (this.OrderDate != null && this.OrderDate > this.DeliveryDate)
            {
                yield return new ValidationResult("OrderDate is greater than delivery date", new List<string> { "DeliveryDate" });
            }

            if (Category == "FABRIC")
            {
                if (string.IsNullOrWhiteSpace(DryRubbing))
                {
                    yield return new ValidationResult("DryRubbing is required", new List<string> { "DryRubbing" });
                }
                if (string.IsNullOrWhiteSpace(Washing))
                {
                    yield return new ValidationResult("Washing is required", new List<string> { "Washing" });
                }
                if (string.IsNullOrWhiteSpace(WetRubbing))
                {
                    yield return new ValidationResult("WetRubbing is required", new List<string> { "WetRubbing" });
                }
                if (string.IsNullOrWhiteSpace(DarkPerspiration))
                {
                    yield return new ValidationResult("DarkPerspiration is required", new List<string> { "DarkPerspiration" });
                }
                if (string.IsNullOrWhiteSpace(LightMedPerspiration))
                {
                    yield return new ValidationResult("LightMedPerspiration is required", new List<string> { "LightMedPerspiration" });
                }
                if (string.IsNullOrWhiteSpace(PieceLength))
                {
                    yield return new ValidationResult("PieceLength is required", new List<string> { "PieceLength" });
                }
                if (string.IsNullOrWhiteSpace(QualityStandardType))
                {
                    yield return new ValidationResult("QualityStandardType is required", new List<string> { "QualityStandardType" });
                }
                if (string.IsNullOrWhiteSpace(Shrinkage))
                {
                    yield return new ValidationResult("Shrinkage is required", new List<string> { "Shrinkage" });
                }
            }

            if (Items == null || Items.Count <= 0)
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


                    if (item.Product == null)
                    {
                        itemErrorCount++;
                        itemError += "Product: 'Product tidak boleh kosong', ";
                    }
                    else
                    {
                        if (item.Product.Name == "PROCESS")
                        {
                            if (String.IsNullOrEmpty(Remark))
                            {
                                yield return new ValidationResult("Keterangan Harus Diisi", new List<string> { "Remark" });
                            }
                        }
                    }

                    if (item.DealQuantity <= 0)
                    {
                        itemErrorCount++;
                        itemError += "DealQuantity: 'Quantity harus lebih dari 0', ";
                    }

                    if (item.PricePerDealUnit <= 0)
                    {
                        itemErrorCount++;
                        itemError += "PricePerDealUnit: 'Harga harus lebih dari 0', ";
                    }

                    if (item.DealUom == null)
                    {
                        itemErrorCount++;
                        itemError += "DealUom: 'Satuan tidak boleh kosong', ";
                    }
                    else if (String.IsNullOrWhiteSpace(item.DealUom.Id) || item.DealUom.Id.Equals("0") || String.IsNullOrWhiteSpace(item.DealUom.Unit))
                    {
                        itemErrorCount++;
                        itemError += "DealUom: 'Data Satuan tidak benar', ";
                    }
                    if (item.SmallUom == null)
                    {
                        itemErrorCount++;
                        itemError += "SmallUom: 'Satuan Kacil tidak boleh kosong', ";
                    }
                    else if (String.IsNullOrWhiteSpace(item.SmallUom.Id) || item.SmallUom.Id.Equals("0") || String.IsNullOrWhiteSpace(item.SmallUom.Unit))
                    {
                        itemErrorCount++;
                        itemError += "SmallUom: 'Data Satuan Kecil tidak benar', ";
                    }

                    if (string.IsNullOrWhiteSpace(item.Remark))
                    {
                        itemErrorCount++;
                        itemError += "Remark: 'Design/Color/Keterangan tidak boleh kosong', ";
                    }

                    if (item.IsOverBudget && !((PaymentMethod == "CMT" || PaymentMethod == "FREE FROM BUYER") && (PaymentType == "FREE" || PaymentType == "EX MASTER FREE")))
                    {
                        if (string.IsNullOrWhiteSpace(item.OverBudgetRemark))
                        {
                            itemErrorCount++;
                            itemError += "OverBudgetRemark: 'Keterangan OverBudget Harus Diisi', ";
                        }
                    }
                    if(item.DealUom!=null && item.SmallUom != null)
                    {
                        if (item.DealUom.Unit == item.SmallUom.Unit)
                        {
                            if (item.Conversion != 1)
                            {
                                itemErrorCount++;
                                itemError += "Conversion: 'Konversi harus 1', ";
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
