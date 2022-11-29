using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringCorrectionNoteReceptionViewModel;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReports
{
    public class MonitoringCorrectionNoteReceptionControllerTest
    {
        private MonitoringCorrectionNoteReceptionViewModel ViewModel
        {
            get
            {
                return new MonitoringCorrectionNoteReceptionViewModel
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

        private MonitoringCorrectionNoteReceptionController GetController(Mock<IMonitoringCorrectionNoteReceptionFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> facadeDO)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            var controller = new MonitoringCorrectionNoteReceptionController(servicePMock.Object, facadeM.Object)
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
        public void Should_Error_Get_Report_Data()
        {
            var mockFacade = new Mock<IMonitoringCorrectionNoteReceptionFacade>();
            mockFacade.Setup(x => x.GetMonitoringTerimaNKReport(null, null, null, 1, 25, "{}", 7))
                .Returns(Tuple.Create(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MonitoringCorrectionNoteReceptionViewModel>>(It.IsAny<List<MonitoringCorrectionNoteReceptionViewModel>>()))
                .Returns(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MonitoringCorrectionNoteReceptionController controller = new MonitoringCorrectionNoteReceptionController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(null, null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IMonitoringCorrectionNoteReceptionFacade>();
            mockFacade.Setup(x => x.GetMonitoringTerimaNKReport(null, null, null, 1, 25, "{}", 7))
                .Returns(Tuple.Create(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MonitoringCorrectionNoteReceptionViewModel>>(It.IsAny<List<MonitoringCorrectionNoteReceptionViewModel>>()))
                .Returns(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MonitoringCorrectionNoteReceptionController controller = new MonitoringCorrectionNoteReceptionController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_Report_By_User_Data()
        {
            var mockFacade = new Mock<IMonitoringCorrectionNoteReceptionFacade>();
            mockFacade.Setup(x => x.GetMonitoringTerimaNKByUserReport(null, null, null, 1, 25, "{}", 7))
                .Returns(Tuple.Create(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MonitoringCorrectionNoteReceptionViewModel>>(It.IsAny<List<MonitoringCorrectionNoteReceptionViewModel>>()))
                .Returns(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MonitoringCorrectionNoteReceptionController controller = new MonitoringCorrectionNoteReceptionController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReportByUser(null, null, null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_By_User_Xls_Data()
        {
            var mockFacade = new Mock<IMonitoringCorrectionNoteReceptionFacade>();
            mockFacade.Setup(x => x.GetMonitoringTerimaNKByUserReport(null, null, null, 1, 25, "{}", 7))
                .Returns(Tuple.Create(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MonitoringCorrectionNoteReceptionViewModel>>(It.IsAny<List<MonitoringCorrectionNoteReceptionViewModel>>()))
                .Returns(new List<MonitoringCorrectionNoteReceptionViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MonitoringCorrectionNoteReceptionController controller = new MonitoringCorrectionNoteReceptionController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsByUser(null, null, null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
