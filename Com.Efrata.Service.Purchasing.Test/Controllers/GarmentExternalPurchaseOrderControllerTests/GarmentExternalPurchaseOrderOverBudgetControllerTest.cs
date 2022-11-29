using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentExternalPurchaseOrderControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentExternalPurchaseOrderControllerTests
{
	public class GarmentExternalPurchaseOrderOverBudgetControllerTest
	{
		private GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel ViewModel
		{
			get
			{
				return new GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel
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

		private GarmentExternalPurchaseOrderOverBudgetMonitoringController GetController(Mock<IGarmentExternalPurchaseOrderFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentInternalPurchaseOrderFacade> facadeIPO)
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

			var controller = new GarmentExternalPurchaseOrderOverBudgetMonitoringController(servicePMock.Object, facadeM.Object)
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
			var mockFacade = new Mock<IGarmentExternalPurchaseOrderFacade>();
			mockFacade.Setup(x => x.GetEPOOverBudgetReport(null, null, null, null, null, null, 1, 25, "{}", 7))
				.Returns(Tuple.Create(new List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel> { ViewModel }, 25));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel>>(It.IsAny<List<GarmentExternalPurchaseOrder>>()))
				.Returns(new List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel> { ViewModel });

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);
			GarmentExternalPurchaseOrderOverBudgetMonitoringController controller = new GarmentExternalPurchaseOrderOverBudgetMonitoringController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};
			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			var response = controller.GetReport(null, null, null, null, null, null, 1, 25, "{}");
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		}

		[Fact]
		public void Should_Error_Get_Report_Xls_Data()
		{
			var mockFacade = new Mock<IGarmentExternalPurchaseOrderFacade>();
			mockFacade.Setup(x => x.GetEPOOverBudgetReport(null, null, null, null, null, null, 1, 25, "{}", 7))
				.Returns(Tuple.Create(new List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel> { ViewModel }, 25));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel>>(It.IsAny<List<GarmentExternalPurchaseOrder>>()))
				.Returns(new List<GarmentExternalPurchaseOrderOverBudgetMonitoringViewModel> { ViewModel });

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);
			GarmentExternalPurchaseOrderOverBudgetMonitoringController controller = new GarmentExternalPurchaseOrderOverBudgetMonitoringController(GetServiceProvider().Object, mockFacade.Object);
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = user.Object
				}
			};
			controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
			var response = controller.GetXls("no", null, null, null, null, null, 1, 25, "{}");
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		}
	}

}
