using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentDeliveryOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-delivery-orders")]
    [Authorize]
    public class GarmentDeliveryOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentDeliveryOrderFacade facade;
        private readonly IdentityService identityService;

        public GarmentDeliveryOrderController(IServiceProvider serviceProvider, IMapper mapper, IGarmentDeliveryOrderFacade facade)
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

        [HttpGet("by-supplier")]
        public IActionResult GetBySupplier(string Keyword = "", string Filter = "{}")
        {
            var Data = facade.ReadBySupplier(Keyword, Filter);
            var newData = mapper.Map<List<GarmentDeliveryOrderViewModel>>(Data);
            Dictionary<string, object> Result =
                   new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                   .Ok(newData);
            return Ok(Result);
        }
        [HttpGet("forCustoms")]
        public IActionResult GetForCustoms(string Keyword = "", string Filter = "{}", string BillNo = null)
        {
            var Data = facade.DOForCustoms(Keyword, Filter, BillNo);
            var newData = mapper.Map<List<GarmentDeliveryOrderViewModel>>(Data);
            Dictionary<string, object> Result =
                   new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                   .Ok(newData);
            return Ok(Result);
        }
        [HttpGet("isReceived")]
        public IActionResult GetIsReceived(List<int> Id)
        {
            var Data = facade.IsReceived(Id);
            Dictionary<string, object> Result =
                   new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                   .Ok(Data);
            return Ok(Result);
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                //var viewModel = mapper.Map<List<GarmentDeliveryOrderViewModel>>(Data.Item1);

                //List<object> listData = new List<object>();
                //listData.AddRange(
                //    viewModel.AsQueryable().Select(s => new
                //    {
                //        s.Id,
                //        s.doNo,
                //        s.doDate,
                //        s.arrivalDate,
                //        s.billNo,
                //        s.paymentBill,
                //        supplier = new { s.supplier.Name },
                //        items = s.items.Select(i => new { i.purchaseOrderExternal, i.fulfillments }),
                //        s.CreatedBy,
                //        s.isClosed,
                //        s.isCustoms,
                //        s.isInvoice,
                //        s.LastModifiedUtc
                //    }).ToList()
                //);

                var listData = Data.Item1.Select(x => new
                {
                    x.Id,
                    doNo = x.DONo,
                    doDate = x.DODate,
                    arrivalDate = x.ArrivalDate,
                    billNo = x.BillNo,
                    paymentBill = x.PaymentBill,
                    supplier = new { Id = x.SupplierId, Code = x.SupplierCode, Name = x.SupplierName },
                    items = x.Items.Select(i => new
                    {
                        purchaseOrderExternal = new { Id = i.EPOId, no = i.EPONo },
                        fulfillments = new List<object>(),
                        details = i.Details.Select(d => new
                        {
                            d.Id,
                            doQuantity = d.DOQuantity,
                        }),
                    }),
                    x.CreatedBy,
                    isClosed = x.IsClosed,
                    isCustoms = x.IsCustoms,
                    isInvoice = x.IsInvoice,
                    x.LastModifiedUtc
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

        [HttpGet("loader")]
        public IActionResult GetLoader(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}", string select = "{}", string search = "[]")
        {
            try
            {
                var Data = facade.ReadLoader(page, size, order, keyword, filter, select, search);

                var info = new Dictionary<string, object>
                    {
                        { "count", Data.Data.Count },
                        { "total", Data.TotalData },
                        { "order", Data.Order },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data.Data, info);
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
                var viewModel = mapper.Map<GarmentDeliveryOrderViewModel>(model);
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
        public async Task<IActionResult> Post([FromBody] GarmentDeliveryOrderViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                foreach (var item in ViewModel.items)
                {
                    item.fulfillments = item.fulfillments.Where(s => s.isSave).ToList();
                }

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentDeliveryOrder>(ViewModel);

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
        public async Task<IActionResult> Put(int id, [FromBody] GarmentDeliveryOrderViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                var vmString = JsonConvert.SerializeObject(ViewModel);
                var vmValidation = JsonConvert.DeserializeObject<GarmentDeliveryOrderViewModel>(vmString);

                foreach (var vmItem in vmValidation.items)
                {
                    vmItem.fulfillments = vmItem.fulfillments.Where(s => s.isSave).ToList();
                    if (vmItem.Id == 0)
                    {
                        foreach (var vItem in ViewModel.items.Where(s => s.Id == vmItem.Id))
                        {
                            vItem.fulfillments = vItem.fulfillments.Where(s => s.isSave).ToList();
                        }
                    }
                }

                validateService.Validate(vmValidation);

                var model = mapper.Map<GarmentDeliveryOrder>(ViewModel);

                await facade.Update(id, ViewModel, model, identityService.Username);

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

        [HttpGet("unit-receipt-note")]
        public IActionResult GetForUnitReceiptNote(int page = 1, int size = 10, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadForUnitReceiptNote(page, size, order, keyword, filter);

                var info = new Dictionary<string, object>
                    {
                        { "count", result.Data.Count },
                        { "total", result.TotalData },
                        { "order", result.Order },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result.Data, info);
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

        [HttpGet("correction-note-quantity")]
        public IActionResult GetForCorrectionNoteQuantity(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadForCorrectionNoteQuantity(page, size, order, keyword, filter);

                var info = new Dictionary<string, object>
                    {
                        { "count", result.Data.Count },
                        { "total", result.TotalData },
                        { "order", result.Order },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(result.Data, info);
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
        //      
        [HttpGet("data/{id}")]
        public IActionResult GetDataDOById(int id)
        {
            try
            {
                var viewModel = facade.GetDataDO(id);
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
        //
        #region MONITORING ARRIVAL
        [HttpGet("arrivalReport")]
        public IActionResult GetReport(string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            var data = facade.GetAccuracyOfArrivalHeader(category, dateFrom, dateTo, offset);
            return Ok(new
            {
                apiVersion = ApiVersion,
                data,
                info = new { total = data.ReportHeader.Count },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }
        [HttpGet("arrivalReport/download")]
        public IActionResult GetXlsArrivalHeader(string category, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTimeOffset DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);


                var xls = facade.GenerateExcelArrivalHeader(category, dateFrom, dateTo, offset);

                if (category == "")
                {
                    category = "Bahan Baku dan Bahan Pendukung";
                }

                string filename = String.Format($"Monitoring Ketepatan Kedatangan {category} - {DateFrom.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))} - {DateTo.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))}.xlsx");

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

        [HttpGet("arrivalReportDetail")]
        public IActionResult GetReportDetail(string supplier, string category, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            var data = facade.GetAccuracyOfArrivalDetail(supplier, category, dateFrom, dateTo, offset);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data,
                info = new { total = data.Count },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }
        [HttpGet("arrivalReportDetail/download")]
        public IActionResult GetXlsArrivalDetail(string supplier, string category, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTimeOffset DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelArrivalDetail(supplier, category, dateFrom, dateTo, offset);

                if (category == "")
                {
                    category = "Bahan Baku dan Bahan Pendukung";
                }

                string filename = String.Format($"Monitoring Detail Ketepatan Kedatangan {category} - {DateFrom.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))} - {DateTo.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))}.xlsx");

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

        #region MONITORING DELIVERY
        [HttpGet("deliveryReport")]
        public IActionResult GetReport2(DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            //try
            //{
            var data = facade.GetReportHeaderAccuracyofDelivery(dateFrom, dateTo, paymentType, paymentMethod, offset);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data = data.Item1,
                info = new { total = data.Item2 },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
            //}
            //catch (Exception e)
            //{
            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
            //        .Fail();
            //    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            //}
        }
        [HttpGet("deliveryReport/download")]
        public IActionResult GetXlsDeliveryHeader(DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod)
        {
            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTimeOffset DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelDeliveryHeader(dateFrom, dateTo, paymentType, paymentMethod, offset);

                string filename = String.Format($"Monitoring Ketepatan Pengiriman - {DateFrom.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))} - {DateTo.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))}.xlsx");

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

        [HttpGet("deliveryReportDetail")]
        public IActionResult GetReportDetail2(string supplier, DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            //try
            //{
            var data = facade.GetReportDetailAccuracyofDelivery(supplier, dateFrom, dateTo, paymentType, paymentMethod, offset);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data = data.Item1,
                info = new { total = data.Item2 },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
            //}
            //catch (Exception e)
            //{
            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
            //        .Fail();
            //    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            //}
        }
        [HttpGet("deliveryReportDetail/download")]
        public IActionResult GetXlsDeliveryDetail(string supplier, DateTime? dateFrom, DateTime? dateTo, string paymentType, string paymentMethod)
        {
            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTimeOffset DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelDeliveryDetail(supplier, dateFrom, dateTo, paymentType, paymentMethod, offset);

                string filename = String.Format($"Monitoring Detail Ketepatan Pengiriman - {DateFrom.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))} - {DateTo.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"))}.xlsx");

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
        //MONITORING
        #region MONITORING DELIVERY ORDER
        [HttpGet("monitoring")]
        public IActionResult GetReportDO(DateTime? dateFrom, DateTime? dateTo, string no, string poEksNo, long supplierId, string billNo, string paymentBill, int page, int size, string Order = "{}")
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = facade.GetReportDO(no, poEksNo, supplierId, billNo, paymentBill, dateFrom, dateTo, page, size, Order, offset);


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
        public IActionResult GetXlsDO(string no, string poEksNo, long supplierId, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcelDO(no, poEksNo, supplierId, billNo, paymentBill, dateFrom, dateTo, offset);

                string filename = String.Format("Surat Jalan - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
