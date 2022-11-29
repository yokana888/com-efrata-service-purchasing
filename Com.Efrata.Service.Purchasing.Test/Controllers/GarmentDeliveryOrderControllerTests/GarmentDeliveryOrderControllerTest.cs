using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentDeliveryOrderControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentDeliveryOrderControllerTests
{
    public class GarmentDeliveryOrderControllerTest
    {
        private GarmentDeliveryOrderViewModel ViewModel
        {
            get
            {
                return new GarmentDeliveryOrderViewModel
                {
                    supplier = new SupplierViewModel(),
                    docurrency = new CurrencyViewModel(),
                    incomeTax = new IncomeTaxViewModel(),
                    items = new List<GarmentDeliveryOrderItemViewModel>
                    {
                        new GarmentDeliveryOrderItemViewModel()
                        {
                            currency = new CurrencyViewModel(),
                            fulfillments = new List<GarmentDeliveryOrderFulfillmentViewModel>
                            {
                                new GarmentDeliveryOrderFulfillmentViewModel()
                                {
                                    unit = new UnitViewModel(),
                                    product = new GarmentProductViewModel(),
                                    purchaseOrderUom = new UomViewModel(),
                                    smallUom = new UomViewModel(),

                                }
                            }
                        }
                    }
                };
            }
        }

        private GarmentDeliveryOrder Model
        {
            get
            {
                return new GarmentDeliveryOrder { };
            }
        }

        private AccuracyOfArrivalReportViewModel ViewModelAccuracyArrival
        {
            get
            {
                return new AccuracyOfArrivalReportViewModel
                {
                    supplier = new SupplierViewModel(),
                    product = new GarmentProductViewModel(),
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

        private GarmentDeliveryOrderController GetController(Mock<IGarmentDeliveryOrderFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrder>(It.IsAny<GarmentDeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();

            var facadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            facadeMock
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = await controller.Delete(1);

            //Assert
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetReportDetail()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();

            var facadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            facadeMock
                .Setup(x => x.GetAccuracyOfArrivalDetail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
                .Returns(new List<AccuracyOfArrivalReportDetail>() { new AccuracyOfArrivalReportDetail()});

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response =  controller.GetReportDetail(null,null,null,null,0);

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_GetReportDetail2()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();

            var facadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            facadeMock
                .Setup(x => x.GetReportDetailAccuracyofDelivery(It.IsAny<string>(),  It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Tuple<List<AccuracyOfArrivalReportViewModel>, int>(new List<AccuracyOfArrivalReportViewModel>(),1));

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.GetReportDetail2(null, null, null, null,null);

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Fail_Delete()
        {
            //Setup
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mapperMock = new Mock<IMapper>();

            var facadeMock = new Mock<IGarmentDeliveryOrderFacade>();
            facadeMock
                .Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = await controller.Delete(1);

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrder>(It.IsAny<GarmentDeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser(filter: "{ 'IsClosed': false }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Loader()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadLoader(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 1, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetLoader();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Loader()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadLoader(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetLoader();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
                .Returns(ViewModel);

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrder>(It.IsAny<GarmentDeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentDeliveryOrderViewModel>(), It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }
        //       
        [Fact]
        public void Should_Success_Get_Data_DO_By_Id()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetDataDO(1))
                .Returns(new List<GarmentDOUrnViewModel>
                        {
                         new GarmentDOUrnViewModel
                                 {
                                   DOId = 1,
                                   DONo = "DONo",
                                   BCNo = "BeacukaiNo",
                                   BCType = "CustomsType",
                                   URNNo = "URNNo",
                                 },
                         new GarmentDOUrnViewModel
                                 {
                                   DOId = 1,
                                   DONo = "DONo",
                                   BCNo = "BeacukaiNo",
                                   BCType = "CustomsType",
                                   URNNo = "URNNo1",
                                 },
                         }
                );

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = controller.GetDataDOById(1);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_DO_By_Id()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GetDataDO(1))
               .Returns(new List<GarmentDOUrnViewModel>());

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(0);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        //

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentDeliveryOrder>(It.IsAny<GarmentDeliveryOrderViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentDeliveryOrderViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

		[Fact]
		public void Should_Error_Get_Data_By_Supplier()
		{
			var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

			mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
				.Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
				.Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

			GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
			var response = controller.GetBySupplier();
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		}

		[Fact]
		public void Should_Error_Get_Data_For_Customs()
		{
			var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

			mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
				.Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
				.Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

			GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
			var response = controller.GetForCustoms();
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		}
		[Fact]
		public void Should_Error_Get_Data_is_Received()
		{
			var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

			mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
				.Returns(Tuple.Create(new List<GarmentDeliveryOrder>(), 0, new Dictionary<string, string>()));
			
			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
				.Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });
			List<int> listID = new List<int>();
			listID.Add(1);

			GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
			var response = controller.GetIsReceived(listID);
			Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		}

        [Fact]
        public void Should_Success_Get_All_Data_For_UnitReceiptNote()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadForUnitReceiptNote(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetForUnitReceiptNote();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_For_UnitReceiptNote()
        {
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, null, null);
            var response = controller.GetForUnitReceiptNote();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_For_CorrectionNoteQuantity()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.ReadForCorrectionNoteQuantity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));


            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetForCorrectionNoteQuantity();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_For_CorrectionNoteQuantity()
        {
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, null, null);
            var response = controller.GetForCorrectionNoteQuantity();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Data_Arrival_Header()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetAccuracyOfArrivalHeader(null, null, null, 0))
                .Returns(new Lib.Facades.GarmentDeliveryOrderFacades.AccuracyOfArrivalReportHeaderResult());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport(null, null, null, 0);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data_Header_AccuracyArrival()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportHeaderAccuracyofArrival(null, null, null, It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsArrivalHeader(null, null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data_Header_AccuracyArrival()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportHeaderAccuracyofArrival(null, null, null, It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsArrivalHeader(null, null, null);
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }

        //[Fact]
        //public void Should_Success_Get_Report_Data_Arrival_Detail()
        //{
        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";

        //    var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //    mockFacade.Setup(x => x.GetReportDetailAccuracyofArrival($"BuyerCode{nowTicksA}", null, null, null, It.IsAny<int>()))
        //        .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
        //        .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
        //    var response = controller.GetReportDetail($"BuyerCode{nowTicksA}", null, null, null);
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Error_Get_Report_Xls_Data_Detail_AccuracyArrival()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportDetailAccuracyofArrival($"BuyerCode{nowTicksA}", null, null, null, It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsArrivalDetail($"BuyerCode{nowTicksA}", null, null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data_Detail_AccuracyArrival()
        {
            List<GarmentCategoryViewModel> garmentCategory = new List<GarmentCategoryViewModel>
            {
                new GarmentCategoryViewModel
                {
                    Id = 7,
                    Code = "LBL",
                    Name = "LABEL",
                    CodeRequirement = "BP"
                },
                new GarmentCategoryViewModel
                {
                    Id = 13,
                    Code = "SUB",
                    Name = "SUBKON",
                    CodeRequirement = "BB"
                }
            };
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportDetailAccuracyofArrival($"BuyerCode{nowTicksA}", null, null, null, It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            var vmString = JsonConvert.SerializeObject(garmentCategory);
            
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsArrivalDetail($"BuyerCode{nowTicksA}", null, null, null);
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }



        [Fact]
        public void Should_Success_Get_Report_Data_Delivery_Header()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportHeaderAccuracyofDelivery( null, null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetReport2(null, null, "", "");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_Xls_Data_Header_AccuracyDelivery()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportHeaderAccuracyofDelivery(null, null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsDeliveryHeader(null, null, "", "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data_Header_AccuracyDelivery()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportHeaderAccuracyofDelivery(null, null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsDeliveryHeader(null, null, "", "");
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }

        //[Fact]
        //public void Should_Success_Get_Report_Data_Delivery_Detail()
        //{
        //    long nowTicks = DateTimeOffset.Now.Ticks;
        //    string nowTicksA = $"{nowTicks}a";

        //    var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
        //    mockFacade.Setup(x => x.GetReportDetailAccuracyofDelivery($"BuyerCode{nowTicksA}", null, null, It.IsAny<int>()))
        //        .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

        //    var mockMapper = new Mock<IMapper>();
        //    mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
        //        .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user.Object
        //        }
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
        //    var response = controller.GetReportDetail2($"BuyerCode{nowTicksA}", null, null);
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        [Fact]
        public void Should_Error_Get_Report_Xls_Data_Detail_AccuracyDelivery()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportDetailAccuracyofDelivery($"BuyerCode{nowTicksA}", null, null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsDeliveryDetail($"BuyerCode{nowTicksA}", null, null, "", "");
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report_Xls_Data_Detail_DeliveryArrival()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";

            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GetReportDetailAccuracyofDelivery($"BuyerCode{nowTicksA}", null, null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccuracyOfArrivalReportViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<AccuracyOfArrivalReportViewModel> { this.ViewModelAccuracyArrival });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsDeliveryDetail($"BuyerCode{nowTicksA}", null, null, "", "");
            Assert.Null(response.GetType().GetProperty("FileStream"));
        }
        #region Report Delivery Order All
        [Fact]
        public void Should_Success_Get_Report_DO()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            mockFacade.Setup(x => x.GetReportDO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<GarmentDeliveryOrderReportViewModel>(), 5));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentDeliveryOrderViewModel>>(It.IsAny<List<GarmentDeliveryOrder>>()))
                .Returns(new List<GarmentDeliveryOrderViewModel> { ViewModel });

            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportDO(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(),It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Xls_DO()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockFacade.Setup(x => x.GenerateExcelDO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
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
            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXlsDO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_Get_Report_DO()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();


            GarmentDeliveryOrderController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetReportDO(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Xls_DO()
        {
            var mockFacade = new Mock<IGarmentDeliveryOrderFacade>();

            var mockMapper = new Mock<IMapper>();

            GarmentDeliveryOrderController controller = new GarmentDeliveryOrderController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = controller.GetXlsDO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }
        #endregion
    }
}
