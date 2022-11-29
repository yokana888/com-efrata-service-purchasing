using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.DeliveryOrderTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class MonitoringTest
    {
        private IServiceProvider ServiceProvider { get; set; }

        public MonitoringTest(ServiceProviderFixture fixture)
        {
            ServiceProvider = fixture.ServiceProvider;

            IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
            identityService.Username = "Unit Test";
        }

        private DeliveryOrderDataUtil DataUtil
        {
            get { return (DeliveryOrderDataUtil)ServiceProvider.GetService(typeof(DeliveryOrderDataUtil)); }
        }

        private DeliveryOrderFacade Facade
        {
            get { return (DeliveryOrderFacade)ServiceProvider.GetService(typeof(DeliveryOrderFacade)); }
        }

        public async Task Should_Success_Get_Report_Data()
        {
            DeliveryOrder model = await DataUtil.GetTestData("Unit test");
            var Response = Facade.GetReport( model.DONo, model.SupplierId, null, null, 1, 25, "{}", 7);
            Assert.NotEqual(0, Response.Item2);
        }

        
    }
}
