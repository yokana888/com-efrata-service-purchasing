using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentPurchasingBookReport.PDF;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Reports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-purchasing-book-reports")]
    [Authorize]
    public class GarmentPurchasingBookReportController : Controller
    {
        private readonly IGarmentPurchasingBookReportService _service;
        private readonly IdentityService _identityService;
        private string ApiVersion = "1.0.0";

        public GarmentPurchasingBookReportController(IServiceProvider serviceProvider)
        {
            _service = serviceProvider.GetService<IGarmentPurchasingBookReportService>();
            _identityService = serviceProvider.GetService<IdentityService>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult GetReport([FromQuery] string billNo, [FromQuery] string paymentBill, [FromQuery] string category, [FromQuery] DateTimeOffset? startDate, [FromQuery] DateTimeOffset? endDate, [FromQuery] bool isForeignCurrency, [FromQuery] bool isImportSupplier)
        {
            try
            {
                startDate = startDate.HasValue ? startDate.GetValueOrDefault() : DateTimeOffset.MinValue;
                endDate = endDate.HasValue ? endDate.GetValueOrDefault() : DateTimeOffset.MaxValue;

                var result = _service.GetReport(billNo, paymentBill, category, startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), isForeignCurrency, isImportSupplier);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = result,
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

        [HttpGet("bill-no")]
        public IActionResult GetBillNo([FromQuery] string keyword)
        {
            try
            {
                var result = _service.GetBillNos(keyword);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = result,
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

        [HttpGet("payment-bill")]
        public IActionResult GetPaymentBill([FromQuery] string keyword)
        {
            try
            {
                var result = _service.GetPaymentBills(keyword);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = result,
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

        [HttpGet("garment-accounting-categories")]
        public IActionResult GetGarmentAccountingCategories([FromQuery] string keyword)
        {
            try
            {
                var result = _service.GetAccountingCategories(keyword);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = result,
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

        [HttpGet("downloads/xls")]
        public async Task<IActionResult> GetXls([FromQuery] string billNo, [FromQuery] string paymentBill, [FromQuery] string category, [FromQuery] DateTimeOffset? startDate, [FromQuery] DateTimeOffset? endDate, [FromQuery] bool isForeignCurrency, [FromQuery] bool isImportSupplier)
        {
            try
            {
                startDate = startDate.HasValue ? startDate.GetValueOrDefault() : DateTimeOffset.MinValue;
                endDate = endDate.HasValue ? endDate.GetValueOrDefault() : DateTimeOffset.MaxValue;

                billNo = billNo == "undefined" ? null : billNo;
                paymentBill = paymentBill == "undefined" ? null : paymentBill;
                
                var data = _service.GetReport(billNo, paymentBill, category, startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), isForeignCurrency, isImportSupplier);

                MemoryStream result = new MemoryStream();
                var filename = "Laporan Buku Pembelian Lokal";
                if (isForeignCurrency)
                {
                    filename = "Laporan Buku Pembelian Lokal Valas";
                    result = await Lib.Facades.GarmentPurchasingBookReport.Excel.GarmentPurchasingBookReportValasLocalExcel.GenerateExcel(startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), data, _identityService.TimezoneOffset);
                }
                else if (!isForeignCurrency && isImportSupplier)
                {
                    filename = "Laporan Buku Pembelian Impor";
                    result = await Lib.Facades.GarmentPurchasingBookReport.Excel.GarmentPurchasingBookReportImportExcel.GenerateExcel(startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), data, _identityService.TimezoneOffset);
                }
                else
                    result = await Lib.Facades.GarmentPurchasingBookReport.Excel.GarmentPurchasingBookReportLocalExcel.GenerateExcel(startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), data, _identityService.TimezoneOffset);

                filename += ".xlsx";


                //result = await _service.GenerateExcel(billNo, paymentBill, category, startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), isForeignCurrency, isImportSupplier,_identityService.TimezoneOffset);

                //return Ok(new
                //{
                //    apiVersion = ApiVersion,
                //    data = result,
                //    message = General.OK_MESSAGE,
                //    statusCode = General.OK_STATUS_CODE
                //});
                var bytes = result.ToArray();

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("download/pdf")]
        public IActionResult GetPDF([FromQuery] string billNo, [FromQuery] string paymentBill, [FromQuery] string category, [FromQuery] DateTimeOffset? startDate, [FromQuery] DateTimeOffset? endDate, [FromQuery] bool isForeignCurrency, [FromQuery] bool isImportSupplier)
        {
            try
            {
                VerifyUser();
                startDate = startDate.HasValue ? startDate.GetValueOrDefault() : DateTimeOffset.MinValue;
                endDate = endDate.HasValue ? endDate.GetValueOrDefault() : DateTimeOffset.MaxValue;

                var result = _service.GetReport(billNo, paymentBill, category, startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), isForeignCurrency, isImportSupplier);

                var stream = GarmentPurchasingBookReportPDFGenerator.Generate(result, startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), isForeignCurrency, isImportSupplier, _identityService.TimezoneOffset);

                var filename = "Laporan Buku Pembelian Lokal";
                if (isForeignCurrency)
                    filename = "Laporan Buku Pembelian Lokal Valas";
                else if (isImportSupplier)
                    filename = "Laporan Buku Pembelian Import";
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
    }
}
