using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReceiptCorrectionViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentReceiptCorrectionDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentReceiptCorrectionTests
{
    public class BasicTests
    {
        private const string ENTITY = "GarmentReceiptCorrection";
        private const string USERNAME = "unitTest";
        private IServiceProvider ServiceProvider { get; set; }

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

        private IServiceProvider GetServiceProvider()
        {
            var httpClientService = new Mock<IHttpClientService>();
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-suppliers"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new SupplierDataUtil().GetResultFormatterOkString()) });
            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/garment-currencies"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new CurrencyDataUtil().GetMultipleResultFormatterOkString()) });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            return serviceProviderMock.Object;
        }

        private GarmentReceiptCorrectionDataUtil dataUtil(GarmentReceiptCorrectionFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            var garmentExternalPurchaseOrderFacade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(testName));
            var garmentExternalPurchaseOrderDataUtil = new GarmentExternalPurchaseOrderDataUtil(garmentExternalPurchaseOrderFacade, garmentInternalPurchaseOrderDataUtil);

            var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade(GetServiceProvider(), _dbContext(testName));
            var garmentDeliveryOrderDataUtil = new GarmentDeliveryOrderDataUtil(garmentDeliveryOrderFacade, garmentExternalPurchaseOrderDataUtil);

            var garmentUnitReceiptNoteFacade = new GarmentUnitReceiptNoteFacade(GetServiceProvider(), _dbContext(testName));
            var garmentUnitReceiptNoteDataUtil = new GarmentUnitReceiptNoteDataUtil(garmentUnitReceiptNoteFacade, garmentDeliveryOrderDataUtil, null);

            return new GarmentReceiptCorrectionDataUtil(facade, garmentUnitReceiptNoteDataUtil);
        }

        private GarmentDeliveryOrderDataUtil _dataUtilDO(GarmentDeliveryOrderFacade facade, string testName)
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
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()),GetServiceProvider());
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiJumlahPlus();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var data = await dataUtil(facade, GetCurrentMethod()).GetTestDataKoreksiJumlahMinus();
            var Response = facade.ReadById((int)data.Id);
            Assert.NotEqual(0, Response.Id);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var facade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataKoreksiKonversi();
            var Response = await facade.Create(data, USERNAME);
            Assert.NotEqual(0, Response);

            var data2 = await dataUtil(facade, GetCurrentMethod()).GetNewDataKoreksiKonversi();
            var dataItem = data.Items.First();
            long nowTicks = DateTimeOffset.Now.Ticks;
            data2.Items.First().ProductId = nowTicks;
            data2.Items.First().SmallUomId = dataItem.SmallUomId;
            data2.StorageId = data.StorageId;
            var Response2 = await facade.Create(data2, USERNAME);
            Assert.NotEqual(0, Response2);

            var data3 = await dataUtil(facade, GetCurrentMethod()).GetNewDataKoreksiJumlahPlus();
            var dataItem1 = data.Items.First();
            data3.Items.First().ProductId = nowTicks;
            data3.Items.First().SmallUomId = dataItem.SmallUomId;
            data3.StorageId = nowTicks;
            var Response3 = await facade.Create(data3, USERNAME);
            Assert.NotEqual(0, Response2);
        }

        //[Fact]
        //public async Task Should_Success_Create_Data_With_Tax()
        //{
        //    var facade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
        //    var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
        //    var Response = await facade.Create(data, false, USERNAME);
        //    Assert.NotEqual(Response, 0);

        //    var data2nd = await dataUtil(facade, GetCurrentMethod()).GetNewDataWithTax();
        //    var Response2nd = await facade.Create(data2nd, false, USERNAME);
        //    Assert.NotEqual(Response2nd, 0);
        //}

        [Fact]
        public async Task Should_Error_Create_Data_Null_Items()
        {
            var facade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()), GetServiceProvider());
            var data = await dataUtil(facade, GetCurrentMethod()).GetNewDataKoreksiJumlahMinus();
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(data, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            GarmentReceiptCorrectionViewModel AllNullViewModel = new GarmentReceiptCorrectionViewModel();
            Assert.True(AllNullViewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_Null_Items()
        {
            GarmentReceiptCorrectionViewModel viewModel = new GarmentReceiptCorrectionViewModel
            {
                CorrectionType = "Harga Satuan",
                URNNo = "test",
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }


        [Fact]
        public void Should_Success_Validate_Data_Retur()
        {
            GarmentReceiptCorrectionViewModel viewModel = new GarmentReceiptCorrectionViewModel
            {
                CorrectionType = "Jumlah",
                URNNo = "test",
                CorrectionDate=DateTimeOffset.Now,
                CorrectionNo="test",
                Remark=It.IsAny<string>(),
                Unit= It.IsAny<UnitViewModel>(),
                Storage=It.IsAny<Lib.ViewModels.IntegrationViewModel.StorageViewModel>(),
                URNId= It.IsAny<long>(),
                Items = new List<GarmentReceiptCorrectionItemViewModel>
                {
                    new GarmentReceiptCorrectionItemViewModel
                    {
                        QuantityCheck=100,
                        Quantity = 0,
                        CorrectionQuantity=0,
                        Conversion=It.IsAny<double>(),
                        CorrectionConversion=It.IsAny<double>(),
                        DODetailId=It.IsAny<long>(),
                        EPOItemId=It.IsAny<long>(),
                        FabricType=It.IsAny<string>(),
                        IsSave=true,
                        PRItemId=It.IsAny<long>(),
                        RONo=It.IsAny<string>(),
                        POItemId=It.IsAny<long>(),
                        DesignColor=It.IsAny<string>(),
                        PricePerDealUnit=It.IsAny<double>(),
                        POSerialNumber=It.IsAny<string>(),
                        SmallQuantity=It.IsAny<double>(),
                        URNItemId=It.IsAny<long>(),
                        SmallUomId=It.IsAny<long>(),
                        UomId=It.IsAny<long>(),
                        UomUnit=It.IsAny<string>(),
                        SmallUomUnit=It.IsAny<string>(),
                        ProductRemark=It.IsAny<string>(),
                        Product=It.IsAny<ProductViewModel>(),
                        OrderQuantity=It.IsAny<double>(),
                    },
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);

            GarmentReceiptCorrectionViewModel viewModel2 = new GarmentReceiptCorrectionViewModel
            {
                CorrectionType = "Konversi",
                URNNo = "test",
                Items = new List<GarmentReceiptCorrectionItemViewModel>
                {
                    new GarmentReceiptCorrectionItemViewModel
                    {
                        QuantityCheck=100,
                        Quantity = 500,
                        CorrectionQuantity=0,
                        CorrectionConversion=0,
                        IsSave=true,
                        OrderQuantity=200
                    },
                }
            };
            Assert.True(viewModel2.Validate(null).Count() > 0);

            GarmentReceiptCorrectionViewModel viewModel3 = new GarmentReceiptCorrectionViewModel
            {
                CorrectionType = "Jumlah",
                URNNo = "test",
                Items = new List<GarmentReceiptCorrectionItemViewModel>
                {
                    new GarmentReceiptCorrectionItemViewModel
                    {
                        QuantityCheck=100,
                        Quantity = 500,
                        CorrectionQuantity=-200,
                        IsSave=true,

                    },
                }
            };
            Assert.True(viewModel3.Validate(null).Count() > 0);

            GarmentReceiptCorrectionViewModel viewModel4 = new GarmentReceiptCorrectionViewModel
            {
                CorrectionType = "Konversi",
                URNNo = "test",
                Items = new List<GarmentReceiptCorrectionItemViewModel>
                {
                    new GarmentReceiptCorrectionItemViewModel
                    {
                        QuantityCheck=100,
                        Quantity = 500,
                        CorrectionQuantity=0,
                        CorrectionConversion=1,
                        IsSave=true,
                        OrderQuantity=1000
                    },
                }
            };
            Assert.True(viewModel4.Validate(null).Count() > 0);
        }



        //[Fact]
        //public async Task Should_Success_Get_All_Data_Report2()
        //{
        //    var dbContext = _dbContext(GetCurrentMethod());
        //    var garmentDeliveryOrderFacade = new GarmentDeliveryOrderFacade( GetServiceProvider(), dbContext);
        //    var modelLocalDO = _dataUtilDO(garmentDeliveryOrderFacade, GetCurrentMethod());
        //    var dataDO = await modelLocalDO.GetNewData5();

        //    var garmentCorrection = new GarmentCorrectionNotePriceFacade(GetServiceProvider(), dbContext);
        //    var dataUtilCorr = new GarmentCorrectionNoteDataUtil(garmentCorrection, modelLocalDO);
        //    var modelLocalCorr = await dataUtilCorr.GetTestData2(modelLocalDO);
        //}


        [Fact]
        public async Task Should_Success_Get_All_Data_Report()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());
            

            var facadeDO = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
            var dataUtilDO = _dataUtilDO(facadeDO, GetCurrentMethod());

            var FacadeCorrection = new GarmentCorrectionNotePriceFacade(serviceProvider, dbContext);
            var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(FacadeCorrection, dataUtilDO);

            var FacadeUnitReceipt = new GarmentUnitReceiptNoteFacade(serviceProvider, dbContext);
            var dataUtilUnitReceipt = new GarmentUnitReceiptNoteDataUtil(FacadeUnitReceipt, dataUtilDO, null);

            var Facade = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilReceiptCorr = new GarmentReceiptCorrectionDataUtil(Facade, dataUtilUnitReceipt);

            var dataDO = await dataUtilDO.GetTestData5();

            var dataCorr = await dataUtilCorrection.GetTestData2(dataDO);

            long nowTicks = DateTimeOffset.Now.Ticks;
            var dataUnit = await dataUtilUnitReceipt.GetTestData(dataDO, nowTicks);

            var dataReceipt = await dataUtilReceiptCorr.GetTestData(dataUnit);

            var dateFrom = DateTimeOffset.MinValue;
            var dateTo = DateTimeOffset.UtcNow;
            var facade1 = new GarmentReceiptCorrectionReportFacade(dbContext, serviceProvider);

            var Response = facade1.GetReport(dataReceipt.UnitCode, null, dateFrom, dateTo, "{}", 1, 25);

            Assert.NotNull(Response.Item1);
        }


        [Fact]
        public async Task Should_Success_Get_Excel()
        {
            var serviceProvider = GetServiceProvider();
            var dbContext = _dbContext(GetCurrentMethod());


            var facadeDO = new GarmentDeliveryOrderFacade(serviceProvider, dbContext);
            var dataUtilDO = _dataUtilDO(facadeDO, GetCurrentMethod());

            var FacadeCorrection = new GarmentCorrectionNotePriceFacade(serviceProvider, dbContext);
            var dataUtilCorrection = new GarmentCorrectionNoteDataUtil(FacadeCorrection, dataUtilDO);

            var FacadeUnitReceipt = new GarmentUnitReceiptNoteFacade(serviceProvider, dbContext);
            var dataUtilUnitReceipt = new GarmentUnitReceiptNoteDataUtil(FacadeUnitReceipt, dataUtilDO, null);

            var Facade = new GarmentReceiptCorrectionFacade(dbContext, serviceProvider);
            var dataUtilReceiptCorr = new GarmentReceiptCorrectionDataUtil(Facade, dataUtilUnitReceipt);

            var dataDO = await dataUtilDO.GetTestData5();

            var dataCorr = await dataUtilCorrection.GetTestData2(dataDO);

            long nowTicks = DateTimeOffset.Now.Ticks;
            var dataUnit = await dataUtilUnitReceipt.GetTestData(dataDO, nowTicks);

            var dataReceipt = await dataUtilReceiptCorr.GetTestData(dataUnit);

            var dateFrom = DateTimeOffset.MinValue;
            var dateTo = DateTimeOffset.UtcNow;
            var facade1 = new GarmentReceiptCorrectionReportFacade(dbContext, serviceProvider);



            var Response = facade1.GenerateExcel(dataReceipt.UnitCode, null, dateFrom, dateTo, "{}");
            //var garmentReceiptCorrectionFacade = new GarmentReceiptCorrectionFacade(_dbContext(GetCurrentMethod()),GetServiceProvider() );
            // var dataUtilReceiptNote = await dataUtil(Facade, GetCurrentMethod()).GetTestData();

            Assert.IsType<System.IO.MemoryStream>(Response);
        }

    }
}
