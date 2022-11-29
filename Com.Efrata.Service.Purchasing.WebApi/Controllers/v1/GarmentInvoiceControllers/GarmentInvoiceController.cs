using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInvoiceControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-invoices")]
    [Authorize]
    public class GarmentInvoiceController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentInvoice facade;
		private readonly IGarmentDeliveryOrderFacade DOfacade;
		private readonly IdentityService identityService;
	 

		public GarmentInvoiceController(IServiceProvider serviceProvider, IMapper mapper, IGarmentInvoice facade, IGarmentDeliveryOrderFacade DOfacade)
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

				var viewModel = mapper.Map<List<GarmentInvoiceViewModel>>(Data.Item1);

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
		[HttpGet("pdf/income-tax/{id}")]
		public IActionResult GetIncomePDF(int id)
		{
			try
			{
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
				var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

				GarmentInvoice model = facade.ReadById(id);
				GarmentInvoiceViewModel viewModel = mapper.Map<GarmentInvoiceViewModel>(model);
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

                    /* tambahan */
                    /* get gsupplier */
                    string supplierUri = "master/garment-suppliers";
                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    var response = httpClient.GetAsync($"{Lib.Helpers.APIEndpoint.Core}{supplierUri}/{model.SupplierId}").Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                    SupplierViewModel supplier = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                    /* tambahan */

                    IncomeTaxPDFTemplate PdfTemplateLocal = new IncomeTaxPDFTemplate();
					MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel,supplier,clientTimeZoneOffset, DOfacade);

					return new FileStreamResult(stream, "application/pdf")
					{
						FileDownloadName = $"{viewModel.nph}.pdf"
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
		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			try
			{
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
				var model = facade.ReadById(id);
				var viewModel = mapper.Map<GarmentInvoiceViewModel>(model);
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
                        item.deliveryOrder.items = deliveryOrderViewModel.items;
						item.deliveryOrder.useIncomeTax = deliveryOrderViewModel.useIncomeTax;
						item.deliveryOrder.useVat = deliveryOrderViewModel.useVat;
						item.deliveryOrder.supplier = deliveryOrderViewModel.supplier;
						item.deliveryOrder.docurrency = deliveryOrderViewModel.docurrency;
						if(item.deliveryOrder.incomeTax !=null)
						{
							item.deliveryOrder.incomeTax.Id = (int)deliveryOrder.IncomeTaxId;
							item.deliveryOrder.incomeTax.Name = deliveryOrder.IncomeTaxName;
							item.deliveryOrder.incomeTax.Rate = (double)deliveryOrder.IncomeTaxRate ;
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
		[HttpGet("pdf/vat/{id}")]
		public IActionResult GetVatPDF(int id)
		{
			try
			{
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
				var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

				GarmentInvoice model = facade.ReadById(id);
				GarmentInvoiceViewModel viewModel = mapper.Map<GarmentInvoiceViewModel>(model);
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

                    /* tambahan */
                    /* get gsupplier */
                    string supplierUri = "master/garment-suppliers";
                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    var response = httpClient.GetAsync($"{Lib.Helpers.APIEndpoint.Core}{supplierUri}/{model.SupplierId}").Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                    SupplierViewModel supplier = JsonConvert.DeserializeObject<SupplierViewModel>(result.GetValueOrDefault("data").ToString());
                    /* tambahan */

                    VatPDFTemplate PdfTemplateLocal = new VatPDFTemplate();
					MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel,supplier,clientTimeZoneOffset, DOfacade);

					return new FileStreamResult(stream, "application/pdf")
					{
						FileDownloadName = $"{viewModel.npn}.pdf"
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
		public async Task<IActionResult> Post([FromBody]GarmentInvoiceViewModel ViewModel)
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(ViewModel);

				var model = mapper.Map<GarmentInvoice>(ViewModel);

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
				return StatusCode(General.BAD_REQUEST_STATUS_CODE, Result);
				//return BadRequest(Result);
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
		public async Task<IActionResult> Put(int id, [FromBody]GarmentInvoiceViewModel ViewModel)
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(ViewModel);

				var model = mapper.Map<GarmentInvoice>(ViewModel);

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

		 
	}
}