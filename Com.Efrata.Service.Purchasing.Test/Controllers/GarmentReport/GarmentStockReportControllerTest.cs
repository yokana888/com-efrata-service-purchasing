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
    public class GarmentStockReportControllerTest
    {
        private GarmentStockReportViewModel viewModel
        {
            get
            {
                return new GarmentStockReportViewModel
                {

                    ProductCode = "",
                    RO = "",
                    PlanPo = "",
                    NoArticle = "",
                    //ProductName = "",
                    ProductRemark = "",
                    Buyer = "",
                    BeginningBalanceQty = 0,
                    BeginningBalanceUom = "",
                    ReceiptCorrectionQty = 0,
                    ReceiptQty = 0,
                    ReceiptUom = "",
                    ExpendQty = 0,
                    ExpandUom = "",
                    EndingBalanceQty = 0,
                    EndingUom = "",
                    PaymentMethod = ""

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

        private GarmentStockReportController GetController(Mock<IGarmentStockReportFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new GarmentStockReportController(facadeM.Object, servicePMock.Object)
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
			var mockFacade = new Mock<IGarmentStockReportFacade>();
			mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
				.Returns(Tuple.Create(new List<GarmentStockReportViewModel> { viewModel }, 25));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentStockReportViewModel>>(It.IsAny<List<GarmentStockReportViewModel>>()))
				.Returns(new List<GarmentStockReportViewModel> { viewModel });

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);
			GarmentStockReportController controller = new GarmentStockReportController(mockFacade.Object, GetServiceProvider().Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};

		 
		}
		//[Fact]
		//public void Should_Success_Get_Xls()
		//{
		//	var mockFacade = new Mock<IGarmentStockReportFacade>();
		//	mockFacade.Setup(x => x.GenerateExcelStockReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
		//		.Returns(new MemoryStream());

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<List<GarmentStockReportViewModel>>(It.IsAny<List<GarmentStockReportViewModel>>()))
		//		.Returns(new List<GarmentStockReportViewModel> { viewModel });

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);
		//	GarmentStockReportController controller = new GarmentStockReportController(mockFacade.Object, GetServiceProvider().Object);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};

		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
		//	var response = controller.GetXls(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
		//	Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

		//}

		[Fact]
        public void Should_Error_Get_Report_Data()
        {
            var mockFacade = new Mock<IGarmentStockReportFacade>();
            mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Tuple.Create(new List<GarmentStockReportViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentStockReportViewModel>>(It.IsAny<List<GarmentStockReportViewModel>>()))
                .Returns(new List<GarmentStockReportViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentStockReportController controller = new GarmentStockReportController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReportGarmentStock(null, null, null, null, "", 0, 0);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentStockReportFacade>();
            mockFacade.Setup(x => x.GetStockReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Tuple.Create(new List<GarmentStockReportViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentStockReportViewModel>>(It.IsAny<List<GarmentStockReportViewModel>>()))
                .Returns(new List<GarmentStockReportViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentStockReportController controller = new GarmentStockReportController(mockFacade.Object, GetServiceProvider().Object);
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
