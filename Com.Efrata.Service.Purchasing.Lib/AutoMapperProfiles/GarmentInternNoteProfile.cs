using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentInternNoteProfile : Profile
    {
        public GarmentInternNoteProfile()
        {
            CreateMap<GarmentInternNote, GarmentInternNoteViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.inNo, opt => opt.MapFrom(s => s.INNo))
                .ForMember(d => d.inDate, opt => opt.MapFrom(s => s.INDate))
                .ForMember(d => d.remark, opt => opt.MapFrom(s => s.Remark))
                
                /*Supplier*/
                .ForPath(d => d.supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.supplier.Name, opt => opt.MapFrom(s => s.SupplierName))

                /*Supplier*/
                .ForPath(d => d.currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))

                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))

                ///*Position*/
                //.ForPath(d => d.position, opt => opt.MapFrom(s => (int)s.Position))

                .ReverseMap();

            CreateMap<GarmentInternNoteItem, GarmentInternNoteItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.garmentInvoice.Id, opt => opt.MapFrom(s => s.InvoiceId))
                .ForPath(d => d.garmentInvoice.invoiceNo, opt => opt.MapFrom(s => s.InvoiceNo))
                .ForPath(d => d.garmentInvoice.invoiceDate, opt => opt.MapFrom(s => s.InvoiceDate))
                .ForPath(d => d.garmentInvoice.totalAmount, opt => opt.MapFrom(s => s.TotalAmount))

                .ForMember(d => d.details, opt => opt.MapFrom(s => s.Details))
                .ReverseMap();

            CreateMap<GarmentInternNoteDetail, GarmentInternNoteDetailViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.ePOId, opt => opt.MapFrom(s => s.EPOId))
                .ForPath(d => d.ePONo, opt => opt.MapFrom(s => s.EPONo))
                
                .ForPath(d => d.deliveryOrder.Id, opt => opt.MapFrom(s => s.DOId))
                .ForPath(d => d.deliveryOrder.doNo, opt => opt.MapFrom(s => s.DONo))
                .ForPath(d => d.deliveryOrder.doDate, opt => opt.MapFrom(s => s.DODate))
                .ForPath(d => d.deliveryOrder.paymentType, opt => opt.MapFrom(s => s.PaymentType))
                .ForPath(d => d.deliveryOrder.paymentMethod, opt => opt.MapFrom(s => s.PaymentMethod))

                .ForPath(d => d.pricePerDealUnit, opt => opt.MapFrom(s => s.PricePerDealUnit))
                .ForPath(d => d.priceTotal, opt => opt.MapFrom(s => s.PriceTotal))
                .ForPath(d => d.poSerialNumber, opt => opt.MapFrom(s => s.POSerialNumber))
                .ForPath(d => d.paymentDueDays, opt => opt.MapFrom(s => s.PaymentDueDays))
                .ForPath(d => d.paymentDueDate, opt => opt.MapFrom(s => s.PaymentDueDate))
                .ForPath(d => d.invoiceDetailId, opt => opt.MapFrom(s => s.InvoiceDetailId))

                /*UOM*/
                .ForPath(d => d.uomUnit.Id, opt => opt.MapFrom(s => s.UOMId))
                .ForPath(d => d.uomUnit.Unit, opt => opt.MapFrom(s => s.UOMUnit))

                /*Product*/
                .ForPath(d => d.product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.product.Name, opt => opt.MapFrom(s => s.ProductName))
                /*Unit*/
                .ForPath(d => d.unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.unit.Name, opt => opt.MapFrom(s => s.UnitName))

                .ForPath(d => d.quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForPath(d => d.pricePerDealUnit, opt => opt.MapFrom(s => s.PricePerDealUnit))
                .ForPath(d => d.priceTotal, opt => opt.MapFrom(s => s.PriceTotal))
                .ReverseMap();
        }
    }
}
