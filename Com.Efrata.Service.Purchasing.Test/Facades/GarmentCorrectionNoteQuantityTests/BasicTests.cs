using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
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

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentCorrectionNoteQuantityTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentCorrectionNoteQuantity";

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

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var HttpClientService = new Mock<IHttpClientService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"_id\":1,\"_deleted\":false,\"_active\":false,\"_createdDate\":\"2018-06-21T01:57:47.6772924\",\"_createdBy\":\"\",\"_createAgent\":\"\",\"_updatedDate\":\"2018-06-21T01:57:47.6772924\",\"_updatedBy\":\"\",\"_updateAgent\":\"\",\"code\":\"A001\",\"name\":\"ADI KARYA. UD\",\"address\":\"JL.JAMBU,JAJAR,SOLO\",\"contact\":\"\",\"PIC\":\"\",\"import\":true,\"NPWP\":\"\",\"serialNumber\":\"\"}}");
            string supplierUri = "master/garment-suppliers";
            HttpClientService
                .Setup(x => x.GetAsync(It.IsRegex( $"^{APIEndpoint.Core}{supplierUri}")))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }
        private Mock<IServiceProvider> GetServiceProviderDO()
        {
            var HttpClientService = new Mock<IHttpClientService>();
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            string gCurrencyUri = "master/garment-currencies";
            HttpClientService
                .Setup(x => x.GetAsync(It.IsRegex($"^{APIEndpoint.Core}{gCurrencyUri}")))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }


        private GarmentCorrectionNoteQuantityDataUtil dataUtil(GarmentCorrectionNoteQuantityFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentCorrectionNoteQuantityDataUtil(facade, garmentDeliveryOrderDataUtil);
        }
        

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(data,false, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_With_Tax()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
            var Response = await facade.Create(data,false,USERNAME);
            Assert.NotEqual(0, Response);

            var data2nd = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
            var Response2nd = await facade.Create(data2nd,false,USERNAME);
            Assert.NotEqual(0, Response2nd);
        }

        [Fact]
        public async Task Should_Error_Create_Data_Null_Items()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data,false, USERNAME));
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
        public async Task Should_Success_Get_Data_By_Supplier()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Responses = await facade.Create(data,false, USERNAME);
            var Response = facade.GetSupplier(data.SupplierId);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_DOId()
        {
            var facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            GarmentCorrectionNote data = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.ReadByDOId((int)data.DOId);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_Validate_Data_Koreksi_Harga_Jumlah()
        {
            GarmentCorrectionNoteViewModel viewModel = new GarmentCorrectionNoteViewModel
            {
                CorrectionType = "Jumlah",
                DONo = "DONo",
                Items = new List<GarmentCorrectionNoteItemViewModel>
                {
                    new GarmentCorrectionNoteItemViewModel
                    {
                        Quantity = 0,

                        DODetailId = 0,
                        EPOId = 0,
                        EPONo = null,
                        PRId = 0,
                        PRNo = null,
                        POId = 0,
                        POSerialNumber = null,
                        RONo = null,
                        Product = null,
                        Uom = null
                    },
                },

                DOId = 0,
                Currency = null,
                IncomeTax = null,
                Remark = null,
                TotalCorrection = 0,
                NKPH = null,
                NKPN = null
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }
        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var facade = new GarmentCorrectionNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var facadeQty = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facadeQty, GetCurrentMethod()).GetTestData(USERNAME);
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }
    }
}