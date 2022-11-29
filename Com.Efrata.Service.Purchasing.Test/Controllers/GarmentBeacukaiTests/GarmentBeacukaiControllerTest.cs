using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentBeacukaiControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentBeacukaiTests
{
//	[Collection("TestServerFixture Collection")]
	public class GarmentBeacukaiControllerTest
	{
        ////private const string MediaType = "application/json";
        //private readonly string URI = "v1/garment-beacukai";
        ////private readonly string USERNAME = "dev2";
        //private TestServerFixture TestFixture { get; set; }
        //private GarmentBeacukaiViewModel ViewModel
        //{
        //	get
        //	{
        //		return new GarmentBeacukaiViewModel
        //		{
        //                  UId = null,
        //                  supplier = new SupplierViewModel
        //			{
        //				Name = "supplier",
        //			},
        //			items = new List<GarmentBeacukaiItemViewModel>
        //			{
        //				new GarmentBeacukaiItemViewModel
        //				{
        //					deliveryOrder= new GarmentDeliveryOrderViewModel
        //					{
        //						Id=It.IsAny<int>()
        //					}
        //				}

        //			}

        //		};
        //	}
        //}
        //public GarmentBeacukaiControllerTest(TestServerFixture fixture)
        //{
        //	TestFixture = fixture;
        //}
        //private HttpClient Client
        //{
        //	get { return this.TestFixture.Client; }
        //}
        //protected GarmentBeacukaiDataUtil DataUtil
        //{
        //	get { return (GarmentBeacukaiDataUtil)this.TestFixture.Service.GetService(typeof(GarmentBeacukaiDataUtil)); }
        //}
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
        //private GarmentBeacukaiController GetController(Mock<IGarmentBeacukaiFacade> facadeM, Mock<IGarmentDeliveryOrderFacade> facadeDO, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentBeacukaiFacade> facadeBC)
        //{
        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);

        //    var servicePMock = GetServiceProvider();
        //    if (validateM != null)
        //    {
        //        servicePMock
        //            .Setup(x => x.GetService(typeof(IValidateService)))
        //            .Returns(validateM.Object);
        //    }

        //    GarmentBeacukaiController controller = new GarmentBeacukaiController(servicePMock.Object, mapper.Object, facadeM.Object, facadeDO.Object)
        //    {
        //        ControllerContext = new ControllerContext()
        //        {
        //            HttpContext = new DefaultHttpContext()
        //            {
        //                User = user.Object
        //            }
        //        }
        //    };
        //    controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
        //    controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

        //    return controller;
        //}
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
        //private GarmentBeacukai Model
        //{
        //	get
        //	{
        //		return new GarmentBeacukai { };
        //	}
        //}
        //private GarmentDeliveryOrder DeliveryOrderModel
        //{
        //	get
        //	{
        //		return new GarmentDeliveryOrder {
        //			Id = 1
        //		};
        //	}
        //}
        //private ServiceValidationExeption GetServiceValidationExeption()
        //{
        //	Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
        //	List<ValidationResult> validationResults = new List<ValidationResult>();

        //	ViewModel.beacukaiNo = ViewModel.beacukaiNo;
        //	ViewModel.beacukaiDate = ViewModel.beacukaiDate;
        //	ViewModel.supplier = ViewModel.supplier;
        //	foreach (var item in ViewModel.items)
        //	{
        //		item.deliveryOrder = item.deliveryOrder;
        //		if (item.deliveryOrder != null)
        //		{
        //			item.deliveryOrder.paymentType = "type";
        //			item.deliveryOrder.paymentMethod = "method";
        //		}
        //	}
        //	System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
        //	return new ServiceValidationExeption(validationContext, validationResults);
        //}
        private GarmentBeacukaiController GetController(Mock<IGarmentBeacukaiFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> facadeDO)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            var controller = new GarmentBeacukaiController(servicePMock.Object, mapper.Object, facadeM.Object, facadeDO.Object)
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
        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //	var response = await this.Client.GetAsync(URI);
        //	Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //	// add error ^_^
        //	var responseError = await this.Client.GetAsync(URI + "?filter={'IsPosted':}");
        //	Assert.Equal(HttpStatusCode.InternalServerError, responseError.StatusCode);
        //}

        //[Fact]
        //public void Should_Success_Get_All_Data_By_User()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();

        //	mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
        //		.Returns(Tuple.Create(new List<GarmentBeacukai>(), 0, new Dictionary<string, string>()));

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<List<GarmentBeacukaiViewModel>>(It.IsAny<List<GarmentBeacukai>>()))
        //		.Returns(new List<GarmentBeacukaiViewModel> { ViewModel });

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

        //	GarmentBeacukaiController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
        //	var response = controller.GetByUser();
        //	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        //[Fact]
        //public async Task Should_Error_Get_Invalid_Id()
        //{
        //	var response = await this.Client.GetAsync($"{URI}/0");
        //	Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public void Should_Sucscess_Get_Data_By_Id()
        //{
        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //		.Returns(Model);

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<GarmentBeacukaiViewModel>(It.IsAny<GarmentBeacukai>()))
        //		.Returns(ViewModel);
        //	mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
        //		.Returns(new GarmentDeliveryOrderViewModel());

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //	IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //		 .Returns(DeliveryOrderModel);
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();


        //	GarmentBeacukaiController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
        //	var response = controller.Get(It.IsAny<int>());
        //	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}
        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<GarmentBeacukai>(It.IsAny<GarmentBeacukaiViewModel>()))
        //		.Returns(Model);

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.Create(It.IsAny<GarmentBeacukai>(), "unittestusername", 7))
        //	   .ReturnsAsync(1);

        //	var IPOvalidateMock = new Mock<IValidateService>();
        //	IPOvalidateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();

        //	var IPOmockMapper = new Mock<IMapper>();
        //	IPOmockMapper.Setup(x => x.Map<List<GarmentBeacukai>>(It.IsAny<List<GarmentBeacukaiViewModel>>()))
        //		.Returns(new List<GarmentBeacukai>());

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //	IPOmockFacade.Setup(x => x.Create(It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
        //	   .ReturnsAsync(1);

        //	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

        //	var response = await controller.Post(this.ViewModel);
        //	Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        //}

        //[Fact]
        //public async Task Should_Validate_Create_Data()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Throws(GetServiceValidationExeption());

        //	var mockMapper = new Mock<IMapper>();

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

        //	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

        //	var response = await controller.Post(this.ViewModel);
        //	Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        //}

        //[Fact]
        //public async Task Should_Validate_Create_Data_Empty()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Throws(GetServiceValidationExeption());

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<GarmentBeacukai>(It.IsAny<GarmentBeacukai>()))
        //		.Returns(Model);

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.Create(It.IsAny<GarmentBeacukai>(), "unittestusername", 7))
        //	   .ReturnsAsync(1);

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

        //	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

        //	var response = await controller.Post(this.ViewModel);
        //	Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        //}
        //[Fact]
        //public async Task Should_Error_Create_Data()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<GarmentBeacukai>(It.IsAny<GarmentBeacukaiViewModel>()))
        //		.Returns(Model);

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.Create(It.IsAny<GarmentBeacukai>(), "unittestusername", 7))
        //	   .ReturnsAsync(1);

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

        //	GarmentBeacukaiController controller = new GarmentBeacukaiController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object,IPOmockFacade.Object);

        //	var response = await controller.Post(this.ViewModel);
        //	Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	validateMock.Setup(s => s.Validate(It.IsAny<GarmentBeacukaiViewModel>())).Verifiable();

        //	var mockMapper = new Mock<IMapper>();
        //	mockMapper.Setup(x => x.Map<GarmentBeacukai>(It.IsAny<GarmentBeacukaiViewModel>()))
        //		.Returns(Model);

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.Create(It.IsAny<GarmentBeacukai>(), "unittestusername", 7))
        //	   .ReturnsAsync(1);

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

        //	var controller = new GarmentBeacukaiController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object,IPOmockFacade.Object);

        //	var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentBeacukaiViewModel>());
        //	Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}

        //[Fact]
        //public void Should_Success_Delete_Data()
        //{
        //	var validateMock = new Mock<IValidateService>();
        //	var mockMapper = new Mock<IMapper>();

        //	var mockFacade = new Mock<IGarmentBeacukaiFacade>();
        //	mockFacade.Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
        //		.Returns(1);

        //	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();


        //	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

        //	var response = controller.Delete(It.IsAny<int>());
        //	Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Success_ReadBCByPO()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var list = new List<object>();

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var mockFacade = new Mock<IGarmentBeacukaiFacade>();
            mockFacade.Setup(x => x.ReadBCByPOSerialNumbers(It.IsAny<string>()))
                .Returns(list);


            var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

            var response = controller.BCByPo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_ReadBCByPO()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var list = new List<object>();

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var mockFacade = new Mock<IGarmentBeacukaiFacade>();
            mockFacade.Setup(x => x.ReadBCByPOSerialNumbers(It.IsAny<string>()))
                .Throws(new Exception());


            var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

            var response = controller.BCByPo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
