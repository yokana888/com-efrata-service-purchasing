using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ReportTest
{
    //[Collection("ServiceProviderFixture Collection")]
    public class ImportPurchasingBookReportTest
    {
        private const string ENTITY = "ImportPurchasingReport";

        private const string USERNAME = "Unit Test";

        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
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

        private UnitPaymentOrderDataUtil _dataUtil(UnitPaymentOrderFacade facade, PurchasingDbContext dbContext, string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var distributionCache = new Mock<IDistributedCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(distributionCache.Object);

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
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCodeDate(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);




            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider.Object, dbContext);
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider.Object, dbContext);
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider.Object, dbContext);
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, serviceProvider.Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(serviceProvider.Object, dbContext);
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            return new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, facade);
        }

        private Mock<IServiceProvider> _getServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();

            var mockIDistributeCache = new Mock<IDistributedCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { TimezoneOffset = 7, Token = "test", Username = "UnitTest" });

            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCodeDateList(It.IsAny<IEnumerable<Tuple<string, DateTimeOffset>>>()))
                .ReturnsAsync(new List<Currency>());

            mockCurrencyProvider
                .Setup(x => x.GetUnitsIdsByAccountingUnitId(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            mockCurrencyProvider
                .Setup(x => x.GetCategoryIdsByAccountingCategoryId(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            mockCurrencyProvider
                .Setup(x => x.GetUnitsByUnitIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Unit>());

            mockCurrencyProvider
                .Setup(x => x.GetAccountingUnitsByUnitIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<AccountingUnit>());

            mockCurrencyProvider
                .Setup(x => x.GetCategoriesByCategoryIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Category>());

            mockCurrencyProvider
                .Setup(x => x.GetAccountingCategoriesByCategoryIds(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<AccountingCategory>());

            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(mockIDistributeCache.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            return serviceProvider;
        }

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
        //    var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportDataValas();

        //    var urnId = dataUtil.Items.FirstOrDefault().URNId;
        //    var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
        //    var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
        //    var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

        //    var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

        //    var result = await facade.GetReport(urn.URNNo, urn.UnitCode, pr.CategoryCode, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
        //    Assert.NotNull(result.Reports);
        //}

        [Fact]
        public async Task Should_Success_Get_Data_Empty()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

            //var result = await facade.GetReport("Invalid URNNo", urn.UnitCode, pr.CategoryCode, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            var result = await facade.GetReportV2(string.Empty, 0, 0, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), 0);

            Assert.NotNull(result.Reports);
        }

        [Fact]
        public async Task Should_Success_Get_Data_Empty_V2()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

            //var result = await facade.GetReportV2("Invalid URNNo", int.Parse(urn.UnitCode), int.Parse(pr.CategoryCode), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), int.Parse(urn.DivisionId));
            var result = await facade.GetReportV2("Invalid URNNo", 0, 0, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7), 0);

            Assert.NotNull(result.Reports);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel_Data_Empty()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

            var result = await facade.GenerateExcel(urn.URNNo, 0, 0, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7),0);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Import_Purchasing()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

            //var result = await facade.GetReport("Invalid URNNo", urn.UnitCode, pr.CategoryCode, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            var result = await facade.GetReportDataImportPurchasing("", 1, 1, DateTime.Now.AddDays(-7), null, 1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Import_Purchasing_Correction()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = _getServiceProvider(GetCurrentMethod()).Object;

            var unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            var dataUtil = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetTestImportData();

            var urnId = dataUtil.Items.FirstOrDefault().URNId;
            var urn = dbContext.UnitReceiptNotes.FirstOrDefault(f => f.Id.Equals(urnId));
            var prId = urn.Items.FirstOrDefault(f => f.URNId.Equals(urn.Id)).PRId;
            var pr = dbContext.PurchaseRequests.FirstOrDefault(f => f.Id.Equals(prId));

            var facade = new ImportPurchasingBookReportFacade(serviceProvider, dbContext);

            //var result = await facade.GetReport("Invalid URNNo", urn.UnitCode, pr.CategoryCode, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7));
            var result = await facade.GetReportDataImportPurchasingCorrection("", 1, 1, DateTime.Now.AddDays(-7), null, 1);

            Assert.NotNull(result);
        }
    }
}
