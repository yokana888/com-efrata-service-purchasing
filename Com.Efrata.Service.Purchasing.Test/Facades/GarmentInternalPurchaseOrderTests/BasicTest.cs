using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentInternalPurchaseOrderTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentInternalPurchaseOrder";

        private const string USERNAME = "Unit Test";
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

        private GarmentInternalPurchaseOrderDataUtil dataUtil(GarmentInternalPurchaseOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            return new GarmentInternalPurchaseOrderDataUtil(facade, garmentPurchaseRequestDataUtil);
        }

        private GarmentExternalPurchaseOrderDataUtil EPOdataUtil(GarmentExternalPurchaseOrderFacade facade, string testName)
        {
            var garmentPurchaseRequestFacade = new GarmentPurchaseRequestFacade(ServiceProvider, _dbContext(testName));
            var garmentPurchaseRequestDataUtil = new GarmentPurchaseRequestDataUtil(garmentPurchaseRequestFacade);

            var garmentInternalPurchaseOrderFacade = new GarmentInternalPurchaseOrderFacade(_dbContext(testName));
            var garmentInternalPurchaseOrderDataUtil = new GarmentInternalPurchaseOrderDataUtil(garmentInternalPurchaseOrderFacade, garmentPurchaseRequestDataUtil);

            return new GarmentExternalPurchaseOrderDataUtil(facade, garmentInternalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Create_Multiple_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.CreateMultiple(listData, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Multiple_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            foreach (var data in listData)
            {
                data.Items = null;
            }
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.CreateMultiple(listData, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_With_Items_Order()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read(Order: "{\"Items.ProductName\" : \"desc\"}");
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int) listData.First().Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Check_Cuplicate_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.CheckDuplicate(listData.First());
            Assert.False(Response);
        }

        [Fact]
        public async Task Should_Success_Split_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new GarmentInternalPurchaseOrderFacade(dbContext);
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var data = dbContext.GarmentInternalPurchaseOrders.AsNoTracking().Include(m => m.Items).Single(m => m.Id == listData.First().Id);

            var Response = await facade.Split((int)data.Id, data, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Split_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var data = listData.First();
            data.Items = null;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Split((int)data.Id, data, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var data = listData.First();
            var Response = await facade.Delete((int)data.Id, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Delete_Data()
        {
            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Delete(0, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Validate_Data()
        {
            var viewModelNullItems = new GarmentInternalPurchaseOrderViewModel
            {
                Items = null
            };
            Assert.True(viewModelNullItems.Validate(null).Count() > 0);

            var viewModelZeroQuantity = new GarmentInternalPurchaseOrderViewModel
            {
                Items = new List<GarmentInternalPurchaseOrderItemViewModel>
                {
                    new GarmentInternalPurchaseOrderItemViewModel
                    {
                        Quantity = 0
                    }
                }
            };
            Assert.True(viewModelZeroQuantity.Validate(null).Count() > 0);

            var facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var data = listData.First();
            var item = data.Items.First();

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.
                Setup(x => x.GetService(typeof(PurchasingDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            var viewModelDuplicate = new GarmentInternalPurchaseOrderViewModel
            {
                Id = data.Id,
                Items = new List<GarmentInternalPurchaseOrderItemViewModel>
                {
                    new GarmentInternalPurchaseOrderItemViewModel
                    {
                        Id = item.Id,
                        Quantity = item.Quantity
                    }
                }
            };
            ValidationContext validationDuplicateContext = new ValidationContext(viewModelDuplicate, serviceProvider.Object, null);
            Assert.True(viewModelDuplicate.Validate(validationDuplicateContext).Count() > 0);

            var viewModelNotFoundDuplicate = new GarmentInternalPurchaseOrderViewModel
            {
                Id = 1,
                Items = new List<GarmentInternalPurchaseOrderItemViewModel>
                {
                    new GarmentInternalPurchaseOrderItemViewModel
                    {
                        Id = 0,
                        Quantity = 1
                    }
                }
            };
            ValidationContext validationNotFoundDuplicateContext = new ValidationContext(viewModelNotFoundDuplicate, serviceProvider.Object, null);
            Assert.True(viewModelNotFoundDuplicate.Validate(validationNotFoundDuplicateContext).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Tags()
        {
            GarmentInternalPurchaseOrderFacade facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            GarmentInternalPurchaseOrder data = model.FirstOrDefault();
          
            var Response = facade.ReadByTags("Accessories",$"#{data.UnitName} #{data.BuyerName}", data.ShipmentDate.AddDays(-1), data.ShipmentDate.AddDays(1), USERNAME);
            Assert.NotNull(Response);

            var ResponseWhiteSpace = facade.ReadByTags("fabric", "", DateTimeOffset.MinValue, DateTimeOffset.MinValue, USERNAME);
            Assert.NotNull(ResponseWhiteSpace);
        }

        [Fact]
        public async Task Should_Success_Create_Data_Fabric()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await EPOdataUtil(facade, GetCurrentMethod()).GetNewDataFabric();
            await facade.Create(data, USERNAME);


        }

        [Fact]
        public async Task Should_Success_Get_Report_POIPOExDuration_Data()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await EPOdataUtil(facade, GetCurrentMethod()).GetNewDataFabric();
            await facade.Create(data, USERNAME);
            GarmentInternalPurchaseOrderFacade Facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));
            var Response = Facade.GetIPOEPODurationReport("", "0-7 hari", null, null, 1, 25, "{}", 7);
            Assert.NotEqual(-1, Response.Item2);

            var Response1 = Facade.GetIPOEPODurationReport("", "8-14 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response1.Item1);

            var Response2 = Facade.GetIPOEPODurationReport("", "15-30 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response2.Item1);

            var Response3 = Facade.GetIPOEPODurationReport("", ">30 hari", null, null, 1, 25, "{}", 7);
            Assert.NotNull(Response3.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Report_POIPOEDuration_Excel()
        {
            var facade = new GarmentExternalPurchaseOrderFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var data = await EPOdataUtil(facade, GetCurrentMethod()).GetNewDataFabric();
            await facade.Create(data, USERNAME);
            GarmentInternalPurchaseOrderFacade Facade = new GarmentInternalPurchaseOrderFacade(_dbContext(GetCurrentMethod()));

            var Response = Facade.GenerateExcelIPOEPODuration("", "8-14 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response);

            var Response1 = Facade.GenerateExcelIPOEPODuration("", "0-7 hari", null, null, 7);
            Assert.IsType<System.IO.MemoryStream>(Response1);
        }


		[Fact]
		public async Task Should_Success_Get_Data_By_Name()
		{
			var dbContext = _dbContext(GetCurrentMethod());
			var facade = new GarmentInternalPurchaseOrderFacade(dbContext);
			var listData = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var data = dbContext.GarmentInternalPurchaseOrders.AsNoTracking().Include(m => m.Items).Single(m => m.Id == listData.First().Id);

			var Response = facade.ReadName(data.CreatedBy);
			Assert.NotNull(Response);
		}
	}
}
