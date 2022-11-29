using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.ExternalPurchaseOrderTests.Reports
{
	//[Collection("TestServerFixture Collection")]
	public class TotalPurchaseByUnitCategoriesControllerTest
    {
		//private const string MediaType = "application/json";
		//private readonly string URI = "v1/purchase-orders/reports/units-categories";
		//private TestServerFixture TestFixture { get; set; }
		//private HttpClient Client
		//{
		//	get { return this.TestFixture.Client; }
		//}
		//protected ExternalPurchaseOrderDataUtil DataUtil
		//{
		//	get { return (ExternalPurchaseOrderDataUtil)this.TestFixture.Service.GetService(typeof(ExternalPurchaseOrderDataUtil)); }
		//}
		//public TotalPurchaseByUnitCategoriesControllerTest(TestServerFixture fixture)
		//{
		//	TestFixture = fixture;
		//}
		//[Fact]
		//public async Task Should_Success_Get_Report()
		//{
		//	var response = await this.Client.GetAsync(URI + "?page=1&size=25");
		//	Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		//	var json = await response.Content.ReadAsStringAsync();
		//	Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
		//	Assert.True(result.ContainsKey("apiVersion"));
		//	Assert.True(result.ContainsKey("message"));
		//	Assert.True(result.ContainsKey("data"));
		//	Assert.Equal("JArray", result["data"].GetType().Name);
		//}

		//[Fact]
		//public async Task Should_Success_Get_Report_Excel()
		//{
		//	var response = await this.Client.GetAsync(URI + "/download");
		//	Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//}
	}
}
