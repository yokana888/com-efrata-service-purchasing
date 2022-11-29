using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentCorrectionNoteProfile : Profile
    {
        public GarmentCorrectionNoteProfile()
        {
            CreateMap<GarmentCorrectionNote, GarmentCorrectionNoteViewModel>()
                .ForPath(d => d.Supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.Name, opt => opt.MapFrom(s => s.SupplierName))

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))

                .ForPath(d => d.IncomeTax.Id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.IncomeTax.Name, opt => opt.MapFrom(s => s.IncomeTaxName))
                .ForPath(d => d.IncomeTax.Rate, opt => opt.MapFrom(s => s.IncomeTaxRate))

                .ForPath(d => d.Vat.Id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.Vat.Rate, opt => opt.MapFrom(s => s.VatRate))

                .ReverseMap();

            CreateMap<GarmentCorrectionNoteItem, GarmentCorrectionNoteItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomIUnit))

                .ReverseMap();
        }
    }
}
