using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringUnitReceiptFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitExpenditureDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentReportTests
{
	public class TraceableBeacukaiFacadeTest
	{
		private const string ENTITY = "TraceableBeacukai";

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
			message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"codeRequirement\":\"BB\",\"code\":\"BB\",\"rate\":13700.0,\"name\":\"FABRIC\",\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
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

		public GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil(GarmentUnitReceiptNoteFacade garmentUnitReceiptNoteFacade, string testName)
		{
			var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

			var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
			var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

			var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

			var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

			return new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);
		}

		private GarmentUnitDeliveryOrderDataUtil UnitDOdataUtil(GarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade, string testName)
		{
			var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentUnitReceiptNoteDataUtil = this.garmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, testName);

			return new GarmentUnitDeliveryOrderDataUtil(garmentUnitDeliveryOrderFacade, garmentUnitReceiptNoteDataUtil);
		}

		private GarmentUnitExpenditureNoteDataUtil UnitExpenditureNoteDataUtil(GarmentUnitExpenditureNoteFacade  garmentUnitExpenditureNoteFacade, string testName)
		{
			//var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(GetServiceProvider().Object, _dbContext(testName));
			//var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

			//var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
			//var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

			//var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(GetServiceProvider().Object, _dbContext(testName));
			//var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

			var garmentUnitDeliveryOrderFacade = new GarmentUnitDeliveryOrderFacade(_dbContext(testName),GetServiceProvider().Object);

			var GarmentUnitExpenditureNoteFacade = new GarmentUnitExpenditureNoteFacade(GetServiceProvider().Object, _dbContext(testName));
			var garmentUnitDeliveryOrderDataUtil = this.UnitDOdataUtil(garmentUnitDeliveryOrderFacade, testName);

			return new GarmentUnitExpenditureNoteDataUtil(GarmentUnitExpenditureNoteFacade, garmentUnitDeliveryOrderDataUtil);
		}

		[Fact]
		public async Task Should_Success_Get_Trace_ByBUM()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var urnfacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

			var facade = new TraceableBeacukaiFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await UnitExpenditureNoteDataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
            var data1 = await garmentUnitReceiptNoteDataUtil(urnfacade, GetCurrentMethod()).GetNewDataWithStorage();

            string Year = data.ExpenditureDate.ToString("yy");
			string Month = data.ExpenditureDate.ToString("MM");
			string Day = data.ExpenditureDate.ToString("dd");



			//long nowTicks =  DateTimeOffset.Now.Ticks;
			var urnno = string.Concat("BUM",data1.UnitCode,Year, Month, Day,"001");

			var Response = facade.Read(urnno);
			Assert.NotNull(Response);
		}

		[Fact]
		public async Task Should_Success_Get_Excel_Trace_ByBUM()
		{

			var dbContext = _dbContext(GetCurrentMethod());
			var facadeExpend = new GarmentUnitExpenditureNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var urnfacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

			var facade = new TraceableBeacukaiFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var data = await UnitExpenditureNoteDataUtil(facadeExpend, GetCurrentMethod()).GetTestDataMonitoringFlow();
			//var data1 = garmentUnitReceiptNoteDataUtil(urnfacade, GetCurrentMethod()).GetTestDataWithStorage();

			long nowTicks = DateTimeOffset.Now.Ticks;
			var urnno = string.Concat("BUMUnitCode", nowTicks);

			var Response = facade.GetExceltracebyBUM(urnno);
			Assert.NotNull(Response);
		}
	}
}
