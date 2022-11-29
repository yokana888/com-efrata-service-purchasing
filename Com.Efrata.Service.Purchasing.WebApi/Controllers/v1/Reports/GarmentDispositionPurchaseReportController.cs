using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentDispositionPurchaseFacades;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.Reports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/reports/garment-disposition-purchase")]
    [Authorize]
    public class GarmentDispositionPurchaseReportController : Controller
    {
        private readonly IGarmentDispositionPurchaseFacade _service;
        private readonly IdentityService _identityService;
        private const string ApiVersion = "1.0";

        public GarmentDispositionPurchaseReportController(IServiceProvider serviceProvider)
        {
            _service = serviceProvider.GetService<IGarmentDispositionPurchaseFacade>();
            _identityService = serviceProvider.GetService<IdentityService>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string username, [FromQuery] int supplierId, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo, [FromQuery] int page, [FromQuery] int size)
        {
            try
            {
                VerifyUser();
                _identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = _service.GetReport(supplierId, username, dateFrom, dateTo, size, page);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
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

        [HttpGet("list-user")]
        public async Task<IActionResult> GetUser([FromQuery] string keyword)
        {
            try
            {
                VerifyUser();
                _identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = await _service.GetListUsers(keyword);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data);
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

        [HttpGet("xlsx")]
        public IActionResult DownloadExcel([FromQuery] string username, [FromQuery] int supplierId, [FromQuery] string supplierName, [FromQuery] DateTimeOffset? dateFrom, [FromQuery] DateTimeOffset? dateTo)
        {

            try
            {
                VerifyUser();

                var Data =  _service.GetReport(supplierId, username, dateFrom, dateTo, 0, 0);


                var stream = GenerateExcel(Data.Data, _identityService.TimezoneOffset, username,supplierName,dateFrom,dateTo);

                var filename = "Laporan Disposisi Pembayaran.xlsx";

                var bytes = stream.ToArray();

                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception e)
            {
                var result = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message).Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, result);
            }
        }

        private MemoryStream GenerateExcel(List<DispositionPurchaseReportTableDto> data, int timezoneOffset, string username, string supplierName, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {

            var title = "LAPORAN DISPOSISI PEMBAYARAN";

            const int headerRow = 1;
            const int startingRow = 6;
            const int tableGap = 3;
            const int columnA = 1;
            const int columnB = 2;
            const int columnC = 3;
            const int columnD = 4;
            const int columnE = 5;

            int row = 1;
            int col = 1;
            int maxCol = 12;


            using (var package = new ExcelPackage())
            {
                #region headerExcel
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells[row,col].Value = title;
                worksheet.Cells[row,col,row,maxCol].Merge = true;
                worksheet.Cells[row,col,row,maxCol].Style.Font.Size = 20;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Bold = true;
                row++;

                row++;
                worksheet.Cells[row, col].Value = "filter";
                worksheet.Cells[row, col, row, maxCol].Merge = true;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Size = 14;
                row++;

                worksheet.Cells[row, col].Value = "Supplier : "+supplierName;
                worksheet.Cells[row, col, row, maxCol].Merge = true;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Size = 14;
                row++;

                worksheet.Cells[row, col].Value = "Staff Pembelian : " + username;
                worksheet.Cells[row, col, row, maxCol].Merge = true;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Size = 14;
                row++;

                worksheet.Cells[row, col].Value = "Tanggal Awal Disposisi : " + (dateFrom.HasValue? dateFrom.GetValueOrDefault().ToString("dd MMM yyyy"):"");
                worksheet.Cells[row, col, row, maxCol].Merge = true;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Size = 14;
                row++;

                worksheet.Cells[row, col].Value = "Tanggal Akhir Disposisi : " + (dateTo.HasValue? dateTo.GetValueOrDefault().ToString("dd MMM yyyy"):"");
                worksheet.Cells[row, col, row, maxCol].Merge = true;
                worksheet.Cells[row, col, row, maxCol].Style.Font.Size = 14;
                row++;
                #endregion

                #region headerTable
                row++;
                int rowSpan = 1;
                int colSpan = 1;
                worksheet.Cells[row, col].Value = "No.";
                worksheet.Cells[row, col, row+rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row+rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Staff Pembelian";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Disposisi";
                worksheet.Cells[row, col, row, col+colSpan].Merge = true;
                worksheet.Cells[row, col, row, col+colSpan].Style.Font.Size = 12;
                worksheet.Cells[row, col, row, col+colSpan].Style.Font.Bold = true;
                //col+=2;
                row++;

                worksheet.Cells[row, col].Value = "Nomor";
                worksheet.Cells[row, col].Style.Font.Size = 12;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Tanggal";
                worksheet.Cells[row, col].Style.Font.Size = 12;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                col++;
                row--;

                worksheet.Cells[row, col].Value = "Supplier";
                worksheet.Cells[row, col, row, col + colSpan].Merge = true;
                worksheet.Cells[row, col, row, col + colSpan].Style.Font.Size = 12;
                worksheet.Cells[row, col, row, col + colSpan].Style.Font.Bold = true;
                //col += 2;
                row++;

                worksheet.Cells[row, col].Value = "Kode";
                worksheet.Cells[row, col].Style.Font.Size = 12;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Nama";
                worksheet.Cells[row, col].Style.Font.Size = 12;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                col++;
                row--;

                worksheet.Cells[row, col].Value = "Kategori";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Tipe Bayar";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "No Proforma/Invoice";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Tanggal Jatuh Tempo";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Mata Uang";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;

                worksheet.Cells[row, col].Value = "Nominal";
                worksheet.Cells[row, col, row + rowSpan, col].Merge = true;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Size = 12;
                worksheet.Cells[row, col, row + rowSpan, col].Style.Font.Bold = true;
                col++;
                row += 2;
                #endregion
                #region DataBody
                col = 1;
                var dataTable = ConvertListDataToTable(data);
                worksheet.Cells[row, col].LoadFromDataTable(dataTable, false);
                #endregion
              
                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private DataTable ConvertListDataToTable(List<DispositionPurchaseReportTableDto> data)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("No",typeof(string)));
            dataTable.Columns.Add(new DataColumn("Staff Pembelian", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Nomor Disposisi", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Tanggal Disposisi", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Kode Supplier", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Nama Supplier", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Kategori", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Tipe Bayar", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Nomor Invoice", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Tanggal Jatuh Tempo", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Mata Uang", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Nominal", typeof(string)));

            var dataWithIndex = data.Select((item, index) => new { Index = index, Item = item }).ToList();

            foreach (var item in dataWithIndex)
            {
                dataTable.Rows.Add(
                    item.Index.ToString(),
                    item.Item.StaffName,
                    item.Item.DispositionNo,
                    item.Item.DispositionDate.ToString("dd/MM/yyyy"),
                    item.Item.SupplierCode,
                    item.Item.SupplierName,
                    item.Item.Category,
                    item.Item.PaymentType,
                    item.Item.InvoiceNo,
                    item.Item.DueDate.ToString("dd/MM/yyyy"),
                    item.Item.CurrencyCode,
                    item.Item.Nominal.ToString("N2")
                    );
            }

            return dataTable;
        }
    }
}
