using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitExpenditureNoteControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentUnitExpenditureNoteControllerTests
{
    public class GarmentUnitExpenditureNoteControllerTest
    {
        private GarmentUnitExpenditureNoteViewModel ViewModel
        {
            get
            {
                return new GarmentUnitExpenditureNoteViewModel
                {
                    UId = null,
                    UENNo = "UENO1234",
                    ExpenditureType = "PROSES",
                    ExpenditureTo = "PROSES",
                    UnitDOId = It.IsAny<int>(),
                    UnitDONo = "UnitDONo",
                    Storage = new Lib.ViewModels.IntegrationViewModel.StorageViewModel(),
                    StorageRequest = new Lib.ViewModels.IntegrationViewModel.StorageViewModel(),
                    UnitSender = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel(),
                    UnitRequest = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel(),
                    Items = new List<GarmentUnitExpenditureNoteItemViewModel>
                    {
                        new GarmentUnitExpenditureNoteItemViewModel
                        {
                            UENId = It.IsAny<int>(),
                            URNItemId = It.IsAny<int>(),
                            DODetailId = It.IsAny<int>(),
                            EPOItemId = It.IsAny<int>(),
                            UnitDOItemId = It.IsAny<int>(),
                            POItemId = It.IsAny<int>(),
                            PRItemId = It.IsAny<int>(),
                            POSerialNumber = "POSerial1234",
                            ProductId = It.IsAny<int>(),
                            ProductCode = "Code",
                            ProductName = "Name",
                            ProductRemark = "remark",
                            RONo = "RONO",
                            UomUnit = "units",
                            PricePerDealUnit = It.IsAny<int>(),
                            FabricType = "SLICK",
                            BuyerId = It.IsAny<int>(),
                            BuyerCode = "COdes",
                            DesignColor = "design",
                            Conversion = 1,
                            DOCurrency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                            {
                                Rate = 1,
                            },
                            BasicPrice = 1,
                            ReturQuantity = 1,
                            ItemStatus = "REJECT"
                        }
                    }
                };
            }
        }

        private GarmentUnitExpenditureNote Model
        {
            get
            {
                return new GarmentUnitExpenditureNote
                {
                    Items = new List<GarmentUnitExpenditureNoteItem>
                    {
                        new GarmentUnitExpenditureNoteItem()
                    }
                };
            }
        }
        private GarmentUnitDeliveryOrder ModelUnitDO
        {
            get
            {
                return new GarmentUnitDeliveryOrder
                {
                    Id = It.IsAny<int>(),
                    Items = new List<GarmentUnitDeliveryOrderItem>
                    {
                        new GarmentUnitDeliveryOrderItem
                        {
                            Id = It.IsAny<int>(),
                            DesignColor = "design",
                            
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

        private GarmentUnitExpenditureNoteController GetController(Mock<IGarmentUnitExpenditureNoteFacade> facadeM, Mock<IGarmentUnitDeliveryOrderFacade> facadeUnitDO, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            if (facadeUnitDO != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IGarmentUnitDeliveryOrderFacade)))
                    .Returns(facadeUnitDO.Object);
            }

            if (facadeM != null)
            {
                servicePMock
                   .Setup(x => x.GetService(typeof(IGarmentUnitExpenditureNoteFacade)))
                   .Returns(facadeM.Object);
            }

            var controller = new GarmentUnitExpenditureNoteController(servicePMock.Object, mapper.Object, facadeM.Object, facadeUnitDO.Object)
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
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            
            var response = controller.GetByUser(filter: "{ 'UENNo': "+ ViewModel.UENNo+" }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();


            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacadeUnitDO.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ModelUnitDO);
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ViewModel);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel {
                    Id = It.IsAny<int>(),
                    Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                    {
                        new GarmentUnitDeliveryOrderItemViewModel
                        {
                            Id = It.IsAny<int>(),
                            DesignColor = "design"
                        }
                    }
                });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucscess_Get_Data_UEN_By_Id()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacadeUnitDO.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ModelUnitDO);
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadByUENId(It.IsAny<int>()))
                .Returns(new GarmentUnitExpenditureNote());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitExpenditureNoteViewModel>(It.IsAny<GarmentUnitExpenditureNote>()))
                .Returns(new GarmentUnitExpenditureNoteViewModel());

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);

            var response = controller.GetByUEN(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns((GarmentUnitExpenditureNoteViewModel)null);
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_UEN_By_Id()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadByUENId(It.IsAny<int>()))
                .Returns((GarmentUnitExpenditureNote)null);
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.GetByUEN(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNote>>(It.IsAny<List<GarmentUnitExpenditureNoteViewModel>>()))
                .Returns(new List<GarmentUnitExpenditureNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentUnitExpenditureNote>()))
               .ReturnsAsync(1);

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.Post(ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = await controller.Post(new GarmentUnitExpenditureNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNote>>(It.IsAny<List<GarmentUnitExpenditureNoteViewModel>>()))
                .Returns(new List<GarmentUnitExpenditureNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitExpenditureNote>()))
               .ReturnsAsync(1);

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = await controller.Put(It.IsAny<int>(), new GarmentUnitExpenditureNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var mockMapper = new Mock<IMapper>();
            var validateMock = new Mock<IValidateService>();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
               .ReturnsAsync(1);
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, null, null, null);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF_By_Id()
        {
            Test_Get_PDF_By_Id();
        }

        private void Test_Get_PDF_By_Id()
        {

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ViewModel);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitExpenditureNoteViewModel>(It.IsAny<GarmentUnitExpenditureNote>()))
                .Returns(ViewModel);

            var serviceProvider = GetServiceProvider();

            var garmentUnitDeliveryOrder = new GarmentUnitDeliveryOrder
            {
                Id = It.IsAny<int>(),
                UnitDONo = "unitdono",
                Article = "Article12345",
                RONo = "RONo12345",
                Items = new List<GarmentUnitDeliveryOrderItem>
                {
                    new GarmentUnitDeliveryOrderItem
                    {
                        Id = It.IsAny<int>(),
                        RONo = "RONO",
                    }
                }
            };

            var mockGarmentUnitDeliveryOrderFacade = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockGarmentUnitDeliveryOrderFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(garmentUnitDeliveryOrder);

            mockMapper.Setup(x => x.Map<GarmentUnitDeliveryOrderViewModel>(It.IsAny<GarmentUnitDeliveryOrder>()))
                .Returns(new GarmentUnitDeliveryOrderViewModel {
                    Id = It.IsAny<int>(),
                    Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                    {
                        new GarmentUnitDeliveryOrderItemViewModel
                        {
                            Id = It.IsAny<int>()
                        }
                    }
                });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockGarmentUnitDeliveryOrderFacade, null, mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_Get_All_Data_UnitExpenditure_for_Preparing()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadForGPreparing(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.GetForGarmentPreparing();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_UnitExpenditure_for_Preparing()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            var response = controller.GetForGarmentPreparing();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data_for_Preparing_Create()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNote>>(It.IsAny<List<GarmentUnitExpenditureNoteViewModel>>()))
                .Returns(new List<GarmentUnitExpenditureNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitExpenditureNote>()))
               .ReturnsAsync(1);

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.PutIsPreparingTrue(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data_for_Preparing_Delete()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNote>>(It.IsAny<List<GarmentUnitExpenditureNoteViewModel>>()))
                .Returns(new List<GarmentUnitExpenditureNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitExpenditureNote>()))
               .ReturnsAsync(1);

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.PutIsPreparingFalse(It.IsAny<int>(), ViewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data_for_Preparing_Create()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = await controller.PutIsPreparingTrue(It.IsAny<int>(), new GarmentUnitExpenditureNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data_for_Preparing_Delete()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = await controller.PutIsPreparingFalse(It.IsAny<int>(), new GarmentUnitExpenditureNoteViewModel());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data_for_DeliveryReturn()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNote>>(It.IsAny<List<GarmentUnitExpenditureNoteViewModel>>()))
                .Returns(new List<GarmentUnitExpenditureNote>());

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentUnitExpenditureNote>()))
               .ReturnsAsync(1);

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockFacadeUnitDO, validateMock, mockMapper);

            var response = await controller.PutReturQuantity(It.IsAny<int>(), new Dictionary<string, double>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data_for_DeliveryReturn()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);

            var response = await controller.PutReturQuantity(It.IsAny<int>(), null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
		private ExpenditureROViewModel expenditureROViewModel
		{
			get
			{
				return new ExpenditureROViewModel
				{
					DetailExpenditureId = It.IsAny<int>(),
					ROAsal = "RONo"
				};
			}
		}

		[Fact]
		public void Should_Success_Get_ROAsalById()
		{
			var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
			mockFacadeUnitDO.Setup(x => x.ReadById(It.IsAny<int>()))
				.Returns(ModelUnitDO);
			var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
			mockFacade.Setup(x => x.GetROAsalById(It.IsAny<int>()))
				.Returns(new ExpenditureROViewModel());

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<ExpenditureROViewModel>(It.IsAny<ExpenditureROViewModel>()))
				.Returns(new ExpenditureROViewModel());

			GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);

			var response = controller.GetROAsal(It.IsAny<int>());
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		}


		[Fact]
		public void Should_Error_Get_ROAsalById()
		{
		
			var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
			var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
			var mockMapper = new Mock<IMapper>();
			GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
			var response = controller.GetROAsal(It.IsAny<int>());
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		}
        //
        [Fact]
        public void Should_Success_Get_UEN_By_Id()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetDataUEN(It.IsAny<int>()))
                .Returns(new List<GarmentUENViewModel> { 
                    new GarmentUENViewModel
                    {
                        UENId = 1,
                        UENNo = "UENNO",
                        UENDate = DateTimeOffset.Now,
                        UnitRequestName = "UnitRequestName",
                        UnitSenderName = "UnitSenderName",
                        FabricType = "FabricType",
                        RONo = "RONo",
                        Quntity = 1,
                        UOMUnit = "UomUnit",
                    },
                    new GarmentUENViewModel
                    {
                        UENId = 1,
                        UENNo = "UENNO",
                        UENDate = DateTimeOffset.Now,
                        UnitRequestName = "UnitRequestName",
                        UnitSenderName = "UnitSenderName",
                        FabricType = "FabricType",
                        RONo = "RONo1",
                        Quntity = 2,
                        UOMUnit = "UomUnit",
                    },
                    }
                );

            var mockMapper = new Mock<IMapper>();

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);
            var response = controller.GetDataById(1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        
        [Fact]
        public void Should_Error_Get_UEN_By_Id()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();

            mockFacade.Setup(x => x.GetDataUEN(It.IsAny<int>()))
                .Returns(new List<GarmentUENViewModel>());

            var mockMapper = new Mock<IMapper>();

            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        //

        //[Fact]
        //public void Should_Succces_Get_Monitoring_Out()
        //{
        //    var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
        //    mockFacade.Setup(x => x.GetReportOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
        //        .Returns(Tuple.Create(new List<MonitoringOutViewModel>(), 25));

        //    var mockMapper = new Mock<IMapper>();
        //    var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    var response = controller.GetMonitoringOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Error_Get_Monitoring_Out()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetReportOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<MonitoringOutViewModel>(), 25));

            var mockMapper = new Mock<IMapper>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetMonitoringOut(null, null, null, 0, 0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        //[Fact]
        //public void Should_Success_Get_Xls_Report_Out()
        //{
        //    var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
        //    mockFacade.Setup(x => x.GenerateExcelMonOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
        //        .Returns(new MemoryStream());

        //    var mockMapper = new Mock<IMapper>();
        //    var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
        //    var response = controller.GetXlsMonOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
        //    Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        //}

        [Fact]
        public void Should_Error_Get_Xls_Report_Out()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();


            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            //controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsMonOut(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Patch_One()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.PatchOne(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentUnitExpenditureNote>>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, new Mock<IGarmentUnitDeliveryOrderFacade>(), new Mock<IValidateService>(), new Mock<IMapper>());

            var response = await controller.PatchOne(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentUnitExpenditureNote>>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Patch_One()
        {
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.PatchOne(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentUnitExpenditureNote>>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, new Mock<IGarmentUnitDeliveryOrderFacade>(), new Mock<IValidateService>(), new Mock<IMapper>());

            var response = await controller.PatchOne(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentUnitExpenditureNote>>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_BasicPriceBy_POSerialNumber()
        {
            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            mockFacadeUnitDO.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(ModelUnitDO);
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetBasicPriceByPOSerialNumber(It.IsAny<string>()))
                .Returns(new GarmentUnitExpenditureNoteItem());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentUnitExpenditureNoteItemViewModel>(It.IsAny<GarmentUnitExpenditureNoteItemViewModel>()))
                .Returns(new GarmentUnitExpenditureNoteItemViewModel());

            GarmentUnitExpenditureNoteController controller = GetController(mockFacade, mockFacadeUnitDO, null, mockMapper);

            var response = controller.GetBasicPrice(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }


        [Fact]
        public void Should_Error_Get_BasicPriceBy_POSerialNumber()
        {

            var mockFacadeUnitDO = new Mock<IGarmentUnitDeliveryOrderFacade>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            var mockMapper = new Mock<IMapper>();
            GarmentUnitExpenditureNoteController controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadeUnitDO.Object);
            var response = controller.GetBasicPrice(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Revise_CreateDate()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.UenDateRevise(It.IsAny<List<long>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(1);

            var mockunitdo = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockunitdo, validateMock, mockMapper);

            var response = controller.UrnReviseDate(DateTime.Now, new List <GarmentUnitExpenditureNoteViewModel> { ViewModel }); ;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Revise_CreateDate()
        {
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();

            var mockunitdo = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = new GarmentUnitExpenditureNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockunitdo.Object);

            var response = controller.UrnReviseDate(DateTime.Now, new List<GarmentUnitExpenditureNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetLoaderByRO_Success()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
                .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
                .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadLoaderProductByROJob(It.IsAny<string>(), "{}", 50))
                .Returns(new List<object>());

            var mockunitdo = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockunitdo, validateMock, mockMapper);
            var response = controller.GetLoaderByRO(It.IsAny<string>(), "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetLoaderByRO_Error()
        {
            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<GarmentUnitExpenditureNoteViewModel>>(It.IsAny<List<GarmentUnitExpenditureNote>>()))
            //    .Returns(new List<GarmentUnitExpenditureNoteViewModel> { ViewModel });

            var validateMock = new Mock<IValidateService>();
            //validateMock.Setup(s => s.Validate(It.IsAny<GarmentUnitExpenditureNoteViewModel>()))
            //    .Verifiable();

            var mockFacade = new Mock<IGarmentUnitExpenditureNoteFacade>();
            //mockFacade.Setup(x => x.ReadLoaderProductByROJob(It.IsAny<string>(), "{}", 1))
            //    .Returns(new List<object>());

            var mockunitdo = new Mock<IGarmentUnitDeliveryOrderFacade>();

            var controller = GetController(mockFacade, mockunitdo, validateMock, mockMapper);
            var response = controller.GetLoaderByRO();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
