using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentPreparing
{
    public class GarmentPreparingViewModel
    {

        public long UENId { get; set; }
        public string UENNo { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public bool IsCuttingIn { get; set; }
        public string CreatedBy { get; set; }
        public string ProductCode { get; set; }
        public double RemainingQuantity { get; set; }
        public double Quantity { get; set; }
        public long UENItemId { get; set; }
    }

    public class Buyer
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class UnitDepartment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }



}
