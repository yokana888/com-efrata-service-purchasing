using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitPaymentOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-payment-orders")]
    [Authorize]
    public class UnitPaymentOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IUnitPaymentOrderFacade facade;
        private readonly IdentityService identityService;

        public UnitPaymentOrderController(IServiceProvider serviceProvider, IMapper mapper, IUnitPaymentOrderFacade facade)
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
                var Data = facade.Read(page, size, order, keyword, filter);
                var newData = mapper.Map<List<UnitPaymentOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.supplier,
                    s.division,
                    s.date,
                    s.dueDate,
                    s.no,
                    s.useIncomeTax,
                    s.useVat,
                    s.category,
                    s.currency,
                    s.importInfo,
                    items = s.items.Select(i => new
                    {
                        unitReceiptNote = new
                        {
                            i.unitReceiptNote._id,
                            i.unitReceiptNote.no,
                            i.unitReceiptNote.deliveryOrder,
                            i.unitReceiptNote.items
                        }
                    }),
                    s.LastModifiedUtc,
                }));

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

        [HttpGet("by-po-ext-ids")] 
        public IActionResult GetByPOExtIds(List<long> poExtIds, int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = facade.Read(poExtIds, page, size, order, keyword, filter);
                var newData = mapper.Map<List<UnitPaymentOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.supplier,
                    s.division,
                    s.date,
                    s.dueDate,
                    s.no,
                    s.useIncomeTax,
                    s.useVat,
                    s.category,
                    s.currency,
                    items = s.items.Select(i => new
                    {
                        unitReceiptNote = new
                        {
                            i.unitReceiptNote._id,
                            i.unitReceiptNote.no,
                            i.unitReceiptNote.deliveryOrder,
                            i.unitReceiptNote.items
                        }
                    }),
                    s.LastModifiedUtc,
                }));

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

        [HttpGet("by-user")]
        public IActionResult GetByUser(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                var model = facade.ReadById(id);

                if (indexAcceptPdf < 0)
                {
                    var viewModel = mapper.Map<UnitPaymentOrderViewModel>(model);

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
                    identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                    /* tambahan */
                    /* get Supplier */
                    string supplierUri = "master/suppliers";
                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    var response = httpClient.GetAsync($"{Lib.Helpers.APIEndpoint.Core}{supplierUri}/{model.SupplierId}").Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                    SupplierViewModel supplier = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                    /* tambahan */

                    UnitPaymentOrderPDFTemplate PdfTemplate = new UnitPaymentOrderPDFTemplate();
                    var stream = PdfTemplate.Generate(model, facade, supplier ,clientTimeZoneOffset, identityService.Username);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{model.UPONo}.pdf"
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

        [HttpGet("EPO/{epo}")]
        public IActionResult GetEpo(string epo)
        {
            try
            {
                var model = facade.ReadByEPONo(epo);
                var viewModel = mapper.Map<List<UnitPaymentOrderViewModel>>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
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
        public async Task<IActionResult> Post([FromBody] UnitPaymentOrderViewModel viewModel)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(viewModel);

                var model = mapper.Map<UnitPaymentOrder>(viewModel);

                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                int result = await facade.Create(model, identityService.Username, viewModel.supplier.import, clientTimeZoneOffset);

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
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]UnitPaymentOrderViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

            UnitPaymentOrder m = mapper.Map<UnitPaymentOrder>(vm);

            IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

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
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

            try
            {
                await facade.Delete(id, identityService.Username);
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

        [HttpGet("spb")]
        public IActionResult GetSpb(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = facade.ReadSpb(page, size, order, keyword, filter);
                var newData = mapper.Map<List<UnitPaymentOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.supplier,
                    s.division,
                    s.category,
                    s.currency,
                    s.paymentMethod,
                    s.invoiceDate,
                    s.invoiceNo,
                    s.pibNo,
                    s.useIncomeTax,
                    s.useVat,
                    s.vatNo,
                    s.vatDate,
                    s.remark,
                    s.dueDate,
                    s.date,
                    s.no,
                    items = s.items.Select(i => new
                    {
                        unitReceiptNote = new
                        {
                            i.unitReceiptNote._id,
                            i.unitReceiptNote.no,
                            i.unitReceiptNote.deliveryOrder,
                            i.unitReceiptNote.items
                        }
                    }),
                    s.LastModifiedUtc,
                }));

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

        [HttpGet("spb-for-verification")]
        public IActionResult GetSpbForVerification(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = facade.ReadSpbForVerification(page, size, order, keyword, filter);
                var newData = mapper.Map<List<UnitPaymentOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.supplier,
                    s.division,
                    s.category,
                    s.currency,
                    s.paymentMethod,
                    s.invoiceDate,
                    s.invoiceNo,
                    s.pibNo,
                    s.useIncomeTax,
                    s.useVat,
                    s.vatTax,
                    s.vatNo,
                    s.incomeTax,
                    s.incomeTaxBy,
                    s.vatDate,
                    s.remark,
                    s.dueDate,
                    s.date,
                    s.no,
                    items = s.items.Select(i => new
                    {
                        unitReceiptNote = new
                        {
                            i.unitReceiptNote._id,
                            i.unitReceiptNote.no,
                            i.unitReceiptNote.deliveryOrder,
                            i.unitReceiptNote.items
                        }
                    }),
                    s.LastModifiedUtc,
                }));

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


        [HttpGet("by-position")]
        public IActionResult GetByPosition(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                var Data = facade.ReadPositionFiltered(page, size, order, keyword, filter);
                var newData = mapper.Map<List<UnitPaymentOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.supplier,
                    s.division,
                    s.date,
                    s.dueDate,
                    s.no,
                    s.useIncomeTax,
                    s.useVat,
                    s.category,
                    s.currency,
                    s.incomeTax,
                    s.incomeTaxDate,
                    s.incomeTaxNo,
                    s.IsCorrection,
                    s.invoiceDate,
                    s.paymentMethod,
                    s.invoiceNo,
                    items = s.items.Select(i => new
                    {
                        unitReceiptNote = new
                        {
                            i.unitReceiptNote._id,
                            i.unitReceiptNote.no,
                            i.unitReceiptNote.deliveryOrder,
                            i.unitReceiptNote.items
                        }
                    }),
                    s.LastModifiedUtc,
                }));

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

        #region MonitoringAll
        [HttpGet("monitoringall")]
        public IActionResult GetReportAll(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = facade.GetReportAll(unitId, supplierId, noSPB, dateFrom, dateTo, page, size, Order, offset);

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

        [HttpGet("monitoringall/download")]
        public IActionResult GetXlsAll(string unitId, string supplierId,string noSPB, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcel(unitId, supplierId, noSPB, dateFrom, dateTo, offset);

                string filename = String.Format("UnitPaymentOrder - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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

        #region MonitoringTax
        [HttpGet("monitoringtax")]
        public IActionResult GetReportTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = facade.GetReportTax(supplierId, taxno, dateFrom, dateTo, taxdateFrom, taxdateTo, page, size, Order, offset);

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

        [HttpGet("monitoringtax/download")]
        public IActionResult GetXlsTax(string supplierId, string taxno, DateTime? dateFrom, DateTime? dateTo, DateTime? taxdateFrom, DateTime? taxdateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelTax(supplierId, taxno, dateFrom, dateTo, taxdateFrom, taxdateTo, offset);

                string filename = String.Format("Monitoring PPN dan PPH - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
