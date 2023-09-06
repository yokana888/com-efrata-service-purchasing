using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInternalPurchaseOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-internal-purchase-orders")]
    [Authorize]
    public class GarmentInternalPurchaseOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentInternalPurchaseOrderFacade facade;
        private readonly IdentityService identityService;

        public GarmentInternalPurchaseOrderController(IServiceProvider serviceProvider, IMapper mapper, IGarmentInternalPurchaseOrderFacade facade)
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

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<GarmentInternalPurchaseOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.PRNo,
                        s.RONo,
                        s.Article,
                        s.ShipmentDate,
                        s.Buyer,
                        Items = s.Items.Select(i => new
                        {
                            i.Product,
                            i.Quantity,
                            i.Uom
                        }).ToList(),
                        s.CreatedBy,
                        s.IsPosted,
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
                var viewModel = mapper.Map<GarmentInternalPurchaseOrderViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                viewModel.HasDuplicate = facade.CheckDuplicate(model);

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
        public async Task<IActionResult> Post([FromBody]List<GarmentInternalPurchaseOrderViewModel> ListViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                foreach (var viewModel in ListViewModel)
                {
                    validateService.Validate(viewModel);
                }

                var ListModel = mapper.Map<List<GarmentInternalPurchaseOrder>>(ListViewModel);

                await facade.CreateMultiple(ListModel, identityService.Username);

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

        [HttpPut("split/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]GarmentInternalPurchaseOrderViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentInternalPurchaseOrder>(ViewModel);

                await facade.Split(id, model, identityService.Username);

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

        [HttpGet("by-tags")]
        public IActionResult GetByTags(string category, string tags, string shipmentDateFrom, string shipmentDateTo)
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                DateTimeOffset shipmentFrom = DateTimeOffset.MinValue;
                DateTimeOffset shipmentTo = DateTimeOffset.MinValue;
                if (!string.IsNullOrWhiteSpace(shipmentDateFrom) && !string.IsNullOrWhiteSpace(shipmentDateTo))
                {
                    if (!DateTimeOffset.TryParseExact(shipmentDateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out shipmentFrom) ||
                        !DateTimeOffset.TryParseExact(shipmentDateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out shipmentTo))
                    {
                        shipmentFrom = DateTimeOffset.MinValue;
                        shipmentTo = DateTimeOffset.MinValue;
                    }
                }

                var data = facade.ReadByTags(category, tags, shipmentFrom, shipmentTo, identityService.Username);
                var newData = mapper.Map<List<GarmentInternalPurchaseOrderViewModel>>(data);

                var info = new Dictionary<string, object>
                    {
                        { "count", newData.Count },
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(newData, info);
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
		[HttpGet("by-name")]
		public IActionResult ByName(string keyword = null, string Filter = "{}")
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

				var Data = facade.ReadName(keyword, Filter);

				//var newData = mapper.Map<List<GarmentInternalPurchaseOrderViewModel>>(Data);

				//List<object> listData = new List<object>();
				//listData.AddRange(
				//	newData.AsQueryable().Select(s => new
				//	{
				//		s.CreatedBy
				//	}).ToList()
				//);

				return Ok(new
				{
					apiVersion = ApiVersion,
					statusCode = General.OK_STATUS_CODE,
					message = General.OK_MESSAGE,
					data = Data
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