﻿using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.ReportTest
{
  //  [Collection("ServiceProviderFixture Collection")]
    public class UnitPaymentOrderUnpaidReportTests
    {
        //private IServiceProvider ServiceProvider { get; set; }

        //private UnitPaymentOrderUnpaidReportDataUtil DataUtil
        //{
        //    get { return (UnitPaymentOrderUnpaidReportDataUtil)ServiceProvider.GetService(typeof(UnitPaymentOrderUnpaidReportDataUtil)); }
        //}

        //private UnitPaymentOrderUnpaidReportFacade Facade
        //{
        //    get { return (UnitPaymentOrderUnpaidReportFacade)ServiceProvider.GetService(typeof(UnitPaymentOrderUnpaidReportFacade)); }
        //}

        //public UnitPaymentOrderUnpaidReportTests(ServiceProviderFixture fixture)
        //{
        //    ServiceProvider = fixture.ServiceProvider;
        //    this.DataUtil.CleanOldData();
        //}

        //[Fact]
        //public void Should_Success_Get_SQL_Data()
        //{
        //    var result = this.Facade.GetPurchasingDocumentExpedition(25, 1, null, null, DateTimeOffset.Now.AddMonths(-1), DateTimeOffset.Now);
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public async Task Should_Success_Get_Mongo_Data()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReportMongo( "", "", date.AddDays(-15), date.AddDays(15));
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data()
        //{
        //    var data = DataUtil.GetTestData();
        //    var result = await this.Facade.GetReport(25, 1, "{}", GetBsonValue.ToString(data.Item1, "no"), GetBsonValue.ToString(data.Item1, "supplier.code"), null, null,7);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Mongo_Data_Parm()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReportMongo(GetBsonValue.ToString(data.Item1, "no"), GetBsonValue.ToString(data.Item1, "supplier.code"), date.AddDays(-15), date.AddDays(15));
        //    Assert.NotEmpty(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Parm()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReport(25, 1, "{}", "", "", date.AddDays(-15), date.AddDays(15), 0);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Parm_Order()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReport(25, 1, "{\"UnitPaymentOrderNo\":\"asc\"}", "", "", date.AddDays(-15), date.AddDays(15), 0);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Parm_Order_From()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReport(25, 1, "{\"UnitPaymentOrderNo\":\"asc\"}", GetBsonValue.ToString(data.Item1, "no"), GetBsonValue.ToString(data.Item1, "supplier.code"), date.AddDays(-15), null, 0);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Parm_Order_To()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReport(25, 1, "{\"UnitPaymentOrderNo\":\"asc\"}", GetBsonValue.ToString(data.Item1, "no"), GetBsonValue.ToString(data.Item1, "supplier.code"), null,date.AddDays(15), 0);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}

        //[Fact]
        //public async Task Should_Success_Get_Report_Data_Parm_All()
        //{
        //    var data = DataUtil.GetTestData();
        //    var date = data.Item1["dueDate"].ToUniversalTime();
        //    var result = await this.Facade.GetReport(25, 1, "{\"UnitPaymentOrderNo\":\"asc\"}", GetBsonValue.ToString(data.Item1, "no"), GetBsonValue.ToString(data.Item1, "supplier.code"), date.AddDays(-15), date.AddDays(15), 0);
        //    Assert.NotNull(result);
        //    this.Facade.DeleteDataMongoUPO("{ _id : ObjectId('" + data.Item1["_id"].AsObjectId.ToString() + "') }");
        //    this.Facade.DeleteDataMongoURN("{ _id : ObjectId('" + data.Item2["_id"].AsObjectId.ToString() + "') }");
        //}
    }
}
