using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;
using Com.Efrata.Service.Purchasing.Test.DataUtils.BankExpenditureNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PPHBankExpenditureNoteDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Com.Efrata.Service.Purchasing.Test.Facades.UnitPaymentOrderPaidStatusTests
{
    public class ReportTest
    {
        private const string ENTITY = "UnitPaymentOrderPaidStatus";
        private const string USERNAME = "Unit Test";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }


        private Mock<IServiceProvider> GetServiceProvider(string testname)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(InternalPurchaseOrderFacade)))
                .Returns(new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testname)));

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProviders = services.BuildServiceProvider();
            var memoryCache = serviceProviders.GetService<IMemoryCache>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IMemoryCacheManager)))
                .Returns(new MemoryCacheManager(memoryCache));

            var mockCurrencyProvider = new Mock<ICurrencyProvider>();
            mockCurrencyProvider
                .Setup(x => x.GetCurrencyByCurrencyCode(It.IsAny<string>()))
                .ReturnsAsync((Currency)null);
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyProvider)))
                .Returns(mockCurrencyProvider.Object);

            return serviceProvider;
        }

        private UnitPaymentOrderDataUtil _dataUtil(UnitPaymentOrderFacade facade, PurchasingDbContext dbContext, string testname)
        {

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(GetServiceProvider(testname).Object, dbContext);
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(GetServiceProvider(testname).Object, dbContext);
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(GetServiceProvider(testname).Object, dbContext);
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);

            DeliveryOrderFacade deliveryOrderFacade = new DeliveryOrderFacade(dbContext, GetServiceProvider(testname).Object);
            DeliveryOrderDetailDataUtil deliveryOrderDetailDataUtil = new DeliveryOrderDetailDataUtil();
            DeliveryOrderItemDataUtil deliveryOrderItemDataUtil = new DeliveryOrderItemDataUtil(deliveryOrderDetailDataUtil);
            DeliveryOrderDataUtil deliveryOrderDataUtil = new DeliveryOrderDataUtil(deliveryOrderItemDataUtil, deliveryOrderDetailDataUtil, externalPurchaseOrderDataUtil, deliveryOrderFacade);

            UnitReceiptNoteFacade unitReceiptNoteFacade = new UnitReceiptNoteFacade(GetServiceProvider(testname).Object, dbContext);
            UnitReceiptNoteItemDataUtil unitReceiptNoteItemDataUtil = new UnitReceiptNoteItemDataUtil();
            UnitReceiptNoteDataUtil unitReceiptNoteDataUtil = new UnitReceiptNoteDataUtil(unitReceiptNoteItemDataUtil, unitReceiptNoteFacade, deliveryOrderDataUtil);

            return new UnitPaymentOrderDataUtil(unitReceiptNoteDataUtil, facade);
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

        [Fact]
        public void Should_Success_GetReport()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            
            var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
            var results = facade.GetReport(25, 1, "{}", null, null, null, "IMPORT", null, "LUNAS", null, null, null, null, 0);
            var results2 = facade.GetReport(25, 1, "{}", null, null, null, "LOCAL", null, "SUDAH BAYAR DPP+PPN", DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now, 0);
            var results3 = facade.GetReport(25, 1, "{}", null, null, null, null, null, "SUDAH BAYAR PPH", DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now, 0);
            var results4 = facade.GetReport(25, 1, "{}", null, null, null, null, null, "BELUM BAYAR", null, null, null, null, 0);

            Assert.NotNull(results);
            Assert.NotNull(results2);
            Assert.NotNull(results3);
        }

        //[Fact]
        //public async Task Should_Success_GetReport_SPB()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", null, null, null, null, null, null, null, null, null, null, 0);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results);
        //}

        //[Fact]
        //public async Task Should_Success_GetReport_SPB_local()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", null, null, null, "LOCAL", null, null, null, null, null, null, 0);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results);
        //}


        //[Fact]
        //public async Task Should_Success_GetReport_SPB_import()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", null, null, null, "IMPORT", null, null, null, null, null, null, 0);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results);
        //}
        //[Fact]
        //public async Task Should_Success_GetReport_SPB_With_Params()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, "", "", "", dateFromDue, dateToDue, dateFrom, dateTo, 1);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotEmpty(results.Data);
        //}

        //[Fact]
        //public async Task Should_Success_GetReport_SPB_LUNAS()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, "", "", "LUNAS", dateFromDue, dateToDue, dateFrom, dateTo, 1);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results.Data);
        //}

        //[Fact]
        //public async Task Should_Success_GetReport_SPB_DPP_PPN()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, "", "", "SUDAH BAYAR DPP+PPN", dateFromDue, dateToDue, dateFrom, dateTo, 1);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results.Data);
        //}

        //[Fact]
        //public async Task Should_Success_GetReport_SPB_PPH()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, "", "", "SUDAH BAYAR PPH", dateFromDue, dateToDue, dateFrom, dateTo, 1);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results.Data);
        //}

        //[Fact]
        //public async Task Should_Success_GetReport_BELUM_BAYAR()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var unitPaymentOrderFacade = new UnitPaymentOrderFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var modelLocalSupplier = await _dataUtil(unitPaymentOrderFacade, dbContext, GetCurrentMethod()).GetNewData();
        //    var responseLocalSupplier = await unitPaymentOrderFacade.Create(modelLocalSupplier, USERNAME, false);

        //    var purchasingDocumentExpeditionFacade = new PurchasingDocumentExpeditionFacade(GetServiceProvider(GetCurrentMethod()).Object, dbContext);
        //    var sendToVerificationDataUtil = new SendToVerificationDataUtil(purchasingDocumentExpeditionFacade);
        //    var purchasingDocumentExpedition = sendToVerificationDataUtil.GetNewData(modelLocalSupplier);
        //    PurchasingDocumentExpedition model = purchasingDocumentExpedition;
        //    await purchasingDocumentExpeditionFacade.SendToVerification(new List<PurchasingDocumentExpedition>() { model }, "Unit Test");

        //    var facade = new UnitPaymentOrderPaidStatusReportFacade(dbContext);
        //    var dateTo = modelLocalSupplier.Date;
        //    var dateFrom = modelLocalSupplier.Date;
        //    var dateToDue = modelLocalSupplier.DueDate;
        //    var dateFromDue = modelLocalSupplier.DueDate;
        //    var results = facade.GetReport(25, 1, "{}", modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, "", "", "BELUM BAYAR", dateFromDue, dateToDue, dateFrom, dateTo, 1);
        //    // var results = await facade.GetReport(25,1,"{}",modelLocalSupplier.UPONo, modelLocalSupplier.SupplierCode, modelLocalSupplier.DivisionCode, null, dateFromDue, dateToDue, dateFrom, dateTo, 1);

        //    Assert.NotNull(results.Data);
        //}



    }
}
