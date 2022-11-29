using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInternNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-intern-notes")]
    [Authorize]
    public class GarmentInternNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentInternNoteFacade facade;
        private readonly IGarmentDeliveryOrderFacade deliveryOrderFacade;
        private readonly IGarmentInvoice invoiceFacade;
        private readonly IdentityService identityService;

        public GarmentInternNoteController(IServiceProvider serviceProvider, IMapper mapper, IGarmentInternNoteFacade facade, IGarmentDeliveryOrderFacade deliveryOrderFacade, IGarmentInvoice invoiceFacade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.deliveryOrderFacade = deliveryOrderFacade;
            this.invoiceFacade = invoiceFacade;
        }

        protected void VerifyUser()
        {
            identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet("by-user")]
        public IActionResult GetByUser(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

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

        [HttpGet("dpp-vat-bank-expenditures")]
        public IActionResult GetDPPVATBankExpenditures([FromQuery] string currencyCode, [FromQuery] int supplierId)
        {
            try
            {
                var result = facade.BankExpenditureReadInternalNotes(currencyCode, supplierId);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);
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

        [HttpGet("dpp-vat-bank-expenditures-optimized")]
        public IActionResult GetDPPVATBankExpendituresOptimized([FromQuery] int currencyId, [FromQuery] int supplierId)
        {
            try
            {
                var result = facade.BankExpenditureReadInternalNotesOptimized(currencyId, supplierId);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);
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

        [HttpPut("dpp-vat-bank-expenditures/is-paid")]
        public IActionResult UpdateIsPaidDPPVATBankExpenditures([FromQuery] bool dppVATIsPaid, [FromQuery] int bankExpenditureNoteId, [FromQuery] string bankExpenditureNoteNo, [FromQuery] string internalNoteIds = "[]", [FromQuery] string invoiceNoteIds = "[]")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                var result = facade.BankExpenditureUpdateIsPaidInternalNoteAndInvoiceNote(dppVATIsPaid, bankExpenditureNoteId, bankExpenditureNoteNo, internalNoteIds, invoiceNoteIds);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result);
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

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<GarmentInternNoteViewModel>>(Data.Item1);

                HashSet<long> garmentInvoiceIds = new HashSet<long>(viewModel.SelectMany(vm => vm.items.Select(i => i.garmentInvoice.Id)));
                List<GarmentInvoice> garmentInvoices = invoiceFacade.ReadForInternNote(garmentInvoiceIds.ToList());

                HashSet<long> deliveryOrderIds = new HashSet<long>(viewModel.SelectMany(vm => vm.items.SelectMany(i => i.details.Select(d => d.deliveryOrder.Id))));
                List<GarmentDeliveryOrder> garmentDeliveryOrders = deliveryOrderFacade.ReadForInternNote(deliveryOrderIds.ToList());

                foreach (var d in viewModel)
                {
                    foreach (var item in d.items)
                    {
                        GarmentInvoice garmentInvoice = garmentInvoices.SingleOrDefault(gi => gi.Id == item.garmentInvoice.Id);
                        if (garmentInvoice != null)
                        {
                            GarmentInvoiceViewModel invoiceViewModel = mapper.Map<GarmentInvoiceViewModel>(garmentInvoice);

                            item.garmentInvoice.items = invoiceViewModel.items;
                        }
                        foreach (var detail in item.details)
                        {
                            var deliveryOrder = garmentDeliveryOrders.SingleOrDefault(gdo => gdo.Id == detail.deliveryOrder.Id);
                            if (deliveryOrder != null)
                            {
                                var deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
                                detail.deliveryOrder.items = deliveryOrderViewModel.items;
                            }
                        }
                    }
                }

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.inNo,
                        s.inDate,
                        supplier = new { s.supplier.Name },
                        items = s.items.Select(i => new
                        {
                            //i.garmentInvoice,
                            garmentInvoice = new
                            {
                                i.garmentInvoice.invoiceNo,
                                items = i.garmentInvoice.items == null ? null : i.garmentInvoice.items.Select(gii => new
                                {
                                    details = gii.details == null ? null : gii.details.Select(gid => new
                                    {
                                        gid.dODetailId
                                    })
                                })
                            },
                            details = i.details.Select(d => new
                            {
                                //d.deliveryOrder
                                deliveryOrder = new
                                {
                                    items = d.deliveryOrder.items == null ? null : d.deliveryOrder.items.Select(doi => new
                                    {
                                        fulfillments = doi.fulfillments == null ? null : doi.fulfillments.Select(dof => new
                                        {
                                            dof.Id,
                                            dof.receiptQuantity
                                        })
                                    })
                                }
                            }).ToList(),
                        }).ToList(),
                        s.CreatedBy,
                        s.LastModifiedUtc
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
        public IActionResult Get(int id)
        {
            try
            {
                var model = facade.ReadById(id);
                var viewModel = mapper.Map<GarmentInternNoteViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                else
                {
                    viewModel.isEdit = model.Position <= PurchasingGarmentExpeditionPosition.Purchasing;
                    foreach (GarmentInternNoteItemViewModel item in viewModel.items)
                    {
                        GarmentInvoice garmentInvoice = invoiceFacade.ReadById((int)item.garmentInvoice.Id);
                        if (garmentInvoice != null)
                        {
                            GarmentInvoiceViewModel invoiceViewModel = mapper.Map<GarmentInvoiceViewModel>(garmentInvoice);

                            item.garmentInvoice.items = invoiceViewModel.items;

                        }
                        foreach (GarmentInternNoteDetailViewModel detail in item.details)
                        {
                            GarmentDeliveryOrder deliveryOrder = deliveryOrderFacade.ReadById((int)detail.deliveryOrder.Id);
                            if (deliveryOrder != null)
                            {
                                GarmentDeliveryOrderViewModel deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
                                detail.deliveryOrder.items = deliveryOrderViewModel.items;
                                if (detail.invoiceDetailId != 0)
                                {
                                    var invoiceItem = garmentInvoice.Items.First(s => s.Details.Any(d => d.Id == detail.invoiceDetailId));

                                    var invoiceDetail = invoiceItem.Details.First(i => i.Id == detail.invoiceDetailId);

                                    if (invoiceDetail != null)
                                    {
                                        detail.dODetailId = invoiceDetail.DODetailId;
                                    }
                                }

                            }
                        }
                    }
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(viewModel);
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
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post([FromBody] GarmentInternNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentInternNote>(ViewModel);

                await facade.Create(model, ViewModel.supplier.Import, identityService.Username);

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
                    
                //return BadRequest(Result);
                return StatusCode(General.BAD_REQUEST_STATUS_CODE, Result);
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
        public async Task<IActionResult> Put(int id, [FromBody] GarmentInternNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentInternNote>(ViewModel);

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

        [HttpGet("pdf/{id}")]
        public IActionResult GetInternNotePDF(int id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                GarmentInternNote model = facade.ReadById(id);
                GarmentInternNoteViewModel viewModel = mapper.Map<GarmentInternNoteViewModel>(model);
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

                    foreach (var item in viewModel.items)
                    {
                        var garmentInvoice = invoiceFacade.ReadById((int)item.garmentInvoice.Id);
                        var garmentInvoiceViewModel = mapper.Map<GarmentInvoiceViewModel>(garmentInvoice);
                        item.garmentInvoice = garmentInvoiceViewModel;

                        foreach (var detail in item.details)
                        {
                            var deliveryOrder = deliveryOrderFacade.ReadById((int)detail.deliveryOrder.Id);
                            var deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
                            detail.deliveryOrder = deliveryOrderViewModel;
                        }
                    }

                    GarmentInternNotePDFTemplate PdfTemplateLocal = new GarmentInternNotePDFTemplate();
                    MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel, serviceProvider, clientTimeZoneOffset, deliveryOrderFacade);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.inNo}.pdf"
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
        #region Monitoring
        [HttpGet("monitoring")]
        public IActionResult GetReportIN(DateTime? dateFrom, DateTime? dateTo, string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {

                VerifyUser();
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = facade.GetReport(no, supplierCode, curencyCode, invoiceNo, npn, doNo, billNo, paymentBill, dateFrom, dateTo, page, size, Order, offset);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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
        [HttpGet("monitoring/download")]
        public IActionResult GetXlsDO2(DateTime? dateFrom, DateTime? dateTo, string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill)
        {
            //Console.WriteLine("sampai sini");
            try
            {
                VerifyUser();
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelIn(no, supplierCode, curencyCode, invoiceNo, npn, doNo, billNo, paymentBill, dateFrom, dateTo, offset);
                string filename = String.Format("Nota Intern - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                   .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }

        }
        #endregion
    }
}
