using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.ExternalPurchaseOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/external-purchase-orders")]
    [Authorize]
    public class ExternalPurchaseOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly ExternalPurchaseOrderFacade _facade;
        private readonly IdentityService identityService;

        public ExternalPurchaseOrderController(IMapper mapper, ExternalPurchaseOrderFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.Read(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<ExternalPurchaseOrderViewModel>>(Data.Item1);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.orderDate,
                    s.supplier,
                    //unit = new
                    //{
                    //    division = new { 
                    //        s.unit.division.name,
                    //        s.unit.division._id,
                    //        s.unit.division.code
                    //    },
                    //    s.unit.name,
                    //    s.unit._id,
                    //    s.unit.code
                    //},
                    s.unit,
                    s.isPosted,
                    s.items,
                    s.IsCreateOnVBRequest,
                    CurrencyCode = s.currency.code,
                    CurrencyRate = s.currency.rate,
                    s.currency,
                    s.useVat,
                    s.useIncomeTax,
                    s.incomeTax,
                    s.incomeTaxBy
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

        [HttpGet("unused")]
        public IActionResult GetUnused(string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.ReadUnused(keyword, filter);

            var newData = _mapper.Map<List<ExternalPurchaseOrderViewModel>>(Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.orderDate,
                    s.supplier,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name,
                        s.unit._id,
                        s.unit.code
                    },
                    s.isPosted,
                    s.items
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
                },
            });
        }

        [HttpGet("disposition")]
        public IActionResult Getdisposition(string keyword = null, string currencyId = "", string supplierId = "", string categoryId = "", string divisionId = "", string incomeTaxBy="")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.ReadDisposition(keyword, currencyId, supplierId, categoryId, divisionId, incomeTaxBy);

            //var newData = _mapper.Map<List<ExternalPurchaseOrderViewModel>>(Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                Data.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.orderDate,
                    s.supplier,
                    s.incomeTax,
                    s.useIncomeTax,
                    s.useVat,
                    s.unit,
                    s.isPosted,
                    s.items,
                    s.vatTax
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
                },
            });
        }

        

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                ExternalPurchaseOrder model = _facade.ReadModelById(id);
                ExternalPurchaseOrderViewModel viewModel = _mapper.Map<ExternalPurchaseOrderViewModel>(model);
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
                    ExternalPurchaseOrderPDFTemplate PdfTemplate = new ExternalPurchaseOrderPDFTemplate();
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
        public async Task<IActionResult> Post([FromBody]ExternalPurchaseOrderViewModel vm)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            ExternalPurchaseOrder m = _mapper.Map<ExternalPurchaseOrder>(vm);

            ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(vm);

                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                int result = await _facade.Create(m, identityService.Username, clientTimeZoneOffset);

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
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]ExternalPurchaseOrderViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            ExternalPurchaseOrder m = _mapper.Map<ExternalPurchaseOrder>(vm);

            ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));

            try
            {
                validateService.Validate(vm);

                int result = await _facade.Update(id, m, identityService.Username);

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
                _facade.Delete(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPost("post")]
        public IActionResult EPOPost([FromBody]List<ExternalPurchaseOrderViewModel> ListExternalPurchaseOrderViewModel)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {
                _facade.EPOPost(
                    ListExternalPurchaseOrderViewModel.Select(vm => _mapper.Map<ExternalPurchaseOrder>(vm)).ToList(), identityService.Username
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("update-from-vb-with-po-req-finance/{PONo}")]
        public IActionResult UpdateFromSalesReceiptAsync([FromRoute] string PONo, [FromBody] POExternalUpdateModel model)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                _facade.HideUnpost(PONo, identityService.Username, model);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("unpost/{id}")]
        public IActionResult EPOUnpost([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                _facade.EPOUnpost(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("cancel/{id}")]
        public IActionResult EPOCancel([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                _facade.EPOCancel(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("close/{id}")]
        public IActionResult EPOClose([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                _facade.EPOClose(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }
    }
}
