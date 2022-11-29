using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Moonlay.NetCore.Lib.Service;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Microsoft.EntityFrameworkCore;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using System.IO;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/expedition/pph-bank-expenditure-notes")]
    [Authorize]
    public class PPHBankExpenditureNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IPPHBankExpenditureNoteFacade PPHBankExpenditureNoteFacade;
        private readonly IdentityService identityService;

        public PPHBankExpenditureNoteController(IServiceProvider serviceProvider, IPPHBankExpenditureNoteFacade PPHBankExpenditureNoteFacade)
        {
            this.serviceProvider = serviceProvider;
            this.PPHBankExpenditureNoteFacade = PPHBankExpenditureNoteFacade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet("loader/unit-payment-orders")]
        public ActionResult GetUPO(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string incomeTaxName, double incomeTaxRate, string currency, string divisionCodes)
        {
            List<object> Data = this.PPHBankExpenditureNoteFacade.GetUnitPaymentOrder(dateFrom, dateTo, incomeTaxName, incomeTaxRate, currency, divisionCodes);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data = Data,
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet]
        public ActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            ReadResponse<object> Response = this.PPHBankExpenditureNoteFacade.Read(page, size, order, keyword, filter);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Response.Data,
                info = new Dictionary<string, object>
                {
                    { "count", Response.Data.Count },
                    { "total", Response.TotalData },
                    { "order", Response.Order },
                    { "page", page },
                    { "size", size }
                },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                var model = await PPHBankExpenditureNoteFacade.ReadById(Id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }

                var viewModel = new PPHBankExpenditureNoteViewModel(model);

                if (indexAcceptPdf < 0)
                {
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        data = viewModel,
                        message = General.OK_MESSAGE,
                        statusCode = General.OK_STATUS_CODE
                    });
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    PPHBankExpenditureNotePDFTemplate PdfTemplate = new PPHBankExpenditureNotePDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(model, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"PPH Bank Expenditure Note {viewModel.No}pdf"
                    };
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PPHBankExpenditureNoteViewModel viewModel)
        {
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(viewModel);

                PPHBankExpenditureNote model = viewModel.ToModel();

                await PPHBankExpenditureNoteFacade.Create(model, this.identityService.Username);

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
                var innerException = e.InnerException != null ? e.InnerException.Message : "";
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, $"- {e.Message}\n- {innerException}")
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Put([FromRoute] int Id, [FromBody] PPHBankExpenditureNoteViewModel viewModel)
        {
            try
            {
                this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));
                validateService.Validate(viewModel);

                PPHBankExpenditureNote model = viewModel.ToModel();

                if (Id != model.Id)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                        .Fail();
                    return BadRequest(Result);
                }

                await PPHBankExpenditureNoteFacade.Update(Id, model, identityService.Username);

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
            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

            try
            {
                int Result = await PPHBankExpenditureNoteFacade.Delete(id, identityService.Username);

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

        [HttpPut("posting")]
        public async Task<IActionResult> Posting([FromBody] List<long> ids)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                var result = await PPHBankExpenditureNoteFacade.Posting(ids);

                return NoContent();
            }
            catch (Exception e)
            {
                var innerException = e.InnerException != null ? e.InnerException.Message : "";
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, $"- {e.Message}\n- {innerException}")
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}