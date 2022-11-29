using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel
{
    public class ExternalPurchaseOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string no { get; set; }
        public DivisionViewModel division { get; set; }
        public UnitViewModel unit { get; set; }
        public SupplierViewModel supplier { get; set; }
        public DateTimeOffset orderDate { get; set; }
        public DateTimeOffset deliveryDate { get; set; }
        public string freightCostBy { get; set; }
        public CurrencyViewModel currency { get; set; }
        public string paymentMethod { get; set; }
        public string poCashType { get; set; }
        public string paymentDueDays { get; set; }
        public bool useVat { get; set; }
        public IncomeTaxViewModel incomeTax { get; set; }

        public VatTaxViewModel vatTax { get; set; }
        public string incomeTaxBy { get; set; }
        public bool useIncomeTax { get; set; }
        public bool isPosted { get; set; }
        public bool isClosed { get; set; }
        public bool isCanceled { get; set; }
        public string remark { get; set; }
        public bool IsCreateOnVBRequest { get; set; }
        public List<ExternalPurchaseOrderItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.unit == null)
            {
                yield return new ValidationResult("Unit is required", new List<string> { "unit" });
            }
            //if (this.supplier == null)
            //{
            //    yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            //}
            if (this.currency == null)
            {
                yield return new ValidationResult("Currency is required", new List<string> { "currency" });
            }
            if (this.orderDate.Equals(DateTimeOffset.MinValue) || this.orderDate == null)
            {
                yield return new ValidationResult("OrderDate is required", new List<string> { "orderDate" });
            }
            else if (this.deliveryDate != null && this.orderDate > this.deliveryDate)
            {
                yield return new ValidationResult("OrderDate is greater than delivery date", new List<string> { "orderDate" });
            }

            if (this.deliveryDate.Equals(DateTimeOffset.MinValue) || this.deliveryDate == null)
            {
                yield return new ValidationResult("Delivery Date is required", new List<string> { "deliveryDate" });
            }
            else if (this.deliveryDate != null && this.orderDate > this.deliveryDate)
            {
                yield return new ValidationResult("OrderDate is greater than delivery date", new List<string> { "deliveryDate" });
            }


            if (this.paymentMethod == "CASH" && (this.poCashType == null || this.poCashType == ""))
            {
                yield return new ValidationResult("poCashType is required", new List<string> { "poCashType" });
            }
            if (this.poCashType != "VB" && this.supplier == null)
            {
                yield return new ValidationResult("Supplier is required", new List<string> { "supplier" });
            }

            if (this.useIncomeTax)
            {
                if (string.IsNullOrEmpty(incomeTaxBy))
                {
                    yield return new ValidationResult("Income tax by is required", new List<string> { "incomeTaxBy" });
                }
            }

            int itemErrorCount = 0;
            int detailErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            else
            {
                var prNo = items.ToArray();
                List<String> duplicate=new List<string>();
                string externalPurchaseOrderItemError = "[";

                foreach (ExternalPurchaseOrderItemViewModel Item in items)
                {
                    externalPurchaseOrderItemError += "{ ";

                    if (string.IsNullOrWhiteSpace(Item.poNo))
                    {
                        itemErrorCount++;
                        externalPurchaseOrderItemError += "purchaseOrder: 'PurchaseRequest is required', ";
                    }
                    else
                    {
                        if (duplicate.Count <= 0)
                        {
                            duplicate.Add(Item.poNo);
                        }
                        else
                        {
                            //ExternalPurchaseOrderItemViewModel dup = Array.Find(duplicate, o => o.prNo == Item.prNo);
                            var x=duplicate.Find(a => a == Item.poNo);
                            if (x != null)
                            {
                                itemErrorCount++;
                                externalPurchaseOrderItemError += "purchaseOrder: 'PurchaseRequest is already used', ";


                            }
                            else
                            {
                                duplicate.Add(Item.poNo);
                            }
                        }
                        
                    }

                    if (Item.details == null || Item.details.Count.Equals(0))
                    {
                        yield return new ValidationResult("Details is required", new List<string> { "details" });
                    }
                    else
                    {

                        string externalPurchaseOrderDetailError = "[";

                        foreach (ExternalPurchaseOrderDetailViewModel Detail in Item.details)
                        {
                            

                            externalPurchaseOrderDetailError += "{ ";
                            if (!this.useVat)
                            {
                                Detail.includePpn = false;
                            }
                            //if (Detail.DefaultUom.unit.Equals(Detail.DealUom.unit) && Detail.DefaultQuantity == Detail.DealQuantity && Detail.Convertion != 1)
                            if (Detail.defaultUom == null)
                            {
                                Detail.defaultUom = Detail.product.uom;
                            }
                            if (Detail.defaultUom.unit.Equals(Detail.dealUom.unit) && Detail.conversion != 1)
                            {
                                detailErrorCount++;
                                externalPurchaseOrderDetailError += "conversion: 'Conversion should be 1', ";
                            }
                            else if(Detail.conversion == 0)
                            {
                                detailErrorCount++;
                                externalPurchaseOrderDetailError += "conversion: 'Conversion must be greater than 0', ";
                            }

                            if (Detail.priceBeforeTax <= 0)
                            {
                                detailErrorCount++;
                                externalPurchaseOrderDetailError += "price: 'Price should be more than 0', ";
                            }
                            
                            if(/*Detail.productPrice==null || */Detail.productPrice == 0)
                            {
                                if(Detail.product!=null && Detail.product._id != null)
                                {
                                    ExternalPurchaseOrderFacade Service = (ExternalPurchaseOrderFacade)validationContext.GetService(typeof(ExternalPurchaseOrderFacade));
                                    ProductViewModel product = Service.GetProduct(Detail.product._id);

                                    if (Detail.priceBeforeTax > product.price)
                                    {
                                        detailErrorCount++;
                                        externalPurchaseOrderDetailError += "price: 'Price must not be greater than default price', ";
                                    }
                                }
                                
                            }
                            else
                            {
                                if (Detail.priceBeforeTax > Detail.productPrice)
                                {
                                    detailErrorCount++;
                                    externalPurchaseOrderDetailError += "price: 'Price must not be greater than default price', ";
                                }
                            }

                            if (Detail.dealQuantity <= 0)
                            {
                                detailErrorCount++;
                                externalPurchaseOrderDetailError += "dealQuantity: 'Quantity should be more than 0', ";
                            }

                            externalPurchaseOrderDetailError += " }, ";
                        }

                        externalPurchaseOrderDetailError += "]";

                        if (detailErrorCount > 0)
                        {
                            itemErrorCount++;
                            externalPurchaseOrderItemError += string.Concat("details: ", externalPurchaseOrderDetailError);
                        }

                    }
                    externalPurchaseOrderItemError += " }, ";
                }

                externalPurchaseOrderItemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(externalPurchaseOrderItemError, new List<string> { "items" });
            }
        
        }
    }
}
