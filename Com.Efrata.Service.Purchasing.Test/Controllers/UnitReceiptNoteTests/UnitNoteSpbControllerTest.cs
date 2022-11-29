using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitReceiptNoteTests
{
    public class UnitNoteSpbControllerTest
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

        private UnitNoteSpbController GetController(Mock<IUnitReceiptNoteFacade> facadeMock, Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> autoMapperMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            //var servicePMock = GetServiceProvider();
            //servicePMock
            //    .Setup(x => x.GetService(typeof(IValidateService)))
            //    .Returns(validateM.Object);

            UnitNoteSpbController controller = new UnitNoteSpbController(autoMapperMock.Object, facadeMock.Object, serviceProviderMock.Object)
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
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GetSpbReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new ReadResponse<UnitNoteSpbViewModel>(new List<UnitNoteSpbViewModel>() { new UnitNoteSpbViewModel() }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitNoteSpbController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetSpbReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }


        [Fact]
        public void Should_Fail_Get_All_Data()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GetSpbReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new ReadResponse<UnitNoteSpbViewModel>(new List<UnitNoteSpbViewModel>() { new UnitNoteSpbViewModel() }, 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitNoteSpbController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetSpbReport(null, null, null, null, null, 0, 0, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Xls_Data_Spb()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            mockFacade.Setup(x => x.GenerateExcelSpb(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();

            UnitNoteSpbController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response.GetType().GetProperty("FileDownloadName"));
        }

        [Fact]
        public void Should_Fail_Get_Xls_Data_Spb()
        {
            var mockFacade = new Mock<IUnitReceiptNoteFacade>();

            //mockFacade.Setup(x => x.GenerateExcelSpb(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
            //.Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();

            UnitNoteSpbController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());

            //Assert.Equal((int)HttpStatusCode.InternalServerError, (int)response.GetType().GetProperty("FileDownloadName").GetValue(response, null));
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }



    }
}
