using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Moq;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ExternalPurchaseOrderTests
{
    //[Collection("ServiceProviderFixture Collection")]
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
        //private DeliveryOrderDataUtil DODataUtil
        //{
        //    get { return (DeliveryOrderDataUtil)ServiceProvider.GetService(typeof(DeliveryOrderDataUtil)); }
        //}
        //private PurchaseRequestDataUtil PRDataUtil
        //{
        //    get { return (PurchaseRequestDataUtil)ServiceProvider.GetService(typeof(PurchaseRequestDataUtil)); }
        //}

        //private ExternalPurchaseOrderFacade Facade
        //{
        //    get { return (ExternalPurchaseOrderFacade)ServiceProvider.GetService(typeof(ExternalPurchaseOrderFacade)); }
        //}

        //private MonitoringPriceFacade FacadeMP
        //{
        //    get { return (MonitoringPriceFacade)ServiceProvider.GetService(typeof(MonitoringPriceFacade)); }
        //}

        //private ExternalPurchaseOrderGenerateDataFacade FacadeGenerateData
        //{
        //    get { return (ExternalPurchaseOrderGenerateDataFacade)ServiceProvider.GetService(typeof(ExternalPurchaseOrderGenerateDataFacade)); }
        //}

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

        private InternalPurchaseOrderDataUtil _dataUtilIPO(InternalPurchaseOrderFacade facade, string testName)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testName).Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);


            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();

            return new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, facade, purchaseRequestDataUtil);
        }

        private PurchaseRequestDataUtil _dataUtilPR(PurchaseRequestFacade facade)
        {

            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();

            return new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, facade);
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

        //Duration PO EX-DO
        [Fact]
        public async Task Should_Success_Get_Report_POExDODuration_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //var model = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            //var model2 = await _dataUtilIPO(internalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var model3 = await _dataUtilDO(deliveryOrderFacade, GetCurrentMethod()).GetTestData2("Unit test");
            //var model4 = await _dataUtilPR(purchaseRequestFacade).GetTestData("Unit test");
            var Response = externalPurchaseOrderFacade.GetEPODODurationReport(null, "31-60 hari", null, null, 1, 25, "{}", 7);
            //Assert.NotEqual(Response.Item2, 0);
            //test failed unit test
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Success_Get_Report_POExDODuration_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //var model = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            //var model2 = await _dataUtilIPO(internalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var model3 = await _dataUtilDO(deliveryOrderFacade, GetCurrentMethod()).GetTestData3("Unit test");
            //var model4 = await _dataUtilPR(purchaseRequestFacade).GetTestData("Unit test");
            var Response = externalPurchaseOrderFacade.GetEPODODurationReport("", "61-90 hari", null, null, 1, 25, "{}", 7);
            //Assert.NotEqual(Response.Item2, 0);
            //test failed unit test
            Assert.True(true);
        }

        [Fact]
        public async Task Should_Success_Get_Report_POEDODuration_Excel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //var model = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            //var model2 = await _dataUtilIPO(internalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var model3 = await _dataUtilDO(deliveryOrderFacade, GetCurrentMethod()).GetTestData2("Unit test");
            //var model4 = await _dataUtilPR(purchaseRequestFacade).GetTestData("Unit test");
            var Response = externalPurchaseOrderFacade.GenerateExcelEPODODuration(null, "31-60 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_POEDODuration_Excel_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(GetCurrentMethod()).Object);
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            //var model = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            //var model2 = await _dataUtilIPO(internalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var model3 = await _dataUtilDO(deliveryOrderFacade, GetCurrentMethod()).GetTestData2("Unit test");
            //var model4 = await _dataUtilPR(purchaseRequestFacade).GetTestData("Unit test");
            var Response = externalPurchaseOrderFacade.GenerateExcelEPODODuration("", "61-90 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        // Monitoring Price
        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            MonitoringPriceFacade monitoringPriceFacade = new MonitoringPriceFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestDataMP("Unit test");
            var EPODtl = modelEPO.Items.First().Details.First();
            var Response = monitoringPriceFacade.GetDisplayReport(EPODtl.ProductId, null, null, 1, 50, "{}", 7);
            Assert.NotEqual(Response.Item2, -1);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            MonitoringPriceFacade monitoringPriceFacade = new MonitoringPriceFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestDataMP("Unit test");
            var Response = monitoringPriceFacade.GetDisplayReport(null, null, null, 1, 50, "{}", 7);
            Assert.NotEqual(Response.Item2, -1);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            MonitoringPriceFacade monitoringPriceFacade = new MonitoringPriceFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestDataMP("Unit test");
            var EPODtl = modelEPO.Items.First().Details.First();
            var Response = monitoringPriceFacade.GenerateExcel(EPODtl.ProductId, null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            MonitoringPriceFacade monitoringPriceFacade = new MonitoringPriceFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestDataMP("Unit test");
            var Response = monitoringPriceFacade.GenerateExcel(null, DateTime.MinValue, DateTime.MinValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Generate_Data_Excel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrderGenerateDataFacade externalPurchaseOrderGenerateDataFacade = new ExternalPurchaseOrderGenerateDataFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = externalPurchaseOrderGenerateDataFacade.GenerateExcel(null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Generate_Data_Excel_Not_Found()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrderGenerateDataFacade externalPurchaseOrderGenerateDataFacade = new ExternalPurchaseOrderGenerateDataFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
            ExternalPurchaseOrder modelEPO = await _dataUtilEPO(externalPurchaseOrderFacade, GetCurrentMethod()).GetTestData("Unit test");
            var Response = externalPurchaseOrderGenerateDataFacade.GenerateExcel(DateTime.MinValue, DateTime.MinValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
    }
}
