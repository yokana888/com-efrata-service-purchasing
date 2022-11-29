using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInternalPurchaseOrderControllers;
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
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentInternalPurchaseOrderControllerTests
{
    public class DurationReportControllerTest
    {
        private GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel ViewModel
        {
            get
            {
                return new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel
                {
                    artikelNo= It.IsAny<string>(),
                    buyerName= It.IsAny<string>(),
                    planPO= It.IsAny<string>()
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

        private GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController GetController(Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> mapperMock, Mock<IGarmentInternalPurchaseOrderFacade> facadeMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
           
            var controller = new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController(serviceProviderMock.Object, mapperMock.Object, facadeMock.Object)
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
        public void Should_Success_GetReport()
        {
            //Setup
            var serviceProviderMock = GetServiceProvider();
            var facadeMock = new Mock<IGarmentInternalPurchaseOrderFacade>();
            facadeMock
                .Setup(x => x.GetIPOEPODurationReport(null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel }, 25));

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel });

            //Act
            GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController controller = GetController(serviceProviderMock, mapperMock, facadeMock);
            var response = controller.GetReport(null, It.IsAny<string>(), null, null, 1, 25, "{}");

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }


        [Fact]
        public void Should_Fail_GetReport()
        {
            //Setup
            var serviceProviderMock = GetServiceProvider();
            var facadeMock = new Mock<IGarmentInternalPurchaseOrderFacade>();
            facadeMock
                .Setup(x => x.GetIPOEPODurationReport(null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Throws(new Exception());

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel });

            //Act
            GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController controller = GetController(serviceProviderMock, mapperMock, facadeMock);
            var response = controller.GetReport(null, It.IsAny<string>(), null, null, 1, 25, "{}");

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentInternalPurchaseOrderFacade>();
            mockFacade.Setup(x => x.GetIPOEPODurationReport(null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController controller = new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, "", null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentInternalPurchaseOrderFacade>();
            mockFacade.Setup(x => x.GetIPOEPODurationReport(null, It.IsAny<string>(), null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController controller = new GarmentInternalPurchaseOrderExternalPurchaseOrderDurationReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, It.IsAny<string>(), null, null);
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }
    }
}
