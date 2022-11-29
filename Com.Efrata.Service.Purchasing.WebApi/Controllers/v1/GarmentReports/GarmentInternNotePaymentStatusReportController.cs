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
    [Route("v{version:apiVersion}/intern-note-payment-status")]
    [Authorize]
    public class GarmentInternNotePaymentStatusReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IGarmenInternNotePaymentStatusFacade _facade;
        private readonly IServiceProvider serviceProvider;
        private IdentityService IdentityService;

        public GarmentInternNotePaymentStatusReportController(IdentityService identityService, IGarmenInternNotePaymentStatusFacade facade, IServiceProvider serviceProvider)
        {
            this._facade = facade;
            this.serviceProvider = serviceProvider;
            IdentityService = identityService;
        }

        protected void VerifyUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            IdentityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult GetReport(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {
                VerifyUser();

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = _facade.GetReport(inno, invono, dono, billno, paymentbill, npn, nph, corrno, supplier, dateNIFrom, dateNITo, dueDateFrom, dueDateTo, status, page , size, Order,offset);

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
        public IActionResult GetXlsPayment(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {
                VerifyUser();
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = _facade.GetXLs(inno, invono, dono, billno, paymentbill, npn, nph, corrno, supplier, dateNIFrom, dateNITo, dueDateFrom, dueDateTo, status, offset);

                string filename = status == "BB" ? String.Format("Laporan Status Bayar Nota Intern Belum Bayar - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : status == "SB" ? String.Format("Laporan Status Bayar Nota Intern Sudah Bayar - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy")) : String.Format("Laporan Status Bayar Nota Intern All - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
