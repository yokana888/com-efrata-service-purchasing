using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/reports/purchase-monitoring")]
    [Authorize]
    public class PurchaseMonitoringController : Controller
    {
        private readonly IPurchaseMonitoringService _service;
        private readonly string _apiVersion = "1.0";

        public PurchaseMonitoringController(IPurchaseMonitoringService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get(string unitId, string categoryId, string divisionId, string budgetId, string createdBy, string status, DateTime? startDate, DateTime? endDate, DateTime? startDatePO, DateTime? endDatePO, string supplierId, long prId = 0, long poExtId = 0, [FromQuery] int page = 1, [FromQuery] int size = 25)
        {
            try
            {
                startDate = startDate.GetValueOrDefault().Date;
                endDate = endDate.HasValue ? endDate.Value.Date.AddDays(1).AddTicks(-1) : DateTime.Now.Date.AddDays(1).AddTicks(-1);

                //startDatePO = startDatePO.GetValueOrDefault().Date;
                //endDatePO = endDatePO.HasValue ? endDatePO.Value.Date.AddDays(1).AddTicks(-1) : DateTime.Now.Date.AddDays(1).AddTicks(-1);

                var timezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                startDate = startDate.Value == DateTime.MinValue ? startDate.GetValueOrDefault() : startDate.Value.AddHours(timezoneOffset * -1).Date;
                endDate = endDate.Value.AddHours(timezoneOffset * -1).Date;

                //startDatePO = startDatePO.Value == DateTime.MinValue ? startDatePO.GetValueOrDefault() : startDatePO.Value.AddHours(timezoneOffset * 1).Date;
                //endDatePO = endDatePO.Value.AddHours(timezoneOffset * -1).Date;

                var result = await _service.GetReport(unitId, categoryId, divisionId, budgetId, prId, createdBy, status, startDate.Value.ToUniversalTime(), endDate.Value.ToUniversalTime(), startDatePO, endDatePO, poExtId, supplierId, page, size);

                //var data = importPurchasingBookReportService.GetReport();

                return Ok(new
                {
                    apiVersion = _apiVersion,
                    data = result.Data,
                    info = new { total = result.Total },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_apiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("all/download")]
        public async Task<IActionResult> GetXls(string unitId, string categoryId, string divisionId, string budgetId, string createdBy, string status, DateTime? startDate, DateTime? endDate, DateTime? startDatePO, DateTime? endDatePO, string supplierId, long prId = 0, long poExtId = 0)
        {
            try
            {
                startDate = startDate.GetValueOrDefault().Date;
                endDate = endDate.HasValue ? endDate.Value.Date.AddDays(1).AddTicks(-1) : DateTime.Now.Date.AddDays(1).AddTicks(-1);


                var timezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                startDate = startDate.Value == DateTime.MinValue ? startDate.GetValueOrDefault() : startDate.Value.AddHours(timezoneOffset * -1).Date;
                endDate = endDate.Value.AddHours(timezoneOffset * -1).Date;

                //startDatePO = startDatePO.Value == DateTime.MinValue ? startDatePO.GetValueOrDefault() : startDatePO.Value.AddHours(timezoneOffset * -1).Date;
                //endDatePO = endDatePO.Value.AddHours(timezoneOffset * -1).Date;

                byte[] xlsInBytes;

                var xls = await _service.GenerateExcel(unitId, categoryId, divisionId, budgetId, prId, createdBy, status, startDate.Value.ToUniversalTime(), endDate.Value.ToUniversalTime(), startDatePO, endDatePO, poExtId, supplierId, timezoneOffset);

                string filename = $"Laporan Purchase All {startDate.Value.ToString("dd-MM-yyyy")}_{endDate.Value.ToString("dd-MM-yyyy")}";
                filename += ".xlsx";

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_apiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
