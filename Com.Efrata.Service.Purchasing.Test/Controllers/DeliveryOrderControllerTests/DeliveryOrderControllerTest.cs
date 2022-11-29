using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.DeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.DeliveryOrderControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.DeliveryOrderControllerTests
{
  
    public class DeliveryOrderControllerTest
    {
        

        private DeliveryOrderViewModel ViewModel
        {
            get
            {
                List<DeliveryOrderItemViewModel> items = new List<DeliveryOrderItemViewModel>
                {
                    new DeliveryOrderItemViewModel()
                    {
                        fulfillments = new List<DeliveryOrderFulFillMentViewModel>()
                        {
                            new DeliveryOrderFulFillMentViewModel()
                            {

                            }
                        }                    }
                };

                return new DeliveryOrderViewModel
                {
                    UId = null,

                    items = items,

                };
            }
        }

        private DeliveryOrder Model
        {
            get
            {
                return new DeliveryOrder
                {
                    Items = new List<DeliveryOrderItem>() { new DeliveryOrderItem(){

                        Details = new List<DeliveryOrderDetail>()
                            {

                            }
                        }
                    }
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


        private DeliveryOrderController GetController(Mock<IDeliveryOrderFacade> facadeMock, Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> autoMapperMock)
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

            DeliveryOrderController controller = new DeliveryOrderController(autoMapperMock.Object, facadeMock.Object, serviceProviderMock.Object)
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
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Tuple<List<DeliveryOrder>, int, Dictionary<string, string>>(new List<DeliveryOrder>() { Model }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<DeliveryOrderViewModel>>(It.IsAny<List<DeliveryOrder>>()))
                .Returns(new List<DeliveryOrderViewModel> { ViewModel });

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_All_Data()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("error"));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<DeliveryOrderViewModel>>(It.IsAny<List<DeliveryOrder>>()))
                .Returns(new List<DeliveryOrderViewModel> { ViewModel });

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_ByUser()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Tuple<List<DeliveryOrder>, int, Dictionary<string, string>>(new List<DeliveryOrder>() { Model }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<DeliveryOrderViewModel>>(It.IsAny<List<DeliveryOrder>>()))
                .Returns(new List<DeliveryOrderViewModel> { ViewModel });

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetByUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_ById()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new Tuple<DeliveryOrder, List<long>>(Model, new List<long>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrderViewModel>(It.IsAny<DeliveryOrder>()))
                .Returns(ViewModel);

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_ById_when_InvalidId()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new Tuple<DeliveryOrder, List<long>>(Model, new List<long>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrderViewModel>(It.IsAny<DeliveryOrder>()))
                .Returns(()=>null);

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_ById()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Throws(new Exception("error"));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrderViewModel>(It.IsAny<DeliveryOrder>()))
                .Returns(ViewModel);

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }



        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception("error"));

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_Validate_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Throws(GetServiceValidationExeption());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception("error"));

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Verifiable();

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception("error"));

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_Validate_Data_Update()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<DeliveryOrderViewModel>()))
                .Throws(GetServiceValidationExeption());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateMock.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeliveryOrder>(It.IsAny<DeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DeliveryOrder>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception("error"));

            var controller = GetController(mockFacade, serviceProviderMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Delete_Data()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();
            mockFacade
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception("error"));

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_BySupplier()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadBySupplier(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<DeliveryOrder>());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<DeliveryOrderViewModel>>(It.IsAny<List<DeliveryOrder>>()))
                .Returns(new List<DeliveryOrderViewModel> { ViewModel });

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.BySupplier(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetReport()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<DeliveryOrderReportViewModel>, int>(new List<DeliveryOrderReportViewModel>(), It.IsAny<int>()));

            var mockMapper = new Mock<IMapper>();

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_GetReport()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception("error"));

            var mockMapper = new Mock<IMapper>();

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetXls()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                 It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>());
            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Fail_GetXls()
        {
            var mockFacade = new Mock<IDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                 It.IsAny<int>()))
                .Throws(new Exception("error"));

            var mockMapper = new Mock<IMapper>();

            DeliveryOrderController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
