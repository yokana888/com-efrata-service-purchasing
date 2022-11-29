using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports
{
    public class TraceableInBeacukaiViewModelTemp
    {
        public int count { get; set; }
        public string BCNo { get; set; }
        public string BonNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset BCDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double ProduksiQty { get; set; }
        public string BUM { get; set; }
        public double ReceiptQty { get; set; }
        public double BJQty { get; set; }
        public string SatuanReceipt { get; set; }
        public double EksporQty { get; set; }
        public string PO { get; set; }
        //public string SatuanWIP { get; set; }
        public string ROJob { get; set; }
        public string ROSample { get; set; }
        public string Invoice { get; set; }
        public DateTimeOffset? PEBDate { get; set; }
        public string PEB { get; set; }
        public string BUK { get; set; }
        public double QtyBUK { get; set; }
        public string SatuanBUK { get; set; }
        public double SampleQty { get; set; }
        public double SampleQtyOut { get; set; }
        public double SubkonOutQty { get; set; }
        public double Sisa { get; set; }
        public string ExType { get; set; }
        public double Subcon { get; set; }
        public string SampleOut { get; set; }
        public string UnitDOType { get; set; }
        public string ExpenditureType { get; set; }
        public long UENItemId { get; set; }
        public double WIP { get; set; }


    }

    public class TraceableInBeacukaiViewModel
    {
        public int count { get; set; }
        public string BCNo { get; set; }
        public string BonNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset BCDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ProduksiQty { get; set; }
        public string BUM { get; set; }
        public string ReceiptQty { get; set; }
        public string BJQty { get; set; }
        public string SatuanReceipt { get; set; }
        public string EksporQty { get; set; }
        public string PO { get; set; }
        //public string SatuanWIP { get; set; }
        public string ROJob { get; set; }
        public string ROSample { get; set; }
        public string Invoice { get; set; }
        public DateTimeOffset? PEBDate { get; set; }
        public string PEB { get; set; }
        public string BUK { get; set; }
        public string SampleQtyOut { get; set; }
        public string QtyBUK { get; set; }
        public string SatuanBUK { get; set; }
        public string SampleQty { get; set; }
        public string SubkonOutQty { get; set; }
        public string Sisa { get; set; }
        public string ExType { get; set; }
        public string Subcon { get; set; }
        public string SampleOut { get; set; }
        public string UnitDOType { get; set; }
        public string WIP { get; set; }




        public class TraceableInDataBeacukaiViewModel
        {
            public string BCNo { get; set; }
            public string BonNo { get; set; }
            public string BCType { get; set; }
            public DateTimeOffset BCDate { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ProduksiQty { get; set; }
            public string BUM { get; set; }
            public double ReceiptQty { get; set; }
            public double BJQty { get; set; }
            public string SatuanReceipt { get; set; }
            public double EksporQty { get; set; }
            public string PO { get; set; }
            //public string SatuanWIP { get; set; }
            public string ROJob { get; set; }
            public string ROSample { get; set; }
            public string Invoice { get; set; }
            public DateTimeOffset? PEBDate { get; set; }
            public string PEB { get; set; }
            public string BUK { get; set; }
            public double QtyBUK { get; set; }
            public double SampleQtyOut { get; set; }
            public string SatuanBUK { get; set; }
            public double SampleQty { get; set; }
            public double SubkonOutQty { get; set; }
            public string Sisa { get; set; }
            public string ExType { get; set; }
            public string Subcon { get; set; }
            public string SampleOut { get; set; }
            public string UnitDOType { get; set; }
        }


    }

    public class TraceableInWithBUMBeacukaiViewModel
    {
        public string URNNo { get; set; }
        public decimal SmallQty { get; set; }
        public string SatuanReceipt { get; set; }
        public string UnitDONo { get; set; }
        public string ROJob { get; set; }
        public string UnitDOType { get; set; }

        public double DOQuantity { get; set; }
        public string DOUomUnit { get; set; }
        public string PO { get; set; }

        public string BUK { get; set; }
        public string UENType { get; set; }
        public double QtyBUK { get; set; }
        public string SatuanBUK { get; set; }
        public DateTime LastModifiedUtc { get; set; }

    }

    public class TraceableInForToReceipt
    {
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset BCDate { get; set; }
        public string BonNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double ReceiptQty { get; set; }
        public string SatuanReceipt { get; set; }
        public long URNItemId { get; set; }
        public string POSerialNumber { get; set; }
    }

    public class TraceableInForToExpend
    {
        public string ROJob { get; set; }
        public string ROSample { get; set; }
        public string BUK { get; set; }
        public double QtyBUK { get; set; }
        public string SatuanBUK { get; set; }
        public double SampleQty { get; set; }
        public double SampleQtyOut { get; set; }
        public string UnitDOType { get; set; }
        public string ExpenditureType { get; set; }
        public long URNItemId { get; set; }
        public long UENItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string POSerialNumber { get; set; }

    }
}
