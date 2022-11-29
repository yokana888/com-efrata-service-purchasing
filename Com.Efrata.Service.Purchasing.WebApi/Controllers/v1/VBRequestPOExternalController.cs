using Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/vb-request-po-external")]
    [Authorize]
    public class VBRequestPOExternalController : Controller
    {
        private readonly IVBRequestPOExternalService _service;
        private readonly IdentityService _identityService;
        private const string ApiVersion = "1.0";

        public VBRequestPOExternalController(IVBRequestPOExternalService service, IServiceProvider serviceProvider)
        {
            _service = service;
            _identityService = serviceProvider.GetService<IdentityService>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string keyword, [FromQuery] string division, [FromQuery] string currencyCode)
        {

            try
            {
                var result = _service.ReadPOExternal(keyword, division, currencyCode);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result,
                    info = new Dictionary<string, object>
                {
                    { "page", 1 },
                    { "size", 10 }
                },
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("spb")]
        public IActionResult GetSPB([FromQuery] string keyword, [FromQuery] string division, [FromQuery] string epoIds, [FromQuery] string currencyCode, [FromQuery]string typePurchasing)
        {

            try
            {
                var epoIdList = JsonConvert.DeserializeObject<List<long>>(epoIds);
                var result = _service.ReadSPB(keyword, division, epoIdList, currencyCode, typePurchasing);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result,
                    info = new Dictionary<string, object>
                {
                    { "page", 1 },
                    { "size", 10 }
                },
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpPut("spb/{id}")]
        public IActionResult UpdateVBCreatedFlag([FromQuery] string division, [FromRoute] int id)
        {

            try
            {
                _identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = _service.UpdateSPB(division, id);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result,
                    info = new Dictionary<string, object>
                {
                    { "page", 1 },
                    { "size", 10 }
                },
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpPost("auto-journal-epo")]
        public async Task<IActionResult> AutoJournalEPO([FromBody] VBFormDto form)
        {

            try
            {
                VerifyUser();

                var result = await _service.AutoJournalVBRequest(form);
                
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }
    }
}
