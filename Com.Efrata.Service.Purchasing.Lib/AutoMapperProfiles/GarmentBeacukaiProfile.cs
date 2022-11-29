using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
	public class GarmentBeacukaiProfile : Profile
	{
		public GarmentBeacukaiProfile()
		{
			CreateMap<GarmentBeacukai, GarmentBeacukaiViewModel>()
				.ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
				.ForMember(d => d.beacukaiNo, opt => opt.MapFrom(s => s.BeacukaiNo))
				.ForMember(d => d.beacukaiDate, opt => opt.MapFrom(s => s.BeacukaiDate))
				.ForMember(d => d.customType, opt => opt.MapFrom(s => s.CustomsType))
				.ForMember(d => d.customCategory, opt => opt.MapFrom(s => s.CustomsCategory))
				.ForMember(d => d.validationDate, opt => opt.MapFrom(s => s.ValidationDate))
				.ForMember(d => d.billNo, opt => opt.MapFrom(s => s.BillNo))
			  	.ForPath(d => d.netto, opt => opt.MapFrom(s => s.Netto))
			  	.ForPath(d => d.bruto, opt => opt.MapFrom(s => s.Bruto))
			  	.ForPath(d => d.packaging, opt => opt.MapFrom(s => s.Packaging))
			  	.ForPath(d => d.packagingQty, opt => opt.MapFrom(s => s.PackagingQty))
			  
				/*Supplier*/
				.ForPath(d => d.supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
			  	.ForPath(d => d.supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
			  	.ForPath(d => d.supplier.Name, opt => opt.MapFrom(s => s.SupplierName))
			
				.ForPath(d => d.currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
			  	.ForPath(d => d.currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
				//.ForPath(d=> d.importValue.Id, opt=> opt.MapFrom(s=> s.ImportValueId))
				.ForPath(d=> d.importValue, opt => opt.MapFrom(s=> s.ImportValue))
			  	.ReverseMap();

			CreateMap<GarmentBeacukaiItem, GarmentBeacukaiItemViewModel>()
			   .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
			   .ForPath(d => d.deliveryOrder.Id, opt => opt.MapFrom(s => s.GarmentDOId))
			   .ForPath(d => d.deliveryOrder.doNo, opt => opt.MapFrom(s => s.GarmentDONo))
			   .ForPath(d => d.deliveryOrder.doDate, opt => opt.MapFrom(s => s.DODate))
			   .ForPath(d => d.deliveryOrder.arrivalDate, opt => opt.MapFrom(s => s.ArrivalDate))
			   .ForPath(d => d.deliveryOrder.totalAmount, opt => opt.MapFrom(s => s.TotalAmount))
			   .ForMember(d => d.quantity, opt => opt.MapFrom(s => s.TotalQty ))
			  .ReverseMap();
		}
	}
}
