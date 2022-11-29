using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentReceiptCorrectionProfile : Profile
    {
        public GarmentReceiptCorrectionProfile()
        {
            CreateMap<GarmentReceiptCorrection, GarmentReceiptCorrectionViewModel>()
                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))

                .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
                .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
                .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))
                .ReverseMap();

            CreateMap<GarmentReceiptCorrectionItem, GarmentReceiptCorrectionItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ReverseMap();
        }

    }
}
