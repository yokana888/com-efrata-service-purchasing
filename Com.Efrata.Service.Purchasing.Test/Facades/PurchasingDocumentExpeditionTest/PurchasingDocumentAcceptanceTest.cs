using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PurchasingDocumentExpeditionTest
{
  //  [Collection("ServiceProviderFixture Collection")]
    public class PurchasingDocumentAcceptanceTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public PurchasingDocumentAcceptanceTest(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private PurchasingDocumentAcceptanceDataUtil DataUtil
        //{
        //    get { return (PurchasingDocumentAcceptanceDataUtil)ServiceProvider.GetService(typeof(PurchasingDocumentAcceptanceDataUtil)); }
        //}

        //private PurchasingDocumentExpeditionFacade Facade
        //{
        //    get { return (PurchasingDocumentExpeditionFacade)ServiceProvider.GetService(typeof(PurchasingDocumentExpeditionFacade)); }
        //}

        //[Fact]
        //private async Task Should_Success_Accept_Document_With_Verification_Role()
        //{
        //    PurchasingDocumentAcceptanceViewModel vModel = await DataUtil.GetVerificationNewData();
        //    int AffectedRows = await Facade.PurchasingDocumentAcceptance(vModel, "Unit Test");
        //    Assert.True(AffectedRows > 0);
        //}

        //[Fact]
        //private async Task Should_Success_Accept_Document_With_Cashier_Role()
        //{
        //    PurchasingDocumentAcceptanceViewModel vModel = await DataUtil.GetCashierNewData();
        //    int AffectedRows = await Facade.PurchasingDocumentAcceptance(vModel, "Unit Test");
        //    Assert.True(AffectedRows > 0);
        //}

        /*
        [Fact]
        private async Task Should_Success_Accept_Document_With_Finance_Role()
        {
            PurchasingDocumentAcceptanceViewModel vModel = DataUtil.GetFinanceNewData();
            int AffectedRows = await Facade.PurchasingDocumentAcceptance(vModel, "Unit Test");
            Assert.True(AffectedRows > 0);
        }
        */

        //[Fact]
        //private async Task Should_Success_Delete_Verification_Document()
        //{
        //    PurchasingDocumentExpedition model = await DataUtil.GetVerificationTestData();
        //    int AffectedRows = await Facade.DeletePurchasingDocumentAcceptance(model.Id);
        //    Assert.True(AffectedRows > 0);
        //}

        //[Fact]
        //private async Task Should_Success_Delete_Cashier_Document()
        //{
        //    PurchasingDocumentExpedition model = await DataUtil.GetCashierTestData();
        //    int AffectedRows = await Facade.DeletePurchasingDocumentAcceptance(model.Id);
        //    Assert.True(AffectedRows > 0);
        //}

        /*
        [Fact]
        private async Task Should_Success_Delete_Finance_Document()
        {
            PurchasingDocumentExpedition model = await DataUtil.GetFinanceTestData();
            int AffectedRows = await Facade.DeletePurchasingDocumentAcceptance(model.Id);
            Assert.True(AffectedRows > 0);
        }
        */
    }
}
