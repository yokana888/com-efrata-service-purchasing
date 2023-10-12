using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DebtBookReportViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentDebtBalanceReportFacade : IGarmentDebtBalanceReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        ILocalDbCashFlowDbContext dbContextLocal;
        public readonly IServiceProvider serviceProvider;
        public GarmentDebtBalanceReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext, ILocalDbCashFlowDbContext dbContextLocal)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbContextLocal = dbContextLocal;
        }

        public List<GarmentDebtBalanceViewModel> getQuery(int month, int year, bool? suppliertype, string category)
        {
            List<GarmentDebtBalanceViewModel> report = new List<GarmentDebtBalanceViewModel>();
            var date = new DateTime(year, month, DateTime.MinValue.Day);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var supplier = from a in dbContext.GarmentDeliveryOrders
                           join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                           join d in dbContext.GarmentDeliveryOrderDetails on b.Id equals d.GarmentDOItemId
                           join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                           where
                           a.ArrivalDate.Date >= date.Date
                           && a.ArrivalDate.Date <= endDate.Date
                           && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                           && d.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? d.CodeRequirment : category)
             
                           && a.IsDeleted == false
                           && b.IsDeleted == false
                           && c.IsDeleted == false
                           && d.IsDeleted == false

                           group new { a, b, c, d } by new { a.SupplierCode, a.DOCurrencyCode } into groupdata
                           select new
                           {
                               SupplierName = groupdata.FirstOrDefault().a.SupplierName,
                               SupplierCode = groupdata.FirstOrDefault().a.SupplierCode,
                               DOCurrencyCode = groupdata.FirstOrDefault().a.DOCurrencyCode
                           };
            var saldoawal = from b in dbContext.GarmentSupplierBalanceDebts
                            join c in dbContext.GarmentSupplierBalanceDebtItems on b.Id equals c.GarmentDebtId
                            join d in dbContext.GarmentDeliveryOrders on c.DOId equals d.Id
                            where b.Import == (suppliertype.HasValue ? suppliertype : b.Import)
                            && b.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? b.CodeRequirment : category)

                            select new GarmentDebtBalanceViewModel
                            {
                                SupplierCode = b.SupplierCode,
                                SupplierName = b.SupplierName,
                                DOCurrencyCode = b.DOCurrencyCode,
                                BillNo = c.BillNo,
                                DONo = c.DONo,
                                PaymentBill = d.PaymentBill,
                                CodeRequirment = b.CodeRequirment,
                                InitialBalance = c.Valas,
                                TotalDebit = 0,
                                NoDebit = "",
                                TglDebit = DateTimeOffset.MinValue,
                                TotalKredit = 0,
                                TotalEndingBalance = 0
                            };

            var SaldoAwalSeblmBulan = (from a in dbContext.GarmentDeliveryOrders
                                       join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                       join d in dbContext.GarmentDeliveryOrderDetails on b.Id equals d.GarmentDOItemId
                                       join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                                       where a.ArrivalDate >= new DateTime(year - 1, 1, 1)
                                       && a.ArrivalDate < date.Date
                                       && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                                       && d.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? d.CodeRequirment : category)
                                       && a.IsDeleted == false
                                       && b.IsDeleted == false
                                       && c.IsDeleted == false
                                       && d.IsDeleted == false

                                       select new GarmentDebtBalanceViewModel
                                       {
                                           SupplierCode = a.SupplierCode,
                                           SupplierName = a.SupplierName,
                                           DOCurrencyCode = a.DOCurrencyCode,
                                           BillNo = a.BillNo,
                                           DONo = a.DONo,
                                           PaymentBill = a.PaymentBill,
                                           CodeRequirment = d.CodeRequirment,                                           
                                           InitialBalance = a.TotalAmount,
                                           TotalDebit = 0,
                                           NoDebit = "",
                                           TglDebit = DateTimeOffset.MinValue,
                                           TotalKredit = 0,
                                           TotalEndingBalance = 0
                                       }).Distinct();
            var SaldoAwalBulan = (from a in dbContext.GarmentDeliveryOrders
                                  join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                  join d in dbContext.GarmentDeliveryOrderDetails on b.Id equals d.GarmentDOItemId
                                  join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                                  where
                                  a.ArrivalDate.Date >= date.Date
                                  && a.ArrivalDate.Date <= endDate.Date
                                  && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                                  && d.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? d.CodeRequirment : category)

                                  select new GarmentDebtBalanceViewModel
                                  {
                                      SupplierCode = a.SupplierCode,
                                      SupplierName = a.SupplierName,
                                      DOCurrencyCode = a.DOCurrencyCode,
                                      BillNo = a.BillNo,
                                      DONo = a.DONo,
                                      PaymentBill = a.PaymentBill,
                                      CodeRequirment = d.CodeRequirment, 
                                      InitialBalance = 0,
                                      TotalDebit = 0,
                                      NoDebit = "",
                                      TglDebit = DateTimeOffset.MinValue,
                                      TotalKredit = a.TotalAmount,
                                      TotalEndingBalance = 0
                                  }).Distinct();
            var SaldoAwaljoin = saldoawal.Union(SaldoAwalSeblmBulan).ToList();
            var SaldoAwaljoin2 = from a in supplier
                                 join b in SaldoAwaljoin on new { a.DOCurrencyCode, a.SupplierCode } equals new { b.DOCurrencyCode, b.SupplierCode }
                                 select new GarmentDebtBalanceViewModel
                                 {
                                     SupplierCode = a.SupplierCode,
                                     SupplierName = a.SupplierName,
                                     DOCurrencyCode = b.DOCurrencyCode,
                                     BillNo = b.BillNo,
                                     DONo = b.DONo,
                                     PaymentBill = b.PaymentBill,
                                     CodeRequirment = b.CodeRequirment,
                                     InitialBalance = b.InitialBalance,
                                     TotalDebit = b.TotalDebit,
                                     NoDebit = b.NoDebit,
                                     TglDebit = b.TglDebit,
                                     TotalKredit = b.TotalKredit,
                                     TotalEndingBalance = b.TotalEndingBalance
                                 };
            var saldoawaltotal = from a in SaldoAwaljoin
                                 group a by new { a.DOCurrencyCode, a.SupplierCode } into groupdata
                                 select new
                                 {
                                     SupplierName = groupdata.FirstOrDefault().SupplierName,
                                     SupplierCode = groupdata.FirstOrDefault().SupplierCode,
                                     DOCurrencyCode = groupdata.FirstOrDefault().DOCurrencyCode,
                                     TotalInitialBalance = groupdata.Sum(x => x.InitialBalance)
                                 };
            var SaldoAkhir = SaldoAwaljoin2.Union(SaldoAwalBulan).ToList();
            var saldoawaljointotal = (from a in SaldoAkhir
                                      join b in saldoawaltotal on new { a.DOCurrencyCode, a.SupplierCode } equals new { b.DOCurrencyCode, b.SupplierCode } into debt
                                      from ab in debt.DefaultIfEmpty()

                                      select new GarmentDebtBalanceViewModel
                                      {
                                          SupplierName = a.SupplierName,
                                          SupplierCode = a.SupplierCode,
                                          DOCurrencyCode = a.DOCurrencyCode,
                                          TotalInitialBalance = ab != null ? ab.TotalInitialBalance : 0,
                                          BillNo = a.BillNo,
                                          DONo = a.DONo,
                                          PaymentBill = a.PaymentBill,
                                          CodeRequirment = a.CodeRequirment,                                         
                                          InitialBalance = a.InitialBalance,
                                          TotalDebit = a.TotalDebit,
                                          NoDebit = a.NoDebit,
                                          TglDebit = a.TglDebit,
                                          TotalKredit = a.TotalKredit,
                                          TotalEndingBalance = a.TotalEndingBalance
                                      }).OrderBy(x => x.SupplierCode).ThenBy(x => x.DOCurrencyCode);
            
            foreach (GarmentDebtBalanceViewModel i in saldoawaljointotal)
            {
                string cmddetail2 = "Select jumlah, rate, rate1, nomor, tgl from RincianDetil where no_bon = '" + i.BillNo + "' and no_sj = '" + i.DONo + "'";
                var data = dbContextLocal.ExecuteReaderOnlyQuery(cmddetail2);
                while (data.Read())
                {
                    i.TotalDebit = (decimal)data["jumlah"];
                    i.NoDebit = data["nomor"].ToString();
                    i.TglDebit = data.GetDateTime(4);
                }
               
                report.Add(i);
            }
            report.Aggregate(
                new List<GarmentDebtBalanceViewModel>(),
                (newList, element) =>
                {
                    element.TotalEndingBalance = element.TotalInitialBalance + element.TotalKredit - (double)element.TotalDebit;
                    newList.Add(element);
                    return newList;
                });
            return report;
        }

        public Tuple<List<GarmentDebtBalanceViewModel>, int> GetDebtBookReport(int month, int year, bool? suppliertype, string category)
        {
            List<GarmentDebtBalanceViewModel> result = getQuery(month, year, suppliertype, category);
            return Tuple.Create(result, result.Count);
        }

        public MemoryStream GenerateExcelDebtReport(int month, int year, bool? suppliertype, string category)
        {
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(month);
            Tuple<List<GarmentDebtBalanceViewModel>, int> Data = this.GetDebtBookReport(month, year, suppliertype, category);
            DataTable result = new DataTable();
            ExcelPackage package = new ExcelPackage();
            string kop1 = "PT. EFRATA GARMINDO UTAMA";
            string kop2 = "LAPORAN SALDO HUTANG";
            string kop3 = string.Format("Periode Bulan {0} {1}", Month, year);
            var headers = new string[] { "Kode Supplier", "Nama Supplier", "Mata Uang", "Saldo Awal Total", "No BP", "No BP Kecil", "Tipe", "Saldo Awal", "Hutang", "Bayar", "No Bayar", "Tgl Bayar", "Saldo Akhir" };
            var headers2 = new string[] { "Total", "", "", "", "", "", "", "", "", "Jumlah", "Nota Bayar", "Tgl Bayar", ""};
            for (int i = 0; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });

            }

            if (Data.Item2 == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
                var sheet = package.Workbook.Worksheets.Add("Data");
                sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);

            }
            else
            {
                Dictionary<string, double?> TotalPerSupplier = new Dictionary<string, double?>();
                Dictionary<string, double?> TotalIDRPerSupplier = new Dictionary<string, double?>();
                //Dictionary<string, int> SupplierCount = new Dictionary<string, int>();
                Dictionary<string, int> SupplierCurCodeCount = new Dictionary<string, int>();
                double AmountTotalInitialBalance = 0;
                //double AmountTotalIDRInitialBalance = 0;
                double AmountTotal = 0;
                //double AmountTotalIDR = 0;
                double AmountTotalDebit = 0;
                //double AmountTotalDebitIDR = 0;
                double AmountTotalKredit = 0;
                //double AmountTotalKreditIDR = 0;
                double AmountTotalEndingBalance = 0;
                //double AmountTotalEndingBalanceIDR = 0;
                //string[] supp = new string[];
                int value;
                List<string> supplier = new List<string>();
                int rowPosition = 1;
                foreach (GarmentDebtBalanceViewModel data in Data.Item1)
                {

                    string SupplierName = data.SupplierName;
                    string TglDebit = data.TglDebit == new DateTime(1970, 1, 1) || data.TglDebit.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : data.TglDebit.Value.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string TotalInitialBalance = string.Format("{0:N2}", data.TotalInitialBalance);
                    //string TotalCurrencyInitialBalance = string.Format("{0:N2}", data.TotalCurrencyInitialBalance);
                    //string TotalIDR = string.Format("{0:N2}", data.TotalIDR);
                    string InitialBalance = string.Format("{0:N2}", data.InitialBalance);
                    //string CurrencyKredit = string.Format("{0:N2}", data.CurrencyKredit);
                    //string IDR = string.Format("{0:N2}", data.IDR);
                    string TotalDebit = string.Format("{0:N2}", data.TotalDebit);
                    //string CurrencyDebit = string.Format("{0:N2}", data.CurrencyDebit);
                    //string TotalIDRDebit = string.Format("{0:N2}", data.TotalIDRDebit);
                    string TotalKredit = string.Format("{0:N2}", data.TotalKredit);
                    //string TotalIDRKredit = string.Format("{0:N2}", data.TotalIDRKredit);
                    string TotalEndingBalance = string.Format("{0:N2}", data.TotalEndingBalance);
                    //string CurrencyEndingBalance = string.Format("{0:N2}", data.CurrencyEndingBalance);
                    //string TotalIDREndingBalance = string.Format("{0:N2}", data.TotalIDREndingBalance);

                    if (!TotalPerSupplier.ContainsKey(SupplierName))
                    {
                        TotalPerSupplier.Add(SupplierName, 0);
                    };

                    if (SupplierCurCodeCount.TryGetValue(data.SupplierName + data.DOCurrencyCode, out value))
                    {
                        SupplierCurCodeCount[data.SupplierName + data.DOCurrencyCode]++;
                    }
                    else
                    {
                        SupplierCurCodeCount[data.SupplierName + data.DOCurrencyCode] = 1;
                    }

                    if (!TotalIDRPerSupplier.ContainsKey(SupplierName))
                    {
                        TotalIDRPerSupplier.Add(SupplierName, 0);
                    };

                    TotalPerSupplier[SupplierName] = data.TotalInitialBalance;
                    supplier.Add(data.SupplierName);
                    AmountTotal += (double)data.InitialBalance;
                    //AmountTotalIDR += (double)data.IDR;
                    AmountTotalDebit += (double)data.TotalDebit;
                    //AmountTotalDebitIDR += (double)data.TotalIDRDebit;
                    AmountTotalKredit += data.TotalKredit;
                    //AmountTotalKreditIDR += (double)data.TotalIDRKredit;
                    AmountTotalEndingBalance += (double)data.TotalEndingBalance;
                    //AmountTotalEndingBalanceIDR += (double)data.TotalIDREndingBalance;
                    result.Rows.Add(data.SupplierCode, data.SupplierName, data.DOCurrencyCode, TotalInitialBalance, data.BillNo, data.PaymentBill, data.CodeRequirment, InitialBalance, TotalKredit, TotalDebit, data.NoDebit, TglDebit, TotalEndingBalance);
                    rowPosition += 1;
                }
                var supp = supplier.Distinct().ToArray();
                foreach (var i in supp)
                {                    
                    AmountTotalInitialBalance += (double)TotalPerSupplier[i];
                    //AmountTotalIDRInitialBalance += (double)TotalIDRPerSupplier[i];
                }
                string AmountTotalstr = string.Format("{0:N2}", AmountTotal);
                //string AmountTotalIDRstr = string.Format("{0:N2}", AmountTotalIDR);
                string AmountTotalDebitstr = string.Format("{0:N2}", AmountTotalDebit);
                //string AmountTotalDebitIDRstr = string.Format("{0:N2}", AmountTotalDebitIDR);
                string AmountTotalKreditstr = string.Format("{0:N2}", AmountTotalKredit);
                //string AmountTotalKreditIDRstr = string.Format("{0:N2}", AmountTotalKreditIDR);
                string AmountTotalEndingBalancestr = string.Format("{0:N2}", AmountTotalEndingBalance);
                //string AmountTotalEndingBalanceIDRstr = string.Format("{0:N2}", AmountTotalEndingBalanceIDR);
                string AmountTotalInitialBalancestr = string.Format("{0:N2}", AmountTotalInitialBalance);
                //string AmountTotalIDRInitialBalancestr = string.Format("{0:N2}", AmountTotalIDRInitialBalance);
              
                result.Rows.Add("TOTAL", "", "", AmountTotalInitialBalancestr, "", "", "", AmountTotalstr, AmountTotalKreditstr, AmountTotalDebitstr, "", "", AmountTotalEndingBalancestr);
                rowPosition += 1;
                var sheet = package.Workbook.Worksheets.Add("Data");
                var colkop = (char)('A' + headers.Length - 1);
   
                #region KopTable
                sheet.Cells[$"A1:{colkop}1"].Value = kop1;
                sheet.Cells[$"A1:{colkop}1"].Merge = true;
                sheet.Cells[$"A1:{colkop}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A1:{colkop}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A1:{colkop}1"].Style.Font.Bold = true;
                sheet.Cells[$"A2:{colkop}2"].Value = kop2;
                sheet.Cells[$"A2:{colkop}2"].Merge = true;
                sheet.Cells[$"A2:{colkop}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A2:{colkop}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A2:{colkop}2"].Style.Font.Bold = true;
                sheet.Cells[$"A3:{colkop}3"].Value = kop3;
                sheet.Cells[$"A3:{colkop}3"].Merge = true;
                sheet.Cells[$"A3:{colkop}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A3:{colkop}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A3:{colkop}3"].Style.Font.Bold = true;
                #endregion
                #region Headers and Data
                sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);
                for (var i = 7; i <= result.Rows.Count + 7; i++)
                {
                    for (var j = 0; j < headers.Length; j++)
                    {
                        var col = (char)('A' + j);
                        sheet.Cells[$"{col}{i}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                }
               
                sheet.Cells["C5"].Value = headers[2];
                sheet.Cells["D5"].Value = headers[3];
                sheet.Cells["E5"].Value = headers[4];
                sheet.Cells["F5"].Value = headers[5];
                sheet.Cells["G5"].Value = headers[6];
                sheet.Cells["H5"].Value = headers[7];
                sheet.Cells["I5"].Value = headers[8];
              
                sheet.Cells["J5"].Value = headers[9];
                sheet.Cells["J5:L5"].Merge = true;
                sheet.Cells["J5:L5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["J5:L5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["J5:L5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["M5"].Value = headers[12];

                sheet.Cells[$"A{rowPosition + 5}:C{ rowPosition + 5}"].Merge = true;
                sheet.Cells[$"A{rowPosition + 5}:C{ rowPosition + 5}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"E{rowPosition + 5}:G{ rowPosition + 5}"].Merge = true;
                sheet.Cells[$"E{rowPosition + 5}:G{ rowPosition + 5}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{rowPosition + 5}:M{ rowPosition + 5}"].Style.Font.Bold = true;
                #endregion
                #region Merge
                foreach (var i in Enumerable.Range(0, 9))
                {
                    var col = (char)('A' + i);
                    sheet.Cells[$"{col}5"].Value = headers[i];
                    sheet.Cells[$"{col}5:{col}6"].Merge = true;
                    sheet.Cells[$"{col}5:{col}6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }

                foreach (var i in Enumerable.Range(0, 1))
                {
                    var col = (char)('M' + i);
                    sheet.Cells[$"{col}5"].Value = headers[i];
                    sheet.Cells[$"{col}5:{col}6"].Merge = true;
                    sheet.Cells[$"{col}5:{col}6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }

                for (var i = 0; i < 3; i++)
                {
                    var col = (char)('J' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i + 9];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
               
                int index = 7;
                foreach (KeyValuePair<string, int> c in SupplierCurCodeCount)
                {
                    sheet.Cells["A" + index + ":A" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }

                #endregion
                var widths = new int[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
                foreach (var i in Enumerable.Range(0, headers.Length))
                {
                    sheet.Column(i + 1).Width = widths[i];
                }

            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
    }
}
