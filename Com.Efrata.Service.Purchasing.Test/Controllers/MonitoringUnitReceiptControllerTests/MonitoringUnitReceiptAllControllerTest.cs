using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringUnitReceiptFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.MonitoringUnitReceiptAllViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitReceiptNoteControllers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.MonitoringUnitReceiptControllerTests
{
	//[Collection("TestServerFixture Collection")]
	public class MonitoringUnitReceiptAllControllerTest
	{
		////private const string MediaType = "application/json";
		////private readonly string URI = "v1/unit-receipt-note-monitoring-all";
		////private readonly string USERNAME = "dev2";
		//private TestServerFixture TestFixture { get; set; }
		//private MonitoringUnitReceiptAll ViewModel
		//{
		//	get
		//	{
		//		return new MonitoringUnitReceiptAll
		//		{
		//			no="no"
		//		};
		//	}
		//}


		//private Mock<IServiceProvider> GetServiceProvider()
		//{
		//	var serviceProvider = new Mock<IServiceProvider>();
		//	serviceProvider
		//		.Setup(x => x.GetService(typeof(IdentityService)))
		//		.Returns(new IdentityService() { Token = "Token", Username = "Test" });

		//	serviceProvider
		//		.Setup(x => x.GetService(typeof(IHttpClientService)))
		//		.Returns(new HttpClientTestService());

		//	return serviceProvider;
		//}
		//private MonitoringUnitReceiptNoteAllController GetController(Mock<IMonitoringUnitReceiptAllFacade> facadeM, Mock<IValidateService> validateM)
		//{
		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);

		//	var servicePMock = GetServiceProvider();
		//	if (validateM != null)
		//	{
		//		servicePMock
		//			.Setup(x => x.GetService(typeof(IValidateService)))
		//			.Returns(validateM.Object);
		//	}

		//	var controller = new MonitoringUnitReceiptNoteAllController(servicePMock.Object, facadeM.Object)
		//	{
		//		ControllerContext = new ControllerContext()
		//		{
		//			HttpContext = new DefaultHttpContext()
		//			{
		//				User = user.Object
		//			}
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
		//	controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

		//	return controller;
		//}
		//protected int GetStatusCode(IActionResult response)
		//{
		//	return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
		//}

		//[Fact]
		//public void Should_Error_Get_Report_Data()
		//{
		//	var mockFacade = new Mock<IMonitoringUnitReceiptAllFacade>();
		//	mockFacade.Setup(x => x.GetReport("no","c","b","b", "b", "b", DateTime.Now, DateTime.Now,1,25,"{}",7))
		//		.Returns(Tuple.Create(new List<MonitoringUnitReceiptAll> { ViewModel }, 25));

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);
		//	MonitoringUnitReceiptNoteAllController controller = new MonitoringUnitReceiptNoteAllController(GetServiceProvider().Object,  mockFacade.Object);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
		//	var response = controller.Get(null, null, null, null, null, null, null, null, 1, 25, "{}");
		//	Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		//}

		//[Fact]
		//public void Should_Sucess_Get_Report_Data()
		//{
		//	var mockFacade = new Mock<IMonitoringUnitReceiptAllFacade>();
		//	mockFacade.Setup(x => x.GetReport(null, null, null, null, null, null, null, null, 1, 25, "{}", 7))
		//		.Returns(Tuple.Create(new List<MonitoringUnitReceiptAll> { ViewModel }, 25));

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);
		//	MonitoringUnitReceiptNoteAllController controller = new MonitoringUnitReceiptNoteAllController(GetServiceProvider().Object, mockFacade.Object);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
		//	var response = controller.Get("no", null, null, null, null, null, null, null, 1, 25, "{}");
		//	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		//}

		//[Fact]
		//public void Should_Error_Get_Report_Xls_Data()
		//{
		//	var mockFacade = new Mock<IMonitoringUnitReceiptAllFacade>();
		//	mockFacade.Setup(x => x.GetReport("no", "c", "b", "b", "b", "b", DateTime.Now, DateTime.Now, 1, 25, "{}", 7))
		//		.Returns(Tuple.Create(new List<MonitoringUnitReceiptAll> { ViewModel }, 25)); 

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);
		//	MonitoringUnitReceiptNoteAllController controller = new MonitoringUnitReceiptNoteAllController(GetServiceProvider().Object, mockFacade.Object);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
		//	var response = controller.GetXls(null, null, null, null, null, null, null, null, 1, 25, "{}");
		//	Assert.Null(response.GetType().GetProperty("FileStream"));
		//}

		//[Fact]
		//public void Should_Success_Get_Report_Xls_Data()
		//{

		//	var mockFacade = new Mock<IMonitoringUnitReceiptAllFacade>();
		//	mockFacade.Setup(x => x.GetReport(null, null, null, null, null, null, null, null))
		//		.Returns(Tuple.Create(new List<MonitoringUnitReceiptAll> { ViewModel }, 25));

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);
		//	MonitoringUnitReceiptNoteAllController controller = new MonitoringUnitReceiptNoteAllController(GetServiceProvider().Object, mockFacade.Object);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
		//	var response = controller.GetXls(null, null, null, null, null, null, null, null);
		//	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		//}

	}
}
