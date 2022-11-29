using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitExpenditureNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-unit-expenditure-notes")]
    [Authorize]
    public class GarmentUnitExpenditureNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentUnitExpenditureNoteFacade facade;
        private readonly IGarmentUnitDeliveryOrderFacade facadeUnitDO;
        private readonly IdentityService identityService;

        public GarmentUnitExpenditureNoteController(IServiceProvider serviceProvider, IMapper mapper, IGarmentUnitExpenditureNoteFacade facade, IGarmentUnitDeliveryOrderFacade facadeUnitDO)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.facadeUnitDO = facadeUnitDO;
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
        [HttpGet("custom-loader")]
        public IActionResult GetCustomLoader(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}", Lib.Helpers.ConditionType conditionType = Lib.Helpers.ConditionType.ENUM_INT)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var model = facade.ReadLoader(page, size, order, keyword, filter, conditionType);

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
                else
                {
                    foreach (var item in viewModel.Items)
                    {
                        GarmentUnitDeliveryOrder garmentUnitDeliveryOrder = facadeUnitDO.ReadById((int)viewModel.UnitDOId);
                        if (garmentUnitDeliveryOrder!=null)
                        {
                            GarmentUnitDeliveryOrderViewModel garmentUnitDeliveryOrderViewModel = mapper.Map<GarmentUnitDeliveryOrderViewModel>(garmentUnitDeliveryOrder);
                            var garmentUnitDOItem = garmentUnitDeliveryOrder.Items.FirstOrDefault(i => i.Id == item.UnitDOItemId);
                            if (garmentUnitDOItem != null)
                            {
                                item.DesignColor = garmentUnitDOItem.DesignColor;
								item.RONo = garmentUnitDOItem.RONo;
                            }
                        }

                    }
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

                    var stream = GarmentUnitExpenditureNotePDFTemplate.GeneratePdfTemplate(serviceProvider, viewModel);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.UENNo}.pdf"
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

		[HttpGet("ro-asal/{id}")]
		public IActionResult GetROAsal(int id)
		{
			try
			{
				var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
				var viewModel = facade.GetROAsalById(id);
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
        [HttpGet("data/{id}")]
        public IActionResult GetDataById(int id)
        {
            try
            {
                var viewModel = facade.GetDataUEN(id);
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
        public async Task<IActionResult> Post([FromBody]GarmentUnitExpenditureNoteViewModel viewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                if (viewModel.Items!=null)    
                {
                    viewModel.Items = viewModel.Items.Where(s => s.IsSave).ToList();
                }

                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));
                validateService.Validate(viewModel);

                var Model = mapper.Map<GarmentUnitExpenditureNote>(viewModel);

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
        public async Task<IActionResult> Put(int id, [FromBody]GarmentUnitExpenditureNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentUnitExpenditureNote>(ViewModel);

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                await facade.Delete(id);
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchOne(long id, [FromBody]JsonPatchDocument<GarmentUnitExpenditureNote> jsonPatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.PatchOne(id, jsonPatch);

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

        [HttpGet("unit-expenditure-note")]
        public IActionResult GetForGarmentPreparing(int page = 1, int size = 10, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadForGPreparing(page, size, order, keyword, filter);

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

        [HttpPut("isPreparingTrue/{id}")]
        public async Task<IActionResult> PutIsPreparingTrue(int id, [FromBody]GarmentUnitExpenditureNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                ViewModel.IsPreparing = true;

                var model = mapper.Map<GarmentUnitExpenditureNote>(ViewModel);

                await facade.UpdateIsPreparing(id, model);

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

        [HttpPut("isPreparingFalse/{id}")]
        public async Task<IActionResult> PutIsPreparingFalse(int id, [FromBody]GarmentUnitExpenditureNoteViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                ViewModel.IsPreparing = false;

                var model = mapper.Map<GarmentUnitExpenditureNote>(ViewModel);

                await facade.UpdateIsPreparing(id, model);

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

        [HttpPut("returQuantity/{id}")]
        public async Task<IActionResult> PutReturQuantity(int id, [FromBody]Dictionary<string, double> ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.UpdateReturQuantity(id, ViewModel.GetValueOrDefault("ReturQuantity"), ViewModel.GetValueOrDefault("Quantity"));

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

        [HttpGet("by-unit-expenditure-note/{id}")]
        public IActionResult GetByUEN(int id)
        {
            try
            {
                var model = facade.ReadByUENId(id);

                var viewModel = mapper.Map<GarmentUnitExpenditureNoteViewModel>(model);

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
        [HttpGet("monitoring-out")]
        public IActionResult GetMonitoringOut(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = facade.GetReportOut(dateFrom, dateTo, type, page, size, Order, offset);


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
        [HttpGet("monitoring-out/download")]
        public IActionResult GetXlsMonOut(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = facade.GenerateExcelMonOut(dateFrom, dateTo, type, offset);

                string filename = "Monitoring Pengeluaran";
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

        [HttpGet("basic-price/{po}")]
        public IActionResult GetBasicPrice(string po)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                var viewModel = facade.GetBasicPriceByPOSerialNumber(po);
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

        [HttpPut("revisedate-uen/{reviseDate}")]
        public IActionResult UrnReviseDate(DateTime reviseDate, [FromBody]List<GarmentUnitExpenditureNoteViewModel> Ids)
        {

            try
            {
                var ids = Ids.Select(x => x.Id).ToList();
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                facade.UenDateRevise(
                    ids, identityService.Username, reviseDate
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpGet("loader-by-ro")]
        public IActionResult GetLoaderByRO(string keyword = null, string filter = "{}")
        {
            try
            {
                var result = facade.ReadLoaderProductByROJob(keyword, filter);

                if (result == null)
                {
                    throw new Exception("Invalid RO");
                }

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
    }
}
