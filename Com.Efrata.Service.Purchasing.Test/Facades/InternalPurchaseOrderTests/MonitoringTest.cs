using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.InternalPurchaseOrderTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class MonitoringTest
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //public MonitoringTest(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;

        //    IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
        //    identityService.Username = "Unit Test";
        //}

        //private InternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (InternalPurchaseOrderDataUtil)ServiceProvider.GetService(typeof(InternalPurchaseOrderDataUtil)); }
        //}

        //private ExternalPurchaseOrderDataUtil EPODataUtil
        //{
        //    get { return (ExternalPurchaseOrderDataUtil)ServiceProvider.GetService(typeof(ExternalPurchaseOrderDataUtil)); }
        //}

        //private InternalPurchaseOrderFacade Facade
        //{
        //    get { return (InternalPurchaseOrderFacade)ServiceProvider.GetService(typeof(InternalPurchaseOrderFacade)); }
        //}

        //public async Task Should_Success_Get_Report_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReport( model.UnitId, model.CategoryId, model.CreatedBy, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Null_Parameter()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReport("", null, null, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Excel()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GenerateExcel(model.UnitId, model.CategoryId, model.CreatedBy, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Excel_Null_Parameter()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GenerateExcel("", "", "", null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //// Monitoring PO Internal Belum Proses PO Eksternal
        //[Fact]
        //public async Task Should_Success_Get_Report_Data_UnProcessed()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReportUnProcessed(model.UnitId, model.CategoryId, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_UnProcessed_Null_Parameter()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GetReportUnProcessed("", null, null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_UnProcessed_Excel()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GenerateExcelUnProcessed(model.UnitId, model.CategoryId, null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Excel_UnProcessed_Null_Parameter()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("Unit test");
        //    var Response = Facade.GenerateExcelUnProcessed("", "", null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        ////Duration PO In-PO Ex
        //[Fact]
        //public async Task Should_Success_Get_Report_POIPOExDuration_Data()
        //{
        //    var model = await EPODataUtil.GetTestData2("Unit test");
        //    var Response = Facade.GetIPOEPODurationReport(model.UnitId, "8-14 hari", null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_POIPOExDuration_Null_Parameter()
        //{
        //    var model = await EPODataUtil.GetTestData3("Unit test");
        //    var Response = Facade.GetIPOEPODurationReport("", "15-30 hari", null, null, 1, 25, "{}", 7);
        //    Assert.NotEqual(-1, Response.Item2);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_POIPOEDuration_Excel()
        //{
        //    var model = await EPODataUtil.GetTestData2("Unit test");
        //    var Response = Facade.GenerateExcelIPOEPODuration(model.UnitId, "8-14 hari", null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_POIPOEDuration_Excel_Null_Parameter()
        //{
        //    var model = await EPODataUtil.GetTestData3("Unit test");
        //    var Response = Facade.GenerateExcelIPOEPODuration("", "15-30 hari", null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
    }
}
