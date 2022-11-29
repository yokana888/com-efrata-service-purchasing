using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentUnitDeliveryOrderProfile : Profile
    {
        public GarmentUnitDeliveryOrderProfile()
        {
            CreateMap<GarmentUnitDeliveryOrder, GarmentUnitDeliveryOrderViewModel>()
                .ForPath(d => d.UnitRequest.Id, opt => opt.MapFrom(s => s.UnitRequestId))
                .ForPath(d => d.UnitRequest.Code, opt => opt.MapFrom(s => s.UnitRequestCode))
                .ForPath(d => d.UnitRequest.Name, opt => opt.MapFrom(s => s.UnitRequestName))

                .ForPath(d => d.UnitSender.Id, opt => opt.MapFrom(s => s.UnitSenderId))
                .ForPath(d => d.UnitSender.Code, opt => opt.MapFrom(s => s.UnitSenderCode))
                .ForPath(d => d.UnitSender.Name, opt => opt.MapFrom(s => s.UnitSenderName))

                .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
                .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
                .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))

                .ForPath(d => d.StorageRequest._id, opt => opt.MapFrom(s => s.StorageRequestId))
                .ForPath(d => d.StorageRequest.code, opt => opt.MapFrom(s => s.StorageRequestCode))
                .ForPath(d => d.StorageRequest.name, opt => opt.MapFrom(s => s.StorageRequestName))

                .ForPath(d => d.Supplier._id, opt => opt.MapFrom(s => s.SupplierReceiptId))
                .ForPath(d => d.Supplier.name, opt => opt.MapFrom(s => s.SupplierReceiptName))
                .ForPath(d => d.Supplier.code, opt => opt.MapFrom(s => s.SupplierReceiptCode))

                .ReverseMap();

            CreateMap<GarmentUnitDeliveryOrderItem, GarmentUnitDeliveryOrderItemViewModel>()
                .ForPath(d => d.ProductId, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.ProductCode, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.ProductName, opt => opt.MapFrom(s => s.ProductName))
                .ForPath(d => d.ProductRemark, opt => opt.MapFrom(s => s.ProductRemark))

                .ForPath(d => d.UomId, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.UomUnit, opt => opt.MapFrom(s => s.UomUnit))

                .ForPath(d => d.DOCurrency.Rate, opt => opt.MapFrom(s => s.DOCurrencyRate))

                .ReverseMap();
        }
    }
}
