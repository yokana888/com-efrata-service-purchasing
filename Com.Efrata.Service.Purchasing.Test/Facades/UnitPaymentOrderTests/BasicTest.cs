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
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitPaymentOrderTests
{
    public class BasicTest
    {
        private const string ENTITY = "UnitPaymentOrder";

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

            //var memoryCacheService = serviceProviders.GetService<IMemoryCacheManager>();
            //memoryCacheService.Set(MemoryCacheConstant.Categories, new List<CategoryCOAResult>() { new CategoryCOAResult() { _id = 1 } });

            var opts = Options.Create(new MemoryDistributedCacheOptions());
            var cache = new MemoryDistributedCache(opts);

            serviceProvider
                .Setup(x => x.GetService(typeof(IDistributedCache)))
                .Returns(cache);

            var memoryCacheManager = new MemoryCacheManager(memoryCache);
            memoryCacheManager.Set(MemoryCacheConstant.Categories, new List<CategoryCOAResult>() { new CategoryCOAResult() { Id = 1 } });
            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(memoryCacheManager);

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private UnitPaymentOrderDataUtil _dataUtil(UnitPaymentOrderFacade facade, string testName)
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

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_EPONo()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var i in model.Items)
            {
                foreach (var d in i.Details)
                {
                    d.EPONo = "EPONo";
                }
            }
            await facade.Create(model, "Unit Test", false);
            var Response = facade.ReadByEPONo("EPONo");
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {

            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, USERNAME, false);
            Assert.NotEqual(0, ResponseLocalSupplier);

            var modelImportSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            //modelImportSupplier.UseVat = true;
            var ResponseImportSupplier = await facade.Create(modelImportSupplier, USERNAME, true);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            var datautil = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var modelItem = datautil.Items.First();
            //model.Items.Clear();
            model.Items.Add(modelItem);
            var ResponseAdd = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAdd);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var Data = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            int Deleted = await facade.Delete((int)Data.Id, USERNAME);
            Assert.True(Deleted > 0);
        }

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

        [Fact]
        public async Task Should_Success_Get_Data_Spb()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadSpb();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_SpbForVerification()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadSpbForVerification();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Position()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadPositionFiltered(1, 25, "{}", null, "{position : [1,6]}");
            Assert.NotEmpty(Response.Item1);
        }
        #region Monitoring All 
        [Fact]
        public async Task Should_Success_Get_Report_All()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportAll(null, model.SupplierId,model.UPONo, DateTime.MinValue, DateTime.MaxValue, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Report_All_Null_Parameter()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportAll("", "","", DateTime.MinValue, DateTime.MaxValue, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Generate_Data_Excel()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcel(null, model.SupplierId, model.UPONo, DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Generate_Data_Excel_Null_Parameter()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcel("", "","", DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        #endregion

        [Fact]
        public async Task Should_Success_Get_Generate_Data_Excel1()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facade.GenerateDataExcel(null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Generate_Data_Excel1_Not_Found()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facade.GenerateDataExcel(DateTime.MinValue, DateTime.MinValue, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Create_DataWithVAT()
        {
            var serviceProvider = GetServiceProvider(GetCurrentMethod()).Object;
            var memoryCacheService = serviceProvider.GetService<IMemoryCacheManager>();
            memoryCacheService.Set(MemoryCacheConstant.Categories, new List<CategoryCOAResult>() { new CategoryCOAResult() { Id = 1 } });

            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));

            var modelImportSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            modelImportSupplier.UseVat = true;
            var ResponseImportSupplier = await facade.Create(modelImportSupplier, USERNAME, true);
            Assert.NotEqual(0, ResponseImportSupplier);
        }

        #region Monitoring Tax All 
        [Fact]
        public async Task Should_Success_Get_Tax_Report_All()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportTax(model.SupplierId, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Tax_Report_All_Null_Parameter()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetReportTax("", "", DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, 1, 25, "{}", 7);
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Tex_Generate_Data_Excel()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelTax(model.SupplierId, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Tax_Generate_Data_Excel_Null_Parameter()
        {
            UnitPaymentOrderFacade facade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcelTax("", "", DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }
        #endregion

    }
}
