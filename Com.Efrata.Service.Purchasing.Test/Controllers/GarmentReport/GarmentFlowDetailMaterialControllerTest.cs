using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Test.Helpers;
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
    public class GarmentFlowDetailMaterialControllerTest
    {
        private GarmentFlowDetailMaterialViewModel ViewModel
        {
            get
            {
                return new GarmentFlowDetailMaterialViewModel
                {

                };
            }
        }


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

        private GarmentFlowDetailMaterialController GetController(Mock<IGarmentFlowDetailMaterialReport> facadeMock, Mock<IValidateService> validateMock = null, Mock<IMapper> mapperMock = null)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
            new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateMock.Object);
            }
            if (mapperMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IMapper)))
                    .Returns(mapperMock.Object);
            }

            GarmentFlowDetailMaterialController controller = new GarmentFlowDetailMaterialController(servicePMock.Object, facadeMock.Object)
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

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        //[Fact]
        //public void Should_Success_Get_All_Data()
        //{
        //    var mockFacade = new Mock<IGarmentFlowDetailMaterialReport>();
        //    mockFacade.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(),It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .Returns(Tuple.Create(new List<GarmentFlowDetailMaterialViewModel> { ViewModel }, 25));

        //    var mockMapper = new Mock<IMapper>();

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentFlowDetailMaterialController controller = GetController(mockFacade, null, mockMapper);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

        //    var response = controller.GetReport(null, null, null, null, null,  0, 0, "");
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Fail_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentFlowDetailMaterialReport>();

            var mockMapper = new Mock<IMapper>();

            GarmentFlowDetailMaterialController controller = GetController(mockFacade, null, null);
            var response = controller.GetReport(null, null, null, null, null, 0, 0, "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Success_Get_Xls_Data()
        //{
        //    var mockFacade = new Mock<IGarmentFlowDetailMaterialReport>();
        //    mockFacade.Setup(x => x.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
        //        .Returns(new MemoryStream());

        //    var mockMapper = new Mock<IMapper>();


        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentFlowDetailMaterialController controller = GetController(mockFacade, null, mockMapper);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

        //    var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}


        [Fact]
        public void Should_Fail_Get_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentFlowDetailMaterialReport>();

            var mockMapper = new Mock<IMapper>();


            GarmentFlowDetailMaterialController controller = GetController(mockFacade, null, null);

            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }

    }
}
