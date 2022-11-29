using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentUnitRecetipNoteProfile : Profile
    {
        public GarmentUnitRecetipNoteProfile()
        {
            CreateMap<GarmentUnitReceiptNote, GarmentUnitReceiptNoteViewModel>()
                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))

                .ForPath(d => d.Supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.Name, opt => opt.MapFrom(s => s.SupplierName))

                .ForPath(d => d.Storage._id, opt => opt.MapFrom(s => s.StorageId))
                .ForPath(d => d.Storage.code, opt => opt.MapFrom(s => s.StorageCode))
                .ForPath(d => d.Storage.name, opt => opt.MapFrom(s => s.StorageName))

                .ForPath(d => d.DOCurrency.Rate, opt => opt.MapFrom(s => s.DOCurrencyRate))

                .ReverseMap();

            CreateMap<GarmentUnitReceiptNoteItem, GarmentUnitReceiptNoteItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))
                .ForPath(d => d.Product.Remark, opt => opt.MapFrom(s => s.ProductRemark))

                .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

                .ForPath(d => d.SmallUom.Id, opt => opt.MapFrom(s => s.SmallUomId))
                .ForPath(d => d.SmallUom.Unit, opt => opt.MapFrom(s => s.SmallUomUnit))

                .ReverseMap();
        }
    }
}
