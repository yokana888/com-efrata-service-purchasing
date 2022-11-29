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
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class BeacukaiNoFeatureControllerTest
    {
        private BeacukaiNoFeatureViewModel viewModel
        {
            get
            {
                return new BeacukaiNoFeatureViewModel
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

        private BeacukaiNoFeatureController GetController(Mock<IBeacukaiNoFeature> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new BeacukaiNoFeatureController(facadeM.Object, servicePMock.Object)
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

        [Fact]
        public void Should_Success_Get_BCno()
        {
            var mockFacade = new Mock<IBeacukaiNoFeature>();
            mockFacade.Setup(x => x.GetBeacukaiNo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<BeacukaiNoFeatureViewModel> { viewModel });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<BeacukaiNoFeatureViewModel>>(It.IsAny<List<BeacukaiNoFeatureViewModel>>()))
                .Returns(new List<BeacukaiNoFeatureViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            BeacukaiNoFeatureController controller = new BeacukaiNoFeatureController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            var response = controller.GetReportBC(It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_Get_RO()
        {
            var mockFacade = new Mock<IBeacukaiNoFeature>();
            var mockMapper = new Mock<IMapper>();
            BeacukaiNoFeatureController controller = new BeacukaiNoFeatureController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetReportBC(null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }

    

}
