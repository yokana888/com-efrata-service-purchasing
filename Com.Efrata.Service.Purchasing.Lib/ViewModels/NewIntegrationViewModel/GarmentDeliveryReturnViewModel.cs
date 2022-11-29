using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class GarmentDeliveryReturnViewModel
    {
        public string Id { get; set; }
        public string DRNo { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public long UnitDOId { get; set; }
        public string UnitDONo { get; set; }
        public int UENId { get; set; }
        public string PreparingId { get; set; }
        public DateTimeOffset? ReturnDate { get; set; }
        public string ReturnType { get; set; }
        public UnitViewModel Unit { get; set; }
        public StorageViewModel Storage { get; set; }
        public bool IsUsed { get; set; }
        public virtual List<GarmentDeliveryReturnItemViewModel> Items { get; set; }
    }

    public class GarmentDeliveryReturnItemViewModel
    {
        public string Id { get; set; }
        public Guid DRId { get; set; }
        public int UnitDOItemId { get; set; }
        public int UENItemId { get; set; }
        public string PreparingItemId { get; set; }
        public ProductViewModel Product { get; set; }
        public string DesignColor { get; set; }
        public string RONo { get; set; }
        public double Quantity { get; set; }
        public double QuantityUENItem { get; set; }
        public double RemainingQuantityPreparingItem { get; set; }
        public bool IsSave { get; set; }

        public UomViewModel Uom { get; set; }
    }
}
