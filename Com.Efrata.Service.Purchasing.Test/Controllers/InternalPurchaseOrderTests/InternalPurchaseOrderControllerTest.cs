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

namespace Com.Efrata.Service.Purchasing.Test.Controllers.InternalPurchaseOrderControllerTests
{
   // [Collection("TestServerFixture Collection")]
    public class InternalPurchaseOrderControllerTest
    {
        //private const string MediaType = "application/json";
        ////private const string MediaTypePdf = "application/pdf";
        //private readonly string URI = "v1/internal-purchase-orders";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected InternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (InternalPurchaseOrderDataUtil)this.TestFixture.Service.GetService(typeof(InternalPurchaseOrderDataUtil)); }
        //}

        //public InternalPurchaseOrderControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_All_User()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/all");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Invalid_Id()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        ////[Fact]
        ////public async Task Should_Success_Create_Data()
        ////{
        ////    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        ////    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        ////    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        ////}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data()
        //{
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.prNo = null;
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<InternalPurchaseOrderItemViewModel> { };
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Null_Item_Data()
        //{
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<InternalPurchaseOrderItemViewModel> {null} ;
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_when_Quantity_is_same()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    InternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<InternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_when_Quantity_is_bigger()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    InternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<InternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());

        //    foreach (var items in viewModel.items)
        //    {
        //            items._id = 0;
        //            items.quantity = items.quantity + 1;
        //    }

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_Id()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/0", new StringContent(JsonConvert.SerializeObject(new InternalPurchaseOrderViewModel()).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Invalid_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    InternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<InternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel.prNo = null;
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<InternalPurchaseOrderItemViewModel> { };

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data_By_Id()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.DeleteAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Delete_Data_Invalid_Id()
        //{
        //    var response = await this.Client.DeleteAsync($"{URI}/0");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Succes_Split_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/spliting/{model.Id}");
        //    var json = responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.True(result["data"].GetType().Name.Equals("JObject"));

        //    InternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<InternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel._id = 1;
        //    viewModel.prNo = null;
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    viewModel.items = new List<InternalPurchaseOrderItemViewModel> { };

        //    var response = await this.Client.PutAsync($"{URI}/spliting/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Split_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel._id = 0;
        //    foreach (var items in viewModel.items)
        //    {
        //        items._id = 0;
        //    }
        //    List<InternalPurchaseOrderViewModel> viewModelList = new List<InternalPurchaseOrderViewModel> { viewModel };

        //    var response = await this.Client.PostAsync($"{URI}/spliting/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModelList).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}
        //[Fact]
        //public async Task Should_Success_Split_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    //InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.True(result["data"].GetType().Name.Equals("JObject"));

        //    InternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<InternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Split_Data()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel._id = model.Id;
        //    List<InternalPurchaseOrderViewModel> viewModelList = new List<InternalPurchaseOrderViewModel> { viewModel };
        //    var response = await this.Client.PostAsync($"{URI}/spliting/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModelList).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Split_Data_with_Validation()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.prNo = null;
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    //List<InternalPurchaseOrderViewModel> viewModelList = new List<InternalPurchaseOrderViewModel> { viewModel };
        //    var response = await this.Client.PostAsync($"{URI}/spliting/{viewModel._id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Split_Data_When_Quantity_Splited_Bigger_than_Quantity_Before_Splited()
        //{
        //    InternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    InternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.prNo = null;
        //    viewModel.prDate = DateTimeOffset.MinValue;
        //    viewModel.budget = null;
        //    viewModel.unit = null;
        //    viewModel.category = null;
        //    foreach (var items in viewModel.items)
        //    {
        //        items._id = 0;
        //        items.quantity = items.quantity + items.quantity;
        //    }
        //    //List<InternalPurchaseOrderViewModel> viewModelList = new List<InternalPurchaseOrderViewModel> { viewModel };

        //    var response = await this.Client.PostAsync($"{URI}/spliting/{viewModel._id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report()
        //{
        //    var response = await this.Client.GetAsync(URI+"/monitoring" + "?page=1&size=25");
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
        //    var response = await this.Client.GetAsync(URI + "/monitoring/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Report_Without_Page()
        //{
        //    var response = await this.Client.GetAsync(URI + "/monitoring");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_Empty_Data()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/monitoring/download?unitId=0");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        ////Monitoring PO Internal Belum PO Eksternal
        //[Fact]
        //public async Task Should_Success_Get_Report_UnProcessed()
        //{
        //    var response = await this.Client.GetAsync(URI + "/monitoring-unprocessed" + "?page=1&size=25");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var json = await response.Content.ReadAsStringAsync();
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());

        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JArray", result["data"].GetType().Name);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Excel_UnProcessed()
        //{
        //    var response = await this.Client.GetAsync(URI + "/monitoring-unprocessed/download");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Report_Without_Page_UnProcessed()
        //{
        //    var response = await this.Client.GetAsync(URI + "/monitoring-unprocessed");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Get_Report_Excel_UnProcessed()
        //{
        //    var response = await this.Client.GetAsync(URI + "/monitoring-unprocessed");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}
    }
}
