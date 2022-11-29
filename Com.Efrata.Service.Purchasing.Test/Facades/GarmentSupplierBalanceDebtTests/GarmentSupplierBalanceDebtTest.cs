using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentSupplierBalanceDebtFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BalanceStockModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentSupplierBalanceDebtModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentSupplierBalanceDebtViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentSupplierBalanceDebtTests
{
    public class GarmentSupplierBalanceDebtTest
    {
        private const string ENTITY = "GarmentSupplierBalanceDebt";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);

        }

        private PurchasingDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }

        private GarmentDeliveryOrderDataUtil dataUtil(GarmentDeliveryOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            return new GarmentDeliveryOrderDataUtil(facade, garmentExternalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentSupplierBalanceDebt data = new GarmentSupplierBalanceDebt
            {
                SupplierCode = $"BuyerCode{DateTimeOffset.Now.Ticks}a",
                DOCurrencyCode = "IDR",
                DOCurrencyRate = 1,
                CodeRequirment = $"{DateTimeOffset.Now.Ticks}a",
                CreatedBy = "UnitTest",
                Import = false,
                SupplierName = "SupplierTest123",
                TotalValas = DateTimeOffset.Now.Ticks,
                TotalAmountIDR = DateTimeOffset.Now.Ticks,
                DOCurrencyId = 1,
                SupplierId = 1,
                Year = 2020,
                Items = new List<GarmentSupplierBalanceDebtItem> {
                    new GarmentSupplierBalanceDebtItem{
                        BillNo = "BP181122142947000001",
                        ArrivalDate = DateTimeOffset.Now,
                        DONo = $"{DateTimeOffset.Now.Ticks}a",
                        DOId = 1,
                        InternNo = "InternNO1234",
                        IDR = DateTimeOffset.Now.Ticks,
                        Valas =DateTimeOffset.Now.Ticks,
                        PaymentMethod = "PaymentMethodTest",
                        PaymentType = "PaymentMethodTest"

                    }
                }
            };
            var Responses = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Responses);
        }
        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var facade = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentSupplierBalanceDebt data = new GarmentSupplierBalanceDebt
            {
                SupplierCode = $"BuyerCode{DateTimeOffset.Now.Ticks}a",
                DOCurrencyCode = "IDR",
                DOCurrencyRate = 1,
                CodeRequirment = $"{DateTimeOffset.Now.Ticks}a",
                CreatedBy = "UnitTest",
                Import = false,
                SupplierName = "SupplierTest123",
                TotalValas = DateTimeOffset.Now.Ticks,
                TotalAmountIDR = DateTimeOffset.Now.Ticks,
                DOCurrencyId = 1,
                SupplierId = 1,
                Year = 2020,
                Items = new List<GarmentSupplierBalanceDebtItem> {
                    new GarmentSupplierBalanceDebtItem{
                        BillNo = "BP181122142947000001",
                        ArrivalDate = DateTimeOffset.Now,
                        DONo = $"{DateTimeOffset.Now.Ticks}a",
                        DOId = 1,
                        InternNo = "InternNO1234",
                        IDR = DateTimeOffset.Now.Ticks,
                        Valas =DateTimeOffset.Now.Ticks,
                        PaymentMethod = "PaymentMethodTest",
                        PaymentType = "PaymentMethodTest"

                    }
                }
            };
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(null, USERNAME));
            Assert.NotNull(e.Message);
        }
        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var facade = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentSupplierBalanceDebt data = new GarmentSupplierBalanceDebt
            {
                SupplierCode = $"BuyerCode{DateTimeOffset.Now.Ticks}a",
                DOCurrencyCode = "IDR",
                DOCurrencyRate = 1,
                CodeRequirment = $"{DateTimeOffset.Now.Ticks}a",
                CreatedBy = "UnitTest",
                Import = false,
                SupplierName = "SupplierTest123",
                TotalValas = DateTimeOffset.Now.Ticks,
                TotalAmountIDR = DateTimeOffset.Now.Ticks,
                DOCurrencyId = 1,
                SupplierId = 1,
                Year = 2020,
                Items = new List<GarmentSupplierBalanceDebtItem> {
                    new GarmentSupplierBalanceDebtItem{
                        BillNo = "BP181122142947000001",
                        ArrivalDate = DateTimeOffset.Now,
                        DONo = $"{DateTimeOffset.Now.Ticks}a",
                        DOId = 1,
                        InternNo = "InternNO1234",
                        IDR = DateTimeOffset.Now.Ticks,
                        Valas =DateTimeOffset.Now.Ticks,
                        PaymentMethod = "PaymentMethodTest",
                        PaymentType = "PaymentMethodTest"
                    }
                }
            };
            var Responses = await facade.Create(data, USERNAME);
            var Response = facade.Read(1, 25, "{}", null, "{}");
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            GarmentSupplierBalanceDebt data = new GarmentSupplierBalanceDebt
            {
                SupplierCode = $"BuyerCode{DateTimeOffset.Now.Ticks}a",
                DOCurrencyCode = "IDR",
                DOCurrencyRate = 1,
                CodeRequirment = $"{DateTimeOffset.Now.Ticks}a",
                CreatedBy = "UnitTest",
                Import = false,
                SupplierName = "SupplierTest123",
                TotalValas = DateTimeOffset.Now.Ticks,
                TotalAmountIDR = DateTimeOffset.Now.Ticks,
                DOCurrencyId = 1,
                SupplierId = 1,
                Year = 2020,
                Items = new List<GarmentSupplierBalanceDebtItem> {
                    new GarmentSupplierBalanceDebtItem{
                        BillNo = "BP181122142947000001",
                        ArrivalDate = DateTimeOffset.Now,
                        DONo = $"{DateTimeOffset.Now.Ticks}a",
                        DOId = 1,
                        InternNo = "InternNO1234",
                        IDR = DateTimeOffset.Now.Ticks,
                        Valas =DateTimeOffset.Now.Ticks,
                        PaymentMethod = "PaymentMethodTest",
                        PaymentType = "PaymentMethodTest"
                    }
                }
            };
            var Responses = await facade.Create(data, USERNAME);
            var Response = facade.ReadById((int)data.Id);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Loader()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var facadedebt = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facadedebt.ReadLoader(1, 25, "{}", DateTime.Now.Year);
            Assert.NotNull(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Loader_With_Params()
        {
            GarmentDeliveryOrderFacade facade = new GarmentDeliveryOrderFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var facadedebt = new GarmentSupplierBalanceDebtFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Response = facadedebt.ReadLoader(Search: "[\"BillNo\"]", Select: "{ \"billNo\" : \"BillNo\", \"dONo\" : \"DONo\", \"ArrivalDate\" : 1 }", year: DateTime.Now.Year);
            Assert.NotNull(Response.Data);
        }
        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentSupplierBalanceDebtViewModel nullViewModel = new GarmentSupplierBalanceDebtViewModel
            {
                dOCurrencyRate = 0,
            };
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            GarmentSupplierBalanceDebtViewModel viewModel = new GarmentSupplierBalanceDebtViewModel
            {
                supplier = new SupplierViewModel
                {
                    Id = 1,
                    Code = "test",
                    Import = true,
                    Name = "test"
                },

                items = new List<GarmentSupplierBalanceDebtItemViewModel> { }

            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }
        [Fact]
        public void Should_Success_Create_Data_BalanceStock()
        {
            BalanceStock balanceStock = new BalanceStock
            {
                BalanceStockId = "BS1807011000951",
                StockId = 1801,
                EPOID = "PM16205859",
                EPOItemId = 1,
                RO = "162004M",
                ArticleNo = "FAB 28.12.16/M",
                SmallestUnitQty = "9",
                PeriodeMonth = "9",
                PeriodeYear = "2018",
                OpenStock = 950.06,
                DebitStock = 0,
                CreditStock = 0,
                CloseStock = 2.74280000000181,
                OpenPrice = Convert.ToDecimal(21736103.1398),
                DebitPrice = 0,
                CreditPrice = 0,
                ClosePrice = Convert.ToDecimal(62659.1333),
                CreateDate = new DateTime(2018, 07, 01)

            };

            Assert.NotNull(balanceStock);
        }


    }
}
