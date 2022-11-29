using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class PurchasingDispositionProfile : Profile
    {
        public PurchasingDispositionProfile()
        {
            CreateMap<PurchasingDisposition, PurchasingDispositionViewModel>()
                .ForPath(d => d.Supplier._id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.Currency._id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.Currency.rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.Currency.description, opt => opt.MapFrom(s => s.CurrencyDescription))
                .ForPath(d => d.Category._id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.Category.code, opt => opt.MapFrom(s => s.CategoryCode))
                .ForPath(d => d.Category.name, opt => opt.MapFrom(s => s.CategoryName))
                .ForPath(d => d.Division._id, opt => opt.MapFrom(s => s.DivisionId))
                .ForPath(d => d.Division.code, opt => opt.MapFrom(s => s.DivisionCode))
                .ForPath(d => d.Division.name, opt => opt.MapFrom(s => s.DivisionName))
                .ReverseMap();

            CreateMap<PurchasingDispositionItem, PurchasingDispositionItemViewModel>()
                .ForPath(d => d.IncomeTax._id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.IncomeTax.name, opt => opt.MapFrom(s => s.IncomeTaxName))
                .ForPath(d => d.IncomeTax.rate, opt => opt.MapFrom(s => s.IncomeTaxRate))

                .ForPath(d => d.vatTax._id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.vatTax.rate, opt => opt.MapFrom(s => s.VatRate))

                .ReverseMap();

            CreateMap<PurchasingDispositionDetail, PurchasingDispositionDetailViewModel>()
                .ForPath(d => d.Product._id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.name, opt => opt.MapFrom(s => s.ProductName))
                .ForPath(d => d.DealUom._id, opt => opt.MapFrom(s => s.DealUomId))
                .ForPath(d => d.DealUom.unit, opt => opt.MapFrom(s => s.DealUomUnit))
                .ForPath(d => d.Unit._id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.name, opt => opt.MapFrom(s => s.UnitName))
                .ReverseMap();
        }
    }
}
