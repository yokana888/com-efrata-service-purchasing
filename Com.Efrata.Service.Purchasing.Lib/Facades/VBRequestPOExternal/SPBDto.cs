using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Moonlay.EntityFrameworkCore;
using iTextSharp.text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class SPBDto
    {
        public SPBDto(GarmentInternNote element, List<GarmentInvoice> invoices)
        {
            Id = element.Id;
            No = element.INNo;
            Date = element.INDate;

            var invoiceIds = element.Items.Select(item => item.InvoiceId).ToList();
            var elementInvoice = invoices.FirstOrDefault(entity => invoiceIds.Contains(entity.Id));

            Amount = element.Items.SelectMany(item => item.Details).Sum(detail => detail.PriceTotal);

            if (elementInvoice != null)
            {
                UseVat = elementInvoice.UseVat;
                UseIncomeTax = elementInvoice.UseIncomeTax;
                IncomeTax = new IncomeTaxDto(elementInvoice.IncomeTaxId, elementInvoice.IncomeTaxName, elementInvoice.IncomeTaxRate);
                IncomeTaxBy = elementInvoice.IsPayTax ? "Efrata Garmindo Utama" : "Supplier";
                VatTax = new VatTaxDto(Convert.ToString(elementInvoice.VatId), elementInvoice.VatRate);
                IsPayVat = elementInvoice.IsPayVat;
                IsPayTax = elementInvoice.IsPayTax;
            }
        }

        public SPBDto(UnitPaymentOrder element)
        {
            Id = element.Id;
            No = element.UPONo;
            Date = element.Date;

            Amount = element.Items.SelectMany(item => item.Details).Sum(detail => detail.PriceTotal);

            UseVat = element.UseVat;
            UseIncomeTax = element.UseIncomeTax;
            IncomeTax = new IncomeTaxDto(element.IncomeTaxId, element.IncomeTaxName, element.IncomeTaxRate);
            IncomeTaxBy = element.IncomeTaxBy;
            VatTax = new VatTaxDto(element.VatId, element.VatRate);
            IsPayVat = element.UseVat;
            IsPayTax = element.UseIncomeTax;

            UnitCosts = new List<UnitCostDto>();
        }

        public SPBDto(UnitPaymentOrder element, List<UnitPaymentOrderDetail> spbDetails, List<UnitPaymentOrderItem> spbItems, List<UnitReceiptNoteItem> unitReceiptNoteItems, List<UnitReceiptNote> unitReceiptNotes)
        {
            Id = element.Id;
            No = element.UPONo;
            Date = element.Date;
            

            Supplier = new SupplierDto(element);
            Division = new DivisionDto(element.DivisionCode, element.DivisionId, element.DivisionName);

            UseVat = element.UseVat;
            VatTax = new VatTaxDto(element.VatId, element.VatRate);
            UseIncomeTax = element.UseIncomeTax;
            IncomeTax = new IncomeTaxDto(element.IncomeTaxId, element.IncomeTaxName, element.IncomeTaxRate);
            IncomeTaxBy = element.IncomeTaxBy;
            IsPayVat = element.UseVat;
            IsPayTax = element.UseIncomeTax;



            //var selectedSPBDetails = spbDetails.Where(detail =)
            var selectedSPBItems = spbItems.Where(item => item.UPOId == element.Id).ToList();
            var selectedSPBItemIds = selectedSPBItems.Select(item => item.Id).ToList();
            var selectedSPBDetails = spbDetails.Where(detail => selectedSPBItemIds.Contains(detail.UPOItemId)).ToList();


            UnitCosts = selectedSPBDetails.Select(detail => new UnitCostDto(detail, selectedSPBItems, unitReceiptNoteItems, unitReceiptNotes, element)).ToList();
            //Amount = selectedSPBDetails.Sum(detail => detail.PriceTotal);
            Amount = selectedSPBDetails.Sum(detail => {
                var quantity = detail.ReceiptQuantity;
                if (detail.QuantityCorrection > 0)
                    quantity = detail.QuantityCorrection;

                var price = detail.PricePerDealUnit;
                if (detail.PricePerDealUnitCorrection > 0)
                    price = detail.PricePerDealUnitCorrection;

                var result = quantity * price;
                if (detail.PriceTotalCorrection > 0)
                    result = detail.PriceTotalCorrection;

                var total = result;
                /*if (element != null)
                {
                    if (element.UseVat)
                    {
                        result += total * 0.1;
                    }

                    if (element.UseIncomeTax && (element.IncomeTaxBy == "Supplier" || element.IncomeTaxBy == "SUPPLIER"))
                    {
                        result -= total * (element.IncomeTaxRate / 100);
                    }
                }*/

                return result;
            });
        }

        public SPBDto(GarmentInternNote element, List<GarmentInvoice> invoices, List<GarmentInternNoteItem> internNoteItems, List<GarmentInternNoteDetail> internNoteDetails)
        {
            Id = element.Id;
            No = element.INNo;
            Date = element.INDate;

            var invoiceIds = element.Items.Select(item => item.InvoiceId).ToList();
            var elementInvoice = invoices.FirstOrDefault(entity => invoiceIds.Contains(entity.Id));


            Supplier = new SupplierDto(element);
            

            if (elementInvoice != null)
            {
                Division = new DivisionDto("", "0", "GARMENT");
                UseVat = elementInvoice.UseVat;
                UseIncomeTax = elementInvoice.UseIncomeTax;
                IncomeTax = new IncomeTaxDto(elementInvoice.IncomeTaxId, elementInvoice.IncomeTaxName, elementInvoice.IncomeTaxRate);
                //IncomeTaxBy = elementInvoice.IsPayTax ? "Efrata Garmindo Utama" : "Supplier";
                IsPayVat = elementInvoice.IsPayVat;
                IsPayTax = elementInvoice.IsPayTax;
                VatTax = new VatTaxDto(Convert.ToString(elementInvoice.VatId), elementInvoice.VatRate);
                IncomeTaxBy = "Supplier";
            }

            var selectedInternNoteItems = internNoteItems.Where(item => item.GarmentINId == element.Id).ToList();
            var selectedInternNoteItemIds = selectedInternNoteItems.Select(item => item.Id).ToList();
            var selectedInternNoteDetails = internNoteDetails.Where(detail => selectedInternNoteItemIds.Contains(detail.GarmentItemINId)).ToList();

            //Amount = selectedInternNoteDetails.Sum(detail => detail.PriceTotal);
        }

        public SPBDto(GarmentInternNote internNote, List<GarmentInvoice> invoices, List<GarmentInternNoteItem> internNoteItems, List<GarmentInternNoteDetail> internNoteDetails, List<GarmentCorrectionNote> corrections, List<GarmentCorrectionNoteItem> correctionItems) : this(internNote, invoices, internNoteItems, internNoteDetails)
        {
            var selectedInternalNoteItems = internNoteItems.Where(element => element.GarmentINId == internNote.Id).ToList();
            var selectedInternalNoteItemIds = selectedInternalNoteItems.Select(element => element.Id).ToList();
            var selectedInvoiceIds = selectedInternalNoteItems.Select(element => element.InvoiceId).ToList();

            var selectedInternalNoteDetails = internNoteDetails.Where(element => selectedInternalNoteItemIds.Contains(element.GarmentItemINId)).ToList();
            var selectedDOIds = selectedInternalNoteDetails.Select(element => element.DOId).ToList();
            var selectedCorrections = corrections.Where(element => selectedDOIds.Contains(element.DOId)).ToList();

            Amount = invoices.Where(element => selectedInvoiceIds.Contains(element.Id)).Sum(element => element.TotalAmount);

            var correctionAmount = selectedCorrections.Sum(element =>
            {
                var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);

                var total = 0.0;
                if (element.CorrectionType.ToUpper() == "RETUR")
                    total = (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity);
                else
                    total = (double)element.TotalCorrection;

                return total;
            });
            Amount += correctionAmount;
            
            var invoiceIds = selectedInternalNoteItems.Select(item => item.InvoiceId).ToList();
            var elementInvoice = invoices.FirstOrDefault(entity => invoiceIds.Contains(entity.Id));
            VatTax = new VatTaxDto( Convert.ToString(elementInvoice.VatId), elementInvoice.VatRate);
            UnitCosts = selectedInternalNoteDetails.Select(detail => new UnitCostDto(detail, selectedInternalNoteItems, internNote, elementInvoice)).ToList();

        }

        public long Id { get; private set; }
        public string No { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public List<SPBDtoItem> Items { get; private set; }
        public double Amount { get; private set; }
        public SupplierDto Supplier { get; private set; }
        public DivisionDto Division { get; private set; }
        public bool UseVat { get; private set; }
        public VatTaxDto VatTax { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public IncomeTaxDto IncomeTax { get; private set; }
        public string IncomeTaxBy { get; private set; }
        public bool IsPayVat { get; private set; }
        public bool IsPayTax { get; private set; }
        public List<UnitCostDto> UnitCosts { get; private set; }
    }
}