using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.PurchasingDispositionControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.PurchasingDispositionControllerTests
{
    public class PurchasingDispositionControllerTest
    {
        private PurchasingDispositionViewModel ViewModel
        {
            get
            {
                List<PurchasingDispositionItemViewModel> items = new List<PurchasingDispositionItemViewModel>();

                List<PurchasingDispositionDetailViewModel> details = new List<PurchasingDispositionDetailViewModel>();

                items.Add(
                    new PurchasingDispositionItemViewModel
                    {
                        UId = null,
                        IncomeTax = new IncomeTaxViewModel
                        {
                            name = "test",
                            rate = "1",
                            _id = "1"
                        },
                        UseIncomeTax = true,
                        Details = details

                    });

                details.Add(
                    new PurchasingDispositionDetailViewModel
                    {
                        UId = null,
                        //EPODetailId = It.IsAny<string>(),
                        PRId = It.IsAny<string>(),
                        PRNo = "test",
                        
                        PricePerDealUnit = 1000,
                        PriceTotal = 10000,
                        DealQuantity = 10,
                        Product = new ProductViewModel
                        {
                            name = "test"
                        },
                        DealUom = new UomViewModel
                        {
                            unit = "test"
                        },
                        Unit = new UnitViewModel
                        {
                            _id="test",
                            name = "test",

                        }


                    });

                details.Add(
                    new PurchasingDispositionDetailViewModel
                    {
                        UId = null,
                        //EPODetailId = It.IsAny<string>(),
                        PRId = It.IsAny<string>(),
                        PRNo = "test",

                        PricePerDealUnit = 1000,
                        PriceTotal = 10000,
                        DealQuantity = 10,
                        Product = new ProductViewModel
                        {
                            name = "test"
                        },
                        DealUom = new UomViewModel
                        {
                            unit = "test"
                        },
                        Unit = new UnitViewModel
                        {
                            _id = "35",
                            name = "test 35",

                        }


                    });

                details.Add(
                    new PurchasingDispositionDetailViewModel
                    {
                        UId = null,
                        //EPODetailId = It.IsAny<string>(),
                        PRId = It.IsAny<string>(),
                        PRNo = "test",

                        PricePerDealUnit = 1000,
                        PriceTotal = 10000,
                        DealQuantity = 10,
                        Product = new ProductViewModel
                        {
                            name = "test"
                        },
                        DealUom = new UomViewModel
                        {
                            unit = "test"
                        },
                        Unit = new UnitViewModel
                        {
                            _id = "50",
                            name = "test 50",

                        }


                    });

                return new PurchasingDispositionViewModel
                {
                    UId = null,
                    Remark = "Test",
                    Calculation = "axa",
                    Amount = 1000,
                    Category = new CategoryViewModel
                    {
                        _id = "1",
                        name = "Test",
                        code = "test"
                    },
                    Currency = new CurrencyViewModel
                    {
                        code = "test",
                        rate = 1
                    },
                    Supplier = new SupplierViewModel
                    {
                        name = "NameSupp",
                        _id = It.IsAny<string>()
                    },
                    Unit = new UnitViewModel
                    {
                        
                    },
                    Items = items
                };
            }
        }

        private PurchasingDispositionViewModel ViewModel2
        {
            get
            {
                List<PurchasingDispositionItemViewModel> items = new List<PurchasingDispositionItemViewModel>();

                List<PurchasingDispositionDetailViewModel> details = new List<PurchasingDispositionDetailViewModel>();

                items.Add(
                    new PurchasingDispositionItemViewModel
                    {
                        UId = null,
                        IncomeTax = new IncomeTaxViewModel
                        {
                            name = "test",
                            rate = "1",
                            _id = "1"
                        },
                        UseIncomeTax = true,
                        Details = details

                    });

                details.Add(
                    new PurchasingDispositionDetailViewModel
                    {
                        UId = null,
                        //EPODetailId = It.IsAny<string>(),
                        PRId = It.IsAny<string>(),
                        PRNo = "test",

                        PricePerDealUnit = 1000,
                        PriceTotal = 10000,
                        DealQuantity = 10,
                        Product = new ProductViewModel
                        {
                            name = "test"
                        },
                        DealUom = new UomViewModel
                        {
                            unit = "test"
                        },
                        Unit = new UnitViewModel
                        {
                            _id = "test",
                            name = "test"

                        }


                    });

                return new PurchasingDispositionViewModel
                {
                    UId = null,
                    Remark = "Test",
                    Calculation = "axa",
                    Amount = 1000,
                    Category = new CategoryViewModel
                    {
                        _id = "1",
                        name = "Test",
                        code = "test"
                    },
                    Currency = new CurrencyViewModel
                    {
                        code = "test",
                        rate = 1
                    },
                    Supplier = new SupplierViewModel
                    {
                        name = "NameSupp",
                        _id = It.IsAny<string>()
                    },
                    Items = items
                };
            }
        }

        private PurchasingDisposition Model
        {
            get
            {
                return new PurchasingDisposition
                {
                    SupplierId = It.IsAny<string>(),
                    SupplierCode = "SupplierCode",
                    SupplierName = "SupplierName",


                    PaymentMethod = "CASH",

                   // InvoiceNo = "INV000111",


                    Remark = null,

                    PaymentDueDate = new DateTimeOffset(), // ???

                    Items = new List<PurchasingDispositionItem> {

                        new PurchasingDispositionItem
                        {
                            Details=new List<PurchasingDispositionDetail> { }
                        }
                    }
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

        private PurchasingDispositionController GetController(Mock<IPurchasingDispositionFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            PurchasingDispositionController controller = new PurchasingDispositionController(servicePMock.Object, mapper.Object, facadeM.Object)
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
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDisposition>(It.IsAny<PurchasingDispositionViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PurchasingDisposition>(), "unittestusername", 7))
               .ReturnsAsync(1);



            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();


            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDisposition>(It.IsAny<PurchasingDispositionViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PurchasingDisposition>(), "unittestusername", 7))
               .ReturnsAsync(1);


            PurchasingDispositionController controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Success_Get_All_Data_By_User()
        //{
        //    var validateMock = new Mock<IValidateService>();
        //    validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

        //    var mockFacade = new Mock<IPurchasingDispositionFacade>();

        //    mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
        //        .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));
        //    mockFacade.Setup(x => x.GetTotalPaidPrice(It.IsAny<List<PurchasingDispositionViewModel>>())).Returns(new List<PurchasingDispositionViewModel>());
        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
        //        .Returns(new List<PurchasingDispositionViewModel> { ViewModel });


        //    PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
        //    var response = controller.GetByUser();
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}


        [Fact]
        public void GetPDF_Return_OK()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>()))
                .Verifiable();

            var facadeMock = new Mock<IPurchasingDispositionFacade>();
            facadeMock
                .Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());
            
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });

            //Act
            PurchasingDispositionController controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.GetPDF(It.IsAny<int>());
            
            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetPDF_Return_InternalServerError()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>()))
                .Verifiable();

            var facadeMock = new Mock<IPurchasingDispositionFacade>();
            facadeMock
                .Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Throws(new Exception());

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });

            //Act
            PurchasingDispositionController controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.GetPDF(It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            mockFacade.Setup(x => x.GetTotalPaidPrice(It.IsAny<List<PurchasingDispositionViewModel>>())).Returns(new List<PurchasingDispositionViewModel>());

            var mockMapper = new Mock<IMapper>();

            PurchasingDispositionController controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            mockFacade.Setup(x => x.GetTotalPaidPrice(It.IsAny<List<PurchasingDispositionViewModel>>())).Returns(new List<PurchasingDispositionViewModel>());

            var mockMapper = new Mock<IMapper>();

            PurchasingDispositionController controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByUser(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Throws(new Exception("Error Mapping"));

            PurchasingDispositionController controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id_When_InvalidId()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock
                .Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade
                .Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(()=>null);

            //Act
            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            
            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<PurchasingDispositionViewModel>());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();


            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<PurchasingDispositionViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDisposition>(It.IsAny<PurchasingDispositionViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PurchasingDisposition>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<PurchasingDispositionViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Position()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            mockFacade.Setup(x => x.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Position()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Position()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
                .Returns(1);


            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
                .Returns(1);

            var controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_Disposition()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });
            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Getdisposition();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Supplier()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetByDisposition();

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF()
        {

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            var ViewModel = this.ViewModel;
            ViewModel.IncomeTaxBy = "Efrata Garmindo Utama";
            ViewModel.Currency.description = "rupiah";
           // ViewModel.Unit._id = "50";


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_Get_PDF_Except()
        {

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .Returns(new PurchasingDisposition());

            var ViewModel = this.ViewModel2;
            ViewModel.IncomeTaxBy = "Efrata Garmindo Utama";
            ViewModel.Currency.description = "rupiah";
            


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public async Task Should_Error_Get_IsPaidTrue()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<PurchasingDispositionViewModel>(It.IsAny<PurchasingDisposition>()))
                .Returns(ViewModel);

            var mockFacade = new Mock<IPurchasingDispositionFacade>();
            mockFacade.Setup(x => x.UpdatePosition(It.IsAny<PurchasingDispositionUpdatePositionPostedViewModel>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = new PurchasingDispositionController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.SetIsPaidTrue(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        [Fact]
        public void Should_Success_Get_Data_MemoLoader()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetMemoLoader(1);

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_MemoSPBLoader()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionViewModel>())).Verifiable();

            var mockFacade = new Mock<IPurchasingDispositionFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<PurchasingDisposition>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<PurchasingDispositionViewModel>>(It.IsAny<List<PurchasingDisposition>>()))
                .Returns(new List<PurchasingDispositionViewModel> { ViewModel });

            PurchasingDispositionController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetMemoSPBLoader("",1,false,"");

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
