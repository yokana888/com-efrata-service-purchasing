using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using System.IO;
using System.Globalization;
using OfficeOpenXml;
using System.Data;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class DebtCardReportFacade : IDebtCardReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        ILocalDbCashFlowDbContext dbContextLocal;
        public readonly IServiceProvider serviceProvider;
        public DebtCardReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext, ILocalDbCashFlowDbContext dbContextLocal)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbContextLocal = dbContextLocal;
        }
        public List<GarmentDebtCardReportViewModel> getQuery(int month, int year, string suppliercode, string suppliername, string currencyCode, string paymentMethod, int offset)
        {
            List<GarmentDebtCardReportViewModel> report = new List<GarmentDebtCardReportViewModel>();
            var date = new DateTime(year, month, DateTime.MinValue.Day);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var saldoawaldebt = (from a in dbContext.GarmentSupplierBalanceDebts
                                 join b in dbContext.GarmentSupplierBalanceDebtItems on a.Id equals b.GarmentDebtId
                                 where a.SupplierCode == suppliercode
                                 && a.DOCurrencyCode == currencyCode
                                 && a.Year == year - 1
                                 && b.PaymentMethod == (string.IsNullOrWhiteSpace(paymentMethod) ? b.PaymentMethod : paymentMethod)
                                 && a.IsDeleted == false && b.IsDeleted == false
                                 select new GarmentDebtCardReportViewModel
                                 {
                                     SupplierCode = a.SupplierCode,
                                     NoDebit = "Saldo Awal",
                                     CurrencyCodeEndingBalance = a.DOCurrencyCode,
                                     TotalEndingBalance = b.Valas,
                                     TotalIDREndingBalance = b.IDR
                                 });
            var SaldoAwalSeblmBulan = (from a in dbContext.GarmentDeliveryOrders
                                           //join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                       where a.ArrivalDate >= new DateTime(year, 1, 1)
                                       && a.ArrivalDate < date.Date
                                       && a.SupplierCode == suppliercode
                                       && a.DOCurrencyCode == currencyCode
                                       && a.IsDeleted == false
                                       && a.PaymentMethod == (string.IsNullOrWhiteSpace(paymentMethod) ? a.PaymentMethod : paymentMethod)
                                       && a.IsDeleted == false
                                       select new GarmentDebtCardReportViewModel
                                       {
                                           SupplierCode = a.SupplierCode,
                                           CurrencyCodeEndingBalance = a.DOCurrencyCode,
                                           NoDebit = "Saldo Awal",
                                           TotalEndingBalance = a.TotalAmount,
                                           TotalIDREndingBalance = a.TotalAmount * a.DOCurrencyRate
                                       }).Distinct();
            var SaldoBulan = from a in dbContext.GarmentDeliveryOrders
                             where a.ArrivalDate >= date.Date
                             && a.ArrivalDate <= endDate.Date
                             && a.SupplierCode == suppliercode
                             && a.DOCurrencyCode == currencyCode
                             && a.IsDeleted == false
                             && a.PaymentMethod == (string.IsNullOrWhiteSpace(paymentMethod) ? a.PaymentMethod : paymentMethod)
                             select new GarmentDebtCardReportViewModel
                             {
                                 SupplierCode = a.SupplierCode,
                                 CurrencyCodeKredit = a.DOCurrencyCode,
                                 CurrencyCodeEndingBalance = a.DOCurrencyCode,
                                 NoKredit = a.BillNo,
                                 DONo = a.DONo,
                                 SupplierName = a.SupplierName,
                                 DateKredit = a.ArrivalDate,
                                 TotalKredit = a.TotalAmount,
                                 TotalIDRKredit = a.TotalAmount * a.DOCurrencyRate,
                             };

            var SaldoAwal = saldoawaldebt.Union(SaldoAwalSeblmBulan).ToList();
            var SaldoAwal2 = (from a in SaldoAwal
                            group a by new { a.CurrencyCodeEndingBalance, a.SupplierCode } into groupdata
                            select new GarmentDebtCardReportViewModel
                            {
                                SupplierCode = groupdata.FirstOrDefault().SupplierCode,
                                CurrencyCodeEndingBalance = groupdata.FirstOrDefault().CurrencyCodeEndingBalance,
                                NoDebit = groupdata.FirstOrDefault().NoDebit,
                                TotalEndingBalance = groupdata.Sum(x => x.TotalEndingBalance),
                                TotalIDREndingBalance = groupdata.Sum(x => x.TotalIDREndingBalance)
                            }).SingleOrDefault();
            if (SaldoAwal2 != null) {
                report.Add(SaldoAwal2);
            }
            //report.Add(SaldoAwal2 != null ? SaldoAwal2 : new GarmentDebtCardReportViewModel { SupplierCode = "", CurrencyCodeEndingBalance = currencyCode, NoDebit = "SALDO AWAL", TotalEndingBalance = 0, TotalIDREndingBalance = 0 });
            //report.Add(SaldoAwal2);
            foreach (var item in SaldoBulan) {
                string cmddetail2 = "Select jumlah, rate, rate1, nomor, tgl from RincianDetil where no_bon = '" + item.NoKredit + "' and no_sj = '" + item.DONo + "' and ketr = '" + item.SupplierName + "'";
                var data = dbContextLocal.ExecuteReaderOnlyQuery(cmddetail2);
                while (data.Read())
                {
                    item.TotalDebit = (decimal)data["jumlah"];
                    item.TotalIDRDebit = (decimal)data["jumlah"] * (decimal)data["rate"];
                    item.NoDebit = data["nomor"].ToString();
                    item.DateDebit = data.GetDateTime(4);
                    item.Remark = item.NoKredit;
                    item.NoKredit = "";
                    item.CurrencyCodeDebit = item.CurrencyCodeKredit;
                    item.CurrencyCodeKredit = "";
                    item.DateKredit = null;
                    item.TotalKredit = 0;
                    item.TotalIDRKredit = 0;
                }
                report.Add(item);
            }
            var report2 = report.ToArray();
            if(report2.Length != 1)
            {
                for (var i = 0; i < report2.Length - 1; i++)
                {
                    var harga = report2[i].TotalEndingBalance + report2[i + 1].TotalKredit - Convert.ToDouble(report2[i + 1].TotalDebit);
                    var hargaIDR = report2[i].TotalIDREndingBalance + report2[i + 1].TotalIDRKredit - Convert.ToDouble(report2[i + 1].TotalIDRDebit);
                    report2[i + 1].TotalEndingBalance = harga;
                    report2[i + 1].TotalIDREndingBalance = hargaIDR;

                }
            }
            
            return report2.ToList();
        }
        public Tuple<List<GarmentDebtCardReportViewModel>, int> GetDebtCardReport(int month, int year, string suppliercode, string suppliername, string currencyCode, string paymentMethod, int offset)
        {
            List<GarmentDebtCardReportViewModel> result = getQuery(month, year, suppliercode, suppliername, currencyCode, paymentMethod, offset);
            return Tuple.Create(result, result.Count);
        }

        public MemoryStream GenerateExcelCardReport(int month, int year, string suppliercode, string suppliername, string currencyCode, string currencyName, string paymentMethod, int offset)
        {
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(month);
            var query = getQuery(month, year, suppliercode, suppliername, currencyCode, paymentMethod, offset);
            DataTable result = new DataTable();
            ExcelPackage package = new ExcelPackage();
            string kop1 = "PT. EFRATA GARMINDO UTAMA";
            string kop2 = string.Format("DETAIL REKAP SALDO HUTANG - SUPPLIER {0} ", suppliername);
            string kop3 = string.Format("Periode Bulan {0} {1}", Month, year);
            string kop4 = string.Format("Mata Uang {0}",  currencyName);
            var headers = new string[] { "Debit", "Debit1", "Debit2", "Debit3", "Debit4", "Debit5", "Kredit", "Kredit1", "Kredit2", "Kredit3", "Kredit4", "Saldo Akhir", "Saldo Akhir1", "Saldo Akhir2" };
            var subheaders = new string[] { "No Nota", "Tgl.Nota", "Total", "Mata Uang", "Total(IDR)", "Keterangan", "No Nota", "Tgl.Nota", "Total", "Mata Uang", "Total(IDR)", "Total", "Mata Uang", "Total(IDR)" };
            for (int i = 0; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });

            }
            if (query.Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
                var sheet = package.Workbook.Worksheets.Add("Data");
                sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);

            }
            else
            {
                foreach (GarmentDebtCardReportViewModel data in query) {
                    string DateDebit = data.DateDebit == new DateTime(1970, 1, 1) || data.DateDebit == null ? "" : data.DateDebit.Value.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string TotalDebit = data.TotalDebit == null ? "" : string.Format("{0:N2}", data.TotalDebit);
                    string TotalIDRDebit = data.TotalIDRDebit == null ? "" : string.Format("{0:N2}", data.TotalIDRDebit);
                    string TotalKredit = data.TotalKredit == null ? "" : string.Format("{0:N2}", data.TotalKredit);
                    string TotalIDRKredit = data.TotalIDRKredit == null ? "" : string.Format("{0:N2}", data.TotalIDRKredit);
                    string TotalEndingBalance = data.TotalEndingBalance == null ? "" : string.Format("{0:N2}", data.TotalEndingBalance);
                    string TotalIDREndingBalance = data.TotalIDREndingBalance == null ? "" : string.Format("{0:N2}", data.TotalIDREndingBalance);
                    string DateKredit = data.DateKredit == new DateTime(1970, 1, 1) || data.DateKredit == null ? "" : data.DateKredit.Value.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(data.NoDebit, DateDebit, TotalDebit, data.CurrencyCodeDebit, TotalIDRDebit, data.Remark, data.NoKredit, DateKredit, TotalKredit, data.CurrencyCodeKredit, TotalIDRKredit, TotalEndingBalance, data.CurrencyCodeEndingBalance, TotalIDREndingBalance);
                }
                var sheet = package.Workbook.Worksheets.Add("Data");
                var colkop = (char)('A' + headers.Length - 1);
                #region KopTable
                sheet.Cells[$"A1:{colkop}1"].Value = kop1;
                sheet.Cells[$"A1:{colkop}1"].Merge = true;
                sheet.Cells[$"A1:{colkop}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A1:{colkop}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A1:{colkop}1"].Style.Font.Bold = true;
                sheet.Cells[$"A2:{colkop}2"].Value = kop2;
                sheet.Cells[$"A2:{colkop}2"].Merge = true;
                sheet.Cells[$"A2:{colkop}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A2:{colkop}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A2:{colkop}2"].Style.Font.Bold = true;
                sheet.Cells[$"A3:{colkop}3"].Value = kop3;
                sheet.Cells[$"A3:{colkop}3"].Merge = true;
                sheet.Cells[$"A3:{colkop}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A3:{colkop}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A3:{colkop}3"].Style.Font.Bold = true;
                sheet.Cells[$"A4:{colkop}4"].Value = kop4;
                sheet.Cells[$"A4:{colkop}4"].Merge = true;
                sheet.Cells[$"A4:{colkop}4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A4:{colkop}4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A4:{colkop}4"].Style.Font.Bold = true;
                #endregion

                #region Headers and Data
                sheet.Cells["A8"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);
                for (var i = 7; i <= result.Rows.Count + 7; i++)
                {
                    for (var j = 0; j < headers.Length; j++)
                    {
                        var col = (char)('A' + j);
                        sheet.Cells[$"{col}{i}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                }
                sheet.Cells["A6"].Value = headers[0];
                sheet.Cells["A6:F6"].Merge = true;
                sheet.Cells["A6:F6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["A6:F6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["A6:F6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["G6"].Value = headers[6];
                sheet.Cells["G6:K6"].Merge = true;
                sheet.Cells["G6:K6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["G6:K6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["G6:K6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["L6"].Value = headers[11];
                sheet.Cells["L6:N6"].Merge = true;
                sheet.Cells["L6:N6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["L6:N6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["L6:N6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                for (var i = 0; i < subheaders.Length; i++)
                {
                    var col = (char)('A' + i);
                    sheet.Cells[$"{col}7"].Value = subheaders[i];
                    sheet.Cells[$"{col}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                }
                var widths = new int[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
                foreach (var i in Enumerable.Range(0, headers.Length))
                {
                    sheet.Column(i + 1).Width = widths[i];
                }
                #endregion
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
    }
}
