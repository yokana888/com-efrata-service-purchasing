using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentSupplierBalanceDebtViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentSupplierBalanceDebtProfile : Profile
    {
        public GarmentSupplierBalanceDebtProfile()
        {
            CreateMap<GarmentSupplierBalanceDebt, GarmentSupplierBalanceDebtViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Year, opt => opt.MapFrom(c => c.Year))
                .ForMember(d => d.dOCurrencyRate, opt => opt.MapFrom(c => c.DOCurrencyRate))
                .ForPath(d => d.currency.Id, opt => opt.MapFrom(c => c.DOCurrencyId))
                .ForPath(d => d.currency.Code, opt => opt.MapFrom(c => c.DOCurrencyCode))
                .ForPath(d => d.supplier.Id, opt => opt.MapFrom(c => c.SupplierId))
                .ForPath(d => d.supplier.Code, opt => opt.MapFrom(c => c.SupplierCode))
                .ForPath(d => d.supplier.Name, opt => opt.MapFrom(c => c.SupplierName))
                .ForPath(d => d.supplier.Import, opt => opt.MapFrom(c => c.Import))
                .ForMember(d => d.codeRequirment, opt => opt.MapFrom(c => c.CodeRequirment))
                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                .ReverseMap();
            CreateMap<GarmentSupplierBalanceDebtItem, GarmentSupplierBalanceDebtItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.valas, opt => opt.MapFrom(c => c.Valas))
                .ForMember(d => d.IDR, opt => opt.MapFrom(c => c.IDR))
                .ForPath(d => d.deliveryOrder.billNo, opt => opt.MapFrom(c => c.BillNo))
                .ForPath(d => d.deliveryOrder.dONo, opt => opt.MapFrom(c => c.DONo))
                .ForPath(d => d.deliveryOrder.Id, opt => opt.MapFrom(c => c.DOId))
                .ForPath(d => d.deliveryOrder.internNo, opt => opt.MapFrom(c => c.InternNo))
                .ForPath(d => d.deliveryOrder.arrivalDate, opt => opt.MapFrom(c => c.ArrivalDate))
                .ForPath(d => d.deliveryOrder.paymentType, opt => opt.MapFrom(c => c.PaymentType))
                .ForPath(d => d.deliveryOrder.paymentMethod, opt => opt.MapFrom(c => c.PaymentMethod))
                .ReverseMap();

        }
    }
}
