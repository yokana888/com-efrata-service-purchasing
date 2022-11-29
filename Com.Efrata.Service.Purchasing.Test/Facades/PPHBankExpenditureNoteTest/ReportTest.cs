using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExpeditionDataUtil;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PPHBankExpenditureNoteDataUtil;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PPHBankExpenditureNoteTest
{
    public class ReportTest
    {
        private const string ENTITY = "PPHBankExpenditureNoteReport";

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

        [Fact]
        public void Should_Success_Get_Data()
        {
            PPHBankExpenditureNoteReportFacade facade = new PPHBankExpenditureNoteReportFacade(_dbContext(GetCurrentMethod()));
            ReadResponse<object> response = facade.GetReport(1, 25, null, null, null, null, null, null, 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Data_With_Params()
        {
            PPHBankExpenditureNoteReportFacade facade = new PPHBankExpenditureNoteReportFacade(_dbContext(GetCurrentMethod()));
            ReadResponse<object> response = facade.GetReport(1, 25, "", "", "", "", null, null, 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Data_With_Date()
        {
            PPHBankExpenditureNoteReportFacade facade = new PPHBankExpenditureNoteReportFacade(_dbContext(GetCurrentMethod()));
            ReadResponse<object> response = facade.GetReport(1, 25, null, null, null, null, new DateTimeOffset(), new DateTimeOffset(), 0);

            Assert.NotNull(response);
        }

        [Fact]
        public void Should_Success_Get_Data_With_Date_And_Params()
        {
            PPHBankExpenditureNoteReportFacade facade = new PPHBankExpenditureNoteReportFacade(_dbContext(GetCurrentMethod()));
            ReadResponse<object> response = facade.GetReport(1, 25, "", "", "", "", new DateTimeOffset(), new DateTimeOffset(), 0);

            Assert.NotNull(response);
        }
    }
}
