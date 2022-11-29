using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReceiptCorrectionControllers;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReceiptCorrectionControllerTests
{
    public class GarmentReceiptCorrectionReportControllerTest
    {

        private GarmentReceiptCorrectionReportViewModel ViewModel
        {
            get
            {
                return new GarmentReceiptCorrectionReportViewModel
                {

                };
            }
        }


        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
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


        private GarmentReceiptCorrectionReportController GetController(Mock<IGarmentReceiptCorrectionReportFacade> facadeMock, Mock<IValidateService> validateMock = null, Mock<IMapper> mapperMock = null)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
            new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateMock.Object);
            }
            if (mapperMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IMapper)))
                    .Returns(mapperMock.Object);
            }

            GarmentReceiptCorrectionReportController controller = new GarmentReceiptCorrectionReportController(servicePMock.Object, facadeMock.Object)
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
            var mockFacade = new Mock<IGarmentReceiptCorrectionReportFacade>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentReceiptCorrectionReportViewModel> { ViewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReceiptCorrectionReportViewModel>>(It.IsAny<List<GarmentReceiptCorrection>>()))
                .Returns(new List<GarmentReceiptCorrectionReportViewModel> { ViewModel });

            GarmentReceiptCorrectionReportController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReport(null, null, null,null,0,0,"");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionReportFacade>();

            var mockMapper = new Mock<IMapper>();

            GarmentReceiptCorrectionReportController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReport(null, null, null, null, 0, 0, "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        [Fact]
        public void Should_Success_Get_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionReportFacade>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReceiptCorrectionReportViewModel>>(It.IsAny<List<GarmentReceiptCorrection>>()))
                .Returns(new List<GarmentReceiptCorrectionReportViewModel> { ViewModel });

            GarmentReceiptCorrectionReportController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),  It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.NotNull(response.GetType().GetProperty("FileDownloadName"));
        }

        [Fact]
        public void Should_Fail_Get_Xls_Data()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionReportFacade>();

            var mockMapper = new Mock<IMapper>();
            

            GarmentReceiptCorrectionReportController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetXls(null, null, null, null, 0, 0, "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
