using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentUnitReceiptNoteDataUtils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentStockOpnameDataUtils
{
    public class GarmentStockOpnameDataUtil
    {
        private readonly GarmentStockOpnameFacade facade;
        public readonly GarmentUnitReceiptNoteDataUtil unitReceiptNoteDataUtil;

        public GarmentStockOpnameDataUtil(GarmentStockOpnameFacade facade, GarmentUnitReceiptNoteDataUtil garmentUnitReceiptNoteDataUtil)
        {
            this.facade = facade;
            this.unitReceiptNoteDataUtil = garmentUnitReceiptNoteDataUtil;
        }

        public async Task<List<GarmentDOItems>> GetNewData()
        {
            var urn = await unitReceiptNoteDataUtil.GetTestData();
            var doItems = urn.Items.Select(i => unitReceiptNoteDataUtil.ReadDOItemsByURNItemId((int)i.Id)).ToList();
            return doItems;
        }

        public async Task<GarmentStockOpname> GetTestData()
        {
            var urn = await unitReceiptNoteDataUtil.GetTestData();
            var doItems = urn.Items.Select(i => unitReceiptNoteDataUtil.ReadDOItemsByURNItemId((int)i.Id)).ToList();
            var firstDOItems = doItems.First();
            var newData = await facade.Upload(GetExcel(doItems, null, firstDOItems.UnitCode, firstDOItems.StorageCode, firstDOItems.StorageName));
            return newData;
        }

        public Stream GetExcel(List<GarmentDOItems> doItems, DateTimeOffset? date = null, string unit = "unit", string storage = "storage", string storageName = "storageName")
        {
            date = date ?? DateTimeOffset.Now;

            var data = doItems.Select(i => new
            {
                i.Id,
                i.POSerialNumber,
                i.RO,
                i.ProductCode,
                i.ProductName,
                i.DesignColor,
                BeforeQuantity = i.RemainingQuantity,
                Quantity = i.RemainingQuantity
            }).ToList();

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn() { ColumnName = "DOItemId", DataType = typeof(int) });
            table.Columns.Add(new DataColumn() { ColumnName = "PONo", DataType = typeof(string) });
            table.Columns.Add(new DataColumn() { ColumnName = "RONo", DataType = typeof(string) });
            table.Columns.Add(new DataColumn() { ColumnName = "Product Code", DataType = typeof(string) });
            table.Columns.Add(new DataColumn() { ColumnName = "Product Name", DataType = typeof(string) });
            table.Columns.Add(new DataColumn() { ColumnName = "Design Color", DataType = typeof(string) });
            table.Columns.Add(new DataColumn() { ColumnName = "Before Quantity", DataType = typeof(decimal) });
            table.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(decimal) });

            foreach (var d in data)
            {
                for (int i = 0; i < 100; i++)
                    table.Rows.Add(d.Id, d.POSerialNumber, d.RO, d.ProductCode, d.ProductName, d.DesignColor, d.BeforeQuantity, d.Quantity);
            }

            var excelPack = new ExcelPackage();
            var ws = excelPack.Workbook.Worksheets.Add("WriteTest");
            ws.Cells["A1"].Value = "Tanggal Stock Opname";
            ws.Cells["A2"].Value = "Unit";
            ws.Cells["A3"].Value = "Nama Gudang";
            ws.Cells["B1"].Value = date;
            ws.Cells["B2"].Value = unit;
            ws.Cells["B3"].Value = $"{storage} - {storageName}";
            ws.Cells["A5"].LoadFromDataTable(table, true);
            ws.Cells["H6:H" + data.Count + 6].Style.Locked = false;
            ws.Protection.IsProtected = true;
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            Stream stream = new MemoryStream();
            excelPack.SaveAs(stream);

            return stream;
        }
    }
}