using AutoMapper;

using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;



namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReceiptCorrectionControllers
{

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-receipt-correction-reports")]
    [Authorize]
    public class GarmentReceiptCorrectionReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IGarmentReceiptCorrectionReportFacade facade;
        private readonly IdentityService identityService;
        public readonly IServiceProvider serviceProvider;

        public GarmentReceiptCorrectionReportController(IServiceProvider serviceProvider, IGarmentReceiptCorrectionReportFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));
        }
        [HttpGet]
        public IActionResult GetReport([FromQuery] string unit, [FromQuery] string category, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo, [FromQuery] int size = 25, [FromQuery] int page = 1, [FromQuery] string Order = "{}")
        {
            if (dateTo == null)
                dateTo = DateTimeOffset.UtcNow;

            if (dateFrom == null)
                dateFrom = DateTimeOffset.MinValue;
            // int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = facade.GetReport(unit, category, dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), Order, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
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

        [HttpGet("download")]
        public IActionResult GetXls([FromQuery] string unit, [FromQuery] string category, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo, [FromQuery] int size = 25, [FromQuery] int page = 1, [FromQuery] string Order = "{}")
        {

            if (dateTo == null)
                dateTo = DateTimeOffset.UtcNow;

            if (dateFrom == null)
                dateFrom = DateTimeOffset.MinValue;

            try
            {
                byte[] xlsInBytes;

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcel(unit, category, dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), Order);

                string filename = "Laporan Koreksi Penerimaan";
               

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

        //[HttpGet]
        //public async Task<IActionResult> GetReport([FromQuery] string unit, [FromQuery] string category, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo, [FromQuery] int size=  25, [FromQuery] int page = 1, [FromQuery] string Order = "{'Date': 'desc'}")
        //{
        //    if (dateTo == null)
        //        dateTo = DateTimeOffset.UtcNow;

        //    if (dateFrom == null)
        //        dateFrom = DateTimeOffset.MinValue;

        //    try
        //    {
        //        if (Request.Headers["accept"] == "application/xls")
        //        {
        //            MemoryStream xls = facade.GenerateExcel(unit, category, dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), Order);
        //            byte[] xlsInBytes = xls.ToArray();
        //            var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Laporan Koreksi Penerimaan.xlsx");
        //            return file;
        //        }

        //        var result =await  facade.GetReport(unit, category,  dateFrom.GetValueOrDefault(), dateTo.GetValueOrDefault(), Order, page, size);

        //        return Ok(new
        //        {
        //            apiVersion = ApiVersion,
        //            data = result.Data,
        //            info = new { total = result.Total, page, size }
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, result);
        //    }
        //}
    }

    


}
