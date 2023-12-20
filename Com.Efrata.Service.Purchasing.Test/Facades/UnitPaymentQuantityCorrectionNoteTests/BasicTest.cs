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
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;

namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitPaymentQuantityCorrectionNoteTests
{
    public class BasicTest
    {
        private const string ENTITY = "UnitPaymentQuantityCorrectionNote";

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

        private Mock<IServiceProvider> GetServiceProvider(string testname, PurchasingDbContext dbContext)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, dbContext));

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();

            var opts = Options.Create(new MemoryDistributedCacheOptions());
            var cache = new MemoryDistributedCache(opts);

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(cache);

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

            return serviceProvider;
        }

        private UnitPaymentCorrectionNoteDataUtil _dataUtil(UnitPaymentQuantityCorrectionNoteFacade facade, PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider, dbContext);
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider, dbContext);
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider, dbContext);
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, serviceProvider);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(serviceProvider, dbContext);
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            UnitPaymentOrderFacade unitPaymentOrderFacade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
            UnitPaymentOrderDataUtil unitPaymentOrderDataUtil = new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, unitPaymentOrderFacade);

            return new UnitPaymentCorrectionNoteDataUtil(unitPaymentOrderDataUtil, facade, unitReceiptNoteFacade);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var model = await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Error_Get_Null_Data_UnitReceiptNote()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var model = await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.ReadByURNNo((string)model.UPCNo);
            Assert.Null(Response);
        }

        //[Fact]
        //public async Task Should_Success_Get_Null_Data_UnitReceiptNote()
        //{
        //    var serviceProvider = new Mock<IServiceProvider>();
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider.Object, dbContext);
        //    var model = await _dataUtil(facade, dbContext).GetTestDataUrn();
        //    var Response = facade.ReadByURNNo((string)model.URNNo);
        //    Assert.NotNull(Response);
        //}

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var modelLocalSupplier = await _dataUtil(facade, dbContext, serviceProvider).GetNewData();
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseLocalSupplier);

            var modelImportSupplier = await _dataUtil(facade, dbContext, serviceProvider).GetNewData();
            var ResponseImportSupplier = await facade.Create(modelImportSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        [Fact]
        public async Task Should_Success_Create_Data_garment()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var modelLocalSupplier = await _dataUtil(facade, dbContext, serviceProvider).GetNewData();
            modelLocalSupplier.DivisionName = "EFRATA";
            var ResponseImportSupplier = await facade.Create(modelLocalSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        [Fact]
        public async Task Should_Success_Create_Data_when_Supplier_IsNull()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var modelLocalSupplier = await _dataUtil(facade, dbContext, serviceProvider).GetNewData();
            modelLocalSupplier.SupplierId = null;
            var ResponseImportSupplier = await facade.Create(modelLocalSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        [Fact]
        public async Task Should_Success_Create_Data_when_Supplier_Is_not_Null()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var modelLocalSupplier = await _dataUtil(facade, dbContext, serviceProvider).GetNewData();
            modelLocalSupplier.SupplierId = "670";
            var ResponseImportSupplier = await facade.Create(modelLocalSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        [Fact]
        public async Task Should_Error_Create_Data_Null_Parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);

            Exception exception = await Assert.ThrowsAsync<Exception>(() => facade.Create(null, USERNAME, 7));
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }


        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
        //    UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(serviceProvider, dbContext);
        //    var model = await _dataUtil(facade, dbContext, serviceProvider).GetTestData();

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
            UnitPaymentCorrectionNoteViewModel nullViewModel = new UnitPaymentCorrectionNoteViewModel();
            nullViewModel.items = new List<UnitPaymentCorrectionNoteItemViewModel> { };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            UnitPaymentCorrectionNoteViewModel viewModel = new UnitPaymentCorrectionNoteViewModel()
            {
                useIncomeTax = true,
                useVat = true,
                items = new List<UnitPaymentCorrectionNoteItemViewModel>
                {
                    new UnitPaymentCorrectionNoteItemViewModel()
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_2()
        {
            UnitPaymentCorrectionNoteViewModel nullViewModel = new UnitPaymentCorrectionNoteViewModel();
            nullViewModel.items = new List<UnitPaymentCorrectionNoteItemViewModel> { };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            UnitPaymentCorrectionNoteViewModel viewModel = new UnitPaymentCorrectionNoteViewModel()
            {
                correctionType = "Jumlah",
                useIncomeTax = true,
                useVat = true,
                items = new List<UnitPaymentCorrectionNoteItemViewModel>
                {
                    new UnitPaymentCorrectionNoteItemViewModel()
                    {
                        quantity = 1,
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_3()
        {
            UnitPaymentCorrectionNoteViewModel nullViewModel = new UnitPaymentCorrectionNoteViewModel();
            nullViewModel.items = new List<UnitPaymentCorrectionNoteItemViewModel> { };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            UnitPaymentCorrectionNoteViewModel viewModel = new UnitPaymentCorrectionNoteViewModel()
            {
                correctionType = "Jumlah",
                useIncomeTax = true,
                useVat = true,
                items = new List<UnitPaymentCorrectionNoteItemViewModel>
                {
                    new UnitPaymentCorrectionNoteItemViewModel()
                    {
                        quantity = -1,
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_Correction_State()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            var facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            var data = await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var response = facade.GetCorrectionStateByUnitPaymentOrderId((int)data.UPOId);
            Assert.NotNull(response);
        }

        #region Monitoring Koreksi Jumlah 
        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.GetReport(DateTime.MinValue, DateTime.MaxValue, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Generate_Excel()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.GenerateExcel(DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Generate_Excel_Null_parameter()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var serviceProvider = GetServiceProvider(GetCurrentMethod(), dbContext).Object;
            UnitPaymentQuantityCorrectionNoteFacade facade = new UnitPaymentQuantityCorrectionNoteFacade(serviceProvider, dbContext);
            await _dataUtil(facade, dbContext, serviceProvider).GetTestData();
            var Response = facade.GenerateExcel(null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }


        #endregion

    }
}
