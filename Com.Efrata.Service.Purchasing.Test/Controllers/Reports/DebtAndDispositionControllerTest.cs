using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Services;
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
    public class DebtAndDispositionControllerTest
    {
        private Mock<IDebtAndDispositionSummaryService> _serviceMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private DebtAndDispositionController _controller;

        public DebtAndDispositionControllerTest()
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
        public void SetDefaultServiceMockProvider(Mock<IDebtAndDispositionSummaryService> serviceMock)
        {
            _serviceProviderMock.Setup(s => s.GetService(typeof(IDebtAndDispositionSummaryService)))
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

            var controller = new DebtAndDispositionController(provider.Object)
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
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Get_Should_Success_IfDueDateNull()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));


            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcel_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadExcel_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcel_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcel_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcel_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcel_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }



        [Fact]
        public void DownloadPdf_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadPdf_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadPdf_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdf_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdf_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdf_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdf(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }



        [Fact]
        public void GetDebt_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetDebt_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void GetDebt_Should_Success_IfDueDateNull()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetDebtCashFlow_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTimeOffset>(),It.IsAny<string>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDebt(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetDebtCashFlow_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDebt(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetSummary_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetSummary_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetSummary(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcelDebt_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadExcelDebt_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcelDebt_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());


            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDebt_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDebt_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDebt_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDebt_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadPdfDebt_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadPdfDebt_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());


            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDebt_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDebt_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDebt_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDebtSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDebt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }



        [Fact]
        public void GetDisposition_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetDisposition_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void GetDisposition_Should_Success_IfDueDateNull()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetReportDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ReadResponse<DebtAndDispositionSummaryDto>(new List<DebtAndDispositionSummaryDto>(), 10, new Dictionary<string, string>()));


            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.GetDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcelDisposition_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadExcelDisposition_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadExcelDisposition_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDisposition_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDisposition_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadExcelDisposition_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                 .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadExcelDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }





        [Fact]
        public void DownloadPdfDisposition_Should_Success()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.NotNull(response);

        }

        [Fact]
        public void DownloadPdfDisposition_Should_InternalServerError()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception("Exception test"));

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Nullable<DateTimeOffset>>(), It.IsAny<bool>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void DownloadPdfDisposition_Should_Success_IfIsImportTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, true, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDisposition_Should_Success_IfIsImportFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, false, It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDisposition_Should_Success_IfIsForeignCurrencyTrue()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public void DownloadPdfDisposition_Should_Success_IfIsForeignCurrencyFalse()
        {
            var mockFacade = new Mock<IDebtAndDispositionSummaryService>();
            mockFacade.Setup(facade => facade.GetDispositionSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                 .Returns(new List<DebtAndDispositionSummaryDto>());

            SetDefaultServiceMockProvider(mockFacade);

            var response = _controller.DownloadPdfDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), false);

            Assert.NotNull(response);
        }
    }
}
