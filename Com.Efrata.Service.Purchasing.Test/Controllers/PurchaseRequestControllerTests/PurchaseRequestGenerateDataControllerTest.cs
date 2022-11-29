using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Com.Efrata.Service.Purchasing.Test.Controllers.PurchaseRequestControllerTests
{
  //  [Collection("TestServerFixture Collection")]
    public class PurchaseRequestGenerateDataControllerTest
    {
        //private const string MediaType = "application/json";
        //private readonly string URI = "v1/generating-data/purchase-request";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected PurchaseRequestDataUtil DataUtil
        //{
        //    get { return (PurchaseRequestDataUtil)this.TestFixture.Service.GetService(typeof(PurchaseRequestDataUtil)); }
        //}

        //public PurchaseRequestGenerateDataControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel()
        //{
        //    var response = await this.Client.GetAsync(URI + "/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Data_Excel()
        //{
        //    HttpRequestMessage requestMessage = new HttpRequestMessage()
        //    {
        //        RequestUri = new Uri($"{Client.BaseAddress}{URI}/download"),
        //        Method = HttpMethod.Get
        //    };
        //    requestMessage.Headers.Add("x-timezone-offset", "B");
        //    var response = await this.Client.SendAsync(requestMessage);
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}
    }
}
