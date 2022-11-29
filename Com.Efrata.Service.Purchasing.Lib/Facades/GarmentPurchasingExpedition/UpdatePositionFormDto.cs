using Com.Efrata.Service.Purchasing.Lib.Enums;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingExpedition
{
    public class UpdatePositionFormDto
    {
        public List<int> Ids { get; set; }
        public PurchasingGarmentExpeditionPosition Position { get; set; }
    }
}