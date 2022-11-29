using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Migrations;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentUnitDeliveryOrderTests
{
    public class BasicTests
    {
        private const string ENTITY = "GarmentUnitDeliveryOrder";

        private const string USERNAME = "Unit Test";

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

        private Mock<IServiceProvider> GetServiceProvider()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);

            HttpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }

        /*private GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil(GarmentUnitReceiptNoteFacade garmentUnitReceiptNoteFacade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider().Object, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil);
        }*/

        /*private GarmentUnitDeliveryOrderDataUtil dataUtil(GarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade, string testName)
        {
            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(testName));
            var garmentUnitReceiptNoteDataUtil = this.garmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, testName);

            return new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDataUtil);
        }*/

        /*[Fact]
        public async Task Should_Success_Create_Data()
        {
            var GarmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName), GetServiceProvider().Object);
            var garmentUnitDeliveryOrderDataUtil = this.dataUtil(GarmentUnitDeliveryOrderFacade, testName);

            var Response = await facade.Create(data);
            Assert.NotEqual(0, Response);
        }*/

        /*[Fact]
        public async Task Should_Error_Create_Data()
        {
            var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var data = dataUtil(facade, GetCurrentMethod()).GetNewData();

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(null));
            Assert.NotNull(e.Message);
        }*/

        //[Fact]
        //public async Task Should_Error_Create_Data_DOCurrencyRate()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var facade = new GarmentUnitDeliveryOrderFacade(dbContext, GetServiceProvider().Object);
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewData();
        //    foreach (var item in data.Items)
        //    {
        //        var urn = dbContext.GarmentUnitReceiptNotes.Single(s => s.Id == item.URNId);
        //        urn.DOCurrencyRate = 0;
        //    }

        //    Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data));
        //    Assert.NotNull(e.Message);
        //}

        /*[Fact]
        public async Task Should_Success_Update_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitDeliveryOrderFacade(dbContext, GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    dbContext.Entry(data).State = EntityState.Detached;
        //    foreach (var item in data.Items)
        //    {
        //        dbContext.Entry(item).State = EntityState.Detached;
        //    }

        //    var newItem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().Single(m => m.Id == data.Items.First().Id);
        //    newItem.Id = 0;
        //    newItem.IsSave = true;

        //    data.Items.Add(newItem);

        //    var ResponseUpdate = await facade.Update((int)data.Id, data);
        //    Assert.NotEqual(0, ResponseUpdate);

        //    var newData = dbContext.GarmentUnitDeliveryOrders
        //        .AsNoTracking()
        //        .Include(x => x.Items)
        //        .Single(m => m.Id == data.Id);

        //    newData.Items = newData.Items.Take(1).ToList();
        //    newData.Items.First().IsSave = true;

            var ResponseUpdateRemoveItem = await facade.Update((int)newData.Id, newData);
            Assert.NotEqual(0, ResponseUpdateRemoveItem);
        }*/

        /*[Fact]
        public async Task Should_Success_Update_Data_MARKETING()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitDeliveryOrderFacade(dbContext, GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataMarketing();

        //    dbContext.Entry(data).State = EntityState.Detached;
        //    foreach (var item in data.Items)
        //    {
        //        dbContext.Entry(item).State = EntityState.Detached;
        //    }

        //    var newItem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().Single(m => m.Id == data.Items.First().Id);
        //    newItem.Id = 0;
        //    newItem.IsSave = true;

        //    data.Items.Add(newItem);

        //    var ResponseUpdate = await facade.Update((int)data.Id, data);
        //    Assert.NotEqual(0, ResponseUpdate);

        //    var newData = dbContext.GarmentUnitDeliveryOrders
        //        .AsNoTracking()
        //        .Include(x => x.Items)
        //        .Single(m => m.Id == data.Id);

        //    newData.Items = newData.Items.Take(1).ToList();
        //    newData.Items.First().IsSave = true;

            var ResponseUpdateRemoveItem = await facade.Update((int)newData.Id, newData);
            Assert.NotEqual(0, ResponseUpdateRemoveItem);
        }*/

        /*[Fact]
        public async Task Should_Error_Update_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitDeliveryOrderFacade(dbContext, GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    dbContext.Entry(data).State = EntityState.Detached;
        //    foreach (var item in data.Items)
        //    {
        //        dbContext.Entry(item).State = EntityState.Detached;
        //    }

        //    data.Items = null;

            Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)data.Id, data));
            Assert.NotNull(errorNullItems.Message);
        }*/

        /*[Fact]
        public async Task Should_Error_Update_Data_DOCurrencyRate()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentUnitDeliveryOrderFacade(dbContext, GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            dbContext.Entry(data).State = EntityState.Detached;
            foreach (var item in data.Items)
            {
                dbContext.Entry(item).State = EntityState.Detached;
            }

            var newItem = dbContext.GarmentUnitDeliveryOrderItems.AsNoTracking().Single(m => m.Id == data.Items.First().Id);
            newItem.Id = 0;
            newItem.IsSave = true;
            var urn = dbContext.GarmentUnitReceiptNotes.Single(s => s.Id == newItem.URNId);
            urn.DOCurrencyRate = 0;

            data.Items.Add(newItem);

            Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)data.Id, data));
            Assert.NotNull(errorNullItems.Message);
        }*/

        /*[Fact]
        public async Task Should_Success_Delete_Data()
        {
            var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = await facade.Delete((int)data.Id);
            Assert.NotEqual(0, Response);
        }*/

        //    data.Items.Add(newItem);

        //    Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)data.Id, data));
        //    Assert.NotNull(errorNullItems.Message);
        //}

        /* [Fact]
         public async Task Should_Success_Get_All_Data()
         {
             var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
             var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

             var Response = facade.Read();
             Assert.NotEmpty(Response.Data);
         }*/

        /*[Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var Response = facade.ReadById((int)data.Id);
            Assert.NotNull(Response);
        }*/

        /*[Fact]
        public async Task Should_Success_Validate_Data()
        {
            GarmentUnitDeliveryOrderViewModel viewModel = new GarmentUnitDeliveryOrderViewModel {
                UnitRequest = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitSender = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitDOType = "TRANSFER" };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModelNullItems = new GarmentUnitDeliveryOrderViewModel
            {
                RONo = "RONo"
            };
            Assert.True(viewModelNullItems.Validate(null).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModelWithItems = new GarmentUnitDeliveryOrderViewModel
            {
                RONo = "RONo",
                Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                {
                    new GarmentUnitDeliveryOrderItemViewModel
                    {
                        IsSave = true,
                        Quantity = 0
                    }
                }
            };
            Assert.True(viewModelWithItems.Validate(null).Count() > 0);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var dataUtil = garmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, GetCurrentMethod());
            var data = await dataUtil.GetTestData();
            var item = data.Items.First();

            var serviceProvider = GetServiceProvider();
            serviceProvider.Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            GarmentUnitDeliveryOrderViewModel viewModelWithItemsQuantityOver = new GarmentUnitDeliveryOrderViewModel
            {
                RONo = "RONo",
                Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                {
                    new GarmentUnitDeliveryOrderItemViewModel
                    {
                        URNItemId = item.Id,
                        DOItemsId = (int)dataUtil.ReadDOItemsByURNItemId((int)item.Id).Id,
                        IsSave = true,
                        Quantity = (double)10000
                    }
                }
            };
            System.ComponentModel.DataAnnotations.ValidationContext validationDuplicateContext = new System.ComponentModel.DataAnnotations.ValidationContext(viewModelWithItemsQuantityOver, serviceProvider.Object, null);
            Assert.True(viewModelWithItemsQuantityOver.Validate(validationDuplicateContext).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModel1 = new GarmentUnitDeliveryOrderViewModel
            {
                UnitDOType = "MARKETING",
                UnitDODate= DateTimeOffset.Now.AddDays(3)
            };
            Assert.True(viewModel1.Validate(null).Count() > 0);
        }*/

        /*[Fact]
        public async Task Should_Success_Get_Data_For_GarmentUnitExpenditureNote()
        {
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<List<GarmentUnitDeliveryOrderViewModel>>(It.IsAny<List<GarmentUnitDeliveryOrder>>()))
                .Returns(new List<GarmentUnitDeliveryOrderViewModel>
                {
                    new GarmentUnitDeliveryOrderViewModel
                    {
                        Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                        {
                            new GarmentUnitDeliveryOrderItemViewModel()
                        }
                    }
                });

            var serviceProvider = GetServiceProvider();
            serviceProvider
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(mapper.Object);

            GarmentUnitDeliveryOrderFacade facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), serviceProvider.Object);
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadForUnitExpenditureNote();
            Assert.NotEmpty(Response.Data);
        }*/

        //[Fact]
        //public async Task Should_Success_Get_Data_Item_By_Id()
        //{
        //    var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    var Response = facade.ReadItemById((int)data.Items.Take(1).Select(x => x.Id).Single());
        //    Assert.NotNull(Response);
        //}
        /*[Fact]
        public async Task Should_Success_Get_Feature_NoBc()
        {
            var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            */
        [Fact]
        public async Task Should_Success_Validate_Data()
        {
            GarmentUnitDeliveryOrderViewModel viewModel = new GarmentUnitDeliveryOrderViewModel {
                UnitRequest = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitSender = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitDOType = "TRANSFER" };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModelNullItems = new GarmentUnitDeliveryOrderViewModel
            {
                RONo = "RONo"
            };
            Assert.True(viewModelNullItems.Validate(null).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModelWithItems = new GarmentUnitDeliveryOrderViewModel
            {
                RONo = "RONo",
                Items = new List<GarmentUnitDeliveryOrderItemViewModel>
                {
                    new GarmentUnitDeliveryOrderItemViewModel
                    {
                        IsSave = true,
                        Quantity = 0
                    }
                }
            };
            Assert.True(viewModelWithItems.Validate(null).Count() > 0);

            GarmentUnitDeliveryOrderViewModel viewModelLL = new GarmentUnitDeliveryOrderViewModel
            {
                UnitRequest = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitSender = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
                {
                    Id = "1"
                },
                UnitDOType = "LAIN-LAIN"
            };
            Assert.True(viewModelLL.Validate(null).Count() > 0);
        }
        
        //[Fact]
        //public async Task Should_Success_Get_Data_For_GarmentUnitExpenditureNote()
        //{
        //    var mapper = new Mock<IMapper>();
        //    mapper.Setup(m => m.Map<List<GarmentUnitDeliveryOrderViewModel>>(It.IsAny<List<GarmentUnitDeliveryOrder>>()))
        //        .Returns(new List<GarmentUnitDeliveryOrderViewModel>
        //        {
        //            new GarmentUnitDeliveryOrderViewModel
        //            {
        //                Items = new List<GarmentUnitDeliveryOrderItemViewModel>
        //                {
        //                    new GarmentUnitDeliveryOrderItemViewModel()
        //                }
        //            }
        //        });

        //    var serviceProvider = GetServiceProvider();
        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IMapper)))
        //        .Returns(mapper.Object);

        //    GarmentUnitDeliveryOrderFacade facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), serviceProvider.Object);
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    var Response = facade.ReadForUnitExpenditureNote();
        //    Assert.NotEmpty(Response.Data);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Feature_NoBc()
        //{
        //    var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();
        //    var facadeReport = new BeacukaiNoFeature(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //[Fact]
        //public async Task Should_Success_Validate_Data()
        //{
        //    GarmentUnitDeliveryOrderViewModel viewModel = new GarmentUnitDeliveryOrderViewModel {
        //        UnitRequest = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
        //        {
        //            Id = "1"
        //        },
        //        UnitSender = new Lib.ViewModels.NewIntegrationViewModel.UnitViewModel
        //        {
        //            Id = "1"
        //        },
        //        UnitDOType = "TRANSFER" };
        //    Assert.True(viewModel.Validate(null).Count() > 0);

        //    GarmentUnitDeliveryOrderViewModel viewModelNullItems = new GarmentUnitDeliveryOrderViewModel
        //    {
        //        RONo = "RONo"
        //    };
        //    Assert.True(viewModelNullItems.Validate(null).Count() > 0);

            //foreach (var item in data.Items)
            //{
            //    PO = item.POSerialNumber;
            //    RO = item.RONo;
            //}

            //var Response1 = facadeReport.GetBeacukaiNo("BCNo", "");
            //var Response2 = facadeReport.GetBeacukaiNo("PONo", PO);
            //var Response3 = facadeReport.GetBeacukaiNo("RONo", RO);

            //Assert.NotNull(Response1);
            //Assert.NotNull(Response2);
            //Assert.NotNull(Response3);

            //var facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //GarmentDeliveryOrder dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();

            //foreach (var i in dataDO.Items)
            //{
            //    foreach (var d in i.Details)
            //    {
            //        d.POSerialNumber = "PONO123";
            //        d.RONo = "RONO123";
            //    }
            //}

            //await facadeDO.Create(dataDO, USERNAME);

            //var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            //GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME, dataDO);

            //data.CustomsType = "BC 23";
            //var Responses = await facade.Create(data, USERNAME);

            //var facadeReport = new BeacukaiNoFeature(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            //var Response1 = facadeReport.GetBeacukaiNo("BCNo", data.BeacukaiNo);
            //var Response2 = facadeReport.GetBeacukaiNo("PONo", "PONO123");
            //var Response3 = facadeReport.GetBeacukaiNo("RONo", "RONO123");

            //Assert.NotNull(Response1);
            //Assert.NotNull(Response2);
            //Assert.NotNull(Response3);
        // }

        //[Fact]
        //public async Task Should_Success_ReadForLeftOver()
        //{
        //    var facade = new GarmentUnitDeliveryOrderFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetTestData();

        //    var Response = facade.ReadForLeftOver("1,1");
        //    Assert.NotNull(Response);
        //}
    }
}
