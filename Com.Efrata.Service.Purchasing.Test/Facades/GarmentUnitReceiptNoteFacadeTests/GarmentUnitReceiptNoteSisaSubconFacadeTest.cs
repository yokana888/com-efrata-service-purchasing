using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Migrations;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;
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
    public class GarmentUnitReceiptNoteSisaSubconFacadeTest
    {
        private const string ENTITY = "GarmentUnitReceiptNoteSisaSubcon";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        private IServiceProvider GetServiceProvider()
        {
            var httpClientService = new Mock<IHttpClientService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

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

            var mockGarmentUENFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockGarmentUENFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitExpenditureNoteViewModel());

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
                .Setup(x => x.GetService(typeof(IGarmentUnitExpenditureNoteFacade)))
                .Returns(mockGarmentUENFacade.Object);


            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProviderMock.Object;
        }

        private IServiceProvider GetServiceProvider_DOCurrency()
        {
            var httpClientService = new Mock<IHttpClientService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });


            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("delivery-returns"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("delivery-returns")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentDeliveryReturnDataUtil().GetResultFormatterOkString()) });

            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/fabric")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureFabricDataUtil().GetResultFormatterOkString()) });
            httpClientService
               .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("garment/leftover-warehouse-expenditures/accessories")), It.IsAny<HttpContent>()))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil().GetResultFormatterOkString()) });


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

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(testName));
            var garmentUnitReceiptNoteDatautil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil,null);

            var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName), GetServiceProvider());
            var garmentUnitDeliveryOrderDatautil = new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDatautil);

            var garmentUENFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider(), _dbContext(testName));
            var dataUtilUEN = new GarmentUnitExpenditureNoteDataUtil(garmentUENFacade, garmentUnitDeliveryOrderDatautil);

            return new GarmentUnitReceiptNoteDataUtil(facade, garmentDeliveryOrderDataUtil, dataUtilUEN);
        }
        

        private IServiceProvider GetServiceProviderUnitReceiptNote()
        {
            throw new NotImplementedException();
        }

        /*[Fact]
        public async Task Should_Success_Create_Data_SISA_SUBCON()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithStorageSubcon();
            data.URNType = "SISA SUBCON";
            data.ExpenditureId = 1;
            data.ExpenditureNo = "no";
            data.Category = "FABRIC";
            data.UENId = 1;
            data.UENNo = "no";
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);

        }*/

        [Fact]
        public async Task Should_Success_Delete_Data_SISA_SUBCON()
        {
            var facade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataWithStorageSubcon();

            var Response = await facade.Delete((int)data.Id, (string)data.DeletedReason);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Validate_Data()
        {
            GarmentUnitReceiptNoteViewModel viewModel = new GarmentUnitReceiptNoteViewModel
            {
                Unit = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                URNType = "SISA SUBCON"
            };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelNullItems = new GarmentUnitReceiptNoteViewModel
            {
                URNType = "SISA SUBCON"
            };
            Assert.True(viewModelNullItems.Validate(null).Count() > 0);

            GarmentUnitReceiptNoteViewModel viewModelWithItems = new GarmentUnitReceiptNoteViewModel
            {
                URNType = "SISA SUBCON",
                Items = new List<GarmentUnitReceiptNoteItemViewModel>
                {
                    new GarmentUnitReceiptNoteItemViewModel
                    {
                        ReceiptQuantity=0
                    }
                }
            };
            Assert.True(viewModelWithItems.Validate(null).Count() > 0);

        }
    }
}