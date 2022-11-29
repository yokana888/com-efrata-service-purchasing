using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-payment-orders-expedition-report")]
    [Authorize]
    public class UnitPaymentOrderExpeditionReportController : Controller
    {
        private const string _apiVersion = "1.0";
        private readonly IUnitPaymentOrderExpeditionReportService _service;

        public UnitPaymentOrderExpeditionReportController(IUnitPaymentOrderExpeditionReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] string no, [FromQuery] string supplierCode, [FromQuery] string divisionCode, [FromQuery] int status, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo, [FromQuery] string order = "{'Date': 'desc'}", [FromQuery] int page = 1, [FromQuery] int size = 25)
        {
            if (dateTo == null)
                dateTo = DateTimeOffset.UtcNow;

            if (dateFrom == null)
                dateFrom = DateTimeOffset.MinValue;

            try
            {
                if (Request.Headers["accept"] == "application/xls")
                {
                    MemoryStream xls = await _service.GetExcel(no, supplierCode, divisionCode, status, dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), order);
                    byte[] xlsInBytes = xls.ToArray();
                    var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Laporan Ekspedisi Surat Perintah Bayar {no ?? supplierCode ?? divisionCode ?? ""}.xlsx");
                    return file;
                }

                var result = await _service.GetReport(no, supplierCode, divisionCode, status, dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), order, page, size);

                return Ok(new
                {
                    apiVersion = _apiVersion,
                    data = result.Data,
                    info = new { total = result.Total, page, size }
                });
            }
            catch(Exception e)
            {
                Dictionary<string, object> result =
                    new ResultFormatter(_apiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, result);
            }
        }
    }
}
