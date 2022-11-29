using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentSupplierBalanceDebtViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentSupplierBalanceDebtControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentSupplierBalanceDebtControllerTests
{
    public class GarmentSupplierBalanceDebtControllerTest
    {
        private GarmentSupplierBalanceDebtViewModel viewModel
        {
            get
            {
                return new GarmentSupplierBalanceDebtViewModel
                {
                    supplier = new SupplierViewModel { Name = "", Import = false, },
                    codeRequirment = "",
                    currency = new CurrencyViewModel { Code = "", Rate = 0, Symbol = "", Description = "", },
                    totalValas = 0,
                    totalAmountIDR = 0,
                    Year = 0,
                    items = new List<GarmentSupplierBalanceDebtItemViewModel> {
                        new GarmentSupplierBalanceDebtItemViewModel{
                            valas = 0,
                            IDR = 0,
                            deliveryOrder = new GarmentDelivOrderViewModel
                            {
                                arrivalDate = DateTimeOffset.Now,
                                billNo = "Test",
                                dONo = "DonOTest",
                                Id =1,
                                internNo = "InternTest",
                                supplierName = "supplierTest",
                                paymentMethod = "paymentMethodTest",
                                paymentType = "paymentTypeTest"

                            }
                        }
                    }
                };
            }
        }
        private GarmentSupplierBalanceDebt Model
        {
            get
            {
                return new GarmentSupplierBalanceDebt { };
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
        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.viewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }
        private GarmentSupplierBalanceDebtController GetController(Mock<IBalanceDebtFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            var controller = new GarmentSupplierBalanceDebtController(facadeM.Object, mapper.Object, servicePMock.Object)
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
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentSupplierBalanceDebt>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentSupplierBalanceDebtViewModel>>(It.IsAny<List<GarmentSupplierBalanceDebt>>()))
                .Returns(new List<GarmentSupplierBalanceDebtViewModel> { viewModel });

            GarmentSupplierBalanceDebtController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentSupplierBalanceDebt>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentSupplierBalanceDebtViewModel>>(It.IsAny<List<GarmentSupplierBalanceDebt>>()))
                .Returns(new List<GarmentSupplierBalanceDebtViewModel> { viewModel });

            GarmentSupplierBalanceDebtController controller = new GarmentSupplierBalanceDebtController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentSupplierBalanceDebtViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentSupplierBalanceDebt>(It.IsAny<GarmentSupplierBalanceDebtViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IBalanceDebtFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentSupplierBalanceDebt>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentSupplierBalanceDebtViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentSupplierBalanceDebt>(It.IsAny<GarmentSupplierBalanceDebtViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IBalanceDebtFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentSupplierBalanceDebt>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new GarmentSupplierBalanceDebtController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object); ;

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Loader()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.ReadLoader(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            GarmentSupplierBalanceDebtController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetLoader(year:0);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Loader_With_year()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.ReadLoader(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            GarmentSupplierBalanceDebtController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetLoader(year: DateTime.Now.Year);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_Loader()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.ReadLoader(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            GarmentSupplierBalanceDebtController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetLoader();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentSupplierBalanceDebt());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentSupplierBalanceDebtViewModel>(It.IsAny<GarmentSupplierBalanceDebt>()))
                .Returns(viewModel);

            GarmentSupplierBalanceDebtController controller = new GarmentSupplierBalanceDebtController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBalanceDebtFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentSupplierBalanceDebt());

            var mockMapper = new Mock<IMapper>();

            GarmentSupplierBalanceDebtController controller = new GarmentSupplierBalanceDebtController(mockFacade.Object, mockMapper.Object, GetServiceProvider().Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentSupplierBalanceDebtViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IBalanceDebtFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }


       


    }
}
