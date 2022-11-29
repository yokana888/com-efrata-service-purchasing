using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentBeacukaiModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentBeacukaiViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentBeacukaiControllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/garment-beacukai")]
	[Authorize]
	public class GarmentBeacukaiController : Controller
	{
		private string ApiVersion = "1.0.0";
		public readonly IServiceProvider serviceProvider;
		private readonly IMapper mapper;
		private readonly IGarmentBeacukaiFacade facade;
		private readonly IGarmentDeliveryOrderFacade DOfacade;
		private readonly IdentityService identityService;

		public GarmentBeacukaiController(IServiceProvider serviceProvider, IMapper mapper, IGarmentBeacukaiFacade facade, IGarmentDeliveryOrderFacade DOfacade)
		{
			this.serviceProvider = serviceProvider;
			this.mapper = mapper;
			this.facade = facade;
			this.DOfacade = DOfacade;
			this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
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

		[HttpGet]
		public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
				var Data = facade.Read(page, size, order, keyword, filter);

				var viewModel = mapper.Map<List<GarmentBeacukaiViewModel>>(Data.Item1);

				List<object> listData = new List<object>();
				listData.AddRange(
					viewModel.AsQueryable().Select(s => s).ToList()
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

		[HttpPost]
		public async Task<IActionResult> Post([FromBody]GarmentBeacukaiViewModel ViewModel)
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(ViewModel);

				var model = mapper.Map<GarmentBeacukai>(ViewModel);

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

		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			try
			{
				var model = facade.ReadById(id);
				var viewModel = mapper.Map<GarmentBeacukaiViewModel>(model);
				if (viewModel == null)
				{
					throw new Exception("Invalid Id");
				}
				foreach (var item in viewModel.items)
				{
					GarmentDeliveryOrder deliveryOrder = DOfacade.ReadById((int)item.deliveryOrder.Id);
					if (deliveryOrder != null)
					{
						GarmentDeliveryOrderViewModel deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
						item.deliveryOrder.isInvoice = deliveryOrderViewModel.isInvoice;
						item.deliveryOrder.items = deliveryOrderViewModel.items;
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

		[HttpDelete("{id}")]
		public IActionResult Delete([FromRoute]int id)
		{
			identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
			identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

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

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromBody]GarmentBeacukaiViewModel ViewModel)
		{
			try
		    {
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(ViewModel);

				var model = mapper.Map<GarmentBeacukai>(ViewModel);

				await facade.Update(id,ViewModel, model, identityService.Username);

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

        [HttpGet("by-poserialnumber")]
        public IActionResult UnitDOByRO(string keyword = null, string Filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadBCByPOSerialNumber(keyword, Filter);

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

        [HttpGet("by-poserialnumbers")]
        public IActionResult BCByPo([FromBody]string Keyword)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadBCByPOSerialNumbers(Keyword);

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