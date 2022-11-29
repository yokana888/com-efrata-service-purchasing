using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitPaymentOrderControllers;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.UnitPaymentOrderControllerTests
{
    public class UnitPaymentOrderControllerTest
    {
        private UnitPaymentOrderViewModel ViewModel
        {
            get
            {
               
                List<UnitPaymentOrderItemViewModel> items = new List<UnitPaymentOrderItemViewModel>();

                List<UnitPaymentOrderDetailViewModel> details = new List<UnitPaymentOrderDetailViewModel>();

                items.Add(
                    new UnitPaymentOrderItemViewModel
                    {
                        unitReceiptNote = new UnitReceiptNote
                        {
                            items = details
                        }
                    });

                details.Add(
                    new UnitPaymentOrderDetailViewModel
                    {
                        pricePerDealUnit = 1000,
                        PricePerDealUnitCorrection = 10000,
                        QuantityCorrection = 10,
                        deliveredQuantity = 10,
                        PriceTotal = 10000,
                        PriceTotalCorrection = 10000,

                    });

                return new UnitPaymentOrderViewModel
                {
                    pibDate = new DateTimeOffset(),
                    importDuty = 0,
                    totalIncomeTaxAmount = 0,
                    totalVatAmount = 0,
                    UId = null,
                    supplier = new SupplierViewModel
                    {
                        import = false
                    },
                    items = items,
                    isPosted = false,
                    position = 100,
                    vatTax = new VatTaxViewModel { 
                        _id= "1",
                        rate = "11"
                    }
                };
            }
        }


        private UnitPaymentOrder Model
        {
            get
            {
                return new UnitPaymentOrder
                {
                    DivisionId = "DivisionId",
                    DivisionCode = "DivisionCode",
                    DivisionName = "DivisionName",

                    SupplierId = "SupplierId",
                    SupplierCode = "SupplierCode",
                    SupplierName = "SupplierName",
                    SupplierAddress = "SupplierAddress",

                    Date = new DateTimeOffset(),

                    CategoryId = "CategoryId ",
                    CategoryCode = "CategoryCode",
                    CategoryName = "CategoryName",

                    CurrencyId = "CurrencyId",
                    CurrencyCode = "CurrencyCode",
                    CurrencyRate = 5,
                    CurrencyDescription = "CurrencyDescription",

                    PaymentMethod = "CASH",

                    InvoiceNo = "INV000111",
                    InvoiceDate = new DateTimeOffset(),
                    PibNo = null,

                    UseIncomeTax = true,
                    IncomeTaxId = "IncomeTaxId",
                    IncomeTaxName = "IncomeTaxName",
                    IncomeTaxRate = 1.5,
                    IncomeTaxNo = "IncomeTaxNo",
                    IncomeTaxDate = new DateTimeOffset(),

                    VatId = "VatId",
                    VatRate = 11,
                    UseVat = false,
                    VatNo = null,
                    VatDate = new DateTimeOffset(),

                    Remark = null,

                    IsPosted = false,
                    Position = 100,

                    DueDate = new DateTimeOffset(), // ???

                    Items = new List<UnitPaymentOrderItem> { }
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

        private UnitPaymentOrderController GetController(Mock<IUnitPaymentOrderFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            UnitPaymentOrderController controller = new UnitPaymentOrderController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentOrderViewModel>>(It.IsAny<List<UnitPaymentOrder>>()))
                .Returns(new List<UnitPaymentOrderViewModel> { ViewModel });

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var Model = this.Model;
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitPaymentOrderViewModel>(It.IsAny<UnitPaymentOrder>()))
                .Throws(new Exception("Error Mapping"));

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Data_By_EPONo()
        {
            var Model = this.Model;
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };
            List<UnitPaymentOrder> paymentOrders = new List<UnitPaymentOrder>();
            paymentOrders.Add(Model);
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadByEPONo(It.IsAny<string>()))
                .Returns(paymentOrders);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentOrderViewModel>>(It.IsAny<List<UnitPaymentOrder>>()))
                .Returns(new List<UnitPaymentOrderViewModel> { ViewModel });

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = controller.GetEpo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_EPONo()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            List<UnitPaymentOrder> paymentOrders = new List<UnitPaymentOrder>();
            paymentOrders.Add(Model);
            mockFacade.Setup(x => x.ReadByEPONo(It.IsAny<string>()))
                .Returns(paymentOrders);

            var mockMapper = new Mock<IMapper>();


            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetEpo(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_PDF_Data_By_Id()
        {
            var Model = this.Model;
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);
            mockFacade.Setup(x => x.GetUnitReceiptNote(It.IsAny<long>()))
                .Returns(new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote { UnitName = "UnitName", ReceiptDate = DateTimeOffset.Now });
            mockFacade.Setup(x => x.GetExternalPurchaseOrder(It.IsAny<string>()))
                .Returns(new ExternalPurchaseOrder { PaymentDueDays = "0" });

            var mockMapper = new Mock<IMapper>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
        public void Should_Success_Get_PDF_Data_By_Id_Except()
        {
            var Model = this.Model;
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);
            mockFacade.Setup(x => x.GetUnitReceiptNote(It.IsAny<long>()))
                .Returns(new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote { UnitId="50", UnitName = "UnitName", ReceiptDate = DateTimeOffset.Now });
            mockFacade.Setup(x => x.GetExternalPurchaseOrder(It.IsAny<string>()))
                .Returns(new ExternalPurchaseOrder { PaymentDueDays = "0" });

            var mockMapper = new Mock<IMapper>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
        public void Should_Success_Get_PDF_Data_By_Id_Except1()
        {
            var Model = this.Model;
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);
            mockFacade.Setup(x => x.GetUnitReceiptNote(It.IsAny<long>()))
                .Returns(new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote { UnitId = "35", UnitName = "UnitName", ReceiptDate = DateTimeOffset.Now });
            mockFacade.Setup(x => x.GetExternalPurchaseOrder(It.IsAny<string>()))
                .Returns(new ExternalPurchaseOrder { PaymentDueDays = "0" });

            var mockMapper = new Mock<IMapper>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
        public void Should_Success_Get_PDF_Data_By_Id_Kredit()
        {
            var Model = this.Model;
            Model.PaymentMethod = "KREDIT";
            Model.Items = new List<UnitPaymentOrderItem>
            {
                new UnitPaymentOrderItem
                {
                    Details = new List<UnitPaymentOrderDetail>
                    {
                        new UnitPaymentOrderDetail()
                    }
                }
            };

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);
            mockFacade.Setup(x => x.GetUnitReceiptNote(It.IsAny<long>()))
                .Returns(new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote { UnitName = "UnitName", ReceiptDate = DateTimeOffset.Now });
            mockFacade.Setup(x => x.GetExternalPurchaseOrder(It.IsAny<string>()))
                .Returns(new ExternalPurchaseOrder { PaymentDueDays = "0" });

            var mockMapper = new Mock<IMapper>();


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
        public void Should_Success_Get_PDF_incomeTaxBy_Supplier()
        {
            var Model = this.Model;
            Model.IncomeTaxBy = "Supplier";
            Model.UseVat = true;
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(Model);
            mockFacade.Setup(x => x.GetUnitReceiptNote(It.IsAny<long>()))
                .Returns(new Lib.Models.UnitReceiptNoteModel.UnitReceiptNote { UnitName = "UnitName", ReceiptDate = DateTimeOffset.Now });
            mockFacade.Setup(x => x.GetExternalPurchaseOrder(It.IsAny<string>()))
                .Returns(new ExternalPurchaseOrder { PaymentDueDays = "0" });

            //var mockMapper = new Mock<IMapper>();

            var ViewModelSupp = this.ViewModel;
            ViewModelSupp.incomeTaxBy = "Supplier";
            ViewModelSupp.useIncomeTax = true;
            //ViewModelSupp.vatTax.rate = "11";
            

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitPaymentOrderViewModel>(It.IsAny<UnitPaymentOrder>()))
                .Returns(ViewModelSupp);


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
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
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UnitPaymentOrder>(It.IsAny<UnitPaymentOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitPaymentOrder>(), "unittestusername", false, 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitPaymentOrder>(), "unittestusername", false, 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<UnitPaymentOrder>(), "unittestusername", false, 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitPaymentOrder>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(1, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitPaymentOrder>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(1, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UnitPaymentOrder>(), "unittestusername"))
               .ThrowsAsync(new Exception("Invalid Id"));

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<UnitPaymentOrderViewModel>())).Verifiable();

            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_Spb()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadSpb(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentOrderViewModel>>(It.IsAny<List<UnitPaymentOrder>>()))
                .Returns(new List<UnitPaymentOrderViewModel> { ViewModel });

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetSpb(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_Spb()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadSpb(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetSpb(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_SpbForVerification()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadSpbForVerification(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentOrderViewModel>>(It.IsAny<List<UnitPaymentOrder>>()))
                .Returns(new List<UnitPaymentOrderViewModel> { ViewModel });

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetSpbForVerification(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_SpbForVerification()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadSpbForVerification(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetSpbForVerification(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Position()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadPositionFiltered(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<UnitPaymentOrderViewModel>>(It.IsAny<List<UnitPaymentOrder>>()))
                .Returns(new List<UnitPaymentOrderViewModel> { ViewModel });

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByPosition(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Position()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.ReadPositionFiltered(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<UnitPaymentOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByPosition(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        #region Monitoring All 
        [Fact]
        public void Should_Success_Get_Report_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            mockFacade.Setup(x => x.GetReportAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentOrderReportViewModel>(), 5));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            var mockMapper = new Mock<IMapper>();


            UnitPaymentOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }



        [Fact]
        public void Should_Success_Get_Xls_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
            //    .Returns(new List<GarmentDeliveryOrderReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_Get_Xls_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = controller.GetXlsAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }
        #endregion

        #region Monitoring Tax All 
        [Fact]
        public void Should_Success_Get_Tax_Report_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.GetReportTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<UnitPaymentOrderTaxReportViewModel>(), 5));

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Tax_Report_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            var mockMapper = new Mock<IMapper>();


            UnitPaymentOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }



        [Fact]
        public void Should_Success_Get_Tax_Xls_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();
            mockFacade.Setup(x => x.GenerateExcelTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
            //    .Returns(new List<GarmentDeliveryOrderReportViewModel> { ViewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_Get_Tax_Xls_All()
        {
            var mockFacade = new Mock<IUnitPaymentOrderFacade>();

            var mockMapper = new Mock<IMapper>();

            UnitPaymentOrderController controller = new UnitPaymentOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = controller.GetXlsTax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }
        #endregion
    }
}
