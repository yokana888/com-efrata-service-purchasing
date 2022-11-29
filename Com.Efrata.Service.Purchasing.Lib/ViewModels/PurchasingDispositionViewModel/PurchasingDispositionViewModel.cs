using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel
{
    public class PurchasingDispositionViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string DispositionNo { get; set; }
        public SupplierViewModel Supplier { get; set; }
        public CurrencyViewModel Currency { get; set; }
        public CategoryViewModel Category { get; set; }
        public DivisionViewModel Division { get; set; }
        public string Bank { get; set; }
        public string ConfirmationOrderNo { get; set; }
        public string IncomeTaxBy { get; set; }
        public string PaymentMethod { get; set; }
        public DateTimeOffset PaymentDueDate { get; set; }
        public string Calculation { get; set; }
        public string Remark { get; set; }
        public string ProformaNo { get; set; }
        public double Amount { get; set; }
        public int Position { get; set; }
        public double PaymentCorrection { get; set; }
        public double IncomeTaxValue { get; set; }
        public double VatValue { get; set; }
        public double DPP { get; set; }
        public UnitViewModel Unit { get; set; }
        public virtual List<PurchasingDispositionItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.PaymentDueDate.Equals(DateTimeOffset.MinValue) || this.PaymentDueDate == null)
            {
                yield return new ValidationResult("Tanggal Jatuh Tempo harus diisi", new List<string> { "PaymentDueDate" });
            }

            if (this.Supplier == null || string.IsNullOrWhiteSpace(Supplier._id))
            {
                yield return new ValidationResult("Supplier harus diisi", new List<string> { "Supplier" });
            }

            if (this.Division == null || string.IsNullOrWhiteSpace(Division._id))
            {
                yield return new ValidationResult("Divisi harus diisi", new List<string> { "Division" });
            }

            if (this.Category == null || string.IsNullOrWhiteSpace(Category._id))
            {
                yield return new ValidationResult("Kategori harus diisi", new List<string> { "Category" });
            }

            if (this.Currency == null || string.IsNullOrWhiteSpace(Currency._id))
            {
                yield return new ValidationResult("Mata Uang harus diisi", new List<string> { "Currency" });
            }

            if(string.IsNullOrWhiteSpace(ConfirmationOrderNo) || ConfirmationOrderNo == null)
            {
                yield return new ValidationResult("No Order Confirmation harus diisi", new List<string> { "ConfirmationOrderNo" });
            }

            //if (string.IsNullOrWhiteSpace(Investation) || Investation == null)
            //{
            //    yield return new ValidationResult("Investasi harus diisi", new List<string> { "Investation" });
            //}

            //if (string.IsNullOrWhiteSpace(InvoiceNo) || InvoiceNo == null)
            //{
            //    yield return new ValidationResult("No Invoice harus diisi", new List<string> { "InvoiceNo" });
            //}

            if (string.IsNullOrWhiteSpace(PaymentMethod) || PaymentMethod == null)
            {
                yield return new ValidationResult("Term Pembayaran harus diisi", new List<string> { "PaymentMethod" });
            }

            if (string.IsNullOrWhiteSpace(ProformaNo) || ProformaNo == null)
            {
                yield return new ValidationResult("No Proforma harus diisi", new List<string> { "ProformaNo" });
            }

            if (string.IsNullOrWhiteSpace(Bank) || Bank == null)
            {
                yield return new ValidationResult("Bank harus diisi", new List<string> { "Bank" });
            }

            int itemErrorCount = 0;
            int detailErrorCount = 0;

            if (this.Items.Count.Equals(0))
            {
                yield return new ValidationResult("Items harus diisi", new List<string> { "itemscount" });
            }
            else
            {
                string tax = "";
                var epoNo = Items.ToArray();
                List<String> duplicate = new List<string>();
                string disposisiItemError = "[";

                foreach (PurchasingDispositionItemViewModel Item in Items)
                {
                    disposisiItemError += "{ ";

                    if (string.IsNullOrWhiteSpace(Item.EPONo))
                    {
                        itemErrorCount++;
                        disposisiItemError += "Epo: 'PurchaseOrderExternal harus diisi', ";
                    }
                    else
                    {
                        if (duplicate.Count <= 0)
                        {
                            duplicate.Add(Item.EPONo);
                        }
                        else
                        {
                            //ExternalPurchaseOrderItemViewModel dup = Array.Find(duplicate, o => o.prNo == Item.prNo);
                            var x = duplicate.Find(a => a == Item.EPONo);
                            if (x != null)
                            {
                                itemErrorCount++;
                                disposisiItemError += "Epo: 'PurchaseOrderExternal sudah dipilih', ";


                            }
                            else
                            {
                                duplicate.Add(Item.EPONo);
                            }
                        }
                        var taxId = "";
                        if (Item.UseIncomeTax)
                        {
                            taxId = Item.IncomeTax._id;
                        }
                        if (tax == "")
                        {
                            
                            tax = Item.UseIncomeTax.ToString() + Item.UseVat.ToString() + taxId;
                        }
                        else if(tax != Item.UseIncomeTax.ToString() + Item.UseVat.ToString() + taxId)
                        {
                            itemErrorCount++;
                            disposisiItemError += "incomeTax: 'Pajak PPN dan PPH PO Eksternal harus sama', ";
                        }
                    }

                    if (Item.Details==null || Item.Details.Count.Equals(0))
                    {
                        yield return new ValidationResult("Details harus diisi", new List<string> { "Details" });
                    }
                    else
                    {

                        string dispositionDetailError = "[";

                        foreach (PurchasingDispositionDetailViewModel Detail in Item.Details)
                        {


                            dispositionDetailError += "{ ";
                            
                           
                            if (Detail.PaidPrice <= 0)
                            {
                                detailErrorCount++;
                                dispositionDetailError += "PaidPrice: 'Harga Dibayar harus lebih dari 0', ";
                            }

                            

                            if (Detail.PaidQuantity <= 0)
                            {
                                detailErrorCount++;
                                dispositionDetailError += "PaidQuantity: 'Jumlah dibayar harus lebih dari 0', ";
                            }

                            dispositionDetailError += " }, ";
                        }

                        dispositionDetailError += "]";

                        if (detailErrorCount > 0)
                        {
                            itemErrorCount++;
                            disposisiItemError += string.Concat("Details: ", dispositionDetailError);
                        }

                    }
                    disposisiItemError += " }, ";
                }

                disposisiItemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(disposisiItemError, new List<string> { "Items" });
            }

        
        }
    }
}
