using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentDeliveryOrderProfile : Profile
    {
        public GarmentDeliveryOrderProfile()
        {
            CreateMap<GarmentDeliveryOrder, GarmentDeliveryOrderViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.doNo, opt => opt.MapFrom(s => s.DONo))
                .ForMember(d => d.doDate, opt => opt.MapFrom(s => s.DODate))
                .ForMember(d => d.arrivalDate, opt => opt.MapFrom(s => s.ArrivalDate))

                /*Supplier*/
                .ForPath(d => d.supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.supplier.Name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.supplier.Import, opt => opt.MapFrom(s => s.SupplierIsImport))
                .ForPath(d => d.supplier.Country, opt => opt.MapFrom(s => s.Country))

                .ForPath(d => d.shipmentNo, opt => opt.MapFrom(s => s.ShipmentNo))
                .ForPath(d => d.shipmentType, opt => opt.MapFrom(s => s.ShipmentType))

                .ForPath(d => d.remark, opt => opt.MapFrom(s => s.Remark))
                .ForPath(d => d.isClosed, opt => opt.MapFrom(s => s.IsClosed))
                .ForPath(d => d.isCustoms, opt => opt.MapFrom(s => s.IsCustoms))
                .ForPath(d => d.isInvoice, opt => opt.MapFrom(s => s.IsInvoice))

                .ForPath(d => d.internNo, opt => opt.MapFrom(s => s.InternNo))
                .ForPath(d => d.billNo, opt => opt.MapFrom(s => s.BillNo))
                .ForPath(d => d.paymentBill, opt => opt.MapFrom(s => s.PaymentBill))
				 .ForPath(d => d.customsId, opt => opt.MapFrom(s => s.CustomsId))
				.ForPath(d => d.totalAmount, opt => opt.MapFrom(s => s.TotalAmount))

                .ForMember(d => d.isCorrection, opt => opt.MapFrom(s => s.IsCorrection))

                .ForPath(d => d.incomeTax.Id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.incomeTax.Name, opt => opt.MapFrom(s => s.IncomeTaxName))
                .ForPath(d => d.incomeTax.Rate, opt => opt.MapFrom(s => s.IncomeTaxRate))

                .ForPath(d => d.paymentMethod, opt => opt.MapFrom(s => s.PaymentMethod))
                .ForPath(d => d.paymentType, opt => opt.MapFrom(s => s.PaymentType))
                .ForPath(d => d.docurrency.Id, opt => opt.MapFrom(s => s.DOCurrencyId))
                .ForPath(d => d.docurrency.Code, opt => opt.MapFrom(s => s.DOCurrencyCode))
                .ForPath(d => d.docurrency.Rate, opt => opt.MapFrom(s => s.DOCurrencyRate))

                .ForPath(d => d.incomeTax.Id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.incomeTax.Name, opt => opt.MapFrom(s => s.IncomeTaxName))
                .ForPath(d => d.incomeTax.Rate, opt => opt.MapFrom(s => s.IncomeTaxRate))

                .ForPath(d => d.vat.Id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.vat.Rate, opt => opt.MapFrom(s => s.VatRate))

                .ForPath(d=> d.isPayIncomeTax,opt=> opt.MapFrom(s=> s.IsPayIncomeTax))
                .ForPath(d => d.isPayVAT, opt => opt.MapFrom(s => s.IsPayVAT))

                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                
                .ReverseMap();


            CreateMap<GarmentDeliveryOrderItem, GarmentDeliveryOrderItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.purchaseOrderExternal.Id, opt => opt.MapFrom(s => s.EPOId))
                .ForPath(d => d.purchaseOrderExternal.no, opt => opt.MapFrom(s => s.EPONo))

                .ForPath(d => d.currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))

                .ForPath(d => d.paymentDueDays, opt => opt.MapFrom(s => s.PaymentDueDays))
                
                .ForMember(d => d.fulfillments, opt => opt.MapFrom(s => s.Details))

                .ReverseMap();

            CreateMap<GarmentDeliveryOrderDetail, GarmentDeliveryOrderFulfillmentViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.pRId, opt => opt.MapFrom(s => s.PRId))
                .ForPath(d => d.pRNo, opt => opt.MapFrom(s => s.PRNo))

                .ForPath(d => d.pOId, opt => opt.MapFrom(s => s.POId))
                .ForPath(d => d.pOItemId, opt => opt.MapFrom(s => s.POItemId))

                .ForPath(d => d.pOItemId, opt => opt.MapFrom(s => s.POItemId))

                /*Unit*/
                .ForPath(d => d.unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.unit.Code, opt => opt.MapFrom(s => s.UnitCode))

                /*Product*/
                .ForPath(d => d.product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.product.Name, opt => opt.MapFrom(s => s.ProductName))
                .ForPath(d => d.product.Remark, opt => opt.MapFrom(s => s.ProductRemark))

                .ForPath(d => d.doQuantity, opt => opt.MapFrom(s => s.DOQuantity))
                .ForPath(d => d.dealQuantity, opt => opt.MapFrom(s => s.DealQuantity))

                /*UOM*/
                .ForPath(d => d.purchaseOrderUom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.purchaseOrderUom.Unit, opt => opt.MapFrom(s => s.UomUnit))

                .ForPath(d => d.smallQuantity, opt => opt.MapFrom(s => s.SmallQuantity))

                /*SmallUOM*/
                .ForPath(d => d.smallUom.Id, opt => opt.MapFrom(s => s.SmallUomId))
                .ForPath(d => d.smallUom.Unit, opt => opt.MapFrom(s => s.SmallUomUnit))

                .ForPath(d => d.pricePerDealUnit, opt => opt.MapFrom(s => s.PricePerDealUnit))
                .ForPath(d => d.priceTotal, opt => opt.MapFrom(s => s.PriceTotal))

                .ForPath(d => d.receiptQuantity, opt => opt.MapFrom(s => s.ReceiptQuantity))

                .ForPath(d => d.rONo, opt => opt.MapFrom(s => s.RONo))

                .ForPath(d => d.quantityCorrection, opt => opt.MapFrom(s => s.QuantityCorrection))
                .ForPath(d => d.pricePerDealUnitCorrection, opt => opt.MapFrom(s => s.PricePerDealUnitCorrection))
                .ForPath(d => d.priceTotalCorrection, opt => opt.MapFrom(s => s.PriceTotalCorrection))

                .ForPath(d => d.codeRequirment, opt => opt.MapFrom(s => s.CodeRequirment))
                .ReverseMap();
        }
    }
}
