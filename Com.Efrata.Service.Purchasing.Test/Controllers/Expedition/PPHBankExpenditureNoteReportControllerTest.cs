using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.Expedition
{
    public class PPHBankExpenditureNoteReportControllerTest
    {
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_Report()
        {
            Mock<IPPHBankExpenditureNoteReportFacade> mockFacade = new Mock<IPPHBankExpenditureNoteReportFacade>();
            mockFacade.Setup(p => p.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>()))
                .Returns(new ReadResponse<object>(new List<object>(), 0, new Dictionary<string, string>()));

            PPHBankExpenditureNoteReportController controller = new PPHBankExpenditureNoteReportController(mockFacade.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";

            var response = controller.Get(1, 1, "", "", "", "", null, null);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
    }
}