using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Moonlay.NetCore.Lib.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PurchasingDocumentExpeditionTest
{
  //  [Collection("ServiceProviderFixture Collection")]
    public class SendToVerificationTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public SendToVerificationTest(ServiceProviderFixture fixture)
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
        //private async Task Should_Success_Send_To_Verification_Division()
        //{
        //    PurchasingDocumentExpedition model = DataUtil.GetNewData();
        //    int AffectedRows = await Facade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");
        //    Assert.True(AffectedRows > 0);
        //}

        //[Fact]
        //public async Task Should_Error_Send_To_Verification_Division_Same_Keys()
        //{
        //    try
        //    {
        //        PurchasingDocumentExpedition Data = await DataUtil.GetTestData();
        //        Data.Items = new List<PurchasingDocumentExpeditionItem>();
        //        Data.Id = 0;

        //        await this.Facade.SendToVerification(new List<PurchasingDocumentExpedition>() { Data }, "Unit Test");
        //    }
        //    catch (ServiceValidationExeption ex)
        //    {
        //            ValidationResult result = ex.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains("UnitPaymentOrdersCollection", StringComparer.CurrentCultureIgnoreCase));
        //            Assert.NotNull(result);
        //    }
        //}
    }
}
