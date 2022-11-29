using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitReceiptNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-unit-receipt-notes")]
    [Authorize]
    public class GarmentUnitReceiptNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentUnitReceiptNoteFacade facade;
        private readonly IdentityService identityService;

        public GarmentUnitReceiptNoteController(IServiceProvider serviceProvider, IMapper mapper, IGarmentUnitReceiptNoteFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
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

                var model = facade.Read(page, size, order, keyword, filter);

                var info = new Dictionary<string, object>
                    {
                        { "count", model.Data.Count },
                        { "total", model.TotalData },
                        { "order", model.Order },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(model.Data, info);
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

                var viewModel = facade.ReadById(id);
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
                    identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    var stream = facade.GeneratePdf(viewModel);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.URNNo}.pdf"
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
        public async Task<IActionResult> Post([FromBody]GarmentUnitReceiptNoteViewModel viewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token= Request.Headers["Authorization"].First().Replace("Bearer ", "");

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));
                validateService.Validate(viewModel);

                var Model = mapper.Map<GarmentUnitReceiptNote>(viewModel);

                await facade.Create(Model);

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
        public async Task<IActionResult> Put(int id, [FromBody]GarmentUnitReceiptNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentUnitReceiptNote>(ViewModel);

                await facade.Update(id, model);

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

        [HttpPut("deleted/{id}")]
        public async Task<IActionResult> DeleteData([FromRoute]int id, [FromBody]GarmentUnitReceiptNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                await facade.Delete(id, ViewModel.DeletedReason);
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute]int id)
        //{
        //    try
        //    {
        //        identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
        //        identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

        //        await facade.Delete(id);
        //        return NoContent();
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}

        [HttpGet("unit-delivery-order")]
        public IActionResult GetForUnitDO(string keyword = null, string filter = "{}")
        {
            try
            {
                var result = facade.ReadForUnitDO(keyword, filter);
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

        [HttpGet("unit-delivery-order-header")]
        public IActionResult GetForUnitDOHeader(string keyword = null, string filter = "{}")
        {
            try
            {
                var result = facade.ReadForUnitDOHeader(keyword, filter);
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

        [HttpGet("items")]
        public IActionResult GetURNItem(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

               // var model = facade.ReadURNItem(page, size, order, keyword, filter);
                var result = facade.ReadURNItem(keyword, filter);
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

        [HttpGet("by-ro")]
        public IActionResult GetByRO(string keyword = null, string filter = "{}")
        {
            try
            {
                var result = facade.ReadItemByRO(keyword, filter);
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
        //
        [HttpGet("by-do")]
        public IActionResult GetByDO(string keyword = null, string filter = "{}")
        {
            try
            {
                var result = facade.ReadDataByDO(keyword, filter);
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
        //

        #region Flow Detail Permintaan 
        [HttpGet("laporan")]
        public IActionResult GetReportDO(DateTime? dateFrom, DateTime? dateTo, string unit, string category, int page = 1, int size = int.MaxValue, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = facade.GetReportFlow(dateFrom, dateTo, unit, category, page, size, Order, offset);


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


        [HttpGet("laporan/download")]
        public IActionResult GetXls(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, string unitname, int page, int size, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcelLow(dateFrom, dateTo, unit, category, categoryname, offset, unitname);

                string filename = "Laporan Rekap BUM";
                        if (dateFrom != null) filename += " " + ((DateTime) dateFrom).ToString("dd-MM-yyyy");
                        if (dateTo != null) filename += "_" + ((DateTime) dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("laporan/download-for-unit")]
        public IActionResult GetXlsForUnit(DateTime? dateFrom, DateTime? dateTo, string unit, string category, string categoryname, string unitname, int page, int size, string Order = "{}")
        {
            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcelFlowForUnit(dateFrom, dateTo, unit, category, categoryname, offset, unitname);

                string filename = "Laporan Flow BUM";
                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("monitoring-in")]
        public IActionResult GetMonitoringIN(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = facade.GetReportIN(dateFrom, dateTo, type, page, size, Order, offset);


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
        [HttpGet("monitoring-in/download")]
        public IActionResult GetXlsMonIn(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcelMonIN(dateFrom, dateTo, type, offset);

                string filename = "Monitoring Pemasukan";
                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");
                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("do-items/{id}")]
        public IActionResult GetDOItems(int id)
        {
            try
            {
                var viewModel = facade.ReadDOItemsByURNItemId(id);
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

        [HttpPut("revisedate-urn/{reviseDate}")]
        public IActionResult UrnReviseDate(DateTime reviseDate, [FromBody]List<GarmentUnitReceiptNoteViewModel> Ids)
        {
            
            try
            {
                var ids = Ids.Select(x => x.Id).ToList();
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                facade.UrnDateRevise(
                    ids, identityService.Username, reviseDate
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

       


    }
}
