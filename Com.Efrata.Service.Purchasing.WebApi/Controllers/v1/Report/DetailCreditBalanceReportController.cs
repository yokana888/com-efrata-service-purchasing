using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
//using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using System.Collections.Generic;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/report/detail-credit-balance")]
    [Authorize]
    public class DetailCreditBalanceReportController : Controller
    {
        private readonly IDetailCreditBalanceReportFacade _service;
        private readonly IdentityService _identityService;
        private string ApiVersion = "1.0.0";

        public DetailCreditBalanceReportController(IServiceProvider serviceProvider)
        {
            _service = serviceProvider.GetService<IDetailCreditBalanceReportFacade>();
            _identityService = serviceProvider.GetService<IdentityService>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            try
            {
                VerifyUser();

                if (!dateTo.HasValue)
                    dateTo = DateTimeOffset.MaxValue;

                var data = await _service.GetReport(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);
                
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    info = new { total = data.Reports.Count },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE

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

        [HttpGet("pdf")]
        public async Task<IActionResult> GetPdf(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            try
            {
                if (!dateTo.HasValue)
                    dateTo = DateTimeOffset.MaxValue;

                var clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                var data = await _service.GetReport(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);

                var stream = DetailCreditBalanceReportPdfTemplate.Generate(data, clientTimeZoneOffset, dateTo, isImport, isForeignCurrency, accountingUnitId, divisionId);

                var filename = isImport ? "Laporan Detail Saldo Hutang Usaha Impor" : isForeignCurrency ? "Laporan Detail Saldo Hutang Usaha Lokal Valas" : "Laporan Detail Saldo Hutang Usaha Lokal";
                filename += ".pdf";

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("xls")]
        public async Task<IActionResult> GetXls(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset? dateTo, bool isImport, bool isForeignCurrency)
        {
            try
            {
                if (!dateTo.HasValue)
                    dateTo = DateTimeOffset.MaxValue;

                byte[] xlsInBytes;
                var xls = await _service.GenerateExcel(categoryId, accountingUnitId, divisionId, dateTo, isImport, isForeignCurrency);

                var sTitle = isImport ? "Impor" : isForeignCurrency ? "Lokal Valas" : "Lokal";
                string filename = $"Laporan Saldo Hutang Usaha (Detail) {sTitle}";
                filename += ".xlsx";

                //if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
                //if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");

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
