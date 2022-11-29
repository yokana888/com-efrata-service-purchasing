using Com.Efrata.Service.Purchasing.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel
{
    public class GarmentPreSalesContractViewModel : BaseViewModel
    {
        public string SCNo { get; set; }
        public DateTimeOffset SCDate { get; set; }
        public string SCType { get; set; }
        public int SectionId { get; set; }
        public string SectionCode { get; set; }
        public string BuyerAgentId { get; set; }
        public string BuyerAgentName { get; set; }
        public string BuyerAgentCode { get; set; }
        public string BuyerBrandId { get; set; }
        public string BuyerBrandName { get; set; }
        public string BuyerBrandCode { get; set; }
        public int OrderQuantity { get; set; }
        public string Remark { get; set; }
        public bool IsCC { get; set; }
        public bool IsPR { get; set; }
    }
}
