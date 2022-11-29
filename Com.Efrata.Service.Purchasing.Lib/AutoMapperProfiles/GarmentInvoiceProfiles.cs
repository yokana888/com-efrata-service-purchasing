using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentInvoiceProfiles : Profile
    {
        public GarmentInvoiceProfiles()
        {
            CreateMap<GarmentInvoice, GarmentInvoiceViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.invoiceNo, opt => opt.MapFrom(s => s.InvoiceNo))
                .ForMember(d => d.invoiceDate, opt => opt.MapFrom(s => s.InvoiceDate))
                .ForMember(d => d.isPayVat, opt => opt.MapFrom(s => s.IsPayVat))
                .ForMember(d => d.isPayTax, opt => opt.MapFrom(s => s.IsPayTax))
                .ForMember(d => d.useVat, opt => opt.MapFrom(s => s.UseVat))
                .ForMember(d => d.useIncomeTax, opt => opt.MapFrom(s => s.UseIncomeTax))
                .ForMember(d => d.vatNo, opt => opt.MapFrom(s => s.VatNo))
                .ForMember(d => d.vatDate, opt => opt.MapFrom(s => s.VatDate))
                .ForMember(d => d.incomeTaxNo, opt => opt.MapFrom(s => s.IncomeTaxNo))
				.ForMember(d => d.hasInternNote, opt => opt.MapFrom(s => s.HasInternNote))
				.ForMember(d => d.incomeTaxDate, opt => opt.MapFrom(s => s.IncomeTaxDate))
				.ForMember(d => d.nph, opt => opt.MapFrom(s => s.NPH))
				.ForMember(d => d.npn, opt => opt.MapFrom(s => s.NPN))
				.ForPath(d => d.incomeTaxRate, opt => opt.MapFrom(s => s.IncomeTaxRate))
				.ForPath(d => d.incomeTaxName , opt => opt.MapFrom(s => s.IncomeTaxName))
				.ForPath(d => d.incomeTaxId, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.vatRate, opt => opt.MapFrom(s => s.VatRate))
                .ForPath(d => d.vatId, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
				.ForPath(d => d.currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
				/*Supplier*/
				.ForPath(d => d.supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.supplier.Name, opt => opt.MapFrom(s => s.SupplierName))

                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                .ReverseMap();

            CreateMap<GarmentInvoiceItem, GarmentInvoiceItemViewModel>()
               .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
			   .ForPath(d => d.deliveryOrder.Id, opt => opt.MapFrom(s => s.DeliveryOrderId))
			   .ForPath(d => d.deliveryOrder.doNo, opt => opt.MapFrom(s => s.DeliveryOrderNo))
			   .ForPath(d => d.deliveryOrder.doDate, opt => opt.MapFrom(s => s.DODate))
			   .ForPath(d => d.deliveryOrder.arrivalDate, opt => opt.MapFrom(s => s.ArrivalDate))
			   .ForPath(d => d.deliveryOrder.totalAmount, opt => opt.MapFrom(s =>  s.TotalAmount))
			   .ForPath(d => d.deliveryOrder.paymentMethod, opt => opt.MapFrom(s => s.PaymentMethod))
			   .ForPath(d => d.deliveryOrder.paymentType, opt => opt.MapFrom(s => s.PaymentType))
			   .ReverseMap();

            CreateMap<GarmentInvoiceDetail, GarmentInvoiceDetailViewModel>()
			  .ForPath(d => d.ePOId, opt => opt.MapFrom(s => s.EPOId))
			  .ForPath(d => d.ePONo, opt => opt.MapFrom(s => s.EPONo))
			  .ForPath(d => d.pOId, opt => opt.MapFrom(s => s.IPOId))
			  .ForPath(d => d.dODetailId, opt => opt.MapFrom(s => s.DODetailId))
			  .ForPath(d => d.product.Id, opt => opt.MapFrom(s => s.ProductId))
			  .ForPath(d => d.product.Code, opt => opt.MapFrom(s => s.ProductCode))
			  .ForPath(d => d.product.Name, opt => opt.MapFrom(s => s.ProductName))
			  .ForPath(d => d.pRItemId, opt => opt.MapFrom(s => s.PRItemId))
			  .ForPath(d => d.pRNo, opt => opt.MapFrom(s => s.PRNo))
			  .ForPath(d => d.uoms.Id, opt => opt.MapFrom(s => s.UomId))
			  .ForPath(d => d.uoms.Unit, opt => opt.MapFrom(s => s.UomUnit))
			  .ForPath(d => d.doQuantity, opt => opt.MapFrom(s => s.DOQuantity))
			  .ForPath(d => d.pricePerDealUnit, opt => opt.MapFrom(s => s.PricePerDealUnit))
			  .ForPath(d => d.roNo, opt => opt.MapFrom(s => s.RONo))
			  .ForPath(d => d.paymentDueDays, opt => opt.MapFrom(s => s.PaymentDueDays))
			  .ForPath(d => d.pOSerialNumber, opt => opt.MapFrom(s => s.POSerialNumber))
			  .ReverseMap();
        }
    }
}
