using Com.Efrata.Service.Purchasing.Lib.Facades.UnpaidDispositionReportFacades;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnpaidDispositionReport;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnpaidDispositionReport;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnpaidDispositionReport
{
    public class UnpaidDispositionReportDetailControllerTest
    {
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private UnpaidDispositionReportDetailController GetController(Mock<IUnpaidDispositionReportDetailFacade> serviceMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(s => s.GetService(typeof(IUnpaidDispositionReportDetailFacade)))
                .Returns(serviceMock.Object);

            serviceProviderMock.Setup(s => s.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { TimezoneOffset = 7, Token = "Bearer unittesttoken", Username = "unittest" });

            var controller = new UnpaidDispositionReportDetailController(serviceProviderMock.Object)
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

        [Fact]
        public async Task Get_Should_Success()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel());

            var controller = GetController(serviceMock);

            var response = await controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task Get_Should_Success_If_DateNull()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel());

            var controller = GetController(serviceMock);

            var response = await controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task Get_Should_Exception()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("test failed"));

            var controller = GetController(serviceMock);

            var response = await controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DownloadExcelAsync_Should_Success()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GenerateExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new MemoryStream());

            var controller = GetController(serviceMock);

            var response = await controller.DownloadExcelAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task DownloadExcelAsync_Should_Exception()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GenerateExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("test failed"));

            var controller = GetController(serviceMock);

            var response = await controller.DownloadExcelAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DownloadExcelAsync_Should_Success_IfForeignCurrencyTrue()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GenerateExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new MemoryStream());

            var controller = GetController(serviceMock);

            var response = await controller.DownloadExcelAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), true);

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task DownloadExcelAsync_Should_Success_IfIsImport()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GenerateExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new MemoryStream());

            var controller = GetController(serviceMock);

            var response = await controller.DownloadExcelAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), true, It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task DownloadExcelAsync_Should_Success_IfDateNull()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GenerateExcel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new MemoryStream());

            var controller = GetController(serviceMock);

            var response = await controller.DownloadExcelAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task DownloadPdfAsync_Should_Success()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel()
                {
                    CategorySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    CurrencySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    GrandTotal = 10,
                    Reports = new List<DispositionReport>() { new DispositionReport() { CurrencyCode = "idr", CategoryId = "1", CategoryName = "test", AccountingLayoutIndex = 0 } },
                    UnitSummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    UnitSummaryTotal = 10
                });

            var controller = GetController(serviceMock);

            var response = await controller.DownloadPdfAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);

        }

        [Fact]
        public async Task DownloadPdfAsync_Should_Exception()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("test failed"));

            var controller = GetController(serviceMock);

            var response = await controller.DownloadPdfAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>());

            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DownloadPdfAsync_Should_Success_IfForeignCurrencyTrue()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel()
                {
                    CategorySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    CurrencySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    GrandTotal = 10,
                    Reports = new List<DispositionReport>() { new DispositionReport() { CurrencyCode = "idr", CategoryId = "1", CategoryName = "test", AccountingLayoutIndex = 0 } },
                    UnitSummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    UnitSummaryTotal = 10
                });

            var controller = GetController(serviceMock);

            var response = await controller.DownloadPdfAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), true);

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);

        }

        [Fact]
        public async Task DownloadPdfAsync_Should_Success_IfIsImport()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel()
                {
                    CategorySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    CurrencySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    GrandTotal = 10,
                    Reports = new List<DispositionReport>() { new DispositionReport() { CurrencyCode = "idr", CategoryId = "1", CategoryName = "test", AccountingLayoutIndex = 0 } },
                    UnitSummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    UnitSummaryTotal = 10
                });

            var controller = GetController(serviceMock);

            var response = await controller.DownloadPdfAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), true, It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);

        }

        [Fact]
        public async Task DownloadPdfAsync_Should_Success_IfDateNull()
        {
            var serviceMock = new Mock<IUnpaidDispositionReportDetailFacade>();
            serviceMock.Setup(service => service.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new UnpaidDispositionReportDetailViewModel()
                {
                    CategorySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    CurrencySummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    GrandTotal = 10,
                    Reports = new List<DispositionReport>() { new DispositionReport() { CurrencyCode = "idr", CategoryId = "1", CategoryName = "test", AccountingLayoutIndex = 0 } },
                    UnitSummaries = new List<Summary>() { new Summary() { AccountingLayoutIndex = 0, CategoryId = "1", CategoryName = "test", CurrencyCode = "idr", Name = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                    UnitSummaryTotal = 10
                });

            var controller = GetController(serviceMock);

            var response = await controller.DownloadPdfAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<bool>());

            //var statusCode = GetStatusCode(response);

            //Assert.Equal((int)HttpStatusCode.OK, statusCode);
            Assert.NotNull(response);

        }
    }
}
