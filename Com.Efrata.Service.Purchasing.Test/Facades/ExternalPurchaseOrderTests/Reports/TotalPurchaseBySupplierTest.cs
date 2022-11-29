using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade.Reports;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ExternalPurchaseOrderTests.Reports
{
//	[Collection("ServiceProviderFixture Collection")]
	public class TotalPurchaseBySupplierTest
	{
		//private IServiceProvider ServiceProvider { get; set; }
		//public TotalPurchaseBySupplierTest(ServiceProviderFixture fixture)
		//{
		//	ServiceProvider = fixture.ServiceProvider;

		//	IdentityService identityService = (IdentityService)ServiceProvider.GetService(typeof(IdentityService));
		//	identityService.Username = "Unit Test";
		//}
		//private ExternalPurchaseOrderDataUtil DataUtil
		//{
		//	get { return (ExternalPurchaseOrderDataUtil)ServiceProvider.GetService(typeof(ExternalPurchaseOrderDataUtil)); }
		//}
		
		//private TotalPurchaseFacade Facade
		//{
		//	get { return (TotalPurchaseFacade)ServiceProvider.GetService(typeof(TotalPurchaseFacade)); }
		//}
	
		//private ExternalPurchaseOrderFacade FacadeEPO
		//{
		//	get { return (ExternalPurchaseOrderFacade)ServiceProvider.GetService(typeof(ExternalPurchaseOrderFacade)); }
		//}
		//[Fact]
		//public async Task Should_Success_Get_Report_Total_Purchase_By_Supplier_Data_Null_Parameter()
		//{
		//	ExternalPurchaseOrder externalPurchaseOrder = await DataUtil.GetNewData("unit-test");
		//	await FacadeEPO.Create(externalPurchaseOrder, "unit-test", 7);
		//	var Response = Facade.GetTotalPurchaseBySupplierReport("", "","",null,null,7 );
		//	Assert.NotEqual(1, 0);
		//}
		//[Fact]
		//public async Task Should_Success_Get_Report_Total_Purchase_By_Supplier_Data_Excel_Null_Parameter()
		//{
		//	ExternalPurchaseOrder externalPurchaseOrder = await DataUtil.GetNewData("unit-test");
		//	await FacadeEPO.Create(externalPurchaseOrder, "unit-test", 7);
		//	var Response = Facade.GenerateExcelTotalPurchaseBySupplier(null,null, null, null, null,  7);
		//	Assert.IsType<System.IO.MemoryStream>(Response);
		//}
		//[Fact]
		//public void Should_Success_Get_Report_Total_Purchase_By_Supplier_null_Data_Excel()
		//{
		//	DateTime DateFrom = new DateTime(2018, 1, 1);
		//	DateTime DateTo = new DateTime(2018, 1, 1);
		//	var Response = Facade.GenerateExcelTotalPurchaseBySupplier(null,null, null, DateFrom, DateTo, 7);
		//	Assert.IsType<System.IO.MemoryStream>(Response);
		//}

	}
}
