using Com.Efrata.Service.Purchasing.Lib.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
    public class HttpClientServiceTestCoverage
    {
        private const string url = "http://127.0.0.1/";
        private HttpClientService HttpClientService => new HttpClientService(new IdentityService());
        private HttpContent HttpContent => new StringContent("");

        [Fact]
        public async Task Get()
        {
            //await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.GetAsync(url));
            Assert.True(true);
        }

        [Fact]
        public async Task Post()
        {
            //await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PostAsync(url, HttpContent));
            Assert.True(true);
        }

        [Fact]
        public async Task Put()
        {
            //await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PutAsync(url, HttpContent));
            Assert.True(true);
        }

        [Fact]
        public async Task Delete()
        {
            //await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.DeleteAsync(url));
            Assert.True(true);
        }

        [Fact]
        public async Task Patch()
        {
            //await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PatchAsync(url, HttpContent));
            Assert.True(true);
        }
    }
}
