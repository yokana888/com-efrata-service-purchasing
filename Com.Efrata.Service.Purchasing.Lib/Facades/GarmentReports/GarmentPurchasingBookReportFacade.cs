using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchasingBookReportViewModel;
using Microsoft.EntityFrameworkCore;
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
    public class GarmentPurchasingBookReportFacade : IGarmentPurchasingBookReportFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public GarmentPurchasingBookReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }
        public Tuple<List<GarmentPurchasingBookReportViewModel>, int> GetBookReport(int offset, bool? suppliertype, string suppliercode, string tipebarang, int page, int size, string Order, DateTime? dateFrom, DateTime? dateTo)
        {
            var Query = GetBookQuery(tipebarang, suppliertype, suppliercode, dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono).ToList();
            List<GarmentPurchasingBookReportViewModel> Data = Query;
            int TotalData = Data.Count();
            return Tuple.Create(Data, TotalData);
        }
        public List<GarmentPurchasingBookReportViewModel> GetBookQuery(string ctg, bool? suppliertype, string suppliercode, DateTime? datefrom, DateTime? dateto, int offset)
        {
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;

            IQueryable<GarmentPurchasingBookReportViewModel> d1 = from a in dbContext.GarmentInvoices
                                                                  join b in dbContext.GarmentInvoiceItems on a.Id equals b.InvoiceId
                                                                  join c in dbContext.GarmentInvoiceDetails on b.Id equals c.InvoiceItemId
                                                                  join d in dbContext.GarmentDeliveryOrderDetails on c.DODetailId equals d.Id
                                                                  join e in dbContext.GarmentDeliveryOrderItems on d.GarmentDOItemId equals e.Id
                                                                  join f in dbContext.GarmentDeliveryOrders on e.GarmentDOId equals f.Id
                                                                  join g in dbContext.GarmentBeacukais on f.CustomsId equals g.Id
                                                                  join h in dbContext.GarmentExternalPurchaseOrders on e.EPOId equals h.Id
                                                                  //join i in dbContext.GarmentInternNotes on f.InternNo equals i.INNo
                                                                  where c.DOQuantity != 0
                                                                  && h.SupplierImport == (suppliertype.HasValue ? suppliertype : h.SupplierImport)
                                                                  && (string.IsNullOrWhiteSpace(suppliercode) ? true : f.SupplierCode == suppliercode)
                                                                  && f.ArrivalDate >= DateFrom.Date && f.ArrivalDate <= DateTo.Date
                                                                  && d.CodeRequirment == (string.IsNullOrWhiteSpace(ctg) ? d.CodeRequirment : ctg)
                                                                  && !g.BeacukaiNo.Contains("BCDL")
                                                                  && a.SupplierCode != "GDG"
                                                                  select new GarmentPurchasingBookReportViewModel
                                                                  {
                                                                      BillNo = f.BillNo,
                                                                      CurrencyCode = f.DOCurrencyCode,
                                                                      CurrencyRate = f.DOCurrencyRate,
                                                                      Dono = f.DONo,
                                                                      internDate = f.ArrivalDate != null ? f.ArrivalDate : g.BeacukaiDate,
                                                                      InternNo = f.InternNo,
                                                                      InvoiceNo = a.InvoiceNo,
                                                                      SupplierName = f.SupplierName,
                                                                      PaymentBill = f.PaymentBill,
                                                                      Tipe = d.CodeRequirment,
                                                                      priceTotal = f.TotalAmount,
                                                                      dpp = f.TotalAmount,
                                                                      ppn = 0,
                                                                      total = f.TotalAmount * f.DOCurrencyRate,
                                                                      totalppn = 0,
                                                                      paymentduedate = f.DODate.AddDays(e.PaymentDueDays)
                                                                  };

            IQueryable<GarmentPurchasingBookReportViewModel> d2 = from gc in dbContext.GarmentCorrectionNotes
                                                                  join gci in dbContext.GarmentCorrectionNoteItems on gc.Id equals gci.GCorrectionId
                                                                  join ipo in dbContext.GarmentInternalPurchaseOrders on gci.POId equals ipo.Id
                                                                  join gdd in dbContext.GarmentDeliveryOrderDetails on gci.DODetailId equals gdd.Id
                                                                  join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                  join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                  join epo in dbContext.GarmentExternalPurchaseOrders on gci.EPOId equals epo.Id
                                                                  join invi in dbContext.GarmentInvoiceItems on gdo.Id equals invi.DeliveryOrderId
                                                                  join inv in dbContext.GarmentInvoices on invi.InvoiceId equals inv.Id
                                                                  join g in dbContext.GarmentBeacukais on gdo.CustomsId equals g.Id
                                                                  where gci.Quantity != 0
                                                                  && epo.SupplierImport == (suppliertype.HasValue ? suppliertype : epo.SupplierImport)
                                                                  && (string.IsNullOrWhiteSpace(suppliercode) ? true : gdo.SupplierCode == suppliercode)
                                                                  && gc.CorrectionDate.AddHours(offset).Date >= DateFrom.Date && gc.CorrectionDate.AddHours(offset).Date <= DateTo.Date
                                                                  && gc.SupplierCode != "GDG"
                                                                  && gdd.CodeRequirment == (string.IsNullOrWhiteSpace(ctg) ? gdd.CodeRequirment : ctg)
                                                                  && !g.BeacukaiNo.Contains("BCDL")
                                                                  group new { gc,gci,gdo,gdi,gdd,epo,inv,invi,g } by gdo.DONo into groupdata
                                                                  select new GarmentPurchasingBookReportViewModel
                                                                  {
                                                                      SupplierName = groupdata.FirstOrDefault().gdo.SupplierName,
                                                                      BillNo = groupdata.FirstOrDefault().gc.CorrectionNo,
                                                                      PaymentBill = groupdata.FirstOrDefault().gdo.PaymentBill,
                                                                      Dono = groupdata.FirstOrDefault().gdo.DONo,
                                                                      InvoiceNo = groupdata.FirstOrDefault().inv.InvoiceNo,
                                                                      InternNo = groupdata.FirstOrDefault().gdo.InternNo,
                                                                      Tipe = groupdata.FirstOrDefault().gdd.CodeRequirment,
                                                                      internDate = groupdata.FirstOrDefault().gdo.ArrivalDate != null ? groupdata.FirstOrDefault().gdo.ArrivalDate : groupdata.FirstOrDefault().g.BeacukaiDate,
                                                                      paymentduedate = groupdata.FirstOrDefault().gdo.DODate.AddDays(groupdata.FirstOrDefault().gdi.PaymentDueDays),
                                                                      priceTotal = groupdata.FirstOrDefault().gdo.TotalAmount,
                                                                      ppn = 0,
                                                                      total = Convert.ToDouble(groupdata.FirstOrDefault().gc.TotalCorrection) * groupdata.FirstOrDefault().gdo.DOCurrencyRate,
                                                                      totalppn = 0,
                                                                      dpp = Convert.ToDouble(groupdata.FirstOrDefault().gc.TotalCorrection),
                                                                      CurrencyCode = groupdata.FirstOrDefault().gdo.DOCurrencyCode,
                                                                      CurrencyRate = groupdata.FirstOrDefault().gdo.DOCurrencyRate
                                                                  };
            IQueryable<GarmentPurchasingBookReportViewModel> d3 = from inv in dbContext.GarmentInvoices
                                                                 join invi in dbContext.GarmentInvoiceItems on inv.Id equals invi.InvoiceId
                                                                 join invd in dbContext.GarmentInvoiceDetails on invi.Id equals invd.InvoiceItemId
                                                                 join gdd in dbContext.GarmentDeliveryOrderDetails on invd.DODetailId equals gdd.Id
                                                                 join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                 join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                 join epo in dbContext.GarmentExternalPurchaseOrders on invd.EPOId equals epo.Id
                                                                 join g in dbContext.GarmentBeacukais on gdo.CustomsId equals g.Id
                                                                 where invd.DOQuantity != 0
                                                                 && epo.SupplierImport == (suppliertype.HasValue ? suppliertype : epo.SupplierImport)
                                                                 && inv.IsPayTax == true
                                                                 && inv.UseVat == true
                                                                 && inv.NPN != null
                                                                 && (string.IsNullOrWhiteSpace(suppliercode) ? true : gdo.SupplierCode == suppliercode)
                                                                 && inv.InvoiceDate.AddHours(offset).Date >= DateFrom.Date && inv.InvoiceDate.AddHours(offset).Date <= DateTo.Date
                                                                 && gdd.CodeRequirment == (string.IsNullOrWhiteSpace(ctg) ? gdd.CodeRequirment : ctg)
                                                                 && inv.SupplierCode != "GDG"
                                                                 && !g.BeacukaiNo.Contains("BCDL")
                                                                 select new GarmentPurchasingBookReportViewModel
                                                                 {
                                                                     SupplierName = gdo.SupplierName,
                                                                     BillNo = inv.NPN,
                                                                     PaymentBill = gdo.PaymentBill,
                                                                     Dono = gdo.DONo,
                                                                     InvoiceNo = inv.InvoiceNo,
                                                                     InternNo = gdo.InternNo,
                                                                     Tipe = gdd.CodeRequirment,
                                                                     internDate = gdo.ArrivalDate != null ? gdo.ArrivalDate : g.BeacukaiDate,
                                                                     paymentduedate = gdo.DODate.AddDays(gdi.PaymentDueDays),
                                                                     priceTotal = (inv.TotalAmount * 10) / 100,
                                                                     ppn = (inv.TotalAmount * 10) / 100,
                                                                     dpp = 0,
                                                                     total = ((inv.TotalAmount * 10) / 100) * gdo.DOCurrencyRate,
                                                                     totalppn = ((inv.TotalAmount * 10) / 100) * gdo.DOCurrencyRate,
                                                                     CurrencyCode = gdo.DOCurrencyCode,
                                                                     CurrencyRate = gdo.DOCurrencyRate
                                                                 };
            IQueryable<GarmentPurchasingBookReportViewModel> d4 = from inv in dbContext.GarmentInvoices
                                                                  join invi in dbContext.GarmentInvoiceItems on inv.Id equals invi.InvoiceId
                                                                  join invd in dbContext.GarmentInvoiceDetails on invi.Id equals invd.InvoiceItemId
                                                                  join gdd in dbContext.GarmentDeliveryOrderDetails on invd.DODetailId equals gdd.Id
                                                                  join gdi in dbContext.GarmentDeliveryOrderItems on gdd.GarmentDOItemId equals gdi.Id
                                                                  join gdo in dbContext.GarmentDeliveryOrders on gdi.GarmentDOId equals gdo.Id
                                                                  join epo in dbContext.GarmentExternalPurchaseOrders on invd.EPOId equals epo.Id
                                                                  join g in dbContext.GarmentBeacukais on gdo.CustomsId equals g.Id
                                                                  where invd.DOQuantity != 0
                                                                  && epo.SupplierImport == (suppliertype.HasValue ? suppliertype : epo.SupplierImport)
                                                                  && inv.IsPayTax == true
                                                                  && inv.UseIncomeTax == true
                                                                  && inv.NPH != null
                                                                  && (string.IsNullOrWhiteSpace(suppliercode) ? true : gdo.SupplierCode == suppliercode)
                                                                  && inv.InvoiceDate.AddHours(offset).Date >= DateFrom.Date && inv.InvoiceDate.AddHours(offset).Date <= DateTo.Date
                                                                  && gdd.CodeRequirment == (string.IsNullOrWhiteSpace(ctg) ? gdd.CodeRequirment : ctg)
                                                                  && inv.SupplierCode != "GDG"
                                                                  && !g.BeacukaiNo.Contains("BCDL")
                                                                  select new GarmentPurchasingBookReportViewModel
                                                                  {
                                                                      SupplierName = gdo.SupplierName,
                                                                      BillNo = inv.NPH,
                                                                      PaymentBill = gdo.PaymentBill,
                                                                      Dono = gdo.DONo,
                                                                      InvoiceNo = inv.InvoiceNo,
                                                                      InternNo =gdo.InternNo,
                                                                      Tipe = gdd.CodeRequirment,
                                                                      internDate = gdo.ArrivalDate != null ? gdo.ArrivalDate : g.BeacukaiDate,
                                                                      paymentduedate = gdo.DODate.AddDays(gdi.PaymentDueDays),
                                                                      priceTotal = gdo.TotalAmount,
                                                                      ppn = (inv.TotalAmount * 10) / 100 ,
                                                                      totalppn = (inv.IncomeTaxRate * inv.TotalAmount) / 100 ,
                                                                      total = (inv.IncomeTaxRate * inv.TotalAmount) / 100 * gdo.DOCurrencyRate,
                                                                      dpp = 0,
                                                                      CurrencyCode = gdo.DOCurrencyCode,
                                                                      CurrencyRate = gdo.DOCurrencyRate
                                                                  };
            List<GarmentPurchasingBookReportViewModel> Combine = d1.Union(d2).Union(d3).Union(d4).ToList();
            var DataCoba = (from a in Combine
                           group a by new
                           {
                               a.SupplierName,
                               a.BillNo,
                               a.PaymentBill,
                               a.Dono,
                               a.InvoiceNo,
                               a.InternNo,
                               a.Tipe,
                               a.internDate,
                               a.paymentduedate,
                               a.priceTotal,
                               a.ppn,
                               a.totalppn,
                               a.total,
                               a.dpp,
                               a.CurrencyCode,
                               a.CurrencyRate
                           } into groupdata
                           select new GarmentPurchasingBookReportViewModel
                           {
                               SupplierName = groupdata.Key.SupplierName,
                               BillNo = groupdata.Key.BillNo,
                               PaymentBill = groupdata.Key.PaymentBill,
                               Dono = groupdata.Key.Dono,
                               InvoiceNo = groupdata.Key.InvoiceNo,
                               InternNo = groupdata.Key.InternNo,
                               Tipe = groupdata.Key.Tipe,
                               internDate = groupdata.Key.internDate,
                               paymentduedate = groupdata.Key.paymentduedate,
                               priceTotal = groupdata.Key.priceTotal,
                               ppn = groupdata.Key.ppn,
                               totalppn = groupdata.Key.totalppn,
                               total = groupdata.Key.total,
                               dpp = groupdata.Key.dpp,
                               CurrencyCode = groupdata.Key.CurrencyCode,
                               CurrencyRate = groupdata.Key.CurrencyRate
                           }).ToList();
            

            return DataCoba;



        }
        public MemoryStream GenerateExcelBookReport(string ctg, bool? suppliertype, string suppliercode, DateTime? datefrom, DateTime? dateto, int offset)
        {
            var Query = GetBookQuery(ctg, suppliertype, suppliercode, datefrom, dateto, offset);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Nomor", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Jatuh Tempo", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "DPP", DataType = typeof(Decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Valas", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(Decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total(IDR)", DataType = typeof(Decimal) });

            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", 0, 0, 0);
            else
            {
                Dictionary<string, List<GarmentPurchasingBookReportViewModel>> bySupplier = new Dictionary<string, List<GarmentPurchasingBookReportViewModel>>();
                Dictionary<string, double> subTotalDPP = new Dictionary<string, double>();
                Dictionary<string, double> subTotalPPN = new Dictionary<string, double>();
                Dictionary<string, double?> subTotal = new Dictionary<string, double?>();
                foreach (GarmentPurchasingBookReportViewModel data in Query)
                {
                    string supplierName = data.SupplierName;
                    if (!bySupplier.ContainsKey(supplierName))
                        bySupplier.Add(supplierName, new List<GarmentPurchasingBookReportViewModel> { });
                    bySupplier[supplierName].Add(new GarmentPurchasingBookReportViewModel
                    {
                        BillNo = data.BillNo,
                        CurrencyCode = data.CurrencyCode,
                        CurrencyRate = data.CurrencyRate,
                        Dono = data.Dono,
                        dpp = data.dpp,
                        internDate = data.internDate,
                        InvoiceNo = data.InvoiceNo,
                        PaymentBill = data.PaymentBill,
                        paymentduedate = data.paymentduedate,
                        ppn = data.ppn,
                        priceTotal = data.priceTotal,
                        SupplierName = data.SupplierName,
                        Tipe = data.Tipe,
                        total = data.total,
                        totalppn = data.totalppn
                    });
                    if (!subTotalDPP.ContainsKey(supplierName))
                    {
                        subTotalDPP.Add(supplierName, 0);
                    }
                    if (!subTotalPPN.ContainsKey(supplierName))
                    {
                        subTotalPPN.Add(supplierName, 0);
                    }
                    if (!subTotal.ContainsKey(supplierName))
                    {
                        subTotal.Add(supplierName, 0);
                    }

                    subTotalDPP[supplierName] += data.dpp;
                    subTotalPPN[supplierName] += data.ppn;
                    subTotal[supplierName] += data.total;
                }

                double TotalDpp = 0;
                double TotalPPn = 0;
                double? jmlTotal = 0;

                int rowPosition = 6;

                foreach (KeyValuePair<string, List<GarmentPurchasingBookReportViewModel>> supplName in bySupplier)
                {
                    string supplierName = "";
                    int index = 0;
                    foreach (GarmentPurchasingBookReportViewModel item in supplName.Value)
                    {
                        index++;
                        result.Rows.Add(index, item.SupplierName, item.BillNo, item.PaymentBill, item.Dono, item.InvoiceNo, item.InternNo, item.Tipe, item.internDate == null ? "-" : item.internDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.paymentduedate == new DateTime(1970, 1, 1) ? "-" : item.paymentduedate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID")), (Decimal)Math.Round((item.dpp), 2), item.CurrencyCode, item.CurrencyRate, (Decimal)Math.Round(Convert.ToDecimal(item.totalppn), 2), (Decimal)Math.Round(Convert.ToDecimal(item.total), 2));
                        rowPosition += 1;
                        supplierName = item.SupplierName;
                    }
                    result.Rows.Add("SUB TOTAL", "", "", "", "", "", "", "", "", supplierName, Math.Round(subTotalDPP[supplName.Key], 2), "", 0, Math.Round(subTotalPPN[supplName.Key], 2), Math.Round(Convert.ToDouble(subTotal[supplName.Key]), 2));

                    rowPosition += 1;
                    mergeCells.Add(($"A{rowPosition}:D{rowPosition}", OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom));

                    TotalDpp += subTotalDPP[supplName.Key];
                    TotalPPn += subTotalPPN[supplName.Key];
                    jmlTotal += subTotal[supplName.Key];
                }
                result.Rows.Add("SUB TOTAL", "", "", "", "", "", "", "", "", "", Math.Round(TotalDpp, 2), "", 0, Math.Round(TotalPPn, 2), Math.Round(Convert.ToDouble(jmlTotal), 2));

                rowPosition += 1;
                mergeCells.Add(($"A{rowPosition}:D{rowPosition}", OfficeOpenXml.Style.ExcelHorizontalAlignment.Right, OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom));

            }
            ExcelPackage package = new ExcelPackage();
            DateTime DateFrom = datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)datefrom;
            DateTime DateTo = dateto == null ? DateTime.Now : (DateTime)dateto;
            CultureInfo Id = new CultureInfo("id-ID");
            string Month = Id.DateTimeFormat.GetMonthName(DateTo.Month);
            var sheet = package.Workbook.Worksheets.Add("Report");

            #region Kop Table
            var col = (char)('A' + result.Columns.Count);
            sheet.Cells[$"A1:{col}1"].Value = "BUKU PEMBELIAN DIV. GARMENT";
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("Bulan {0} {1}", Month, DateTo.Year);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("{0}", ctg == "BB" ? "BAHAN BAKU" : ctg == "BP" ? "BAHAN PENDUKUNG" : ctg == "BE" ? "BAHAN EMBALANCE" : ctg == "PRC" ? "PROSES" : "ALL");
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A4:{col}4"].Value = string.Format("Supplier : {0}", suppliertype.HasValue ? suppliertype == true ? "IMPORT" : "LOCAL" : "ALL");
            sheet.Cells[$"A4:{col}4"].Merge = true;
            sheet.Cells[$"A4:{col}4"].Style.Font.Bold = true;
            sheet.Cells[$"A4:{col}4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A4:{col}4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            #endregion

            foreach (var i in Enumerable.Range(0, result.Columns.Count))
            {
                var colheader = (char)('A' + i);
                sheet.Cells[$"{colheader}6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            }
            sheet.Cells["A6"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);
            foreach ((string cells, Enum hAlign, Enum vAlign) in mergeCells)
            {
                sheet.Cells[cells].Merge = true;
                sheet.Cells[cells].Style.HorizontalAlignment = (OfficeOpenXml.Style.ExcelHorizontalAlignment)hAlign;
                sheet.Cells[cells].Style.VerticalAlignment = (OfficeOpenXml.Style.ExcelVerticalAlignment)vAlign;
            }
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
    }
}
