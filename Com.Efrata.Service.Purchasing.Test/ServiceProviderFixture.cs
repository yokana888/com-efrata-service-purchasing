using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Serializers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseOrder;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNote;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using Xunit;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.DeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Test.DataUtils.BankExpenditureNoteDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade.Reports;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.UnitPaymentOrderDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternNoteDataUtils;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.PurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCentralBillReceptionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCentralBillExpenditureFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteReceptionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteExpenditureFacades;
using Com.Efrata.Service.Purchasing.Lib.Utilities.Currencies;

namespace Com.Efrata.Service.Purchasing.Test
{
    public class ServiceProviderFixture
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private void RegisterEndpoints(IConfigurationRoot Configuration)
        {
            APIEndpoint.Purchasing = Configuration.GetValue<string>(Constant.PURCHASING_ENDPOINT) ?? Configuration[Constant.PURCHASING_ENDPOINT];
        }

        private void RegisterSerializationProvider()
        {
            BsonSerializer.RegisterSerializationProvider(new SerializationProvider());
        }

        private void RegisterClassMap()
        {
            ClassMap<UnitReceiptNoteViewModel>.Register();
            ClassMap<UnitReceiptNoteItemViewModel>.Register();
            ClassMap<UnitViewModel>.Register();
            ClassMap<DivisionViewModel>.Register();
            ClassMap<CategoryViewModel>.Register();
            ClassMap<ProductViewModel>.Register();
            ClassMap<UomViewModel>.Register();
            ClassMap<PurchaseOrderViewModel>.Register();
            ClassMap<SupplierViewModel>.Register();
            ClassMap<UnitPaymentOrderUnpaidViewModel>.Register();
        }

        public ServiceProviderFixture()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Constant.SECRET, "DANLIRISDEVELOPMENT"),
                    new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    //new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=(localdb)\\mssqllocaldb;Database=com-danliris-db-test;Trusted_Connection=True;MultipleActiveResultSets=true"),
                    new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=localhost,1401;Database=com.Efrata.db.purchasing.service.test;User Id=sa;Password=Standar123.;MultipleActiveResultSets=True;"),
                    new KeyValuePair<string, string>(Constant.MONGODB_CONNECTION, "mongodb://localhost:27017/admin")
                })
                .Build();

            RegisterEndpoints(configuration);
            string connectionString = configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? configuration[Constant.DEFAULT_CONNECTION];
			APIEndpoint.ConnectionString = configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? configuration[Constant.DEFAULT_CONNECTION];

			this.ServiceProvider = new ServiceCollection()
                .AddDbContext<PurchasingDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Transient)
                .AddTransient<PurchasingDocumentExpeditionFacade>()
                .AddTransient<PurchasingDocumentExpeditionReportFacade>()
                .AddTransient<ImportPurchasingBookReportFacade>()
                .AddTransient<LocalPurchasingBookReportFacade>()
                .AddTransient<SendToVerificationDataUtil>()

                .AddTransient<UnitPaymentOrderUnpaidReportFacade>()
                .AddTransient<UnitPaymentOrderNotVerifiedReportFacade>()
                .AddTransient<PurchasingDocumentAcceptanceDataUtil>()
                .AddTransient<UnitReceiptNoteBsonDataUtil>()
                .AddTransient<UnitPaymentOrderUnpaidReportDataUtil>()
                .AddTransient<UnitReceiptNoteImportFalseBsonDataUtil>()

                .AddTransient<PurchaseRequestFacade>()
                .AddTransient<PurchaseRequestDataUtil>()
                .AddTransient<PurchaseRequestItemDataUtil>()
                .AddTransient<PurchaseOrderMonitoringAllFacade>()
                .AddTransient<MonitoringPriceFacade>()
                .AddTransient<PurchaseRequestGenerateDataFacade>()
                .AddTransient<ExternalPurchaseOrderGenerateDataFacade>()
                .AddTransient<UnitReceiptNoteGenerateDataFacade>()

                .AddTransient<InternalPurchaseOrderFacade>()
                .AddTransient<InternalPurchaseOrderDataUtil>()
                .AddTransient<InternalPurchaseOrderItemDataUtil>()

                .AddTransient<ExternalPurchaseOrderFacade>()
                .AddTransient<ExternalPurchaseOrderDataUtil>()
                .AddTransient<ExternalPurchaseOrderItemDataUtil>()
                .AddTransient<ExternalPurchaseOrderDetailDataUtil>()

                .AddTransient<DeliveryOrderFacade>()
                .AddTransient<IDeliveryOrderFacade, DeliveryOrderFacade>()
                .AddTransient<DeliveryOrderDataUtil>()
                .AddTransient<DeliveryOrderItemDataUtil>()
                .AddTransient<DeliveryOrderDetailDataUtil>()

                .AddTransient<BankExpenditureNoteFacade>()
                .AddTransient<BankExpenditureNoteDataUtil>()

                //.AddTransient<UnitReceiptNoteFacade>()
                .AddTransient<UnitReceiptNoteFacade>()
                .AddTransient<UnitReceiptNoteDataUtil>()
                .AddTransient<UnitReceiptNoteItemDataUtil>()
				.AddTransient<TotalPurchaseFacade>()
				.AddTransient<ImportPurchasingBookReportFacade>()
				.AddTransient<IGarmentInvoice,GarmentInvoiceFacade>()
				.AddTransient<GarmentInvoiceDataUtil>()
				.AddTransient<GarmentInvoiceItemDataUtil>()
				.AddTransient<GarmentInvoiceDetailDataUtil>()

                .AddTransient<GarmentInternNoteFacades>()
                .AddTransient<GarmentInternNoteDataUtil>()

                .AddTransient<IUnitPaymentOrderFacade, UnitPaymentOrderFacade>()
                .AddTransient<UnitPaymentOrderDataUtil>()
                .AddTransient<IUnitPaymentPriceCorrectionNoteFacade, UnitPaymentPriceCorrectionNoteFacade>()
                .AddTransient<IMonitoringCentralBillReceptionFacade, MonitoringCentralBillReceptionFacade>()
                .AddTransient<IMonitoringCentralBillExpenditureFacade, MonitoringCentralBillExpenditureFacade>()
                .AddTransient<IMonitoringCorrectionNoteReceptionFacade, MonitoringCorrectionNoteReceptionFacade>()
                .AddTransient<IMonitoringCorrectionNoteExpenditureFacade, MonitoringCorrectionNoteExpenditureFacade>()
                .AddTransient<UnitPaymentPriceCorrectionNoteDataUtils>()
                .AddTransient<UnitPaymentCorrectionNoteDataUtil>()
				.AddSingleton<IHttpClientService, HttpClientTestService>()
				.AddSingleton<ICurrencyProvider, CurrencyProvider>()
                .AddSingleton<IdentityService>()
                .BuildServiceProvider();

            RegisterSerializationProvider();
            RegisterClassMap();
            MongoDbContext.connectionString = configuration.GetConnectionString(Constant.MONGODB_CONNECTION) ?? configuration[Constant.MONGODB_CONNECTION];

            PurchasingDbContext dbContext = ServiceProvider.GetService<PurchasingDbContext>();
            if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                dbContext.Database.Migrate();
            }
        }     
    }

    [CollectionDefinition("ServiceProviderFixture Collection")]
    public class ServiceProviderFixtureCollection : ICollectionFixture<ServiceProviderFixture>
    {
    }
}
