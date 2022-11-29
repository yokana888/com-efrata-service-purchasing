using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.BankExpenditureNote;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.BankExpenditureNoteControllerTests
{
    public class BankExpenditureNoteControllerTest
    {
        private BankExpenditureNoteViewModel ViewModel
        {
            get
            {
                return new BankExpenditureNoteViewModel()
                {
                    UId = null,
                    Bank = new AccountBankViewModel() { Currency = new CurrencyViewModel() },
                    CurrencyCode = "Code",
                    CurrencyId = 1,
                    CurrencyRate = 1,
                    Details = new List<BankExpenditureNoteDetailViewModel>() { new BankExpenditureNoteDetailViewModel() { Items = new List<BankExpenditureNoteItemViewModel>() { new BankExpenditureNoteItemViewModel() } } }
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

        private BankExpenditureNoteController GetController(Mock<IBankExpenditureNoteFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            BankExpenditureNoteController controller = new BankExpenditureNoteController(servicePMock.Object, facadeM.Object, mapper.Object)
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

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Data()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>()))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";

            var response = controller.GetReport(null, null, null, null, null, null, null, null, 25, 1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_By_Position_Data()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetAllByPosition(1, int.MaxValue, "{}", null, "{}"))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetAllCashierPosition(1, int.MaxValue, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_DocumentNo()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade
                .Setup(x => x.GetAllByPosition(1, int.MaxValue, "{}", null, "{}"))
                .Returns(new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();
            var mockDocumentNoGenerator = new Mock<IBankDocumentNumberGenerator>();
            mockDocumentNoGenerator
                .Setup(x => x.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("DocumentNo");

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IBankDocumentNumberGenerator)))
                .Returns(mockDocumentNoGenerator.Object);


            var controller = new BankExpenditureNoteController(mockServiceProvider.Object, mockFacade.Object, mockMapper.Object);
            var response = await controller.GetDocumentNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(this.Model);

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        private BankExpenditureNoteModel Model
        {
            get
            {
                return new BankExpenditureNoteModel()
                {
                    Active = true,
                    BankAccountName = "",
                    BankAccountNumber = "",
                    BankCode = "",
                    BankId = 0,
                    BankName = "",
                    BGCheckNumber = "",
                    CreatedAgent = "",
                    CreatedBy = "",
                    CreatedUtc = DateTime.UtcNow,
                    BankCurrencyCode = "",
                    BankCurrencyId = 0,
                    BankCurrencyRate = "",
                    DeletedAgent = "",
                    DeletedBy = "",
                    DeletedUtc = DateTime.UtcNow,
                    Id = 1,
                    IsDeleted = false,
                    Details = new List<BankExpenditureNoteDetailModel>() { new BankExpenditureNoteDetailModel() { Items = new List<BankExpenditureNoteItemModel>() { new BankExpenditureNoteItemModel() { UnitCode = "code" }, new BankExpenditureNoteItemModel() { UnitCode = "code" } } } },
                };
            }
        }

        private BankExpenditureNoteModel ModelIDR
        {
            get
            {
                return new BankExpenditureNoteModel()
                {
                    Active = true,
                    BankAccountName = "",
                    BankAccountNumber = "",
                    BankCode = "",
                    BankId = 0,
                    BankName = "",
                    BGCheckNumber = "",
                    CreatedAgent = "",
                    CreatedBy = "",
                    CreatedUtc = DateTime.UtcNow,
                    BankCurrencyCode = "IDR",
                    BankCurrencyId = 0,
                    BankCurrencyRate = "",
                    DeletedAgent = "",
                    DeletedBy = "",
                    DeletedUtc = DateTime.UtcNow,
                    Id = 1,
                    IsDeleted = false,
                    CurrencyCode = "USD",
                    Details = new List<BankExpenditureNoteDetailModel>() { new BankExpenditureNoteDetailModel() { Items = new List<BankExpenditureNoteItemModel>() { new BankExpenditureNoteItemModel() { UnitCode = "code" }, new BankExpenditureNoteItemModel() { UnitCode = "code" } } } },
                };
            }
        }

        [Fact]
        public async Task Should_Not_Found_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync((BankExpenditureNoteModel)null);

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
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
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
               .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NoContent_PostingData()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Posting(It.IsAny<List<long>>()))
               .ReturnsAsync("1");

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, new Mock<IValidateService>(), mockMapper);

            var response = await controller.Posting(It.IsAny<List<long>>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), identityService))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), identityService))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var IdentityMock = new Mock<IdentityService>();
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), It.IsAny<IdentityService>()))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), identityService))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), identityService))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(0, ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Bad_Request_Id_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), identityService))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(s => s.Map<BankExpenditureNoteModel>(It.IsAny<BankExpenditureNoteViewModel>()))
                .Returns(new BankExpenditureNoteModel());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(1, ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), It.IsAny<IdentityService>()))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(0, this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<IdentityService>()))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Return_Not_Found_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), identityService))
               .ReturnsAsync(0);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",
                Username = "unittestusername"
            };
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<IdentityService>()))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Get_PDF_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(this.Model);

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = await controller.GetById(It.IsAny<int>());
            Assert.NotNull(response);
        }

        //[Fact]
        //public async Task Should_Success_Get_PDF_IDR_NONIDR_By_Id()
        //{
        //    var mockFacade = new Mock<IBankExpenditureNoteFacade>();
        //    mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
        //        .ReturnsAsync(this.ModelIDR);

        //    var mockMapper = new Mock<IMapper>();

        //    BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

        //    var response = await controller.GetById(It.IsAny<int>());
        //    Assert.NotNull(response.GetType().GetProperty("FileStream"));
        //}

        [Fact]
        public void Should_Success_Get_Document_By_Period()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetByPeriod(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<ExpenditureInfo>());
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetBGCheckAndDocumentNoByPeriod(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
    }
}
