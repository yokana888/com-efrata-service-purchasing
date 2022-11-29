using Com.Efrata.Service.Purchasing.Lib.Facades.PurchasingDispositionFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel.EPODispositionLoader;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.PurchasingDispositionDataUtils
{
    public class PurchasingDispositionDataUtil
    {
        private readonly PurchasingDispositionFacade facade;
        private readonly ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil;

        public PurchasingDispositionDataUtil(PurchasingDispositionFacade facade, ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil)
        {
            this.facade = facade;
            this.externalPurchaseOrderDataUtil = externalPurchaseOrderDataUtil;
        }

        public async Task<PurchasingDisposition> GetNewData()
        {
            var s = await Task.Run(() =>  externalPurchaseOrderDataUtil.GetTestData("unit-test"));
            ExternalPurchaseOrderDetail d = new ExternalPurchaseOrderDetail();
            ExternalPurchaseOrderItem i = new ExternalPurchaseOrderItem();
            var itemData = s.Items;
            foreach (var item in itemData)
            {
                i = item; break;
            }
            foreach (var detail in i.Details)
            {
                d = detail; break;
            }

            EPODetailViewModel detailEPO = new EPODetailViewModel
            {
                _id = d.Id,
                poItemId = d.POItemId,
                prItemId = d.PRItemId,
                product = new ProductViewModel
                {
                    _id = d.ProductId,
                    code = d.ProductCode,
                    name = d.ProductName,
                },

                dealQuantity = d.DealQuantity,
                dealUom = new UomViewModel
                {
                    _id = d.DealUomId,
                    unit = d.DealUomUnit,
                },
                doQuantity = d.DOQuantity,
                dispositionQuantity = d.DispositionQuantity,
                productRemark = d.ProductRemark,
                priceBeforeTax = d.PriceBeforeTax,
                pricePerDealUnit = d.PricePerDealUnit,
            };

            EPOItemViewModel itemEPO = new EPOItemViewModel
            {
                _id = i.Id,
                IsDeleted = i.IsDeleted,
                prId = i.PRId,
                poId = i.POId,
                prNo = i.PRNo,
                category = new CategoryViewModel
                    {
                        _id = "CategoryId",
                        code = "CategoryCode",
                        name = "CategoryName"
                    },
                details= new List<EPODetailViewModel>() { detailEPO}
            };

            EPOViewModel dataEPO = new EPOViewModel
            {
                _id = s.Id,
                no = s.EPONo,
                unit = new UnitViewModel
                {
                    _id = s.UnitId,
                    name = s.UnitName,
                    code = s.UnitCode,
                },
                useVat = s.UseVat,
                useIncomeTax = s.UseIncomeTax,
                incomeTax = new IncomeTaxViewModel
                {
                    _id = s.IncomeTaxId,
                    name = s.IncomeTaxName,
                    rate = s.IncomeTaxRate,
                },
                items = new List<EPOItemViewModel>() { itemEPO}

            };
            
            
            return new PurchasingDisposition
            {
                SupplierId = "1",
                SupplierCode = "Supplier1",
                SupplierName = "supplier1",

                Bank="Bank",
                Amount=1000,
                Calculation="axb+c",
                //InvoiceNo="test",
                ConfirmationOrderNo="test",
                //Investation = "test",

                PaymentDueDate=new DateTimeOffset(),
                ProformaNo="aaa",
                PaymentMethod="Test",

                Remark = "Remark1",
                CategoryCode = "test",
                CategoryId = "1",
                CategoryName = "test",


                Items = new List<PurchasingDispositionItem>
                {
                    new PurchasingDispositionItem
                    {
                       EPOId=dataEPO._id.ToString(),
                       EPONo=dataEPO.no,
                       IncomeTaxId="1",
                       IncomeTaxName="tax",
                       IncomeTaxRate=1,
                       UseIncomeTax=true,
                       UseVat=true,
                       Details=new List<PurchasingDispositionDetail>
                       {
                            new PurchasingDispositionDetail
                            {
                                EPODetailId=detailEPO._id.ToString(),
                                
                                DealQuantity=10,
                                PaidQuantity=1000,
                                DealUomId="1",
                                DealUomUnit="test",
                                PaidPrice=1000,
                                PricePerDealUnit=100,
                                PriceTotal=10000,
                                PRId="1",
                                PRNo="test",
                                ProductCode="test",
                                ProductName="test",
                                ProductId="1",
                                   UnitName="test",
                                   UnitCode="test",
                                   UnitId="1",

                            }
                       }
                    }
                }
            };
        }

       

        public async Task<PurchasingDisposition> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test",7);
            return data;
        }

    }
}
