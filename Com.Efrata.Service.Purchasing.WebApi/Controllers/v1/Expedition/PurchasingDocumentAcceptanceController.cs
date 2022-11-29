using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/expedition/purchasing-document-acceptance")]
    [Authorize]
    public class PurchasingDocumentAcceptanceController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade;
        private readonly IdentityService identityService;

        public PurchasingDocumentAcceptanceController(PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade, IdentityService identityService)
        {
            this.purchasingDocumentExpeditionFacade = purchasingDocumentExpeditionFacade;
            this.identityService = identityService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PurchasingDocumentAcceptanceViewModel viewModel)
        {
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            ValidateService validateService = (ValidateService)purchasingDocumentExpeditionFacade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(viewModel);

                await purchasingDocumentExpeditionFacade.PurchasingDocumentAcceptance(viewModel, this.identityService.Username);

                return NoContent();
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

            try
            {
                int Result = await purchasingDocumentExpeditionFacade.DeletePurchasingDocumentAcceptance(id);

                if (Result.Equals(0))
                {
                    Dictionary<string, object> ResultNotFound =
                       new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                       .Fail();
                    return NotFound(ResultNotFound);
                }

                return NoContent();
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