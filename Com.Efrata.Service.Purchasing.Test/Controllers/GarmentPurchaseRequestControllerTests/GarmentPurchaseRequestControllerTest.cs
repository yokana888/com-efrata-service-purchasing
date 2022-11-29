using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentPurchaseRequestControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentPurchaseRequestControllerTests
{
    public class GarmentPurchaseRequestControllerTest
    {
        private GarmentPurchaseRequestViewModel viewModel
        {
            get
            {
                return new GarmentPurchaseRequestViewModel
                {
                    UId = null,
                    Buyer = new BuyerViewModel(),
                    Unit = new UnitViewModel(),
                    SectionName = "SectionName",
                    CreatedBy="Fetih",
                    Remark="Remark",
                    Items=new List<GarmentPurchaseRequestItemViewModel>()
                    {
                        new GarmentPurchaseRequestItemViewModel()
                        {
                            Category=new CategoryViewModel()
                        }
                    }
                };
            }
        }

        private GarmentPurchaseRequest Model
        {
            get
            {
                return new GarmentPurchaseRequest { };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.viewModel, serviceProvider.Object, null);
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

        private GarmentPurchaseRequestController GetController(Mock<IGarmentPurchaseRequestFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if(validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequest>(It.IsAny<GarmentPurchaseRequestViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentPurchaseRequest>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequest>(It.IsAny<GarmentPurchaseRequestViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentPurchaseRequest>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentPurchaseRequest>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentPurchaseRequestViewModel>>(It.IsAny<List<GarmentPurchaseRequest>>()))
                .Returns(new List<GarmentPurchaseRequestViewModel> { viewModel });

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentPurchaseRequest>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentPurchaseRequestViewModel>>(It.IsAny<List<GarmentPurchaseRequest>>()))
                .Returns(new List<GarmentPurchaseRequestViewModel> { viewModel });

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByUser(filter:"{ 'IsPosted': false }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentPurchaseRequest>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentPurchaseRequestViewModel>>(It.IsAny<List<GarmentPurchaseRequest>>()))
                .Returns(new List<GarmentPurchaseRequestViewModel> { viewModel });

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<GarmentPurchaseRequest>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentPurchaseRequestViewModel>>(It.IsAny<List<GarmentPurchaseRequest>>()))
                .Returns(new List<GarmentPurchaseRequestViewModel> { viewModel });

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByUser();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_Dynamic()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadDynamic(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetDynamic();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_Dynamic()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadDynamic(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetDynamic();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentPurchaseRequest());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequestViewModel>(It.IsAny<GarmentPurchaseRequest>()))
                .Returns(viewModel);

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentPurchaseRequest());

            var mockMapper = new Mock<IMapper>();

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF_By_Id()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentPurchaseRequest());

            mockFacade.Setup(x => x.GeneratePdf(It.IsAny<GarmentPurchaseRequestViewModel>()))
                .Returns(new System.IO.MemoryStream());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequestViewModel>(It.IsAny<GarmentPurchaseRequest>()))
                .Returns(viewModel);

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";

            var response = controller.Get(It.IsAny<int>());
            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Success_Get_Data_By_RONo()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentPurchaseRequest());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequestViewModel>(It.IsAny<GarmentPurchaseRequest>()))
                .Returns(viewModel);

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_RONo()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentPurchaseRequest());

            var mockMapper = new Mock<IMapper>();

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequest>(It.IsAny<GarmentPurchaseRequestViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<GarmentPurchaseRequest>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentPurchaseRequestViewModel>());
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentPurchaseRequestViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<GarmentPurchaseRequestViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<GarmentPurchaseRequest>(It.IsAny<GarmentPurchaseRequestViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<GarmentPurchaseRequest>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentPurchaseRequestViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_PRPost()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRPost(It.IsAny<List<long>>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRPost(It.IsAny<List<long>>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_PRPost()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRPost(It.IsAny<List<long>>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRPost(It.IsAny<List<long>>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_PRUnpost()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRUnpost(It.IsAny<long>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRUnpost(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_PRUnpost()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRUnpost(It.IsAny<long>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRUnpost(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_Tags()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadByTags(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(new List<GarmentInternalPurchaseOrder>());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternalPurchaseOrderViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderViewModel>());

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByTags(null, "lol", "lol");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_Tags()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            var mockMapper = new Mock<IMapper>();

            GarmentPurchaseRequestController controller = new GarmentPurchaseRequestController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetByTags(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_Tags_With_ShipmentDate()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();

            mockFacade.Setup(x => x.ReadByTags(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(new List<GarmentInternalPurchaseOrder>());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<GarmentInternalPurchaseOrderViewModel>>(It.IsAny<List<GarmentInternalPurchaseOrder>>()))
                .Returns(new List<GarmentInternalPurchaseOrderViewModel>());

            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);
            var response = controller.GetByTags(null, "2018-10-31", "2018-10-31");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_PRApprove()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRApprove(It.IsAny<long>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRApprove(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_PRApprove()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRApprove(It.IsAny<long>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRApprove(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_PRUnApprove()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRUnApprove(It.IsAny<long>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRUnApprove(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_PRUnApprove()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.PRUnApprove(It.IsAny<long>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade, null, new Mock<IMapper>());

            var response = await controller.PRUnApprove(It.IsAny<long>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        private async Task<int> GetStatusCodePatch(Mock<IGarmentPurchaseRequestFacade> mockFacade, Mock<IMapper> mockMapper, long id)
        {
            GarmentPurchaseRequestController controller = GetController(mockFacade, null, mockMapper);

            JsonPatchDocument<GarmentPurchaseRequest> patch = new JsonPatchDocument<GarmentPurchaseRequest>();
            IActionResult response = await controller.Patch(id, patch);

            return this.GetStatusCode(response);
        }

        [Fact]
        public async Task Patch_ReturnNotFound()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequest>>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            int statusCode = await this.GetStatusCodePatch(mockFacade, new Mock<IMapper>(), 1);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task Patch_ReturnInternalServerError()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<long>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequest>>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            int statusCode = await this.GetStatusCodePatch(mockFacade, new Mock<IMapper>(), 1);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        

        [Fact]
        public void BySupplier_Return_OK()
        {
            //Setup
            var facadeMock = new Mock<IGarmentPurchaseRequestFacade>();

            facadeMock
               .Setup(x => x.ReadName(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(new List<GarmentPurchaseRequest>());
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(s => s.Map<List<GarmentPurchaseRequestViewModel>>(It.IsAny<List<GarmentPurchaseRequest>>()))
                .Returns(new List<GarmentPurchaseRequestViewModel>() {viewModel});

            Mock<IValidateService> validateMock = new Mock<IValidateService>();

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response =  controller.BySupplier(It.IsAny<string>(), It.IsAny<string>());

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void BySupplier_Return_InternalServerError()
        {
            //Setup
            var facadeMock = new Mock<IGarmentPurchaseRequestFacade>();

            facadeMock
               .Setup(x => x.ReadName(It.IsAny<string>(), It.IsAny<string>()))
               .Throws(new Exception());
            var mapperMock = new Mock<IMapper>();
            
            Mock<IValidateService> validateMock = new Mock<IValidateService>();

            //Act
            var controller = GetController(facadeMock, validateMock, mapperMock);
            var response = controller.BySupplier(It.IsAny<string>(), It.IsAny<string>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
