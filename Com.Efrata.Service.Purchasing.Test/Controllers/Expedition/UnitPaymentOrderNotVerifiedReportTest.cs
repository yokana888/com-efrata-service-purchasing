using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Expedition
{
    [Collection("TestServerFixture Collection")]
    public class UnitPaymentOrderNotVerifiedReportTest
    {
        //private const string MediaType = "application/json";
        //private readonly string URI = "v1/purchasing/unit-payment-orders-not-verified-report/history";
        //private readonly string URINotHistory = "v1/purchasing/unit-payment-orders-not-verified-report";


        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //public UnitPaymentOrderNotVerifiedReportTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    DateTimeOffset yesterday = DateTimeOffset.UtcNow.AddDays(-1);
        //    DateTimeOffset tomorrow = DateTimeOffset.UtcNow.AddDays(1);
        //    string param = "?dateFrom=" + yesterday.ToString("yyyy-MM-dd") + "&dateTo=" + tomorrow.ToString("yyyy-MM-dd") + "&page=1&size=25";
        //    var response = await this.Client.GetAsync(URI+param);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Error_Get_All_Data_History()
        //{
        //    var yesterday = "aa";
        //    var tomorrow = "aa";
        //    string param = "?dateFrom=" + yesterday.ToString() + "&dateTo=" + tomorrow.ToString();
        //    var response = await this.Client.GetAsync(URI + param);
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel()
        //{
        //    var response = await this.Client.GetAsync(URI + "/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_Not_History()
        //{
        //    DateTimeOffset yesterday = DateTimeOffset.UtcNow.AddDays(-1);
        //    DateTimeOffset tomorrow = DateTimeOffset.UtcNow.AddDays(1);
        //    string param = "?dateFrom=" + yesterday.ToString("yyyy-MM-dd") + "&dateTo=" + tomorrow.ToString("yyyy-MM-dd") + "&page=1&size=25";
        //    var response = await this.Client.GetAsync(URINotHistory + param);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Error_Get_All_Data_Not_History()
        //{
        //    var yesterday = "aa";
        //    var tomorrow = "aa";
        //    string param = "?dateFrom=" + yesterday.ToString() + "&dateTo=" + tomorrow.ToString();
        //    var response = await this.Client.GetAsync(URINotHistory + param);
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        //    //var json = await response.Content.ReadAsStringAsync();
        //    //Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    //Assert.True(result.ContainsKey("apiVersion"));
        //    //Assert.True(result.ContainsKey("data"));
        //    //Assert.True(result["data"].GetType().Name.Equals("JArray"));
        //}

       

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_Not_History()
        //{
        //    var response = await this.Client.GetAsync(URINotHistory + "/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

    }
}
