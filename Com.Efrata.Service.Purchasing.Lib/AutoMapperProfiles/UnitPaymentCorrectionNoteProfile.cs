using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Moonlay.Models;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class UnitPaymentCorrectionNoteProfile : Profile
    {
        public UnitPaymentCorrectionNoteProfile()
        {
            CreateMap<UnitPaymentCorrectionNoteItem, UnitPaymentCorrectionNoteItemViewModel>()
                .ForPath(d => d._id, opt => opt.MapFrom(s => s.Id))
                /* Product */
                .ForPath(d => d.product._id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.product.code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.product.name, opt => opt.MapFrom(s => s.ProductName))
                /* UOM */
                .ForPath(d => d.uom._id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.uom.unit, opt => opt.MapFrom(s => s.UomUnit))
                /* Currency */
                .ForPath(d => d.currency._id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.currency.code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.currency.rate, opt => opt.MapFrom(s => s.CurrencyRate))

                .ReverseMap();

            CreateMap<UnitPaymentCorrectionNote, UnitPaymentCorrectionNoteViewModel>()
                .ForPath(d => d._id, opt => opt.MapFrom(s => s.Id))
                /* Supplier */
                .ForPath(d => d.supplier._id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.supplier.code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.supplier.name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.supplier.npwp, opt => opt.MapFrom(s => s.SupplierNpwp))
                /* Division */
                .ForPath(d => d.division._id, opt => opt.MapFrom(s => s.DivisionId))
                .ForPath(d => d.division.code, opt => opt.MapFrom(s => s.DivisionCode))
                .ForPath(d => d.division.name, opt => opt.MapFrom(s => s.DivisionName))
                /* Category */
                .ForPath(d => d.category._id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.category.code, opt => opt.MapFrom(s => s.CategoryCode))
                .ForPath(d => d.category.name, opt => opt.MapFrom(s => s.CategoryName))

                .ReverseMap();
        }
    }
}
