using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitReceiptNoteTests
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

        private UnitReceiptNoteDataUtil DataUtil
        {
            get { return (UnitReceiptNoteDataUtil)ServiceProvider.GetService(typeof(UnitReceiptNoteDataUtil)); }
        }

        private UnitReceiptNoteFacade Facade
        {
            get { return (UnitReceiptNoteFacade)ServiceProvider.GetService(typeof(UnitReceiptNoteFacade)); }
        }

        private UnitReceiptNoteGenerateDataFacade FacadeGenerateData
        {
            get { return (UnitReceiptNoteGenerateDataFacade)ServiceProvider.GetService(typeof(UnitReceiptNoteGenerateDataFacade)); }
        }
     
        //[Fact]
        //public async Task Should_Success_Get_Report_Generate_Data_Excel()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("Unit test");
        //    var Response = FacadeGenerateData.GenerateExcel(null, null, 7);
        //    Assert.IsType(typeof(System.IO.MemoryStream), Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Generate_Data_Excel_Not_Found()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("Unit test");
        //    var Response = FacadeGenerateData.GenerateExcel(DateTime.MinValue, DateTime.MinValue, 7);
        //    Assert.IsType(typeof(System.IO.MemoryStream), Response);
        //}

    }
}
