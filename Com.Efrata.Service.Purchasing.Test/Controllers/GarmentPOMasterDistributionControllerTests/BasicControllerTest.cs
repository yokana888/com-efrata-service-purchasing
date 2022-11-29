using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentPOMasterDistributionControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentPOMasterDistributionControllerTests
{
    public class BasicControllerTest
    {
        private Mock<IServiceProvider> GetMockServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private GarmentPOMasterDistributionController GetController(Mock<IGarmentPOMasterDistributionFacade> mockFacade, Mock<IMapper> mockMapper, Mock<IValidateService> mockValidate)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var mockServiceProvider = GetMockServiceProvider();

            if (mockFacade != null)
                mockServiceProvider.Setup(x => x.GetService(typeof(IGarmentPOMasterDistributionFacade))).Returns(mockFacade.Object);

            if (mockMapper != null)
                mockServiceProvider.Setup(x => x.GetService(typeof(IMapper))).Returns(mockMapper.Object);

            if (mockValidate != null)
                mockServiceProvider.Setup(x => x.GetService(typeof(IValidateService))).Returns(mockValidate.Object);

            GarmentPOMasterDistributionController controller = new GarmentPOMasterDistributionController(mockServiceProvider.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user.Object }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        private ServiceValidationExeption GetServiceValidationExeption(GarmentPOMasterDistributionViewModel viewModel)
        {
            var serviceProvider = GetMockServiceProvider();

            ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider.Object, null);

            List<ValidationResult> validationResults = new List<ValidationResult>();

            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 1, new Dictionary<string, string>()));

            GarmentPOMasterDistributionController controller = GetController(mockFacade, null, null);
            IActionResult response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception(string.Empty));

            GarmentPOMasterDistributionController controller = GetController(mockFacade, null, null);
            IActionResult response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<long>()))
                .Returns(new GarmentPOMasterDistribution { });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPOMasterDistributionViewModel>(It.IsAny<GarmentPOMasterDistribution>()))
                .Returns(new GarmentPOMasterDistributionViewModel());

            GarmentPOMasterDistributionController controller = GetController(mockFacade, mockMapper, null);

            IActionResult response = controller.Get(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
           //Setup
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(x => x.ReadById(It.IsAny<long>()))
                .Returns(()=>null);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<GarmentPOMasterDistributionViewModel>(It.IsAny<GarmentPOMasterDistribution>()))
                .Returns(new GarmentPOMasterDistributionViewModel());

            //Act
            GarmentPOMasterDistributionController controller = GetController(mockFacade, mockMapper, null);
            IActionResult response = controller.Get(It.IsAny<long>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Create(It.IsAny<GarmentPOMasterDistribution>()))
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(s => s.Map<GarmentPOMasterDistribution>(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Returns(new GarmentPOMasterDistribution());

            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Verifiable();

            var controller = GetController(mockFacade, mockMapper, mockValidate);

            IActionResult response = await controller.Post(new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Validate_Create_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Throws(GetServiceValidationExeption(new GarmentPOMasterDistributionViewModel()));

            var controller = GetController(null, null, mockValidate);

            IActionResult response = await controller.Post(new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Create(It.IsAny<GarmentPOMasterDistribution>()))
                .ThrowsAsync(new Exception(string.Empty));

            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Verifiable();

            var controller = GetController(mockFacade, null, mockValidate);

            IActionResult response = await controller.Post(new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Update(It.IsAny<long>(), It.IsAny<GarmentPOMasterDistribution>()))
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(s => s.Map<GarmentPOMasterDistribution>(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Returns(new GarmentPOMasterDistribution());

            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Verifiable();

            var controller = GetController(mockFacade, mockMapper, mockValidate);

            IActionResult response = await controller.Put(1, new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Validate_Update_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Throws(GetServiceValidationExeption(new GarmentPOMasterDistributionViewModel()));

            var controller = GetController(null, null, mockValidate);

            IActionResult response = await controller.Put(1, new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Update(It.IsAny<long>(), It.IsAny<GarmentPOMasterDistribution>()))
                .ThrowsAsync(new Exception(string.Empty));

            var mockValidate = new Mock<IValidateService>();
            mockValidate
                .Setup(s => s.Validate(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Verifiable();

            var controller = GetController(mockFacade, null, mockValidate);

            IActionResult response = await controller.Put(1, new GarmentPOMasterDistributionViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Date()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Delete(It.IsAny<long>()))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade, null, null);

            IActionResult response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Date()
        {
            var mockFacade = new Mock<IGarmentPOMasterDistributionFacade>();
            mockFacade
                .Setup(s => s.Delete(It.IsAny<long>()))
                .ThrowsAsync(new Exception(string.Empty));

            var controller = GetController(mockFacade, null, null);

            IActionResult response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
