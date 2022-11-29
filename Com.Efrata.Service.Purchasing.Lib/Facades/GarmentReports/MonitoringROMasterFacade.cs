using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class MonitoringROMasterFacade : IMonitoringROMasterFacade
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;
        private readonly PurchasingDbContext dbContext;

        public MonitoringROMasterFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            this.dbContext = dbContext;
        }

        private List<MonitoringROMasterViewModel> GetData(long prId)
        {
            var result = (from pri in dbContext.GarmentPurchaseRequestItems
                          join epoi in dbContext.GarmentExternalPurchaseOrderItems on pri.PO_SerialNumber equals epoi.PO_SerialNumber
                          where pri.GarmentPRId == prId && pri.PO_SerialNumber != null
                          select new MonitoringROMasterViewModel
                          {
                              POMaster = pri.PO_SerialNumber,
                              ProductCode = pri.ProductCode,
                              ProductName = pri.ProductName,
                              Quantity = pri.Quantity,
                              UomUnit = pri.UomUnit,
                              DealQuantity = epoi.DealQuantity,
                              DealUomUnit = epoi.DealUomUnit,
                          }).ToList();

            var poMasters = result.Select(s => s.POMaster).ToList();

            var deliveryOrders = (from gdo in dbContext.GarmentDeliveryOrders
                                  join gdoi in dbContext.GarmentDeliveryOrderItems on gdo.Id equals gdoi.GarmentDOId
                                  join gdod in dbContext.GarmentDeliveryOrderDetails on gdoi.Id equals gdod.GarmentDOItemId
                                  where poMasters.Contains(gdod.POSerialNumber)
                                  select new
                                  {
                                      gdo.DONo,
                                      gdo.SupplierName,
                                      gdod.DOQuantity,
                                      gdod.Id,
                                      gdod.POSerialNumber
                                  }).ToList();

            var doDetailIds = deliveryOrders.Select(s => s.Id).ToList();

            var distributions = (from distItem in dbContext.GarmentPOMasterDistributionItems
                                 join distDetail in dbContext.GarmentPOMasterDistributionDetails on distItem.Id equals distDetail.POMasterDistributionItemId
                                 where doDetailIds.Contains(distItem.DODetailId)
                                 select new
                                 {
                                     distItem.DODetailId,
                                     distDetail.RONo,
                                     distDetail.POSerialNumber,
                                     distDetail.Quantity,
                                     distDetail.Conversion,
                                     distDetail.UomUnit
                                 }).ToList();


            if (result != null && result.Count() > 0)
            {
                Parallel.ForEach(result, r =>
                {
                    r.DeliveryOrders = deliveryOrders.Where(w => w.POSerialNumber == r.POMaster).Select(deliveryOrder => new MonitoringROMasterDeliveryOrderViewModel
                    {
                        DONo = deliveryOrder.DONo,
                        SupplierName = deliveryOrder.SupplierName,
                        DOQuantity = deliveryOrder.DOQuantity,
                        Distributions = distributions.Where(w => w.DODetailId == deliveryOrder.Id).Select(dist => new MonitoringROMasterDistributionViewModel
                        {
                            RONo = dist.RONo,
                            POSerialNumber = dist.POSerialNumber,
                            DistributionQuantity = dist.Quantity * (decimal)dist.Conversion,
                            UomUnit = dist.UomUnit
                        }).ToList()
                    }).ToList();
                });
            }

            return result;
        }

        public List<MonitoringROMasterViewModel> GetMonitoring(long prId)
        {
            var data = GetData(prId);
            return data;
        }

        public Tuple<MemoryStream, string> GetExcel(long prId)
        {
            var RONo = dbContext.GarmentPurchaseRequests
                .Where(w => w.Id == prId)
                .Select(s => s.RONo)
                .Single();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "RO Job", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "PO Job", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pembagian PO", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });

            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            var dataResults = GetData(prId);

            int rowPosition = 3;

            if (dataResults != null && dataResults.Count > 0)
            {
                foreach (var data in dataResults)
                {
                    if (data.DeliveryOrders != null && data.DeliveryOrders.Count > 0)
                    {
                        var firstDataMergedRowPosition = rowPosition;
                        var lastDataMergedRowPosition = rowPosition;
                        foreach (var deliveryOrder in data.DeliveryOrders)
                        {
                            if (deliveryOrder.Distributions != null && deliveryOrder.Distributions.Count > 0)
                            {
                                var firstDeliveryMergedRowPosition = rowPosition;
                                var lastDeliveryMergedRowPosition = rowPosition;
                                foreach (var distribution in deliveryOrder.Distributions)
                                {
                                    dataTable.Rows.Add(data.POMaster, data.ProductCode, data.ProductName, data.Quantity, data.UomUnit, data.DealQuantity, data.DealUomUnit, deliveryOrder.DONo, deliveryOrder.SupplierName, deliveryOrder.DOQuantity, distribution.RONo, distribution.POSerialNumber, distribution.DistributionQuantity, distribution.UomUnit);
                                    lastDataMergedRowPosition = lastDeliveryMergedRowPosition = rowPosition++;
                                }
                                if (firstDeliveryMergedRowPosition != lastDeliveryMergedRowPosition)
                                {
                                    mergeCells.Add(($"H{firstDeliveryMergedRowPosition}:H{lastDeliveryMergedRowPosition}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                                    mergeCells.Add(($"I{firstDeliveryMergedRowPosition}:I{lastDeliveryMergedRowPosition}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                                    mergeCells.Add(($"J{firstDeliveryMergedRowPosition}:J{lastDeliveryMergedRowPosition}", ExcelHorizontalAlignment.Right, ExcelVerticalAlignment.Bottom));
                                }
                            }
                            else
                            {
                                dataTable.Rows.Add(data.POMaster, data.ProductCode, data.ProductName, data.Quantity, data.UomUnit, data.DealQuantity, data.DealUomUnit, deliveryOrder.DONo, deliveryOrder.SupplierName, deliveryOrder.DOQuantity, null, null, null, null);
                                lastDataMergedRowPosition = rowPosition++;
                            }
                        }
                        foreach (var col in new[] { "A", "B", "C", "D", "E", "F", "G" })
                        {
                            if (firstDataMergedRowPosition != lastDataMergedRowPosition)
                            {
                                mergeCells.Add(($"{col}{firstDataMergedRowPosition}:{col}{lastDataMergedRowPosition}", col == "D" || col == "F" ? ExcelHorizontalAlignment.Right : ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                            }
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add(data.POMaster, data.ProductCode, data.ProductName, data.Quantity, data.UomUnit, data.DealQuantity, data.DealUomUnit, null, null, null, null, null, null, null);
                        rowPosition++;
                    }
                }
            }
            else
            {
                dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            ExcelPackage package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(RONo);
            
            var cols = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            var headers = new[] { "PO Master", "Kode Barang", "Nama Barang", "Jumlah Diminta", "Satuan Diminta", "Jumlah Beli", "Satuan Beli", "No Surat Jalan", "Supplier", "Jumlah SJ PO" };

            foreach (var col in cols)
            {
                sheet.Cells[$"{col}1"].Value = headers[Array.IndexOf(cols, col)];
                sheet.Cells[$"{col}1:{col}2"].Merge = true;
            }

            sheet.Cells["K1"].Value = "Pembagian RO Master";
            sheet.Cells["K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["K1:N1"].Merge = true;
            sheet.Cells["A2"].LoadFromDataTable(dataTable, true, OfficeOpenXml.Table.TableStyles.None);

            foreach ((string cells, Enum hAlign, Enum vAlign) in mergeCells)
            {
                sheet.Cells[cells].Merge = true;
                sheet.Cells[cells].Style.HorizontalAlignment = (ExcelHorizontalAlignment)hAlign;
                sheet.Cells[cells].Style.VerticalAlignment = (ExcelVerticalAlignment)vAlign;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            MemoryStream xls = new MemoryStream();
            package.SaveAs(xls);

            return new Tuple<MemoryStream, string>(xls, $"Monitoring RO Master - {RONo}");
        }
    }

    public interface IMonitoringROMasterFacade
    {
        List<MonitoringROMasterViewModel> GetMonitoring(long prId);
        Tuple<MemoryStream, string> GetExcel(long prId);
    }
}
