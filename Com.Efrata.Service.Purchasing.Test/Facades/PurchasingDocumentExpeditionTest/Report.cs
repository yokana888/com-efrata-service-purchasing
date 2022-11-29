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
    public class Report
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public Report(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private SendToVerificationDataUtil DataUtil
        //{
        //    get { return (SendToVerificationDataUtil)ServiceProvider.GetService(typeof(SendToVerificationDataUtil)); }
        //}

        //private PurchasingDocumentExpeditionReportFacade Facade
        //{
        //    get { return (PurchasingDocumentExpeditionReportFacade)ServiceProvider.GetService(typeof(PurchasingDocumentExpeditionReportFacade)); }
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data()
        //{
        //    PurchasingDocumentExpedition model = await DataUtil.GetTestData();
        //    List<string> unitPaymentOrders = new List<string>() { model.UnitPaymentOrderNo };
        //    var Response = this.Facade.GetReport(unitPaymentOrders);
        //    Assert.NotEmpty(Response);
        //}
    }
}
