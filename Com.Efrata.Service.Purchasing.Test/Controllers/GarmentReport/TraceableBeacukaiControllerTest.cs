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
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class TraceableBeacukaiControllerTest
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

        private TraceableBeacukaiController GetController(Mock<ITraceableBeacukaiFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new TraceableBeacukaiController(facadeM.Object, servicePMock.Object)
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
        public void Should_Success_Get_Report_Trace_IN()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportTraceableIN(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TraceableInBeacukaiViewModel> { }, 25));


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Success_Fail_Get_Report_Trace_IN()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportTraceableIN(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TraceableInBeacukaiViewModel> { }, 25));


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }

        [Fact]
        public void Should_Success_Get_Xls_Trace_IN()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetTraceableInExcel(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MemoryStream());


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetInExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void Should_Error_Get_Xls_Trace_IN()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetTraceableInExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MemoryStream());


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetInExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Trace_Out()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.getQueryTraceableOut(It.IsAny<string>()))
                .Returns(new List<TraceableOutBeacukaiViewModel> { });


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.GettraceOut(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Trace_Out()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.getQueryTraceableOut(It.IsAny<string>()))
                .Returns(new List<TraceableOutBeacukaiViewModel>());

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
                {
                    new Claim("username", "unittestusername")
                };

            user.Setup(u => u.Claims).Returns(claims);

            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GettraceOut(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_byBUM()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<string>()))
                .Returns(new List<TraceableInWithBUMBeacukaiViewModel> { });


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.Get(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Success_Fail_Get_Report__byBUM()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<string>()))
                .Returns(new List<TraceableInWithBUMBeacukaiViewModel> { });


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.Get(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }

        [Fact]
        public void Should_Success_Get_Xls_Trace_ByBUM()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetExceltracebyBUM(It.IsAny<string>()))
                .Returns(new MemoryStream());


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<string>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void Should_Error_Get_Xls_Trace_ByBUM()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetExceltracebyBUM(It.IsAny<string>()))
                .Returns(new MemoryStream());


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        //[Fact]
        //public void Should_Success_Get_Report_Detail_Trace_Out()
        //{
        //    var mockFacade = new Mock<ITraceableBeacukaiFacade>();
        //    mockFacade.Setup(x => x.getQueryDetail(It.IsAny<string>()))
        //        .Returns(new List<TraceableOutBeacukaiDetailViewModel> { });


        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
        //    controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

        //    var response = controller.GettraceOutDetail(It.IsAny<string>());
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        //[Fact]
        //public void Should_Success_Get_Error_Report_Detail_Trace_Out()
        //{
        //    var mockFacade = new Mock<ITraceableBeacukaiFacade>();
        //    mockFacade.Setup(x => x.getQueryDetail(It.IsAny<string>()))
        //        .Returns(new List<TraceableOutBeacukaiDetailViewModel>());


        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    var response = controller.GettraceOutDetail(It.IsAny<string>());
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Success_Get_Xls_Trace_Out()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetExceltraceOut(It.IsAny<string>()))
                .Returns(new MemoryStream());


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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


            var response = controller.GetXlsOutTraceable(It.IsAny<string>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void Should_Error_Get_Xls_Trace_Out()
        {
            var mockFacade = new Mock<ITraceableBeacukaiFacade>();
            mockFacade.Setup(x => x.GetExceltraceOut(It.IsAny<string>()))
                .Returns(new MemoryStream());

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TraceableBeacukaiController controller = new TraceableBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };


            var response = controller.GetXlsOutTraceable(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
