using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
    public class HttpClientTestService : IHttpClientService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return Task.Run(() => new HttpResponseMessage() { Content = new StringContent("{data : {}}")});
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}
