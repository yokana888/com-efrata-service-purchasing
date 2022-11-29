using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitReceiptNoteControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentUnitReceiptNoteControllerTests
{
    public class GarmentDOItemControllerTest
    {
        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        private GarmentDOItemController GetController(Mock<IGarmentDOItemFacade> facadeM)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new GarmentDOItemController(servicePMock.Object, facadeM.Object)
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

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void GetForUnitDO_Success()
        {
            var mockFacade = new Mock<IGarmentDOItemFacade>();
            mockFacade.Setup(x => x.ReadForUnitDO(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<object>());

            GarmentDOItemController controller = GetController(mockFacade);
            var response = controller.GetForUnitDO();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetForUnitDO_Error()
        {
            var mockFacade = new Mock<IGarmentDOItemFacade>();
            mockFacade.Setup(x => x.ReadForUnitDO(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            GarmentDOItemController controller = GetController(mockFacade);
            var response = controller.GetForUnitDO();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetForUnitDOMore_Success()
        {
            var mockFacade = new Mock<IGarmentDOItemFacade>();
            mockFacade.Setup(x => x.ReadForUnitDOMore(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<object>());

            GarmentDOItemController controller = GetController(mockFacade);
            var response = controller.GetForUnitDOMore();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetForUnitDOMore_Error()
        {
            var mockFacade = new Mock<IGarmentDOItemFacade>();
            mockFacade.Setup(x => x.ReadForUnitDOMore(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());

            GarmentDOItemController controller = GetController(mockFacade);
            var response = controller.GetForUnitDOMore();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
