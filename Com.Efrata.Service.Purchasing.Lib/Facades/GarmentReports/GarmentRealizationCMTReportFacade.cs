using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentExpenditureGood;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentInvoice;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentRealizationCMTReportFacade : IGarmentRealizationCMTReportFacade
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly PurchasingDbContext dbContext;

        public GarmentRealizationCMTReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
        }

        public IQueryable<GarmentRealizationCMTReportViewModel> GetQuery(DateTime? dateFrom, DateTime? dateTo, string unit, int offset)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var packinginventories = GetInvoiceFromPacking(DateFrom, DateTo);

            var invoicess = packinginventories.Select(x => x.InvoiceNo).ToList();

            var invoices = string.Join(",", packinginventories.Select(x => x.Ronos));

            var expendRo = GetExpenditureGood(invoices);

            var Ros = expendRo.Where(x => invoicess.Contains(x.Invoice)).Select(x => x.RONo).Distinct().ToArray();

            string[] paymentmethods = { "FREE FROM BUYER", "CMT" };

            List<GarmentRealizationCMTReportViewModel> realizationCMT = new List<GarmentRealizationCMTReportViewModel>();

            var Query = (from a in dbContext.GarmentUnitExpenditureNotes
                         join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                         join c in dbContext.GarmentUnitDeliveryOrders on a.UnitDONo equals c.UnitDONo
                         join i in dbContext.GarmentUnitDeliveryOrderItems on c.Id equals i.UnitDOId
                         join e in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals e.Id
                         join d in dbContext.GarmentUnitReceiptNotes on e.URNId equals d.Id
                         join f in dbContext.GarmentDeliveryOrderDetails on e.DODetailId equals f.Id
                         join g in dbContext.GarmentDeliveryOrderItems on f.GarmentDOItemId equals g.Id
                         join h in dbContext.GarmentDeliveryOrders on g.GarmentDOId equals h.Id
                         //join j in dbContext.GarmentExternalPurchaseOrderItems.IgnoreQueryFilters() on c.RONo equals j.RONo
                         //join k in dbContext.GarmentExternalPurchaseOrders.IgnoreQueryFilters() on j.GarmentEPOId equals k.Id
                         where
                               //a.ExpenditureDate.AddHours(offset).Date >= DateFrom.Date
                               //      && a.ExpenditureDate.AddHours(offset).Date <= DateTo.Date
                               //&& 
                               a.UnitSenderCode == (string.IsNullOrWhiteSpace(unit) ? a.UnitSenderCode : unit)
                               && b.ProductName == "FABRIC"
                               && a.ExpenditureType == "PROSES"
                               && Ros.Contains(c.RONo)
                               && paymentmethods.Contains(h.PaymentMethod)

                         //&& k.PaymentMethod == paymentmethod

                         select new GarmentRealizationCMTReportViewModel
                         {
                             UENNo = a.UENNo,
                             ProductRemark = b.ProductRemark.Trim(),
                             Quantity = b.Quantity,
                             EAmountVLS = (decimal)b.Quantity * (decimal)b.PricePerDealUnit,
                             EAmountIDR = (decimal)b.Quantity * (decimal)b.PricePerDealUnit * (decimal)h.DOCurrencyRate,
                             RONo = c.RONo,
                             URNNo = d.URNNo,
                             ProductRemark2 = e.ProductRemark.Trim(),
                             ReceiptQuantity = e.ReceiptQuantity,
                             UAmountVLS = e.ReceiptQuantity * e.PricePerDealUnit,
                             UAmountIDR = e.ReceiptQuantity * e.PricePerDealUnit * (decimal)h.DOCurrencyRate,
                             SupplierName = h.SupplierName == null ? "-" : h.SupplierName,
                             BillNo = h.BillNo,
                             PaymentBill = h.PaymentBill,
                             DONo = h.DONo,
                             NoKasBank = paymentmethods.Contains(h.PaymentMethod) ? "-" : "BY DANLIRIS"

                         }).GroupBy(a => new { a.UENNo, a.RONo, a.URNNo, a.BillNo, a.PaymentBill, a.ProductRemark, a.ProductRemark2, a.SupplierName, a.DONo, a.EAmountVLS, a.EAmountIDR, a.ReceiptQuantity, a.UAmountIDR, a.UAmountVLS, a.Quantity, a.NoKasBank }, (key, group) => new GarmentRealizationCMTReportViewModel
                         {
                             UENNo = key.UENNo,
                             ProductRemark = key.ProductRemark,
                             //Quantity = group.Sum(x => x.Quantity),
                             Quantity = key.Quantity,
                             //EAmountVLS = group.Sum(x => x.EAmountVLS),
                             EAmountVLS = key.EAmountVLS,
                             //EAmountIDR = group.Sum(x => x.EAmountIDR),
                             EAmountIDR = key.EAmountVLS,
                             RONo = key.RONo,
                             URNNo = key.URNNo,
                             ProductRemark2 = key.ProductRemark2,
                             //ReceiptQuantity = group.Sum(x => x.ReceiptQuantity),
                             ReceiptQuantity = key.ReceiptQuantity,
                             //UAmountVLS = group.Sum(x => x.UAmountVLS),
                             UAmountVLS = key.UAmountVLS,
                             //UAmountIDR = group.Sum(x => x.UAmountIDR),
                             UAmountIDR = key.UAmountIDR,
                             SupplierName = key.SupplierName,
                             BillNo = key.BillNo,
                             PaymentBill = key.PaymentBill,
                             DONo = key.DONo,
                             NoKasBank = key.NoKasBank
                         });

            var realization = (from a in Query
                               join expenditure in (from bb in expendRo where bb.ExpenditureType == "EXPORT" select bb) on a.RONo equals expenditure.RONo /*into expend*/
                               join v in dbContext.GarmentInvoiceItems on a.DONo equals v.DeliveryOrderNo into invoiceitems
                               from vv in invoiceitems.DefaultIfEmpty()
                               where invoicess.Contains(expenditure.Invoice)
                               //from expenditure in expend.DefaultIfEmpty()
                               select new GarmentRealizationCMTReportViewModel
                               {
                                   UENNo = a.UENNo,
                                   ProductRemark = a.ProductRemark,
                                   Quantity = a.Quantity,
                                   EAmountVLS = a.EAmountVLS,
                                   EAmountIDR = a.EAmountIDR,
                                   RONo = a.RONo,
                                   URNNo = a.URNNo,
                                   ProductRemark2 = a.ProductRemark2,
                                   ReceiptQuantity = a.ReceiptQuantity,
                                   UAmountVLS = a.UAmountVLS,
                                   UAmountIDR = a.UAmountIDR,
                                   SupplierName = a.SupplierName,
                                   BillNo = a.BillNo,
                                   PaymentBill = a.PaymentBill,
                                   DONo = a.DONo,
                                   InvoiceNo = expenditure == null ? "-" : expenditure.Invoice,
                                   ExpenditureGoodNo = expenditure == null ? "-" : expenditure.ExpenditureGoodNo,
                                   Article = expenditure == null ? "-" : expenditure.Article,
                                   UnitQty = expenditure == null ? 0 : expenditure.TotalQuantity,
                                   InvoiceId = vv != null ? vv.InvoiceId : 0,
                                   NoKasBank = a.NoKasBank
                               });

            realization = realization.OrderBy(x => x.InvoiceNo).ThenBy(x => x.ExpenditureGoodNo).ThenBy(x => x.RONo).ThenBy(x => x.Article).ThenBy(x => x.UnitQty).ThenBy(x => x.UENNo).ThenBy(x => x.ProductRemark)
                .ThenBy(x => x.Quantity).ThenBy(x => x.EAmountVLS).ThenBy(x => x.EAmountIDR).ThenBy(x => x.URNNo).ThenBy(x => x.ProductRemark2).ThenBy(x => x.ReceiptQuantity).ThenBy(x => x.UAmountVLS).ThenBy(x => x.UAmountIDR).ThenBy(x => x.SupplierName).ThenBy(x => x.BillNo)
                .ThenBy(x => x.PaymentBill).ThenBy(x => x.DONo);

            var invoiceids = realization.Select(x => x.InvoiceId).Distinct().ToList();

            var nokasbanks = new List<DPPVATBankExpenditureNoteViewModel>();

            foreach (var i in invoiceids)
            {
                var banks = GetNoKasBank(i);
                if(banks.ExpenditureNoteNo != null)
                {
                    nokasbanks.Add(banks);
                }
            }


            var result = (from i in realization
                          join c in nokasbanks on i.InvoiceId equals c.InvoiceId into nokas
                          from cc in nokas.DefaultIfEmpty()
                          select new GarmentRealizationCMTReportViewModel
                          {
                              UENNo = i.UENNo,
                              ProductRemark = i.ProductRemark,
                              Quantity = i.Quantity,
                              EAmountVLS = i.EAmountVLS,
                              EAmountIDR = i.EAmountIDR,
                              RONo = i.RONo,
                              URNNo = i.URNNo,
                              ProductRemark2 = i.ProductRemark2,
                              ReceiptQuantity = i.ReceiptQuantity,
                              UAmountVLS = i.UAmountVLS,
                              UAmountIDR = i.UAmountIDR,
                              SupplierName = i.SupplierName,
                              BillNo = i.BillNo,
                              PaymentBill = i.PaymentBill,
                              DONo = i.DONo,
                              InvoiceNo = i.InvoiceNo,
                              ExpenditureGoodNo = i.ExpenditureGoodNo,
                              Article = i.Article,
                              UnitQty = i.UnitQty,
                              Count = i.Count,
                              NoKasBank = cc != null ? cc.ExpenditureNoteNo : i.NoKasBank
                          }).ToList();

            var realizationViews = result.ToArray();
            var index = 0;
            foreach (GarmentRealizationCMTReportViewModel a in realizationViews)
            {
                GarmentRealizationCMTReportViewModel dup = Array.Find(realizationViews, o => o.InvoiceNo == a.InvoiceNo && o.ExpenditureGoodNo == a.ExpenditureGoodNo && o.Article == a.Article && o.UnitQty == a.UnitQty);
                if (dup != null)
                {
                    if (dup.Count == 0)
                    {
                        index++;
                        dup.Count = index;
                    }
                }
                a.Count = dup.Count;
            }

            return result.AsQueryable();

        }

        public Tuple<List<GarmentRealizationCMTReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string unit, int page, int size, string Order, int offset)
        {
            var Query = GetQuery(dateFrom, dateTo, unit, offset);

            //Query = Query.OrderBy(x => x.InvoiceNo).ThenBy(x => x.ExpenditureGoodNo).ThenBy(x => x.RONo).ThenBy(x => x.Article).ThenBy(x => x.UnitQty).ThenBy(x => x.UENNo).ThenBy(x => x.ProductRemark)
            //    .ThenBy(x => x.Quantity).ThenBy(x => x.EAmountVLS).ThenBy(x => x.EAmountIDR).ThenBy(x => x.URNNo).ThenBy(x => x.ProductRemark2).ThenBy(x => x.ReceiptQuantity).ThenBy(x => x.UAmountVLS).ThenBy(x => x.UAmountIDR).ThenBy(x => x.SupplierName).ThenBy(x => x.BillNo)
            //    .ThenBy(x => x.PaymentBill).ThenBy(x => x.DONo);

            //var b = Query.ToArray();
            //var index = 0;

            //foreach (GarmentRealizationCMTReportViewModel a in Query)
            //{
            //    GarmentRealizationCMTReportViewModel dup = Array.Find(b, o => o.InvoiceNo == a.InvoiceNo && o.ExpenditureGoodNo == a.ExpenditureGoodNo);
            //    if (dup != null)
            //    {
            //        if (dup.Count == 0)
            //        {
            //            index++;
            //            dup.Count = index;
            //        }
            //    }
            //    a.Count = dup.Count;
            //}

            //Pageable<GarmentRealizationCMTReportViewModel> pageable = new Pageable<GarmentRealizationCMTReportViewModel>(Query.OrderBy(o => o.Count).ThenBy(o => o.InvoiceNo).ThenBy(o => o.ExpenditureGoodNo), page - 1, size);
            Pageable<GarmentRealizationCMTReportViewModel> pageable = new Pageable<GarmentRealizationCMTReportViewModel>(Query, page - 1, size);
            List<GarmentRealizationCMTReportViewModel> Data = pageable.Data.ToList<GarmentRealizationCMTReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string unit, int offset, string unitname)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = GetQuery(dateFrom, dateTo, unit, offset);
            var headers = new string[] { "No", "No Invoice", "No. BON", "RO", "Artikel", "Qty BJ" /*"Fabric Cost"*/ };
            var subheaders = new string[] { "No. BON", "Keterangan", "Qty", "Amount Valas", "Amount IDR", "Asal", "No. BON", "Keterangan", "Qty", "Amount Valas", "Amount IDR", "Supplier", "No Nota", "No BON Kecil", "Surat Jalan", "No Kas Bank" };
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty BJ", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Fabric Cost", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Pemakaian", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Amount Valas - Pemakaian", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Amount IDR - Pemakaian", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON Peneriamaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Peneriamaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Peneriamaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Amount Valas - Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Amount IDR - Penerimaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BON Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Kas Bank", DataType = typeof(String) });

            ExcelPackage package = new ExcelPackage();
            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", 0, /*0,*/ "", "", 0, 0, 0, "", "", "", 0, 0, 0, "", "", "", "", "");
                var sheet = package.Workbook.Worksheets.Add("Data");
                sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);// to allow column name to be generated properly for empty data as template
            }
            else
            {
                var Qr = Query.ToArray();
                var q = Query.ToList();
                var index = 0;
                //foreach (GarmentRealizationCMTReportViewModel a in q)
                //{
                //    GarmentRealizationCMTReportViewModel dup = Array.Find(Qr, o => o.InvoiceNo == a.InvoiceNo /*&& o.ExpenditureGoodNo == a.ExpenditureGoodNo && o.Article == a.Article*/ && o.UnitQty == a.UnitQty);
                //    if (dup != null)
                //    {
                //        if (dup.Count == 0)
                //        {
                //            index++;
                //            dup.Count = index;
                //        }
                //    }
                //    a.Count = dup.Count;
                //}
                //Query = q.AsQueryable();

                foreach (var item in Query)
                {
                    index++;
                    result.Rows.Add(item.Count, item.InvoiceNo, item.ExpenditureGoodNo, item.RONo, item.Article, item.UnitQty,/* item.EGAmountIDR,*/ item.UENNo, item.ProductRemark, item.Quantity, item.EAmountVLS,
                                    item.EAmountIDR, item.RONo, item.URNNo, item.ProductRemark2, item.ReceiptQuantity, item.UAmountVLS, item.UAmountIDR, item.SupplierName, item.BillNo, item.PaymentBill, item.DONo, item.NoKasBank);
                }

                // bool styling = true;

                foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
                {
                    var sheet = package.Workbook.Worksheets.Add(item.Value);
                    #region KopTable
                    sheet.Cells[$"A1:U1"].Value = "LAPORAN DATA REALISASI CMT GARMENT";
                    sheet.Cells[$"A1:U1"].Merge = true;
                    sheet.Cells[$"A1:U1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A1:U1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A1:U1"].Style.Font.Bold = true;
                    sheet.Cells[$"A2:U2"].Value = string.Format("Periode Tanggal {0} s/d {1}", DateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID")), DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID")));
                    sheet.Cells[$"A2:U2"].Merge = true;
                    sheet.Cells[$"A2:U2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A2:U2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A2:U2"].Style.Font.Bold = true;
                    sheet.Cells[$"A3:U3"].Value = string.Format("Konfeksi {0}", string.IsNullOrWhiteSpace(unit) ? "ALL" : "EFR");
                    sheet.Cells[$"A3:U3"].Merge = true;
                    sheet.Cells[$"A3:U3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A3:U3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A3:U3"].Style.Font.Bold = true;
                    #endregion


                    sheet.Cells["A8"].LoadFromDataTable(item.Key, false, OfficeOpenXml.Table.TableStyles.Light16);
                    sheet.Cells["G6"].Value = "BON PEMAKAIAN";
                    sheet.Cells["G6:L6"].Merge = true;
                    sheet.Cells["G6:L6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["M6"].Value = "BON PENERIMAAN";
                    sheet.Cells["M6:V6"].Merge = true;
                    sheet.Cells["M6:V6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    foreach (var i in Enumerable.Range(0, 6))
                    {
                        var col = (char)('A' + i);
                        sheet.Cells[$"{col}6"].Value = headers[i];
                        sheet.Cells[$"{col}6:{col}7"].Merge = true;
                        sheet.Cells[$"{col}6:{col}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    }
                    foreach (var i in Enumerable.Range(0, 16))
                    {
                        var col = (char)('G' + i);
                        sheet.Cells[$"{col}7"].Value = subheaders[i];
                        sheet.Cells[$"{col}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    }
                    sheet.Cells["A6:V7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells["A6:V7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells["A6:V7"].Style.Font.Bold = true;
                    //sheet.Cells["C1:D1"].Merge = true;
                    //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //sheet.Cells["E1:F1"].Merge = true;
                    //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Dictionary<string, int> invoicespan = new Dictionary<string, int>();
                    Dictionary<string, int> invvoiceqtyspan = new Dictionary<string, int>();
                    Dictionary<string, int> uenspan = new Dictionary<string, int>();
                    Dictionary<string, int> remarkuenspan = new Dictionary<string, int>();
                    Dictionary<string, int> qtybukspan = new Dictionary<string, int>();
                    Dictionary<string, int> urnspan = new Dictionary<string, int>();
                    Dictionary<string, int> remarkurnspan = new Dictionary<string, int>();
                    Dictionary<string, int> qtyurnspan = new Dictionary<string, int>();
                    Dictionary<string, int> supplierspan = new Dictionary<string, int>();
                    Dictionary<string, int> billspan = new Dictionary<string, int>();
                    Dictionary<string, int> rouenspan = new Dictionary<string, int>();
                    var docNo = Query.ToArray();

                    int value;
                    foreach (var a in Query)
                    {
                        //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                        if (invoicespan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.RONo + a.Article + a.UnitQty.ToString(), out value))
                        {
                            invoicespan[a.InvoiceNo + a.ExpenditureGoodNo + a.RONo + a.Article + a.UnitQty.ToString()]++;
                        }
                        else
                        {
                            invoicespan[a.InvoiceNo + a.ExpenditureGoodNo + a.RONo + a.Article + a.UnitQty.ToString()] = 1;
                        }

                        if (invvoiceqtyspan.TryGetValue(a.InvoiceNo + a.UnitQty.ToString(), out value))
                        {
                            invvoiceqtyspan[a.InvoiceNo + a.UnitQty.ToString()]++;
                        }
                        else
                        {
                            invvoiceqtyspan[a.InvoiceNo + a.UnitQty.ToString()] = 1;
                        }

                        //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                        if (uenspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo, out value))
                        {
                            uenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo]++;
                        }
                        else
                        {
                            uenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo] = 1;
                        }

                        if (remarkuenspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.ProductRemark, out value))
                        {
                            remarkuenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.ProductRemark]++;
                        }
                        else
                        {
                            remarkuenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.ProductRemark] = 1;
                        }

                        if (qtybukspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.Quantity.ToString() + "qtybuk", out value))
                        {
                            qtybukspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.Quantity.ToString() + "qtybuk"]++;
                        }
                        else
                        {
                            qtybukspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.Quantity.ToString() + "qtybuk"] = 1;
                        }

                        if (urnspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo, out value))
                        {
                            urnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo]++;
                        }
                        else
                        {
                            urnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo] = 1;
                        }

                        if (remarkurnspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ProductRemark2, out value))
                        {
                            remarkurnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ProductRemark2]++;
                        }
                        else
                        {
                            remarkurnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ProductRemark2] = 1;
                        }

                        if (qtyurnspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ReceiptQuantity.ToString() + "qtyurn", out value))
                        {
                            qtyurnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ReceiptQuantity.ToString() + "qtyurn"]++;
                        }
                        else
                        {
                            qtyurnspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.ReceiptQuantity.ToString() + "qtyurn"] = 1;
                        }

                        if (supplierspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.SupplierName, out value))
                        {
                            supplierspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.SupplierName]++;
                        }
                        else
                        {
                            supplierspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.SupplierName] = 1;
                        }

                        if (billspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.BillNo + a.PaymentBill + a.DONo, out value))
                        {
                            billspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.BillNo + a.PaymentBill + a.DONo]++;
                        }
                        else
                        {
                            billspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.URNNo + a.BillNo + a.PaymentBill + a.DONo] = 1;
                        }

                        if (rouenspan.TryGetValue(a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.RONo, out value))
                        {
                            rouenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.RONo]++;
                        }
                        else
                        {
                            rouenspan[a.InvoiceNo + a.ExpenditureGoodNo + a.UENNo + a.RONo] = 1;
                        }
                    }

                    index = 8;
                    foreach (KeyValuePair<string, int> b in invoicespan)
                    {
                        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;



                        index += b.Value;
                    }

                    //index = 8;
                    //foreach (KeyValuePair<string, int> b in invoicespan)
                    //{
                        
                    //    sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Merge = true;
                    //    sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //    sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Merge = true;
                    //    sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //    sheet.Cells["W" + index + ":W" + (index + b.Value - 1)].Merge = true;
                    //    sheet.Cells["W" + index + ":W" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;


                    //    index += b.Value;
                    //}

                    index = 8;
                    foreach (KeyValuePair<string, int> c in uenspan)
                    {
                        sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }

                    index = 8;
                    foreach (KeyValuePair<string, int> c in remarkuenspan)
                    {
                        sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }

                    index = 8;
                    foreach (KeyValuePair<string, int> c in qtybukspan)
                    {
                        sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }

                    index = 8;
                    foreach (KeyValuePair<string, int> c in urnspan)
                    {
                        sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    index = 8;
                    foreach (KeyValuePair<string, int> c in remarkurnspan)
                    {
                        sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    index = 8;
                    foreach (KeyValuePair<string, int> c in qtyurnspan)
                    {
                        sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    index = 8;
                    foreach (KeyValuePair<string, int> c in supplierspan)
                    {
                        sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    index = 8;
                    foreach (KeyValuePair<string, int> c in billspan)
                    {
                        sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["V" + index + ":V" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["V" + index + ":V" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    index = 8;
                    foreach (KeyValuePair<string, int> c in rouenspan)
                    {
                        sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    //sheet.Cells[sheet.Dimension.Address].AutoFitColumns();


                }
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        //        public List<GarmentExpenditureGoodViewModel> GetExpenditureGood(string RONo)
        //        {

        //            string expenditureUri = "expenditure-goods/byRO";
        //           // string queryUri = "?filter=" + filter + "&keyword=" + keyword + "&order=" + order + "&page=" + page + "&size=" + size;
        //           // string uri = dispositionUri + queryUri;

        //            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

        //            var response = httpClient.GetAsync($"{APIEndpoint.GarmentProduction}{expenditureUri}?RONo={RONo}").Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = response.Content.ReadAsStringAsync().Result;
        //                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

        //                List<GarmentExpenditureGoodViewModel> viewModel;
        //                if (result.GetValueOrDefault("data") == null)
        //                {
        //                    viewModel = null;
        //                }
        //                else
        //                {
        ////                  var viewModels = JsonConvert.DeserializeObject<List<GarmentExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());
        ////                  viewModel = viewModels.Count() == 0 ? null : viewModels;
        //                    viewModel = JsonConvert.DeserializeObject<List<GarmentExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());

        //                }
        //                return viewModel;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }

        public List<GarmentExpenditureGoodViewModel> GetExpenditureGood(string invoices)
        {
            var param = new StringContent(JsonConvert.SerializeObject(invoices), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.GarmentProduction + $"expenditure-goods/traceable-by-ro";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentExpenditureGoodViewModel> viewModel;
                //if (result.GetValueOrDefault("data") == null)
                //{
                //    viewModel = new List<GarmentExpenditureGoodViewModel>();
                //}
                //else
                //{
                    viewModel = JsonConvert.DeserializeObject<List<GarmentExpenditureGoodViewModel>>(result.GetValueOrDefault("data").ToString());

                //}
                return viewModel;
            }
            else
            {
                return new List<GarmentExpenditureGoodViewModel>();
            }
        }

        public List<GarmentInvoiceMonitoringViewModel> GetInvoiceFromPacking(DateTime datefrom, DateTime dateTo)
        {
            //var param = new StringContent(JsonConvert.SerializeObject(RONo), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.PackingInventory + $"garment-shipping/monitoring/garment-cmt-sales";
            string queryUri = "?dateFrom=" + datefrom + "&dateTo=" + dateTo;

            string uri = expenditureUri + queryUri;

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var httpResponse = httpClient.GetAsync(uri).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<GarmentInvoiceMonitoringViewModel> viewModel;
                //if (result.GetValueOrDefault("data") == null)
                //{
                //    viewModel = new List<GarmentInvoiceMonitoringViewModel>();
                //}
                //else
                //{
                    viewModel = JsonConvert.DeserializeObject<List<GarmentInvoiceMonitoringViewModel>>(result.GetValueOrDefault("data").ToString());
                //}
                return viewModel;
            }
            else
            {
                return new List<GarmentInvoiceMonitoringViewModel>();
            }
        }

        public DPPVATBankExpenditureNoteViewModel GetNoKasBank(long donos)
        {
            var param = new StringContent(JsonConvert.SerializeObject(donos), Encoding.UTF8, "application/json");
            string expenditureUri = APIEndpoint.Finance + $"dpp-vat-bank-expenditure-notes/invoice/" + donos;
            //string queryUri = "?dateFrom=" + datefrom + "&dateTo=" + dateTo;

            //string uri = expenditureUri + queryUri;

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var httpResponse = httpClient.SendAsync(HttpMethod.Get, expenditureUri, param).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                DPPVATBankExpenditureNoteViewModel viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new DPPVATBankExpenditureNoteViewModel();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<DPPVATBankExpenditureNoteViewModel>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new DPPVATBankExpenditureNoteViewModel();
            }
        }


    }   
}
