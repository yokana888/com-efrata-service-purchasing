using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.PurchasingDispositionControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/purchasing-dispositions")]
    [Authorize]
    public class PurchasingDispositionController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IPurchasingDispositionFacade facade;
        private readonly IdentityService identityService;

        public PurchasingDispositionController(IServiceProvider serviceProvider, IMapper mapper, IPurchasingDispositionFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet("by-user")]
        public IActionResult GetByUser(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                return Get(page, size, order, keyword, filter);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadOptimized(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<PurchasingDispositionViewModel>>(Data.Item1);
                var newData = facade.GetTotalPaidPrice(viewModel);
                //List<object> listData = new List<object>();
                //listData.AddRange(
                //    newData.AsQueryable().Select(s => new
                //    {
                //        s.DispositionNo,
                //        s.Id,
                //        s.Supplier,
                //        s.Bank,
                //        s.ConfirmationOrderNo,
                //        //s.InvoiceNo,
                //        s.PaymentMethod,
                //        s.CreatedBy,
                //        s.Calculation,
                //        //s.Investation,
                //        s.Remark,
                //        s.ProformaNo,
                //        s.Amount,
                //        s.Currency,
                //        s.LastModifiedUtc,
                //        s.CreatedUtc,
                //        s.PaymentDueDate,
                //        s.Position,
                //        s.Items,
                //        s.Category,
                //        s.Division,
                //        s.DPP,
                //        s.IncomeTaxValue,
                //        s.IncomeTaxBy,
                //        s.VatValue,
                //        s.PaymentCorrection
                //    }).ToList()
                //);

                var listData = newData.Select(s => new
                {
                    s.DispositionNo,
                    s.Id,
                    s.Supplier,
                    s.Bank,
                    s.ConfirmationOrderNo,
                    //s.InvoiceNo,
                    s.PaymentMethod,
                    s.CreatedBy,
                    s.Calculation,
                    //s.Investation,
                    s.Remark,
                    s.ProformaNo,
                    s.Amount,
                    s.Currency,
                    s.LastModifiedUtc,
                    s.CreatedUtc,
                    s.PaymentDueDate,
                    s.Position,
                    s.Items,
                    s.Category,
                    s.Division,
                    s.DPP,
                    s.IncomeTaxValue,
                    s.IncomeTaxBy,
                    s.VatValue,
                    s.PaymentCorrection
                }).ToList();

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                var result = facade.ReadModelById(id);
                PurchasingDispositionViewModel viewModel = mapper.Map<PurchasingDispositionViewModel>(result);
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

                    PurchasingDispositionPDFTemplate PdfTemplate = new PurchasingDispositionPDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{result.DispositionNo}.pdf"
                    };
                }

                //Dictionary<string, object> Result =
                //    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                //    .Ok(viewModel);
                //return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("pdf/{id}")]
        public IActionResult GetPDF(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                PurchasingDisposition model = facade.ReadModelById(id);

                PurchasingDispositionViewModel viewModel = mapper.Map<PurchasingDispositionViewModel>(model);

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

                    PurchasingDispositionPDFTemplate PdfTemplate = new PurchasingDispositionPDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{model.DispositionNo}.pdf"
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
        public async Task<IActionResult> Post([FromBody] PurchasingDispositionViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<PurchasingDisposition>(ViewModel);

                await facade.Create(model, identityService.Username);

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
        public async Task<IActionResult> Put(int id, [FromBody] PurchasingDispositionViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<PurchasingDisposition>(ViewModel);

                await facade.Update(id, model, identityService.Username);

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
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;


                facade.Delete(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }


        [HttpGet("disposition")]
        public IActionResult Getdisposition(string keyword = null, string filter = "{}", string epoId = "")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = facade.ReadDisposition(keyword, filter, epoId);

            var viewModel = mapper.Map<List<PurchasingDispositionViewModel>>(Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                viewModel.AsQueryable().Select(s => new
                {
                    s.DispositionNo,
                    s.Id,
                    s.Supplier,
                    s.Bank,
                    s.ConfirmationOrderNo,
                    s.IncomeTaxBy,
                    s.PaymentMethod,
                    s.CreatedBy,
                    s.Currency,
                    s.LastModifiedUtc,
                    s.CreatedUtc,
                    s.PaymentDueDate,
                    s.Items
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

        [HttpGet("by-disposition")]
        public IActionResult GetByDisposition(string Keyword = "", string Filter = "{}")
        {
            var Data = facade.ReadByDisposition(Keyword, Filter);
            var newData = mapper.Map<List<PurchasingDispositionViewModel>>(Data);
            Dictionary<string, object> Result =
                   new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                   .Ok(newData);
            return Ok(Result);
        }

        [HttpPut("update/position")]
        public async Task<IActionResult> UpdatePosition([FromBody] PurchasingDispositionUpdatePositionPostedViewModel data)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(data);

                await facade.UpdatePosition(data, identityService.Username);
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

        [HttpPut("update/is-paid-true/{dispositionNo}")]
        public async Task<IActionResult> SetIsPaidTrue([FromRoute] string dispositionNo)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.SetIsPaidTrue(dispositionNo, identityService.Username);
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

        [HttpGet("memo-loader/{dispositionId}")]
        public IActionResult GetMemoLoader(int dispositionId)
        {
            try
            {
                var result = facade.GetDispositionMemoLoader(dispositionId);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result,
                });

                //Dictionary<string, object> Result =
                //    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                //    .Ok(viewModel);
                //return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("memo-spb-loader")]
        public IActionResult GetMemoSPBLoader(string keyword, int divisionId, bool supplierIsImport, string currencyCode)
        {
            try
            {
                var result = facade.GetUnitPaymentOrderMemoLoader(keyword, divisionId, supplierIsImport, currencyCode);
                var info = new Dictionary<string, object>
                    {
                        { "count", result.Data.Count },
                        { "total", result.TotalData },
                        { "order", result.Order},
                        { "page", 1 },
                        { "size", 10 }
                    };

                var response = new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result.Data, info);
                return Ok(response);

                //Dictionary<string, object> Result =
                //    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                //    .Ok(viewModel);
                //return Ok(Result);
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
