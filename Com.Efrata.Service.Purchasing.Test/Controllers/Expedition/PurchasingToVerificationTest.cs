using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
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
   // [Collection("TestServerFixture Collection")]
    public class PurchasingToVerificationTest
    {
        //private const string MediaType = "application/json";
        //private readonly string URI = "v1/expedition/purchasing-to-verification";

        //private TestServerFixture TestFixture { get; set; }

        //private HttpClient Client
        //{
        //    get { return this.TestFixture.Client; }
        //}

        //public PurchasingToVerificationTest(TestServerFixture fixture)
        //{
        //    TestFixture = fixture;
        //}

        //[Fact]
        //public async Task Should_Success_Create_Data()
        //{
        //    PurchasingToVerificationViewModel viewModel = new PurchasingToVerificationViewModel()
        //    {
        //        UnitPaymentOrders = new List<UnitPaymentOrderViewModel>()
        //        {
        //            new UnitPaymentOrderViewModel(){
        //                No = Guid.NewGuid().ToString(),
        //                Currency = "IDR",
        //                DivisionCode = "Division",
        //                DivisionName = "Division",
        //                CategoryCode = "Category",
        //                CategoryName = "Category",
        //                SupplierCode = "Supplier",
        //                SupplierName = "Supplier",
        //                DueDate = DateTimeOffset.UtcNow,
        //                InvoiceNo = "Invoice",
        //                IncomeTax = 1,
        //                IncomeTaxId = "IncomeTaxId",
        //                IncomeTaxName = "PPH",
        //                IncomeTaxRate = 1,
        //                Vat = 500,
        //                TotalPaid = 1000000,
        //                UPODate = DateTimeOffset.UtcNow,
        //                Items = new List<UnitPaymentOrderItemViewModel>()
        //                {
        //                    new UnitPaymentOrderItemViewModel()
        //                    {
        //                        ProductId = "ProductId",
        //                        ProductCode = "ProductCode",
        //                        ProductName = "ProductName",
        //                        Price = 5000,
        //                        Quantity = 5,
        //                        UnitId = "UnitId",
        //                        UnitCode = "UnitCode",
        //                        UnitName = "UnitName",
        //                        Uom = "MTR"
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    var response = await Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(viewModel).ToString(), Encoding.UTF8, MediaType));
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}
    }
}
