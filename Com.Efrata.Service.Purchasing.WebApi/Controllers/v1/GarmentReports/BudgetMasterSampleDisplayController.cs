using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/display/budget-master-sample")]
    [Authorize]
    public class BudgetMasterSampleDisplayController : Controller
    {
        private readonly string ApiVersion = "1.0.0";
        private readonly IServiceProvider serviceProvider;
        private readonly IBudgetMasterSampleDisplayFacade facade;
        private readonly IdentityService identityService;

        public BudgetMasterSampleDisplayController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            facade = (IBudgetMasterSampleDisplayFacade)serviceProvider.GetService(typeof(IBudgetMasterSampleDisplayFacade)); ;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetReport(long prId, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                var data = facade.GetMonitoring(prId, Order);

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
        public IActionResult GetXls(long prId)
        {
            try
            {
                byte[] xlsInBytes;

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcel(prId);

                string filename = "Display Budget Master";

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
