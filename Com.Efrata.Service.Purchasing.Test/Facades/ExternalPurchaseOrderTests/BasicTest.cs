using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ExternalPurchaseOrderTests
{
  //  [Collection("ServiceProviderFixture Collection")]
    public class BasicTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public BasicTest(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private ExternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (ExternalPurchaseOrderDataUtil)ServiceProvider.GetService(typeof(ExternalPurchaseOrderDataUtil)); }
        //}

        //private ExternalPurchaseOrderFacade Facade
        //{
        //    get { return (ExternalPurchaseOrderFacade)ServiceProvider.GetService(typeof(ExternalPurchaseOrderFacade)); }
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    await DataUtil.GetTestData("Unit test");
        //    Tuple<List<ExternalPurchaseOrder>, int, Dictionary<string, string>> Response = Facade.Read();
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_Unused()
        //{
        //    ExternalPurchaseOrder externalPurchaseOrder = await DataUtil.GetTestDataUnused("Unit test");
        //    List<ExternalPurchaseOrder> Response = Facade.ReadUnused(Keyword:externalPurchaseOrder.EPONo);
        //    Assert.NotEmpty(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_Posted()
        //{
        //    await DataUtil.GetTestDataPosted("Unit test");
        //    Tuple<List<PurchaseRequest>, int, Dictionary<string, string>> Response = Facade.ReadModelPosted();
        //    Assert.NotEqual(Response.Item1.Count, 0);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_With_Arguments()
        //{
        //    string order = "{\"UnitCode\":\"desc\"}";
        //    string filter = "{\"CreatedBy\":\"Unit Test\"}";
        //    string keyword = "Unit";

        //    await DataUtil.GetTestData("Unit Test");
        //    var Response = this.Facade.Read(1, 25, order, keyword, filter);
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.ReadModelById((int)model.Id);
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetNewData("Unit Test");
        //    var Response = await Facade.Create(model, "Unit Test",7);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = await Facade.Update((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(Response, 0);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.Delete((int)model.Id, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_Invalid_Id()
        //{
        //    Exception exception = await Assert.ThrowsAsync<Exception>(() => Facade.Update(0, new ExternalPurchaseOrder(), "Unit Test"));
        //    Assert.Equal(exception.Message, "Invalid Id");
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    model.UseIncomeTax = false;
        //    foreach (var item in model.Items)
        //    {
        //        foreach (var detail in item.Details)
        //        {
        //            detail.DealQuantity -= 1;
        //        }
        //    }
        //    var Response = await Facade.Update((int)model.Id, model, "Unit test");
        //    Assert.NotEqual(0, Response);


        //    ExternalPurchaseOrderItem oldItem = model.Items.FirstOrDefault();
        //    ExternalPurchaseOrderDetail oldDetail = oldItem.Details.FirstOrDefault();
        //    ExternalPurchaseOrderItem newDuplicateItem = new ExternalPurchaseOrderItem
        //    {
        //        POId= oldItem.POId,
        //        PONo=oldItem.PONo,
        //        PRId=oldItem.PRId,
        //        PRNo=oldItem.PRNo,
        //        UnitId=oldItem.UnitId,
        //        UnitCode=oldItem.UnitCode,
        //        UnitName=oldItem.UnitName,
        //        Details = new List<ExternalPurchaseOrderDetail>()
        //    };
        //    ExternalPurchaseOrderDetail oldDuplicateDetail = new ExternalPurchaseOrderDetail
        //    {
        //        PRItemId = oldDetail.PRItemId,
        //        ProductId = oldDetail.ProductId,
        //        ProductCode = oldDetail.ProductCode,
        //        ProductName = oldDetail.ProductName,
        //        ProductRemark = oldDetail.ProductRemark,
        //        DOQuantity = oldDetail.DOQuantity,
        //        DealQuantity = oldDetail.DealQuantity,
        //        DealUomId = oldDetail.DealUomId,
        //        DealUomUnit = oldDetail.DealUomUnit,
        //        ReceiptQuantity = oldDetail.ReceiptQuantity,
        //        DefaultUomId=oldDetail.DefaultUomId,
        //        DefaultUomUnit=oldDetail.DefaultUomUnit,
        //        POItemId=oldDetail.POItemId
        //    };
        //    ExternalPurchaseOrderDetail newDuplicateDetail = new ExternalPurchaseOrderDetail
        //    {
        //        PRItemId = oldDetail.PRItemId,
        //        ProductRemark = oldDetail.ProductRemark,
        //        DOQuantity = oldDetail.DOQuantity,
        //        DealQuantity = oldDetail.DealQuantity,
        //        DealUomId = oldDetail.DealUomId,
        //        DealUomUnit = oldDetail.DealUomUnit,
        //        ReceiptQuantity = oldDetail.ReceiptQuantity,
        //        DefaultUomId = oldDetail.DefaultUomId,
        //        DefaultUomUnit = oldDetail.DefaultUomUnit,
        //        POItemId = oldDetail.POItemId,
        //        ProductId = "PrdId2",
        //        ProductCode = "PrdCode2",
        //        ProductName = "PrdName2",
        //    };
        //    newDuplicateItem.Details.Add(oldDuplicateDetail);
        //    newDuplicateItem.Details.Add(newDuplicateDetail);
        //    model.Items.Add(newDuplicateItem);
        //    var ResponseAddDuplicateItem = await Facade.Update((int)model.Id, model, "Unit test");
        //    Assert.NotEqual(0, ResponseAddDuplicateItem);

        //    var newModelForAddItem = await DataUtil.GetNewData("Unit test");
        //    ExternalPurchaseOrderItem newModelItem = newModelForAddItem.Items.FirstOrDefault();
        //    model.Items.Add(newModelItem);
        //    var ResponseAddItem = await Facade.Update((int)model.Id, model, "Unit test");
        //    Assert.NotEqual(0, ResponseAddItem);

        //    model.Items.Remove(newModelItem);
        //    model.Items.FirstOrDefault().Details.Remove(oldDetail);
        //    var ResponseRemoveItemDetail = await Facade.Update((int)model.Id, model, "Unit test");
        //    Assert.NotEqual(0, ResponseRemoveItemDetail);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_Disposition()
        //{
        //    await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.ReadDisposition();
        //    Assert.NotNull(Response);
        //}
    }
}
