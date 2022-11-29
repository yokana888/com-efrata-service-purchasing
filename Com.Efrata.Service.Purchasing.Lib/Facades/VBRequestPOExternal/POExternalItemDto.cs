using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class POExternalItemDto
    {
        public POExternalItemDto(ExternalPurchaseOrderDetail element, ExternalPurchaseOrder entity)
        {
            Product = new ProductDto(element.ProductCode, element.ProductId, element.ProductName, element.DefaultUomId, element.DefaultUomUnit);
            DefaultQuantity = element.DefaultQuantity;
            DealQuantity = element.DealQuantity;
            DealUOM = new UOMDto(element.DealUomId, element.DealUomUnit);
            Conversion = element.Conversion;
            Price = element.PricePerDealUnit;
            UseVat = entity.UseVat;
            VatTax = new VatTaxDto(entity.VatId, entity.VatRate);
            Unit = new UnitDto(entity.UnitId, entity.UnitCode, entity.UnitName, entity.DivisionCode, entity.DivisionId, entity.DivisionName);
            IncomeTax = new IncomeTaxDto(entity.IncomeTaxId, entity.IncomeTaxName, entity.IncomeTaxRate);
            IncomeTaxBy = entity.IncomeTaxBy;
            UseIncomeTax = entity.UseIncomeTax;
            EPOId = (int)entity.Id;
        }

        public POExternalItemDto(GarmentExternalPurchaseOrderItem element, GarmentExternalPurchaseOrder entity, GarmentInternalPurchaseOrder purchaseOrder)
        {
            Product = new ProductDto(element.ProductCode, element.ProductId, element.ProductName, element.DefaultUomId, element.DefaultUomUnit);
            DefaultQuantity = element.DefaultQuantity;
            DealQuantity = element.DealQuantity;
            DealUOM = new UOMDto(element.DealUomId, element.DealUomUnit);
            Conversion = element.Conversion;
            Price = element.PricePerDealUnit;
            UseVat = entity.IsUseVat;
            VatTax = new VatTaxDto(entity.VatId, entity.VatRate);
            Unit = new UnitDto(purchaseOrder.UnitId, purchaseOrder.UnitCode, purchaseOrder.UnitName, purchaseOrder.DivisionCode, purchaseOrder.DivisionId, purchaseOrder.DivisionName);
            IncomeTax = new IncomeTaxDto(entity.IncomeTaxId, entity.IncomeTaxName, entity.IncomeTaxRate);
            UseIncomeTax = entity.IsIncomeTax;
            EPOId = (int)entity.Id;
        }

        public ProductDto Product { get; private set; }
        public double DefaultQuantity { get; private set; }
        public double DealQuantity { get; private set; }
        public UOMDto DealUOM { get; private set; }
        public double Conversion { get; private set; }
        public double Price { get; private set; }
        public bool UseVat { get; private set; }
        public UnitDto Unit { get; private set; }
        public IncomeTaxDto IncomeTax { get; private set; }
        public VatTaxDto VatTax { get; private set; }
        public string IncomeTaxBy { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public int EPOId { get; private set; }
    }
}