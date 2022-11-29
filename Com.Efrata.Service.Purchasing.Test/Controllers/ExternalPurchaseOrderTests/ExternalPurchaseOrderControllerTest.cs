using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.ExternalPurchaseOrderTests
{
  //  [Collection("TestServerFixture Collection")]
    public class ExternalPurchaseOrderControllerTest
    {
        //private const string MediaType = "application/json";
        //private const string MediaTypePdf = "application/pdf";
        //private readonly string URI = "v1/external-purchase-orders";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected ExternalPurchaseOrderDataUtil DataUtil
        //{
        //    get { return (ExternalPurchaseOrderDataUtil)this.TestFixture.Service.GetService(typeof(ExternalPurchaseOrderDataUtil)); }
        //}

        //public ExternalPurchaseOrderControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}
        //[Fact]
        //public async Task Should_Success_EPOPost()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel._id = model.Id;
        //    List<ExternalPurchaseOrderViewModel> viewModelList = new List<ExternalPurchaseOrderViewModel> { viewModel };
        //    var response = await this.Client.PostAsync($"{URI}/post", new StringContent(JsonConvert.SerializeObject(viewModelList).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_EPOUnpost()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.PutAsync($"{URI}/unpost/{model.Id}", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}
        //[Fact]
        //public async Task Should_Error_EPOUnpost_Data_InvalidId()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/unpost/0", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_HideUnpost()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    POExternalUpdateModel modelupdate = new POExternalUpdateModel
        //    {
        //        IsCreateOnVBRequest = true
        //    };
        //    var response = await this.Client.PutAsync($"{URI}/update-from-vb-with-po-req-finance/{model.EPONo}", new StringContent(JsonConvert.SerializeObject(modelupdate).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_HideUnpost_Data_InvalidId()
        //{
        //    POExternalUpdateModel modelupdate = new POExternalUpdateModel
        //    {
        //        IsCreateOnVBRequest = true
        //    };

        //    var response = await this.Client.PutAsync($"{URI}/update-from-vb-with-po-req-finance/0", new StringContent(JsonConvert.SerializeObject(modelupdate).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_EPOCancel()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.PutAsync($"{URI}/cancel/{model.Id}", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_EPOCancel_Data_InvalidId()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/cancel/0", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_EPOClose()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.PutAsync($"{URI}/close/{model.Id}", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    //var responseError = await this.Client.GetAsync(URI );
        //    //Assert.Equal(HttpStatusCode.InternalServerError, responseError.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_With_Filter()
        //{
        //    var response = await this.Client.GetAsync(URI + "?filter={'IsPosted':false}");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Data_By_Id()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
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
        //public async Task Should_Error_Get_Invalid_Id()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/null");
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.orderDate = DateTimeOffset.MinValue;
        //    viewModel.currency = null;
        //    viewModel.unit = null;
        //    viewModel.supplier = null;
        //    viewModel.items = new List<ExternalPurchaseOrderItemViewModel> { };
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_Data_Item()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    foreach (ExternalPurchaseOrderItemViewModel item in viewModel.items)
        //    {
        //        item.prNo = "";
        //        item.poNo = "";
        //    }
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_PO_Cash_Null()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.orderDate = DateTimeOffset.MinValue;
        //    viewModel.paymentMethod = "CASH";
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_PO_Cash_String_Empty()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.orderDate = DateTimeOffset.MinValue;
        //    viewModel.paymentMethod = "CASH";
        //    viewModel.poCashType = "";
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Invalid_PO_Cash_Supplier_Null()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.orderDate = DateTimeOffset.MinValue;
        //    viewModel.paymentMethod = "CASH";
        //    viewModel.poCashType = "DISPOSISI";
        //    viewModel.supplier = null;
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Duplicate_Item()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDuplicateDataViewModel("dev2");
        //    var item = viewModel.items.FirstOrDefault();
        //    var detail = item.details.FirstOrDefault();
        //    detail.conversion = 0;
        //    detail.productPrice = 0;
        //    detail.priceBeforeTax = double.MaxValue;
        //    item.details.Add(detail);
        //    viewModel.items.Add(item);
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Detail_Null()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDuplicateDataViewModel("dev2");
        //    var item = viewModel.items.FirstOrDefault();
        //    item.details = null;
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Data_date_more_than_expectedDeliveryDate()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    viewModel.orderDate = DateTimeOffset.MaxValue;
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Create_Data_without_Detail()
        //{
        //    ExternalPurchaseOrderViewModel viewModel = await DataUtil.GetNewDataViewModel("dev2");
        //    foreach(var item in viewModel.items)
        //    {
        //        item.details = new List<ExternalPurchaseOrderDetailViewModel> { }; ;
        //    }
        //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType);
        //    httpContent.Headers.Add("x-timezone-offset", "0");
        //    var response = await this.Client.PostAsync(URI, httpContent);
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        

        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
            
        //    var json = await responseGetById.Content.ReadAsStringAsync();
            
        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);
        //    ExternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<ExternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    foreach (var item in viewModel.items)
        //    {
        //        foreach (var detail in item.details)
        //        {
        //            detail.productPrice = 100000;
        //            detail.pricePerDealUnit = 10000;
        //        }
        //    }
        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Data_Id()
        //{
        //    var response = await this.Client.PutAsync($"{URI}/0", new StringContent(JsonConvert.SerializeObject(new ExternalPurchaseOrderViewModel()).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Invalid_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    ExternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<ExternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel.orderDate = DateTimeOffset.MinValue;
        //    viewModel.supplier = null;
        //    viewModel.unit = null;
        //    viewModel.currency = null;
        //    viewModel.items = new List<ExternalPurchaseOrderItemViewModel> { };

        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        //    //ExternalPurchaseOrderViewModel viewModel2 = JsonConvert.DeserializeObject<ExternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    //viewModel2.useIncomeTax = true;
        //    //viewModel2.incomeTax = new Lib.ViewModels.IntegrationViewModel.IncomeTaxViewModel
        //    //{
        //    //    name = "income",
        //    //    _id = "1",
        //    //    rate = "2"
        //    //};
        //    //viewModel2.incomeTaxBy = "";

        //    //var response2 = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel2).ToString(), Encoding.UTF8, MediaType));
        //    //Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_Product_Price_less_than_dealPrice_Data()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    ExternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<ExternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    viewModel.incomeTax = new Lib.ViewModels.IntegrationViewModel.IncomeTaxViewModel
        //    {
        //        _id = "1",
        //        name = "test",
        //        rate = "1"
        //    };
        //    viewModel.incomeTaxBy = null;
        //    foreach (var item in viewModel.items)
        //    {
        //        foreach (var detail in item.details)
        //        {
        //            detail.productPrice = 1;
        //            detail.pricePerDealUnit = 10000;
        //            detail.dealQuantity = 0;
        //        }
        //    }
        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_Update_PriceBeforeTax_less_than_0()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");

        //    var responseGetById = await this.Client.GetAsync($"{URI}/{model.Id}");
        //    var json = await responseGetById.Content.ReadAsStringAsync();

        //    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
        //    Assert.True(result.ContainsKey("apiVersion"));
        //    Assert.True(result.ContainsKey("message"));
        //    Assert.True(result.ContainsKey("data"));
        //    Assert.Equal("JObject", result["data"].GetType().Name);

        //    ExternalPurchaseOrderViewModel viewModel = JsonConvert.DeserializeObject<ExternalPurchaseOrderViewModel>(result.GetValueOrDefault("data").ToString());
        //    foreach (var item in viewModel.items)
        //    {
        //        foreach (var detail in item.details)
        //        {
        //            detail.productPrice = 100000;
        //            detail.pricePerDealUnit = 10000;
        //            detail.dealQuantity = 0;
        //            detail.priceBeforeTax = 0;
        //        }
        //    }
        //    var response = await this.Client.PutAsync($"{URI}/{model.Id}", new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_PDF_By_Id()
        //{
        //    ExternalPurchaseOrder model = await DataUtil.GetTestData("dev2");
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
        //public async Task Should_Success_Get_All_Data_Unused()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/unused");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_All_Data_Disposition()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/disposition");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}
    }
}
