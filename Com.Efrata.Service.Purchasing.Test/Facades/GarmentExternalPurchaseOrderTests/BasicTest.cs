using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacade.Reports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades.Reports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentExternalPurchaseOrderTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentExternalPurchaseOrder";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
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
                .Setup(x => x.GetAsync(It.IsRegex($"^master/garment-suppliers")))
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        apiVersion = "1.0",
                        statusCode = 200,
                        message = "Ok",
                        data = JsonConvert.SerializeObject(new SupplierViewModel { })
                    }))
                });

            HttpClientService
                .Setup(x => x.GetAsync(It.IsRegex($"^master/garmentProducts")))
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        apiVersion = "1.0",
                        statusCode = 200,
                        message = "Ok",
                        data = JsonConvert.SerializeObject(new GarmentProductViewModel { })
                    }))
                });

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
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

        private GarmentExternalPurchaseOrderDataUtil dataUtil(GarmentExternalPurchaseOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            return new GarmentExternalPurchaseOrderDataUtil(facade, garmentInternalPurchaseOrderDataUtil);
        }

        private GarmentDeliveryOrderDataUtil dataUtilDO(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Fabric()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataFabric();
            var Response = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Fabric_OB()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataFabric();

            var Response = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Acc()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            var Response = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Acc_FREE()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            data.PaymentMethod = "CMT";
            data.PaymentType = "FREE";
            var Response = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            model.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_ReadItemByPOSerialNumberLoader()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.ReadItemByPOSerialNumberLoader("PO", "{'PO_SerialNumber' : 'PO'}", 50);
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            var Responses = await facade.Create(data, USERNAME);
            var Response = facade.ReadById((int)data.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            //string nowTicksA = $"{nowTicks}a";
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataFabric();
            List<GarmentExternalPurchaseOrderItem> item = new List<GarmentExternalPurchaseOrderItem>(data.Items);

            data.Items.Add(new GarmentExternalPurchaseOrderItem
            {
                PO_SerialNumber = "PO_SerialNumber",
                ProductId = item[0].ProductId,
                PRId = item[0].PRId,
                POId = item[0].POId,
                ProductCode = "item.ProductCode",
                ProductName = "item.ProductName",
                DealQuantity = 2,
                BudgetPrice = 100,
                DealUomId = 1,
                DealUomUnit = "unit",
                Remark = "item.ProductRemark",
                IsOverBudget = true,
                OverBudgetRemark = "OB"
            });

            var ResponseUpdate = await facade.Update((int)data.Id, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate);
            var newItem = new GarmentExternalPurchaseOrderItem
            {

                PO_SerialNumber = "PO_SerialNumber",
                ProductId = item[0].ProductId,
                PRId = item[0].PRId,
                POId = item[0].POId,
                ProductCode = "ProductCode",
                ProductName = "ProductName",
                DealQuantity = 2,
                BudgetPrice = 100,
                DealUomId = 1,
                DealUomUnit = "unit",
                Remark = "ProductRemark",
                IsOverBudget = true,
                OverBudgetRemark = "OB"
            };
            List<GarmentExternalPurchaseOrderItem> Newitems = new List<GarmentExternalPurchaseOrderItem>(data.Items);
            Newitems.Add(newItem);
            data.Items = Newitems;

            var ResponseUpdate1 = await facade.Update((int)data.Id, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate1);

            data.PaymentMethod = "CMT";
            data.PaymentType = "FREE";
            var ResponseUpdate2 = await facade.Update((int)data.Id, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate2);

            List<GarmentExternalPurchaseOrderItem> Newitems1 = new List<GarmentExternalPurchaseOrderItem>(data.Items);
            var newItem2 = new GarmentExternalPurchaseOrderItem
            {
                PO_SerialNumber = "PO_SerialNumber2",
                ProductId = item[0].ProductId,
                PRId = item[0].PRId,
                POId = item[0].POId,
                ProductCode = "ProductCode",
                ProductName = "ProductName",
                DealQuantity = 2,
                BudgetPrice = 100,
                DealUomId = 1,
                DealUomUnit = "unit",
                Remark = "ProductRemark",
                IsOverBudget = true,
                OverBudgetRemark = "OB"
            };

            Newitems1.Add(newItem2);
            data.Items = Newitems1;
            data.PaymentMethod = "CMT";
            data.PaymentType = "FREE";
            var ResponseUpdate3 = await facade.Update((int)data.Id, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate3);
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(0, model, USERNAME));
            Assert.NotNull(errorInvalidId.Message);

            model.Items = null;
            Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)model.Id, model, USERNAME));
            Assert.NotNull(errorNullItems.Message);
        }

        [Fact]
        public async Task Should_Success_EPOPost()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            List<GarmentExternalPurchaseOrder> modelList = new List<GarmentExternalPurchaseOrder>();
            GarmentExternalPurchaseOrder model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            modelList.Add(model);
            var Response = facade.EPOPost(modelList, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_EPOApprove()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            List<GarmentExternalPurchaseOrder> modelList = new List<GarmentExternalPurchaseOrder>();
            GarmentExternalPurchaseOrder model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            modelList.Add(model);
            var Response = facade.EPOApprove(modelList, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_EPOUnpost()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentExternalPurchaseOrder model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.EPOUnpost((int)model.Id, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_EPOCancel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentExternalPurchaseOrder model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.EPOCancel((int)model.Id, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_EPOClose()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentExternalPurchaseOrder model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.EPOClose((int)model.Id, "Unit Test");
            Assert.NotEqual(0, Response);
        }


        [Fact]
        public async Task Should_Error_EPOUnpost()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();

            Exception errorInvalidId = Assert.Throws<Exception>(() => facade.EPOUnpost(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Error_EPOCancel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();

            Exception errorInvalidId = Assert.Throws<Exception>(() => facade.EPOCancel(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        [Fact]
        public async Task Should_Error_EPOClose()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();

            Exception errorInvalidId = Assert.Throws<Exception>(() => facade.EPOClose(0, USERNAME));
            Assert.NotNull(errorInvalidId.Message);
        }

        //public async Task Should_Error_EPOClose()
        //{
        //    GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();

        //    Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => facade.EPOClose(0, USERNAME));
        //    Assert.NotNull(errorInvalidId.Message);
        //}

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataAcc();
            var Response = facade.Delete((int)data.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Error_Delete_Data()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

            Exception e = Assert.Throws<Exception>(() => facade.Delete(0, USERNAME));
            Assert.NotNull(e.Message);
        }

        //public async Task Should_Error_Delete_Data()
        //{
        //    GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    Exception e = await Assert.ThrowsAsync<Exception>(async () => facade.Delete(0, USERNAME));
        //    Assert.NotNull(e.Message);
        //}

        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentExternalPurchaseOrderViewModel nullViewModel = new GarmentExternalPurchaseOrderViewModel();
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentExternalPurchaseOrderViewModel viewModel = new GarmentExternalPurchaseOrderViewModel
            {
                Category = "FABRIC",
                Supplier = new SupplierViewModel(),
                IsUseVat = true,
                Vat =  new VatViewModel()
                {
                    Id = 0,
                    Rate = 0
                },
                Items = new List<GarmentExternalPurchaseOrderItemViewModel>
                {
                    new GarmentExternalPurchaseOrderItemViewModel(),
                    new GarmentExternalPurchaseOrderItemViewModel
                    {
                        Product = new GarmentProductViewModel(),
                        DealUom = new UomViewModel(),
                        SmallUom= new UomViewModel(),

                        PRId = 0,
                        PONo = null,
                        DefaultQuantity = 0,
                        DefaultUom = null,
                        SmallQuantity = 0,
                        UsedBudget = 0,
                        DOQuantity = 0,
                        OverBudgetRemark = null,
                        UENItemId = null
                    },
                    new GarmentExternalPurchaseOrderItemViewModel
                    {
                        Product = new GarmentProductViewModel{ Name = "PROCESS"},
                        DealUom = new UomViewModel(),
                        SmallUom= new UomViewModel(),

                        PRId = 0,
                        PONo = null,
                        DefaultQuantity = 0,
                        DefaultUom = null,
                        SmallQuantity = 0,
                        UsedBudget = 0,
                        DOQuantity = 0,
                        OverBudgetRemark = null,
                        UENItemId = null
                    }
                },

                UENId = null,
                BudgetRate = 0
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Supplier()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            var Responses = await facade.Create(data, USERNAME);
            var Response = facade.ReadBySupplier(data.SupplierCode);
            Assert.NotNull(Response);

            var Response2 = facade.ReadItemByRO();
            Assert.NotNull(Response2);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_RO()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataFabric();

            var Responses = await facade.Create(data, USERNAME);
            var ro = data.Items.First().RONo;
            var Response = facade.ReadItemByRO();
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_EPONoSimply()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataACC();
            var Response = facade.ReadItemByEPONoSimply(data.EPONo, "{}", 0, 0, 1, 10);
            Assert.NotNull(Response);
        }

        //Duration
        [Fact]
        public async Task Should_Success_Get_Report_POEDODuration_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var Facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GetEPODODurationReport("", "", "0-30 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response.Item1);

            var Response1 = Facade.GetEPODODurationReport("", "", "31-60 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response1.Item1);

            var Response2 = Facade.GetEPODODurationReport("", "", ">60 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response2.Item1);

        }

        [Fact]
        public async Task Should_Success_Get_Report_POEDODuration_Excel()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var Facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GenerateExcelEPODODuration("", "", "0-30 hari", DateTime.MinValue, DateTime.MaxValue, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);

            var Response1 = Facade.GenerateExcelEPODODuration("", "", "0-30 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response1);

            var Response2 = Facade.GenerateExcelEPODODuration("", "", ">60 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response2);
        }

        //OVER BUDGET
        [Fact]
        public async Task Should_Success_Get_Report_POOverBudget_Data()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var Facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GetEPOOverBudgetReport(null, null, null, null, null, null, 1, 25, "{}", 7);
            var Response1 = Facade.GetEPOOverBudgetReport(null, null, null, "BELUM", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response.Item1);
            Assert.NotNull(Response1.Item1);

        }

        [Fact]
        public async Task Should_Success_Get_Report_POOverBudget_Excel()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await dataUtilDO(facade, GetCurrentMethod()).GetNewData();
            await facade.Create(data, USERNAME);
            var Facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = Facade.GenerateExcelEPOOverBudget(null, null, null, null, null, null, 1, 25, "{}", 7);
            Assert.IsType<System.IO.MemoryStream>(Response);

        }

        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TotalGarmentPurchaseFacade facadetotal = new TotalGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var Response = facadetotal.GetTotalGarmentPurchaseBySupplierReport(model.garmentInternalPurchaseOrder.UnitId, model.garmentExternalPurchaseOrder.SupplierImport, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, null, null, 7);

            Assert.NotEmpty(Response.ToList());
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TotalGarmentPurchaseFacade facadetotal = new TotalGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var Response = facadetotal.GenerateExcelTotalGarmentPurchaseBySupplier(model.garmentInternalPurchaseOrder.UnitId, model.garmentExternalPurchaseOrder.SupplierImport, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data_Excel_Null_Parameter()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TotalGarmentPurchaseFacade facadetotal = new TotalGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Response = facadetotal.GenerateExcelTotalGarmentPurchaseBySupplier(null, It.IsAny<Boolean>(), null, null, DateTime.Now, DateTime.Now, 0);
            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        #region ToptenFacadeTest
        [Fact]
        public async Task Should_Success_Get_Report_TopTen()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TopTenGarmentPurchaseFacade facadetotal = new TopTenGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var Response = facadetotal.GetTopTenGarmentPurchaseSupplierReport(model.garmentInternalPurchaseOrder.UnitId, model.garmentExternalPurchaseOrder.SupplierImport, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, null, null, 7);

            Assert.NotEmpty(Response.ToList());
        }

        [Fact]
        public async Task Should_Success_Get_Report_TopTen_Excel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TopTenGarmentPurchaseFacade facadetotal = new TopTenGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var Response = facadetotal.GenerateExcelTopTenGarmentPurchaseSupplier(model.garmentInternalPurchaseOrder.UnitId, true, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_TopTen_Excel1()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TopTenGarmentPurchaseFacade facadetotal = new TopTenGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData1();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var Response = facadetotal.GenerateExcelTopTenGarmentPurchaseSupplier(model.garmentInternalPurchaseOrder.UnitId, false, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, null, null, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_TopTen_Excel_Null_Parameter()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TopTenGarmentPurchaseFacade facadetotal = new TopTenGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var date1 = model.garmentExternalPurchaseOrder.OrderDate.AddDays(30);
            var date2 = model.garmentExternalPurchaseOrder.OrderDate.AddDays(30);
            var Response = facadetotal.GenerateExcelTopTenGarmentPurchaseSupplier(model.garmentInternalPurchaseOrder.UnitId, true, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, date1.Date, date2.Date, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Report_TopTen_Excel_Null_Parameter1()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TopTenGarmentPurchaseFacade facadetotal = new TopTenGarmentPurchaseFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData1();

            var GEPODtl = model.garmentExternalPurchaseOrder.Items.First();
            var date1 = model.garmentExternalPurchaseOrder.OrderDate.AddDays(30);
            var date2 = model.garmentExternalPurchaseOrder.OrderDate.AddDays(30);
            var Response = facadetotal.GenerateExcelTopTenGarmentPurchaseSupplier(model.garmentInternalPurchaseOrder.UnitId, false, model.garmentExternalPurchaseOrder.PaymentMethod, GEPODtl.ProductName, date1.Date, date2.Date, 7);

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

        #endregion

        [Fact]
        public void Should_Success_Get_SupplierViewModel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var result = facade.GetSupplier(It.IsAny<long>());

            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Null_Get_SupplierViewModel()
        {
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(null);

            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));
            var result = facade.GetSupplier(It.IsAny<long>());

            Assert.Null(result);
        }

        [Fact]
        public void Should_Success_Get_GarmentProductViewModel()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var result = facade.GetProduct(It.IsAny<long>());

            Assert.NotNull(result);
        }

        [Fact]
        public void Should_Null_Get_GarmentProductViewModel()
        {
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(null);

            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));
            var result = facade.GetProduct(It.IsAny<long>());

            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Success_ReadItemForUnitDOByRO()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataFabric();

            var Responses = await facade.Create(data, USERNAME);
            var ro = data.Items.First().RONo;
            var Response = facade.ReadItemForUnitDOByRO(ro, Filter: "{'RONo':'" + ro + "'}");
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_Get_IsUnpost()
        {
            GarmentExternalPurchaseOrderFacade facade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var result = facade.GetIsUnpost(It.IsAny<int>());

            Assert.False(result);
        }

        [Fact]
        public void Should_Success_ReadItemByEPONo()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facade.ReadItemByEPONo();
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_ReadItemByEPONoSimply()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facade.ReadItemByEPONoSimply(null, "{}", 0, 0, 1, 10);
            Assert.NotNull(Response.Item1);
        }

        [Fact]
        public void Should_Success_ReadItemByROLoader()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facade.ReadItemByROLoader();
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_ReadEPOForSubconDeliveryLoader()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facade.ReadEPOForSubconDeliveryLoader();
            Assert.NotNull(Response);
        }
    }
}