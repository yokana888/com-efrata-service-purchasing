using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class DispositionPurchaseIndexDto
    {
        public DispositionPurchaseIndexDto(List<DispositionPurchaseTableDto> data, int page, int total)
        {
            Data = data;
            Page = page;
            Total = total;
        }

        public List<DispositionPurchaseTableDto> Data { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }
    }
}
