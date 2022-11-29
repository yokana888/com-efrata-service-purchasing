using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils
{
    public class UnitPaymentOrderDataUtil
    {
        private UnitReceiptNoteDataUtil unitReceiptNoteDataUtil;
        private readonly UnitPaymentOrderFacade facade;
        private PurchasingDbContext _dbContext;
        public UnitPaymentOrderDataUtil(PurchasingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UnitPaymentOrderDataUtil(UnitReceiptNoteDataUtil unitReceiptNoteDataUtil, UnitPaymentOrderFacade facade)
        {
            this.unitReceiptNoteDataUtil = unitReceiptNoteDataUtil;
            this.facade = facade;
        }

        public async Task<UnitPaymentOrder> GetNewData()
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await Task.Run(() => this.unitReceiptNoteDataUtil.GetTestData("Unit Test"));

            List<UnitPaymentOrderDetail> unitPaymentOrderDetails = new List<UnitPaymentOrderDetail>();
            foreach (var item in unitReceiptNote.Items)
            {
                unitPaymentOrderDetails.Add(new UnitPaymentOrderDetail
                {
                    URNItemId = item.Id,

                    EPODetailId = item.EPODetailId,
                    PRId = item.PRId,
                    PRNo = item.PRNo,
                    PRItemId = item.PRItemId,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,

                    ReceiptQuantity = item.ReceiptQuantity,

                    UomId = item.UomId,
                    UomUnit = item.Uom,

                    PricePerDealUnit = item.PricePerDealUnit,
                    PriceTotal = item.PricePerDealUnit * item.ReceiptQuantity,
                    QuantityCorrection = 0,

                    ProductRemark = item.ProductRemark
                });
            }

            List<UnitPaymentOrderItem> unitPaymentOrderItems = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    URNId = unitReceiptNote.Id,
                    URNNo = unitReceiptNote.URNNo,

                    DOId = unitReceiptNote.DOId,
                    DONo = unitReceiptNote.DONo,
                    Details = unitPaymentOrderDetails
                }
            };

            UnitPaymentOrder unitPaymentOrder = new UnitPaymentOrder
            {
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",

                SupplierId = "SupplierId",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",

                Date = new DateTimeOffset(),

                CategoryId = "CategoryId ",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",

                CurrencyId = "CurrencyId",
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 5,

                PaymentMethod = "CASH",

                InvoiceNo = "INV000111",
                InvoiceDate = new DateTimeOffset(),
                PibNo = null,
                PibDate = new DateTimeOffset(),
                ImportDuty = 0,
                TotalIncomeTaxAmount = 0,
                TotalVatAmount = 0,

                UseIncomeTax = true,
                IncomeTaxId = null,
                IncomeTaxName = null,
                IncomeTaxRate = 0,
                IncomeTaxNo = null,
                IncomeTaxDate = null,

                UseVat = true,
                VatNo = null,
                VatDate = new DateTimeOffset(),
                Position = 1,
                Remark = null,

                DueDate = new DateTimeOffset(), // ???

                Items = unitPaymentOrderItems
            };
            return unitPaymentOrder;
        }

        public async Task<UnitPaymentOrder> GetNewLocalData()
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await Task.Run(() => this.unitReceiptNoteDataUtil.GetTestDataLocalSupplier("Unit Test"));

            List<UnitPaymentOrderDetail> unitPaymentOrderDetails = new List<UnitPaymentOrderDetail>();
            foreach (var item in unitReceiptNote.Items)
            {
                unitPaymentOrderDetails.Add(new UnitPaymentOrderDetail
                {
                    URNItemId = item.Id,

                    EPODetailId = item.EPODetailId,
                    PRId = item.PRId,
                    PRNo = item.PRNo,
                    PRItemId = item.PRItemId,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,

                    ReceiptQuantity = item.ReceiptQuantity,

                    UomId = item.UomId,
                    UomUnit = item.Uom,

                    PricePerDealUnit = item.PricePerDealUnit,
                    PriceTotal = item.PricePerDealUnit * item.ReceiptQuantity,
                    QuantityCorrection = item.ReceiptQuantity,

                    ProductRemark = item.ProductRemark
                });
            }

            List<UnitPaymentOrderItem> unitPaymentOrderItems = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    URNId = unitReceiptNote.Id,
                    URNNo = unitReceiptNote.URNNo,

                    DOId = unitReceiptNote.DOId,
                    DONo = unitReceiptNote.DONo,
                    Details = unitPaymentOrderDetails
                }
            };

            UnitPaymentOrder unitPaymentOrder = new UnitPaymentOrder
            {
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",

                SupplierId = "SupplierId",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",

                Date = new DateTimeOffset(),

                CategoryId = "CategoryId ",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",

                CurrencyId = "CurrencyId",
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 5,

                PaymentMethod = "CASH",

                InvoiceNo = "INV000111",
                InvoiceDate = new DateTimeOffset(),
                PibNo = null,

                UseIncomeTax = false,
                IncomeTaxId = null,
                IncomeTaxName = null,
                IncomeTaxRate = 0,
                IncomeTaxNo = null,
                IncomeTaxDate = null,

                UseVat = false,
                VatNo = null,
                VatDate = new DateTimeOffset(),
                Position = 1,
                Remark = null,

                DueDate = new DateTimeOffset(), // ???

                Items = unitPaymentOrderItems
            };
            return unitPaymentOrder;
        }

        public async Task<UnitPaymentOrder> GetNewImportDataValas()
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await Task.Run(() => this.unitReceiptNoteDataUtil.GetTestDataImportSupplierValas("Unit Test"));

            List<UnitPaymentOrderDetail> unitPaymentOrderDetails = new List<UnitPaymentOrderDetail>();
            foreach (var item in unitReceiptNote.Items)
            {
                unitPaymentOrderDetails.Add(new UnitPaymentOrderDetail
                {
                    URNItemId = item.Id,

                    EPODetailId = item.EPODetailId,
                    PRId = item.PRId,
                    PRNo = item.PRNo,
                    PRItemId = item.PRItemId,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,

                    ReceiptQuantity = item.ReceiptQuantity,

                    UomId = item.UomId,
                    UomUnit = item.Uom,

                    PricePerDealUnit = item.PricePerDealUnit,
                    PriceTotal = item.PricePerDealUnit * item.ReceiptQuantity,
                    QuantityCorrection = 0,

                    ProductRemark = item.ProductRemark
                });
            }

            List<UnitPaymentOrderItem> unitPaymentOrderItems = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    URNId = unitReceiptNote.Id,
                    URNNo = unitReceiptNote.URNNo,

                    DOId = unitReceiptNote.DOId,
                    DONo = unitReceiptNote.DONo,
                    Details = unitPaymentOrderDetails
                }
            };

            UnitPaymentOrder unitPaymentOrder = new UnitPaymentOrder
            {
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",

                SupplierId = "SupplierId",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",

                Date = new DateTimeOffset(),

                CategoryId = "CategoryId ",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",

                CurrencyId = "CurrencyId",
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 5,

                PaymentMethod = "CASH",

                InvoiceNo = "INV000111",
                InvoiceDate = new DateTimeOffset(),
                PibNo = null,

                UseIncomeTax = false,
                IncomeTaxId = null,
                IncomeTaxName = null,
                IncomeTaxRate = 0,
                IncomeTaxNo = null,
                IncomeTaxDate = null,

                UseVat = false,
                VatNo = null,
                VatDate = new DateTimeOffset(),
                Position = 1,
                Remark = null,

                DueDate = new DateTimeOffset(), // ???

                Items = unitPaymentOrderItems
            };
            return unitPaymentOrder;
        }

        public async Task<UnitPaymentOrder> GetNewImportData()
        {
            Lib.Models.UnitReceiptNoteModel.UnitReceiptNote unitReceiptNote = await Task.Run(() => this.unitReceiptNoteDataUtil.GetTestDataImportSupplier("Unit Test"));

            List<UnitPaymentOrderDetail> unitPaymentOrderDetails = new List<UnitPaymentOrderDetail>();
            foreach (var item in unitReceiptNote.Items)
            {
                unitPaymentOrderDetails.Add(new UnitPaymentOrderDetail
                {
                    URNItemId = item.Id,

                    EPODetailId = item.EPODetailId,
                    PRId = item.PRId,
                    PRNo = item.PRNo,
                    PRItemId = item.PRItemId,

                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,

                    ReceiptQuantity = item.ReceiptQuantity,

                    UomId = item.UomId,
                    UomUnit = item.Uom,

                    PricePerDealUnit = item.PricePerDealUnit,
                    PriceTotal = item.PricePerDealUnit * item.ReceiptQuantity,
                    QuantityCorrection = 0,

                    ProductRemark = item.ProductRemark
                });
            }

            List<UnitPaymentOrderItem> unitPaymentOrderItems = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    URNId = unitReceiptNote.Id,
                    URNNo = unitReceiptNote.URNNo,

                    DOId = unitReceiptNote.DOId,
                    DONo = unitReceiptNote.DONo,
                    Details = unitPaymentOrderDetails
                }
            };

            UnitPaymentOrder unitPaymentOrder = new UnitPaymentOrder
            {
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",

                SupplierId = "SupplierId",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",

                Date = new DateTimeOffset(),

                CategoryId = "CategoryId ",
                CategoryCode = "CategoryCode",
                CategoryName = "CategoryName",

                CurrencyId = "CurrencyId",
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 5,

                PaymentMethod = "CASH",

                InvoiceNo = "INV000111",
                InvoiceDate = new DateTimeOffset(),
                PibNo = null,

                UseIncomeTax = false,
                IncomeTaxId = null,
                IncomeTaxName = null,
                IncomeTaxRate = 0,
                IncomeTaxNo = null,
                IncomeTaxDate = null,

                UseVat = false,
                VatNo = null,
                VatDate = new DateTimeOffset(),
                Position = 1,
                Remark = null,

                DueDate = new DateTimeOffset(), // ???

                Items = unitPaymentOrderItems
            };
            return unitPaymentOrder;
        }

        public async Task<UnitPaymentOrder> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test", false);
            return data;
        }

        public async Task<UnitPaymentOrder> GetTestLocalData()
        {
            var data = await GetNewLocalData();
            await facade.Create(data, "Unit Test", false);
            return data;
        }

        public async Task<UnitPaymentOrder> GetTestImportData()
        {
            var data = await GetNewImportData();
            await facade.Create(data, "Unit Test", false);
            return data;
        }
        public async Task<UnitPaymentOrder> GetTestImportDataValas()
        {
            var data = await GetNewImportDataValas();
            await facade.Create(data, "Unit Test", false);
            return data;
        }

        public UnitPaymentOrder GetNewData_VBRequestPOExternal()
        {
            return new UnitPaymentOrder()
            {
                Items =new List<UnitPaymentOrderItem>()
                {
                    new UnitPaymentOrderItem()
                    {
                        Details =new List<UnitPaymentOrderDetail>()
                        {

                        }
                    }
                }
            };
        }

        public  UnitPaymentOrder GetTestData_VBRequestPOExternal()
        {
            var data = GetNewData_VBRequestPOExternal();
            _dbContext.UnitPaymentOrders.Add(data);
            _dbContext.SaveChanges();
            return data;
        }


    }
}
