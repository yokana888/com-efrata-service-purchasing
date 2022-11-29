using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPOMasterDistributionDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentPOMasterDistributionFacadeTests
{
    public class BasicFacadeTest
    {
        private const string ENTITY = "GarmentPOMasterDistribution";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private Mock<IServiceProvider> GetMockServiceProvider()
        {
            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}") });
            httpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });

            var mockGarmentDeliveryOrderFacade = new Mock<IGarmentDeliveryOrderFacade>();
            mockGarmentDeliveryOrderFacade
                .Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new GarmentDeliveryOrder());

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username" });
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IGarmentDeliveryOrderFacade)))
                .Returns(mockGarmentDeliveryOrderFacade.Object);


            return mockServiceProvider;
        }

        private PurchasingDbContext GetDbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private BasicDataUtil dataUtil(GarmentPOMasterDistributionFacade facade, PurchasingDbContext dbContext)
        {
            var mockServiceProvider = GetMockServiceProvider();
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(mockServiceProvider.Object, dbContext);
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(dbContext);
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(mockServiceProvider.Object, dbContext);
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(mockServiceProvider.Object, dbContext);
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new BasicDataUtil(facade, garmentDeliveryOrderDataUtil);
        }

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);
        //    var data = await dataUtil(facade, dbContext).GetTestData();

        //    var Response = facade.Read(Select: "{ 'Id': 1, 'DONo': 1 }");
        //    Assert.NotEmpty(Response.Data);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);
        //    var data = await dataUtil(facade, dbContext).GetTestData();

        //    var Response = facade.ReadById(data.Id);
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);
        //    var data = await dataUtil(facade, dbContext).GetNewData();

        //    var Response = await facade.Create(data);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);

        //    var data = await dataUtil(facade, dbContext).GetNewData();
        //    data.Items = null;

        //    Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
        //    Assert.NotNull(e.Message);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);

        //    var dataUtil = this.dataUtil(facade, dbContext);
        //    var data = await dataUtil.GetTestData();

        //    var copiedData = dataUtil.CopyData(data);
        //    copiedData.Items = new List<GarmentPOMasterDistributionItem>
        //    {
        //        data.Items.Select(i => {
        //            var copiedItem = dataUtil.CopyDataItem(i);
        //            if (i.Details != null)
        //            {
        //                var copiedDetail = dataUtil.CopyDataDetail(i.Details.First());
        //                var newDetail = dataUtil.CopyDataDetail(i.Details.First());
        //                newDetail.Id=0;
        //                copiedItem.Details = new List<GarmentPOMasterDistributionDetail>
        //                {
        //                    copiedDetail,
        //                    newDetail
        //                };
        //            }
        //            return copiedItem;
        //        }).First(),
        //        dataUtil.CopyDataItem(data.Items.Last())
        //    };

        //    //copiedData.Items = data.Items.Select(i =>
        //    //    {
        //    //        var copiedItem = dataUtil.CopyDataItem(i);
        //    //        if (i.Details != null)
        //    //        {
        //    //            copiedItem.Details = i.Details.Select(d => dataUtil.CopyDataDetail(d)).ToList();
        //    //        }
        //    //        return copiedItem;
        //    //    })
        //    //    .ToList();

        //    var Response = await facade.Update(copiedData.Id, copiedData);
        //    Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);

        //    var dataUtil = this.dataUtil(facade, dbContext);
        //    var data = await dataUtil.GetTestData();

        //    var copiedData = dataUtil.CopyData(data);
        //    copiedData.Items = null;

        //    Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(copiedData.Id, copiedData));
        //    Assert.NotNull(e.Message);
        //}

        //[Fact]
        //public async Task Shoud_Success_Delete_Data()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);
        //    var data = await dataUtil(facade, dbContext).GetTestData();

        //    var Response = await facade.Delete(data.Id);
        //    Assert.NotEqual(0, Response);
        //}

        [Fact]
        public async Task Shoud_Error_Delete_Data()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0));
            Assert.NotNull(e.Message);
        }

        //[Fact]
        //public async Task Should_Success_Get_OthersQuantity()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var facade = new GarmentPOMasterDistributionFacade(GetMockServiceProvider().Object, dbContext);
        //    var data = await dataUtil(facade, dbContext).GetTestData();

        //    var viewModel = new GarmentPOMasterDistributionViewModel { Id = data.Id, Items = new List<GarmentPOMasterDistributionItemViewModel>() };
        //    foreach (var i in data.Items)
        //    {
        //        var viewModelItem = new GarmentPOMasterDistributionItemViewModel { Details = new List<GarmentPOMasterDistributionDetailViewModel>() };
        //        foreach (var d in i.Details)
        //        {
        //            viewModelItem.Details.Add(new GarmentPOMasterDistributionDetailViewModel
        //            {
        //                POSerialNumber = d.POSerialNumber,
        //                Quantity = d.Quantity
        //            });
        //        }
        //        viewModel.Items.Add(viewModelItem);
        //    }

        //    var Response = facade.GetOthersQuantity(viewModel);
        //    Assert.NotNull(Response);
        //}
    }
}
