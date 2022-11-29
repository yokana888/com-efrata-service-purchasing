using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using System.Collections.Generic;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using System;
using Com.Moonlay.NetCore.Lib.Service;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-payment-order-verification")]
    [Authorize]
    public class UnitPaymentOrderVerificationController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade;
        private readonly IdentityService identityService;

        public UnitPaymentOrderVerificationController(PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade, IdentityService identityService)
        {
            this.purchasingDocumentExpeditionFacade = purchasingDocumentExpeditionFacade;
            this.identityService = identityService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UnitPaymentOrderVerificationViewModel viewModel)
        {
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            ValidateService validateService = (ValidateService)purchasingDocumentExpeditionFacade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(viewModel);

                PurchasingDocumentExpedition model = viewModel.ToModel();

                await purchasingDocumentExpeditionFacade.UnitPaymentOrderVerification(model, this.identityService.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
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
    }
}

