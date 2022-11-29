using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils
{
    public class PurchaseRequestItemDataUtil
    {
        public PurchaseRequestItem GetNewData() => new PurchaseRequestItem
        {
            ProductId = "ProductId",
            ProductCode = "ProductCode",
            ProductName = "ProductName",
            Quantity = 10,
            UomId = "UomId",
            Uom = "Uom",
            Remark = "Remark"
        };
        public PurchaseRequestItemViewModel GetNewDataViewModel() => new PurchaseRequestItemViewModel
        {
            product = new ProductViewModel
            {
                _id = "ProductId",
                code  = "ProductCode",
                name  = "ProductName",
                uom = new UomViewModel
                {
                    _id = "UomId",
                    unit = "Uom",
                }
            },
            quantity = 10,
            remark = "Remark"
        };
    }
}
