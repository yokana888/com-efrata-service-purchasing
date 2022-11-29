using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager.CacheData;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Utils
{
    public class CoreDataTest
    {
        [Fact]
        public void Should_Success_Build_ExternalPurchaseDeliveryOrderDurationReportViewModel()
        {
            var viewModel = new ExternalPurchaseDeliveryOrderDurationReportViewModel();
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void Should_Success_Generate_Memory_Cache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IMemoryCacheManager))).Returns(new MemoryCacheManager(memoryCache));
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ICoreHttpClientService))).Returns(new CoreHttpClientServiceTest());

            var coreData = new CoreData(mockServiceProvider.Object);

            try
            {
                coreData.SetBankAccount();
                coreData.SetCategoryCOA();
                coreData.SetDivisionCOA();
                coreData.SetPPhCOA();
                coreData.SetUnitCOA();

                Assert.True(true);
                //coreData.
            } catch (Exception e)
            {
                throw e;
            }
        }

        [Fact]
        public void Should_Success_Build_BaseResponses()
        {
            var idCOAResult = new IdCOAResult()
            {
                COACode = "",
                Code = "",
                Id = 0
            };

            var bankAccountCOAResult = new BankAccountCOAResult()
            {   
                AccountCOA = "",
                Id = 0
            };

            var incomeTaxCOAResult = new IncomeTaxCOAResult()
            {
                COACodeCredit = "",
                Id = 0
            };

            Assert.NotNull(idCOAResult);
            Assert.NotNull(bankAccountCOAResult);
            Assert.NotNull(incomeTaxCOAResult);
        }
    }

    public class CoreHttpClientServiceTest : ICoreHttpClientService
    {
        public Task<HttpResponseMessage> GetAsync(string url, string token)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new { data = new List<object>() }))
            });
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonConvert.SerializeObject(new BaseResponse<string>()))
            });
        }
    }
}
