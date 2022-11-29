using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentCorrectionNoteControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentCorrectionNoteControllerTests
{
    public class GarmentCorrectionNotePriceControllerTest
    {
        private GarmentCorrectionNoteViewModel ViewModel
        {
            get
            {
                return new GarmentCorrectionNoteViewModel
                {
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
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private GarmentCorrectionNotePriceController GetController(Mock<IGarmentCorrectionNotePriceFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> garmentDeliveryOrderFacadeM = null, Mock<IGarmentInternalPurchaseOrderFacade> garmentInternalPurchaseOrderFacadeM = null, Mock<IGarmentInvoice> garmentInvoiceMock = null)
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

            var controller = new GarmentCorrectionNotePriceController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentCorrectionNote>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentCorrectionNoteViewModel>>(It.IsAny<List<GarmentCorrectionNote>>()))
                .Returns(new List<GarmentCorrectionNoteViewModel> { ViewModel });

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentCorrectionNotePriceController controller = new GarmentCorrectionNotePriceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF_By_Id()
        {
            Test_Get_PDF_By_Id(null);
        }

        [Fact]
        public void Should_Success_Get_Harga_Total_PDF_By_Id()
        {
            Test_Get_PDF_By_Id("Harga Total");
        }

        [Fact]
        public void Should_Success_Get_Harga_Satuan_PDF_By_Id()
        {
            Test_Get_PDF_By_Id("Harga Satuan");
        }

        private void Test_Get_PDF_By_Id(string correctionType)
        {
            var Model = this.Model;
            Model.CorrectionType = correctionType;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder { DOCurrencyRate = 1 });

            var mockGarmentInternalPurchaseOrderFacade = new Mock<IGarmentInternalPurchaseOrderFacade>();
            mockGarmentInternalPurchaseOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentInternalPurchaseOrder { UnitCode = "UnitCode" });

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper, mockGarmentDeliveryOrderFacade, mockGarmentInternalPurchaseOrderFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();

            GarmentCorrectionNotePriceController controller = new GarmentCorrectionNotePriceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentCorrectionNote>>(It.IsAny<List<GarmentCorrectionNoteViewModel>>()))
                .Returns(new List<GarmentCorrectionNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentCorrectionNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentCorrectionNote>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentCorrectionNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();

            var controller = new GarmentCorrectionNotePriceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(new GarmentCorrectionNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Ppn_Data_By_Id()
        {
            var ViewModel = this.ViewModel;
            ViewModel.UseVat = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReturnNotePpn(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Ppn_PDF_By_Id()
        {
            Get_Ppn_PDF_By_Id(null);
        }

        [Fact]
        public void Should_Success_Get_Ppn_Harga_Total_PDF_By_Id()
        {
            Get_Ppn_PDF_By_Id("Harga Total");
        }

        [Fact]
        public void Should_Success_Get_Ppn_Harga_Satuan_PDF_By_Id()
        {
            Get_Ppn_PDF_By_Id("Harga Satuan");
        }

        private void Get_Ppn_PDF_By_Id(string correctionType)
        {
            var Model = this.Model;
            Model.CorrectionType = correctionType;

            var ViewModel = this.ViewModel;
            ViewModel.UseVat = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            var garmentdeliveryOrder = new GarmentDeliveryOrder
            {
                DOCurrencyRate = 1,
                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        Details = Model.Items.Select(i => new GarmentDeliveryOrderDetail
                        {
                            Id = i.DODetailId
                        }).ToList()
                    }
                }
            };

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(garmentdeliveryOrder);

            var mockGarmentInvoiceFacade = new Mock<IGarmentInvoice>();
            mockGarmentInvoiceFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
                .Returns(new GarmentInvoice());

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper, mockGarmentDeliveryOrderFacade, null, mockGarmentInvoiceFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.GetReturnNotePpn(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Error_Get_Ppn_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();

            GarmentCorrectionNotePriceController controller = new GarmentCorrectionNotePriceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetReturnNotePpn(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Ppn_Data_Invalid_By_Id()
        {
            var ViewModel = this.ViewModel;
            ViewModel.UseVat = false;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReturnNotePpn(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Pph_Data_By_Id()
        {
            var ViewModel = this.ViewModel;
            ViewModel.UseIncomeTax = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReturnNotePph(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Pph_PDF_By_Id()
        {
            Get_Pph_PDF_By_Id(null);
        }

        [Fact]
        public void Should_Success_Get_Pph_Harga_Total_PDF_By_Id()
        {
            Get_Pph_PDF_By_Id("Harga Total");
        }

        [Fact]
        public void Should_Success_Get_Pph_Harga_Satuan_PDF_By_Id()
        {
            Get_Pph_PDF_By_Id("Harga Satuan");
        }

        private void Get_Pph_PDF_By_Id(string correctionType)
        {
            var Model = this.Model;
            Model.CorrectionType = correctionType;

            var ViewModel = this.ViewModel;
            ViewModel.UseIncomeTax = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            var garmentdeliveryOrder = new GarmentDeliveryOrder
            {
                DOCurrencyRate = 1,
                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        Details = Model.Items.Select(i => new GarmentDeliveryOrderDetail
                        {
                            Id = i.DODetailId
                        }).ToList()
                    }
                }
            };

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(garmentdeliveryOrder);

            var mockGarmentInvoiceFacade = new Mock<IGarmentInvoice>();
            mockGarmentInvoiceFacade.Setup(x => x.ReadByDOId(It.IsAny<int>()))
                .Returns(new GarmentInvoice());

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper, mockGarmentDeliveryOrderFacade, null, mockGarmentInvoiceFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.GetReturnNotePph(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Error_Get_Pph_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();

            GarmentCorrectionNotePriceController controller = new GarmentCorrectionNotePriceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetReturnNotePph(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Pph_Data_Invalid_By_Id()
        {
            var ViewModel = this.ViewModel;
            ViewModel.UseIncomeTax = false;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReturnNotePph(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Pph_PDF_By_Id_NullInvoice()
        {
            Get_Pph_PDF_By_Id_NullInvoice(null);
        }

        private void Get_Pph_PDF_By_Id_NullInvoice(string correctionType)
        {
            var Model = this.Model;
            Model.CorrectionType = correctionType;

            var ViewModel = this.ViewModel;
            ViewModel.UseIncomeTax = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            var garmentdeliveryOrder = new GarmentDeliveryOrder
            {
                DOCurrencyRate = 1,
                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        Details = Model.Items.Select(i => new GarmentDeliveryOrderDetail
                        {
                            Id = i.DODetailId
                        }).ToList()
                    }
                }
            };

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(garmentdeliveryOrder);

            var mockGarmentInvoiceFacade = new Mock<IGarmentInvoice>();
            mockGarmentInvoiceFacade.Setup(x => x.ReadByDOId(1))
                .Returns(new GarmentInvoice());

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper, mockGarmentDeliveryOrderFacade, null, mockGarmentInvoiceFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.GetReturnNotePph(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_Get_Ppn_PDF_By_Id_NullInvoice()
        {
            Get_Ppn_PDF_By_Id_NullInvoice(null);
        }

        private void Get_Ppn_PDF_By_Id_NullInvoice(string correctionType)
        {
            var Model = this.Model;
            Model.CorrectionType = correctionType;

            var ViewModel = this.ViewModel;
            ViewModel.UseVat = true;

            var mockFacade = new Mock<IGarmentCorrectionNotePriceFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentCorrectionNoteViewModel>(It.IsAny<GarmentCorrectionNote>()))
                .Returns(ViewModel);

            var garmentdeliveryOrder = new GarmentDeliveryOrder
            {
                DOCurrencyRate = 1,
                Items = new List<GarmentDeliveryOrderItem>
                {
                    new GarmentDeliveryOrderItem
                    {
                        Details = Model.Items.Select(i => new GarmentDeliveryOrderDetail
                        {
                            Id = i.DODetailId
                        }).ToList()
                    }
                }
            };

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(garmentdeliveryOrder);

            var mockGarmentInvoiceFacade = new Mock<IGarmentInvoice>();
            mockGarmentInvoiceFacade.Setup(x => x.ReadByDOId(1))
                .Returns(new GarmentInvoice());

            GarmentCorrectionNotePriceController controller = GetController(mockFacade, null, mockMapper, mockGarmentDeliveryOrderFacade, null, mockGarmentInvoiceFacade);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.GetReturnNotePpn(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }
    }
}
