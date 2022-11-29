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
    public class PurchaseRequestByUserControllerTest
    {
        //private const string MediaType = "application/json";
        //private const string MediaTypePdf = "application/pdf";
        //private readonly string URI = "v1/purchase-requests/by-user";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected PurchaseRequestDataUtil DataUtil
        //{
        //    get { return (PurchaseRequestDataUtil)this.TestFixture.Service.GetService(typeof(PurchaseRequestDataUtil)); }
        //}

        //public PurchaseRequestByUserControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var responseError = await this.Client.GetAsync(URI + "?filter={'IsPosted':}");
        //    Assert.Equal(HttpStatusCode.InternalServerError, responseError.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_With_Filter()
        //{
        //    var response = await this.Client.GetAsync(URI + "?filter={'IsPosted':false}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Invalid_Id()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_PDF_By_Id()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");
        //    HttpRequestMessage requestMessage = new HttpRequestMessage()
        //    {
        //        RequestUri = new Uri($"{Client.BaseAddress}{URI}/{model.Id}"),
        //        Method = HttpMethod.Get
        //    };
        //    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypePdf));
        //    requestMessage.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.SendAsync(requestMessage);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_PDF_By_Id_Except()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestDataPdf("dev2");
        //    HttpRequestMessage requestMessage = new HttpRequestMessage()
        //    {
        //        RequestUri = new Uri($"{Client.BaseAddress}{URI}/{model.Id}"),
        //        Method = HttpMethod.Get
        //    };
        //    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypePdf));
        //    requestMessage.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.SendAsync(requestMessage);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_PDF_By_Id_Except1()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestDataPdf1("dev2");
        //    HttpRequestMessage requestMessage = new HttpRequestMessage()
        //    {
        //        RequestUri = new Uri($"{Client.BaseAddress}{URI}/{model.Id}"),
        //        Method = HttpMethod.Get
        //    };
        //    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypePdf));
        //    requestMessage.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.SendAsync(requestMessage);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    PurchaseRequestViewModel viewModel = DataUtil.GetNewDataViewModel();
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data()
        //{
        //    PurchaseRequestViewModel viewModel = DataUtil.GetNewDataViewModel();
        //    viewModel.date = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<PurchaseRequestItemViewModel> { };
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data_Item()
        //{
        //    PurchaseRequestViewModel viewModel = DataUtil.GetNewDataViewModel();
        //    foreach (PurchaseRequestItemViewModel item in viewModel.items)
        //    {
        //        item.product = null;
        //        item.quantity = 0;
        //    }
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Data_date_more_than_expectedDeliveryDate()
        //{
        //    PurchaseRequestViewModel viewModel = DataUtil.GetNewDataViewModel();
        //    viewModel.date = DateTimeOffset.MaxValue;
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    PurchaseRequestViewModel viewModel = JsonConvert.DeserializeObject<PurchaseRequestViewModel>(result.GetValueOrDefault("data").ToString());

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_Id()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/0", new StringContent(JsonConvert.SerializeObject(new PurchaseRequestViewModel()).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Invalid_Data()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    PurchaseRequestViewModel viewModel = JsonConvert.DeserializeObject<PurchaseRequestViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel.date = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<PurchaseRequestItemViewModel> { };

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data_By_Id()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.DeleteAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Delete_Data_Invalid_Id()
        //{
        //    var response = await this.Client.DeleteAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

    }
}
