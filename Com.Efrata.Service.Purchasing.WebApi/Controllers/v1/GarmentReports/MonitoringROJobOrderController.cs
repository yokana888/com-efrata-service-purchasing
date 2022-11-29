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

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/monitoring/ro-job-order")]
    [Authorize]
    public class MonitoringROJobOrderController : Controller
    {
        private readonly string ApiVersion = "1.0.0";
        private readonly IServiceProvider serviceProvider;
        private readonly IMonitoringROJobOrderFacade facade;
        private readonly IdentityService identityService;

        public MonitoringROJobOrderController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            facade = (IMonitoringROJobOrderFacade)serviceProvider.GetService(typeof(IMonitoringROJobOrderFacade)); ;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public async Task<IActionResult> GetMonitoring(long CostCalculationId)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                if (Request.Headers["accept"] == "application/xls")
                {
                    Tuple<MemoryStream, string> xls = await facade.GetExcel(CostCalculationId);
                    byte[] xlsInBytes = xls.Item1.ToArray();
                    var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{xls.Item2}.xlsx");
                    return file;
                }

                var result = await facade.GetMonitoring(CostCalculationId);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);
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
    }
}