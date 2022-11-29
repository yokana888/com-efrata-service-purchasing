using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.InternalPurchaseOrderController
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/internal-purchase-orders")]
    [Authorize]
    public class InternalPurchaseOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly InternalPurchaseOrderFacade _facade;
        private readonly IdentityService identityService;
        public InternalPurchaseOrderController(IMapper mapper, InternalPurchaseOrderFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            //Tuple<List<object>, int, Dictionary<string, string>> Data = _facade.Read(page, size, order, keyword, filter);
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

            var Data = _facade.Read(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<InternalPurchaseOrderViewModel>>(Data.Item1);
            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.poNo,
                    s.isoNo,
                    s.prId,
                    s.prNo,
                    s.prDate,
                    s.expectedDeliveryDate,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name
                    },
                    category = new
                    {
                        s.category.name
                    },
                    s.remark,
                    s.status,
                    s.CreatedBy,
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
                    { "total", Data.Item2 },
                    { "order", Data.Item3 },
                    { "page", page },
                    { "size", size }
                },
            });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                InternalPurchaseOrder model = _facade.ReadById(id);
                InternalPurchaseOrderViewModel viewModel = _mapper.Map<InternalPurchaseOrderViewModel>(model);
                viewModel.countPRNo = _facade.ReadByPRNo(viewModel.prNo);
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]InternalPurchaseOrderViewModel vm)
        {
            identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            InternalPurchaseOrder m = _mapper.Map<InternalPurchaseOrder>(vm);
            ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));

            try
            {                
                validateService.Validate(vm);

                int result = await _facade.Create(m, identityService.Username);

                if (result.Equals(0))
                {
                    return StatusCode(500);
                }
                else
                {
                    return Created(Request.Path,result);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]InternalPurchaseOrderViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            InternalPurchaseOrder m = _mapper.Map<InternalPurchaseOrder>(vm);

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

        [HttpPost("spliting/{id}")]
        public async Task<IActionResult> Split([FromRoute]int id, [FromBody]InternalPurchaseOrderViewModel vm)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            InternalPurchaseOrder m = _mapper.Map<InternalPurchaseOrder>(vm);
            ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));
            try
            {
                validateService.Validate(vm);
                int result = await _facade.Split(id, m, identityService.Username);

                return NoContent();
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);

            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            //Tuple<List<object>, int, Dictionary<string, string>> Data = _facade.Read(page, size, order, keyword, filter);
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.Read(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<InternalPurchaseOrderViewModel>>(Data.Item1);
            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.poNo,
                    s.isoNo,
                    s.prId,
                    s.prNo,
                    s.prDate,
                    s.expectedDeliveryDate,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name
                    },
                    category = new
                    {
                        s.category.name
                    },
                    s.remark,
                    s.status,
                    s.CreatedBy,
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
                    { "total", Data.Item2 },
                    { "order", Data.Item3 },
                    { "page", page },
                    { "size", size }
                },
            });
        }

        //MONITORING
        #region MONITORING
        [HttpGet("monitoring")]
        public IActionResult GetReport(string unitId, string categoryId, string staff, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = _facade.GetReport(unitId, categoryId, staff, dateFrom, dateTo, page, size, Order, offset);

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
        public IActionResult GetXls(string unitId, string categoryId, string staff, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcel(unitId, categoryId, staff, dateFrom, dateTo, offset);

                string filename = String.Format("Purchase Order Internal - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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

        //MONITORING-UNPROCESSED
        #region MONITORING-UNPROCESSED
        [HttpGet("monitoring-unprocessed")]
        public IActionResult GetReportUnProcessed(string unitId, string categoryId, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = _facade.GetReportUnProcessed(unitId, categoryId, dateFrom, dateTo, page, size, Order, offset);

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

        [HttpGet("monitoring-unprocessed/download")]
        public IActionResult GetXls(string unitId, string categoryId, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcelUnProcessed(unitId, categoryId, dateFrom, dateTo, offset);

                string filename = String.Format("PO Internal Belum Proses PO External - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
