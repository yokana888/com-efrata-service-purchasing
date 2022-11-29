

using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitReceiptNoteTests
{
    public class UnitReceiptNoteBySupplierUnitControllerTest
    {
        private UnitReceiptNoteViewModel ViewModel
        {
            get
            {
                List<UnitReceiptNoteItemViewModel> items = new List<UnitReceiptNoteItemViewModel>
                {
                    new UnitReceiptNoteItemViewModel()
                    {
                        product = new ProductViewModel()
                        {
                            uom = new UomViewModel()
                        }
                    }
                };

                return new UnitReceiptNoteViewModel
                {
                    storage = new StorageViewModel(),
                    supplier = new SupplierViewModel()
                    {
                        import = false
                    },
                    items = items,
                    unit = new UnitViewModel()
                    {
                        division = new DivisionViewModel()
                    }
                };
            }
        }

        private UnitReceiptNote Model
        {
            get
            {
                return new UnitReceiptNote
                {
                    Items = new List<UnitReceiptNoteItem>()
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

        private UnitReceiptNoteBySupplierUnitController GetController(Mock<IUnitReceiptNoteFacade> facadeMock, Mock<IServiceProvider> serviceProviderMock, Mock<IMapper> autoMapperMock)
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

            UnitReceiptNoteBySupplierUnitController controller = new UnitReceiptNoteBySupplierUnitController(autoMapperMock.Object, facadeMock.Object, serviceProviderMock.Object)
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

            mockFacade.Setup(x => x.ReadBySupplierUnit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<UnitReceiptNote>(new List<UnitReceiptNote>() { Model }, 1, new Dictionary<string, string>()));

            mockFacade.Setup(x => x.GetPurchaseRequestCategoryCode(It.IsAny<int>()))
                .Returns("");

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitReceiptNoteViewModel>>(It.IsAny<List<UnitReceiptNote>>()))
                .Returns(new List<UnitReceiptNoteViewModel> { ViewModel });

            UnitReceiptNoteBySupplierUnitController controller = GetController(mockFacade, GetServiceProvider(), mockMapper);
            var response = controller.GetBySupplierUnit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
    }
}
