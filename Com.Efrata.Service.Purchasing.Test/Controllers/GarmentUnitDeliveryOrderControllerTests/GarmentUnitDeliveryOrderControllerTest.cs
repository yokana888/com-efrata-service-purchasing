using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitDeliveryOrderControllers;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentUnitDeliveryOrderControllerTests
{
    public class GarmentUnitDeliveryOrderControllerTest
    {
        private GarmentUnitDeliveryOrderViewModel ViewModel
        {
            get
            {
                return new GarmentUnitDeliveryOrderViewModel
                {
                    UId = null,
                    Article = "ArticleCoba"
                };
            }
        }

        private GarmentUnitDeliveryOrder Model
        {
            get
            {
                return new GarmentUnitDeliveryOrder { };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private GarmentUnitDeliveryOrderControllers GetController(Mock<IGarmentUnitDeliveryOrderFacade> facadeMock, Mock<IValidateService> validateMock = null, Mock<IMapper> mapperMock = null)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateMock.Object);
            }
            if (mapperMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IMapper)))
                    .Returns(mapperMock.Object);
            }

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(servicePMock.Object, facadeMock.Object)
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

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>()))
                .Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrder>(It.IsAny<GarmentUnitDeliveryOrderViewModel>()))
                .Returns(new GarmentUnitDeliveryOrder());

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentUnitDeliveryOrder>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade);

            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade);

            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel());

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrder());

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Item_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadItemById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrderItem());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderItemViewModel>(It.IsAny<GarmentUnitDeliveryOrderItem>()))
                .Returns(new GarmentUnitDeliveryOrderItemViewModel());

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

            var response = controller.GetItemById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Item_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadItemById(It.IsAny<int>()))
                .Returns(new GarmentUnitDeliveryOrderItem());

            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = controller.GetItemById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitDeliveryOrder>()))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrder>(It.IsAny<GarmentUnitDeliveryOrderViewModel>()))
                .Returns(new GarmentUnitDeliveryOrder());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentUnitDeliveryOrderViewModel>());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentUnitDeliveryOrderViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentUnitDeliveryOrder>()))
               .ReturnsAsync(1);

            var controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentUnitDeliveryOrderViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
                .Throws(new Exception(""));

            var controller = GetController(mockFacade);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Validate_Update_Data()
        //{
        //    var validateMock = new Mock<IValidateService>();
        //    validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitDeliveryOrderViewModel>())).Throws(GetServiceValidationExeption());

        //    var mockMapper = new Mock<IMapper>();

        //    var mockFacade = new Mock<IGarmentUnitDeliveryOrder>();

        //    var controller = GetController(mockFacade, validateMock, mockMapper);

        //    var response = controller.Put(It.IsAny<int>(), It.IsAny<GarmentUnitDeliveryOrderViewModel>());
        //    Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Success_Get_All_Data_For_GarmentUnitExpenditureNote()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadForUnitExpenditureNote(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));


            var mockMapper = new Mock<IMapper>();

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetForUnitExpenditureNote();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_For_GarmentUnitExpenditureNote()
        {
            GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, null);
            var response = controller.GetForUnitExpenditureNote();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Data_By_RO()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadForLeftOver(It.IsAny<string>()))
                .Returns(new List<object>());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel());

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

            var response = controller.GetbyROleftover(It.IsAny<string>());
        
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        
        [Fact]
        public void Should_Success_Get_Data_Item_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadForLeftOver(It.IsAny<string>()))
                .Returns(new List<object>());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel());

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

            var response = controller.GetbyROleftover(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_RO()
        {
            var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacade.Setup(x => x.ReadForLeftOver(It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel());

            GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

            var response = controller.GetbyROleftover(It.IsAny<string>());
      
        
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Error_Get_Data_Item_By_Id()
        //{
        //    var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
        //    mockFacade.Setup(x => x.ReadItemById(It.IsAny<int>()))
        //        .Returns(new GarmentUnitDeliveryOrderItem());

        //    GarmentUnitDeliveryOrderControllers controller = new GarmentUnitDeliveryOrderControllers(GetServiceProvider().Object, mockFacade.Object);

        //    var response = controller.GetItemById(It.IsAny<int>());
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}

        //[Fact]
        //public void Should_Null_Mapper_Get_Data_Item_By_Id()
        //{
        //    var mockFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
        //    mockFacade.Setup(x => x.ReadItemById(It.IsAny<int>()))
        //        .Returns(new GarmentUnitDeliveryOrderItem());

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderItemViewModel>(null))
        //        .Returns(new GarmentUnitDeliveryOrderItemViewModel());

        //    GarmentUnitDeliveryOrderControllers controller = GetController(mockFacade, null, mockMapper);

        //    var response = controller.GetItemById(It.IsAny<int>());
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}
    }
}
