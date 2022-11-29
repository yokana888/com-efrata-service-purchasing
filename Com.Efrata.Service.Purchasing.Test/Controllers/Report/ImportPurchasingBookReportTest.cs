using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Report
{
    public class ImportPurchasingBookReportTest
    {
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task Should_Success_GetLocalPurchasingBookReport()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReportV2(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),It.IsAny<int>())).ReturnsAsync(new LocalPurchasingBookReportViewModel());

            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReport_WithException()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReportV2(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),It.IsAny<int>())).ThrowsAsync(new Exception());

            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReportXls()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),It.IsAny<int>())).ReturnsAsync(new MemoryStream());

            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReportXls_WithException()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),It.IsAny<int>())).ThrowsAsync(new Exception());

            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetXls()
        {
            //Setup
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),It.IsAny<int>())).ReturnsAsync(new MemoryStream());

            //Act
            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.GetXls(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            //Assert
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
            Assert.Equal("Laporan Buku Pembelian Impor 01-01-0001_01-01-0001.xlsx", response.GetType().GetProperty("FileDownloadName").GetValue(response, null));
        }

        [Fact]
        public async Task Should_Throws_InternalServerError_GetXls()
        {
            //Setup
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),It.IsAny<int>())).Throws(new Exception());

            //Act
            var controller = new ImportPurchasingBookReportController(mockFacade.Object);
            var response = await controller.GetXls(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetImportPurchasingBookReport_Pdf()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReportV2(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).ReturnsAsync(new LocalPurchasingBookReportViewModel()
            {
                CategorySummaries = new List<Summary>() { new Summary { AccountingLayoutIndex = 0, Category = "test", CurrencyCode = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                CategorySummaryTotal = 1,
                CurrencySummaries = new List<Summary>() { new Summary { AccountingLayoutIndex = 0, Category = "test", CurrencyCode = "test", SubTotal = 10, SubTotalCurrency = 10 } },
                GrandTotal = 1,
                Reports = new List<PurchasingReport>() { new PurchasingReport { URNNo = "test", CategoryCode = "test", CategoryName = "test", CurrencyCode = "test" ,AccountingUnitName="test", AccountingCategoryName ="test"} }
            });

            var controller = new ImportPurchasingBookReportController(mockFacade.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {

                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            var response = await controller.GetPdf(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Failed_GetImportPurchasingBookReport_Pdf_WithException()
        {
            var mockFacade = new Mock<IImportPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReportV2(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),It.IsAny<int>())).ThrowsAsync(new Exception());

            var controller = new ImportPurchasingBookReportController(mockFacade.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {

                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            var response = await controller.GetPdf(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

    }
}
