using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentClosingDateFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentClosingDateDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentClosingDateTests
{
    public class BasicTests
    {
        private const string ENTITY = "GarmentClosingDate";

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

        private GarmentClosingDateDataUtil dataUtil(GarmentClosingDateFacade facade, string testName)
        {
            return new GarmentClosingDateDataUtil(facade);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            GarmentClosingDateFacade facade = new GarmentClosingDateFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
            var model = dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            GarmentClosingDateFacade facade = new GarmentClosingDateFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
            var dataUtil = this.dataUtil(facade, GetCurrentMethod());
            var model = await dataUtil.GetTestData();

            GarmentClosingDate newModel = dataUtil.CopyData(model);

            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Create(newModel, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            GarmentClosingDateFacade facade = new GarmentClosingDateFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }
        //[Fact]
        //public async Task Should_Success_Update_Data()
        //{
        //    GarmentClosingDateFacade facade = new GarmentClosingDateFacade(_dbContext(GetCurrentMethod()), ServiceProvider);
        //    var dataUtil = this.dataUtil(facade, GetCurrentMethod());
        //    var model = await dataUtil.GetTestData();

        //    GarmentClosingDate newModel = dataUtil.CopyData(model);

        //    var Response = await facade.Create(newModel, USERNAME);
        //    Assert.NotEqual(0, Response);
        //}
    }
}
