using Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.VBRequestPOExternal
{
    public class VBFormDtoTest
    {
        [Fact]
        public void Should_Success_Instantiate_First()
        {
            CurrencyViewModel currency = new CurrencyViewModel();
            currency.Code = "";
            currency.Description = "";
            currency.Id = 1;
            currency.Rate = 1;
            currency.Symbol = "";

            AccountBankViewModel accountBank = new AccountBankViewModel();
            accountBank.AccountCOA = "";
            accountBank.AccountName = "";
            accountBank.AccountNumber = "";
            accountBank.BankCode = "";
            accountBank.BankName = "";
            accountBank.Code = "";
            accountBank.Currency = currency;
            accountBank.Id = 1;

            List<long> number = new List<long>();
            number.Add(1);

            List<UPOAndAmountDto> amountDtos = new List<UPOAndAmountDto>();
            UPOAndAmountDto amountDto = new UPOAndAmountDto();
            amountDto.Amount = 1;
            amountDto.UPOId = 1;
            amountDtos.Add(amountDto);

            VBFormDto vBForm = new VBFormDto();
            vBForm.Amount = 1;
            vBForm.Bank = accountBank;
            vBForm.Date = DateTimeOffset.Now;
            vBForm.DocumentNo = "";
            vBForm.EPOIds = number;
            vBForm.UPOIds = amountDtos;

            Assert.NotNull(vBForm);
        }
    }
}
