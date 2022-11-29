using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.InternalPurchaseOrderTests
{
   // [Collection("ServiceProviderFixture Collection")]
    public class BasicTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public BasicTest(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private InternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (InternalPurchaseOrderDataUtil)ServiceProvider.GetService(typeof(InternalPurchaseOrderDataUtil)); }
        //}

        //private InternalPurchaseOrderFacade Facade
        //{
        //    get { return (InternalPurchaseOrderFacade)ServiceProvider.GetService(typeof(InternalPurchaseOrderFacade)); }
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    await DataUtil.GetTestData("Unit test");
        //    Tuple<List<InternalPurchaseOrder>, int, Dictionary<string, string>> Response = Facade.Read();
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.ReadById((int)model.Id);
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetNewData("Unit test");
        //    var Response = await Facade.Create(model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = await Facade.Update((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.Delete((int)model.Id, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_CountPRNo()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.ReadByPRNo(model.PRNo);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Fulfillment_Data()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    var Response = await Facade.CreateFulfillmentAsync(model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Fail_Create_Fulfillment_Data()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    //model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    await Assert.ThrowsAnyAsync<Exception>(() => Facade.CreateFulfillmentAsync(model, "Unit Test"));
        //    //Assert.NotEqual(Response, 0);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Fulfillment_Data()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    var created = await Facade.CreateFulfillmentAsync(model, "Unit Test");
        //    var Response = await Facade.UpdateFulfillmentAsync((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Fail_Update_Fulfillment_Data_NotFound()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    //var created = await Facade.CreateFulfillment(model, "Unit Test");
        //    //var Response = await Facade.UpdateFulfillment((int)model.Id, model, "Unit Test");
        //    await Assert.ThrowsAnyAsync<Exception>(() => Facade.UpdateFulfillmentAsync((int)model.Id, model, "Unit Test"));
        //}

        //[Fact]
        //public async Task Should_Fail_Update_Fulfillment_Data_Exception()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    var created = await Facade.CreateFulfillmentAsync(model, "Unit Test");
        //    var Response = await Facade.UpdateFulfillmentAsync((int)model.Id, model, "Unit Test");
        //    model.POItemId = 0;
        //    await Assert.ThrowsAnyAsync<Exception>(() => Facade.UpdateFulfillmentAsync((int)model.Id, model, "Unit Test"));
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Fulfillment_Data()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    var created = await  Facade.CreateFulfillmentAsync(model, "Unit Test");
        //    var Response = Facade.DeleteFulfillment((int)model.Id, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Fail_Delete_Fulfillment_Data()
        //{
        //    InternalPurchaseOrder modelIpo = await DataUtil.GetTestData("Unit test");
        //    var model = DataUtil.GetNewFulfillmentData("Unit test");
        //    model.POItemId = modelIpo.Items.FirstOrDefault().Id;

        //    var created = await Facade.CreateFulfillmentAsync(model, "Unit Test");
        //    //var Response = Facade.DeleteFulfillment((int)0, "Unit Test");
        //    Assert.ThrowsAny<Exception>(() => Facade.DeleteFulfillment((int)0, "Unit Test"));
        //}
    }
}
