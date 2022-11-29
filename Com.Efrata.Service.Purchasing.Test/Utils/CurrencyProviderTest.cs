using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Utils
{
    public class CurrencyProviderTest
    {
        private const string ENTITY = "UnitReceiptNote";

        private const string USERNAME = "Unit Test";
        //private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            var httpServiceMock = new Mock<IHttpClientService>();
            httpServiceMock.Setup(s => s.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new BaseResponse<Currency>() { data = new Currency() }))
                });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpServiceMock.Object);

            return serviceProvider.Object;
        }

        [Fact]
        public async Task Should_Success_Get_Currency()
        {
            var currencyProvider = new CurrencyProvider(GetServiceProvider());
            var result = await currencyProvider.GetCurrencyByCurrencyCode("any code");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Get_Currency_Code_Date()
        {
            var currencyProvider = new CurrencyProvider(GetServiceProvider());
            var result = await currencyProvider.GetCurrencyByCurrencyCodeDate("any code", DateTimeOffset.UtcNow);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Get_Currency_List()
        {
            var currencyProvider = new CurrencyProvider(GetServiceProvider());
            var result = await currencyProvider.GetCurrencyByCurrencyCodeDateList(
                new List<Tuple<string, DateTimeOffset>>() { new Tuple<string, DateTimeOffset>("any", DateTimeOffset.UtcNow) });

            Assert.NotNull(result);
        }
    }
}
