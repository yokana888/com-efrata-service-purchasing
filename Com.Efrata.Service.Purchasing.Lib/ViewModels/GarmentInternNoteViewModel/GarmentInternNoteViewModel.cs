using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class GarmentInternNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string inNo { get; set; }
        public DateTimeOffset inDate { get; set; }
        public string remark { get; set; }
        public CurrencyViewModel currency { get; set; }
        public SupplierViewModel supplier { get; set; }
        public List<GarmentInternNoteItemViewModel> items { get; set; }
        public bool isEdit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IGarmentInvoice invoiceFacade = validationContext == null ? null : (IGarmentInvoice)validationContext.GetService(typeof(IGarmentInvoice));
            IGarmentDeliveryOrderFacade doFacade = validationContext == null ? null : (IGarmentDeliveryOrderFacade)validationContext.GetService(typeof(IGarmentDeliveryOrderFacade));

            if (currency == null)
            {
                yield return new ValidationResult("currency is required", new List<string> { "currency" });
            }
            if (supplier == null)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }

            int itemErrorCount = 0;
            int detailErrorCount = 0;

            if (this.items == null || items.Count <= 0)
            {
                yield return new ValidationResult("Item is required", new List<string> { "itemscount" });
            }
            else
            {
                //Enhance Jason Sept 2021
                List<string> arrNo = new List<string>();
                foreach (var detailItem in items)
                {
                    if (detailItem.garmentInvoice != null)
                    {
                        arrNo.Add(detailItem.garmentInvoice.invoiceNo);
                    }
                }

                string itemError = "[";
                bool? prevUseIncomeTax= null;
                bool? prevUseVat = null;
                string paymentMethod = "";
                long? IncomeTaxId = null;

                foreach (var item in items)
                {
                    itemError += "{";

                    if (item.garmentInvoice == null || item.garmentInvoice.Id == 0)
                    {
                        itemErrorCount++;
                        itemError += "garmentInvoice: 'No Garment Invoice selected', ";
                    }
                    else
                    {
                        //Enhance Jason Sept 2019 : Invoice No Validation

                        //Check Duplicate Invoice No for 1 Invoice
                        //if (arrNo.FindAll(e => e == item.garmentInvoice.invoiceNo).Count > 1)
                        //{
                        //    itemErrorCount++;
                        //    itemError += "garmentInvoice: 'there is duplication of invoiceNo " + item.garmentInvoice.invoiceNo + "', ";
                        //}

                        if (items.ToList().Where(x=>x.garmentInvoice.invoiceNo == item.garmentInvoice.invoiceNo && x.garmentInvoice.invoiceDate == item.garmentInvoice.invoiceDate).Count() > 1)
                        {
                            itemErrorCount++;
                            itemError += "garmentInvoice: 'there is duplication of invoiceNo " + item.garmentInvoice.invoiceNo + "', ";
                        }

                        //Check if Invoice No for Specific Supplier is Existed
                        PurchasingDbContext purchasingDbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
                        var detailData = purchasingDbContext.GarmentInternNoteItems.Where(w => w.InvoiceId == item.garmentInvoice.Id && w.InvoiceDate == item.garmentInvoice.invoiceDate && w.IsDeleted == false).Select(s => new { s.Id, s.GarmentINId, s.InvoiceId, s.InvoiceNo});
                        if (detailData.ToList().Count > 0)
                        {
                            foreach (var itemDetail in detailData)
                            {
                                var headerData = purchasingDbContext.GarmentInternNotes.Where(w => w.Id == itemDetail.GarmentINId  && w.SupplierId == supplier.Id && w.IsDeleted == false).Select(s => new { s.INNo });
                                if (headerData.ToList().Count > 0)
                                {
                                    foreach (var itemHeader in headerData)
                                    {
                                        itemErrorCount++;
                                        itemError += "garmentInvoice: 'invoiceNo " + item.garmentInvoice.invoiceNo  + " already existed on Intern Note No " + itemHeader.INNo.ToString() + "', ";
                                    }
                                }
                            }
                        }

                        var invoice = invoiceFacade.ReadById((int)item.garmentInvoice.Id);
                        if (prevUseIncomeTax != null && prevUseIncomeTax != invoice.UseIncomeTax)
                        {
                            itemErrorCount++;
                            itemError += "useincometax: 'UseIncomeTax harus sama', ";
                        }
                        prevUseIncomeTax = invoice.UseIncomeTax;
                        if (prevUseVat != null && prevUseVat != invoice.UseVat)
                        {
                            itemErrorCount++;
                            itemError += "usevat: 'UseVat harus sama', ";
                        }
                        prevUseVat = invoice.UseVat;
                        if (IncomeTaxId != null && IncomeTaxId != invoice.IncomeTaxId)
                        {
                            itemErrorCount++;
                            itemError += "incometax: 'Income Tax Harus Sama', ";
                        }
                        IncomeTaxId = invoice.IncomeTaxId;
                        if (item.details == null || item.details.Count.Equals(0))
                        {
                            itemErrorCount++;
                            itemError += "detailscount: 'Details is required', ";
                        }
                        else
                        {
                            string detailError = "[";

                            foreach (var detail in item.details)
                            {
                                detailError += "{";
                                var deliveryOrder = doFacade.ReadById((int)detail.deliveryOrder.Id);
                                var invitem = invoice.Items.First(s => s.InvoiceId == item.garmentInvoice.Id);

                                if (invitem != null)
                                {
                                    if (paymentMethod != "" && paymentMethod != invitem.PaymentMethod)
                                    {
                                        detailErrorCount++;
                                        detailError += "paymentMethod: 'TermOfPayment Harus Sama', ";
                                    }
                                    paymentMethod = deliveryOrder.PaymentMethod;
                                }

                                detailError += "}, ";
                            }

                            detailError += "]";

                            if (detailErrorCount > 0)
                            {
                                itemErrorCount++;
                                itemError += $"details: {detailError}, ";
                            }
                        }

                    }


                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "items" });
            }
        }
    }
}
