using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
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

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentCorrectionNotePriceTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentCorrectionNotePrice";

        private IServiceProvider GetServiceProvider()
        {
            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            return serviceProviderMock.Object;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
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

        private GarmentCorrectionNoteDataUtil dataUtil(GarmentCorrectionNotePriceFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider(), _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentCorrectionNoteDataUtil(facade, garmentDeliveryOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Koreksi_Harga_Satuan()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiHargaSatuan();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Koreksi_Harga_Total()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiHargaTotal();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id_Koreksi_Harga_Satuan()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiHargaSatuan();
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id_Koreksi_Harga_Total()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiHargaTotal();
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautil = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var data = datautil.GarmentCorrectionNote;
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_With_Tax()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);

            var data2nd = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
            var Response2nd = await facade.Create(data2nd);
            Assert.NotEqual(0, Response2nd);
        }

        [Fact]
        public async Task Should_Error_Create_Data_Null_Items()
        {
            var facade = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), _dbContext(GetCurrentMethod()));
            var datautil = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var data = datautil.GarmentCorrectionNote;
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Error_Create_Data_Failed_Get_Supplier()
        {
            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("") });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var facade = new GarmentCorrectionNotePriceFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));
            var datautil = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var data = datautil.GarmentCorrectionNote;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentCorrectionNoteViewModel AllNullViewModel = new GarmentCorrectionNoteViewModel();
            Assert.True(AllNullViewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Null_Items()
        {
            GarmentCorrectionNoteViewModel viewModel = new GarmentCorrectionNoteViewModel
            {
                CorrectionType = "Harga Satuan",
                DONo = "DONo",
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Koreksi_Harga_Satuan()
        {
            GarmentCorrectionNoteViewModel viewModel = new GarmentCorrectionNoteViewModel
            {
                CorrectionType = "Harga Satuan",
                DONo = "DONo",
                Items = new List<GarmentCorrectionNoteItemViewModel>
                {
                    new GarmentCorrectionNoteItemViewModel
                    {
                        PricePerDealUnitAfter = -1,
                    },
                    new GarmentCorrectionNoteItemViewModel
                    {
                        PricePerDealUnitBefore = 1,
                        PricePerDealUnitAfter = 1,
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Koreksi_Harga_Total()
        {
            GarmentCorrectionNoteViewModel viewModel = new GarmentCorrectionNoteViewModel
            {
                CorrectionType = "Harga Total",
                DONo = "DONo",
                Items = new List<GarmentCorrectionNoteItemViewModel>
                {
                    new GarmentCorrectionNoteItemViewModel
                    {
                        PriceTotalAfter = -1,
                    },
                    new GarmentCorrectionNoteItemViewModel
                    {
                        PriceTotalBefore = 1,
                        PriceTotalAfter = 1,
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }
    }
}
