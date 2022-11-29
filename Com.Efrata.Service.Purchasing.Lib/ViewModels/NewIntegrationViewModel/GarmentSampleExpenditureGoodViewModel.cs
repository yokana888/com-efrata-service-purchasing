using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentSampleExpenditureGood
{
    public class GarmentSampleExpenditureGoodViewModel
    {
        public Guid Id { get; internal set; }
        public string ExpenditureGoodNo { get; internal set; }
        public Unit Unit { get; internal set; }
        public string ExpenditureType { get; internal set; }
        public string RONo { get; internal set; }
        public string Article { get; internal set; }
        public GarmentComodity Comodity { get; internal set; }
        public Buyer Buyer { get; internal set; }
        public DateTimeOffset ExpenditureDate { get; internal set; }
        public string Invoice { get; internal set; }
        public string ContractNo { get; internal set; }
        public double Carton { get; internal set; }
        public string Description { get; internal set; }
        public bool IsReceived { get; private set; }
        public double TotalQuantity { get; set; }
        public double TotalPrice { get; set; }
    }
    public class Unit
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


    public class GarmentComodity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
