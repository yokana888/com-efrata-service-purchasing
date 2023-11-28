using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentPurchaseRequestControllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/garment-purchase-orders/monitoring")]
	[Authorize]
	public class GarmentMonitoringPurchaseController : Controller
	{
		private string ApiVersion = "1.0.0";
		public readonly IServiceProvider serviceProvider;
		//private readonly IMapper mapper;
		private readonly IGarmentPurchaseRequestFacade facade;
		private readonly IdentityService identityService;

		public GarmentMonitoringPurchaseController(IServiceProvider serviceProvider, IGarmentPurchaseRequestFacade facade)
		{
			this.serviceProvider = serviceProvider;
			this.facade = facade;
			this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
		}
		[HttpGet]
		public async Task<IActionResult> GetReport(string epono, string unit,string roNo, string article,string poSerialNumber,string username,string doNo,string ipoStatus,string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order = "{}")
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{

				var data = await facade.GetMonitoringPurchaseReport(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, page, size, Order, offset);

				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data,
					//info = new { total = data.Item2 },
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
		[HttpGet("by-user")]
		public async Task<IActionResult> GetReportByUser(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFromEx, DateTime? dateToEx, int page, int size, string Order = "{}")
		{
			identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
			username = identityService.Username;

			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{

				var result = await facade.GetMonitoringPurchaseReport(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, page, size, Order, offset);

				return Ok(new
				{
					apiVersion = ApiVersion,
					data = result,
					 
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
		[HttpGet("download")]
		public async Task<IActionResult> GetXls(string epono, string unit,string roNo, string article,string poSerialNumber,string username,string doNo,string ipoStatus,string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order = "{}")
		{
			try
			{
				byte[] xlsInBytes;

				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = await facade.GenerateExcelPurchase(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, page, size, Order, offset);

				string filename = "Laporan Pembelian Garment";
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
		[HttpGet("by-user/download")]
		public async Task<IActionResult> GetXlsByUser(string epono, string unit, string roNo, string article, string poSerialNumber, string username, string doNo, string ipoStatus, string supplier, string status, DateTime? dateFrom, DateTime? dateTo, DateTimeOffset? dateFromEx, DateTimeOffset? dateToEx, int page, int size, string Order = "{}")
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				username = identityService.Username;

				byte[] xlsInBytes;

				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = await facade.GenerateExcelPurchase(epono, unit, roNo, article, poSerialNumber, username, doNo, ipoStatus, supplier, status, dateFrom, dateTo, dateFromEx, dateToEx, page, size, Order, offset);

				string filename = "Laporan Pembelian Garment - " + username;
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
	}
}