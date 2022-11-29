using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.InternalPurchaseOrderTests
{
  //  [Collection("ServiceProviderFixture Collection")]
    public class SplitTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public SplitTest(ServiceProviderFixture fixture)
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
        //public async Task Should_Success_Split_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");

        //    var NewDataId = model.Id;
        //    model.Id = 0;
        //    foreach(var items in model.Items)
        //    {
        //        items.Id = 0;
        //    }
        //    var Response = await Facade.Split((int)NewDataId, model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Error_Split_Data_When_Quantity_After_Split_more_than_Quantity_Before_Split()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var error = false;
        //    var NewDataId = model.Id;
        //    model.Id = 0;
        //    foreach (var items in model.Items)
        //    {
        //        items.Id = 0;
        //        items.Quantity = items.Quantity + 1;
        //    }
        //    try
        //    {
        //        var Response = await Facade.Split((int)NewDataId, model, "Unit Test");
        //    }
        //    catch (Exception)
        //    {
        //        error = true;
        //    }
        //    Assert.Equal(error,true);
        //}

        //[Fact]
        //public async Task Should_Success_Split_Data_When_Quantity_Before_Split_less_than_Quantity_After_Split()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");

        //    var NewDataId = model.Id;
        //    model.Id = 0;
        //    foreach (var items in model.Items)
        //    {
        //        items.Id = 0;
        //        items.Quantity = items.Quantity - 1;
        //    }
        //    var Response = await Facade.Split((int)NewDataId, model, "Unit Test");
        //    Assert.NotEqual(0, Response);
        //}
    }
}
