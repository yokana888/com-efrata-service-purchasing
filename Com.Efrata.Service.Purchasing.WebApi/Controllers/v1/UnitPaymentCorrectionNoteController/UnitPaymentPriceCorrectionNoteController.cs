using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentCorrectionNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitPaymentCorrectionNoteController
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-payment-correction-notes/price-correction")]
    [Authorize]
    public class UnitPaymentPriceCorrectionNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper _mapper;
        private readonly IUnitPaymentPriceCorrectionNoteFacade _facade;
        private readonly IUnitPaymentOrderFacade _spbFacade;
        private readonly IdentityService identityService;
        public UnitPaymentPriceCorrectionNoteController(IServiceProvider serviceProvider, IMapper mapper, IUnitPaymentPriceCorrectionNoteFacade facade, IUnitPaymentOrderFacade spbFacade)
        {
            this.serviceProvider = serviceProvider;
            _mapper = mapper;
            _facade = facade;
            _spbFacade = spbFacade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = _facade.Read(page, size, order, keyword, filter);

                var newData = _mapper.Map<List<UnitPaymentCorrectionNoteViewModel>>(Data.Item1);
                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.uPCNo,
                        s.correctionDate,
                        s.uPONo,
                        s.supplier,
                        s.invoiceCorrectionNo,
                        s.dueDate,
                        s.LastModifiedUtc,
                        s.useIncomeTax,
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UnitPaymentCorrectionNoteViewModel vm)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            UnitPaymentCorrectionNote m = _mapper.Map<UnitPaymentCorrectionNote>(vm);
            //ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));
            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(vm);
                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                int result = await _facade.Create(m, vm.supplier.import, identityService.Username, clientTimeZoneOffset);

                if (result.Equals(0) || vm == null)
                {
                    return StatusCode(500);
                }
                else
                {
                    return Created(Request.Path, result);
                }
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                UnitPaymentCorrectionNote model = _facade.ReadById(id);
                UnitPaymentCorrectionNoteViewModel viewModel = _mapper.Map<UnitPaymentCorrectionNoteViewModel>(model);
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

        [HttpGet("pdf/{id}")]
        public IActionResult GetPDF(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                UnitPaymentCorrectionNote model = _facade.ReadById(id);

                UnitPaymentCorrectionNoteViewModel viewModel = _mapper.Map<UnitPaymentCorrectionNoteViewModel>(model);

                //if (indexAcceptPdf < 0)
                //{
                //    return Ok(new
                //    {
                //        apiVersion = ApiVersion,
                //        statusCode = General.OK_STATUS_CODE,
                //        message = General.OK_MESSAGE,
                //        data = viewModel,
                //    });
                //}
                //else
                //{
                SupplierViewModel supplier = _facade.GetSupplier(viewModel.supplier._id);

                viewModel.supplier.address = supplier == null ? "" : supplier.address;
                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                UnitPaymentOrder spbModel = _spbFacade.ReadById((int)model.UPOId);
                UnitPaymentOrderViewModel viewModelSpb = _mapper.Map<UnitPaymentOrderViewModel>(spbModel);
                DateTimeOffset? receiptDate = null;
                var today = new DateTime(1970, 1, 1);
                foreach (var item in model.Items)
                {
                    Lib.Models.UnitReceiptNoteModel.UnitReceiptNote urnModel = _facade.GetUrn(item.URNNo);

                    if (receiptDate == null || urnModel.ReceiptDate > receiptDate)
                        receiptDate = urnModel == null ? today : urnModel.ReceiptDate;
                }
                UnitPaymentPriceCorrectionNotePDFTemplate PdfTemplate = new UnitPaymentPriceCorrectionNotePDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, viewModelSpb, identityService.Username, clientTimeZoneOffset, receiptDate);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"{model.UPCNo}.pdf"
                };
                //}
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
