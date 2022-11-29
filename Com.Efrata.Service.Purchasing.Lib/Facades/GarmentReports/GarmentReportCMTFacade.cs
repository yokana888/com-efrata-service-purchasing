using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Style;
using System.Globalization;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentReportCMTFacade : IGarmentReportCMTFacade
    {
        private readonly PurchasingDbContext dbContext;
        ILocalDbCashFlowDbContext dbContextLocal;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentUnitExpenditureNote> dbSet;

        public GarmentReportCMTFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext, ILocalDbCashFlowDbContext dbContextLocal)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbContextLocal = dbContextLocal;
            this.dbSet = dbContext.Set<GarmentUnitExpenditureNote>();
        }

        public IQueryable<GarmentReportCMTViewModel> GetQuery( DateTime? dateFrom, DateTime? dateTo, int unit, int offset)
        {

            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            List<GarmentReportCMTViewModel> reportData = new List<GarmentReportCMTViewModel>();
                string cmdexpenditure = "select Invoice,e.ExpenditureGoodId, e.RO,Article, SUM(ed.Qty) as qtyBJ from Production.dbo.ExpenditureGood e join Production.dbo.ExpenditureGoodDetail ed on e.ExpenditureGoodId = ed.ExpenditureGoodId where e.ProcessDate between '" + DateFrom + "' and '" + DateTo + "' and e.UnitCode = '" + unit + "' and e.ExpenditureGoodId like '%GE%'  group by e.RO,Invoice,e.ExpenditureGoodId,e.ProcessDate,e.UnitCode, Article order by e.ProcessDate DESC";
                string cmdexpenditure2 = "select Invoice,e.ExpenditureGoodId, e.RO,Article, SUM(ed.Qty) as qtyBJ from Production.dbo.ExpenditureGood e join Production.dbo.ExpenditureGoodDetail ed on e.ExpenditureGoodId = ed.ExpenditureGoodId where e.ProcessDate between '" + DateFrom + "' and '" + DateTo + "'  and e.ExpenditureGoodId like '%GE%'  group by e.RO,Invoice,e.ExpenditureGoodId,e.ProcessDate,e.UnitCode, Article order by e.ProcessDate DESC";
                var querycmd = unit ==0 ? cmdexpenditure2 : cmdexpenditure;
                //List<SqlParameter> parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("start", dateFrom));
                //parameters.Add(new SqlParameter("end", dateTo));
                //parameters.Add(new SqlParameter("unitcode", unit));
                var data = dbContextLocal.ExecuteReaderOnlyQuery(querycmd);


                while (data.Read())
                {
                   var RONo = data["RO"].ToString();
                    var query =
                        (from a in dbContext.GarmentUnitExpenditureNotes
                         join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                         join c in dbContext.GarmentUnitDeliveryOrders on a.UnitDONo equals c.UnitDONo
                         join i in dbContext.GarmentUnitDeliveryOrderItems on c.Id equals i.UnitDOId
                         join e in dbContext.GarmentUnitReceiptNoteItems on b.URNItemId equals e.Id
                         join d in dbContext.GarmentUnitReceiptNotes on e.URNId equals d.Id

                         join f in dbContext.GarmentDeliveryOrderDetails on i.DODetailId equals f.Id
                         join g in dbContext.GarmentDeliveryOrderItems on f.GarmentDOItemId equals g.Id
                         join h in dbContext.GarmentDeliveryOrders on g.GarmentDOId equals h.Id
                         where 
                         c.RONo == RONo
                         && b.ProductName == "FABRIC"
                         //c.RONo == (string)data["e.RO"]

                         select new {
                             a.UENNo,
                             ProductRemark = b.ProductRemark,
                             b.Quantity,
                             c.RONo,
                             d.URNNo,
                             ProductRemark2 = e.ProductRemark,
                             e.ReceiptQuantity,
                             d.SupplierName,
                             h.BillNo,
                             h.PaymentBill,
                             h.DONo

                         }


                    
                        );

                var querygroup = from a in query
                                 group a by new { a.UENNo, a.RONo, a.URNNo, a.BillNo, a.PaymentBill } into groupdata
                                 select new GarmentReportCMTViewModel
                                 {
                                     UENNo = groupdata.FirstOrDefault().UENNo,
                                     ProductRemark = groupdata.FirstOrDefault().ProductRemark,
                                     Quantity = groupdata.Sum(x => x.Quantity),
                                     RONo = groupdata.FirstOrDefault().RONo,
                                     URNNo = groupdata.FirstOrDefault().URNNo,
                                     ProductRemark2 = groupdata.FirstOrDefault().ProductRemark2,
                                     ReceiptQuantity = groupdata.Sum( x => x.ReceiptQuantity),
                                     SupplierName = groupdata.FirstOrDefault().SupplierName,
                                     BillNo = groupdata.FirstOrDefault().BillNo,
                                     PaymentBill = groupdata.FirstOrDefault().PaymentBill,
                                     DONo= groupdata.FirstOrDefault().DONo 
                                     
                                 };

             //   int index = 0;
                foreach (var i in querygroup) {
                  //  index++;
                    reportData.Add(new GarmentReportCMTViewModel
                    {
                           // Count = index,
                            Invoice = data["Invoice"].ToString(),
                            ExpenditureGoodId = data["ExpenditureGoodId"].ToString(),
                            RO = data["RO"].ToString(),
                            Article = data["Article"].ToString(),
                            UnitQty = (double)data["qtyBJ"],
                            UENNo = i.UENNo,
                            ProductRemark = i.ProductRemark,
                            Quantity = i.Quantity,
                            RONo = i.RONo,
                            URNNo = i.URNNo,
                            ProductRemark2 = i.ProductRemark2,
                            ReceiptQuantity = i.ReceiptQuantity,
                            SupplierName = i.SupplierName,
                            BillNo = i.BillNo,
                            PaymentBill = i.PaymentBill,
                            DONo = i.DONo

                            

                        }

                       );
                    } 
                    

                }
                return reportData.AsQueryable();
            
            
           
        }

        public Tuple<List<GarmentReportCMTViewModel>, int> GetReport( DateTime? dateFrom, DateTime?dateTo, int unit, int page, int size, string Order, int offset)
        {
            var Query = GetQuery( dateFrom, dateTo, unit, offset);


            var b = Query.ToArray();
            var index = 0;


            foreach (GarmentReportCMTViewModel a in Query)
            {
                GarmentReportCMTViewModel dup = Array.Find(b, o => o.Invoice == a.Invoice && o.ExpenditureGoodId == a.ExpenditureGoodId);
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

            Pageable<GarmentReportCMTViewModel> pageable = new Pageable<GarmentReportCMTViewModel>(Query, page - 1, size);
            List<GarmentReportCMTViewModel> Data = pageable.Data.ToList<GarmentReportCMTViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }





        public MemoryStream GenerateExcel( DateTime? dateFrom, DateTime? dateTo, int unitcode, int offset, string unitname)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            var Query = GetQuery(dateFrom, dateTo, unitcode, offset);
            //Query = Query.OrderBy(b => b.Invoice).ThenBy(b => b.ExpenditureGoodId);
            var headers = new string[] { "No", "No Invoice", "No. BON", "RO","Artikel", "Qty BJ"};
            var subheaders = new string[] { "No. BON", "Keterangan", "Qty", "Asal", "No. BON", "Keterangan", "Qty", "Supplier", "No Nota", "No BON Kecil", "Surat Jalan" };
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty BJ", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Pemakaian", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal Pemakaian", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No. BON Peneriamaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan Peneriamaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Qty Peneriamaan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BON Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Surat Jalan", DataType = typeof(String) });

            ExcelPackage package = new ExcelPackage();
            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", 0, "", "", 0, "", "", "", 0, "", "", "", "");
                var sheet = package.Workbook.Worksheets.Add("Data");
                sheet.Cells["A7"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light1);// to allow column name to be generated properly for empty data as template
            }
            else
            {
                var Qr = Query.ToArray();
                var q = Query.ToList();
                var index = 0;
                foreach (GarmentReportCMTViewModel a in q)
                {
                    GarmentReportCMTViewModel dup = Array.Find(Qr, o => o.Invoice == a.Invoice && o.ExpenditureGoodId == a.ExpenditureGoodId);
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
                Query = q.AsQueryable();
                foreach (var item in Query)
                {
                    result.Rows.Add(item.Count, item.Invoice, item.ExpenditureGoodId, item.RO, item.Article, item.UnitQty, item.UENNo, item.ProductRemark,
                                    item.Quantity, item.RONo, item.URNNo, item.ProductRemark2, item.ReceiptQuantity, item.SupplierName, item.BillNo,
                                    item.PaymentBill, item.DONo);

                }



                // bool styling = true;

                foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
                {
                    var sheet = package.Workbook.Worksheets.Add(item.Value);
                    #region KopTable
                    sheet.Cells[$"A1:Q1"].Value = "LAPORAN DATA REALISASI CMT";
                    sheet.Cells[$"A1:Q1"].Merge = true;
                    sheet.Cells[$"A1:Q1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A1:Q1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A1:Q1"].Style.Font.Bold = true;
                    sheet.Cells[$"A2:Q2"].Value = string.Format("Periode Tanggal {0} s/d {1}", DateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID")), DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID")));
                    sheet.Cells[$"A2:Q2"].Merge = true;
                    sheet.Cells[$"A2:Q2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A2:Q2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A2:Q2"].Style.Font.Bold = true;
                    sheet.Cells[$"A3:Q3"].Value = string.Format("Konfeksi {0}", string.IsNullOrWhiteSpace(unitname) ? "ALL" : unitname);
                    sheet.Cells[$"A3:Q3"].Merge = true;
                    sheet.Cells[$"A3:Q3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[$"A3:Q3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[$"A3:Q3"].Style.Font.Bold = true;
                    #endregion


                    sheet.Cells["A8"].LoadFromDataTable(item.Key, false, OfficeOpenXml.Table.TableStyles.Light16);
                    sheet.Cells["G6"].Value = "BON PEMAKAIAN";
                    sheet.Cells["G6:J6"].Merge = true;
                    sheet.Cells["G6:J6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    sheet.Cells["K6"].Value = "BON PENERIMAAN";
                    sheet.Cells["K6:Q6"].Merge = true;
                    sheet.Cells["K6:Q6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    foreach (var i in Enumerable.Range(0, 6))
                    {
                        var col = (char)('A' + i);
                        sheet.Cells[$"{col}6"].Value = headers[i];
                        sheet.Cells[$"{col}6:{col}7"].Merge = true;
                        sheet.Cells[$"{col}6:{col}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    }
                    foreach (var i in Enumerable.Range(0, 11))
                    {
                        var col = (char)('G' + i);
                        sheet.Cells[$"{col}7"].Value = subheaders[i];
                        sheet.Cells[$"{col}7"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                    }
                    sheet.Cells["A6:Q7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells["A6:Q7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells["A6:Q7"].Style.Font.Bold = true;
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
                        if (counts.TryGetValue(a.Invoice + a.ExpenditureGoodId, out value))
                        {
                            counts[a.Invoice + a.ExpenditureGoodId]++;
                        }
                        else
                        {
                            counts[a.Invoice + a.ExpenditureGoodId] = 1;
                        }

                        //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                        if (countsType.TryGetValue(a.Invoice, out value))
                        {
                            countsType[a.Invoice]++;
                        }
                        else
                        {
                            countsType[a.Invoice] = 1;
                        }
                    }

                    index = 8;
                    foreach (KeyValuePair<string, int> b in counts)
                    {
                        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
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

                    index = 8;
                    foreach (KeyValuePair<string, int> c in countsType)
                    {
                        sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["B" + index + ":B" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();


                }
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }






        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            //string Key = "";
            string cmd = "";

            List<UnitLoader> data = new List<UnitLoader>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            //Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            //foreach (var f in FilterDictionary)
            //{
            //    Key = f.Value;

            //}

            cmd = "SELECT Distinct UnitName, UnitCode  FROM DL_Supports.dbo.UNIT_KONVEKSI where UnitName LIKE @key ";

            parameters.Add(new SqlParameter("key", "%" + Keyword + "%"));
            var reader = dbContextLocal.ExecuteReader(cmd, parameters);


            while (reader.Read())
            {
                data.Add(new UnitLoader
                {
                    
                    UnitName = reader["UnitName"].ToString(),
                    Code = reader["UnitCode"].ToString(),

                });



            }

            Pageable<UnitLoader> pageable = new Pageable<UnitLoader>(data, Page - 1, Size);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            List<UnitLoader> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();

            ListData.AddRange(Data.Select(s => new
            {
                s.Code,
                s.UnitName
            }));


            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public class ReadResponse<TModel>
        {
            public List<TModel> Data { get; set; }
            public int TotalData { get; set; }
            public Dictionary<string, string> Order { get; set; }

            public ReadResponse(List<TModel> Data, int TotalData, Dictionary<string, string> Order)
            {
                this.Data = Data;
                this.TotalData = TotalData;
                this.Order = Order;
            }
        }


        public class UnitLoader {
            public string UnitName { get; set; }
            public string Code { get; set; }
        }
    }
}
