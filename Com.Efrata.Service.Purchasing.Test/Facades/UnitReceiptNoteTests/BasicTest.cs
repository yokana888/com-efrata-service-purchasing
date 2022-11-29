using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitReceiptNoteTests
{
    public class BasicTest
    {
        private const string ENTITY = "UnitReceiptNote";

        private const string USERNAME = "Unit Test";
        //private IServiceProvider ServiceProvider { get; set; }

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
            //var cache = new Mock<IMemoryCache>();
            //cache.Setup(x => x.GetOrCreate<List<IdCOAResult>>());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();

            //var memoryCache = serviceProviders.GetService<IMemoryCache>();
            //var mockMemoryCache = new Mock<IMemoryCacheManager>();
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Categories, It.IsAny<Func<ICacheEntry, List<CategoryCOAResult>>>()))
            //    .Returns(new List<CategoryCOAResult>() { new CategoryCOAResult() {
            //    Code = "BB"
            //    } });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Units, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
            //   .Returns(new List<BankAccountCOAResult>() { });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
            //   .Returns(new List<IncomeTaxCOAResult>() { });
            //serviceProvider
            //    .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
            //    .Returns(mockMemoryCache.Object);

            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Categories)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Units)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Divisions)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<CategoryCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.IncomeTaxes)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IncomeTaxCOAResult>())));
            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(mockDistributedCache.Object);

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private Mock<IServiceProvider> GetServiceProviderCurrencyNotNull(string testname)
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

            //var cache = new Mock<IMemoryCache>();
            //cache.Setup(x => x.GetOrCreate<List<IdCOAResult>>());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();

            //var memoryCache = serviceProviders.GetService<IMemoryCache>();
            //var mockMemoryCache = new Mock<IMemoryCacheManager>();
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Categories, It.IsAny<Func<ICacheEntry, List<CategoryCOAResult>>>()))
            //    .Returns(new List<CategoryCOAResult>() { new CategoryCOAResult() {
            //    Code = "BB"
            //    } });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Units, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
            //   .Returns(new List<BankAccountCOAResult>() { });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
            //   .Returns(new List<IncomeTaxCOAResult>() { });
            //serviceProvider
            //    .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
            //    .Returns(mockMemoryCache.Object);

            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Categories)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Units)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Divisions)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<CategoryCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.IncomeTaxes)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IncomeTaxCOAResult>())));
            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(mockDistributedCache.Object);

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider.Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync(new Currency()
                {
                    Code = "CurrencyCode",
                    Date = DateTime.UtcNow,
                    Rate = 100000000
                });
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private Mock<IServiceProvider> GetServiceProvider2(string testname)
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

            //var cache = new Mock<IMemoryCache>();
            //cache.Setup(x => x.GetOrCreate<List<IdCOAResult>>());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();

            //var memoryCache = serviceProviders.GetService<IMemoryCache>();
            //var mockMemoryCache = new Mock<IMemoryCacheManager>();
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Categories, It.IsAny<Func<ICacheEntry, List<CategoryCOAResult>>>()))
            //    .Returns(new List<CategoryCOAResult>() { new CategoryCOAResult() {
            //    Code = "BB"
            //    } });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Units, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
            //   .Returns(new List<IdCOAResult>() { new IdCOAResult() });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
            //   .Returns(new List<BankAccountCOAResult>() { });
            //mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
            //   .Returns(new List<IncomeTaxCOAResult>() { });
            //serviceProvider
            //    .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
            //    .Returns(mockMemoryCache.Object);

            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Categories)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Units)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IdCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.Divisions)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<CategoryCOAResult>())));
            mockDistributedCache.Setup(s => s.Get(It.Is<string>(i => i == MemoryCacheConstant.IncomeTaxes)))
                .Returns(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new List<IncomeTaxCOAResult>())));
            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(mockDistributedCache.Object);

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            var opts = Options.Create(new MemoryDistributedCacheOptions());
            var cache = new MemoryDistributedCache(opts);

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(cache);

            return serviceProvider;
        }

        private Mock<IServiceProvider> _ServiceProvider(string testname) => GetServiceProvider(testname);

        private UnitReceiptNoteDataUtil _dataUtil(UnitReceiptNoteFacade facade, PurchasingDbContext _DbContext, string testname)
        {
            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(_ServiceProvider(testname).Object, _DbContext);
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(_ServiceProvider(testname).Object, _DbContext);
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(_ServiceProvider(testname).Object, _DbContext);
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(_DbContext, _ServiceProvider(testname).Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(_ServiceProvider(testname).Object, _DbContext);
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            return new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, facade, deliveryOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.ReadById((int)dataUtil.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var model = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetNewData(USERNAME);
            model.IsStorage = true;
            model.UnitId = null;
            var response = await facade.Create(model, USERNAME);

            Assert.NotEqual(0, response);

        }

        [Fact]
        public async Task Should_Success_Create_Data_SupplierIsImport()
        {
            var dbContext = _dbContext(GetCurrentMethod());

            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(GetServiceProviderCurrencyNotNull(GetCurrentMethod()).Object, dbContext);
            var model = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetNewData(USERNAME);
            model.IsStorage = true;
            model.UnitId = null;
            model.SupplierIsImport = true;

            var response = await facade.Create(model, USERNAME);

            Assert.NotEqual(0, response);

        }

        [Fact]
        public async Task Should_Success_CreateNullCOA_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());

            UnitReceiptNoteFacade facade2 = new UnitReceiptNoteFacade(GetServiceProvider2(GetCurrentMethod()).Object, dbContext);
            var model2 = await _dataUtil(facade2, dbContext, GetCurrentMethod()).GetNewData(USERNAME);
            model2.IsStorage = true;
            model2.UnitId = null;
            var response2 = await facade2.Create(model2, USERNAME);
            Assert.NotEqual(0, response2);
        }

        [Fact]
        public async Task Should_Success_Create_Having_Stock_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var model = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetNewHavingStockData(USERNAME);
            model.IsStorage = true;
            var response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, response);
        }

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
        //    var response = await facade.Update((int)dataUtil.Id, dataUtil, dataUtil.CreatedBy);
        //    Assert.NotEqual(0, response);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
        //    var response = await facade.Delete((int)dataUtil.Id, dataUtil.CreatedBy);
        //    Assert.NotEqual(0, response);
        //}

        [Fact]
        public void Should_Success_Read_DataBySupplier()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var filter = JsonConvert.SerializeObject(new
            {
                DivisionId = "1",
                SupplierId = "1",
                PaymentMethod = "test",
                CurrencyCode = "IDR",
                UseIncomeTax = true,
                UseVat = false,
                CategoryId = "1"
            });

            var response = facade.ReadBySupplierUnit(Filter: filter);

            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Should_Success_GetReport()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GetReport(dataUtil.URNNo, "", dataUtil.UnitId,"", dataUtil.SupplierId,"", null, null, 1, 25, "{}", 1);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GenerateExcel(dataUtil.URNNo, "", dataUtil.UnitId, "", dataUtil.SupplierId,"", null, null, 1);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Success_Get_By_No()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.ReadByNoFiltered(1, 25, "{}", null, "{ no : ['" + dataUtil.URNNo + "']}");
            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_GetPurchaseRequestCategoryCode()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);

            var Response = facade.Read();
            Assert.NotEmpty(Response.Data);

            foreach (var data in Response.Data)
            {
                foreach (var item in data.Items)
                {
                    var categoryCode = facade.GetPurchaseRequestCategoryCode(item.PRId);
                    Assert.NotNull(categoryCode);
                }
            }
        }

        [Fact]
        public async Task Should_Success_Get_By_List_Of_No()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GetByListOfNo(new List<string>() { dataUtil.URNNo });
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task Should_Success_Get_Subledger()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = await facade.GetUnitReceiptNoteForSubledger(new List<string>() { dataUtil.URNNo });
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task Should_Success_GetSpbReport()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GetSpbReport(dataUtil.URNNo, "", "", null, null, 25, 1, "{}", 1);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel_Spb()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GenerateExcelSpb(dataUtil.URNNo, "", "", null, null, 1);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Success_Get_Creditor_Account_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            UnitReceiptNoteFacade facade = new UnitReceiptNoteFacade(_ServiceProvider(GetCurrentMethod()).Object, dbContext);
            var dataUtil = await _dataUtil(facade, dbContext, GetCurrentMethod()).GetTestData(USERNAME);
            var response = facade.GetCreditorAccountDataByURNNo(dataUtil.URNNo);
            Assert.NotNull(response);
        }
        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(_dbContext(GetCurrentMethod()));
        //    var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    var modelItem = _dataUtil(facade, GetCurrentMethod()).GetNewData().Items.First();
        //    //model.Items.Clear();
        //    model.Items.Add(modelItem);
        //    var ResponseAdd = await facade.Update((int)model.Id, model, USERNAME);
        //    Assert.NotEqual(ResponseAdd, 0);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(_dbContext(GetCurrentMethod()));
        //    var Data = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    int Deleted = await facade.Delete((int)Data.Id, USERNAME);
        //    Assert.True(Deleted > 0);
        //}

        [Fact]
        public void Should_Success_Validate_Data()
        {
            UnitPaymentOrderViewModel nullViewModel = new UnitPaymentOrderViewModel();
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            UnitPaymentOrderViewModel viewModel = new UnitPaymentOrderViewModel()
            {
                useIncomeTax = true,
                useVat = true,
                items = new List<UnitPaymentOrderItemViewModel>
                {
                    new UnitPaymentOrderItemViewModel(),
                    new UnitPaymentOrderItemViewModel()
                    {
                        unitReceiptNote = new UnitReceiptNote
                        {
                            _id = 1
                        }
                    },
                    new UnitPaymentOrderItemViewModel()
                    {
                        unitReceiptNote = new UnitReceiptNote
                        {
                            _id = 1
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        //[Fact]
        //public async Task Should_Success_Get_Data_Spb()
        //{
        //    UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(_dbContext(GetCurrentMethod()));
        //    await _dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    var Response = facade.ReadSpb();
        //    Assert.NotEqual(Response.Item1.Count, 0);
        //}
    }
}
