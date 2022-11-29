using Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.ViewModels.VBRequestPOExternalViewModel
{
    public class VBFormDtoTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            List<long> epoids = new List<long>();
            epoids.Add(1);

            List<UPOAndAmountDto> uPOAndAmounts = new List<UPOAndAmountDto>();
            UPOAndAmountDto uPOAndAmount = new UPOAndAmountDto();
            uPOAndAmount.Amount = 1;
            uPOAndAmount.UPOId = 1;
            uPOAndAmounts.Add(uPOAndAmount);

            CurrencyViewModel currency = new CurrencyViewModel();
            currency.Id = 1;
            currency.Code = "";
            currency.Symbol = "";
            currency.Rate = 1;
            currency.Description = "";

            AccountBankViewModel accountBank = new AccountBankViewModel();
            accountBank.Id = 1;
            accountBank.Code = "";
            accountBank.BankCode = "";
            accountBank.AccountName = "";
            accountBank.AccountNumber = "";
            accountBank.BankName = "";
            accountBank.AccountCOA = "";
            accountBank.Currency = currency;

            VBFormDto viewModel = new VBFormDto();
            viewModel.Date = DateTimeOffset.Now;
            viewModel.DocumentNo = "";
            viewModel.EPOIds = epoids;
            viewModel.Amount = 1;
            viewModel.UPOIds = uPOAndAmounts;
            viewModel.Bank = accountBank;

            Assert.NotNull(viewModel);
        }
    }
}
