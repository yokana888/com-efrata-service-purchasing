using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.BankExpenditureNote;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Moonlay.NetCore.Lib.Service;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.BankExpenditureNote
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/bank-expenditure-notes")]
    [Authorize]
    public class BankExpenditureNoteController : Controller
    {
        private readonly string ApiVersion = "1.0.0";
        private readonly IServiceProvider serviceProvider;
        private readonly IBankExpenditureNoteFacade facade;
        private readonly IdentityService identityService;
        private readonly IBankDocumentNumberGenerator _bankDocumentNumberGenerator;
        private readonly IMapper mapper;

        public BankExpenditureNoteController(IServiceProvider serviceProvider, IBankExpenditureNoteFacade facade, IMapper mapper)
        {
            this.serviceProvider = serviceProvider;
            this.facade = facade;
            this.mapper = mapper;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            _bankDocumentNumberGenerator = serviceProvider.GetService<IBankDocumentNumberGenerator>();
        }


        [HttpGet]
        public ActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            ReadResponse<object> Response = this.facade.Read(page, size, order, keyword, filter);

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

        [HttpGet("bank-document-no")]
        public async Task<IActionResult> GetDocumentNo([FromQuery] string type, [FromQuery] string bankCode, [FromQuery] string username)
        {
            var result = await _bankDocumentNumberGenerator.GenerateDocumentNumber(type, bankCode, username);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = result,
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("bank-document-no-date")]
        public async Task<IActionResult> GetDocumentNoDate([FromQuery] string type, [FromQuery] string bankCode, [FromQuery] string username, [FromQuery] DateTime date)
        {
            var result = await _bankDocumentNumberGenerator.GenerateDocumentNumber(type, bankCode, username, date);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = result,
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
                var model = await facade.ReadById(Id);
                BankExpenditureNoteViewModel viewModel = mapper.Map<BankExpenditureNoteViewModel>(model);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }

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

                    //BankExpenditureNotePDFTemplate PdfTemplate = new BankExpenditureNotePDFTemplate();
                    //MemoryStream stream = PdfTemplate.GeneratePdfTemplate(model, clientTimeZoneOffset);
                    MemoryStream stream = facade.GeneratePdfTemplate(model, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"Bank Expenditure Note {model.DocumentNo}.pdf"
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
        public async Task<IActionResult> Post([FromBody] BankExpenditureNoteViewModel viewModel)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            //ValidateService validateService = (ValidateService)facade.serviceProvider.GetService(typeof(ValidateService));
            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(viewModel);

                BankExpenditureNoteModel model = mapper.Map<BankExpenditureNoteModel>(viewModel);

                int result = await facade.Create(model, identityService);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

            try
            {
                int Result = await facade.Delete(id, identityService);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] BankExpenditureNoteViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(vm);

                BankExpenditureNoteModel m = mapper.Map<BankExpenditureNoteModel>(vm);

                int result = await facade.Update(id, m, identityService);

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

        [HttpGet("no-select/by-position")]
        public ActionResult GetAllCashierPosition(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            size = int.MaxValue;
            ReadResponse<object> Response = facade.GetAllByPosition(page, size, order, keyword, filter);

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

        [HttpGet("get-documentno/by-period")]
        public ActionResult GetBGCheckAndDocumentNoByPeriod([FromQuery] int month = 0, [FromQuery] int year = 0, [FromQuery] int timeoffset = 0)
        {
            var result = facade.GetByPeriod(month, year, timeoffset);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = result,
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpPut("posting")]
        public async Task<IActionResult> Posting([FromBody] List<long> ids)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                var result = await facade.Posting(ids);

                return Ok(new
                {
                    apiVersion = "1.0.0",
                    data = result,
                    message = General.OK_MESSAGE,
                    statusCode = General.CREATED_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter("1.0.0", General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
            
        }

        [HttpGet("reports/list")]
        public ActionResult GetReport(string DocumentNo, string UnitPaymentOrderNo, string InvoiceNo, string SupplierCode, string DivisionCode, string PaymentMethod, DateTimeOffset? DateFrom, DateTimeOffset? DateTo, int Size = 25, int Page = 1)
        {
            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
            ReadResponse<object> response = this.facade.GetReport(Size, Page, DocumentNo, UnitPaymentOrderNo, InvoiceNo, SupplierCode, DivisionCode, PaymentMethod, DateFrom, DateTo, clientTimeZoneOffset);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data = response.Data,
                info = new Dictionary<string, object>
                {
                    { "count", response.Data.Count },
                    { "total", response.TotalData },
                    { "order", response.Order },
                    { "page", Page },
                    { "size", Size }
                },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }
    }
}
