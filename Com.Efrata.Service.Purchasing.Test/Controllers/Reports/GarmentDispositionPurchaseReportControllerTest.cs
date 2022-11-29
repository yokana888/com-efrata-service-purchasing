using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Reports
{
    public class GarmentDispositionPurchaseReportControllerTest
    {
        private Mock<IGarmentDispositionPurchaseFacade> _serviceMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private GarmentDispositionPurchaseReportController _controller;

        public GarmentDispositionPurchaseReportControllerTest()
        {
            SetDefaultServiceMockProvider();
            SetDefaultController();
        }
        public void SetDefaultServiceMockProvider()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            _serviceProviderMock = serviceProviderMock;
        }
        public void SetDefaultServiceMockProvider(Mock<IGarmentDispositionPurchaseFacade> serviceMock)
        {
            _serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentDispositionPurchaseFacade)))
                .Returns(serviceMock.Object);
            SetDefaultController();
        }

        public void SetDefaultController()
        {
            SetDefaultController(_serviceProviderMock);
        }

        public void SetDefaultController(Mock<IServiceProvider> provider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var controller = new GarmentDispositionPurchaseReportController(provider.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                },

            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            _controller = controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Get_Should_Success()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new DispositionPurchaseReportIndexDto(It.IsAny<List<DispositionPurchaseReportTableDto>>(),It.IsAny<int>(), It.IsAny<int>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.Get(It.IsAny<string>(), It.IsAny<int>(),It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        //[Fact]
        //public async Task Get_Should_InternalServerError()
        //{
        //    var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
        //    mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .Throws(new Exception("Exception test"));

        //    SetDefaultServiceMockProvider(mockFacade);

        //    var response = await _controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>());

        //    Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}

        [Fact]
        public void Get_Should_Success_IfDueDateNull()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new DispositionPurchaseReportIndexDto(It.IsAny<List<DispositionPurchaseReportTableDto>>(), It.IsAny<int>(), It.IsAny<int>()));


            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task GetUser_Should_Success()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetListUsers(It.IsAny<string>()))
                .ReturnsAsync(new DispositionUserIndexDto(It.IsAny<List<DispositionUserDto>>(), It.IsAny<int>(), It.IsAny<int>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = await _controller.GetUser(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task GetUser_Should_InternalServerError()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetListUsers(It.IsAny<string>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = await _controller.GetUser(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcel_Should_Success()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new DispositionPurchaseReportIndexDto(new List<DispositionPurchaseReportTableDto>{ new DispositionPurchaseReportTableDto { Category="testCateg"} }, It.IsAny<int>(), It.IsAny<int>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadExcel_Should_InternalServerError()
        {
            var mockFacade = new Mock<IGarmentDispositionPurchaseFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response =  _controller.DownloadExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<Nullable<DateTimeOffset>>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
