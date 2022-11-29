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
using static Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports.GarmentReportCMTFacade;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class GarmentReportCMTControllerTest
    {
        public GarmentReportCMTViewModel viewModel
        {
            get
            {
                return new GarmentReportCMTViewModel
                {

                };
            }
        }

        private UnitLoader viewModel2
        {
            get
            {
                return new UnitLoader
                {

                    Code = "",
                    UnitName = ""
                    

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


        private GarmentReportCMTController GetController(Mock<IGarmentReportCMTFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new GarmentReportCMTController(facadeM.Object, servicePMock.Object)
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
        public void Should_Success_Get_Report()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            mockFacade.Setup(x => x.GetReport( It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentReportCMTViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReportCMTViewModel>>(It.IsAny<List<GarmentReportCMTViewModel>>()))
                .Returns(new List<GarmentReportCMTViewModel> { viewModel });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReport( It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(),  It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns( new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReportCMTViewModel>>(It.IsAny<List<GarmentReportCMTViewModel>>()))
                .Returns(new List<GarmentReportCMTViewModel> { viewModel });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            //var dateTo = DateTime.UtcNow.AddDays(1);
            //var dateFrom = dateTo.AddDays(-30);

            var response = controller.GetXlsOUT(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void Should_Success_Get_Master()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            mockFacade.Setup(x => x.Read( It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),  It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>() { new UnitLoader() }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitLoader>>(It.IsAny<List<UnitLoader>>()))
                .Returns(new List<UnitLoader> { viewModel2 });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetUnit( It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_Report()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentReportCMTViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReportCMTViewModel>>(It.IsAny<List<GarmentReportCMTViewModel>>()))
                .Returns(new List<GarmentReportCMTViewModel> { viewModel });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReport(null, null,0,0,0,null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_Report_Xls()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReportCMTViewModel>>(It.IsAny<List<GarmentReportCMTViewModel>>()))
                .Returns(new List<GarmentReportCMTViewModel> { viewModel });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            //var dateTo = DateTime.UtcNow.AddDays(1);
            //var dateFrom = dateTo.AddDays(-30);

            var response = controller.GetXlsOUT(null, null ,0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_Master()
        {
            var mockFacade = new Mock<IGarmentReportCMTFacade>();
            //mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            //    .Returns(new ReadResponse<object>(new List<object>() { new UnitLoader() }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitLoader>>(It.IsAny<List<UnitLoader>>()))
                .Returns(new List<UnitLoader> { viewModel2 });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentReportCMTController controller = new GarmentReportCMTController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetUnit(0,0,null,null,null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
