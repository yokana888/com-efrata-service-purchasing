using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using System.IO;
using Microsoft.AspNetCore.Authorization;


namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.PurchaseRequestControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/purchase-requests/by-user")]
    [Authorize]

    public class PurchaseRequestByUserController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly PurchaseRequestFacade facade;
        private readonly IdentityService identityService;

        public PurchaseRequestByUserController(IMapper mapper, PurchaseRequestFacade facade, IdentityService identityService)
        {
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                var Data = facade.Read(page, size, order, keyword, filter);

                var newData = mapper.Map<List<PurchaseRequestViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.no,
                        s.date,
                        s.expectedDeliveryDate,
                        unit = new
                        {
                            division = new { s.unit.division.name },
                            s.unit.name
                        },
                        category = new { s.category.name },
                        s.isPosted,
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    },
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                PurchaseRequest model = facade.ReadById(id);
                PurchaseRequestViewModel viewModel = mapper.Map<PurchaseRequestViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                if (indexAcceptPdf < 0)
                {
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
                    });
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    PurchaseRequestPDFTemplate PdfTemplate = new PurchaseRequestPDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.no}.pdf"
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
        public async Task<IActionResult> Post([FromBody]PurchaseRequestViewModel vm)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            PurchaseRequest m = mapper.Map<PurchaseRequest>(vm);

            ValidateService validateService = (ValidateService)facade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(vm);

                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                int result = await facade.Create(m, identityService.Username, clientTimeZoneOffset);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]PurchaseRequestViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            PurchaseRequest m = mapper.Map<PurchaseRequest>(vm);

            ValidateService validateService = (ValidateService)facade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(vm);

                int result = await facade.Update(id, m, identityService.Username);

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

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                facade.Delete(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }
    }
}
