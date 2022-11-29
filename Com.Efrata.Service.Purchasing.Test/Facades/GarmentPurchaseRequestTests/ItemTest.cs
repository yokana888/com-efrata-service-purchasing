using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.PRMasterValidationReportFacade;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentCorrectionNoteDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentPurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentPurchaseRequestTests
{
    public class ItemTest
    {
        private const string ENTITY = "GarmentPurchaseRequestItem";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private PurchasingDbContext dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentPurchaseRequestDataUtil dataUtil(GarmentPurchaseRequestFacade facade, string testName)
        {
            return new GarmentPurchaseRequestDataUtil(facade);
        }

		private Mock<IServiceProvider> GetServiceProvider()
		{
			var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
		}

        [Fact]
        public async Task Should_Success_Get_Items()
        {
            var ServiceProvider = GetServiceProvider().Object;

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            GarmentPurchaseRequestItemFacade itemFacade = new GarmentPurchaseRequestItemFacade(ServiceProvider, dbContext(GetCurrentMethod()));

            var Response = itemFacade.Read(Select: "new(Id)");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Patch_Item()
        {
            var ServiceProvider = GetServiceProvider().Object;

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            GarmentPurchaseRequestItemFacade itemFacade = new GarmentPurchaseRequestItemFacade(ServiceProvider, dbContext(GetCurrentMethod()));

            JsonPatchDocument<GarmentPurchaseRequestItem> jsonPatch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            jsonPatch.Replace(m => m.IsOpenPO, false);

            var ItemIDs = model.Items.Select(i => i.Id).ToArray();
            var Response = await itemFacade.Patch($"[{string.Join(",", ItemIDs)}]", jsonPatch);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Patch_Item()
        {
            var ServiceProvider = GetServiceProvider().Object;

            GarmentPurchaseRequestFacade facade = new GarmentPurchaseRequestFacade(ServiceProvider, dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();

            GarmentPurchaseRequestItemFacade itemFacade = new GarmentPurchaseRequestItemFacade(ServiceProvider, dbContext(GetCurrentMethod()));

            JsonPatchDocument<GarmentPurchaseRequestItem> jsonPatch = new JsonPatchDocument<GarmentPurchaseRequestItem>();
            jsonPatch.Replace(m => m.Id, 0);

            var ItemIDs = model.Items.Select(i => i.Id).ToArray();
            var Response = Assert.ThrowsAnyAsync<Exception>(async () => await itemFacade.Patch($"[{string.Join(",", ItemIDs)}]", jsonPatch));
            Assert.NotNull(Response);
        }
    }
}
