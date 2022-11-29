using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.GarmentStockOpnameTests
{
    public class GarmentStockOpnameDownloadValidationTest
    {
        private Mock<IServiceProvider> GetServiceProviderMock()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProviderMock;
        }

        [Fact]
        public void Null_NotEmpty()
        {
            GarmentStockOpnameDownload downloadFile = new GarmentStockOpnameDownload(null, null, null, null);

            var validationResults = downloadFile.Validate(null);

            Assert.NotEmpty(validationResults.ToList());
        }

        [Fact]
        public void DateLessOrEqualThanLastData_NotEmpty()
        {
            var stockOpname = new GarmentStockOpname { Date = new DateTimeOffset(new DateTime(2020, 11, 22)) };

            var facadeMock = new Mock<IGarmentStockOpnameFacade>();
            facadeMock.Setup(s => s.GetLastDataByUnitStorage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stockOpname);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IGarmentStockOpnameFacade)))
                .Returns(facadeMock.Object);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            GarmentStockOpnameDownload downloadFile = new GarmentStockOpnameDownload(stockOpname.Date, "not null", "not null", "not null");

            var validationContext = new ValidationContext(downloadFile, serviceProviderMock.Object, null);
            var validationResults = downloadFile.Validate(validationContext);

            Assert.NotEmpty(validationResults.ToList());
        }
    }
}
