using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BankDocumentNumber;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers
{
    public class BankDocumentNumberGenerator : IBankDocumentNumberGenerator
    {
        private readonly DbSet<BankDocumentNumber> dbSet;
        private readonly PurchasingDbContext dbContext;
        private readonly string USER_AGENT = "document-number-generator";

        public BankDocumentNumberGenerator(PurchasingDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<BankDocumentNumber>();
        }

        public async Task<string> GenerateDocumentNumber(string Type, string BankCode, string Username)
        {
            var result = "";
            var lastData = await dbSet.Where(w => w.BankCode.Equals(BankCode) && w.Type.Equals(Type)).OrderByDescending(o => o.LastModifiedUtc).FirstOrDefaultAsync();

            DateTime Now = DateTime.Now;

            if (lastData == null)
            {

                result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}0001";
                var bankDocumentNumber = new BankDocumentNumber()
                {
                    BankCode = BankCode,
                    Type = Type,
                    LastDocumentNumber = 1
                };
                EntityExtension.FlagForCreate(bankDocumentNumber, Username, USER_AGENT);

                dbContext.BankDocumentNumbers.Add(bankDocumentNumber);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                if (lastData.LastModifiedUtc.Month != Now.Month)
                {
                    result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}0001";

                    lastData.LastDocumentNumber = 1;
                }
                else
                {
                    lastData.LastDocumentNumber += 1;
                    result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}{lastData.LastDocumentNumber.ToString().PadLeft(4, '0')}";
                }
                EntityExtension.FlagForUpdate(lastData, Username, USER_AGENT);
                dbContext.BankDocumentNumbers.Update(lastData);
                //dbContext.Entry(lastData).Property(x => x.LastDocumentNumber).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedAgent).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedBy).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedUtc).IsModified = true;

                await dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<string> GenerateDocumentNumber(string Type, string BankCode, string Username, DateTime Date)
        {
            var result = "";
            var lastData = await dbSet.Where(w => w.BankCode.Equals(BankCode) && w.Type.Equals(Type) && (w.Month == Date.Month && w.Year == Date.Year)).OrderByDescending(o => o.LastModifiedUtc).FirstOrDefaultAsync();

            DateTime Now = Date;

            if (lastData == null)
            {

                result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}0001";
                var bankDocumentNumber = new BankDocumentNumber()
                {
                    BankCode = BankCode,
                    Type = Type,
                    LastDocumentNumber = 1,
                    Month = Date.Month,
                    Year = Date.Year
                };
                EntityExtension.FlagForCreate(bankDocumentNumber, Username, USER_AGENT);

                dbContext.BankDocumentNumbers.Add(bankDocumentNumber);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                //if (lastData.LastModifiedUtc.Month != Now.Month)
                //{
                //    result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}0001";

                //    lastData.LastDocumentNumber = 1;
                //}
                //else
                //{
                lastData.LastDocumentNumber += 1;
                result = $"{Now.ToString("yy")}{Now.ToString("MM")}{BankCode}{Type}{lastData.LastDocumentNumber.ToString().PadLeft(4, '0')}";
                //}
                EntityExtension.FlagForUpdate(lastData, Username, USER_AGENT);
                lastData.LastModifiedUtc = Date;
                dbContext.BankDocumentNumbers.Update(lastData);
                //dbContext.Entry(lastData).Property(x => x.LastDocumentNumber).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedAgent).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedBy).IsModified = true;
                //dbContext.Entry(lastData).Property(x => x.LastModifiedUtc).IsModified = true;

                await dbContext.SaveChangesAsync();
            }

            return result;
        }
    }
}
