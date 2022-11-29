using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class MonitoringROJobOrderControllerTest
    {
        private Mock<IServiceProvider> GetMockServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private MonitoringROJobOrderController GetController(Mock<IMonitoringROJobOrderFacade> mockFacade)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var mockServiceProvider = GetMockServiceProvider();

            if (mockFacade != null)
                mockServiceProvider.Setup(x => x.GetService(typeof(IMonitoringROJobOrderFacade))).Returns(mockFacade.Object);

            MonitoringROJobOrderController controller = new MonitoringROJobOrderController(mockServiceProvider.Object)
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

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task Should_Success_Get_Monitoring()
        {
            var mockFacade = new Mock<IMonitoringROJobOrderFacade>();
            mockFacade.Setup(s => s.GetMonitoring(It.IsAny<long>()))
                .ReturnsAsync(new List<MonitoringROJobOrderViewModel>());

            var controller = GetController(mockFacade);

            IActionResult result = await controller.GetMonitoring(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task Should_Success_Get_Excel_Monitoring()
        {
            var fileName = "filename";

            var mockFacade = new Mock<IMonitoringROJobOrderFacade>();
            mockFacade.Setup(s => s.GetExcel(It.IsAny<long>()))
                .ReturnsAsync(new Tuple<MemoryStream, string>(new MemoryStream(), fileName));

            var controller = GetController(mockFacade);
            controller.ControllerContext.HttpContext.Request.Headers["accept"] = "application/xls";

            FileContentResult result = await controller.GetMonitoring(It.IsAny<long>()) as FileContentResult;
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.Equal($"{fileName}.xlsx", result.FileDownloadName);
        }

        [Fact]
        public async Task Should_Error_Get_Monitoring()
        {
            var mockFacade = new Mock<IMonitoringROJobOrderFacade>();
            mockFacade.Setup(s => s.GetMonitoring(It.IsAny<long>()))
                .ThrowsAsync(new Exception(string.Empty));

            var controller = GetController(mockFacade);

            IActionResult result = await controller.GetMonitoring(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }
    }
}
