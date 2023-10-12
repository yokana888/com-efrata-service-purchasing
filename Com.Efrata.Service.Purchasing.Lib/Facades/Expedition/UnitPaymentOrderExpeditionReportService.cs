using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.Expedition
{
    public class UnitPaymentOrderExpeditionReportService : IUnitPaymentOrderExpeditionReportService
    {
        private readonly PurchasingDbContext _dbContext;

        public UnitPaymentOrderExpeditionReportService(PurchasingDbContext dbContext)
        {
            _dbContext = dbContext;
            DateTime dt = DateTime.Parse("0001-01-01T00:00:00.0000000+00:00");
        }

        public IQueryable<UnitPaymentOrderExpeditionReportViewModel> GetQuery(string no, string supplierCode, string divisionCode, int status, DateTimeOffset dateFrom, DateTimeOffset dateTo, string order)
        {
            var expeditionDocumentQuery = _dbContext.Set<PurchasingDocumentExpedition>().AsQueryable();
            var query = _dbContext.Set<UnitPaymentOrder>().AsQueryable();
            var externalPurchaseOrderQuery = _dbContext.Set<ExternalPurchaseOrder>().AsQueryable();
            var UPOItemQuery = _dbContext.Set<UnitPaymentOrderItem>().AsQueryable();
            var UPODetailQuery = _dbContext.Set<UnitPaymentOrderDetail>().AsQueryable();

            DateTime dt = DateTime.Parse("0001-01-01T00:00:00.0000000+00:00");

            DateTimeOffset DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTimeOffset)dateFrom;
            DateTimeOffset DateTo = dateTo == null ? DateTimeOffset.Now : (DateTimeOffset)dateTo;

            if (!string.IsNullOrWhiteSpace(no))
            {
                query = query.Where(document => document.UPONo.Equals(no));
            }

            if (!string.IsNullOrWhiteSpace(supplierCode))
            {
                query = query.Where(document => document.SupplierCode.Equals(supplierCode));
            }

            if (!string.IsNullOrWhiteSpace(divisionCode))
            {
                query = query.Where(document => document.DivisionCode.Equals(divisionCode));
            }

            //if (status != 0)
            //{
            //    query = query.Where(document => document.Position.Equals(status));
            //}


            query = query.Where(document => document.Date >= DateFrom && document.Date <= DateTo);


            var joinedQuery = query.GroupJoin(
                expeditionDocumentQuery,
                unitPaymentOrder => unitPaymentOrder.UPONo,
                expeditionDocument => expeditionDocument.UnitPaymentOrderNo,
                (unitPaymentOrder, expeditionDocuments) => new { UnitPaymentOrder = unitPaymentOrder, ExpeditionDocuments = expeditionDocuments })

                .SelectMany(
                    joined => joined.ExpeditionDocuments,
                    (joinResult, expeditionDocument) => new UnitPaymentOrderExpeditionReportViewModel()
                    {
                        SendToVerificationDivisionDate = expeditionDocument.SendToVerificationDivisionDate != null ? expeditionDocument.SendToVerificationDivisionDate : null,
                        VerificationDivisionDate = expeditionDocument.VerificationDivisionDate != null ? expeditionDocument.VerificationDivisionDate : null,
                        VerifyDate = expeditionDocument.VerifyDate != null ? expeditionDocument.VerifyDate : null,
                        SendDate = (expeditionDocument.Position == ExpeditionPosition.CASHIER_DIVISION || expeditionDocument.Position == ExpeditionPosition.SEND_TO_CASHIER_DIVISION) ? expeditionDocument.SendToCashierDivisionDate : (expeditionDocument.Position == ExpeditionPosition.FINANCE_DIVISION || expeditionDocument.Position == ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION) ? expeditionDocument.SendToAccountingDivisionDate : (expeditionDocument.Position == ExpeditionPosition.SEND_TO_PURCHASING_DIVISION) ? expeditionDocument.SendToPurchasingDivisionDate : null,
                        CashierDivisionDate = expeditionDocument.CashierDivisionDate != null ? expeditionDocument.CashierDivisionDate : null,
                        BankExpenditureNoteNo = expeditionDocument.BankExpenditureNoteNo != null ? expeditionDocument.BankExpenditureNoteNo : "-",
                        Date = expeditionDocument.UPODate,
                        DueDate = expeditionDocument.DueDate,
                        InvoiceNo = expeditionDocument.InvoiceNo != null ? expeditionDocument.InvoiceNo : "-",
                        No = expeditionDocument.UnitPaymentOrderNo != null ? expeditionDocument.UnitPaymentOrderNo : "-",
                        Position = expeditionDocument.Position,
                        DPP = expeditionDocument.TotalPaid - expeditionDocument.Vat,
                        PPn = expeditionDocument.Vat,
                        PPh = expeditionDocument.IncomeTax,
                        TotalTax = expeditionDocument.TotalPaid,
                        Supplier = new NewSupplierViewModel()
                        {
                            code = expeditionDocument.SupplierCode != null ? expeditionDocument.SupplierCode : "-",
                            name = expeditionDocument.SupplierName != null ? expeditionDocument.SupplierName : "-",
                        },
                        Currency = expeditionDocument.Currency,

                        Category = new CategoryViewModel()
                        {
                            Code = expeditionDocument.CategoryCode != null ? expeditionDocument.CategoryCode : "-",
                            Name = expeditionDocument.CategoryName != null ? expeditionDocument.CategoryName : "-"
                        },
                        Unit = new UnitViewModel()
                        {
                            Code = expeditionDocument.Items.FirstOrDefault().UnitCode != null ? expeditionDocument.Items.FirstOrDefault().UnitCode : "-",
                            Name = expeditionDocument.Items.FirstOrDefault().UnitName != null ? expeditionDocument.Items.FirstOrDefault().UnitName : "-"
                        },
                        Division = new DivisionViewModel()
                        {
                            Code = expeditionDocument.DivisionCode != null ? expeditionDocument.DivisionCode : "-",
                            Name = expeditionDocument.DivisionName != null ? expeditionDocument.DivisionName : "-"
                        },

                        VerifiedBy = expeditionDocument.VerificationDivisionBy != null ? expeditionDocument.VerificationDivisionBy : "-",
                        CreatedBy = expeditionDocument.CreatedBy != null ? expeditionDocument.CreatedBy : "-",
                        PaymentMethod = expeditionDocument.PaymentMethod != null ? expeditionDocument.PaymentMethod : "-",
                        PaymentNominal = expeditionDocument.SupplierPayment,
                        PaymentDifference = (expeditionDocument.TotalPaid - expeditionDocument.IncomeTax) - (expeditionDocument.AmountPaid + expeditionDocument.SupplierPayment)
                    }
                );//.Where(document => status == 0 ? document.Position.Equals(FormatPosition(status)): document.Position.Equals(FormatPosition(status)));


            //if (!string.IsNullOrWhiteSpace(no))
            //{
            //    joinedQuery = joinedQuery.Where(document => document.No.Equals(no));
            //}

            //if (!string.IsNullOrWhiteSpace(supplierCode))
            //{
            //    joinedQuery = joinedQuery.Where(document => document.Supplier.code.Equals(supplierCode));
            //}

            //if (!string.IsNullOrWhiteSpace(divisionCode))
            //{
            //   joinedQuery  = joinedQuery.Where(document => document.Division.Code.Equals(divisionCode));
            //}

            if (status != 0)
            {
                joinedQuery = joinedQuery.Where(document => document.Position.Equals(FormatPosition(status)));
            }


            //joinedQuery = joinedQuery.Where(document => document.Date >= DateFrom && document.Date <= DateTo);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            /* Default Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("Date", "desc");

                joinedQuery = joinedQuery.OrderBy("Date desc");
               
            }
            /* Custom Order */
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                joinedQuery = joinedQuery.OrderBy(string.Concat(Key, " ", OrderType));
            }

            return joinedQuery;
        }

        public async Task<UnitPaymentOrderExpeditionReportWrapper> GetReport(string no, string supplierCode, string divisionCode, int status, DateTimeOffset dateFrom, DateTimeOffset dateTo, string order, int page, int size)
        {
            var joinedQuery = GetQuery(no, supplierCode, divisionCode, status, dateFrom, dateTo, order);

            return new UnitPaymentOrderExpeditionReportWrapper()
            {
                Total = await joinedQuery.CountAsync(),
                Data = await joinedQuery
                .Skip((page - 1) * size)
                .Take(size).Distinct()
                .ToListAsync()
            };
        }

        public async Task<MemoryStream> GetExcel(string no, string supplierCode, string divisionCode, int status, DateTimeOffset dateFrom, DateTimeOffset dateTo, string order)
        {
            var query = GetQuery(no, supplierCode, divisionCode, status, dateFrom, dateTo, order);

            var data = new List<UnitPaymentOrderExpeditionReportViewModel> { new UnitPaymentOrderExpeditionReportViewModel { Supplier = new NewSupplierViewModel(), Division = new DivisionViewModel() } };
            var listData = await query.Distinct().ToListAsync();
            if (listData != null && listData.Count > 0)
            {
                data = listData;
            }

            var headers = new string[] { "No. SPB", "Tgl SPB", "Tgl Jatuh Tempo", "Nomor Invoice", "Supplier", "Term Pembayaran", "Kurs", "Jumlah", "Jumlah1", "Jumlah2", "Jumlah3", "Tempo", "Kategori", "Unit", "Divisi", "Posisi", "Tgl Pembelian Kirim", "Admin", "Verifikasi", "Verifikasi1", "Verifikasi2", "Kasir", "Kasir1", "Kasir2", "Sisa yang Belum Dibayar" };
            var subHeaders = new string[] { "DPP", "PPn", "PPh", "Total", "Tgl Terima", "Tgl Cek", "Tgl Kirim", "Tgl Terima", "No Kuitansi", "Nominal Pembayaran" };

            DataTable dataTable = new DataTable();

            //var headersDateType = new int[] { 1, 2, 15, 17, 18, 19, 20 };
            var headersDateType = new int[] { 1, 2, 16, 18, 19, 20, 21 };
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                if (headersDateType.Contains(i))
                {
                    dataTable.Columns.Add(new DataColumn() { ColumnName = header, DataType = typeof(DateTime) });
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn() { ColumnName = header, DataType = typeof(string) });
                }
            }

            int cells = 7; // initial start body data
            foreach (var d in data)
            {
                decimal selisih = d.DueDate != null && d.Date != null ? ((d.DueDate.Value) - (d.Date.Value)).Days : 0;

                dataTable.Rows.Add(d.No ?? "-", GetFormattedDate(d.Date), GetFormattedDate(d.DueDate), d.InvoiceNo ?? "-", d.Supplier.name ?? "-", d.PaymentMethod ?? "-",
                    d.Currency ?? "-", d.DPP.ToString("#,##0.#0"), d.PPn.ToString("#,##0.#0"), d.PPh.ToString("#,##0.#0"), d.TotalTax.ToString("#,##0.#0"), Math.Abs(Math.Ceiling(selisih)), d.Category.Name ?? "-", d.Unit.Name ?? "-", d.Division.Name ?? "-", GetFormattedPosition(d.Position),
                    GetFormattedDate(d.SendToVerificationDivisionDate),
                    d.CreatedBy ?? "-",
                    GetFormattedDate(d.VerificationDivisionDate),
                    GetFormattedDate(d.VerifyDate),
                    GetFormattedDate(d.SendDate),
                    GetFormattedDate(d.CashierDivisionDate),
                    d.BankExpenditureNoteNo ?? "-",
                    d.PaymentNominal.ToString("#,##0.#0"),
                    d.PaymentDifference.ToString("#,##0.#0"));
                    cells++;
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");
            string dateFromXls = dateFrom == DateTimeOffset.MinValue ? "-" : dateFrom.Date.ToString("dd MMMM yyyy"),
                dateToXls = dateFrom == DateTimeOffset.MinValue && dateTo.Date == DateTimeOffset.UtcNow.Date ? "-" : dateTo.Date.ToString("dd MMMM yyyy");

            sheet.Cells["A1"].Value = "PT.Efrata Garmindo Utama";
            sheet.Cells["A1:D1"].Merge = true;

            sheet.Cells["A2"].Value = "Laporan Expedisi Surat Perintah Bayar";
            sheet.Cells["A2:D2"].Merge = true;

            sheet.Cells["A3"].Value = $"PERIODE : {dateFromXls} sampai dengan {dateToXls}";
            sheet.Cells["A3:D3"].Merge = true;

            sheet.Cells["A7"].LoadFromDataTable(dataTable, false, OfficeOpenXml.Table.TableStyles.None);

            /*sheet.Cells["G5"].Value = headers[6];
            sheet.Cells["G5:J5"].Merge = true;
            sheet.Cells["R5"].Value = headers[17];
            sheet.Cells["R5:T5"].Merge = true;
            sheet.Cells["U5"].Value = headers[20];
            sheet.Cells["U5:V5"].Merge = true; */

            sheet.Cells["H5"].Value = headers[7];
            sheet.Cells["H5:K5"].Merge = true;
            sheet.Cells["S5"].Value = headers[18];
            sheet.Cells["S5:U5"].Merge = true;
            sheet.Cells["V5"].Value = headers[21];
            sheet.Cells["V5:X5"].Merge = true;

            foreach (var i in Enumerable.Range(0, 6))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}5"].Value = headers[i];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            /*foreach (var i in Enumerable.Range(0, 4))
            {
                var col = (char)('G' + i);
                sheet.Cells[$"{col}5"].Value = subHeaders[i];
            }

            foreach (var i in Enumerable.Range(0, 7))
            {
                var col = (char)('K' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 10];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 5))
            {
                var col = (char)('R' + i);
                sheet.Cells[$"{col}6"].Value = subHeaders[i + 4];
            }
            sheet.Cells["A5:V6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A5:V6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A5:V6"].Style.Font.Bold = true;
            sheet.Cells[$"G7:J{cells}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;*/

            foreach (var i in Enumerable.Range(0, 4))
            {
                var col = (char)('H' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 7];
                sheet.Cells[$"{col}6"].Value = subHeaders[i + 0];
            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                var col = (char)('G' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 6];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 7))
            {
                var col = (char)('L' + i);
                sheet.Cells[$"{col}5"].Value = headers[i + 11];
                sheet.Cells[$"{col}5:{col}6"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 6))
            {
                var col = (char)('S' + i);
                sheet.Cells[$"{col}6"].Value = subHeaders[i + 4];
            }

            sheet.Cells["Y5"].Value = "Sisa yang Belum Dibayar";
            sheet.Cells["Y5:Y6"].Merge = true;

            sheet.Cells["A5:Y6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A5:Y6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A5:Y6"].Style.Font.Bold = true;
            sheet.Cells[$"H7:K{cells}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            foreach (var headerDateType in headersDateType)
            {
                sheet.Column(headerDateType + 1).Style.Numberformat.Format = "dd MMMM yyyy";
            }

            var widths = new int[] { 20, 20, 20, 50, 30, 20, 10, 20, 20, 20, 20, 20, 30, 30, 20, 40, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
            foreach (var i in Enumerable.Range(0, widths.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        DateTime? GetFormattedDate(DateTimeOffset? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            else
            {
                return dateTime.Value.ToOffset(new TimeSpan(7, 0, 0)).DateTime;
            }
        }

        string GetFormattedPosition(ExpeditionPosition position) {
            string name;

            return name = position == ExpeditionPosition.PURCHASING_DIVISION ? "Bag. Pembelian": position == ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION ? "Di Kirim ke Bag. Verifikasi":
                          position == ExpeditionPosition.VERIFICATION_DIVISION ? "Bag. Verifikasi": position == ExpeditionPosition.SEND_TO_CASHIER_DIVISION ? "Di Kirim ke Bag. Keuangan": 
                          position == ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION ? "Di Kirim ke Bag. Accounting":position == ExpeditionPosition.SEND_TO_PURCHASING_DIVISION ? "Di Kirim ke Bag. Pembelian" :
                          position == ExpeditionPosition.CASHIER_DIVISION ? "Bag. Kasir": position == ExpeditionPosition.FINANCE_DIVISION ? "Bag. Keuangan"
                          : "-";
        }

        ExpeditionPosition FormatPosition(int num) {
            ExpeditionPosition posisi;

            return
            posisi = num == 1 ? ExpeditionPosition.PURCHASING_DIVISION :
                     num == 2 ? ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION :
                     num == 3 ? ExpeditionPosition.VERIFICATION_DIVISION :
                     num == 4 ? ExpeditionPosition.SEND_TO_CASHIER_DIVISION :
                     num == 5 ? ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION :
                     num == 6 ? ExpeditionPosition.SEND_TO_PURCHASING_DIVISION:
                     num == 7 ? ExpeditionPosition.CASHIER_DIVISION :
                     num == 8 ? ExpeditionPosition.FINANCE_DIVISION :
                     ExpeditionPosition.INVALID;

        }


    }
}
