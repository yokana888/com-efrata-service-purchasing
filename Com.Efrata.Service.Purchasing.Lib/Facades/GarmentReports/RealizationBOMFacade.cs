using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentExpenditureGood;
using System.Net.Http;
using Newtonsoft.Json;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentFinishingOut;
using System.IO;
using System.Data;
using OfficeOpenXml;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.BeacukaiAdded;
using System.Globalization;
using OfficeOpenXml.Style;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentCuttingOut;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel.GarmentPreparing;
using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class RealizationBOMFacade : IRealizationBOMFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        //private TraceableBeacukaiFacade;

        public RealizationBOMFacade (IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
        }

        public List<BeacukaiAddedViewModel> GetPEBbyDate(DateTime? dateFrom, DateTime? dateTo)
        {
            //var param = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            string shippingInvoiceUri = APIEndpoint.CustomsReport + $"customs-reports/getPEB/byDate?dateFrom={dateFrom}&dateTo={dateTo}";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var httpResponse = httpClient.GetAsync(shippingInvoiceUri).Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                List<BeacukaiAddedViewModel> viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = new List<BeacukaiAddedViewModel>();
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<List<BeacukaiAddedViewModel>>(result.GetValueOrDefault("data").ToString());

                }
                return viewModel;
            }
            else
            {
                return new List<BeacukaiAddedViewModel>();
            }
        }

        public List<TraceableOutBeacukaiViewModel> GetQuery(string unitcode, DateTime? dateFrom, DateTime? dateTo)
        {
            var PEB = GetPEBbyDate(dateFrom, dateTo);
            string invoices = string.Join(",", PEB.Select(x => x.BonNo));

            var facade = new TraceableBeacukaiFacade(serviceProvider, dbContext);

            var expend = facade.GetRono(invoices);

            var Query = (from a in PEB
                         join b in expend on a.BonNo.Trim() equals b.Invoice.Trim()
                         where b.Unit.Code == (string.IsNullOrWhiteSpace(unitcode) ? b.Unit.Code : unitcode)
                         select new TraceableOutBeacukaiViewModel
                         {
                             BCDate = a.BonDate,
                             BCNo = a.BCNo,
                             BCType = a.BCType,
                             BuyerName = b.Buyer.Name,
                             BuyerCode = b.Buyer.Code,
                             ComodityName = b.Comodity.Name,
                             //ExpenditureDate = b.ExpenditureDate,
                             //ExpenditureGoodId = b.ExpenditureGoodNo,
                             ExpenditureNo = b.Invoice,
                             Qty = b.TotalQuantity,
                             RO = b.RONo,
                             UnitQtyName = "PCS"

                         }).GroupBy(x => new { x.ComodityName, x.BuyerName, x.BuyerCode, x.BCType, x.BCNo, x.BCDate, x.RO, x.UnitQtyName,x.ExpenditureNo }, (key, group) => new TraceableOutBeacukaiViewModel
                         {
                             BCDate = key.BCDate,
                             BCNo = key.BCNo,
                             BCType = key.BCType,
                             BuyerName = key.BuyerName,
                             BuyerCode = key.BuyerCode,
                             ComodityName = key.ComodityName,
                             //ExpenditureDate = key.ExpenditureDate,
                             //ExpenditureGoodId = key.ExpenditureGoodId,
                             ExpenditureNo = key.ExpenditureNo,
                             Qty = group.Sum(x => x.Qty),
                             RO = key.RO,
                             UnitQtyName = key.UnitQtyName

                         }).OrderBy(x => x.RO).ToList();

            var listRo = Query.Select(x => x.RO).Distinct().ToList();

            var rinciandetil = (from a in dbContext.GarmentUnitDeliveryOrderItems
                                join b in dbContext.GarmentUnitDeliveryOrders on a.UnitDOId equals b.Id
                                join c in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals c.UnitDOItemId
                                join d in dbContext.GarmentUnitReceiptNoteItems on a.URNItemId equals d.Id
                                join i in dbContext.GarmentUnitReceiptNotes on d.URNId equals i.Id
                                join e in dbContext.GarmentDeliveryOrderDetails on d.DODetailId equals e.Id
                                join f in dbContext.GarmentDeliveryOrderItems on e.GarmentDOItemId equals f.Id
                                join g in dbContext.GarmentDeliveryOrders on f.GarmentDOId equals g.Id
                                join h in dbContext.GarmentBeacukais on g.CustomsId equals h.Id
                                where listRo.Contains(b.RONo)
                                && i.URNType == "PEMBELIAN"
                                select new TraceableOutBeacukaiDetailViewModel
                                {
                                    BCDate = h.BeacukaiDate.DateTime,
                                    BCNo = h.BeacukaiNo,
                                    BCType = h.CustomsType,
                                    DestinationJob = b.RONo,
                                    ItemCode = d.ProductCode,
                                    ItemName = d.ProductName,
                                    SmallestQuantity = c.Quantity,
                                    UnitQtyName = c.UomUnit,
                                    DONo = g.DONo,
                                    SupplierName = g.SupplierName,

                                }).GroupBy(x => new { x.BCDate, x.BCNo, x.BCType, x.ItemCode, x.DestinationJob, x.ItemName, x.UnitQtyName, x.DONo, x.SupplierName }, (key, group) => new TraceableOutBeacukaiDetailViewModel
                                {
                                    BCDate = key.BCDate,
                                    BCNo = key.BCNo,
                                    BCType = key.BCType,
                                    DestinationJob = key.DestinationJob,
                                    ItemCode = key.ItemCode,
                                    ItemName = key.ItemName,
                                    SmallestQuantity = group.Sum(x => x.SmallestQuantity),
                                    UnitQtyName = key.UnitQtyName,
                                    DONo = key.DONo,
                                    SupplierName = key.SupplierName
                                }).ToList();

            foreach (var i in Query)
            {
                var rinci = rinciandetil.Where(x => x.DestinationJob == i.RO).ToList();

                i.rincian = rinci;

            }
            return Query;
        }

        public MemoryStream GetExcel(string unitcode, string unitname, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = GetQuery(unitcode, dateFrom, dateTo);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "ro", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no bc out", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jenis dokumen", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "qty out", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal bc out", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "barang jadi", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "kode buyer", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "buyer", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "invoice", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "nama barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "kode barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "qty in", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "satuan in", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "asal sj", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "supplier", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no bc masuk", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jenis bc masuk", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal bc masuk", DataType = typeof(string) });


            int counter = 3;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            foreach (var item in query)
            {
                foreach (var detail in item.rincian)
                {

                    idx++;
                    if (!Rowcount.ContainsKey(item.RO))
                    {
                        rCount = 0;
                        var index1 = idx;
                        Rowcount.Add(item.RO, index1.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.RO] = Rowcount[item.RO] + "-" + rCount.ToString();
                        var val = Rowcount[item.RO].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.RO] = val[0] + "-" + rCount.ToString();
                        }
                    }

                    string BCDateOut = item.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(item.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDateIn = detail.BCDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : Convert.ToDateTime(detail.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.RO, item.BCNo, item.BCType, item.Qty, item.UnitQtyName, BCDateOut, item.ComodityName, item.BuyerCode, item.BuyerName,item.ExpenditureNo, detail.ItemName, detail.ItemCode, detail.SmallestQuantity, detail.UnitQtyName, detail.DONo, detail.SupplierName, detail.BCNo, detail.BCType, BCDateIn);
                }
            }

            ExcelPackage package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets.Add("Data");

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var col = (char)('A' + result.Columns.Count);
            string tglawal = new DateTimeOffset(DateFrom).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            string tglakhir = new DateTimeOffset(DateTo).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

            sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN REALISASI BOM (BILL of MATERIAL)");
            sheet.Cells[$"A1:{col}1"].Merge = true;
            sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Value = string.Format("PERIODE TANGGAL {0} - {1}", tglawal, tglakhir);
            sheet.Cells[$"A2:{col}2"].Merge = true;
            sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
            sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells[$"A3:{col}3"].Value = string.Format("UNIT KONFEKSI : {0}", unitname);
            sheet.Cells[$"A3:{col}3"].Merge = true;
            sheet.Cells[$"A3:{col}3"].Style.Font.Bold = true;
            sheet.Cells[$"A3:{col}3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[$"A3:{col}3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);

            var header = new string[] { "RO", "NO DOKUMEN BC KELUAR", "JENIS DOKUMEN", "QTY KIRIM", "SATUAN", "TANGGAL DOKUMEN BC KELUAR", "BARANG JADI", "KODE BUYER", "BUYER","INVOICE NO", "NAMA BARANG", "KODE BARANG", "QTY","SATUAN","ASAL SJ","SUPPLIER","NO BC MASUK","JENIS BC MASUK","TANGGAL BC MASUK" };

            #region style Title and Header
            foreach (var i in Enumerable.Range(0, 19))
            {
                var cols = (char)('A' + i);
                sheet.Cells[$"{cols}5"].Value = header[i];
                sheet.Cells[$"{cols}5:{cols}6"].Merge = true;
            }

            sheet.Cells["A1:S6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:S6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A1:S6"].Style.Font.Bold = true;
            sheet.Cells["A5:S6"].Style.WrapText = true;
            sheet.Cells.AutoFitColumns();

            sheet.Column(2).Width = 15;
            sheet.Column(3).Width = 10;
            sheet.Column(6).Width = 20;
            sheet.Column(15).Width = 37;
            sheet.Column(16).Width = 35;


            #endregion

            #region Merge,Colaps And Expand Cell
            var row4 = Rowcount.ToArray();
            int rowcount4 = row4.Count();

            for (int i = 0; i < rowcount4; i++)
            {
                if (i < 1)
                {
                    var row1 = row4[0];
                    var UnitrowNum = row1.Value.Split("-");
                    int rowNum2 = 1;
                    int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                    if (UnitrowNum.Length > 1)
                    {
                        rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                    }
                    else
                    {
                        rowNum2 = Convert.ToInt32(rowNum1);
                    }

                    sheet.InsertRow((rowNum2  + 6), 1);

                    //Uncomand if add Summary Below data
                    //sheet.Cells[$"E{rowNum2 + countdata + 10}"].Formula = "SUM("+ sheet.Cells[$"E{(rowNum1 + countdata + 9)}:E{(rowNum2 + countdata) + 9}"].Address +")";
                    //sheet.Calculate();

                    sheet.Cells[$"A{(rowNum1  + 5)}:A{(rowNum2 ) + 6}"].Merge = true;
                    sheet.Cells[$"A{(rowNum1  + 5)}:A{(rowNum2 ) + 6}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A{(rowNum1  + 5)}:A{(rowNum2 ) + 6}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    #region merge sub coloum with random row
                    Dictionary<string, string> Rowcount2 = new Dictionary<string, string>();
         
                    List<string> ListRow = new List<string>();

                    for (int a = (rowNum1 + 5); a <= (rowNum2) + 5; a++ )
                    {
                        var cell3 = sheet.Cells[a, 10].Text;
                        ListRow.Add(cell3);
                    }

                    int ids = 1;
                    var rCounts = 0;

                    foreach (var ss in ListRow)
                    {

                        ids++;
                        if (!Rowcount2.ContainsKey(ss))
                        {
                            rCounts = 0;
                            var indexs = ids;
                            Rowcount2.Add(ss, indexs.ToString());
                        }
                        else
                        {
                            rCounts += 1;
                            Rowcount2[ss] = Rowcount2[ss] + "-" + rCounts.ToString();
                            var val = Rowcount2[ss].Split("-");
                            if ((val).Length > 0)
                            {
                                Rowcount2[ss] = val[0] + "-" + rCounts.ToString();
                            }
                        }
                    }

                    var rowsmerege2 = Rowcount2.ToArray();
                    for (int v = 0; v < Rowcount2.Count(); v++)
                    {
                        var rowsss = rowsmerege2[v];

                        var Unitrow = rowsss.Value.Split("-");
                        int rows2 = 1;
                        int rows1 = Convert.ToInt32(Unitrow[0]) + rowNum1 - 1;
                        if (Unitrow.Length > 1)
                        {
                            rows2 = Convert.ToInt32(rows1) + Convert.ToInt32(Unitrow[1]);
                        }
                        else
                        {
                            rows2 = Convert.ToInt32(rows1);
                        }

                        sheet.Cells[$"B{(rows1 + 4)}:B{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"B{(rows1 + 4)}:B{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"B{(rows1 + 4)}:B{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"C{(rows1 + 4)}:C{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"C{(rows1 + 4)}:C{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"C{(rows1 + 4)}:C{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"D{(rows1 + 4)}:D{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"D{(rows1 + 4)}:D{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"D{(rows1 + 4)}:D{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"E{(rows1 + 4)}:E{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"E{(rows1 + 4)}:E{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"E{(rows1 + 4)}:E{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"F{(rows1 + 4)}:F{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"F{(rows1 + 4)}:F{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"F{(rows1 + 4)}:F{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"G{(rows1 + 4)}:G{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"G{(rows1 + 4)}:G{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"G{(rows1 + 4)}:G{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"H{(rows1 + 4)}:H{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"H{(rows1 + 4)}:H{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"H{(rows1 + 4)}:H{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"I{(rows1 + 4)}:I{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"I{(rows1 + 4)}:I{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"I{(rows1 + 4)}:I{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"J{(rows1 + 4)}:J{(rows2) + 4}"].Merge = true;
                        sheet.Cells[$"J{(rows1 + 4)}:J{(rows2) + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"J{(rows1 + 4)}:J{(rows2) + 4}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        if (v > 0)
                        {
                           
                            sheet.Cells[$"K{(rows1 + 4)}:S{(rows2) + 4}"].Clear();

                        }
                    }

                  
                    #endregion

                    for (int ii = (rowNum1  + 5); ii <= (rowNum2  + 5); ii++)
                    {
                        sheet.Row(ii).OutlineLevel = 1;
                        sheet.Row(ii).Collapsed = true;
                    }
                }
                else
                {
                    var row1 = row4[i];
                    var UnitrowNum = row1.Value.Split("-");
                    int rowNum2 = 0;
                    int rowNum1 = (Convert.ToInt32(UnitrowNum[0]) + 1);
                    if (UnitrowNum.Length > 1)
                    {
                        rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                    }
                    else
                    {
                        rowNum2 = Convert.ToInt32(rowNum1);
                    }

                    sheet.InsertRow((rowNum2 + 6 + (i - 1)), 1);
                    //Uncomand if add Summary Below data
                    //sheet.Cells[$"E{rowNum2 + countdata + 10 + (i - 1)}"].Formula = "SUM(" + sheet.Cells[$"E{(rowNum1 + countdata + 9) + (i - 1)}:E{(rowNum2 + countdata) + 9 + (i - 1)}"].Address + ")";
                    //sheet.Calculate();

                    sheet.Cells[$"A{(rowNum1 + 5) + (i - 1)}:A{(rowNum2) + 6 + (i - 1)}"].Merge = true;
                    sheet.Cells[$"A{(rowNum1 + 5) + (i - 1)}:A{(rowNum2) + 6 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A{(rowNum1 + 5) + (i - 1)}:A{(rowNum2) + 6 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    #region merge sub coloum with random row
                    Dictionary<string, string> Rowcount2 = new Dictionary<string, string>();

                    List<string> ListRow = new List<string>();

                    for (int a = (rowNum1 + 5) + (i - 1); a <= (rowNum2) + 5 + (i - 1); a++)
                    {
                        var cell3 = sheet.Cells[a, 10].Text;
                        ListRow.Add(cell3);
                    }

                    int ids = 1;
                    var rCounts = 0;

                    foreach (var ss in ListRow)
                    {
                        ids++;
                        if (!Rowcount2.ContainsKey(ss))
                        {
                            rCounts = 0;
                            var indexs = ids;
                            Rowcount2.Add(ss, indexs.ToString());
                        }
                        else
                        {
                            rCounts += 1;
                            Rowcount2[ss] = Rowcount2[ss] + "-" + rCounts.ToString();
                            var val = Rowcount2[ss].Split("-");
                            if ((val).Length > 0)
                            {
                                Rowcount2[ss] = val[0] + "-" + rCounts.ToString();
                            }
                        }
                    }

                    var rowsmerege2 = Rowcount2.ToArray();
                    for (int v = 0; v < Rowcount2.Count(); v++)
                    {
                        var rowsss = rowsmerege2[v];

                        var Unitrow = rowsss.Value.Split("-");
                        int rows2 = 1;
                        int rows1 = Convert.ToInt32(Unitrow[0]) + rowNum1 - 1;
                        if (Unitrow.Length > 1)
                        {
                            rows2 = Convert.ToInt32(rows1) + Convert.ToInt32(Unitrow[1]);
                        }
                        else
                        {
                            rows2 = Convert.ToInt32(rows1);
                        }

                        sheet.Cells[$"B{(rows1 + 4) + (i - 1)}:B{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"B{(rows1 + 4) + (i - 1)}:B{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"B{(rows1 + 4) + (i - 1)}:B{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"C{(rows1 + 4) + (i - 1)}:C{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"C{(rows1 + 4) + (i - 1)}:C{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"C{(rows1 + 4) + (i - 1)}:C{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"D{(rows1 + 4) + (i - 1)}:D{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"D{(rows1 + 4) + (i - 1)}:D{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"D{(rows1 + 4) + (i - 1)}:D{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"E{(rows1 + 4) + (i - 1)}:E{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"E{(rows1 + 4) + (i - 1)}:E{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"E{(rows1 + 4) + (i - 1)}:E{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"F{(rows1 + 4) + (i - 1)}:F{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"F{(rows1 + 4) + (i - 1)}:F{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"F{(rows1 + 4) + (i - 1)}:F{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"G{(rows1 + 4) + (i - 1)}:G{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"G{(rows1 + 4) + (i - 1)}:G{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"G{(rows1 + 4) + (i - 1)}:G{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"H{(rows1 + 4) + (i - 1)}:H{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"H{(rows1 + 4) + (i - 1)}:H{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"H{(rows1 + 4) + (i - 1)}:H{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"I{(rows1 + 4) + (i - 1)}:I{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"I{(rows1 + 4) + (i - 1)}:I{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"I{(rows1 + 4) + (i - 1)}:I{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        sheet.Cells[$"J{(rows1 + 4) + (i - 1)}:J{(rows2) + 4 + (i - 1)}"].Merge = true;
                        sheet.Cells[$"J{(rows1 + 4) + (i - 1)}:J{(rows2) + 4 + (i - 1)}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        sheet.Cells[$"J{(rows1 + 4) + (i - 1)}:J{(rows2) + 4 + (i - 1)}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        if (v > 0)
                        {

                            sheet.Cells[$"K{(rows1 + 4) + (i - 1)}:S{(rows2) + 4 + (i - 1)}"].Clear();

                        }

                    }

                    #endregion

                    for (int ii = (rowNum1 + 5 + (i - 1)); ii <= (rowNum2 + 5 + (i - 1)); ii++)
                    {
                        sheet.Row(ii).OutlineLevel = 1;
                        sheet.Row(ii).Collapsed = true;
                    }
                }
            }
            #endregion


            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }
        



    }
}
