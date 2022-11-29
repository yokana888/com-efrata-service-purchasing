using AutoMapper;
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
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class BudgetMasterSampleDisplayControllerTest
    {
        private Mock<IServiceProvider> GetMockServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private BudgetMasterSampleDisplayController GetController(Mock<IBudgetMasterSampleDisplayFacade> mockFacade)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var mockServiceProvider = GetMockServiceProvider();

            if (mockFacade != null)
                mockServiceProvider.Setup(x => x.GetService(typeof(IBudgetMasterSampleDisplayFacade))).Returns(mockFacade.Object);

            BudgetMasterSampleDisplayController controller = new BudgetMasterSampleDisplayController(mockServiceProvider.Object)
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
        public void Should_Success_Get_Monitoring()
        {
            var mockFacade = new Mock<IBudgetMasterSampleDisplayFacade>();
            mockFacade.Setup(s => s.GetMonitoring(It.IsAny<long>(), "{}"))
                .Returns(Tuple.Create(new List<BudgetMasterSampleDisplayViewModel>()
                {
                    new BudgetMasterSampleDisplayViewModel
                    {

                    }
                }, 1));

            var controller = GetController(mockFacade);

            IActionResult result = controller.GetReport(It.IsAny<long>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public void Should_Success_Get_Excel_Monitoring()
        {
            var mockFacade = new Mock<IBudgetMasterSampleDisplayFacade>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<long>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            BudgetMasterSampleDisplayController controller = GetController(mockFacade);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<long>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_Get_Monitoring()
        {
            var mockFacade = new Mock<IBudgetMasterSampleDisplayFacade>();
            mockFacade.Setup(s => s.GetMonitoring(It.IsAny<long>(), "{}"))
                .Throws(new Exception(string.Empty));

            var controller = GetController(mockFacade);

            IActionResult result = controller.GetReport(It.IsAny<long>(), "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }
    }
}
