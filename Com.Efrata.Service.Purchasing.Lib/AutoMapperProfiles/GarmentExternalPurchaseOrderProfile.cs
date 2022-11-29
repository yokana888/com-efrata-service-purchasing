using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentExternalPurchaseOrderProfile : Profile
    {
        public GarmentExternalPurchaseOrderProfile()
        {
            CreateMap<GarmentExternalPurchaseOrderItem, GarmentExternalPurchaseOrderItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ForPath(d => d.DealUom.Id, opt => opt.MapFrom(s => s.DealUomId))
                .ForPath(d => d.DealUom.Unit, opt => opt.MapFrom(s => s.DealUomUnit))

                .ForPath(d => d.DefaultUom.Id, opt => opt.MapFrom(s => s.DefaultUomId))
                .ForPath(d => d.DefaultUom.Unit, opt => opt.MapFrom(s => s.DefaultUomUnit))

                .ForPath(d => d.SmallUom.Id, opt => opt.MapFrom(s => s.SmallUomId))
                .ForPath(d => d.SmallUom.Unit, opt => opt.MapFrom(s => s.SmallUomUnit))

                .ReverseMap();

            CreateMap<GarmentExternalPurchaseOrder, GarmentExternalPurchaseOrderViewModel>()
                .ForPath(d => d.Supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.Name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.Supplier.Import, opt => opt.MapFrom(s => s.SupplierImport))

                .ForPath(d => d.IncomeTax.Id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.IncomeTax.Rate, opt => opt.MapFrom(s => s.IncomeTaxRate))
                .ForPath(d => d.IncomeTax.Name, opt => opt.MapFrom(s => s.IncomeTaxName))

                .ForPath(d => d.Vat.Id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.Vat.Rate, opt => opt.MapFrom(s => s.VatRate))

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))

                .ReverseMap();
            #region GarmentDisposition
            CreateMap<GarmentExternalPurchaseOrderItem, ViewModels.GarmentDispositionPurchase.GarmentExternalPurchaseOrderItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ForPath(d => d.DealUom.Id, opt => opt.MapFrom(s => s.DealUomId))
                .ForPath(d => d.DealUom.Unit, opt => opt.MapFrom(s => s.DealUomUnit))

                .ForPath(d => d.DefaultUom.Id, opt => opt.MapFrom(s => s.DefaultUomId))
                .ForPath(d => d.DefaultUom.Unit, opt => opt.MapFrom(s => s.DefaultUomUnit))

                .ForPath(d => d.SmallUom.Id, opt => opt.MapFrom(s => s.SmallUomId))
                .ForPath(d => d.SmallUom.Unit, opt => opt.MapFrom(s => s.SmallUomUnit))

                
                .ReverseMap();

            CreateMap<GarmentExternalPurchaseOrder,ViewModels.GarmentDispositionPurchase.GarmentExternalPurchaseOrderViewModel>()
                .ForPath(d => d.Supplier.Id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.Code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.Name, opt => opt.MapFrom(s => s.SupplierName))
                .ForPath(d => d.Supplier.Import, opt => opt.MapFrom(s => s.SupplierImport))

                .ForPath(d => d.IncomeTax.Id, opt => opt.MapFrom(s => s.IncomeTaxId))
                .ForPath(d => d.IncomeTax.Rate, opt => opt.MapFrom(s => s.IncomeTaxRate))
                .ForPath(d => d.IncomeTax.Name, opt => opt.MapFrom(s => s.IncomeTaxName))

                .ForPath(d => d.Vat.Id, opt => opt.MapFrom(s => s.VatId))
                .ForPath(d => d.Vat.Rate, opt => opt.MapFrom(s => s.VatRate))

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ReverseMap();
            #endregion
        }
    }
}
