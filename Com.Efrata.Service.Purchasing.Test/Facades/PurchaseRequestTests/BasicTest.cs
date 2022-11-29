using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PurchaseRequestTests
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

        //private PurchaseRequestDataUtil DataUtil
        //{
        //    get { return (PurchaseRequestDataUtil)ServiceProvider.GetService(typeof(PurchaseRequestDataUtil)); }
        //}

        //private PurchaseRequestFacade Facade
        //{
        //    get { return (PurchaseRequestFacade)ServiceProvider.GetService(typeof(PurchaseRequestFacade)); }
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    await DataUtil.GetTestData("Unit test");
        //    Tuple<List<PurchaseRequest>, int, Dictionary<string, string>> Response = Facade.Read();
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_Posted()
        //{
        //    await DataUtil.GetTestDataPosted("Unit test");
        //    Tuple<List<PurchaseRequest>, int, Dictionary<string, string>> Response = Facade.ReadModelPosted();
        //    Assert.NotEmpty(Response.Item1);
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
        //    PurchaseRequest model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.ReadById((int)model.Id);
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    PurchaseRequest model = DataUtil.GetNewData();
        //    var Response = await Facade.Create(model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("Unit test");

        //    var Response = await Facade.Update((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(0, Response);

        //    PurchaseRequestItem modelItem = DataUtil.GetNewData().Items.FirstOrDefault();
        //    model.Items.Add(modelItem);
        //    var ResponseAddItem = await Facade.Update((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(0, ResponseAddItem);

        //    model.Items.Remove(modelItem);
        //    var ResponseRemoveItem = await Facade.Update((int)model.Id, model, "Unit Test");
        //    Assert.NotEqual(0, ResponseRemoveItem);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.Delete((int)model.Id, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}
    }
}
