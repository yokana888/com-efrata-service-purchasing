using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentDispositionControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentDispositionControllersTest
{
    public class GarmentDispositionControllerTest
    {
        private Mock<IGarmentDispositionPurchaseFacade> _serviceMock;
        private Mock<IGarmentExternalPurchaseOrderFacade> _serviceExternalMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IMapper> _mapperMock;

        private GarmentDispositionController _controller;

        public GarmentDispositionControllerTest()
        {
            _serviceMock = new Mock<IGarmentDispositionPurchaseFacade>();
            _serviceExternalMock = new Mock<IGarmentExternalPurchaseOrderFacade>();
            _mapperMock = new Mock<IMapper>();
            SetDefaultServiceMockProvider();
            SetDefaultController();
        }
        public void SetDefaultServiceMockProvider()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            _serviceProviderMock = serviceProviderMock;
        }
        public void SetDefaultServiceMockProvider(Mock<IGarmentDispositionPurchaseFacade> serviceMock, Mock<IGarmentExternalPurchaseOrderFacade> serviceExternal,Mock<IMapper> mapperMock)
        {
            _serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentDispositionPurchaseFacade)))
                .Returns(serviceMock.Object);
            _serviceProviderMock.Setup(s => s.GetService(typeof(IGarmentExternalPurchaseOrderFacade)))
                .Returns(serviceExternal.Object);
            _serviceProviderMock.Setup(s => s.GetService(typeof(IMapper)))
                .Returns(mapperMock.Object);
            SetDefaultController();
        }
        public void SetDefaultController()
        {
            SetDefaultController(_serviceProviderMock,_serviceMock,_serviceExternalMock,_mapperMock);
        }
        public void SetDefaultController(Mock<IServiceProvider> provider, Mock<IGarmentDispositionPurchaseFacade> serviceMock, Mock<IGarmentExternalPurchaseOrderFacade> serviceExternal, Mock<IMapper> mapperMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            IdentityService identityService = new IdentityService() { Token = "Token", Username = "Test" };

            var controller = new GarmentDispositionController(provider.Object,serviceMock.Object,identityService,mapperMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                },

            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            _controller = controller;
        }

        public void SetDefaultControllerPdf(Mock<IServiceProvider> provider, Mock<IGarmentDispositionPurchaseFacade> serviceMock, Mock<IGarmentExternalPurchaseOrderFacade> serviceExternal, Mock<IMapper> mapperMock)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            IdentityService identityService = new IdentityService() { Token = "Token", Username = "Test" };

            var controller = new GarmentDispositionController(provider.Object, serviceMock.Object, identityService, mapperMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                },

            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            _controller = controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
        public Mock<IGarmentDispositionPurchaseFacade> SetDefaultSuccessService()
        {
            var mockService = new Mock<IGarmentDispositionPurchaseFacade>();

            mockService.Setup(s =>
            s.GetAll(It.IsAny<string>(),It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>(),It.IsAny<string>())
            )
            .ReturnsAsync(new DispositionPurchaseIndexDto( new List<DispositionPurchaseTableDto>(),1,10));

            mockService.Setup(s =>
            s.Read(It.IsAny<PurchasingGarmentExpeditionPosition>(),It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<int>())
            )
            .Returns(Tuple.Create(new List<FormDto>(), 0, new Dictionary<string, string>()));

            mockService.Setup(s =>
            s.GetFormById(It.IsAny<int>(), It.IsAny<bool>())
            )
            .ReturnsAsync(new FormDto());

            mockService.Setup(s =>
            s.ReadByDispositionNo(It.IsAny<string>(),It.IsAny<int>(),It.IsAny<int>())
            )
            .ReturnsAsync(new List<FormDto>());

            mockService.Setup(s =>
            s.Post(It.IsAny<FormDto>())
            )
            .ReturnsAsync(1);

            mockService.Setup(s =>
            s.Update(It.IsAny<FormEditDto>())
            )
            .ReturnsAsync(1);

            mockService.Setup(s =>
            s.Delete(It.IsAny<int>())
            )
            .ReturnsAsync(1);

            mockService.Setup(s =>
            s.ReadByEPOWithDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .Returns(new GarmentExternalPurchaseOrderViewModel());


            mockService.Setup(s =>
            s.SetIsPaidTrue(It.IsAny<string>(),It.IsAny<string>())
            )
            .ReturnsAsync(1);

            return mockService;
        }

        public Mock<IGarmentDispositionPurchaseFacade> SetDefaultExceptionService()
        {
            var mockService = new Mock<IGarmentDispositionPurchaseFacade>();

            mockService.Setup(s =>
            s.GetAll(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.Read(It.IsAny<PurchasingGarmentExpeditionPosition>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.GetFormById(It.IsAny<int>(), It.IsAny<bool>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.ReadByDispositionNo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.Post(It.IsAny<FormDto>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.Update(It.IsAny<FormEditDto>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.Delete(It.IsAny<int>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.ReadByEPOWithDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .Throws(new Exception("Exception Test"));


            mockService.Setup(s =>
            s.SetIsPaidTrue(It.IsAny<string>(), It.IsAny<string>())
            )
            .Throws(new Exception("Exception Test"));

            mockService.Setup(s =>
            s.GetGarmentDispositionPurchase()
            )
            .Throws(new Exception("Exception Test"));

            return mockService;
        }

        [Fact]
        public async Task Get_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock,_serviceExternalMock,_mapperMock);

            var response = await _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Get_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetAl_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetAll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task GetAl_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetAll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetLoader_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetLoader(It.IsAny<PurchasingGarmentExpeditionPosition>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetLoader_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetLoader(It.IsAny<PurchasingGarmentExpeditionPosition>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetAllById_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetPdfAll(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task GetAllById_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetPdfAll(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetAllById_Should_Success_IfPDF()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);
            _serviceMock
                .Setup(service => service.GetFormById(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new FormDto()
                {
                    Bank = "Bank",
                    Category = "Category",
                    ConfirmationOrderNo = "ConfirmationOrderNo",
                    CurrencyCode = "CurrencyCode",
                    CurrencyName = "CurrencyName",
                    DispositionNo = "DispositionNo",
                    PaymentType = "Type",
                    Position = PurchasingGarmentExpeditionPosition.VerificationAccepted,
                    ProformaNo = "ProformaNo",
                    Remark = "Remark",
                    SupplierCode = "SupplierCode",
                    SupplierName = "SupplierName",
                    Items = new List<FormItemDto>()
                    {
                        new FormItemDto()
                        {
                            CurrencyCode = "CurrencyCode",
                            EPONo = "EPONo",
                            IncomeTaxName = "IncomeTaxName",
                            Details = new List<FormDetailDto>()
                            {
                                new FormDetailDto()
                                {
                                    IPONo = "IPONo",
                                    ProductName = "ProductName",
                                    RONo = "RONo",
                                    QTYUnit = "Unit",
                                    UnitCode = "Unit",
                                    UnitName = "Unit"
                                }
                            }
                        }
                    }
                });
            SetDefaultControllerPdf(_serviceProviderMock, _serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetPdfAll(It.IsAny<int>(), It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetAllById_Should_Exception_IfPDF()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);
            SetDefaultControllerPdf(_serviceProviderMock, _serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetPdfAll(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetById_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetById(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task GetById_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetById(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task GetById_Should_Success_IfPDF()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);
            _serviceMock
                .Setup(service => service.GetFormById(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new FormDto()
                {
                    Bank = "Bank",
                    Category = "Category",
                    ConfirmationOrderNo = "ConfirmationOrderNo",
                    CurrencyCode = "CurrencyCode",
                    CurrencyName = "CurrencyName",
                    DispositionNo = "DispositionNo",
                    PaymentType = "Type",
                    Position = PurchasingGarmentExpeditionPosition.VerificationAccepted,
                    ProformaNo = "ProformaNo",
                    Remark = "Remark",
                    SupplierCode = "SupplierCode",
                    SupplierName = "SupplierName",
                    Items = new List<FormItemDto>()
                    {
                        new FormItemDto()
                        {
                            CurrencyCode = "CurrencyCode",
                            EPONo = "EPONo",
                            IncomeTaxName = "IncomeTaxName",
                            Details = new List<FormDetailDto>()
                            {
                                new FormDetailDto()
                                {
                                    IPONo = "IPONo",
                                    ProductName = "ProductName",
                                    RONo = "RONo",
                                    QTYUnit = "Unit",
                                    UnitCode = "Unit",
                                    UnitName = "Unit"
                                }
                            }
                        }
                    }
                });
            SetDefaultControllerPdf(_serviceProviderMock, _serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetById(It.IsAny<int>(), It.IsAny<bool>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetById_Should_Exception_IfPDF()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);
            SetDefaultControllerPdf(_serviceProviderMock, _serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.GetById(It.IsAny<int>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetByDisposition_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetByDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetByDisposition_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetByDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Post_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Post(It.IsAny<FormDto>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Post_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Post(It.IsAny<FormDto>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Put_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Put(It.IsAny<int>(),It.IsAny<FormEditDto>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Put_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Put(It.IsAny<int>(), It.IsAny<FormEditDto>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Delete_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Delete(It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Delete_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.Delete(It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetPOExternalId_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response =  _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetPOExternalId_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetPOExternalId_Should_Exception_IfViewModelIsNull()
        {
            _serviceMock = SetDefaultSuccessService();
            _serviceMock.Setup(s => s.ReadByEPOWithDisposition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(()=> null);
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task SetIsPaidTrue_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.SetIsPaidTrue(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task SetIsPaidTrue_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = await _controller.SetIsPaidTrue(It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetAll_Should_Success()
        {
            _serviceMock = SetDefaultSuccessService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetAll();

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetAll_Should_Exception()
        {
            _serviceMock = SetDefaultExceptionService();
            SetDefaultServiceMockProvider(_serviceMock, _serviceExternalMock, _mapperMock);

            var response = _controller.GetAll();

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
