using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentExpenditureGood
{
    public class GarmentExpenditureGoodViewModel //: BaseViewModel
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ExpenditureDate { get; set; }
        public GarmentComodity Comodity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public Buyer Buyer { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string RONo { get; set; }
        public string Invoice { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public string ExpenditureType { get; set; }
        public string Article { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalPrice { get; set; }
        public Unit Unit { get; set; }
    }

    public class GarmentComodity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Buyer
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Unit
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
