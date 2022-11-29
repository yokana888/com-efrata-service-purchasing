using Com.Efrata.Service.Purchasing.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Helpers
{
  public  class BasicViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            BasicViewModel viewModel = new BasicViewModel()
            {
                Id = 1,
                Active = true,
                _CreatedAgent = "agen",
                _CreatedBy = "someone",
                _CreatedUtc = DateTime.Now,
                _IsDeleted = false,
                _LastModifiedAgent="agen",
                _LastModifiedBy="someone",
                _LastModifiedUtc=DateTime.Now,
            };

            Assert.Equal(1, viewModel.Id);
            Assert.True(viewModel.Active);
            Assert.Equal("agen", viewModel._CreatedAgent);
            Assert.Equal("someone", viewModel._CreatedBy);
            Assert.True(DateTime.MinValue < viewModel._CreatedUtc);
            Assert.True(DateTime.MinValue < viewModel._LastModifiedUtc);
            Assert.Equal("agen", viewModel._LastModifiedAgent);
            Assert.Equal("someone", viewModel._LastModifiedBy);
            Assert.False(viewModel._IsDeleted);
        }
    }
}
