using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition
{
    public class PPHBankExpenditureNoteViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string No { get; set; }
        public DateTimeOffset? Date { get; set; }
        public bool IsPosted { get; set; }
        public NewIntegrationViewModel.AccountBankViewModel Bank { get; set; }
        public IncomeTaxExpeditionViewModel IncomeTax { get; set; }
        public NewIntegrationViewModel.DivisionViewModel Division { get; set; }
        public string Currency { get; set; }
        public string BGNo { get; set; }
        public double TotalDPP { get; set; }
        public double TotalIncomeTax { get; set; }
        public List<UnitPaymentOrderViewModel> PPHBankExpenditureNoteItems { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "Date" });
            }
            else if (this.Date > DateTimeOffset.UtcNow)
            {
                yield return new ValidationResult("Date must be lower or equal than today's date", new List<string> { "Date" });
            }

            if (Bank == null)
            {
                yield return new ValidationResult("Bank is required", new List<string> { "Bank" });
            }

            if (Division == null)
            {
                yield return new ValidationResult("Division is required", new List<string> { "Division" });
            }

            if (PPHBankExpenditureNoteItems.Count == 0)
            {
                yield return new ValidationResult("Items is required", new List<string> { "PPHBankExpenditureNoteItems" });

            }
        }

        public PPHBankExpenditureNoteViewModel()
        {    
        }

        public PPHBankExpenditureNoteViewModel(PPHBankExpenditureNote model)
        {
            int.TryParse(model.BankId, out var bankId);
            Id = model.Id;
            No = model.No;
            TotalIncomeTax = model.TotalIncomeTax;
            TotalDPP = model.TotalDPP;
            Date = model.Date;
            IsPosted = model.IsPosted;
            Bank = new NewIntegrationViewModel.AccountBankViewModel()
            {
                Id = bankId,
                Code = model.BankCode,
                BankName = model.BankName,
                AccountName = model.BankAccountName,
                AccountNumber = model.BankAccountNumber,
                Currency = new NewIntegrationViewModel.CurrencyViewModel()
                {
                    Code = model.Currency
                }
            };
            IncomeTax = new IncomeTaxExpeditionViewModel()
            {
                _id = model.IncomeTaxId,
                name = model.IncomeTaxName,
                rate = model.IncomeTaxRate
            };
            Division = new NewIntegrationViewModel.DivisionViewModel()
            {
                Code = model.DivisionCode,
                Id = model.DivisionId,
                Name = model.DivisionName
            };
            BGNo = model.BGNo;
            Currency = model.Currency;
            PPHBankExpenditureNoteItems = new List<UnitPaymentOrderViewModel>();

            foreach(var item in model.Items)
            {
                var m = new UnitPaymentOrderViewModel()
                {
                    Id = item.Id,
                    PurchasingDocumentExpeditionId = item.PurchasingDocumentExpeditionId,
                    No = item.UnitPaymentOrderNo,
                    UPODate = item.PurchasingDocumentExpedition.UPODate,
                    DueDate = item.PurchasingDocumentExpedition.DueDate,
                    InvoiceNo = item.PurchasingDocumentExpedition.InvoiceNo,
                    SupplierCode = item.PurchasingDocumentExpedition.SupplierCode,
                    SupplierName = item.PurchasingDocumentExpedition.SupplierName,
                    DivisionCode = item.PurchasingDocumentExpedition.DivisionCode,
                    DivisionName = item.PurchasingDocumentExpedition.DivisionName,
                    IncomeTax = item.PurchasingDocumentExpedition.IncomeTax,
                    Vat = item.PurchasingDocumentExpedition.Vat,
                    IncomeTaxId = item.PurchasingDocumentExpedition.IncomeTaxId,
                    IncomeTaxName = item.PurchasingDocumentExpedition.IncomeTaxName,
                    IncomeTaxRate = item.PurchasingDocumentExpedition.IncomeTaxRate,
                    TotalPaid = item.PurchasingDocumentExpedition.TotalPaid,
                    Currency = item.PurchasingDocumentExpedition.Currency,
                    Items = new List<UnitPaymentOrderItemViewModel>()
                };

                foreach(var detail in item.PurchasingDocumentExpedition.Items)
                {
                    m.Items.Add(new UnitPaymentOrderItemViewModel()
                    {
                        ProductId = detail.ProductId,
                        ProductCode = detail.ProductCode,
                        ProductName = detail.ProductName,
                        Price = detail.Price,
                        Quantity = detail.Quantity,
                        UnitCode = detail.UnitCode,
                        UnitId = detail.UnitId,
                        UnitName = detail.UnitName,
                        Uom = detail.Uom
                    });
                }

                this.PPHBankExpenditureNoteItems.Add(m);
            }
        }

        public PPHBankExpenditureNote ToModel()
        {
            PPHBankExpenditureNote model = new PPHBankExpenditureNote()
            {
                Id = Id,
                No = No,
                Date = Date.Value,
                TotalIncomeTax = TotalIncomeTax,
                TotalDPP = TotalDPP,
                BankId = Bank.Id.ToString(),
                BankCode = Bank.BankCode,
                BankName = Bank.BankName,
                BankAccountName = Bank.AccountName,
                BankAccountNumber = Bank.AccountNumber,
                //IncomeTaxId = IncomeTax._id,
                //IncomeTaxName = IncomeTax.name,
                //IncomeTaxRate = IncomeTax.rate,
                BGNo = BGNo,
                Currency = Currency,
                DivisionId = Division.Id,
                DivisionCode = Division.Code,
                DivisionName = Division.Name,
                Items = new List<PPHBankExpenditureNoteItem>()
            };

            foreach (var item in PPHBankExpenditureNoteItems)
            {
                model.Items.Add(new PPHBankExpenditureNoteItem()
                {
                    Id = item.Id,
                    PurchasingDocumentExpeditionId = item.PurchasingDocumentExpeditionId,
                    UnitPaymentOrderNo = item.No
                });
            }

            return model;
        }
    }
}
