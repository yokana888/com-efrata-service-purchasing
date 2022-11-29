using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class UnitPaymentOrderProfile : Profile
    {
        public UnitPaymentOrderProfile()
        {
            CreateMap<UnitPaymentOrder, UnitPaymentOrderViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.no, opt => opt.MapFrom(s => s.UPONo))
                
                /*Division*/
                .ForPath(d => d.division._id, opt => opt.MapFrom(s => s.DivisionId))
                .ForPath(d => d.division.code, opt => opt.MapFrom(s => s.DivisionCode))
                .ForPath(d => d.division.name, opt => opt.MapFrom(s => s.DivisionName))
                
                /*Supplier*/
                .ForPath(d => d.supplier._id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.supplier.code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.supplier.name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.supplier.address, opt => opt.MapFrom(s => s.SupplierAddress))

                /*Category*/
                .ForPath(d => d.category._id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.category.code, opt => opt.MapFrom(s => s.CategoryCode))
                .ForPath(d => d.category.name, opt => opt.MapFrom(s => s.CategoryName))
                
                /*Currency*/
                .ForPath(d => d.currency._id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.currency.code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.currency.rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.currency.description, opt => opt.MapFrom(s => s.CurrencyDescription))

                /*IncomeTax*/
                .ForPath(d => d.incomeTax._id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.incomeTax.name, opt => opt.MapFrom(s => s.IncomeTaxName))
                .ForPath(d => d.incomeTax.rate, opt => opt.MapFrom(s => s.IncomeTaxRate))

                /*VatTax*/

                .ForPath(d => d.vatTax._id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.vatTax.rate, opt => opt.MapFrom(s => s.VatRate))


                .ReverseMap();

            CreateMap<UnitPaymentOrderItem, UnitPaymentOrderItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))

                .ForPath(d => d.unitReceiptNote._id, opt => opt.MapFrom(s => s.URNId))
                .ForPath(d => d.unitReceiptNote.no, opt => opt.MapFrom(s => s.URNNo))
                .ForPath(d => d.unitReceiptNote.deliveryOrder._id, opt => opt.MapFrom(s => s.DOId))
                .ForPath(d => d.unitReceiptNote.deliveryOrder.no, opt => opt.MapFrom(s => s.DONo))

                .ForPath(d => d.unitReceiptNote.items, opt => opt.MapFrom(s => s.Details))

                .ReverseMap();

            CreateMap<UnitPaymentOrderDetail, UnitPaymentOrderDetailViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))

                /*Product*/
                .ForPath(d => d.product._id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.product.code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.product.name, opt => opt.MapFrom(s => s.ProductName))

                .ForMember(d => d.deliveredQuantity, opt => opt.MapFrom(s => s.ReceiptQuantity))

                /*UOM*/
                .ForPath(d => d.deliveredUom._id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.deliveredUom.unit, opt => opt.MapFrom(s => s.UomUnit))

                .ReverseMap();
        }
    }
}
