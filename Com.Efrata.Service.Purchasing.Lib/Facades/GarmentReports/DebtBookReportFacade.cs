using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.DebtBookReportViewModels;
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
    public class DebtBookReportFacade : IDebtBookReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        ILocalDbCashFlowDbContext dbContextLocal;
        public readonly IServiceProvider serviceProvider;
        public DebtBookReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext, ILocalDbCashFlowDbContext dbContextLocal)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbContextLocal = dbContextLocal;
        }

        public List<DebtBookReportViewModel> getQuery(int month, int year, bool? suppliertype, string suppliername)
        {
            List<DebtBookReportViewModel> report = new List<DebtBookReportViewModel>();
            var date = new DateTime(year, month, DateTime.MinValue.Day);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var supplier = from a in dbContext.GarmentDeliveryOrders
                           join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                           join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                           where
                           a.ArrivalDate.Date >= date.Date
                           && a.ArrivalDate.Date <= endDate.Date
                           && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                           && (string.IsNullOrWhiteSpace(suppliername) ? true : (suppliername == "EFRATA GARMINDO UTAMA" ? a.SupplierCode.Substring(0, 2) == "DL" : a.SupplierCode.Substring(0, 2) != "DL"))
                           && a.IsDeleted == false
                           && b.IsDeleted == false
                           && c.IsDeleted == false
                           group new { a, b, c } by new { a.SupplierCode, a.DOCurrencyCode } into groupdata
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
                            && (string.IsNullOrWhiteSpace(suppliername) ? true : (suppliername == "EFRATA GARMINDO UTAMA" ? b.SupplierCode.Substring(0, 2) == "DL" : b.SupplierCode.Substring(0, 2) != "DL"))
                            select new DebtBookReportViewModel
                            {
                                SupplierCode = b.SupplierCode,
                                SupplierName = b.SupplierName,
                                DOCurrencyCode = b.DOCurrencyCode,
                                BillNo = c.BillNo,
                                DONo = c.DONo,
                                PaymentBill = d.PaymentBill,
                                INNo = c.InternNo,
                                TotalCurrencyInitialBalance = b.DOCurrencyRate,
                                DebtAge = (DateTime.Now - c.ArrivalDate.DateTime).Days,
                                InitialBalance = c.Valas,
                                CurrencyInitialBalance = b.DOCurrencyRate,
                                IDR = c.IDR,
                                TotalDebit = 0,
                                CurrencyDebit = 0,
                                TotalIDRDebit = 0,
                                NoDebit = "",
                                TglDebit = DateTimeOffset.MinValue,
                                TotalKredit = 0,
                                CurrencyKredit = 0,
                                TotalIDRKredit = 0,
                                DifferenceRate = 0,
                                TotalEndingBalance = 0,
                                CurrencyEndingBalance = 0,
                                TotalIDREndingBalance = 0
                            };
            var SaldoAwalSeblmBulan = (from a in dbContext.GarmentDeliveryOrders
                                       join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                       join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                                       where a.ArrivalDate >= new DateTime(year - 1, 1, 1)
                                       && a.ArrivalDate < date.Date
                                       && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                                       && (string.IsNullOrWhiteSpace(suppliername) ? true : (suppliername == "EFRATA GARMINDO UTAMA" ? a.SupplierCode.Substring(0, 2) == "DL" : a.SupplierCode.Substring(0, 2) != "DL"))
                                       && a.IsDeleted == false
                                       && b.IsDeleted == false
                                       && c.IsDeleted == false
                                       select new DebtBookReportViewModel
                                       {
                                           SupplierCode = a.SupplierCode,
                                           SupplierName = a.SupplierName,
                                           DOCurrencyCode = a.DOCurrencyCode,
                                           BillNo = a.BillNo,
                                           DONo = a.DONo,
                                           PaymentBill = a.PaymentBill,
                                           INNo = a.InternNo,
                                           DebtAge = (DateTime.Now - a.ArrivalDate.DateTime).Days,
                                           InitialBalance = a.TotalAmount,
                                           TotalCurrencyInitialBalance = a.DOCurrencyRate,
                                           CurrencyInitialBalance = a.DOCurrencyRate,
                                           IDR = a.TotalAmount * a.DOCurrencyRate,
                                           TotalDebit = 0,
                                           CurrencyDebit = 0,
                                           TotalIDRDebit = 0,
                                           NoDebit = "",
                                           TglDebit = DateTimeOffset.MinValue,
                                           TotalKredit = 0,
                                           CurrencyKredit = 0,
                                           TotalIDRKredit = 0,
                                           DifferenceRate = 0,
                                           TotalEndingBalance = 0,
                                           CurrencyEndingBalance = 0,
                                           TotalIDREndingBalance = 0
                                       }).Distinct();
            var SaldoAwalBulan = (from a in dbContext.GarmentDeliveryOrders
                                  join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                                  join c in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals c.Id
                                  where
                                  a.ArrivalDate.Date >= date.Date
                                  && a.ArrivalDate.Date <= endDate.Date
                                  && c.SupplierImport == (suppliertype.HasValue ? suppliertype : c.SupplierImport)
                                  && (string.IsNullOrWhiteSpace(suppliername) ? true : (suppliername == "EFRATA GARMINDO UTAMA" ? a.SupplierCode.Substring(0, 2) == "DL" : a.SupplierCode.Substring(0, 2) != "DL"))
                                  select new DebtBookReportViewModel
                                  {
                                      SupplierCode = a.SupplierCode,
                                      SupplierName = a.SupplierName,
                                      DOCurrencyCode = a.DOCurrencyCode,
                                      BillNo = a.BillNo,
                                      DONo = a.DONo,
                                      PaymentBill = a.PaymentBill,
                                      INNo = a.InternNo,
                                      DebtAge = (DateTime.Now - a.ArrivalDate.DateTime).Days,
                                      InitialBalance = 0,
                                      CurrencyInitialBalance = 0,
                                      IDR = 0,
                                      TotalDebit = 0,
                                      CurrencyDebit = 0,
                                      TotalIDRDebit = 0,
                                      NoDebit = "",
                                      TglDebit = DateTimeOffset.MinValue,
                                      TotalKredit = a.TotalAmount,
                                      CurrencyKredit = a.DOCurrencyRate,
                                      TotalIDRKredit = a.TotalAmount * a.DOCurrencyRate,
                                      DifferenceRate = 0,
                                      TotalEndingBalance = 0,
                                      CurrencyEndingBalance = 0,
                                      TotalIDREndingBalance = 0

                                  }).Distinct();
            var SaldoAwaljoin = saldoawal.Union(SaldoAwalSeblmBulan).ToList();
            var SaldoAwaljoin2 = from a in supplier
                                 join b in SaldoAwaljoin on new { a.DOCurrencyCode, a.SupplierCode } equals new { b.DOCurrencyCode, b.SupplierCode }
                                 select new DebtBookReportViewModel
                                 {
                                     SupplierCode = a.SupplierCode,
                                     SupplierName = a.SupplierName,
                                     DOCurrencyCode = b.DOCurrencyCode,
                                     BillNo = b.BillNo,
                                     DONo = b.DONo,
                                     PaymentBill = b.PaymentBill,
                                     INNo = b.INNo,
                                     DebtAge = b.DebtAge,
                                     InitialBalance = b.InitialBalance,
                                     CurrencyInitialBalance = b.CurrencyInitialBalance,
                                     IDR = b.IDR,
                                     TotalDebit = b.TotalDebit,
                                     CurrencyDebit = b.CurrencyDebit,
                                     TotalIDRDebit = b.TotalIDRDebit,
                                     NoDebit = b.NoDebit,
                                     TglDebit = b.TglDebit,
                                     TotalKredit = b.TotalKredit,
                                     CurrencyKredit = b.CurrencyKredit,
                                     TotalIDRKredit = b.TotalIDRKredit,
                                     DifferenceRate = b.DifferenceRate,
                                     TotalEndingBalance = b.TotalEndingBalance,
                                     CurrencyEndingBalance = b.CurrencyEndingBalance,
                                     TotalIDREndingBalance = b.TotalIDREndingBalance
                                 };
            var saldoawaltotal = from a in SaldoAwaljoin
                                 group a by new { a.DOCurrencyCode, a.SupplierCode } into groupdata
                                 select new
                                 {
                                     SupplierName = groupdata.FirstOrDefault().SupplierName,
                                     SupplierCode = groupdata.FirstOrDefault().SupplierCode,
                                     DOCurrencyCode = groupdata.FirstOrDefault().DOCurrencyCode,
                                     TotalCurrencyInitialBalance = groupdata.FirstOrDefault().TotalCurrencyInitialBalance,
                                     TotalInitialBalance = groupdata.Sum(x => x.InitialBalance),
                                     TotalIDR = groupdata.Sum(x => x.IDR)
                                 };
            var SaldoAkhir = SaldoAwaljoin2.Union(SaldoAwalBulan).ToList();
            var saldoawaljointotal = (from a in SaldoAkhir
                                      join b in saldoawaltotal on new { a.DOCurrencyCode, a.SupplierCode } equals new { b.DOCurrencyCode, b.SupplierCode } into debt
                                      from ab in debt.DefaultIfEmpty()
                                      select new DebtBookReportViewModel
                                      {
                                          SupplierName = a.SupplierName,
                                          SupplierCode = a.SupplierCode,
                                          DOCurrencyCode = a.DOCurrencyCode,
                                          TotalInitialBalance = ab != null ? ab.TotalInitialBalance : 0,
                                          TotalCurrencyInitialBalance = ab != null ? ab.TotalCurrencyInitialBalance : 0,
                                          TotalIDR = ab != null ? ab.TotalIDR : 0,
                                          BillNo = a.BillNo,
                                          DONo = a.DONo,
                                          PaymentBill = a.PaymentBill,
                                          INNo = a.INNo,
                                          DebtAge = a.DebtAge,
                                          InitialBalance = a.InitialBalance,
                                          CurrencyInitialBalance = a.CurrencyInitialBalance,
                                          IDR = a.IDR,
                                          TotalDebit = a.TotalDebit,
                                          CurrencyDebit = a.CurrencyDebit,
                                          TotalIDRDebit = a.TotalIDRDebit,
                                          NoDebit = a.NoDebit,
                                          TglDebit = a.TglDebit,
                                          TotalKredit = a.TotalKredit,
                                          CurrencyKredit = a.CurrencyKredit,
                                          TotalIDRKredit = a.TotalIDRKredit,
                                          DifferenceRate = a.DifferenceRate,
                                          TotalEndingBalance = a.TotalEndingBalance,
                                          CurrencyEndingBalance = a.CurrencyEndingBalance,
                                          TotalIDREndingBalance = a.TotalIDREndingBalance
                                      }).OrderBy(x => x.SupplierCode).ThenBy(x => x.DOCurrencyCode);


            //var Akhir = (from a in Kredit
            //             join b in SaldoAwal on new { a.SupplierCode, a.DOCurrencyCode } equals new { b.SupplierCode, b.DOCurrencyCode } into SaldoAkhir
            //             from SA in SaldoAkhir.DefaultIfEmpty()
            //             select new DebtBookReportViewModel
            //             {
            //                 BillNo = a.BillNo,
            //                 DebtAge = a.DebtAge,
            //                 CurrencyKredit = a.CurrencyKredit,
            //                 DOCurrencyCode = a.DOCurrencyCode,
            //                 DONo = a.DONo,
            //                 IDR = a.IDR,
            //                 InitialBalance = a.InitialBalance,
            //                 INNo = a.INNo,
            //                 PaymentBill = a.PaymentBill,
            //                 SupplierCode = a.SupplierCode,
            //                 SupplierName = a.SupplierName,
            //                 TotalCurrencyInitialBalance = SA.TotalCurrencyInitialBalance != null ? SA.TotalCurrencyInitialBalance : 0,
            //                 TotalIDR = SA.TotalIDR != null ? SA.TotalIDR : 0,
            //                 CurrencyInitialBalance = a.CurrencyInitialBalance,
            //                 TotalInitialBalance = SA.TotalValas != null ? SA.TotalValas : 0,
            //                 TotalIDRKredit = a.TotalIDRKredit,
            //                 TotalKredit = a.TotalKredit,
            //                 TglDebit = new DateTime(1970,1,1),

            //             }).OrderBy(x => x.SupplierName).ThenBy(x=>x.DOCurrencyCode);
            foreach (DebtBookReportViewModel i in saldoawaljointotal)
            {
                string cmddetail2 = "Select jumlah, rate, rate1, nomor, tgl from RincianDetil where no_bon = '" + i.BillNo + "' and no_sj = '" + i.DONo + "' and ketr = '" + i.SupplierName + "'";
                var data = dbContextLocal.ExecuteReaderOnlyQuery(cmddetail2);
                while (data.Read())
                {
                    i.TotalDebit = (decimal)data["jumlah"];
                    i.CurrencyDebit = (decimal)data["rate"];
                    i.TotalIDRDebit = (decimal)data["jumlah"] * (decimal)data["rate"];
                    i.DifferenceRate = (decimal)data["rate"] - (decimal)data["rate1"];
                    i.NoDebit = data["nomor"].ToString();
                    i.TglDebit = data.GetDateTime(4);
                }
                //data.Close();

                //i.TotalEndingBalance = i.TotalInitialBalance + i.TotalKredit - (double)i.TotalDebit;
                //i.CurrencyEndingBalance = i.TotalCurrencyInitialBalance;
                //i.TotalIDREndingBalance = i.TotalEndingBalance * i.CurrencyEndingBalance;
                report.Add(i);
            }
            report.Aggregate(
                new List<DebtBookReportViewModel>(),
                (newList, element) =>
                {
                    element.TotalEndingBalance = element.TotalInitialBalance + element.TotalKredit - (double)element.TotalDebit;
                    element.CurrencyEndingBalance = element.TotalCurrencyInitialBalance;
                    element.TotalIDREndingBalance = element.TotalEndingBalance * element.CurrencyEndingBalance;
                    newList.Add(element);
                    return newList;
                });
            return report;
        }

        public Tuple<List<DebtBookReportViewModel>, int> GetDebtBookReport(int month, int year, bool? suppliertype, string suppliername)
        {
            List<DebtBookReportViewModel> result = getQuery(month, year, suppliertype, suppliername);
            return Tuple.Create(result, result.Count);
        }

        public MemoryStream GenerateExcelDebtReport(int month, int year, bool? suppliertype, string suppliername)
        {
            CultureInfo Id = new CultureInfo("id-ID");
            //string Month = month == 1 ? "JANUARI" : month == 2 ? "FEBRUARI" : month == 3 ? "MARET" : month == 4 ? "APRIL" : month == 5 ? "MEI" : month == 6 ? "JUNI" : month == 7 ? "JULI" : month == 8 ? "AGUSTUS" : month == 9 ? "SEPTEMBER" : month == 10 ? "OKTOBER" : month == 11 ? "NOVEMBER" : month == 12 ? "DESEMBER" : "";
            string Month = Id.DateTimeFormat.GetMonthName(month);
            Tuple<List<DebtBookReportViewModel>, int> Data = this.GetDebtBookReport(month, year, suppliertype, suppliername);
            DataTable result = new DataTable();
            ExcelPackage package = new ExcelPackage();
            string kop1 = "PT. EFRATA GARMINDO UTAMA";
            string kop2 = "DETAIL REKAP SALDO HUTANG";
            string kop3 = string.Format("Periode Bulan {0} {1}", Month, year);
            var headers = new string[] { "Nama Supplier", "Mata Uang", "Saldo Awal Total", "Saldo Awal Total1", "Saldo Awal Total2", "No BP", "No BP Kecil", "Umur Hutang (Hari)", "Nota Intern", "Saldo Awal", "Saldo Awal1", "Saldo Awal2", "Debit", "Debit1", "Debit2", "Debit3", "Debit4", "Kredit", "Kredit1", "Kredit2", "Selisih Kurs", "Saldo Akhir", "Saldo Akhir1", "Saldo Akhir2" };
            var headers2 = new string[] { "Total", "Kurs", "Total IDR", "Total", "Kurs", "Total IDR", "Total", "Kurs", "Total IDR", "Nota Bayar", "Tgl Bayar", "Total", "Kurs", "Total IDR", "Total", "Kurs", "Total IDR" };
            for (int i = 0; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });

            }

            if (Data.Item2 == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
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
                double AmountTotalIDRInitialBalance = 0;
                double AmountTotal = 0;
                double AmountTotalIDR = 0;
                double AmountTotalDebit = 0;
                double AmountTotalDebitIDR = 0;
                double AmountTotalKredit = 0;
                double AmountTotalKreditIDR = 0;
                double AmountTotalEndingBalance = 0;
                double AmountTotalEndingBalanceIDR = 0;
                //string[] supp = new string[];
                int value;
                List<string> supplier = new List<string>();
                int rowPosition = 1;
                foreach (DebtBookReportViewModel data in Data.Item1)
                {

                    string SupplierName = data.SupplierName;
                    string TglDebit = data.TglDebit == new DateTime(1970, 1, 1) || data.TglDebit.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : data.TglDebit.Value.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string TotalInitialBalance = string.Format("{0:N2}", data.TotalInitialBalance);
                    string TotalCurrencyInitialBalance = string.Format("{0:N2}", data.TotalCurrencyInitialBalance);
                    string TotalIDR = string.Format("{0:N2}", data.TotalIDR);
                    string InitialBalance = string.Format("{0:N2}", data.InitialBalance);
                    string CurrencyKredit = string.Format("{0:N2}", data.CurrencyKredit);
                    string IDR = string.Format("{0:N2}", data.IDR);
                    string TotalDebit = string.Format("{0:N2}", data.TotalDebit);
                    string CurrencyDebit = string.Format("{0:N2}", data.CurrencyDebit);
                    string TotalIDRDebit = string.Format("{0:N2}", data.TotalIDRDebit);
                    string TotalKredit = string.Format("{0:N2}", data.TotalKredit);
                    string TotalIDRKredit = string.Format("{0:N2}", data.TotalIDRKredit);
                    string TotalEndingBalance = string.Format("{0:N2}", data.TotalEndingBalance);
                    string CurrencyEndingBalance = string.Format("{0:N2}", data.CurrencyEndingBalance);
                    string TotalIDREndingBalance = string.Format("{0:N2}", data.TotalIDREndingBalance);

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

                    //if (SupplierCount.TryGetValue(data.SupplierName, out value))
                    //{
                    //    SupplierCount[data.SupplierName]++;
                    //}
                    //else
                    //{
                    //    SupplierCount[data.SupplierName] = 1;
                    //}
                    if (!TotalIDRPerSupplier.ContainsKey(SupplierName))
                    {
                        TotalIDRPerSupplier.Add(SupplierName, 0);
                    };

                    TotalPerSupplier[SupplierName] = data.TotalInitialBalance;
                    TotalIDRPerSupplier[SupplierName] = data.TotalIDR;
                    supplier.Add(data.SupplierName);
                    AmountTotal += (double)data.InitialBalance;
                    AmountTotalIDR += (double)data.IDR;
                    AmountTotalDebit += (double)data.TotalDebit;
                    AmountTotalDebitIDR += (double)data.TotalIDRDebit;
                    AmountTotalKredit += data.TotalKredit;
                    AmountTotalKreditIDR += (double)data.TotalIDRKredit;
                    AmountTotalEndingBalance += (double)data.TotalEndingBalance;
                    AmountTotalEndingBalanceIDR += (double)data.TotalIDREndingBalance;
                    result.Rows.Add(data.SupplierName, data.DOCurrencyCode, TotalInitialBalance, TotalCurrencyInitialBalance, TotalIDR, data.BillNo, data.PaymentBill, data.DebtAge, data.INNo, InitialBalance, data.CurrencyInitialBalance, IDR, TotalDebit, CurrencyDebit, TotalIDRDebit, data.NoDebit, TglDebit, TotalKredit, CurrencyKredit, TotalIDRKredit, data.DifferenceRate, TotalEndingBalance, CurrencyEndingBalance, TotalIDREndingBalance);
                    rowPosition += 1;
                }
                var supp = supplier.Distinct().ToArray();
                foreach (var i in supp)
                {
                    //TotalPerSupplier[i] = TotalPerSupplier[i] / SupplierCount[i];
                    //TotalIDRPerSupplier[i] = TotalIDRPerSupplier[i] / SupplierCount[i];

                    AmountTotalInitialBalance += (double)TotalPerSupplier[i];
                    AmountTotalIDRInitialBalance += (double)TotalIDRPerSupplier[i];
                }
                string AmountTotalstr = string.Format("{0:N2}", AmountTotal);
                string AmountTotalIDRstr = string.Format("{0:N2}", AmountTotalIDR);
                string AmountTotalDebitstr = string.Format("{0:N2}", AmountTotalDebit);
                string AmountTotalDebitIDRstr = string.Format("{0:N2}", AmountTotalDebitIDR);
                string AmountTotalKreditstr = string.Format("{0:N2}", AmountTotalKredit);
                string AmountTotalKreditIDRstr = string.Format("{0:N2}", AmountTotalKreditIDR);
                string AmountTotalEndingBalancestr = string.Format("{0:N2}", AmountTotalEndingBalance);
                string AmountTotalEndingBalanceIDRstr = string.Format("{0:N2}", AmountTotalEndingBalanceIDR);
                string AmountTotalInitialBalancestr = string.Format("{0:N2}", AmountTotalInitialBalance);
                string AmountTotalIDRInitialBalancestr = string.Format("{0:N2}", AmountTotalIDRInitialBalance);

                result.Rows.Add("TOTAL", "", AmountTotalInitialBalancestr, "", AmountTotalIDRInitialBalancestr, "", "", "", "", AmountTotalstr, "", AmountTotalIDRstr, AmountTotalDebitstr, "", AmountTotalDebitIDRstr, "", "", AmountTotalKreditstr, "", AmountTotalKreditIDRstr, "", AmountTotalEndingBalancestr, "", AmountTotalEndingBalanceIDRstr);
                rowPosition += 1;
                var sheet = package.Workbook.Worksheets.Add("Data");
                var colkop = (char)('A' + headers.Length - 1);
                //var colkop2 = (char)('A' + headers.Length);
                //var colkop3 = (char)('B' + headers.Length);
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
                //Style.Border.B(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                //sheet.Cells[$"A1:{colkop}{headers.Length - 1}"].Style.;
                //sheet.Cells[$"A1:{colkop}{headers.Length - 1}"].Style.
                sheet.Cells["C5"].Value = headers[2];
                sheet.Cells["C5:E5"].Merge = true;
                sheet.Cells["C5:E5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["C5:E5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["C5:E5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["J5"].Value = headers[9];
                sheet.Cells["J5:L5"].Merge = true;
                sheet.Cells["J5:L5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["J5:L5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["J5:L5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["M5"].Value = headers[12];
                sheet.Cells["M5:Q5"].Merge = true;
                sheet.Cells["M5:Q5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["M5:Q5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["M5:Q5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["R5"].Value = headers[17];
                sheet.Cells["R5:T5"].Merge = true;
                sheet.Cells["R5:T5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["R5:T5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["R5:T5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["V5"].Value = headers[21];
                sheet.Cells["V5:X5"].Merge = true;
                sheet.Cells["V5:X5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["V5:X5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["V5:X5"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells["U5"].Value = headers[20];
                sheet.Cells["U5:U6"].Merge = true;
                sheet.Cells["U5:U6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells["U5:U6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells["U5:U6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                sheet.Cells[$"A{rowPosition + 5}:B{ rowPosition + 5}"].Merge = true;
                sheet.Cells[$"A{rowPosition + 5}:B{ rowPosition + 5}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[$"F{rowPosition + 5}:I{ rowPosition + 5}"].Merge = true;
                sheet.Cells[$"P{rowPosition + 5}:Q{ rowPosition + 5}"].Merge = true;
                sheet.Cells[$"A{rowPosition + 5}:X{ rowPosition + 5}"].Style.Font.Bold = true;
                #endregion
                #region Merge
                foreach (var i in Enumerable.Range(0, 2))
                {
                    var col = (char)('A' + i);
                    sheet.Cells[$"{col}5"].Value = headers[i];
                    sheet.Cells[$"{col}5:{col}6"].Merge = true;
                    sheet.Cells[$"{col}5:{col}6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                for (var i = 0; i < 4; i++)
                {
                    var col = (char)('F' + i);
                    sheet.Cells[$"{col}5"].Value = headers[i + 5];
                    sheet.Cells[$"{col}5:{col}6"].Merge = true;
                    sheet.Cells[$"{col}5:{col}6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"{col}5:{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                for (var i = 0; i < 3; i++)
                {
                    var col = (char)('C' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                }
                for (var i = 0; i < 3; i++)
                {
                    var col = (char)('J' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i + 3];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                }
                for (var i = 0; i < 5; i++)
                {
                    var col = (char)('M' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i + 6];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                for (var i = 0; i < 3; i++)
                {
                    var col = (char)('R' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i + 11];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                for (var i = 0; i < 3; i++)
                {
                    var col = (char)('V' + i);
                    sheet.Cells[$"{col}6"].Value = headers2[i + 14];
                    sheet.Cells[$"{col}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }

                int index = 7;
                foreach (KeyValuePair<string, int> c in SupplierCurCodeCount)
                {
                    sheet.Cells["A" + index + ":A" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["A" + index + ":A" + (index + c.Value - 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["C" + index + ":C" + (index + c.Value - 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["D" + index + ":D" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["D" + index + ":D" + (index + c.Value - 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    index += c.Value;
                }

                #endregion
                var widths = new int[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };
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
