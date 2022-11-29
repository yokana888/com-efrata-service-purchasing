using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentStockOpnameModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentUnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentStockOpnameFacades
{
    public class GarmentStockOpnameFacade : IGarmentStockOpnameFacade
    {
        private string USER_AGENT = "GarmentStockOpnameFacade";

        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentStockOpname> dbSet;
        private readonly DbSet<GarmentDOItems> dbSetDOItem;
        private readonly DbSet<GarmentUnitReceiptNoteItem> dbSetGarmentUnitReceiptNoteItems;

        public GarmentStockOpnameFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentStockOpname>();
            dbSetDOItem = dbContext.Set<GarmentDOItems>();
            dbSetGarmentUnitReceiptNoteItems = dbContext.Set<GarmentUnitReceiptNoteItem>();
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentStockOpname> Query = dbSet;

            if (!string.IsNullOrWhiteSpace(identityService.Username))
            {
                Query = Query.Where(x => x.CreatedBy == identityService.Username);
            }

            List<string> searchAttributes = new List<string>()
            {
                "UnitCode", "UnitName", "StorageName"
            };

            Query = QueryHelper<GarmentStockOpname>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentStockOpname>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentStockOpname>.ConfigureOrder(Query, OrderDictionary);

            Query = Query.Select(m => new GarmentStockOpname
            {
                Id = m.Id,
                LastModifiedUtc = m.LastModifiedUtc,

                Date = m.Date,
                UnitCode = m.UnitCode,
                UnitName = m.UnitName,
                StorageName = m.StorageName
            });

            Pageable<GarmentStockOpname> pageable = new Pageable<GarmentStockOpname>(Query, Page - 1, Size);
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            ListData.AddRange(pageable.Data.Select(s => new
            {
                s.Id,
                s.LastModifiedUtc,

                s.Date,
                s.UnitCode,
                s.UnitName,
                s.StorageName
            }));

            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public GarmentStockOpname ReadById(int id)
        {
            var data = dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return data;
        }

        public Stream Download(DateTimeOffset date, string unit, string storage, string storageName)
        {
            var data = dbSetDOItem.Where(i => i.UnitCode == unit && i.StorageCode == storage && i.ProductName == (storageName == "GUDANG BAHAN BAKU" ? "FABRIC" : i.ProductName))
                .Where(i => i.CreatedUtc <= date.DateTime)
                .Select(i => new
                {
                    i.Id,
                    i.POSerialNumber,
                    i.RO,
                    i.ProductCode,
                    i.ProductName,
                    i.DesignColor,
                    BeforeQuantity = i.RemainingQuantity,
                    Quantity = i.RemainingQuantity
                })
                .ToList();

            if (data.Count > 0)
            {
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
                    table.Rows.Add(d.Id, d.POSerialNumber, d.RO, d.ProductCode, d.ProductName, d.DesignColor, d.BeforeQuantity, d.Quantity);
                }

                var excelPack = new ExcelPackage();
                var ws = excelPack.Workbook.Worksheets.Add("WriteTest");
                ws.Cells["A1"].Value = "Tanggal Stock Opname";
                ws.Cells["A2"].Value = "Unit";
                ws.Cells["A3"].Value = "Nama Gudang";
                ws.Cells["B1"].Value = date.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0));
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
            else
            {
                throw new Exception("Tidak data yang cocok");
            }
        }

        public async Task<GarmentStockOpname> Upload(Stream stream)
        {
            ExcelPackage excelPackage = new ExcelPackage();
            excelPackage.Load(stream);

            var ws = excelPackage.Workbook.Worksheets[0];

            var storage = new string[2] { "", "" };
            if (!string.IsNullOrWhiteSpace((string)ws.Cells["B3"].Value))
            {
                storage = ((string)ws.Cells["B3"].Value).Split("-");
            }

            var data = new GarmentStockOpname
            {
                Date = DateTimeOffset.Parse((string)ws.Cells["B1"].Value),
                UnitCode = (string)ws.Cells["B2"].Value,
                StorageCode = storage[0].Trim(),
                StorageName = storage[1].Trim(),
                Items = new List<GarmentStockOpnameItem>()
            };
            EntityExtension.FlagForCreate(data, identityService.Username, USER_AGENT);

            var doItem = dbSetDOItem.Where(i => i.UnitCode == data.UnitCode && i.StorageCode == data.StorageCode)
                .Select(i => new { i.UnitId, i.UnitName, i.StorageId })
                .FirstOrDefault();

            if (doItem != null)
            {
                data.UnitId = (int)doItem.UnitId;
                data.UnitName = doItem.UnitName;
                data.StorageId = (int)doItem.StorageId;
            }

            for (int row = 6; row <= ws.Dimension.End.Row; row++)
            {
                if (!string.IsNullOrWhiteSpace(ws.Cells[row, 1].Text))
                {
                    GarmentStockOpnameItem item = new GarmentStockOpnameItem
                    {
                        DOItemId = (int)(double)ws.Cells[row, 1].Value,
                        BeforeQuantity = (decimal)(double)ws.Cells[row, 7].Value,
                        Quantity = (decimal)(double)ws.Cells[row, 8].Value
                    };

                    EntityExtension.FlagForCreate(item, identityService.Username, USER_AGENT);
                    data.Items.Add(item);
                }
            }

            ValidateUpload(data);

            foreach (var item in data.Items)
            {
                var DOItem = dbSetDOItem.Where(doi => doi.Id == item.DOItemId).FirstOrDefault();
                if (DOItem != null)
                {
                    item.DOItemNo = DOItem.DOItemNo;
                    item.UId = DOItem.UId;
                    item.UnitId = DOItem.UnitId;
                    item.UnitCode = DOItem.UnitCode;
                    item.UnitName = DOItem.UnitName;
                    item.StorageId = DOItem.StorageId;
                    item.StorageCode = DOItem.StorageCode;
                    item.StorageName = DOItem.StorageName;
                    item.POId = DOItem.POId;
                    item.POItemId = DOItem.POItemId;
                    item.PRItemId = DOItem.PRItemId;
                    item.EPOItemId = DOItem.EPOItemId;
                    item.POSerialNumber = DOItem.POSerialNumber;
                    item.ProductId = DOItem.ProductId;
                    item.ProductCode = DOItem.ProductCode;
                    item.ProductName = DOItem.ProductName;
                    item.DesignColor = DOItem.DesignColor;
                    item.SmallQuantity = DOItem.SmallQuantity;
                    item.SmallUomId = DOItem.SmallUomId;
                    item.SmallUomUnit = DOItem.SmallUomUnit;
                    item.DOCurrencyRate = DOItem.DOCurrencyRate;
                    item.DetailReferenceId = DOItem.DetailReferenceId;
                    item.URNItemId = DOItem.URNItemId;
                    item.RO = DOItem.RO;

                    DOItem.RemainingQuantity = item.Quantity;
                    EntityExtension.FlagForUpdate(DOItem, identityService.Username, USER_AGENT);
                    //if (item.BeforeQuantity != item.Quantity)
                    //{
                    //    DOItem.RemainingQuantity = item.Quantity;
                    //    EntityExtension.FlagForUpdate(DOItem, identityService.Username, USER_AGENT);
                    //}
                }

                var urnItem = dbSetGarmentUnitReceiptNoteItems.FirstOrDefault(urni => urni.Id == item.URNItemId);
                if (urnItem != null)
                {
                    item.Price = Math.Round((item.Quantity * (decimal)item.DOCurrencyRate * (decimal)urnItem.PricePerDealUnit),2);
                }
            }

            dbSet.Add(data);

            await dbContext.SaveChangesAsync();

            return data;
        }

        void ValidateUpload(GarmentStockOpname data)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (data.Date == DateTimeOffset.MinValue)
            {
                validationResults.Add(new ValidationResult("Tanggal harus diisi", new List<string> { "date" }));
            }
            //else if (!string.IsNullOrWhiteSpace(data.UnitCode) && !string.IsNullOrWhiteSpace(data.StorageCode))
            //{
            //    var lastData = GetLastDataByUnitStorage(data.UnitCode, data.StorageCode);
            //    if (lastData != null)
            //    {
            //        if (data.Date <= lastData.Date)
            //        {
            //            validationResults.Add(new ValidationResult("Tanggal harus lebih dari " + lastData.Date.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("id-ID")), new List<string> { "date" }));
            //        }
            //    }
            //}

            if (string.IsNullOrWhiteSpace(data.UnitCode))
            {
                validationResults.Add(new ValidationResult("Unit harus diisi", new List<string> { "unit" }));
            }

            if (string.IsNullOrWhiteSpace(data.StorageCode))
            {
                validationResults.Add(new ValidationResult("Storage harus diisi", new List<string> { "storage" }));
            }

            if (data.Items.Count < 1)
            {
                validationResults.Add(new ValidationResult("Items harus diisi", new List<string> { "itemsCount" }));
            }
            else
            {
                foreach (var item in data.Items)
                {
                    if (item.Quantity < 0)
                    {
                        validationResults.Add(new ValidationResult("Quantity tidak boleh kurang dari 0", new List<string> { "item" }));
                    }
                }
            }

            if (validationResults.Count > 0)
            {
                throw new ServiceValidationExeption(null, validationResults);
            }
        }

        public GarmentStockOpname GetLastDataByUnitStorage(string unit, string storage)
        {
            var data = dbSet.Where(w => w.UnitCode == unit && w.StorageCode == storage)
                .OrderByDescending(o => o.Date)
                .FirstOrDefault();

            return data;
        }
    }
}
