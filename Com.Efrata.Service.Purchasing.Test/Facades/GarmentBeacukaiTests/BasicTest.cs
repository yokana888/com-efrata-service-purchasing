using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentBeacukaiDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
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

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentBeacukaiTests
{
	public class BasicTest
	{
		private const string ENTITY = "GarmentBeacukai";

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

		private Mock<IServiceProvider> GetServiceProvider()
		{
			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
			var HttpClientService = new Mock<IHttpClientService>();
			HttpClientService
				.Setup(x => x.GetAsync(It.IsAny<string>()))
				.ReturnsAsync(message);
            HttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers/byCodes"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentSupplierDataUtil().GetMultipleResultFormatterOkString()) });

            HttpClientService
                .Setup(x => x.SendAsync(It.IsAny<HttpMethod>(), It.Is<string>(s => s.Contains("master/garmentProducts")), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new GarmentProductDataUtil().GetMultipleResultFormatterOkString()) });

            var mockDebtBalanceService = new Mock<IGarmentDebtBalanceService>();
            mockDebtBalanceService
                .Setup(x => x.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .ReturnsAsync(1);

            var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			serviceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(HttpClientService.Object);

            serviceProvider
               .Setup(x => x.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(mockDebtBalanceService.Object);

            return serviceProvider;
		}
		private GarmentBeacukaiDataUtil dataUtil(GarmentBeacukaiFacade facade, string testName)
		{
			var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
			var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

			var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
			var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

			var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
			var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

			var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);
			return new GarmentBeacukaiDataUtil(garmentDeliveryOrderDataUtil ,facade);
		}

        private GarmentDeliveryOrderDataUtil dataUtilDO(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            //var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(testName));
            //var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);
            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        private GarmentUnitReceiptNoteDataUtil dataUtilURN(GarmentUnitReceiptNoteFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            return new GarmentUnitReceiptNoteDataUtil(facade, garmentDeliveryOrderDataUtil, null);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {

            //var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
            var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);

            var Response = await facade.Create(data, USERNAME);
            //Assert.NotEqual(0, Response);
            Assert.NotNull(Response);
        }
        //[Fact]
        //public async Task Should_Success_Create_Data_null_BillNo()
        //{

        //	var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //	GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);
        //	data.BillNo = "";
        //	var Response = await facade.Create(data, USERNAME);
        //	Assert.NotEqual(0, Response);
        //}

        //[Fact]
        //public async Task Should_Validate_Double_Data()
        //{
        //	var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //	GarmentBeacukai model = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);

        //	GarmentBeacukaiViewModel viewModel = new GarmentBeacukaiViewModel
        //	{
        //		supplier = new SupplierViewModel(),
        //	};
        //	viewModel.Id = model.Id + 1;
        //	viewModel.beacukaiNo = model.BeacukaiNo;
        //	viewModel.supplier.Id = model.SupplierId;
        //	viewModel.beacukaiDate = model.BeacukaiDate;


        //	Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
        //	serviceProvider.
        //		Setup(x => x.GetService(typeof(PurchasingDbContext)))
        //		.Returns(_dbContext(GetCurrentMethod()));

        //	ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider.Object, null);

        //	var validationResultCreate = viewModel.Validate(validationContext).ToList();

        //	var errorDuplicate = validationResultCreate.SingleOrDefault(r => r.ErrorMessage.Equals("No is already exist"));
        //	Assert.NotNull(errorDuplicate);
        //}
        //	[Fact]
        //	public async Task Should_Error_Create_Data()
        //	{
        //		var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //		GarmentBeacukai model = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);
        //		model.Items = null;
        //		Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
        //		Assert.NotNull(e.Message);
        //	}
        //	[Fact]
        //	public async Task Should_Success_Get_All_Data()
        //	{
        //		var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //		GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData (USERNAME);
        //		var Responses = await facade.Create(data, USERNAME);
        //		var Response = facade.Read();
        //		Assert.NotNull(Response);
        //	}

        //	[Fact]
        //	public async Task Should_Success_Get_Data_By_Id()
        //	{
        //		var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //		GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);
        //		var Responses = await facade.Create(data, USERNAME);
        //		var Response = facade.ReadById((int)data.Id);
        //		Assert.NotNull(Response);
        //	}
        [Fact]
        public async Task Should_Success_Update_Data()
        {
            //var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
            var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            var facadeDO = new GarmentDeliveryOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

            GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetTestData1(USERNAME);

            GarmentBeacukaiViewModel viewModel = await dataUtil(facade, GetCurrentMethod()).GetViewModel(USERNAME);

            var newModelItem = new GarmentBeacukaiItem
            {
                GarmentDOId = viewModel.items.First().deliveryOrder.Id,
                TotalQty = 1,
                TotalAmount = 1
            };
            data.Items.Add(newModelItem);

            List<GarmentBeacukaiItemViewModel> Newitems = new List<GarmentBeacukaiItemViewModel>();


            foreach (GarmentBeacukaiItem i in data.Items)
            {
                var newItem =
                new GarmentBeacukaiItemViewModel
                {
                    selected = true,
                    deliveryOrder = new Lib.ViewModels.GarmentDeliveryOrderViewModel.GarmentDeliveryOrderViewModel
                    {
                        Id = i.GarmentDOId,
                    },
                    Id = i.Id,

                    billNo = null,
                    quantity = 0
                };
                Newitems.Add(newItem);
            }

            viewModel.Id = data.Id;
            viewModel.items = Newitems;

            var ResponseUpdate1 = await facade.Update((int)data.Id, viewModel, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate1);

            var ResponseUpdate2 = await facade.Update((int)data.Id, viewModel, data, USERNAME);
            Assert.NotEqual(0, ResponseUpdate2);
        }

        //	[Fact]
        //	public async Task Should_Success_Delete_Data()
        //	{
        //		var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //		GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);
        //		await facade.Create(data, USERNAME);
        //		var Response = facade.Delete((int)data.Id, USERNAME);
        //		Assert.NotEqual(0, Response);
        //	}
        //	[Fact]
        //	public void Should_Error_Delete_Data()
        //	{
        //		var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //		Exception e = Assert.Throws<Exception>(() => facade.Delete(0, USERNAME));
        //		Assert.NotNull(e.Message);
        //	}
        [Fact]
		public void Should_Success_Validate_Data()
		{
			GarmentBeacukaiViewModel nullViewModel = new GarmentBeacukaiViewModel();
			Assert.True(nullViewModel.Validate(null).Count() > 0);
			GarmentBeacukaiViewModel viewModel = new GarmentBeacukaiViewModel
			{
				beacukaiNo = "",
				beacukaiDate = DateTimeOffset.MinValue,
				supplier = { },
				customType=null,
				packagingQty=0,
				netto=0,
				bruto=0,
				packaging="",
				currency = { },
				items = { },

                billNo = null,
                validationDate = DateTimeOffset.MinValue
			};
			Assert.True(viewModel.Validate(null).Count() > 0);

		}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_BC_23()
        //{
        //    var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);

        //    data.CustomsType = "BC 23";
        //    var Responses = await facade.Create(data, USERNAME);

        //    var facadeReport = new GarmentBC23ReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    var Response = facadeReport.GetReport(DateTime.Now, DateTime.Now, 1, 25, "", 7);

        //    Assert.NotNull(Response.Item1);
        //}


        //[Fact]
        //public async Task Should_Success_Get_Excel_Data_BC_23()
        //{
        //    var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);

        //    data.CustomsType = "BC 23";
        //    var Responses = await facade.Create(data, USERNAME);

        //    var facadeReport = new GarmentBC23ReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    var Response = facadeReport.GetXLs(DateTime.Now, DateTime.Now, 7);

        //    Assert.IsType<MemoryStream>(Response);
        //}
        

        [Fact]
        public async Task Should_Success_Get_by_PO()
        {

            var facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            GarmentDeliveryOrder dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();

            foreach (var i in dataDO.Items)
            {
                foreach (var d in i.Details)
                {
                    d.POSerialNumber = "PONO123";
                    d.RONo = "RONO123";
                }
            }

            await facadeDO.Create(dataDO, USERNAME);

            var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME, dataDO);

            data.CustomsType = "BC 23";
            var Responses = await facade.Create(data, USERNAME);

            //var facadeReport = new GarmentBeacukaiFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Response = facade.ReadBCByPOSerialNumbers("PONO123,PONO123");

            Assert.NotNull(Response);

        }

        [Fact]
        public async Task Should_Success_Get_DO_BC_URN()
        {
            var facadeDO = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            GarmentDeliveryOrder dataDO = await dataUtilDO(facadeDO, GetCurrentMethod()).GetNewData();

            await facadeDO.Create(dataDO, USERNAME);

            var facade = new GarmentBeacukaiFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            GarmentBeacukai data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME, dataDO);

            await facade.Create(data, USERNAME);

            var facadeurn = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            GarmentUnitReceiptNote dataUrn = await dataUtilURN(facadeurn, GetCurrentMethod()).GetTestData(dataDO, 1);

            var Response = facadeDO.GetDataDO(1);

            Assert.NotNull(Response);

        }

    }
}
