using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/purchasing/unit-payment-orders-not-verified-report")]
    [Authorize]

    public class UnitPaymentOrderNotVerifiedReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly UnitPaymentOrderNotVerifiedReportFacade unitPaymentOrderNotVerifiedReportFacade;

        public UnitPaymentOrderNotVerifiedReportController(UnitPaymentOrderNotVerifiedReportFacade unitPaymentOrderNotVerifiedReportFacade)
        {
            this.unitPaymentOrderNotVerifiedReportFacade = unitPaymentOrderNotVerifiedReportFacade;
        }

        [HttpGet("history")]
        public IActionResult Get(string no, string supplier, string division, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                var data = unitPaymentOrderNotVerifiedReportFacade.GetReport(no, supplier, division, dateFrom, dateTo, page, size, Order, offset, "history");

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2, page = page, size = size }
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("history/download")]
        public IActionResult GetXls(string no, string supplier, string division, DateTime? dateFrom, DateTime? dateTo)
        {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = unitPaymentOrderNotVerifiedReportFacade.GenerateExcel(no, supplier, division, dateFrom, dateTo, offset,"history");

                string filename = String.Format("History SPB Not Verified - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            
        }

        [HttpGet]
        public IActionResult GetReport(string no, string supplier, string division, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                var data = unitPaymentOrderNotVerifiedReportFacade.GetReport(no, supplier, division, dateFrom, dateTo, page, size, Order, offset, "not-history");

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2, page = page, size = size }
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("download")]
        public IActionResult GetXlsReport(string no, string supplier, string division, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = unitPaymentOrderNotVerifiedReportFacade.GenerateExcel(no, supplier, division, dateFrom, dateTo, offset,"not-history");

                string filename = String.Format("Laporan SPB Not Verified - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
