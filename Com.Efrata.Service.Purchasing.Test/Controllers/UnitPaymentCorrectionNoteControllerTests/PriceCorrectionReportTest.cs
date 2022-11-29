using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitPaymentCorrectionNoteController;
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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitPaymentCorrectionNoteControllerTests
{
    public class PriceCorrectionReportTest
    {
        private UnitPaymentPriceCorrectionNoteReportViewModel ViewModel
        {
            get
            {
                return new UnitPaymentPriceCorrectionNoteReportViewModel
                {
                    supplier = "SupplierName",
                    correctionType = "Harga Satuan",
                    correctionDate = new DateTimeOffset(),
                    prNo = "pr123",
                    upcNo = "upc123",
                    upoNo = "upo123",
                    epoNo = "123",
                    quantity = 1,
                    productName = "ProductName",
                    productCode = "ProductCode",
                    uom = "UomUnit",
                    vatTaxCorrectionDate = null,
                    vatTaxCorrectionNo = "kl",
                    user = "test",
                    LastModifiedUtc = new DateTime(),
                    pricePerDealUnitAfter=2000,
                    priceTotalAfter=1700,
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

        private UnitPaymentPriceCorrectionNoteReportController GetController(Mock<IServiceProvider> serviceProviderMock,Mock<IMapper> mapper, Mock<IUnitPaymentPriceCorrectionNoteFacade> facade)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentPriceCorrectionNoteReportController controller = new UnitPaymentPriceCorrectionNoteReportController(serviceProviderMock.Object, mapper.Object, facade.Object)
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
        public void GetReport_Return_OK()
        {
            //Setup
            var facadeMock = new Mock<IUnitPaymentPriceCorrectionNoteFacade>();
            facadeMock
                .Setup(x => x.GetReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel }, 25));

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<UnitPaymentPriceCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel });

            //Act
            UnitPaymentPriceCorrectionNoteReportController controller = GetController(GetServiceProvider(), mapperMock, facadeMock);
            var response = controller.GetReport(null, null, 1, 25, "{}");

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_Return_InternalServerError()
        {
            //Setup
            var facadeMock = new Mock<IUnitPaymentPriceCorrectionNoteFacade>();
            facadeMock
                .Setup(x => x.GetReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<UnitPaymentPriceCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel });

            //Act
            UnitPaymentPriceCorrectionNoteReportController controller = GetController(GetServiceProvider(), mapperMock, facadeMock);
            var response = controller.GetReport(null, null, 1, 25, "{}");

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IUnitPaymentPriceCorrectionNoteFacade>();
            mockFacade.Setup(x => x.GetReport(null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentPriceCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentPriceCorrectionNoteReportController controller = new UnitPaymentPriceCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IUnitPaymentPriceCorrectionNoteFacade>();
            mockFacade.Setup(x => x.GenerateExcel(null, null, It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentPriceCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentPriceCorrectionNoteReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentPriceCorrectionNoteReportController controller = new UnitPaymentPriceCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(null, null);
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }
    }
}
