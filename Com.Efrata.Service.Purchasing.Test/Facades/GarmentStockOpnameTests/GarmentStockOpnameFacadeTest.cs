using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentStockOpnameDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentStockOpnameTests
{
    public class GarmentStockOpnameFacadeTest
    {
        private const string ENTITY = "GarmentStockOpnameFacadeTest";

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private Mock<IServiceProvider> GetServiceProviderMock()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProviderMock;
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

        private GarmentStockOpnameDataUtil dataUtil(GarmentStockOpnameFacade facade, IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(serviceProvider, dbContext);
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(dbContext);
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(serviceProvider, dbContext);
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(serviceProvider, dbContext);
            var garmentUnitReceiptNoteDataUtil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);

            return new GarmentStockOpnameDataUtil(facade, garmentUnitReceiptNoteDataUtil);
        }

        [Fact]
        public async Task Read_Success()
        {
            var serviceProvider = GetServiceProviderMock().Object;
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentStockOpnameFacade(serviceProvider, dbContext);
            var data = await dataUtil(facade, serviceProvider, dbContext).GetTestData();

            var Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task ReadById_Success()
        {
            var serviceProvider = GetServiceProviderMock().Object;
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentStockOpnameFacade(serviceProvider, dbContext);
            var data = await dataUtil(facade, serviceProvider, dbContext).GetTestData();

            var Response = facade.ReadById(data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Download_Success()
        {
            var serviceProvider = GetServiceProviderMock().Object;
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentStockOpnameFacade(serviceProvider, dbContext);
            var data = await dataUtil(facade, serviceProvider, dbContext).GetNewData();
            var firstData = data.First();

            var Response = facade.Download(DateTimeOffset.Now, firstData.UnitCode, firstData.StorageCode, firstData.StorageName);
            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public async Task Upload_Success()
        {
            var serviceProvider = GetServiceProviderMock().Object;
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentStockOpnameFacade(serviceProvider, dbContext);
            var dataUtil = this.dataUtil(facade, serviceProvider, dbContext);
            var data = await dataUtil.GetNewData();
            var firstData = data.First();
            var excel = dataUtil.GetExcel(data, null, firstData.UnitCode, firstData.StorageCode, firstData.StorageName);

            var Response = await facade.Upload(excel);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task GetLastDataByUnitStorage_Success()
        {
            var serviceProvider = GetServiceProviderMock().Object;
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentStockOpnameFacade(serviceProvider, dbContext);
            var data = await dataUtil(facade, serviceProvider, dbContext).GetTestData();

            var Response = facade.GetLastDataByUnitStorage(data.UnitCode, data.StorageCode);
            Assert.NotEqual(0, Response.Id);
        }
    }
}
