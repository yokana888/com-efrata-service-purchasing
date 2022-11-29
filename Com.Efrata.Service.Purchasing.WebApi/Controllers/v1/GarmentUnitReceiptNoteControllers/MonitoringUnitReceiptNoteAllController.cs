using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.MonitoringUnitReceiptFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentUnitReceiptNoteControllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/unit-receipt-note-monitoring-all")]
	[Authorize]
	public class MonitoringUnitReceiptNoteAllController : Controller
	{
		private string ApiVersion = "1.0.0";
		private readonly IMonitoringUnitReceiptAllFacade monitoringUnitReceiptAllFacade;
		public readonly IServiceProvider serviceProvider;
		public MonitoringUnitReceiptNoteAllController(IServiceProvider serviceProvider, IMonitoringUnitReceiptAllFacade monitoringUnitReceiptAllFacade)
		{
			this.monitoringUnitReceiptAllFacade = monitoringUnitReceiptAllFacade;
			this.serviceProvider = serviceProvider;
		}

		[HttpGet]
		public IActionResult Get(string no, string refNo, string roNo, string doNo, string unit, string supplier, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
		{
			try
			{
				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var data = monitoringUnitReceiptAllFacade.GetReport(no, refNo, roNo, doNo, unit, supplier, dateFrom, dateTo,page,size,Order,offset);
				
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
		public IActionResult GetXls(string no, string refNo, string roNo, string doNo, string unit, string supplier, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
		{
			try
			{
				byte[] xlsInBytes;
				int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				var xls = monitoringUnitReceiptAllFacade.GenerateExcel(no, refNo, roNo, doNo, unit, supplier, dateFrom, dateTo, page, size, Order, offset);

				string filename = "Laporan Bon Terima Unit ALL";
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