using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentCuttingOut
{
    public class GarmentCuttingOutViewModel
    {
        public string CutOutNo { get; set; }
        public string CuttingOutType { get; set; }

        public DateTimeOffset CuttingOutDate { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }

        public double TotalCuttingOutQuantity { get; set; }
    }

}
