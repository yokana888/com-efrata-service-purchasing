using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInventoryViewModels;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentInventoryProfile : Profile
    {
        public GarmentInventoryProfile()
        {
            //CreateMap<GarmentInventoryDocument, GarmentInventoryDocumentViewModel>()
            //    .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
            //    .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
            //    .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))

            //    .ReverseMap();

            //CreateMap<GarmentInventoryDocumentItem, GarmentInventoryDocumentItemViewModel>()
            //    .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
            //    .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
            //    .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))
            //    .ForPath(d => d.Product.Remark, opt => opt.MapFrom(s => s.ProductRemark))

            //    .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
            //    .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

            //    .ReverseMap();

            //CreateMap<GarmentInventorySummary, GarmentInventorySummaryViewModel>()
            //    .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
            //    .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
            //    .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))

            //    .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
            //    .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
            //    .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

            //    .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
            //    .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

            //    .ReverseMap();

            //CreateMap<GarmentInventoryMovement, GarmentInventoryMovementViewModel>()
            //    .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
            //    .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
            //    .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

            //    .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
            //    .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
            //    .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))

            //    .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
            //    .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

            //    .ReverseMap();
        }
    }
}
