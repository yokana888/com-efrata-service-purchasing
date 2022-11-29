using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentUnitReceiptNoteFacadeTests
{
	public class GarmentDOItemFacadeTest
	{
        private const string ENTITY = "GarmentDOItem";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        private IServiceProvider GetServiceProvider()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("delivery-returns"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("delivery-returns")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });


            var mapper = new Mock<IMapper>();
            mapper
                .Setup(x => x.Map<GarmentUnitReceiptNoteViewModel>(It.IsAny<GarmentUnitReceiptNote>()))
                .Returns(new GarmentUnitReceiptNoteViewModel
                {
                    Id = 1,
                    DOId = 1,
                    DOCurrency = new CurrencyViewModel(),
                    Supplier = new SupplierViewModel(),
                    Unit = new UnitViewModel(),
                    Items = new List<GarmentUnitReceiptNoteItemViewModel>
                    {
                        new GarmentUnitReceiptNoteItemViewModel {
                            Product = new GarmentProductViewModel(),
                            Uom = new UomViewModel()
                        }
                    }
                });

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProviderMock.Object;
        }

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

        private GarmentUnitReceiptNoteDataUtil dataUtil(GarmentUnitReceiptNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentUnitReceiptNoteDataUtil(facade, garmentDeliveryOrderDataUtil, null);
        }

        [Fact]
		public async Task TestModel()
		{
			GarmentDOItems dOItems = new GarmentDOItems { UId = "aaa",DOCurrencyRate=1,DesignColor="aa",DetailReferenceId=1,DOItemNo="sdoitem",EPOItemId=2,POId=1,POItemId=2,POSerialNumber="pp",PRItemId=1,ProductCode="ss",ProductId=2,ProductName="name",RemainingQuantity=100,SmallQuantity=100,SmallUomId=2,SmallUomUnit="pcs",StorageCode="ss",StorageName="ss",StorageId=3,RO="@2",UnitCode="s",UnitId=4,UnitName="s",URNItemId=4 };
			Assert.NotNull(dOItems.UId);
		}

        [Fact]
        public async Task ReadForUnitDO_Success()
        {
            var receiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(receiptNoteFacade, GetCurrentMethod()).GetTestDataWithStorage();

            GarmentDOItemFacade facade = new GarmentDOItemFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var Response = facade.ReadForUnitDO(null, "{'UnitId': " + data.UnitId + ", 'StorageId': " + data.StorageId + ", 'RONo': '" + data.Items.First().RONo + "'}");
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task ReadForUnitDOMore_Success()
        {
            var receiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(receiptNoteFacade, GetCurrentMethod()).GetTestDataWithStorage();

            GarmentDOItemFacade facade = new GarmentDOItemFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var Response = facade.ReadForUnitDOMore(null, "{'UnitId': " + data.UnitId + ", 'StorageId': " + data.StorageId + ", 'RONo': 'aaa'}");
            Assert.NotEmpty(Response);
        }
    }
}
