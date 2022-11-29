using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
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
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitPaymentCorrectionNoteControllerTests
{
    public class QuantityCerrectionReportTest
    {
        private UnitPaymentQuantityCorrectionNoteReportViewModel ViewModel
        {
            get
            {
                return new UnitPaymentQuantityCorrectionNoteReportViewModel
                {
                    upcNo = "upc123",
                    correctionDate = new DateTimeOffset(),
                    upoNo = "upo123",
                    epoNo = "epo123",
                    prNo = "pr123",
                    notaRetur = "notaretur",
                    vatTaxCorrectionNo = "vattax",
                    vatTaxCorrectionDate = new DateTimeOffset(),
                    unit = "unit",
                    category = "category",
                    supplier = "supplier",
                    productCode= "ProductCode",
                    productName = "ProductName",
                    jumlahKoreksi = 3,
                    satuanKoreksi = "satuan",
                    hargaSatuanKoreksi = 1000,
                    LastModifiedUtc = new DateTime(),
                    hargaTotalKoreksi = 3000,
                    user = "user",
                    jenisKoreksi = "jumlah",
                };
            }
        }
        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
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
        private UnitPaymentQuantityCorrectionNoteReportController GetController(Mock<IUnitPaymentQuantityCorrectionNoteFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateM.Object);

            UnitPaymentQuantityCorrectionNoteReportController controller = new UnitPaymentQuantityCorrectionNoteReportController(servicePMock.Object, mapper.Object, facadeM.Object)
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
        public void Should_Success_Get_Report_Data()
        {
            var mockFacade = new Mock<IUnitPaymentQuantityCorrectionNoteFacade>();
            mockFacade.Setup(x => x.GetReport(null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentQuantityCorrectionNoteReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentQuantityCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentQuantityCorrectionNoteReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentQuantityCorrectionNoteReportController controller = new UnitPaymentQuantityCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Data()
        {
            var mockFacade = new Mock<IUnitPaymentQuantityCorrectionNoteFacade>();
           
            var mockMapper = new Mock<IMapper>();
           

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("usernameeee", "unittestusernameeeee")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentQuantityCorrectionNoteReportController controller = new UnitPaymentQuantityCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(null, null, 1, 25, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data()
        {
            var mockFacade = new Mock<IUnitPaymentQuantityCorrectionNoteFacade>();
            mockFacade.Setup(x => x.GetReport(null, null, It.IsAny<int>(), It.IsAny<int>(), "{}", It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentQuantityCorrectionNoteReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentQuantityCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentQuantityCorrectionNoteReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentQuantityCorrectionNoteReportController controller = new UnitPaymentQuantityCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
            var mockFacade = new Mock<IUnitPaymentQuantityCorrectionNoteFacade>();
            mockFacade.Setup(x => x.GenerateExcel(null, null, It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentQuantityCorrectionNoteReportViewModel>>(It.IsAny<List<UnitPaymentCorrectionNote>>()))
                .Returns(new List<UnitPaymentQuantityCorrectionNoteReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentQuantityCorrectionNoteReportController controller = new UnitPaymentQuantityCorrectionNoteReportController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
