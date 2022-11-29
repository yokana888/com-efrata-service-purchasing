using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDailyPurchasingReportFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentInvoiceTests
{

	public class BasicTest
	{
		private const string ENTITY = "GarmentInvoice";

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
				.EnableSensitiveDataLogging()
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

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }

        private GarmentInvoiceDataUtil dataUtil(GarmentInvoiceFacade facade, string testName)
		{
			var garmentInvoiceFacade = new GarmentInvoiceFacade(_dbContext(testName), ServiceProvider);
			var garmentInvoiceDetailDataUtil = new GarmentInvoiceDetailDataUtil();
			var garmentInvoiceItemDataUtil = new GarmentInvoiceItemDataUtil(garmentInvoiceDetailDataUtil);
			var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
			var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

			var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
			var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

			var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
			var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

			var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);
			return new GarmentInvoiceDataUtil(garmentInvoiceItemDataUtil,garmentInvoiceDetailDataUtil,garmentDeliveryOrderDataUtil,facade );
		}

		//[Fact]
		//public async Task Should_Success_Create_Data()
		//{
			 
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		
		//	var Response = await facade.Create(data, USERNAME);
		//	Assert.NotEqual(0, Response);
		//	GarmentInvoice data2 = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	DateTime dateWithoutOffset = new DateTime(2010,8, 16, 13, 32, 00);
		//	data2.InvoiceDate = dateWithoutOffset;
		//	var Response1 = await facade.Create(data2, USERNAME);
		//	Assert.NotEqual(0, Response1);
		//}

		//[Fact]
		//public async Task Should_Validate_Double_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice model = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);

		//	GarmentInvoiceViewModel viewModel = new GarmentInvoiceViewModel
		//	{
		//		supplier = new SupplierViewModel(),
		//	};
		//	viewModel.Id = model.Id + 1;
		//	viewModel.invoiceNo = model.InvoiceNo;
		//	viewModel.supplier.Id = model.SupplierId;
		//	viewModel.invoiceDate = model.InvoiceDate;
		 

		//	Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
		//	serviceProvider.
		//		Setup(x => x.GetService(typeof(PurchasingDbContext)))
		//		.Returns(_dbContext(GetCurrentMethod()));

		//	ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider.Object, null);

		//	var validationResultCreate = viewModel.Validate(validationContext).ToList();

		//	var errorDuplicate = validationResultCreate.SingleOrDefault(r => r.ErrorMessage.Equals("No is already exist"));
		//	Assert.NotNull(errorDuplicate);
		//}
		//[Fact]
		//public async Task Should_Error_Create_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice model =await  dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	model.Items = null;
		//	Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(model, USERNAME));
		//	Assert.NotNull(e.Message);
		//}


		//[Fact]
		//public async Task Should_Success_Get_All_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	var Responses = await facade.Create(data, USERNAME);
		//	var Response = facade.Read();
		//	Assert.NotNull(Response);
		//}

		//[Fact]
		//public async Task Should_Success_Get_Data_By_Id()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	var Responses = await facade.Create(data, USERNAME);
		//	var Response = facade.ReadById((int)data.Id);
		//	Assert.NotNull(Response);
		//}
		//[Fact]
		//public async Task Should_Success_Update_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	var facadeDO = new GarmentDeliveryOrderFacade(ServiceProvider,_dbContext(GetCurrentMethod()));
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	GarmentInvoiceItem item= await dataUtil(facade, GetCurrentMethod()).GetNewDataItem(USERNAME);

		//	var ResponseUpdate = await facade.Update((int)data.Id, data, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate);
			 
		//	List<GarmentInvoiceItem> Newitems = new List<GarmentInvoiceItem>(data.Items);
		//	Newitems.Add(item);
		//	data.Items = Newitems;
			 
		//	var ResponseUpdate1 = await facade.Update((int)data.Id, data, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate1);

  //          //Newitems.Remove(newItem);
  //          //data.Items = Newitems;
  //          //var ResponseUpdate2 = await facade.Update((int)data.Id, data, USERNAME);
  //          //Assert.NotEqual(ResponseUpdate2, 0);
  //      }

		//[Fact]
		//public async Task Should_Success_Update_Data2()
		//{
		//	var dbContext = _dbContext(GetCurrentMethod());
		//	var facade = new GarmentInvoiceFacade(dbContext, ServiceProvider);
		//	var facadeDO = new GarmentDeliveryOrderFacade(ServiceProvider, dbContext);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	GarmentInvoiceItem item = await dataUtil(facade, GetCurrentMethod()).GetNewDataItem(USERNAME);

		//	var ResponseUpdate = await facade.Update((int)data.Id, data, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate);

		//	List<GarmentInvoiceItem> Newitems = new List<GarmentInvoiceItem>(data.Items);
		//	Newitems.Add(item);
		//	data.Items = Newitems;

		//	var ResponseUpdate1 = await facade.Update((int)data.Id, data, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate);

		//	dbContext.Entry(data).State = EntityState.Detached;
		//	foreach (var items in data.Items)
		//	{
		//		dbContext.Entry(items).State = EntityState.Detached;
		//		foreach (var detail in items.Details)
		//		{
		//			dbContext.Entry(detail).State = EntityState.Detached;
		//		}
		//	}

		//	var newData = dbContext.GarmentInvoices.AsNoTracking()
		//		.Include(m => m.Items)
		//			.ThenInclude(i => i.Details)
		//		.FirstOrDefault(m => m.Id == data.Id);

		//	newData.Items = newData.Items.Take(1).ToList();

		//	var ResponseUpdate2 = await facade.Update((int)newData.Id, newData, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate2);
		//}

		//[Fact]
		//public async Task Should_Error_Update_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewData(USERNAME);
		//	List<GarmentInvoiceItem> item = new List<GarmentInvoiceItem>(data.Items);

		//	data.Items.Add(new GarmentInvoiceItem
		//	{
		//		DeliveryOrderId = It.IsAny<int>(),
		//		DODate = DateTimeOffset.Now,
		//		DeliveryOrderNo = "donos",
		//		ArrivalDate = DateTimeOffset.Now,
		//		TotalAmount = 2000,
		//		Details = null
		//	});

		//	var ResponseUpdate = await facade.Update((int)data.Id, data, USERNAME);
		//	Assert.NotEqual(0, ResponseUpdate);
		//	var newItem = new GarmentInvoiceItem
		//	{
		//		DeliveryOrderId = It.IsAny<int>(),
		//		DODate = DateTimeOffset.Now,
		//		DeliveryOrderNo = "dono",
		//		ArrivalDate = DateTimeOffset.Now,
		//		TotalAmount = 2000,
		//		Details =null
		//	};
		//	List<GarmentInvoiceItem> Newitems = new List<GarmentInvoiceItem>(data.Items);
		//	Newitems.Add(newItem);
		//	data.Items = Newitems;

		//	Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)data.Id, data, USERNAME));
		//	Assert.NotNull(errorNullItems.Message);
  //      }

		//[Fact]
		//public async Task Should_Success_Delete_Data()
		//{
		//	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		//	GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
		//	await facade.Create(data, USERNAME); 
		//	var Response = facade.Delete((int)data.Id, USERNAME);
		//	Assert.NotEqual(0, Response);
		//}

		//[Fact]
		////public async Task Should_Error_Delete_Data()
		////{
		////	var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
		////	Exception e = await Assert.ThrowsAsync<Exception>(async () => facade.Delete(0, USERNAME));
		////	Assert.NotNull(e.Message);
		////}

  //      public void Should_Error_Delete_Data()
  //      {
  //          var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
  //          Exception e = Assert.Throws<Exception>(() => facade.Delete(0, USERNAME));
  //          Assert.NotNull(e.Message);
  //      }

        [Fact]
		public void Should_Success_Validate_Data()
		{
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            GarmentInvoiceViewModel nullViewModel = new GarmentInvoiceViewModel();
			Assert.True(nullViewModel.Validate(null).Count() > 0);
            var tomorrow = DateTime.Now.Date.AddDays(+1);

            
            GarmentInvoiceViewModel viewModel = new GarmentInvoiceViewModel
			{
				invoiceNo = "",
				invoiceDate = tomorrow,
				supplier = { },
				incomeTaxId = It.IsAny<int>(),
				incomeTaxName = "name",
				incomeTaxNo = "",
				incomeTaxDate = DateTimeOffset.MinValue,
				incomeTaxRate = 2,
				vatNo = "",
                vatId = It.IsAny<int>(),
                vatRate = 10,
                vatDate = DateTimeOffset.MinValue,
				useIncomeTax = true,
				useVat = true,
				isPayTax = true,
				hasInternNote = false,
				currency = { },
				items = new List<GarmentInvoiceItemViewModel>
					{
						new GarmentInvoiceItemViewModel
						{
							deliveryOrder =null,
							 
							details= new List<GarmentInvoiceDetailViewModel>
							{
								new GarmentInvoiceDetailViewModel
								{
									doQuantity=0
                                    
								}
							}
						}
					}
			};

            System.ComponentModel.DataAnnotations.ValidationContext garmentInvoiceValidate1 = new System.ComponentModel.DataAnnotations.ValidationContext(viewModel, serviceProvider.Object, null);
            Assert.True(viewModel.Validate(garmentInvoiceValidate1).Count() > 0);
            

            GarmentInvoiceViewModel viewModels = new GarmentInvoiceViewModel
			{
				invoiceNo = "",
				invoiceDate = DateTimeOffset.MinValue,
				supplier = { },
				incomeTaxId = It.IsAny<int>(),
				incomeTaxName = "name",
				incomeTaxNo = "",
				incomeTaxDate = DateTimeOffset.MinValue,
				incomeTaxRate = 2,
				vatNo = "",
                vatId = It.IsAny<int>(),
                vatRate = 10,
                vatDate = DateTimeOffset.MinValue,
				useIncomeTax = true,
				useVat = true,
				isPayTax = true,
				hasInternNote = false,
				currency =new CurrencyViewModel{ Id = It.IsAny<int>(), Code = "USD", Symbol = "$", Rate = 13000, Description = "" },
				items = new List<GarmentInvoiceItemViewModel>
					{
						new GarmentInvoiceItemViewModel
						{
							deliveryOrder =null,
						
							details= null
						}
					}
			};

            System.ComponentModel.DataAnnotations.ValidationContext garmentInvoiceValidate11 = new System.ComponentModel.DataAnnotations.ValidationContext(viewModels, serviceProvider.Object, null);

            Assert.True(viewModels.Validate(garmentInvoiceValidate11).Count() > 0);

            GarmentInvoiceViewModel viewModels1 = new GarmentInvoiceViewModel
            {
                invoiceNo = "",
                invoiceDate = DateTimeOffset.MinValue,
                supplier = { },
                incomeTaxId = It.IsAny<int>(),
                incomeTaxName = "",
                incomeTaxNo = "",
                incomeTaxDate = DateTimeOffset.MinValue,
                incomeTaxRate = 2,
                vatNo = "",
                vatId = It.IsAny<int>(),
                vatRate = 10,
                vatDate = DateTimeOffset.MinValue,
                useIncomeTax = true,
                useVat = true,
                isPayTax = true,
                poSerialNumber="any",
                hasInternNote = false,
                currency = new CurrencyViewModel { Id = It.IsAny<int>(), Code = "USD", Symbol = "$", Rate = 13000, Description = "" },
                items = new List<GarmentInvoiceItemViewModel>
                    {
                        new GarmentInvoiceItemViewModel
                        {
                            deliveryOrder =null,

                            details= null
                        }
                    }
            };

            System.ComponentModel.DataAnnotations.ValidationContext garmentInvoiceValidate12 = new System.ComponentModel.DataAnnotations.ValidationContext(viewModels1, serviceProvider.Object, null);
            Assert.True(viewModels1.Validate(garmentInvoiceValidate12).Count() > 0);

            GarmentInvoiceViewModel viewModels12 = new GarmentInvoiceViewModel
            {
                invoiceNo = "",
                invoiceDate = DateTimeOffset.MinValue,
                supplier = { },
                incomeTaxId = It.IsAny<int>(),
                incomeTaxName = "",
                incomeTaxNo = "",
                incomeTaxDate = DateTimeOffset.MinValue,
                incomeTaxRate = 2,
                vatNo = "",
                vatId = It.IsAny<int>(),
                vatRate = 10,
                vatDate = DateTimeOffset.MinValue,
                useIncomeTax = true,
                useVat = true,
                isPayTax = true,
                poSerialNumber = "any",
                hasInternNote = false,
                currency = new CurrencyViewModel { Id = It.IsAny<int>(), Code = "USD", Symbol = "$", Rate = 13000, Description = "" },
                items = new List<GarmentInvoiceItemViewModel>
                    {
                        new GarmentInvoiceItemViewModel
                        {
                            deliveryOrder = new GarmentDeliveryOrderViewModel {
                                doNo = "test1",
                                arrivalDate = tomorrow,
                                doDate = tomorrow
                            },

                            details= null
                        },

                        new GarmentInvoiceItemViewModel
                        {
                            deliveryOrder = new GarmentDeliveryOrderViewModel {
                                doNo = "test1",
                                arrivalDate = tomorrow,
                                doDate = tomorrow
                            },

                            details= null
                        }
                    }
            };

            System.ComponentModel.DataAnnotations.ValidationContext garmentInvoiceValidate13 = new System.ComponentModel.DataAnnotations.ValidationContext(viewModels12, serviceProvider.Object, null);
            Assert.True(viewModels12.Validate(garmentInvoiceValidate13).Count() > 0);

        }

        //[Fact]
        //public async Task Should_Success_Get_Data_By_DOId()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetTestData(USERNAME);
        //    var Response = facade.ReadByDOId((int)data.Items.First().DeliveryOrderId);
        //    Assert.NotNull(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_For_InternNote()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);
        //    var Responses = await facade.Create(data, USERNAME);
        //    var Response = facade.ReadForInternNote(new List<long> { data.Id });
        //    Assert.NotEmpty(Response);
        //}

        //// Buku Harian Pembelian
        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Data()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);

        //    GarmentDailyPurchasingReportFacade DataInv = new GarmentDailyPurchasingReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    DateTime d1 = data.InvoiceDate.DateTime;
        //    DateTime d2 = data.InvoiceDate.DateTime;

        //    var Response = DataInv.GetGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Null_Parameter()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);

        //    GarmentDailyPurchasingReportFacade DataInv = new GarmentDailyPurchasingReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    DateTime d1 = data.InvoiceDate.DateTime.AddDays(30);
        //    DateTime d2 = data.InvoiceDate.DateTime.AddDays(30);

        //    var Response = DataInv.GetGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.NotNull(Response.Item1);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);

        //    GarmentDailyPurchasingReportFacade DataInv = new GarmentDailyPurchasingReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    DateTime d1 = data.InvoiceDate.DateTime;
        //    DateTime d2 = data.InvoiceDate.DateTime;

        //    var Response = DataInv.GenerateExcelGDailyPurchasingReport(null, true, null, null, null,null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Buku_Sub_Beli_Excel_Null_Parameter()
        //{
        //    var facade = new GarmentInvoiceFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    GarmentInvoice data = await dataUtil(facade, GetCurrentMethod()).GetNewDataViewModel(USERNAME);

        //    GarmentDailyPurchasingReportFacade DataInv = new GarmentDailyPurchasingReportFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

        //    DateTime d1 = data.InvoiceDate.DateTime.AddDays(30);
        //    DateTime d2 = data.InvoiceDate.DateTime.AddDays(30);

        //    var Response = DataInv.GenerateExcelGDailyPurchasingReport(null, true, null, null,null, null, 7);
        //    Assert.IsType<System.IO.MemoryStream>(Response);
        //}
    }
}