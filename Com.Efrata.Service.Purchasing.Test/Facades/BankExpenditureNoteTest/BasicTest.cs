using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote;
using Com.Efrata.Service.Purchasing.Test.DataUtils.BankExpenditureNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.BankExpenditureNoteTest
{
    public class BasicTest
    {
        private const string ENTITY = "BankExpenditureNote";
        private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;

        [MethodImpl(MethodImplOptions.NoInlining)]
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

        private BankExpenditureNoteDataUtil _dataUtil(BankExpenditureNoteFacade facade, string testName)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());


            PurchasingDocumentExpeditionFacade pdeFacade = new PurchasingDocumentExpeditionFacade(serviceProvider.Object, _dbContext(testName));
            SendToVerificationDataUtil stvDataUtil = new SendToVerificationDataUtil(pdeFacade);
            pdaDataUtil = new PurchasingDocumentAcceptanceDataUtil(pdeFacade, stvDataUtil);

            return new BankExpenditureNoteDataUtil(facade, pdaDataUtil);
        }

        private Mock<IServiceProvider> GetServiceProviderMock()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(new CurrencyProviderTestService());

            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            ReadResponse<object> Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Unit_Payment_Order()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            _dataUtil(facade, GetCurrentMethod());
            PurchasingDocumentExpedition model = await pdaDataUtil.GetCashierTestData();

            var filter = new
            {
                model.Currency
            };
            var filterJson = JsonConvert.SerializeObject(filter);

            var Response = facade.GetAllByPosition(1, 25, "{}", model.UnitPaymentOrderNo, filterJson);
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Unit_Payment_Order_Verification_Filter()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            _dataUtil(facade, GetCurrentMethod());
            PurchasingDocumentExpedition model = await pdaDataUtil.GetCashierTestData();

            var filter = new
            {
                verificationFilter = 1
            };
            var filterJson = JsonConvert.SerializeObject(filter);

            var Response = facade.GetAllByPosition(1, 25, "{}", model.UnitPaymentOrderNo, filterJson);
            Assert.NotNull(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Posting_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Posting(new List<long>() { model.Id });
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Posting_Data_Debit_Greater_Than_Credit()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData2();
            var Response = facade.Posting(new List<long>() { model.Id });
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Create_Data_Vat_null()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetNewDataVatZero();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Create_Import_Supplier_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetImportData();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_And_Error_With_AmountPaid_and_Expedition()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            var exception = await Assert.ThrowsAsync<Exception>(() => facade.Create(model, identityService));
            Assert.NotNull(exception.Message);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            BankExpenditureNoteDetailModel modelDetail = await _dataUtil(facade, GetCurrentMethod()).GetNewDetailGarmentData();
            model.Details.Clear();
            model.Details.Add(modelDetail);
            var Response = await facade.Update((int)model.Id, model, identityService);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel Data = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            int AffectedRows = await facade.Delete((int)Data.Id, identityService);
            Assert.True(AffectedRows > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            BankExpenditureNoteViewModel vm = new BankExpenditureNoteViewModel()
            {
                Date = null,
                Bank = null,
                Details = new List<BankExpenditureNoteDetailViewModel>()
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Date_Greater_Than_Now()
        {
            BankExpenditureNoteViewModel vm = new BankExpenditureNoteViewModel()
            {
                Date = DateTime.Now.AddDays(2),
                Bank = null,
                Details = null
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_Report_Data()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            ReadResponse<object> response = facade.GetReport(1, 25, null, null, null, null, null, null, null, null, 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Report_Data_With_Params()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            ReadResponse<object> response = facade.GetReport(1, 25, "", "", "", "", null, null, null, null, 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Report_Data_With_Date()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            ReadResponse<object> response = facade.GetReport(1, 25, null, null, null, null, null, null, new DateTimeOffset(), new DateTimeOffset(), 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Report_Data_With_Date_And_Params()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            ReadResponse<object> response = facade.GetReport(1, 25, "Test", "Test", "Test", "Test", "Test", "Test", new DateTimeOffset(), new DateTimeOffset(), 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Succes_Create_Report_VM_Instance()
        {
            BankExpenditureNoteReportViewModel reportViewModel = new BankExpenditureNoteReportViewModel()
            {
                CategoryName = "",
                DivisionName = "",
                Date = new DateTimeOffset(),
                Currency = "",
                BankName = "",
                DocumentNo = "",
                DPP = 0,
                InvoiceNumber = "",
                PaymentMethod = "",
                SupplierName = "",
                TotalPaid = 0,
                UnitPaymentOrderNo = "",
                VAT = 0,
                DifferenceNominal = 0
            };

            Assert.NotNull(reportViewModel);
        }

        [Fact]
        public async Task Should_Success_Create_Daily_Bank_Transaction()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Daily_Bank_Transaction_IDR()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            numberGeneratorMock.Setup(s => s.GenerateDocumentNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("test-code");
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetNewDataIDR();
            IdentityService identityService = new IdentityService()
            {
                Token = "Token",

                Username = "Unit Test"
            };
            var Response = await facade.Create(model, identityService);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_ByPeriod()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();

            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            var result = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetByPeriod(result.Date.Month, result.Date.Year, 0);
            Assert.NotEmpty(Response);

            var Response2 = facade.GetByPeriod(0, 0, 0);
            Assert.NotEmpty(Response2);
        }

        [Fact]
        public void Should_Success_InitiateExpenditureInfo()
        {
            var expenditureInfo = new ExpenditureInfo()
            {
                BankName = "",
                BGCheckNumber = "",
                DocumentNo = ""
            };

            Assert.NotNull(expenditureInfo);
            Assert.True(string.IsNullOrWhiteSpace(expenditureInfo.BankName));
            Assert.True(string.IsNullOrWhiteSpace(expenditureInfo.BGCheckNumber));
            Assert.True(string.IsNullOrWhiteSpace(expenditureInfo.DocumentNo));
        }

        [Fact]
        public async Task Should_Success_Get_PDF_By_Id()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GeneratePdfTemplate(model, 0);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_PDF_IDR_NONIDR_By_Id()
        {
            var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            BankExpenditureNoteFacade facade = new BankExpenditureNoteFacade(_dbContext(GetCurrentMethod()), numberGeneratorMock.Object, GetServiceProviderMock().Object);
            BankExpenditureNoteModel model = await _dataUtil(facade, GetCurrentMethod()).GetTestDataIDRNONIDR();
            var Response = facade.GeneratePdfTemplate(model, 0);
            Assert.NotNull(Response);
        }
    }
}
