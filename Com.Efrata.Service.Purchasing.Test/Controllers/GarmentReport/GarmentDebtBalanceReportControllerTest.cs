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
    public class GarmentDebtBalanceReportControllerTest
    {
        private GarmentDebtBalanceViewModel viewModel
        {
            get
            {
                return new GarmentDebtBalanceViewModel
                {
                    BillNo = "",
                    DOCurrencyCode = "",
                    DONo = "",
                    InitialBalance = 0,
                    NoDebit = "",
                    PaymentBill = "",
                    SupplierCode = "",
                    SupplierName = "",
                    TglDebit = null,
                    TotalDebit = 0,
                    TotalEndingBalance = 0,
                    TotalInitialBalance = 0,
                    TotalKredit = 0
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
        private GarmentDebtBalanceReportController GetController(Mock<IGarmentDebtBalanceReportFacade> facadeM, Mock<IMapper> mapper)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new GarmentDebtBalanceReportController(facadeM.Object, mapper.Object, servicePMock.Object)
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
            var mockFacade = new Mock<IGarmentDebtBalanceReportFacade>();
            mockFacade.Setup(x => x.GetDebtBookReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDebtBalanceViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDebtBalanceViewModel>>(It.IsAny<List<GarmentDebtBalanceViewModel>>()))
                .Returns(new List<GarmentDebtBalanceViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDebtBalanceReportController controller = new GarmentDebtBalanceReportController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        [Fact]
        public void Should_Success_Get_Xls()
        {
            var mockFacade = new Mock<IGarmentDebtBalanceReportFacade>();
            mockFacade.Setup(x => x.GenerateExcelDebtReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDebtBalanceViewModel>>(It.IsAny<List<GarmentDebtBalanceViewModel>>()))
                .Returns(new List<GarmentDebtBalanceViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDebtBalanceReportController controller = new GarmentDebtBalanceReportController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }
        [Fact]
        public void Should_Error_Get_Report_Data()
        {
            var mockFacade = new Mock<IGarmentDebtBalanceReportFacade>();
            mockFacade.Setup(x => x.GetDebtBookReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDebtBalanceViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDebtBalanceViewModel>>(It.IsAny<List<GarmentDebtBalanceViewModel>>()))
                .Returns(new List<GarmentDebtBalanceViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDebtBalanceReportController controller = new GarmentDebtBalanceReportController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(0, 0, null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentDebtBalanceReportFacade>();
            mockFacade.Setup(x => x.GetDebtBookReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDebtBalanceViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDebtBalanceViewModel>>(It.IsAny<List<GarmentDebtBalanceViewModel>>()))
                .Returns(new List<GarmentDebtBalanceViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDebtBalanceReportController controller = new GarmentDebtBalanceReportController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
