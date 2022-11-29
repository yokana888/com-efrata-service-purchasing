using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PurchasingDocumentExpeditionTest
{
 //   [Collection("ServiceProviderFixture Collection")]
    public class BasicTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public BasicTest(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private SendToVerificationDataUtil DataUtil
        //{
        //    get { return (SendToVerificationDataUtil)ServiceProvider.GetService(typeof(SendToVerificationDataUtil)); }
        //}

        //private PurchasingDocumentExpeditionFacade Facade
        //{
        //    get { return (PurchasingDocumentExpeditionFacade)ServiceProvider.GetService(typeof(PurchasingDocumentExpeditionFacade)); }
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    await DataUtil.GetTestData();
        //    var Response = this.Facade.Read();
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_With_Arguments()
        //{
        //    string order = "{\"UnitPaymentOrderNo\":\"desc\"}";
        //    string filter = "{\"Position\":2, \"IsPaidPPH\":\"false\"}";
        //    string keyword = "Supplier";

        //    await DataUtil.GetTestData();
        //    var Response = this.Facade.Read(1, 25, order, keyword, filter);
        //    Assert.NotEmpty(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    PurchasingDocumentExpedition Data = await DataUtil.GetTestData();
        //    int AffectedRows = await this.Facade.Delete(Data.Id);
        //    Assert.True(AffectedRows > 0);
        //}        
    }
}
