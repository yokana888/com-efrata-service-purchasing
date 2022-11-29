using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Moonlay.NetCore.Lib;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class GarmentPurchaseDayBookReportFacade : IGarmentPurchaseDayBookReport
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;
       // private readonly IdentityService identityService;


        public GarmentPurchaseDayBookReportFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }


        public IQueryable<GarmentPurchaseDayBookReportViewModel> GetQuery(string unit, bool supplier, string jnsbyr, string category, int Year, int offset)
        {
            //DateTimeOffset dateFrom = DateFrom == null ? new DateTime(1970,1,1) : (DateTimeOffset)DateFrom;
            //DateTimeOffset dateTo = DateTo == null ? new DateTime(2100, 1, 1) : (DateTimeOffset)DateTo;
          //  DateTimeOffset YEAR = year == null ? new DateTime(1970) : (DateTimeOffset)year;
            var Query1 = (from a in dbContext.GarmentDeliveryOrders
                         join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId// into i from b in i.DefaultIfEmpty()

                          join c in dbContext.GarmentDeliveryOrderDetails on b.Id equals c.GarmentDOItemId //into j from c in j.DefaultIfEmpty()
                          join d in dbContext.GarmentBeacukais on a.CustomsId equals d.Id //into k //from d in k.DefaultIfEmpty()
                          join e in dbContext.GarmentExternalPurchaseOrders on b.EPOId equals e.Id //into l from e in l.DefaultIfEmpty()

                          where
                         a.CustomsId != 0
                         && e.SupplierImport == supplier //(supplier.HasValue ? supplier : e.SupplierImport)
                         && a.PaymentMethod == (string.IsNullOrWhiteSpace(jnsbyr) ? a.PaymentMethod : jnsbyr)
                         && c.UnitId == (string.IsNullOrWhiteSpace(unit) ? c.UnitId : unit)
                         && (string.IsNullOrWhiteSpace(category) ? true : (category == "BAHAN BAKU" ? c.ProductName == "FABRIC" : c.ProductName == category))
                         && d.ArrivalDate.Value.AddHours(offset).Year == Year
                          //&& (d.ArrivalDate.Value.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0))).ToString("yyyy") == Year

                          select new {
                             SupplierName = a.SupplierName,
                             UomUnit = c.UomUnit,
                             c.ReceiptQuantity,
                             c.PriceTotal,
                             a.ArrivalDate
                         } );

            var Query = from a in Query1
                        group a by new { a.SupplierName, a.UomUnit } into data
                        select new GarmentPurchaseDayBookReportViewModel
                        {
                            SupplierName = data.Key.SupplierName,
                            UomUnit = data.Key.UomUnit,
                            Qty_Jan = data.FirstOrDefault().ArrivalDate.Month == 1 ? data.Sum(x=>x.ReceiptQuantity):0,
                            Qty_Feb = data.FirstOrDefault().ArrivalDate.Month == 2 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Mar = data.FirstOrDefault().ArrivalDate.Month == 3 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Apr = data.FirstOrDefault().ArrivalDate.Month == 4 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Mei = data.FirstOrDefault().ArrivalDate.Month == 5 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Jun = data.FirstOrDefault().ArrivalDate.Month == 6 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Jul = data.FirstOrDefault().ArrivalDate.Month == 7 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Aug = data.FirstOrDefault().ArrivalDate.Month == 8 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Sep = data.FirstOrDefault().ArrivalDate.Month == 9 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Oct = data.FirstOrDefault().ArrivalDate.Month == 10 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Nov = data.FirstOrDefault().ArrivalDate.Month == 11 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Qty_Dec = data.FirstOrDefault().ArrivalDate.Month == 12 ? data.Sum(x => x.ReceiptQuantity) : 0,
                            Nominal_Jan = data.FirstOrDefault().ArrivalDate.Month == 1? data.Sum(x => x.PriceTotal):0,
                            Nominal_Feb = data.FirstOrDefault().ArrivalDate.Month == 2 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Mar = data.FirstOrDefault().ArrivalDate.Month == 3 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Apr = data.FirstOrDefault().ArrivalDate.Month == 4 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Mei = data.FirstOrDefault().ArrivalDate.Month == 5 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Jun = data.FirstOrDefault().ArrivalDate.Month == 6 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Jul = data.FirstOrDefault().ArrivalDate.Month == 7 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Aug = data.FirstOrDefault().ArrivalDate.Month == 8 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Sep = data.FirstOrDefault().ArrivalDate.Month == 9 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Oct = data.FirstOrDefault().ArrivalDate.Month == 10 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Nov = data.FirstOrDefault().ArrivalDate.Month == 11 ? data.Sum(x => x.PriceTotal) : 0,
                            Nominal_Dec = data.FirstOrDefault().ArrivalDate.Month == 12 ? data.Sum(x => x.PriceTotal) : 0,

                        };

            return Query.OrderBy(x => x.SupplierName);
        }

        public Tuple<List<GarmentPurchaseDayBookReportViewModel>, int> GetReport(string unit, bool supplier, string jnsbyr, string category, int Year, int offset, string order, int page, int size)
        {
            var Query = GetQuery(unit, supplier,jnsbyr,category,Year, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //	Query = Query.OrderByDescending(b => b.poExtDate);
            //}

            Pageable<GarmentPurchaseDayBookReportViewModel> pageable = new Pageable<GarmentPurchaseDayBookReportViewModel>(Query, page - 1, size = int.MaxValue);
            List<GarmentPurchaseDayBookReportViewModel> Data = pageable.Data.ToList<GarmentPurchaseDayBookReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(string unit, bool supplier, string jnsbyr, string category, int Year, int offset)
        {
            var Query = GetQuery(unit, supplier, jnsbyr,category,Year, offset);
            DataTable result = new DataTable();
            DataTable result2 = new DataTable();
            var headers = new string[] { "No", "Supplier", "Satuan", "Total", "Total1", "Total2", "Total3", "Total4", "Total5", "Total6", "Total7", "Total8", "Total9", "Total10", "Total11", "Total12", "Total13", "Total14", "Total15", "Total16", "Total17", "Total18", "Total19", "Total20", "Total21", "Total22", "Total23" };
            var headers2 = new string[] {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            var subheaders = new string[] {"Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)", "Qty", "Nominal (Rp.)" };

            for (int i = 0; i < headers.Length; i++)
            {
                result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            }

            for (int i = 0; i < headers.Length; i++)
            {
                result2.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            }

            var index = 1;
            double qty_jan = 0;
            double qty_feb = 0;
            double qty_mar = 0;
            double qty_apr = 0;
            double qty_mei = 0;
            double qty_jun = 0;
            double qty_jul = 0;
            double qty_aug = 0;
            double qty_sep = 0;
            double qty_oct = 0;
            double qty_nov = 0;
            double qty_dec = 0;

            double nom_jan = 0;
            double nom_feb = 0;
            double nom_mar = 0;
            double nom_apr = 0;
            double nom_mei = 0;
            double nom_jun = 0;
            double nom_jul = 0;
            double nom_aug = 0;
            double nom_sep = 0;
            double nom_oct = 0;
            double nom_nov = 0;
            double nom_dec = 0;
            foreach (var item in Query)
            {
                result.Rows.Add(index++, item.SupplierName, item.UomUnit, NumberFormat(item.Qty_Jan), NumberFormat(item.Nominal_Jan), NumberFormat(item.Qty_Feb), NumberFormat(item.Nominal_Feb), NumberFormat(item.Qty_Mar),
                    NumberFormat(item.Nominal_Mar), NumberFormat(item.Qty_Apr), NumberFormat(item.Nominal_Apr), NumberFormat(item.Qty_Mei), NumberFormat(item.Nominal_Mei), NumberFormat(item.Qty_Jun), NumberFormat(item.Nominal_Jun),
                    NumberFormat(item.Qty_Jul), NumberFormat(item.Nominal_Jul), NumberFormat(item.Qty_Aug), NumberFormat(item.Nominal_Aug), NumberFormat(item.Qty_Sep), NumberFormat(item.Nominal_Sep), NumberFormat(item.Qty_Oct),
                    NumberFormat(item.Nominal_Oct), NumberFormat(item.Qty_Nov), NumberFormat(item.Nominal_Nov), NumberFormat(item.Qty_Dec), NumberFormat(item.Nominal_Dec));
                qty_jan += item.Qty_Jan;
                qty_feb += item.Qty_Feb;
                qty_mar += item.Qty_Mar;
                qty_apr += item.Qty_Apr;
                qty_mei += item.Qty_Mei;
                qty_jun += item.Qty_Jun;
                qty_jul += item.Qty_Jul;
                qty_aug += item.Qty_Aug;
                qty_sep += item.Qty_Sep;
                qty_oct += item.Qty_Oct;
                qty_nov += item.Qty_Nov;
                qty_dec += item.Qty_Dec;

                nom_jan += item.Nominal_Jan;
                nom_feb += item.Nominal_Feb;
                nom_mar += item.Nominal_Mar;
                nom_apr += item.Nominal_Apr;
                nom_mei += item.Nominal_Mei;
                nom_jun += item.Nominal_Jun;
                nom_jul += item.Nominal_Jul;
                nom_aug += item.Nominal_Aug;
                nom_sep += item.Nominal_Sep;
                nom_oct += item.Nominal_Oct;
                nom_nov += item.Nominal_Nov;
                nom_dec += item.Nominal_Dec;
            }
                result2.Rows.Add( "TOTAL . . . ", "", "", NumberFormat(qty_jan), NumberFormat(nom_jan), NumberFormat(qty_feb), NumberFormat(nom_feb), NumberFormat(qty_mar), NumberFormat(nom_mar),
                                   NumberFormat(qty_apr), NumberFormat(nom_apr), NumberFormat(qty_mei), NumberFormat(nom_mei), NumberFormat(qty_jun), NumberFormat(nom_jun), NumberFormat(qty_jul), 
                                   NumberFormat(nom_jul), NumberFormat(qty_aug), NumberFormat(nom_aug), NumberFormat(qty_sep), NumberFormat(nom_sep), NumberFormat(qty_oct), NumberFormat(nom_oct), 
                                   NumberFormat(qty_nov), NumberFormat(nom_nov), NumberFormat(qty_dec), NumberFormat(nom_dec) );

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            sheet.Cells["A4"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells["D1"].Value = headers[3];
            sheet.Cells["D1:AA1"].Merge = true;
            sheet.Cells["D2"].Value = headers2[0]; //Jan
            sheet.Cells["D2:E2"].Merge = true;
            sheet.Cells["F2"].Value = headers2[1]; //Feb
            sheet.Cells["F2:G2"].Merge = true;
            sheet.Cells["H2"].Value = headers2[2]; //Mar
            sheet.Cells["H2:I2"].Merge = true;
            sheet.Cells["J2"].Value = headers2[3]; //Apr
            sheet.Cells["J2:K2"].Merge = true;
            sheet.Cells["L2"].Value = headers2[4]; //Mei
            sheet.Cells["L2:M2"].Merge = true;
            sheet.Cells["N2"].Value = headers2[5]; //Jun
            sheet.Cells["N2:O2"].Merge = true;
            sheet.Cells["P2"].Value = headers2[6]; //Jul
            sheet.Cells["P2:Q2"].Merge = true;
            sheet.Cells["R2"].Value = headers2[7]; // Aug
            sheet.Cells["R2:S2"].Merge = true;
            sheet.Cells["T2"].Value = headers2[8]; // Sep
            sheet.Cells["T2:U2"].Merge = true;
            sheet.Cells["V2"].Value = headers2[9]; // Oct
            sheet.Cells["V2:W2"].Merge = true;
            sheet.Cells["X2"].Value = headers2[10]; // Nov
            sheet.Cells["X2:Y2"].Merge = true;
            sheet.Cells["Z2"].Value = headers2[11]; // Dec
            sheet.Cells["Z2:AA2"].Merge = true;


            foreach (var i in Enumerable.Range(0, 3))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}1"].Value = headers[i];
                sheet.Cells[$"{col}1:{col}3"].Merge = true;
            }

            for (var i = 0; i < 23; i++)
            {
                var col = (char)('D' + i);
                sheet.Cells[$"{col}3"].Value = subheaders[i];

            }

            for (var i = 23; i < 24; i++)
            {
                var col = (char)('A' + i - 23);
                sheet.Cells[$"A{col}3"].Value = subheaders[i];

            }

            sheet.Cells["A1:AA3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:AA3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A1:AA3"].Style.Font.Bold = true;

            var widths = new int[] {5, 30, 10, 20,  20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20};
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }


            var countdata = (Query.Count())+4;

            sheet.Cells[$"A{countdata}"].LoadFromDataTable(result2, false, OfficeOpenXml.Table.TableStyles.Light16);
            sheet.Cells[$"A{countdata}:C{countdata}"].Merge = true;
            sheet.Cells[$"A{countdata}:AA{countdata}"].Style.Font.Bold = true;
            sheet.Cells[$"A{countdata}:C{countdata}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }

        String NumberFormat(double numb)
        {

            var number = string.Format("{0:0,0.00}", numb);

            return number;
        }
    }
}
