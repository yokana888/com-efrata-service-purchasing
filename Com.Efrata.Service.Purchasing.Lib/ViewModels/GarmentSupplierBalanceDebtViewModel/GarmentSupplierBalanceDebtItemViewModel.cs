using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentSupplierBalanceDebtViewModel
{
    public class GarmentSupplierBalanceDebtItemViewModel : BaseViewModel
    {
        public double valas { get; set; }
        public double IDR { get; set; }
        public GarmentDelivOrderViewModel deliveryOrder { get; set; }

    }

    public class GarmentDelivOrderViewModel
    {
        public long Id { get; set; }
        public string billNo { get; set; }
        public string dONo { get; set; }
        public string internNo { get; set; }
        public string supplierName { get; set; }
        public DateTimeOffset arrivalDate { get; set; }
        public string paymentType { get; set; }
        public string paymentMethod { get; set; }
    }
}
