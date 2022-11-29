using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitReceiptNoteTests
{
    //[Collection("TestServerFixture Collection")]
    public class UnitReceiptNoteControllerTest
    {
        //private const string MediaType = "application/json";
        //private const string MediaTypePdf = "application/pdf";
        //private readonly string URI = "v1/unit-receipt-notes/by-user";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected UnitReceiptNoteDataUtil DataUtil
        //{
        //    get { return (UnitReceiptNoteDataUtil)this.TestFixture.Service.GetService(typeof(UnitReceiptNoteDataUtil)); }
        //}

        //protected DeliveryOrderDataUtil DataUtilDO
        //{
        //    get { return (DeliveryOrderDataUtil)this.TestFixture.Service.GetService(typeof(DeliveryOrderDataUtil)); }
        //}

        //public UnitReceiptNoteControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_With_Filter()
        //{
        //    var response = await this.Client.GetAsync(URI + "?filter={'UnitName':'UnitName'}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Invalid_Id()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_PDF_By_Id()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("dev2");
        //    HttpRequestMessage requestMessage = new HttpRequestMessage()
        //    {
        //        RequestUri = new Uri($"{Client.BaseAddress}{URI}/{model.Id}"),
        //        Method = HttpMethod.Get
        //    };
        //    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypePdf));
        //    requestMessage.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.SendAsync(requestMessage);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    UnitReceiptNoteViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data()
        //{
        //    UnitReceiptNoteViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.date = DateTimeOffset.MinValue;
        //    viewModel.unit = null;
        //    viewModel.items = new List<UnitReceiptNoteItemViewModel> { };
        //    viewModel.isStorage = true;
        //    viewModel.storage = null;
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Date_Data()
        //{
        //    DeliveryOrder ModelDO = await DataUtilDO.GetTestData("dev2");
        //    //var response1 = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModelDO).ToString(), Encoding.UTF8, MediaType));

        //    UnitReceiptNoteViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.doId = ModelDO.Id.ToString();
        //    viewModel.date = ModelDO.DODate.AddDays(-90);
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data_Item()
        //{
        //    UnitReceiptNoteViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    foreach (UnitReceiptNoteItemViewModel item in viewModel.items)
        //    {
        //        item.product = null;
        //        item.deliveredQuantity = 0;
        //    }
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}


        ////[Fact]
        ////public async Task Should_Success_Update_Data()
        ////{
        ////    UnitReceiptNote model = await DataUtil.GetTestData("dev2");

        ////    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        ////    var json = responseGetById.Content.ReadAsStringAsync();

        ////    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        ////    Assert.True(result.ContainsKey("apiVersion"));
        ////    Assert.True(result.ContainsKey("message"));
        ////    Assert.True(result.ContainsKey("data"));
        ////    Assert.True(result["data"].GetType().Name.Equals("JObject"));

        ////    UnitReceiptNoteViewModel viewModel = JsonConvert.DeserializeObject<UnitReceiptNoteViewModel>(result.GetValueOrDefault("data").ToString());

        ////    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        ////    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ////}

        //[Fact]
        //public async Task Should_Error_Update_Data_Id()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/0", new StringContent(JsonConvert.SerializeObject(new UnitReceiptNoteViewModel()).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Invalid_Data()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.True(result["data"].GetType().Name.Equals("JObject"));

        //    UnitReceiptNoteViewModel viewModel = JsonConvert.DeserializeObject<UnitReceiptNoteViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel.date = DateTimeOffset.MinValue;
        //    viewModel.supplier = null;
        //    viewModel.unit = null;
        //    viewModel.items = new List<UnitReceiptNoteItemViewModel> { };

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data_By_Id()
        //{
        //    UnitReceiptNote model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.DeleteAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Delete_Data_Invalid_Id()
        //{
        //    var response = await this.Client.DeleteAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        private UnitReceiptNoteViewModel ViewModel
        {
            get
            {
                List<UnitReceiptNoteItemViewModel> items = new List<UnitReceiptNoteItemViewModel>
                {
                    new UnitReceiptNoteItemViewModel()
                    {
                        epoDetailId= It.IsAny<long>(),
                        epoId=It.IsAny<long>(),
                        epoNo=It.IsAny<string>(),
                        isPaid=false,
                        product = new ProductViewModel()
                        {
                            uom = new UomViewModel()
                        }
                    }
                };

                return new UnitReceiptNoteViewModel
                {
                    UId = null,
                    storage = new StorageViewModel(),
                    supplier = new SupplierViewModel()
                    {
                        import = false
                    },
                    items = items,
                    unit = new UnitViewModel()
                    {
                        division = new DivisionViewModel()
                    }
                };
            }
        }

        private UnitReceiptNoteViewModel ViewModel2
        {
            get
            {
                List<UnitReceiptNoteItemViewModel> items = new List<UnitReceiptNoteItemViewModel>
                {
                    new UnitReceiptNoteItemViewModel()
                    {
                        epoDetailId= It.IsAny<long>(),
                        epoId=It.IsAny<long>(),
                        epoNo=It.IsAny<string>(),
                        isPaid=false,
                        product = new ProductViewModel()
                        {
                            uom = new UomViewModel()
                        }
                    }
                };

                return new UnitReceiptNoteViewModel
                {
                    UId = null,
                    storage = new StorageViewModel(),
                    supplier = new SupplierViewModel()
                    {
                        import = false
                    },
                    items = items,
                    unit = new UnitViewModel()
                    {
                        _id = "50",
                        division = new DivisionViewModel()
                    }
                };
            }
        }

        private UnitReceiptNoteViewModel ViewModel3
        {
            get
            {
                List<UnitReceiptNoteItemViewModel> items = new List<UnitReceiptNoteItemViewModel>
                {
                    new UnitReceiptNoteItemViewModel()
                    {
                        epoDetailId= It.IsAny<long>(),
                        epoId=It.IsAny<long>(),
                        epoNo=It.IsAny<string>(),
                        isPaid=false,
                        product = new ProductViewModel()
                        {
                            uom = new UomViewModel()
                        }
                    }
                };

                return new UnitReceiptNoteViewModel
                {
                    UId = null,
                    storage = new StorageViewModel(),
                    supplier = new SupplierViewModel()
                    {
                        import = false
                    },
                    items = items,
                    unit = new UnitViewModel()
                    {
                        _id = "35",
                        division = new DivisionViewModel()
                    }
                };
            }
        }

        private UnitReceiptNote Model
        {
            get
            {
                return new UnitReceiptNote
                {
                    Items = new List<UnitReceiptNoteItem>()
                };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private UnitReceiptNoteController GetController(Mock<IUnitReceiptNoteFacade> facadeMock, Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> autoMapperMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            //var servicePMock = GetServiceProvider();
            //servicePMock
            //    .Setup(x => x.GetService(typeof(IValidateService)))
            //    .Returns(validateM.Object);

            UnitReceiptNoteController controller = new UnitReceiptNoteController(autoMapperMock.Object, facadeMock.Object, serviceProviderMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<UnitReceiptNote>(new List<UnitReceiptNote>() { Model }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitReceiptNoteViewModel>>(It.IsAny<List<UnitReceiptNote>>()))
                .Returns(new List<UnitReceiptNoteViewModel> { ViewModel });

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_ById()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNoteViewModel>(It.IsAny<UnitReceiptNote>()))
                .Returns(ViewModel);

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetPdf_ById()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNoteViewModel>(It.IsAny<UnitReceiptNote>()))
                .Returns(ViewModel);

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_GetPdf_ById_Except()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNoteViewModel>(It.IsAny<UnitReceiptNote>()))
                .Returns(ViewModel2);

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_GetPdf_ById_Except1()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNoteViewModel>(It.IsAny<UnitReceiptNote>()))
                .Returns(ViewModel3);

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }


        [Fact]
        public void Should_ThrowException_Get_ById()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_ThrowBadRequest_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Throws(GetServiceValidationExeption());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_ThrowException_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Throws(new Exception());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_ThrowBadRequest_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Throws(GetServiceValidationExeption());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_ThrowException_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<UnitReceiptNoteViewModel>()))
                .Throws(new Exception());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitReceiptNote>(It.IsAny<UnitReceiptNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitReceiptNote>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync("");

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_ThrowException_Delete_Data()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();
            mockFacade
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_Payload_Creditor_Account_Get_ById()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GetCreditorAccountDataByURNNo(It.IsAny<string>()))
                .ReturnsAsync(new { test = 1 });

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = await controller.GetCreditorAccountByURNNo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Throws_Exception_Get_Payload_Creditor_Account_Get_ById_Null()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GetCreditorAccountDataByURNNo(It.IsAny<string>()))
                .ReturnsAsync(null);

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = await controller.GetCreditorAccountByURNNo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Throw_Exception_Get_Payload_Creditor_Account_Get_ById()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GetCreditorAccountDataByURNNo(It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            UnitReceiptNoteController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = await controller.GetCreditorAccountByURNNo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
