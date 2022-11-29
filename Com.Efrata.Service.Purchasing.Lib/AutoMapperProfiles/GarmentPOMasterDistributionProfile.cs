using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentPOMasterDistributionProfile : Profile
    {
        public GarmentPOMasterDistributionProfile()
        {
            CreateMap<GarmentPOMasterDistribution, GarmentPOMasterDistributionViewModel>()
                .ForPath(d => d.Supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.Name, opt => opt.MapFrom(s => s.SupplierName))
                .ReverseMap();

            CreateMap<GarmentPOMasterDistributionItem, GarmentPOMasterDistributionItemViewModel>()
                .ReverseMap();

            CreateMap<GarmentPOMasterDistributionDetail, GarmentPOMasterDistributionDetailViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.UomCC.Id, opt => opt.MapFrom(s => s.UomCCId))
                .ForPath(d => d.UomCC.Unit, opt => opt.MapFrom(s => s.UomCCUnit))
                .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))
                .ReverseMap();
        }
    }
}
