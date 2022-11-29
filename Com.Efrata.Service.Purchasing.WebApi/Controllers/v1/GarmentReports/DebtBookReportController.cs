using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/debt-book-report")]
    [Authorize]
    public class DebtBookReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IDebtBookReportFacade _facade;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        //private readonly string ContentType = "application/vnd.openxmlformats";
        //private readonly string FileName = string.Concat("Error Log - ", typeof(GarmentSupplierBalanceDebt).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");

        public DebtBookReportController(IDebtBookReportFacade facade, IMapper mapper, IServiceProvider serviceProvider)
        {
            this._facade = facade;
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService)); ;
        }
        [HttpGet]
        public IActionResult GetReport(int month, int year, bool? suppliertype, string suppliername)
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = _facade.GetDebtBookReport(month, year, suppliertype, suppliername);

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
        public IActionResult GetXls(int month, int year, bool? suppliertype, string suppliername)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);


                MemoryStream xls = _facade.GenerateExcelDebtReport(month, year, suppliertype, suppliername);


                string filename = String.Format("Detail Rekap Saldo Hutang - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
