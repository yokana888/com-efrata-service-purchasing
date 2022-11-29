using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.InternalPurchaseOrderTests
{
   
    public class POMonitoringAllTest
    {

        private const string ENTITY = "UnitPaymentPriceCorrectionNote";

        private const string USERNAME = "Unit Test";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private UnitPaymentOrderDataUtil _dataUtil2(UnitPaymentOrderFacade facade, string testName)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(_dbContext(testName), GetServiceProvider(testName).Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            return new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, facade);
        }

        private Mock<IServiceProvider> GetServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(new MemoryCacheManager(memoryCache));

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));
            return serviceProvider;
        }

        private UnitPaymentPriceCorrectionNoteDataUtils _dataUtil(UnitPaymentPriceCorrectionNoteFacade facade, string testName)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(_dbContext(testName), GetServiceProvider(testName).Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            UnitPaymentOrderFacade unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            UnitPaymentOrderDataUtil unitPaymentOrderDataUtil = new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, unitPaymentOrderFacade);

            return new UnitPaymentPriceCorrectionNoteDataUtils(unitPaymentOrderDataUtil, facade);
        }

        private DeliveryOrderDataUtil _dataUtilDO(DeliveryOrderFacade facade, string testName)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            
            return new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, facade);
        }

        private InternalPurchaseOrderDataUtil _dataUtilIPO(InternalPurchaseOrderFacade facade, string testName)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);


            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();

            return new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, facade, purchaseRequestDataUtil);
        }

        private ExternalPurchaseOrderDataUtil _dataUtilEPO(ExternalPurchaseOrderFacade facade, string testName)
        {
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);

            return new ExternalPurchaseOrderDataUtil(facade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

        }




        private PurchasingDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        //private PurchaseOrderMonitoringAllFacade Facade
        //{
        //    get { return (PurchaseOrderMonitoringAllFacade)ServiceProvider.GetService(typeof(PurchaseOrderMonitoringAllFacade)); }
        //}

        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrderFacade epofacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder model = await _dataUtilEPO(epofacade, GetCurrentMethod()).GetTestData("Unit Test");
            //InternalPurchaseOrderFacade ipoFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //InternalPurchaseOrder model = await _dataUtilIPO(ipoFacade, GetCurrentMethod()).GetTestData("Unit test");

            var Response = facade.GetReport(null, null, null, null, null, null, model.EPONo, null, null, null, null, null, null, 1, 25, "{}", 7, "");
            Assert.NotNull(Response);
        }

        //[Fact]
        //public async Task Should_Success_Get_Report_Data2()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    PurchaseOrderMonitoringAllFacade monitoringFacade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
        //    var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
        //    var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
        //    var prNo = "";
        //    foreach (var item in modelLocalSupplier.Items)
        //    {
        //        prNo = item.PRNo;
        //    }

        //    var Response = monitoringFacade.GetReport(prNo, null, null, null, null, null, null, modelLocalSupplier.CreatedBy, null, null, null, null, null, 1, 25, "{}", 7, "");
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_All_Null_Parameter()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    PurchaseOrderMonitoringAllFacade monitoringFacade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);

        //    UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
        //    var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
        //    var today = DateTime.Now;
        //    var tomorrow = today.AddDays(1);
        //    var Response = monitoringFacade.GetReport(null, null, null, null, null, null, null, null, null, null, null, null, tomorrow.ToShortDateString(), 1, 25, "{}", 7, "");

        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Null_Parameter()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    UnitPaymentPriceCorrectionNoteFacade facadeUPO = new UnitPaymentPriceCorrectionNoteFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
        //    var modelLocalSupplier = await _dataUtil(facadeUPO, GetCurrentMethod()).GetNewData();
        //    var ResponseLocalSupplier = await facadeUPO.Create(modelLocalSupplier, false, USERNAME, 7);
        //    //var exter = await externalPurchaseOrderDataUtil.GetTestData("Unit test");
        //    var Response = facade.GetReport("", "0", null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}", 7, null);
        //    Assert.NotEqual(Response.Item2, -1);
        //    //Assert.NotNull(Response.Item1);
        //}

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrderFacade epofacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder model = await _dataUtilEPO(epofacade, GetCurrentMethod()).GetTestData("Unit Test");


            //InternalPurchaseOrderFacade ipoFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //var model = await _dataUtilIPO(ipoFacade, GetCurrentMethod()).GetTestData("Unit test");
            ////ExternalPurchaseOrder modelexternal = 
            var Response = facade.GenerateExcel(null, null, null, null, null, null, model.EPONo, null, null, null, null, null, null, 7, "");
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Excel_Null_Parameter()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    //PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    //PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider("Unit test").Object, _dbContext("Unit test"));
        //    //PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
        //    //PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

        //    //InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider("Unit test").Object, _dbContext("Unit test"));
        //    //InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
        //    //InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

        //    //ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider("Unit test").Object, _dbContext("Unit test"));
        //    //ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
        //    //ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
        //    //ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

        //    //InternalPurchaseOrder d = await internalPurchaseOrderDataUtil.GetNewData("Unit test");
        //    //await internalPurchaseOrderFacade.Create(d, "Unit test");
        //    //var model = await externalPurchaseOrderDataUtil.GetNewData("Unit test", d);
        //    //await externalPurchaseOrderFacade.Create(model, "Unit test", 7);
        //    UnitPaymentPriceCorrectionNoteFacade facadeUPO = new UnitPaymentPriceCorrectionNoteFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
        //    var modelLocalSupplier = await _dataUtil(facadeUPO, GetCurrentMethod()).GetNewData();
        //    var ResponseLocalSupplier = await facadeUPO.Create(modelLocalSupplier, false, USERNAME, 7);
        //    //InternalPurchaseOrderFacade ipoFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    //InternalPurchaseOrder model = await _dataUtilIPO(ipoFacade, GetCurrentMethod()).GetTestData("Unit test");
        //    var Response = facade.GenerateExcel("", "0", null, null, null, null, null, null, null, null, null, null, null, 7, "");
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}


        #region Staff Sarmut 
        [Fact]
        public async void Should_Success_Get_Report_Data_Staffs()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GetReportStaff(DateTime.MinValue, DateTime.MaxValue, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Staffs_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GetReportStaff(null, null, null, 0);
            Assert.NotNull(Response);
        }


        [Fact]
        public async void Should_Success_Get_Report_Data_SubStaffs()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GetReportsubStaff(DateTime.MinValue, DateTime.MaxValue, null, model.CreatedBy, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_SubStaffs_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GetReportsubStaff(null, null, null, null, 0);
            Assert.NotNull(Response);
        }


        [Fact]
        public async void Should_Success_Get_Report_Data_Excel_subStaffs()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GenerateExcelSarmut(DateTime.MinValue, DateTime.MaxValue, null, model.CreatedBy, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async void Should_Success_Get_Report_Data_Excel_subStaffs_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            PurchaseOrderMonitoringAllFacade facade = new PurchaseOrderMonitoringAllFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade doFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            var model = await _dataUtilDO(doFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = facade.GenerateExcelSarmut(null, null, null, null, 0);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        #endregion
    }
}
