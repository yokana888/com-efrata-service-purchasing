using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/report/local-purchasing-book-reports")]
    [Authorize]
    public class LocalPurchasingBookReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly ILocalPurchasingBookReportFacade localPurchasingBookReportFacade;

        public LocalPurchasingBookReportController(ILocalPurchasingBookReportFacade localPurchasingBookReportFacade)
        {
            this.localPurchasingBookReportFacade = localPurchasingBookReportFacade;
        }

        //public async Task<IActionResult> Get(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo, bool isValas)
        [HttpGet]
        public async Task<IActionResult> Get(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, bool isValas, int divisionId)
        {
            try
            {
                //var data = await localPurchasingBookReportFacade.GetReport(no, unit, category, dateFrom, dateTo, isValas);
                var data = await localPurchasingBookReportFacade.GetReportV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, isValas, divisionId);
                //var data = importPurchasingBookReportService.GetReport();

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

        //public async Task<IActionResult> GetPdf(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo, bool isValas)
        [HttpGet("pdf")]
        public async Task<IActionResult> GetPdf(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, bool isValas, int divisionId)
        {
            try
            {
                var clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                //var data = await localPurchasingBookReportFacade.GetReport(no, unit, category, dateFrom, dateTo, isValas);
                var data = await localPurchasingBookReportFacade.GetReportV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, isValas, divisionId);

                var stream = isValas ? LocalPurchasingForeignCurrencyBookReportPdfTemplate.Generate(data, clientTimeZoneOffset, dateFrom, dateTo) : LocalPurchasingBookReportPdfTemplate.Generate(data, clientTimeZoneOffset, dateFrom, dateTo);

                var filename = isValas ? "Laporan Buku Pembelian Lokal Valas" : "Laporan Buku Pembelian Lokal";
                //if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
                //if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".pdf";

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception e)
            {
                var result = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message).Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, result);
            }
        }

        //public async Task<IActionResult> GetXls(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo, bool isValas)
        [HttpGet("download")]
        public async Task<IActionResult> GetXls(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, bool isValas, int divisionId)
        {
            try
            {
                byte[] xlsInBytes;

                //var xls = await localPurchasingBookReportFacade.GenerateExcel(no, unit, category, dateFrom, dateTo, isValas);
                var xls = await localPurchasingBookReportFacade.GenerateExcel(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, isValas, divisionId);

                string filename = isValas ? "Laporan Buku Pembelian Lokal Valas" : "Laporan Buku Pembelian Lokal";
                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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