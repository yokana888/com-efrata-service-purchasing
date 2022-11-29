using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PPHBankExpenditureNoteDataUtil;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PPHBankExpenditureNoteTest
{
    public class BasicTest
    {
        private const string ENTITY = "PPHBankExpenditureNote";
        private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;

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

        private PPHBankExpenditureNoteDataUtil _dataUtil(PPHBankExpenditureNoteFacade facade, string testName)
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
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);


            PurchasingDocumentExpeditionFacade pdeFacade = new PurchasingDocumentExpeditionFacade(serviceProvider.Object, _dbContext(testName));
            SendToVerificationDataUtil stvDataUtil = new SendToVerificationDataUtil(pdeFacade);
            pdaDataUtil = new PurchasingDocumentAcceptanceDataUtil(pdeFacade, stvDataUtil);

            return new PPHBankExpenditureNoteDataUtil(facade, pdaDataUtil);
        }

        //[Fact]
        //public async Task Should_Success_Get_Data()
        //{
        //    var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

        //    var serviceProvider = new Mock<IServiceProvider>();

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(new HttpClientTestService());

        //    var services = new ServiceCollection();
        //    services.AddMemoryCache();
        //    var serviceProviders = services.BuildServiceProvider();
        //    var memoryCache = serviceProviders.GetService<IMemoryCache>();
        //    var mockMemoryCache = new Mock<IMemoryCacheManager>();
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
        //        .Returns(new List<IdCOAResult>());
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
        //       .Returns(new List<BankAccountCOAResult>());
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
        //       .Returns(new List<IncomeTaxCOAResult>());
        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
        //        .Returns(mockMemoryCache.Object);

        //    PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
        //    await _dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    ReadResponse<object> Response = facade.Read();
        //    Assert.NotEmpty(Response.Data);
        //}

        [Fact]
        public async Task Should_Success_Get_Unit_Payment_Order()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            _dataUtil(facade, GetCurrentMethod());
            PurchasingDocumentExpedition model = await pdaDataUtil.GetCashierTestData();

            var Response = facade.GetUnitPaymentOrder(null, null, model.IncomeTaxName, model.IncomeTaxRate, model.Currency, model.DivisionCode);
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Unit_Payment_Order_With_Date()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            _dataUtil(facade, GetCurrentMethod());
            PurchasingDocumentExpedition model = await pdaDataUtil.GetCashierTestData();

            var Response = facade.GetUnitPaymentOrder(model.DueDate, model.DueDate, model.IncomeTaxName, model.IncomeTaxRate, model.Currency, model.DivisionCode);
            Assert.NotEmpty(Response);
        }

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

        //    var serviceProvider = new Mock<IServiceProvider>();

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(new HttpClientTestService());

        //    var services = new ServiceCollection();
        //    services.AddMemoryCache();
        //    var serviceProviders = services.BuildServiceProvider();
        //    var memoryCache = serviceProviders.GetService<IMemoryCache>();
        //    var mockMemoryCache = new Mock<IMemoryCacheManager>();
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
        //        .Returns(new List<IdCOAResult>());
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
        //       .Returns(new List<BankAccountCOAResult>());
        //    mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
        //       .Returns(new List<IncomeTaxCOAResult>());
        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
        //        .Returns(mockMemoryCache.Object);

        //    PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
        //    PPHBankExpenditureNote model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    var Response = facade.ReadById((int)model.Id);
        //    Assert.NotNull(Response);
        //}

        [Fact]
        public void Should_Success_Read()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            _dataUtil(facade, GetCurrentMethod());

            var Response = facade.Read();
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_success_Posting_data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            PPHBankExpenditureNote model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Posting(new List<long> { model.Id });
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);
            serviceProvider
               .Setup(x => x.GetService(typeof(IdentityService)))
               .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            PPHBankExpenditureNote model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(model, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);
            serviceProvider
               .Setup(x => x.GetService(typeof(IdentityService)))
               .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            PPHBankExpenditureNote model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            PPHBankExpenditureNoteItem modelItem = await _dataUtil(facade, GetCurrentMethod()).GetItemNewData();
            model.Items.Clear();
            model.Items.Add(modelItem);
            var Response = await facade.Update((int)model.Id, model, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();
            var mockMemoryCache = new Mock<IMemoryCacheManager>();
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.Divisions, It.IsAny<Func<ICacheEntry, List<IdCOAResult>>>()))
                .Returns(new List<IdCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.BankAccounts, It.IsAny<Func<ICacheEntry, List<BankAccountCOAResult>>>()))
               .Returns(new List<BankAccountCOAResult>());
            mockMemoryCache.Setup(x => x.Get(MemoryCacheConstant.IncomeTaxes, It.IsAny<Func<ICacheEntry, List<IncomeTaxCOAResult>>>()))
               .Returns(new List<IncomeTaxCOAResult>());
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(mockMemoryCache.Object);
            serviceProvider
               .Setup(x => x.GetService(typeof(IdentityService)))
               .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            PPHBankExpenditureNoteFacade facade = new PPHBankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, serviceProvider.Object);
            PPHBankExpenditureNote Data = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            int AffectedRows = await facade.Delete(Data.Id, "Test");
            Assert.True(AffectedRows > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            PPHBankExpenditureNoteViewModel vm = new PPHBankExpenditureNoteViewModel()
            {
                Date = null,
                Bank = null,
                IncomeTax = null,
                PPHBankExpenditureNoteItems = new List<UnitPaymentOrderViewModel>()
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Date_Data()
        {
            PPHBankExpenditureNoteViewModel vm = new PPHBankExpenditureNoteViewModel()
            {
                Date = DateTimeOffset.UtcNow.AddDays(1),
                Bank = null,
                IncomeTax = null,
                PPHBankExpenditureNoteItems = new List<UnitPaymentOrderViewModel>()
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }
    }
}
