using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentCorrectionNoteControllers;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentCorrectionNoteControllerTests
{
    public class GarmentCorrectionNoteControllerTest
    {
        private GarmentCorrectionNoteViewModel ViewModel
        {
            get
            {
                return new GarmentCorrectionNoteViewModel
                {
                    Supplier = new SupplierViewModel(),
                    Items = new List<GarmentCorrectionNoteItemViewModel>
                    {
                        new GarmentCorrectionNoteItemViewModel()
                    }
                };
            }
        }

        private GarmentCorrectionNote Model
        {
            get
            {
                return new GarmentCorrectionNote
                {
                    Items = new List<GarmentCorrectionNoteItem>
                    {
                        new GarmentCorrectionNoteItem()
                    }
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
        private GarmentCorrectionNoteController GetController(Mock<IGarmentCorrectionNoteFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> garmentDeliveryOrderFacadeM = null, Mock<IGarmentInternalPurchaseOrderFacade> garmentInternalPurchaseOrderFacadeM = null, Mock<IGarmentInvoice> garmentInvoiceMock = null)
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

            if (garmentDeliveryOrderFacadeM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                    .Returns(garmentDeliveryOrderFacadeM.Object);
            }

            if (garmentInternalPurchaseOrderFacadeM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IGarmentInternalPurchaseOrderFacade)))
                    .Returns(garmentInternalPurchaseOrderFacadeM.Object);
            }

            if (garmentInvoiceMock != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IGarmentInvoice)))
                    .Returns(garmentInvoiceMock.Object);
            }

            var controller = new GarmentCorrectionNoteController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            var mockFacade = new Mock<IGarmentCorrectionNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentCorrectionNote>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentCorrectionNoteViewModel>>(It.IsAny<List<GarmentCorrectionNote>>()))
                .Returns(new List<GarmentCorrectionNoteViewModel> { ViewModel });

            GarmentCorrectionNoteController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_All_Data() {
            var mockFacade = new Mock<IGarmentCorrectionNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentCorrectionNoteController controller = new GarmentCorrectionNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
