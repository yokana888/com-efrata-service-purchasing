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

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/pr-master-validation-report")]
	[Authorize]
	public class PRMasterValidationReportController : Controller
	{
		private string ApiVersion = "1.0.0";
		public readonly IServiceProvider serviceProvider;
		private readonly IPRMasterValidationReportFacade facade;
		private readonly IdentityService identityService;

		public PRMasterValidationReportController(IServiceProvider serviceProvider, IPRMasterValidationReportFacade facade)
		{
			this.serviceProvider = serviceProvider;
			this.facade = facade;
			this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
		}
		[HttpGet]
		public IActionResult GetReport(string unit, string sectionName, DateTime? dateFrom, DateTime? dateTo, string Order = "{}")
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{

				var data = facade.GetDisplayReport(unit, sectionName, dateFrom, dateTo, Order, offset);

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
	
		[HttpGet("download")]
		public IActionResult GetXls(string unit, string sectionName, DateTime? dateFrom, DateTime? dateTo)
		{
			try
			{
				byte[] xlsInBytes;

				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = facade.GenerateExcel(unit, sectionName, dateFrom, dateTo, offset);

				string filename = "Monitoring PR Master Validation";
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