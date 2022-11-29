using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Reports
{
    public class GarmentPurchasingBookReportControllerTest
    {
        private Mock<IGarmentPurchasingBookReportService> _serviceMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private GarmentPurchasingBookReportController _controller;

        public GarmentPurchasingBookReportControllerTest()
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
        public void SetDefaultServiceMockProvider(Mock<IGarmentPurchasingBookReportService> serviceMock)
        {
            _serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentPurchasingBookReportService)))
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

            var controller = new GarmentPurchasingBookReportController(provider.Object)
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
        public Mock<IGarmentPurchasingBookReportService> SetDefaultSuccessService()
        {
            var mockService = new Mock<IGarmentPurchasingBookReportService>();

            mockService.Setup(s =>
            s.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>())
            )
            .ReturnsAsync(new MemoryStream());

            mockService.Setup(s =>
            s.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>())
            )
            .Returns(new ReportDto(new List<ReportIndexDto>(),new List<ReportCategoryDto>(),new List<ReportCurrencyDto>()));

            mockService.Setup(s =>
            s.GetAccountingCategories(It.IsAny<string>())
            )
            .Returns(new List<AutoCompleteDto>());

            mockService.Setup(s =>
            s.GetBillNos(It.IsAny<string>())
            )
            .Returns(new List<AutoCompleteDto>());

            mockService.Setup(s =>
            s.GetPaymentBills(It.IsAny<string>())
            )
            .Returns(new List<AutoCompleteDto>());

            return mockService;
        }

        public Mock<IGarmentPurchasingBookReportService> SetDefaultExceptionService()
        {
            var mockService = new Mock<IGarmentPurchasingBookReportService>();

            mockService.Setup(s =>
            s.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>())
            )
            .ThrowsAsync(new Exception("ExceptionTest"));

            mockService.Setup(s =>
            s.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>())
            )
            .Throws(new Exception("ExceptionTest"));

            mockService.Setup(s =>
            s.GetAccountingCategories(It.IsAny<string>())
            )
            .Throws(new Exception("ExceptionTest"));

            mockService.Setup(s =>
            s.GetBillNos(It.IsAny<string>())
            )
            .Throws(new Exception("ExceptionTest"));

            mockService.Setup(s =>
            s.GetPaymentBills(It.IsAny<string>())
            )
            .Throws(new Exception("ExceptionTest"));

            return mockService;
        }


        [Fact]
        public void GetReport_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetReport(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),It.IsAny<bool>(),It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void GetReport_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_Should_Success_DateNull()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null, It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetBillNo_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetBillNo(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void GetBillNo_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetBillNo(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetPaymentBill_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPaymentBill(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void GetPaymentBill_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPaymentBill(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        [Fact]
        public void GetGarmentAccountingCategories_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetGarmentAccountingCategories(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void GetGarmentAccountingCategories_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetGarmentAccountingCategories(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetXls_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = await _controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetXls_Should_Success_BillNoPaymentNoUndefined()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = await _controller.GetXls(It.IsAny<string>(), "undefined", "undefined", It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetXls_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = await _controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetPdf_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response =  _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);
        }
        [Fact]
        public void GetPdf_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void GetPdf_Should_Success_IsForeignCurrencyTrue()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), true, It.IsAny<bool>());

            Assert.NotNull(response);

        }
        [Fact]
        public void GetPdf_Should_Success_IsForeignCurrencyFalse()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), false, It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void GetPdf_Should_Success_IsImportSupplierTrue()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), true);

            Assert.NotNull(response);

        }
        [Fact]
        public void GetPdf_Should_Success_IsImportSupplierFalse()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock);

            var response = _controller.GetPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), false);

            Assert.NotNull(response);

        }
    }
}
