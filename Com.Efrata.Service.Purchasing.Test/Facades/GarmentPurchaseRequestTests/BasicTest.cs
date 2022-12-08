using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.PRMasterValidationReportFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
//using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentPurchaseRequestTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentPurchaseRequest";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private PurchasingDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentPurchaseRequestDataUtil dataUtil(GarmentPurchaseRequestFacade facade, string testName)
        {
            return new GarmentPurchaseRequestDataUtil(facade);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Master()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.PRType = "MASTER";
            model.UnitCode = "EFR";
            model.RONo = null;
            foreach (var item in model.Items)
            {
                item.PO_SerialNumber = null;
            }
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Sampel()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.PRType = "SAMPLE";
            model.UnitCode = "EFR";
            model.RONo = null;
            foreach (var item in model.Items)
            {
                item.CategoryName = "FABRIC";
                item.PO_SerialNumber = null;
            }
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Error_Create_Data_PRType_RONo()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            //model.PRType = "MASTER";
            model.UnitCode = "EFR";
            model.RONo = null;
            foreach (var item in model.Items)
            {
                item.PO_SerialNumber = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            //var Response = await facade.Create(model, USERNAME);
            //Assert.NotEqual(0, Response);
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Error_Create_Data_UnitCode_RONo()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.PRType = "MASTER";
            //model.UnitCode = "AG1";
            model.RONo = null;
            foreach (var item in model.Items)
            {
                item.PO_SerialNumber = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            //var Response = await facade.Create(model, USERNAME);
            //Assert.NotEqual(0, Response);
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Error_Create_Data_PRType_POSerialNumber()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            //model.PRType = "MASTER";
            model.UnitCode = "EFR";
            //model.RONo = null;
            foreach (var item in model.Items)
            {
                item.PO_SerialNumber = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            //var Response = await facade.Create(model, USERNAME);
            //Assert.NotEqual(0, Response);
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Error_Create_Data_UnitCode_POSerialNumber()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.PRType = "MASTER";
            //model.UnitCode = "AG1";
            //model.RONo = null;
            foreach (var item in model.Items)
            {
                item.PO_SerialNumber = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            //var Response = await facade.Create(model, USERNAME);
            //Assert.NotEqual(0, Response);
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Error_Create_Data_Update_IsPR()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("") });

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            model.PRType = "SAMPLE";
            model.UnitCode = "EFR";
            model.RONo = null;
            foreach (var item in model.Items)
            {
                item.CategoryName = "FABRIC";
                item.PO_SerialNumber = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
		 
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            GarmentPurchaseRequest newModel = dataUtil.CopyData(model);
            newModel.Items = new List<GarmentPurchaseRequestItem>();
            foreach (var item in model.Items)
            {
                newModel.Items.Add(dataUtil.CopyDataItem(item));
            }
            var firstItem = newModel.Items.First();
            firstItem.Id = 0;
            firstItem.PO_SerialNumber = $"PO_SerialNumber{DateTime.Now.Ticks}";

            var Response = await facade.Update((int)newModel.Id, newModel, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data_PRType()
        {
            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            GarmentPurchaseRequest newModel = dataUtil.CopyData(model);
            newModel.Items = new List<GarmentPurchaseRequestItem>();
            foreach (var item in model.Items)
            {
                newModel.Items.Add(dataUtil.CopyDataItem(item));
            }
            newModel.PRType = "SAMPLE";
            newModel.UnitCode = "EFR";
            var firstItem = newModel.Items.First();
            firstItem.Id = 0;
            firstItem.PO_SerialNumber = null;

            var Response = await facade.Update((int)newModel.Id, newModel, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(0, model, USERNAME));
            Assert.NotNull(errorInvalidId.Message);

            GarmentPurchaseRequest newModel = new GarmentPurchaseRequest();
            Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)model.Id, newModel, USERNAME));
            Assert.NotNull(errorNullItems.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.Delete((int)model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data_With_Update_IsPR()
        {
            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(mockHttpClient.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
            var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();
            model.PRType = "MASTER";

            var Response = await facade.Delete((int)model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));

			Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_PRPost_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.PRPost(new List<long> { model.Id }, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_PRPost_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.PRPost(new List<long> { 0 }, null));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_PRUnpost_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.PRUnpost(model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_PRUnpost_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));

			Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.PRUnpost(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_PRApprove_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.PRApprove(model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_PRApprove_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.PRApprove(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_PRUnApprove_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await this.dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.PRUnApprove(model.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_PRUnApprove_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));

			Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.PRUnApprove(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Order()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"PRNo\": \"asc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Order_Date_Asc()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"Date\": \"asc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Order_Date_Desc()
        {
            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"Date\": \"desc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Order_Status_Asc()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"Status\": \"asc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Order_Status_Desc()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"Status\": \"desc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Dynamic()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadDynamic(Select: "{Id: 1}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int) model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_RONo()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadByRONo(model.RONo);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Tags()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facade.ReadByTags($"#{model.UnitName} #{model.BuyerName}", model.ShipmentDate.AddDays(-1), model.ShipmentDate.AddDays(1));
            Assert.NotNull(Response);

            var ResponseWhiteSpace = facade.ReadByTags("", DateTimeOffset.MinValue, DateTimeOffset.MinValue);
            Assert.NotNull(ResponseWhiteSpace);
        }

        [Fact]
        public void Should_Success_GeneratePdf()
        {
            Mock<IGarmentPurchaseRequestFacade> garmentPurchaseRequestFacade = new Mock<IGarmentPurchaseRequestFacade>();
            garmentPurchaseRequestFacade
                .Setup(x => x.GetGarmentPreSalesContract(It.IsAny<long>()))
                .Returns(new GarmentPreSalesContractViewModel());

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentPurchaseRequestFacade)))
                .Returns(garmentPurchaseRequestFacade.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var Response = facade.GeneratePdf(new GarmentPurchaseRequestViewModel
            {
                Buyer = new BuyerViewModel(),
                Items = new List<GarmentPurchaseRequestItemViewModel>
                {
                    new GarmentPurchaseRequestItemViewModel
                    {
                        UId = null,
                        Category = new CategoryViewModel(),
                        Product = new ProductViewModel(),
                        Uom = new UomViewModel(),
                        Status = null,
                        IsUsed = false,
                        PriceUom = new UomViewModel(),
                        Quantity = 5,
                        BudgetPrice = 2,
                        PriceConversion = 1,
                        IsOpenPO = false,
                        OpenPOBy = null,
                        OpenPODate = DateTimeOffset.MinValue,
                        IsApprovedOpenPOMD = false,
                        ApprovedOpenPOMDBy = null,
                        ApprovedOpenPOMDDate = DateTimeOffset.MinValue,
                        IsApprovedOpenPOPurchasing = false,
                        ApprovedOpenPOPurchasingBy = null,
                        ApprovedOpenPOPurchasingDate = DateTimeOffset.MinValue,
                        IsApprovedOpenPOKadivMd = false,
                        ApprovedOpenPOKadivMdBy = null,
                        ApprovedOpenPOKadivMdDate = DateTimeOffset.MinValue,
                    }
                }
            });

            Assert.IsType<MemoryStream>(Response);
        }

        [Fact]
        public void Should_Success_Get_GarmentPreSalesContract()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();

            Mock<IHttpClientService> httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("merchandiser/garment-pre-sales-contracts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentPreSalesContractDataUtil().GetResultFormatterOkString()) });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));
            var garmentPreSalesContract = facade.GetGarmentPreSalesContract(It.IsAny<long>());

            Assert.NotNull(garmentPreSalesContract);
        }

        [Fact]
        public void Should_Error_Get_GarmentPreSalesContract()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();

            Mock<IHttpClientService> httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("merchandiser/garment-pre-sales-contracts"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(string.Empty) });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));

            Exception exception = Assert.Throws<Exception>(() => facade.GetGarmentPreSalesContract(It.IsAny<long>()));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentPurchaseRequestViewModel nullViewModel = new GarmentPurchaseRequestViewModel();
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentPurchaseRequestViewModel viewModel = new GarmentPurchaseRequestViewModel
            {
                Buyer = new BuyerViewModel(),
                Unit = new UnitViewModel(),
                Items = new List<GarmentPurchaseRequestItemViewModel>
                {
                    new GarmentPurchaseRequestItemViewModel(),
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Product = new ProductViewModel(),
                        Uom = new UomViewModel(),
                        Category = new CategoryViewModel(),
                        BudgetPrice = -1
                    }
                },
                IsValidatedMD1 = false,
                ValidatedMD1By = null,
                ValidatedMD1Date = DateTimeOffset.MinValue,
                IsValidatedMD2 = false,
                ValidatedMD2By = null,
                ValidatedMD2Date = DateTimeOffset.MinValue,
                IsValidatedPurchasing = false,
                ValidatedPurchasingBy = null,
                ValidatedPurchasingDate = DateTimeOffset.MinValue,
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Master()
        {
            GarmentPurchaseRequestViewModel viewModel = new GarmentPurchaseRequestViewModel
            {
                PRType = "MASTER",
                Buyer = new BuyerViewModel(),
                Unit = new UnitViewModel(),
                Items = new List<GarmentPurchaseRequestItemViewModel>
                {
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Category = new CategoryViewModel { Name = "FABRIC" },
                        PriceUom = new UomViewModel()
                    },
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Category = new CategoryViewModel { Name = "FABRIC" },
                        Composition = new GarmentProductViewModel { Composition = "Composition" },
                    },
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Category = new CategoryViewModel { Name = "FABRIC" },
                        Composition = new GarmentProductViewModel { Composition = "Composition" },
                        Const = new GarmentProductViewModel { Const = "Const" }
                    },
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Category = new CategoryViewModel { Name = "FABRIC" },
                        Composition = new GarmentProductViewModel { Composition = "Composition" },
                        Const = new GarmentProductViewModel { Const = "Const" },
                        Yarn = new GarmentProductViewModel { Yarn = "Yarn" },
                    },
                    new GarmentPurchaseRequestItemViewModel
                    {
                        Category = new CategoryViewModel { Name = "FABRIC" },
                        Composition = new GarmentProductViewModel { Composition = "Composition" },
                        Const = new GarmentProductViewModel { Const = "Const" },
                        Yarn = new GarmentProductViewModel { Yarn = "Yarn" },
                        Width = new GarmentProductViewModel { Width = "Width" },
                    },
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Validate_Data_Duplicate()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            GarmentPurchaseRequestViewModel viewModel = new GarmentPurchaseRequestViewModel();
            viewModel.RONo = model.RONo;

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider.Object, null);

            var validationResultCreate = viewModel.Validate(validationContext).ToList();

            var errorDuplicateRONo = validationResultCreate.SingleOrDefault(r => r.ErrorMessage.Equals("RONo sudah ada"));
            Assert.NotNull(errorDuplicateRONo);

            viewModel.Id = model.Id;
            viewModel.Items = new List<GarmentPurchaseRequestItemViewModel>();
            viewModel.Items.AddRange(model.Items.Select(i => new GarmentPurchaseRequestItemViewModel
            {
                PO_SerialNumber = i.PO_SerialNumber
            }));

            var validationResultUpdate = viewModel.Validate(validationContext).ToList();
            var errorItems = validationResultUpdate.SingleOrDefault(r => r.MemberNames.Contains("Items"));
            List<Dictionary<string, object>> errorItemsMessage = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(errorItems.ErrorMessage);
            var errorDuplicatePO_SerialNumber = errorItemsMessage.FirstOrDefault(m => m.ContainsValue("PO SerialNumber sudah ada"));
            Assert.NotNull(errorDuplicatePO_SerialNumber);
        }

		//monitoring purchase all
		private Mock<IServiceProvider> GetServiceProviderDO()
		{
			var HttpClientService = new Mock<IHttpClientService>();
			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
			string gCurrencyUri = "master/garment-currencies";
			HttpClientService
				.Setup(x => x.GetAsync(It.IsRegex($"^{APIEndpoint.Core}{gCurrencyUri}")))
				.ReturnsAsync(message);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			serviceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(HttpClientService.Object);

			return serviceProvider;
		}
		private Mock<IServiceProvider> GetServiceProvider()
		{
			var HttpClientService = new Mock<IHttpClientService>();
			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"_id\":1,\"_deleted\":false,\"_active\":false,\"_createdDate\":\"2018-06-21T01:57:47.6772924\",\"_createdBy\":\"\",\"_createAgent\":\"\",\"_updatedDate\":\"2018-06-21T01:57:47.6772924\",\"_updatedBy\":\"\",\"_updateAgent\":\"\",\"code\":\"A001\",\"name\":\"ADI KARYA. UD\",\"address\":\"JL.JAMBU,JAJAR,SOLO\",\"contact\":\"\",\"PIC\":\"\",\"import\":true,\"NPWP\":\"\",\"serialNumber\":\"\"}}");
			string supplierUri = "master/garment-suppliers";
			HttpClientService
				.Setup(x => x.GetAsync(It.IsRegex($"^{APIEndpoint.Core}{supplierUri}")))
				.ReturnsAsync(message);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			serviceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(HttpClientService.Object);

			return serviceProvider;
		}
		private GarmentCorrectionNoteQuantityDataUtil dataUtil(GarmentCorrectionNoteQuantityFacade facade, string testName)
		{
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			 
			var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(testName));
			var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

			var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
			var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

			var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(mockServiceProvider.Object, _dbContext(testName));
			var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

			var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(testName));
			var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

			return new GarmentCorrectionNoteQuantityDataUtil(facade, garmentDeliveryOrderDataUtil);
		}
        private GarmentDeliveryOrderDataUtil dataUtilDo(GarmentDeliveryOrderFacade facade, string testName)
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);
			var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(mockServiceProvider.Object, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }
		[Fact]
		public async Task Should_Success_Get_Report_Purchase_All_Data()
		{
            GarmentDeliveryOrderFacade facadeDo = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(GetCurrentMethod()));
            var datautildelivery = dataUtilDo(facadeDo, GetCurrentMethod());
            var dataDo = await dataUtilDo(facadeDo, GetCurrentMethod()).GetTestData();

            GarmentUnitReceiptNoteFacade facadeURN = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var datautilURN = new GarmentUnitReceiptNoteDataUtil(facadeURN, datautildelivery,null);
            var dataUrn = await datautilURN.GetNewData(null,dataDo);
            await facadeURN.Create(dataUrn);

            GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetNewData(dataDo);
			await facade.Create(data,false, USERNAME);
			var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var Response = Facade.GetMonitoringPurchaseReport(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
			Assert.NotNull(Response);

		}

        [Fact]
        public async Task Should_Success_Get_Report_Purchase_All_Data_Filter_Status_Belum()
        {
            GarmentDeliveryOrderFacade facadeDo = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(GetCurrentMethod()));
            var datautildelivery = dataUtilDo(facadeDo, GetCurrentMethod());
            var dataDo = await dataUtilDo(facadeDo, GetCurrentMethod()).GetTestData();

            GarmentUnitReceiptNoteFacade facadeURN = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var datautilURN = new GarmentUnitReceiptNoteDataUtil(facadeURN, datautildelivery, null);
            var dataUrn = await datautilURN.GetNewData(null, dataDo);
            await facadeURN.Create(dataUrn);

            GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData(dataDo);
            await facade.Create(data, false, USERNAME);
            var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GetMonitoringPurchaseReport(null, null, null, null, null, null, null, "BELUM", null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Purchase_All_Data_Filter_Status_Sudah()
        {
            GarmentDeliveryOrderFacade facadeDo = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(GetCurrentMethod()));
            var datautildelivery = dataUtilDo(facadeDo, GetCurrentMethod());
            var dataDo = await dataUtilDo(facadeDo, GetCurrentMethod()).GetTestData();

            GarmentUnitReceiptNoteFacade facadeURN = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var datautilURN = new GarmentUnitReceiptNoteDataUtil(facadeURN, datautildelivery, null);
            var dataUrn = await datautilURN.GetNewData(null, dataDo);
            await facadeURN.Create(dataUrn);

            GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewData(dataDo);
            await facade.Create(data, false, USERNAME);

			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			var HttpClientService = new Mock<IHttpClientService>();
		 
			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"code\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
			string gCurrencyUri = "master/garmentProducts/fabricByCode";
			HttpClientService
				.Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.IsRegex($"^{APIEndpoint.Core}{gCurrencyUri}"), It.IsAny<HttpContent>()))
				.ReturnsAsync(message);
			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			serviceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(HttpClientService.Object);
			serviceProvider.Setup(x => x.GetService(typeof(IHttpClientService))).Returns(HttpClientService.Object);
			var Facade = new GarmentPurchaseRequestFacade(serviceProvider.Object, _dbContext(GetCurrentMethod()));
          
				var Response = Facade.GetMonitoringPurchaseReport(null, null, null, null, null, null, null, "SUDAH", null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response);
        }

        [Fact]
		public async Task Should_Success_Get_Report_Purchase_All_Excel()
		{
            GarmentDeliveryOrderFacade facadeDo = new GarmentDeliveryOrderFacade(GetServiceProviderDO().Object, _dbContext(GetCurrentMethod()));
            var datautildelivery = dataUtilDo(facadeDo, GetCurrentMethod());
            var dataDo = await dataUtilDo(facadeDo, GetCurrentMethod()).GetTestData();

            GarmentUnitReceiptNoteFacade facadeURN = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var datautilURN = new GarmentUnitReceiptNoteDataUtil(facadeURN, datautildelivery, null);
            var dataUrn = await datautilURN.GetNewData(null, dataDo);
            await facadeURN.Create(dataUrn);

            GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetNewData(dataDo);
			await facade.Create(data, false, USERNAME);
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade Facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));

			var dateTo = DateTime.Now;
            var dateFrom = DateTime.Now.AddDays(-30);
            var dateFromEx = DateTimeOffset.MinValue;
            var dateToEx = DateTimeOffset.Now;
            var Response = Facade.GenerateExcelPurchase(null, null, null, null, null, null, null,"SUDAH", null, null, dateFrom, dateTo, dateFromEx, dateToEx, 1,25,"{}", 7);
			Assert.NotNull(Response);

		}
		[Fact]
		public async Task Should_Success_Get_Report_Purchase_All_Excel_null_Result()
		{
			GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
			await facade.Create(data, false, USERNAME);
			var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var dateTo = DateTime.Now;
			var dateFrom = DateTime.Now.AddDays(-30);
			var dateFromEx = DateTimeOffset.MinValue;
			var dateToEx = DateTimeOffset.Now;
			var Response = Facade.GenerateExcelPurchase(null, null, null, null, null, null, null, null, null, null, dateFrom, dateTo, dateFromEx, dateToEx, 1, 25, "{}", 7);
			Assert.NotNull(Response);
		}


		[Fact]
		public async Task Should_Success_Get_Data_By_Name()
		{
			var facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var data = dataUtil(facade, GetCurrentMethod()).GetNewData();
			var Responses = await facade.Create(data, USERNAME);
			var Response = facade.ReadName(data.CreatedBy);
			Assert.NotNull(Response);
		}

		[Fact]
		public void Should_Success_Get_Report_Purchase_By_User_Data()
		{
			GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
		 
			var datas = dataUtil(facade, GetCurrentMethod()).GetNewDoubleCorrectionData(USERNAME);
		
			var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var Response = Facade.GetMonitoringPurchaseByUserReport(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 25, "{}", 7);
			Assert.NotNull(Response);

		}

		[Fact]
		public async Task Should_Success_Get_Report_Purchase_By_User_Excel()
		{
			GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
			await facade.Create(data, false, USERNAME);
			var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var Response = Facade.GenerateExcelByUserPurchase(null, null, null, null, null, null, null, null, null, null, null, null,  null, null, 1, 25, "{}", 7);
			Assert.NotNull(Response);

		}

		[Fact]
		public async Task Should_Success_Get_Report_Purchase_By_User_noData_Excel()
		{
			GarmentCorrectionNoteQuantityFacade facade = new GarmentCorrectionNoteQuantityFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
			await facade.Create(data, false, USERNAME);
			var Facade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
			var Response = Facade.GenerateExcelByUserPurchase("coba", null, null, null, null, null, null, null, null, null, null,null, null, null, 1, 25, "{}", 7);
			Assert.NotNull(Response);

		}

        [Fact]
        public async void Should_Success_Patch_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            JsonPatchDocument<GarmentPurchaseRequest> jsonPatch = new JsonPatchDocument<GarmentPurchaseRequest>();
            jsonPatch.Replace(m => m.IsValidated, false);

            var Response = await facade.Patch(model.Id, jsonPatch, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Error_Patch_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            JsonPatchDocument<GarmentPurchaseRequest> jsonPatch = new JsonPatchDocument<GarmentPurchaseRequest>();
            jsonPatch.Replace(m => m.Id, 0);

            var Response = await Assert.ThrowsAnyAsync<Exception>(async () => await facade.Patch(model.Id, jsonPatch, USERNAME));
            Assert.NotNull(Response.Message);
        }
        // PR MASTER VALIDATION REPORT
        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			PRMasterValidationReportFacade facadevld = new PRMasterValidationReportFacade(_dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facadevld.GetDisplayReport(model.UnitId, model.SectionName, null, DateTime.UtcNow.AddDays(1), "{}", 7);

            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Get_Report_Data_Null_Parameter()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			PRMasterValidationReportFacade facadevld = new PRMasterValidationReportFacade(_dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facadevld.GetDisplayReport(model.UnitId, model.SectionName, model.ValidatedMD2Date.DateTime.AddDays(30), model.ValidatedMD2Date.DateTime.AddDays(30), "{}", 7);

        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			PRMasterValidationReportFacade facadevld = new PRMasterValidationReportFacade(_dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facadevld.GenerateExcel(model.UnitId, model.SectionName, null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Get_Report_Data_Excel_Null_Parameter()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			PRMasterValidationReportFacade facadevld = new PRMasterValidationReportFacade(_dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facadevld.GenerateExcel(model.UnitId, model.SectionName, model.ValidatedMD2Date.DateTime.AddDays(30), model.ValidatedMD2Date.DateTime.AddDays(30), 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public void Should_Success_Get_ReadByTagsOptimized()
        {
			var mockHttpClient = new Mock<IHttpClientService>();
			mockHttpClient
				.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var mockServiceProvider = GetServiceProvider();
			mockServiceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(mockHttpClient.Object);

			GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, _dbContext(GetCurrentMethod()));
			var Response = facade.ReadByTagsOptimized("test", DateTimeOffset.Now, DateTimeOffset.Now);

            Assert.NotNull(Response);
        }
    }
}
