using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Expedition
{
    // [Collection("TestServerFixture Collection")]
    public class PurchasingDocumentExpeditionTest
    {
        //private const string MediaType = "application/json";
        //private readonly string URI = "v1/expedition/purchasing-document-expeditions";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected SendToVerificationDataUtil DataUtil
        //{
        //    get { return (SendToVerificationDataUtil)this.TestFixture.Service.GetService(typeof(SendToVerificationDataUtil)); }
        //}

        //public PurchasingDocumentExpeditionTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    PurchasingDocumentExpedition Model = await DataUtil.GetTestData();

        //    var response = await this.Client.GetAsync(string.Concat(URI, "/", Model.Id));
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data()
        //{
        //    PurchasingDocumentExpedition Model = await DataUtil.GetTestData();

        //    var response = await this.Client.DeleteAsync(string.Concat(URI, "/", Model.Id));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data_By_UPO_No()
        //{
        //    PurchasingDocumentExpedition Model = await DataUtil.GetTestData();

        //    var response = await this.Client.DeleteAsync(string.Concat(URI, "/PDE/", Model.UnitPaymentOrderNo));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

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

        private PurchasingDocumentExpeditionController GetController(Mock<IPurchasingDocumentExpeditionFacade> facadeM)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            //if (validateM != null)
            //{
            //    servicePMock
            //        .Setup(x => x.GetService(typeof(IValidateService)))
            //        .Returns(validateM.Object);
            //}

            var controller = new PurchasingDocumentExpeditionController(facadeM.Object, servicePMock.Object)
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
        public void Should_Success_GetData()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(Tuple.Create(new List<object>(), 0, new Dictionary<string, string>()));

            var controller = GetController(mockFacade);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NoContent_Delete()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade);
            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NotFound_Delete()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(0);

            var controller = GetController(mockFacade);
            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_InternalServerError_Delete()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.Delete(It.IsAny<int>()))
                .Throws(new Exception());

            var controller = GetController(mockFacade);
            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NoContent_DeleteBySPBNo()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.DeleteByUPONo(It.IsAny<string>()))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade);
            var response = await controller.Delete(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_InternalServerError_DeleteBySPBNo()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.DeleteByUPONo(It.IsAny<string>()))
                .Throws(new Exception());

            var controller = GetController(mockFacade);
            var response = await controller.Delete(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetById()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .ReturnsAsync(new PurchasingDocumentExpedition() { Items = new List<PurchasingDocumentExpeditionItem>() { new PurchasingDocumentExpeditionItem() } });

            var controller = GetController(mockFacade);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_NotFound_GetById()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .ReturnsAsync((PurchasingDocumentExpedition)null);

            var controller = GetController(mockFacade);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_InternalServerError_GetById()
        {
            var mockFacade = new Mock<IPurchasingDocumentExpeditionFacade>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var controller = GetController(mockFacade);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
