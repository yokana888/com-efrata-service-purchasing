using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel
{
    public class GarmentPurchaseRequestViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string PRNo { get; set; }
        public string PRType { get; set; }
        public string RONo { get; set; }
        public string MDStaff { get; set; }

        public long SCId { get; set; }
        public string SCNo { get; set; }

        public string SectionName { get; set; }
        public string ApprovalPR { get; set; }

        public BuyerViewModel Buyer { get; set; }

        public string Article { get; set; }

        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset? ShipmentDate { get; set; }

        public UnitViewModel Unit { get; set; }

        public bool IsPosted { get; set; }
        public bool IsUsed { get; set; }
        public string Remark { get; set; }

        public bool IsValidated { get; set; }
        public string ValidatedBy { get; set; }
        public DateTimeOffset ValidatedDate { get; set; }

        public bool IsValidatedMD1 { get; set; }
        public string ValidatedMD1By { get; set; }
        public DateTimeOffset ValidatedMD1Date { get; set; }

        public bool IsValidatedMD2 { get; set; }
        public string ValidatedMD2By { get; set; }
        public DateTimeOffset ValidatedMD2Date { get; set; }

        public bool IsValidatedPurchasing { get; set; }
        public string ValidatedPurchasingBy { get; set; }
        public DateTimeOffset ValidatedPurchasingDate { get; set; }

        public List<GarmentPurchaseRequestItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            PurchasingDbContext dbContext = validationContext == null ? null : (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));

            if (Buyer == null) {
                yield return new ValidationResult("Buyer tidak boleh kosong", new List<string> { "Buyer" });
            }
            else if (String.IsNullOrWhiteSpace(Buyer.Id) || Buyer.Id.Equals("0") || String.IsNullOrWhiteSpace(Buyer.Code) || String.IsNullOrWhiteSpace(Buyer.Name))
            {
                yield return new ValidationResult("Data Buyer tidak benar", new List<string> { "Buyer" });
            }

            if (String.IsNullOrWhiteSpace(Article))
            {
                yield return new ValidationResult("Artikel tidak boleh kosong", new List<string> { "Article" });
            }

            if (Date.Equals(DateTimeOffset.MinValue) || Date == null)
            {
                yield return new ValidationResult("Tanggal tidak boleh kosong", new List<string> { "Date" });
            }

            if (SCId < 1 || string.IsNullOrWhiteSpace(SCNo))
            {
                yield return new ValidationResult("Sales Contract tidak boleh kosong", new List<string> { "SalesContract" });
            }

            if (new string[] { "MASTER", "SAMPLE" }.Contains(PRType))
            {
                if (ShipmentDate.Equals(DateTimeOffset.MinValue) || ShipmentDate == null)
                {
                    yield return new ValidationResult("Tanggal Shipment tidak boleh kosong", new List<string> { "ShipmentDate" });
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(RONo))
                {
                    yield return new ValidationResult("RONo tidak boleh kosong", new List<string> { "RONo" });
                }
                else
                {
                    var duplicateRONo = dbContext.GarmentPurchaseRequests.Where(m => m.RONo.Equals(RONo) && m.Id != Id).Count();
                    if (duplicateRONo > 0)
                    {
                        yield return new ValidationResult("RONo sudah ada", new List<string> { "RONo" });
                    }
                }

                if (ShipmentDate.Equals(DateTimeOffset.MinValue) || ShipmentDate == null)
                {
                    yield return new ValidationResult("Tanggal Shipment tidak boleh kosong", new List<string> { "ShipmentDate" });
                }
            }

            //if (PRType != "MASTER")
            //{
                if (Unit == null)
                {
                    yield return new ValidationResult("Unit tidak boleh kosong", new List<string> { "Unit" });
                }
                else if (String.IsNullOrWhiteSpace(Unit.Id) || Unit.Id.Equals("0") || String.IsNullOrWhiteSpace(Unit.Code) || String.IsNullOrWhiteSpace(Unit.Name))
                {
                    yield return new ValidationResult("Data Unit tidak benar", new List<string> { "Unit" });
                }
            //}

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
                        itemError += "Product: 'Barang tidak boleh kosong', ";
                    }
                    else if (String.IsNullOrWhiteSpace(item.Product.Id) || item.Product.Id.Equals("0") || String.IsNullOrWhiteSpace(item.Product.Code) || String.IsNullOrWhiteSpace(item.Product.Name))
                    {
                        itemErrorCount++;
                        itemError += "Product: 'Data Barang tidak benar', ";
                    }

                    if(item.Quantity <= 0)
                    {
                        itemErrorCount++;
                        itemError += "Quantity: 'Jumlah harus lebih dari 0', ";
                    }

                    if (item.BudgetPrice < 0)
                    {
                        itemErrorCount++;
                        itemError += "BudgetPrice: 'Price harus lebih dari atau sama dengan 0', ";
                    }

                    if (item.Uom == null)
                    {
                        itemErrorCount++;
                        itemError += "UOM: 'Satuan tidak boleh kosong', ";
                    }
                    else if (String.IsNullOrWhiteSpace(item.Uom.Id) || item.Uom.Id.Equals("0") || String.IsNullOrWhiteSpace(item.Uom.Unit))
                    {
                        itemErrorCount++;
                        itemError += "UOM: 'Data Satuan tidak benar', ";
                    }

                    if (item.Category == null)
                    {
                        itemErrorCount++;
                        itemError += "Category: 'Kategori tidak boleh kosong', ";
                    }
                    else if (String.IsNullOrWhiteSpace(item.Category.Id) || item.Category.Id.Equals("0") || String.IsNullOrWhiteSpace(item.Category.Name))
                    {
                        itemErrorCount++;
                        itemError += "Category: 'Data Kategori tidak benar', ";
                    }

                    if (string.IsNullOrWhiteSpace(item.ProductRemark))
                    {
                        itemErrorCount++;
                        itemError += "ProductRemark: 'Design/Color/Keterangan tidak boleh kosong', ";
                    }

                    if (new string[] { "MASTER", "SAMPLE" }.Contains(PRType))
                    {
                        if (item.Category != null && item.Category.Name == "FABRIC")
                        {
                            if (item.Composition == null || string.IsNullOrWhiteSpace(item.Composition.Composition))
                            {
                                itemErrorCount++;
                                itemError += "Composition: 'Komposisi tidak boleh kosong', ";
                            }
                            else
                            {
                                if (item.Const == null || string.IsNullOrWhiteSpace(item.Const.Const))
                                {
                                    itemErrorCount++;
                                    itemError += "Const: 'Konstruksi tidak boleh kosong', ";
                                }
                                else
                                {
                                    if (item.Yarn == null || string.IsNullOrWhiteSpace(item.Yarn.Yarn))
                                    {
                                        itemErrorCount++;
                                        itemError += "Yarn: 'Yarn tidak boleh kosong', ";
                                    }
                                    else
                                    {
                                        if (item.Width == null || string.IsNullOrWhiteSpace(item.Width.Width))
                                        {
                                            itemErrorCount++;
                                            itemError += "Width: 'Width tidak boleh kosong', ";
                                        }
                                    }
                                }
                            }
                        }

                        if (item.PriceUom == null)
                        {
                            itemErrorCount++;
                            itemError += "PriceUom: 'Satuan Harga tidak boleh kosong', ";
                        }
                        else if (string.IsNullOrWhiteSpace(item.PriceUom.Id) || item.PriceUom.Id.Equals("0") || string.IsNullOrWhiteSpace(item.PriceUom.Unit))
                        {
                            itemErrorCount++;
                            itemError += "PriceUom: 'Data Satuan Harga tidak benar', ";
                        }

                        if (item.PriceConversion <= 0)
                        {
                            itemErrorCount++;
                            itemError += "PriceConversion: 'Konversi harus lebih dari 0', ";
                        }
                        else if (item.Uom != null && item.PriceUom != null && (item.PriceUom.Id == item.Uom.Id || item.PriceUom.Unit == item.Uom.Unit) && item.PriceConversion != 1)
                        {
                            itemErrorCount++;
                            itemError += "PriceConversion: 'Satuan Sama, Konversi harus 1', ";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(item.PO_SerialNumber))
                        {
                            itemErrorCount++;
                            itemError += "PO_SerialNumber: 'PO SerialNumber tidak boleh kosong', ";
                        }
                        else if (Id != 0)
                        {
                            var duplicatePO_SerialNumber = dbContext.GarmentPurchaseRequests
                                .SingleOrDefault(m => m.Id == Id && m.Items.Any(i => i.PO_SerialNumber.Equals(item.PO_SerialNumber) && i.Id != item.Id));
                            if (duplicatePO_SerialNumber != null)
                            {
                                itemErrorCount++;
                                itemError += "PO_SerialNumber: 'PO SerialNumber sudah ada', ";
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
