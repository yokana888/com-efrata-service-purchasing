using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
    public class BankDocumentNumberGeneratorTest
    {
        private const string ENTITY = "BankDocumentNumberGenerator";

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
        public async Task Should_Success_Get_Data()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            BankDocumentNumberGenerator generator = new BankDocumentNumberGenerator(dbContext);
            string result = await generator.GenerateDocumentNumber("test", "test", "test");
            var bankDocument = dbContext.BankDocumentNumbers.FirstOrDefault();
            bankDocument.LastModifiedUtc = bankDocument.LastModifiedUtc.AddMonths(-1);
            dbContext.Update(bankDocument);
            dbContext.SaveChanges();
            string result2 = await generator.GenerateDocumentNumber("test", "test", "test");
            string result3 = await generator.GenerateDocumentNumber("test", "test", "test");
            Assert.NotNull(result3);
        }
    }
}
