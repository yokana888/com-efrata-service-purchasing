using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase
{
    public class DispositionUserIndexDto
    {
        public List<DispositionUserDto> Data { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }

        public DispositionUserIndexDto(List<DispositionUserDto> data, int page, int total)
        {
            Data = data;
            Page = page;
            Total = total;
        }
    }
}
