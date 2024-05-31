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
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class AccountingStockReportControllerTest
    {
        private AccountingStockReportViewModel viewModel
        {
            get
            {
                return new AccountingStockReportViewModel
                {
                    BeginningBalancePrice = 0,
                    EndingBalanceQty = 0,
                    EndingBalancePrice = 0,
                    Buyer = "",
                    BeginningBalanceUom = "",
                    BeginningBalanceQty = 0,
                    ExpendKon1APrice = 0,
                    ExpendKon1AQty = 0,
                    ExpendKon2APrice = 0,
                    ExpendKon2AQty = 0,
                    ExpendReturQty = 0,
                    ExpendReturPrice = 0,
                    ExpendRestQty = 0,
                    ExpendRestPrice = 0,
                    ExpendProcessQty = 0,
                    ExpendKon2BPrice = 0,
                    ExpendKon2BQty = 0,
                    ExpendKon2CPrice = 0,
                    ExpendKon2CQty = 0,
                    ExpendKon1BPrice = 0,
                    ExpendKon1BQty = 0,
                    ExpendProcessPrice = 0,
                    ExpendSamplePrice = 0,
                    ExpendSampleQty = 0,
                    NoArticle = "",
                    PlanPo = "",
                    //POId = 0,
                    ProductCode = "",
                    ProductName = "",
                    ReceiptCorrectionPrice = 0,
                    ReceiptCorrectionQty = 0,
                    ReceiptKon1APrice = 0,
                    ReceiptKon1AQty = 0,
                    ReceiptKon2APrice = 0,
                    ReceiptKon2AQty = 0,
                    ReceiptKon2BPrice = 0,
                    ReceiptKon2BQty = 0,
                    ReceiptKon2CPrice = 0,
                    ReceiptKon2CQty = 0,
                    ReceiptKon1BPrice = 0,
                    ReceiptKon1BQty = 0,
                    ReceiptProcessPrice = 0,
                    ReceiptProcessQty = 0,
                    ReceiptPurchasePrice = 0,
                    ReceiptPurchaseQty = 0,
                    RO = ""

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
        private AccountingStockReportController GetController(Mock<IAccountingStockReportFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new AccountingStockReportController(facadeM.Object, servicePMock.Object)
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
        public async Task Should_Success_Get_ReportAsync()
        {
            var mockFacade = new Mock<IAccountingStockReportFacade>();
            mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Tuple.Create(new List<AccountingStockReportViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccountingStockReportViewModel>>(It.IsAny<List<AccountingStockReportViewModel>>()))
                .Returns(new List<AccountingStockReportViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            AccountingStockReportController controller = new AccountingStockReportController(mockFacade.Object, GetServiceProvider().Object);
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

            var response = controller.GetReportAccountStock(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }
        //[Fact]
        //public void Should_Success_Get_Xls()
        //{
        //    var mockFacade = new Mock<IAccountingStockReportFacade>();
        //    mockFacade.Setup(x => x.GenerateExcelAStockReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
        //        .ReturnsAsync(new MemoryStream());

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<AccountingStockReportViewModel>>(It.IsAny<List<AccountingStockReportViewModel>>()))
        //        .Returns(new List<AccountingStockReportViewModel> { viewModel });

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    AccountingStockReportController controller = new AccountingStockReportController(mockFacade.Object, GetServiceProvider().Object);
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

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
        //    var response = controller.GetXlsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}
        [Fact]
        public async Task Should_Error_Get_Report_DataAsync()
        {
            var mockFacade = new Mock<IAccountingStockReportFacade>();
            mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Tuple.Create(new List<AccountingStockReportViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccountingStockReportViewModel>>(It.IsAny<List<AccountingStockReportViewModel>>()))
                .Returns(new List<AccountingStockReportViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            AccountingStockReportController controller = new AccountingStockReportController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReportAccountStock(null, null, null, "", null, 0, 0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Error_Get_Report_Xls_DataAsync()
        {
            var mockFacade = new Mock<IAccountingStockReportFacade>();
            mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Tuple.Create(new List<AccountingStockReportViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccountingStockReportViewModel>>(It.IsAny<List<AccountingStockReportViewModel>>()))
                .Returns(new List<AccountingStockReportViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            AccountingStockReportController controller = new AccountingStockReportController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
