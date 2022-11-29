using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Expedition
{
    public class PPHBankExpenditureNoteControllerTest
    {
        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private PPHBankExpenditureNote Model
        {
            get
            {
                return new PPHBankExpenditureNote()
                {
                    Active = true,
                    BankAccountName = "",
                    BankAccountNumber = "",
                    BankCode = "",
                    BankId = "",
                    BankName = "",
                    BGNo = "",
                    CreatedAgent = "",
                    CreatedBy = "",
                    CreatedUtc = DateTime.UtcNow,
                    Currency = "",
                    DeletedAgent = "",
                    DeletedBy = "",
                    DeletedUtc = DateTime.UtcNow,
                    Id = 1,
                    IncomeTaxId = "",
                    IncomeTaxName = "",
                    IncomeTaxRate = 1,
                    IsDeleted = false,
                    Items = new List<PPHBankExpenditureNoteItem>() { new PPHBankExpenditureNoteItem() { PurchasingDocumentExpedition = new PurchasingDocumentExpedition() { Items = new List<PurchasingDocumentExpeditionItem>() { new PurchasingDocumentExpeditionItem() { UnitCode = "1", Price = -1000 }, new PurchasingDocumentExpeditionItem() { UnitCode = "2", Price = 1000 }, new PurchasingDocumentExpeditionItem() { UnitCode = "1", Price = 100000.20 } } } } },
                };
            }
        }

        private PPHBankExpenditureNoteViewModel ViewModel
        {
            get
            {
                return new PPHBankExpenditureNoteViewModel()
                {
                    Date = DateTimeOffset.UtcNow,
                    IncomeTax = new IncomeTaxExpeditionViewModel(),
                    Bank = new Lib.ViewModels.NewIntegrationViewModel.AccountBankViewModel() { Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel() },
                    Division = new Lib.ViewModels.NewIntegrationViewModel.DivisionViewModel(),
                    PPHBankExpenditureNoteItems = new List<UnitPaymentOrderViewModel>() { new UnitPaymentOrderViewModel() { Items = new List<UnitPaymentOrderItemViewModel>() { new UnitPaymentOrderItemViewModel() } } }
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

        private PPHBankExpenditureNoteController GetController(Mock<IPPHBankExpenditureNoteFacade> facadeM, Mock<IValidateService> validateM)
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

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(servicePMock.Object, facadeM.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_Unit_Payment_Order()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetUnitPaymentOrder(null, null, "IncomeTaxName", 2, "IDR", "DivisionCode"))
                .Returns(
                    new List<object>() { new { Id = 1,
                        No = "UPONO",
                        UPODate = DateTime.UtcNow,
                        DueDate = DateTime.UtcNow,
                        InvoiceNo = "InvoiceNo",
                        SupplierCode = "SupplierCode",
                        SupplierName = "SupplierName",
                        CategoryCode = "CategoryCode",
                        CategoryName = "CategoryName",
                        DivisionCode = "DivisionCode",
                        DivisionName = "DivisionName",
                        IncomeTax = 2000,
                        Vat = 1000,
                        TotalPaid = 20000,
                        Currency = "IDR",
                        LastModifiedUtc = DateTime.UtcNow
                    }
               });

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            var response = controller.GetUPO(null, null, "IncomeTaxName", 2, "IDR", "DivisionCode");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NoContent_PostingData()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Posting(It.IsAny<List<long>>()))
               .ReturnsAsync(1);

            PPHBankExpenditureNoteController controller = GetController(mockFacade, new Mock<IValidateService>());

            var response = await controller.Posting(It.IsAny<List<long>>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(this.Model);

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Not_Found_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync((PPHBankExpenditureNote)null);

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
               .Throws(new Exception());

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Id_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Put(1, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<PPHBankExpenditureNote>(), "unittestusername"))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Not_Found_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(0);

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PPHBankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, validateMock);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_PDF_By_Id()
        {
            var mockFacade = new Mock<IPPHBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(this.Model);

            PPHBankExpenditureNoteController controller = new PPHBankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = await controller.GetById(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }
    }
}
