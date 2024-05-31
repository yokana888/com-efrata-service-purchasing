using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils
{
    public class GarmentDeliveryOrderDataUtil
    {
        private readonly GarmentDeliveryOrderFacade facade;
        private readonly GarmentExternalPurchaseOrderDataUtil garmentExternalPurchaseOrderDataUtil;

        public GarmentDeliveryOrderDataUtil(GarmentDeliveryOrderFacade facade, GarmentExternalPurchaseOrderDataUtil garmentExternalPurchaseOrderDataUtil)
        {
            this.facade = facade;
            this.garmentExternalPurchaseOrderDataUtil = garmentExternalPurchaseOrderDataUtil;
        }

        public async Task<GarmentDeliveryOrder> GetNewData(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder = null)
        {
            var datas = garmentExternalPurchaseOrder ?? await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataFabric());
            List<GarmentExternalPurchaseOrderItem> EPOItem = new List<GarmentExternalPurchaseOrderItem>(datas.Items);
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksA}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksA}",
                ShipmentNo = $"ShipmentNo{nowTicksA}",

                Remark = $"Remark{nowTicksA}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = "InternNO1234",
                PaymentBill = "BB181122003",
                BillNo = "BP181122142947000001",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,
                CustomsId = 1,

                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        EPOId = datas.Id,
                        EPONo = datas.EPONo,
                        CurrencyId = datas.CurrencyId,
                        CurrencyCode = "USD",
                        PaymentDueDays = datas.PaymentDueDays,

                        Details = new List<GarmentDeliveryOrderDetail>
                        {
                            new GarmentDeliveryOrderDetail
                            {
                                EPOItemId = EPOItem[0].Id,
                                POId = EPOItem[0].POId,
                                POItemId = (int)nowTicks,
                                PRId = EPOItem[0].PRId,
                                PRNo = EPOItem[0].PRNo,
                                PRItemId = nowTicks,
                                POSerialNumber = EPOItem[0].PO_SerialNumber,
                                UnitId =  $"{nowTicksA}",
                                UnitCode = $"{nowTicksA}",
                                ProductId = EPOItem[0].ProductId,
                                ProductCode = EPOItem[0].ProductCode,
                                ProductName = EPOItem[0].ProductName,
                                ProductRemark = EPOItem[0].Remark,
                                DOQuantity = EPOItem[0].DOQuantity,
                                DealQuantity = EPOItem[0].DealQuantity,
                                Conversion = EPOItem[0].Conversion,
                                UomId = EPOItem[0].DealUomId.ToString(),
                                UomUnit = EPOItem[0].DealUomUnit,
                                SmallQuantity = EPOItem[0].SmallQuantity,
                                SmallUomId = EPOItem[0].SmallUomId.ToString(),
                                SmallUomUnit = EPOItem[0].SmallUomUnit,
                                PricePerDealUnit = EPOItem[0].PricePerDealUnit,
                                PriceTotal = EPOItem[0].PricePerDealUnit,
                                RONo = EPOItem[0].RONo,
                                ReceiptQuantity = 0,
                                QuantityCorrection = EPOItem[0].DOQuantity,
                                PricePerDealUnitCorrection = EPOItem[0].PricePerDealUnit,
                                PriceTotalCorrection = EPOItem[0].PricePerDealUnit,
                                CodeRequirment = $"{nowTicksA}",
                                ReturQuantity=0
                            }
                        }
                    }
                }
            };
        }

        public async Task<GarmentDeliveryOrder> GetNewData21()
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataFabric2());
            List<GarmentExternalPurchaseOrderItem> EPOItem = new List<GarmentExternalPurchaseOrderItem>(datas.Items);
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksA}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksA}",
                ShipmentNo = $"ShipmentNo{nowTicksA}",

                Remark = $"Remark{nowTicksA}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = "InternNO1234",
                PaymentBill = "BB181122003",
                BillNo = "BP181122142947000001",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        EPOId = datas.Id,
                        EPONo = datas.EPONo,
                        CurrencyId = datas.CurrencyId,
                        CurrencyCode = "USD",
                        PaymentDueDays = datas.PaymentDueDays,

                        Details = new List<GarmentDeliveryOrderDetail>
                        {
                            new GarmentDeliveryOrderDetail
                            {
                                EPOItemId = EPOItem[0].Id,
                                POId = EPOItem[0].POId,
                                POItemId = (int)nowTicks,
                                PRId = EPOItem[0].PRId,
                                PRNo = EPOItem[0].PRNo,
                                PRItemId = nowTicks,
                                POSerialNumber = EPOItem[0].PO_SerialNumber,
                                UnitId =  $"{nowTicksA}",
                                UnitCode = $"{nowTicksA}",
                                ProductId = EPOItem[0].ProductId,
                                ProductCode = EPOItem[0].ProductCode,
                                ProductName = EPOItem[0].ProductName,
                                ProductRemark = EPOItem[0].Remark,
                                DOQuantity = EPOItem[0].DOQuantity,
                                DealQuantity = EPOItem[0].DealQuantity,
                                Conversion = EPOItem[0].Conversion,
                                UomId = EPOItem[0].DealUomId.ToString(),
                                UomUnit = EPOItem[0].DealUomUnit,
                                SmallQuantity = EPOItem[0].SmallQuantity,
                                SmallUomId = EPOItem[0].SmallUomId.ToString(),
                                SmallUomUnit = EPOItem[0].SmallUomUnit,
                                PricePerDealUnit = EPOItem[0].PricePerDealUnit,
                                PriceTotal = EPOItem[0].PricePerDealUnit,
                                RONo = EPOItem[0].RONo,
                                ReceiptQuantity = 0,
                                QuantityCorrection = EPOItem[0].DOQuantity,
                                PricePerDealUnitCorrection = EPOItem[0].PricePerDealUnit,
                                PriceTotalCorrection = EPOItem[0].PricePerDealUnit,
                                CodeRequirment = $"{nowTicksA}",
                                ReturQuantity=0
                            }
                        }
                    }
                }
            };
        }

        public async Task<GarmentDeliveryOrder> GetNewData2()
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataForDo());
            List<GarmentExternalPurchaseOrderItem> EPOItem = new List<GarmentExternalPurchaseOrderItem>(datas.Items);
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksA}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksA}",
                ShipmentNo = $"ShipmentNo{nowTicksA}",

                Remark = $"Remark{nowTicksA}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = $"{nowTicksB}",
                PaymentBill = $"{nowTicksB}",
                BillNo = $"{nowTicksB}",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        EPOId = datas.Id,
                        EPONo = datas.EPONo,
                        CurrencyId = datas.CurrencyId,
                        CurrencyCode = "USD",
                        PaymentDueDays = datas.PaymentDueDays,

                        Details = new List<GarmentDeliveryOrderDetail>
                        {
                            new GarmentDeliveryOrderDetail
                            {
                                EPOItemId = EPOItem[0].Id,
                                POId = EPOItem[0].POId,
                                POItemId = (int)nowTicks,
                                PRId = EPOItem[0].PRId,
                                PRNo = EPOItem[0].PRNo,
                                PRItemId = nowTicks,
                                POSerialNumber = EPOItem[0].PO_SerialNumber,
                                UnitId =  $"{nowTicksA}",
                                UnitCode = $"{nowTicksA}",
                                ProductId = EPOItem[0].ProductId,
                                ProductCode = EPOItem[0].ProductCode,
                                ProductName = EPOItem[0].ProductName,
                                ProductRemark = EPOItem[0].Remark,
                                DOQuantity = EPOItem[0].DOQuantity,
                                DealQuantity = EPOItem[0].DealQuantity,
                                Conversion = EPOItem[0].Conversion,
                                UomId = EPOItem[0].DealUomId.ToString(),
                                UomUnit = EPOItem[0].DealUomUnit,
                                SmallQuantity = EPOItem[0].SmallQuantity,
                                SmallUomId = EPOItem[0].SmallUomId.ToString(),
                                SmallUomUnit = EPOItem[0].SmallUomUnit,
                                PricePerDealUnit = EPOItem[0].PricePerDealUnit,
                                PriceTotal = EPOItem[0].PricePerDealUnit,
                                RONo = EPOItem[0].RONo,
                                ReceiptQuantity = 0,
                                QuantityCorrection = EPOItem[0].DOQuantity,
                                PricePerDealUnitCorrection = EPOItem[0].PricePerDealUnit,
                                PriceTotalCorrection = EPOItem[0].PricePerDealUnit,
                                CodeRequirment = $"{nowTicksA}",
                                ReturQuantity=0
                            }
                        }
                    }
                }
            };
        }

        public async Task<GarmentDeliveryOrder> GetNewData3()
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataForDo2());
            List<GarmentExternalPurchaseOrderItem> EPOItem = new List<GarmentExternalPurchaseOrderItem>(datas.Items);
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksA}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksA}",
                ShipmentNo = $"ShipmentNo{nowTicksA}",

                Remark = $"Remark{nowTicksA}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = $"{nowTicksB}",
                PaymentBill = $"{nowTicksB}",
                BillNo = $"{nowTicksB}",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        EPOId = datas.Id,
                        EPONo = datas.EPONo,
                        CurrencyId = datas.CurrencyId,
                        CurrencyCode = "USD",
                        PaymentDueDays = datas.PaymentDueDays,

                        Details = new List<GarmentDeliveryOrderDetail>
                        {
                            new GarmentDeliveryOrderDetail
                            {
                                EPOItemId = EPOItem[0].Id,
                                POId = EPOItem[0].POId,
                                POItemId = (int)nowTicks,
                                PRId = EPOItem[0].PRId,
                                PRNo = EPOItem[0].PRNo,
                                PRItemId = nowTicks,
                                POSerialNumber = EPOItem[0].PO_SerialNumber,
                                UnitId =  $"{nowTicksA}",
                                UnitCode = $"{nowTicksA}",
                                ProductId = EPOItem[0].ProductId,
                                ProductCode = EPOItem[0].ProductCode,
                                ProductName = EPOItem[0].ProductName,
                                ProductRemark = EPOItem[0].Remark,
                                DOQuantity = EPOItem[0].DOQuantity,
                                DealQuantity = EPOItem[0].DealQuantity,
                                Conversion = EPOItem[0].Conversion,
                                UomId = EPOItem[0].DealUomId.ToString(),
                                UomUnit = EPOItem[0].DealUomUnit,
                                SmallQuantity = EPOItem[0].SmallQuantity,
                                SmallUomId = EPOItem[0].SmallUomId.ToString(),
                                SmallUomUnit = EPOItem[0].SmallUomUnit,
                                PricePerDealUnit = EPOItem[0].PricePerDealUnit,
                                PriceTotal = EPOItem[0].PricePerDealUnit,
                                RONo = EPOItem[0].RONo,
                                ReceiptQuantity = 0,
                                QuantityCorrection = EPOItem[0].DOQuantity,
                                PricePerDealUnitCorrection = EPOItem[0].PricePerDealUnit,
                                PriceTotalCorrection = EPOItem[0].PricePerDealUnit,
                                CodeRequirment = $"{nowTicksA}",
                                ReturQuantity=0
                            }
                        }
                    }
                }
            };
        }

        public async Task<GarmentDeliveryOrder> GetNewData4(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder = null)
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataForDo2(garmentExternalPurchaseOrder));

            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            var gdo = new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksB}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksB}",
                ShipmentNo = $"ShipmentNo{nowTicksB}",

                Remark = $"Remark{nowTicksB}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = $"{nowTicksB}",
                PaymentBill = $"{nowTicksA}",
                BillNo = $"{nowTicksA}",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>()
            };

            foreach (var item in datas.Items)
            {
                gdo.Items = (List<GarmentDeliveryOrderItem>)gdo.Items.Concat(new[] { new GarmentDeliveryOrderItem
                {
                    EPOId = datas.Id,
                    EPONo = datas.EPONo,
                    CurrencyId = datas.CurrencyId,
                    CurrencyCode = "USD",
                    PaymentDueDays = datas.PaymentDueDays,

                    Details = new List<GarmentDeliveryOrderDetail>
                            {
                                new GarmentDeliveryOrderDetail
                                {
                                    EPOItemId = item.Id,
                                    POId = item.POId,
                                    POItemId = (int)nowTicks,
                                    PRId = item.PRId,
                                    PRNo = item.PRNo,
                                    PRItemId = nowTicks,
                                    POSerialNumber = item.PO_SerialNumber,
                                    UnitId =  $"{nowTicksB}",
                                    UnitCode = $"{nowTicksB}",
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    ProductRemark = item.Remark,
                                    DOQuantity = item.DOQuantity,
                                    DealQuantity = item.DealQuantity,
                                    Conversion = item.Conversion,
                                    UomId = item.DealUomId.ToString(),
                                    UomUnit = item.DealUomUnit,
                                    SmallQuantity = item.SmallQuantity,
                                    SmallUomId = item.SmallUomId.ToString(),
                                    SmallUomUnit = item.SmallUomUnit,
                                    PricePerDealUnit = item.PricePerDealUnit,
                                    PriceTotal = item.PricePerDealUnit,
                                    RONo = item.RONo,
                                    ReceiptQuantity = 0,
                                    QuantityCorrection = item.DOQuantity,
                                    PricePerDealUnitCorrection = item.PricePerDealUnit,
                                    PriceTotalCorrection = item.PricePerDealUnit,
                                    CodeRequirment = $"{nowTicksA}",
                                    ReturQuantity=0
                                }
                            }
                }});
            }

            return gdo;
        }

        public async Task<GarmentDeliveryOrder> GetNewData5(GarmentExternalPurchaseOrder garmentExternalPurchaseOrder = null)
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestDataForDo2(garmentExternalPurchaseOrder));

            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            var gdo = new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksB}",

                SupplierId = 1,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksB}",
                ShipmentNo = $"ShipmentNo{nowTicksB}",

                Remark = $"Remark{nowTicksB}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = $"{nowTicksB}",
                PaymentBill = $"{nowTicksA}",
                BillNo = $"{nowTicksA}",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>()
            };

            foreach (var item in datas.Items)
            {
                gdo.Items = (List<GarmentDeliveryOrderItem>)gdo.Items.Concat(new[] { new GarmentDeliveryOrderItem
                {
                    EPOId = datas.Id,
                    EPONo = datas.EPONo,
                    CurrencyId = datas.CurrencyId,
                    CurrencyCode = "USD",
                    PaymentDueDays = datas.PaymentDueDays,

                    Details = new List<GarmentDeliveryOrderDetail>
                            {
                                new GarmentDeliveryOrderDetail
                                {
                                    EPOItemId = item.Id,
                                    POId = item.POId,
                                    POItemId = (int)nowTicks,
                                    PRId = item.PRId,
                                    PRNo = item.PRNo,
                                    PRItemId = nowTicks,
                                    POSerialNumber = item.PO_SerialNumber,
                                    UnitId =  $"{nowTicksB}",
                                    UnitCode = $"{nowTicksB}",
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    ProductRemark = item.Remark,
                                    DOQuantity = item.DOQuantity,
                                    DealQuantity = item.DealQuantity,
                                    Conversion = item.Conversion,
                                    UomId = item.DealUomId.ToString(),
                                    UomUnit = item.DealUomUnit,
                                    SmallQuantity = item.SmallQuantity,
                                    SmallUomId = item.SmallUomId.ToString(),
                                    SmallUomUnit = item.SmallUomUnit,
                                    PricePerDealUnit = item.PricePerDealUnit,
                                    PriceTotal = item.PricePerDealUnit,
                                    RONo = item.RONo,
                                    ReceiptQuantity = 0,
                                    QuantityCorrection = item.DOQuantity,
                                    PricePerDealUnitCorrection = item.PricePerDealUnit,
                                    PriceTotalCorrection = item.PricePerDealUnit,
                                    CodeRequirment = $"{nowTicksA}",
                                    ReturQuantity=0
                                }
                            }
                }});
            }

            return gdo;
        }

        public async Task<GarmentDeliveryOrder> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetNewData_DOCurrency()
        {
            var datas = await Task.Run(() => garmentExternalPurchaseOrderDataUtil.GetTestData_DOCurrency());
            List<GarmentExternalPurchaseOrderItem> EPOItem = new List<GarmentExternalPurchaseOrderItem>(datas.Items);
            Random rnd = new Random();
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";

            return new GarmentDeliveryOrder
            {
                DONo = $"{nowTicksA}",

                SupplierId = nowTicks,
                SupplierCode = $"SupplierCode{nowTicksA}",
                SupplierName = $"SupplierName{nowTicksA}",
                Country = $"Country{nowTicksA}",

                DODate = DateTimeOffset.Now,
                ArrivalDate = DateTimeOffset.Now,

                ShipmentType = $"ShipmentType{nowTicksA}",
                ShipmentNo = $"ShipmentNo{nowTicksA}",

                Remark = $"Remark{nowTicksA}",

                IsClosed = false,
                IsCustoms = false,
                IsInvoice = false,

                UseVat = datas.IsUseVat,
                UseIncomeTax = datas.IsIncomeTax,
                IncomeTaxId = Convert.ToInt32(datas.IncomeTaxId),
                IncomeTaxName = datas.IncomeTaxName,
                IncomeTaxRate = Convert.ToDouble(datas.IncomeTaxRate),

                IsCorrection = false,

                InternNo = "InternNO1234",
                PaymentBill = "BB181122003",
                BillNo = "BP181122142947000001",
                PaymentType = datas.PaymentType,
                PaymentMethod = datas.PaymentMethod,
                DOCurrencyId = datas.CurrencyId,
                DOCurrencyCode = datas.CurrencyCode,
                DOCurrencyRate = datas.CurrencyRate,

                TotalAmount = nowTicks,

                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        EPOId = datas.Id,
                        EPONo = datas.EPONo,
                        CurrencyId = datas.CurrencyId,
                        CurrencyCode = "USD",
                        PaymentDueDays = datas.PaymentDueDays,

                        Details = new List<GarmentDeliveryOrderDetail>
                        {
                            new GarmentDeliveryOrderDetail
                            {
                                EPOItemId = EPOItem[0].Id,
                                POId = EPOItem[0].POId,
                                POItemId = (int)nowTicks,
                                PRId = EPOItem[0].PRId,
                                PRNo = EPOItem[0].PRNo,
                                PRItemId = nowTicks,
                                POSerialNumber = EPOItem[0].PO_SerialNumber,
                                UnitId =  $"{nowTicksA}",
                                UnitCode = $"{nowTicksA}",
                                ProductId = EPOItem[0].ProductId,
                                ProductCode = EPOItem[0].ProductCode,
                                ProductName = EPOItem[0].ProductName,
                                ProductRemark = EPOItem[0].Remark,
                                DOQuantity = EPOItem[0].DOQuantity,
                                DealQuantity = EPOItem[0].DealQuantity,
                                Conversion = EPOItem[0].Conversion,
                                UomId = EPOItem[0].DealUomId.ToString(),
                                UomUnit = EPOItem[0].DealUomUnit,
                                SmallQuantity = EPOItem[0].SmallQuantity,
                                SmallUomId = EPOItem[0].SmallUomId.ToString(),
                                SmallUomUnit = EPOItem[0].SmallUomUnit,
                                PricePerDealUnit = EPOItem[0].PricePerDealUnit,
                                PriceTotal = EPOItem[0].PricePerDealUnit,
                                RONo = EPOItem[0].RONo,
                                ReceiptQuantity = 0,
                                QuantityCorrection = EPOItem[0].DOQuantity,
                                PricePerDealUnitCorrection = EPOItem[0].PricePerDealUnit,
                                PriceTotalCorrection = EPOItem[0].PricePerDealUnit,
                                CodeRequirment = $"{nowTicksA}",
                                ReturQuantity=0
                            }
                        }
                    }
                }
            };
        }

        public async Task<GarmentDeliveryOrder> GetTestDataDO_Currency()
        {
            var data = await GetNewData_DOCurrency();
            data.DOCurrencyRate = 0;
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetTestData21()
        {
            var data = await GetNewData21();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetTestData2()
        {
            var data = await GetNewData2();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetTestData3()
        {
            var data = await GetNewData3();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetTestData4(GarmentDeliveryOrder data = null)
        {
            data = data ?? await GetNewData4();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<GarmentDeliveryOrder> GetNewData(string user)
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }



        public async Task<GarmentDeliveryOrder> GetTestData5()
        {
            var data = await GetNewData3();
            await facade.Create(data, "Unit Test");
            return data;
        }
        public async Task<GarmentDeliveryOrder> GetDatas(string user)
        {
            GarmentDeliveryOrder garmentDeliveryOrder = await GetNewData();
            garmentDeliveryOrder.IsInvoice = false;
            foreach (var item in garmentDeliveryOrder.Items)
            {
                foreach (var detail in item.Details)
                {
                    detail.DOQuantity = 0;
                    detail.DealQuantity = 2;
                }
            }

            await facade.Create(garmentDeliveryOrder, user, 7);

            return garmentDeliveryOrder;
        }
    }
}