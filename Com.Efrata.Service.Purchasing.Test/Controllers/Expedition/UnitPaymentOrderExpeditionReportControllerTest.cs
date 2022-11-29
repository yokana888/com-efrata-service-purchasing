using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Expedition
{
    public class UnitPaymentOrderExpeditionReportControllerTest
    {
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

        private UnitPaymentOrderExpeditionReportController GetController(Mock<IUnitPaymentOrderExpeditionReportService> reportServiceMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            //servicePMock
            //    .Setup(x => x.GetService(typeof(IValidateService)))
            //    .Returns(validateM.Object);

            var controller = new UnitPaymentOrderExpeditionReportController(reportServiceMock.Object)
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

            return controller;
        }

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task Should_Success_GetReport()
        {
            var reportServiceMock = new Mock<IUnitPaymentOrderExpeditionReportService>();
            reportServiceMock.Setup(s => s.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UnitPaymentOrderExpeditionReportWrapper());
            var controller = GetController(reportServiceMock);

            var response = await controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Throw_InternalServerError_If_GetReport_Failed()
        {
            var reportServiceMock = new Mock<IUnitPaymentOrderExpeditionReportService>();
            reportServiceMock.Setup(s => s.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("Error"));
            var controller = GetController(reportServiceMock);

            var response = await controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetReport_Excel()
        {
            var reportServiceMock = new Mock<IUnitPaymentOrderExpeditionReportService>();
            reportServiceMock
                .Setup(s => s.GetExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .ReturnsAsync(new MemoryStream());
            var controller = GetController(reportServiceMock);
            controller.ControllerContext.HttpContext.Request.Headers["accept"] = "application/xls";

            FileContentResult result = await controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) as FileContentResult;
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }
    }
}
