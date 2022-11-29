using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.InternalPurchaseOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/purchase-orders/monitoring")]
    [Authorize]
    public class PurchaseOrderMonitoringAllController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly PurchaseOrderMonitoringAllFacade _facade;
        private readonly IdentityService identityService;
        public PurchaseOrderMonitoringAllController(IMapper mapper, PurchaseOrderMonitoringAllFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult GetReportAll(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = _facade.GetReport(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo,dateFromPO,dateToPO, status,  page, size, Order, offset, "");

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
        public IActionResult GetXlsAll(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcel(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo, dateFromPO, dateToPO, status, offset, "");

                string filename = String.Format("Monitoring Purchase Order All - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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


        [HttpGet("by-user")]
        public IActionResult GetReportByUser(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {

                var data = _facade.GetReport(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo, dateFromPO, dateToPO, status, page, size, Order, offset, identityService.Username);

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

        [HttpGet("by-user/download")]
        public IActionResult GetXlsByUser(string prNo, string supplierId, string divisionCode, string unitId, string categoryId, string budgetId, string epoNo, string staff, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromPO, DateTime? dateToPO, string status)
        {

            //try
            //{
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcel(prNo, supplierId, divisionCode, unitId, categoryId, budgetId, epoNo, staff, dateFrom, dateTo, dateFromPO, dateToPO, status, offset, identityService.Username);

                string filename = String.Format("Monitoring Purchase Order All - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            //}
            //catch (Exception e)
            //{
            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
            //        .Fail();
            //    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            //}
        }

        #region Sarmut Staff
        [HttpGet("staffs")]
        public IActionResult GetReportStaff(DateTime? dateFrom, DateTime? dateTo, string divisi)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            var data = _facade.GetReportStaff(dateFrom, dateTo, divisi, offset);
            return Ok(new
            {
                apiVersion = ApiVersion,
                data = data.Item1,
                info = new { total = data.Item2 },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("substaffs")]
        public IActionResult GetReportsubStaff(DateTime? dateFrom, DateTime? dateTo, string divisi, string staff)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            var data = _facade.GetReportsubStaff(dateFrom, dateTo, divisi, staff, offset);
            return Ok(new
            {
                apiVersion = ApiVersion,
                data = data.Item1,
                info = new { total = data.Item2 },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }


        [HttpGet("substaffs/download")]
        public IActionResult GetXlsSarmut(DateTime? dateFrom, DateTime? dateTo, string divisi, string staff)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcelSarmut(dateFrom, dateTo, divisi, staff, offset);

                string filename = String.Format("Staff - {0}.xlsx", staff);

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
        #endregion
    }
}
