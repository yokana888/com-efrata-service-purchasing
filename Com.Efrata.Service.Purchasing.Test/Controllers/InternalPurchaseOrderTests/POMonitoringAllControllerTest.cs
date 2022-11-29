using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.InternalPurchaseOrderTests
{
    //[Collection("TestServerFixture Collection")]
    public class POMonitoringAllControllerTest
    {
        //private const string MediaType = "application/json";
        ////private const string MediaTypePdf = "application/pdf";
        //private readonly string URI = "v1/purchase-orders/monitoring";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected InternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (InternalPurchaseOrderDataUtil)this.TestFixture.Service.GetService(typeof(InternalPurchaseOrderDataUtil)); }
        //}

        //public POMonitoringAllControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report()
        //{
        //    var response = await this.Client.GetAsync(URI  + "?page=1&size=25");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel()
        //{
        //    var response = await this.Client.GetAsync(URI + "/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        ////[Fact]
        ////public async Task Should_Error_Get_Report_Without_Page()
        ////{
        ////    var response = await this.Client.GetAsync(URI + "?page=0&size=0");
        ////    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        ////}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_Empty_Data()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/download?unitId=0");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_ByUser()
        //{
        //    var response = await this.Client.GetAsync(URI + "/by-user?page=1&size=25");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_ByUser()
        //{
        //    var response = await this.Client.GetAsync(URI + "/by-user/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        ////[Fact]
        ////public async Task Should_Error_Get_Report_Without_Page_ByUser()
        ////{
        ////    var response = await this.Client.GetAsync(URI + "/by-user?page=0&size=0");
        ////    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        ////}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_Empty_Data_ByUser()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/by-user/download?unitId=0");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}


        //#region Sarmut Staff
        //[Fact]
        //public async Task Should_Success_Get_Report_Staffs()
        //{
        //    var response = await this.Client.GetAsync(URI + "/staffs");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = response.Content.ReadAsStringAsync().Result;
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}



        //[Fact]
        //public async Task Should_Success_Get_Report_StaffsDetail()
        //{
        //    var response = await this.Client.GetAsync(URI + "/subStaffs");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = response.Content.ReadAsStringAsync().Result;
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_StaffDetail()
        //{
        //    var response = await this.Client.GetAsync(URI + "/subStaffs/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

       

       // #endregion
    }
}
