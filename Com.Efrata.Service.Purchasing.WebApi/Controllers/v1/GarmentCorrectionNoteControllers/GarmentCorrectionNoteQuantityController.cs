using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentCorrectionNotePDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentCorrectionNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-correction-quantity-notes")]
    [Authorize]
    public class GarmentCorrectionNoteQuantityController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentCorrectionNoteQuantityFacade facade;
        private readonly IdentityService identityService;

        public GarmentCorrectionNoteQuantityController(IServiceProvider serviceProvider, IMapper mapper, IGarmentCorrectionNoteQuantityFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<GarmentCorrectionNoteViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable()
                        .Select(s => new
                        {
                            s.Id,
                            s.CorrectionNo,
                            s.CorrectionDate,
                            s.CorrectionType,
                            s.Supplier,
                            s.DONo,
                            s.UseIncomeTax,
                            s.UseVat,
                            s.CreatedBy,
                            s.LastModifiedUtc,
                            s.Items
                        })
                        .ToList()
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
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var model = facade.ReadById(id);
                var viewModel = mapper.Map<GarmentCorrectionNoteViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                if (indexAcceptPdf < 0)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(viewModel);
                    return Ok(Result);
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    var stream = GarmentCorrectionNoteQuantityPDFTemplate.Generate(model, serviceProvider, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{model.CorrectionNo}.pdf"
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
        public async Task<IActionResult> Post([FromBody]GarmentCorrectionNoteViewModel viewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));
                validateService.Validate(viewModel);

                var Model = mapper.Map<GarmentCorrectionNote>(viewModel);

                await facade.Create(Model,viewModel.Supplier.Import, identityService.Username);

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

        [HttpGet("return-note-ppn/{id}")]
        public IActionResult GetReturnNotePpn(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var model = facade.ReadById(id);
                var viewModel = mapper.Map<GarmentCorrectionNoteViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                else if (!viewModel.UseVat)
                {
                    throw new Exception("Data is not UseVat");
                }

                if (indexAcceptPdf < 0)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(viewModel);
                    return Ok(Result);
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    var stream = GarmentCorrectionNoteQuantityPpnPDFTemplate.Generate(model, serviceProvider, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{model.NKPN}-ppn.pdf"
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

        [HttpGet("return-note-pph/{id}")]
        public IActionResult GetReturnNotePph(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var model = facade.ReadById(id);
                var viewModel = mapper.Map<GarmentCorrectionNoteViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                else if (!viewModel.UseIncomeTax)
                {
                    throw new Exception("Data is not UseIncomeTax");
                }

                if (indexAcceptPdf < 0)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(viewModel);
                    return Ok(Result);
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    var stream = GarmentCorrectionNoteQuantityPphPDFTemplate.Generate(model, serviceProvider, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{model.NKPH}-pph.pdf"
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
    }
}
