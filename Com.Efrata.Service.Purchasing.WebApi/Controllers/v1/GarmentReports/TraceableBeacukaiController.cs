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
    [Route("v{version:apiVersion}/traceable")]
    [Authorize]
    public class TraceableBeacukaiController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ITraceableBeacukaiFacade _facade;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public TraceableBeacukaiController(ITraceableBeacukaiFacade facade, IServiceProvider serviceProvider)
        {
            this._facade = facade;
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet("in")]
        public IActionResult GetReport(string bcno, string type, string tipebc)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = _facade.GetReportTraceableIN(bcno, type, tipebc);

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

        [HttpGet("in/download")]
        public IActionResult GetInExcel(string bcno, string type, string tipebc)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var xls = _facade.GetTraceableInExcel(bcno, type, tipebc);

                string filename = String.Format("Laporan Traceable Masuk - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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


        [HttpGet("out")]
        public IActionResult GettraceOut(string bcno)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];



            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                var data2 = _facade.getQueryTraceableOut(bcno);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data2
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

        [HttpGet("out/download")]
        public IActionResult GetXlsOutTraceable(string bcno)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

            string accept = Request.Headers["Accept"];

            
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                //DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GetExceltraceOut(bcno);

                string filename = String.Format("Laporan Traceable Keluar - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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

        [HttpGet]
        public IActionResult Get(string bum)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var model = _facade.Read(bum);

                //var info = new Dictionary<string, object>
                //    {
                //        { "count", model.Data.Count },
                //        { "total", model.TotalData },
                //        { "order", model.Order },
                //        { "page", page },
                //        { "size", size }
                //    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(model);
                return Ok(Result);
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
        public IActionResult GetXls(string bum)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

            string accept = Request.Headers["Accept"];


            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                //DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GetExceltracebyBUM(bum);

                string filename = String.Format("Laporan Fitur Penggunaan BUM - {0}.xlsx", bum);

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

        //[HttpGet("out/detail")]
        //public IActionResult GettraceOutDetail(string ro)
        //{
        //    int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        //    string accept = Request.Headers["Accept"];



        //    try
        //    {
        //        identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
        //        identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
        //        identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

        //        var data2 = _facade.getQueryDetail(ro);
        //        return Ok(new
        //        {
        //            apiVersion = ApiVersion,
        //            data = data2,

        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}
    }
}
