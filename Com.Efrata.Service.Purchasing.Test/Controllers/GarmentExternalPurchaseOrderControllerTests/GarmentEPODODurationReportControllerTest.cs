using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentExternalPurchaseOrderControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentExternalPurchaseOrderControllerTests
{
    public class GarmentEPODODurationReportControllerTest
    {
        private GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel ViewModel
        {
            get
            {
                return new GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel
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

        private GarmentExternalPurchaseOrderDeliveryOrderDurationReportController GetController(Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> mapper, Mock<IGarmentExternalPurchaseOrderFacade> facade)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var controller = new GarmentExternalPurchaseOrderDeliveryOrderDurationReportController(serviceProviderMock.Object, mapper.Object, facade.Object)
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
        public void Should_Fail_GetReport()
        {
            //Setup
            var serviceProviderMock = GetServiceProvider();
            var facadeMock = new Mock<IGarmentExternalPurchaseOrderFacade>();
            facadeMock
                .Setup(x => x.GetEPODODurationReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel>>(It.IsAny<List<GarmentExternalPurchaseOrder>>()))
                .Returns(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel });

            //Act
            GarmentExternalPurchaseOrderDeliveryOrderDurationReportController controller =GetController(serviceProviderMock, mapperMock, facadeMock);
            var response = controller.GetReport(null, null, null, null, null, 1, 25, "{}");

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Data()
        {
            var mockFacade = new Mock<IGarmentExternalPurchaseOrderFacade>();
            mockFacade.Setup(x => x.GetEPODODurationReport(null, null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel>>(It.IsAny<List<GarmentExternalPurchaseOrder>>()))
                .Returns(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentExternalPurchaseOrderDeliveryOrderDurationReportController controller = new GarmentExternalPurchaseOrderDeliveryOrderDurationReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(null, null, It.IsAny<string>(), null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentExternalPurchaseOrderFacade>();
            mockFacade.Setup(x => x.GetEPODODurationReport(null, null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel>>(It.IsAny<List<GarmentExternalPurchaseOrder>>()))
                .Returns(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentExternalPurchaseOrderDeliveryOrderDurationReportController controller = new GarmentExternalPurchaseOrderDeliveryOrderDurationReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, null, It.IsAny<string>(), null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentExternalPurchaseOrderFacade>();
            mockFacade.Setup(x => x.GetEPODODurationReport(null, null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentExternalPurchaseOrderDeliveryOrderDurationReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentExternalPurchaseOrderDeliveryOrderDurationReportController controller = new GarmentExternalPurchaseOrderDeliveryOrderDurationReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, null, "0-30 hari", null, null);
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }
    }
}
