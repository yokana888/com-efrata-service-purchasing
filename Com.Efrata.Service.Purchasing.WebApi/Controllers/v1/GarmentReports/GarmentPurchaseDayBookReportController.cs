using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-purchase-book-reports")]
    [Authorize]
    public class GarmentPurchaseDayBookReportController : Controller
    {

        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IGarmentPurchaseDayBookReport facade;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public GarmentPurchaseDayBookReportController(IServiceProvider serviceProvider, IGarmentPurchaseDayBookReport facade)
        {
            this.serviceProvider = serviceProvider;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));
        }

        [HttpGet]
        public IActionResult GetReport(string unit, bool jnsSpl, string payMtd, string category, int year, int size = 25, int page = 1, string Order = "{}")
        {
            if (year == 0)
                year = ((DateTimeOffset.UtcNow).Year)-1;

            //if (dateFrom == null)
            //    dateFrom = DateTimeOffset.MinValue;
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.GetReport(unit, jnsSpl, payMtd, category, year, offset, Order, page, size);

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
        public IActionResult GetXls(string unit, bool jnsSpl, string payMtd, string category, int year, int size = 25, int page = 1, string Order = "{}")
        {
            if (year == 0)
                year = ((DateTimeOffset.UtcNow).Year) - 1;
            //if (dateTo == null)
            //    dateTo = DateTimeOffset.UtcNow;

            //if (dateFrom == null)
            //    dateFrom = DateTimeOffset.MinValue;

            try
            {
                byte[] xlsInBytes;

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcel(unit, jnsSpl, payMtd, category, year, offset);

                string filename =String.Format("Laporan Buku Harian Pembelian - " + year  );


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
