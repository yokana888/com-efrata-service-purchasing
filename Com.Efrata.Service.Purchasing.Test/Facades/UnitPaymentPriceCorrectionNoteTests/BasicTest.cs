using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Com.Efrata.Service.Purchasing.Lib.Enums;

namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitPaymentPriceCorrectionNoteTests
{
    public class BasicTest
    {
        private const string ENTITY = "UnitPaymentPriceCorrectionNote";

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

        private Mock<IServiceProvider> GetServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(new MemoryCacheManager(memoryCache));

            var opts = Options.Create(new MemoryDistributedCacheOptions());
            var cache = new MemoryDistributedCache(opts);

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(cache);

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private UnitPaymentPriceCorrectionNoteDataUtils _dataUtil(UnitPaymentPriceCorrectionNoteFacade facade, string testName, IServiceProvider serviceProvider)
        {
            
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider, _dbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(_dbContext(testName), serviceProvider);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(serviceProvider, _dbContext(testName));
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            UnitPaymentOrderFacade unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, _dbContext(testName));
            UnitPaymentOrderDataUtil unitPaymentOrderDataUtil = new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, unitPaymentOrderFacade);

            return new UnitPaymentPriceCorrectionNoteDataUtils(unitPaymentOrderDataUtil, facade);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
           try
            {
                var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
                UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
                var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetNewData();
                var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
                Assert.NotEqual(0, ResponseLocalSupplier);

                var modelImportSupplier = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetNewData();
                var ResponseImportSupplier = await facade.Create(modelImportSupplier, true, USERNAME, 7);
                Assert.NotEqual(0, ResponseImportSupplier);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [Fact]
        public async Task Should_Error_Create_Data_Null_Parameter()
        {
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            
            Exception exception = await Assert.ThrowsAsync<Exception>(() => facade.Create(null, true, USERNAME, 7));
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }

        [Fact]
        public async Task Should_Success_Create_Data_garment()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetNewData();
            modelLocalSupplier.DivisionName = "EFRATA";
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
            Assert.NotEqual(0, ResponseLocalSupplier);
        }

        [Fact]
        public async Task Should_Success_Get_Data_Supplier()
        {

            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetNewData();
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
            var Response = facade.GetSupplier(modelLocalSupplier.SupplierId);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Error_Get_Data_URN()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetNewData();
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, false, USERNAME, 7);
            //var id = 0;
            var items = modelLocalSupplier.Items.ToList();
            var Response = facade.GetUrn(items[0].URNNo);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();
            var Response = facade.GetReport(null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel_Null_Parameter()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();
            var Response = facade.GenerateExcel(null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();

            var Response = facade.GenerateDataExcel(null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel_Not_Found()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            UnitPaymentPriceCorrectionNoteFacade facade = new UnitPaymentPriceCorrectionNoteFacade(serviceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod(), serviceProvider).GetTestData();

            var Response = facade.GenerateDataExcel(DateTime.MinValue, DateTime.MinValue, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }
    }
}
