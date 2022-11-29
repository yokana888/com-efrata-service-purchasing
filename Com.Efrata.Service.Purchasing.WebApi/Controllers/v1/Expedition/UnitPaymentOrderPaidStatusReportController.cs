using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.Facades.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/expedition/unit-payment-order-paid-status-report")]
    [Authorize]
    public class UnitPaymentOrderPaidStatusReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IUnitPaymentOrderPaidStatusReportFacade unitPaymentOrderPaidStatusReportFacade;

        public UnitPaymentOrderPaidStatusReportController(IUnitPaymentOrderPaidStatusReportFacade unitPaymentOrderPaidStatusReportFacade)
        {
            this.unitPaymentOrderPaidStatusReportFacade = unitPaymentOrderPaidStatusReportFacade;
        }

        [HttpGet]
        public ActionResult Get(int Size, int Page, string Order, string UnitPaymentOrderNo, string SupplierCode, string DivisionCode, string SupplierType, string PaymentMethod, string Status, DateTimeOffset? DateFromDue, DateTimeOffset? DateToDue, DateTimeOffset? DateFrom, DateTimeOffset? DateTo)
        {
            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

            ReadResponse<object> response = this.unitPaymentOrderPaidStatusReportFacade.GetReport(Size, Page, Order, UnitPaymentOrderNo, SupplierCode, DivisionCode, SupplierType, PaymentMethod, Status, DateFromDue, DateToDue, DateFrom, DateTo, clientTimeZoneOffset);

            return Ok(new
            {
                apiVersion = ApiVersion,
                data = response.Data,
                info = new Dictionary<string, object>
                {
                    { "count", response.Data.Count },
                    { "total", response.TotalData },
                    { "order", response.Order },
                    { "page", Page },
                    { "size", Size }
                },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }
    }
}