using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentStockOpnameControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentStockOpnameControllerTests
{
    public class GarmentStockOpnameControllerTest
    {
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService());

            return serviceProvider;
        }

        private GarmentStockOpnameController GetController(Mock<IGarmentStockOpnameFacade> facadeMock, Mock<IServiceProvider> servicePMock = null)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            servicePMock = servicePMock ?? GetServiceProvider();

            GarmentStockOpnameController controller = new GarmentStockOpnameController(facadeMock.Object, servicePMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object,
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        private ServiceValidationExeption GetServiceValidationExeption(GarmentStockOpnameDownload obj)
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(obj, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        [Fact]
        public void Get_Success()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));

            var controller = GetController(mockFacade);

            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_Error()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var controller = GetController(mockFacade);

            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetById_Success()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentStockOpname());

            var controller = GetController(mockFacade);

            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetById_Error()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Throws(new Exception());

            var controller = GetController(mockFacade);

            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Download_Success()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Download(It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock.Setup(s => s.Validate(It.IsAny<GarmentStockOpnameDownload>()))
                .Verifiable();

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateServiceMock.Object);

            var controller = GetController(mockFacade, servicePMock);

            var response = controller.DownloadFile(DateTimeOffset.Now, "unit", "storage", "storageName");
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Download_NoFilter_BadRequest()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock.Setup(s => s.Validate(It.IsAny<GarmentStockOpnameDownload>()))
                .Throws(GetServiceValidationExeption(new GarmentStockOpnameDownload(null, null, null, null)));

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateServiceMock.Object);

            var controller = GetController(mockFacade, servicePMock);

            var response = controller.DownloadFile(null, null, null, null);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Download_Error()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Download(It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock.Setup(s => s.Validate(It.IsAny<GarmentStockOpnameDownload>()))
                .Verifiable();

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateServiceMock.Object);

            var controller = GetController(mockFacade, servicePMock);

            var response = controller.DownloadFile(DateTimeOffset.Now, "unit", "storage", "storageName");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Upload_Success()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Upload(It.IsAny<Stream>()))
                .ReturnsAsync(new GarmentStockOpname());

            var controller = GetController(mockFacade);
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(), 0, 0, "Data", "data.xlsx");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = await controller.UploadFile();
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Upload_BadRequest()
        {
            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Upload(It.IsAny<Stream>()))
                .ThrowsAsync(new ServiceValidationExeption(null, new List<ValidationResult>()));

            var controller = GetController(mockFacade);
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(), 0, 0, "Data", "data.xlsx");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = await controller.UploadFile();
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Upload_NoFile()
        {

            var mockFacade = new Mock<IGarmentStockOpnameFacade>();

            var controller = GetController(mockFacade);
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

            var response = await controller.UploadFile();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Upload_Error()
        {

            var mockFacade = new Mock<IGarmentStockOpnameFacade>();
            mockFacade.Setup(x => x.Upload(It.IsAny<Stream>()))
                .Throws(new Exception());

            var controller = GetController(mockFacade);
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(), 0, 0, "Data", "data.xlsx");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = await controller.UploadFile();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
