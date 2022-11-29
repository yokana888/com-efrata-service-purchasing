using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentPurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentPurchaseRequestControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-purchase-requests")]
    [Authorize]
    public class GarmentPurchaseRequestController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentPurchaseRequestFacade facade;
        private readonly IdentityService identityService;

        public GarmentPurchaseRequestController(IServiceProvider serviceProvider, IMapper mapper, IGarmentPurchaseRequestFacade facade)
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

                var newData = mapper.Map<List<GarmentPurchaseRequestViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.PRNo,
                        s.RONo,
                        s.MDStaff,
                        s.Article,
                        s.Date,
                        s.ExpectedDeliveryDate,
                        s.ShipmentDate,
                        Buyer = new {
                            s.Buyer.Id,
                            s.Buyer.Code,
                            s.Buyer.Name
                        },
                        Unit = new {
                            s.Unit.Id,
                            s.Unit.Code,
                            s.Unit.Name
                        },
                        s.IsPosted,
                        s.CreatedBy, 
                        s.LastModifiedUtc,

                        s.PRType,
                        s.SCId,
                        s.SCNo,
                        s.IsValidatedMD1,
                        s.IsValidatedMD2,
                        s.IsValidatedPurchasing,
                        s.IsValidated,
                        s.ValidatedMD1Date,
                        s.ValidatedMD2Date,
                        s.ValidatedPurchasingDate,
                        s.ValidatedDate,
                        s.SectionName
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

        [HttpGet("dynamic")]
        public IActionResult GetDynamic(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}", string select = "{}", string search = "[]")
        {
            try
            {
                var Data = facade.ReadDynamic(page, size, order, keyword, filter, select, search);

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
                var result = facade.ReadById(id);
                GarmentPurchaseRequestViewModel viewModel = mapper.Map<GarmentPurchaseRequestViewModel>(result);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                if (indexAcceptPdf < 0)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(viewModel);
                    return Ok(Result);
                }
                else
                {
                    identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                    identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    var stream = facade.GeneratePdf(viewModel);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.PRNo}.pdf"
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

        [HttpGet("by-rono/{rono}")]
        public IActionResult Get(string rono)
        {
            try
            {
                var result = facade.ReadByRONo(rono);
                GarmentPurchaseRequestViewModel viewModel = mapper.Map<GarmentPurchaseRequestViewModel>(result);
                if (viewModel == null)
                {
                    throw new Exception("Invalid rono");
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
        public async Task<IActionResult> Post([FromBody]GarmentPurchaseRequestViewModel ViewModel)
        {
            try
            {
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentPurchaseRequest>(ViewModel);

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
        public async Task<IActionResult> Put(int id, [FromBody]GarmentPurchaseRequestViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentPurchaseRequest>(ViewModel);

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
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.Delete(id, identityService.Username);
                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> PRPost([FromBody]List<long> listId)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.PRPost(listId, identityService.Username);

                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("unpost/{id}")]
        public async Task<IActionResult> PRUnpost([FromRoute]long id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.PRUnpost(id, identityService.Username);

                return Ok();
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
        public IActionResult GetByTags(string tags, string shipmentDateFrom, string shipmentDateTo)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                DateTimeOffset shipmentFrom;
                DateTimeOffset shipmentTo;
                if (!string.IsNullOrWhiteSpace(shipmentDateFrom) && !string.IsNullOrWhiteSpace(shipmentDateTo))
                {
                    if (!DateTimeOffset.TryParseExact(shipmentDateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out shipmentFrom) ||
                        !DateTimeOffset.TryParseExact(shipmentDateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out shipmentTo))
                    {
                        shipmentFrom = DateTimeOffset.MinValue;
                        shipmentTo = DateTimeOffset.MinValue;
                    }
                    else
                    {
                        shipmentFrom = new DateTimeOffset(shipmentFrom.DateTime, new TimeSpan(clientTimeZoneOffset, 0, 0));
                        shipmentTo = new DateTimeOffset(shipmentTo.DateTime, new TimeSpan(clientTimeZoneOffset, 0, 0));
                    }
                }

                var data = facade.ReadByTagsOptimized(tags, shipmentFrom, shipmentTo);
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
		public IActionResult BySupplier(string keyword = null, string Filter = "{}")
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

				var Data = facade.ReadName(keyword, Filter);

				var newData = mapper.Map<List<GarmentPurchaseRequestViewModel>>(Data);

				List<object> listData = new List<object>();
				listData.AddRange(
					newData.AsQueryable().Select(s => new
					{
						s.CreatedBy
					}).ToList()
				);

				return Ok(new
				{
					apiVersion = ApiVersion,
					statusCode = General.OK_STATUS_CODE,
					message = General.OK_MESSAGE,
					data = listData	
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

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> PRApprove([FromRoute]long id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.PRApprove(id, identityService.Username);

                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("unapprove/{id}")]
        public async Task<IActionResult> PRUnApprove([FromRoute]long id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.PRUnApprove(id, identityService.Username);

                return Ok();
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
        public async Task<IActionResult> Patch(long id, [FromBody]JsonPatchDocument<GarmentPurchaseRequest> jsonPatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await facade.Patch(id, jsonPatch, identityService.Username);

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
    }
}