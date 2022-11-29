using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using System.Net.Http;
using Newtonsoft.Json;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using System.IO;
using System.Data;
using System.Globalization;
using OfficeOpenXml;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class MonitoringFlowProductFacade : IMonitoringFlowProductFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        //private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public MonitoringFlowProductFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            //this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        public Tuple<List<MonitoringFlowProductViewModel>, int> GetFlow(string Dono, string beacukaiNo, string ProductCode)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<MonitoringFlowProductViewModel> Data = Get(Dono, beacukaiNo, ProductCode);
            //Data = Data.OrderByDescending(x => x.DONo).ToList();
            //int TotalData = Data.Count();
            return Tuple.Create(Data, Data.Count());
        }

        public List<MonitoringFlowProductViewModel> Get(string Dono, string beacukaiNo, string ProductCode)
        {
            var Query = (from a in dbContext.GarmentDeliveryOrders
                         join b in dbContext.GarmentDeliveryOrderItems on a.Id equals b.GarmentDOId
                         join c in dbContext.GarmentDeliveryOrderDetails on b.Id equals c.GarmentDOItemId
                         join e in dbContext.GarmentBeacukaiItems on a.Id equals e.GarmentDOId
                         join f in dbContext.GarmentBeacukais on e.BeacukaiId equals f.Id
                         join g in dbContext.GarmentUnitReceiptNoteItems on c.Id equals g.DODetailId into urnitems
                         from urnitem in urnitems.DefaultIfEmpty()
                         join h in dbContext.GarmentUnitReceiptNotes on urnitem.URNId equals h.Id into urns
                         from urn in urns.DefaultIfEmpty()
                         join i in dbContext.GarmentUnitExpenditureNoteItems on urnitem.Id equals i.URNItemId into uenitems
                         from uenitem in uenitems.DefaultIfEmpty()
                         join j in dbContext.GarmentUnitExpenditureNotes on uenitem.UENId equals j.Id into uens
                         from uen in uens.DefaultIfEmpty()
                         where a.DONo == (string.IsNullOrWhiteSpace(Dono) ? a.DONo : Dono)
                         && f.BeacukaiNo == (string.IsNullOrWhiteSpace(beacukaiNo) ? f.BeacukaiNo : beacukaiNo)
                         && c.ProductCode == (string.IsNullOrWhiteSpace(ProductCode) ? c.ProductCode : ProductCode)
                         && a.IsDeleted == false && b.IsDeleted == false && c.IsDeleted == false && e.IsDeleted == false && f.IsDeleted == false
                         select new MonitoringFlowProductViewModel
                         {
                             DONo = a.DONo,
                             DODate = a.DODate.DateTime,
                             ArrivalDate = a.ArrivalDate.DateTime,
                             SupplierName = a.SupplierName,
                             SupplierType = (a.SupplierIsImport == false ? "LOKAL" : "IMPORT"),
                             BCNo = f.BeacukaiNo,
                             BCType = f.CustomsType,
                             BCDate = f.BeacukaiDate.DateTime,
                             DOQty = c.SmallQuantity,
                             Urnno = (urn != null ? urn.URNNo : "urnno-"),
                             ReceiptDate = (urn != null ? urn.CreatedUtc : DateTime.MinValue),
                             ProductCode = c.ProductCode,
                             ProductName = c.ProductRemark,
                             ReceiptQty = (urnitem != null ? (double)urnitem.SmallQuantity : 0),
                             URNType = (urn != null ? urn.URNType : "urntype-"),
                             UENno = (uen != null ? uen.UENNo : "-"),
                             ExpenditureDate = (uen != null ? uen.CreatedUtc : DateTime.MinValue),
                             ExpendQty = (uenitem != null ? uenitem.Quantity : 0),
                             DOUom = c.SmallUomUnit,
                             ExpendUom = (uenitem != null ? uenitem.UomUnit : "-"),
                             PO = c.POSerialNumber,
                             ReceiptUom = (urnitem != null ? urnitem.SmallUomUnit : "reciptuom-"),
                             ExpenditureType = (uen != null ? uen.ExpenditureType : "-")
                         }).Distinct().ToList();

            Query = Query.OrderBy(x => x.DONo).ThenBy(x => x.DODate).ThenBy(x => x.ArrivalDate).ThenBy(x => x.SupplierName).ThenBy(x => x.SupplierType).ThenBy(x => x.BCNo).ThenBy(x => x.BCDate)
                .ThenBy(x => x.BCType).ThenBy(x => x.PO).ThenBy(x => x.ProductCode).ThenBy(x => x.ProductName).ThenBy(x => x.DOQty).ThenBy(x => x.DOQty).ThenBy(x => x.Urnno).ToList();

            List<MonitoringFlowProductViewModel> stock1 = new List<MonitoringFlowProductViewModel>();

            var Querys = Query.ToArray();
            var index = 0;
            foreach (MonitoringFlowProductViewModel a in Querys)
            {
                MonitoringFlowProductViewModel dup = Array.Find(Querys, o => o.DONo == a.DONo && o.DODate == a.DODate && o.ArrivalDate == a.ArrivalDate && o.BCNo == a.BCNo && o.BCType == a.BCType && o.BCDate == a.BCDate && o.SupplierName == a.SupplierName && o.SupplierType == a.SupplierType);
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


            var PrdoctCodes = string.Join(",", Querys.Select(x => x.ProductCode).Distinct().ToList());

            var Codes = GetProductCode(PrdoctCodes);

            foreach(var a in Querys)
            {
                var product = Codes.FirstOrDefault(x => x.Code == a.ProductCode);

                var Composition = product == null ? "-" : product.Composition;
                var Width = product == null ? "-" : product.Width;
                var Const = product == null ? "-" : product.Const;
                var Yarn = product == null ? "-" : product.Yarn;

                stock1.Add(new MonitoringFlowProductViewModel
                {
                    count = a.count,
                    DONo = a.DONo,
                    DODate = a.DODate,
                    ArrivalDate = a.ArrivalDate,
                    SupplierName = a.SupplierName,
                    SupplierType = a.SupplierType,
                    BCNo = a.BCNo,
                    BCType = a.BCType,
                    BCDate = a.BCDate,
                    DOQty = a.DOQty,
                    Urnno = a.Urnno,
                    ReceiptDate = a.ReceiptDate,
                    ProductCode = a.ProductCode,
                    ProductName = product.ProductType == "FABRIC" ? string.Concat(Composition, "", Width, "", Const, "", Yarn) : product.Name,
                    ReceiptQty = a.ReceiptQty,
                    URNType = a.URNType,
                    UENno = a.UENno,
                    ExpenditureDate = a.ExpenditureDate,
                    ExpendQty = a.ExpendQty,
                    DOUom = a.DOUom,
                    ExpendUom = a.ExpendUom,
                    PO = a.PO,
                    ReceiptUom = a.ReceiptUom,
                    ExpenditureType = a.ExpenditureType
                });

            }

            return stock1;
        }

        public MemoryStream GetProductFlowExcel(string Dono, string beacukaiNo, string ProductCode)
        {
            var Query = Get(Dono, beacukaiNo, ProductCode);
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Tiba", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Beacukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah SJ", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan SJ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Masuk", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bon Masuk", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Pengeluaran", DataType = typeof(String) });

            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "",
                    "", 0, "", "", "", 0, "", "", "", 0, "", ""); // to allow column name to be generated properly for empty data as template
                var sheet2 = package.Workbook.Worksheets.Add("Territory");
                sheet2.Cells["A1"].LoadFromDataTable(result, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
            }
            else
            {
                foreach (var item in Query)
                {
                    string ReceiptDate = item.ReceiptDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : item.ReceiptDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ExpenditureDate = item.ExpenditureDate == new DateTimeOffset(new DateTime(1970, 1, 1)) ? "-" : item.ExpenditureDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string Urnno = item.Urnno == "urnno-" ? "-" : item.Urnno;
                    string ReceiptUom = item.ReceiptUom == "reciptuom-" ? "-" : item.ReceiptUom;
                    string URNType = item.URNType == "urntype-" ? "-" : item.URNType;
                    string DODate = item.DODate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string ArrivalDate = item.ArrivalDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string BCDate = item.BCDate.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.count, item.DONo, DODate, ArrivalDate, item.SupplierName,
                        item.SupplierType, item.BCNo, item.BCType,
                        BCDate,
                        item.PO, item.ProductCode,
                        item.ProductName, item.DOQty, item.DOUom, Urnno, ReceiptDate,
                        item.ReceiptQty, ReceiptUom, URNType, item.UENno, item.ExpendQty, item.ExpendUom, item.ExpenditureType);

                }

                foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
                {
                    var sheet = package.Workbook.Worksheets.Add(item.Value);
                    sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);

                    Dictionary<string, int> donospan = new Dictionary<string, int>();
                    Dictionary<string, int> ponospan = new Dictionary<string, int>();
                    Dictionary<string, int> urnspan = new Dictionary<string, int>();

                    int value;
                    foreach (var a in Query)
                    {
                        if (donospan.TryGetValue(a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate, out value))
                        {
                            donospan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate]++;
                        }
                        else
                        {
                            donospan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate] = 1;
                        }

                        if (ponospan.TryGetValue(a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom, out value))
                        {
                            ponospan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom]++;
                        }
                        else
                        {
                            ponospan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom] = 1;
                        }

                        if (urnspan.TryGetValue(a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom + a.Urnno + a.ReceiptDate + a.ReceiptQty + a.URNType, out value))
                        {
                            urnspan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom + a.Urnno + a.ReceiptDate + a.ReceiptQty + a.URNType]++;
                        }
                        else
                        {
                            urnspan[a.DONo + a.DODate + a.ArrivalDate + a.SupplierName + a.SupplierType + a.BCNo + a.BCType + a.BCDate + a.PO + a.ProductCode + a.ProductName + a.DOQty + a.DOUom + a.Urnno + a.ReceiptDate + a.ReceiptQty + a.URNType] = 1;
                        }

                    }

                    int index = 2;
                    foreach (KeyValuePair<string, int> b in donospan)
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
                        sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["H" + index + ":H" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["H" + index + ":H" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["I" + index + ":I" + (index + b.Value - 1)].Merge = true;
                        sheet.Cells["I" + index + ":I" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += b.Value;
                    }

                    index = 2;
                    foreach (KeyValuePair<string, int> c in ponospan)
                    {
                        sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }

                    index = 2;
                    foreach (KeyValuePair<string, int> c in urnspan)
                    {
                        sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Merge = true;
                        sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        index += c.Value;
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


        }

        private List<GarmentProductViewModel> GetProductCode(string codes)
        {
            IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));

            var httpContent = new StringContent(JsonConvert.SerializeObject(codes), Encoding.UTF8, "application/json");

            var garmentProductionUri = APIEndpoint.Core + $"master/garmentProducts/byCode";
            var httpResponse = httpClient.SendAsync(HttpMethod.Get, garmentProductionUri, httpContent).Result;

            List<GarmentProductViewModel> viewModel = new List<GarmentProductViewModel>();

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                viewModel = JsonConvert.DeserializeObject<List<GarmentProductViewModel>>(result.GetValueOrDefault("data").ToString());
              
            }

            return viewModel;

        }
    }
}
