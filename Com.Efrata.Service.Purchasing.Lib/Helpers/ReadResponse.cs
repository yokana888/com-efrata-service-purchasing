using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse
{
    public class ReadResponse<TModel>
    {
        public List<TModel> Data { get; set; }
        public int TotalData { get; set; }
        public Dictionary<string, string> Order { get; set; }

        public ReadResponse(List<TModel> Data, int TotalData, Dictionary<string, string> Order)
        {
            this.Data = Data;
            this.TotalData = TotalData;
            this.Order = Order;
        }
    }
}
