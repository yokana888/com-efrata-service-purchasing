using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
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
using General = Com.Efrata.Service.Purchasing.WebApi.Helpers.General;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/account-stock-report")]
    [Authorize]
    public class AccountingStockReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IAccountingStockReportFacade _facade;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public AccountingStockReportController(IAccountingStockReportFacade facade, IServiceProvider serviceProvider)
        {
            this._facade = facade;
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetReportAccountStock(DateTime? dateFrom, DateTime? dateTo, string category, string unitcode,string planPO, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = _facade.GetStockReport(offset, unitcode, planPO, category, page, size, Order, dateFrom, dateTo);



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
        public IActionResult GetXls(DateTime? dateFrom, DateTime? dateTo, string category, string categoryname, string unitcode, string unitname, string planPO)
        {

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                MemoryStream xls = _facade.GenerateExcelAStockReport(category, categoryname, planPO, unitcode ,unitname, DateFrom, DateTo, offset);


                //string filename = String.IsNullOrWhiteSpace(unitcode) ? String.Format("Laporan Stock Pembukuan All Unit - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : unitcode == "C2A" ? String.Format("Laporan Stock Pembukuan KONFEKSI 2A - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : unitcode == "C2B" ? String.Format("Laporan Stock Pembukuan KONFEKSI 2B - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : unitcode == "C2C" ? String.Format("Laporan Stock Pembukuan KONFEKSI 2C - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : unitcode == "C1B" ? String.Format("Laporan Stock Pembukuan KONFEKSI 2D - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : String.Format("Laporan Stock Pembukuan All KONFEKSI 1 MNS - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));
                string filename = string.Format("Laporan Stock Pembukuan - {0} {1} - {2}.xlsx", unitname, ((DateTime)dateFrom).ToString("dd-MM-yyyy"), ((DateTime)dateTo).ToString("dd-MM-yyyy"));
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
