using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
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
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentPurchaseRequestControllerTests
{
    public class GarmentPurchaseRequestItemControllerTest
    {
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

        private GarmentPurchaseRequestItemController GetController(Mock<IGarmentPurchaseRequestItemFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            GarmentPurchaseRequestItemController controller = new GarmentPurchaseRequestItemController(servicePMock.Object, mapper.Object, facadeM.Object)
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

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private int GetStatusCodeGet(Mock<IGarmentPurchaseRequestItemFacade> mockFacade)
        {
            GarmentPurchaseRequestItemController controller = GetController(mockFacade, null, new Mock<IMapper>());

            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        [Fact]
        public void Get_Ok()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<dynamic>(new List<dynamic>(), 0, new Dictionary<string, string>()));

            int statusCode = GetStatusCodeGet(mockFacade);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_InternalServerError()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Error"));

            int statusCode = GetStatusCodeGet(mockFacade);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodePatchOne(Mock<IGarmentPurchaseRequestItemFacade> mockFacade, Mock<IMapper> mockMapper, int id)
        {
            GarmentPurchaseRequestItemController controller = GetController(mockFacade, null, mockMapper);

            JsonPatchDocument<GarmentPurchaseRequestItem> patch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            IActionResult response = await controller.PatchOne(patch, id);

            return GetStatusCode(response);
        }

        [Fact]
        public async Task PatchOne_NoContent()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<string>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequestItem>>()))
               .ReturnsAsync(1);

            int statusCode = await GetStatusCodePatchOne(mockFacade, new Mock<IMapper>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task PatchOne_InternalServerError()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<string>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequestItem>>()))
               .ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodePatchOne(mockFacade, new Mock<IMapper>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task PatchOne_BadRequest()
        {
            GarmentPurchaseRequestItemController controller = GetController(new Mock<IGarmentPurchaseRequestItemFacade>(), null, new Mock<IMapper>());
            controller.ModelState.AddModelError("op", "Invalid op");

            JsonPatchDocument<GarmentPurchaseRequestItem> patch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            IActionResult response = await controller.PatchOne(patch, It.IsAny<int>());

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        private async Task<int> GetStatusCodePatch(Mock<IGarmentPurchaseRequestItemFacade> mockFacade, Mock<IMapper> mockMapper, string id)
        {
            GarmentPurchaseRequestItemController controller = GetController(mockFacade, null, mockMapper);

            JsonPatchDocument<GarmentPurchaseRequestItem> patch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            IActionResult response = await controller.Patch(patch, id);

            return GetStatusCode(response);
        }

        [Fact]
        public async Task Patch_NoContent()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<string>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequestItem>>()))
               .ReturnsAsync(1);

            int statusCode = await GetStatusCodePatch(mockFacade, new Mock<IMapper>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task Patch_InternalServerError()
        {
            var mockFacade = new Mock<IGarmentPurchaseRequestItemFacade>();
            mockFacade.Setup(x => x.Patch(It.IsAny<string>(), It.IsAny<JsonPatchDocument<GarmentPurchaseRequestItem>>()))
               .ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodePatch(mockFacade, new Mock<IMapper>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Patch_BadRequest()
        {
            GarmentPurchaseRequestItemController controller = GetController(new Mock<IGarmentPurchaseRequestItemFacade>(), null, new Mock<IMapper>());
            controller.ModelState.AddModelError("op", "Invalid op");

            JsonPatchDocument<GarmentPurchaseRequestItem> patch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            IActionResult response = await controller.Patch(patch, It.IsAny<string>());

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }
    }
}
