using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager
{
    public class CoreHttpClientService : ICoreHttpClientService
    {
        private readonly HttpClient _client = new HttpClient();

        public Task<HttpResponseMessage> GetAsync(string url, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return _client.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return _client.PostAsync(url, content);
        }
    }

    public interface ICoreHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string url, string token);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
    }

}
