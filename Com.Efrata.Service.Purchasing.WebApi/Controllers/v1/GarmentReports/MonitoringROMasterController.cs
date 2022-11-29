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
    [Route("v{version:apiVersion}/monitoring/ro-master")]
    [Authorize]
    public class MonitoringROMasterController : Controller
    {
        private readonly string ApiVersion = "1.0.0";
        private readonly IServiceProvider serviceProvider;
        private readonly IMonitoringROMasterFacade facade;
        private readonly IdentityService identityService;

        public MonitoringROMasterController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            facade = (IMonitoringROMasterFacade)serviceProvider.GetService(typeof(IMonitoringROMasterFacade)); ;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetMonitoring(long prId)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                if (Request.Headers["accept"] == "application/xls")
                {
                    Tuple<MemoryStream, string> xls = facade.GetExcel(prId);
                    byte[] xlsInBytes = xls.Item1.ToArray();
                    var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{xls.Item2}.xlsx");
                    return file;
                }

                var result = facade.GetMonitoring(prId);

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
