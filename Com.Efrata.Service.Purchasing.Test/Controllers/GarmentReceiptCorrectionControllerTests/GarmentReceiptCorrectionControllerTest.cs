using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentReceiptCorrectionModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
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
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReceiptCorrectionControllerTests
{
    public class GarmentReceiptCorrectionControllerTest
    {

        private GarmentReceiptCorrectionViewModel ViewModel
        {
            get
            {
                return new GarmentReceiptCorrectionViewModel
                {
                    URNNo = "Test",
                    Storage = new StorageViewModel
                    {
                        name = "test",
                        _id = "1",
                        code = "test"
                    },
                    Unit = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                    {
                        Id = "1",
                        Name = "testUnit",
                        Code = "unitCode"
                    },
                    Items = new List<GarmentReceiptCorrectionItemViewModel>
                    {
                        new GarmentReceiptCorrectionItemViewModel()
                    }
                };
            }
        }

        private GarmentReceiptCorrection Model
        {
            get
            {
                return new GarmentReceiptCorrection
                {

                    Items = new List<GarmentReceiptCorrectionItem>
                    {
                        new GarmentReceiptCorrectionItem()
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

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private GarmentReceiptCorrectionController GetController(Mock<IGarmentReceiptCorrectionFacade> facadeMock, Mock<IValidateService> validateMock = null, Mock<IMapper> mapperMock = null)
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

            GarmentReceiptCorrectionController controller = new GarmentReceiptCorrectionController(servicePMock.Object, facadeMock.Object)
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

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentReceiptCorrection>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReceiptCorrectionViewModel>>(It.IsAny<List<GarmentReceiptCorrection>>()))
                .Returns(new List<GarmentReceiptCorrectionViewModel> { ViewModel });

            GarmentReceiptCorrectionController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentReceiptCorrectionController controller = new GarmentReceiptCorrectionController(GetServiceProvider().Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentReceiptCorrectionViewModel>(It.IsAny<GarmentReceiptCorrection>()))
                .Returns(ViewModel);

            GarmentReceiptCorrectionController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            //Setup
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(s => s.Map<GarmentReceiptCorrectionViewModel>(It.IsAny<GarmentReceiptCorrection>()))
                .Returns(()=>null);
                
            //Act
            GarmentReceiptCorrectionController controller = GetController(mockFacade, null, mockMapper); 
            var response = controller.Get(It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentReceiptCorrection>>(It.IsAny<List<GarmentReceiptCorrectionViewModel>>()))
                .Returns(new List<GarmentReceiptCorrection>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentReceiptCorrectionViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentReceiptCorrection>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentReceiptCorrectionViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetByUser()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentReceiptCorrectionViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();
            var facadeMock = new Mock<IGarmentReceiptCorrectionFacade>();
            facadeMock
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Tuple<List<GarmentReceiptCorrection>, int, Dictionary<string, string>>(new List<GarmentReceiptCorrection>(), 1, new Dictionary<string, string>()));

            mapperMock
                .Setup(s => s.Map<List<GarmentReceiptCorrectionViewModel>>(It.IsAny<List<GarmentReceiptCorrection>>()))
                .Returns(new List<GarmentReceiptCorrectionViewModel>());

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.GetByUser();

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_GetByUser()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentReceiptCorrectionViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();
            var facadeMock = new Mock<IGarmentReceiptCorrectionFacade>();
            facadeMock
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Tuple<List<GarmentReceiptCorrection>, int, Dictionary<string, string>>(new List<GarmentReceiptCorrection>(), 1, new Dictionary<string, string>()));

            mapperMock
                .Setup(s => s.Map<List<GarmentReceiptCorrectionViewModel>>(It.IsAny<List<GarmentReceiptCorrection>>()))
                .Throws(new Exception());

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.GetByUser();

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentReceiptCorrectionFacade>();

            var controller = new GarmentReceiptCorrectionController(GetServiceProvider().Object, mockFacade.Object);

            var response = await controller.Post(new GarmentReceiptCorrectionViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
