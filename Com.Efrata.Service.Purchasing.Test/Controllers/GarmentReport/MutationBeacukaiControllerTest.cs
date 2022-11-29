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
    public class MutationBeacukaiControllerTest
    {
        public MutationBBCentralViewModel viewModelBB
        {
            get
            {
                return new MutationBBCentralViewModel
                {

                };
            }
        }

        public MutationBPCentralViewModel viewModelBP
        {
            get
            {
                return new MutationBPCentralViewModel
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


        private MutationBeacukaiController GetController(Mock<IMutationBeacukaiFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new MutationBeacukaiController(facadeM.Object, servicePMock.Object)
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
        public void Should_Success_Get_Report_BBCentral()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportBBCentral(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<MutationBBCentralViewModel> { viewModelBB }, 25));

            var mockMapper = new Mock<AutoMapper.IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
                .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.GetReportBBCentrals(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_BPCentral()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportBPCentral(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<MutationBPCentralViewModel> { viewModelBP }, 25));

            var mockMapper = new Mock<AutoMapper.IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBPCentralViewModel>>(It.IsAny<List<MutationBPCentralViewModel>>()))
                .Returns(new List<MutationBPCentralViewModel> { viewModelBP });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.GetReportBPCentrals(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Success_Get_Report_Xls()
        //{
        //    var mockFacade = new Mock<IMutationBeacukaiFacade>();
        //    mockFacade.Setup(x => x.GenerateExcelBBCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
        //        .Returns(new MemoryStream());

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
        //        .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

        //    var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    //var dateTo = DateTime.UtcNow.AddDays(1);
        //    //var dateFrom = dateTo.AddDays(-30);

        //    var response = controller.GetXlsBBCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}
        //[Fact]
        //public void Should_Success_Get_Report_Xls_BP()
        //{
        //    var mockFacade = new Mock<IMutationBeacukaiFacade>();
        //    mockFacade.Setup(x => x.GenerateExcelBPCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
        //        .Returns(new MemoryStream());

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
        //        .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

        //    var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    //var dateTo = DateTime.UtcNow.AddDays(1);
        //    //var dateFrom = dateTo.AddDays(-30);

        //    var response = controller.GetXlsBPCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}

        [Fact]
        public void Should_Fail_Get_Report_BB()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportBBCentral(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<MutationBBCentralViewModel> { viewModelBB }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
                .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReportBBCentrals(null, null, 0, 0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Fail_Get_Report_BP()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GetReportBPCentral(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<MutationBPCentralViewModel> { viewModelBP }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
                .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReportBPCentrals(null, null, 0, 0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_Report_Xls()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GenerateExcelBBCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
                .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            //var dateTo = DateTime.UtcNow.AddDays(1);
            //var dateFrom = dateTo.AddDays(-30);

            var response = controller.GetXlsBBCentral(null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_Report_Xls_BP()
        {
            var mockFacade = new Mock<IMutationBeacukaiFacade>();
            mockFacade.Setup(x => x.GenerateExcelBPCentral(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MutationBBCentralViewModel>>(It.IsAny<List<MutationBBCentralViewModel>>()))
                .Returns(new List<MutationBBCentralViewModel> { viewModelBB });

            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MutationBeacukaiController controller = new MutationBeacukaiController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            //var dateTo = DateTime.UtcNow.AddDays(1);
            //var dateFrom = dateTo.AddDays(-30);

            var response = controller.GetXlsBPCentral(null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
