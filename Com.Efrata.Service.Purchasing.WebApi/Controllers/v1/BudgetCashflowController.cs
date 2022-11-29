using Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService;
using Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.ExcelGenerator;
using Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService.PdfGenerator;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/budget-cashflows")]
    [Authorize]
    public class BudgetCashflowController : Controller
    {
        private readonly IBudgetCashflowService _service;
        private readonly IdentityService _identityService;
        private readonly IValidateService _validateService;
        private readonly IBudgetCashflowUnitPdf _budgetCashflowUnitPdf;
        private readonly IBudgetCashflowDivisionPdf _budgetCashflowDivisionPdf;
        private readonly IBudgetCashflowUnitExcelGenerator _budgetCashflowUnitExcelGenerator;
        private readonly IBudgetCashflowDivisionExcelGenerator _budgetCashflowDivisionExcelGenerator;
        private const string ApiVersion = "1.0";

        public BudgetCashflowController(IServiceProvider serviceProvider)
        {
            _service = serviceProvider.GetService<IBudgetCashflowService>();
            _identityService = serviceProvider.GetService<IdentityService>();
            _validateService = serviceProvider.GetService<IValidateService>();
            _budgetCashflowUnitPdf = serviceProvider.GetService<IBudgetCashflowUnitPdf>();
            _budgetCashflowDivisionPdf = serviceProvider.GetService<IBudgetCashflowDivisionPdf>();
            _budgetCashflowUnitExcelGenerator = serviceProvider.GetService<IBudgetCashflowUnitExcelGenerator>();
            _budgetCashflowDivisionExcelGenerator = serviceProvider.GetService<IBudgetCashflowDivisionExcelGenerator>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet("best-case")]
        public IActionResult GetBudgetCashflowBestCase([FromQuery] BudgetCashflowCategoryLayoutOrder layoutOrder, [FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetBudgetCashflowUnit(layoutOrder, unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/by-category")]
        public IActionResult GetBudgetCashflowBestCaseByCategory([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate, [FromQuery] int divisionId, [FromQuery] bool isImport, [FromQuery] string categoryIds = "[]")
        {

            try
            {
                VerifyUser();
                var parsedCategoryIds = JsonConvert.DeserializeObject<List<int>>(categoryIds);
                var result = _service.GetBudgetCashflowByCategoryAndUnitId(parsedCategoryIds, unitId, dueDate, divisionId, isImport);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-in-operational")]
        public IActionResult GetCashInOperational([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInOperatingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-out-operational")]
        public IActionResult GetCashOutOperational([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutOperatingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/diff-operational")]
        public IActionResult GetDiffOperational([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffOperatingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-in-investment")]
        public IActionResult GetCashInInvestment([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInInvestingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-out-investment")]
        public IActionResult GetCashOutInvestment([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutInvestingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/diff-investment")]
        public IActionResult GetDiffInvestment([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffInvestingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-in-financial")]
        public IActionResult GetCashInFinancial([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInFinancingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/cash-out-financial")]
        public IActionResult GetCashOutFinancial([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutFinancingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("best-case/diff-financial")]
        public IActionResult GetDiffFinancial([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffFinancingActivitiesByUnit(unitId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("worst-case")]
        public IActionResult GetBudgetCashflowWorstCase([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetBudgetCashflowWorstCase(dueDate, unitId);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("unit/xls")]
        public IActionResult GenerateExcel([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var stream = _budgetCashflowUnitExcelGenerator.Generate(unitId, dueDate);

                var filename = "Laporan Budget Cash Flow Unit";
                filename += ".xls";

                var bytes = stream.ToArray();

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("unit/pdf")]
        public IActionResult GeneratePdf([FromQuery] int unitId, [FromQuery] DateTimeOffset dueDate)
        {
            try
            {
                VerifyUser();
                var stream = _budgetCashflowUnitPdf.Generate(unitId, dueDate);

                var filename = "Laporan Budget Cashflow Unit";
                filename += ".pdf";

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }
        
        [HttpGet("division/xls")]
        public IActionResult GenerateExcelDivision([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var stream = _budgetCashflowDivisionExcelGenerator.Generate(divisionId, dueDate);

                var isAll = divisionId | 0;

                var filename = "Laporan Budget Cashflow Semua Divisi";
                if (isAll != 0)
                    filename = "Laporan Budget Cashflow Divisi";
                filename += ".xls";

                var bytes = stream.ToArray();

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/pdf")]
        public IActionResult GeneratePdfDivision([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {
            try
            {
                VerifyUser();
                var stream = _budgetCashflowDivisionPdf.Generate(divisionId, dueDate);

                var isAll = divisionId | 0;

                var filename = "Laporan Budget Cashflow Semua Divisi";
                if(isAll != 0)
                    filename = "Laporan Budget Cashflow Divisi";
                filename += ".pdf";

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpPut("worst-case")]
        public async Task<IActionResult> Put([FromBody] WorstCaseBudgetCashflowFormDto form)
        {
            try
            {
                VerifyUser();
                _validateService.Validate(form);

                var result = await _service.UpsertWorstCaseBudgetCashflowUnit(form);

                var response = new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE).Ok();
                return NoContent();
            }
            catch (ServiceValidationExeption e)
            {
                var response = new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE).Fail(e);
                return BadRequest(response);
            }
            catch (Exception e)
            {
                var response = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message).Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, response);
            }
        }

        [HttpGet("division")]
        public IActionResult GetBudgetCashflowByDivision([FromQuery] BudgetCashflowCategoryLayoutOrder layoutOrder, [FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetBudgetCashflowDivision(layoutOrder, divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-in-operational")]
        public IActionResult GetDivisionCashInOperational([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInOperatingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-out-operational")]
        public IActionResult GetDivisionCashOutOperational([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutOperatingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/diff-operational")]
        public IActionResult GetDivisionDiffOperational([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffOperatingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-in-investment")]
        public IActionResult GetDivisionCashInInvestment([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInInvestingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-out-investment")]
        public IActionResult GetDivisionCashOutInvestment([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutInvestingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/diff-investment")]
        public IActionResult GetDivisionDiffInvestment([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffInvestingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-in-financial")]
        public IActionResult GetDivisionCashInFinancial([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashInFinancingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/cash-out-financial")]
        public IActionResult GetDivisionCashOutFinancial([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetCashOutFinancingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

        [HttpGet("division/diff-financial")]
        public IActionResult GetDivisionDiffFinancial([FromQuery] int divisionId, [FromQuery] DateTimeOffset dueDate)
        {

            try
            {
                VerifyUser();
                var result = _service.GetDiffFinancingActivitiesByUnit(divisionId, dueDate);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, e.Message + " " + e.StackTrace);
            }
        }

    }
}
