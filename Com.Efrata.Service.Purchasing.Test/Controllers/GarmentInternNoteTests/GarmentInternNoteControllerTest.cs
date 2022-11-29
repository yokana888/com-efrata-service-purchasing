using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInternNoteControllers;
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
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentInternNoteTests
{
    public class GarmentInternNoteControllerTest
    {
        private GarmentInternNoteViewModel ViewModel
        {
            get
            {
                return new GarmentInternNoteViewModel
                {
                    UId = null,
                    supplier = new SupplierViewModel(),
                    currency = new CurrencyViewModel(),
                    items = new List<GarmentInternNoteItemViewModel>
                    {
                        new GarmentInternNoteItemViewModel()
                        {
                            garmentInvoice = new GarmentInvoiceViewModel{
                                Id = 1,
                            },
                            details = new List<GarmentInternNoteDetailViewModel>
                            {
                                new GarmentInternNoteDetailViewModel()
                                {
                                    unit = new UnitViewModel(),
                                    product = new ProductViewModel(),
                                    uomUnit = new UomViewModel(),
                                    deliveryOrder = new Lib.ViewModels.GarmentDeliveryOrderViewModel.GarmentDeliveryOrderViewModel{
                                        Id = 1
                                    },
                                    invoiceDetailId = 1,
                                    dODetailId = 1,
                                }
                            }
                        }
                    }
                };
            }
        }
        private GarmentInternNote Model
        {
            get
            {
                return new GarmentInternNote { };
            }
        }

        private GarmentDeliveryOrder DeliveryOrderModel
        {
            get
            {
                return new GarmentDeliveryOrder {
                    Id = 1,
                    Items = new List<GarmentDeliveryOrderItem> {
                            new GarmentDeliveryOrderItem
                        {
                            Id = 1,
                            Details = new List<GarmentDeliveryOrderDetail>
                            {
                                new GarmentDeliveryOrderDetail
                                {
                                    Id =1,
                                }
                            }
                        }
                    }
                };
            }
        }

        private GarmentDeliveryOrderViewModel DeliveryOrderModelViewModel
        {
            get
            {
                return new GarmentDeliveryOrderViewModel
                {
                    Id = 1,
                    items = new List<GarmentDeliveryOrderItemViewModel>
                    {
                        new GarmentDeliveryOrderItemViewModel
                        {
                            Id = 1,
                            fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                            {
                                new GarmentDeliveryOrderFulfillmentViewModel
                                {
                                    Id = 1,
                                }
                            }
                        }
                    }
                    
                };
            }
        }

        private GarmentInvoice garmentInvoiceModel
        {
            get
            {
                return new GarmentInvoice {
                Id = 1,
                Items = new List<GarmentInvoiceItem>
                {
                    new GarmentInvoiceItem
                    {
                        Id = 1,
                        Details = new List<GarmentInvoiceDetail>
                        {
                            new GarmentInvoiceDetail
                            {
                                Id = 1,
                                DODetailId = 1,
                            }
                        }
                    }
                }
                };
            }
        }

        private GarmentInvoiceViewModel garmentInvoiceViewModel
        {
            get
            {
                return new GarmentInvoiceViewModel
                {
                    Id = 1,
                    items = new List<GarmentInvoiceItemViewModel>
                {
                    new GarmentInvoiceItemViewModel
                    {
                        Id = 1,
                        details = new List<GarmentInvoiceDetailViewModel>
                        {
                            new GarmentInvoiceDetailViewModel
                            {
                                Id = 1,
                                dODetailId = 1,
                            }
                        }
                    }
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

        private GarmentInternNoteController GetController(Mock<IGarmentInternNoteFacade> facadeM, Mock<IGarmentDeliveryOrderFacade> facadeDO , Mock<IValidateService> validateM, Mock<IMapper> mapper,Mock<IGarmentInvoice> facadeINV, Mock<IGarmentCorrectionNoteQuantityFacade> correctionNote = null)
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
            if (correctionNote != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IGarmentCorrectionNoteQuantityFacade)))
                    .Returns(correctionNote.Object);
            }

            GarmentInternNoteController controller = new GarmentInternNoteController(servicePMock.Object, mapper.Object, facadeM.Object,facadeDO.Object, facadeINV.Object)
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
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentInternNote>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternNoteViewModel>>(It.IsAny<List<GarmentInternNote>>()))
                .Returns(new List<GarmentInternNoteViewModel> { ViewModel });

            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentInternNote>(), 0, new Dictionary<string, string>()));

            IPOmockFacade.Setup(x => x.ReadForInternNote(It.IsAny<List<long>>()))
                 .Returns(new List<GarmentDeliveryOrder> { DeliveryOrderModel });

            INVFacade.Setup(x => x.ReadForInternNote(It.IsAny<List<long>>()))
                 .Returns(new List<GarmentInvoice> { garmentInvoiceModel });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternNoteViewModel>>(It.IsAny<List<GarmentInternNote>>()))
                .Returns(new List<GarmentInternNoteViewModel> { ViewModel });
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(new GarmentDeliveryOrderViewModel());
            mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(new GarmentInvoiceViewModel());

            GarmentInternNoteController controller = GetController(mockFacade,IPOmockFacade, validateMock, mockMapper,INVFacade);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            var INVFacade = new Mock<IGarmentInvoice>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNoteViewModel>()))
                .Returns(Model);
            var INVFacade = new Mock<IGarmentInvoice>();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentInternNote>(), false, "unittestusername", 7))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var controller = GetController(mockFacade,IPOmockFacade, validateMock, mockMapper, INVFacade);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var controller = GetController(mockFacade,IPOmockFacade, validateMock, mockMapper, INVFacade);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data_Empty()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNote>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentInternNote>(),false, "unittestusername", 7))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var controller = GetController(mockFacade,IPOmockFacade, validateMock, mockMapper, INVFacade);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
                .Returns(ViewModel);
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(DeliveryOrderModelViewModel);
            mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(garmentInvoiceViewModel);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                 .Returns(DeliveryOrderModel);

            var INVFacade = new Mock<IGarmentInvoice>();
            INVFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                 .Returns(garmentInvoiceModel);

            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade , null, mockMapper, INVFacade);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentInternNote>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternNoteViewModel>>(It.IsAny<List<GarmentInternNote>>()))
                .Returns(new List<GarmentInternNoteViewModel> { ViewModel });
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(new GarmentDeliveryOrderViewModel());
            mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(new GarmentInvoiceViewModel());

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            IPOmockFacade.Setup(x => x.ReadForInternNote(It.IsAny<List<long>>()))
                 .Returns(new List<GarmentDeliveryOrder> { DeliveryOrderModel });

            var INVFacade = new Mock<IGarmentInvoice>();
            INVFacade.Setup(x => x.ReadForInternNote(It.IsAny<List<long>>()))
                 .Returns(new List<GarmentInvoice> { garmentInvoiceModel });

            GarmentInternNoteController controller = GetController(mockFacade,IPOmockFacade, null, mockMapper, INVFacade);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentInternNote>(), false, "unittestusername", 7))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentInternNote>(),false, "unittestusername", 7))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentInternNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade
                .Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
                .Returns(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            //Act
            var controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVFacade);
            var response = controller.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_Delete()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            var mapperMock = new Mock<IMapper>();

            var garmentInternNoteFacadeMock = new Mock<IGarmentInternNoteFacade>();
            garmentInternNoteFacadeMock
                .Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
                .Throws(new Exception());

            var garmentDeliveryOrderFacadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            var GarmentInvoiceMock = new Mock<IGarmentInvoice>();

            //Act
            var controller = GetController(garmentInternNoteFacadeMock, garmentDeliveryOrderFacadeMock, validateMock, mapperMock, GarmentInvoiceMock);
            var response = controller.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentInternNote>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVFacade);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentInternNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_PDF_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentInternNote>(It.IsAny<GarmentInternNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentInternNote>(),false, "unittestusername", 7))
               .ReturnsAsync(1);
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            var response = controller.Get(It.IsAny<int>());
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var INVFacade = new Mock<IGarmentInvoice>();

            var controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVFacade);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentInternNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }
        private GarmentInternNoteViewModel ViewModelPDF
        {

            get
            {
                return new GarmentInternNoteViewModel
                {
                    inNo = "InvoiceNo",
                    inDate = DateTimeOffset.Now,
                    supplier = new SupplierViewModel
                    {
                        Import = true,
                        Id = It.IsAny<int>(),
                        PIC = "importTest",
                        Contact = "0987654",
                        Code = "SupplierImport",
                        Name = "SupplierImport"
                    },
                    currency = new CurrencyViewModel
                    {
                        Id = It.IsAny<int>(),
                        Code = "TEST",
                        Rate = 1,
                        Symbol = "tst"
                    },
                    items = new List<GarmentInternNoteItemViewModel>
                    {
                        new GarmentInternNoteItemViewModel
                        {
                            garmentInvoice = new GarmentInvoiceViewModel
                            {
                                Id = 1,
                                invoiceNo = "1245",
                                invoiceDate =  DateTimeOffset.Now,
                                useVat  =  true,
                                useIncomeTax = true,
                                vatRate = 11,
                                totalAmount=2000,
                                isPayTax = true,
                                isPayVat = false,
                            },

                            details= new List<GarmentInternNoteDetailViewModel>
                            {
                                new GarmentInternNoteDetailViewModel
                                {
                                    ePOId=It.IsAny<int>(),
                                    ePONo="epono",
                                    roNo="12343",
                                    deliveryOrder = new GarmentDeliveryOrderViewModel
                                    {
                                        Id = It.IsAny<int>(),
                                        doNo = "Dono",
                                        doDate = DateTimeOffset.Now,
                                        paymentMethod = "PaymentMethod",
                                        paymentType = "PaymentType",
                                        docurrency = new CurrencyViewModel
                                        {
                                            Id = It.IsAny<int>(),
                                            Code = "IDR",
                                            Rate = 1,
                                        }
                                    },
                                    product= new ProductViewModel
                                    {
                                        Id = "1",
                                        Name="button",
                                        Code="btn"
                                    },
                                    uomUnit= new UomViewModel
                                    {
                                        Id="1",
                                        Unit="ROLL"
                                    },
                                    unit = new UnitViewModel
                                    {
                                        Id = "1",
                                        Name = "UnitName",
                                        Code = "UnitCode"
                                    },
                                    quantity=40,
                                    pricePerDealUnit=5000,
                                    paymentDueDays=2,
                                    poSerialNumber = "PM132434",
                                    priceTotal = 10000
                                    
                                }
                            }
                        }
                    }
                };
            }
        }

        //[Fact]
        //public void Should_Success_Get_PDF_By_Id()
        //{
        //    var validateMock = new Mock<IValidateService>();
        //    validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

        //    var mockFacade = new Mock<IGarmentInternNoteFacade>();
        //    mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //        .Returns(Model);

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
        //        .Returns(ViewModelPDF);

        //    mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
        //        .Returns(new GarmentDeliveryOrderViewModel {
        //            Id = 1,
        //            doNo = "Dono",
        //            doDate = DateTimeOffset.Now,
        //            paymentMethod = "PaymentMethod",
        //            paymentType = "PaymentType",
        //            docurrency = new CurrencyViewModel
        //            {
        //                Id = It.IsAny<int>(),
        //                Code = "IDR",
        //                Rate = 1,
        //            }
        //        });

        //    mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
        //        .Returns(new GarmentInvoiceViewModel { Id = 1, useIncomeTax = true, useVat = true, incomeTaxId = It.IsAny<int>(), incomeTaxRate = 2, isPayTax = true });

        //    var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //    IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //         .Returns(new GarmentDeliveryOrder { Id=1, DOCurrencyRate = 1 });

        //    var INVmockFacade = new Mock<IGarmentInvoice>();
        //    INVmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //         .Returns(new GarmentInvoice());

        //    var mockGarmentCorrectionNoteFacade = new Mock<IGarmentCorrectionNoteQuantityFacade>();
        //    mockGarmentCorrectionNoteFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
        //        .Returns(new List<GarmentCorrectionNote>());

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);

        //    GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVmockFacade, mockGarmentCorrectionNoteFacade);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };
        //    controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

        //    var response = controller.GetInternNotePDF(It.IsAny<int>());
        //    Assert.NotNull(response.GetType().GetProperty("FileStream"));
        //}

        [Fact]
        public void Should_Return_OK_Stream_GetInternNotePDF()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
                .Returns(ViewModelPDF);

            mockMapper
                .Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(new GarmentDeliveryOrderViewModel
                {
                    Id = 1,
                    doNo = "Dono",
                    doDate = DateTimeOffset.Now,
                    paymentMethod = "PaymentMethod",
                    paymentType = "PaymentType",
                    docurrency = new CurrencyViewModel
                    {
                        Id = It.IsAny<int>(),
                        Code = "IDR",
                        Rate = 1,
                    }
                });

            mockMapper
                .Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(new GarmentInvoiceViewModel { Id = 1, useIncomeTax = true, useVat = true, incomeTaxId = It.IsAny<int>(), incomeTaxRate = 2, isPayTax = true });

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            IPOmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder { Id = 1, DOCurrencyRate = 1 });

            var INVmockFacade = new Mock<IGarmentInvoice>();
            INVmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentInvoice());

            var mockGarmentCorrectionNoteFacade = new Mock<IGarmentCorrectionNoteQuantityFacade>();
            mockGarmentCorrectionNoteFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
                .Returns(new List<GarmentCorrectionNote>());

            //Act
            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVmockFacade, mockGarmentCorrectionNoteFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            //Assert
            var response = controller.GetInternNotePDF(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_OK_GetInternNotePDF()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
                .Returns(ViewModelPDF);

            mockMapper
                .Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(new GarmentDeliveryOrderViewModel
                {
                    Id = 1,
                    doNo = "Dono",
                    doDate = DateTimeOffset.Now,
                    paymentMethod = "PaymentMethod",
                    paymentType = "PaymentType",
                    docurrency = new CurrencyViewModel
                    {
                        Id = It.IsAny<int>(),
                        Code = "IDR",
                        Rate = 1,
                    }
                });

            mockMapper
                .Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(new GarmentInvoiceViewModel { Id = 1, useIncomeTax = true, useVat = true, incomeTaxId = It.IsAny<int>(), incomeTaxRate = 2, isPayTax = true });

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            IPOmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder { Id = 1, DOCurrencyRate = 1 });

            var INVmockFacade = new Mock<IGarmentInvoice>();
            INVmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentInvoice());

            var mockGarmentCorrectionNoteFacade = new Mock<IGarmentCorrectionNoteQuantityFacade>();
            mockGarmentCorrectionNoteFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
                .Returns(new List<GarmentCorrectionNote>());

            //Act
            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVmockFacade, mockGarmentCorrectionNoteFacade);
            
            //Assert
            var response = controller.GetInternNotePDF(It.IsAny<int>());
            //Assert.NotNull(response.GetType().GetProperty("FileStream"));
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_OK_PayVatTrue_GetInternNotePDF()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
                .Returns(ViewModelPDF);

            mockMapper
                .Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(new GarmentDeliveryOrderViewModel
                {
                    Id = 1,
                    doNo = "Dono",
                    doDate = DateTimeOffset.Now,
                    paymentMethod = "PaymentMethod",
                    paymentType = "PaymentType",
                    docurrency = new CurrencyViewModel
                    {
                        Id = It.IsAny<int>(),
                        Code = "IDR",
                        Rate = 1,
                    }
                });

            mockMapper
                .Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
                .Returns(new GarmentInvoiceViewModel { Id = 1, useIncomeTax = true, useVat = true, isPayVat = true, incomeTaxId = It.IsAny<int>(), incomeTaxRate = 2, isPayTax = true });

            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            IPOmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder { Id = 1, DOCurrencyRate = 1 });

            var INVmockFacade = new Mock<IGarmentInvoice>();
            INVmockFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentInvoice());

            var mockGarmentCorrectionNoteFacade = new Mock<IGarmentCorrectionNoteQuantityFacade>();
            mockGarmentCorrectionNoteFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
                .Returns(new List<GarmentCorrectionNote>());

            //Act
            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVmockFacade, mockGarmentCorrectionNoteFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            //Assert
            var response = controller.GetInternNotePDF(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Fail_GetInternNotePDF_ById()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

            var facadeMock = new Mock<IGarmentInternNoteFacade>();
            facadeMock
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(() => null);

            var mapperMock = new Mock<IMapper>();
            var garmentDeliveryOrderFacadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            var garmentInvoiceFacadeMock = new Mock<IGarmentInvoice>();
            var garmentCorrectionNoteFacadeMock = new Mock<IGarmentCorrectionNoteQuantityFacade>();
           
            //Act
            GarmentInternNoteController controller = GetController(facadeMock, garmentDeliveryOrderFacadeMock, validateMock, mapperMock, garmentInvoiceFacadeMock, garmentCorrectionNoteFacadeMock);
            var response = controller.GetInternNotePDF(It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Success_Get_PDF_By_Id_False_IsPayTax()
        //{
        //    var validateMock = new Mock<IValidateService>();
        //    validateMock.Setup(s => s.Validate(It.IsAny<GarmentInternNoteViewModel>())).Verifiable();

        //    var mockFacade = new Mock<IGarmentInternNoteFacade>();
        //    mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //        .Returns(Model);

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<GarmentInternNoteViewModel>(It.IsAny<GarmentInternNote>()))
        //        .Returns(ViewModelPDF);

        //    mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
        //        .Returns(new GarmentDeliveryOrderViewModel
        //        {
        //            Id = 1,
        //            doNo = "Dono",
        //            doDate = DateTimeOffset.Now,
        //            paymentMethod = "PaymentMethod",
        //            paymentType = "PaymentType",
        //            docurrency = new CurrencyViewModel
        //            {
        //                Id = It.IsAny<int>(),
        //                Code = "IDR",
        //                Rate = 1,
        //            }
        //        });

        //    mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
        //        .Returns(new GarmentInvoiceViewModel { Id = 1, useIncomeTax = true, useVat = true, incomeTaxId = It.IsAny<int>(), incomeTaxRate = 2, isPayTax = false });

        //    var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //    IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //         .Returns(new GarmentDeliveryOrder { Id = 1, DOCurrencyRate = 1 });

        //    var INVmockFacade = new Mock<IGarmentInvoice>();
        //    INVmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //         .Returns(new GarmentInvoice());

        //    var mockGarmentCorrectionNoteFacade = new Mock<IGarmentCorrectionNoteQuantityFacade>();
        //    mockGarmentCorrectionNoteFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
        //        .Returns(new List<GarmentCorrectionNote>());

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);

        //    GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, validateMock, mockMapper, INVmockFacade, mockGarmentCorrectionNoteFacade);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };
        //    controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

        //    var response = controller.GetInternNotePDF(It.IsAny<int>());
        //    Assert.NotNull(response.GetType().GetProperty("FileStream"));
        //}
        #region Report
        [Fact]
        public void Should_Success_Get_Report()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentInternNoteReportViewModel>(), 5));
            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<GarmentInternNoteViewModel>>(It.IsAny<List<GarmentInternNote>>()))
            //.Returns(new List<GarmentInternNoteViewModel> { ViewModel });
            //var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, null, mockMapper, INVFacade);
            var response = controller.GetReportIN(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        //[Fact]
        //public void Should_Sucess_Get_Excel()
        //{
        //    var mockFacade = new Mock<IGarmentInternNoteFacade>();
        //    mockFacade.Setup(x => x.GenerateExcelIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
        //        .Returns(new MemoryStream());
        //    var mockMapper = new Mock<IMapper>();
        //    var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //    var INVFacade = new Mock<IGarmentInvoice>();
        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
        //    var response = controller.GetXlsDO2(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}
        [Fact]
        public void Should_Error_Get_Report()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            GarmentInternNoteController controller = GetController(mockFacade, IPOmockFacade, null, mockMapper, INVFacade);
            var response = controller.GetReportIN(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_Excel()
        {
            var mockFacade = new Mock<IGarmentInternNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            var INVFacade = new Mock<IGarmentInvoice>();

            GarmentInternNoteController controller = new GarmentInternNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object, INVFacade.Object);
            var response = controller.GetXlsDO2(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        #endregion
    }
}