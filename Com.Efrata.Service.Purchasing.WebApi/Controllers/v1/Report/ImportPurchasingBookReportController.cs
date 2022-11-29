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
    [Route("v{version:apiVersion}/report/import-purchasing-book-reports")]
    [Authorize]
    public class ImportPurchasingBookReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IImportPurchasingBookReportFacade importPurchasingBookReportFacade;

        public ImportPurchasingBookReportController(IImportPurchasingBookReportFacade importPurchasingBookReportFacade)
        {
            this.importPurchasingBookReportFacade = importPurchasingBookReportFacade;
        }

        //public async Task<IActionResult> Get(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo)
        [HttpGet]
        public async Task<IActionResult> Get(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            try
            {
                //var data = await importPurchasingBookReportFacade.GetReport(no, unit, category, dateFrom, dateTo);
                var data = await importPurchasingBookReportFacade.GetReportV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
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

        //public async Task<IActionResult> GetPdf(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo)
        [HttpGet("pdf")]
        public async Task<IActionResult> GetPdf(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            try
            {
                var clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                var data = await importPurchasingBookReportFacade.GetReportV2(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
                //var data = await importPurchasingBookReportFacade.GetReport(no, unit, category, dateFrom, dateTo);
                //var data = importPurchasingBookReportService.GetReport();

                var stream = ImportPurchasingBookReportPdfTemplate.Generate(data, clientTimeZoneOffset, dateFrom, dateTo);

                var filename = "Laporan Buku Pembelian Import.pdf";

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

        //public async Task<IActionResult> GetXls(string no, string unit, string category, DateTime? dateFrom, DateTime? dateTo)
        [HttpGet("download")]
        public async Task<IActionResult> GetXls(string no, int accountingUnitId, int accountingCategoryId, DateTime? dateFrom, DateTime? dateTo, int divisionId)
        {
            try
            {
                byte[] xlsInBytes;

                var xls = await importPurchasingBookReportFacade.GenerateExcel(no, accountingUnitId, accountingCategoryId, dateFrom, dateTo, divisionId);
                //var xls = await importpurchasingbookreportfacade.generateexcel(no, unit, category, datefrom, dateto);

                string filename = "Laporan Buku Pembelian Impor";

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
