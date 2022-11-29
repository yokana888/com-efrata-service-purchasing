using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentInternalPurchaseOrderProfile : Profile
    {
        public GarmentInternalPurchaseOrderProfile()
        {
            CreateMap<GarmentInternalPurchaseOrderItem, GarmentInternalPurchaseOrderItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

                .ForPath(d => d.Category.Id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.Category.Name, opt => opt.MapFrom(s => s.CategoryName))
                .ReverseMap();

            CreateMap<GarmentInternalPurchaseOrder, GarmentInternalPurchaseOrderViewModel>()
                .ForPath(d => d.Buyer.Id, opt => opt.MapFrom(s => s.BuyerId))
                .ForPath(d => d.Buyer.Code, opt => opt.MapFrom(s => s.BuyerCode))
                .ForPath(d => d.Buyer.Name, opt => opt.MapFrom(s => s.BuyerName))

                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))
                .ReverseMap();
        }
    }
}
