using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentPurchaseRequestControllers;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentPurchaseRequestControllerTests
{
	public class GarmentMonitoringPurchaseAllControllerTest
	{
		private MonitoringPurchaseAllUserViewModel ViewModel
		{
			get
			{
				return new MonitoringPurchaseAllUserViewModel
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

		private GarmentMonitoringPurchaseController GetController(Mock<IGarmentPurchaseRequestFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> facadeDO)
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

			var controller = new GarmentMonitoringPurchaseController(servicePMock.Object, facadeM.Object)
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
		public void Should_OK_Get_Report_Data()
		{
			var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
			mockFacade.Setup(s => s.GetMonitoringPurchaseReport(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}",7))
			   .ReturnsAsync(new List<MonitoringPurchaseAllUserViewModel>());

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};

			user.Setup(u => u.Claims).Returns(claims);
			GarmentMonitoringPurchaseController controller = new GarmentMonitoringPurchaseController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};

			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			//Act
			var response = controller.GetReport(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}");

			//Assert
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response.Result));
		}

		[Fact]
		public void Should_Error_Get_Report_Xls_Data()
		{
			var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
			mockFacade.Setup(x => x.GenerateExcelPurchase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}", 7))
				.ReturnsAsync(new MemoryStream());


			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};

			user.Setup(u => u.Claims).Returns(claims);
			GarmentMonitoringPurchaseController controller = new GarmentMonitoringPurchaseController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};

			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			var response = controller.GetXls(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 23, "{}");
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
		}
		[Fact]
		public async Task Should_OK_Get_Report_By_User_Data()
		{
			var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
			mockFacade.Setup(s => s.GetMonitoringPurchaseByUserReport(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}", 7))
			   .ReturnsAsync(new List<MonitoringPurchaseAllUserViewModel>());

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};

			user.Setup(u => u.Claims).Returns(claims);
			GarmentMonitoringPurchaseController controller = new GarmentMonitoringPurchaseController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};

			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			//Act
			Task<IActionResult> response = controller.GetReportByUser(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}");
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response.Result));
		}

		[Fact]
		public void Should_Error_Get_Report_By_User_Xls_Data()
		{
			var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
			mockFacade.Setup(x => x.GenerateExcelByUserPurchase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}",7))
				.ReturnsAsync(new MemoryStream());


			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};

			user.Setup(u => u.Claims).Returns(claims);
			GarmentMonitoringPurchaseController controller = new GarmentMonitoringPurchaseController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};

			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			var response = controller.GetXlsByUser(null, null, null, null, null, null, null, null, null, null, null, null, null, null,1,23,"{}");
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
			 
		}
	}
}
