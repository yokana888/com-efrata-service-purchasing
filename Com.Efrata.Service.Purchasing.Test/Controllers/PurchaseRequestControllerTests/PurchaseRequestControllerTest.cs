using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.PurchaseRequestControllerTests
{
    //[Collection("TestServerFixture Collection")]
    public class PurchaseRequestControllerTest
    {
        //private const string MediaType = "application/json";
        //private readonly string URI = "v1/purchase-requests";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //protected PurchaseRequestDataUtil DataUtil
        //{
        //    get { return (PurchaseRequestDataUtil)this.TestFixture.Service.GetService(typeof(PurchaseRequestDataUtil)); }
        //}

        //public PurchaseRequestControllerTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        ////[Fact]
        ////public async Task Should_Success_Get_All_Data()
        ////{
        ////    var response = await this.Client.GetAsync(URI);
        ////    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ////    var responseError = await this.Client.GetAsync(URI + "?filter={'IsPosted':}");
        ////    Assert.Equal(HttpStatusCode.InternalServerError, responseError.StatusCode);
        ////}

        //[Fact]
        //public async Task Should_Success_Get_All_Data()
        //{
        //    var response = await this.Client.GetAsync(URI);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Data_By_Id()
        //{
        //    var response = await this.Client.GetAsync($"{URI}/1");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(new PurchaseRequest()).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_PRPost()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");
        //    PurchaseRequestViewModel viewModel = DataUtil.GetNewDataViewModel();
        //    viewModel._id = model.Id;
        //    List<PurchaseRequestViewModel> viewModelList = new List<PurchaseRequestViewModel> { viewModel };
        //    var response = await this.Client.PostAsync($"{URI}/post", new StringContent(JsonConvert.SerializeObject(viewModelList).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Success_PRUnpost()
        //{
        //    PurchaseRequest model = await DataUtil.GetTestData("dev2");
        //    var response = await this.Client.PutAsync($"{URI}/unpost/{model.Id}", new StringContent("", Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

        //[Fact]
        //public async Task Should_Error_GetDataPosted()
        //{
        //    var response = await this.Client.GetAsync(URI + "?filter={'IsPosted':false}");
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}
    }
}
