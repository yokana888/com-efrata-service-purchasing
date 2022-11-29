using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentDispositionControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-disposition-purchase")]
    [Authorize]
    public class GarmentDispositionController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IGarmentDispositionPurchaseFacade facade;
        private readonly IGarmentExternalPurchaseOrderFacade EPOfacade;
        private readonly IdentityService identityService;
        private readonly IMapper mapper;

        public GarmentDispositionController( IServiceProvider serviceProvider, IGarmentDispositionPurchaseFacade facade, IdentityService identityService, IMapper mapper)
        {
            this.serviceProvider = serviceProvider;
            //this.facade = serviceProvider.GetService<IGarmentDispositionPurchaseFacade>();
            this.facade = facade;
            //this.identityService = serviceProvider.GetService<IdentityService>();
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.mapper = mapper;
        }

        private void VerifyUser()
        {
            identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                VerifyUser();
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = await facade.GetAll(keyword,page, size,filter,order);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
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

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = await facade.GetAll(keyword, page, size, filter, order);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
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

        [HttpGet("all/{id}")]
        public async Task<IActionResult> GetPdfAll([FromRoute]int id, [FromQuery] bool isVerifiedAmountCalculated = false)
        {
            try
            {
                VerifyUser();
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var Data = await facade.GetFormById(id, isVerifiedAmountCalculated);
                if (indexAcceptPdf < 0)
                {
                    //return Ok(new
                    //{
                    //    apiVersion = ApiVersion,
                    //    statusCode = General.OK_STATUS_CODE,
                    //    message = General.OK_MESSAGE,
                    //    data = Data,
                    //});
                    Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
                    return Ok(Result);
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    DispotitionPurchasingAllPDFTemplate PdfTemplate = new DispotitionPurchasingAllPDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(Data, clientTimeZoneOffset, identityService.Username);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{Data.DispositionNo}.pdf"
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

        [HttpGet("loader")]
        public IActionResult GetLoader(PurchasingGarmentExpeditionPosition position = PurchasingGarmentExpeditionPosition.Invalid,int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}", int supplierId=0)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(position, page, size, order, keyword, filter, supplierId);

                //var viewModel = mapper.Map<List<PurchasingDispositionViewModel>>(Data.Item1);
                //var newData = facade.GetTotalPaidPrice(viewModel);
                List<object> listData = new List<object>();
                listData.AddRange(
                    Data.Item1.Select(s => new
                    {
                        s.DispositionNo,
                        s.Id,
                        s.SupplierName,
                        s.Bank,
                        s.ConfirmationOrderNo,
                        //s.InvoiceNo,
                        s.PaymentType,
                        //s.CreatedBy,
                        //s.Bank,
                        //s.Investation,
                        s.Remark,
                        s.ProformaNo,
                        s.Amount,
                        s.CurrencyCode,
                        //s.lastt,
                        s.CreatedUtc,
                        s.PaymentDueDate,
                        s.Position,
                        s.Items,
                        s.Category,
                        //s.Division,
                        s.DPP,
                        s.IncomeTaxValue,
                        //s.income,
                        s.VatValue,
                        s.MiscAmount
                    }).ToList()
                );

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
        public async Task<IActionResult> GetById([FromRoute]int id,[FromQuery] bool isVerifiedAmountCalculated =false)
        {
            try
            {
                VerifyUser();
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var Data = await facade.GetFormById(id, isVerifiedAmountCalculated);
                if (indexAcceptPdf < 0)
                {
                    //return Ok(new
                    //{
                    //    apiVersion = ApiVersion,
                    //    statusCode = General.OK_STATUS_CODE,
                    //    message = General.OK_MESSAGE,
                    //    data = Data,
                    //});
                    Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
                    return Ok(Result);
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    GarmentPurchasingPDFTemplate PdfTemplate = new GarmentPurchasingPDFTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(Data, clientTimeZoneOffset,identityService.Username);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{Data.DispositionNo}.pdf"
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

        [HttpGet("by-disposition-no")]
        public IActionResult GetByDisposition(int page = 1, int size = 25, string dispositionNo = null)
        {
            try
            {
                VerifyUser();
                var Data = facade.ReadByDispositionNo(dispositionNo, page, size);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormDto model)
        {
            try
            {
                VerifyUser();
                var Data = await facade.Post(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(new { Message = "Data Saved" });
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id,[FromBody] FormEditDto model)
        {
            try
            {
                VerifyUser();
                var Data = await facade.Update(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(new { Message = "Data Saved" });
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                VerifyUser();
                var Data = await facade.Delete(id);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(new { Message = "Data Saved"});
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

        [HttpGet("po-external-id/{id}")]
        public IActionResult Get([FromRoute] int id,[FromQuery]int supplierId, [FromQuery]string currencyCode)
        {
            try
            {
                VerifyUser();
                var viewModel = facade.ReadByEPOWithDisposition(id,supplierId, currencyCode);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
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

        [HttpGet("all-garment-disposition")]
        public IActionResult GetAll()
        {
            try
            {
                VerifyUser();
                var viewModel = facade.GetGarmentDispositionPurchase();
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = viewModel,
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
    }
}
