using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class PurchaseRequestProfile : Profile
    {
        public PurchaseRequestProfile()
        {
            CreateMap<PurchaseRequestItem, PurchaseRequestItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.product._id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.product.code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.product.name, opt => opt.MapFrom(s => s.ProductName))
                .ForPath(d => d.product.uom.unit, opt => opt.MapFrom(s => s.Uom))
                .ForPath(d => d.product.uom._id, opt => opt.MapFrom(s => s.UomId))
                .ReverseMap();

            CreateMap<PurchaseRequest, PurchaseRequestViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                /* Budget */
                .ForPath(d => d.budget._id, opt => opt.MapFrom(s => s.BudgetId))
                .ForPath(d => d.budget.code, opt => opt.MapFrom(s => s.BudgetCode))
                .ForPath(d => d.budget.name, opt => opt.MapFrom(s => s.BudgetName))
                /* Category */
                .ForPath(d => d.category._id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.category.code, opt => opt.MapFrom(s => s.CategoryCode))
                .ForPath(d => d.category.name, opt => opt.MapFrom(s => s.CategoryName))
                /* Unit */
                .ForPath(d => d.unit._id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.unit.code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.unit.name, opt => opt.MapFrom(s => s.UnitName))
                /* Division */
                .ForPath(d => d.unit.division._id, opt => opt.MapFrom(s => s.DivisionId))
                .ForPath(d => d.unit.division.code, opt => opt.MapFrom(s => s.DivisionCode))
                .ForPath(d => d.unit.division.name, opt => opt.MapFrom(s => s.DivisionName))
                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                .ReverseMap();
        }
    }
}
