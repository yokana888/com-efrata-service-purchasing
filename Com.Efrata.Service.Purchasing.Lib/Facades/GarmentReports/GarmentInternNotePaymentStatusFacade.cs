using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentInternNotePaymentStatusFacade : IGarmenInternNotePaymentStatusFacade
    {
        private readonly PurchasingDbContext dbContext;
        ILocalDbCashFlowDbContext dbContextLocal;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentInternNote> dbSet;

        public GarmentInternNotePaymentStatusFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext, ILocalDbCashFlowDbContext dbContextLocal)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbContextLocal = dbContextLocal;
            this.dbSet = dbContext.Set<GarmentInternNote>();
        }

        public IQueryable<GarmentInternNotePaymentStatusViewModel> GetQuery(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int offset)
        {
            DateTime DateNIFrom = dateNIFrom == null ? new DateTime(1070, 1, 1) : (DateTime)dateNIFrom;
            DateTime DateNITo = dateNITo == null ? DateTime.Now : (DateTime)dateNITo;
            DateTime DueDateFrom = dueDateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dueDateFrom;
            DateTime DueDateTo = dueDateTo == null ? DateTime.Now : (DateTime)dueDateTo;
            List<GarmentInternNotePaymentStatusViewModel> paymentStatus = new List<GarmentInternNotePaymentStatusViewModel>();
            var Query = (from a in dbContext.GarmentInternNotes
                        join b in dbContext.GarmentInternNoteItems on a.Id equals b.GarmentINId
                        join c in dbContext.GarmentInternNoteDetails on b.Id equals c.GarmentItemINId
                        join e in dbContext.GarmentInvoices on b.InvoiceId equals e.Id
                        join d in dbContext.GarmentDeliveryOrders on c.DOId equals d.Id
                        join f in dbContext.GarmentCorrectionNotes on d.Id equals f.DOId into correction
                        from t in correction.DefaultIfEmpty()
                        where a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && d.IsDeleted == false && e.IsDeleted == false //&& t.IsDeleted == false
                        && a.INNo == (String.IsNullOrWhiteSpace(inno) ? a.INNo : inno)
                        && b.InvoiceNo == (String.IsNullOrWhiteSpace(invono) ? b.InvoiceNo : invono)
                        && c.DONo == (String.IsNullOrWhiteSpace(dono) ? c.DONo : dono)
                        && d.BillNo == (String.IsNullOrWhiteSpace(billno) ? d.BillNo : billno)
                        && d.PaymentBill == (String.IsNullOrWhiteSpace(paymentbill) ? d.PaymentBill : paymentbill)
                        && (e.NPN != null ? e.NPN == (String.IsNullOrWhiteSpace(npn) ? e.NPN : npn) : true )
                        && t.CorrectionNo == (String.IsNullOrWhiteSpace(corrno) ? t.CorrectionNo : corrno)
                        && (e.NPH != null ? e.NPH == (String.IsNullOrWhiteSpace(nph) ? e.NPH : nph) : true ) 
                        && a.SupplierCode == (String.IsNullOrWhiteSpace(supplier) ? a.SupplierCode : supplier)
                        && a.INDate.AddHours(offset).Date >= DateNIFrom.Date
                        && a.INDate.AddHours(offset).Date <= DateNITo.Date
                        && c.PaymentDueDate.AddHours(offset).Date >= DueDateFrom.Date
                        && c.PaymentDueDate.AddHours(offset).Date <= DueDateTo.Date
                        select new GarmentInternNotePaymentStatusViewModel
                        {
                            INNo = a.INNo,
                            INDate = a.INDate,
                            SuppCode = a.SupplierCode,
                            SuppName = a.SupplierName,
                            PaymentMethod = c.PaymentMethod,
                            CurrCode = a.CurrencyCode,
                            PaymentDueDate = c.PaymentDueDate,
                            InvoiceNo = b.InvoiceNo,
                            InvoDate = b.InvoiceDate,
                            DoNo = c.DONo,
                            DoDate = c.DODate,
                            PriceTot = c.PriceTotal,
                            BillNo = d.BillNo,
                            PayBill = d.PaymentBill,
                            NPN = e.NPN == null ? "" : e.NPN,
                            VatDate = e.VatDate,
                            NPH = e.NPH == null ? "" : e.NPH,
                            IncomeTaxDate = e.IncomeTaxDate,
                            CorrNo = t.CorrectionNo ?? "",
                            CorrType = t.CorrectionType ?? "",
                            CorDate = t.CorrectionDate == null ? new DateTime(1970, 1, 1) : t.CorrectionDate,
                            Nomor = "",
                            Tgl = new DateTime(1970, 1, 1),
                            Jumlah = 0,
                            InvoiceId = e.Id

                        });
            var Data = from a in Query
                       group a by new { a.BillNo, a.DoNo } into datagroup

                       orderby datagroup.FirstOrDefault().InvoiceId
                       select new GarmentInternNotePaymentStatusViewModel
                       {
                           INNo = datagroup.FirstOrDefault().INNo,
                           INDate = datagroup.FirstOrDefault().INDate,
                           SuppCode = datagroup.FirstOrDefault().SuppCode,
                           SuppName = datagroup.FirstOrDefault().SuppName,
                           PaymentMethod = datagroup.FirstOrDefault().PaymentMethod,
                           CurrCode = datagroup.FirstOrDefault().CurrCode,
                           PaymentDueDate = datagroup.FirstOrDefault().PaymentDueDate,
                           InvoiceNo = datagroup.FirstOrDefault().InvoiceNo,
                           InvoDate = datagroup.FirstOrDefault().InvoDate,
                           DoNo = datagroup.FirstOrDefault().DoNo,
                           DoDate = datagroup.FirstOrDefault().DoDate,
                           PriceTot = datagroup.Sum(x => x.PriceTot),
                           BillNo = datagroup.FirstOrDefault().BillNo,
                           PayBill = datagroup.FirstOrDefault().PayBill,
                           NPN = datagroup.FirstOrDefault().NPN,
                           VatDate = datagroup.FirstOrDefault().VatDate,
                           NPH = datagroup.FirstOrDefault().NPH,
                           IncomeTaxDate = datagroup.FirstOrDefault().IncomeTaxDate,
                           CorrNo = datagroup.FirstOrDefault().CorrNo,
                           CorrType = datagroup.FirstOrDefault().CorrType,
                           CorDate = datagroup.FirstOrDefault().CorDate,
                           Nomor = datagroup.FirstOrDefault().Nomor,
                           Tgl = datagroup.FirstOrDefault().Tgl,
                           Jumlah = datagroup.FirstOrDefault().Jumlah,
                           InvoiceId = datagroup.FirstOrDefault().InvoiceId
                           
                       };

            //foreach (GarmentInternNotePaymentStatusViewModel i in Data)
            //{
            //    string cmddetail = "Select nomor,tgl,jumlah from RincianDetil where no_bon = '" + i.BillNo + "' and no_sj = '" + i.DoNo + "'";
            //    string cmdmemo = "Select nomor,tgl,jumlah from RincianMemo where no_bon = @bon";
            //    List<SqlParameter> parameters = new List<SqlParameter>();
            //    parameters.Add(new SqlParameter("bon", i.BillNo));
            //    parameters.Add(new SqlParameter("do", i.DoNo));
            //    List<SqlParameter> parameters2 = new List<SqlParameter>();
            //    parameters2.Add(new SqlParameter("bon", i.BillNo));

            //    var data = dbContextLocal.ExecuteReaderOnlyQuery(cmddetail);

            //    while (data.Read())
            //    {
            //        i.Nomor = data["nomor"].ToString();
            //        i.Tgl = data.GetDateTime(1);
            //        i.Jumlah = (decimal)data["jumlah"];
            //    };
            //    //data.Close();
            //    paymentStatus.Add(i);


            //    var data2 = dbContextLocal.ExecuteReader(cmdmemo, parameters2);
            //    while (data2.Read())
            //    {
            //        paymentStatus.Add(new GarmentInternNotePaymentStatusViewModel
            //        {
            //            BillNo = i.BillNo,
            //            CorDate = i.CorDate,
            //            CorrNo = i.CorrNo,
            //            CorrType = i.CorrType,
            //            CurrCode = i.CurrCode,
            //            DoDate = i.DoDate,
            //            DoNo = i.DoNo,
            //            IncomeTaxDate = i.IncomeTaxDate,
            //            INDate = i.INDate,
            //            INNo = i.INNo,
            //            InvoDate = i.InvoDate,
            //            InvoiceNo = i.InvoiceNo,
            //            NPH = i.NPH,
            //            NPN = i.NPN,
            //            PayBill = i.PayBill,
            //            PaymentDueDate = i.PaymentDueDate,
            //            PaymentMethod = i.PaymentMethod,
            //            PriceTot = i.PriceTot,
            //            SuppCode = i.SuppCode,
            //            SuppName = i.SuppName,
            //            VatDate = i.VatDate,
            //            Nomor = data2["nomor"].ToString(),
            //            Tgl = data2.GetDateTime(1),
            //            Jumlah = (decimal)data2["jumlah"]
            //        });
            //    }

            //    //data2.Close();
            //};
            foreach (GarmentInternNotePaymentStatusViewModel i in Data)
            {
                var data1 = GetInvoice(i.InvoiceId);

                //if (data1 == null)
                //{
                paymentStatus.Add(new GarmentInternNotePaymentStatusViewModel
                {
                    BillNo = i.BillNo,
                    CorDate = i.CorDate,
                    CorrNo = i.CorrNo,
                    CorrType = i.CorrType,
                    CurrCode = i.CurrCode,
                    DoDate = i.DoDate,
                    DoNo = i.DoNo,
                    IncomeTaxDate = i.IncomeTaxDate,
                    INDate = i.INDate,
                    INNo = i.INNo,
                    InvoDate = i.InvoDate,
                    InvoiceNo = i.InvoiceNo,
                    NPH = i.NPH,
                    NPN = i.NPN,
                    PayBill = i.PayBill,
                    PaymentDueDate = i.PaymentDueDate,
                    PaymentMethod = i.PaymentMethod,
                    PriceTot = i.PriceTot,
                    SuppCode = i.SuppCode,
                    SuppName = i.SuppName,
                    VatDate = i.VatDate,
                    Nomor = data1 == null ? "-" : data1.ExpenditureNoteNo,
                    Tgl = data1 == null ? new DateTime(1970, 1, 1) : data1.ExpenditureDate,
                    Jumlah = data1 == null? 0: (decimal)data1.AmountDetail
                    });
                    
                //}
                //else
                //{
                //    paymentStatus.Add(new GarmentInternNotePaymentStatusViewModel
                //    {
                //        BillNo = i.BillNo,
                //        CorDate = i.CorDate,
                //        CorrNo = i.CorrNo,
                //        CorrType = i.CorrType,
                //        CurrCode = i.CurrCode,
                //        DoDate = i.DoDate,
                //        DoNo = i.DoNo,
                //        IncomeTaxDate = i.IncomeTaxDate,
                //        INDate = i.INDate,
                //        INNo = i.INNo,
                //        InvoDate = i.InvoDate,
                //        InvoiceNo = i.InvoiceNo,
                //        NPH = i.NPH,
                //        NPN = i.NPN,
                //        PayBill = i.PayBill,
                //        PaymentDueDate = i.PaymentDueDate,
                //        PaymentMethod = i.PaymentMethod,
                //        PriceTot = i.PriceTot,
                //        SuppCode = i.SuppCode,
                //        SuppName = i.SuppName,
                //        VatDate = i.VatDate,
                //        Nomor = data1.ExpenditureNoteNo,
                //        Tgl = data1.ExpenditureDate,
                //        Jumlah = (decimal)data1.AmountDetail
                //    });
                //}

            };



                var datastatus = status == "BB" ? paymentStatus.Where(x => Convert.ToDouble(x.Jumlah) - x.PriceTot < 0) : status == "SB" ? paymentStatus.Where(x => Convert.ToDouble(x.Jumlah) - x.PriceTot >= 0) : paymentStatus;
            //return paymentStatus.Distinct().AsQueryable();

            return datastatus.AsQueryable();
           
        }
        public Tuple<List<GarmentInternNotePaymentStatusViewModel>, int> GetReport(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int page, int size, string Order, int offset)
        {
            var Query = GetQuery(inno, invono, dono, billno, paymentbill, npn, nph, corrno, supplier, dateNIFrom, dateNITo, dueDateFrom, dueDateTo, status, offset);
            Query.OrderBy(x => x.BillNo).ThenBy(x => x.CorDate).ThenBy(x => x.CorrNo).ThenBy(x => x.CorrType).ThenBy(x => x.CurrCode).ThenBy(x => x.DoDate).ThenBy(x => x.DoNo).ThenBy(x => x.IncomeTaxDate).ThenBy(x => x.INDate).ThenBy(x => x.INNo).ThenBy(x => x.InvoDate).ThenBy(x => x.InvoiceNo).ThenBy(x => x.NPH).ThenBy(x => x.NPN).ThenBy(x => x.PayBill).ThenBy(x => x.PaymentDueDate).ThenBy(x => x.PaymentMethod).ThenBy(x => x.PriceTot).ThenBy(x => x.SuppCode).ThenBy(x => x.SuppName).ThenBy(x => x.VatDate);
            //Query.OrderBy(x =>x.InvoiceNo).ThenBy(x => x.BillNo).ThenBy(x => x.CorDate).ThenBy(x => x.CorrNo).ThenBy(x => x.CorrType).ThenBy(x => x.CurrCode).ThenBy(x => x.DoDate).ThenBy(x => x.DoNo).ThenBy(x => x.IncomeTaxDate).ThenBy(x => x.INDate).ThenBy(x => x.INNo).ThenBy(x => x.InvoDate).ThenBy(x => x.NPH).ThenBy(x => x.NPN).ThenBy(x => x.PayBill).ThenBy(x => x.PaymentDueDate).ThenBy(x => x.PaymentMethod).ThenBy(x => x.PriceTot).ThenBy(x => x.SuppCode).ThenBy(x => x.SuppName).ThenBy(x => x.VatDate);

            //Console.WriteLine(Query);
            var b = Query.ToArray();
            var index = 0;
            foreach (GarmentInternNotePaymentStatusViewModel a in Query)
            {
                GarmentInternNotePaymentStatusViewModel dup = Array.Find(b, o => o.BillNo == a.BillNo && o.CorDate == a.CorDate && o.CorrNo == a.CorrNo && o.CorrType == a.CorrType && o.CurrCode == a.CurrCode && o.DoDate == a.DoDate && o.DoNo == a.DoNo && o.IncomeTaxDate == a.IncomeTaxDate && o.INDate == a.INDate && o.INNo == a.INNo && o.InvoDate == a.InvoDate && o.InvoiceNo == a.InvoiceNo && o.NPH == a.NPH && o.NPN == a.NPN && o.PayBill == a.PayBill && o.PaymentDueDate == a.PaymentDueDate && o.PaymentMethod == a.PaymentMethod && o.PriceTot == a.PriceTot && o.SuppCode == a.SuppCode && o.SuppName == a.SuppName && o.VatDate == a.VatDate);
                if (dup != null)
                {
                    if (dup.count == 0)
                    {
                        index++;
                        dup.count = index;
                    }
                }
                a.count = dup.count;
            }
            Pageable<GarmentInternNotePaymentStatusViewModel> pageable = new Pageable<GarmentInternNotePaymentStatusViewModel>(Query, page - 1, size);
            List<GarmentInternNotePaymentStatusViewModel> Data = pageable.Data.ToList<GarmentInternNotePaymentStatusViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GetXLs(string inno, string invono, string dono, string billno, string paymentbill, string npn, string nph, string corrno, string supplier, DateTime? dateNIFrom, DateTime? dateNITo, DateTime? dueDateFrom, DateTime? dueDateTo, string status, int offset)
        {
            var Data = GetQuery(inno, invono, dono, billno, paymentbill, npn, nph, corrno, supplier, dateNIFrom, dateNITo, dueDateFrom, dueDateTo, status, offset);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Suppler", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Term Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Jatuh Tempo", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nominal SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BP Besar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BP Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Pajak PPN", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Pajak PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Pajak PPH", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Kasbon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Kasbon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Bayar", DataType = typeof(Decimal) });


            if (Data.ToArray().Count() == 0)
                // result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, 0, 0, ""); // to allow column name to be generated properly for empty data as template
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0);
            else
            {
                int index = 0;
                foreach (var item in Data)
                {
                    index++;
                    string dateintern = item.INDate == new DateTime(1970, 1, 1) || item.INDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.INDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string dueDate = item.PaymentDueDate == new DateTime(1970, 1, 1) || item.PaymentDueDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.PaymentDueDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string invodate = item.InvoDate == new DateTime(1970, 1, 1) || item.InvoDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.InvoDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string doDate = item.DoDate == new DateTime(1970, 1, 1) || item.DoDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.DoDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string incometaxDate = item.IncomeTaxDate == new DateTime(1970, 1, 1) || item.IncomeTaxDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.IncomeTaxDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string VatDate = item.VatDate == new DateTime(1970, 1, 1) || item.VatDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.VatDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string corrDate = item.CorDate == new DateTime(1970, 1, 1) || item.CorDate.Value.ToString("dd MMM yyyy") == "01 Jan 0001" ? "-" : item.CorDate.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string kasbon = item.Tgl == new DateTime(1970, 1, 1) || item.Tgl == null? "-" : item.Tgl.Value.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string nonpn = item.NPN == "" ? "-" : item.NPN;
                    string nonph = item.NPH == "" ? "-" : item.NPH;
                    string correction = item.CorrNo == "" ? "-" : item.CorrNo;
                    string typecorrection = item.CorrType == "" ? "-" : item.CorrType;
                    string NoKasbon = item.Nomor == "" ? "-" : item.Nomor;

                    

                    // result.Rows.Add(index, item.supplierCode, item.supplierName, item.no, supplierDoDate, date, item.ePONo, item.productCode, item.productName, item.productRemark, item.dealQuantity, item.dOQuantity, item.remainingQuantity, item.uomUnit);
                    result.Rows.Add(index, item.INNo, dateintern, item.SuppCode, item.SuppName, item.PaymentMethod, item.CurrCode, dueDate, item.InvoiceNo, invodate, item.DoNo, doDate, item.PriceTot, item.BillNo, item.PayBill, nonpn, VatDate, nonph, incometaxDate, correction, corrDate, typecorrection, NoKasbon, kasbon, item.Jumlah);
                }
            }


            ExcelPackage package = new ExcelPackage();
            bool styling = true;
            var Query = Data;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                //sheet.Cells["C1:D1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //sheet.Cells["E1:F1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                Dictionary<string, int> counts = new Dictionary<string, int>();
                Dictionary<string, int> countsType = new Dictionary<string, int>();
                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (counts.TryGetValue(a.InvoiceNo, out value))
                    {
                        counts[a.InvoiceNo]++;
                    }
                    else
                    {
                        counts[a.InvoiceNo] = 1;
                    }

                    //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                    //if (countsType.TryGetValue(a.BCType, out value))
                    //{
                    //    countsType[a.BCType]++;
                    //}
                    //else
                    //{
                    //    countsType[a.BCType] = 1;
                    //}
                }

                int index = 2;
                foreach (KeyValuePair<string, int> b in counts)
                {
                   
                    sheet.Cells["Y" + index + ":Y" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["Y" + index + ":Y" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += b.Value;
                }

                //index = 2;
                //foreach (KeyValuePair<string, int> c in countsType)
                //{
                //    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //    index += c.Value;
                //}
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;

           // return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }


        public DPPVATBankExpenditureNoteViewModel GetInvoice(long InvoiceId)
        {
            string financeUri = "dpp-vat-bank-expenditure-notes/invoice";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Finance}{financeUri}/{InvoiceId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                DPPVATBankExpenditureNoteViewModel viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                     viewModel = null;
                }
                else
                {
                     viewModel = JsonConvert.DeserializeObject<DPPVATBankExpenditureNoteViewModel>(result.GetValueOrDefault("data").ToString());
                }
                return viewModel;
            }
            else
            {
                return null;
            }
        }


        //public DPPVATBankExpenditureNoteViewModel GetInvoice(long InvoiceId = 1)
        //{
        //    //string supplierUri = "master/suppliers";
        //    string financeUri = "dpp-vat-bank-expenditure-notes/invoice";
        //    IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));
        //    if (httpClient != null)
        //    {
        //        var response = httpClient.GetAsync($"{APIEndpoint.Finance}{financeUri}?{InvoiceId}").Result.Content.ReadAsStringAsync();
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
        //        DPPVATBankExpenditureNoteViewModel viewModel = JsonConvert.DeserializeObject<DPPVATBankExpenditureNoteViewModel>(result.GetValueOrDefault("data").ToString());
        //        return viewModel;
        //    }
        //    else
        //    {
        //        DPPVATBankExpenditureNoteViewModel viewModel = null;
        //        return viewModel;
        //    }

        //}

    }
}
