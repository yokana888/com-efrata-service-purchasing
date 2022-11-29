using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;

namespace Com.Efrata.Service.Purchasing.Test
{
    public class TestServerFixture
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }
        public IServiceProvider Service { get; }

        public TestServerFixture()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    //DANLIRISTESTENVIRONMENT
                    new KeyValuePair<string, string>(Constant.SECRET, "DANLIRISTESTENVIRONMENT"),
                    new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    new KeyValuePair<string, string>(Constant.PURCHASING_ENDPOINT, "http://localhost:5004/v1/"),
                   //new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=(localdb)\\mssqllocaldb;Database=com-danliris-db-test;Trusted_Connection=True;MultipleActiveResultSets=true"),
                    new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=localhost,1401;Database=com.Efrata.db.purchasing.controller.test;User Id=sa;Password=Standar123.;MultipleActiveResultSets=True;"),
                    new KeyValuePair<string, string>(Constant.MONGODB_CONNECTION, "mongodb://localhost:27017/admin")
                })
                .Build();


            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services
                       .AddTransient<SendToVerificationDataUtil>()
                       .AddTransient<PurchasingDocumentAcceptanceDataUtil>()
                       .AddTransient<PurchaseRequestDataUtil>()
                       .AddTransient<PurchaseRequestItemDataUtil>()
                       .AddTransient<InternalPurchaseOrderDataUtil>()
                       .AddTransient<InternalPurchaseOrderItemDataUtil>()
                       .AddTransient<ExternalPurchaseOrderDataUtil>()
                       .AddTransient<ExternalPurchaseOrderItemDataUtil>()
                       .AddTransient<ExternalPurchaseOrderDetailDataUtil>()
                       .AddTransient<DeliveryOrderDataUtil>()
                       .AddTransient<DeliveryOrderItemDataUtil>()
                       .AddTransient<DeliveryOrderDetailDataUtil>()
                       .AddTransient<UnitReceiptNoteDataUtil>()
                       .AddTransient<UnitReceiptNoteItemDataUtil>()
                       .AddTransient<UnitPaymentOrderDataUtil>()
                       .AddTransient<UnitPaymentCorrectionNoteDataUtil>()
                       .AddTransient<UnitPaymentPriceCorrectionNoteDataUtils>()
                       .AddTransient<UnitPaymentCorrectionNoteDataUtil>()
                       .AddTransient<UnitPaymentOrderUnpaidReportDataUtil>()
                       .AddScoped<IHttpClientService, HttpClientTestService>()
                       .AddScoped<ICurrencyProvider, CurrencyProvider>()
                       .AddDbContext<PurchasingDbContext>(options => options.UseSqlServer(configuration[Constant.DEFAULT_CONNECTION]), ServiceLifetime.Transient);
                })
                .UseStartup<Startup>();

            string authority = configuration["Authority"];
            string clientId = configuration["ClientId"];

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Service = _server.Host.Services;

            var User = new { username = "dev2", password = "Standar123" };

            HttpClient httpClient = new HttpClient();

            var response = httpClient.PostAsync("http://localhost:5000/v1/authenticate", new StringContent(JsonConvert.SerializeObject(User).ToString(), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();

            var data = response.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Result.ToString());
            var token = result["data"].ToString();

            IdentityService identityService = Service.GetService<IdentityService>();
            identityService.Username = User.username;

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }

    [CollectionDefinition("TestServerFixture Collection")]
    public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
