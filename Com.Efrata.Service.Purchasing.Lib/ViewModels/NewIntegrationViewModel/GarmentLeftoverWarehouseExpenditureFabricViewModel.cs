using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class GarmentLeftoverWarehouseExpenditureFabricViewModel
    {
        public int Id { get; set; }
        public string ExpenditureNo { get; set; }
        public DateTimeOffset? ExpenditureDate { get; set; }
        public string ExpenditureDestination { get; set; }
        public UnitViewModel UnitExpenditure { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public string EtcRemark { get; set; }
        public string Remark { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }
        public double QtyKG { get; set; }
        public bool IsUsed { get; set; }

        public List<GarmentLeftoverWarehouseExpenditureFabricItemViewModel> Items { get; set; }
    }

    public class GarmentLeftoverWarehouseExpenditureFabricItemViewModel
    {
        public int Id { get; set; }
        public int StockId { get; set; }

        public UnitViewModel Unit { get; set; }

        public string PONo { get; set; }

        public double Quantity { get; set; }

        public UomViewModel Uom { get; set; }
        public double BasicPrice { get; set; }
    }

    public class GarmentLeftoverWarehouseReportExpenditureViewModel
    {
        public string ExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureDestination { get; set; }
        public string DescriptionOfPurpose { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public UnitViewModel UnitExpenditure { get; set; }
        public string EtcRemark { get; set; }
        public string PONo { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset? BCDate { get; set; }
        public double QtyKG { get; set; }
        public string Composition { get; set; }
        public string Const { get; set; }
        public double Price { get; set; }

    }

    public class GarmentLeftoverWarehouseStockMonitoringViewModel
    {
        public int index { get; set; }
        public string UnitCode { get; set; }
        public string PONo { get; set; }
        public string RO { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public string FabricRemark { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityUnitCode { get; set; }
        public string Comodity { get; set; }
        public double BeginingbalanceQty { get; set; }
        public double QuantityReceipt { get; set; }
        public double QuantityExpend { get; set; }
        public double EndbalanceQty { get; set; }
        public string UomUnit { get; set; }
        public string ReferenceType { get; set; }
    }

    public class ReceiptMonitoringViewModel
    {
        public int index { get; set; }
        public string ReceiptNoteNo { get; set; }
        public string UENNo { get; set; }
        public string FabricRemark { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string POSerialNumber { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public string Composition { get; set; }

        public List<string> CustomsNo { get; set; }
        public List<string> CustomsType { get; set; }
        public List<DateTimeOffset> CustomsDate { get; set; }
        public double Price { get; set; }


    }
}
